#:package Markdig@0.41.0
#:package DocumentFormat.OpenXml@3.3.0

// MdToDocx.cs — convert a Markdown file to a Word document.
//
// Requires the .NET 10 SDK (file-based apps). Run with:
//     dotnet run MdToDocx.cs -- input.md [output.docx]
//
// Supports: headings, paragraphs, bold/italic/strikethrough, inline code,
// fenced/indented code blocks, nested bullet & numbered lists, task lists,
// hyperlinks, blockquotes, horizontal rules, and GFM pipe tables.
// Images are rendered as an "[image: alt]" placeholder.

using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Markdig;
using Markdig.Extensions.TaskLists;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using MdTable = Markdig.Extensions.Tables.Table;
using MdTableCell = Markdig.Extensions.Tables.TableCell;
using MdTableRow = Markdig.Extensions.Tables.TableRow;

if (args.Length < 1)
{
    Console.Error.WriteLine("Usage: dotnet run MdToDocx.cs -- <input.md> [output.docx]");
    return 1;
}

var inputPath = args[0];
if (!File.Exists(inputPath))
{
    Console.Error.WriteLine($"Input file not found: {inputPath}");
    return 1;
}

var outputPath = args.Length > 1 ? args[1] : Path.ChangeExtension(inputPath, ".docx");

var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
var mdDocument = Markdown.Parse(File.ReadAllText(inputPath), pipeline);

using (var wordDoc = WordprocessingDocument.Create(outputPath, WordprocessingDocumentType.Document))
{
    new MdToDocxConverter(wordDoc).Convert(mdDocument);
}

Console.WriteLine($"Wrote {Path.GetFullPath(outputPath)}");
return 0;

/// <summary>Formatting state carried down through nested inline elements.</summary>
internal readonly record struct Fmt(bool Bold, bool Italic, bool Strike, bool Link);

internal sealed class MdToDocxConverter
{
    private const int BulletAbstractId = 1;
    private const int DecimalAbstractId = 2;
    private const int BulletNumId = 1;

    // A4 page with 1" margins. For US Letter use 12240 x 15840.
    private const int PageWidthTwips = 11906;
    private const int PageHeightTwips = 16838;
    private const int PageMarginTwips = 1440;
    private const int ContentWidthTwips = PageWidthTwips - 2 * PageMarginTwips;

    private readonly MainDocumentPart _main;
    private readonly Body _body;
    private readonly Numbering _numbering;
    private int _nextNumId = 2; // 1 is reserved for the shared bullet list

    public MdToDocxConverter(WordprocessingDocument doc)
    {
        _main = doc.AddMainDocumentPart();
        _main.Document = new Document(new Body());
        _body = _main.Document.Body!;
        AddHeadingStyles();
        _numbering = AddNumberingDefinitions();
    }

    public void Convert(MarkdownDocument mdDocument)
    {
        foreach (var block in mdDocument)
            foreach (var element in ConvertBlock(block))
                _body.Append(element);

        // Explicit page setup so the content width used by tables is deterministic.
        _body.Append(new SectionProperties(
            new PageSize
            {
                Width = (UInt32Value)(uint)PageWidthTwips,
                Height = (UInt32Value)(uint)PageHeightTwips
            },
            new PageMargin
            {
                Top = PageMarginTwips,
                Right = (UInt32Value)(uint)PageMarginTwips,
                Bottom = PageMarginTwips,
                Left = (UInt32Value)(uint)PageMarginTwips,
                Header = 720U,
                Footer = 720U,
                Gutter = 0U
            }));

        _main.Document.Save();
    }

    // ---------------------------------------------------------------- blocks

