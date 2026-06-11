# Mycelium Software System Specification (SSS)

This document defines the software system specification for the Mycelium platform that is to be developed in contract 4000151333/26/NL/GP/mdu in accordance with ECSS-E-ST-40C Rev.1 Annex B. It contains the Product Owner's requirements for the Mycelium software system.

Each requirement uses the form:

> **\<Component\> shall** \<active verb\> **when** "\<condition\>"

Where component is one of: Mycelium Bloom, Mycelium Fabric, or Mycelium Forge.

Every verb must describe something the software actively does: renders a UI, processes a request, persists data, sends a notification, or blocks an operation.

Each requirement has a unique identifier.

The requirements are organized in tables. The tables list the `Requirement Identifier`, the `roles` it applies to, the requirement body or text and a `reference` to the Kerml or SysML2 specification in case this is applicable. If the kerml or syml2 reference is not applicable a `-` is used. The last two columns describe the priority (`low - (L)`, `medium - (M)`, `high - (H)`). This specification only includes the High priority requirements that will be implemented in contract 4000151333/26/NL/GP/mdu

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

The following table lists the SysML v2 Definition types (and their corresponding Usage types) that Mycelium supports. Each Definition describes a reusable type; each Usage represents a specific occurrence or application of that type in a model context. Any Definition or Usage can own Attributes. Models are organized into Packages, which are Namespaces rather than Definition or Usage types; package management is covered in 5.2.1.9.

| Definition | Usage | Description | SSS section |
|-----------|-------|-------------|-------------|
| OccurrenceDefinition | OccurrenceUsage | Entity occurring in time and space; base for parts, actions, and states, with time slices, snapshots, and individual designation | 5.2.1.11 |
| PartDefinition | PartUsage | Structural building block of a system (system, subsystem, equipment, component) | 5.2.1.11 |
| ItemDefinition | ItemUsage | Non-structural element representing data, signals, energy, or resources | 5.2.1.11 |
| AttributeDefinition | AttributeUsage | Data characteristic (quantity, text, boolean) with optional unit and measurement scale | 5.2.1.11, 5.2.1.14 |
| EnumerationDefinition | EnumerationUsage | Fixed set of allowed values restricting an attribute | 5.2.1.15 |
| PortDefinition | PortUsage | Interaction point on a part with directional features (in, out, inout) | 5.2.1.11 |
| ConnectionDefinition | ConnectionUsage | Link between parts or items (physical, logical, or data) | 5.2.1.11 |
| InterfaceDefinition | InterfaceUsage | Standardized connection between ports with compatibility rules | 5.2.1.11 |
| ActionDefinition | ActionUsage | Function or behavior with input/output parameters, decomposable into sub-actions | 5.2.1.16 |
| StateDefinition | StateUsage | Condition or mode with entry, do, and exit actions | 5.2.1.16 |
| — | TransitionUsage | Transition between states with trigger, guard, and effect | 5.2.1.16 |
| FlowDefinition | FlowUsage | Transfer of items, energy, or data between parts | 5.2.1.16 |
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
| RenderingDefinition | RenderingUsage | Presentation styling applied to rendered view content | 5.2.1.19 |
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

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-OA-USR-N35 | OA | Mycelium Bloom shall provide operations to create, update, deactivate, and delete user accounts when "an authenticated user with the Organization Administrator role accesses the user management interface." | - | H |
| SSS-OA-USR-T18 | OA | Mycelium Bloom shall display a list of all registered users within the organization and their current status when "the Organization Administrator navigates to the user management view." | - | H |
| SSS-OA-ROLE-TBX | OA | Mycelium Bloom shall provide operations to assign and revoke Organization Administrator and Organization Member roles when "the Organization Administrator selects a user and modifies their organization-level role." | - | H |
| SSS-OA-ROLE-PQH | OA | Mycelium Bloom shall provide a setting to control whether Organization Members can create projects within that Organization when "the Organization Administrator accesses the member permission settings." | - | H |
| SSS-CC-SS-HV9 | All | Mycelium Bloom shall create a new Organization and assign the requesting user as its Organization Administrator when "an authenticated user initiates organization creation from their account dashboard." | - | H |
| SSS-CC-SS-FUU | OA, OM | Mycelium Bloom shall create a new Project and assign the requesting user as its Project Administrator when "an Organization Member initiates project creation and the organization permits member project creation." | - | H |
| SSS-CC-SS-LEZ | All | Mycelium Bloom shall enforce project visibility rules (Private, Organization-visible, Public) when "a user attempts to access a project." | - | H |
| SSS-IA-ORG-V4R | IA | Mycelium Bloom shall display a list of all organizations on the installation with their name, creation date, member count, project count, and status (active/suspended) when "an Installation Administrator navigates to the installation administration view." | - | H |
| SSS-IA-ORG-K8W | IA | Mycelium Bloom shall provide operations to create, update, suspend, reactivate, and delete organizations when "an Installation Administrator accesses the organization management interface." | - | H |
| SSS-IA-ORG-M3J | IA | Mycelium Bloom shall display the details of an organization including its members, projects, roles, authentication configuration, and audit log when "an Installation Administrator selects an organization from the installation administration view." | - | H |
| SSS-IA-USR-B6P | IA | Mycelium Bloom shall display a list of all user accounts across all organizations with their username, email, organization memberships, roles, and status (active/deactivated) when "an Installation Administrator navigates to the installation user management view." | - | H |
| SSS-IA-USR-Q2N | IA | Mycelium Bloom shall provide operations to create, update, deactivate, and delete user accounts across all organizations when "an Installation Administrator accesses the installation user management interface." | - | H |
| SSS-IA-USR-H7F | IA | Mycelium Bloom shall provide operations to assign and remove users to and from any organization with a specified role when "an Installation Administrator selects a user and modifies their organization memberships." | - | H |

##### 5.2.1.2 User profile

Users have a profile showing their identity, projects, and contributions, and is where they manage how they appear to others across the platform. The requirements in this section cover the profile page contents, personal details, project list with key metadata, and a contribution heatmap showing activity over time, and the editing of the user's own personal details and appearance (avatar and collaborator colour). Where identity is federated through an external identity provider, IdP-sourced attributes (such as email address) are displayed read-only and managed in the identity provider; the appearance attributes (avatar, collaborator colour) are always managed within Mycelium.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-CC-PROF-L6D | All | Mycelium Bloom shall display the user's profile details, a list of all projects the user is a member of, and a contribution heatmap when "a user navigates to their profile page." | - | H |
| SSS-CC-PROF-52O | All | Mycelium Bloom shall display each project in the profile project list with: project name, description, license, last updated date, visibility (private, organization, public), and activity sparkline when "the user views their profile project list." | - | H |
| SSS-CC-PROF-K7B | All | Mycelium Bloom shall update the user's profile personal details, display name, job title, and biography, and persist the change when "a user edits their personal details on their profile page and saves." | - | H |
| SSS-CC-PROF-W1N | All | Mycelium Bloom shall propagate an updated display name, avatar, or collaborator colour to every surface that renders it, the project presence indicator, diagram co-presence indicators, comments, and contribution views, in near real-time when "a user changes their display name, avatar, or collaborator colour." | - | H |

##### 5.2.1.3 Project management

A project is the unit of collaboration in Mycelium. Each project owns a SysML v2 model, a team, branches, and Ownership assignments. The Project Administrator (typically the study lead) configures the project, assigns roles, defines Ownerships, and oversees the model's structural integrity. The requirements in this section cover project creation, configuration, team management, and Ownership administration. Owner administration is only relevant in case the project is a Concurrent Design project.


| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-OA-PROJ-TWK | OA | Mycelium Bloom shall create a Project with metadata (name, description) and a default Branch, consistent with the Systems Modelling API Project concept, when "the Organization Administrator submits a valid project creation form." | TBC | H |
| SSS-OA-PROJ-PFY | OA | Mycelium Bloom shall delete a project within the organization, applying configurable deletion policies regarding project usages by other projects, when "the Organization Administrator initiates project deletion and confirms the action." | - | H |
| SSS-PA-MGMT-B3R | PA | Mycelium Bloom shall provide an interface to update project properties including name, description, default branch and visibility when "the Project Administrator edits an existing project's settings." | - | H |
| SSS-PA-MGMT-8EF | PA | Mycelium Bloom shall provide operations to add and remove users (including Outside Collaborators) with assigned roles and Ownerships when "the Project Administrator accesses the team management interface of a project." | - | H |
| SSS-PA-MGMT-KYM | PA | Mycelium Bloom shall transfer the Project Administrator role to another team member when "the current Project Administrator selects a team member and confirms the transfer." | - | H |
| SSS-PA-MGMT-73C | PA | Mycelium Bloom shall provide a setting to configure the project mode (Regular or Concurrent Design) when "the Project Administrator accesses the project mode settings." | - | H |

##### 5.2.1.4 Project lifecycle state

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PA-STATE-V4R | PA, PT, VW | Mycelium Bloom shall display the current lifecycle state of a project in the project header and project list when "a user views a project." | - | H |
| SSS-PA-STATE-K7N | PA | Mycelium Bloom shall provide operations to transition a project between lifecycle states when "the Project Administrator changes the project's lifecycle state." | - | H |
| SSS-PA-STATE-W2D | All | Mycelium Bloom shall enforce the following project lifecycle states and their editing constraints: | - | H |

The following lifecycle states are defined (TBC):

| State | Description | Editing |
|-------|-------------|---------|
| **Preparation** | Project setup: structure, team, ownerships, reference data. Core team configures the baseline model. | Open to Project Administrator only |
| **Open** | Active modeling: all team members contribute within their Ownerships. Design sessions take place. | Open to all Participants per Ownership |
| **Review** | Model under review: no modifications permitted. Stakeholders and Viewers inspect the model and provide feedback. | Read-only for all roles |
| **Archived** | Study completed: model preserved as an immutable historical record. Can be reopened or used as a template for new projects. | Read-only for all roles |

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PA-STATE-M8T | PA | Mycelium Bloom shall transition a project from its current lifecycle state to any other lifecycle state (Preparation, Open, Review, Archived) when "the Project Administrator selects a target lifecycle state for the project." | - | H |
| SSS-PA-STATE-F3B | PA, OA | Mycelium Bloom shall create a new project pre-populated with the content of an Archived project when "a user creates a new project using an archived project as a template." | - | H |
| SSS-PA-STATE-R6G | PA, PT, VW | Mycelium Bloom shall display a visual indicator (e.g. banner, badge, or icon) showing the project's current lifecycle state (Preparation, Open, Review, Archived) when "a user views a project." | - | H |
| SSS-PA-STATE-Q8L | All | Mycelium Bloom shall assign Preparation as the default lifecycle state to a newly created project when "a user creates a new project and the organization has not configured a different default state." | - | H |

##### 5.2.1.5 Collaboration and awareness

Mycelium is a multi-user platform: in any project, several engineers from different ownerships are typically working on the model at the same time. The requirements in this section cover how Mycelium Bloom and Mycelium Fabric make collaboration *live*, visible, immediate, and lock-free, so that every user has continuous awareness of who else is in the project, what they are working on, and what is changing.

###### 5.2.1.5a Project-level user presence

When a user opens a project, they should see at a glance who else is currently working in the same project, without having to navigate to a separate panel. This is the equivalent of the avatar cluster Microsoft Word and Google Docs show in a shared document's title bar: a small, always-visible indication that "I am not alone here". The requirements in this subsection cover the **project-level** presence indicator only.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-CC-PRESENCE-A4N | All | Mycelium Bloom shall display a project presence indicator listing every Account currently connected to the project, persistently visible in the project chrome from every view within the project, when "a user has a project open." | - | H |
| SSS-CC-PRESENCE-B7P | All | Mycelium Bloom shall display each connected Account's display name, avatar, and assigned collaborator colour in the project presence indicator when "the project presence indicator is rendered." | - | H |
| SSS-CC-PRESENCE-D2K | All | Mycelium Bloom shall update the project presence indicator in near real-time as Accounts connect to or disconnect from the project when "Mycelium Fabric delivers a project presence event." | - | H |
| SSS-CC-PRESENCE-H6T | All | Mycelium Fabric shall publish a project presence event to all clients connected to a project when "an Account connects to or disconnects from the project." | - | H |

###### 5.2.1.5b Deep linking and sharing

Engineers need to share specific surfaces of the model, a part, a requirement, a diagram, or an element pinned to a particular view, by copying a URL into email, chat, a comment, or a browser bookmark. The recipient pastes the URL and lands directly on that surface, signed in if necessary. The requirements in this section cover URL addressability of every navigable surface, an in-app "copy link" affordance, and the resolution behavior when a URL is opened (including stability across renames, scoping to a specific view, and graceful handling of missing or inaccessible targets). These requirements are also the technical foundation for future external integrations (chat, email, third-party notification routing) that embed Mycelium URLs.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-CC-LINK-A2P | All | Mycelium Bloom shall expose every project, branch, model element, view, and diagram as a unique URL displayed in the browser address bar when "a user has a project, branch, element, view, or diagram open." | - | H |
| SSS-CC-LINK-B5R | All | Mycelium Bloom shall provide a "copy link" action on the detail panel and on the context (right-click) menu of every model element, view, and diagram that copies the current URL to the clipboard when "a user activates the copy link action." | - | H |
| SSS-CC-LINK-D8K | All | Mycelium Bloom shall navigate the user to the addressed project, branch, element, view, or diagram when "a user opens a Mycelium URL." | - | H |
| SSS-CC-LINK-E3M | All | Mycelium Bloom shall redirect the user through the authentication flow and resume navigation to the originally addressed surface after sign-in when "a user opens a Mycelium URL while not authenticated." | - | H |
| SSS-CC-LINK-F7N | All | Mycelium Bloom shall encode the branch (and optionally the commit) of the model in URLs it generates so that a shared link resolves to the same model state the link author was viewing when "Mycelium Bloom generates a shareable URL." | - | H |
| SSS-CC-LINK-G1V | All | Mycelium Bloom shall construct URLs using stable element identifiers so that the URL remains valid across element renames and namespace moves when "Mycelium Bloom generates a URL referencing a model element." | - | H |

###### 5.2.1.5c Live model updates

When user A edits the model, user B should see the change in near real-time without manually refreshing. Mycelium Bloom listens for change notifications from the backend and updates open views accordingly. The requirements in this section cover the UI behavior on receipt of live updates, including conflict indicators and preservation of the user's editing context.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-CC-LIVE-R4K | All | Mycelium Bloom shall update the hierarchical browser, detail panels, and tabular browsers in near real-time to reflect changes committed by other users when "Mycelium Fabric delivers a model change notification." | - | H |
| SSS-CC-LIVE-W7N | All | Mycelium Bloom shall update all open diagrams in near real-time to reflect changes to model elements committed by other users when "a diagram contains elements that have been modified by another user." | - | H |
| SSS-CC-LIVE-T9F | PA, PT | Mycelium Bloom shall present a conflict indicator when the current user has uncommitted local changes to an element that another user has also modified and committed when "a model change notification is received for an element with pending local edits." | - | H |
| SSS-CC-LIVE-K2B | All | Mycelium Bloom shall maintain the user's current scroll position, selection, and editing state when applying live model updates from other users when "the UI refreshes in response to incoming model changes." | - | H |

##### 5.2.1.6 Change persistence

Mycelium Bloom operates in two persistence modes. In immediate mode, each edit is persisted to Mycelium Fabric as an individual Commit on the active branch, making it visible to other users in near real-time. In batch mode, the user collects multiple changes locally before persisting them as a single atomic Commit. Both modes produce Systems Modelling API Commits.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PA-PERSIST-K4R | PA, PT | Mycelium Bloom shall persist each model edit (create, update, delete) to Mycelium Fabric as an individual Commit on the active branch in immediate mode when "a user completes an edit and immediate persistence mode is active." | API 7.2.3 | H |
| SSS-PA-PERSIST-W8N | PA, PT | Mycelium Bloom shall accumulate model edits locally without persisting them to Mycelium Fabric in batch mode when "a user performs edits and batch persistence mode is active." | API 7.2.3 | H |

##### 5.2.1.7 Concurrent Design

