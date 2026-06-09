# Mycelium Software System Specification (SSS)

This document defines the software system specification for the Mycelium platform in accordance with ECSS-E-ST-40C Rev.1 Annex B. It contains the Product Owner's requirements for the Mycelium software system. Together with the interface requirements, it provides the criteria used to validate and accept the software.

Each requirement uses the form:

> **\<Component\> shall** \<active verb\> **when** "\<condition\>"

Where component is one of: Mycelium Bloom, Mycelium Fabric, or Mycelium Forge.

Every verb must describe something the software actively does: renders a UI, processes a request, persists data, sends a notification, or blocks an operation.

Each requirement has a unique identifier.

The requirements are organized in tables. The tables list the `Requirement Identifier`, the `roles` it applies to, the requirement body or text and a `reference` to the Kerml or SysML2 specification in case this is applicable. If the kerml or syml2 reference is not applicable a `-` is used. The last two columns describe the priority (`low - (L)`, `medium - (M)`, `high - (H)`) and the estimated effort.

---

## 1. Introduction

This Software System Specification (SSS) defines the customer's requirements for the Mycelium platform, a next-generation web-based SysML v2 Model-Based Systems Engineering (MBSE) platform evolving from CDP4-COMET. The Mycelium platform is composed of three components:

- **Mycelium Bloom**, the end-user web application providing the interactive frontend for mycelium administration, model browsing, editing, diagramming, and collaboration.
- **Mycelium Fabric**, the backend server combining authentication and authorization, the Systems Modelling API implementation, ownership enforcement, real-time notifications, model persistence, and concurrent design support.
- **Mycelium Forge**, the package registry for publishing, discovering, and importing SysML v2 model libraries and reusable packages.

Requirements are identified using the convention `SSS-<role>-<area>-<alpha-numeric>` where role prefixes are: OA (Organization Administrator), PA (Project Administrator), PT (Participant), VW (Viewer), CC (Cross-Cutting), FB (Fabric), FG (Forge). The `<alpha-numeric>` component is randomized and does not convey any ordering.

This document is the primary input for the System Requirements Review (SRR).

---

## 2. Applicable and reference documents

### 2.1 Applicable documents

| ID | Document |
|----|----------|
| AD-01 | ECSS-E-ST-40C Rev.1, Space engineering: Software (30 April 2025) |
| AD-02 | OMG SysML v2, Systems Modeling Language, version 2.0 (formal/25-09-03) |
| AD-03 | OMG KerML, Kernel Modeling Language, version 1.0 (formal/25-09-03) |
| AD-04 | OMG Systems Modelling API and Services, version 1.0 (formal/25-09-04) |

### 2.2 Reference documents

| ID | Document |
|----|----------|
| RD-01 | [Roles and Permissions](Roles-and-Permissions.md), Mycelium role and permission model |
| RD-02 | ECSS-E-TM-10-25A, Technical Memorandum: Engineering design model data exchange |
| RD-03 | ReqIF, Requirements Interchange Format (OMG formal/16-07-01) |
| RD-04 | BS08823, CDP4-COMET SysML v2 (R)Evolution Technical Proposal |

---

## 3. Terms, definitions and abbreviated terms

### 3.1 Terms

| Term | Definition |
|------|-----------|
| Ownership | The SysML v2 metadata-based mechanism in Mycelium for element-level access control. Modeled as an `Owner` MetadataDefinition referencing an `Ownership` ItemUsage. See RD-01. |
| Organization | The tenant boundary in the Mycelium platform. On SaaS each customer is an Organization; on-premise deployments may have multiple Organization. |
| Organization Administrator | Owner of an Organization; manages users, authentication and security within the org. |
| Organization Member | Regular user within an Organization; can create projects if permitted. |
| Outside Collaborator | User with access to specific projects without organization membership. |
| Project Administrator | Owner of a Project; manages structure, team, branches, tags and merges. |
| Participant | Subject matter specialist who creates and modifies model elements within their assigned Ownership. |
| Viewer | Read-only observer of the model. |
| Concurrent Design Mode | Project mode with strict ownership enforcement and publication workflow. |
| Regular Mode | Project mode without ownership enforcement or publication workflow. |
| Branch Protection Rules | Configurable rules on branches governing merge access, review requirements, and validation. |
| Project Lifecycle State | The current phase of a project controlling editing permissions: Preparation (PA only), Open (all Participants), Review (read-only), Archived (read-only, immutable historical record). |

### 3.1.1 SysML v2 Definition and Usage types

The following table lists the SysML v2 Definition types (and their corresponding Usage types) that Mycelium supports. Each Definition describes a reusable type; each Usage represents a specific occurrence or application of that type in a model context. Any Definition or Usage can own Attributes.

| Definition | Usage | Description | SSS section |
|-----------|-------|-------------|-------------|
| PackageDefinition | — | Organizational container grouping related model elements | 5.2.1.9 |
| PartDefinition | PartUsage | Structural building block of a system (system, subsystem, equipment, component) | 5.2.1.11 |
| ItemDefinition | ItemUsage | Non-structural element representing data, signals, energy, or resources | 5.2.1.11 |
| AttributeDefinition | AttributeUsage | Data characteristic (quantity, text, boolean) with optional unit and measurement scale | 5.2.1.11, 5.2.1.14 |
| EnumerationDefinition | EnumerationUsage | Fixed set of allowed values restricting an attribute | 5.2.1.15 |
| PortDefinition | PortUsage | Interaction point on a part with directional features (in, out, inout) | 5.2.1.11 |
| ConnectionDefinition | ConnectionUsage | Link between parts or items (physical, logical, or data) | 5.2.1.11 |
| InterfaceDefinition | InterfaceUsage | Standardized connection between ports with compatibility rules | 5.2.1.11 |
| ActionDefinition | ActionUsage | Function or behavior with input/output parameters, decomposable into sub-actions | 5.2.1.16 |
| StateDefinition | StateUsage | Condition or mode with entry, do, and exit actions | 5.2.1.16 |
| TransitionUsage | — | Transition between states with trigger, guard, and effect | 5.2.1.16 |
| FlowConnectionDefinition | FlowConnectionUsage | Transfer of items, energy, or data between parts | 5.2.1.16 |
| UseCaseDefinition | UseCaseUsage | System behavior from an external actor perspective | 5.2.1.10 |
| RequirementDefinition | RequirementUsage | Stakeholder-imposed condition with textual statement and constraint features | 5.2.1.10 |
| ConcernDefinition | ConcernUsage | Stakeholder concern linked to requirements and viewpoints | 5.2.1.10 |
| ConstraintDefinition | ConstraintUsage | Boolean expression assertable against model elements for validation | 5.2.1.17 |
| AnalysisCaseDefinition | AnalysisCaseUsage | Evaluation of system properties with subject and objectives | 5.2.1.17 |
| VerificationCaseDefinition | VerificationCaseUsage | Verification activity with method (test, analysis, inspection, demonstration) and verdict | 5.2.1.17 |
| CalculationDefinition | CalculationUsage | Domain-specific computation over model attributes | 5.2.1.17 |
| AllocationDefinition | AllocationUsage | Mapping from functional to physical elements | 5.2.1.13 |
| ViewDefinition | ViewUsage | Presentation of model content for a specific purpose | 5.2.1.19 |
| ViewpointDefinition | ViewpointUsage | Stakeholder concerns specifying what a view addresses | 5.2.1.19 |
| MetadataDefinition | MetadataUsage | Tool-specific or process-specific annotation on model elements | 5.2.1.26 |

### 3.2 Abbreviated terms

| Abbreviation | Meaning |
|-------------|---------|
| MBSE | Model-Based Systems Engineering |
| SSS | Software System Specification |
| SRS | Software Requirements Specification |
| SRR | System Requirements Review |
| PDR | Preliminary Design Review |
| API | Application Programming Interface |
| JWT | JSON Web Token |
| OIDC | OpenID Connect |
| LDAP | Lightweight Directory Access Protocol |
| SAML | Security Assertion Markup Language |
| KerML | Kernel Modeling Language |
| SysML | Systems Modeling Language |
| IRI | Internationalized Resource Identifier |
| ReqIF | Requirements Interchange Format |
| HMI | Human-Machine Interface |

---

## 4. General description

### 4.1 Product perspective

Mycelium is the successor to CDP4-COMET, a collaborative MBSE tool based on ECSS-E-TM-10-25 that has been used for over 10 years to support the concurrent design process. Mycelium replaces CDP4-COMET's desktop-first, ECSS-E-TM-10-25-based approach with a cloud-native, web-first architecture natively implementing SysML v2.

The platform consists of three components:

- **Mycelium Bloom** communicates with Mycelium Fabric via REST/HTTP and SignalR (WebSocket). It renders the user interface, handles user interaction, and presents model data received from the backend.
- **Mycelium Fabric** implements the OMG Systems Modelling API, manages model persistence, enforces ownership-based access control, handles authentication/authorization, and propagates real-time notifications.
- **Mycelium Forge** provides a package registry for SysML v2 model libraries, enabling publishing, discovering, versioning and importing reusable model packages.

### 4.2 General capabilities

The Mycelium platform provides the following high-level capabilities:

- SysML v2 model creation, browsing, editing and visualization.
- Concurrent design session support for 20-30 simultaneous participants on one project.
- Lock-free collaborative modeling with ownership-based access control.
- Version control with branching, merging, tagging and commit history.
- Requirements management with traceability to design elements.
- Near real-time model change notification to all connected users.
- Diagram editing (Interconnection, Action Flow, State Transition, Sequence, General, Grid Views).
- Model import/export in SysML v2 JSON and XMI format and ECSS-E-TM-10-25 migration.
- Model export in a variety or formats inlcuding CSV, SVG, the SysML2 Textual Notation
- Self-service organization and project creation.
- SysML v2 library package management via Mycelium Forge based on kerml kpar.
- Mycelium Bloom must work not only with Mycelium Fabric but with any backend that implements the OMG Systems Modelling API.
- Attachment upload and download

### 4.3 General constraints

- The platform shall natively implement the SysML v2 metamodel (OMG formal/25-09-03) as its data model.
- The model server shall conform to the OMG Systems Modelling API and Services specification (formal/25-09-04) using the REST/HTTP PSM.
- The platform shall support the Kernel Modelling Language (KerML) as the underlying formalism for SysML v2.
- The web application shall not provide SysML v2 textual notation editing or parsing capabilities; The sysml v2 textual notation is generated read-only.

### 4.4 Operational environment

The Mycelium platform operates in two deployment models:

- **SaaS (multi-tenant):** Hosted by Starion. Each customer is an Organization (tenant boundary). Self-service organization and project creation.
- **On-premise (single-tenant):** Deployed on the customer's infrastructure using container orchestration. Customer IT handles infrastructure operations.

Mycelium Bloom is accessed through modern web browsers (desktop-optimized, responsive) without requiring desktop installation. Mycelium Fabric and Mycelium Forge are deployed as cloud-native containerized services.

External interfaces include:
- Browser ↔ Mycelium Bloom: HTTPS (HTML/JS content delivery)
- Mycelium Bloom ↔ Mycelium Fabric: HTTPS (REST/JSON, MessagePack) and WebSocket (SignalR)
- Mycelium Bloom ↔ Mycelium Forge: HTTPS (REST/JSON/KPAR)
- Mycelium Fabric ↔ Keycloak: OIDC/SAML for identity management
- External tools ↔ Mycelium Fabric: REST API (OMG Systems Modelling API)

### 4.5 Assumptions and dependencies

- An external identity provider (Keycloak) is available for authentication and authorization.
- PostgreSQL is available as the persistence layer.
- S3 Buckets are used to store attachments
- Users access the platform through modern web browsers with WebSocket support.
- On-premise deployments have container orchestration infrastructure (e.g. Kubernetes, Docker Compose).

---

## 5. Specific requirements

### 5.1 General

Requirements are uniquely identified using the convention `SSS-<role>-<area>-<alpha-numeric>`. Each requirement specifies the applicable roles (OA, PA, PT, VW, or All) indicating which user roles can exercise the capability. Role permissions are defined in [Roles and Permissions](Roles-and-Permissions.md).

| Abbreviation | Role | Description |
|-------------|------|-------------|
| IA | Installation Administrator | Super-admin managing all users and organizations across the entire installation; on SaaS maps to Platform Operator, on on-premise assigned to the first user at setup |
| OA | Organization Administrator | Owner of an Organization; manages users, authentication and security within the org |
| OM | Organization Member | Regular user within an Organization; can create projects if permitted |
| PA | Project Administrator | Owner of a Project; manages structure, team, branches, tags and merges |
| PT | Participant | Subject matter specialist who creates and modifies model elements within their assigned Ownership |
| VW | Viewer | Read-only observer of the model |
| All | All roles | Requirement applies to all authenticated users regardless of role |

The component is implicit from the section: 5.2.1 requirements apply to Mycelium Bloom, 5.2.2 to Mycelium Fabric, 5.2.3 to Mycelium Forge.

### 5.2 Capabilities requirements

#### 5.2.1 Mycelium Bloom

##### 5.2.1.1 Organization and user management

Mycelium is built around organizations as tenant boundaries. Users self-register, create or join organizations, and form project teams. The Organization Administrator manages the organization's user base, authentication settings and role assignments. The requirements in this section cover the everyday user and organization management operations that all team members rely on to participate in collaborative modelling and (potentialy) concurrent design.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-OA-USR-N35 | OA | Mycelium Bloom shall provide operations to create, update, deactivate, and delete user accounts when "an authenticated user with the Organization Administrator role accesses the user management interface." | - | H |  |
| SSS-OA-USR-T18 | OA | Mycelium Bloom shall display a list of all registered users within the organization and their current status when "the Organization Administrator navigates to the user management view." | - | H |  |
| SSS-OA-ROLE-TBX | OA | Mycelium Bloom shall provide operations to assign and revoke Organization Administrator and Organization Member roles when "the Organization Administrator selects a user and modifies their organization-level role." | - | H |  |
| SSS-OA-ROLE-PQH | OA | Mycelium Bloom shall provide a setting to control whether Organization Members can create projects within that Organization when "the Organization Administrator accesses the member permission settings." | - | H |  |
| SSS-CC-SS-HV9 | All | Mycelium Bloom shall create a new Organization and assign the requesting user as its Organization Administrator when "an authenticated user initiates organization creation from their account dashboard." | - | H |  |
| SSS-CC-SS-FUU | OA, OM | Mycelium Bloom shall create a new Project and assign the requesting user as its Project Administrator when "an Organization Member initiates project creation and the organization permits member project creation." | - | H |  |
| SSS-CC-SS-G6B | PA | Mycelium Bloom shall grant Outside Collaborators access to specific projects without organization membership when "a Project Administrator grants access to an external user with an assigned project-level role." | - | M |  |
| SSS-CC-SS-LEZ | All | Mycelium Bloom shall enforce project visibility rules (Private, Organization-visible, Public) when "a user attempts to access a project." | - | H |  |
| SSS-IA-ORG-V4R | IA | Mycelium Bloom shall display a list of all organizations on the installation with their name, creation date, member count, project count, and status (active/suspended) when "an Installation Administrator navigates to the installation administration view." | - | H |  |
| SSS-IA-ORG-K8W | IA | Mycelium Bloom shall provide operations to create, update, suspend, reactivate, and delete organizations when "an Installation Administrator accesses the organization management interface." | - | H |  |
| SSS-IA-ORG-M3J | IA | Mycelium Bloom shall display the details of an organization including its members, projects, roles, authentication configuration, and audit log when "an Installation Administrator selects an organization from the installation administration view." | - | H |  |
| SSS-IA-USR-B6P | IA | Mycelium Bloom shall display a list of all user accounts across all organizations with their username, email, organization memberships, roles, and status (active/deactivated) when "an Installation Administrator navigates to the installation user management view." | - | H |  |
| SSS-IA-USR-Q2N | IA | Mycelium Bloom shall provide operations to create, update, deactivate, and delete user accounts across all organizations when "an Installation Administrator accesses the installation user management interface." | - | H |  |
| SSS-IA-USR-H7F | IA | Mycelium Bloom shall provide operations to assign and remove users to and from any organization with a specified role when "an Installation Administrator selects a user and modifies their organization memberships." | - | H |  |
| SSS-IA-SYS-W9D | IA | Mycelium Bloom shall display installation-wide metrics including total users, total organizations, total projects, storage usage, and active sessions when "an Installation Administrator navigates to the installation dashboard." | - | H |  |
| SSS-IA-SYS-E3T | IA | Mycelium Bloom shall display an installation-wide audit log of administrative actions (organization creation/deletion, user creation/deactivation, role changes) when "an Installation Administrator navigates to the installation audit log view." | - | H |  |

##### 5.2.1.2 User profile

Users have a profile showing their identity, projects, and contributions, and is where they manage how they appear to others across the platform. The requirements in this section cover the profile page contents, personal details, project list with key metadata, and a contribution heatmap showing activity over time, and the editing of the user's own personal details and appearance (avatar and collaborator colour). Where identity is federated through an external identity provider, IdP-sourced attributes (such as email address) are displayed read-only and managed in the identity provider; the appearance attributes (avatar, collaborator colour) are always managed within Mycelium.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-CC-PROF-L6D | All | Mycelium Bloom shall display the user's profile details, a list of all projects the user is a member of, and a contribution heatmap when "a user navigates to their profile page." | - | H |  |
| SSS-CC-PROF-52O | All | Mycelium Bloom shall display each project in the profile project list with: project name, description, license, last updated date, visibility (private, organization, public), and activity sparkline when "the user views their profile project list." | - | H |  |
| SSS-CC-PROF-K7B | All | Mycelium Bloom shall update the user's profile personal details, display name, job title, and biography, and persist the change when "a user edits their personal details on their profile page and saves." | - | H |  |
| SSS-CC-PROF-M2F | All | Mycelium Bloom shall upload a profile avatar image, validating its format (PNG, JPG, WebP, or SVG) and maximum file size and offering a square crop, when "a user uploads or replaces their profile avatar." | - | M |  |
| SSS-CC-PROF-P9R | All | Mycelium Bloom shall remove the user's uploaded avatar and revert to a generated avatar derived from the user's initials when "a user removes their profile avatar." | - | M |  |
| SSS-CC-PROF-T4H | All | Mycelium Bloom shall display the user's assigned collaborator colour and provide a picker to change it from a palette constrained for legibility and contrast when "a user views or edits the appearance settings on their profile page." | - | M |  |
| SSS-CC-PROF-W1N | All | Mycelium Bloom shall propagate an updated display name, avatar, or collaborator colour to every surface that renders it, the project presence indicator, diagram co-presence indicators, comments, and contribution views, in near real-time when "a user changes their display name, avatar, or collaborator colour." | - | H |  |
| SSS-CC-PROF-B8C | All | Mycelium Fabric shall persist a user's avatar image and expose it through a stable avatar URL referenced by the presence and co-presence indicators when "a user uploads or removes their profile avatar." | - | M |  |
| SSS-CC-PROF-D5G | All | Mycelium Fabric shall assign a unique default collaborator colour to an Account on creation and persist any subsequent change to that colour when "an Account is created or its collaborator colour is changed." | - | M |  |

##### 5.2.1.3 Project management

A project is the unit of collaboration in Mycelium. Each project owns a SysML v2 model, a team, branches, and Ownership assignments. The Project Administrator (typically the study lead) configures the project, assigns roles, defines Ownerships, and oversees the model's structural integrity. The requirements in this section cover project creation, configuration, team management, and Ownership administration. Owner administration is only relevant in case the project is a Concurrent Design project.


| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-OA-PROJ-TWK | OA | Mycelium Bloom shall create a Project with metadata (name, description) and a default Branch, consistent with the Systems Modelling API Project concept, when "the Organization Administrator submits a valid project creation form." | TBC | H |  |
| SSS-OA-PROJ-PFY | OA | Mycelium Bloom shall delete a project within the organization, applying configurable deletion policies regarding project usages by other projects, when "the Organization Administrator initiates project deletion and confirms the action." | - | H |  |
| SSS-OA-PROJ-PZ9 | OA | Mycelium Bloom shall provide a policy setting that, when enabled, grants the Organization Administrator read-only access to all projects in the organization for audit purposes when "the Organization Administrator enables the organization-wide audit access policy." | - | M |  |
| SSS-PA-MGMT-B3R | PA | Mycelium Bloom shall provide an interface to update project properties including name, description, default branch and visibility when "the Project Administrator edits an existing project's settings." | - | H |  |
| SSS-PA-MGMT-8EF | PA | Mycelium Bloom shall provide operations to add and remove users (including Outside Collaborators) with assigned roles and Ownerships when "the Project Administrator accesses the team management interface of a project." | - | H |  |
| SSS-PA-MGMT-KYM | PA | Mycelium Bloom shall transfer the Project Administrator role to another team member when "the current Project Administrator selects a team member and confirms the transfer." | - | H |  |
| SSS-PA-MGMT-73C | PA | Mycelium Bloom shall provide a setting to configure the project mode (Regular or Concurrent Design) when "the Project Administrator accesses the project mode settings." | - | H |  |

##### 5.2.1.4 Project lifecycle state

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-STATE-V4R | PA, PT, VW | Mycelium Bloom shall display the current lifecycle state of a project in the project header and project list when "a user views a project." | - | H |  |
| SSS-PA-STATE-K7N | PA | Mycelium Bloom shall provide operations to transition a project between lifecycle states when "the Project Administrator changes the project's lifecycle state." | - | H |  |
| SSS-PA-STATE-W2D | All | Mycelium Bloom shall enforce the following project lifecycle states and their editing constraints: | - | H |  |

The following lifecycle states are defined (TBC):

| State | Description | Editing |
|-------|-------------|---------|
| **Preparation** | Project setup: structure, team, ownerships, reference data. Core team configures the baseline model. | Open to Project Administrator only |
| **Open** | Active modeling: all team members contribute within their Ownerships. Design sessions take place. | Open to all Participants per Ownership |
| **Review** | Model under review: no modifications permitted. Stakeholders and Viewers inspect the model and provide feedback. | Read-only for all roles |
| **Archived** | Study completed: model preserved as an immutable historical record. Can be reopened or used as a template for new projects. | Read-only for all roles |

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-STATE-M8T | PA | Mycelium Bloom shall transition a project from its current lifecycle state to any other lifecycle state (Preparation, Open, Review, Archived) when "the Project Administrator selects a target lifecycle state for the project." | - | H |  |
| SSS-PA-STATE-F3B | PA, OA | Mycelium Bloom shall create a new project pre-populated with the content of an Archived project when "a user creates a new project using an archived project as a template." | - | H |  |
| SSS-PA-STATE-R6G | PA, PT, VW | Mycelium Bloom shall display a visual indicator (e.g. banner, badge, or icon) showing the project's current lifecycle state (Preparation, Open, Review, Archived) when "a user views a project." | - | H |  |
| SSS-PA-STATE-Q8L | All | Mycelium Bloom shall assign Preparation as the default lifecycle state to a newly created project when "a user creates a new project and the organization has not configured a different default state." | - | H |  |
| SSS-OA-STATE-Z3W | OA | Mycelium Bloom shall provide a setting to configure the default project lifecycle state for newly created projects within the organization when "the Organization Administrator accesses the organization's project defaults settings." | - | M |  |

##### 5.2.1.5 Collaboration and awareness

Mycelium is a multi-user platform: in any project, several engineers from different ownerships are typically working on the model at the same time. The requirements in this section cover how Mycelium Bloom and Mycelium Fabric make collaboration *live*, visible, immediate, and lock-free, so that every user has continuous awareness of who else is in the project, what they are working on, and what is changing.

###### 5.2.1.5a Project-level user presence

When a user opens a project, they should see at a glance who else is currently working in the same project, without having to navigate to a separate panel. This is the equivalent of the avatar cluster Microsoft Word and Google Docs show in a shared document's title bar: a small, always-visible indication that "I am not alone here". The requirements in this subsection cover the **project-level** presence indicator only.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-CC-PRESENCE-A4N | All | Mycelium Bloom shall display a project presence indicator listing every Account currently connected to the project, persistently visible in the project chrome from every view within the project, when "a user has a project open." | - | H |  |
| SSS-CC-PRESENCE-B7P | All | Mycelium Bloom shall display each connected Account's display name, avatar, and assigned collaborator colour in the project presence indicator when "the project presence indicator is rendered." | - | H |  |
| SSS-CC-PRESENCE-D2K | All | Mycelium Bloom shall update the project presence indicator in near real-time as Accounts connect to or disconnect from the project when "Mycelium Fabric delivers a project presence event." | - | H |  |
| SSS-CC-PRESENCE-E5J | All | Mycelium Bloom shall render the project presence indicator as a compact avatar stack with a "+N more" overflow affordance that expands on demand when "more Accounts are connected to the project than the indicator's compact display can show." | - | M |  |
| SSS-CC-PRESENCE-F8M | All | Mycelium Bloom shall display the connected Account's full display name, identifier, and connected-since timestamp in a tooltip or popover when "a user hovers over or activates an entry in the project presence indicator." | - | M |  |
| SSS-CC-PRESENCE-G1R | All | Mycelium Bloom shall visually distinguish the local user's own entry in the project presence indicator (e.g. labelled "You" or rendered in a distinct slot) when "the project presence indicator includes the local user." | - | M |  |
| SSS-CC-PRESENCE-H6T | All | Mycelium Fabric shall publish a project presence event to all clients connected to a project when "an Account connects to or disconnects from the project." | - | H |  |

###### 5.2.1.5b Deep linking and sharing

Engineers need to share specific surfaces of the model, a part, a requirement, a diagram, or an element pinned to a particular view, by copying a URL into email, chat, a comment, or a browser bookmark. The recipient pastes the URL and lands directly on that surface, signed in if necessary. The requirements in this section cover URL addressability of every navigable surface, an in-app "copy link" affordance, and the resolution behavior when a URL is opened (including stability across renames, scoping to a specific view, and graceful handling of missing or inaccessible targets). These requirements are also the technical foundation for future external integrations (chat, email, third-party notification routing) that embed Mycelium URLs.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-CC-LINK-A2P | All | Mycelium Bloom shall expose every project, branch, model element, view, and diagram as a unique URL displayed in the browser address bar when "a user has a project, branch, element, view, or diagram open." | - | H |  |
| SSS-CC-LINK-B5R | All | Mycelium Bloom shall provide a "copy link" action on the detail panel and on the context (right-click) menu of every model element, view, and diagram that copies the current URL to the clipboard when "a user activates the copy link action." | - | H |  |
| SSS-CC-LINK-D8K | All | Mycelium Bloom shall navigate the user to the addressed project, branch, element, view, or diagram when "a user opens a Mycelium URL." | - | H |  |
| SSS-CC-LINK-E3M | All | Mycelium Bloom shall redirect the user through the authentication flow and resume navigation to the originally addressed surface after sign-in when "a user opens a Mycelium URL while not authenticated." | - | H |  |
| SSS-CC-LINK-F7N | All | Mycelium Bloom shall encode the branch (and optionally the commit) of the model in URLs it generates so that a shared link resolves to the same model state the link author was viewing when "Mycelium Bloom generates a shareable URL." | - | H |  |
| SSS-CC-LINK-G1V | All | Mycelium Bloom shall construct URLs using stable element identifiers so that the URL remains valid across element renames and namespace moves when "Mycelium Bloom generates a URL referencing a model element." | - | H |  |
| SSS-CC-LINK-H4T | All | Mycelium Bloom shall accept a URL that scopes a model element to a specific view, open that view, and select and center the addressed element when "a user opens a URL combining a view identifier and an element identifier." | - | M |  |
| SSS-CC-LINK-J9W | All | Mycelium Bloom shall display an informative message indicating whether the target was deleted, moved, or is inaccessible due to permissions when "a user opens a Mycelium URL whose target cannot be resolved or is not accessible." | - | M |  |

###### 5.2.1.5c Live model updates

When user A edits the model, user B should see the change in near real-time without manually refreshing. Mycelium Bloom listens for change notifications from the backend and updates open views accordingly. The requirements in this section cover the UI behavior on receipt of live updates, including conflict indicators and preservation of the user's editing context.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-CC-LIVE-R4K | All | Mycelium Bloom shall update the hierarchical browser, detail panels, and tabular browsers in near real-time to reflect changes committed by other users when "Mycelium Fabric delivers a model change notification." | - | H |  |
| SSS-CC-LIVE-W7N | All | Mycelium Bloom shall update all open diagrams in near real-time to reflect changes to model elements committed by other users when "a diagram contains elements that have been modified by another user." | - | H |  |
| SSS-CC-LIVE-H3D | All | Mycelium Bloom shall display a visual indicator (e.g. highlight, flash, or badge) on model elements that have been modified by another user when "a model change notification is received for an element visible in the current view." | - | M |  |
| SSS-CC-LIVE-M6J | All | Mycelium Bloom shall display a notification summary indicating the number and nature of changes made by other users when "one or more model change notifications are received while the user is working." | - | M |  |
| SSS-CC-LIVE-T9F | PA, PT | Mycelium Bloom shall present a conflict indicator when the current user has uncommitted local changes to an element that another user has also modified and committed when "a model change notification is received for an element with pending local edits." | - | H |  |
| SSS-CC-LIVE-K2B | All | Mycelium Bloom shall maintain the user's current scroll position, selection, and editing state when applying live model updates from other users when "the UI refreshes in response to incoming model changes." | - | H |  |

##### 5.2.1.6 Change persistence