    private IEnumerable<OpenXmlElement> ConvertBlock(Block block, int quoteDepth = 0)
    {
        switch (block)
        {
            case HeadingBlock heading:
            {
                var props = new ParagraphProperties(
                    new ParagraphStyleId { Val = $"Heading{Math.Clamp(heading.Level, 1, 6)}" });
                yield return MakeParagraph(props, ConvertInlines(heading.Inline, default));
                break;
            }

            case ParagraphBlock paragraph:
                yield return MakeParagraph(QuoteProps(quoteDepth), ConvertInlines(paragraph.Inline, default));
                break;

            case QuoteBlock quote:
                foreach (var child in quote)
                    foreach (var element in ConvertBlock(child, quoteDepth + 1))
                        yield return element;
                break;

            case ListBlock list:
                foreach (var element in ConvertList(list, level: 0, quoteDepth))
                    yield return element;
                break;

            case CodeBlock code: // also covers FencedCodeBlock
                foreach (var element in ConvertCodeBlock(code))
                    yield return element;
                break;

            case MdTable table:
                yield return ConvertTable(table);
                break;

            case ThematicBreakBlock:
                yield return MakeParagraph(
                    new ParagraphProperties(new ParagraphBorders(
                        new BottomBorder { Val = BorderValues.Single, Size = 6, Color = "999999", Space = 1 })),
                    Array.Empty<OpenXmlElement>());
                break;

            case LinkReferenceDefinitionGroup:
            case HtmlBlock:
                break; // not rendered

            case LeafBlock leaf when leaf.Inline != null:
                yield return MakeParagraph(QuoteProps(quoteDepth), ConvertInlines(leaf.Inline, default));
                break;

            case ContainerBlock container:
                foreach (var child in container)
                    foreach (var element in ConvertBlock(child, quoteDepth))
                        yield return element;
                break;
        }
    }

    private IEnumerable<OpenXmlElement> ConvertList(ListBlock list, int level, int quoteDepth)
    {
        int numberingId = BulletNumId;
        if (list.IsOrdered)
        {
            int start = int.TryParse(list.OrderedStart, out var s) ? s : 1;
            numberingId = NewOrderedNumberingInstance(start);
        }

        foreach (var item in list.OfType<ListItemBlock>())
        {
            bool firstParagraph = true;
            foreach (var child in item)
            {
                switch (child)
                {
                    case ListBlock nested:
                        foreach (var element in ConvertList(nested, level + 1, quoteDepth))
                            yield return element;
                        break;

                    case ParagraphBlock paragraph:
                    {
                        var props = firstParagraph
                            ? new ParagraphProperties(new NumberingProperties(
                                  new NumberingLevelReference { Val = level },
                                  new NumberingId { Val = numberingId }))
                            : new ParagraphProperties(
                                  new Indentation { Left = ((level + 1) * 720).ToString() });
                        yield return MakeParagraph(props, ConvertInlines(paragraph.Inline, default));
                        firstParagraph = false;
                        break;
                    }

                    default:
                        foreach (var element in ConvertBlock(child, quoteDepth))
                            yield return element;
                        firstParagraph = false;
                        break;
                }
            }
        }
    }

    private static IEnumerable<OpenXmlElement> ConvertCodeBlock(CodeBlock code)
    {
        var lines = code.Lines;
        for (int i = 0; i < lines.Count; i++)
        {
            var text = lines.Lines[i].Slice.ToString();
            var props = new ParagraphProperties(
                new Shading { Val = ShadingPatternValues.Clear, Fill = "F2F2F2" },
                new SpacingBetweenLines { After = "0" });
            var run = new Run(
                new RunProperties(
                    new RunFonts { Ascii = "Consolas", HighAnsi = "Consolas" },
                    new FontSize { Val = "20" }),
                new Text(text) { Space = SpaceProcessingModeValues.Preserve });
            yield return MakeParagraph(props, new OpenXmlElement[] { run });
        }
    }

    private DocumentFormat.OpenXml.Wordprocessing.Table ConvertTable(MdTable mdTable)
    {
        var mdRows = mdTable.OfType<MdTableRow>().ToList();

        // Markdig's ColumnDefinitions can contain one more entry than the table
        // actually has, so derive the column count from the real cells instead.
        int columnCount = mdRows
            .Select(r => r.OfType<MdTableCell>().Count())
            .DefaultIfEmpty(1)
            .Max();
        if (columnCount < 1)
            columnCount = 1;

        int[] columnWidths = ComputeColumnWidths(mdRows, columnCount);
        int WidthOf(int columnIndex) => columnWidths[columnIndex];

        var table = new DocumentFormat.OpenXml.Wordprocessing.Table();
        table.Append(new TableProperties(
            new TableWidth { Width = ContentWidthTwips.ToString(), Type = TableWidthUnitValues.Dxa },
            new TableBorders(
                new TopBorder { Val = BorderValues.Single, Size = 4, Color = "999999" },
                new LeftBorder { Val = BorderValues.Single, Size = 4, Color = "999999" },
                new BottomBorder { Val = BorderValues.Single, Size = 4, Color = "999999" },
                new RightBorder { Val = BorderValues.Single, Size = 4, Color = "999999" },
                new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4, Color = "999999" },
                new InsideVerticalBorder { Val = BorderValues.Single, Size = 4, Color = "999999" }),
            new TableLayout { Type = TableLayoutValues.Fixed }));