Concurrent design brings 20-30 engineers from different domains into the same room (or video call) to design a system together in real time. Mycelium must handle this scale, propagate changes across all connected users, and present session-aware views that show what is happening across the team. The requirements in this section cover concurrent session participation and the views engineers need during a session.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PT-CDS-RKV | PA, PT | Mycelium Bloom shall support concurrent design sessions with at least 20-30 Participants from multiple Ownerships working simultaneously when "multiple Participants are connected to the same project and actively modifying model data." | - | H |
| SSS-PT-UI-256 | PT | Mycelium Bloom shall present a selector to switch the active Ownership when "the Participant is assigned to multiple Ownerships and selects a different active Ownership from the Ownership selector." | - | H |

###### 5.2.1.7a Subscriptions

When one engineer's work depends on another's outputs, they need to track those outputs and decide how changes propagate into their own work. Mycelium models these dependencies as ParameterSubscriptions: a subscriber's Ownership expresses interest in an attribute owned by another Ownership and is notified when its value is published. The requirements in this section cover creating subscriptions individually, in bulk (by attribute kind, by element, or by owner), and through standing rules that automatically subscribe to matching attributes created later; choosing how each subscription sources its value (the owner's published value or the subscriber's own override); reviewing subscriptions and their up-to-date status; and keeping subscription sets consistent as the model changes.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PT-COLLAB-8U9 | PT | Mycelium Bloom shall create a ParameterSubscription on an AttributeUsage owned by another Ownership when "the Participant selects an attribute from another Ownership and initiates a subscription." | - |  |  |
| SSS-PT-COLLAB-12K | PT | Mycelium Bloom shall deliver a near real-time notification when "an attribute that the Participant has subscribed to is published by another Ownership." | - |  |  || SSS-PT-SUB-A1K | PT | Mycelium Bloom shall create a standing subscription rule that subscribes the Participant's Ownership to every existing and future AttributeUsage typed by a selected AttributeDefinition kind (e.g. mass, mass margin) and owned by another Ownership when "the Participant defines a standing subscription rule for one or more attribute kinds." | - | H |
| SSS-PT-SUB-D9T | All | Mycelium Fabric shall evaluate the applicable standing subscription rules and create the corresponding ParameterSubscription when "an AttributeUsage matching a standing rule's attribute kind and owner scope is created or becomes owned by another Ownership." | - | H |
| SSS-PT-SUB-E2F | PT | Mycelium Bloom shall create ParameterSubscriptions on all existing AttributeUsages typed by one or more selected AttributeDefinition kinds and owned by other Ownerships, optionally restricted to selected Ownerships or a selected package subtree, in a single operation when "the Participant selects one or more attribute kinds and invokes batch subscribe." | - | H |
| SSS-PT-SUB-L4G | PT | Mycelium Bloom shall display a Subscriptions view listing every ParameterSubscription held by the Participant's Ownership, showing the subscribed attribute and its owning element, the owning Ownership, the latest published value, the subscriber's effective value and value source, and the subscription status, when "the Participant opens the Subscriptions view." | - | H |

###### 5.2.1.7b Publication workflow

In Concurrent Design Mode, attribute owners edit their own values (OwnedValue) without immediately affecting the values visible to subscribers. A publication event copies the OwnedValue to the AttributeUsage value, making it available to all consumers. This staged, manual publication is the default. A project may instead enable *auto-publish mode*, in which each owner edit is published immediately and the manual publication step is not required. The publication mechanism is modeled in the Concurrent Design library using PublicationDefinition, PublishedIn, and OwnedValue MetadataDefinitions (see [Roles and Permissions](Roles-and-Permissions.md)).

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PT-PUB-K4W | PT | Mycelium Bloom shall store the owner's pending value as an OwnedValue metadata annotation on the AttributeUsage, without overwriting the published attribute value visible to subscribers, when "a Participant edits an attribute value in Concurrent Design Mode with auto-publish disabled." | - | H |
| SSS-PT-PUB-C8L | PT | Mycelium Bloom shall publish an attribute value immediately upon edit, making the new value visible to all subscribers without requiring a manual publish operation, when "a Participant edits an attribute value while auto-publish mode is enabled." | - | H |
| SSS-PT-PUB-R7N | PA, PT, VW | Mycelium Bloom shall visually distinguish attributes with pending unpublished changes (OwnedValue differs from published value) from attributes where OwnedValue and published value are equal when "a user views attributes in the model browser, detail panel, or tabular views." | - | H |
| SSS-PT-PUB-D3M | PA, PT, VW | Mycelium Bloom shall display the old (published) value, the new (owned) value, and the difference (absolute and percentage) for each attribute with pending changes when "a user opens the publication review view." | - | H |
| SSS-PT-PUB-H8J | PA | Mycelium Bloom shall publish all pending attribute changes across all Ownerships in a single operation, copying each OwnedValue to its corresponding AttributeUsage value and creating a PublicationDefinition record with timestamp, when "the Project Administrator initiates a publish-all operation." | - | H |
| SSS-PT-PUB-W5T | PA | Mycelium Bloom shall publish pending attribute changes for a single selected Ownership, copying only that Ownership's OwnedValues to their corresponding AttributeUsage values and recording the publication, when "the Project Administrator initiates a publish-per-ownership operation and selects one or more Ownerships." | - | H |
| SSS-PT-PUB-M2F | PA, PT, VW | Mycelium Bloom shall display a publication history listing all past publications with their timestamp, the publishing user, and the Ownerships included when "a user opens the publication history view." | - | H |
| SSS-PT-PUB-N6K | PA, PT, VW | Mycelium Bloom shall display the list of attributes that were published in a specific publication event, showing the attribute name, element, old value, new value, and Ownership, when "a user selects a publication record from the publication history." | - | H |
| SSS-PT-PUB-F1V | All | Mycelium Fabric shall reject direct modification of published attribute values by non-owner Participants and enforce that only the publication workflow updates the shared attribute value in Concurrent Design Mode when "a Participant attempts to write directly to an AttributeUsage value they subscribe to." | - | H |

##### 5.2.1.8 Model navigation and browsing

Engineers spend a lot of of their time finding, selecting, and understanding model elements. Mycelium offers complementary navigation views: a hierarchical tree for structural exploration and a tabular browser for flat searching with namespace path columns. The requirements in this section ensure that users can find any element quickly, see its qualified context, and follow relationships to related elements without losing their place.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PA-NAV-8IB | PA, PT, VW | Mycelium Bloom shall display the model as a hierarchical tree (Browser View) with collapsible and expandable nodes representing packages, parts, and nested elements when "a user opens a project and navigates to the Browser View." | - | H |
| SSS-PA-NAV-ZLS | PA, PT, VW | Mycelium Bloom shall return filtered model elements matching the specified criteria when "a user enters search terms or applies filters by name, type or Ownership." | - | H |
| SSS-PA-NAV-ZRW | PA, PT, VW | Mycelium Bloom shall display element properties including attributes, relationships, and ownership in a detail panel when "a user selects a model element." | - | H |
| SSS-PT-NAV-7U5 | PA, PT, VW | Mycelium Bloom shall display the Ownership of each element and attribute when "a user views a model element's properties or browses the model tree." | - | H |
| SSS-PA-NAV-F3K | PA, PT, VW | Mycelium Bloom shall display the qualified name (namespace path) of each model element when "a user views an element's properties in the detail panel." | - | H |
| SSS-PA-NAV-G5X | PA, PT, VW | Mycelium Bloom shall provide a tabular element browser that lists Definitions and Usages for each kind of Definition and Usage in a sortable, filterable table showing element name, namespace path, type, Ownership, and key attributes when "a user opens the tabular element browser." | - | H |
| SSS-PA-NAV-W4B | PA, PT, VW | Mycelium Bloom shall support the hierarchical Browser View and the tabular element browser as independent views that can be open simultaneously when "a user has both views open." | - | H |
| SSS-PA-NAV-M2C | PA, PT, VW | Mycelium Bloom shall open and display multiple hierarchical Browser Views and multiple tabular element browsers at the same time, without limiting the user to a single instance of either, each maintaining its own scope, filters, sorting, and selection, when "a user opens an additional Browser View or tabular element browser." | - | H |

##### 5.2.1.9 Namespace and package management

SysML v2 organizes models into Packages and Namespaces. Packages group related elements; Namespaces control naming and visibility; Imports allow reuse without duplication. The requirements in this section ensure users can structure their models hierarchically, share content between packages, and apply visibility rules without leaving the model browser.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PA-PKG-R8W | PA, PT | Mycelium Bloom shall support creating, renaming, moving, and deleting Packages to organize model elements into logical groups when "a user manages packages in the model browser." | SysML 7.5 | H |
| SSS-PA-PKG-V2J | PA, PT | Mycelium Bloom shall support nesting Packages within other Packages to create hierarchical model organization when "a user creates a child package within an existing package." | SysML 7.5 | H |
| SSS-PA-PKG-D4N | PA, PT | Mycelium Bloom shall support importing elements from one Namespace into another using Membership Imports and Namespace Imports when "a user creates an import relationship between namespaces." | SysML 7.5 | H |
| SSS-PA-PKG-J3W | PA, PT | Mycelium Bloom shall create a Filtered Import that imports only elements matching a metadata-based condition (e.g. import only elements annotated with a specific Metadata Usage) when "a user creates a namespace import and specifies a metadata filter expression." | SysML 7.5.4 | H |
| SSS-PA-PKG-H6T | PA, PT | Mycelium Bloom shall support setting member visibility (public, private) on elements within a Namespace when "a user configures the visibility of a model element within its owning namespace." | KerML 7.2.5 | H |
| SSS-PA-PKG-V8N | PA, PT, VW | Mycelium Bloom shall display the visibility marker (public, private, protected) of every Membership alongside its owning element in the model browser, the detail panel, and tabular views when "a user views a namespace or its members." | KerML 7.2.5 | H |
| SSS-PA-PKG-T5C | PA, PT, VW | Mycelium Bloom shall display the imported members of a Namespace in the model browser, visually distinguished from owned members by a dedicated icon or rendering style, when "a user expands a namespace that declares one or more imports." | KerML 7.2.5 | H |
| SSS-PA-PKG-C7B | PA, PT | Mycelium Bloom shall display the validation error returned by Mycelium Fabric for a duplicate `memberName` conflict, highlighting the conflicting members and blocking the offending edit, when "a commit or edit submitted by a user is rejected by Fabric due to a duplicate `memberName`." | KerML 7.2.5 | H |
| SSS-PA-NAV-S6P | PA, PT, VW | Mycelium Bloom shall provide a global search interface that matches model elements by `name` and by `qualifiedName` across every Namespace in the current project, returning results with their qualified path and navigation link, when "a user enters a search term into the global search bar." | KerML 7.2.5 | H |
| SSS-PA-ELEM-R3G | PA, PT, VW | Mycelium Bloom shall propagate a rename operation to every displayed `qualifiedName` of the renamed element and of its transitive descendants (in the model browser, detail panels, diagram labels, tooltips, breadcrumbs, and tabular views) when "a user renames a Namespace or one of its members." | KerML 7.2.5 | H |
| SSS-PA-ELEM-M9T | PA, PT | Mycelium Bloom shall re-parent a model element to a new owning Namespace, update its `qualifiedName` and those of its transitive descendants, and preserve all incoming references to the moved element when "a user moves an element to a different namespace via drag-and-drop or the move action." | KerML 7.2.5 | H |
| SSS-PA-PKG-N4J | PA, PT | Mycelium Bloom shall create a NamespaceImport in the importing Namespace, referencing the imported Namespace, when "a user selects a target Namespace and invokes the 'Import Namespace' action from a package or namespace." | KerML 7.2.5.4 | H |
| SSS-PA-PKG-M5P | PA, PT | Mycelium Bloom shall create a MembershipImport in the importing Namespace, referencing the imported Membership, when "a user selects a single named element from another Namespace and invokes the 'Import Member' action." | KerML 7.2.5.4 | H |
| SSS-PA-PKG-R9K | PA, PT | Mycelium Bloom shall set the `isRecursive` flag on a NamespaceImport, causing nested namespaces of the imported Namespace to be imported as well, when "a user toggles the 'include nested namespaces' option on a NamespaceImport." | KerML 7.2.5.4 | H |
| SSS-PA-PKG-H3W | PA, PT | Mycelium Bloom shall set the visibility of an Import to public, private, or protected, controlling whether the Import is re-exported through transitive imports, when "a user edits the visibility of an Import." | KerML 7.2.5.4 | H |
| SSS-PA-PKG-L6D | PA, PT, VW | Mycelium Bloom shall display, in the detail panel of a Namespace, the list of Imports it declares, showing the import kind (NamespaceImport or MembershipImport), the imported target, and the `isRecursive`, `isImportAll`, and visibility values, when "a user views a Namespace that declares one or more Imports." | KerML 7.2.5.4 | H |
| SSS-PA-PKG-X8C | PA, PT | Mycelium Bloom shall delete an Import from a Namespace when "a user selects an Import in the detail panel and invokes the 'Remove import' action." | KerML 7.2.5.4 | H |
| SSS-PA-PKG-X1J | PA, PT | Mycelium Bloom shall detect when a user operation (drag-and-drop from another package or library, type assignment, specialization, reference creation, or any other operation) references an Element whose owning Namespace is not already visible in the current Namespace, and shall create the appropriate Import (a MembershipImport for a single-element reference, or a NamespaceImport when the user chooses to import the whole Namespace) as part of the same user operation, when "a user uses an Element from another Namespace that is not yet imported into the current Namespace." | KerML 7.2.5.4 | H |
| SSS-PA-PKG-X2K | PA, PT | Mycelium Bloom shall present a confirmation dialog identifying the referenced Element, its owning Namespace, and the proposed Import kind (MembershipImport of the specific Element or NamespaceImport of the owning Namespace), and shall not create the Import or complete the triggering operation until the user confirms the proposed action or selects an alternative, when "Mycelium Bloom is about to auto-create an Import in response to a cross-namespace user operation." | KerML 7.2.5.4 | H |
| SSS-PA-PKG-X3L | PA, PT | Mycelium Bloom shall not create a new Import when the referenced Element is already resolvable in the current Namespace through an existing MembershipImport, NamespaceImport, transitive NamespaceImport, or AliasMembership of compatible visibility, and shall complete the triggering user operation without modifying the Import set, when "a user uses an Element whose owning Namespace is already imported." | KerML 7.2.5.4 | H |
| SSS-PA-PKG-P8D | PA | Mycelium Bloom shall convert a Package into a LibraryPackage, or create a new LibraryPackage, when "a user invokes the 'Convert to Library' action on a Package or the 'New Library Package' action in the model browser." | KerML 7.4.14 | H |
| SSS-PA-PKG-S1E | PA | Mycelium Bloom shall set the `isStandard` flag on a LibraryPackage, marking it as a standard library distinct from a user library, when "a user toggles the 'Standard library' option on a LibraryPackage." | KerML 7.4.14 | H |
| SSS-PA-PKG-V4H | PA, PT, VW | Mycelium Bloom shall render a LibraryPackage in the model browser, tabular views, and diagrams with a distinguishing icon or badge that sets it apart from a regular Package, when "a user views a LibraryPackage." | KerML 7.4.14 | H |
| SSS-PA-IE-GYP | PA | Mycelium Bloom shall provide operations to create and manage Project Usages to reference elements from one Project within another, consistent with the Systems Modelling API ProjectUsageService, when "the Project Administrator creates a Project Usage and selects the target project to reference." | API 7.2.6 | H |
| SSS-PA-MGMT-YC1 | PA | Mycelium Bloom shall provide operations to create, rename and remove Ownership Usages within the project package when "the Project Administrator accesses the Ownership management interface." | - | H |
| SSS-PA-MGMT-BA7 | PA | Mycelium Bloom shall reassign element ownership by updating the Owner metadata on a model element to a different Ownership when "the Project Administrator selects a model element and changes its Owner annotation." | - | H |

##### 5.2.1.10 Requirements modeling

Requirements capture stakeholder-imposed conditions that a design must satisfy. SysML v2 models requirements as Constraint Definitions with subjects, actors, stakeholders, assumed and required constraints, and concerns. Requirements can be nested, derived, satisfied by design elements, and verified by Verification Cases. The requirements in this section cover modeling the full SysML v2 requirements metamodel as first-class model elements through user-facing operations.

