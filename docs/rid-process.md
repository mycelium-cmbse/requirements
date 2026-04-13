# RID Process

This repository uses **Review Item Discrepancies (RIDs)** to handle formal review findings against the SSS, SRS, and supporting requirements documents. The process follows the review principles defined in **ECSS-M-ST-10-01C**, adapted to GitHub Issues and GitHub Projects.

## What a RID is

A RID is a formal record of a discrepancy, question, or proposed change raised by a reviewer against a document under review. Each RID has one **Originator**, one **Actionee** (once assigned), and a **Final disposition** that closes the loop.

- Every RID is a GitHub Issue created from the **Review Item Discrepancy (RID)** issue form.
- The issue number *is* the RID identifier (e.g. `#42` is RID-42).
- Every RID carries the `rid` label automatically.

## Roles

| Role | GitHub mechanism |
|---|---|
| **Originator** | Issue author |
| **Review chair** | Member of the `@starion/review-chairs` team; screens new RIDs |
| **Actionee** | Issue Assignee |
| **Evaluator** | Same person as Originator; re-engages when response is provided |

## Lifecycle (kanban columns)

The RID kanban is a **GitHub Project (v2)** with a single-select **Status** field. The values, in order, are the columns of the board:

1. **Submitted** — RID has just been raised; not yet screened.
2. **Screening** — review chair checks validity, scope, and duplication.
3. **Rejected (screening)** — *terminal*. Not a valid RID (e.g. duplicate, out of scope, misunderstanding).
4. **Assigned** — accepted by the chair; Actionee assigned.
5. **In Response** — Actionee is drafting the response.
6. **Response Provided** — response filled in; waiting for Originator evaluation.
7. **In Discussion** — Originator disagreed; item is discussed at the next review board meeting.
8. **Closed – Accepted** — *terminal*. Agreed resolution applied to the document.
9. **Closed – Rejected** — *terminal*. Originator withdrew, or review board rejected the RID.
10. **Escalated** — *terminal*. Promoted to an action item tracked outside the review.

```
Submitted → Screening → Assigned → In Response → Response Provided → (Agree) → Closed – Accepted
                │                                       │
                │ reject                                 │ disagree
                ▼                                        ▼
       Rejected (screening)                       In Discussion → { Closed – Accepted |
                                                                    Closed – Rejected |
                                                                    Escalated }
```

## GitHub setup

### Labels
- `rid` — applied automatically by the issue form
- `review:SRR`, `review:PDR`, `review:CDR`, `review:QR`, `review:AR`, `review:ORR`
- `doc:SSS`, `doc:SRS`, `doc:roles`
- `sev:critical`, `sev:major`, `sev:minor`, `sev:editorial`

Labels are for **classification and filtering**, not lifecycle. Lifecycle lives in the Project's Status field.

### Project automation
Configure these built-in Project (v2) workflows:

- **Auto-add to project** — when an issue is labeled `rid`, add it with `Status = Submitted`.
- **Item closed** — when the issue is closed *as completed* → `Status = Closed – Accepted`; *as not planned* → `Status = Closed – Rejected`.
- **Auto-archive** — archive items in terminal columns after 30 days.

### Manual transitions
All other column moves are manual. The review chair drags cards through Screening → Assigned; the actionee moves their own card Assigned → In Response → Response Provided; the originator moves Response Provided → In Discussion when they disagree.

## RID content

The issue form captures:

- **Review** (SRR, PDR, CDR, …)
- **Document** under review
- **Location** (section / requirement ID / line range)
- **Severity** (Critical / Major / Minor / Editorial)
- **Category** (Technical / Editorial / Question)
- **Problem description** (required)
- **Proposed change** (required)
- **Response** — filled by the actionee during *In Response*
- **Originator evaluation** — Agree / Disagree, set during *Response Provided*
- **Final disposition** — set at terminal column

## Relationship to ECSS-M-ST-10-01C

ECSS-M-ST-10-01C defines the formal space project review process. This workflow implements the RID concept from that standard, mapped to GitHub primitives:

| ECSS concept | GitHub realization |
|---|---|
| RID form | Issue form (`.github/ISSUE_TEMPLATE/rid.yml`) |
| RID log | Issue list filtered by label `rid` |
| RID status | Project (v2) Status field |
| Review panel meeting | *In Discussion* column / issue comment thread |
| RID closure minutes | Final issue comment + close reason |