Mycelium Bloom operates in two persistence modes. In immediate mode, each edit is persisted to Mycelium Fabric as an individual Commit on the active branch, making it visible to other users in near real-time. In batch mode, the user collects multiple changes locally before persisting them as a single atomic Commit. Both modes produce Systems Modelling API Commits.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-PERSIST-K4R | PA, PT | Mycelium Bloom shall persist each model edit (create, update, delete) to Mycelium Fabric as an individual Commit on the active branch in immediate mode when "a user completes an edit and immediate persistence mode is active." | API 7.2.3 | H |  |
| SSS-PA-PERSIST-W8N | PA, PT | Mycelium Bloom shall accumulate model edits locally without persisting them to Mycelium Fabric in batch mode when "a user performs edits and batch persistence mode is active." | API 7.2.3 | H |  |
| SSS-PA-PERSIST-D3J | PA, PT | Mycelium Bloom shall persist all accumulated local edits to Mycelium Fabric as a single atomic Commit on the active branch when "a user submits the batch with a commit description in batch mode." | API 7.2.3 | H |  |
| SSS-PA-PERSIST-C6M | PA, PT | Mycelium Bloom shall provide an input for the user to enter a commit message, stored as the description of the resulting Commit, when "a user commits one or more model changes." | API 7.2.3 | H |  |
| SSS-PA-PERSIST-H7T | PA, PT | Mycelium Bloom shall provide a toggle to switch between immediate mode and batch mode when "a user changes the persistence mode in the application settings or toolbar." | - | H |  |
| SSS-PA-PERSIST-M2F | PA, PT | Mycelium Bloom shall display a pending changes indicator showing the number of uncommitted local edits when "the user is in batch mode and has accumulated local changes." | - | H |  |
| SSS-PA-PERSIST-R5V | PA, PT | Mycelium Bloom shall display a list of all pending local changes with element name, change type (created, updated, deleted), and changed properties when "the user reviews the pending changes before committing a batch." | - | H |  |
| SSS-PA-PERSIST-N9B | PA, PT | Mycelium Bloom shall discard all pending local changes and revert to the last committed model state when "the user cancels a batch in batch mode." | - | H |  |
| SSS-PA-PERSIST-T1G | PA, PT | Mycelium Bloom shall warn the user about unsaved local changes when "the user attempts to close the application, switch projects, or switch branches while in batch mode with pending changes." | - | H |  |

##### 5.2.1.7 Concurrent Design

Concurrent design brings 20-30 engineers from different domains into the same room (or video call) to design a system together in real time. Mycelium must handle this scale, propagate changes across all connected users, and present session-aware views that show what is happening across the team. The requirements in this section cover concurrent session participation and the views engineers need during a session.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PT-CDS-RKV | PA, PT | Mycelium Bloom shall support concurrent design sessions with at least 20-30 Participants from multiple Ownerships working simultaneously when "multiple Participants are connected to the same project and actively modifying model data." | - |  |  |
| SSS-PT-UI-256 | PT | Mycelium Bloom shall present a selector to switch the active Ownership when "the Participant is assigned to multiple Ownerships and selects a different active Ownership from the Ownership selector." | - | H |  |

###### 5.2.1.7a Subscriptions

When one engineer's work depends on another's outputs, they need to track those outputs and decide how changes propagate into their own work. Mycelium models these dependencies as ParameterSubscriptions: a subscriber's Ownership expresses interest in an attribute owned by another Ownership and is notified when its value is published. The requirements in this section cover creating subscriptions individually, in bulk (by attribute kind, by element, or by owner), and through standing rules that automatically subscribe to matching attributes created later; choosing how each subscription sources its value (the owner's published value or the subscriber's own override); reviewing subscriptions and their up-to-date status; and keeping subscription sets consistent as the model changes.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PT-COLLAB-8U9 | PT | Mycelium Bloom shall create a ParameterSubscription on an AttributeUsage owned by another Ownership when "the Participant selects an attribute from another Ownership and initiates a subscription." | - |  |  |
| SSS-PT-COLLAB-12K | PT | Mycelium Bloom shall deliver a near real-time notification when "an attribute that the Participant has subscribed to is published by another Ownership." | - |  |  || SSS-PT-SUB-A1K | PT | Mycelium Bloom shall create a standing subscription rule that subscribes the Participant's Ownership to every existing and future AttributeUsage typed by a selected AttributeDefinition kind (e.g. mass, mass margin) and owned by another Ownership when "the Participant defines a standing subscription rule for one or more attribute kinds." | - | H |  |
| SSS-PT-SUB-B4R | PT | Mycelium Bloom shall scope a standing subscription rule to all other Ownerships or to one or more selected Ownerships when "the Participant configures the owner scope of a standing subscription rule." | - | M |  |
| SSS-PT-SUB-C7M | PT | Mycelium Bloom shall display, edit, enable, disable, and delete the Participant's standing subscription rules when "the Participant opens the standing subscription rules manager." | - | M |  |
| SSS-PT-SUB-D9T | All | Mycelium Fabric shall evaluate the applicable standing subscription rules and create the corresponding ParameterSubscription when "an AttributeUsage matching a standing rule's attribute kind and owner scope is created or becomes owned by another Ownership." | - | H |  |
| SSS-PT-SUB-E2F | PT | Mycelium Bloom shall create ParameterSubscriptions on all existing AttributeUsages typed by one or more selected AttributeDefinition kinds and owned by other Ownerships, optionally restricted to selected Ownerships or a selected package subtree, in a single operation when "the Participant selects one or more attribute kinds and invokes batch subscribe." | - | H |  |
| SSS-PT-SUB-F5J | PT | Mycelium Bloom shall delete all of the Participant's ParameterSubscriptions in the project in a single confirmed operation when "the Participant invokes delete-all-subscriptions and confirms." | - | M |  |
| SSS-PT-SUB-G8P | PT | Mycelium Bloom shall delete all of the Participant's ParameterSubscriptions to attributes owned by one or more selected Ownerships in a single operation when "the Participant selects one or more owning Ownerships and invokes batch unsubscribe." | - | H |  |
| SSS-PT-SUB-H3W | PT | Mycelium Bloom shall delete all of the Participant's ParameterSubscriptions on AttributeUsages typed by one or more selected AttributeDefinition kinds in a single operation when "the Participant selects one or more attribute kinds and invokes batch unsubscribe." | - | M |  |
| SSS-PT-SUB-L4G | PT | Mycelium Bloom shall display a Subscriptions view listing every ParameterSubscription held by the Participant's Ownership, showing the subscribed attribute and its owning element, the owning Ownership, the latest published value, the subscriber's effective value and value source, and the subscription status, when "the Participant opens the Subscriptions view." | - | H |  |
| SSS-PT-SUB-P7K | PT | Mycelium Bloom shall create ParameterSubscriptions on all AttributeUsages of a selected element that are owned by another Ownership in a single operation when "the Participant selects an element and invokes subscribe-to-element." | - | M |  |
| SSS-PT-SUB-Q5R | PT | Mycelium Bloom shall create ParameterSubscriptions on all AttributeUsages owned by a selected Ownership in a single operation when "the Participant selects an Ownership and invokes subscribe-to-owner." | - | M |  |
| SSS-PT-SUB-V3J | PT | Mycelium Bloom shall re-evaluate and notify the subscriber when the Ownership of a subscribed AttributeUsage is reassigned, removing the subscription if the attribute becomes owned by the subscriber's own Ownership, when "the Ownership of a subscribed attribute is reassigned." | - | M |  |

###### 5.2.1.7b Publication workflow

In Concurrent Design Mode, attribute owners edit their own values (OwnedValue) without immediately affecting the values visible to subscribers. A publication event copies the OwnedValue to the AttributeUsage value, making it available to all consumers. This staged, manual publication is the default. A project may instead enable *auto-publish mode*, in which each owner edit is published immediately and the manual publication step is not required. The publication mechanism is modeled in the Concurrent Design library using PublicationDefinition, PublishedIn, and OwnedValue MetadataDefinitions (see [Roles and Permissions](Roles-and-Permissions.md)).

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PT-PUB-A3P | PA | Mycelium Bloom shall provide a setting to enable or disable auto-publish mode for a Concurrent Design project, defaulting to disabled, when "the Project Administrator edits the project's publication settings." | - | H |  |
| SSS-PT-PUB-K4W | PT | Mycelium Bloom shall store the owner's pending value as an OwnedValue metadata annotation on the AttributeUsage, without overwriting the published attribute value visible to subscribers, when "a Participant edits an attribute value in Concurrent Design Mode with auto-publish disabled." | - | H |  |
| SSS-PT-PUB-C8L | PT | Mycelium Bloom shall publish an attribute value immediately upon edit, making the new value visible to all subscribers without requiring a manual publish operation, when "a Participant edits an attribute value while auto-publish mode is enabled." | - | H |  |
| SSS-PT-PUB-R7N | PA, PT, VW | Mycelium Bloom shall visually distinguish attributes with pending unpublished changes (OwnedValue differs from published value) from attributes where OwnedValue and published value are equal when "a user views attributes in the model browser, detail panel, or tabular views." | - | H |  |
| SSS-PT-PUB-D3M | PA, PT, VW | Mycelium Bloom shall display the old (published) value, the new (owned) value, and the difference (absolute and percentage) for each attribute with pending changes when "a user opens the publication review view." | - | H |  |
| SSS-PT-PUB-H8J | PA | Mycelium Bloom shall publish all pending attribute changes across all Ownerships in a single operation, copying each OwnedValue to its corresponding AttributeUsage value and creating a PublicationDefinition record with timestamp, when "the Project Administrator initiates a publish-all operation." | - | H |  |
| SSS-PT-PUB-W5T | PA | Mycelium Bloom shall publish pending attribute changes for a single selected Ownership, copying only that Ownership's OwnedValues to their corresponding AttributeUsage values and recording the publication, when "the Project Administrator initiates a publish-per-ownership operation and selects one or more Ownerships." | - | H |  |
| SSS-PT-PUB-M2F | PA, PT, VW | Mycelium Bloom shall display a publication history listing all past publications with their timestamp, the publishing user, and the Ownerships included when "a user opens the publication history view." | - | H |  |
| SSS-PT-PUB-N6K | PA, PT, VW | Mycelium Bloom shall display the list of attributes that were published in a specific publication event, showing the attribute name, element, old value, new value, and Ownership, when "a user selects a publication record from the publication history." | - | H |  |
| SSS-PT-PUB-F1V | All | Mycelium Fabric shall reject direct modification of published attribute values by non-owner Participants and enforce that only the publication workflow updates the shared attribute value in Concurrent Design Mode when "a Participant attempts to write directly to an AttributeUsage value they subscribe to." | - | H |  |

##### 5.2.1.8 Model navigation and browsing

Engineers spend a lot of of their time finding, selecting, and understanding model elements. Mycelium offers complementary navigation views: a hierarchical tree for structural exploration and a tabular browser for flat searching with namespace path columns. The requirements in this section ensure that users can find any element quickly, see its qualified context, and follow relationships to related elements without losing their place.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-NAV-8IB | PA, PT, VW | Mycelium Bloom shall display the model as a hierarchical tree (Browser View) with collapsible and expandable nodes representing packages, parts, and nested elements when "a user opens a project and navigates to the Browser View." | - | H |  |
| SSS-PA-NAV-ZLS | PA, PT, VW | Mycelium Bloom shall return filtered model elements matching the specified criteria when "a user enters search terms or applies filters by name, type or Ownership." | - | H |  |
| SSS-PA-NAV-ZRW | PA, PT, VW | Mycelium Bloom shall display element properties including attributes, relationships, and ownership in a detail panel when "a user selects a model element." | - | H |  |
| SSS-PT-NAV-7U5 | PA, PT, VW | Mycelium Bloom shall display the Ownership of each element and attribute when "a user views a model element's properties or browses the model tree." | - | H |  |
| SSS-PA-NAV-KVE | PA, PT, VW | Mycelium Bloom shall navigate to the related element when "a user activates a relationship link on a model element (e.g. from a requirement to the part that satisfies it)." | - | M |  |
| SSS-PA-NAV-F3K | PA, PT, VW | Mycelium Bloom shall display the qualified name (namespace path) of each model element when "a user views an element's properties in the detail panel." | - | H |  |
| SSS-PA-NAV-G5X | PA, PT, VW | Mycelium Bloom shall provide a tabular element browser that lists Definitions and Usages for each kind of Definition and Usage in a sortable, filterable table showing element name, namespace path, type, Ownership, and key attributes when "a user opens the tabular element browser." | - | H |  |
| SSS-PA-NAV-W4B | PA, PT, VW | Mycelium Bloom shall support the hierarchical Browser View and the tabular element browser as independent views that can be open simultaneously when "a user has both views open." | - | H |  |
| SSS-PA-NAV-M2C | PA, PT, VW | Mycelium Bloom shall open and display multiple hierarchical Browser Views and multiple tabular element browsers at the same time, without limiting the user to a single instance of either, each maintaining its own scope, filters, sorting, and selection, when "a user opens an additional Browser View or tabular element browser." | - | H |  |

##### 5.2.1.9 Namespace and package management

SysML v2 organizes models into Packages and Namespaces. Packages group related elements; Namespaces control naming and visibility; Imports allow reuse without duplication. The requirements in this section ensure users can structure their models hierarchically, share content between packages, and apply visibility rules without leaving the model browser.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-PKG-R8W | PA, PT | Mycelium Bloom shall support creating, renaming, moving, and deleting Packages to organize model elements into logical groups when "a user manages packages in the model browser." | SysML 7.5 | H |  |
| SSS-PA-PKG-V2J | PA, PT | Mycelium Bloom shall support nesting Packages within other Packages to create hierarchical model organization when "a user creates a child package within an existing package." | SysML 7.5 | H |  |
| SSS-PA-PKG-D4N | PA, PT | Mycelium Bloom shall support importing elements from one Namespace into another using Membership Imports and Namespace Imports when "a user creates an import relationship between namespaces." | SysML 7.5 | H |  |
| SSS-PA-PKG-J3W | PA, PT | Mycelium Bloom shall create a Filtered Import that imports only elements matching a metadata-based condition (e.g. import only elements annotated with a specific Metadata Usage) when "a user creates a namespace import and specifies a metadata filter expression." | SysML 7.5.4 | H |  |
| SSS-PA-PKG-H6T | PA, PT | Mycelium Bloom shall support setting member visibility (public, private) on elements within a Namespace when "a user configures the visibility of a model element within its owning namespace." | KerML 7.2.5 | H |  |
| SSS-PA-PKG-Q1M | PA, PT | Mycelium Bloom shall support creating Alias memberships to provide alternative names for elements within a namespace when "a user assigns an alias to an imported or local element." | KerML 7.5.2 | M |  |
| SSS-PA-PKG-V8N | PA, PT, VW | Mycelium Bloom shall display the visibility marker (public, private, protected) of every Membership alongside its owning element in the model browser, the detail panel, and tabular views when "a user views a namespace or its members." | KerML 7.2.5 | H |  |
| SSS-PA-PKG-T5C | PA, PT, VW | Mycelium Bloom shall display the imported members of a Namespace in the model browser, visually distinguished from owned members by a dedicated icon or rendering style, when "a user expands a namespace that declares one or more imports." | KerML 7.5 | H |  |
| SSS-PA-PKG-C7B | PA, PT | Mycelium Bloom shall display the validation error returned by Mycelium Fabric for a duplicate `memberName` conflict, highlighting the conflicting members and blocking the offending edit, when "a commit or edit submitted by a user is rejected by Fabric due to a duplicate `memberName`." | KerML 7.2.5 | H |  |
| SSS-PA-NAV-S6P | PA, PT, VW | Mycelium Bloom shall provide a global search interface that matches model elements by `name` and by `qualifiedName` across every Namespace in the current project, returning results with their qualified path and navigation link, when "a user enters a search term into the global search bar." | KerML 7.2.5 | H |  |
| SSS-PA-ELEM-R3G | PA, PT, VW | Mycelium Bloom shall propagate a rename operation to every displayed `qualifiedName` of the renamed element and of its transitive descendants (in the model browser, detail panels, diagram labels, tooltips, breadcrumbs, and tabular views) when "a user renames a Namespace or one of its members." | KerML 7.2.5 | H |  |
| SSS-PA-ELEM-M9T | PA, PT | Mycelium Bloom shall re-parent a model element to a new owning Namespace, update its `qualifiedName` and those of its transitive descendants, and preserve all incoming references to the moved element when "a user moves an element to a different namespace via drag-and-drop or the move action." | KerML 7.2.5 | H |  |
| SSS-PA-NAV-B8D | PA, PT, VW | Mycelium Bloom shall display a breadcrumb trail of the `qualifiedName` segments of the currently selected element and shall navigate to the corresponding owning Namespace when "a user clicks a segment in the breadcrumb trail." | KerML 7.2.5 | M |  |
| SSS-PA-PKG-N4J | PA, PT | Mycelium Bloom shall create a NamespaceImport in the importing Namespace, referencing the imported Namespace, when "a user selects a target Namespace and invokes the 'Import Namespace' action from a package or namespace." | KerML 7.5.3 | H |  |
| SSS-PA-PKG-M5P | PA, PT | Mycelium Bloom shall create a MembershipImport in the importing Namespace, referencing the imported Membership, when "a user selects a single named element from another Namespace and invokes the 'Import Member' action." | KerML 7.5.3 | H |  |
| SSS-PA-PKG-R9K | PA, PT | Mycelium Bloom shall set the `isRecursive` flag on a NamespaceImport, causing nested namespaces of the imported Namespace to be imported as well, when "a user toggles the 'include nested namespaces' option on a NamespaceImport." | KerML 7.5.3 | H |  |
| SSS-PA-PKG-A7Q | PA, PT | Mycelium Bloom shall set the `isImportAll` flag on an Import, causing non-public Memberships to be included in the imported set, when "a user toggles the 'include private members' option on an Import." | KerML 7.5.3 | H |  |
| SSS-PA-PKG-H3W | PA, PT | Mycelium Bloom shall set the visibility of an Import to public, private, or protected, controlling whether the Import is re-exported through transitive imports, when "a user edits the visibility of an Import." | KerML 7.5.3 | H |  |
| SSS-PA-PKG-L6D | PA, PT, VW | Mycelium Bloom shall display, in the detail panel of a Namespace, the list of Imports it declares, showing the import kind (NamespaceImport or MembershipImport), the imported target, and the `isRecursive`, `isImportAll`, and visibility values, when "a user views a Namespace that declares one or more Imports." | KerML 7.5.3 | H |  |
| SSS-PA-PKG-X8C | PA, PT | Mycelium Bloom shall delete an Import from a Namespace when "a user selects an Import in the detail panel and invokes the 'Remove import' action." | KerML 7.5.3 | H |  |
| SSS-PA-PKG-X1J | PA, PT | Mycelium Bloom shall detect when a user operation (drag-and-drop from another package or library, type assignment, specialization, reference creation, or any other operation) references an Element whose owning Namespace is not already visible in the current Namespace, and shall create the appropriate Import (a MembershipImport for a single-element reference, or a NamespaceImport when the user chooses to import the whole Namespace) as part of the same user operation, when "a user uses an Element from another Namespace that is not yet imported into the current Namespace." | KerML 7.5.3 | H |  |
| SSS-PA-PKG-X2K | PA, PT | Mycelium Bloom shall present a confirmation dialog identifying the referenced Element, its owning Namespace, and the proposed Import kind (MembershipImport of the specific Element or NamespaceImport of the owning Namespace), and shall not create the Import or complete the triggering operation until the user confirms the proposed action or selects an alternative, when "Mycelium Bloom is about to auto-create an Import in response to a cross-namespace user operation." | KerML 7.5.3 | H |  |
| SSS-PA-PKG-X3L | PA, PT | Mycelium Bloom shall not create a new Import when the referenced Element is already resolvable in the current Namespace through an existing MembershipImport, NamespaceImport, transitive NamespaceImport, or AliasMembership of compatible visibility, and shall complete the triggering user operation without modifying the Import set, when "a user uses an Element whose owning Namespace is already imported." | KerML 7.5.3 | H |  |
| SSS-PA-PKG-X4M | PA | Mycelium Bloom shall provide a per-project preference controlling whether `SSS-PA-PKG-X2K` is enforced on every auto-Import or whether auto-Imports are created silently, with a default value of "always confirm", when "a Project Administrator edits the project settings." | KerML 7.5.3 | M |  |
| SSS-PA-PKG-P8D | PA | Mycelium Bloom shall convert a Package into a LibraryPackage, or create a new LibraryPackage, when "a user invokes the 'Convert to Library' action on a Package or the 'New Library Package' action in the model browser." | KerML 7.5.5 | H |  |
| SSS-PA-PKG-S1E | PA | Mycelium Bloom shall set the `isStandard` flag on a LibraryPackage, marking it as a standard library distinct from a user library, when "a user toggles the 'Standard library' option on a LibraryPackage." | KerML 7.5.5 | H |  |
| SSS-PA-PKG-M3G | PA, PT | Mycelium Bloom shall edit the metadata of a Package or LibraryPackage (version, author, description, license, and tags) via the detail panel when "a user edits any of the metadata fields of a Package or LibraryPackage." | - | H |  |
| SSS-PA-PKG-V4H | PA, PT, VW | Mycelium Bloom shall render a LibraryPackage in the model browser, tabular views, and diagrams with a distinguishing icon or badge that sets it apart from a regular Package, when "a user views a LibraryPackage." | KerML 7.5.5 | H |  |
| SSS-PA-PKG-F8M | PA | Mycelium Bloom shall import a LibraryPackage from Mycelium Forge into the current project, creating the corresponding NamespaceImport and fetching the referenced content, when "a user selects a LibraryPackage from Mycelium Forge and invokes the 'Import Library' action." | - | H |  |
| SSS-PA-IE-GYP | PA | Mycelium Bloom shall provide operations to create and manage Project Usages to reference elements from one Project within another, consistent with the Systems Modelling API ProjectUsageService, when "the Project Administrator creates a Project Usage and selects the target project to reference." | API 7.4 | H |  |
| SSS-PA-MGMT-YC1 | PA | Mycelium Bloom shall provide operations to create, rename and remove Ownership Usages within the project package when "the Project Administrator accesses the Ownership management interface." | - | H |  |
| SSS-PA-MGMT-BA7 | PA | Mycelium Bloom shall reassign element ownership by updating the Owner metadata on a model element to a different Ownership when "the Project Administrator selects a model element and changes its Owner annotation." | - | H |  |
| SSS-PA-ELEM-O2K | PA, PT, VW | Mycelium Bloom shall display, in the detail panel of any Namespace or Type, the complete list of its Memberships grouped by kind (OwningMembership, FeatureMembership, AliasMembership, imported Membership, VariantMembership, StakeholderMembership, ActorMembership, SubjectMembership, FramedConcernMembership, RequirementConstraintMembership, RequirementVerificationMembership, ExposeMembership, ObjectiveMembership), showing each member's `memberName`, visibility, and the source of the membership, when "a user views the detail panel of a Namespace or Type." | KerML 7.2 | M |  |

##### 5.2.1.10 Requirements modeling

Requirements capture stakeholder-imposed conditions that a design must satisfy. SysML v2 models requirements as Constraint Definitions with subjects, actors, stakeholders, assumed and required constraints, and concerns. Requirements can be nested, derived, satisfied by design elements, and verified by Verification Cases. The requirements in this section cover modeling the full SysML v2 requirements metamodel as first-class model elements through user-facing operations.

###### 5.2.1.10.a Requirement definitions and constraints

A Requirement Definition captures a stakeholder-imposed condition as a textual statement together with the assumed and required constraints that formalise it. Requirements can be organised into hierarchical specifications, where nested requirements become required constraints of their parent. The requirements in this subsection cover creating, editing, organising, and nesting requirements and editing their constraint expressions.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-REQ-QP0 | PA, PT | Mycelium Bloom shall provide operations to create, edit, and organize Requirement Definitions and Requirement Usages in hierarchical specifications with textual statements when "a user accesses the requirements modeling interface and creates or modifies a requirement." | SysML 7.21 | H |  |
| SSS-PA-REQ-WD0 | PA, PT | Mycelium Bloom shall nest a Requirement Usage within a parent Requirement Definition or Requirement Usage, where nested requirements automatically become required constraints of the parent, when "a user adds a child requirement to an existing requirement." | SysML 7.21, 8.3.21 | H |  |
| SSS-PA-REQ-DS6 | PA, PT | Mycelium Bloom shall provide editors for assumed constraints and required constraints on requirements, where the effective requirement logic is "if all assumed constraints hold then all required constraints must be satisfied", when "a user edits a requirement and adds constraint expressions." | SysML 8.3.21.7 | H |  |

###### 5.2.1.10.b Subjects, actors, stakeholders, and concerns

A requirement is framed by what it applies to and who cares about it. SysML v2 binds a requirement to its subject, to the actors needed to fulfil it, and to the stakeholders whose concerns it addresses. The requirements in this subsection cover assigning subjects, actors, and stakeholders, and modelling stakeholder concerns.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-REQ-T8K | PA, PT | Mycelium Bloom shall assign a subject to a Requirement Definition or Requirement Usage via Subject Membership, binding the requirement to the system or element it applies to, when "a user specifies the subject of a requirement." | SysML 8.3.21.11 | H |  |
| SSS-PA-REQ-M3N | PA, PT | Mycelium Bloom shall assign one or more actors to a Requirement Definition or Requirement Usage via Actor Membership, representing external entities necessary for the requirement to be fulfilled, when "a user adds actors to a requirement." | SysML 8.3.21.2 | H |  |
| SSS-PA-REQ-H6W | PA, PT | Mycelium Bloom shall assign one or more stakeholders to a Requirement Definition or Requirement Usage via Stakeholder Membership, representing entities with concerns about the requirement, when "a user adds stakeholders to a requirement." | SysML 8.3.21.12 | H |  |
| SSS-PA-REQ-SUC | PA, PT | Mycelium Bloom shall provide operations to create Concern Definitions and Concern Usages representing stakeholder concerns, and frame them in requirements or viewpoints via Framed Concern Membership, when "a user creates a Concern and associates it with a requirement or viewpoint." | SysML 8.3.21.3 | M |  |
| SSS-PA-REQ-RF1 | PA, PT, VW | Mycelium Bloom shall display, in the detail panel of a Requirement Definition or Requirement Usage, its subject, actors, stakeholders, and framed concerns, each navigable to the referenced element, when "a user views a requirement." | SysML 8.3.21 | H |  |

###### 5.2.1.10.c Requirement relationships and coverage

Requirements are connected to the rest of the model through trace relationships: derivation between requirements, satisfaction by design elements, and verification by verification cases. The requirements in this subsection cover these trace relationships and the coverage analysis that reports requirements lacking satisfaction or derivation.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-REQ-V4J | PA, PT | Mycelium Bloom shall create a Derivation relationship between requirements, linking an original requirement to one or more derived requirements with the semantic constraint that satisfaction of the original implies satisfaction of all derived requirements, when "a user creates a derivation trace between requirements." | SysML 9.6 | H |  |
| SSS-PA-REQ-W9B | PA, PT | Mycelium Bloom shall link a Verification Case Usage to a Requirement Usage via Requirement Verification Membership, recording which verification cases verify which requirements, when "a user associates a verification case with a requirement." | SysML 8.3.24.2 | H |  |
| SSS-PA-TRACE-Q72 | PA, PT | Mycelium Bloom shall create a SatisfyRequirementUsage recording that a design element satisfies a requirement when "a user selects a design element and a requirement and creates a satisfy relationship." | SysML 8.3.21.10 | H |  |
| SSS-PA-TRACE-N19 | PA | Mycelium Bloom shall identify and report requirements that are neither satisfied by a design element nor derived to a further requirement when "the Project Administrator executes a requirements coverage analysis." | - | M |  |
| SSS-PA-REQ-RF2 | PA, PT, VW | Mycelium Bloom shall display, in the detail panel of a requirement, its assumed and required constraints, the design elements that satisfy it, the verification cases that verify it, and its derivation relationships, each navigable to the referenced element, when "a user views a requirement." | SysML 8.3.21 | M |  |

###### 5.2.1.10.d Use cases

A Use Case Definition captures required system behaviour from the perspective of an external actor pursuing a goal, complementing the textual requirements with an actor-and-goal view of what the system must do. Use cases can include the behaviour of other use cases. The requirements in this subsection cover defining use cases and the include relationships between them.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-BEH-IX9 | PA, PT | Mycelium Bloom shall create a Use Case Definition specifying system behavior from an external actor perspective when "a user creates a Use Case Definition and specifies actors and subjects." | SysML 7.25 | M |  |
| SSS-PA-BEH-T7P | PA, PT | Mycelium Bloom shall create an Include Use Case Usage that includes one Use Case as part of another Use Case when "a user designates one Use Case as included by another." | SysML 7.25 | M |  |

##### 5.2.1.11 System architecture modeling

The core of system modeling is defining the building blocks (Definitions) of the system and instantiating them in a hierarchy (Usages). Engineers compose parts, items, ports, connections, and interfaces into a decomposed system architecture. The requirements in this section cover the SysML v2 structural concepts that engineers use to capture the what and how of a system, plus the everyday operations to duplicate, move, delete, and refine these elements.

###### 5.2.1.11.a General