###### 5.2.1.10.a Requirement definitions and constraints

A Requirement Definition captures a stakeholder-imposed condition as a textual statement together with the assumed and required constraints that formalise it. Requirements can be organised into hierarchical specifications, where nested requirements become required constraints of their parent. The requirements in this subsection cover creating, editing, organising, and nesting requirements and editing their constraint expressions.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PA-REQ-QP0 | PA, PT | Mycelium Bloom shall provide operations to create, edit, and organize Requirement Definitions and Requirement Usages in hierarchical specifications with textual statements when "a user accesses the requirements modeling interface and creates or modifies a requirement." | SysML 7.21 | H |
| SSS-PA-REQ-WD0 | PA, PT | Mycelium Bloom shall nest a Requirement Usage within a parent Requirement Definition or Requirement Usage, where nested requirements automatically become required constraints of the parent, when "a user adds a child requirement to an existing requirement." | SysML 7.21, 8.3.21 | H |
| SSS-PA-REQ-DS6 | PA, PT | Mycelium Bloom shall provide editors for assumed constraints and required constraints on requirements, where the effective requirement logic is "if all assumed constraints hold then all required constraints must be satisfied", when "a user edits a requirement and adds constraint expressions." | SysML 8.3.21.7 | H |

###### 5.2.1.10.b Subjects, actors, stakeholders, and concerns

A requirement is framed by what it applies to and who cares about it. SysML v2 binds a requirement to its subject, to the actors needed to fulfil it, and to the stakeholders whose concerns it addresses. The requirements in this subsection cover assigning subjects, actors, and stakeholders, and modelling stakeholder concerns.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PA-REQ-T8K | PA, PT | Mycelium Bloom shall assign a subject to a Requirement Definition or Requirement Usage via Subject Membership, binding the requirement to the system or element it applies to, when "a user specifies the subject of a requirement." | SysML 8.3.21.11 | H |

###### 5.2.1.10.c Requirement relationships and coverage

Requirements are connected to the rest of the model through trace relationships: derivation between requirements, satisfaction by design elements, and verification by verification cases. The requirements in this subsection cover these trace relationships and the coverage analysis that reports requirements lacking satisfaction or derivation.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PA-REQ-V4J | PA, PT | Mycelium Bloom shall create a Derivation relationship between requirements, linking an original requirement to one or more derived requirements with the semantic constraint that satisfaction of the original implies satisfaction of all derived requirements, when "a user creates a derivation trace between requirements." | SysML 9.6 | H |
| SSS-PA-REQ-W9B | PA, PT | Mycelium Bloom shall link a Verification Case Usage to a Requirement Usage via Requirement Verification Membership, recording which verification cases verify which requirements, when "a user associates a verification case with a requirement." | SysML 8.3.24.2 | H |
| SSS-PA-TRACE-Q72 | PA, PT | Mycelium Bloom shall create a SatisfyRequirementUsage recording that a design element satisfies a requirement when "a user selects a design element and a requirement and creates a satisfy relationship." | SysML 8.3.21.10 | H |

###### 5.2.1.10.d Use cases

A Use Case Definition captures required system behaviour from the perspective of an external actor pursuing a goal, complementing the textual requirements with an actor-and-goal view of what the system must do. Use cases can include the behaviour of other use cases. The requirements in this subsection cover defining use cases and the include relationships between them.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| NA | NA    | NA          | NA  | NA   | NA     |

##### 5.2.1.11 System architecture modeling

The core of system modeling is defining the building blocks (Definitions) of the system and instantiating them in a hierarchy (Usages). Engineers compose parts, items, ports, connections, and interfaces into a decomposed system architecture. The requirements in this section cover the SysML v2 structural concepts that engineers use to capture the what and how of a system, plus the everyday operations to duplicate, move, delete, and refine these elements.

###### 5.2.1.11.a General

The requirements in this section apply to every kind of Definition and Usage. They cover the operations common to all model elements, namely creating and instantiating them, reading and inspecting their details, updating their properties and relationships, deleting them, and navigating to and between them across the hierarchical browser, the tabular browser, and diagrams. Type-specific behaviour is covered in the dedicated subsections that follow.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PT-DATA-XHY | PA, PT | Mycelium Bloom shall create, modify and delete elements (parts, items, attributes, etc.) throughout a Project when "a user creates, modifies or deletes elements annotated with their Ownership." | - | H |
| SSS-PA-ELEM-C1A | PA, PT | Mycelium Bloom shall create a Definition of any kind from the hierarchical browser, the tabular browser, or a diagram when "a user invokes a create action for a Definition in any of these views." | - | H |
| SSS-PA-ELEM-C2B | PA, PT | Mycelium Bloom shall instantiate a Usage from an existing Definition, establishing the FeatureTyping to that Definition, when "a user instantiates a Definition as a Usage in any view." | - | H |
| SSS-PA-ELEM-C3C | PA, PT | Mycelium Bloom shall create a Usage together with a new Definition in a single operation when "a user creates a Usage without selecting an existing Definition." | - | H |
| SSS-PT-DATA-N7O | PT | Mycelium Bloom shall display and enable editing of model elements within the user's assigned Ownership when "the Participant navigates to a model element annotated with their Ownership as Owner." | - | H |
| SSS-PA-ELEM-U1E | PA, PT | Mycelium Bloom shall edit the declared name and declared short name of any element when "a user renames an element in the detail panel, the browser, a tabular view, or a diagram." | - | H |
| SSS-PA-ELEM-U2F | PA, PT | Mycelium Bloom shall edit the documentation of any element when "a user edits the documentation text of an element in the detail panel." | - | H |
| SSS-PT-DATA-M6H | PT | Mycelium Bloom shall automatically annotate newly created model elements with the Participant's active Ownership as Owner when "the Participant creates a new model element and the Model is a Concurrent Desing Model." | - | H |
| SSS-PA-ELEM-W3N | PA, PT | Mycelium Bloom shall move a Usage from its current parent element to a different parent element, preserving all attributes, attribute values, and Ownership assignments, when "a user drags a Usage and drops it onto a different parent element in between the following views: the model browser, tabular browser or a diagram." | - | H |
| SSS-PA-ELEM-J6D | PA, PT | Mycelium Bloom shall delete a Definition or Usage and all its owned nested children when "a user deletes a model element and confirms the deletion.". Nested children that are owned by other Owners than the current Owner are deleted as well.  | - | H |
| SSS-PA-ELEM-V7K | PA, PT | Mycelium Bloom shall set the multiplicity (lower bound, upper bound) on any Usage when "a user edits the multiplicity of a Usage in the detail panel or on a diagram." | KerML 7.4.12 | H |
| SSS-PA-ELEM-N8P | PA, PT | Mycelium Bloom shall set the lower and upper bounds of a Multiplicity Range as either a literal non-negative integer, the unbounded symbol `*`, or an Expression referencing other Features, when "a user edits the bounds of a Multiplicity Range in the detail panel or on a diagram." | KerML 7.4.12 | H |
| SSS-PA-ELEM-O1Q | PA, PT | Mycelium Bloom shall set the `isOrdered` and `isUnique` flags of a Feature, controlling whether its values are ordered and whether duplicates are permitted, when "a user toggles the ordering or uniqueness of a Feature in the detail panel." | KerML 7.3.4.2 | H |
| SSS-PA-ELEM-U3G | PA, PT | Mycelium Bloom shall set whether a Usage is composite, meaning owned by its containing element, or referential, meaning a reference to an element owned elsewhere, when "a user marks a Usage as composite or reference." | KerML 7.3.4.2 | H |
| SSS-PA-ELEM-CD7 | PA, PT | Mycelium Bloom shall prevent the creation or retyping of a composite Usage that would make a Definition a direct or transitive composite part of itself, and shall display an error identifying the resulting containment cycle, when "a user adds a composite Usage or assigns its type such that the composition hierarchy would become circular." | KerML 7.3.4.2 | H |
| SSS-PA-VIS-U7M | PA, PT, VW | Mycelium Bloom shall render the Multiplicity of a Feature or Usage in the model browser, tabular views, detail panel, and diagram labels using the textual notation `[lower..upper]`, displaying `*` for an unbounded upper, `[n]` when lower equals upper, and the expression text when a bound is an Expression, when "a user views an element that declares a Multiplicity." | KerML 7.4.12 | H |
| SSS-PA-ELEM-D2N | PA, PT | Mycelium Bloom shall create a subsetting relationship between a feature and another feature of a compatible type when "a user designates a feature as a subset of another feature." | KerML 7.3.4.4 | H |
| SSS-PA-ELEM-H9W | PA, PT | Mycelium Bloom shall create a redefinition relationship where a feature in a specializing type replaces a feature inherited from a general type when "a user designates a feature as a redefinition of an inherited feature." | KerML 7.3.4.5 | H |
| SSS-PA-ELEM-M4J | PA, PT | Mycelium Bloom shall create a Specialization relationship between two Definitions, where the specializing Definition inherits all features of the general Definition and can add or redefine features, when "a user designates one Definition as a specialization of another." | KerML 7.3.2.3 | H |
| SSS-PA-ELEM-F3T | PA, PT | Mycelium Bloom shall create a FeatureTyping relationship between a Usage and its typing Definition when "a user sets or changes the type of a Usage via the detail panel or by dragging a Definition onto a Usage." | KerML 7.3.4.3 | H |
| SSS-PA-ELEM-R6F | PA, PT, VW | Mycelium Bloom shall display the generalization/specialization hierarchy of a selected Definition, showing its general types and all its specializations, when "a user views the type hierarchy of a Definition." | KerML 7.3.2.3 | H |
| SSS-PA-ELEM-L9P | PA, PT | Mycelium Bloom shall delete a Specialization of any concrete kind (Subclassification, FeatureTyping, Subsetting, ReferenceSubsetting, Redefinition, CrossSubsetting, Conjugation) when "a user selects a Specialization in the detail panel and invokes the 'Remove' action." | KerML 7.3.2.3 | H |
| SSS-PA-ELEM-M6N | PA, PT, VW | Mycelium Bloom shall display the inherited Features of a conjugating Type with their directions shown inverted relative to the original Type, `in` rendered as `out`, `out` rendered as `in`, `inout` preserved, in the detail panel, the model browser, and on diagrams, when "a user views a Type that is the conjugate of another Type." | KerML 7.3.2.4 | H |
| SSS-PA-ELEM-F4M | PA, PT, VW | Mycelium Bloom shall display the owned Features of a Type, derived from its FeatureMemberships, with their visibility, multiplicity, direction, and type, in the detail panel of the Type, when "a user views a Type that owns one or more Features." | KerML 7.3.2.6 | H |
| SSS-PA-ELEM-E5N | PA, PT, VW | Mycelium Bloom shall display the end Features of a Connector, Connection, Interaction, Association, or Flow, derived from their EndFeatureMemberships, showing each end's referent Feature and multiplicity, in the detail panel, when "a user views a relationship element with end features." | KerML 7.4.6 | H |
| SSS-PA-ELEM-N1G | PA, PT, VW | Mycelium Bloom shall reveal and select an element in the hierarchical browser when "a user invokes reveal-in-browser on an element selected in a tabular view or a diagram." | - | H |
| SSS-PA-ELEM-N2H | PA, PT, VW | Mycelium Bloom shall locate and select an element in an open tabular browser when "a user invokes locate-in-table on an element selected in another view." | - | H |
| SSS-PA-ELEM-N4K | PA, PT, VW | Mycelium Bloom shall navigate from a Usage to its defining Definition and from a Definition to its Usages when "a user invokes go-to-definition or find-usages on an element." | - | H |

###### 5.2.1.11.b Occurrences

An Occurrence Definition is a definition of a class of things that have an extent in time, called their lifetime, and that may have spatial extent. An Occurrence Usage is a usage of an occurrence definition. Items, parts, ports, actions, and states are all kinds of occurrences: ItemDefinition specialises OccurrenceDefinition, PartDefinition specialises ItemDefinition, and PortDefinition, ActionDefinition, and StateDefinition specialise OccurrenceDefinition, so each kind inherits the features and temporal semantics of its more general kind. An occurrence keeps its identity throughout its lifetime even though the values of its features may change over time. The lifetime of an occurrence may be partitioned into time slices that represent phases such as a deployment or an operational period, and a time slice of zero duration is a snapshot that represents the occurrence at a single instant. An occurrence definition or usage may also be restricted to an individual, a single real or perceived object with a unique identity, such as a specific car identified by its vehicle identification number. The requirements in this section cover the temporal aspects shared by all occurrence kinds. The behavioural occurrences (actions, states) and their notations are covered in the Behavior modeling section.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| NA | NA    | NA          | NA  | NA   | NA     |

###### 5.2.1.11.c Items

An Item Definition is a kind of occurrence definition representing a class of identifiable objects that can be acted upon over time without necessarily performing actions themselves; an Item Usage is a usage of one or more Item Definitions. Items typically capture the inputs, outputs, and flows of a system, such as water, fuel, electrical signals, or data, that may flow through, be stored by, or be transported by the system, and an item may carry attributes, states, and nested item usages. An item that performs actions is normally modeled as a part: all parts are items, but not all items are necessarily parts. The same object, for example an engine, may be treated as an inert item or an active part at different stages of its lifetime.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PA-ARCH-B2D | PA, PT | Mycelium Bloom shall create an Item Definition representing a class of identifiable objects that may be acted upon over time, such as a data type, signal, or resource, when "a user creates a new Item Definition." | SysML 7.10 | H |
| SSS-PA-ARCH-B2F | PA, PT | Mycelium Bloom shall create an Item Usage typed by one or more Item Definitions as a feature of any Definition or Usage, representing an input, output, stored item, or flowing item, when "a user adds an item to a Definition or Usage." | SysML 7.10 | H |
| SSS-PA-ARCH-B2G | PA, PT | Mycelium Bloom shall nest an Item Usage within an Item Definition or Item Usage when "a user adds a nested item to an item." | SysML 7.10 | H |
| SSS-PA-ARCH-B2J | PA, PT, VW | Mycelium Bloom shall display, for an Item Usage, its typing Item Definitions and whether it is composite or referential, in the detail panel, when "a user views an Item Usage." | SysML 7.10 | H |
| SSS-PA-ARCH-B2K | PA, PT | Mycelium Bloom shall change the kind of a usage between item and part, retyping it with a compatible Part Definition when an Item Usage becomes a Part Usage, while preserving its name, nested features, and references, when "a user changes the kind of a usage between item and part." | SysML 7.11 | H |

###### 5.2.1.11.d Parts

A Part Definition represents a modular unit of structure, such as a system, a system component, or an external entity that may interact with the system. A Part Definition is a kind of Item Definition, so it defines a class of part objects that are occurrences with temporal and possibly spatial extent, while a Part Usage is a usage of one or more Part Definitions (and may also use item definitions that are not parts, allowing the same element to be treated as an item in some situations, for example an engine flowing along an assembly line, and as a part in others, for example that engine once installed in a vehicle). A system is modeled as a composite part whose part usages may themselves have further composite structure. Parts may carry attributes representing performance, physical, and other quality characteristics, expose ports that define where they interconnect, perform actions that cause items to flow across their connections, and exhibit states that enable different actions. A part can represent any level of abstraction, from a purely logical component to a physical component with a part number, and may model hardware, software, facilities, organizations, or users of a system.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PA-ARCH-JQH | PA, PT | Mycelium Bloom shall create a Part Definition as a reusable building block when "a user creates a new Part Definition." | SysML 7.11 | H |
| SSS-PA-ARCH-TB2 | PA, PT | Mycelium Bloom shall create a nested Part Usage within the selected parent Part, extending the system decomposition hierarchy (e.g. System, Subsystem, Equipment, Component), when "a user adds a child part to an existing part in the model hierarchy." | SysML 7.11 | H |
| SSS-PA-ARCH-PR4 | PA, PT | Mycelium Bloom shall instantiate the same Part Definition as multiple Part Usages in one or more containing parts, so that a single definition is reused across the system structure, when "a user instantiates an existing Part Definition more than once." | SysML 7.11 | H |
| SSS-PA-ARCH-PT5 | PA, PT | Mycelium Bloom shall create a Part Usage typed by one or more Part Definitions, and optionally by item definitions that are not part definitions, as a feature of a containing part, when "a user adds a part to a containing part." | SysML 7.11 | H |