        var grid = new TableGrid();
        for (int i = 0; i < columnCount; i++)
            grid.Append(new GridColumn { Width = WidthOf(i).ToString() });
        table.Append(grid);

        foreach (var mdRow in mdRows)
        {
            var row = new TableRow();
            int columnIndex = 0;

            foreach (var mdCell in mdRow.OfType<MdTableCell>())
            {
                if (columnIndex >= columnCount)
                    break;

                var cell = new TableCell();
                cell.Append(new TableCellProperties(
                    new TableCellWidth
                    {
                        Width = WidthOf(columnIndex).ToString(),
                        Type = TableWidthUnitValues.Dxa
                    }));

                bool hasContent = false;
                foreach (var child in mdCell)
                {
                    foreach (var element in ConvertBlock(child))
                    {
                        if (mdRow.IsHeader && element is Paragraph p)
                            BoldifyRuns(p);
                        cell.Append(element);
                        hasContent = true;
                    }
                }
                if (!hasContent)
                    cell.Append(new Paragraph()); // cells must contain at least one paragraph

                row.Append(cell);
                columnIndex++;
            }

            // Pad short rows so every row spans the full table width.
            for (; columnIndex < columnCount; columnIndex++)
            {
                row.Append(new TableCell(
                    new TableCellProperties(
                        new TableCellWidth
                        {
                            Width = WidthOf(columnIndex).ToString(),
                            Type = TableWidthUnitValues.Dxa
                        }),
                    new Paragraph()));
            }

            table.Append(row);
        }
        return table;
    }

    /// <summary>
    /// Sizes columns proportionally to their longest cell text, so text-heavy
    /// columns claim most of the page width and stay readable (roughly 10-15
    /// words per line where the page allows), while short label/number columns
    /// shrink to a sensible minimum instead of wasting space.
    /// </summary>
    private static int[] ComputeColumnWidths(List<MdTableRow> mdRows, int columnCount)
    {
        const int MinColumnWidthTwips = 1080; // ~0.75" floor so narrow columns stay usable
        const int MinWeight = 6;              // treat very short cells as ~6 chars wide
        const int MaxWeight = 90;             // cap so one giant cell can't starve the rest

        var weights = new double[columnCount];
        foreach (var mdRow in mdRows)
        {
            int columnIndex = 0;
            foreach (var mdCell in mdRow.OfType<MdTableCell>())
            {
                if (columnIndex >= columnCount)
                    break;
                int length = Math.Clamp(CellTextLength(mdCell), MinWeight, MaxWeight);
                weights[columnIndex] = Math.Max(weights[columnIndex], length);
                columnIndex++;
            }
        }
        for (int i = 0; i < columnCount; i++)
            if (weights[i] <= 0)
                weights[i] = MinWeight;

        // Proportional allocation with a minimum width per column.
        double totalWeight = weights.Sum();
        var widths = new int[columnCount];
        for (int i = 0; i < columnCount; i++)
            widths[i] = Math.Max(MinColumnWidthTwips, (int)(ContentWidthTwips * weights[i] / totalWeight));

        // Rescale so the widths sum exactly to the content width; the rounding
        // remainder goes to the widest (most text-heavy) column.
        double scale = (double)ContentWidthTwips / widths.Sum();
        int assigned = 0;
        for (int i = 0; i < columnCount; i++)
        {
            widths[i] = Math.Max(720, (int)(widths[i] * scale));
            assigned += widths[i];
        }
        int widest = Array.IndexOf(widths, widths.Max());
        widths[widest] += ContentWidthTwips - assigned;
        return widths;
    }

    private static int CellTextLength(MdTableCell mdCell)
    {
        int length = 0;
        foreach (var block in mdCell)
            if (block is LeafBlock leaf && leaf.Inline != null)
                length += GetPlainText(leaf.Inline).Length;
        return length;
    }

    // --------------------------------------------------------------- inlines

    private IEnumerable<OpenXmlElement> ConvertInlines(ContainerInline? container, Fmt fmt)
    {
        if (container == null)
            yield break;

        foreach (var inline in container)
        {
            switch (inline)
            {
                case LiteralInline literal:
                    yield return MakeRun(literal.Content.ToString(), fmt);
                    break;

                case EmphasisInline emphasis:
                {
                    var f = emphasis.DelimiterChar == '~'
                        ? fmt with { Strike = true }
                        : emphasis.DelimiterCount >= 2
                            ? fmt with { Bold = true }
                            : fmt with { Italic = true };
                    foreach (var element in ConvertInlines(emphasis, f))
                        yield return element;
                    break;
                }

                case CodeInline code:
                    yield return MakeCodeRun(code.Content, fmt);
                    break;

                case LinkInline { IsImage: true } image:
                    yield return MakeRun($"[image: {GetPlainText(image)}]", fmt with { Italic = true });
                    break;

                case LinkInline link:
                {
                    var children = ConvertInlines(link, fmt with { Link = true }).ToList();
                    if (children.Count == 0)
                        children.Add(MakeRun(link.Url ?? string.Empty, fmt with { Link = true }));
                    foreach (var element in WrapInHyperlink(link.Url, children))
                        yield return element;
                    break;
                }

                case AutolinkInline autolink:
                    foreach (var element in WrapInHyperlink(
                                 autolink.Url,
                                 new List<OpenXmlElement> { MakeRun(autolink.Url, fmt with { Link = true }) }))
                        yield return element;
                    break;

                case LineBreakInline lineBreak:
                    yield return lineBreak.IsHard ? new Run(new Break()) : MakeRun(" ", fmt);
                    break;

                case HtmlEntityInline entity:
                    yield return MakeRun(entity.Transcoded.ToString(), fmt);
                    break;

                case TaskList task:
                    yield return MakeRun(task.Checked ? "\u2611 " : "\u2610 ", fmt);
                    break;

                case HtmlInline:
                    break; // raw HTML is not rendered

                case ContainerInline nested:
                    foreach (var element in ConvertInlines(nested, fmt))
                        yield return element;
                    break;
            }
        }
    }

    private IEnumerable<OpenXmlElement> WrapInHyperlink(string? url, List<OpenXmlElement> children)
    {
        if (!string.IsNullOrEmpty(url) && Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            var relationship = _main.AddHyperlinkRelationship(uri, isExternal: true);
            var hyperlink = new Hyperlink { History = true, Id = relationship.Id };
            foreach (var child in children)
                hyperlink.Append(child);
            yield return hyperlink;
        }
        else
        {
            // Relative or malformed URL: keep the styled text without a live link.
            foreach (var child in children)
                yield return child;
        }
    }

    // --------------------------------------------------------------- helpers

    private static Paragraph MakeParagraph(ParagraphProperties? props, IEnumerable<OpenXmlElement> children)
    {
        var paragraph = new Paragraph();
        if (props != null)
            paragraph.Append(props);
        foreach (var child in children)
            paragraph.Append(child);
        return paragraph;
    }

    private static Run MakeRun(string text, Fmt fmt)
    {
        var props = new RunProperties();
        if (fmt.Bold) props.Append(new Bold());
        if (fmt.Italic) props.Append(new Italic());
        if (fmt.Strike) props.Append(new Strike());
        if (fmt.Link)
        {
            props.Append(new Color { Val = "0563C1" });
            props.Append(new Underline { Val = UnderlineValues.Single });
        }

        var run = new Run();
        if (props.HasChildren)
            run.Append(props);
        run.Append(new Text(text) { Space = SpaceProcessingModeValues.Preserve });
        return run;
    }

    private static Run MakeCodeRun(string text, Fmt fmt)
    {
        var props = new RunProperties();
        props.Append(new RunFonts { Ascii = "Consolas", HighAnsi = "Consolas" });
        if (fmt.Bold) props.Append(new Bold());
        if (fmt.Italic) props.Append(new Italic());
        if (fmt.Strike) props.Append(new Strike());
        props.Append(new Shading { Val = ShadingPatternValues.Clear, Fill = "F2F2F2" });

        return new Run(props, new Text(text) { Space = SpaceProcessingModeValues.Preserve });
    }

    private static ParagraphProperties? QuoteProps(int quoteDepth) =>
        quoteDepth == 0
            ? null
            : new ParagraphProperties(
                new ParagraphBorders(
                    new LeftBorder { Val = BorderValues.Single, Size = 18, Color = "CCCCCC", Space = 4 }),
                new Indentation { Left = (quoteDepth * 360).ToString() });

    private static void BoldifyRuns(Paragraph paragraph)
    {
        foreach (var run in paragraph.Descendants<Run>())
        {
            var props = run.GetFirstChild<RunProperties>();
            if (props == null)
            {
                props = new RunProperties();
                run.PrependChild(props);
            }
            if (props.GetFirstChild<Bold>() != null)
                continue;

            // Keep schema order: w:rFonts must precede w:b.
            var fonts = props.GetFirstChild<RunFonts>();
            if (fonts != null)
                props.InsertAfter(new Bold(), fonts);
            else
                props.PrependChild(new Bold());
        }
    }

    private static string GetPlainText(ContainerInline container)
    {
        var sb = new StringBuilder();
        foreach (var inline in container)
        {
            if (inline is LiteralInline literal)
                sb.Append(literal.Content);
            else if (inline is ContainerInline nested)
                sb.Append(GetPlainText(nested));
        }
        return sb.ToString();
    }

    // ----------------------------------------------------- styles & numbering

    private void AddHeadingStyles()
    {
        var part = _main.AddNewPart<StyleDefinitionsPart>();
        var styles = new Styles();

        int[] halfPointSizes = { 40, 32, 28, 26, 24, 22 }; // H1=20pt ... H6=11pt
        for (int i = 0; i < halfPointSizes.Length; i++)
        {
            styles.Append(new Style(
                new StyleName { Val = $"heading {i + 1}" },
                new BasedOn { Val = "Normal" },
                new StyleParagraphProperties(
                    new SpacingBetweenLines { Before = "240", After = "120" }),
                new StyleRunProperties(
                    new Bold(),
                    new Color { Val = "1F3864" },
                    new FontSize { Val = halfPointSizes[i].ToString() },
                    new FontSizeComplexScript { Val = halfPointSizes[i].ToString() }))
            {
                Type = StyleValues.Paragraph,
                StyleId = $"Heading{i + 1}"
            });
        }

        part.Styles = styles;
    }

    private Numbering AddNumberingDefinitions()
    {
        var part = _main.AddNewPart<NumberingDefinitionsPart>();
        var numbering = new Numbering();

        string[] bulletChars = { "\u2022", "\u25CB", "\u25AA" }; // • ○ ▪ cycling by depth
        var bulletAbstract = new AbstractNum { AbstractNumberId = BulletAbstractId };
        var decimalAbstract = new AbstractNum { AbstractNumberId = DecimalAbstractId };

        for (int level = 0; level < 9; level++)
        {
            string indentLeft = ((level + 1) * 720).ToString();

            bulletAbstract.Append(new Level(
                new StartNumberingValue { Val = 1 },
                new NumberingFormat { Val = NumberFormatValues.Bullet },
                new LevelText { Val = bulletChars[level % bulletChars.Length] },
                new LevelJustification { Val = LevelJustificationValues.Left },
                new PreviousParagraphProperties(
                    new Indentation { Left = indentLeft, Hanging = "360" }))
            { LevelIndex = level });

            decimalAbstract.Append(new Level(
                new StartNumberingValue { Val = 1 },
                new NumberingFormat { Val = NumberFormatValues.Decimal },
                new LevelText { Val = $"%{level + 1}." },
                new LevelJustification { Val = LevelJustificationValues.Left },
                new PreviousParagraphProperties(
                    new Indentation { Left = indentLeft, Hanging = "360" }))
            { LevelIndex = level });
        }

        numbering.Append(bulletAbstract, decimalAbstract);
        numbering.Append(new NumberingInstance(
            new AbstractNumId { Val = BulletAbstractId })
        { NumberID = BulletNumId });

        part.Numbering = numbering;
        return numbering;
    }

    /// <summary>Each ordered list gets its own numbering instance so numbering restarts per list.</summary>
    private int NewOrderedNumberingInstance(int start)
    {
        int id = _nextNumId++;
        _numbering.Append(new NumberingInstance(
            new AbstractNumId { Val = DecimalAbstractId },
            new LevelOverride(new StartOverrideNumberingValue { Val = start }) { LevelIndex = 0 })
        { NumberID = id });
        return id;
    }
}
