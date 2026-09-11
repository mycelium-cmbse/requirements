# Introduction

This document provides a justification or contextual description for the requirements expressed in the [Software-System-Specification](Software-System-Specification.md).

Each justification that is present in this document provides context and rationale for the requirement. The requirements are not repeated in this document, only their unique identifier.

# Requirements

## [SSS-OA-PROJ-PZ9](Software-System-Specification.md#5213-project-management)

By default in Mycelium, an Organization Administrator does not automatically have access to the model content of every project in their organization. Project access is granted explicitly per project — a user must be added to the project team. This is consistent with the principle that the Organization Administrator role is concerned with user management, authentication, and security configuration, not model content.

However, for audit and compliance purposes, an organization may need someone who can read every project. Examples include a compliance officer verifying that no export-controlled data is in the wrong project, a study lead reviewing model quality across all studies in the organization, or an audit during a quality review or certification activity.

This requirement specifies an opt-in policy setting in the organization's audit settings. When enabled, the Organization Administrator gains read-only access to all projects within the organization. The policy is:

- **Opt-in** — not the default behavior. The organization must consciously enable it.
- **Read-only** — implicit read access only, never write or modify.
- **Per-organization** — configured by the Organization Administrator in the org-level audit settings.
- **Scoped to the OA role** — gives the Organization Administrator a "god view" over all model content within their organization.