###### 5.2.1.11.e Ports

A Port Definition is a kind of occurrence definition that defines a connection point enabling interactions between occurrences, most commonly parts, and a Port Usage is a usage of a Port Definition. A port usage may be connected to one or more other port usages, and these connections enable interactions between the occurrences that own the ports, with the features of the port usages (whether inherited from the definition or declared locally) specifying what can be exchanged. Because ports are themselves occurrences, port definitions and usages can contain nested port usages. A feature of a port may be directed as in, out, or inout, and flows nested in a connection between ports model transfers between matching directed features, where two features match if they have conforming definitions and either both have no direction or they have conjugate directions (the conjugate of in is out and vice versa, while inout is its own conjugate). A transfer can occur from the out features of one port to the matching in features of connected ports, and in both directions between matching inout features. Two ports conform when each feature of one port has a matching feature on the other, so that a connection allows a flow between every directed feature and its match. Each Port Definition also has a conjugated Port Definition whose directed features are reversed, and a conjugated Port Usage automatically conforms to a usage of the corresponding original Port Definition.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PA-ARCH-5RR | PA, PT | Mycelium Bloom shall create a Port Definition when "a user creates a new Port Definition." | SysML 7.12 | H |
| SSS-PA-ARCH-PU8 | PA, PT | Mycelium Bloom shall create a Port Usage on a part, typed by a Port Definition, when "a user adds a port to a part." | SysML 7.12 | H |
| SSS-PA-ARCH-PF1 | PA, PT | Mycelium Bloom shall define the features of a Port, representing the items, attributes, or signals that can be exchanged, each with a direction of in, out, or inout, when "a user adds or edits a feature of a Port Definition or Port Usage." | SysML 7.12 | H |

###### 5.2.1.11.f Connections

A Connection Definition is both a relationship and a kind of Part Definition that classifies connections between related things, such as items and parts. Unless it is abstract, a connection definition has at least two connection ends, which specify the things being related, and a connection with exactly two ends is a binary connection. Any other features of a connection definition characterize the connection itself, separately from the connected things, and because a connection is a part, those values may change over the lifetime of the connection while the connected ends do not. A Connection Usage is a part usage of a connection definition that connects specific usage elements, such as item and part usages, by redefining the
connection ends to associate them with the particular usages to be connected. A connection usage between parts is often a logical connection that abstracts away how the parts are physically connected, but it can also be refined into a physical connection by modeling the connecting medium itself as a part.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PA-ARCH-CD1 | PA, PT | Mycelium Bloom shall create a Connection Definition that classifies connections between related things, with at least two connection ends, when "a user creates a new Connection Definition." | SysML 7.13 | H |
| SSS-PA-ARCH-IGA | PA, PT | Mycelium Bloom shall create a Connection Usage that connects two or more participating part or item usages, associating each of its connection ends with a participating usage, when "a user selects two compatible elements and creates a connection." | SysML 7.13 | H |
| SSS-PA-ARCH-Y2D | PA, PT | Mycelium Bloom shall create a Binding Connector that asserts equality between two compatible features of model elements when "a user selects two features and creates a binding between them." | KerML 7.4.6.3 | H |

###### 5.2.1.11.g Interfaces

An Interface Definition is a kind of Connection Definition whose ends are restricted to port definitions, and an Interface Usage is a usage of an interface definition whose ends are restricted to port usages. In other words, an interface is simply a connection all of whose ends are ports, which lets compatible connections between parts be specified once and reused. For example, a Power interface between an appliance and wall power exposes a power port on one end and an outlet port on the other, and the same interface can connect many different appliances to wall power. When modeling physical interactions, an interface definition or usage may carry constraints on the features of its port ends.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PA-ARCH-ID1 | PA, PT | Mycelium Bloom shall create an Interface Definition whose ends are restricted to port definitions, when "a user creates a new Interface Definition." | SysML 7.14 | H |
| SSS-PA-ARCH-IU2 | PA, PT | Mycelium Bloom shall create an Interface Usage connecting two or more ports by associating its port ends with the participating port usages, when "a user selects two compatible ports and creates an interface." | SysML 7.14 | H |
| SSS-PA-ARCH-IC4 | PA, PT | Mycelium Bloom shall add constraints to an Interface Definition or Usage that relate the features of its port ends, such as conservation laws across the interface, when "a user adds a constraint to an interface." | SysML 7.14 | H |

###### 5.2.1.11.h Attributes

An Attribute Definition defines a set of data values, such as numbers, quantitative values with units, qualitative values such as text strings, or data structures of such values, and an Attribute Usage is a usage of an attribute definition. An attribute usage is always referential, as are any of its nested features, and its values are constrained to the range specified by its definition, while an Enumeration Definition is a specialised attribute definition that restricts the values to a discrete set. Attribute usages may be typed by SysML attribute definitions or by KerML primitive data types such as String, Boolean, Integer, and Real, whereas quantities with units are defined using the SysML Quantities and Units Domain Library or extensions of it. A guiding principle is that only the kind of unit, for example mass or length, is associated with the attribute definition, while a specific unit, for example kilograms or metres, is given only with an actual value, so that an attribute is independent of the units used and values convert automatically between units of the same kind. The values of an attribute usage do not themselves change over time, but when the attribute is owned by an occurrence such as an item, part, or action, its value may differ at different points in that occurrence's lifetime.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PA-ARCH-AD7 | PA, PT | Mycelium Bloom shall create an Attribute Definition typed by a quantity kind or data type, with its associated measurement unit where applicable, when "a user creates a new Attribute Definition." | SysML 7.7 | H |
| SSS-PA-ARCH-97Z | PA, PT | Mycelium Bloom shall create an Attribute Usage on any Definition or Usage, typed by an Attribute Definition or a primitive data type and expressed with its measurement unit where applicable, when "a user adds an attribute to a Definition or Usage, irrespective of the assigned Ownership of the target Definition or Usage." | SysML 7.7 | H |
| SSS-PA-ARCH-AV5 | PA, PT | Mycelium Bloom shall set the value of an Attribute Usage, expressed with its measurement unit where applicable, when "a user edits an attribute value." | SysML 7.7 | H |
| SSS-PT-DATA-OH2 | PA, PT | Mycelium Bloom shall override an attribute value on a specific element usage without changing the parent definition when "a user edits an attribute value on a usage that inherits from a definition." | KerML 7.3.4.5 | H |
| SSS-PT-DATA-492 | PA, PT | Mycelium Bloom shall assign attribute values that vary by exhibited State Usage (e.g. operational mode) when "a user associates attribute values with specific states on an element." | SysML 7.18 | H |

##### 5.2.1.12 Variation point and variant modeling

Early-phase design explores a family of possible solutions before committing to one. In SysML v2 this is modelled with variation. A variation, sometimes called a variation point, is any Definition or Usage, except an enumeration, that is designated as a point which can vary from one design configuration to another, and its alternatives are called variants. For example, the engine of a vehicle may be a variation whose variants are a four-cylinder engine and a six-cylinder engine. Variations can be nested to any depth, and constraints can restrict which variants may be chosen together, so that the model forms a superset from which a complete configuration is obtained by selecting one variant per variation. Mycelium offers two complementary mechanisms for exploring alternatives: Branches for fully independent design alternatives, and variation points and variants for in-place variability within a single branch. The requirements in this section cover designating variation points, managing their variants, selecting and resolving configurations, and comparing alternatives.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PA-VAR-K3T | PA, PT | Mycelium Bloom shall mark any Definition or Usage, except an Enumeration Definition, as a variation point by setting `isVariation = true` when "a user designates an element as a variation point." | SysML 7.6 | H |
| SSS-PA-VAR-R7W | PA, PT | Mycelium Bloom shall add a variant to a variation point, either by creating a new variant Usage or by referencing an existing Usage defined elsewhere, when "a user adds a variant to an existing variation point." | SysML 7.6 | H |
| SSS-PA-VAR-N5D | PA, PT | Mycelium Bloom shall remove a variant Usage from a variation point when "a user deletes a variant from a variation point." | SysML 7.6 | H |
| SSS-PA-VAR-NV1 | PA, PT | Mycelium Bloom shall create a variation point nested within a variant, to any level of nesting, when "a user designates an element inside a variant as a further variation point." | SysML 7.6 | H |

##### 5.2.1.13 Allocations and relationships

An allocation is a mapping across the structures and hierarchies of a system model, asserting that a target element is responsible for realising some or all of the intent of a source element, for example a function allocated to a component. Beyond allocation, Mycelium supports the generic KerML and SysML v2 relationship constructs (typed relationships, dependencies, and external relationships) and a Relationship Matrix for visualising and editing relationships of any type across sets of elements. The requirements in this section cover allocation, generic relationships, and the matrix view. Requirement-specific trace relationships such as Satisfy, Derive, and Verify are covered in the Requirements modeling section.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PA-TRACE-AD1 | PA, PT | Mycelium Bloom shall create an Allocation Definition specifying that a target element realises the intent of a source element when "a user creates a new Allocation Definition." | SysML 7.15 | H |
| SSS-PA-TRACE-YWQ | PA, PT | Mycelium Bloom shall create an Allocation Usage, typed by one or more Allocation Definitions, that maps a source element to a target element responsible for realising it, when "a user selects source and target elements and creates an allocation." | SysML 7.15 | H |
| SSS-PA-TRACE-IKS | PA, PT, VW | Mycelium Bloom shall display a Relationship Matrix showing binary relationships between element sets (e.g. requirements vs. parts) when "a user opens the Relationship Matrix view and selects the element sets and relationship type." | - | H |
| SSS-PA-TRACE-V3H | PA, PT, VW | Mycelium Bloom shall populate the Relationship Matrix rows and columns from user-selected element types, packages, or query results when "a user configures the row source and column source of a Relationship Matrix." | - | H |
| SSS-PA-TRACE-K7W | PA, PT, VW | Mycelium Bloom shall indicate the presence and direction of relationships in each matrix cell using visual markers (e.g. filled cell, arrow, relationship count) when "the Relationship Matrix renders cells where relationships exist between the row and column elements." | - | H |
| SSS-PA-TRACE-D2R | PA, PT | Mycelium Bloom shall create a relationship of the selected type between the row element and the column element when "a user clicks an empty cell in the Relationship Matrix." | - | H |
| SSS-PA-TRACE-J8N | PA, PT | Mycelium Bloom shall delete the relationship between the row element and the column element when "a user removes a relationship from an occupied cell in the Relationship Matrix." | - | H |
| SSS-PA-TRACE-F5M | PA, PT, VW | Mycelium Bloom shall filter the Relationship Matrix by relationship type, Ownership, Applied MetaDataUsage or element type when "a user applies filters to the Relationship Matrix." | - | H |
| SSS-PA-TRACE-H4P | PA, PT, VW | Mycelium Bloom shall navigate to the detail panel of the related elements when "a user double-clicks an occupied cell in the Relationship Matrix." | - | H |
| SSS-PA-TRACE-8ZB | PA, PT | Mycelium Bloom shall create a typed relationship between any two model elements when "a user selects source and target elements and specifies a relationship type." | KerML 7.2.2 | H |
| SSS-PA-TRACE-V8K | PA, PT | Mycelium Bloom shall create a Dependency relationship between two model elements, asserting that the source element depends on the target element, when "a user creates a generic dependency between two model elements." | KerML 7.2.3 | H |

##### 5.2.1.14 Quantities, units, and measurement management

Numerical engineering values must always be expressed with a quantity kind, a measurement unit, and a measurement scale. The SysML v2 Quantities and Units Domain Library provides a normative model of these concepts as Attribute Definitions and Attribute Usages. Mycelium presents this library as user-friendly browsers for quantities, units, and scales, with drag-and-drop assignment of attributes to elements and the ability to import standard or custom libraries.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PA-QU-T3K | PA, PT, VW | Mycelium Bloom shall provide a tabular view listing all Attribute Definitions available in the project (including those imported from libraries) with their name, quantity kind, default unit, and source library when "a user opens the Attribute Definitions browser." | SysML 9.8 | H |
| SSS-PA-QU-R7N | PA, PT, VW | Mycelium Bloom shall provide a tabular view listing all Measurement Units available in the project with their name, symbol, unit type (simple, derived, prefixed), and associated quantity kind when "a user opens the Measurement Units browser." | SysML 9.8.3 | H |
| SSS-PA-QU-W5J | PA, PT, VW | Mycelium Bloom shall provide a tabular view listing all Measurement Scales available in the project with their name, scale type (ratio, interval, ordinal, cyclic ratio, logarithmic), unit, and value range when "a user opens the Measurement Scales browser." | SysML 9.8.3 | H |
| SSS-PA-QU-D8M | PA, PT, VW | Mycelium Bloom shall provide a tabular view listing all Quantity Kinds available in the project with their name, dimension symbol, and classification (base, derived, specialized) when "a user opens the Quantity Kinds browser." | SysML 9.8.2 | H |
| SSS-PA-QU-H2V | PA, PT | Mycelium Bloom shall provide operations to create, edit, and delete custom Attribute Definitions typed by a Quantity Kind with an associated Measurement Unit when "a user accesses the Attribute Definitions management interface of a project or a library." | SysML 9.8 | H |
| SSS-PA-QU-K6F | PA, PT | Mycelium Bloom shall provide operations to create, edit, and delete custom Measurement Units (simple, derived, prefixed) with conversion factors when "a user accesses the Measurement Units management interface of a project or a library." | SysML 9.8.3 | H |
| SSS-PA-QU-B4P | PA, PT | Mycelium Bloom shall provide operations to create, edit, and delete Measurement Scales (ratio, interval, ordinal, cyclic ratio, logarithmic) with their associated unit and value constraints when "a user accesses the Measurement Scales management interface of a project or a library." | SysML 9.8.3 | H |
| SSS-PA-QU-QK1 | PA, PT | Mycelium Bloom shall provide operations to create, edit, and delete custom Quantity Kinds (simple, specialized, derived) with their dimension when "a user accesses the Quantity Kinds management interface of a project or a library." | SysML 9.8.2 | H |
| SSS-PA-QU-N9X | PA, PT | Mycelium Bloom shall create an Attribute Usage typed by the dropped Attribute Definition on the target element when "a user drags an Attribute Definition from the Attribute Definitions browser and drops it onto an element Definition or Usage in the model browser or a diagram." | SysML 7.7 | H |
| SSS-PA-QU-G1W | PA, PT | Mycelium Bloom shall import Quantity Kinds, Measurement Units, Measurement Scales, and Attribute Definitions from the SysML v2 standard libraries (ISQ, SI, USCustomary) and from Mycelium Forge packages when "a user selects library content for import into a project." | SysML 9.8 | H |

##### 5.2.1.15 Enumerations

An Enumeration Definition is a value type whose instances are restricted to a fixed set of named literals, the non-numeric counterpart to the quantity-kind-and-unit typing of numeric attributes. Engineers use enumerations to constrain an attribute to a controlled vocabulary (e.g. operational mode, criticality class). The requirements in this section cover defining enumerations and their literals, displaying them, constraining attribute values to them, and validating those values.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PA-ARCH-9W5 | PA, PT | Mycelium Bloom shall create an Enumeration Definition that restricts its values to a fixed set of enumerated values when "a user creates an Enumeration Definition and specifies its literals." | SysML 7.8 | H |
| SSS-PA-ARCH-E1A | PA, PT | Mycelium Bloom shall add, rename, and remove Enumeration Literals of an Enumeration Definition, with optional Documentation per literal, when "a user edits the literal set of an Enumeration Definition in the detail panel." | SysML 7.8 | H |
| SSS-PA-ARCH-E2B | PA, PT | Mycelium Bloom shall reorder the Enumeration Literals of an Enumeration Definition when "a user changes the position in the literal list." | SysML 7.8 | H |
| SSS-PA-VIS-E3C | PA, PT, VW | Mycelium Bloom shall display the literal set of an Enumeration Definition in its detail panel, showing each literal's `name`, its ordinal position, and its Documentation, when "a user views an Enumeration Definition." | SysML 7.8 | H |
| SSS-PA-ARCH-E4D | PA, PT | Mycelium Bloom shall present the allowed literals of the typing Enumeration Definition as a dropdown selection when "a user edits the value of an Attribute Usage whose type is an Enumeration Definition." | SysML 7.8 | H |