The requirements in this section apply to every kind of Definition and Usage. They cover the operations common to all model elements, namely creating and instantiating them, reading and inspecting their details, updating their properties and relationships, deleting them, and navigating to and between them across the hierarchical browser, the tabular browser, and diagrams. Type-specific behaviour is covered in the dedicated subsections that follow.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PT-DATA-XHY | PA, PT | Mycelium Bloom shall create, modify and delete elements (parts, items, attributes, etc.) throughout a Project when "a user creates, modifies or deletes elements annotated with their Ownership." | - | H |  |
| SSS-PA-ELEM-C1A | PA, PT | Mycelium Bloom shall create a Definition of any kind from the hierarchical browser, the tabular browser, or a diagram when "a user invokes a create action for a Definition in any of these views." | - | H |  |
| SSS-PA-ELEM-C2B | PA, PT | Mycelium Bloom shall instantiate a Usage from an existing Definition, establishing the FeatureTyping to that Definition, when "a user instantiates a Definition as a Usage in any view." | - | H |  |
| SSS-PA-ELEM-C3C | PA, PT | Mycelium Bloom shall create a Usage together with a new Definition in a single operation when "a user creates a Usage without selecting an existing Definition." | - | H |  |
| SSS-PT-DATA-N7O | PT | Mycelium Bloom shall display and enable editing of model elements within the user's assigned Ownership when "the Participant navigates to a model element annotated with their Ownership as Owner." | - | H |  |
| SSS-PA-ELEM-R1D | PA, PT, VW | Mycelium Bloom shall display the full details of a selected element, including its name, short name, type, multiplicity, Ownership, documentation, attributes, and relationships, in the detail panel when "a user selects an element in any view." | - | H |  |
| SSS-PA-ELEM-U1E | PA, PT | Mycelium Bloom shall edit the declared name and declared short name of any element when "a user renames an element in the detail panel, the browser, a tabular view, or a diagram." | - | H |  |
| SSS-PA-ELEM-U2F | PA, PT | Mycelium Bloom shall edit the documentation of any element when "a user edits the documentation text of an element in the detail panel." | - | H |  |
| SSS-PT-DATA-M6H | PT | Mycelium Bloom shall automatically annotate newly created model elements with the Participant's active Ownership as Owner when "the Participant creates a new model element and the Model is a Concurrent Desing Model." | - | H |  |
| SSS-PA-ELEM-K4T | PA, PT | Mycelium Bloom shall present a duplicate dialog offering the user three independent options, preserve original Ownership (yes/no), copy attribute values (yes/no), copy nested children recursively (yes/no), when "a user initiates the duplication of a model element." | - | M |  |
| SSS-PA-ELEM-R8V | PA, PT | Mycelium Bloom shall duplicate a Definition or Usage with the user's active Ownership assigned as Owner on the copy when "a user duplicates a model element with the preserve-ownership option set to no." | - | M |  |
| SSS-PA-ELEM-T2N | PA, PT | Mycelium Bloom shall duplicate a Definition or Usage with the original Ownership assignments preserved on the copy when "a user duplicates a model element with the preserve-ownership option set to yes." | - | M |  |
| SSS-PA-ELEM-P5K | PA, PT | Mycelium Bloom shall duplicate a Definition or Usage and copy all attribute values from the source to the copy when "a user duplicates a model element with the copy-attribute-values option set to yes." | - | M |  |
| SSS-PA-ELEM-H8W | PA, PT | Mycelium Bloom shall duplicate a Definition or Usage and reset all attribute values on the copy to unset when "a user duplicates a model element with the copy-attribute-values option set to no." | - | M |  |
| SSS-PA-ELEM-D7M | PA, PT | Mycelium Bloom shall duplicate a Definition or Usage and recursively copy all nested children, applying the same Ownership and attribute-value rules to each nested copy, when "a user duplicates a model element with the copy-nested-children option set to yes." | - | M |  |
| SSS-PA-ELEM-W4F | PA, PT | Mycelium Bloom shall duplicate only the selected Definition or Usage without copying any of its nested children when "a user duplicates a model element with the copy-nested-children option set to no." | - | M |  |
| SSS-PA-ELEM-B6J | PA, PT | Mycelium Bloom shall provide a setting to remember the user's last-used duplicate options as defaults for the next duplication when "a user accesses the duplication preferences." | - | M |  |
| SSS-PA-ELEM-W3N | PA, PT | Mycelium Bloom shall move a Usage from its current parent element to a different parent element, preserving all attributes, attribute values, and Ownership assignments, when "a user drags a Usage and drops it onto a different parent element in between the following views: the model browser, tabular browser or a diagram." | - | H |  |
| SSS-PA-ELEM-J6D | PA, PT | Mycelium Bloom shall delete a Definition or Usage and all its owned nested children when "a user deletes a model element and confirms the deletion.". Nested children that are owned by other Owners than the current Owner are deleted as well.  | - | H |  |
| SSS-PA-ELEM-V7K | PA, PT | Mycelium Bloom shall set the multiplicity (lower bound, upper bound) on any Usage when "a user edits the multiplicity of a Usage in the detail panel or on a diagram." | KerML 7.6.6 | H |  |
| SSS-PA-ELEM-N8P | PA, PT | Mycelium Bloom shall set the lower and upper bounds of a Multiplicity Range as either a literal non-negative integer, the unbounded symbol `*`, or an Expression referencing other Features, when "a user edits the bounds of a Multiplicity Range in the detail panel or on a diagram." | KerML 7.6.6 | H |  |
| SSS-PA-ELEM-O1Q | PA, PT | Mycelium Bloom shall set the `isOrdered` and `isUnique` flags of a Feature, controlling whether its values are ordered and whether duplicates are permitted, when "a user toggles the ordering or uniqueness of a Feature in the detail panel." | KerML 7.6.6 | H |  |
| SSS-PA-ELEM-U3G | PA, PT | Mycelium Bloom shall set whether a Usage is composite, meaning owned by its containing element, or referential, meaning a reference to an element owned elsewhere, when "a user marks a Usage as composite or reference." | KerML 7.6 | H |  |
| SSS-PA-ELEM-RU1 | PA, PT | Mycelium Bloom shall create a Reference Usage, a feature declared with the `ref` keyword that references an element owned elsewhere without composing it, when "a user creates a reference usage." | SysML 7.6 | M |  |
| SSS-PA-ELEM-RU2 | PA, PT, VW | Mycelium Bloom shall display whether a Usage is composite or referential using a distinguishing indicator, showing the `ref` keyword in the textual notation and a reference marker in the model browser, detail panel, and diagrams, when "a user views a Usage." | KerML 7.6 | M |  |
| SSS-PA-ELEM-CD7 | PA, PT | Mycelium Bloom shall prevent the creation or retyping of a composite Usage that would make a Definition a direct or transitive composite part of itself, and shall display an error identifying the resulting containment cycle, when "a user adds a composite Usage or assigns its type such that the composition hierarchy would become circular." | KerML 7.6 | H |  |
| SSS-PA-VIS-U7M | PA, PT, VW | Mycelium Bloom shall render the Multiplicity of a Feature or Usage in the model browser, tabular views, detail panel, and diagram labels using the textual notation `[lower..upper]`, displaying `*` for an unbounded upper, `[n]` when lower equals upper, and the expression text when a bound is an Expression, when "a user views an element that declares a Multiplicity." | KerML 7.6.6 | H |  |
| SSS-PA-ARCH-N5W | PA, PT | Mycelium Bloom shall create a Featuring relationship establishing that one type features another type when "a user explicitly specifies a featuring relationship between two types." | KerML 7.6 | M |  |
| SSS-PA-ELEM-D2N | PA, PT | Mycelium Bloom shall create a subsetting relationship between a feature and another feature of a compatible type when "a user designates a feature as a subset of another feature." | KerML 7.6.5 | H |  |
| SSS-PA-ELEM-H9W | PA, PT | Mycelium Bloom shall create a redefinition relationship where a feature in a specializing type replaces a feature inherited from a general type when "a user designates a feature as a redefinition of an inherited feature." | KerML 7.6.5 | H |  |
| SSS-PA-ELEM-M4J | PA, PT | Mycelium Bloom shall create a Specialization relationship between two Definitions, where the specializing Definition inherits all features of the general Definition and can add or redefine features, when "a user designates one Definition as a specialization of another." | KerML 7.6 | H |  |
| SSS-PA-ELEM-F3T | PA, PT | Mycelium Bloom shall create a FeatureTyping relationship between a Usage and its typing Definition when "a user sets or changes the type of a Usage via the detail panel or by dragging a Definition onto a Usage." | KerML 7.6.4 | H |  |
| SSS-PA-ELEM-R4S | PA, PT | Mycelium Bloom shall create a ReferenceSubsetting on a reference Feature of a Connection end, Flow end, or Interface end, identifying the participating Feature that the end refers to, when "a user sets the referent of a connector, flow, or interface end." | KerML 7.6.5 | H |  |
| SSS-PA-ELEM-C5X | PA, PT | Mycelium Bloom shall create a CrossSubsetting on a cross Feature of an Association Definition when "a user designates a Feature as the cross-feature of an Association between two participating Types." | KerML 7.6.5 | H |  |
| SSS-PA-ELEM-R6F | PA, PT, VW | Mycelium Bloom shall display the generalization/specialization hierarchy of a selected Definition, showing its general types and all its specializations, when "a user views the type hierarchy of a Definition." | KerML 7.6 | H |  |
| SSS-PA-ELEM-D8K | PA, PT, VW | Mycelium Bloom shall display, in the detail panel of any Type or Feature, all incoming and outgoing Specialization relationships grouped by kind (Subclassification, FeatureTyping, Subsetting, ReferenceSubsetting, Redefinition, CrossSubsetting, Conjugation) when "a user views the detail panel of a Type or Feature." | KerML 7.6 | H |  |
| SSS-PA-ELEM-J4K | PA, PT | Mycelium Bloom shall create a Conjugation relationship between two Types, designating one as the conjugating Type whose inherited Features have inverted directions relative to the original Type, when "a user designates a Type as the conjugate of another Type, either via the detail panel or via `~` notation when typing a Usage." | KerML 7.6.3 | H |  |
| SSS-PA-ELEM-L9P | PA, PT | Mycelium Bloom shall delete a Specialization of any concrete kind (Subclassification, FeatureTyping, Subsetting, ReferenceSubsetting, Redefinition, CrossSubsetting, Conjugation) when "a user selects a Specialization in the detail panel and invokes the 'Remove' action." | KerML 7.6 | H |  |
| SSS-PA-ELEM-M6N | PA, PT, VW | Mycelium Bloom shall display the inherited Features of a conjugating Type with their directions shown inverted relative to the original Type, `in` rendered as `out`, `out` rendered as `in`, `inout` preserved, in the detail panel, the model browser, and on diagrams, when "a user views a Type that is the conjugate of another Type." | KerML 7.6.3 | H |  |
| SSS-PA-ELEM-V3W | PA, PT, VW | Mycelium Bloom shall display, in the detail panel of the owning element, a warning for any Feature whose actual value count falls outside the literal bounds of its Multiplicity Range when "a user runs model validation or opens the detail panel of such a Feature." | KerML 7.6.6 | M |  |
| SSS-PA-ELEM-F4M | PA, PT, VW | Mycelium Bloom shall display the owned Features of a Type, derived from its FeatureMemberships, with their visibility, multiplicity, direction, and type, in the detail panel of the Type, when "a user views a Type that owns one or more Features." | KerML 7.6.1 | H |  |
| SSS-PA-ELEM-E5N | PA, PT, VW | Mycelium Bloom shall display the end Features of a Connector, Connection, Interaction, Association, or Flow Connection, derived from their EndFeatureMemberships, showing each end's referent Feature and multiplicity, in the detail panel, when "a user views a relationship element with end features." | KerML 7.13.2 | H |  |
| SSS-PA-ELEM-P6Q | PA, PT, VW | Mycelium Bloom shall display the parameter Features of an Action, Calculation, Function, or Case, derived from their ParameterMemberships, with each parameter's direction, type, and multiplicity, in the detail panel, when "a user views an Action, Calculation, Function, or Case." | KerML 7.12.3 | H |  |
| SSS-PA-ELEM-R7S | PA, PT, VW | Mycelium Bloom shall display the return Feature of a Function or Calculation, derived from its ReturnParameterMembership, with its type and multiplicity, in the detail panel, when "a user views a Function or Calculation." | KerML 7.12.3 | H |  |
| SSS-PA-ELEM-X8T | PA, PT, VW | Mycelium Bloom shall display the result expression of a Calculation or Boolean Expression, derived from its ResultExpressionMembership, in its textual form, in the detail panel, when "a user views a Calculation or Boolean Expression." | KerML 7.12.3 | H |  |
| SSS-PA-ELEM-N1G | PA, PT, VW | Mycelium Bloom shall reveal and select an element in the hierarchical browser when "a user invokes reveal-in-browser on an element selected in a tabular view or a diagram." | - | M |  |
| SSS-PA-ELEM-N2H | PA, PT, VW | Mycelium Bloom shall locate and select an element in an open tabular browser when "a user invokes locate-in-table on an element selected in another view." | - | M |  |
| SSS-PA-ELEM-N3J | PA, PT, VW | Mycelium Bloom shall reveal and select an element on every open diagram that contains it when "a user invokes show-on-diagram on an element selected in the browser or a tabular view." | - | M |  |
| SSS-PA-ELEM-N4K | PA, PT, VW | Mycelium Bloom shall navigate from a Usage to its defining Definition and from a Definition to its Usages when "a user invokes go-to-definition or find-usages on an element." | - | H |  |

###### 5.2.1.11.b Occurrences

An Occurrence Definition is a definition of a class of things that have an extent in time, called their lifetime, and that may have spatial extent. An Occurrence Usage is a usage of an occurrence definition. Items, parts, ports, actions, and states are all kinds of occurrences: ItemDefinition specialises OccurrenceDefinition, PartDefinition specialises ItemDefinition, and PortDefinition, ActionDefinition, and StateDefinition specialise OccurrenceDefinition, so each kind inherits the features and temporal semantics of its more general kind. An occurrence keeps its identity throughout its lifetime even though the values of its features may change over time. The lifetime of an occurrence may be partitioned into time slices that represent phases such as a deployment or an operational period, and a time slice of zero duration is a snapshot that represents the occurrence at a single instant. An occurrence definition or usage may also be restricted to an individual, a single real or perceived object with a unique identity, such as a specific car identified by its vehicle identification number. The requirements in this section cover the temporal aspects shared by all occurrence kinds. The behavioural occurrences (actions, states) and their notations are covered in the Behavior modeling section.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-OCC-H0 | PA, PT | Mycelium Bloom shall make the occurrence operations of this section (lifetime, time slices, snapshots, and individual designation) available on every occurrence kind, since ItemDefinition specialises OccurrenceDefinition, PartDefinition specialises ItemDefinition, and PortDefinition, ActionDefinition, and StateDefinition specialise OccurrenceDefinition, when "a user works with an item, part, port, action, or state." | SysML 7.9 | L |  |
| SSS-PA-OCC-D1 | PA, PT | Mycelium Bloom shall create an Occurrence Definition representing a class of things with a lifetime when "a user creates a new Occurrence Definition." | SysML 7.9 | L |  |
| SSS-PA-OCC-U2 | PA, PT | Mycelium Bloom shall create an Occurrence Usage typed by one or more Occurrence Definitions as a feature of a containing element when "a user adds an occurrence to an element." | SysML 7.9 | L |  |
| SSS-PA-OCC-L3 | PA, PT | Mycelium Bloom shall display and edit the lifetime of an occurrence, including its start and end and whether the lifetime is actual or projected, when "a user edits the lifetime of an occurrence." | SysML 7.9 | L |  |
| SSS-PA-OCC-T5 | PA, PT | Mycelium Bloom shall create a time slice of an occurrence representing a phase of its lifetime, and nest time slices within time slices, when "a user adds a time slice to an occurrence." | SysML 7.9 | L |  |
| SSS-PA-OCC-S6 | PA, PT | Mycelium Bloom shall create a snapshot of an occurrence or time slice, representing the occurrence at a start, end, or intermediate instant, when "a user adds a snapshot to an occurrence." | SysML 7.9 | L |  |
| SSS-PA-OCC-I7 | PA, PT | Mycelium Bloom shall define an Individual by restricting an Occurrence Definition to a single real or perceived object with a unique identity, and instantiate it as an Individual Usage representing a role the individual plays for a period, when "a user designates an occurrence as an individual." | SysML 7.9 | L |  |
| SSS-PA-OCC-V8 | PA, PT | Mycelium Bloom shall assign attribute values that differ across the time slices or snapshots of an occurrence, so that the condition of the occurrence can be specified at different points in its lifetime, when "a user sets an attribute value on a time slice or snapshot." | SysML 7.9 | L |  |
| SSS-PA-OCC-R9 | PA, PT, VW | Mycelium Bloom shall display the lifetime, time slices, snapshots, and individual status of an occurrence in the detail panel when "a user views an occurrence." | SysML 7.9 | L |  |

###### 5.2.1.11.c Items

An Item Definition is a kind of occurrence definition representing a class of identifiable objects that can be acted upon over time without necessarily performing actions themselves; an Item Usage is a usage of one or more Item Definitions. Items typically capture the inputs, outputs, and flows of a system, such as water, fuel, electrical signals, or data, that may flow through, be stored by, or be transported by the system, and an item may carry attributes, states, and nested item usages. An item that performs actions is normally modeled as a part: all parts are items, but not all items are necessarily parts. The same object, for example an engine, may be treated as an inert item or an active part at different stages of its lifetime.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-ARCH-B2D | PA, PT | Mycelium Bloom shall create an Item Definition representing a class of identifiable objects that may be acted upon over time, such as a data type, signal, or resource, when "a user creates a new Item Definition." | SysML 7.10 | H |  |
| SSS-PA-ARCH-B2F | PA, PT | Mycelium Bloom shall create an Item Usage typed by one or more Item Definitions as a feature of any Definition or Usage, representing an input, output, stored item, or flowing item, when "a user adds an item to a Definition or Usage." | SysML 7.10 | H |  |
| SSS-PA-ARCH-B2G | PA, PT | Mycelium Bloom shall nest an Item Usage within an Item Definition or Item Usage when "a user adds a nested item to an item." | SysML 7.10 | H |  |
| SSS-PA-ARCH-B2J | PA, PT, VW | Mycelium Bloom shall display, for an Item Usage, its typing Item Definitions and whether it is composite or referential, in the detail panel, when "a user views an Item Usage." | SysML 7.10 | H |  |
| SSS-PA-ARCH-B2K | PA, PT | Mycelium Bloom shall change the kind of a usage between item and part, retyping it with a compatible Part Definition when an Item Usage becomes a Part Usage, while preserving its name, nested features, and references, when "a user changes the kind of a usage between item and part." | SysML 7.11 | H |  |

###### 5.2.1.11.d Parts

A Part Definition represents a modular unit of structure, such as a system, a system component, or an external entity that may interact with the system. A Part Definition is a kind of Item Definition, so it defines a class of part objects that are occurrences with temporal and possibly spatial extent, while a Part Usage is a usage of one or more Part Definitions (and may also use item definitions that are not parts, allowing the same element to be treated as an item in some situations, for example an engine flowing along an assembly line, and as a part in others, for example that engine once installed in a vehicle). A system is modeled as a composite part whose part usages may themselves have further composite structure. Parts may carry attributes representing performance, physical, and other quality characteristics, expose ports that define where they interconnect, perform actions that cause items to flow across their connections, and exhibit states that enable different actions. A part can represent any level of abstraction, from a purely logical component to a physical component with a part number, and may model hardware, software, facilities, organizations, or users of a system.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-ARCH-JQH | PA, PT | Mycelium Bloom shall create a Part Definition as a reusable building block when "a user creates a new Part Definition." | SysML 7.11 | H |  |
| SSS-PA-ARCH-TB2 | PA, PT | Mycelium Bloom shall create a nested Part Usage within the selected parent Part, extending the system decomposition hierarchy (e.g. System, Subsystem, Equipment, Component), when "a user adds a child part to an existing part in the model hierarchy." | SysML 7.11 | H |  |
| SSS-PA-ARCH-PR4 | PA, PT | Mycelium Bloom shall instantiate the same Part Definition as multiple Part Usages in one or more containing parts, so that a single definition is reused across the system structure, when "a user instantiates an existing Part Definition more than once." | SysML 7.11 | H |  |
| SSS-PA-ARCH-PT5 | PA, PT | Mycelium Bloom shall create a Part Usage typed by one or more Part Definitions, and optionally by item definitions that are not part definitions, as a feature of a containing part, when "a user adds a part to a containing part." | SysML 7.11 | H |  |
| SSS-PA-ARCH-PD6 | PA, PT, VW | Mycelium Bloom shall display the composite decomposition of a selected Part, showing its nested Part Usages, their multiplicities, and their typing Part Definitions, when "a user views the structure of a Part." | SysML 7.11 | M |  |

###### 5.2.1.11.e Ports

A Port Definition is a kind of occurrence definition that defines a connection point enabling interactions between occurrences, most commonly parts, and a Port Usage is a usage of a Port Definition. A port usage may be connected to one or more other port usages, and these connections enable interactions between the occurrences that own the ports, with the features of the port usages (whether inherited from the definition or declared locally) specifying what can be exchanged. Because ports are themselves occurrences, port definitions and usages can contain nested port usages. A feature of a port may be directed as in, out, or inout, and flows nested in a connection between ports model transfers between matching directed features, where two features match if they have conforming definitions and either both have no direction or they have conjugate directions (the conjugate of in is out and vice versa, while inout is its own conjugate). A transfer can occur from the out features of one port to the matching in features of connected ports, and in both directions between matching inout features. Two ports conform when each feature of one port has a matching feature on the other, so that a connection allows a flow between every directed feature and its match. Each Port Definition also has a conjugated Port Definition whose directed features are reversed, and a conjugated Port Usage automatically conforms to a usage of the corresponding original Port Definition.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-ARCH-5RR | PA, PT | Mycelium Bloom shall create a Port Definition when "a user creates a new Port Definition." | SysML 7.12 | H |  |
| SSS-PA-ARCH-PU8 | PA, PT | Mycelium Bloom shall create a Port Usage on a part, typed by a Port Definition, when "a user adds a port to a part." | SysML 7.12 | H |  |
| SSS-PA-ARCH-PF1 | PA, PT | Mycelium Bloom shall define the features of a Port, representing the items, attributes, or signals that can be exchanged, each with a direction of in, out, or inout, when "a user adds or edits a feature of a Port Definition or Port Usage." | SysML 7.12 | H |  |
| SSS-PA-ARCH-PN2 | PA, PT | Mycelium Bloom shall nest a Port Usage within a Port Definition or Port Usage, forming a compound port, when "a user adds a nested port to a port." | SysML 7.12 | M |  |
| SSS-PA-ARCH-K7M | PA, PT | Mycelium Bloom shall create a conjugated Port Usage with reversed feature directions (in becomes out, out becomes in) when "a user designates a Port Usage as the conjugate of an existing Port Definition." | SysML 7.12 | M |  |
| SSS-PA-ARCH-PV3 | PA, PT, VW | Mycelium Bloom shall display, for a Port, its directed features with their directions and whether it is a conjugate of another Port, in the detail panel, when "a user views a Port." | SysML 7.12 | M |  |

###### 5.2.1.11.f Connections

A Connection Definition is both a relationship and a kind of Part Definition that classifies connections between related things, such as items and parts. Unless it is abstract, a connection definition has at least two connection ends, which specify the things being related, and a connection with exactly two ends is a binary connection. Any other features of a connection definition characterize the connection itself, separately from the connected things, and because a connection is a part, those values may change over the lifetime of the connection while the connected ends do not. A Connection Usage is a part usage of a connection definition that connects specific usage elements, such as item and part usages, by redefining the
connection ends to associate them with the particular usages to be connected. A connection usage between parts is often a logical connection that abstracts away how the parts are physically connected, but it can also be refined into a physical connection by modeling the connecting medium itself as a part.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-ARCH-CD1 | PA, PT | Mycelium Bloom shall create a Connection Definition that classifies connections between related things, with at least two connection ends, when "a user creates a new Connection Definition." | SysML 7.13 | H |  |
| SSS-PA-ARCH-IGA | PA, PT | Mycelium Bloom shall create a Connection Usage that connects two or more participating part or item usages, associating each of its connection ends with a participating usage, when "a user selects two compatible elements and creates a connection." | SysML 7.13 | H |  |
| SSS-PA-ARCH-Y2D | PA, PT | Mycelium Bloom shall create a Binding Connector that asserts equality between two compatible features of model elements when "a user selects two features and creates a binding between them." | KerML 7.13.3 | H |  |
| SSS-PA-ARCH-CR9 | PA, PT | Mycelium Bloom shall refine a logical Connection Usage into a physical connection by modeling the connecting medium as a part and routing the connection through it, when "a user converts a logical connection into a physical connection." | SysML 7.13 | M |  |

###### 5.2.1.11.g Interfaces

An Interface Definition is a kind of Connection Definition whose ends are restricted to port definitions, and an Interface Usage is a usage of an interface definition whose ends are restricted to port usages. In other words, an interface is simply a connection all of whose ends are ports, which lets compatible connections between parts be specified once and reused. For example, a Power interface between an appliance and wall power exposes a power port on one end and an outlet port on the other, and the same interface can connect many different appliances to wall power. When modeling physical interactions, an interface definition or usage may carry constraints on the features of its port ends.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-ARCH-ID1 | PA, PT | Mycelium Bloom shall create an Interface Definition whose ends are restricted to port definitions, when "a user creates a new Interface Definition." | SysML 7.14 | H |  |
| SSS-PA-ARCH-IU2 | PA, PT | Mycelium Bloom shall create an Interface Usage connecting two or more ports by associating its port ends with the participating port usages, when "a user selects two compatible ports and creates an interface." | SysML 7.14 | H |  |
| SSS-PA-ARCH-IR3 | PA, PT | Mycelium Bloom shall instantiate the same Interface Definition as multiple Interface Usages connecting different pairs of ports, so that a single interface specification is reused across the model, when "a user instantiates an existing Interface Definition more than once." | SysML 7.14 | M |  |
| SSS-PA-ARCH-IC4 | PA, PT | Mycelium Bloom shall add constraints to an Interface Definition or Usage that relate the features of its port ends, such as conservation laws across the interface, when "a user adds a constraint to an interface." | SysML 7.14 | H |  |

###### 5.2.1.11.h Attributes

An Attribute Definition defines a set of data values, such as numbers, quantitative values with units, qualitative values such as text strings, or data structures of such values, and an Attribute Usage is a usage of an attribute definition. An attribute usage is always referential, as are any of its nested features, and its values are constrained to the range specified by its definition, while an Enumeration Definition is a specialised attribute definition that restricts the values to a discrete set. Attribute usages may be typed by SysML attribute definitions or by KerML primitive data types such as String, Boolean, Integer, and Real, whereas quantities with units are defined using the SysML Quantities and Units Domain Library or extensions of it. A guiding principle is that only the kind of unit, for example mass or length, is associated with the attribute definition, while a specific unit, for example kilograms or metres, is given only with an actual value, so that an attribute is independent of the units used and values convert automatically between units of the same kind. The values of an attribute usage do not themselves change over time, but when the attribute is owned by an occurrence such as an item, part, or action, its value may differ at different points in that occurrence's lifetime.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-ARCH-AD7 | PA, PT | Mycelium Bloom shall create an Attribute Definition typed by a quantity kind or data type, with its associated measurement unit where applicable, when "a user creates a new Attribute Definition." | SysML 7.7 | H |  |
| SSS-PA-ARCH-97Z | PA, PT | Mycelium Bloom shall create an Attribute Usage on any Definition or Usage, typed by an Attribute Definition or a primitive data type and expressed with its measurement unit where applicable, when "a user adds an attribute to a Definition or Usage, irrespective of the assigned Ownership of the target Definition or Usage." | SysML 7.7 | H |  |
| SSS-PA-ARCH-AV5 | PA, PT | Mycelium Bloom shall set the value of an Attribute Usage, expressed with its measurement unit where applicable, when "a user edits an attribute value." | SysML 7.7 | H |  |
| SSS-PT-DATA-OH2 | PA, PT | Mycelium Bloom shall override an attribute value on a specific element usage without changing the parent definition when "a user edits an attribute value on a usage that inherits from a definition." | KerML 7.6 | H |  |
| SSS-PT-DATA-492 | PA, PT | Mycelium Bloom shall assign attribute values that vary by exhibited State Usage (e.g. operational mode) when "a user associates attribute values with specific states on an element." | SysML 7.18 | H |  |
| SSS-PT-DATA-D5I | PT | Mycelium Bloom shall provide a selector to set attribute value sources as Manual (hand-entered), Computed (calculated), or Reference (sourced from another element) when "the Participant edits an attribute value." (TBC) | SysML 7.7 | M |  |

##### 5.2.1.12 Variation point and variant modeling

Early-phase design explores a family of possible solutions before committing to one. In SysML v2 this is modelled with variation. A variation, sometimes called a variation point, is any Definition or Usage, except an enumeration, that is designated as a point which can vary from one design configuration to another, and its alternatives are called variants. For example, the engine of a vehicle may be a variation whose variants are a four-cylinder engine and a six-cylinder engine. Variations can be nested to any depth, and constraints can restrict which variants may be chosen together, so that the model forms a superset from which a complete configuration is obtained by selecting one variant per variation. Mycelium offers two complementary mechanisms for exploring alternatives: Branches for fully independent design alternatives, and variation points and variants for in-place variability within a single branch. The requirements in this section cover designating variation points, managing their variants, selecting and resolving configurations, and comparing alternatives.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-VAR-K3T | PA, PT | Mycelium Bloom shall mark any Definition or Usage, except an Enumeration Definition, as a variation point by setting `isVariation = true` when "a user designates an element as a variation point." | SysML 7.6 | H |  |
| SSS-PA-VAR-R7W | PA, PT | Mycelium Bloom shall add a variant to a variation point, either by creating a new variant Usage or by referencing an existing Usage defined elsewhere, when "a user adds a variant to an existing variation point." | SysML 7.6 | H |  |
| SSS-PA-VAR-N5D | PA, PT | Mycelium Bloom shall remove a variant Usage from a variation point when "a user deletes a variant from a variation point." | SysML 7.6 | H |  |
| SSS-PA-VAR-H2J | PA, PT, VW | Mycelium Bloom shall visually distinguish variation points from regular Usages in the model browser, tabular view and on diagrams using a distinct indicator when "the model contains Usages with `isVariation = true`." | SysML 7.6 | H |  |
| SSS-PA-VAR-M8F | PA, PT, VW | Mycelium Bloom shall display all variant Usages nested under a variation point as selectable alternatives when "a user expands a variation point in the model browser." | SysML 7.6 | H |  |
| SSS-PA-VAR-D4B | PA, PT | Mycelium Bloom shall select an active variant for a variation point, filtering the model browser, tabular view, diagrams, and calculations to show only the selected variant's content, when "a user selects a variant from the variant selector of a variation point." | SysML 7.6 | H |  |
| SSS-PA-VAR-W6N | PA, PT, VW | Mycelium Bloom shall display a side-by-side comparison of attribute values across all variants of a variation point when "a user opens the variant comparison view for a variation point." | SysML 7.6 | M |  |
| SSS-PA-VAR-J9K | PA, PT | Mycelium Bloom shall propagate structural changes (added attributes, ports, nested usages) from the variation point to all its variants when "a user modifies the shared structure of a variation point." | SysML 7.6 | H |  |
| SSS-PA-VAR-F1P | PA, PT | Mycelium Bloom shall override attribute values on individual variant Usages without affecting other variants or the variation point definition when "a user edits an attribute value on a specific variant." | SysML 7.6 | H |  |
| SSS-PA-VAR-B3G | PA, PT, VW | Mycelium Bloom shall indicate in the Browser View which variant is active for each variation point using visual markers when "the model contains variation points with a selected active variant." | SysML 7.6 | H |  |
| SSS-PA-VAR-T6L | PA, PT, VW | Mycelium Bloom shall display the system structure rooted at a variation point with the decomposition of each variant shown side-by-side or in switchable tabs when "a user opens the variant decomposition tree view for a variation point." | SysML 7.6 | M |  |
| SSS-PA-VAR-E2Q | PA, PT, VW | Mycelium Bloom shall highlight structural differences (added, removed, or changed elements and attributes) between variants in the per-variation-point decomposition tree view when "two or more variants are displayed for comparison." | SysML 7.6 | M |  |
| SSS-PA-VAR-G8X | PA, PT | Mycelium Bloom shall provide a configuration selector where the user selects one active variant for each reachable variation point, revealing a nested variation point's choices only once its containing variant is selected, to define a complete system configuration, when "the model contains one or more variation points." | SysML 7.6 | H |  |
| SSS-PA-VAR-C5H | PA, PT, VW | Mycelium Bloom shall display a resolved decomposition tree showing the full system decomposition with only the selected variants included, as if the configuration were the actual design, when "a user applies a variant configuration." | SysML 7.6 | H |  |
| SSS-PA-VAR-NV1 | PA, PT | Mycelium Bloom shall create a variation point nested within a variant, to any level of nesting, when "a user designates an element inside a variant as a further variation point." | SysML 7.6 | H |  |
| SSS-PA-VAR-CC2 | PA, PT | Mycelium Bloom shall create constraints that restrict which variants may be selected together across variation points, for example a six-cylinder engine requiring an automatic transmission, when "a user adds a configuration constraint between variants." | SysML 7.6 | H |  |
| SSS-PA-VAR-CV3 | PA, PT, VW | Mycelium Bloom shall evaluate whether a selected configuration satisfies all configuration constraints and indicate any invalid or conflicting selections when "a user selects variants to define a configuration." | SysML 7.6 | H |  |
| SSS-PA-VAR-RR4 | PA, PT | Mycelium Bloom shall compute the resolved model for a selected configuration by including only the selected variant at each variation point, applying that variant's redefinitions, recursively resolving nested variations, and resolving variants declared by reference, when "a user applies a variant configuration." | SysML 7.6 | H |  |
| SSS-PA-VAR-NC5 | PA, PT | Mycelium Bloom shall treat a configuration as incomplete until a variant is selected for every reachable variation point, and shall indicate the variation points still requiring a selection, when "a user configures a model containing nested variation points." | SysML 7.6 | M |  |
| SSS-PA-VAR-AC6 | PA, PT | Mycelium Bloom shall enumerate the valid configurations of the model by generating every combination of one variant per reachable variation point that satisfies all configuration constraints, and shall report the total count, when "a user requests all valid configurations." | SysML 7.6 | M |  |
| SSS-PA-VAR-AC7 | PA, PT | Mycelium Bloom shall report the number of valid configurations without materialising them all, and shall generate or resolve individual configurations on demand, when "the number of valid configurations exceeds a configurable threshold." | SysML 7.6 | M |  |
| SSS-PA-VAR-SC8 | PA, PT | Mycelium Bloom shall save a configuration as a named, persistent element in the model that specialises the variation-bearing root and redefines each reachable variation point to its selected variant, when "a user saves the current configuration." | SysML 7.6 | M |  |
| SSS-PA-VAR-SC9 | PA, PT | Mycelium Bloom shall list, rename, apply, compare, and delete saved configurations, when "a user manages the project's saved configurations." | SysML 7.6 | M |  |
| SSS-PA-VAR-SCA | PA, PT | Mycelium Bloom shall restore the user's most recently applied configuration selection when "a user reopens a project that contains variation points." | SysML 7.6 | L |  |

