# Introduction

This document provides a justification or contextual description for the requirements expressed in the [Software-System-Specitication]

Each justification that is present in this document provides context and rationale for the requirement. The requiremetns are not repeated in this document, only there unique identifier.

# Requriements

## [SSS-OA-PROJ-PZ9](Software-System-Specification.md#5212-project-management)

By default in Mycelium, an Organization Administrator does not automatically have access to the model content of every project in their organization. Project access is granted explicitly per project — a user must be added to the project team. This is consistent with the principle that the Organization Administrator role is concerned with user management, authentication, and security configuration, not model content.

However, for audit and compliance purposes, an organization may need someone who can read every project. Examples include a compliance officer verifying that no export-controlled data is in the wrong project, a study lead reviewing model quality across all studies in the organization, or an audit during a quality review or certification activity.

This requirement specifies an opt-in policy setting in the organization's audit settings. When enabled, the Organization Administrator gains read-only access to all projects within the organization. The policy is:

- **Opt-in** — not the default behavior. The organization must consciously enable it.
- **Read-only** — implicit read access only, never write or modify.
- **Per-organization** — configured by the Organization Administrator in the org-level audit settings.
- **Scoped to the OA role** — gives the Organization Administrator a "god view" over all model content within their organization.

The requirement separates two concerns that were conflated in some legacy tools: user and security management (always the OA's job) and model content access (normally granted per project, but optionally elevated to org-wide for audit). Without this opt-in mechanism, an organization that needs audit oversight would have to either add the OA to every project team manually or accept that no one has visibility across projects.

This requirement is a complement to the project visibility settings (Private, Organization-visible, Public) controlled by [SSS-CC-SS-LEZ](Software-System-Specification.md#5211-organization-and-user-management), which let Project Administrators opt their projects into broader visibility from the project side. [SSS-OA-PROJ-PZ9](Software-System-Specification.md#5212-project-management) works in the reverse direction by letting the Organization Administrator opt themselves into broader access from the organization side.

## [SSS-PA-REQ-QP0](Software-System-Specification.md#5215-requirements-management)

SysML v2 does not define a dedicated `RequirementSpecification` metaclass. The language provides `RequirementDefinition` (a specialization of `ConstraintDefinition`) and `RequirementUsage`, and every requirement must live inside a `Namespace` — in practice a `Package`. There is no separate container with its own semantics that represents "a specification as a whole".

Consequently, what requirements-engineering tools traditionally call a *specification* is, in SysML v2, simply a `Package` — or a hierarchy of nested `Package`s — whose members are `RequirementDefinition`s and `RequirementUsage`s. The containment is structural (via `OwningMembership`), the identity is the `Package`'s `qualifiedName`, and the organization is already hierarchical because `Package`s can nest arbitrarily deep.

The wording *"in hierarchical specifications"* in `SSS-PA-REQ-QP0` should therefore be read as shorthand for *"in a hierarchy of `Package`s"*. Mycelium does not need to introduce a new first-class concept for a "requirement specification"; it needs to make `Package`s a comfortable home for requirements, and to reuse every capability the `Package` already offers:

- Create a `Package` dedicated to requirements — covered by [SSS-PA-PKG-R8W](Software-System-Specification.md#52131-namespace-and-package-management).
- Nest `Package`s to express section and subsection structure — covered by [SSS-PA-PKG-V2J](Software-System-Specification.md#52131-namespace-and-package-management).
- Attach metadata (version, author, description, license) to the owning `Package` and promote it to a `LibraryPackage` when it is ready for reuse — covered by [SSS-PA-PKG-M3G](Software-System-Specification.md#52131-namespace-and-package-management) and [SSS-PA-PKG-P8D](Software-System-Specification.md#52131-namespace-and-package-management).
- Import individual requirements or whole requirement packages from another `Namespace` or library without duplication — covered by the auto-import flow [SSS-PA-PKG-X1J](Software-System-Specification.md#52131-namespace-and-package-management), [SSS-PA-PKG-X2K](Software-System-Specification.md#52131-namespace-and-package-management), [SSS-PA-PKG-X3L](Software-System-Specification.md#52131-namespace-and-package-management), and [SSS-PA-PKG-X4M](Software-System-Specification.md#52131-namespace-and-package-management).
- Export a `Package` of requirements to a human-readable document — covered by [SSS-PA-IE-B5W](Software-System-Specification.md#52117-import-export-and-migration).

This choice keeps Mycelium aligned with the SysML v2 metamodel rather than inventing a platform-specific `RequirementSpecification` concept that would not round-trip through the Systems Modeling API, and it avoids duplicating capabilities — visibility, imports, ownership, version control, publication via Mycelium Forge — that `Package` already provides. If a future SysML v2 point release or profile introduces a first-class `RequirementSpecification`, the wording of `SSS-PA-REQ-QP0` is general enough to be re-satisfied at that time without changing the intent of the requirement.

## [SSS-PA-VIS-G1A, SSS-PA-VIS-G2B, SSS-PA-VIS-G3C, SSS-PA-VIS-G4D, SSS-PA-VIS-G5E, SSS-PA-VIS-G6F — 3D viewer](Software-System-Specification.md#521912-3d-model-viewer)

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

## [SSS-PA-VIS-C9K](Software-System-Specification.md#52191-graphical-notation-compliance)

### Why custom icons and images matter

The primary audience for a Mycelium diagram is not always a SysML v2 specialist. In a Concurrent Design Facility (CDF) session — Mycelium's primary use case — the room is full of subject-matter experts (thermal, power, propulsion, mechanical, communications) who care about *the system being designed*, not about the modelling language used to express it. The SysML v2 graphical notation, faithful as it is, communicates structure through abstract rectangles, guillemet keywords, and stereotyped lines. To a thermal engineer skimming a diagram during a session, a `«part» battery1` rectangle and a `«part» radio2` rectangle look the same: both are rectangles with text inside. The notation tells them this is a `part`; it does not tell them, at a glance, that one is a battery and the other is a radio.

A small picture closes that gap immediately. A battery icon next to (or in place of) the standard rectangle, a radio icon for the radio, an antenna icon for the antenna — and a non-MBSE participant can read the diagram the way they read a hardware block diagram on a whiteboard. The semantic content is unchanged, the SysML v2 metamodel is untouched; the diagram simply becomes more legible for the people who need to read it.

### Why uploading is required, not a built-in catalogue

Mycelium cannot ship a fixed library of icons that covers every domain its users will model — spacecraft subsystems, terrestrial industrial equipment, robotics, biomedical devices, scientific instruments. Any built-in catalogue is necessarily incomplete and biased toward whatever domain the platform was first built for. Allowing the user to upload or select a custom icon (or full image) per element lets each project team build a visual vocabulary that matches its own domain, without waiting for Mycelium to add support for it.

### Why both Definition and Usage

SysML v2 separates *what something is* (a `Definition`, e.g. `part def Battery`) from *which one we mean* (a `Usage`, e.g. `mainBattery : Battery`). Allowing an icon to be set at either level supports two common authoring patterns:

- **At the Definition** — economical: one upload of a generic battery icon causes every `Usage` of that `Definition` to render with the same picture across every diagram in the project.
- **At the Usage** — specific: a particular `Usage` can override the inherited icon to show, for example, a distinct picture of the *primary* battery versus the *redundant* battery when the design needs to distinguish them visually.

This upload requirement is paired with the rendering requirement [SSS-PA-VIS-J2R](Software-System-Specification.md#52191-graphical-notation-compliance), which actually places the icon or image on the diagram, and with [SSS-PA-VIS-A6F](Software-System-Specification.md#52191-graphical-notation-compliance), which keeps the element's name and type designator visible alongside the custom icon so that legibility for non-experts does not come at the cost of unambiguous identification for SysML v2 readers.