##### 5.2.1.16 Behavior modeling

Beyond structure, systems exhibit behavior: actions performed, states held, transitions triggered, flows of items and data. SysML v2 provides Action Definitions, State Definitions, and Flow Definitions. The requirements in this section cover the behavioral modeling capabilities engineers need to describe what the system does and how its behavior depends on context. Subsections cover actions, states, flows, and performing and exhibiting behaviour on parts.

###### 5.2.1.16.a Actions

Actions define what a system does. An Action Definition specifies a behaviour with input and output parameters that can be decomposed into sub-actions and sequenced by control flow. Mycelium covers action definitions, the control nodes (succession, guard, fork, join, decision, merge), and the primitive and structured action nodes (accept, send, assignment, if, while, for). The requirements in this subsection cover defining actions, composing their control flow, and the individual action node kinds, together with server-side validation of action well-formedness.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PA-BEH-N5Z | PA, PT | Mycelium Bloom shall create an Action Definition with input and output parameters and decompose it into sub-actions when "a user creates or edits an Action Definition." | SysML 7.17 | H |
| SSS-PA-BEH-WG5 | PA, PT | Mycelium Bloom shall create control flow between actions using successions, guards, forks, joins, decisions, and merges when "a user creates control flow elements between existing actions." | SysML 7.17 | H |
| SSS-PA-BEH-Q4N | PA, PT | Mycelium Bloom shall create a generic Succession between two features (e.g. two actions, two states, or two arbitrary occurrences) establishing that the second feature follows the first when "a user creates a succession between two features outside the context of a state machine." | KerML 7.4.6.4 | H |
| SSS-PA-BEH-A1C | PA, PT | Mycelium Bloom shall create an Accept Action Usage that waits for an incoming payload matching a specified trigger Feature, optionally typed and guarded, when "a user adds an accept-action node to an Action Flow View or to an Action Definition in the detail panel." | SysML 7.17.5 | H |
| SSS-PA-BEH-S2N | PA, PT | Mycelium Bloom shall create a Send Action Usage that emits a payload Expression to a target Feature when "a user adds a send-action node to an Action Flow View or to an Action Definition in the detail panel." | SysML 7.17.5 | H |
| SSS-PA-BEH-A3S | PA, PT | Mycelium Bloom shall create an Assignment Action Usage that assigns the value of a source Expression to a target Feature when "a user adds an assignment-action node to an Action Flow View or to an Action Definition in the detail panel." | SysML 7.17.5 | H |
| SSS-PA-BEH-I4F | PA, PT | Mycelium Bloom shall create an If Action Usage composed of a Boolean condition Expression, a then-branch Action Usage, and an optional else-branch Action Usage when "a user adds an if-action to an Action Flow View or to an Action Definition." | SysML 7.17.5 | H |
| SSS-PA-BEH-W5H | PA, PT | Mycelium Bloom shall create a While Loop Action Usage composed of a Boolean condition Expression and a body Action Usage that executes as long as the condition holds when "a user adds a while-loop to an Action Flow View or to an Action Definition." | SysML 7.17.5 | H |
| SSS-PA-BEH-F6L | PA, PT | Mycelium Bloom shall create a For Loop Action Usage composed of a loop-variable Feature, a collection Expression, and a body Action Usage that executes once for each element of the collection when "a user adds a for-loop to an Action Flow View or to an Action Definition." | SysML 7.17.5 | H |

###### 5.2.1.16.b States