##### 5.2.1.13 Allocations and relationships

An allocation is a mapping across the structures and hierarchies of a system model, asserting that a target element is responsible for realising some or all of the intent of a source element, for example a function allocated to a component. Beyond allocation, Mycelium supports the generic KerML and SysML v2 relationship constructs (typed relationships, dependencies, and external relationships) and a Relationship Matrix for visualising and editing relationships of any type across sets of elements. The requirements in this section cover allocation, generic relationships, and the matrix view. Requirement-specific trace relationships such as Satisfy, Derive, and Verify are covered in the Requirements modeling section.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-TRACE-AD1 | PA, PT | Mycelium Bloom shall create an Allocation Definition specifying that a target element realises the intent of a source element when "a user creates a new Allocation Definition." | SysML 7.15 | H |  |
| SSS-PA-TRACE-YWQ | PA, PT | Mycelium Bloom shall create an Allocation Usage, typed by one or more Allocation Definitions, that maps a source element to a target element responsible for realising it, when "a user selects source and target elements and creates an allocation." | SysML 7.15 | H |  |
| SSS-PA-TRACE-NA2 | PA, PT | Mycelium Bloom shall refine an Allocation Usage with nested Allocation Usages that decompose it into finer-grained mappings when "a user adds a nested allocation to an allocation." | SysML 7.15 | M |  |
| SSS-PA-TRACE-AR4 | PA, PT, VW | Mycelium Bloom shall render an Allocation Usage on a diagram as a dashed arrow labelled «allocate» from the source element to the target element, using the SysML v2 graphical notation (section 8.2.3.15), when "a diagram displays an allocation between elements it contains." | SysML 8.2.3.15 | M |  |
| SSS-PA-TRACE-AP5 | PA, PT, VW | Mycelium Bloom shall display, in the detail panel of an element, the allocations in which it participates as source or target, each navigable to the other end and to its typing Allocation Definition, when "a user views an element that participates in one or more allocations." | SysML 7.15 | M |  |
| SSS-PA-TRACE-IKS | PA, PT, VW | Mycelium Bloom shall display a Relationship Matrix showing binary relationships between element sets (e.g. requirements vs. parts) when "a user opens the Relationship Matrix view and selects the element sets and relationship type." | - | H |  |
| SSS-PA-TRACE-V3H | PA, PT, VW | Mycelium Bloom shall populate the Relationship Matrix rows and columns from user-selected element types, packages, or query results when "a user configures the row source and column source of a Relationship Matrix." | - | H |  |
| SSS-PA-TRACE-K7W | PA, PT, VW | Mycelium Bloom shall indicate the presence and direction of relationships in each matrix cell using visual markers (e.g. filled cell, arrow, relationship count) when "the Relationship Matrix renders cells where relationships exist between the row and column elements." | - | H |  |
| SSS-PA-TRACE-D2R | PA, PT | Mycelium Bloom shall create a relationship of the selected type between the row element and the column element when "a user clicks an empty cell in the Relationship Matrix." | - | H |  |
| SSS-PA-TRACE-J8N | PA, PT | Mycelium Bloom shall delete the relationship between the row element and the column element when "a user removes a relationship from an occupied cell in the Relationship Matrix." | - | H |  |
| SSS-PA-TRACE-F5M | PA, PT, VW | Mycelium Bloom shall filter the Relationship Matrix by relationship type, Ownership, Applied MetaDataUsage or element type when "a user applies filters to the Relationship Matrix." | - | H |  |
| SSS-PA-TRACE-W9G | PA, PT, VW | Mycelium Bloom shall sort the Relationship Matrix rows and columns by element name, namespace path, or relationship count when "a user changes the sort order of the Relationship Matrix." | - | H |  |
| SSS-PA-TRACE-B6C | PA, PT, VW | Mycelium Bloom shall display multiple relationship types simultaneously in the Relationship Matrix using distinct visual markers per type when "a user selects more than one relationship type for display." | - | M |  |
| SSS-PA-TRACE-H4P | PA, PT, VW | Mycelium Bloom shall navigate to the detail panel of the related elements when "a user double-clicks an occupied cell in the Relationship Matrix." | - | H |  |
| SSS-PA-TRACE-R1X | PA, PT, VW | Mycelium Bloom shall export the Relationship Matrix to CSV and PDF formats when "a user initiates an export from the Relationship Matrix view." | - | M |  |
| SSS-PA-TRACE-8ZB | PA, PT | Mycelium Bloom shall create a typed relationship between any two model elements when "a user selects source and target elements and specifies a relationship type." | KerML 7.8 | H |  |
| SSS-PA-TRACE-V8K | PA, PT | Mycelium Bloom shall create a Dependency relationship between two model elements, asserting that the source element depends on the target element, when "a user creates a generic dependency between two model elements." | KerML 7.3 | H |  |
| SSS-CC-EXT-5DV | PA, PT | Mycelium Bloom shall support External Relationships linking model elements to external web resources via IRIs when "a user creates a relationship targeting an external resource identified by an IRI." | SysML 7.3 | M |  |
| SSS-PA-TRACE-RX1 | PA, PT, VW | Mycelium Bloom shall display, in the detail panel of an element, the typed relationships, dependencies, and featurings in which it participates, each navigable to the related element, when "a user views an element that participates in relationships." | KerML 7.3 | M |  |

##### 5.2.1.14 Quantities, units, and measurement management

Numerical engineering values must always be expressed with a quantity kind, a measurement unit, and a measurement scale. The SysML v2 Quantities and Units Domain Library provides a normative model of these concepts as Attribute Definitions and Attribute Usages. Mycelium presents this library as user-friendly browsers for quantities, units, and scales, with drag-and-drop assignment of attributes to elements and the ability to import standard or custom libraries.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-QU-T3K | PA, PT, VW | Mycelium Bloom shall provide a tabular view listing all Attribute Definitions available in the project (including those imported from libraries) with their name, quantity kind, default unit, and source library when "a user opens the Attribute Definitions browser." | SysML 9.8 | H |  |
| SSS-PA-QU-R7N | PA, PT, VW | Mycelium Bloom shall provide a tabular view listing all Measurement Units available in the project with their name, symbol, unit type (simple, derived, prefixed), and associated quantity kind when "a user opens the Measurement Units browser." | SysML 9.8.3 | H |  |
| SSS-PA-QU-W5J | PA, PT, VW | Mycelium Bloom shall provide a tabular view listing all Measurement Scales available in the project with their name, scale type (ratio, interval, ordinal, cyclic ratio, logarithmic), unit, and value range when "a user opens the Measurement Scales browser." | SysML 9.8.3 | H |  |
| SSS-PA-QU-D8M | PA, PT, VW | Mycelium Bloom shall provide a tabular view listing all Quantity Kinds available in the project with their name, dimension symbol, and classification (base, derived, specialized) when "a user opens the Quantity Kinds browser." | SysML 9.8.2 | H |  |
| SSS-PA-QU-H2V | PA, PT | Mycelium Bloom shall provide operations to create, edit, and delete custom Attribute Definitions typed by a Quantity Kind with an associated Measurement Unit when "a user accesses the Attribute Definitions management interface of a project or a library." | SysML 9.8 | H |  |
| SSS-PA-QU-K6F | PA, PT | Mycelium Bloom shall provide operations to create, edit, and delete custom Measurement Units (simple, derived, prefixed) with conversion factors when "a user accesses the Measurement Units management interface of a project or a library." | SysML 9.8.3 | H |  |
| SSS-PA-QU-B4P | PA, PT | Mycelium Bloom shall provide operations to create, edit, and delete Measurement Scales (ratio, interval, ordinal, cyclic ratio, logarithmic) with their associated unit and value constraints when "a user accesses the Measurement Scales management interface of a project or a library." | SysML 9.8.3 | H |  |
| SSS-PA-QU-QK1 | PA, PT | Mycelium Bloom shall provide operations to create, edit, and delete custom Quantity Kinds (simple, specialized, derived) with their dimension when "a user accesses the Quantity Kinds management interface of a project or a library." | SysML 9.8.2 | H |  |
| SSS-PA-QU-CV2 | PA, PT | Mycelium Bloom shall create an Attribute Definition for a vector or tensor quantity value, and a compound Attribute Definition composed of named component attributes of possibly different quantity kinds (for example a coordinate with x, y, and z components), when "a user defines a multi-component or structured attribute." | SysML 9.8 | M |  |
| SSS-PA-QU-N9X | PA, PT | Mycelium Bloom shall create an Attribute Usage typed by the dropped Attribute Definition on the target element when "a user drags an Attribute Definition from the Attribute Definitions browser and drops it onto an element Definition or Usage in the model browser or a diagram." | SysML 7.7 | H |  |
| SSS-PA-QU-G1W | PA, PT | Mycelium Bloom shall import Quantity Kinds, Measurement Units, Measurement Scales, and Attribute Definitions from the SysML v2 standard libraries (ISQ, SI, USCustomary) and from Mycelium Forge packages when "a user selects library content for import into a project." | SysML 9.8 | H |  |
| SSS-PA-QU-UC3 | PA, PT, VW | Mycelium Bloom shall display and edit a quantity value in any Measurement Unit compatible with its Quantity Kind, converting between units of the same kind, when "a user selects a display or input unit for a quantity value." | SysML 9.8.3 | M |  |

##### 5.2.1.15 Enumerations

An Enumeration Definition is a value type whose instances are restricted to a fixed set of named literals, the non-numeric counterpart to the quantity-kind-and-unit typing of numeric attributes. Engineers use enumerations to constrain an attribute to a controlled vocabulary (e.g. operational mode, criticality class). The requirements in this section cover defining enumerations and their literals, displaying them, constraining attribute values to them, and validating those values.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-ARCH-9W5 | PA, PT | Mycelium Bloom shall create an Enumeration Definition that restricts its values to a fixed set of enumerated values when "a user creates an Enumeration Definition and specifies its literals." | SysML 7.8 | H |  |
| SSS-PA-ARCH-E1A | PA, PT | Mycelium Bloom shall add, rename, and remove Enumeration Literals of an Enumeration Definition, with optional Documentation per literal, when "a user edits the literal set of an Enumeration Definition in the detail panel." | SysML 7.8 | H |  |
| SSS-PA-ARCH-E2B | PA, PT | Mycelium Bloom shall reorder the Enumeration Literals of an Enumeration Definition when "a user changes the position in the literal list." | SysML 7.8 | H |  |
| SSS-PA-VIS-E3C | PA, PT, VW | Mycelium Bloom shall display the literal set of an Enumeration Definition in its detail panel, showing each literal's `name`, its ordinal position, and its Documentation, when "a user views an Enumeration Definition." | SysML 7.8 | H |  |
| SSS-PA-ARCH-E4D | PA, PT | Mycelium Bloom shall present the allowed literals of the typing Enumeration Definition as a dropdown selection when "a user edits the value of an Attribute Usage whose type is an Enumeration Definition." | SysML 7.8 | H |  |
| SSS-PA-ARCH-E6G | PA, PT | Mycelium Bloom shall create an Enumeration Definition that specialises an Attribute Definition, where each enumerated value binds a value of the specialised attribute (for example allowed diameters of 60, 80, and 100 mm specialising a length attribute), when "a user defines an enumeration over the values of an attribute definition." | SysML 7.8 | M |  |

##### 5.2.1.16 Behavior modeling

Beyond structure, systems exhibit behavior: actions performed, states held, transitions triggered, flows of items and data. SysML v2 provides Action Definitions, State Definitions, and Flow Connections. The requirements in this section cover the behavioral modeling capabilities engineers need to describe what the system does and how its behavior depends on context. Subsections cover actions, states, flows, and performing and exhibiting behaviour on parts.

###### 5.2.1.16.a Actions

Actions define what a system does. An Action Definition specifies a behaviour with input and output parameters that can be decomposed into sub-actions and sequenced by control flow. Mycelium covers action definitions, the control nodes (succession, guard, fork, join, decision, merge), and the primitive and structured action nodes (accept, send, assignment, if, while, for). The requirements in this subsection cover defining actions, composing their control flow, and the individual action node kinds, together with server-side validation of action well-formedness.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-BEH-N5Z | PA, PT | Mycelium Bloom shall create an Action Definition with input and output parameters and decompose it into sub-actions when "a user creates or edits an Action Definition." | SysML 7.17 | H |  |
| SSS-PA-BEH-WG5 | PA, PT | Mycelium Bloom shall create control flow between actions using successions, guards, forks, joins, decisions, and merges when "a user creates control flow elements between existing actions." | SysML 7.17 | H |  |
| SSS-PA-BEH-Q4N | PA, PT | Mycelium Bloom shall create a generic Succession between two features (e.g. two actions, two states, or two arbitrary occurrences) establishing that the second feature follows the first when "a user creates a succession between two features outside the context of a state machine." | KerML 7.13.5 | H |  |
| SSS-PA-BEH-A1C | PA, PT | Mycelium Bloom shall create an Accept Action Usage that waits for an incoming payload matching a specified trigger Feature, optionally typed and guarded, when "a user adds an accept-action node to an Action Flow View or to an Action Definition in the detail panel." | SysML 7.17.5 | H |  |
| SSS-PA-BEH-S2N | PA, PT | Mycelium Bloom shall create a Send Action Usage that emits a payload Expression to a target Feature when "a user adds a send-action node to an Action Flow View or to an Action Definition in the detail panel." | SysML 7.17.5 | H |  |
| SSS-PA-BEH-A3S | PA, PT | Mycelium Bloom shall create an Assignment Action Usage that assigns the value of a source Expression to a target Feature when "a user adds an assignment-action node to an Action Flow View or to an Action Definition in the detail panel." | SysML 7.17.5 | H |  |
| SSS-PA-BEH-I4F | PA, PT | Mycelium Bloom shall create an If Action Usage composed of a Boolean condition Expression, a then-branch Action Usage, and an optional else-branch Action Usage when "a user adds an if-action to an Action Flow View or to an Action Definition." | SysML 7.17.5 | H |  |
| SSS-PA-BEH-W5H | PA, PT | Mycelium Bloom shall create a While Loop Action Usage composed of a Boolean condition Expression and a body Action Usage that executes as long as the condition holds when "a user adds a while-loop to an Action Flow View or to an Action Definition." | SysML 7.17.5 | H |  |
| SSS-PA-BEH-F6L | PA, PT | Mycelium Bloom shall create a For Loop Action Usage composed of a loop-variable Feature, a collection Expression, and a body Action Usage that executes once for each element of the collection when "a user adds a for-loop to an Action Flow View or to an Action Definition." | SysML 7.17.5 | H |  |

###### 5.2.1.16.b States