The requirement separates two concerns that were conflated in some legacy tools: user and security management (always the OA's job) and model content access (normally granted per project, but optionally elevated to org-wide for audit). Without this opt-in mechanism, an organization that needs audit oversight would have to either add the OA to every project team manually or accept that no one has visibility across projects.

This requirement is a complement to the project visibility settings (Private, Organization-visible, Public) controlled by [SSS-CC-SS-LEZ](Software-System-Specification.md#5211-organization-and-user-management), which let Project Administrators opt their projects into broader visibility from the project side. [SSS-OA-PROJ-PZ9](Software-System-Specification.md#5213-project-management) works in the reverse direction by letting the Organization Administrator opt themselves into broader access from the organization side.

## [SSS-PA-REQ-QP0](Software-System-Specification.md#52110-requirements-modeling)

SysML v2 does not define a dedicated `RequirementSpecification` metaclass. The language provides `RequirementDefinition` (a specialization of `ConstraintDefinition`) and `RequirementUsage`, and every requirement must live inside a `Namespace` — in practice a `Package`. There is no separate container with its own semantics that represents "a specification as a whole".

Consequently, what requirements-engineering tools traditionally call a *specification* is, in SysML v2, simply a `Package` — or a hierarchy of nested `Package`s — whose members are `RequirementDefinition`s and `RequirementUsage`s. The containment is structural (via `OwningMembership`), the identity is the `Package`'s `qualifiedName`, and the organization is already hierarchical because `Package`s can nest arbitrarily deep.

The wording *"in hierarchical specifications"* in `SSS-PA-REQ-QP0` should therefore be read as shorthand for *"in a hierarchy of `Package`s"*. Mycelium does not need to introduce a new first-class concept for a "requirement specification"; it needs to make `Package`s a comfortable home for requirements, and to reuse every capability the `Package` already offers:

- Create a `Package` dedicated to requirements — covered by [SSS-PA-PKG-R8W](Software-System-Specification.md#5219-namespace-and-package-management).
- Nest `Package`s to express section and subsection structure — covered by [SSS-PA-PKG-V2J](Software-System-Specification.md#5219-namespace-and-package-management).
- Attach metadata (version, author, description, license) to the owning `Package` and promote it to a `LibraryPackage` when it is ready for reuse — covered by [SSS-PA-PKG-M3G](Software-System-Specification.md#5219-namespace-and-package-management) and [SSS-PA-PKG-P8D](Software-System-Specification.md#5219-namespace-and-package-management).
- Import individual requirements or whole requirement packages from another `Namespace` or library without duplication — covered by the auto-import flow [SSS-PA-PKG-X1J](Software-System-Specification.md#5219-namespace-and-package-management), [SSS-PA-PKG-X2K](Software-System-Specification.md#5219-namespace-and-package-management), [SSS-PA-PKG-X3L](Software-System-Specification.md#5219-namespace-and-package-management), and [SSS-PA-PKG-X4M](Software-System-Specification.md#5219-namespace-and-package-management).
- Export a `Package` of requirements to a human-readable document — covered by [SSS-PA-IE-B5W](Software-System-Specification.md#52125-import-export-and-migration).

This choice keeps Mycelium aligned with the SysML v2 metamodel rather than inventing a platform-specific `RequirementSpecification` concept that would not round-trip through the Systems Modeling API, and it avoids duplicating capabilities — visibility, imports, ownership, version control, publication via Mycelium Forge — that `Package` already provides. If a future SysML v2 point release or profile introduces a first-class `RequirementSpecification`, the wording of `SSS-PA-REQ-QP0` is general enough to be re-satisfied at that time without changing the intent of the requirement.

## [SSS-PA-VIS-G1A, SSS-PA-VIS-G2B, SSS-PA-VIS-G3C, SSS-PA-VIS-G4D, SSS-PA-VIS-G5E, SSS-PA-VIS-G6F — 3D viewer](Software-System-Specification.md#52121-3d-model-viewer)

### Starting point: the CDP4-COMET-WEB 3D viewer

The 3D viewer capability in Mycelium is not new territory: it is a direct evolution of the 3D viewer already shipped in **CDP4-COMET-WEB**, the web-based front end of CDP4-COMET developed by Starion Group. In CDP4-COMET-WEB, the 3D scene is constructed entirely from the parameters carried by `ElementUsage`s in the iteration — geometric parameters such as centre of mass, orientation, and shape dimensions are read from the parameters of each `ElementUsage`, the viewer assembles the scene from those values, and the rendering updates live when any of the parameters change. Users can navigate the scene, select elements, see them highlighted in the product tree, and apply domain-of-expertise colouring. That capability has been validated over multiple ESA Concurrent Design Facility sessions and is a proven way to give engineers an immediate spatial intuition of the system they are designing without requiring CAD geometry to be authored first.

Mycelium adopts the same design philosophy with one substitution: the underlying data model is SysML v2 instead of ECSS-E-TM-10-25. The role that `ElementUsage` + Parameters played in CDP4-COMET-WEB is played by `PartUsage` + `AttributeUsage` in Mycelium, and the role that the CDP4-COMET parameter library played (a catalogue of standard parameter types) is played by the `Mycelium::Geometry3D` `LibraryPackage` (a catalogue of standard `AttributeDefinition`s).

### Why Attribute Usages are the default rendering source

Early-phase system design in the ESA Concurrent Design Facility tradition — the primary use case Mycelium targets — starts long before any CAD data exists. At the Phase-0 / Phase-A stage the engineer typically knows, for each part of a spacecraft, the approximate mass, centre of mass, overall envelope, and rough orientation. CAD geometry does not arrive until much later. A 3D viewer that requires STEP or glTF geometry as its input is therefore useless in the phase where spatial intuition matters most. A 3D viewer that renders from a handful of engineering parameters is immediately useful, and the same parameters also feed mass budgets, inertia calculations, and layout studies.

By declaring the geometric `AttributeUsage`s (`centerOfGravity`, `orientation`, `basicShape`, `dimensions`) as the **primary** rendering source:

- The 3D viewer becomes available from the very first commit of a project, when only rough parameters exist.
- The same numbers drive the 3D picture, the mass budget, and the inertia matrix — there is one source of truth.
- Concurrent design sessions can update the 3D picture in real time simply by editing `AttributeUsage` values, without touching any CAD tool.
- Ownership enforcement, commit / branch / merge semantics, and the notification pipeline apply to 3D content automatically because the 3D content *is* model content.

### Why STEP and glTF files remain an optional source

Later in the lifecycle — after Phase-B, when the design freezes and CAD files become available — the approximate parametric geometry is no longer the most accurate representation the team has. `SSS-PA-VIS-G3C` therefore permits a user to attach a STEP (ISO 10303) or glTF/GLB file to a `PartUsage` and to request that the 3D viewer render from that file instead. The attached file is positioned and oriented using the `centerOfGravity` and `orientation` Attribute Usages from `SSS-PA-VIS-G2B`, so the placement logic is consistent with the parametric rendering. This keeps a single viewer working across the whole lifecycle, from back-of-the-envelope Phase-0 sketches to CAD-dense Phase-C/D reviews, without forking into separate "early" and "late" viewers.

### Why a dedicated Library Package

Declaring the geometric Attribute Definitions once — in `Mycelium::Geometry3D` — and distributing them via Mycelium Forge rather than copying them into every project has three reasons:

- **Interoperability.** Two projects that both use `Mycelium::Geometry3D` share the same Attribute Definitions; a `PartUsage` authored in project A can be imported into project B and rendered identically, because the rendering logic keys on the AttributeDefinition identity, not on structural shape matching.
- **Reuse of standards.** The library imports quantity kinds, units, and scales from the SysML v2 Quantities and Units standard library (ISO 80000), so Mycelium does not redefine `kg`, `m`, or `rad`. This is the `SSS-PA-VIS-G5E` constraint: the library is allowed to import standard libraries but not to duplicate them.
- **Versioning.** Like any other Library Package, `Mycelium::Geometry3D` is versioned, published, and imported via the normal Forge flow, so the 3D rendering contract between Mycelium Bloom and user projects is an explicit, traceable contract with a visible version history instead of a hidden hard-coded assumption inside Bloom.

### Continuity with CDP4-COMET-WEB

Because the underlying design philosophy is identical, users migrating from CDP4-COMET-WEB should find the Mycelium 3D viewer immediately familiar: the same kinds of values produce the same kind of picture. The only change is that the values now live on SysML v2 `AttributeUsage`s typed by standard Mycelium Attribute Definitions, instead of on ECSS-E-TM-10-25 `Parameter`s typed by a parameter type library. The migration of a CDP4-COMET `Iteration` into Mycelium preserves the values of the geometric parameters and binds them to the corresponding AttributeUsages in `Mycelium::Geometry3D`, so a project that was renderable in CDP4-COMET-WEB remains renderable in Mycelium after migration.

## [SSS-PA-VIS-C9K](Software-System-Specification.md#521191-general-diagramming-and-notation)

### Why custom icons and images matter

The primary audience for a Mycelium diagram is not always a SysML v2 specialist. In a Concurrent Design Facility (CDF) session — Mycelium's primary use case — the room is full of subject-matter experts (thermal, power, propulsion, mechanical, communications) who care about *the system being designed*, not about the modelling language used to express it. The SysML v2 graphical notation, faithful as it is, communicates structure through abstract rectangles, guillemet keywords, and stereotyped lines. To a thermal engineer skimming a diagram during a session, a `«part» battery1` rectangle and a `«part» radio2` rectangle look the same: both are rectangles with text inside. The notation tells them this is a `part`; it does not tell them, at a glance, that one is a battery and the other is a radio.

A small picture closes that gap immediately. A battery icon next to (or in place of) the standard rectangle, a radio icon for the radio, an antenna icon for the antenna — and a non-MBSE participant can read the diagram the way they read a hardware block diagram on a whiteboard. The semantic content is unchanged, the SysML v2 metamodel is untouched; the diagram simply becomes more legible for the people who need to read it.

### Why uploading is required, not a built-in catalogue

Mycelium cannot ship a fixed library of icons that covers every domain its users will model — spacecraft subsystems, terrestrial industrial equipment, robotics, biomedical devices, scientific instruments. Any built-in catalogue is necessarily incomplete and biased toward whatever domain the platform was first built for. Allowing the user to upload or select a custom icon (or full image) per element lets each project team build a visual vocabulary that matches its own domain, without waiting for Mycelium to add support for it.

### Why both Definition and Usage

SysML v2 separates *what something is* (a `Definition`, e.g. `part def Battery`) from *which one we mean* (a `Usage`, e.g. `mainBattery : Battery`). Allowing an icon to be set at either level supports two common authoring patterns:

- **At the Definition** — economical: one upload of a generic battery icon causes every `Usage` of that `Definition` to render with the same picture across every diagram in the project.
- **At the Usage** — specific: a particular `Usage` can override the inherited icon to show, for example, a distinct picture of the *primary* battery versus the *redundant* battery when the design needs to distinguish them visually.

This upload requirement is paired with the rendering requirement [SSS-PA-VIS-J2R](Software-System-Specification.md#521191-general-diagramming-and-notation), which actually places the icon or image on the diagram, and with [SSS-PA-VIS-A6F](Software-System-Specification.md#521191-general-diagramming-and-notation), which keeps the element's name and type designator visible alongside the custom icon so that legibility for non-experts does not come at the cost of unambiguous identification for SysML v2 readers.

## [SSS-PA-VIS-TN1 … SSS-PA-VIS-TN7 — editable textual notation](Software-System-Specification.md#5211911-textual-notation)

### Why the earlier constraint was reversed

An earlier version of §4.3 stated that the web application *shall not* provide SysML v2 textual notation editing or parsing capabilities, and that the notation is generated read-only. That constraint has been removed. The reasoning behind it was sound as a scoping decision — parsing a concrete syntax is a substantial piece of engineering, and a modelling tool can be complete without it — but it does not survive contact with how experienced SysML v2 practitioners actually work. For a user fluent in the notation, typing

```
part def Battery {
    attribute mass : ISQ::MassValue;
    port power : PowerPort;
}
```

is faster than creating a Part Definition in the browser, adding an Attribute Usage, choosing its type from a picker, adding a Port Usage, and choosing its type from another picker. The same holds for bulk authoring, for pasting a fragment out of a specification or an email, and for correcting a structural mistake that would take a dozen clicks to unpick graphically. Read-only notation serves review and sharing; it does not serve authoring, and authoring is where the time goes.

Keeping the notation read-only also creates an asymmetry Mycelium does not otherwise have. Every other representation in the platform round-trips: a diagram edit changes the model and a model edit changes the diagram ([SSS-PA-VIS-K8M](Software-System-Specification.md#521191-general-diagramming-and-notation), [SSS-PA-VIS-H2W](Software-System-Specification.md#521191-general-diagramming-and-notation)); a tabular edit changes the model. The textual notation would have been the only view of the model that is not also a way into it.

### Why the parser is client-side

`SSS-PA-VIS-TN3` places the parser in the browser rather than in Mycelium Fabric. Three reasons:

- **Multi-backend portability.** [SSS-CC-BACK-R5W](Software-System-Specification.md#521133-multi-backend-support-and-polling) requires Mycelium Bloom to work against *any* backend implementing the OMG Systems Modelling API, not only Mycelium Fabric. That API has no textual-notation endpoint. A server-side parser would make textual editing a Fabric-only capability, and [SSS-CC-BACK-CD2](Software-System-Specification.md#521133-multi-backend-support-and-polling) would then have to disable it for every third-party backend. A client-side parser keeps the capability available everywhere.
- **Interaction latency.** Syntax highlighting, error markers, and auto-completion (`SSS-PA-VIS-TN2`, `SSS-PA-VIS-TN4`) are keystroke-frequency operations. A network round-trip per parse is the difference between an editor and a form.
- **One commit path.** Because Bloom parses locally, what it submits is an ordinary set of abstract-syntax changes, indistinguishable from an edit made in a diagram or a table. `SSS-PA-VIS-TN6` therefore inherits the existing persistence modes (§5.2.1.6), server-side well-formedness validation ([SSS-FB-VALID-CNF](Software-System-Specification.md#5224-model-validation-and-commit-rejection)), and Ownership enforcement ([SSS-CC-COLLAB-KOR](Software-System-Specification.md#5223-ownership-enforcement)) without any of them needing to learn about text. There is no second write path into the model, and therefore no second place for the rules to be enforced or forgotten.

### Why Fabric still does not ingest textual notation

[SSS-CC-EXT-EG1](Software-System-Specification.md#53-system-interface-requirements) is deliberately unchanged: Mycelium Fabric emits SysML v2 and KerML textual notation as a one-way rendering of the abstract syntax and does not accept it as input. This is not an oversight left over from the previous constraint. The Systems Modelling API is an abstract-syntax interface; accepting concrete syntax at the server boundary would add a parser, a grammar version, and a class of parse-error responses to an interface specified in terms of element payloads, and would give external clients a second, semantically weaker way to write to the model. Text is a user-interface affordance in Mycelium, and it stays on the user-interface side of the boundary.

### Why the change set is shown before it is applied

`SSS-PA-VIS-TN5` requires the implied creations, updates, and deletions to be displayed before anything is written. Editing text is a blunt instrument: deleting three lines can delete three elements and everything they own, and unlike a diagram, the text gives no visual cue that a subtree went with them. The preview turns an irreversible structural edit into a reviewed one. `SSS-PA-VIS-TN7` then regenerates the notation from the persisted model rather than leaving the user's text in place, so that the editor cannot drift out of agreement with what was actually stored — normalisation, defaulting, and any element the user did not write are all visible immediately.

## [SSS-CC-PREF-R1V, SSS-CC-VIEW-N5P — preference scoping and view configuration](Software-System-Specification.md#52124a-preference-scopes)

### Why three scopes rather than one

A single per-user preference store is the obvious design and the wrong one for a collaborative engineering platform. Three distinct needs exist at once:

- An **organization** running many studies wants a house configuration — which columns a requirements table opens with, which theme, which default filters — applied to every project without configuring each one.
- A **project** has conventions the organization cannot anticipate: a thermal study wants different default columns than a launcher study. The Project Administrator needs to set a starting point for their team.
- An **individual** engineer works on their own screen, at their own resolution, on their own subsystem, and must be able to override both without asking anyone.

Collapsing these into one scope forces a choice between an organization that cannot standardise anything and users who cannot adapt anything. The org → project → user precedence in `SSS-CC-PREF-R1V` resolves it in the only order that makes sense: the more specific the scope, the more it knows about the actual situation, so the more specific value wins.

The reset behaviour in `SSS-CC-PREF-C3D` matters as much as the precedence. Clearing a user override falls back to the *inherited* value, not to a hard-coded product default. This is what makes an organization-level or project-level change actually take effect for users who have not deliberately overridden it — without it, the defaults would only ever apply to accounts that had never touched a setting, and the scoping would be decorative. `SSS-CC-PREF-S2N` exists for the same reason: a user who cannot see that a value came from the project scope cannot tell the difference between a setting they chose and a setting that was chosen for them.

`SSS-CC-PREF-L6B` covers the case where the connected backend is not Mycelium Fabric and offers no preference store. Preferences degrade to browser-local storage rather than disappearing, which keeps [SSS-CC-BACK-R5W](Software-System-Specification.md#521133-multi-backend-support-and-polling) honest: multi-backend support should cost the user features they cannot have, not features that merely need somewhere to be kept.

### Why view configuration is deliberately not model content

`SSS-CC-VIEW-N5P` states that view configuration is persisted outside the SysML v2 model and creates no Commit. This is the one requirement in §5.2.1.24b that constrains rather than enables, and it is there because the alternative is tempting and wrong.

Mycelium already persists one category of presentation state *inside* the model boundary: diagram layout, under [SSS-PA-VIS-P1A](Software-System-Specification.md#52120-diagram-persistence-and-real-time-collaboration) and [SSS-FB-VIS-P3C](Software-System-Specification.md#52120-diagram-persistence-and-real-time-collaboration), which explicitly applies commit, branch, merge, and ownership semantics to node positions and routing. That is correct for diagrams: a diagram is a shared artifact that a team reviews together, and where a box sits is part of what was agreed. It is emphatically not correct for view configuration, which is per-user and per-session. If resizing a column produced a Commit, then:

- every column drag would enter the commit history and the version history graph, burying model changes in presentation noise;
- every column drag would fire a real-time notification to all connected users under [SSS-CC-COLLAB-TLB](Software-System-Specification.md#5225-real-time-notifications);
- commit diffs ([SSS-PA-VC-P89](Software-System-Specification.md#521132-version-control-and-branching)) and merge reviews ([SSS-PT-VC-DV3](Software-System-Specification.md#521128-review-workflow)) would surface changes that mean nothing to a reviewer;
- two users could conflict on a merge over how wide a column is.

The same argument applies to the saved view filters of §5.2.1.24c. A filter is a way of looking at the model, not a statement about the system being designed, and modelling it as a SysML v2 Element would make it subject to ownership enforcement, validation, branching, and merge — all of which are costs with no corresponding benefit. Sharing, the one property that might have justified putting filters in the model, is obtained instead by saving them at project or organization scope through §5.2.1.24a.

This is also the boundary that separates §5.2.1.24c from §5.2.1.22. A *query* is executed against the model server, may target any Commit, returns element sets, and is legitimately a shared project asset. A *filter* decides what an already-loaded view shows. They look similar in the interface and are entirely different in where they run and what they cost.

## [SSS-PA-VC-BE1 … SSS-FB-VC-BE6 — branching enablement](Software-System-Specification.md#521132a-branching-enablement)

### Why a project would turn branching off

Version control with branches is one of Mycelium's differentiators against CDP4-COMET, so a setting that disables it deserves explanation. The answer is that Mycelium's primary use case does not use branches, and the interface pays for them anyway.

A Concurrent Design Facility session is 20 to 30 engineers in one room over a few days, working on one model, in one line of development, with changes published and integrated continuously ([SSS-PT-CDS-RKV](Software-System-Specification.md#52117-concurrent-design)). The whole point of the concurrent design method is that divergence is resolved *in the room, immediately*, not in a parallel line of work merged later. There is no branch to create, and a Participant who creates one has almost certainly misunderstood the workflow. The same holds for a small team modelling a single system: the coordination cost of branching exceeds its value below a certain team size.

Meanwhile, branching is not free in the interface. A branch selector sits in the application header ([SSS-PA-VC-R8W](Software-System-Specification.md#521132-version-control-and-branching)), branch management is a view, merges are a workflow with conflict resolution, and protected branches bring a whole review workflow (§5.2.1.28) with reviewers and approval counts. For a project that will never use any of it, this is a permanent tax on the interface and a permanent source of confusion for exactly the audience Mycelium is trying to reach: the subject-matter expert who is an authority on thermal control and has never used Git.

### Why the scope is what it is

The setting is per project with an organization-level default (`SSS-PA-VC-BE1`, `SSS-OA-VC-BE2`) because the decision is genuinely a per-project one — an organization may run CDF studies and long-running system development in parallel — while an organization that only does one kind of work should not have to configure every project by hand.

`SSS-PA-VC-BE4` is the requirement that keeps this from being a mistake. Disabling branching disables *branching*, not version control. Commits, tags, the history graph, commit diffing, and read-only historical snapshots all remain, because these serve every project regardless of workflow: a CDF study still needs a baseline tag at the end of each session, still needs to see what changed since yesterday, and still needs to open the model as it was on Tuesday. What is removed is only the machinery for maintaining parallel lines of development. A project that starts simple and later needs branches re-enables the setting and finds its full history intact.

`SSS-PA-VC-BE5` handles the transition in the other direction. Disabling branching while non-default branches exist would orphan work: the branches would still exist in Mycelium Fabric with no interface reaching them. Requiring them to be merged or deleted first makes the loss explicit and deliberate rather than silent.

Finally, `SSS-FB-VC-BE6` enforces the setting server-side. Hiding the branch surfaces in Mycelium Bloom is a user-interface convenience, not a control — the Systems Modelling API is a public interface ([SSS-CC-EXT-QIN](Software-System-Specification.md#5221-systems-modelling-api)) and any client can call `createBranch` directly. This follows the same principle as ownership enforcement (§5.2.2.3): Bloom presents the rule, Fabric enforces it.