A State Definition models the conditions or modes a system holds over time, each with entry, do, and exit behaviour and transitions to other states. Mycelium supports composite states with nested states and parallel (orthogonal) regions, transitions of every kind with triggers, guards, and effects, and validation of state-machine well-formedness. The requirements in this subsection cover defining state machines, their states and transitions, and how they are displayed and validated.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PA-BEH-RPK | PA, PT | Mycelium Bloom shall create a State Definition with entry, do, and exit actions and connect its states via Transition Usages with triggers, guards, and effects when "a user creates or edits a State Definition." | SysML 7.18 | H |
| SSS-PA-BEH-SD1 | PA, PT | Mycelium Bloom shall designate one State Usage inside a composite State Definition as the default (initial) state entered when the containing state is entered, when "a user marks a State Usage as the default state of its parent State Definition." | SysML 7.18 | H |
| SSS-PA-BEH-SC2 | PA, PT | Mycelium Bloom shall define nested State Usages within a State Definition, producing a composite state machine in which each nested State Usage has its own entry, do, and exit Action, when "a user adds a child State Usage to a State Definition." | SysML 7.18 | H |
| SSS-PA-BEH-SP3 | PA, PT | Mycelium Bloom shall define parallel regions (orthogonal state machines) inside a State Definition, each with its own set of State Usages and Transition Usages, when "a user adds one or more parallel regions to a State Definition." | SysML 7.18 | H |
| SSS-PA-BEH-SE4 | PA, PT | Mycelium Bloom shall attach, replace, and remove an Entry Action, a Do Action, and an Exit Action on any State Usage, each realised as an Action Usage owned via the corresponding Feature Membership, when "a user edits the entry, do, or exit behavior of a State Usage." | SysML 7.18 | H |
| SSS-PA-BEH-TR5 | PA, PT | Mycelium Bloom shall create a Transition Usage of any of the following kinds: normal (between distinct source and target states), self (source and target are the same state), internal (no state exit or entry), or completion (no trigger, fires when the source state's Do Action completes), when "a user creates a transition in a State Transition View or via the detail panel." | SysML 7.18 | H |
| SSS-PA-BEH-TG6 | PA, PT | Mycelium Bloom shall set the trigger (an Accept Action Usage), the guard (a Boolean Expression), and the effect (an Action Usage) of a Transition Usage when "a user edits the trigger, guard, or effect of a Transition Usage." | SysML 7.18 | H |
| SSS-PA-VIS-SH7 | PA, PT, VW | Mycelium Bloom shall display the state-machine structure of a State Definition in its detail panel, showing the default state, the nested State Usages, the parallel regions, the Entry, Do, and Exit Actions on each State Usage, and the outgoing Transition Usages with their triggers, guards, and effects, when "a user views a State Definition that owns at least one State Usage or Transition Usage." | SysML 7.18 | H |
| SSS-FB-BEH-SV8 | - | Mycelium Fabric shall return a validation warning identifying any State Usage that is unreachable from the default state of its owning State Definition, and any State Usage that has two or more outgoing Transition Usages with the same trigger and an overlapping guard, when "a client runs model validation or submits a commit containing a State Definition." | SysML 7.18 | H |

###### 5.2.1.16.c Flows

A flow models the transfer of items, energy, or data between parts. SysML v2 expresses this with Flows and, where ordering matters, Succession Flows. The requirements in this subsection cover defining flows and the sequenced flows used, for example, to convey messages between lifelines in a Sequence View.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PA-BEH-PC7 | PA, PT | Mycelium Bloom shall create a Flow Definition and instantiate it as a Flow Usage to model the transfer of items, energy, or data between parts when "a user creates a Flow Definition and specifies the flow type and endpoints." | SysML 7.16 | H |
| SSS-PA-BEH-X9V | PA, PT | Mycelium Bloom shall create a Succession Flow Usage that conveys items between two features and establishes that the receiving end occurs after the sending end when "a user creates a sequenced flow between two features (e.g. a message between lifelines in a Sequence View, or an ordered item transfer between actions)." | KerML 7.4.10 | H |
| SSS-PA-BEH-D6L | PA, PT | Mycelium Bloom shall create the corresponding Succession Flow Usage in the underlying model when "a user draws a message arrow between two lifelines in a Sequence View." | KerML 7.4.10, SysML 8.2.3.9 | H |

###### 5.2.1.16.d Performing and exhibiting behaviour

Behaviour is connected to structure by performing actions and exhibiting states on the parts that carry them. The requirements in this subsection cover assigning behaviour to parts via Perform Action Usages and Exhibit State Usages.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PA-BEH-H83 | PA, PT | Mycelium Bloom shall assign behaviors to parts using Perform Action Usages and Exhibit State Usages when "a user selects a part and associates an action or state behavior with it." | SysML 7.17, 7.18 | H |

##### 5.2.1.17 Analysis and verification

Engineers need to evaluate design quality and verify that requirements are met. Mycelium supports Analysis Cases (evaluating system properties), Verification Cases (verifying requirements with methods and verdicts), Constraint Definitions (validation rules), and Calculation Definitions (domain-specific computations). The requirements in this section cover the analytical capabilities that turn the model into a basis for design decisions.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PA-AV-QII | PA, PT | Mycelium Bloom shall create an Analysis Case Definition with a subject and objective requirements when "a user creates an Analysis Case Definition and specifies its subject and objectives." | SysML 7.23 | H |
| SSS-PA-AV-AU1 | PA, PT | Mycelium Bloom shall instantiate an Analysis Case Definition as an Analysis Case Usage to evaluate system properties when "a user instantiates an Analysis Case." | SysML 7.23 | H |
| SSS-PA-AV-UCQ | PA, PT | Mycelium Bloom shall create a Verification Case Definition specifying a verification method (test, analysis, inspection, or demonstration) when "a user creates a Verification Case Definition and assigns a method." | SysML 7.24 | H |
| SSS-PA-AV-VU2 | PA, PT | Mycelium Bloom shall instantiate a Verification Case Definition as a Verification Case Usage and record its verdict (pass, fail, or inconclusive) when "a user runs a verification case and records a verdict." | SysML 7.24 | H |
| SSS-PA-AV-LSX | PA, PT | Mycelium Bloom shall create a Constraint Definition expressing a Boolean condition when "a user creates a Constraint Definition." | SysML 7.20 | H |
| SSS-PA-AV-CU3 | PA, PT | Mycelium Bloom shall instantiate a Constraint Definition as a Constraint Usage asserted against one or more model elements for automated validation when "a user applies a constraint to model elements." | SysML 7.20 | H |
| SSS-PA-AV-O9U | PA, PT | Mycelium Bloom shall link a Case (Use Case, Analysis Case, or Verification Case) to its objective Requirement by creating an ObjectiveMembership referencing the target Requirement Usage when "a user sets the objective of a Case from a selected Requirement." | SysML 8.3.22 | H |
| SSS-PT-ANALYSIS-4W2 | PT | Mycelium Bloom shall create a Calculation Definition with input parameters, output parameters, and a computation expression when "the Participant creates a Calculation Definition." | SysML 7.19 | H |
| SSS-PT-ANALYSIS-KU4 | PT | Mycelium Bloom shall instantiate a Calculation Definition as a Calculation Usage over model attributes when "the Participant instantiates a Calculation." | SysML 7.19 | H |

##### 5.2.1.18 In-browser scripting

Some analyses cannot be expressed declaratively and require imperative computation. Mass budgets, power budgets, and complex requirements verification often need iteration and aggregation across the system structure. The requirements in this section describe a desirable in-browser scripting environment that runs computational analyses against model data without leaving the application.

> Even though these requirements are set to M and L, being able to provide support for this in the future must be taken into account in the architecture of the web application.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| NA | NA    | NA          | NA  | NA   | NA     |

##### 5.2.1.19 Diagrams and visualization

Mycelium presents the model through a set of SysML v2 diagram types, each tailored to a modelling concern: structure, behaviour, requirements, and free-form exploration. All diagram types share a common graphical notation, drag-and-drop interaction with the model browser, and a round-trip in which editing a diagram updates the underlying model and editing the model updates every open diagram. The subsections below cover the shared diagramming and notation capabilities first, then each standard view, custom views and viewpoints, textual notation, and diagram export.

###### 5.2.1.19.1 General diagramming and notation

Mycelium Bloom must render model elements using the symbols defined in SysML v2 Part 1 section 8.2.3. This ensures that diagrams produced in and with Mycelium are immediately recognizable to anyone familiar with SysML v2 and exchangeable with other SysML v2 tools. The requirements in this section also cover diagram annotations, custom icons, and drag-and-drop interactions that apply to all diagram types.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PA-VIS-X4G | PA, PT, VW | Mycelium Bloom shall render all model elements using the graphical notation symbols defined in SysML v2 Part 1, section 8.2.3, including Definition and Usage node headers with guillemet kind designators (e.g. «part def», «action», «requirement»), compartment layouts, relationship lines, and adornments when "any diagram view displays model elements." | SysML 8.2.3 | H |
| SSS-PA-VIS-L7Q | PA, PT, VW | Mycelium Bloom shall visually distinguish Definition nodes from Usage nodes by displaying the `def` keyword in the guillemet header of Definition nodes (e.g. «part def») and omitting it for Usage nodes (e.g. «part») when "a diagram renders Definition and Usage elements." | SysML 8.2.3 | H |
| SSS-PA-VIS-R3F | PA, PT | Mycelium Bloom shall create the corresponding graphical node on the diagram canvas when "a user drags a model element from the model browser or a tabular browser and drops it onto a diagram." | - | H |
| SSS-PA-VIS-K8M | PA, PT | Mycelium Bloom shall create the corresponding model element in the underlying model when "a user creates a new graphical node or relationship on a diagram canvas using the diagram toolbox." | - | H |
| SSS-PA-VIS-H2W | PA, PT, VW | Mycelium Bloom shall reflect changes to model elements in all visible diagrams containing those elements when "a model element's properties are modified in any view." | - | H |
| SSS-PA-VIS-N6J | PA, PT | Mycelium Bloom shall provide a toolbox palette for each diagram type listing the element and relationship types that can be created on that diagram when "a user opens a diagram editor." | - | H |
| SSS-PA-VIS-U9P | PA, PT, VW | Mycelium Bloom shall display compartments on graphical nodes (attributes, constraints, ports, nested elements) per the SysML v2 compartment notation when "a user expands or views compartments on a diagram element." | SysML 8.2.3 | H |
| SSS-PA-VIS-D5B | PA, PT, VW | Mycelium Bloom shall display multiplicity, property modifiers (ordered, nonunique, abstract, derived, readonly), and subsetting/redefinition markers on graphical elements per the SysML v2 notation when "a diagram renders elements with these properties." | SysML 8.2.3 | H |
| SSS-PA-VIS-C9K | PA, PT | Mycelium Bloom shall provide an interface to upload or select a custom icon and image for any Definition or Usage element when "a user accesses the icon settings of a model element." | - | H |
| SSS-PA-VIS-J2R | PA, PT, VW | Mycelium Bloom shall render the custom icon next to or in place of the standard SysML v2 graphical notation symbol on all diagrams containing the element when "a model element has a custom icon associated with it and the settings of the element are configured such that the icon shall be visualized." | - | H |
| SSS-PA-VIS-J9M | PA, PT, VW | Mycelium Bloom shall render the custom image in place of the standard SysML v2 graphical notation symbol on all diagrams containing the element when "a model element has a custom image associated with it and the settings of the element are configured such that the image shall be visualized." | - | H |
| SSS-PA-VIS-A6F | PA, PT, VW | Mycelium Bloom shall display the element name and type designator alongside the custom icon when "a diagram renders an element with a custom icon." | - | H |

###### 5.2.1.19.2 Interconnection View

An Interconnection View shows the structural composition of a system: parts, the ports through which they interact, and the connections between those ports. This is the most common diagram type for system architecture work and the entry point for most reviews of the physical decomposition.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PA-VIS-G8N | PA, PT | Mycelium Bloom shall provide an editor for creating and editing Interconnection Views showing parts, ports, and connections when "a user opens or creates an Interconnection View for a selected model scope." | SysML 8.2.3.11 | H |
| SSS-PA-VIS-W3T | PA, PT, VW | Mycelium Bloom shall render Part Usages as rectangular nodes with «part» headers, Port Usages as small squares on part boundaries with directional indicators (in, out, inout), and Connection Usages as lines between ports, using the SysML v2 graphical notation (section 8.2.3.11-14) when "an Interconnection View displays structural model content." | SysML 8.2.3.11-14 | H |
| SSS-PA-VIS-Q7K | PA, PT, VW | Mycelium Bloom shall render Interface Usages as connection lines between ports with the «interface» label and optional constraint compartments using the SysML v2 graphical notation (section 8.2.3.14) when "an Interconnection View displays interface connections." | SysML 8.2.3.14 | H |
| SSS-PA-VIS-I4R | PA, PT, VW | Mycelium Bloom shall render an Item Usage in the model browser, tabular views, and diagrams with a distinguishing icon and the «item» stereotype label, visually distinct from a Part Usage, showing its name, its typing Item Definition, and its multiplicity, when "a user views an Item Usage." | SysML 8.2.3.10 | H |
| SSS-PA-VIS-I5S | PA, PT, VW | Mycelium Bloom shall render Item Usages on a structural diagram as rounded-corner nodes using the SysML v2 graphical notation, and shall create an Item Usage on the canvas by dragging an Item Definition from the model browser or the Item tool from the toolbox, when "a user adds or views an Item Usage on a structural diagram." | SysML 8.2.3.10 | H |
| SSS-PA-VIS-I6T | PA, PT, VW | Mycelium Bloom shall render the payload Item Usage of a Flow Usage alongside the flow line on a diagram, displaying the Item Usage name, its typing Item Definition, and its multiplicity, when "a user views a Flow Usage that carries an Item." | SysML 8.2.3.16 | H |

###### 5.2.1.19.3 Action Flow View

An Action Flow View shows the behavior of the system as a sequence of actions with control flow between them. Engineers use it to describe how the system performs its functions, including parallelism (forks/joins), decisions, and loops. The notation closely follows UML activity diagrams.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PA-VIS-SMC | PA, PT | Mycelium Bloom shall provide an editor for creating and editing Action Flow Views showing action sequencing, control flow, and swim lanes when "a user opens or creates an Action Flow View for a selected action hierarchy." | SysML 8.2.3.17 | H |
| SSS-PA-VIS-E4R | PA, PT, VW | Mycelium Bloom shall render Action Usages as rounded-corner rectangles with «action» headers, and control flow using the SysML v2 standard symbols: start node (filled circle), done node (circled filled circle), fork/join nodes (bars), decision/merge nodes (diamonds), and succession arrows, per section 8.2.3.17, when "an Action Flow View displays behavioral model content." | SysML 8.2.3.17 | H |
| SSS-PA-VIS-J6N | PA, PT, VW | Mycelium Bloom shall render input/output parameters as small rectangles on action node boundaries with directional indicators (in, out, inout) per the SysML v2 graphical notation (section 8.2.3.17) when "an Action Flow View displays actions with parameters." | SysML 8.2.3.17 | H |
| SSS-PA-VIS-M1Z | PA, PT, VW | Mycelium Bloom shall render send action nodes, accept action nodes, while-loop action nodes, for-loop action nodes, and if-else action nodes using the SysML v2 standard symbols (section 8.2.3.17) when "an Action Flow View displays these action types." | SysML 8.2.3.17 | H |

###### 5.2.1.19.4 State Transition View

A State Transition View shows the states a system or part can be in and the transitions between them, triggered by events with optional guards and effects. This is essential for modeling operational modes, fault handling, and any behavior that depends on context.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PA-VIS-DP2 | PA, PT | Mycelium Bloom shall provide an editor for creating and editing State Transition Views showing states and transitions when "a user opens or creates a State Transition View for a selected state machine." | SysML 8.2.3.18 | H |
| SSS-PA-VIS-B8V | PA, PT, VW | Mycelium Bloom shall render State Usages as rounded-corner rectangles with «state» headers containing entry/do/exit action compartments, and Transition Usages as arrows labeled with trigger [guard] / effect, using the SysML v2 graphical notation (section 8.2.3.18) when "a State Transition View displays state-based model content." | SysML 8.2.3.18 | H |
| SSS-PA-VIS-F2C | PA, PT, VW | Mycelium Bloom shall render parallel state regions using the «parallel» designator per the SysML v2 graphical notation (section 8.2.3.18) when "a State Transition View displays concurrent state regions." | SysML 8.2.3.18 | H |

###### 5.2.1.19.5 Sequence View

A Sequence View shows interactions between parts over time as messages exchanged along lifelines. Engineers use it to capture protocol flows, scenario walkthroughs, and timing-sensitive behaviors.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PA-VIS-FA5 | PA, PT | Mycelium Bloom shall provide an editor for creating and editing Sequence Views showing interactions between parts over time when "a user opens or creates a Sequence View for a selected interaction context." | SysML 8.2.3.9 | H |
| SSS-PA-VIS-A9H | PA, PT, VW | Mycelium Bloom shall render lifelines as vertical dashed lines below part/port header nodes, and messages as horizontal arrows between lifelines with message labels, using the SysML v2 graphical notation (section 8.2.3.9) when "a Sequence View displays interaction model content." | SysML 8.2.3.9 | H |

###### 5.2.1.19.6 Use Case View

A Use Case View shows the use cases a system supports, the actors that interact with them, and the system boundary (subject), together with the include and extend relationships between use cases. Engineers and stakeholders use it to frame system functionality from an external, goal-oriented perspective.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| NA | NA    | NA          | NA  | NA   | NA     |

###### 5.2.1.19.7 Requirement View

A Requirement View displays requirements and their satisfaction relationships graphically. Stakeholders can see which design elements satisfy which requirements at a glance, supporting reviews and impact analysis.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| NA | NA    | NA          | NA  | NA   | NA     |

###### 5.2.1.19.8 General View

A General View is an unconstrained canvas where engineers can place any model element type and freely arrange it. It supports brainstorming, mixed concept exploration, and stakeholder-facing presentations that don't fit a single standard diagram type.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PA-VIS-BB9 | PA, PT | Mycelium Bloom shall provide an editor for creating General Views for unconstrained graphical model exploration when "a user creates a new General View and adds model elements to its canvas." | SysML 8.2.3 | H |
| SSS-PA-VIS-P5W | PA, PT, VW | Mycelium Bloom shall create a graphical node for any model element type placed on a General View canvas, using its SysML v2 graphical notation symbol, when "a user adds an element to a General View." | SysML 8.2.3 | H |

###### 5.2.1.19.9 Grid View

A Grid View presents model data in tabular or matrix form. Engineers use it to compare attributes across many elements at once, or to view two-dimensional relationships between element sets.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| NA | NA    | NA          | NA  | NA   | NA     |

###### 5.2.1.19.10 Custom Views, Viewpoints, and Rendering

Different stakeholders have different concerns: a power engineer wants a power-focused view, a thermal engineer wants thermal data, a customer wants high-level summaries. SysML v2 Viewpoint Definitions and View Definitions let users formalize these stakeholder concerns and create reusable filtered views, and Rendering Definitions control how a view presents its exposed content. The requirements in this section cover defining and managing custom views and viewpoints, and selecting how a view renders its content.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| NA | NA    | NA          | NA  | NA   | NA     |

###### 5.2.1.19.11 Textual notation

SysML v2 has a textual notation that some engineers prefer for editing, reviewing or sharing model content. Mycelium generates this notation read-only from the model, providing a reference representation without requiring users to edit text directly.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PA-VIS-IXL | PA, PT, VW | Mycelium Bloom shall generate and display the SysML v2 textual notation representation of model elements (read-only) when "a user selects one or more model elements and requests textual notation view." | SysML 8.2.2 | H |

###### 5.2.1.19.12 Diagram export

Diagrams need to leave Mycelium for reports, presentations, and external tools. The requirements in this section cover export to SVG (vector), PNG (raster, configurable resolution), and JPG (compressed raster) formats.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PA-VIS-V7S | PA, PT, VW | Mycelium Bloom shall export a diagram to SVG format preserving vector graphics fidelity when "a user selects SVG as the export format for a diagram." | - | H |

###### 5.2.1.19.13 Diagram management and canvas operations

Beyond rendering, engineers need to manage diagrams as artifacts and arrange their content. The requirements in this subsection cover the lifecycle of a diagram and the canvas operations common to every diagram type. Diagram persistence and real-time collaboration are covered separately in 5.2.1.20.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PA-VIS-DM1 | PA, PT | Mycelium Bloom shall provide operations to create, open, rename, and delete diagrams and to list the diagrams of a project with their name and type when "a user accesses the project's diagram list." | - | H |
| SSS-PA-VIS-ZP3 | PA, PT, VW | Mycelium Bloom shall zoom, pan, and fit the diagram to the view when "a user zooms, pans, or invokes fit-to-view on a diagram." | - | H |

##### 5.2.1.20 Diagram persistence and real-time collaboration

A diagram in Mycelium Bloom is more than a transient rendering of the underlying model: it is a durable, first-class artifact with its own identity, layout, and collaboration state. KerML and SysML v2 do not (yet) define an abstract syntax for diagram layout persistence, so there is no standard metaclass describing node positions, routing waypoints, or custom per-diagram rendering overrides. OMG is defining a standard library, using SysML v2 constructs, for exchanging diagrams; once it is available Mycelium will use it to exchange diagram-related information. The requirements in this section also state that diagrams participate in Mycelium's lock-free collaboration model and display live presence and activity indicators for every user currently working on the same diagram.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PA-VIS-P1A | PA, PT, VW | Mycelium Bloom shall persist every diagram, including its identity (unique identifier, name, diagram type, description), its associated model scope, the set of displayed model elements, the layout of each node (position, size, collapsed or expanded state, custom icon override, visibility) and the routing of each relationship (waypoints, label position, line style), when "a user creates or edits a diagram." | - | H |
| SSS-FB-VIS-P3C | - | Mycelium Fabric shall persist and serve diagram layout content alongside the model content it annotates, applying the same commit, branch, merge, and ownership semantics to the diagram content as to the model elements, when "a client submits a commit containing diagram layout content or queries a diagram." | - | H |
| SSS-PA-VIS-C4D | PA, PT, VW | Mycelium Bloom shall render the same diagram to multiple users for simultaneous viewing and editing, without acquiring a lock on the diagram or on any of its graphical elements, consistent with the lock-free collaboration model defined in `SSS-CC-COLLAB-62C`, when "more than one user has the same diagram open." | - | H |
| SSS-PA-VIS-C5E | PA, PT, VW | Mycelium Bloom shall propagate every diagram change (node creation, move, resize, deletion, relationship creation, routing edit, label edit, property edit, and any model-side edit that affects a rendered element) to every other user currently viewing the same diagram in near real time via Mycelium Fabric's notification channel, when "a user modifies a diagram element." | - | H |
| SSS-PA-VIS-C6F | PA, PT, VW | Mycelium Bloom shall display, on every open diagram, the list of users currently viewing or editing it, showing each user's display name, avatar, and assigned collaborator colour, when "at least one user has the same diagram open." | - | H |

##### 5.2.1.21 3D model viewer

Spatial decomposition is most intuitive in 3D. Mycelium offers an interactive 3D viewer whose **primary** source of geometry is a set of SysML v2 Attribute Usages on each Part Usage (centre of gravity, orientation, basic shape, and dimensions) sourced from Attribute Definitions that live in a dedicated Mycelium Library Package. As a deferred capability, a Part Usage may additionally carry an attached STEP file, which Mycelium can use as the authoritative rendering source. Users can navigate the scene, select elements to inspect properties, and see Ownership-based colour coding to understand who is responsible for what. When the attribute values are updated (location, orientation, dimensions, shape) the interactive 3D viewer updates as well.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| NA | NA    | NA          | NA  | NA   | NA     |

##### 5.2.1.22 Queries

Engineers need to ask questions of their models: list all elements categorized as Equipment, find all requirements with no Satisfy relationship, retrieve all parts above a mass threshold. Mycelium offers a query interface based on the Systems Modelling API query operations, with the ability to save and re-execute queries against any commit.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| NA | NA    | NA          | NA  | NA   | NA     |

##### 5.2.1.23 Reporting and dashboards

Beyond raw data, engineers and stakeholders need summary views showing model health, progress, and metrics. Mycelium provides dashboards for system monitoring, validation, and project model health, with click-through navigation from summary metrics to underlying elements.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| NA | NA    | NA          | NA  | NA   | NA     |

###### 5.2.1.23a Project model dashboard

The project model dashboard gives the study lead and team a single view of model health: how many attributes are published vs unpublished, how many elements are unused, what the distribution of element types and Ownerships looks like, and how requirements coverage and constraint compliance are progressing. The requirements in this section cover histograms, pie charts, summary metrics, filtering, and click-through navigation, inspired by the equivalent CDP4-COMET-WEB dashboard.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PA-DASH-K7R | PA, PT, VW | Mycelium Bloom shall display a project model dashboard presenting an overview of model health and completeness when "a user opens the project model dashboard." | - | H |
| SSS-PA-DASH-W3D | PA, PT, VW | Mycelium Bloom shall display a histogram and summary count of published vs unpublished attributes per Ownership when "the project model dashboard renders the publication status section." | - | H |
| SSS-PA-DASH-N8F | PA, PT, VW | Mycelium Bloom shall display a histogram and summary count of attributes with missing values (no value assigned) grouped by Ownership when "the project model dashboard renders the missing values section." | - | H |
| SSS-PA-DASH-H2T | PA, PT, VW | Mycelium Bloom shall display a histogram and summary count of unused Definitions (Definitions with no Usages in the model) grouped by element type when "the project model dashboard renders the unused definitions section." | - | H |
| SSS-PA-DASH-D5J | PA, PT, VW | Mycelium Bloom shall display a histogram and summary count of unreferenced Usages (Usages not connected via any relationship, port, or connection to other elements) grouped by element type when "the project model dashboard renders the unreferenced elements section." | - | H |
| SSS-PA-DASH-M6W | PA, PT, VW | Mycelium Bloom shall display a summary of requirements coverage showing the count and percentage of requirements with Satisfy relationships, Verification Case links, and unallocated requirements when "the project model dashboard renders the requirements coverage section." | - | H |
| SSS-PA-DASH-V8G | PA, PT, VW | Mycelium Bloom shall display a summary of subscription activity showing the count of active ParameterSubscriptions and the count of subscribed attributes with stale (unpublished) values when "the project model dashboard renders the subscription status section." | - | H |

###### 5.2.1.23b History and trends

Beyond a snapshot of current model health, engineers track how the model evolves over time. Mycelium plots attribute values, element change history, and project-level metrics across the Commits and Tags of a branch. The requirements in this subsection cover attribute value history, per-element change history and diffs, and trend charts for requirements coverage, verification status, constraint compliance, and model growth.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PA-HIST-K3R | PA, PT, VW | Mycelium Bloom shall display the value history of one or more selected attributes as a time-series chart plotting the attribute values across Commits or Tags on the active branch when "a user opens the attribute history view and selects one or more attributes." | - | H |
| SSS-PA-HIST-R9G | PA, PT, VW | Mycelium Bloom shall display model growth metrics (total element count, total relationship count, total attribute count) as a chart across Commits or Tags when "a user opens the model growth trend view." | - | H |

##### 5.2.1.24 User interface adaptation

Mycelium supports novice, intermediate, and expert users. The interface should adapt to the user's role and Ownership, surface commonly-used features prominently, and provide context-aware help. The requirements in this section cover role-aware interface adaptation, workspace customization, and the About dialog.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PT-UI-NLJ | All | Mycelium Bloom shall provide a role-aware interface that surfaces information and tools relevant to the user's assigned Ownership and tasks when "a user logs in and the application loads their role and Ownership assignments." | - | H |

##### 5.2.1.25 Import, export and migration

Mycelium must interoperate with the broader MBSE ecosystem. Models can be imported and exported in SysML v2 JSON, requirements in ReqIF, content in HTML for documentation. CDP4-COMET ECSS-E-TM-10-25 models can be migrated to SysML v2 via a semi-automated converter. The requirements in this section cover all import, export, and migration capabilities.

###### 5.2.1.25.a Model interchange

Mycelium exchanges model content with other tools and projects. SysML v2 abstract syntax is interchanged as JSON or XMI, requirements as ReqIF, and elements can be referenced live across projects. The requirements in this subsection cover importing and exporting model content and referencing elements from other projects.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PA-IE-QWN | PA | Mycelium Bloom shall import and export models in the standard SysML v2 JSON or XMI serialisation of the abstract syntax, with the JSON serialisation compliant with the OMG Systems Modelling API, when "the Project Administrator initiates an import or export operation and selects the format and the target file or endpoint." | API 7 | H |
| SSS-PA-REQ-D7V | PA | Mycelium Bloom shall import requirements from a ReqIF file when "the Project Administrator initiates an import operation and selects a ReqIF file to import." | - | H |

###### 5.2.1.25.b Migration from CDP4-COMET

Existing CDP4-COMET models, based on ECSS-E-TM-10-25, can be brought into Mycelium and converted to SysML v2 by a semi-automated converter. The requirements in this subsection cover the migration process, the resolution of mapping ambiguities, and the migration report.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PA-IE-ZLQ | PA | Mycelium Bloom shall migrate existing ECSS-E-TM-10-25 models from CDP4-COMET into SysML v2 using a semi-automated converter when "the Project Administrator uploads an ECSS-E-TM-10-25 model and initiates the migration process." | - | H |
| SSS-PA-IE-YSY | PA | Mycelium Bloom shall present mapping ambiguities for user resolution during ECSS-to-SysML v2 migration when "the converter encounters ECSS-E-TM-10-25 elements that do not have a deterministic SysML v2 mapping." | - | H |
| SSS-PA-IE-MR1 | PA | Mycelium Bloom shall produce a migration report listing the ECSS-E-TM-10-25 elements that were migrated, skipped, or failed, together with their resolved SysML v2 mapping and a reference to the original source element, when "an ECSS-to-SysML v2 migration completes." | - | H |

###### 5.2.1.25.c Document and view export

Model content leaves Mycelium as human-readable documents for reports, reviews, and stakeholders. The requirements in this subsection cover export of views, diagrams, and reports to standard formats and the generation of navigable HTML documents from requirements and model element selections.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| NA | NA    | NA          | NA  | NA   | NA     |

##### 5.2.1.26 Metadata

SysML v2 Metadata Definitions and Metadata Usages attach tool-specific or process-specific information to model elements without changing their semantics. Mycelium itself is built on this mechanism: ownership (`Owner`), parameter subscriptions (`ParameterSubscription`), and the publication workflow (`PublishedIn`, `OwnedValue`) are Metadata Definitions in the Concurrent Design library (see [Roles and Permissions](Roles-and-Permissions.md)). Metadata Definitions and Usages are similar to the Category concept in CDP4-COMET. The requirements in this section cover user-defined metadata: defining annotation types, applying them to elements, editing their values, and finding elements by their annotations.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PA-META-K7R | PA, PT | Mycelium Bloom shall create a Metadata Definition whose owned Attribute Usages, typed by Attribute Definitions, define the data fields of the annotation, when "a user creates a Metadata Definition and specifies its attributes." | SysML 7.27 | H |
| SSS-PA-META-W3D | PA, PT | Mycelium Bloom shall annotate a model element with a Metadata Usage typed by a selected Metadata Definition when "a user applies metadata to a model element from the detail panel, context menu or drag-n-drop operation." | SysML 7.27 | H |
| SSS-PA-META-N8F | PA, PT | Mycelium Bloom shall set the attribute values of a Metadata Usage, validated against their typing Attribute Definitions, when "a user fills in the attribute values of an applied Metadata Usage." | SysML 7.27 | H |
| SSS-PA-META-J1B | PA, PT | Mycelium Bloom shall edit the attribute values of an existing Metadata Usage when "a user modifies a metadata annotation on an element." | SysML 7.27 | H |
| SSS-PA-META-D5J | PA, PT | Mycelium Bloom shall delete a Metadata Usage from an annotated element when "a user removes a metadata annotation via the detail panel or context menu." | SysML 7.27 | H |
| SSS-PA-META-T4K | PA, PT, VW | Mycelium Bloom shall display the Metadata Usages applied to a model element, showing each annotation's Metadata Definition and attribute values, in the detail panel, when "a user views a model element's properties." | - | H |

##### 5.2.1.27 Comments and documentation

SysML v2 defines Comment as an annotating element with a textual body that can describe one or more model elements, and Documentation as a specialized Comment that formally documents its owning element. Comments and Documentation are the primary mechanism for adding explanatory text, rationale, design notes, and review feedback to model elements.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PA-CMT-R4K | PA, PT | Mycelium Bloom shall create a Comment with a textual body on one or more model elements when "a user adds a comment to a model element via the detail panel or context menu." | KerML 7.2.4.2 | H |
| SSS-PA-CMT-W7N | PA, PT | Mycelium Bloom shall create a Documentation element on a model element, representing its formal description, when "a user adds or edits the documentation of a model element." | KerML 7.2.4.2 | H |
| SSS-PA-CMT-H3D | PA, PT | Mycelium Bloom shall edit and delete Comments and Documentation owned by the user's Ownership when "a user modifies or removes a comment or documentation entry." | KerML 7.2.4 | H |
| SSS-PA-CMT-M6J | PA, PT, VW | Mycelium Bloom shall display all Comments and Documentation associated with a model element in the detail panel, showing the text body, author, and creation date, when "a user views a model element's properties." | KerML 7.2.4 | H |
| SSS-PA-CMT-T9F | PA, PT, VW | Mycelium Bloom shall indicate in the model browser that an element has Comments or Documentation attached using a visual indicator (e.g. icon or badge) when "an element has one or more Comments or Documentation entries." | KerML 7.2.4 | H |
| SSS-PA-CMT-L7X | PA, PT | Mycelium Bloom shall create an AnnotatingElement (Comment, Documentation, Textual Representation, or Metadata Feature) together with its Annotation relationship(s) to one or more target elements in a single user operation when "a user draws a line in a diagram from the annotation tool in the toolbox, or from an existing annotation node, to one or more diagram nodes." | KerML 7.2.4.1 | H |
| SSS-PA-CMT-Z9K | PA, PT | Mycelium Bloom shall create an AnnotatingElement (Comment, Documentation, Textual Representation, or Metadata Feature) together with its Annotation relationship(s) to the currently selected model element(s) when "a user invokes an 'Add Comment', 'Add Documentation', 'Add Textual Representation', or 'Apply Metadata' action from the context (right-click) menu or from the detail panel of a list or tabular view." | KerML 7.2.4.1 | H |

##### 5.2.1.28 Review workflow

Branch protection rules can require designated Reviewers to approve merges before they enter the default branch. The requirements in this section cover the reviewer interface for approving or requesting changes on protected branch merges, supporting the gatekeeper model for design baselines.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| NA | NA    | NA          | NA  | NA   | NA     |

##### 5.2.1.29 Attachments

Engineering elements often need supporting documentation: thermal analysis PDFs, interface drawings, datasheets, photographs, spreadsheets. Mycelium lets users attach files of any type to any model element and download them later. The requirements in this section cover upload, download, listing, removal, and inline preview for common formats.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| NA | NA    | NA          | NA  | NA   | NA     |

##### 5.2.1.30 Glossary of Terms

Engineering teams need a shared vocabulary. Acronyms, domain terms, and project-specific definitions should be discoverable wherever they appear. Mycelium models a glossary as a Package of Item Definitions with Documentation, and the user interface highlights terms throughout the application with tooltips and click-through navigation. This makes the glossary live and contextual rather than a forgotten document.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| NA | NA    | NA          | NA  | NA   | NA     |

##### 5.2.1.31 Constants

Engineering models reference physical and project-specific constants (the speed of light, gravitational acceleration, target margins). Modelling these as named, typed Attribute Definitions with fixed values and source references makes them reusable across the project and traceable to their origin. Users can drag a constant into any constraint or calculation to ensure consistent values.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| NA | NA    | NA          | NA  | NA   | NA     |

##### 5.2.1.32 Version control and branching

Mycelium models are versioned like source code. Every change becomes a Commit; alternatives live on Branches; milestones are marked with Tags; merges combine work from different lines. The requirements in this section cover the full Systems Modelling API version control model adapted to a collaborative MBSE context, including a Git-style history graph for navigating commits and branches.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PA-VC-8SB | PA, PT | Mycelium Bloom shall create a Commit representing an immutable, non-destructible snapshot of model changes, consistent with the Systems Modelling API Commit concept, when "a user submits pending model changes with a commit description." | API 7.2.3 | H |
| SSS-PA-VC-PPI | PA, PT | Mycelium Bloom shall provide operations to create and manage Branches as independent lines of model development, each pointing to a head Commit, when "a user creates a new Branch from an existing Commit or manages existing Branches." | API 7.2.2 | H |
| SSS-PA-VC-SPJ | PA | Mycelium Bloom shall create an immutable Tag on a specific Commit to mark a milestone, baseline, or release when "the Project Administrator selects a Commit and assigns a Tag name." | API 7.2.4 | H |
| SSS-PA-VC-P89 | PA, PT, VW | Mycelium Bloom shall display the differences between two Commits showing which elements were created, updated, or deleted, consistent with the Systems Modelling API diffCommits operation, when "a user selects two Commits for comparison." | API 7.2.6 | H |
| SSS-PA-VC-7S4 | PA, PT, VW | Mycelium Bloom shall retrieve and display the complete versioned data of a Project at any Commit when "a user selects a historical Commit for inspection." | API 7.2.3 | H |
| SSS-PA-VC-28D | PA | Mycelium Bloom shall provide operations to designate Participants or Viewers as Reviewers for protected branches when "the Project Administrator assigns reviewers in the branch protection settings." | - | H |
| SSS-PA-VC-V3K | PA, PT | Mycelium Bloom shall switch the active branch, loading the model state at the head Commit of the selected branch, when "a user selects a different branch from the branch selector." | API 7.2.2 | H |
| SSS-PA-VC-R8W | PA, PT, VW | Mycelium Bloom shall display the currently active branch name in the application header when "a user is working in a project." | - | H |
| SSS-PA-VC-H4N | PA, PT, VW | Mycelium Bloom shall display a list of all branches in the project with their name, head Commit, creator, and creation date when "a user opens the branch management view." | API 7.2.2 | H |
| SSS-PA-VC-D7J | PA | Mycelium Bloom shall delete a non-default branch when "the Project Administrator initiates branch deletion and confirms the action." | API 7.2.2 | H |
| SSS-PA-VC-M1F | PA, PT, VW | Mycelium Bloom shall display the commit and branch history as a graph visualization with parallel lanes for branches, commit nodes, merge lines, and tag markers when "a user opens the version history graph view." | - | H |
| SSS-PA-VC-W5T | PA, PT, VW | Mycelium Bloom shall display commit metadata (author, date, description, changed element count) in a detail panel when "a user selects a commit node in the version history graph." | API 7.2.3 | H |
| SSS-PA-VC-N9B | PA, PT, VW | Mycelium Bloom shall highlight the active branch and its head Commit in the version history graph when "the version history graph is displayed." | - | H |
| SSS-PA-VC-F2G | PA, PT, VW | Mycelium Bloom shall load the complete model state at a selected historical Commit in read-only mode when "a user selects a Commit other than the head Commit from the version history graph, branch list, or commit history." | API 7.2.3 | H |
| SSS-PA-VC-J6K | PA, PT, VW | Mycelium Bloom shall display a visual indicator (e.g. banner or badge) stating the Commit identifier and date, making clear the user is viewing a historical snapshot and not the current head, when "the model is loaded at a historical Commit." | - | H |
| SSS-PA-VC-T3P | PA, PT | Mycelium Bloom shall create a new Branch from a selected historical Commit when "a user chooses to branch from a historical Commit to continue development from that point in time." | API 7.2.2 | H |
| SSS-PA-VC-B8W | PA, PT, VW | Mycelium Bloom shall return to the head Commit of the active branch when "a user exits the historical snapshot view." | - | H |
| SSS-PA-OPT-09P | PA, PT | Mycelium Bloom shall create a Branch for a design alternative, where each Branch represents an independent line of development for a candidate solution, when "a user creates a Branch for a design alternative from an existing Commit." | API 7.2.2 | H |
| SSS-PA-VC-TG2 | PA | Mycelium Bloom shall provide operations to list and delete Tags, showing each Tag's name, target Commit, and creator, when "a user accesses the tag management view." | API 7.2.4 | H |
| SSS-PA-VC-DB3 | PA | Mycelium Bloom shall set the default branch of a project when "the Project Administrator designates a branch as the default in the branch management view." | API 7.2.2 | H |
| SSS-PA-VC-RN4 | PA, PT | Mycelium Bloom shall rename a non-default branch when "a user renames a branch in the branch management view." | API 7.2.2 | H |

##### 5.2.1.33 Multi-backend support and polling

Mycelium Bloom must work not only with Mycelium Fabric but with any backend that implements the OMG Systems Modelling API. Some backends support push notifications (SignalR/WebSocket); others do not. The requirements in this section cover backend portability and a polling fallback for backends without push capability.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-CC-BACK-M1V | All | Mycelium Bloom shall provide a manual refresh operation that retrieves the complete current model state from the connected backend when "a user initiates a manual refresh." | - | H |

#### 5.2.2 Mycelium Fabric

##### 5.2.2.1 Systems Modelling API

Mycelium Fabric implements the OMG Systems Modelling API and Services specification. This is what makes Mycelium a SysML v2 native platform and what enables interoperability with other tools that consume the standard API. The requirements in this section anchor the Fabric implementation to the standard.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-CC-STD-XSS | All | Mycelium Fabric shall implement the OMG Systems Modelling API and Services specification (formal/25-09-04) using the REST/HTTP PSM when "the model server processes any API request." | API 7 | H |
| SSS-CC-EXT-QIN | All | Mycelium Fabric shall expose a REST API compliant with the OMG Systems Modelling API to enable integration with domain-specific tools when "an external tool issues API requests to the model server." | API 7 | H |

##### 5.2.2.2 Authentication and authorization

User identity, credentials, and session management are handled by Mycelium Fabric in conjunction with an external identity provider (Keycloak by default). The requirements in this section cover authentication enforcement, security policy enforcement, and the user invitation mechanism.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-OA-AUTH-SI7 | All | Mycelium Fabric shall authenticate all user sessions using token-based authentication when "a user submits valid credentials at the login interface." | - | H |
| SSS-FB-AUTH-L0Z | All | Mycelium Fabric shall send an invitation to a user to join the organization as a Member when "the Organization Administrator submits an invitation with the target user's identity (i.e. username or valid email address)." | - | H |
| SSS-FB-IA-R4X | All | Mycelium Fabric shall restrict installation-wide management API endpoints to users with the Installation Administrator role when "a user attempts to access installation administration operations." | - | H |
| SSS-FB-IA-J6C | All | Mycelium Fabric shall assign the Installation Administrator role to the first user who completes the initial setup on an on-premise deployment when "the installation has no existing Installation Administrator." | - | H |
| SSS-PA-STATE-H5J | All | Mycelium Fabric shall reject all create, modify, and delete operations on model elements when "the project is in the Review or Archived state." | - | H |

##### 5.2.2.3 Ownership enforcement

Ownership-based access control is enforced server-side by Mycelium Fabric, Bloom merely presents the UI for it. The requirements in this section ensure that no user can bypass ownership rules by talking directly to the Fabric API or by using a different client.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-CC-COLLAB-KOR | All | Mycelium Fabric shall enforce ownership-based access control using Owner metadata annotations on model elements when "a user attempts to create, modify, or delete a model element." | - | H |
| SSS-PT-COLLAB-G8G | All | Mycelium Fabric shall prevent modification of elements and attributes not owned by the user when "a Participant attempts to edit an element whose Owner metadata does not match their assigned Ownership." | - | H |
| SSS-PT-SUB-R8M | All | Mycelium Fabric shall reject creation of a ParameterSubscription on an AttributeUsage owned by the subscriber's own Ownership when "a Participant attempts to subscribe to an attribute owned by their own Ownership." | - | H |

##### 5.2.2.4 Model Validation and Commit Rejection

Mycelium Fabric is the guardian of model well-formedness. It accepts a commit only if the resulting model conforms to the KerML and SysML v2 abstract syntax (metaclass typing, multiplicities, and containment) and satisfies every OCL well-formedness constraint those specifications define on their metaclasses. Conformance to the specification is captured normatively by the first requirement below; the complete and authoritative set of checks is the abstract syntax together with the named OCL constraints in KerML (formal/25-09-03) and SysML v2 (formal/25-09-03), which this document does not restate. The remaining requirements add Mycelium-specific validation that the specifications do not mandate, such as library-package immutability and model-quality warnings.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-FB-VALID-CNF | - | Mycelium Fabric shall reject any commit whose resulting model would violate the KerML or SysML v2 abstract syntax or any OCL well-formedness constraint defined on the affected metaclasses, returning a validation error that identifies the violated constraint and the offending element, when "a client submits a commit." | KerML 8, SysML 8 | H |
| SSS-FB-PKG-L2F | - | Mycelium Fabric shall reject any commit that modifies the owned content of a LibraryPackage (including creation, modification, deletion, or re-parenting of any of its members) and shall return a validation error identifying the LibraryPackage, when "a client submits a commit that would mutate a LibraryPackage." | KerML 7.4.14 | H |
| SSS-FB-ELEM-CD8 | - | Mycelium Fabric shall reject any commit that introduces a circular composite containment, in which a Definition is directly or transitively the type of a composite Usage that it owns, returning a validation error identifying the cycle, when "a client submits such a commit." | KerML 7.3.4.2 | H |

##### 5.2.2.5 Real-time notifications

Mycelium Fabric is responsible for propagating model changes to all connected clients in near real-time, enabling the live update behavior in Bloom. The requirements in this section cover the server-side notification mechanism using SignalR.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-CC-COLLAB-TLB | All | Mycelium Fabric shall propagate model changes to all connected users in near real-time when "a user commits changes to the shared model." | API 7 | H |

##### 5.2.2.6 Model persistence and versioning

Mycelium Fabric persists model data in a relational (TBC) database with auto-generated schemas from the SysML v2 metamodel. The requirements in this section cover persistence performance and API responsiveness targets.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-CC-PERF-6HL | All | Mycelium Fabric shall persist models with up to 50,000 (TBC) elements within a responsive timeframe (target TBD) when "a user commits changes to a model containing up to 50,000 (TBC) elements." | - | H |
| SSS-CC-PERF-WTU | All | Mycelium Fabric shall respond to standard REST API requests within a responsive timeframe (target TBD) when "an external client or the web application issues an API request to the model server." | - | H |

##### 5.2.2.7 Concurrent design support

Lock-free collaboration is fundamental to concurrent design, no user can block others from editing the model. The requirements in this section anchor the server-side support for lock-free collaboration with optimistic concurrency.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-CC-COLLAB-62C | All | Mycelium Fabric shall support owner/ownership-based lock-free collaborative modeling where no single user can block others from updating the model when "multiple users concurrently modify different elements within the same project." | - | H |

---

#### 5.2.3 Mycelium Forge

##### 5.2.3.1 Package registry

Mycelium Forge is the package registry for the Mycelium ecosystem. It takes its design cues from established, widely-used public registries, **nuget.org**, **Maven Central**, and **PyPI**, and applies them to SysML v2 libraries. Libraries are distributed as **kpar** files (KerML Project Archive, defined in KerML clause 10.3, the Mycelium analogue of `.nupkg`, `.jar`, and `.whl`), each carrying a manifest, the library's KerML/SysML v2 source, a resolved API representation, and optional readme and release notes. The registry is addressable through three independent surfaces that all sit on top of the same backing store: a public web UI for human browsing, a documented HTTP API for programmatic use, and a first-party client library that wraps that API and is embedded directly in Mycelium Bloom so that users can search, preview, import, and update packages without leaving the modelling environment.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| NA | NA    | NA          | NA  | NA   | NA     |

##### 5.2.3.2 Library management

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-PA-IE-OYJ | PA | Mycelium Forge shall provide standard SysML v2 libraries (e.g. Quantities and Units, standard view definitions) for import into a project when "the Project Administrator selects one or more standard libraries for import." | SysML 9.8 | H |

##### 5.2.3.3 Authentication and authorization

Mycelium Forge reuses the identity plumbing that Mycelium Fabric already provides, external identity provider backed Mycelium Accounts and Fabric Organizations, and layers a Forge-specific per-package role model on top. A package has a set of Maintainers drawn from Accounts and Organizations; at least one individual-Account Owner must always exist; ownership is transferable between Accounts and Organizations with explicit acceptance by the receiving party.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-FG-AUTH-S1A | IA, OA, PA, PT, VW | Mycelium Forge shall authenticate users through the same external identity provider used by Mycelium Fabric, accepting a valid Mycelium Account session without requiring Forge-specific registration, when "a user signs in to Mycelium Forge through the web interface, the HTTP API, or the Forge client library." | - | H |
| SSS-FG-AUTH-S2B | - | Mycelium Forge shall use a scoped package identifier of the form `@<scope>/<package-name>`, where `<scope>` resolves to the slug of a Mycelium Account or a Fabric Organization, reserving the unscoped (global) namespace for standard libraries distributed by the Mycelium operator, when "a client publishes, queries, or downloads a package." | - | H |
| SSS-FG-AUTH-M3C | - | Mycelium Forge shall maintain, for every package, a Maintainer set whose entries are Mycelium Accounts and/or Fabric Organizations, each entry holding the role `Owner` or `Maintainer`. An `Owner` may transfer or share ownership, add or remove Maintainers, and unlist or delete versions. A `Maintainer` may publish new versions and unlist versions but shall not modify the Maintainer set. The package's metadata, display name, description, authors, license, tags, dependencies, and release notes, is sourced from the manifest contained in the kpar of each published version and shall not be edited by any role outside of publishing a new version, when "any authenticated request operates on a package's content or ownership." | - | H |
| SSS-FG-AUTH-O4D | - | Mycelium Forge shall reject any operation, removal of a Maintainer, role downgrade, ownership transfer, or Account deletion, that would leave a package with zero individual-Account Owners, and shall require the operation to first install another individual-Account Owner, when "an authenticated client submits a change to a package's Maintainer set or the Mycelium platform deletes an Account that is the last individual Owner of one or more packages." | - | H |
| SSS-FG-AUTH-T5E | - | Mycelium Forge shall transfer or share Ownership of a package only after the receiving Account or the receiving Fabric Organization has explicitly accepted the transfer through the Forge web interface or the Forge client library, leaving the original Maintainer set unchanged until acceptance occurs, when "an Owner initiates a transfer of, or an addition to, a package's Owner set." | - | H |
| SSS-FG-AUTH-G6F | OA | Mycelium Forge shall accept a Fabric Organization as a Maintainer or Owner of a package, granting publish, unlist, and, when the role is `Owner`, ownership-management authority to the Organization's Organization Administrators and to any Organization Member explicitly granted the `Forge Publisher` role by an Organization Administrator, when "an authenticated member of an Organization that holds such a role submits an operation against the package." | - | H |
| SSS-FG-AUTH-P7G | - | Mycelium Forge shall treat a Fabric Organization entry in a package's Maintainer set as a group Owner that does not satisfy the 'at least one individual Owner' invariant of `SSS-FG-AUTH-O4D` on its own; an individual-Account Owner shall remain present alongside any Organization Owner, when "a Maintainer set is established or modified." | - | H |

### 5.3 System interface requirements

This section specifies the interfaces across the Mycelium software boundary, the protocols, identity providers, data formats, human-machine interfaces, and external service integrations through which Mycelium communicates with the outside world. Each requirement below identifies *that* an interface exists and the standards or versions it is expected to comply with. Where a capability is described in §5.2, that description remains the normative capability requirement and this section only pins the interface contract.

KerML-only content is handled through the same SysML v2 abstract-syntax channels: because SysML v2 specialises KerML, any KerML instance is representable in the JSON, XMI, and MessagePack payloads accepted under `SSS-CC-EXT-IN1`, `SSS-CC-EXT-IN2`, and `SSS-CC-EXT-IN3`.

Interfaces between Mycelium Bloom, Mycelium Fabric, and Mycelium Forge, i.e. between the three Mycelium components themselves, are described in §4.4 (Operational environment).

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-CC-EXT-AP1 | - | Mycelium Fabric shall expose the OMG Systems Modelling API and Services (formal/25-09-04) as its primary external programmatic interface over HTTPS with TLS 1.2 or later, when "an external client issues API requests to Mycelium Fabric." | API 7 | H |
| SSS-CC-EXT-WS1 | - | Mycelium Fabric shall deliver near-real-time model-change notifications over a WebSocket transport (SignalR), protected by TLS 1.2 or later and authenticated with the same session credentials as the REST API, when "a client subscribes to live updates from Mycelium Fabric." | - | H |
| SSS-CC-EXT-FG1 | - | Mycelium Forge shall expose the Forge HTTP API specified in §5.2.3.1 over HTTPS with TLS 1.2 or later, when "a client interacts with Mycelium Forge programmatically." | - | H |
| SSS-CC-EXT-ID1 | - | Mycelium Fabric and Mycelium Forge shall authenticate users through OIDC 1.0 sessions brokered by a external identity provider. SAML 2.0 and LDAP v3 back-ends are supported transitively through Keycloak's upstream identity federation and are not directly terminated by Mycelium, when "an identity provider is configured for a Mycelium installation." | - | H |
| SSS-CC-EXT-IN1 | - | Mycelium Fabric shall ingest SysML v2 abstract-syntax instances serialised as JSON conforming to OMG formal/25-09-03, when "a client submits a SysML v2 JSON abstract-syntax payload to Mycelium Fabric." | KerML 10.4 | H |
| SSS-CC-EXT-IN3 | - | Mycelium Fabric shall ingest SysML v2 abstract-syntax instances serialised as MessagePack, carrying the same content as the JSON abstract-syntax payload in `SSS-CC-EXT-IN1`, when "a client submits a SysML v2 MessagePack payload to Mycelium Fabric." | - | H |
| SSS-CC-EXT-IN4 | - | Mycelium Fabric shall ingest ReqIF 1.2 for requirements import, preserving attribute types, enumerations, and structural hierarchy, when "a client submits a ReqIF document to Mycelium Fabric." | - | H |
| SSS-CC-EXT-IN5 | - | Mycelium Fabric shall ingest ECSS-E-TM-10-25 Annex C.3 payloads produced for migration into SysML v2 projects, when "a client submits a ECSS-E-TM-10-25 Annex C.3 archive to Mycelium Fabric." | - | H |
| SSS-CC-EXT-EG1 | - | Mycelium Fabric shall emit, upon request, SysML v2 abstract-syntax instances serialised as JSON, XMI, or MessagePack; SysML v2 textual notation and KerML textual notation rendered as a one-way representation of the abstract syntax (not intended for round-trip ingest). | - | H |
| SSS-CC-EXT-BR1 | - | Mycelium Bloom shall operate correctly on the latest two major versions of Google Chrome, Mozilla Firefox, Apple Safari, and Microsoft Edge running on Windows, macOS, and Linux, when "a user accesses Mycelium Bloom through a supported web browser." | - | H |

### 5.4 Adaptation and missionization requirements

Missionization covers the set of adaptations that turn a generic Mycelium installation into one that fits a specific programme, customer, or mission without recompiling or modifying source code. The axes below, deployment model, identity integration, SysML v2 library catalogue, localisation, retention, and notification, must all be configurable through declarative configuration or administrator-facing interfaces. Programme-specific model content (custom Metadata Definitions, custom Viewpoint / View / Rendering Definitions, custom libraries) is not an adaptation axis: it is authored and distributed like any other model content through §5.2.1 and Mycelium Forge (§5.2.3). Project-level adaptation (Regular vs Concurrent Design mode, per project) is covered by `SSS-PA-MGMT-73C` in §5.2.1.2 and is not repeated here.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-CC-ADAPT-G1P | IA | Mycelium shall read its runtime configuration from a declarative source (configuration file, environment variable, or installation-administrator interface) and apply configuration changes when "an Installation Administrator modifies a configuration value." | - | H |
| SSS-CC-ADAPT-A3R | IA | Mycelium Fabric shall integrate with an external identity provider whose authentication backends (JWT, OIDC, LDAP, SAML) are configured at the installation level when "an Installation Administrator configures the identity-provider backend." | - | H |

### 5.5 Computer resource requirements

#### 5.5.1 Computer hardware resource requirements

TBD: Minimum server specifications, browser hardware requirements.

#### 5.5.2 Computer hardware resource utilization requirements

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-CC-PERF-CDT | All | Mycelium Bloom shall load a model with up to 10,000 elements within a responsive timeframe (target TBD) when "a user opens a project containing up to 10,000 model elements." | - | H |
| SSS-CC-PERF-NGA | All | Mycelium Bloom shall reflect model edits in the UI within a responsive timeframe (target TBD) when "a user or a collaborating user commits a model change." | - | H |
| SSS-CC-PERF-EIU | All | Mycelium Bloom shall render diagrams with 100+ elements within a responsive timeframe (target TBD) when "a user opens a diagram view containing 100 or more graphical elements." | - | H |

#### 5.5.3 Computer software resource requirements

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-CC-WEB-1MV | All | The Mycelium platform shall be deployable as a cloud-native containerized service when "a system operator deploys the application using container orchestration tools." | - | H |

### 5.6 Security requirements

The complete role and permission model is defined in [Roles and Permissions](Roles-and-Permissions.md).

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-VW-AC-R7Y | All | Mycelium Bloom shall prevent all create, modify, and delete operations on model elements when "the authenticated user has the Viewer role." | - | H |
| SSS-VW-AC-VKZ | All | Mycelium Bloom shall restrict project and view access to only those projects the Viewer has been granted access to when "a Viewer attempts to open a project." | - | H |
| SSS-IA-SEC-P5G | All | Mycelium Bloom shall present the installation administration interface only to users with the Installation Administrator role when "a user navigates to the application." | - | H |
| SSS-CC-SUP-SBM | - | The Mycelium platform shall publish, for every released Mycelium Fabric and Mycelium Forge container image, a Software Bill of Materials (SBOM) in a standard machine-readable format (SPDX or CycloneDX) that enumerates the bundled software components with their versions and licenses, when "a Mycelium Fabric or Mycelium Forge container image is released." | - | H |

### 5.7 Safety requirements

Not applicable. The Mycelium platform is a web-based engineering tool and does not perform safety-critical functions.

### 5.8 Reliability and availability requirements

Not applicable at this point in time. The Mycelium platform is developed as part of a TRL6 activity.

### 5.9 Quality requirements

Not applicable at this point in time. The Mycelium platform is developed as part of a TRL6 activity.

### 5.10 Design requirements and constraints

Other than the requirements specified in other sections, there are no specific design requirements at this stage.

### 5.11 Software operations requirements

Not applicable at this point in time. The Mycelium platform is developed as part of a TRL6 activity.

### 5.12 Software maintenance requirements

Not applicable at this point in time. The Mycelium platform is developed as part of a TRL6 activity.

### 5.13 System and software observability requirements

The Mycelium platform must be operable in both SaaS and on-premise deployments, which imposes a minimum observability baseline: structured logs with correlation, distributed traces spanning Bloom, Fabric, and Forge, machine-readable metrics, health endpoints for orchestrators, an append-only security audit trail, user-facing correlation identifiers, progress telemetry for long-running operations, and disciplined retention and privacy on everything that is emitted.

| ID | Roles | Requirement | Ref | Prio |
|----|-------|-------------|-----|------|
| SSS-FB-OBS-S1A | - | Mycelium Fabric and Mycelium Forge shall emit every server log line as a structured JSON (TBC) record that includes at minimum an ISO 8601 timestamp, a log level, a trace identifier, a span identifier, the user identifier (when known), the organisation and project identifiers (when applicable), and a correlation identifier propagated from the originating request, when "any server-side component writes a log entry." | - | H |
| SSS-FB-OBS-H4D | - | Mycelium Fabric and Mycelium Forge shall expose HTTP `/healthz` (TBC) and `/ready` (TBC) endpoints returning a success status when the component is alive and ready to serve traffic and an error status with a machine-readable reason otherwise, when "an orchestrator or load balancer probes the component." | - | H |
| SSS-PA-OBS-E6F | PA, PT, VW | Mycelium Bloom shall display, on every user-facing error or failure dialog, the correlation identifier of the failing request and a one-click action to copy it to the clipboard, so that the user can include it in a support request, when "Mycelium Bloom surfaces an error to the user." | - | H |
| SSS-FB-OBS-R8H | - | Mycelium Fabric and Mycelium Forge shall scrub authentication credentials, session tokens, personal data beyond what the audit log requires, and any attribute values annotated as sensitive from all structured logs and traces, and shall enforce a per-deployment retention bound on log and trace storage, when "any component emits telemetry." | - | H |

---

## 6. Verification, validation and system integration

Verification and Validation are described in the SValP.

---

## 7. System models

N.A.