A State Definition models the conditions or modes a system holds over time, each with entry, do, and exit behaviour and transitions to other states. Mycelium supports composite states with nested states and parallel (orthogonal) regions, transitions of every kind with triggers, guards, and effects, and validation of state-machine well-formedness. The requirements in this subsection cover defining state machines, their states and transitions, and how they are displayed and validated.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-BEH-RPK | PA, PT | Mycelium Bloom shall create a State Definition with entry, do, and exit actions and connect its states via Transition Usages with triggers, guards, and effects when "a user creates or edits a State Definition." | SysML 7.18 | H |  |
| SSS-PA-BEH-SD1 | PA, PT | Mycelium Bloom shall designate one State Usage inside a composite State Definition as the default (initial) state entered when the containing state is entered, when "a user marks a State Usage as the default state of its parent State Definition." | SysML 7.18 | H |  |
| SSS-PA-BEH-SC2 | PA, PT | Mycelium Bloom shall define nested State Usages within a State Definition, producing a composite state machine in which each nested State Usage has its own entry, do, and exit Action, when "a user adds a child State Usage to a State Definition." | SysML 7.18 | H |  |
| SSS-PA-BEH-SP3 | PA, PT | Mycelium Bloom shall define parallel regions (orthogonal state machines) inside a State Definition, each with its own set of State Usages and Transition Usages, when "a user adds one or more parallel regions to a State Definition." | SysML 7.18 | H |  |
| SSS-PA-BEH-SE4 | PA, PT | Mycelium Bloom shall attach, replace, and remove an Entry Action, a Do Action, and an Exit Action on any State Usage, each realised as an Action Usage owned via the corresponding Feature Membership, when "a user edits the entry, do, or exit behavior of a State Usage." | SysML 7.18 | H |  |
| SSS-PA-BEH-TR5 | PA, PT | Mycelium Bloom shall create a Transition Usage of any of the following kinds: normal (between distinct source and target states), self (source and target are the same state), internal (no state exit or entry), or completion (no trigger, fires when the source state's Do Action completes), when "a user creates a transition in a State Transition View or via the detail panel." | SysML 7.18 | H |  |
| SSS-PA-BEH-TG6 | PA, PT | Mycelium Bloom shall set the trigger (an Accept Action Usage), the guard (a Boolean Expression), and the effect (an Action Usage) of a Transition Usage when "a user edits the trigger, guard, or effect of a Transition Usage." | SysML 7.18 | H |  |
| SSS-PA-VIS-SH7 | PA, PT, VW | Mycelium Bloom shall display the state-machine structure of a State Definition in its detail panel, showing the default state, the nested State Usages, the parallel regions, the Entry, Do, and Exit Actions on each State Usage, and the outgoing Transition Usages with their triggers, guards, and effects, when "a user views a State Definition that owns at least one State Usage or Transition Usage." | SysML 7.18 | H |  |
| SSS-FB-BEH-SV8 | - | Mycelium Fabric shall return a validation warning identifying any State Usage that is unreachable from the default state of its owning State Definition, and any State Usage that has two or more outgoing Transition Usages with the same trigger and an overlapping guard, when "a client runs model validation or submits a commit containing a State Definition." | SysML 7.18 | H |  |

###### 5.2.1.16.c Flows

A flow models the transfer of items, energy, or data between parts. SysML v2 expresses this with Flow Connections and, where ordering matters, Succession Item Flows. The requirements in this subsection cover defining flow connections and the sequenced item flows used, for example, to convey messages between lifelines in a Sequence View.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-BEH-PC7 | PA, PT | Mycelium Bloom shall create a Flow Connection Definition and instantiate it as a Flow Connection Usage to model the transfer of items, energy, or data between parts when "a user creates a Flow Connection Definition and specifies the flow type and endpoints." | SysML 7.16 | H |  |
| SSS-PA-BEH-X9V | PA, PT | Mycelium Bloom shall create a Succession Item Flow that conveys items between two features and establishes that the receiving end occurs after the sending end when "a user creates a sequenced flow between two features (e.g. a message between lifelines in a Sequence View, or an ordered item transfer between actions)." | KerML 7.13.6 | H |  |
| SSS-PA-BEH-D6L | PA, PT | Mycelium Bloom shall create the corresponding Succession Item Flow in the underlying model when "a user draws a message arrow between two lifelines in a Sequence View." | KerML 7.13.6, SysML 8.2.3.9 | H |  |

###### 5.2.1.16.d Performing and exhibiting behaviour

Behaviour is connected to structure by performing actions and exhibiting states on the parts that carry them. The requirements in this subsection cover assigning behaviour to parts via Perform Action Usages and Exhibit State Usages.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-BEH-H83 | PA, PT | Mycelium Bloom shall assign behaviors to parts using Perform Action Usages and Exhibit State Usages when "a user selects a part and associates an action or state behavior with it." | SysML 7.17, 7.18 | H |  |

##### 5.2.1.17 Analysis and verification

Engineers need to evaluate design quality and verify that requirements are met. Mycelium supports Analysis Cases (evaluating system properties), Verification Cases (verifying requirements with methods and verdicts), Constraint Definitions (validation rules), and Calculation Definitions (domain-specific computations). The requirements in this section cover the analytical capabilities that turn the model into a basis for design decisions.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-AV-QII | PA, PT | Mycelium Bloom shall create an Analysis Case Definition with a subject and objective requirements when "a user creates an Analysis Case Definition and specifies its subject and objectives." | SysML 7.23 | H |  |
| SSS-PA-AV-AU1 | PA, PT | Mycelium Bloom shall instantiate an Analysis Case Definition as an Analysis Case Usage to evaluate system properties when "a user instantiates an Analysis Case." | SysML 7.23 | H |  |
| SSS-PA-AV-UCQ | PA, PT | Mycelium Bloom shall create a Verification Case Definition specifying a verification method (test, analysis, inspection, or demonstration) when "a user creates a Verification Case Definition and assigns a method." | SysML 7.24 | H |  |
| SSS-PA-AV-VU2 | PA, PT | Mycelium Bloom shall instantiate a Verification Case Definition as a Verification Case Usage and record its verdict (pass, fail, or inconclusive) when "a user runs a verification case and records a verdict." | SysML 7.24 | H |  |
| SSS-PA-AV-LSX | PA, PT | Mycelium Bloom shall create a Constraint Definition expressing a Boolean condition when "a user creates a Constraint Definition." | SysML 7.20 | H |  |
| SSS-PA-AV-CU3 | PA, PT | Mycelium Bloom shall instantiate a Constraint Definition as a Constraint Usage asserted against one or more model elements for automated validation when "a user applies a constraint to model elements." | SysML 7.20 | H |  |
| SSS-PA-AV-CN5 | PA, PT | Mycelium Bloom shall assert or negate a Constraint Usage, where an asserted constraint must hold and a negated constraint must not hold, when "a user marks a Constraint Usage as asserted or negated." | SysML 7.20 | M |  |
| SSS-PA-AV-LLI | PA | Mycelium Bloom shall create a Trade Study that compares design alternatives using evaluation functions and objectives (maximise or minimise) when "the Project Administrator creates a Trade Study and specifies alternatives, criteria, and objective functions." | - | L |  |
| SSS-PA-AV-O9U | PA, PT | Mycelium Bloom shall link a Case (Use Case, Analysis Case, or Verification Case) to its objective Requirement by creating an ObjectiveMembership referencing the target Requirement Usage when "a user sets the objective of a Case from a selected Requirement." | SysML 8.3.22 | H |  |
| SSS-PT-ANALYSIS-4W2 | PT | Mycelium Bloom shall create a Calculation Definition with input parameters, output parameters, and a computation expression when "the Participant creates a Calculation Definition." | SysML 7.19 | H |  |
| SSS-PT-ANALYSIS-KU4 | PT | Mycelium Bloom shall instantiate a Calculation Definition as a Calculation Usage over model attributes when "the Participant instantiates a Calculation." | SysML 7.19 | H |  |
| SSS-PT-ANALYSIS-KE6 | PT | Mycelium Bloom shall evaluate a Calculation Usage to compute its result from its input values when "a user evaluates a Calculation Usage." | SysML 7.19 | M |  |
| SSS-PA-EXPR-X1A | PA, PT | Mycelium Bloom shall provide an editor to author an Expression composed of literal values, operators, references to model features, and invocations of Functions or Calculations, in any context that admits an Expression (constraint body, calculation computation, transition guard, action condition, multiplicity bound, or feature value), when "a user edits an expression." | KerML 8.3.4 | L |  |
| SSS-PA-EXPR-X2B | PA, PT | Mycelium Bloom shall insert a reference to a model feature into an Expression, including a feature chain that navigates nested features (for example, engine.cylinder.diameter), when "a user adds a feature reference to an expression." | KerML 8.3.4 | L |  |
| SSS-PA-EXPR-X3C | PA, PT, VW | Mycelium Bloom shall display an Expression in its SysML v2 textual notation wherever the expression appears, including the detail panel of its owning element, when "a user views an element that owns an Expression." | KerML 8.2.5 | L |  |
| SSS-PA-EXPR-X5E | PA, PT, VW | Mycelium Bloom shall evaluate a model-level-evaluable Expression over the current attribute values and display its computed result when "a user requests evaluation of an expression." | KerML 8.3.4 | L |  |
| SSS-PT-ANALYSIS-EAJ | PA, PT, VW | Mycelium Bloom shall display constraint evaluation results showing which constraints pass or violate when "a user navigates to the constraint evaluation view or triggers constraint evaluation." | SysML 7.20 | L |  |
| SSS-PA-AV-2RG | PA, PT, VW | Mycelium Bloom shall display a validation dashboard showing model quality, constraint violations, and verification status when "a user navigates to the validation dashboard view." | - | L |  |
| SSS-PA-AV-CR1 | PA, PT, VW | Mycelium Bloom shall display the results of an Analysis Case or Verification Case, showing its subject, its objective or verification method, and its computed outputs or recorded verdict, when "a user views a case or its evaluation completes." | SysML 7.23 | L |  |

##### 5.2.1.18 In-browser scripting

Some analyses cannot be expressed declaratively and require imperative computation. Mass budgets, power budgets, and complex requirements verification often need iteration and aggregation across the system structure. The requirements in this section describe a desirable in-browser scripting environment that runs computational analyses against model data without leaving the application.

> Even though these requirements are set to M and L, being able to provide support for this in the future must be taken into account in the architecture of the web application.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-SCRIPT-T4K | PA, PT | Mycelium Bloom should provide an in-browser scripting environment for writing and executing scripts that operate on model data when "a user opens the scripting editor." | - | M |  |
| SSS-PA-SCRIPT-M7R | PA, PT | Mycelium Bloom should execute scripts entirely in the browser without requiring server-side processing when "a user runs a script in the scripting environment." | - | M |  |
| SSS-PA-SCRIPT-H2W | PA, PT | Mycelium Bloom should provide access to the project's model data (element hierarchy, attributes, attribute values, relationships, and metadata) from within the scripting environment when "a script queries or traverses the model." | - | M |  |
| SSS-PA-SCRIPT-D9J | PA, PT | Mycelium Bloom should provide script templates for common computational analyses (e.g. mass budget, power budget, cost budget) that traverse the system structure, filter elements by metadata, and aggregate attribute values when "a user creates a new analysis script." | - | M |  |
| SSS-PA-SCRIPT-N5V | PA, PT | Mycelium Bloom should display script execution results as formatted tables, charts, or summary values within the scripting environment when "a script produces output." | - | L |  |
| SSS-PA-SCRIPT-W3F | PA, PT | Mycelium Bloom should write computed values back to model attributes as Computed value sources when "a script assigns a result to a model attribute and the user confirms the update." | - | L |  |
| SSS-PA-SCRIPT-K8B | PA, PT | Mycelium Bloom should evaluate Constraint Usages against model attribute values and report pass/fail/inconclusive verdicts when "a user executes a requirements verification script." | - | M |  |
| SSS-PA-SCRIPT-R6P | PA, PT | Mycelium Bloom should save and version scripts as part of the project so they are available to all project members when "a user saves a script in the scripting environment." | - | M |  |
| SSS-PA-SCRIPT-SB1 | PA, PT | Mycelium Bloom should execute user scripts in a sandboxed environment that isolates them from other projects, from other users' sessions, and from the host application, and limits their access to the model data the authenticated user is permitted to see, when "a script runs in the scripting environment." | - | M |  |
| SSS-PA-SCRIPT-OW2 | PA, PT | Mycelium Bloom should subject script writes to model attributes to the same ownership and validation rules as manual edits, rejecting writes to attributes the user is not permitted to modify, when "a script assigns a result to a model attribute." | - | M |  |
| SSS-PA-SCRIPT-CT3 | PA, PT | Mycelium Bloom should provide a cancel action for a running script and terminate any script that exceeds a configurable execution time or resource limit when "a script runs longer than permitted or the user requests cancellation." | - | L |  |

##### 5.2.1.19 Diagrams and visualization

Mycelium presents the model through a set of SysML v2 diagram types, each tailored to a modelling concern: structure, behaviour, requirements, and free-form exploration. All diagram types share a common graphical notation, drag-and-drop interaction with the model browser, and a round-trip in which editing a diagram updates the underlying model and editing the model updates every open diagram. The subsections below cover the shared diagramming and notation capabilities first, then each standard view, custom views and viewpoints, textual notation, and diagram export.

###### 5.2.1.19.1 General diagramming and notation

Mycelium Bloom must render model elements using the symbols defined in SysML v2 Part 1 section 8.2.3. This ensures that diagrams produced in and with Mycelium are immediately recognizable to anyone familiar with SysML v2 and exchangeable with other SysML v2 tools. The requirements in this section also cover diagram annotations, custom icons, and drag-and-drop interactions that apply to all diagram types.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-VIS-X4G | PA, PT, VW | Mycelium Bloom shall render all model elements using the graphical notation symbols defined in SysML v2 Part 1, section 8.2.3, including Definition and Usage node headers with guillemet kind designators (e.g. «part def», «action», «requirement»), compartment layouts, relationship lines, and adornments when "any diagram view displays model elements." | SysML 8.2.3 | H |  |
| SSS-PA-VIS-L7Q | PA, PT, VW | Mycelium Bloom shall visually distinguish Definition nodes from Usage nodes by displaying the `def` keyword in the guillemet header of Definition nodes (e.g. «part def») and omitting it for Usage nodes (e.g. «part») when "a diagram renders Definition and Usage elements." | SysML 8.2.3 | H |  |
| SSS-PA-VIS-R3F | PA, PT | Mycelium Bloom shall create the corresponding graphical node on the diagram canvas when "a user drags a model element from the model browser or a tabular browser and drops it onto a diagram." | - | H |  |
| SSS-PA-VIS-K8M | PA, PT | Mycelium Bloom shall create the corresponding model element in the underlying model when "a user creates a new graphical node or relationship on a diagram canvas using the diagram toolbox." | - | H |  |
| SSS-PA-VIS-H2W | PA, PT, VW | Mycelium Bloom shall reflect changes to model elements in all open diagrams containing those elements when "a model element's properties are modified in any view." | - | H |  |
| SSS-PA-VIS-N6J | PA, PT | Mycelium Bloom shall provide a toolbox palette for each diagram type listing the element and relationship types that can be created on that diagram when "a user opens a diagram editor." | - | H |  |
| SSS-PA-VIS-U9P | PA, PT, VW | Mycelium Bloom shall display compartments on graphical nodes (attributes, constraints, ports, nested elements) per the SysML v2 compartment notation when "a user expands or views compartments on a diagram element." | SysML 8.2.3 | H |  |
| SSS-PA-VIS-D5B | PA, PT, VW | Mycelium Bloom shall display multiplicity, property modifiers (ordered, nonunique, abstract, derived, readonly), and subsetting/redefinition markers on graphical elements per the SysML v2 notation when "a diagram renders elements with these properties." | SysML 8.2.3 | H |  |
| SSS-PA-VIS-C9K | PA, PT | Mycelium Bloom shall provide an interface to upload or select a custom icon and image for any Definition or Usage element when "a user accesses the icon settings of a model element." | - | H |  |
| SSS-PA-VIS-J2R | PA, PT, VW | Mycelium Bloom shall render the custom icon next to or in place of the standard SysML v2 graphical notation symbol on all diagrams containing the element when "a model element has a custom icon associated with it and the settings of the element are configured such that the icon shall be visualized." | - | H |  |
| SSS-PA-VIS-J9M | PA, PT, VW | Mycelium Bloom shall render the custom image in place of the standard SysML v2 graphical notation symbol on all diagrams containing the element when "a model element has a custom image associated with it and the settings of the element are configured such that the image shall be visualized." | - | H |  |
| SSS-PA-VIS-A6F | PA, PT, VW | Mycelium Bloom shall display the element name and type designator alongside the custom icon when "a diagram renders an element with a custom icon." | - | H |  |
| SSS-PA-VIS-CL1 | PA, PT | Mycelium Bloom shall provide a colour picker to set the fill and line colour of one or more selected diagram elements when "a user opens the colour settings of a selected diagram element." | - | M |  |
| SSS-PA-VIS-CL2 | PA, PT | Mycelium Bloom shall persist the colour selected for a diagram element so that it is restored when the diagram is reopened when "a user saves a diagram containing elements with a custom colour." | - | M |  |
| SSS-PA-VIS-F8Q | PA, PT | Mycelium Bloom shall provide operations to place free-text notes on the diagram canvas, with optional formatting (bold, italic, color), when "a user creates a note on a diagram." | KerML 7.4 | H |  |
| SSS-PA-VIS-B2M | PA, PT | Mycelium Bloom shall attach a note to a specific model element on the diagram via a dashed anchor line when "a user links a note to a diagram element." | KerML 7.4 | H |  |
| SSS-PA-VIS-G5R | PA, PT | Mycelium Bloom shall provide callout annotations that can point to a specific location on the diagram canvas when "a user creates a callout on a diagram." | KerML 7.4 | H |  |
| SSS-PA-VIS-T1J | PA, PT | Mycelium Bloom shall persist diagram notes and callouts as SysML v2 Comment elements annotating the relevant model elements when "a user saves a diagram containing notes or callouts." | KerML 7.4 | H |  |

###### 5.2.1.19.2 Interconnection View

An Interconnection View shows the structural composition of a system: parts, the ports through which they interact, and the connections between those ports. This is the most common diagram type for system architecture work and the entry point for most reviews of the physical decomposition.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-VIS-G8N | PA, PT | Mycelium Bloom shall provide an editor for creating and editing Interconnection Views showing parts, ports, and connections when "a user opens or creates an Interconnection View for a selected model scope." | SysML 8.2.3.11 | H |  |
| SSS-PA-VIS-W3T | PA, PT, VW | Mycelium Bloom shall render Part Usages as rectangular nodes with «part» headers, Port Usages as small squares on part boundaries with directional indicators (in, out, inout), and Connection Usages as lines between ports, using the SysML v2 graphical notation (section 8.2.3.11-14) when "an Interconnection View displays structural model content." | SysML 8.2.3.11-14 | H |  |
| SSS-PA-VIS-Q7K | PA, PT, VW | Mycelium Bloom shall render Interface Usages as connection lines between ports with the «interface» label and optional constraint compartments using the SysML v2 graphical notation (section 8.2.3.14) when "an Interconnection View displays interface connections." | SysML 8.2.3.14 | H |  |
| SSS-PA-VIS-I4R | PA, PT, VW | Mycelium Bloom shall render an Item Usage in the model browser, tabular views, and diagrams with a distinguishing icon and the «item» stereotype label, visually distinct from a Part Usage, showing its name, its typing Item Definition, and its multiplicity, when "a user views an Item Usage." | SysML 8.2.3.10 | H |  |
| SSS-PA-VIS-I5S | PA, PT, VW | Mycelium Bloom shall render Item Usages on a structural diagram as rounded-corner nodes using the SysML v2 graphical notation, and shall create an Item Usage on the canvas by dragging an Item Definition from the model browser or the Item tool from the toolbox, when "a user adds or views an Item Usage on a structural diagram." | SysML 8.2.3.10 | H |  |
| SSS-PA-VIS-I6T | PA, PT, VW | Mycelium Bloom shall render the payload Item Usage of a Flow Connection Usage alongside the flow connection line on a diagram, displaying the Item Usage name, its typing Item Definition, and its multiplicity, when "a user views a Flow Connection Usage that carries an Item." | SysML 8.2.3.16 | H |  |

###### 5.2.1.19.3 Action Flow View

An Action Flow View shows the behavior of the system as a sequence of actions with control flow between them. Engineers use it to describe how the system performs its functions, including parallelism (forks/joins), decisions, and loops. The notation closely follows UML activity diagrams.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-VIS-SMC | PA, PT | Mycelium Bloom shall provide an editor for creating and editing Action Flow Views showing action sequencing, control flow, and swim lanes when "a user opens or creates an Action Flow View for a selected action hierarchy." | SysML 8.2.3.17 | H |  |
| SSS-PA-VIS-E4R | PA, PT, VW | Mycelium Bloom shall render Action Usages as rounded-corner rectangles with «action» headers, and control flow using the SysML v2 standard symbols: start node (filled circle), done node (circled filled circle), fork/join nodes (bars), decision/merge nodes (diamonds), and succession arrows, per section 8.2.3.17, when "an Action Flow View displays behavioral model content." | SysML 8.2.3.17 | H |  |
| SSS-PA-VIS-J6N | PA, PT, VW | Mycelium Bloom shall render input/output parameters as small rectangles on action node boundaries with directional indicators (in, out, inout) per the SysML v2 graphical notation (section 8.2.3.17) when "an Action Flow View displays actions with parameters." | SysML 8.2.3.17 | H |  |
| SSS-PA-VIS-M1Z | PA, PT, VW | Mycelium Bloom shall render send action nodes, accept action nodes, while-loop action nodes, for-loop action nodes, and if-else action nodes using the SysML v2 standard symbols (section 8.2.3.17) when "an Action Flow View displays these action types." | SysML 8.2.3.17 | H |  |

###### 5.2.1.19.4 State Transition View

A State Transition View shows the states a system or part can be in and the transitions between them, triggered by events with optional guards and effects. This is essential for modeling operational modes, fault handling, and any behavior that depends on context.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-VIS-DP2 | PA, PT | Mycelium Bloom shall provide an editor for creating and editing State Transition Views showing states and transitions when "a user opens or creates a State Transition View for a selected state machine." | SysML 8.2.3.18 | H |  |
| SSS-PA-VIS-B8V | PA, PT, VW | Mycelium Bloom shall render State Usages as rounded-corner rectangles with «state» headers containing entry/do/exit action compartments, and Transition Usages as arrows labeled with trigger [guard] / effect, using the SysML v2 graphical notation (section 8.2.3.18) when "a State Transition View displays state-based model content." | SysML 8.2.3.18 | H |  |
| SSS-PA-VIS-F2C | PA, PT, VW | Mycelium Bloom shall render parallel state regions using the «parallel» designator per the SysML v2 graphical notation (section 8.2.3.18) when "a State Transition View displays concurrent state regions." | SysML 8.2.3.18 | H |  |

###### 5.2.1.19.5 Sequence View

A Sequence View shows interactions between parts over time as messages exchanged along lifelines. Engineers use it to capture protocol flows, scenario walkthroughs, and timing-sensitive behaviors.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-VIS-FA5 | PA, PT | Mycelium Bloom shall provide an editor for creating and editing Sequence Views showing interactions between parts over time when "a user opens or creates a Sequence View for a selected interaction context." | SysML 8.2.3.9 | H |  |
| SSS-PA-VIS-A9H | PA, PT, VW | Mycelium Bloom shall render lifelines as vertical dashed lines below part/port header nodes, and messages as horizontal arrows between lifelines with message labels, using the SysML v2 graphical notation (section 8.2.3.9) when "a Sequence View displays interaction model content." | SysML 8.2.3.9 | H |  |

###### 5.2.1.19.6 Use Case View

A Use Case View shows the use cases a system supports, the actors that interact with them, and the system boundary (subject), together with the include and extend relationships between use cases. Engineers and stakeholders use it to frame system functionality from an external, goal-oriented perspective.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-VIS-UC1 | PA, PT | Mycelium Bloom shall provide an editor for creating and editing Use Case Views showing use cases, actors, the subject boundary, and their relationships when "a user opens or creates a Use Case View for a selected subject." | SysML 8.2.3.25 | H |  |
| SSS-PA-VIS-UC2 | PA, PT, VW | Mycelium Bloom shall render Use Case Usages as ovals enclosed within the subject boundary rectangle, Actors as stick figures connected by association lines, and «include» relationships as dashed arrows, using the SysML v2 graphical notation (section 8.2.3.25), when "a Use Case View displays use case model content." | SysML 8.2.3.25 | H |  |

###### 5.2.1.19.7 Requirement View

A Requirement View displays requirements and their satisfaction relationships graphically. Stakeholders can see which design elements satisfy which requirements at a glance, supporting reviews and impact analysis.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-VIS-C3D | PA, PT, VW | Mycelium Bloom shall render Requirement Usages as rectangles with «requirement» headers containing the requirement text, and Satisfy Requirement Usages as dashed arrows labeled «satisfy», using the SysML v2 graphical notation (section 8.2.3.21) when "a diagram displays requirements and their satisfaction relationships." | SysML 8.2.3.21 | H |  |

###### 5.2.1.19.8 General View

A General View is an unconstrained canvas where engineers can place any model element type and freely arrange it. It supports brainstorming, mixed concept exploration, and stakeholder-facing presentations that don't fit a single standard diagram type.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-VIS-BB9 | PA, PT | Mycelium Bloom shall provide an editor for creating General Views for unconstrained graphical model exploration when "a user creates a new General View and adds model elements to its canvas." | SysML 8.2.3 | H |  |
| SSS-PA-VIS-P5W | PA, PT, VW | Mycelium Bloom shall create a graphical node for any model element type placed on a General View canvas, using its SysML v2 graphical notation symbol, when "a user adds an element to a General View." | SysML 8.2.3 | H |  |

###### 5.2.1.19.9 Grid View

A Grid View presents model data in tabular or matrix form. Engineers use it to compare attributes across many elements at once, or to view two-dimensional relationships between element sets.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-VIS-JPW | PA, PT | Mycelium Bloom shall provide a Grid View for tabular and matrix representations of model data when "a user creates a Grid View and selects the element types and properties to display." | SysML 7.26 | H |  |

###### 5.2.1.19.10 Custom Views, Viewpoints, and Rendering

Different stakeholders have different concerns: a power engineer wants a power-focused view, a thermal engineer wants thermal data, a customer wants high-level summaries. SysML v2 Viewpoint Definitions and View Definitions let users formalize these stakeholder concerns and create reusable filtered views, and Rendering Definitions control how a view presents its exposed content. The requirements in this section cover defining and managing custom views and viewpoints, and selecting how a view renders its content.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-VIS-T2V | PA, PT | Mycelium Bloom shall create custom View Definitions, View Usages, Viewpoint Definitions, and Viewpoint Usages filtered to specific stakeholder concerns when "a user creates a Viewpoint Definition, specifies its concerns, and creates a conforming View Definition." | SysML 7.26 | M |  |
| SSS-PA-VIS-K9R | PA, PT | Mycelium Bloom shall create an Expose relationship that imports filtered model content into a View, with optional metadata-based or query-based filter conditions, when "a user adds exposed model content to a View Definition." | SysML 7.26.2 | M |  |
| SSS-PA-VIS-RD1 | PA, PT | Mycelium Bloom shall apply a RenderingDefinition to a View, controlling how the view's exposed content is presented (for example as a tree, table, textual, or graphical rendering), when "a user selects a rendering for a View." | SysML 7.26.4 | M |  |
| SSS-PA-VIS-RD2 | PA, PT | Mycelium Bloom shall provide the standard SysML v2 rendering kinds and create custom RenderingDefinitions and RenderingUsages when "a user defines a custom rendering or selects a standard rendering kind." | SysML 7.26.4 | L |  |

###### 5.2.1.19.11 Textual notation

SysML v2 has a textual notation that some engineers prefer for editing, reviewing or sharing model content. Mycelium generates this notation read-only from the model, providing a reference representation without requiring users to edit text directly.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-VIS-IXL | PA, PT, VW | Mycelium Bloom shall generate and display the SysML v2 textual notation representation of model elements (read-only) when "a user selects one or more model elements and requests textual notation view." | SysML 8.2.2 | H |  |

###### 5.2.1.19.12 Diagram export

Diagrams need to leave Mycelium for reports, presentations, and external tools. The requirements in this section cover export to SVG (vector), PNG (raster, configurable resolution), and JPG (compressed raster) formats.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-VIS-V7S | PA, PT, VW | Mycelium Bloom shall export a diagram to SVG format preserving vector graphics fidelity when "a user selects SVG as the export format for a diagram." | - | H |  |
| SSS-PA-VIS-T1N | PA, PT, VW | Mycelium Bloom shall export a diagram to PNG format at a user-specified resolution when "a user selects PNG as the export format for a diagram." | - | H |  |
| SSS-PA-VIS-G4L | PA, PT, VW | Mycelium Bloom shall export a diagram to JPG format at a user-specified resolution and quality when "a user selects JPG as the export format for a diagram." | - | H |  |

###### 5.2.1.19.13 Diagram management and canvas operations

Beyond rendering, engineers need to manage diagrams as artifacts and arrange their content. The requirements in this subsection cover the lifecycle of a diagram and the canvas operations common to every diagram type. Diagram persistence and real-time collaboration are covered separately in 5.2.1.20.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-VIS-DM1 | PA, PT | Mycelium Bloom shall provide operations to create, open, rename, and delete diagrams and to list the diagrams of a project with their name and type when "a user accesses the project's diagram list." | - | H |  |
| SSS-PA-VIS-ZP3 | PA, PT, VW | Mycelium Bloom shall zoom, pan, and fit the diagram to the view when "a user zooms, pans, or invokes fit-to-view on a diagram." | - | H |  |
| SSS-PA-VIS-LY2 | PA, PT | Mycelium Bloom shall apply an automatic layout that arranges the nodes and routes the relationships of a diagram when "a user invokes auto-layout on a diagram." | - | L |  |
| SSS-PA-VIS-AL4 | PA, PT | Mycelium Bloom shall align and distribute selected diagram nodes when "a user invokes an alignment or distribution action on a multi-element selection." | - | L |  |
| SSS-PA-VIS-SE5 | PA, PT | Mycelium Bloom shall select multiple diagram elements and cut, copy, and paste them within or between diagrams when "a user performs a multi-selection and a clipboard operation." | - | M |  |
| SSS-PA-VIS-UR6 | PA, PT | Mycelium Bloom shall undo and redo diagram editing operations when "a user invokes undo or redo in a diagram editor." | - | M |  |
| SSS-PA-VIS-RT7 | PA, PT | Mycelium Bloom shall edit the routing of a relationship by adding, moving, and removing waypoints when "a user reroutes a relationship on a diagram." | - | M |  |

##### 5.2.1.20 Diagram persistence and real-time collaboration

A diagram in Mycelium Bloom is more than a transient rendering of the underlying model: it is a durable, first-class artifact with its own identity, layout, and collaboration state. KerML and SysML v2 do not (yet) define an abstract syntax for diagram layout persistence, so there is no standard metaclass describing node positions, routing waypoints, or custom per-diagram rendering overrides. OMG is defining a standard library, using SysML v2 constructs, for exchanging diagrams; once it is available Mycelium will use it to exchange diagram-related information. The requirements in this section also state that diagrams participate in Mycelium's lock-free collaboration model and display live presence and activity indicators for every user currently working on the same diagram.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-VIS-P1A | PA, PT, VW | Mycelium Bloom shall persist every diagram, including its identity (unique identifier, name, diagram type, description), its associated model scope, the set of displayed model elements, the layout of each node (position, size, collapsed or expanded state, custom icon override, visibility) and the routing of each relationship (waypoints, label position, line style), when "a user creates or edits a diagram." | - | H |  |
| SSS-FB-VIS-P3C | - | Mycelium Fabric shall persist and serve diagram layout content alongside the model content it annotates, applying the same commit, branch, merge, and ownership semantics to the diagram content as to the model elements, when "a client submits a commit containing diagram layout content or queries a diagram." | - | H |  |
| SSS-PA-VIS-C4D | PA, PT, VW | Mycelium Bloom shall render the same diagram to multiple users for simultaneous viewing and editing, without acquiring a lock on the diagram or on any of its graphical elements, consistent with the lock-free collaboration model defined in `SSS-CC-COLLAB-62C`, when "more than one user has the same diagram open." | - | H |  |
| SSS-PA-VIS-C5E | PA, PT, VW | Mycelium Bloom shall propagate every diagram change (node creation, move, resize, deletion, relationship creation, routing edit, label edit, property edit, and any model-side edit that affects a rendered element) to every other user currently viewing the same diagram in near real time via Mycelium Fabric's notification channel, when "a user modifies a diagram element." | - | H |  |
| SSS-PA-VIS-C6F | PA, PT, VW | Mycelium Bloom shall display, on every open diagram, the list of users currently viewing or editing it, showing each user's display name, avatar, and assigned collaborator colour, when "at least one user has the same diagram open." | - | H |  |
| SSS-PA-VIS-C7G | PA, PT, VW | Mycelium Bloom shall render, for every other user currently interacting with the diagram, a visual indicator of that user's pointer position, the node or edge they currently have selected, and the node or edge they are currently dragging, routing, or editing, each rendered in the user's assigned collaborator colour and labelled with their display name, when "another user is interacting with the diagram in real time." | - | H |  |
| SSS-PA-VIS-C8H | PA, PT, VW | Mycelium Bloom shall briefly highlight on the local diagram, using an animated outline or flash in the originator's collaborator colour, every node or edge that has just been created, deleted, moved, resized, or otherwise modified by another user, so that the local user notices the change, when "a real-time update from another user modifies a diagram element." | - | M |  |
| SSS-PA-VIS-C9J | PA, PT | Mycelium Bloom shall grant interactive editing control over any single diagram node or relationship to at most one user at a time, shall reject a second user's attempt to move, resize, reroute, relabel, or otherwise interactively modify a diagram element that another user is currently manipulating, shall render the element as busy (greyed-out or locked-cursor) with the holding user's display name and collaborator colour, and shall release the exclusive control as soon as the first user completes or cancels the interaction, when "two or more users attempt to interactively modify the same diagram element concurrently." This element-level, short-lived exclusion applies only to the in-flight interactive gesture on a diagram element and does not constitute a lock on the underlying model element; it therefore does not conflict with the lock-free collaboration principle in `SSS-CC-COLLAB-62C`. | - | M |  |

##### 5.2.1.21 3D model viewer

Spatial decomposition is most intuitive in 3D. Mycelium offers an interactive 3D viewer whose **primary** source of geometry is a set of SysML v2 Attribute Usages on each Part Usage (centre of gravity, orientation, basic shape, and dimensions) sourced from Attribute Definitions that live in a dedicated Mycelium Library Package. As a deferred capability, a Part Usage may additionally carry an attached STEP file, which Mycelium can use as the authoritative rendering source. Users can navigate the scene, select elements to inspect properties, and see Ownership-based colour coding to understand who is responsible for what. When the attribute values are updated (location, orientation, dimensions, shape) the interactive 3D viewer updates as well.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-VIS-E8Q | PA, PT, VW | Mycelium Bloom shall provide an interactive 3D viewer that renders the system decomposition as a three-dimensional scene when "a user opens the 3D model viewer for a project." | - | H |  |
| SSS-PA-VIS-M2K | PA, PT, VW | Mycelium Bloom shall render Part Usages in the 3D viewer using the geometric Attribute Usages defined by `SSS-PA-VIS-G1A` and `SSS-PA-VIS-G2B` as the primary source, falling back to placeholder shapes per `SSS-PA-VIS-G6F` when the required Attribute Usages are absent, when "the 3D viewer displays model elements." | - | H |  |
| SSS-PA-VIS-R5N | PA, PT, VW | Mycelium Bloom shall provide camera controls for orbiting, panning, and zooming the 3D scene when "a user interacts with the 3D viewer using mouse or touch input." | - | H |  |
| SSS-PA-VIS-W1J | PA, PT, VW | Mycelium Bloom shall highlight the selected element in the 3D scene and display its properties in the detail panel when "a user selects a model element in the 3D viewer." | - | H |  |
| SSS-PA-VIS-H7D | PA, PT, VW | Mycelium Bloom shall synchronize selection between the 3D viewer and the hierarchical Browser View when "a user selects an element in either view." | - | L |  |
| SSS-PA-VIS-B4F | PA, PT, VW | Mycelium Bloom shall display Ownership colour-coding on 3D elements when "the 3D viewer renders elements in a project with Ownership assignments." | - | M |  |
| SSS-PA-VIS-G1A | PA, PT, VW | Mycelium Bloom shall render each Part Usage in the 3D viewer from the values of its geometric Attribute Usages, resolving each Attribute Usage against the Attribute Definitions provided by the dedicated Mycelium 3D geometry Library Package, as the primary rendering source, when "the 3D viewer renders a Part Usage that carries the required geometric Attribute Usages." | SysML 7.5.5, 7.7 | H |  |
| SSS-PA-VIS-G2B | PA, PT | Mycelium Bloom shall require the following Attribute Usages on a Part Usage for it to be rendered from Attribute values in the 3D viewer: `centerOfGravity` (a three-component position), `orientation` (a rotation or quaternion), `basicShape` (an Enumeration Literal drawn from the set of supported primitive shapes, at minimum box, cylinder, sphere, cone, torus, and mesh), and `dimensions` (the shape-specific parameter set, for example `length`/`width`/`height` for `box` or `radius`/`height` for `cylinder`), when "a user authors a Part Usage that is intended to appear in the 3D viewer." | SysML 7.7 | H |  |
| SSS-PA-VIS-G3C | PA, PT, VW | Mycelium Bloom shall use an attached STEP (ISO 10303-242) file as an alternative rendering source for a Part Usage when one is present on that Part Usage and requested by the user to be used as rendering source. This requirement is deferred and is not part of the first release of Mycelium. | - | L |  |
| SSS-PA-VIS-G4D | - | Mycelium shall provide a dedicated Library Package, `Mycelium::Geometry3D`, that contains the Attribute Definitions (`centerOfGravity`, `orientation`, `basicShape`, `dimensions`) and the supporting Enumeration Definition for `basicShape`, packaged as a standard Library Package distributable via Mycelium Forge, when "a project needs to render Part Usages in the 3D viewer." | SysML 7.5.5 | H |  |
| SSS-PA-VIS-G5E | - | The `Mycelium::Geometry3D` Library Package shall import the necessary quantity-kind, unit, and scale definitions from the SysML v2 Quantities and Units standard library (ISO 80000) via a Namespace Import so that `centerOfGravity`, `orientation`, and `dimensions` are typed by standard quantity kinds and do not redefine units, when "the Library Package is authored or published to Mycelium Forge." | SysML 7.5.3, 9.8 | H |  |
| SSS-PA-VIS-G6F | PA, PT, VW | Mycelium Bloom shall render a neutral placeholder shape, labelled with the Part Usage name and its typing Part Definition, when a Part Usage lacks a complete set of geometric Attribute Usages per `SSS-PA-VIS-G2B`, when "the 3D viewer attempts to render a Part Usage that has no geometric information." | - | H |  |
| SSS-PA-VIS-UP1 | PA, PT, VW | Mycelium Bloom shall re-render a Part Usage in the 3D viewer when the values of its geometric Attribute Usages change, so the scene reflects the current model, when "a geometric Attribute Usage value of a displayed Part Usage is updated." | - | M |  |
| SSS-PA-VIS-VF2 | PA, PT, VW | Mycelium Bloom shall show, hide, and isolate Part Usages in the 3D scene, and filter the displayed parts by Ownership or category, when "a user changes the visibility or filter settings of the 3D viewer." | - | M |  |

##### 5.2.1.22 Queries

Engineers need to ask questions of their models: list all elements categorized as Equipment, find all requirements with no Satisfy relationship, retrieve all parts above a mass threshold. Mycelium offers a query interface based on the Systems Modelling API query operations, with the ability to save and re-execute queries against any commit.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-QRY-L11 | PA, PT | Mycelium Bloom shall provide a query interface supporting select, scope, where, and orderBy clauses when "a user composes a query and submits it for execution." | API 7.3 | L |  |
| SSS-PA-QRY-JYA | PA, PT, VW | Mycelium Bloom shall execute queries against any commit to retrieve historical model state when "a user specifies a target Commit identifier before executing a query." | API 7.3 | L |  |
| SSS-PA-QRY-QR1 | PA, PT, VW | Mycelium Bloom shall display query results as a sortable, filterable table from which the user can navigate to any matching element when "a query completes execution." | API 7.3 | L |  |
| SSS-PA-QRY-QR2 | PA, PT | Mycelium Bloom shall provide operations to save, list, rename, delete, re-execute, and share queries within the project when "a user accesses the saved queries list." | - | L |  |

##### 5.2.1.23 Reporting and dashboards

Beyond raw data, engineers and stakeholders need summary views showing model health, progress, and metrics. Mycelium provides dashboards for system monitoring, validation, and project model health, with click-through navigation from summary metrics to underlying elements.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-OA-SYS-IEJ | OA | Mycelium Bloom shall display active user sessions and system health metrics when "the Organization Administrator navigates to the system monitoring dashboard." | - | H |  |

###### 5.2.1.23a Project model dashboard

The project model dashboard gives the study lead and team a single view of model health: how many attributes are published vs unpublished, how many elements are unused, what the distribution of element types and Ownerships looks like, and how requirements coverage and constraint compliance are progressing. The requirements in this section cover histograms, pie charts, summary metrics, filtering, and click-through navigation, inspired by the equivalent CDP4-COMET-WEB dashboard.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-DASH-K7R | PA, PT, VW | Mycelium Bloom shall display a project model dashboard presenting an overview of model health and completeness when "a user opens the project model dashboard." | - | H |  |
| SSS-PA-DASH-W3D | PA, PT, VW | Mycelium Bloom shall display a histogram and summary count of published vs unpublished attributes per Ownership when "the project model dashboard renders the publication status section." | - | H |  |
| SSS-PA-DASH-N8F | PA, PT, VW | Mycelium Bloom shall display a histogram and summary count of attributes with missing values (no value assigned) grouped by Ownership when "the project model dashboard renders the missing values section." | - | H |  |
| SSS-PA-DASH-H2T | PA, PT, VW | Mycelium Bloom shall display a histogram and summary count of unused Definitions (Definitions with no Usages in the model) grouped by element type when "the project model dashboard renders the unused definitions section." | - | H |  |
| SSS-PA-DASH-D5J | PA, PT, VW | Mycelium Bloom shall display a histogram and summary count of unreferenced Usages (Usages not connected via any relationship, port, or connection to other elements) grouped by element type when "the project model dashboard renders the unreferenced elements section." | - | H |  |
| SSS-PA-DASH-R9V | PA, PT, VW | Mycelium Bloom shall display a pie chart showing the distribution of model elements by element type (Part, Item, Action, State, Requirement, Constraint, etc.) when "the project model dashboard renders the element composition section." | - | H |  |
| SSS-PA-DASH-T4K | PA, PT, VW | Mycelium Bloom shall display a pie chart showing the distribution of model elements by Ownership when "the project model dashboard renders the ownership distribution section." | - | H |  |
| SSS-PA-DASH-M6W | PA, PT, VW | Mycelium Bloom shall display a summary of requirements coverage showing the count and percentage of requirements with Satisfy relationships, Verification Case links, and unallocated requirements when "the project model dashboard renders the requirements coverage section." | - | M |  |
| SSS-PA-DASH-J1B | PA, PT, VW | Mycelium Bloom shall display a summary of constraint compliance showing the count of satisfied, violated, and unevaluated constraints when "the project model dashboard renders the constraint compliance section." | - | M |  |
| SSS-PA-DASH-V8G | PA, PT, VW | Mycelium Bloom shall display a summary of subscription activity showing the count of active ParameterSubscriptions and the count of subscribed attributes with stale (unpublished) values when "the project model dashboard renders the subscription status section." | - | H |  |
| SSS-PA-DASH-F3K | PA, PT, VW | Mycelium Bloom shall filter all project model dashboard sections by Ownership, element type, metadata annotation, and variant configuration when "a user applies filters to the project model dashboard." | - | M |  |
| SSS-PA-DASH-B7N | PA, PT, VW | Mycelium Bloom shall navigate to the list of matching model elements when "a user clicks a bar in a histogram or a segment in a pie chart on the project model dashboard." | - | H |  |

###### 5.2.1.23b History and trends

Beyond a snapshot of current model health, engineers track how the model evolves over time. Mycelium plots attribute values, element change history, and project-level metrics across the Commits and Tags of a branch. The requirements in this subsection cover attribute value history, per-element change history and diffs, and trend charts for requirements coverage, verification status, constraint compliance, and model growth.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-HIST-K3R | PA, PT, VW | Mycelium Bloom shall display the value history of one or more selected attributes as a time-series chart plotting the attribute values across Commits or Tags on the active branch when "a user opens the attribute history view and selects one or more attributes." | - | H |  |
| SSS-PA-HIST-T6W | PA, PT, VW | Mycelium Bloom shall render multiple attributes of different quantity kinds on the same chart using independent Y-axes (one per quantity kind) with distinct colours and a shared Commit/Tag X-axis when "a user selects attributes with different units or quantity kinds for the attribute history view." | - | H |  |
| SSS-PA-HIST-V2P | PA, PT, VW | Mycelium Bloom shall display the unit and quantity kind label on each Y-axis and provide a legend identifying each plotted attribute by name, element, and unit when "the attribute history chart displays multiple attributes." | - | H |  |
| SSS-PA-HIST-W8D | PA, PT, VW | Mycelium Bloom shall display the change history of any model element listing all Commits in which the element was created, modified, or deleted, with the commit author, date, and description, when "a user opens the element history view." | - | M |  |
| SSS-PA-HIST-N5T | PA, PT, VW | Mycelium Bloom shall display the property-level diff of a model element between two Commits, showing which attributes, relationships, and metadata changed and their old vs new values, when "a user selects two Commits in the element history view." | - | M |  |
| SSS-PA-HIST-D2J | PA, PT, VW | Mycelium Bloom shall display requirements coverage evolution as a chart showing the percentage of requirements with at least one Satisfy relationship across Commits or Tags when "a user opens the requirements coverage trend view." | - | M |  |
| SSS-PA-HIST-H7F | PA, PT, VW | Mycelium Bloom shall display verification status evolution as a chart showing the count of pass, fail, and inconclusive verdicts across Commits or Tags when "a user opens the verification trend view." | - | M |  |
| SSS-PA-HIST-M4B | PA, PT, VW | Mycelium Bloom shall display constraint compliance evolution as a chart showing the count of satisfied vs violated constraints across Commits or Tags when "a user opens the constraint compliance trend view." | - | M |  |
| SSS-PA-HIST-R9G | PA, PT, VW | Mycelium Bloom shall display model growth metrics (total element count, total relationship count, total attribute count) as a chart across Commits or Tags when "a user opens the model growth trend view." | - | H |  |

##### 5.2.1.24 User interface adaptation

Mycelium supports novice, intermediate, and expert users. The interface should adapt to the user's role and Ownership, surface commonly-used features prominently, and provide context-aware help. The requirements in this section cover role-aware interface adaptation, workspace customization, and the About dialog.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PT-UI-NLJ | All | Mycelium Bloom shall provide a role-aware interface that surfaces information and tools relevant to the user's assigned Ownership and tasks when "a user logs in and the application loads their role and Ownership assignments." | - |  |  |
| SSS-PT-UI-L3Q | PA, PT, VW | Mycelium Bloom shall provide workspace customization for dashboard layouts and favourite views when "a user accesses workspace customization settings." | - | L |  |
| SSS-PT-UI-YJL | All | Mycelium Bloom shall present commonly used features first and advanced capabilities on demand (progressive disclosure) when "a user interacts with any feature area of the application." | - | L |  |
| SSS-PT-UI-2BM | PA, PT, VW | Mycelium Bloom shall display context-aware panels showing related diagrams, constraints, and verification results when "a user selects a model element in any view." | - | M |  |
| SSS-PT-UI-G4M | All | Mycelium Bloom shall display an About window showing the application name, version number, license information, copyright notice, and links to documentation and source code when "a user opens the About dialog." | - | H |  |
| SSS-PT-UI-TH1 | All | Mycelium Bloom shall provide a setting to switch the interface theme (light, dark, or high-contrast) when "a user selects a theme in their preferences." | - | L |  |
| SSS-PT-UI-KB3 | All | Mycelium Bloom shall provide keyboard shortcuts for common operations, configurable by the user, when "a user invokes or customises a keyboard shortcut." | - | L |  |

##### 5.2.1.25 Import, export and migration

Mycelium must interoperate with the broader MBSE ecosystem. Models can be imported and exported in SysML v2 JSON, requirements in ReqIF, content in HTML for documentation. CDP4-COMET ECSS-E-TM-10-25 models can be migrated to SysML v2 via a semi-automated converter. The requirements in this section cover all import, export, and migration capabilities.

###### 5.2.1.25.a Model interchange

Mycelium exchanges model content with other tools and projects. SysML v2 abstract syntax is interchanged as JSON or XMI, requirements as ReqIF, and elements can be referenced live across projects. The requirements in this subsection cover importing and exporting model content and referencing elements from other projects.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-IE-QWN | PA | Mycelium Bloom shall import and export models in the standard SysML v2 JSON or XMI serialisation of the abstract syntax, with the JSON serialisation compliant with the OMG Systems Modelling API, when "the Project Administrator initiates an import or export operation and selects the format and the target file or endpoint." | API 7 | H |  |
| SSS-PA-REQ-D7V | PA | Mycelium Bloom shall import requirements from a ReqIF file when "the Project Administrator initiates an import operation and selects a ReqIF file to import." | - | H |  |
| SSS-PA-REQ-D7W | PA | Mycelium Bloom shall export requirements to ReqIF format when "the Project Administrator initiates an export operation and selects a target ReqIF file or target location." | - | H |  |

###### 5.2.1.25.b Migration from CDP4-COMET

Existing CDP4-COMET models, based on ECSS-E-TM-10-25, can be brought into Mycelium and converted to SysML v2 by a semi-automated converter. The requirements in this subsection cover the migration process, the resolution of mapping ambiguities, and the migration report.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-IE-ZLQ | PA | Mycelium Bloom shall migrate existing ECSS-E-TM-10-25 models from CDP4-COMET into SysML v2 using a semi-automated converter when "the Project Administrator uploads an ECSS-E-TM-10-25 model and initiates the migration process." | - | H |  |
| SSS-PA-IE-YSY | PA | Mycelium Bloom shall present mapping ambiguities for user resolution during ECSS-to-SysML v2 migration when "the converter encounters ECSS-E-TM-10-25 elements that do not have a deterministic SysML v2 mapping." | - | H |  |
| SSS-PA-IE-MR1 | PA | Mycelium Bloom shall produce a migration report listing the ECSS-E-TM-10-25 elements that were migrated, skipped, or failed, together with their resolved SysML v2 mapping and a reference to the original source element, when "an ECSS-to-SysML v2 migration completes." | - | H |  |

###### 5.2.1.25.c Document and view export

Model content leaves Mycelium as human-readable documents for reports, reviews, and stakeholders. The requirements in this subsection cover export of views, diagrams, and reports to standard formats and the generation of navigable HTML documents from requirements and model element selections.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-VW-EXP-KVK | PA, PT, VW | Mycelium Bloom shall export views, diagrams, and reports to standard formats (e.g. PDF, image) when "a user initiates an export from a view or dashboard." | - | L |  |
| SSS-PA-IE-B5W | PA, PT, VW | Mycelium Bloom shall export a Requirements Specification as a navigable HTML document preserving the hierarchical structure, requirement text, categories, and constraint details when "a user selects HTML as the export format for a Requirements Specification." | - | L |  |
| SSS-PA-IE-N8G | PA, PT, VW | Mycelium Bloom shall export a user-selected set of model elements (e.g. a Package, a Part Definition with its decomposition, or a filtered query result) as a navigable HTML document showing element properties, attributes, relationships, and Documentation when "a user selects HTML as the export format for a model element selection." | - | M |  |

##### 5.2.1.26 Comments and documentation

SysML v2 defines Comment as an annotating element with a textual body that can describe one or more model elements, and Documentation as a specialized Comment that formally documents its owning element. Comments and Documentation are the primary mechanism for adding explanatory text, rationale, design notes, and review feedback to model elements.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-CMT-R4K | PA, PT | Mycelium Bloom shall create a Comment with a textual body on one or more model elements when "a user adds a comment to a model element via the detail panel or context menu." | KerML 7.4.2 | H |  |
| SSS-PA-CMT-W7N | PA, PT | Mycelium Bloom shall create a Documentation element on a model element, representing its formal description, when "a user adds or edits the documentation of a model element." | KerML 7.4.3 | H |  |
| SSS-PA-CMT-H3D | PA, PT | Mycelium Bloom shall edit and delete Comments and Documentation owned by the user's Ownership when "a user modifies or removes a comment or documentation entry." | KerML 7.4 | H |  |
| SSS-PA-CMT-M6J | PA, PT, VW | Mycelium Bloom shall display all Comments and Documentation associated with a model element in the detail panel, showing the text body, author, and creation date, when "a user views a model element's properties." | KerML 7.4 | H |  |
| SSS-PA-CMT-T9F | PA, PT, VW | Mycelium Bloom shall indicate in the model browser that an element has Comments or Documentation attached using a visual indicator (e.g. icon or badge) when "an element has one or more Comments or Documentation entries." | KerML 7.4 | H |  |
| SSS-PA-CMT-K2B | PA, PT | Mycelium Bloom shall format Comment and Documentation text using rich text (bold, italic, lists, links) when "a user edits the body of a Comment or Documentation element." | KerML 7.4 | M |  |
| SSS-PA-CMT-D5P | PA, PT | Mycelium Bloom shall annotate a single Comment on multiple model elements simultaneously when "a user creates a comment and selects multiple annotated elements." | KerML 7.4.2 | H |  |
| SSS-PA-CMT-N8V | PA, PT | Mycelium Bloom shall specify an optional locale (e.g. "en", "fr", "de") on a Comment or Documentation element when "a user sets the language of a comment or documentation entry." | KerML 7.4.2 | L |  |
| SSS-PA-CMT-Y6L | PA, PT | Mycelium Bloom shall create a Textual Representation on a model element, embedding language-specific text (e.g. a code snippet, formula, or DSL expression) tagged with the language identifier, when "a user adds a textual representation to an element and selects the language." | KerML 7.4.4 | M |  |
| SSS-PA-CMT-L7X | PA, PT | Mycelium Bloom shall create an AnnotatingElement (Comment, Documentation, Textual Representation, or Metadata Feature) together with its Annotation relationship(s) to one or more target elements in a single user operation when "a user draws a line in a diagram from the annotation tool in the toolbox, or from an existing annotation node, to one or more diagram nodes." | KerML 7.4.1 | H |  |
| SSS-PA-CMT-Z9K | PA, PT | Mycelium Bloom shall create an AnnotatingElement (Comment, Documentation, Textual Representation, or Metadata Feature) together with its Annotation relationship(s) to the currently selected model element(s) when "a user invokes an 'Add Comment', 'Add Documentation', 'Add Textual Representation', or 'Apply Metadata' action from the context (right-click) menu or from the detail panel of a list or tabular view." | KerML 7.4.1 | H |  |
| SSS-PA-CMT-RP1 | PA, PT | Mycelium Bloom shall create a reply to an existing Comment, modelled as a Comment annotating the parent Comment, forming a threaded discussion, when "a user replies to a Comment." | KerML 7.4 | M |  |
| SSS-PA-CMT-RS2 | PA, PT | Mycelium Bloom shall mark a Comment as resolved or reopen it, and indicate the resolved status in the detail panel and the model browser, when "a user resolves or reopens a Comment." | - | M |  |

##### 5.2.1.27 Review workflow

Branch protection rules can require designated Reviewers to approve merges before they enter the default branch. The requirements in this section cover the reviewer interface for approving or requesting changes on protected branch merges, supporting the gatekeeper model for design baselines.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PT-VC-MR2 | PA, PT | Mycelium Bloom shall propose a merge to a protected branch and request review from the designated Reviewers when "a user submits a merge into a protected branch." | - | L |  |
| SSS-PT-VC-DV3 | PA, PT, VW | Mycelium Bloom shall display the diff of a proposed merge, showing the added, modified, and deleted model elements and diagrams with their old and new values, when "a Reviewer opens a proposed merge." | - | L |  |
| SSS-PT-VC-JPL | PA, PT, VW | Mycelium Bloom shall provide a review interface to approve or request changes on merges to protected branches when "a user has been designated as a Reviewer and a merge is proposed." | - | L |  |
| SSS-PT-VC-IC7 | PA, PT, VW | Mycelium Bloom shall add review comments on specific elements or changes within a proposed merge when "a Reviewer comments on a change in a proposed merge." | - | L |  |
| SSS-FB-VC-EN5 | - | Mycelium Fabric shall reject completion of a merge into a protected branch until the required number of approving reviews has been recorded when "a client attempts to complete a merge into a protected branch." | - | L |  |
| SSS-PT-VC-CM6 | PA, PT | Mycelium Bloom shall complete the merge once the required approvals are recorded when "a user completes an approved merge into a protected branch." | - | L |  |

##### 5.2.1.28 Attachments

Engineering elements often need supporting documentation: thermal analysis PDFs, interface drawings, datasheets, photographs, spreadsheets. Mycelium lets users attach files of any type to any model element and download them later. The requirements in this section cover upload, download, listing, removal, and inline preview for common formats.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-ATT-W5R | PA, PT | Mycelium Bloom shall upload one or more file attachments to any model element when "a user selects a model element and adds attachments via the attachment interface." | - | H |  |
| SSS-PA-ATT-K3J | PA, PT, VW | Mycelium Bloom shall display a list of all attachments associated with a model element, showing file name, file type, size, upload date, and uploading user, when "a user views the attachments of a model element." | - | H |  |
| SSS-PA-ATT-M8D | PA, PT, VW | Mycelium Bloom shall download an attachment when "a user selects an attachment from the attachment list of a model element." | - | H |  |
| SSS-PA-ATT-F2N | PA, PT | Mycelium Bloom shall remove an attachment from a model element when "a user with write access to the element deletes an attachment from the attachment list." | - | H |  |
| SSS-PA-ATT-V6H | PA, PT, VW | Mycelium Bloom shall display inline previews for image attachments (PNG, JPG, SVG) and PDF attachments when "a user views the attachment list of a model element." | - | L |  |
| SSS-PA-ATT-VR1 | PA, PT | Mycelium Bloom shall upload a new revision of an existing attachment, preserving prior revisions, when "a user replaces an attachment with an updated file." | - | H |  |
| SSS-FB-ATT-VAL2 | - | Mycelium Fabric shall reject an attachment upload that exceeds the configured maximum file size or whose type is not permitted, returning an error, when "a client uploads an attachment." | - | M |  |
| SSS-FB-ATT-SCAN3 | - | Mycelium Fabric shall scan uploaded attachments for malware and reject or quarantine any attachment that fails the scan when "an attachment is uploaded." | - | M |  |

##### 5.2.1.29 Glossary of Terms

Engineering teams need a shared vocabulary. Acronyms, domain terms, and project-specific definitions should be discoverable wherever they appear. Mycelium models a glossary as a Package of Item Definitions with Documentation, and the user interface highlights terms throughout the application with tooltips and click-through navigation. This makes the glossary live and contextual rather than a forgotten document.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-GLOSS-T5R | PA, PT | Mycelium Bloom shall provide operations to create and manage a Glossary Package containing Item Definitions where each Item Definition represents a glossary term (name = term, short name = abbreviation) with Documentation providing the definition when "a user accesses the glossary management interface." | KerML 7.4 | L |  |
| SSS-PA-GLOSS-K2W | PA, PT, VW | Mycelium Bloom shall display a tabular view listing all glossary terms with their name, abbreviation, definition, and owning package when "a user opens the glossary browser." | KerML 7.4 | L |  |
| SSS-PA-GLOSS-H8N | PA, PT | Mycelium Bloom shall provide operations to create, edit, and delete glossary terms (Item Definitions with Documentation) within a Glossary Package when "a user accesses the glossary management interface." | KerML 7.4 | L |  |
| SSS-PA-GLOSS-M3J | PA, PT, VW | Mycelium Bloom shall render any occurrence of a glossary term name or abbreviation as highlighted linked text throughout the application (model browser, detail panels, requirement text, diagram labels, comments) when "text content contains a word matching a glossary term name or abbreviation." | KerML 7.4 | L |  |
| SSS-PA-GLOSS-V9D | PA, PT, VW | Mycelium Bloom shall display a tooltip showing the glossary term definition when "a user hovers over a highlighted glossary term in any view." | KerML 7.4 | L |  |
| SSS-PA-GLOSS-F6B | PA, PT, VW | Mycelium Bloom shall navigate to the glossary term's Item Definition in the glossary browser when "a user clicks a highlighted glossary term link." | KerML 7.4 | L |  |

##### 5.2.1.30 Constants

Engineering models reference physical and project-specific constants (the speed of light, gravitational acceleration, target margins). Modelling these as named, typed Attribute Definitions with fixed values and source references makes them reusable across the project and traceable to their origin. Users can drag a constant into any constraint or calculation to ensure consistent values.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-CONST-N7K | PA, PT | Mycelium Bloom shall provide operations to create and manage a Constants Package containing Attribute Definitions where each Attribute Definition represents a named constant typed by a Quantity Kind, with a fixed Attribute Usage holding the value and Measurement Unit, and Documentation providing the source or reference, when "a user accesses the constants management interface." | SysML 9.8 | L |  |
| SSS-PA-CONST-D3V | PA, PT, VW | Mycelium Bloom shall display a tabular view listing all constants with their name, abbreviation, value, unit, and source reference when "a user opens the constants browser." | SysML 9.8 | L |  |
| SSS-PA-CONST-W8F | PA, PT | Mycelium Bloom shall provide operations to create, edit, and delete constants (Attribute Definitions with fixed Attribute Usages) within a Constants Package when "a user accesses the constants management interface." | SysML 9.8 | L |  |
| SSS-PA-CONST-J5M | PA, PT | Mycelium Bloom shall insert a reference to a constant's value into a constraint expression or calculation when "a user drags a constant from the constants browser and drops it into a constraint or calculation editor." | SysML 9.8 | L |  |
| SSS-PA-CONST-R2H | PA, PT, VW | Mycelium Bloom shall display a tooltip showing the constant's value, unit, and source reference when "a user hovers over a constant reference in a constraint expression, calculation, or attribute value." | SysML 9.8 | L |  |

##### 5.2.1.31 Version control and branching

Mycelium models are versioned like source code. Every change becomes a Commit; alternatives live on Branches; milestones are marked with Tags; merges combine work from different lines. The requirements in this section cover the full Systems Modelling API version control model adapted to a collaborative MBSE context, including a Git-style history graph for navigating commits and branches.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-VC-8SB | PA, PT | Mycelium Bloom shall create a Commit representing an immutable, non-destructible snapshot of model changes, consistent with the Systems Modelling API Commit concept, when "a user submits pending model changes with a commit description." | API 7.2.3 | H |  |
| SSS-PA-VC-PPI | PA, PT | Mycelium Bloom shall provide operations to create and manage Branches as independent lines of model development, each pointing to a head Commit, when "a user creates a new Branch from an existing Commit or manages existing Branches." | API 7.2.2 | H |  |
| SSS-PA-VC-SPJ | PA | Mycelium Bloom shall create an immutable Tag on a specific Commit to mark a milestone, baseline, or release when "the Project Administrator selects a Commit and assigns a Tag name." | API 7.2.4 | H |  |
| SSS-PA-VC-AJ9 | PA, PT | Mycelium Bloom shall merge Commits into a Branch with conflict detection and resolution, consistent with the Systems Modelling API mergeIntoBranch operation, when "a user initiates a merge of a source Branch into a target Branch." | API 7.2.5 | L |  |
| SSS-PA-VC-P89 | PA, PT, VW | Mycelium Bloom shall display the differences between two Commits showing which elements were created, updated, or deleted, consistent with the Systems Modelling API diffCommits operation, when "a user selects two Commits for comparison." | API 7.2.6 | H |  |
| SSS-PA-VC-7S4 | PA, PT, VW | Mycelium Bloom shall retrieve and display the complete versioned data of a Project at any Commit when "a user selects a historical Commit for inspection." | API 7.2.3 | H |  |
| SSS-PA-VC-KXT | PA | Mycelium Bloom shall provide a configuration interface for branch protection rules, including the required number of approving reviews, on any branch when "the Project Administrator accesses the branch protection settings." | - | H |  |
| SSS-PA-VC-28D | PA | Mycelium Bloom shall provide operations to designate Participants or Viewers as Reviewers for protected branches when "the Project Administrator assigns reviewers in the branch protection settings." | - | H |  |
| SSS-VW-VH-WGA | PA, PT, VW | Mycelium Bloom shall display the commit history of a project when "a user navigates to the version history view." | API 7.2.3 | H |  |
| SSS-PA-VC-V3K | PA, PT | Mycelium Bloom shall switch the active branch, loading the model state at the head Commit of the selected branch, when "a user selects a different branch from the branch selector." | API 7.2.2 | H |  |
| SSS-PA-VC-R8W | PA, PT, VW | Mycelium Bloom shall display the currently active branch name in the application header when "a user is working in a project." | - | H |  |
| SSS-PA-VC-H4N | PA, PT, VW | Mycelium Bloom shall display a list of all branches in the project with their name, head Commit, creator, and creation date when "a user opens the branch management view." | API 7.2.2 | H |  |
| SSS-PA-VC-D7J | PA | Mycelium Bloom shall delete a non-default branch when "the Project Administrator initiates branch deletion and confirms the action." | API 7.2.2 | H |  |
| SSS-PA-VC-M1F | PA, PT, VW | Mycelium Bloom shall display the commit and branch history as a graph visualization with parallel lanes for branches, commit nodes, merge lines, and tag markers when "a user opens the version history graph view." | - | H |  |
| SSS-PA-VC-W5T | PA, PT, VW | Mycelium Bloom shall display commit metadata (author, date, description, changed element count) in a detail panel when "a user selects a commit node in the version history graph." | API 7.2.3 | H |  |
| SSS-PA-VC-N9B | PA, PT, VW | Mycelium Bloom shall highlight the active branch and its head Commit in the version history graph when "the version history graph is displayed." | - | H |  |
| SSS-PA-VC-F2G | PA, PT, VW | Mycelium Bloom shall load the complete model state at a selected historical Commit in read-only mode when "a user selects a Commit other than the head Commit from the version history graph, branch list, or commit history." | API 7.2.3 | H |  |
| SSS-PA-VC-J6K | PA, PT, VW | Mycelium Bloom shall display a visual indicator (e.g. banner or badge) stating the Commit identifier and date, making clear the user is viewing a historical snapshot and not the current head, when "the model is loaded at a historical Commit." | - | H |  |
| SSS-PA-VC-T3P | PA, PT | Mycelium Bloom shall create a new Branch from a selected historical Commit when "a user chooses to branch from a historical Commit to continue development from that point in time." | API 7.2.2 | H |  |
| SSS-PA-VC-B8W | PA, PT, VW | Mycelium Bloom shall return to the head Commit of the active branch when "a user exits the historical snapshot view." | - | H |  |
| SSS-PA-OPT-09P | PA, PT | Mycelium Bloom shall create a Branch for a design alternative, where each Branch represents an independent line of development for a candidate solution, when "a user creates a Branch for a design alternative from an existing Commit." | API 7.2.2 | H |  |
| SSS-PA-OPT-DNI | PA, PT, VW | Mycelium Bloom shall display a comparison of design alternatives by diffing Commits across Branches when "a user selects Commits from different Branches for cross-branch comparison." | API 7.2.6 | L |  |
| SSS-PA-OPT-W7T | PA | Mycelium Bloom shall merge a selected design alternative Branch into the default Branch when "the Project Administrator initiates a merge of the alternative Branch and resolves any conflicts." | API 7.2.5 | L |  |
| SSS-PA-VC-TG2 | PA | Mycelium Bloom shall provide operations to list and delete Tags, showing each Tag's name, target Commit, and creator, when "a user accesses the tag management view." | API 7.2.4 | H |  |
| SSS-PA-VC-DB3 | PA | Mycelium Bloom shall set the default branch of a project when "the Project Administrator designates a branch as the default in the branch management view." | API 7.2.2 | H |  |
| SSS-PA-VC-RN4 | PA, PT | Mycelium Bloom shall rename a non-default branch when "a user renames a branch in the branch management view." | API 7.2.2 | H |  |

##### 5.2.1.32 Multi-backend support and polling

Mycelium Bloom must work not only with Mycelium Fabric but with any backend that implements the OMG Systems Modelling API. Some backends support push notifications (SignalR/WebSocket); others do not. The requirements in this section cover backend portability and a polling fallback for backends without push capability.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-CC-BACK-R5W | All | Mycelium Bloom shall connect to any backend server that implements the OMG Systems Modelling API and Services specification (REST/HTTP PSM), not only Mycelium Fabric, when "a user provides the URL of a compliant model server." | API 7 | H |  |
| SSS-CC-BACK-K8N | All | Mycelium Bloom shall detect whether the connected backend supports push-based change notifications (e.g. SignalR/WebSocket) and fall back to a polling mechanism when "the backend does not offer push-based notifications." | - | H |  |
| SSS-CC-BACK-D3T | All | Mycelium Bloom shall poll the connected backend for model changes at a user-configurable interval when "the polling mechanism is active." | - | H |  |
| SSS-CC-BACK-H7J | All | Mycelium Bloom shall provide a setting to configure the polling interval (in seconds) and to enable or disable polling when "a user accesses the connection settings for a backend." | - | H |  |
| SSS-CC-BACK-M1V | All | Mycelium Bloom shall provide a manual refresh operation that retrieves the complete current model state from the connected backend when "a user initiates a manual refresh." | - | H |  |
| SSS-CC-BACK-AU1 | All | Mycelium Bloom shall authenticate to a connected backend using the credentials or token configured for that backend when "a user connects to a backend that requires authentication." | - | H |  |
| SSS-CC-BACK-CD2 | All | Mycelium Bloom shall detect the capabilities offered by the connected backend (such as ownership enforcement, concurrent design support, and diagram-layout persistence) and disable or adapt the features the backend does not support, indicating the limitation to the user, when "Bloom connects to a backend." | - | H |  |
| SSS-CC-BACK-ER3 | All | Mycelium Bloom shall indicate a lost backend connection and attempt to reconnect, resuming push or polling once the connection is restored, when "the connection to the backend is interrupted." | - | M |  |

#### 5.2.2 Mycelium Fabric

##### 5.2.2.1 Systems Modelling API

Mycelium Fabric implements the OMG Systems Modelling API and Services specification. This is what makes Mycelium a SysML v2 native platform and what enables interoperability with other tools that consume the standard API. The requirements in this section anchor the Fabric implementation to the standard.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-CC-STD-XSS | All | Mycelium Fabric shall implement the OMG Systems Modelling API and Services specification (formal/25-09-04) using the REST/HTTP PSM when "the model server processes any API request." | API 7 | H |  |
| SSS-CC-EXT-QIN | All | Mycelium Fabric shall expose a REST API compliant with the OMG Systems Modelling API to enable integration with domain-specific tools when "an external tool issues API requests to the model server." | API 7 | H |  |

##### 5.2.2.2 Authentication and authorization

User identity, credentials, and session management are handled by Mycelium Fabric in conjunction with an external identity provider (Keycloak by default). The requirements in this section cover authentication enforcement, security policy enforcement, and the user invitation mechanism.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-OA-AUTH-SI7 | All | Mycelium Fabric shall authenticate all user sessions using token-based authentication when "a user submits valid credentials at the login interface." | - | H |  |
| SSS-OA-AUTH-GIT | All | Mycelium Fabric shall enforce password policies and session expiration rules when "the Organization Administrator has configured security policies in the authentication settings." | - | H |  |
| SSS-FB-AUTH-L0Z | All | Mycelium Fabric shall send an invitation to a user to join the organization as a Member when "the Organization Administrator submits an invitation with the target user's identity." | - | H |  |
| SSS-FB-IA-R4X | All | Mycelium Fabric shall restrict installation-wide management API endpoints to users with the Installation Administrator role when "a user attempts to access installation administration operations." | - | H |  |
| SSS-FB-IA-J6C | All | Mycelium Fabric shall assign the Installation Administrator role to the first user who completes the initial setup on an on-premise deployment when "the installation has no existing Installation Administrator." | - | H |  |
| SSS-FB-IA-Y2M | IA | Mycelium Fabric shall record all installation-wide administrative actions in an immutable audit log when "an Installation Administrator performs a create, update, delete, suspend, or reactivate operation on a user or organization." | - | M |  |
| SSS-PA-STATE-H5J | All | Mycelium Fabric shall reject all create, modify, and delete operations on model elements when "the project is in the Review or Archived state." | - | H |  |

##### 5.2.2.3 Ownership enforcement

Ownership-based access control is enforced server-side by Mycelium Fabric, Bloom merely presents the UI for it. The requirements in this section ensure that no user can bypass ownership rules by talking directly to the Fabric API or by using a different client.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-CC-COLLAB-KOR | All | Mycelium Fabric shall enforce ownership-based access control using Owner metadata annotations on model elements when "a user attempts to create, modify, or delete a model element." | - | H |  |
| SSS-PT-COLLAB-G8G | All | Mycelium Fabric shall prevent modification of elements and attributes not owned by the user when "a Participant attempts to edit an element whose Owner metadata does not match their assigned Ownership." | - | H |  |
| SSS-FB-ATT-T4X | All | Mycelium Fabric shall enforce ownership-based access control on attachment operations (upload, delete) consistent with the element's Owner metadata when "a user attempts to modify attachments on a model element." | - | H |  |
| SSS-PT-SUB-R8M | All | Mycelium Fabric shall reject creation of a ParameterSubscription on an AttributeUsage owned by the subscriber's own Ownership when "a Participant attempts to subscribe to an attribute owned by their own Ownership." | - | H |  |

##### 5.2.2.4 Model Validation and Commit Rejection

Mycelium Fabric is the guardian of model well-formedness. It accepts a commit only if the resulting model conforms to the KerML and SysML v2 abstract syntax (metaclass typing, multiplicities, and containment) and satisfies every OCL well-formedness constraint those specifications define on their metaclasses. Conformance to the specification is captured normatively by the first requirement below; the complete and authoritative set of checks is the abstract syntax together with the named OCL constraints in KerML (formal/25-09-03) and SysML v2 (formal/25-09-03), which this document does not restate. The remaining requirements add Mycelium-specific validation that the specifications do not mandate, such as library-package immutability and model-quality warnings.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-FB-VALID-CNF | - | Mycelium Fabric shall reject any commit whose resulting model would violate the KerML or SysML v2 abstract syntax or any OCL well-formedness constraint defined on the affected metaclasses, returning a validation error that identifies the violated constraint and the offending element, when "a client submits a commit." | KerML 8, SysML 8 | H |  |
| SSS-FB-PKG-L2F | - | Mycelium Fabric shall reject any commit that modifies the owned content of a LibraryPackage (including creation, modification, deletion, or re-parenting of any of its members) and shall return a validation error identifying the LibraryPackage, when "a client submits a commit that would mutate a LibraryPackage." | KerML 7.5.5 | H |  |
| SSS-FB-ELEM-CD8 | - | Mycelium Fabric shall reject any commit that introduces a circular composite containment, in which a Definition is directly or transitively the type of a composite Usage that it owns, returning a validation error identifying the cycle, when "a client submits such a commit." | KerML 7.6 | H |  |

##### 5.2.2.5 Real-time notifications

Mycelium Fabric is responsible for propagating model changes to all connected clients in near real-time, enabling the live update behavior in Bloom. The requirements in this section cover the server-side notification mechanism using SignalR.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-CC-COLLAB-TLB | All | Mycelium Fabric shall propagate model changes to all connected users in near real-time when "a user commits changes to the shared model." | API 7 | H |  |

##### 5.2.2.6 Model persistence and versioning

Mycelium Fabric persists model data in a relational (TBC) database with auto-generated schemas from the SysML v2 metamodel. The requirements in this section cover persistence performance and API responsiveness targets.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-CC-PERF-6HL | All | Mycelium Fabric shall persist models with up to 50,000 (TBC) elements within a responsive timeframe (target TBD) when "a user commits changes to a model containing up to 50,000 (TBC) elements." | - | H |  |
| SSS-CC-PERF-WTU | All | Mycelium Fabric shall respond to standard REST API requests within a responsive timeframe (target TBD) when "an external client or the web application issues an API request to the model server." | - | H |  |

##### 5.2.2.7 Concurrent design support

Lock-free collaboration is fundamental to concurrent design, no user can block others from editing the model. The requirements in this section anchor the server-side support for lock-free collaboration with optimistic concurrency.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-CC-COLLAB-62C | All | Mycelium Fabric shall support owner/ownership-based lock-free collaborative modeling where no single user can block others from updating the model when "multiple users concurrently modify different elements within the same project." | - | H |  |

---

#### 5.2.3 Mycelium Forge

##### 5.2.3.1 Package registry

Mycelium Forge is the package registry for the Mycelium ecosystem. It takes its design cues from established, widely-used public registries, **nuget.org**, **Maven Central**, and **PyPI**, and applies them to SysML v2 libraries. Libraries are distributed as **kpar** files (KerML Project Archive, defined in KerML clause 10.3, the Mycelium analogue of `.nupkg`, `.jar`, and `.whl`), each carrying a manifest, the library's KerML/SysML v2 source, a resolved API representation, and optional readme and release notes. The registry is addressable through three independent surfaces that all sit on top of the same backing store: a public web UI for human browsing, a documented HTTP API for programmatic use, and a first-party client library that wraps that API and is embedded directly in Mycelium Bloom so that users can search, preview, import, and update packages without leaving the modelling environment.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-FG-REG-K1A | - | Mycelium Forge shall accept and distribute every published SysML v2 library as a single **kpar** file (KerML Project Archive, defined in KerML clause 10.3) whose structure, manifest, and metadata conform to the kpar specification, when "a client publishes or retrieves a library." | KerML 7.4.14, 8.3.4.13.3 | H |  |
| SSS-FG-REG-S2B | - | Mycelium Forge shall assign every package version a SemVer 2.0.0 string of the form `MAJOR.MINOR.PATCH[-prerelease][+build]` and shall reject any publish request whose version string is not SemVer-compliant, whose version is not strictly greater than every previously published version of the same package, or whose major version change is not accompanied by release notes describing the breaking change, when "an authenticated client submits a publish request." | - | H |  |
| SSS-FG-REG-I3C | - | Mycelium Forge shall treat every accepted `{package identifier, version}` pair as immutable: once published, its kpar content and its manifest shall not be mutated. A correction to a published version requires publishing a new version, when "a client attempts to republish an existing `{package identifier, version}`." | - | H |  |
| SSS-FG-REG-U4D | - | Mycelium Forge shall support unlisting a published version, hiding it from search results and from new-install resolution while continuing to serve it on direct download to existing consumers, without deleting its content, when "an authenticated publisher unlists one of their package versions." | - | M |  |
| SSS-FG-REG-A5E | - | Mycelium Forge shall expose an HTTP API endpoint that accepts a kpar file and publisher credentials and atomically registers the kpar either as the first version of a new package or as a new version of an existing package, returning the assigned download URL and the canonical manifest, when "an authenticated client pushes a kpar to the publish endpoint." | - | H |  |
| SSS-FG-REG-D6F | - | Mycelium Forge shall expose HTTP API endpoints returning the kpar content of a package by `{package identifier, version}` and, given a package identifier alone, the kpar content of the latest non-prerelease, non-unlisted version of that package, when "a client requests a package from the download endpoint." | - | H |  |
| SSS-FG-REG-Q7G | - | Mycelium Forge shall expose an HTTP search API accepting free-text query terms matched against package identifier, display name, description, tags, authors, and the indexed content of the library (element names, qualified names, Metadata Definitions, Quantity Kinds), with paginated results, configurable sort (relevance, downloads, last-updated, name), and optional filters on license, author, tag, and updated-since timestamp, when "a client queries the search endpoint." | - | H |  |
| SSS-FG-REG-M8H | - | Mycelium Forge shall expose HTTP API endpoints returning the full manifest, the complete version list, the dependency graph, and the per-version release notes of a package without requiring the kpar content itself to be downloaded, when "a client queries package metadata." | - | H |  |
| SSS-FG-REG-W9J | IA, OA, PA, PT, VW | Mycelium Forge shall provide a public web-based user interface, reachable by unauthenticated and authenticated users, that supports searching for packages by free-text query matched against package identifier, display name, description, tags, and indexed library content, with paginated and sortable results, when "a user visits the Forge web interface." | - | H |  |
| SSS-FG-REG-X1K | IA, OA, PA, PT, VW | Mycelium Forge's web interface shall present, for each package, a detail page displaying the package identifier, display name, description, README, authors, license, tags, the full version history with per-version release notes and publication dates, dependency declarations, download counts, the latest stable version, and a direct download link for each listed version, when "a user opens a package in the Forge web interface." | - | H |  |
| SSS-FG-REG-Y2L | IA, OA, PA | Mycelium Forge shall authenticate publishing clients using revocable API keys issued from the user's Mycelium account through the Forge web interface, scoping each key to a publisher and to a permitted set of operations (publish, unlist, manage-keys), when "a client submits a publish, unlist, or key-management request." Read access to public packages through the web interface, HTTP API, and download endpoint shall be permitted without authentication. | - | H |  |
| SSS-FG-REG-C3M | - | Mycelium shall provide a first-party Forge client library, consumable by Mycelium Bloom, by CI/CD pipelines, and by third-party tooling, that wraps every Forge HTTP API endpoint and exposes programmatic operations for searching, retrieving metadata, listing versions, downloading a kpar, publishing a kpar, unlisting a version, and managing API credentials, when "a tool integrates with Mycelium Forge." | - | H |  |
| SSS-PA-REG-B4N | PA, PT | Mycelium Bloom shall provide a Forge package picker that issues searches against Mycelium Forge via the first-party Forge client library, presents matching packages with their description, latest stable version, authors, license, and tags, and, upon confirmation, imports the selected package(s) into the current project through the auto-import flow defined by `SSS-PA-PKG-F8M`, `SSS-PA-PKG-X1J`, and `SSS-PA-PKG-X2K`, when "a user opens the 'Import from Forge' dialog." | KerML 7.2.5 | H |  |
| SSS-PA-REG-V5P | PA, PT, VW | Mycelium Bloom shall display, per project, the list of Forge packages currently imported, showing each package's pinned version, latest available stable version, publication date, authors, license, and an explicit 'Update' action when a newer compatible stable version is available, when "a user views the project's Forge dependencies." | KerML 7.4.14 | H |  |
| SSS-PA-REG-N6Q | PA, PT | Mycelium Bloom shall notify the Project Administrator, without automatically upgrading, whenever any Forge package imported into the current project has a newer non-prerelease, non-unlisted version available, when "Mycelium Bloom's Forge client detects an update for any imported package during a session." | - | M |  |

##### 5.2.3.2 Library management

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PA-IE-OYJ | PA | Mycelium Forge shall provide standard SysML v2 libraries (e.g. Quantities and Units, standard view definitions) for import into a project when "the Project Administrator selects one or more standard libraries for import." | SysML 9.8 | H |  |
| SSS-FG-PKG-P7L | - | Mycelium Forge shall publish a LibraryPackage as a versioned, downloadable package (including its metadata, content, and transitive library dependencies) when "a user or CI pipeline submits a LibraryPackage for publication to Mycelium Forge." | - | H |  |

##### 5.2.3.3 Authentication and authorization

Mycelium Forge reuses the identity plumbing that Mycelium Fabric already provides, external identity provider backed Mycelium Accounts and Fabric Organizations, and layers a Forge-specific per-package role model on top. A package has a set of Maintainers drawn from Accounts and Organizations; at least one individual-Account Owner must always exist; ownership is transferable between Accounts and Organizations with explicit acceptance by the receiving party.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-FG-AUTH-S1A | IA, OA, PA, PT, VW | Mycelium Forge shall authenticate users through the same external identity provider used by Mycelium Fabric, accepting a valid Mycelium Account session without requiring Forge-specific registration, when "a user signs in to Mycelium Forge through the web interface, the HTTP API, or the Forge client library." | - | H |  |
| SSS-FG-AUTH-S2B | - | Mycelium Forge shall use a scoped package identifier of the form `@<scope>/<package-name>`, where `<scope>` resolves to the slug of a Mycelium Account or a Fabric Organization, reserving the unscoped (global) namespace for standard libraries distributed by the Mycelium operator, when "a client publishes, queries, or downloads a package." | - | H |  |
| SSS-FG-AUTH-M3C | - | Mycelium Forge shall maintain, for every package, a Maintainer set whose entries are Mycelium Accounts and/or Fabric Organizations, each entry holding the role `Owner` or `Maintainer`. An `Owner` may transfer or share ownership, add or remove Maintainers, and unlist or delete versions. A `Maintainer` may publish new versions and unlist versions but shall not modify the Maintainer set. The package's metadata, display name, description, authors, license, tags, dependencies, and release notes, is sourced from the manifest contained in the kpar of each published version and shall not be edited by any role outside of publishing a new version, when "any authenticated request operates on a package's content or ownership." | - | H |  |
| SSS-FG-AUTH-O4D | - | Mycelium Forge shall reject any operation, removal of a Maintainer, role downgrade, ownership transfer, or Account deletion, that would leave a package with zero individual-Account Owners, and shall require the operation to first install another individual-Account Owner, when "an authenticated client submits a change to a package's Maintainer set or the Mycelium platform deletes an Account that is the last individual Owner of one or more packages." | - | H |  |
| SSS-FG-AUTH-T5E | - | Mycelium Forge shall transfer or share Ownership of a package only after the receiving Account or the receiving Fabric Organization has explicitly accepted the transfer through the Forge web interface or the Forge client library, leaving the original Maintainer set unchanged until acceptance occurs, when "an Owner initiates a transfer of, or an addition to, a package's Owner set." | - | H |  |
| SSS-FG-AUTH-G6F | OA | Mycelium Forge shall accept a Fabric Organization as a Maintainer or Owner of a package, granting publish, unlist, and, when the role is `Owner`, ownership-management authority to the Organization's Organization Administrators and to any Organization Member explicitly granted the `Forge Publisher` role by an Organization Administrator, when "an authenticated member of an Organization that holds such a role submits an operation against the package." | - | H |  |
| SSS-FG-AUTH-P7G | - | Mycelium Forge shall treat a Fabric Organization entry in a package's Maintainer set as a group Owner that does not satisfy the 'at least one individual Owner' invariant of `SSS-FG-AUTH-O4D` on its own; an individual-Account Owner shall remain present alongside any Organization Owner, when "a Maintainer set is established or modified." | - | H |  |
| SSS-FG-AUTH-R9J | OA, PA | Mycelium Forge shall record every privileged operation on a package, publish, unlist, delete, add/remove Maintainer, role change, ownership transfer, API-key issuance and revocation, in an append-only, tamper-evident audit log entry that identifies the acting Mycelium Account, the scope (if any) the action was taken on behalf of, the timestamp, and the affected package version, and shall make that log retrievable by the package's Owners, when "any such operation occurs." | - | M |  |

### 5.3 System interface requirements

This section specifies the interfaces across the Mycelium software boundary, the protocols, identity providers, data formats, human-machine interfaces, and external service integrations through which Mycelium communicates with the outside world. Each requirement below identifies *that* an interface exists and the standards or versions it is expected to comply with. Where a capability is described in §5.2, that description remains the normative capability requirement and this section only pins the interface contract.

KerML-only content is handled through the same SysML v2 abstract-syntax channels: because SysML v2 specialises KerML, any KerML instance is representable in the JSON, XMI, and MessagePack payloads accepted under `SSS-CC-EXT-IN1`, `SSS-CC-EXT-IN2`, and `SSS-CC-EXT-IN3`.

Interfaces between Mycelium Bloom, Mycelium Fabric, and Mycelium Forge, i.e. between the three Mycelium components themselves, are described in §4.4 (Operational environment).

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-CC-EXT-AP1 | - | Mycelium Fabric shall expose the OMG Systems Modelling API and Services (formal/25-09-04) as its primary external programmatic interface over HTTPS with TLS 1.2 or later, when "an external client issues API requests to Mycelium Fabric." | API 7 | H |  |
| SSS-CC-EXT-WS1 | - | Mycelium Fabric shall deliver near-real-time model-change notifications over a WebSocket transport (SignalR), protected by TLS 1.2 or later and authenticated with the same session credentials as the REST API, when "a client subscribes to live updates from Mycelium Fabric." | - | H |  |
| SSS-CC-EXT-FG1 | - | Mycelium Forge shall expose the Forge HTTP API specified in §5.2.3.1 over HTTPS with TLS 1.2 or later, when "a client interacts with Mycelium Forge programmatically." | - | H |  |
| SSS-CC-EXT-ID1 | - | Mycelium Fabric and Mycelium Forge shall authenticate users through OIDC 1.0 sessions brokered by a external identity provider. SAML 2.0 and LDAP v3 back-ends are supported transitively through Keycloak's upstream identity federation and are not directly terminated by Mycelium, when "an identity provider is configured for a Mycelium installation." | - | H |  |
| SSS-CC-EXT-IN1 | - | Mycelium Fabric shall ingest SysML v2 abstract-syntax instances serialised as JSON conforming to OMG formal/25-09-03, when "a client submits a SysML v2 JSON abstract-syntax payload to Mycelium Fabric." | KerML 10.4 | H |  |
| SSS-CC-EXT-IN3 | - | Mycelium Fabric shall ingest SysML v2 abstract-syntax instances serialised as MessagePack, carrying the same content as the JSON abstract-syntax payload in `SSS-CC-EXT-IN1`, when "a client submits a SysML v2 MessagePack payload to Mycelium Fabric." | - | H |  |
| SSS-CC-EXT-IN4 | - | Mycelium Fabric shall ingest ReqIF 1.2 for requirements import, preserving attribute types, enumerations, and structural hierarchy, when "a client submits a ReqIF document to Mycelium Fabric." | - | H |  |
| SSS-CC-EXT-IN5 | - | Mycelium Fabric shall ingest ECSS-E-TM-10-25 Annex C.3 payloads produced for migration into SysML v2 projects, when "a client submits a ECSS-E-TM-10-25 Annex C.3 archive to Mycelium Fabric." | - | H |  |
| SSS-CC-EXT-EG1 | - | Mycelium Fabric shall emit, upon request, SysML v2 abstract-syntax instances serialised as JSON, XMI, or MessagePack; SysML v2 textual notation and KerML textual notation rendered as a one-way representation of the abstract syntax (not intended for round-trip ingest). | - | H |  |
| SSS-CC-EXT-BR1 | - | Mycelium Bloom shall operate correctly on the latest two major versions of Google Chrome, Mozilla Firefox, Apple Safari, and Microsoft Edge running on Windows, macOS, and Linux, when "a user accesses Mycelium Bloom through a supported web browser." | - | H |  |
| SSS-CC-EXT-NO1 | - | Mycelium Fabric shall deliver notifications through HTTP webhook channels, with webhook payloads signed using a shared secret and delivered over TLS 1.2 or later, when "a notification channel has been configured for an organisation." | - | H |  |
| SSS-CC-EXT-OB1 | - | Mycelium Fabric and Mycelium Forge shall expose a Prometheus-compatible `/metrics` endpoint and an OpenTelemetry OTLP exporter for traces and metrics, authenticated through the installation's observability credentials, when "a metrics or tracing collector polls or receives from the endpoint." | - | H |  |

### 5.4 Adaptation and missionization requirements

Missionization covers the set of adaptations that turn a generic Mycelium installation into one that fits a specific programme, customer, or mission without recompiling or modifying source code. The axes below, deployment model, identity integration, SysML v2 library catalogue, localisation, retention, and notification, must all be configurable through declarative configuration or administrator-facing interfaces. Programme-specific model content (custom Metadata Definitions, custom Viewpoint / View / Rendering Definitions, custom libraries) is not an adaptation axis: it is authored and distributed like any other model content through §5.2.1 and Mycelium Forge (§5.2.3). Project-level adaptation (Regular vs Concurrent Design mode, per project) is covered by `SSS-PA-MGMT-73C` in §5.2.1.2 and is not repeated here.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-CC-ADAPT-G1P | IA | Mycelium shall read its runtime configuration from a declarative source (configuration file, environment variable, or installation-administrator interface) and apply configuration changes when "an Installation Administrator modifies a configuration value." | - | H |  |
| SSS-CC-ADAPT-A3R | IA | Mycelium Fabric shall integrate with an external identity provider whose authentication backends (JWT, OIDC, LDAP, SAML) are configured at the installation level when "an Installation Administrator configures the identity-provider backend." | - | H |  |
| SSS-CC-ADAPT-I7V | PA, PT, VW | Mycelium Bloom shall render its user interface in the locale selected by the user, applying localised strings, date/number formats, selected from the set of locales installed at the deployment, when "a user selects a locale in their profile settings." | - | L |  |
| SSS-CC-ADAPT-R9X | OA | Mycelium Fabric shall apply per-organisation data retention and archival policies, including commit history retention, audit log retention, and archived-project lifetime, configured at the organisation level, when "an Organisation Administrator configures the retention policy." | - | M |  |
| SSS-CC-ADAPT-N1Y | OA | Mycelium Fabric shall deliver model-change notifications through configurable channels (email, webhook) whose endpoints and filters are set per organisation, when "an Organisation Administrator configures a notification channel." | - | M |  |

### 5.5 Computer resource requirements

#### 5.5.1 Computer hardware resource requirements

TBD: Minimum server specifications, browser hardware requirements.

#### 5.5.2 Computer hardware resource utilization requirements

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-CC-PERF-CDT | All | Mycelium Bloom shall load a model with up to 10,000 elements within a responsive timeframe (target TBD) when "a user opens a project containing up to 10,000 model elements." | - | H |  |
| SSS-CC-PERF-NGA | All | Mycelium Bloom shall reflect model edits in the UI within a responsive timeframe (target TBD) when "a user or a collaborating user commits a model change." | - | H |  |
| SSS-CC-PERF-EIU | All | Mycelium Bloom shall render diagrams with 100+ elements within a responsive timeframe (target TBD) when "a user opens a diagram view containing 100 or more graphical elements." | - | H |  |

#### 5.5.3 Computer software resource requirements

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-CC-WEB-1MV | All | The Mycelium platform shall be deployable as a cloud-native containerized service when "a system operator deploys the application using container orchestration tools." | - | H |  |

### 5.6 Security requirements

The complete role and permission model is defined in [Roles and Permissions](Roles-and-Permissions.md).

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-OA-AUTH-B7A | OA | Mycelium Bloom shall provide a configuration interface for authentication mechanisms (JWT, OIDC, LDAP, SAML) when "the Organization Administrator accesses the authentication settings interface." | - | M |  |
| SSS-VW-AC-R7Y | All | Mycelium Bloom shall prevent all create, modify, and delete operations on model elements when "the authenticated user has the Viewer role." | - | H |  |
| SSS-VW-AC-VKZ | All | Mycelium Bloom shall restrict project and view access to only those projects the Viewer has been granted access to when "a Viewer attempts to open a project." | - | H |  |
| SSS-IA-SEC-P5G | All | Mycelium Bloom shall present the installation administration interface only to users with the Installation Administrator role when "a user navigates to the application." | - | H |  |
| SSS-CC-SUP-SBM | - | The Mycelium platform shall publish, for every released Mycelium Fabric and Mycelium Forge container image, a Software Bill of Materials (SBOM) in a standard machine-readable format (SPDX or CycloneDX) that enumerates the bundled software components with their versions and licenses, when "a Mycelium Fabric or Mycelium Forge container image is released." | - | H |  |

Additional security requirements (data-at-rest encryption, audit logging, vulnerability management): TBD.

### 5.7 Safety requirements

Not applicable. The Mycelium platform is a web-based engineering tool and does not perform safety-critical functions.

### 5.8 Reliability and availability requirements

Not applicable at this point in time. The Mycelium platform is developed as part of a TRL6 activity.

### 5.9 Quality requirements

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-PT-UI-YJL | All | XXX | - | L |  |
| SSS-CC-QUAL-AX2 | All | XXX | - | L |  |


### 5.10 Design requirements and constraints

Other than the requirements specified in other sections, there are no specific design requirements at this stage.

### 5.11 Software operations requirements

Not applicable at this point in time. The Mycelium platform is developed as part of a TRL6 activity.

### 5.12 Software maintenance requirements

Not applicable at this point in time. The Mycelium platform is developed as part of a TRL6 activity.

### 5.13 System and software observability requirements

The Mycelium platform must be operable in both SaaS and on-premise deployments, which imposes a minimum observability baseline: structured logs with correlation, distributed traces spanning Bloom, Fabric, and Forge, machine-readable metrics, health endpoints for orchestrators, an append-only security audit trail, user-facing correlation identifiers, progress telemetry for long-running operations, and disciplined retention and privacy on everything that is emitted.

| ID | Roles | Requirement | Ref | Prio | Effort |
|----|-------|-------------|-----|------|--------|
| SSS-FB-OBS-S1A | - | Mycelium Fabric and Mycelium Forge shall emit every server log line as a structured JSON (TBC) record that includes at minimum an ISO 8601 timestamp, a log level, a trace identifier, a span identifier, the user identifier (when known), the organisation and project identifiers (when applicable), and a correlation identifier propagated from the originating request, when "any server-side component writes a log entry." | - | H |  |
| SSS-FB-OBS-D2B | - | Mycelium Fabric and Mycelium Forge shall emit OpenTelemetry-compatible distributed traces covering inbound HTTP requests, outbound calls between Bloom, Fabric, Forge, and the database, background jobs, and SignalR notification flows, when "a request or background task executes." | - | H |  |
| SSS-FB-OBS-M3C | - | Mycelium Fabric and Mycelium Forge shall expose a Prometheus-compatible `/metrics` endpoint publishing counters, gauges, and histograms for request rates, request latency, error rates, commit throughput, active SignalR connections, queue depths, and resource utilisation, when "a metrics scraper polls the endpoint." | - | H |  |
| SSS-FB-OBS-H4D | - | Mycelium Fabric and Mycelium Forge shall expose HTTP `/healthz` (TBC) and `/ready` (TBC) endpoints returning a success status when the component is alive and ready to serve traffic and an error status with a machine-readable reason otherwise, when "an orchestrator or load balancer probes the component." | - | H |  |
| SSS-FB-OBS-A5E | - | Mycelium Fabric shall record every security-relevant event, authentication success and failure, session creation and termination, role or permission changes, organisation and project lifecycle events, ownership reassignments, and configuration changes, into an append-only audit log that is tamper-evident (TBC), retrievable by authorised Installation Administrators, and retained per the organisation retention policy, when "any such event occurs." | - | M |  |
| SSS-PA-OBS-E6F | PA, PT, VW | Mycelium Bloom shall display, on every user-facing error or failure dialog, the correlation identifier of the failing request and a one-click action to copy it to the clipboard, so that the user can include it in a support request, when "Mycelium Bloom surfaces an error to the user." | - | H |  |
| SSS-FB-OBS-P7G | - | Mycelium Fabric shall publish progress events, start, percentage complete, stage, and terminal success/failure status, for long-running operations (commit creation, merge, library import, migration, package publication) over SignalR so that Bloom can display progress to the initiating user, when "a long-running operation executes." | - | M |  |
| SSS-FB-OBS-R8H | - | Mycelium Fabric and Mycelium Forge shall scrub authentication credentials, session tokens, personal data beyond what the audit log requires, and any attribute values annotated as sensitive from all structured logs and traces, and shall enforce a per-deployment retention bound on log and trace storage, when "any component emits telemetry." | - | H |  |

---

## 6. Verification, validation and system integration

Verification and Validation are described in the SValP.

---

## 7. System models

N.A.

## 8. SysML v2 and KerML concept coverage matrix

This appendix mirrors the complete KerML and SysML v2 metamodel and cross-references every metaclass against the SSS requirements that cover it. The class list, package structure, and abstractness are taken from the abstract syntax, comprising 175 metaclasses across the KerML (Root, Core, Kernel) and SysML (Systems) packages, plus the 7 metamodel enumerations listed in 8.2.

**Columns.**

- **Concept**: the KerML or SysML v2 metaclass name as published in the OMG specifications (KerML formal/25-09-01, SysML v2 formal/25-09-03) and the UML model.
- **Package**: the metaclass's owning package in the metamodel, for example `SysML::Systems::Flows`.
- **Scope**: In (surfaced in Bloom and covered by one or more requirements), Deferred (planned beyond early-phase MBSE), or Out (implemented in the native metamodel per `SSS-CC-STD-UZA`, and therefore persisted and queryable, but not surfaced as a distinct concept in Bloom; users work with the concrete Definition/Usage pairs that specialise these foundational classes). Out rows carry `-` in both coverage columns, or `NA` when the class is also an abstract anchor.
- **Abstract syntax**: SSS requirement identifiers covering the metaclass at the abstract-syntax level (representation, persistence, query, creation, modification, deletion, validation).
- **UX / notation**: SSS requirement identifiers covering the user-facing surface area (browsers, tabular views, diagrams and diagram notation per SysML v2 section 8.2.3, dashboards, property editors, tooltips).

Multiple requirement identifiers are comma-separated.

**Abstract metaclasses (marked with †).** Rows whose concept name carries `†` are flagged `isAbstract = true` in the published UML model and cannot be instantiated directly; they carry `NA` in both coverage columns, because coverage is provided by their concrete subclasses. The published model flags only eight classes abstract (`Element`, `Relationship`, `Import`, `ConnectorAsUsage`, `ControlNode`, `LoopActionUsage`, `Expose`, `InstantiationExpression`); several conceptually-abstract bases such as `Feature`, `Type`, `Definition`, `Usage`, and `Membership` are modelled as concrete and therefore appear without `†`.

### 8.1 SysML v2 - Metaclasses

| Concept | Package | Scope | Abstract syntax | UX / notation |
| --- | --- | --- | --- | --- |
| AcceptActionUsage | SysML::Systems::Actions | In | SSS-PA-BEH-A1C | SSS-PA-VIS-M1Z |
| ActionDefinition | SysML::Systems::Actions | In | SSS-PA-BEH-N5Z | SSS-PA-VIS-SMC, SSS-PA-VIS-E4R, SSS-PA-VIS-M1Z |
| ActionUsage | SysML::Systems::Actions | In | SSS-PA-BEH-N5Z, SSS-PA-BEH-WG5, SSS-PA-BEH-H83 | SSS-PA-VIS-SMC, SSS-PA-VIS-E4R, SSS-PA-VIS-J6N, SSS-PA-VIS-M1Z |
| ActorMembership | SysML::Systems::Requirements | In | SSS-PA-REQ-M3N | SSS-PA-REQ-RF1 |
| AllocationDefinition | SysML::Systems::Allocations | In | SSS-PA-TRACE-AD1 | SSS-PA-TRACE-AR4, SSS-PA-TRACE-IKS |
| AllocationUsage | SysML::Systems::Allocations | In | SSS-PA-TRACE-YWQ, SSS-PA-TRACE-NA2 | SSS-PA-TRACE-AR4, SSS-PA-TRACE-AP5, SSS-PA-TRACE-IKS |
| AnalysisCaseDefinition | SysML::Systems::AnalysisCases | In | SSS-PA-AV-QII | SSS-PA-AV-CR1 |
| AnalysisCaseUsage | SysML::Systems::AnalysisCases | In | SSS-PA-AV-AU1 | SSS-PA-AV-CR1 |
| AnnotatingElement | KerML::Root::Annotations | In | SSS-PA-CMT-L7X, SSS-PA-CMT-Z9K | SSS-PA-CMT-L7X |
| Annotation | KerML::Root::Annotations | In | SSS-PA-CMT-L7X, SSS-PA-CMT-Z9K | SSS-PA-CMT-L7X |
| AssertConstraintUsage | SysML::Systems::Constraints | In | SSS-PA-AV-CU3, SSS-PA-AV-CN5 | SSS-PT-ANALYSIS-EAJ |
| AssignmentActionUsage | SysML::Systems::Actions | In | SSS-PA-BEH-A3S | SSS-PA-VIS-M1Z |
| Association | KerML::Kernel::Associations | Out | - | - |
| AssociationStructure | KerML::Kernel::Associations | Out | - | - |
| AttributeDefinition | SysML::Systems::Attributes | In | SSS-PA-ARCH-97Z, SSS-PA-QU-H2V, SSS-PA-QU-K6F, SSS-PA-CONST-N7K, SSS-PA-CONST-D3V, SSS-PA-CONST-W8F, SSS-PA-META-K7R, SSS-PA-GLOSS-T5R | SSS-PA-QU-T3K, SSS-PA-QU-R7N, SSS-PA-QU-W5J, SSS-PA-QU-D8M, SSS-PA-CONST-D3V, SSS-PA-CONST-J5M, SSS-PA-CONST-R2H |
| AttributeUsage | SysML::Systems::Attributes | In | SSS-PA-ARCH-97Z, SSS-PT-DATA-I9M, SSS-PT-DATA-OH2, SSS-PT-DATA-492, SSS-PA-QU-N9X, SSS-PT-COLLAB-8U9, SSS-PT-PUB-K4W, SSS-PT-PUB-R7N, SSS-PT-PUB-H8J | SSS-PA-NAV-ZRW, SSS-PA-HIST-K3R, SSS-PA-HIST-T6W, SSS-PA-HIST-V2P |
| Behavior | KerML::Kernel::Behaviors | Out | - | - |
| BindingConnector | KerML::Kernel::Connectors | Out | - | - |
| BindingConnectorAsUsage | SysML::Systems::Connections | Out | - | - |
| BooleanExpression | KerML::Kernel::Functions | In | SSS-PA-EXPR-X1A, SSS-FB-VALID-CNF | SSS-PA-EXPR-X3C |
| CalculationDefinition | SysML::Systems::Calculations | In | SSS-PT-ANALYSIS-4W2 | SSS-PA-EXPR-X1A, SSS-PA-EXPR-X3C |
| CalculationUsage | SysML::Systems::Calculations | In | SSS-PT-ANALYSIS-KU4, SSS-PT-ANALYSIS-KE6 | SSS-PA-EXPR-X3C, SSS-PA-EXPR-X5E |
| CaseDefinition | SysML::Systems::Cases | Deferred | TBC | TBC |
| CaseUsage | SysML::Systems::Cases | Deferred | TBC | TBC |
| Class | KerML::Kernel::Classes | Out | - | - |
| Classifier | KerML::Core::Classifiers | Out | - | - |
| CollectExpression | KerML::Kernel::Expressions | In | SSS-PA-EXPR-X1A, SSS-FB-VALID-CNF | SSS-PA-EXPR-X3C |
| Comment | KerML::Root::Annotations | In | SSS-PA-CMT-R4K, SSS-PA-CMT-M6J, SSS-PA-CMT-T9F, SSS-PA-CMT-K2B, SSS-PA-CMT-D5P, SSS-PA-CMT-N8V, SSS-PA-CMT-L7X, SSS-PA-CMT-Z9K | SSS-PA-VIS-F8Q, SSS-PA-VIS-B2M, SSS-PA-VIS-T1J, SSS-PA-VIS-G5R, SSS-PA-CMT-L7X |
| ConcernDefinition | SysML::Systems::Requirements | In | SSS-PA-REQ-SUC | SSS-PA-REQ-RF1 |
| ConcernUsage | SysML::Systems::Requirements | In | SSS-PA-REQ-SUC | SSS-PA-REQ-RF1 |
| ConjugatedPortDefinition | SysML::Systems::Ports | In | SSS-PA-ARCH-K7M, SSS-FB-VALID-CNF | SSS-PA-ELEM-M6N |
| ConjugatedPortTyping | SysML::Systems::Ports | Out | - | - |
| Conjugation | KerML::Core::Types | In | SSS-PA-ARCH-K7M, SSS-PA-ELEM-J4K, SSS-FB-VALID-CNF | SSS-PA-ELEM-D8K, SSS-PA-ELEM-M6N |
| ConnectionDefinition | SysML::Systems::Connections | In | SSS-PA-ARCH-IGA | SSS-PA-VIS-W3T, SSS-PA-VIS-G8N |
| ConnectionUsage | SysML::Systems::Connections | In | SSS-PA-ARCH-IGA, SSS-PA-ARCH-Y2D | SSS-PA-VIS-W3T, SSS-PA-VIS-G8N |
| Connector | KerML::Kernel::Connectors | Out | - | - |
| ConnectorAsUsage † | SysML::Systems::Connections | Out | NA | NA |
| ConstraintDefinition | SysML::Systems::Constraints | In | SSS-PA-AV-LSX, SSS-PT-ANALYSIS-NWL | SSS-PA-EXPR-X1A, SSS-PA-EXPR-X3C |
| ConstraintUsage | SysML::Systems::Constraints | In | SSS-PA-AV-CU3, SSS-PA-AV-CN5, SSS-PT-ANALYSIS-NWL, SSS-PT-ANALYSIS-EAJ, SSS-PA-SCRIPT-K8B | SSS-PT-ANALYSIS-EAJ, SSS-PA-EXPR-X3C |
| ConstructorExpression | KerML::Kernel::Expressions | In | SSS-PA-EXPR-X1A, SSS-FB-VALID-CNF | SSS-PA-EXPR-X3C |
| ControlNode † | SysML::Systems::Actions | In | SSS-PA-BEH-WG5, SSS-FB-BEH-C7F | SSS-PA-VIS-SMC, SSS-PA-VIS-E4R |
| CrossSubsetting | KerML::Core::Features | In | SSS-PA-ELEM-C5X | SSS-PA-ELEM-D8K |
| DataType | KerML::Kernel::DataTypes | Out | - | - |
| DecisionNode | SysML::Systems::Actions | In | SSS-PA-BEH-WG5, SSS-FB-BEH-C7F | SSS-PA-VIS-SMC, SSS-PA-VIS-E4R |
| Definition | SysML::Systems::DefinitionAndUsage | Out | - | - |
| Dependency | KerML::Root::Dependencies | In | SSS-PA-REQ-V4J, SSS-PA-TRACE-V8K | SSS-PA-REQ-RF2, SSS-PA-TRACE-RX1, SSS-PA-TRACE-IKS |
| Differencing | KerML::Core::Types | Deferred | TBC | TBC |
| Disjoining | KerML::Core::Types | Deferred | TBC | TBC |
| Documentation | KerML::Root::Annotations | In | SSS-PA-CMT-W7N, SSS-PA-CMT-H3D, SSS-PA-CMT-M6J, SSS-PA-CMT-L7X, SSS-PA-CMT-Z9K | SSS-PA-CMT-L7X |
| Element † | KerML::Root::Elements | In | NA | NA |
| ElementFilterMembership | KerML::Kernel::Packages | In | SSS-PA-PKG-J3W | SSS-PA-PKG-L6D |
| EndFeatureMembership | KerML::Core::Features | In | SSS-PA-ELEM-E5N | SSS-PA-ELEM-E5N, SSS-PA-ELEM-O2K |
| EnumerationDefinition | SysML::Systems::Enumerations | In | SSS-PA-ARCH-9W5, SSS-PA-ARCH-E1A, SSS-PA-ARCH-E2B, SSS-FB-VALID-CNF | SSS-PA-VIS-E3C |
| EnumerationUsage | SysML::Systems::Enumerations | In | SSS-PA-ARCH-9W5, SSS-PA-ARCH-E4D, SSS-FB-VALID-CNF | SSS-PA-VIS-E3C, SSS-PA-ARCH-E4D |
| EventOccurrenceUsage | SysML::Systems::Occurrences | In | SSS-PA-OCC-U2, SSS-PA-BEH-H83 | SSS-PA-OCC-R9 |
| ExhibitStateUsage | SysML::Systems::States | In | SSS-PA-BEH-H83 | SSS-PA-VIS-SH7 |
| Expose † | SysML::Systems::Views | In | SSS-PA-VIS-K9R | SSS-PA-ELEM-O2K |
| Expression | KerML::Kernel::Functions | In | SSS-PA-EXPR-X1A, SSS-PA-EXPR-X2B, SSS-FB-VALID-CNF, SSS-PA-EXPR-X5E | SSS-PA-EXPR-X3C |
| Feature | KerML::Core::Features | Out | - | - |
| FeatureChainExpression | KerML::Kernel::Expressions | In | SSS-PA-EXPR-X1A, SSS-FB-VALID-CNF | SSS-PA-EXPR-X3C |
| FeatureChaining | KerML::Core::Features | In | SSS-PA-EXPR-X2B | SSS-PA-EXPR-X3C |
| FeatureInverting | KerML::Core::Features | Deferred | TBC | TBC |
| FeatureMembership | KerML::Core::Types | In | SSS-PA-ELEM-F4M | SSS-PA-ELEM-F4M, SSS-PA-ELEM-O2K |
| FeatureReferenceExpression | KerML::Kernel::Expressions | In | SSS-PA-EXPR-X2B | SSS-PA-EXPR-X3C |
| FeatureTyping | KerML::Core::Features | In | SSS-PA-ELEM-F3T | SSS-PA-ELEM-D8K |
| FeatureValue | KerML::Kernel::FeatureValues | In | SSS-PA-ARCH-AV5, SSS-PT-DATA-D5I | SSS-PA-NAV-ZRW |
| Flow | KerML::Kernel::Interactions | Out | - | - |
| FlowDefinition | SysML::Systems::Flows | In | SSS-PA-BEH-PC7 | SSS-PA-VIS-W3T, SSS-PA-VIS-G8N |
| FlowEnd | KerML::Kernel::Interactions | Out | - | - |
| FlowUsage | SysML::Systems::Flows | In | SSS-PA-BEH-PC7, SSS-PA-BEH-Q4N, SSS-PA-BEH-D6L, SSS-PA-BEH-X9V | SSS-PA-VIS-W3T, SSS-PA-VIS-G8N, SSS-PA-VIS-I6T |
| ForkNode | SysML::Systems::Actions | In | SSS-PA-BEH-WG5, SSS-FB-BEH-C7F | SSS-PA-VIS-SMC, SSS-PA-VIS-E4R |
| ForLoopActionUsage | SysML::Systems::Actions | In | SSS-PA-BEH-F6L | SSS-PA-VIS-M1Z |
| FramedConcernMembership | SysML::Systems::Requirements | In | SSS-PA-REQ-SUC | SSS-PA-REQ-RF1 |
| Function | KerML::Kernel::Functions | Out | - | - |
| IfActionUsage | SysML::Systems::Actions | In | SSS-PA-BEH-I4F | SSS-PA-VIS-M1Z |
| Import † | KerML::Root::Namespaces | In | SSS-PA-PKG-D4N, SSS-PA-PKG-A7Q, SSS-PA-PKG-H3W, SSS-PA-PKG-X8C, SSS-PA-PKG-X1J, SSS-PA-PKG-X2K, SSS-PA-PKG-X3L, SSS-PA-PKG-X4M, SSS-FB-VALID-CNF | SSS-PA-PKG-L6D, SSS-PA-PKG-X2K |
| IncludeUseCaseUsage | SysML::Systems::UseCases | In | SSS-PA-BEH-T7P | SSS-PA-VIS-UC2 |
| IndexExpression | KerML::Kernel::Expressions | In | SSS-PA-EXPR-X1A, SSS-FB-VALID-CNF | SSS-PA-EXPR-X3C |
| InstantiationExpression † | KerML::Kernel::Expressions | Out | NA | NA |
| Interaction | KerML::Kernel::Interactions | Out | - | - |
| InterfaceDefinition | SysML::Systems::Interfaces | In | SSS-PA-ARCH-IGA | SSS-PA-VIS-Q7K |
| InterfaceUsage | SysML::Systems::Interfaces | In | SSS-PA-ARCH-IGA | SSS-PA-VIS-Q7K |
| Intersecting | KerML::Core::Types | Deferred | TBC | TBC |
| Invariant | KerML::Kernel::Functions | Out | - | - |
| InvocationExpression | KerML::Kernel::Expressions | In | SSS-PA-EXPR-X1A, SSS-FB-VALID-CNF | SSS-PA-EXPR-X3C |
| ItemDefinition | SysML::Systems::Items | In | SSS-PA-ARCH-B2D, SSS-PA-GLOSS-T5R | SSS-PA-GLOSS-K2W, SSS-PA-GLOSS-M3J, SSS-PA-GLOSS-V9D, SSS-PA-GLOSS-F6B, SSS-PA-VIS-I4R, SSS-PA-VIS-I5S |
| ItemUsage | SysML::Systems::Items | In | SSS-PA-ARCH-B2D | SSS-PA-VIS-I4R, SSS-PA-VIS-I5S, SSS-PA-VIS-I6T |
| JoinNode | SysML::Systems::Actions | In | SSS-PA-BEH-WG5, SSS-FB-BEH-C7F | SSS-PA-VIS-SMC, SSS-PA-VIS-E4R |
| LibraryPackage | KerML::Kernel::Packages | In | SSS-PA-QU-G1W, SSS-PA-IE-OYJ, SSS-PA-PKG-P8D, SSS-PA-PKG-S1E, SSS-FB-PKG-L2F, SSS-PA-PKG-F8M, SSS-FG-PKG-P7L | SSS-PA-PKG-V4H, SSS-PA-PKG-M3G |
| LiteralBoolean | KerML::Kernel::Expressions | In | SSS-PA-EXPR-X1A, SSS-FB-VALID-CNF | SSS-PA-EXPR-X3C |
| LiteralExpression | KerML::Kernel::Expressions | In | SSS-PA-EXPR-X1A, SSS-FB-VALID-CNF | SSS-PA-EXPR-X3C |
| LiteralInfinity | KerML::Kernel::Expressions | In | SSS-PA-EXPR-X1A, SSS-FB-VALID-CNF | SSS-PA-EXPR-X3C |
| LiteralInteger | KerML::Kernel::Expressions | In | SSS-PA-EXPR-X1A, SSS-FB-VALID-CNF | SSS-PA-EXPR-X3C |
| LiteralRational | KerML::Kernel::Expressions | In | SSS-PA-EXPR-X1A, SSS-FB-VALID-CNF | SSS-PA-EXPR-X3C |
| LiteralString | KerML::Kernel::Expressions | In | SSS-PA-EXPR-X1A, SSS-FB-VALID-CNF | SSS-PA-EXPR-X3C |
| LoopActionUsage † | SysML::Systems::Actions | Out | NA | NA |
| Membership | KerML::Root::Namespaces | In | SSS-PA-PKG-H6T, SSS-PA-PKG-Q1M | SSS-PA-ELEM-O2K |
| MembershipExpose | SysML::Systems::Views | In | SSS-PA-VIS-K9R | SSS-PA-ELEM-O2K |
| MembershipImport | KerML::Root::Namespaces | In | SSS-PA-PKG-D4N, SSS-PA-PKG-M5P, SSS-PA-PKG-X1J | SSS-PA-PKG-L6D, SSS-PA-PKG-X2K |
| MergeNode | SysML::Systems::Actions | In | SSS-PA-BEH-WG5 | SSS-PA-VIS-SMC, SSS-PA-VIS-E4R |
| Metaclass | KerML::Kernel::Metadata | Out | - | - |
| MetadataAccessExpression | KerML::Kernel::Expressions | In | SSS-PA-EXPR-X1A, SSS-FB-VALID-CNF | SSS-PA-EXPR-X3C |
| MetadataDefinition | SysML::Systems::Metadata | In | SSS-PA-META-K7R | SSS-PA-META-R9V |
| MetadataFeature | KerML::Kernel::Metadata | In | SSS-PA-META-K7R, SSS-PA-CMT-L7X, SSS-PA-CMT-Z9K | SSS-PA-CMT-L7X |
| MetadataUsage | SysML::Systems::Metadata | In | SSS-PA-META-W3D, SSS-PA-META-N8F, SSS-PA-META-H2T, SSS-PA-META-D5J, SSS-PA-META-T4K, SSS-PA-META-M6W, SSS-PA-META-J1B, SSS-PA-META-V8G, SSS-PT-PUB-B9G | SSS-PA-META-R9V, SSS-PA-META-T4K, SSS-PA-META-M6W, SSS-PA-META-V8G, SSS-PA-VIS-B4F |
| Multiplicity | KerML::Core::Types | In | SSS-PA-ELEM-V7K, SSS-PA-ELEM-O1Q, SSS-PA-ELEM-V3W | SSS-PA-VIS-U7M |
| MultiplicityRange | KerML::Kernel::Multiplicities | In | SSS-PA-ELEM-V7K, SSS-PA-ELEM-N8P, SSS-FB-VALID-CNF | SSS-PA-VIS-U7M |
| Namespace | KerML::Root::Namespaces | In | SSS-PA-PKG-H6T, SSS-PA-PKG-V8N, SSS-PA-PKG-T5C, SSS-FB-PKG-W2M, SSS-FB-VALID-CNF, SSS-PA-PKG-C7B, SSS-PA-ELEM-R3G, SSS-PA-ELEM-M9T | SSS-PA-NAV-F3K, SSS-PA-PKG-V8N, SSS-PA-PKG-T5C, SSS-PA-NAV-S6P, SSS-PA-NAV-B8D |
| NamespaceExpose | SysML::Systems::Views | In | SSS-PA-VIS-K9R | SSS-PA-ELEM-O2K |
| NamespaceImport | KerML::Root::Namespaces | In | SSS-PA-PKG-D4N, SSS-PA-PKG-N4J, SSS-PA-PKG-R9K, SSS-PA-PKG-X1J | SSS-PA-PKG-L6D, SSS-PA-PKG-X2K |
| NullExpression | KerML::Kernel::Expressions | In | SSS-PA-EXPR-X1A, SSS-FB-VALID-CNF | SSS-PA-EXPR-X3C |
| ObjectiveMembership | SysML::Systems::Cases | In | SSS-PA-AV-O9U | SSS-PA-ELEM-O2K |
| OccurrenceDefinition | SysML::Systems::Occurrences | In | SSS-PA-OCC-H0, SSS-PA-OCC-D1, SSS-PA-OCC-L3, SSS-PA-OCC-T5, SSS-PA-OCC-S6, SSS-PA-OCC-V8, SSS-PA-OCC-I7 | SSS-PA-OCC-R9 |
| OccurrenceUsage | SysML::Systems::Occurrences | In | SSS-PA-OCC-U2, SSS-PA-OCC-T5, SSS-PA-OCC-S6, SSS-PA-OCC-V8, SSS-PA-OCC-I7 | SSS-PA-OCC-R9 |
| OperatorExpression | KerML::Kernel::Expressions | In | SSS-PA-EXPR-X1A, SSS-FB-VALID-CNF | SSS-PA-EXPR-X3C |
| OwningMembership | KerML::Root::Namespaces | In | NA | NA |
| Package | KerML::Kernel::Packages | In | SSS-PA-PKG-R8W, SSS-PA-PKG-V2J, SSS-PA-PKG-M3G | SSS-PA-PKG-L6D |
| ParameterMembership | KerML::Kernel::Behaviors | In | SSS-PA-ELEM-P6Q | SSS-PA-ELEM-P6Q, SSS-PA-ELEM-O2K |
| PartDefinition | SysML::Systems::Parts | In | SSS-PA-ARCH-JQH, SSS-PA-ARCH-TB2, SSS-PA-ELEM-K4T, SSS-PA-ELEM-R8V, SSS-PA-ELEM-T2N, SSS-PA-ELEM-D7M, SSS-PA-ELEM-W4F | SSS-PA-VIS-W3T, SSS-PA-NAV-8IB, SSS-PA-NAV-G5X |
| PartUsage | SysML::Systems::Parts | In | SSS-PA-ARCH-JQH, SSS-PA-ARCH-TB2, SSS-PA-VAR-K3T | SSS-PA-VIS-W3T, SSS-PA-VIS-M2K, SSS-PA-VIS-R3F |
| PayloadFeature | KerML::Kernel::Interactions | In | SSS-PA-BEH-PC7 | SSS-PA-VIS-I6T |
| PerformActionUsage | SysML::Systems::Actions | In | SSS-PA-BEH-H83 | SSS-PA-VIS-SMC, SSS-PA-VIS-E4R |
| PortConjugation | SysML::Systems::Ports | Out | - | - |
| PortDefinition | SysML::Systems::Ports | In | SSS-PA-ARCH-5RR | SSS-PA-VIS-W3T |
| PortUsage | SysML::Systems::Ports | In | SSS-PA-ARCH-5RR, SSS-PA-ARCH-K7M, SSS-PA-VAR-K3T | SSS-PA-VIS-W3T |
| Predicate | KerML::Kernel::Functions | Out | - | - |
| Redefinition | KerML::Core::Features | In | SSS-PA-ELEM-H9W, SSS-FB-VALID-CNF | SSS-PA-ELEM-D8K |
| ReferenceSubsetting | KerML::Core::Features | In | SSS-PA-ELEM-R4S | SSS-PA-ELEM-D8K |
| ReferenceUsage | SysML::Systems::DefinitionAndUsage | In | SSS-PA-ELEM-RU1, SSS-PA-ELEM-U3G | SSS-PA-ELEM-RU2 |
| Relationship † | KerML::Root::Elements | In | SSS-PA-TRACE-8ZB, SSS-PA-TRACE-V8K | SSS-PA-TRACE-RX1, SSS-PA-TRACE-IKS |
| RenderingDefinition | SysML::Systems::Views | In | SSS-PA-VIS-RD1, SSS-PA-VIS-RD2 | SSS-PA-VIS-RD1, SSS-PA-VIS-RD2 |
| RenderingUsage | SysML::Systems::Views | In | SSS-PA-VIS-RD1, SSS-PA-VIS-RD2 | SSS-PA-VIS-RD1, SSS-PA-VIS-RD2 |
| RequirementConstraintMembership | SysML::Systems::Requirements | In | SSS-PA-REQ-DS6 | SSS-PA-REQ-DS6, SSS-PA-REQ-RF2 |
| RequirementDefinition | SysML::Systems::Requirements | In | SSS-PA-REQ-QP0, SSS-PA-REQ-WD0, SSS-PA-REQ-T8K, SSS-PA-REQ-M3N, SSS-PA-REQ-H6W | SSS-PA-VIS-C3D, SSS-PA-IE-B5W |
| RequirementUsage | SysML::Systems::Requirements | In | SSS-PA-REQ-QP0, SSS-PA-REQ-WD0, SSS-PA-REQ-DS6, SSS-PA-REQ-T8K, SSS-PA-REQ-M3N, SSS-PA-REQ-H6W, SSS-PA-REQ-V4J, SSS-PA-REQ-W9B | SSS-PA-VIS-C3D, SSS-PA-IE-B5W |
| RequirementVerificationMembership | SysML::Systems::VerificationCases | In | SSS-PA-REQ-W9B | SSS-PA-REQ-RF2 |
| ResultExpressionMembership | KerML::Kernel::Functions | In | SSS-PA-ELEM-X8T | SSS-PA-ELEM-X8T, SSS-PA-ELEM-O2K |
| ReturnParameterMembership | KerML::Kernel::Functions | In | SSS-PA-ELEM-R7S | SSS-PA-ELEM-R7S, SSS-PA-ELEM-O2K |
| SatisfyRequirementUsage | SysML::Systems::Requirements | In | SSS-PA-TRACE-Q72 | SSS-PA-VIS-C3D, SSS-PA-REQ-RF2 |
| SelectExpression | KerML::Kernel::Expressions | In | SSS-PA-EXPR-X1A, SSS-FB-VALID-CNF | SSS-PA-EXPR-X3C |
| SendActionUsage | SysML::Systems::Actions | In | SSS-PA-BEH-S2N | SSS-PA-VIS-M1Z |
| Specialization | KerML::Core::Types | In | SSS-PA-ELEM-M4J, SSS-PA-ELEM-L9P, SSS-FB-VALID-CNF | SSS-PA-ELEM-R6F, SSS-PA-ELEM-D8K |
| StakeholderMembership | SysML::Systems::Requirements | In | SSS-PA-REQ-H6W | SSS-PA-REQ-RF1 |
| StateDefinition | SysML::Systems::States | In | SSS-PA-BEH-RPK, SSS-PT-DATA-492, SSS-PA-BEH-SD1, SSS-PA-BEH-SC2, SSS-PA-BEH-SP3, SSS-FB-BEH-SV8 | SSS-PA-VIS-DP2, SSS-PA-VIS-B8V, SSS-PA-VIS-SH7 |
| StateSubactionMembership | SysML::Systems::States | In | SSS-PA-BEH-SE4 | SSS-PA-VIS-SH7 |
| StateUsage | SysML::Systems::States | In | SSS-PA-BEH-RPK, SSS-PA-BEH-H83, SSS-PA-BEH-SD1, SSS-PA-BEH-SC2, SSS-PA-BEH-SE4 | SSS-PA-VIS-DP2, SSS-PA-VIS-B8V, SSS-PA-VIS-F2C, SSS-PA-VIS-SH7 |
| Step | KerML::Kernel::Behaviors | Out | - | - |
| Structure | KerML::Kernel::Structures | Out | - | - |
| Subclassification | KerML::Core::Classifiers | In | SSS-PA-ELEM-M4J | SSS-PA-ELEM-R6F, SSS-PA-ELEM-D8K |
| SubjectMembership | SysML::Systems::Requirements | In | SSS-PA-REQ-T8K | SSS-PA-REQ-RF1 |
| Subsetting | KerML::Core::Features | In | SSS-PA-ELEM-D2N | SSS-PA-ELEM-D8K |
| Succession | KerML::Kernel::Connectors | Out | - | - |
| SuccessionAsUsage | SysML::Systems::Connections | In | SSS-PA-BEH-WG5 | SSS-PA-VIS-E4R |
| SuccessionFlow | KerML::Kernel::Interactions | Out | - | - |
| SuccessionFlowUsage | SysML::Systems::Flows | Out | - | - |
| TerminateActionUsage | SysML::Systems::Actions | Deferred | TBC | TBC |
| TextualRepresentation | KerML::Root::Annotations | In | SSS-PA-CMT-Y6L, SSS-PA-CMT-L7X, SSS-PA-CMT-Z9K | SSS-PA-CMT-L7X |
| TransitionFeatureMembership | SysML::Systems::States | In | SSS-PA-BEH-TG6 | SSS-PA-VIS-SH7 |
| TransitionUsage | SysML::Systems::States | In | SSS-PA-BEH-RPK, SSS-PA-BEH-TR5, SSS-PA-BEH-TG6, SSS-FB-BEH-SV8 | SSS-PA-VIS-B8V, SSS-PA-VIS-SH7 |
| TriggerInvocationExpression | SysML::Systems::Actions | In | SSS-PA-EXPR-X1A, SSS-FB-VALID-CNF | SSS-PA-EXPR-X3C |
| Type | KerML::Core::Types | Out | - | - |
| TypeFeaturing | KerML::Core::Features | In | SSS-PA-ARCH-N5W | SSS-PA-TRACE-RX1 |
| Unioning | KerML::Core::Types | Deferred | TBC | TBC |
| Usage | SysML::Systems::DefinitionAndUsage | Out | - | - |
| UseCaseDefinition | SysML::Systems::UseCases | In | SSS-PA-BEH-IX9 | SSS-PA-VIS-UC1, SSS-PA-VIS-UC2 |
| UseCaseUsage | SysML::Systems::UseCases | In | SSS-PA-BEH-IX9, SSS-PA-BEH-T7P | SSS-PA-VIS-UC1, SSS-PA-VIS-UC2 |
| VariantMembership | SysML::Systems::DefinitionAndUsage | In | SSS-PA-VAR-R7W, SSS-PA-VAR-J9K, SSS-PA-VAR-F1P | SSS-PA-VAR-M8F, SSS-PA-VAR-H2J |
| VerificationCaseDefinition | SysML::Systems::VerificationCases | In | SSS-PA-AV-UCQ | SSS-PA-AV-CR1, SSS-PA-AV-2RG |
| VerificationCaseUsage | SysML::Systems::VerificationCases | In | SSS-PA-AV-VU2, SSS-PA-REQ-W9B | SSS-PA-AV-2RG, SSS-PA-AV-CR1 |
| ViewDefinition | SysML::Systems::Views | In | SSS-PA-VIS-T2V | SSS-PA-VIS-T2V, SSS-PA-VIS-BB9, SSS-PA-VIS-JPW |
| ViewpointDefinition | SysML::Systems::Views | In | SSS-PA-VIS-T2V | SSS-PA-VIS-T2V |
| ViewpointUsage | SysML::Systems::Views | In | SSS-PA-VIS-T2V | SSS-PA-VIS-T2V |
| ViewRenderingMembership | SysML::Systems::Views | In | SSS-PA-VIS-RD1, SSS-PA-VIS-RD2 | SSS-PA-VIS-RD1 |
| ViewUsage | SysML::Systems::Views | In | SSS-PA-VIS-T2V | SSS-PA-VIS-T2V, SSS-PA-VIS-BB9, SSS-PA-VIS-JPW |
| WhileLoopActionUsage | SysML::Systems::Actions | In | SSS-PA-BEH-W5H | SSS-PA-VIS-M1Z |

### 8.2 Enumerations

These metamodel enumerations are datatypes, not metaclasses. They are listed for completeness; coverage is realised through the attributes of the metaclasses that use them. The KerML primitive types (Boolean, Integer, Rational, Natural, String) are imported library types and are not enumerated here.

| Enumeration | Package |
| --- | --- |
| FeatureDirectionKind | KerML::Core::Types |
| PortionKind | SysML::Systems::Occurrences |
| RequirementConstraintKind | SysML::Systems::Requirements |
| StateSubactionKind | SysML::Systems::States |
| TransitionFeatureKind | SysML::Systems::States |
| TriggerKind | SysML::Systems::Actions |
| VisibilityKind | KerML::Root::Namespaces |
