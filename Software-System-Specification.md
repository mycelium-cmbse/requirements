# Mycelium Software System Specification (SSS)

This document defines the software system specification for the Mycelium platform in accordance with ECSS-E-ST-40C Rev.1 Annex B. It contains the Product Owner's requirements for the Mycelium software system. Together with the interface requirements, it provides the criteria used to validate and accept the software.

Each requirement uses the form:

> **\<Component\> shall** \<active verb\> **when** "\<condition\>"

Where component is one of: Mycelium Bloom, Mycelium Fabric, or Mycelium Forge.

Every verb must describe something the software actively does: renders a UI, processes a request, persists data, sends a notification, or blocks an operation.

Each requirement has a unique identifier.

The requirements are organized in tables. The tables list the `Requirement Identifier`, the `roles` it applies to, the requirement body or text and a `reference` to the Kerml or SysML2 specification in case this is applicable. If the kerml or syml2 reference is not applicable a `-` is used.

---

## 1. Introduction

This Software System Specification (SSS) defines the customer's requirements for the Mycelium platform, a next-generation web-based SysML v2 Model-Based Systems Engineering (MBSE) platform evolving from CDP4-COMET. The Mycelium platform is composed of three components:

- **Mycelium Bloom** — the end-user web application providing the interactive frontend for mycelium administration, model browsing, editing, diagramming, and collaboration.
- **Mycelium Fabric** — the backend server combining authentication and authorization, the Systems Modelling API implementation, ownership enforcement, real-time notifications, model persistence, and concurrent design support.
- **Mycelium Forge** — the package registry for publishing, discovering, and importing SysML v2 model libraries and reusable packages.

Requirements are identified using the convention `SSS-<role>-<area>-<alpha-numeric>` where role prefixes are: OA (Organization Administrator), PA (Project Administrator), PT (Participant), VW (Viewer), CC (Cross-Cutting), FB (Fabric), FG (Forge). The `<alpha-numeric>` component is randomized and does not convey any ordering.

This document is the primary input for the System Requirements Review (SRR).

---

## 2. Applicable and reference documents

### 2.1 Applicable documents

| ID | Document |
|----|----------|
| AD-01 | ECSS-E-ST-40C Rev.1 — Space engineering: Software (30 April 2025) |
| AD-02 | OMG SysML v2 — Systems Modeling Language, version 2.0 (formal/25-09-03) |
| AD-03 | OMG KerML — Kernel Modeling Language, version 1.0 (formal/25-09-03) |
| AD-04 | OMG Systems Modelling API and Services, version 1.0 (formal/25-09-04) |

### 2.2 Reference documents

| ID | Document |
|----|----------|
| RD-01 | [Roles and Permissions](Roles-and-Permissions.md) — Mycelium role and permission model |
| RD-02 | ECSS-E-TM-10-25A — Technical Memorandum: Engineering design model data exchange |
| RD-03 | ReqIF — Requirements Interchange Format (OMG formal/16-07-01) |
| RD-04 | BS08823 — CDP4-COMET SysML v2 (R)Evolution Technical Proposal |

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
| PackageDefinition | — | Organizational container grouping related model elements | 5.2.1.3 |
| PartDefinition | PartUsage | Structural building block of a system (system, subsystem, equipment, component) | 5.2.1.4 |
| ItemDefinition | ItemUsage | Non-structural element representing data, signals, energy, or resources | 5.2.1.4 |
| AttributeDefinition | AttributeUsage | Data characteristic (quantity, text, boolean) with optional unit and measurement scale | 5.2.1.4 |
| EnumerationDefinition | EnumerationUsage | Fixed set of allowed values restricting an attribute | 5.2.1.4 |
| PortDefinition | PortUsage | Interaction point on a part with directional features (in, out, inout) | 5.2.1.4 |
| ConnectionDefinition | ConnectionUsage | Link between parts or items (physical, logical, or data) | 5.2.1.4 |
| InterfaceDefinition | InterfaceUsage | Standardized connection between ports with compatibility rules | 5.2.1.4 |
| ActionDefinition | ActionUsage | Function or behavior with input/output parameters, decomposable into sub-actions | 5.2.1.7 |
| StateDefinition | StateUsage | Condition or mode with entry, do, and exit actions | 5.2.1.7 |
| TransitionUsage | — | Transition between states with trigger, guard, and effect | 5.2.1.7 |
| FlowConnectionDefinition | FlowConnectionUsage | Transfer of items, energy, or data between parts | 5.2.1.7 |
| UseCaseDefinition | UseCaseUsage | System behavior from an external actor perspective | 5.2.1.7 |
| RequirementDefinition | RequirementUsage | Stakeholder-imposed condition with textual statement and constraint features | 5.2.1.5 |
| ConcernDefinition | ConcernUsage | Stakeholder concern linked to requirements and viewpoints | 5.2.1.5 |
| ConstraintDefinition | ConstraintUsage | Boolean expression assertable against model elements for validation | 5.2.1.8 |
| AnalysisCaseDefinition | AnalysisCaseUsage | Evaluation of system properties with subject and objectives | 5.2.1.8 |
| VerificationCaseDefinition | VerificationCaseUsage | Verification activity with method (test, analysis, inspection, demonstration) and verdict | 5.2.1.8 |
| CalculationDefinition | CalculationUsage | Domain-specific computation over model attributes | 5.2.1.8 |
| AllocationDefinition | AllocationUsage | Mapping from functional to physical elements | 5.2.1.6 |
| ViewDefinition | ViewUsage | Presentation of model content for a specific purpose | 5.2.1.9 |
| ViewpointDefinition | ViewpointUsage | Stakeholder concerns specifying what a view addresses | 5.2.1.9 |
| MetadataDefinition | MetadataUsage | Tool-specific or process-specific annotation on model elements | 5.3 |

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

Mycelium is the successor to CDP4-COMET, a collaborative MBSE tool based on ECSS-E-TM-10-25 that has been used for over 10 years in concurrent design. Mycelium replaces CDP4-COMET's desktop-first, ECSS-E-TM-10-25-based approach with a cloud-native, web-first architecture natively implementing SysML v2.

The platform consists of three components:

- **Mycelium Bloom** communicates with Mycelium Fabric via REST/HTTP and SignalR (WebSocket). It renders the user interface, handles user interaction, and presents model data received from the backend.
- **Mycelium Fabric** implements the OMG Systems Modelling API, manages model persistence, enforces ownership-based access control, handles authentication/authorization, and propagates real-time notifications.
- **Mycelium Forge** provides a package registry for SysML v2 model libraries, enabling publishing, discovering, versioning and importing reusable model packages.

### 4.2 General capabilities

The Mycelium platform provides the following high-level capabilities:

- SysML v2 model creation, browsing, editing and visualization.
- Concurrent design session support for 20-30 simultaneous participants.
- Lock-free collaborative modeling with ownership-based access control.
- Version control with branching, merging, tagging and commit history.
- Requirements management with traceability to design elements.
- Near real-time model change notification to all connected users.
- Diagram editing (Interconnection, Action Flow, State Transition, Sequence, General, Grid Views).
- Model import/export in SysML v2 JSON format and ECSS-E-TM-10-25 migration.
- Self-service organization and project creation.
- SysML v2 library package management via Mycelium Forge based on kerml kpar.

### 4.3 General constraints

- The platform shall natively implement the SysML v2 metamodel (OMG formal/25-09-03) as its data model.
- The model server shall conform to the OMG Systems Modelling API and Services specification (formal/25-09-04) using the REST/HTTP PSM.
- The platform shall support the Kernel Modelling Language (KerML) as the underlying formalism for SysML v2.
- The web application shall not provide SysML v2 textual notation editing or parsing capabilities; textual notation is generated read-only.

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

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-OA-USR-N35 | OA | Mycelium Bloom shall provide operations to create, update, deactivate, and delete user accounts when "an authenticated user with the Organization Administrator role accesses the user management interface." | - |
| SSS-OA-USR-T18 | OA | Mycelium Bloom shall display a list of all registered users within the organization and their current status when "the Organization Administrator navigates to the user management view." | - |
| SSS-OA-ROLE-TBX | OA | Mycelium Bloom shall provide operations to assign and revoke Organization Administrator and Organization Member roles when "the Organization Administrator selects a user and modifies their organization-level role." | - |
| SSS-OA-ROLE-PQH | OA | Mycelium Bloom shall provide a setting to control whether Organization Members can create projects within that Organization when "the Organization Administrator accesses the member permission settings." | - |
| SSS-CC-SS-HV9 | All | Mycelium Bloom shall create a new Organization and assign the requesting user as its Organization Administrator when "an authenticated user initiates organization creation from their account dashboard." | - |
| SSS-CC-SS-FUU | OA, OM | Mycelium Bloom shall create a new Project and assign the requesting user as its Project Administrator when "an Organization Member initiates project creation and the organization permits member project creation." | - |
| SSS-CC-SS-G6B | PA | Mycelium Bloom shall grant Outside Collaborators access to specific projects without organization membership when "a Project Administrator grants access to an external user with an assigned project-level role." | - |
| SSS-CC-SS-LEZ | All | Mycelium Bloom shall enforce project visibility rules (Private, Organization-visible, Public) when "a user attempts to access a project." | - |
| SSS-IA-ORG-V4R | IA | Mycelium Bloom shall display a list of all organizations on the installation with their name, creation date, member count, project count, and status (active/suspended) when "an Installation Administrator navigates to the installation administration view." | - |
| SSS-IA-ORG-K8W | IA | Mycelium Bloom shall provide operations to create, update, suspend, reactivate, and delete organizations when "an Installation Administrator accesses the organization management interface." | - |
| SSS-IA-ORG-M3J | IA | Mycelium Bloom shall display the details of an organization including its members, projects, roles, authentication configuration, and audit log when "an Installation Administrator selects an organization from the installation administration view." | - |
| SSS-IA-USR-B6P | IA | Mycelium Bloom shall display a list of all user accounts across all organizations with their username, email, organization memberships, roles, and status (active/deactivated) when "an Installation Administrator navigates to the installation user management view." | - |
| SSS-IA-USR-Q2N | IA | Mycelium Bloom shall provide operations to create, update, deactivate, and delete user accounts across all organizations when "an Installation Administrator accesses the installation user management interface." | - |
| SSS-IA-USR-H7F | IA | Mycelium Bloom shall provide operations to assign and remove users to and from any organization with a specified role when "an Installation Administrator selects a user and modifies their organization memberships." | - |
| SSS-IA-SYS-W9D | IA | Mycelium Bloom shall display installation-wide metrics including total users, total organizations, total projects, storage usage, and active sessions when "an Installation Administrator navigates to the installation dashboard." | - |
| SSS-IA-SYS-E3T | IA | Mycelium Bloom shall display an installation-wide audit log of administrative actions (organization creation/deletion, user creation/deactivation, role changes) when "an Installation Administrator navigates to the installation audit log view." | - |

##### 5.2.1.2 Project management

A project is the unit of collaboration in Mycelium. Each project owns a SysML v2 model, a team, branches, and Ownership assignments. The Project Administrator (typically the study lead) configures the project, assigns roles, defines Ownerships, and oversees the model's structural integrity. The requirements in this section cover project creation, configuration, team management, and Ownership administration. Owner administration is only relevant in case the project is a Concurrent Design project.


| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-OA-PROJ-TWK | OA | Mycelium Bloom shall create a Project with metadata (name, description) and a default Branch, consistent with the Systems Modelling API Project concept, when "the Organization Administrator submits a valid project creation form." | TBC |
| SSS-OA-PROJ-PFY | OA | Mycelium Bloom shall delete a project within the organization, applying configurable deletion policies regarding project usages by other projects, when "the Organization Administrator initiates project deletion and confirms the action." | - |
| SSS-OA-PROJ-PZ9 | OA | Mycelium Bloom shall provide a policy setting that, when enabled, grants the Organization Administrator read-only access to all projects in the organization for audit purposes when "the Organization Administrator enables the organization-wide audit access policy." | - |
| SSS-PA-MGMT-B3R | PA | Mycelium Bloom shall provide an interface to update project properties including name, description, default branch and visibility when "the Project Administrator edits an existing project's settings." | - |
| SSS-PA-MGMT-8EF | PA | Mycelium Bloom shall provide operations to add and remove users (including Outside Collaborators) with assigned roles and Ownerships when "the Project Administrator accesses the team management interface of a project." | - |
| SSS-PA-MGMT-KYM | PA | Mycelium Bloom shall transfer the Project Administrator role to another team member when "the current Project Administrator selects a team member and confirms the transfer." | - |
| SSS-PA-MGMT-73C | PA | Mycelium Bloom shall provide a setting to configure the project mode (Regular or Concurrent Design) when "the Project Administrator accesses the project mode settings." | - |
| SSS-PA-MGMT-YC1 | PA | Mycelium Bloom shall provide operations to create, rename and remove Ownership Usages within the project package when "the Project Administrator accesses the Ownership management interface." | - |
| SSS-PA-MGMT-BA7 | PA | Mycelium Bloom shall reassign element ownership by updating the Owner metadata on a model element to a different Ownership when "the Project Administrator selects a model element and changes its Owner annotation." | - |

###### 5.2.1.2a Project lifecycle state

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-PA-STATE-V4R | PA, PT, VW | Mycelium Bloom shall display the current lifecycle state of a project in the project header and project list when "a user views a project." | - |
| SSS-PA-STATE-K7N | PA | Mycelium Bloom shall provide operations to transition a project between lifecycle states when "the Project Administrator changes the project's lifecycle state." | - |
| SSS-PA-STATE-W2D | All | Mycelium Bloom shall enforce the following project lifecycle states and their editing constraints: | - |

The following lifecycle states are defined (TBC):

| State | Description | Editing |
|-------|-------------|---------|
| **Preparation** | Project setup: structure, team, ownerships, reference data. Core team configures the baseline model. | Open to Project Administrator only |
| **Open** | Active modeling: all team members contribute within their Ownerships. Design sessions take place. | Open to all Participants per Ownership |
| **Review** | Model under review: no modifications permitted. Stakeholders and Viewers inspect the model and provide feedback. | Read-only for all roles |
| **Archived** | Study completed: model preserved as an immutable historical record. Can be reopened or used as a template for new projects. | Read-only for all roles |

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-PA-STATE-H5J | All | Mycelium Fabric shall reject all create, modify, and delete operations on model elements when "the project is in the Review or Archived state." | - |
| SSS-PA-STATE-M8T | PA | Mycelium Bloom shall transition a project from Archived back to Open when "the Project Administrator reopens an archived project." | - |
| SSS-PA-STATE-F3B | PA, OA | Mycelium Bloom shall create a new project pre-populated with the content of an Archived project when "a user creates a new project using an archived project as a template." | - |
| SSS-PA-STATE-R6G | PA, PT, VW | Mycelium Bloom shall display a visual indicator (e.g. banner, badge, or icon) distinguishing projects in Review or Archived state from active projects when "a user views a project in a non-editable state." | - |
| SSS-PA-STATE-Q8L | All | Mycelium Bloom shall assign Preparation as the default lifecycle state to a newly created project when "a user creates a new project and the organization has not configured a different default state." | - |
| SSS-OA-STATE-Z3W | OA | Mycelium Bloom shall provide a setting to configure the default project lifecycle state for newly created projects within the organization when "the Organization Administrator accesses the organization's project defaults settings." | - |

##### 5.2.1.3 Model navigation and browsing

Engineers spend most of their time finding, selecting, and understanding model elements. Mycelium offers complementary navigation views: a hierarchical tree for structural exploration and a tabular browser for flat searching with namespace path columns. The requirements in this section ensure that users can find any element quickly, see its qualified context, and follow relationships to related elements without losing their place.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-PA-NAV-8IB | PA, PT, VW | Mycelium Bloom shall display the model as a hierarchical tree (Browser View) with collapsible and expandable nodes representing packages, parts, and nested elements when "a user opens a project and navigates to the Browser View." | - |
| SSS-PA-NAV-ZLS | PA, PT, VW | Mycelium Bloom shall return filtered model elements matching the specified criteria when "a user enters search terms or applies filters by name, type, category, owner, or Ownership." | - |
| SSS-PA-NAV-ZRW | PA, PT, VW | Mycelium Bloom shall display element properties including attributes, relationships, and ownership in a detail panel when "a user selects a model element." | - |
| SSS-PA-NAV-KVE | PA, PT, VW | Mycelium Bloom shall navigate to the related element when "a user activates a relationship link on a model element (e.g. from a requirement to the part that satisfies it)." | - |
| SSS-PA-NAV-F3K | PA, PT, VW | Mycelium Bloom shall display the qualified name (namespace path) of each model element when "a user views an element's properties in the detail panel." | - |
| SSS-PA-NAV-G5X | PA, PT, VW | Mycelium Bloom shall provide a tabular element browser that lists Definitions and Usages for each kind of Definition and Usage in a sortable, filterable table showing element name, namespace path, type, Ownership, and key attributes when "a user opens the tabular element browser." | - |
| SSS-PA-NAV-W4B | PA, PT, VW | Mycelium Bloom shall support the hierarchical Browser View and the tabular element browser as independent views that can be open simultaneously when "a user has both views open." | - |

##### 5.2.1.3a Namespace and package management

SysML v2 organizes models into Packages and Namespaces. Packages group related elements; Namespaces control naming and visibility; Imports allow reuse without duplication. The requirements in this section ensure users can structure their models hierarchically, share content between packages, and apply visibility rules without leaving the model browser.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-PA-PKG-R8W | PA, PT | Mycelium Bloom shall support creating, renaming, moving, and deleting Packages to organize model elements into logical groups when "a user manages packages in the model browser." | SysML 7.5 |
| SSS-PA-PKG-V2J | PA, PT | Mycelium Bloom shall support nesting Packages within other Packages to create hierarchical model organization when "a user creates a child package within an existing package." | SysML 7.5 |
| SSS-PA-PKG-D4N | PA, PT | Mycelium Bloom shall support importing elements from one Namespace into another using Membership Imports and Namespace Imports when "a user creates an import relationship between namespaces." | SysML 7.5 |
| SSS-PA-PKG-J3W | PA, PT | Mycelium Bloom shall create a Filtered Import that imports only elements matching a metadata-based condition (e.g. import only elements annotated with a specific Metadata Usage) when "a user creates a namespace import and specifies a metadata filter expression." | SysML 7.5.4 |
| SSS-PA-PKG-H6T | PA, PT | Mycelium Bloom shall support setting member visibility (public, private) on elements within a Namespace when "a user configures the visibility of a model element within its owning namespace." | KerML 7.2.5 |
| SSS-PA-PKG-Q1M | PA, PT | Mycelium Bloom shall support creating Alias memberships to provide alternative names for elements within a namespace when "a user assigns an alias to an imported or local element." | KerML 7.5.2 |

##### 5.2.1.4 System architecture modeling

The core of system modeling is defining the building blocks (Definitions) of the system and instantiating them in a hierarchy (Usages). Engineers compose parts, items, ports, connections, and interfaces into a decomposed system architecture. The requirements in this section cover the SysML v2 structural concepts that engineers use to capture the what and how of a system, plus the everyday operations to duplicate, move, delete, and refine these elements.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-PA-ARCH-JQH | PA, PT | Mycelium Bloom shall support defining Part Definitions as reusable building blocks and instantiating them as Part Usages within the system hierarchy when "a user creates a new Part Definition or instantiates an existing one." | SysML 7.9 |
| SSS-PA-ARCH-TB2 | PA, PT | Mycelium Bloom shall support decomposing the system into hierarchical levels (System, Subsystem, Equipment, Component) when "a user adds child parts to an existing part in the model hierarchy." | SysML 7.9 |
| SSS-PA-ARCH-5RR | PA, PT | Mycelium Bloom shall support defining Port Definitions and instantiating them as Port Usages on parts to specify interaction points with direction (in, out, inout) when "a user adds a port to a selected part." | SysML 7.11 |
| SSS-PA-ARCH-IGA | PA, PT | Mycelium Bloom shall support creating Connection Definitions, Connection Usages, Interface Definitions, and Interface Usages between parts to model integration and data flow when "a user selects two compatible ports and creates a connection or interface." | SysML 7.12, 7.13 |
| SSS-PA-ARCH-Y2D | PA, PT | Mycelium Bloom shall create a Binding Connector that asserts equality between two compatible features of model elements when "a user selects two features and creates a binding between them." | KerML 7.13.3 |
| SSS-PA-ARCH-K7M | PA, PT | Mycelium Bloom shall create a conjugated Port Usage with reversed feature directions (in becomes out, out becomes in) when "a user designates a Port Usage as the conjugate of an existing Port Definition." | KerML 7.6 |
| SSS-PA-ARCH-N5W | PA, PT | Mycelium Bloom shall create a Featuring relationship establishing that one type features another type when "a user explicitly specifies a featuring relationship between two types." | KerML 7.6 |
| SSS-PA-ARCH-97Z | PA, PT | Mycelium Bloom shall support defining Attribute Definitions and instantiating them as Attribute Usages on any element definition or usage, with quantities, units, and measurement scales consistent with the SysML v2 Quantities and Units library, when "a user adds or edits an attribute on a model element." | SysML 7.7 |
| SSS-PA-ARCH-9W5 | PA, PT | Mycelium Bloom shall support defining Enumeration Definitions to restrict attribute values to specified sets when "a user creates an Enumeration Definition and specifies its literal values." | SysML 7.7.4 |
| SSS-PA-ARCH-B2D | PA, PT | Mycelium Bloom shall support defining Item Definitions to represent non-structural elements (data types, signals, resources) when "a user creates a new Item Definition." | SysML 7.10 |
| SSS-PT-DATA-N7O | PT | Mycelium Bloom shall display and enable editing of model elements within the user's assigned Ownership when "the Participant navigates to a model element annotated with their Ownership as Owner." | - |
| SSS-PT-DATA-D5I | PT | Mycelium Bloom shall provide a selector to set attribute value sources as Manual (hand-entered), Computed (calculated), or Reference (sourced from another element) when "the Participant edits an attribute value." (TBC) | SysML 7.7 |
| SSS-PT-DATA-I9M | PA, PT | Mycelium Bloom shall support defining new attributes on element definitions when "a user adds an attribute to an element definition within their assigned Ownership." | SysML 7.7 |
| SSS-PT-DATA-OH2 | PA, PT | Mycelium Bloom shall support overriding attribute values on specific element usages without changing the parent definition when "a user edits an attribute value on a usage that inherits from a definition." | KerML 7.6 |
| SSS-PT-DATA-492 | PA, PT | Mycelium Bloom shall support defining attribute values that vary by exhibited State Usage (e.g. operational mode) when "a user associates attribute values with specific states on an element." | SysML 7.17 |
| SSS-PT-DATA-XHY | PA, PT | Mycelium Bloom shall support creating and modifying elements (parts, items, attributes, etc.) throughout a Project "a user creates or modifies elements annotated with their Ownership." | - |
| SSS-PT-DATA-M6H | PT | Mycelium Bloom shall automatically annotate newly created model elements with the Participant's active Ownership as Owner when "the Participant creates a new model element." | - |
| SSS-PA-ELEM-K4T | PA, PT | Mycelium Bloom shall present a duplicate dialog offering the user three independent options — preserve original Ownership (yes/no), copy attribute values (yes/no), copy nested children recursively (yes/no) — when "a user initiates the duplication of a model element." | - |
| SSS-PA-ELEM-R8V | PA, PT | Mycelium Bloom shall duplicate a Definition or Usage with the user's active Ownership assigned as Owner on the copy when "a user duplicates a model element with the preserve-ownership option set to no." | - |
| SSS-PA-ELEM-T2N | PA, PT | Mycelium Bloom shall duplicate a Definition or Usage with the original Ownership assignments preserved on the copy when "a user duplicates a model element with the preserve-ownership option set to yes." | - |
| SSS-PA-ELEM-P5K | PA, PT | Mycelium Bloom shall duplicate a Definition or Usage and copy all attribute values from the source to the copy when "a user duplicates a model element with the copy-attribute-values option set to yes." | - |
| SSS-PA-ELEM-H8W | PA, PT | Mycelium Bloom shall duplicate a Definition or Usage and reset all attribute values on the copy to unset when "a user duplicates a model element with the copy-attribute-values option set to no." | - |
| SSS-PA-ELEM-D7M | PA, PT | Mycelium Bloom shall duplicate a Definition or Usage and recursively copy all nested children, applying the same Ownership and attribute-value rules to each nested copy, when "a user duplicates a model element with the copy-nested-children option set to yes." | - |
| SSS-PA-ELEM-W4F | PA, PT | Mycelium Bloom shall duplicate only the selected Definition or Usage without copying any of its nested children when "a user duplicates a model element with the copy-nested-children option set to no." | - |
| SSS-PA-ELEM-B6J | PA, PT | Mycelium Bloom shall provide a setting to remember the user's last-used duplicate options as defaults for the next duplication when "a user accesses the duplication preferences." | - |
| SSS-PA-ELEM-W3N | PA, PT | Mycelium Bloom shall move a Usage from its current parent element to a different parent element, preserving all attributes, attribute values, and Ownership assignments, when "a user drags a Usage and drops it onto a different parent element in the model browser or a diagram." | - |
| SSS-PA-ELEM-J6D | PA, PT | Mycelium Bloom shall delete a Definition or Usage and all its owned nested children when "a user deletes a model element and confirms the deletion.". Nested children that are owned by other Owners than the current Owner are deleted as well.  | - |
| SSS-PA-ELEM-V7K | PA, PT | Mycelium Bloom shall set the multiplicity (lower bound, upper bound) on any Usage when "a user edits the multiplicity of a Usage in the detail panel or on a diagram." | KerML 7.6.6 |
| SSS-PA-ELEM-D2N | PA, PT | Mycelium Bloom shall create a subsetting relationship between a feature and another feature of a compatible type when "a user designates a feature as a subset of another feature." | KerML 7.6.5 |
| SSS-PA-ELEM-H9W | PA, PT | Mycelium Bloom shall create a redefinition relationship where a feature in a specializing type replaces a feature inherited from a general type when "a user designates a feature as a redefinition of an inherited feature." | KerML 7.6.5 |
| SSS-PA-ELEM-M4J | PA, PT | Mycelium Bloom shall create a Specialization relationship between two Definitions, where the specializing Definition inherits all features of the general Definition and can add or redefine features, when "a user designates one Definition as a specialization of another." | KerML 7.6 |
| SSS-PA-ELEM-R6F | PA, PT, VW | Mycelium Bloom shall display the generalization/specialization hierarchy of a selected Definition, showing its general types and all its specializations, when "a user views the type hierarchy of a Definition." | KerML 7.6 |

##### 5.2.1.4a Quantities, units, and measurement management

Numerical engineering values must always be expressed with a quantity kind, a measurement unit, and a measurement scale. The SysML v2 Quantities and Units Domain Library provides a normative model of these concepts as Attribute Definitions and Attribute Usages. Mycelium presents this library as user-friendly browsers for quantities, units, and scales, with drag-and-drop assignment of attributes to elements and the ability to import standard or custom libraries.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-PA-QU-T3K | PA, PT, VW | Mycelium Bloom shall provide a tabular view listing all Attribute Definitions available in the project (including those imported from libraries) with their name, quantity kind, default unit, and source library when "a user opens the Attribute Definitions browser." | SysML 9.8 |
| SSS-PA-QU-R7N | PA, PT, VW | Mycelium Bloom shall provide a tabular view listing all Measurement Units available in the project with their name, symbol, unit type (simple, derived, prefixed), and associated quantity kind when "a user opens the Measurement Units browser." | SysML 9.8.3 |
| SSS-PA-QU-W5J | PA, PT, VW | Mycelium Bloom shall provide a tabular view listing all Measurement Scales available in the project with their name, scale type (ratio, interval, ordinal, cyclic ratio, logarithmic), unit, and value range when "a user opens the Measurement Scales browser." | SysML 9.8.3 |
| SSS-PA-QU-D8M | PA, PT, VW | Mycelium Bloom shall provide a tabular view listing all Quantity Kinds available in the project with their name, dimension symbol, and classification (base, derived, specialized) when "a user opens the Quantity Kinds browser." | SysML 9.8.2 |
| SSS-PA-QU-H2V | PA, PT | Mycelium Bloom shall support creating, editing, and deleting custom Attribute Definitions typed by a Quantity Kind with an associated Measurement Unit when "a user manages Attribute Definitions in the project or a library." | SysML 9.8 |
| SSS-PA-QU-K6F | PA, PT | Mycelium Bloom shall support creating, editing, and deleting custom Measurement Units (simple, derived, prefixed) with conversion factors when "a user manages Measurement Units in the project or a library." | SysML 9.8.3 |
| SSS-PA-QU-B4P | PA, PT | Mycelium Bloom shall support creating, editing, and deleting Measurement Scales (ratio, interval, ordinal, cyclic ratio, logarithmic) with their associated unit and value constraints when "a user manages Measurement Scales in the project or a library." | SysML 9.8.3 |
| SSS-PA-QU-N9X | PA, PT | Mycelium Bloom shall create an Attribute Usage typed by the dropped Attribute Definition on the target element when "a user drags an Attribute Definition from the Attribute Definitions browser and drops it onto an element Definition or Usage in the model browser or a diagram." | SysML 7.7 |
| SSS-PA-QU-G1W | PA, PT | Mycelium Bloom shall support importing Quantity Kinds, Measurement Units, Measurement Scales, and Attribute Definitions from the SysML v2 standard libraries (ISQ, SI, USCustomary) and from Mycelium Forge packages when "a user selects library content for import into a project." | SysML 9.8 |

##### 5.2.1.4b Attachments

Engineering elements often need supporting documentation: thermal analysis PDFs, interface drawings, datasheets, photographs, spreadsheets. Mycelium lets users attach files of any type to any model element and download them later. The requirements in this section cover upload, download, listing, removal, and inline preview for common formats.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-PA-ATT-W5R | PA, PT | Mycelium Bloom shall support uploading one or more file attachments to any model element when "a user selects a model element and adds attachments via the attachment interface." | - |
| SSS-PA-ATT-K3J | PA, PT, VW | Mycelium Bloom shall display a list of all attachments associated with a model element, showing file name, file type, size, upload date, and uploading user, when "a user views the attachments of a model element." | - |
| SSS-PA-ATT-M8D | PA, PT, VW | Mycelium Bloom shall support downloading an attachment when "a user selects an attachment from the attachment list of a model element." | - |
| SSS-PA-ATT-F2N | PA, PT | Mycelium Bloom shall support removing an attachment from a model element when "a user with write access to the element deletes an attachment from the attachment list." | - |
| SSS-PA-ATT-V6H | PA, PT, VW | Mycelium Bloom shall display inline previews for image attachments (PNG, JPG, SVG) and PDF attachments when "a user views the attachment list of a model element." | - |

##### 5.2.1.4c Glossary

Engineering teams need a shared vocabulary. Acronyms, domain terms, and project-specific definitions should be discoverable wherever they appear. Mycelium models a glossary as a Package of Item Definitions with Documentation, and the user interface highlights terms throughout the application with tooltips and click-through navigation. This makes the glossary live and contextual rather than a forgotten document.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-PA-GLOSS-T5R | PA, PT | Mycelium Bloom shall provide operations to create and manage a Glossary Package containing Item Definitions where each Item Definition represents a glossary term (name = term, short name = abbreviation) with Documentation providing the definition when "a user accesses the glossary management interface." | KerML 7.4 |
| SSS-PA-GLOSS-K2W | PA, PT, VW | Mycelium Bloom shall display a tabular view listing all glossary terms with their name, abbreviation, definition, and owning package when "a user opens the glossary browser." | KerML 7.4 |
| SSS-PA-GLOSS-H8N | PA, PT | Mycelium Bloom shall provide operations to create, edit, and delete glossary terms (Item Definitions with Documentation) within a Glossary Package when "a user manages glossary entries." | KerML 7.4 |
| SSS-PA-GLOSS-M3J | PA, PT, VW | Mycelium Bloom shall render any occurrence of a glossary term name or abbreviation as highlighted linked text throughout the application (model browser, detail panels, requirement text, diagram labels, comments) when "text content contains a word matching a glossary term name or abbreviation." | KerML 7.4 |
| SSS-PA-GLOSS-V9D | PA, PT, VW | Mycelium Bloom shall display a tooltip showing the glossary term definition when "a user hovers over a highlighted glossary term in any view." | KerML 7.4 |
| SSS-PA-GLOSS-F6B | PA, PT, VW | Mycelium Bloom shall navigate to the glossary term's Item Definition in the glossary browser when "a user clicks a highlighted glossary term link." | KerML 7.4 |

##### 5.2.1.4d Constants

Engineering models reference physical and project-specific constants (the speed of light, gravitational acceleration, target margins). Modelling these as named, typed Attribute Definitions with fixed values and source references makes them reusable across the project and traceable to their origin. Users can drag a constant into any constraint or calculation to ensure consistent values.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-PA-CONST-N7K | PA, PT | Mycelium Bloom shall provide operations to create and manage a Constants Package containing Attribute Definitions where each Attribute Definition represents a named constant typed by a Quantity Kind, with a fixed Attribute Usage holding the value and Measurement Unit, and Documentation providing the source or reference, when "a user accesses the constants management interface." | SysML 9.8 |
| SSS-PA-CONST-D3V | PA, PT, VW | Mycelium Bloom shall display a tabular view listing all constants with their name, abbreviation, value, unit, and source reference when "a user opens the constants browser." | SysML 9.8 |
| SSS-PA-CONST-W8F | PA, PT | Mycelium Bloom shall provide operations to create, edit, and delete constants (Attribute Definitions with fixed Attribute Usages) within a Constants Package when "a user manages constant entries." | SysML 9.8 |
| SSS-PA-CONST-J5M | PA, PT | Mycelium Bloom shall insert a reference to a constant's value into a constraint expression or calculation when "a user drags a constant from the constants browser and drops it into a constraint or calculation editor." | SysML 9.8 |
| SSS-PA-CONST-R2H | PA, PT, VW | Mycelium Bloom shall display a tooltip showing the constant's value, unit, and source reference when "a user hovers over a constant reference in a constraint expression, calculation, or attribute value." | SysML 9.8 |

##### 5.2.1.4e Metadata management

SysML v2 Metadata Definitions and Metadata Usages provide a general-purpose mechanism to annotate model elements with structured, user-defined information without altering the element's semantics. Mycelium uses metadata internally for Ownership, Publication, and ParameterSubscription (modeled in the Concurrent Design library). This section specifies user-facing capabilities for defining and applying custom metadata for purposes such as categorization, status tracking, maturity assessment, risk tagging, review annotations, and domain-specific process information.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-PA-META-K7R | PA, PT | Mycelium Bloom shall provide operations to create, edit, and delete Metadata Definitions with typed attributes (text, boolean, enumeration, quantity, reference to other elements) when "a user defines a new metadata type in the project or a library." | SysML 7.27 |
| SSS-PA-META-W3D | PA, PT | Mycelium Bloom shall apply a Metadata Usage to any model element by instantiating a Metadata Definition and specifying its attribute values when "a user annotates a model element with metadata." | SysML 7.27 |
| SSS-PA-META-N8F | PA, PT | Mycelium Bloom shall apply the same Metadata Usage to multiple model elements in a single operation when "a user selects multiple elements and applies a metadata annotation." | SysML 7.27 |
| SSS-PA-META-H2T | PA, PT | Mycelium Bloom shall remove a Metadata Usage from a model element when "a user removes a metadata annotation from an element." | SysML 7.27 |
| SSS-PA-META-D5J | PA, PT, VW | Mycelium Bloom shall display all Metadata Usages applied to a model element in the detail panel, showing the Metadata Definition name and its attribute values, when "a user views the properties of a model element." | SysML 7.27 |
| SSS-PA-META-R9V | PA, PT, VW | Mycelium Bloom shall display a tabular view listing all Metadata Definitions available in the project with their name, attributes, and source (project or library) when "a user opens the metadata definitions browser." | SysML 7.27 |
| SSS-PA-META-T4K | PA, PT, VW | Mycelium Bloom shall filter model elements in the model browser, tabular browsers, and Relationship Matrix by Metadata Usage presence and attribute values when "a user applies a metadata-based filter (e.g. show all elements tagged with category 'Equipment', or all elements with maturity status 'Preliminary')." | SysML 7.27 |
| SSS-PA-META-M6W | PA, PT, VW | Mycelium Bloom shall display Metadata Usages as visual badges or color-coded indicators on elements in the model browser and on diagram nodes when "a user enables metadata visualization." | SysML 7.27 |
| SSS-PA-META-J1B | PA, PT | Mycelium Bloom shall import Metadata Definitions from external libraries or Mycelium Forge packages when "a user selects metadata definitions for import into a project." | SysML 7.27 |
| SSS-PA-META-V8G | PA, PT, VW | Mycelium Bloom shall group and sort model elements by their Metadata Usage values in tabular browsers and the model browser when "a user selects a metadata attribute as a grouping or sorting criterion." | SysML 7.27 |

##### 5.2.1.4f Comments and documentation

SysML v2 defines Comment as an annotating element with a textual body that can describe one or more model elements, and Documentation as a specialized Comment that formally documents its owning element. Comments and Documentation are the primary mechanism for adding explanatory text, rationale, design notes, and review feedback to model elements.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-PA-CMT-R4K | PA, PT | Mycelium Bloom shall create a Comment with a textual body on one or more model elements when "a user adds a comment to a model element via the detail panel or context menu." | KerML 7.4.2 |
| SSS-PA-CMT-W7N | PA, PT | Mycelium Bloom shall create a Documentation element on a model element, representing its formal description, when "a user adds or edits the documentation of a model element." | KerML 7.4.3 |
| SSS-PA-CMT-H3D | PA, PT | Mycelium Bloom shall edit and delete Comments and Documentation owned by the user's Ownership when "a user modifies or removes a comment or documentation entry." | KerML 7.4 |
| SSS-PA-CMT-M6J | PA, PT, VW | Mycelium Bloom shall display all Comments and Documentation associated with a model element in the detail panel, showing the text body, author, and creation date, when "a user views a model element's properties." | KerML 7.4 |
| SSS-PA-CMT-T9F | PA, PT, VW | Mycelium Bloom shall indicate in the model browser that an element has Comments or Documentation attached using a visual indicator (e.g. icon or badge) when "an element has one or more Comments or Documentation entries." | KerML 7.4 |
| SSS-PA-CMT-K2B | PA, PT | Mycelium Bloom shall format Comment and Documentation text using rich text (bold, italic, lists, links) when "a user edits the body of a Comment or Documentation element." | KerML 7.4 |
| SSS-PA-CMT-D5P | PA, PT | Mycelium Bloom shall annotate a single Comment on multiple model elements simultaneously when "a user creates a comment and selects multiple annotated elements." | KerML 7.4.2 |
| SSS-PA-CMT-N8V | PA, PT | Mycelium Bloom shall specify an optional locale (e.g. "en", "fr", "de") on a Comment or Documentation element when "a user sets the language of a comment or documentation entry." | KerML 7.4.2 |
| SSS-PA-CMT-Y6L | PA, PT | Mycelium Bloom shall create a Textual Representation on a model element, embedding language-specific text (e.g. a code snippet, formula, or DSL expression) tagged with the language identifier, when "a user adds a textual representation to an element and selects the language." | KerML 7.4.4 |

##### 5.2.1.5 Requirements management

Requirements capture stakeholder-imposed conditions a design must satisfy. SysML v2 models requirements as Constraint Definitions with subjects, actors, stakeholders, assumed and required constraints, and concerns. Requirements can be nested, derived, satisfied by design elements, and verified by Verification Cases. The requirements in this section cover the full SysML v2 requirements metamodel as user-facing operations.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-PA-REQ-QP0 | PA, PT | Mycelium Bloom shall provide operations to create, edit, and organize Requirement Definitions and Requirement Usages in hierarchical specifications with textual statements when "a user accesses the requirements management interface and creates or modifies a requirement." | SysML 7.21 |
| SSS-PA-REQ-WD0 | PA, PT | Mycelium Bloom shall nest a Requirement Usage within a parent Requirement Definition or Requirement Usage, where nested requirements automatically become required constraints of the parent, when "a user adds a child requirement to an existing requirement." | SysML 7.21, 8.3.21 |
| SSS-PA-REQ-DS6 | PA, PT | Mycelium Bloom shall provide editors for assumed constraints and required constraints on requirements, where the effective requirement logic is "if all assumed constraints hold then all required constraints must be satisfied", when "a user edits a requirement and adds constraint expressions." | SysML 8.3.21.7 |
| SSS-PA-REQ-T8K | PA, PT | Mycelium Bloom shall assign a subject to a Requirement Definition or Requirement Usage via Subject Membership, binding the requirement to the system or element it applies to, when "a user specifies the subject of a requirement." | SysML 8.3.21.11 |
| SSS-PA-REQ-M3N | PA, PT | Mycelium Bloom shall assign one or more actors to a Requirement Definition or Requirement Usage via Actor Membership, representing external entities necessary for the requirement to be fulfilled, when "a user adds actors to a requirement." | SysML 8.3.21.2 |
| SSS-PA-REQ-H6W | PA, PT | Mycelium Bloom shall assign one or more stakeholders to a Requirement Definition or Requirement Usage via Stakeholder Membership, representing entities with concerns about the requirement, when "a user adds stakeholders to a requirement." | SysML 8.3.21.12 |
| SSS-PA-REQ-SUC | PA, PT | Mycelium Bloom shall provide operations to create Concern Definitions and Concern Usages representing stakeholder concerns, and frame them in requirements or viewpoints via Framed Concern Membership, when "a user creates a Concern and associates it with a requirement or viewpoint." | SysML 8.3.21.3 |
| SSS-PA-REQ-V4J | PA, PT | Mycelium Bloom shall create a Derivation relationship between requirements, linking an original requirement to one or more derived requirements with the semantic constraint that satisfaction of the original implies satisfaction of all derived requirements, when "a user creates a derivation trace between requirements." | SysML 9.6 |
| SSS-PA-REQ-W9B | PA, PT | Mycelium Bloom shall link a Verification Case Usage to a Requirement Usage via Requirement Verification Membership, recording which verification cases verify which requirements, when "a user associates a verification case with a requirement." | SysML 8.3.24.2 |
| SSS-PA-REQ-D7V | PA | Mycelium Bloom shall import requirements from a ReqIF file and export requirements to ReqIF format when "the Project Administrator initiates an import or export operation and selects a ReqIF file." | - |

##### 5.2.1.6 Traceability and allocations

MBSE relies on traceability: every requirement traces to design elements that satisfy it; every functional element allocates to physical elements that realize it. Mycelium offers Allocation, Satisfy, and typed relationships, plus a Relationship Matrix for visualizing and editing trace links across element sets. The requirements in this section cover relationship creation, the matrix view, and coverage analysis.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-PA-TRACE-Q72 | PA, PT | Mycelium Bloom shall support creating Satisfy Requirement Usages to trace which design elements satisfy which requirements when "a user selects a design element and a requirement and creates a satisfy relationship." | SysML 8.3.21.10 |
| SSS-PA-TRACE-YWQ | PA, PT | Mycelium Bloom shall support creating Allocation Definitions and instantiating them as Allocation Usages to map functional elements to physical elements across the system hierarchy when "a user selects source and target elements and creates an allocation." | SysML 7.14 |
| SSS-PA-TRACE-IKS | PA, PT, VW | Mycelium Bloom shall display a Relationship Matrix showing binary relationships between element sets (e.g. requirements vs. parts) when "a user opens the Relationship Matrix view and selects the element sets and relationship type." | - |
| SSS-PA-TRACE-V3H | PA, PT, VW | Mycelium Bloom shall populate the Relationship Matrix rows and columns from user-selected element types, packages, or query results when "a user configures the row source and column source of a Relationship Matrix." | - |
| SSS-PA-TRACE-K7W | PA, PT, VW | Mycelium Bloom shall indicate the presence and direction of relationships in each matrix cell using visual markers (e.g. filled cell, arrow, relationship count) when "the Relationship Matrix renders cells where relationships exist between the row and column elements." | - |
| SSS-PA-TRACE-D2R | PA, PT | Mycelium Bloom shall create a relationship of the selected type between the row element and the column element when "a user clicks an empty cell in the Relationship Matrix." |-  |
| SSS-PA-TRACE-J8N | PA, PT | Mycelium Bloom shall delete the relationship between the row element and the column element when "a user removes a relationship from an occupied cell in the Relationship Matrix." | - |
| SSS-PA-TRACE-F5M | PA, PT, VW | Mycelium Bloom shall filter the Relationship Matrix by relationship type, Ownership, or element category when "a user applies filters to the Relationship Matrix." | - |
| SSS-PA-TRACE-W9G | PA, PT, VW | Mycelium Bloom shall sort the Relationship Matrix rows and columns by element name, namespace path, or relationship count when "a user changes the sort order of the Relationship Matrix." | - |
| SSS-PA-TRACE-B6C | PA, PT, VW | Mycelium Bloom shall display multiple relationship types simultaneously in the Relationship Matrix using distinct visual markers per type when "a user selects more than one relationship type for display." | - |
| SSS-PA-TRACE-H4P | PA, PT, VW | Mycelium Bloom shall navigate to the detail panel of the related elements when "a user double-clicks an occupied cell in the Relationship Matrix." | - |
| SSS-PA-TRACE-R1X | PA, PT, VW | Mycelium Bloom shall export the Relationship Matrix to CSV and PDF formats when "a user initiates an export from the Relationship Matrix view." | - |
| SSS-PA-TRACE-N19 | PA | Mycelium Bloom shall identify and report requirements that are not allocated to any design element or derived requirement when "the Project Administrator executes a requirements coverage analysis." | - |
| SSS-PA-TRACE-8ZB | PA, PT | Mycelium Bloom shall support creating and navigating typed relationships between any model elements when "a user selects source and target elements and specifies a relationship type." | KerML 7.8 |
| SSS-PA-TRACE-V8K | PA, PT | Mycelium Bloom shall create a Dependency relationship between two model elements, asserting that the source element depends on the target element, when "a user creates a generic dependency between two model elements." | KerML 7.3 |

##### 5.2.1.7 Behavior modeling

Beyond structure, systems exhibit behavior: actions performed, states held, transitions triggered, flows of items and data. SysML v2 provides Action Definitions, State Definitions, Flow Connections, and Use Case Definitions. The requirements in this section cover the behavioral modeling capabilities engineers need to describe what the system does and how its behavior depends on context.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-PA-BEH-N5Z | PA, PT | Mycelium Bloom shall support defining Action Definitions with input/output parameters and decomposing them into sub-actions when "a user creates or edits an Action Definition." | SysML 7.16 |
| SSS-PA-BEH-WG5 | PA, PT | Mycelium Bloom shall support modeling control flow between actions using successions, guards, forks, joins, decisions, and merges when "a user creates control flow elements between existing actions." | SysML 7.16 |
| SSS-PA-BEH-Q4N | PA, PT | Mycelium Bloom shall create a generic Succession between two features (e.g. two actions, two states, or two arbitrary occurrences) establishing that the second feature follows the first when "a user creates a succession between two features outside the context of a state machine." | KerML 7.13.5 |
| SSS-PA-BEH-RPK | PA, PT | Mycelium Bloom shall support defining State Definitions with entry, do, and exit actions and connecting them via Transition Usages with triggers, guards, and effects when "a user creates or edits a State Definition." | SysML 7.17 |
| SSS-PA-BEH-PC7 | PA, PT | Mycelium Bloom shall support defining Flow Connection Definitions and instantiating them as Flow Connection Usages to model the transfer of items, energy, or data between parts when "a user creates a Flow Connection Definition and specifies the flow type and endpoints." | SysML 7.15 |
| SSS-PA-BEH-X9V | PA, PT | Mycelium Bloom shall create a Succession Item Flow that conveys items between two features and establishes that the receiving end occurs after the sending end when "a user creates a sequenced flow between two features (e.g. a message between lifelines in a Sequence View, or an ordered item transfer between actions)." | KerML 7.13.6 |
| SSS-PA-BEH-D6L | PA, PT | Mycelium Bloom shall create the corresponding Succession Item Flow in the underlying model when "a user draws a message arrow between two lifelines in a Sequence View." | KerML 7.13.6, SysML 8.2.3.9 |
| SSS-PA-BEH-H83 | PA, PT | Mycelium Bloom shall support assigning behaviors to parts using Perform Action Usages and Exhibit State Usages when "a user selects a part and associates an action or state behavior with it." | SysML 7.16, 7.17 |
| SSS-PA-BEH-IX9 | PA, PT | Mycelium Bloom shall support defining Use Case Definitions specifying system behavior from an external actor perspective when "a user creates a Use Case Definition and specifies actors and subjects." | SysML 7.25 |
| SSS-PA-BEH-T7P | PA, PT | Mycelium Bloom shall create an Include Use Case Usage that includes one Use Case as part of another Use Case when "a user designates one Use Case as included by another." | SysML 7.25 |
| SSS-PA-BEH-J3F | PA, PT | Mycelium Bloom shall create an Extend Use Case Usage that extends one Use Case with the optional behavior of another Use Case when "a user designates one Use Case as extending another." | SysML 7.25 |

##### 5.2.1.8 Analysis and verification

Engineers need to evaluate design quality and verify that requirements are met. Mycelium supports Analysis Cases (evaluating system properties), Verification Cases (verifying requirements with methods and verdicts), Constraint Definitions (validation rules), and Calculation Definitions (domain-specific computations). The requirements in this section cover the analytical capabilities that turn the model into a basis for design decisions.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-PA-AV-QII | PA, PT | Mycelium Bloom shall support defining Analysis Case Definitions and instantiating them as Analysis Case Usages to evaluate system properties with subject binding and objective requirements when "a user creates an Analysis Case Definition and specifies its subject and objectives." | SysML 7.23 |
| SSS-PA-AV-UCQ | PA, PT | Mycelium Bloom shall support defining Verification Case Definitions and instantiating them as Verification Case Usages with specified verification methods (test, analysis, inspection, demonstration) and tracking verdicts (pass, fail, inconclusive) when "a user creates a Verification Case Definition and assigns a method and records a verdict." | SysML 7.24 |
| SSS-PA-AV-LSX | PA, PT | Mycelium Bloom shall support defining Constraint Definitions and instantiating them as Constraint Usages asserted against model elements for automated validation when "a user creates a Constraint Definition and applies it to one or more model elements." | SysML 7.20 |
| SSS-PA-AV-LLI | PA | Mycelium Bloom shall support setting up Trade Studies to compare design alternatives using evaluation functions and objectives (maximize/minimize) when "the Project Administrator creates a Trade Study and specifies alternatives, criteria, and objective functions." | - |
| SSS-PA-AV-2RG | PA, PT, VW | Mycelium Bloom shall display a validation dashboard showing model quality, constraint violations, and verification status when "a user navigates to the validation dashboard view." | - |
| SSS-PA-AV-WRI | PA | Mycelium Bloom shall support defining and executing Rule Verification Lists to check model structural integrity against defined rules when "the Project Administrator creates a Rule Verification List and initiates its execution." | - |
| SSS-PT-ANALYSIS-4W2 | PT | Mycelium Bloom shall support defining Calculation Definitions and instantiating them as Calculation Usages to express domain-specific computations over model attributes when "the Participant creates a Calculation Definition and specifies input parameters, output parameters, and the computation expression." | SysML 7.19 |
| SSS-PT-ANALYSIS-NWL | PT | Mycelium Bloom shall support defining parametric Constraint Usages within their Ownership to validate design feasibility when "the Participant creates a Constraint Usage referencing model attributes within their Ownership." | SysML 7.20 |
| SSS-PT-ANALYSIS-EAJ | PA, PT, VW | Mycelium Bloom shall display constraint evaluation results showing which constraints pass or violate when "a user navigates to the constraint evaluation view or triggers constraint evaluation." | SysML 7.20 |

###### 5.2.1.8a In-browser scripting

Some analyses cannot be expressed declaratively and require imperative computation. Mass budgets, power budgets, and complex requirements verification often need iteration and aggregation across the product tree. The requirements in this section describe a desirable in-browser scripting environment that runs computational analyses against model data without leaving the application.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-PA-SCRIPT-T4K | PA, PT | Mycelium Bloom should provide an in-browser scripting environment for writing and executing scripts that operate on model data when "a user opens the scripting editor." | - |
| SSS-PA-SCRIPT-M7R | PA, PT | Mycelium Bloom should execute scripts entirely in the browser without requiring server-side processing when "a user runs a script in the scripting environment." | - |
| SSS-PA-SCRIPT-H2W | PA, PT | Mycelium Bloom should provide access to the project's model data (element hierarchy, attributes, attribute values, relationships, and metadata) from within the scripting environment when "a script queries or traverses the model." | - |
| SSS-PA-SCRIPT-D9J | PA, PT | Mycelium Bloom should provide script templates for common computational analyses (e.g. mass budget, power budget, cost budget) that traverse the product tree, filter elements by metadata, and aggregate attribute values when "a user creates a new analysis script." | - |
| SSS-PA-SCRIPT-N5V | PA, PT | Mycelium Bloom should display script execution results as formatted tables, charts, or summary values within the scripting environment when "a script produces output." | - |
| SSS-PA-SCRIPT-W3F | PA, PT | Mycelium Bloom should write computed values back to model attributes as Computed value sources when "a script assigns a result to a model attribute and the user confirms the update." | - |
| SSS-PA-SCRIPT-K8B | PA, PT | Mycelium Bloom should evaluate Constraint Usages against model attribute values and report pass/fail/inconclusive verdicts when "a user executes a requirements verification script." | - |
| SSS-PA-SCRIPT-R6P | PA, PT | Mycelium Bloom should save and version scripts as part of the project so they are available to all project members when "a user saves a script in the scripting environment." | - |

##### 5.2.1.9 Diagrams and visualization

###### 5.2.1.9.1 Graphical notation compliance

Mycelium Bloom must render model elements using the symbols defined in SysML v2 Part 1 section 8.2.3. This ensures that diagrams produced in Mycelium are immediately recognizable to anyone familiar with SysML v2 and exchangeable with other SysML v2 tools. The requirements in this section also cover diagram annotations, custom icons, and drag-and-drop interactions that apply to all diagram types.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-PA-VIS-X4G | PA, PT, VW | Mycelium Bloom shall render all model elements using the graphical notation symbols defined in SysML v2 Part 1, section 8.2.3, including Definition and Usage node headers with guillemet kind designators (e.g. «part def», «action», «requirement»), compartment layouts, relationship lines, and adornments when "any diagram view displays model elements." | SysML 8.2.3 |
| SSS-PA-VIS-L7Q | PA, PT, VW | Mycelium Bloom shall visually distinguish Definition nodes from Usage nodes by displaying the `def` keyword in the guillemet header of Definition nodes (e.g. «part def») and omitting it for Usage nodes (e.g. «part») when "a diagram renders Definition and Usage elements." | SysML 8.2.3 |
| SSS-PA-VIS-R3F | PA, PT | Mycelium Bloom shall create the corresponding graphical node on the diagram canvas when "a user drags a model element from the model browser or a tabular browser and drops it onto a diagram." | - |
| SSS-PA-VIS-K8M | PA, PT | Mycelium Bloom shall create the corresponding model element in the underlying model when "a user creates a new graphical node or relationship on a diagram canvas using the diagram toolbox." | - |
| SSS-PA-VIS-H2W | PA, PT, VW | Mycelium Bloom shall reflect changes to model elements in all open diagrams containing those elements when "a model element's properties are modified in any view." | - |
| SSS-PA-VIS-N6J | PA, PT | Mycelium Bloom shall provide a toolbox palette for each diagram type listing the element and relationship types that can be created on that diagram when "a user opens a diagram editor." | - |
| SSS-PA-VIS-U9P | PA, PT, VW | Mycelium Bloom shall display compartments on graphical nodes (attributes, constraints, ports, nested elements) per the SysML v2 compartment notation when "a user expands or views compartments on a diagram element." | SysML 8.2.3 |
| SSS-PA-VIS-D5B | PA, PT, VW | Mycelium Bloom shall display multiplicity, property modifiers (ordered, nonunique, abstract, derived, readonly), and subsetting/redefinition markers on graphical elements per the SysML v2 notation when "a diagram renders elements with these properties." | SysML 8.2.3 |
| SSS-PA-VIS-C9K | PA, PT | Mycelium Bloom shall provide an interface to upload or select a custom icon or image for any Definition or Usage element when "a user accesses the icon settings of a model element." | - |
| SSS-PA-VIS-J2R | PA, PT, VW | Mycelium Bloom shall render the custom icon in place of the standard SysML v2 graphical notation symbol on all diagrams containing the element when "a model element has a custom icon associated with it." | - |
| SSS-PA-VIS-A6F | PA, PT, VW | Mycelium Bloom shall display the element name and type designator alongside the custom icon when "a diagram renders an element with a custom icon." | - |
| SSS-PA-VIS-P3W | PA, PT | Mycelium Bloom shall provide a per-diagram toggle between custom icon rendering and standard SysML v2 notation rendering when "a user switches the rendering mode of a diagram." | - |
| SSS-PA-VIS-F8Q | PA, PT | Mycelium Bloom shall provide operations to place free-text notes on the diagram canvas, with optional formatting (bold, italic, color), when "a user creates a note on a diagram." | KerML 7.4 |
| SSS-PA-VIS-B2M | PA, PT | Mycelium Bloom shall attach a note to a specific model element on the diagram via a dashed anchor line when "a user links a note to a diagram element." | KerML 7.4 |
| SSS-PA-VIS-G5R | PA, PT | Mycelium Bloom shall provide callout annotations that can point to a specific location on the diagram canvas when "a user creates a callout on a diagram." | KerML 7.4 |
| SSS-PA-VIS-T1J | PA, PT | Mycelium Bloom shall persist diagram notes and callouts as SysML v2 Comment elements annotating the relevant model elements when "a user saves a diagram containing notes or callouts." | KerML 7.4 |

###### 5.2.1.9.2 Interconnection View

An Interconnection View shows the structural composition of a system: parts, the ports through which they interact, and the connections between those ports. This is the most common diagram type for system architecture work and the entry point for most reviews of the physical decomposition.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-PA-VIS-G8N | PA, PT | Mycelium Bloom shall provide an editor for creating and editing Interconnection Views showing parts, ports, and connections when "a user opens or creates an Interconnection View for a selected model scope." | SysML 8.2.3.11 |
| SSS-PA-VIS-W3T | PA, PT, VW | Mycelium Bloom shall render Part Usages as rectangular nodes with «part» headers, Port Usages as small squares on part boundaries with directional indicators (in, out, inout), and Connection Usages as lines between ports, using the SysML v2 graphical notation (section 8.2.3.11-14) when "an Interconnection View displays structural model content." | SysML 8.2.3.11-14 |
| SSS-PA-VIS-Q7K | PA, PT, VW | Mycelium Bloom shall render Interface Usages as connection lines between ports with the «interface» label and optional constraint compartments using the SysML v2 graphical notation (section 8.2.3.14) when "an Interconnection View displays interface connections." | SysML 8.2.3.14 |

###### 5.2.1.9.3 Action Flow View

An Action Flow View shows the behavior of the system as a sequence of actions with control flow between them. Engineers use it to describe how the system performs its functions, including parallelism (forks/joins), decisions, and loops. The notation closely follows UML activity diagrams.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-PA-VIS-SMC | PA, PT | Mycelium Bloom shall provide an editor for creating and editing Action Flow Views showing action sequencing, control flow, and swim lanes when "a user opens or creates an Action Flow View for a selected action hierarchy." | SysML 8.2.3.17 |
| SSS-PA-VIS-E4R | PA, PT, VW | Mycelium Bloom shall render Action Usages as rounded-corner rectangles with «action» headers, and control flow using the SysML v2 standard symbols: start node (filled circle), done node (circled filled circle), fork/join nodes (bars), decision/merge nodes (diamonds), and succession arrows, per section 8.2.3.17, when "an Action Flow View displays behavioral model content." | SysML 8.2.3.17 |
| SSS-PA-VIS-J6N | PA, PT, VW | Mycelium Bloom shall render input/output parameters as small rectangles on action node boundaries with directional indicators (in, out, inout) per the SysML v2 graphical notation (section 8.2.3.17) when "an Action Flow View displays actions with parameters." | SysML 8.2.3.17 |
| SSS-PA-VIS-M1Z | PA, PT, VW | Mycelium Bloom shall render send action nodes, accept action nodes, while-loop action nodes, for-loop action nodes, and if-else action nodes using the SysML v2 standard symbols (section 8.2.3.17) when "an Action Flow View displays these action types." | SysML 8.2.3.17 |

###### 5.2.1.9.4 State Transition View

A State Transition View shows the states a system or part can be in and the transitions between them, triggered by events with optional guards and effects. This is essential for modeling operational modes, fault handling, and any behavior that depends on context.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-PA-VIS-DP2 | PA, PT | Mycelium Bloom shall provide an editor for creating and editing State Transition Views showing states and transitions when "a user opens or creates a State Transition View for a selected state machine." | SysML 8.2.3.18 |
| SSS-PA-VIS-B8V | PA, PT, VW | Mycelium Bloom shall render State Usages as rounded-corner rectangles with «state» headers containing entry/do/exit action compartments, and Transition Usages as arrows labeled with trigger [guard] / effect, using the SysML v2 graphical notation (section 8.2.3.18) when "a State Transition View displays state-based model content." | SysML 8.2.3.18 |
| SSS-PA-VIS-F2C | PA, PT, VW | Mycelium Bloom shall render parallel state regions using the «parallel» designator per the SysML v2 graphical notation (section 8.2.3.18) when "a State Transition View displays concurrent state regions." | SysML 8.2.3.18 |

###### 5.2.1.9.5 Sequence View

A Sequence View shows interactions between parts over time as messages exchanged along lifelines. Engineers use it to capture protocol flows, scenario walkthroughs, and timing-sensitive behaviors.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-PA-VIS-FA5 | PA, PT | Mycelium Bloom shall provide an editor for creating and editing Sequence Views showing interactions between parts over time when "a user opens or creates a Sequence View for a selected interaction context." | SysML 8.2.3.9 |
| SSS-PA-VIS-A9H | PA, PT, VW | Mycelium Bloom shall render lifelines as vertical dashed lines below part/port header nodes, and messages as horizontal arrows between lifelines with message labels, using the SysML v2 graphical notation (section 8.2.3.9) when "a Sequence View displays interaction model content." | SysML 8.2.3.9 |

###### 5.2.1.9.6 Requirement View

A Requirement View displays requirements and their satisfaction relationships graphically. Stakeholders can see which design elements satisfy which requirements at a glance, supporting reviews and impact analysis.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-PA-VIS-C3D | PA, PT, VW | Mycelium Bloom shall render Requirement Usages as rectangles with «requirement» headers containing the requirement text, and Satisfy Requirement Usages as dashed arrows labeled «satisfy», using the SysML v2 graphical notation (section 8.2.3.21) when "a diagram displays requirements and their satisfaction relationships." | SysML 8.2.3.21 |

###### 5.2.1.9.7 General View

A General View is an unconstrained canvas where engineers can place any model element type and freely arrange it. It supports brainstorming, mixed concept exploration, and stakeholder-facing presentations that don't fit a single standard diagram type.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-PA-VIS-BB9 | PA, PT | Mycelium Bloom shall provide an editor for creating General Views for unconstrained graphical model exploration when "a user creates a new General View and adds model elements to its canvas." | SysML 8.2.3.5 |
| SSS-PA-VIS-P5W | PA, PT, VW | Mycelium Bloom shall support placing any model element type on a General View canvas using its SysML v2 graphical notation symbol when "a user adds an element to a General View." | SysML 8.2.3 |

###### 5.2.1.9.8 Grid View

A Grid View presents model data in tabular or matrix form. Engineers use it to compare attributes across many elements at once, or to view two-dimensional relationships between element sets.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-PA-VIS-JPW | PA, PT | Mycelium Bloom shall provide a Grid View for tabular and matrix representations of model data when "a user creates a Grid View and selects the element types and properties to display." | SysML 7.26 |

###### 5.2.1.9.9 Custom Views and Viewpoints

Different stakeholders have different concerns: a power engineer wants a power-focused view, a thermal engineer wants thermal data, a customer wants high-level summaries. SysML v2 Viewpoint Definitions and View Definitions let users formalize these stakeholder concerns and create reusable filtered views. The requirements in this section cover defining and managing custom views and viewpoints.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-PA-VIS-T2V | PA, PT | Mycelium Bloom shall support defining custom View Definitions, View Usages, Viewpoint Definitions, and Viewpoint Usages filtered to specific stakeholder concerns when "a user creates a Viewpoint Definition, specifies its concerns, and creates a conforming View Definition." | SysML 7.26 |
| SSS-PA-VIS-K9R | PA, PT | Mycelium Bloom shall create an Expose relationship that imports filtered model content into a View, with optional metadata-based or query-based filter conditions, when "a user adds exposed model content to a View Definition." | SysML 7.26.2 |

###### 5.2.1.9.10 Textual notation

SysML v2 has a textual notation that some engineers prefer for reviewing or sharing model content. Mycelium generates this notation read-only from the model, providing a reference representation without requiring users to edit text directly.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-PA-VIS-IXL | PA, PT, VW | Mycelium Bloom shall generate and display the SysML v2 textual notation representation of model elements (read-only) when "a user selects one or more model elements and requests textual notation export." | SysML 8.2.2 |

###### 5.2.1.9.11 Diagram export

Diagrams need to leave Mycelium for reports, presentations, and external tools. The requirements in this section cover export to SVG (vector), PNG (raster, configurable resolution), and JPG (compressed raster) formats.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-PA-VIS-V7S | PA, PT, VW | Mycelium Bloom shall export a diagram to SVG format preserving vector graphics fidelity when "a user selects SVG as the export format for a diagram." | - |
| SSS-PA-VIS-T1N | PA, PT, VW | Mycelium Bloom shall export a diagram to PNG format at a user-specified resolution when "a user selects PNG as the export format for a diagram." | - |
| SSS-PA-VIS-G4L | PA, PT, VW | Mycelium Bloom shall export a diagram to JPG format at a user-specified resolution and quality when "a user selects JPG as the export format for a diagram." | - |

###### 5.2.1.9.12 3D model viewer

Spatial decomposition is most intuitive in 3D. Mycelium offers an interactive 3D viewer that renders the system using attached geometry (STEP, glTF) or placeholder shapes when no geometry is available. Users can navigate the scene, select elements to inspect properties, and see Ownership-based color coding to understand who is responsible for what.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-PA-VIS-E8Q | PA, PT, VW | Mycelium Bloom shall provide an interactive 3D viewer that renders the system decomposition as a three-dimensional scene when "a user opens the 3D model viewer for a project." | - |
| SSS-PA-VIS-M2K | PA, PT, VW | Mycelium Bloom shall render Part Usages in the 3D viewer using associated 3D geometry (STEP, glTF/GLB) or placeholder shapes when no geometry is available when "the 3D viewer displays model elements." | - |
| SSS-PA-VIS-R5N | PA, PT, VW | Mycelium Bloom shall provide camera controls for orbiting, panning, and zooming the 3D scene when "a user interacts with the 3D viewer using mouse or touch input." | - |
| SSS-PA-VIS-W1J | PA, PT, VW | Mycelium Bloom shall highlight the selected element in the 3D scene and display its properties in the detail panel when "a user selects a model element in the 3D viewer." | - |
| SSS-PA-VIS-H7D | PA, PT, VW | Mycelium Bloom shall synchronize selection between the 3D viewer and the hierarchical Browser View when "a user selects an element in either view." | - |
| SSS-PA-VIS-B4F | PA, PT, VW | Mycelium Bloom shall display Ownership color-coding on 3D elements when "the 3D viewer renders elements in a project with Ownership assignments." |-  |

##### 5.2.1.9a Change persistence

Mycelium Bloom operates in two persistence modes. In immediate mode, each edit is persisted to Mycelium Fabric as an individual Commit on the active branch, making it visible to other users in near real-time. In batch mode, the user collects multiple changes locally before persisting them as a single atomic Commit. Both modes produce Systems Modelling API Commits.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-PA-PERSIST-K4R | PA, PT | Mycelium Bloom shall persist each model edit (create, update, delete) to Mycelium Fabric as an individual Commit on the active branch in immediate mode when "a user completes an edit and immediate persistence mode is active." | API 7.2.3 |
| SSS-PA-PERSIST-W8N | PA, PT | Mycelium Bloom shall accumulate model edits locally without persisting them to Mycelium Fabric in batch mode when "a user performs edits and batch persistence mode is active." | API 7.2.3 |
| SSS-PA-PERSIST-D3J | PA, PT | Mycelium Bloom shall persist all accumulated local edits to Mycelium Fabric as a single atomic Commit on the active branch when "a user submits the batch with a commit description in batch mode." | API 7.2.3 |
| SSS-PA-PERSIST-H7T | PA, PT | Mycelium Bloom shall provide a toggle to switch between immediate mode and batch mode when "a user changes the persistence mode in the application settings or toolbar." | - |
| SSS-PA-PERSIST-M2F | PA, PT | Mycelium Bloom shall display a pending changes indicator showing the number of uncommitted local edits when "the user is in batch mode and has accumulated local changes." | - |
| SSS-PA-PERSIST-R5V | PA, PT | Mycelium Bloom shall display a list of all pending local changes with element name, change type (created, updated, deleted), and changed properties when "the user reviews the pending changes before committing a batch." | - |
| SSS-PA-PERSIST-N9B | PA, PT | Mycelium Bloom shall discard all pending local changes and revert to the last committed model state when "the user cancels a batch in batch mode." | - |
| SSS-PA-PERSIST-T1G | PA, PT | Mycelium Bloom shall warn the user about unsaved local changes when "the user attempts to close the application, switch projects, or switch branches while in batch mode with pending changes." | - |

##### 5.2.1.10 Version control and branching

Mycelium models are versioned like source code. Every change becomes a Commit; alternatives live on Branches; milestones are marked with Tags; merges combine work from different lines. The requirements in this section cover the full Systems Modelling API version control model adapted to a collaborative MBSE context, including a Git-style history graph for navigating commits and branches.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-PA-VC-8SB | PA, PT | Mycelium Bloom shall create a Commit representing an immutable, non-destructible snapshot of model changes, consistent with the Systems Modelling API Commit concept, when "a user submits pending model changes with a commit description." | API 7.2.3 |
| SSS-PA-VC-PPI | PA, PT | Mycelium Bloom shall provide operations to create and manage Branches as independent lines of model development, each pointing to a head Commit, when "a user creates a new Branch from an existing Commit or manages existing Branches." | API 7.2.2 |
| SSS-PA-VC-SPJ | PA | Mycelium Bloom shall create an immutable Tag on a specific Commit to mark a milestone, baseline, or release when "the Project Administrator selects a Commit and assigns a Tag name." | API 7.2.4 |
| SSS-PA-VC-AJ9 | PA, PT | Mycelium Bloom shall merge Commits into a Branch with conflict detection and resolution, consistent with the Systems Modelling API mergeIntoBranch operation, when "a user initiates a merge of a source Branch into a target Branch." | API 7.2.5 |
| SSS-PA-VC-P89 | PA, PT, VW | Mycelium Bloom shall display the differences between two Commits showing which elements were created, updated, or deleted, consistent with the Systems Modelling API diffCommits operation, when "a user selects two Commits for comparison." | API 7.2.6 |
| SSS-PA-VC-7S4 | PA, PT, VW | Mycelium Bloom shall retrieve and display the complete versioned data of a Project at any Commit when "a user selects a historical Commit for inspection." | API 7.2.3 |
| SSS-PA-VC-KXT | PA | Mycelium Bloom shall provide a configuration interface for branch protection rules on any branch when "the Project Administrator accesses the branch protection settings." | - |
| SSS-PA-VC-28D | PA | Mycelium Bloom shall provide operations to designate Participants or Viewers as Reviewers for protected branches when "the Project Administrator assigns reviewers in the branch protection settings." | - |
| SSS-VW-VH-WGA | PA, PT, VW | Mycelium Bloom shall display the commit history of a project when "a user navigates to the version history view." | API 7.2.3 |
| SSS-PA-VC-V3K | PA, PT | Mycelium Bloom shall switch the active branch, loading the model state at the head Commit of the selected branch, when "a user selects a different branch from the branch selector." | API 7.2.2 |
| SSS-PA-VC-R8W | PA, PT, VW | Mycelium Bloom shall display the currently active branch name in the application header when "a user is working in a project." | - |
| SSS-PA-VC-H4N | PA, PT, VW | Mycelium Bloom shall display a list of all branches in the project with their name, head Commit, creator, and creation date when "a user opens the branch management view." | API 7.2.2 |
| SSS-PA-VC-D7J | PA | Mycelium Bloom shall delete a non-default branch when "the Project Administrator initiates branch deletion and confirms the action." | API 7.2.2 |
| SSS-PA-VC-M1F | PA, PT, VW | Mycelium Bloom shall display the commit and branch history as a graph visualization with parallel lanes for branches, commit nodes, merge lines, and tag markers when "a user opens the version history graph view." | - |
| SSS-PA-VC-W5T | PA, PT, VW | Mycelium Bloom shall display commit metadata (author, date, description, changed element count) in a detail panel when "a user selects a commit node in the version history graph." | API 7.2.3 |
| SSS-PA-VC-N9B | PA, PT, VW | Mycelium Bloom shall highlight the active branch and its head Commit in the version history graph when "the version history graph is displayed." | - |
| SSS-PA-VC-F2G | PA, PT, VW | Mycelium Bloom shall load the complete model state at a selected historical Commit in read-only mode when "a user selects a Commit other than the head Commit from the version history graph, branch list, or commit history." | API 7.2.3 |
| SSS-PA-VC-J6K | PA, PT, VW | Mycelium Bloom shall display a visual indicator (e.g. banner or badge) stating the Commit identifier and date, making clear the user is viewing a historical snapshot and not the current head, when "the model is loaded at a historical Commit." | - |
| SSS-PA-VC-T3P | PA, PT | Mycelium Bloom shall create a new Branch from a selected historical Commit when "a user chooses to branch from a historical Commit to continue development from that point in time." | API 7.2.2 |
| SSS-PA-VC-B8W | PA, PT, VW | Mycelium Bloom shall return to the head Commit of the active branch when "a user exits the historical snapshot view." | - |

##### 5.2.1.11 Design alternatives and variants

Early-phase design explores multiple solutions before committing to one. Mycelium offers two complementary mechanisms: Branches for fully independent design alternatives, and Variation Points/Variants for in-place variability within a single branch. The requirements in this section cover both, with detailed support for variant comparison and configuration selection.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-PA-OPT-09P | PA, PT | Mycelium Bloom shall support exploring design alternatives using Branches, where each Branch represents an independent line of development for a candidate solution, when "a user creates a Branch for a design alternative from an existing Commit." | API 7.2.2 |
| SSS-PA-OPT-DNI | PA, PT, VW | Mycelium Bloom shall display a comparison of design alternatives by diffing Commits across Branches when "a user selects Commits from different Branches for cross-branch comparison." | API 7.2.6 |
| SSS-PA-OPT-W7T | PA | Mycelium Bloom shall merge a selected design alternative Branch into the default Branch when "the Project Administrator initiates a merge of the alternative Branch and resolves any conflicts." | API 7.2.5 |

###### 5.2.1.11a Variation point and variant modeling

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-PA-VAR-K3T | PA, PT | Mycelium Bloom shall mark a Part Usage, Item Usage, or Port Usage as a variation point by setting `isVariation = true` when "a user designates a Usage as a variation point." | SysML 7.9.6 |
| SSS-PA-VAR-R7W | PA, PT | Mycelium Bloom shall create variant Usages as alternative options nested under a variation point when "a user adds a variant to an existing variation point." | SysML 7.9.6 |
| SSS-PA-VAR-N5D | PA, PT | Mycelium Bloom shall remove a variant Usage from a variation point when "a user deletes a variant from a variation point." | SysML 7.9.6 |
| SSS-PA-VAR-H2J | PA, PT, VW | Mycelium Bloom shall visually distinguish variation points from regular Usages in the model browser and on diagrams using a distinct indicator when "the model contains Usages with `isVariation = true`." | SysML 7.9.6 |
| SSS-PA-VAR-M8F | PA, PT, VW | Mycelium Bloom shall display all variant Usages nested under a variation point as selectable alternatives when "a user expands a variation point in the model browser." | SysML 7.9.6 |
| SSS-PA-VAR-D4B | PA, PT | Mycelium Bloom shall select an active variant for a variation point, filtering the model browser, diagrams, and calculations to show only the selected variant's content, when "a user selects a variant from the variant selector of a variation point." | SysML 7.9.6 |
| SSS-PA-VAR-W6N | PA, PT, VW | Mycelium Bloom shall display a side-by-side comparison of attribute values across all variants of a variation point when "a user opens the variant comparison view for a variation point." | SysML 7.9.6 |
| SSS-PA-VAR-J9K | PA, PT | Mycelium Bloom shall propagate structural changes (added attributes, ports, nested usages) from the variation point to all its variants when "a user modifies the shared structure of a variation point." | SysML 7.9.6 |
| SSS-PA-VAR-F1P | PA, PT | Mycelium Bloom shall override attribute values on individual variant Usages without affecting other variants or the variation point definition when "a user edits an attribute value on a specific variant." | SysML 7.9.6 |
| SSS-PA-VAR-B3G | PA, PT, VW | Mycelium Bloom shall indicate in the product tree which variant is active for each variation point using visual markers when "the model contains variation points with a selected active variant." | SysML 7.9.6 |
| SSS-PA-VAR-T6L | PA, PT, VW | Mycelium Bloom shall display the product breakdown structure rooted at a variation point with the decomposition of each variant shown side-by-side or in switchable tabs when "a user opens the variant product tree view for a variation point." | SysML 7.9.6 |
| SSS-PA-VAR-E2Q | PA, PT, VW | Mycelium Bloom shall highlight structural differences (added, removed, or changed elements and attributes) between variants in the per-variation-point product tree view when "two or more variants are displayed for comparison." | SysML 7.9.6 |
| SSS-PA-VAR-G8X | PA, PT | Mycelium Bloom shall provide a configuration selector where the user selects one active variant per variation point to define a complete system configuration when "the model contains multiple variation points." | SysML 7.9.6 |
| SSS-PA-VAR-C5H | PA, PT, VW | Mycelium Bloom shall display a resolved product tree showing the full system decomposition with only the selected variants included, as if the configuration were the actual design, when "a user applies a variant configuration." | SysML 7.9.6 |

##### 5.2.1.12 Concurrent design sessions

Concurrent design brings 20-30 engineers from different domains into the same room (or video call) to design a system together in real time. Mycelium must handle this scale, propagate changes across all connected users, and present session-aware views that show what is happening across the team. The requirements in this section cover concurrent session participation and the views engineers need during a session.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-PT-CDS-RKV | PA, PT | Mycelium Bloom shall support concurrent design sessions with 20-30 Participants from multiple Ownerships working simultaneously when "multiple Participants are connected to the same project and actively modifying model data." | - |
| SSS-PT-CDS-I22 | PA, PT, VW | Mycelium Bloom shall display the product tree showing the full system decomposition with ownership indicators when "a user navigates to the product tree view during a design session." | - |
| SSS-PT-CDS-9ZA | PA, PT | Mycelium Bloom shall persist updated attribute values to the shared model when "a Participant commits attribute changes during a concurrent design session." | - |
| SSS-PT-CDS-YGL | PA, PT, VW | Mycelium Bloom shall display a dashboard tracking design drivers and key attribute evolution across Commits when "a user navigates to the design convergence dashboard." | - |
| SSS-PT-CDS-TGV | PA, PT, VW | Mycelium Bloom shall display a comparison of attribute values between Commits or Tags when "a user selects two Commits or Tags for attribute comparison." | - |
| SSS-VW-OBS-UZS | VW | Mycelium Bloom shall render a concurrent design session with real-time model updates without editing capability when "a Viewer joins an active design session." | - |
| SSS-VW-OBS-GU1 | PA, PT, VW | Mycelium Bloom shall display which Participants are active and which Ownerships they are editing when "a user views the session participant list." | - |

###### 5.2.1.12a Element history and design convergence

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-PA-HIST-K3R | PA, PT, VW | Mycelium Bloom shall display the value history of one or more selected attributes as a time-series chart plotting the attribute values across Commits or Tags on the active branch when "a user opens the attribute history view and selects one or more attributes." | - |
| SSS-PA-HIST-T6W | PA, PT, VW | Mycelium Bloom shall render multiple attributes of different quantity kinds on the same chart using independent Y-axes (one per quantity kind) with distinct colors and a shared Commit/Tag X-axis when "a user selects attributes with different units or quantity kinds for the attribute history view." | - |
| SSS-PA-HIST-V2P | PA, PT, VW | Mycelium Bloom shall display the unit and quantity kind label on each Y-axis and provide a legend identifying each plotted attribute by name, element, and unit when "the attribute history chart displays multiple attributes." | - |
| SSS-PA-HIST-W8D | PA, PT, VW | Mycelium Bloom shall display the change history of any model element listing all Commits in which the element was created, modified, or deleted, with the commit author, date, and description, when "a user opens the element history view." | - |
| SSS-PA-HIST-N5T | PA, PT, VW | Mycelium Bloom shall display the property-level diff of a model element between two Commits, showing which attributes, relationships, and metadata changed and their old vs new values, when "a user selects two Commits in the element history view." | - |
| SSS-PA-HIST-D2J | PA, PT, VW | Mycelium Bloom shall display requirements coverage evolution as a chart showing the percentage of requirements with at least one Satisfy relationship across Commits or Tags when "a user opens the requirements coverage trend view." | - |
| SSS-PA-HIST-H7F | PA, PT, VW | Mycelium Bloom shall display verification status evolution as a chart showing the count of pass, fail, and inconclusive verdicts across Commits or Tags when "a user opens the verification trend view." | - |
| SSS-PA-HIST-M4B | PA, PT, VW | Mycelium Bloom shall display constraint compliance evolution as a chart showing the count of satisfied vs violated constraints across Commits or Tags when "a user opens the constraint compliance trend view." | - |
| SSS-PA-HIST-R9G | PA, PT, VW | Mycelium Bloom shall display model growth metrics (total element count, total relationship count, total attribute count) as a chart across Commits or Tags when "a user opens the model growth trend view." | - |

##### 5.2.1.13 Collaboration and subscriptions

When one engineer's work depends on another's outputs, they need to know when those outputs change. Mycelium models these dependencies as ParameterSubscriptions: a subscriber expresses interest in an attribute owned by another Ownership, and the system notifies them of updates. The requirements in this section cover subscription management, ownership visibility, and presence indicators showing who is active.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-PT-COLLAB-8U9 | PT | Mycelium Bloom shall create a ParameterSubscription on an AttributeUsage owned by another Ownership when "the Participant selects an attribute from another Ownership and initiates a subscription." | - |
| SSS-PT-COLLAB-12K | PT | Mycelium Bloom shall deliver a near real-time notification when "an attribute that the Participant has subscribed to is published by another Ownership." | - |
| SSS-PT-COLLAB-7U5 | PA, PT, VW | Mycelium Bloom shall display the Ownership of each element and attribute when "a user views a model element's properties or browses the model tree." | - |
| SSS-PT-COLLAB-JB0 | PA, PT, VW | Mycelium Bloom shall display which Participants are currently active in the project when "a user views the collaboration status indicator." | - |

###### 5.2.1.13b Publication workflow

In Concurrent Design Mode, attribute owners edit their own values (OwnedValue) without immediately affecting the values visible to subscribers. A publication event copies the OwnedValue to the AttributeUsage value, making it available to all consumers. The publication mechanism is modeled in the Concurrent Design library using PublicationDefinition, PublishedIn, and OwnedValue MetadataDefinitions (see [Roles and Permissions](Roles-and-Permissions.md)).

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-PT-PUB-K4W | PT | Mycelium Bloom shall store the owner's pending value as an OwnedValue metadata annotation on the AttributeUsage, without overwriting the published attribute value visible to subscribers, when "a Participant edits an attribute value in Concurrent Design Mode." | - |
| SSS-PT-PUB-R7N | PA, PT, VW | Mycelium Bloom shall visually distinguish attributes with pending unpublished changes (OwnedValue differs from published value) from attributes where OwnedValue and published value are equal when "a user views attributes in the model browser, detail panel, or product tree." | - |
| SSS-PT-PUB-D3M | PA, PT, VW | Mycelium Bloom shall display the old (published) value, the new (owned) value, and the difference (absolute and percentage) for each attribute with pending changes when "a user opens the publication review view." | - |
| SSS-PT-PUB-H8J | PA | Mycelium Bloom shall publish all pending attribute changes across all Ownerships in a single operation, copying each OwnedValue to its corresponding AttributeUsage value and creating a PublicationDefinition record with timestamp, when "the Project Administrator initiates a publish-all operation." | - |
| SSS-PT-PUB-W5T | PA | Mycelium Bloom shall publish pending attribute changes for a single selected Ownership, copying only that Ownership's OwnedValues to their corresponding AttributeUsage values and recording the publication, when "the Project Administrator initiates a publish-per-ownership operation and selects an Ownership." | - |
| SSS-PT-PUB-M2F | PA, PT, VW | Mycelium Bloom shall display a publication history listing all past publications with their timestamp, the publishing user, and the Ownerships included when "a user opens the publication history view." | - |
| SSS-PT-PUB-N6K | PA, PT, VW | Mycelium Bloom shall display the list of attributes that were published in a specific publication event, showing the attribute name, element, old value, new value, and Ownership, when "a user selects a publication record from the publication history." | - |
| SSS-PT-PUB-B9G | PA, PT | Mycelium Bloom shall annotate each published AttributeUsage with a PublishedIn metadata recording the old value, new value, and associated PublicationDefinition when "a publication operation completes." | - |
| SSS-PT-PUB-J4R | All | Mycelium Fabric shall deliver the published attribute values to all connected subscribers via SignalR when "a publication operation completes and attribute values are updated." | - |
| SSS-PT-PUB-F1V | All | Mycelium Fabric shall reject direct modification of published attribute values by non-owner Participants and enforce that only the publication workflow updates the shared attribute value in Concurrent Design Mode when "a Participant attempts to write directly to an AttributeUsage value they subscribe to." | - |

###### 5.2.1.13c Live model updates

When user A edits the model, user B should see the change in near real-time without manually refreshing. Mycelium Bloom listens for change notifications from the backend and updates open views accordingly. The requirements in this section cover the UI behavior on receipt of live updates, including conflict indicators and preservation of the user's editing context.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-CC-LIVE-R4K | All | Mycelium Bloom shall update the model browser, detail panels, and tabular browsers in near real-time to reflect changes committed by other users when "Mycelium Fabric delivers a model change notification via SignalR." | -- |
| SSS-CC-LIVE-W7N | All | Mycelium Bloom shall update all open diagrams in near real-time to reflect changes to model elements committed by other users when "a diagram contains elements that have been modified by another user." | - |
| SSS-CC-LIVE-H3D | All | Mycelium Bloom shall display a visual indicator (e.g. highlight, flash, or badge) on model elements that have been modified by another user when "a model change notification is received for an element visible in the current view." | - |
| SSS-CC-LIVE-M6J | All | Mycelium Bloom shall display a notification summary indicating the number and nature of changes made by other users when "one or more model change notifications are received while the user is working." | - |
| SSS-CC-LIVE-T9F | PA, PT | Mycelium Bloom shall present a conflict indicator when the current user has uncommitted local changes to an element that another user has also modified and committed when "a model change notification is received for an element with pending local edits." | - |
| SSS-CC-LIVE-K2B | All | Mycelium Bloom shall maintain the user's current scroll position, selection, and editing state when applying live model updates from other users when "the UI refreshes in response to incoming model changes." | - |

###### 5.2.1.13d Multi-backend support and polling

Mycelium Bloom must work not only with Mycelium Fabric but with any backend that implements the OMG Systems Modelling API. Some backends support push notifications (SignalR/WebSocket); others do not. The requirements in this section cover backend portability and a polling fallback for backends without push capability.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-CC-BACK-R5W | All | Mycelium Bloom shall connect to any backend server that implements the OMG Systems Modelling API and Services specification (REST/HTTP PSM), not only Mycelium Fabric, when "a user provides the URL of a compliant model server." | API 7 |
| SSS-CC-BACK-K8N | All | Mycelium Bloom shall detect whether the connected backend supports push-based change notifications (e.g. SignalR/WebSocket) and fall back to a polling mechanism when "the backend does not offer push-based notifications." | - |
| SSS-CC-BACK-D3T | All | Mycelium Bloom shall poll the connected backend for model changes at a user-configurable interval when "the polling mechanism is active." | - |
| SSS-CC-BACK-H7J | All | Mycelium Bloom shall provide a setting to configure the polling interval (in seconds) and to enable or disable polling when "a user accesses the connection settings for a backend." | - |
| SSS-CC-BACK-M1V | All | Mycelium Bloom shall provide a manual refresh operation that retrieves the complete current model state from the connected backend when "a user initiates a manual refresh." | - |

##### 5.2.1.14 Queries

Engineers need to ask questions of their models: list all elements categorized as Equipment, find all requirements with no Satisfy relationship, retrieve all parts above a mass threshold. Mycelium offers a query interface based on the Systems Modelling API query operations, with the ability to save and re-execute queries against any commit.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-PA-QRY-L11 | PA, PT | Mycelium Bloom shall provide a query interface supporting select, scope, where, and orderBy clauses, with the ability to save and re-execute queries, when "a user composes a query and submits it for execution." | API 7.3 |
| SSS-PA-QRY-JYA | PA, PT, VW | Mycelium Bloom shall execute queries against any commit to retrieve historical model state when "a user specifies a target Commit identifier before executing a query." | API 7.3 |

##### 5.2.1.15 Reporting and dashboards

Beyond raw data, engineers and stakeholders need summary views showing model health, progress, and metrics. Mycelium provides dashboards for system monitoring, validation, and project model health, with click-through navigation from summary metrics to underlying elements.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-OA-SYS-IEJ | OA | Mycelium Bloom shall display active user sessions and system health metrics when "the Organization Administrator navigates to the system monitoring dashboard." | - |

###### 5.2.1.15a Project model dashboard

The project model dashboard gives the study lead and team a single view of model health: how many attributes are published vs unpublished, how many elements are unused, what the distribution of element types and Ownerships looks like, and how requirements coverage and constraint compliance are progressing. The requirements in this section cover histograms, pie charts, summary metrics, filtering, and click-through navigation, inspired by the equivalent CDP4-COMET-WEB dashboard.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-PA-DASH-K7R | PA, PT, VW | Mycelium Bloom shall display a project model dashboard presenting an overview of model health and completeness when "a user opens the project model dashboard." | - |
| SSS-PA-DASH-W3D | PA, PT, VW | Mycelium Bloom shall display a histogram and summary count of published vs unpublished attributes per Ownership when "the project model dashboard renders the publication status section." | - |
| SSS-PA-DASH-N8F | PA, PT, VW | Mycelium Bloom shall display a histogram and summary count of attributes with missing values (no value assigned) grouped by Ownership when "the project model dashboard renders the missing values section." | - |
| SSS-PA-DASH-H2T | PA, PT, VW | Mycelium Bloom shall display a histogram and summary count of unused Definitions (Definitions with no Usages in the model) grouped by element type when "the project model dashboard renders the unused definitions section." | - |
| SSS-PA-DASH-D5J | PA, PT, VW | Mycelium Bloom shall display a histogram and summary count of unreferenced Usages (Usages not connected via any relationship, port, or connection to other elements) grouped by element type when "the project model dashboard renders the unreferenced elements section." | - |
| SSS-PA-DASH-R9V | PA, PT, VW | Mycelium Bloom shall display a pie chart showing the distribution of model elements by element type (Part, Item, Action, State, Requirement, Constraint, etc.) when "the project model dashboard renders the element composition section." | - |
| SSS-PA-DASH-T4K | PA, PT, VW | Mycelium Bloom shall display a pie chart showing the distribution of model elements by Ownership when "the project model dashboard renders the ownership distribution section." | - |
| SSS-PA-DASH-M6W | PA, PT, VW | Mycelium Bloom shall display a summary of requirements coverage showing the count and percentage of requirements with Satisfy relationships, Verification Case links, and unallocated requirements when "the project model dashboard renders the requirements coverage section." | - |
| SSS-PA-DASH-J1B | PA, PT, VW | Mycelium Bloom shall display a summary of constraint compliance showing the count of satisfied, violated, and unevaluated constraints when "the project model dashboard renders the constraint compliance section." | - |
| SSS-PA-DASH-V8G | PA, PT, VW | Mycelium Bloom shall display a summary of subscription activity showing the count of active ParameterSubscriptions and the count of subscribed attributes with stale (unpublished) values when "the project model dashboard renders the subscription status section." | - |
| SSS-PA-DASH-F3K | PA, PT, VW | Mycelium Bloom shall filter all project model dashboard sections by Ownership, element type, metadata annotation, and variant configuration when "a user applies filters to the project model dashboard." | - |
| SSS-PA-DASH-B7N | PA, PT, VW | Mycelium Bloom shall navigate to the list of matching model elements when "a user clicks a bar in a histogram or a segment in a pie chart on the project model dashboard." | - |

##### 5.2.1.16 User interface adaptation

Mycelium supports novice, intermediate, and expert users. The interface should adapt to the user's role and Ownership, surface commonly-used features prominently, and provide context-aware help. The requirements in this section cover role-aware interface adaptation, workspace customization, and the About dialog.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-PT-UI-NLJ | All | Mycelium Bloom shall provide a role-aware interface that surfaces information and tools relevant to the user's assigned Ownership and tasks when "a user logs in and the application loads their role and Ownership assignments." | - |
| SSS-PT-UI-L3Q | PA, PT, VW | Mycelium Bloom shall provide workspace customization for dashboard layouts and favorite views when "a user accesses workspace customization settings." | - |
| SSS-PT-UI-YJL | All | Mycelium Bloom shall present commonly used features first and advanced capabilities on demand (progressive disclosure) when "a user interacts with any feature area of the application." | - |
| SSS-PT-UI-2BM | PA, PT, VW | Mycelium Bloom shall display context-aware panels showing related diagrams, constraints, and verification results when "a user selects a model element in any view." | - |
| SSS-PT-UI-256 | PT | Mycelium Bloom shall present a selector to switch the active Ownership when "the Participant is assigned to multiple Ownerships and selects a different active Ownership from the Ownership selector." | - |
| SSS-PT-UI-G4M | All | Mycelium Bloom shall display an About window showing the application name, version number, license information, copyright notice, and links to documentation and source code when "a user opens the About dialog." | - |

##### 5.2.1.17 Import, export and migration

Mycelium must interoperate with the broader MBSE ecosystem. Models can be imported and exported in SysML v2 JSON, requirements in ReqIF, content in HTML for documentation. CDP4-COMET ECSS-E-TM-10-25 models can be migrated to SysML v2 via a semi-automated converter. The requirements in this section cover all import, export, and migration capabilities.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-PA-IE-QWN | PA | Mycelium Bloom shall import and export models in standard SysML v2 JSON format compliant with the OMG Systems Modelling API when "the Project Administrator initiates an import or export operation and selects the target file or endpoint." | API 7 |
| SSS-PA-IE-ZLQ | PA | Mycelium Bloom shall migrate existing ECSS-E-TM-10-25 models from CDP4-COMET into SysML v2 using a semi-automated converter when "the Project Administrator uploads an ECSS-E-TM-10-25 model and initiates the migration process." | - |
| SSS-PA-IE-YSY | PA | Mycelium Bloom shall present mapping ambiguities for user resolution during ECSS-to-SysML v2 migration when "the converter encounters ECSS-E-TM-10-25 elements that do not have a deterministic SysML v2 mapping." | - |
| SSS-PA-IE-GYP | PA | Mycelium Bloom shall support creating and managing Project Usages to reference elements from one Project within another, consistent with the Systems Modelling API ProjectUsageService, when "the Project Administrator creates a Project Usage and selects the target project to reference." | API 7.4 |
| SSS-VW-EXP-KVK | PA, PT, VW | Mycelium Bloom shall export views, diagrams, and reports to standard formats (e.g. PDF, image) when "a user initiates an export from a view or dashboard." | - |
| SSS-PA-IE-B5W | PA, PT, VW | Mycelium Bloom shall export a Requirements Specification as a navigable HTML document preserving the hierarchical structure, requirement text, categories, and constraint details when "a user selects HTML as the export format for a Requirements Specification." | - |
| SSS-PA-IE-N8G | PA, PT, VW | Mycelium Bloom shall export a user-selected set of model elements (e.g. a Package, a Part Definition with its decomposition, or a filtered query result) as a navigable HTML document showing element properties, attributes, relationships, and Documentation when "a user selects HTML as the export format for a model element selection." | - |

##### 5.2.1.18 Review workflow

Branch protection rules can require designated Reviewers to approve merges before they enter the default branch. The requirements in this section cover the reviewer interface for approving or requesting changes on protected branch merges, supporting the gatekeeper model for design baselines.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-PT-VC-JPL | PA, PT, VW | Mycelium Bloom shall provide a review interface to approve or request changes on merges to protected branches when "a user has been designated as a Reviewer and a merge is proposed." | - |


##### 5.2.1.19 User profile

Users have a profile showing their identity, projects, and contributions. The requirements in this section cover the profile page contents — personal details, project list with key metadata, and a contribution heatmap showing activity over time.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-CC-PROF-L6D | All | Mycelium Bloom shall display the user's profile details, a list of all projects the user is a member of, and a contribution heatmap when "a user navigates to their profile page." | - |
| SSS-CC-PROF-52O | All | Mycelium Bloom shall display each project in the profile project list with: project name, description, license, last updated date, visibility (private, organization, public), and activity sparkline when "the user views their profile project list." | - |

---

#### 5.2.2 Mycelium Fabric

##### 5.2.2.1 Systems Modelling API

Mycelium Fabric implements the OMG Systems Modelling API and Services specification. This is what makes Mycelium a SysML v2 native platform and what enables interoperability with other tools that consume the standard API. The requirements in this section anchor the Fabric implementation to the standard.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-CC-STD-XSS | All | Mycelium Fabric shall implement the OMG Systems Modelling API and Services specification (formal/25-09-04) using the REST/HTTP PSM when "the model server processes any API request." | API 7 |
| SSS-CC-EXT-QIN | All | Mycelium Fabric shall expose a REST API compliant with the OMG Systems Modelling API to enable integration with domain-specific tools when "an external tool issues API requests to the model server." | API 7 |

##### 5.2.2.2 Authentication and authorization

User identity, credentials, and session management are handled by Mycelium Fabric in conjunction with an external identity provider (Keycloak by default). The requirements in this section cover authentication enforcement, security policy enforcement, and the user invitation mechanism.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-OA-AUTH-SI7 | All | Mycelium Fabric shall authenticate all user sessions using token-based authentication when "a user submits valid credentials at the login interface." | - |
| SSS-OA-AUTH-GIT | All | Mycelium Fabric shall enforce password policies and session expiration rules when "the Organization Administrator has configured security policies in the authentication settings." | - |
| SSS-FB-AUTH-L0Z | All | Mycelium Fabric shall send an invitation to a user to join the organization as a Member when "the Organization Administrator submits an invitation with the target user's identity." | - |
| SSS-FB-IA-R4X | All | Mycelium Fabric shall restrict installation-wide management API endpoints to users with the Installation Administrator role when "a user attempts to access installation administration operations." | - |
| SSS-FB-IA-J6C | All | Mycelium Fabric shall assign the Installation Administrator role to the first user who completes the initial setup on an on-premise deployment when "the installation has no existing Installation Administrator." | - |
| SSS-FB-IA-Y2M | IA | Mycelium Fabric shall record all installation-wide administrative actions in an immutable audit log when "an Installation Administrator performs a create, update, delete, suspend, or reactivate operation on a user or organization." | - |

##### 5.2.2.3 Ownership enforcement

Ownership-based access control is enforced server-side by Mycelium Fabric — Bloom merely presents the UI for it. The requirements in this section ensure that no user can bypass ownership rules by talking directly to the Fabric API or by using a different client.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-CC-COLLAB-KOR | All | Mycelium Fabric shall enforce ownership-based access control using Owner metadata annotations on model elements when "a user attempts to create, modify, or delete a model element." | - |
| SSS-PT-COLLAB-G8G | All | Mycelium Fabric shall prevent modification of elements and attributes not owned by the user when "a Participant attempts to edit an element whose Owner metadata does not match their assigned Ownership." | - |
| SSS-FB-ATT-T4X | All | Mycelium Fabric shall enforce ownership-based access control on attachment operations (upload, delete) consistent with the element's Owner metadata when "a user attempts to modify attachments on a model element." | - |

##### 5.2.2.4 Real-time notifications

Mycelium Fabric is responsible for propagating model changes to all connected clients in near real-time, enabling the live update behavior in Bloom. The requirements in this section cover the server-side notification mechanism using SignalR.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-CC-COLLAB-TLB | All | Mycelium Fabric shall propagate model changes to all connected users in near real-time when "a user commits changes to the shared model." | API 7 |

##### 5.2.2.5 Model persistence and versioning

Mycelium Fabric persists model data in a relational database with auto-generated schemas from the SysML v2 metamodel. The requirements in this section cover persistence performance and API responsiveness targets.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-CC-PERF-6HL | All | Mycelium Fabric shall persist models with up to 50,000 (TBC) elements within a responsive timeframe (target TBD) when "a user commits changes to a model containing up to 50,000 (TBC) elements." | - |
| SSS-CC-PERF-WTU | All | Mycelium Fabric shall respond to standard REST API requests within a responsive timeframe (target TBD) when "an external client or the web application issues an API request to the model server." | - |

##### 5.2.2.6 Concurrent design support

Lock-free collaboration is fundamental to concurrent design — no user can block others from editing the model. The requirements in this section anchor the server-side support for lock-free collaboration with optimistic concurrency.

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-CC-COLLAB-62C | All | Mycelium Fabric shall support lock-free collaborative modeling where no single user can block others from updating the model when "multiple users concurrently modify different elements within the same project." | - |

---

#### 5.2.3 Mycelium Forge

##### 5.2.3.1 Package registry

TBD — Requirements for publishing, discovering, versioning and downloading SysML v2 model packages.

##### 5.2.3.2 Library management

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-PA-IE-OYJ | PA | Mycelium Forge shall provide standard SysML v2 libraries (e.g. Quantities and Units, standard view definitions) for import into a project when "the Project Administrator selects one or more standard libraries for import." | SysML 9.8 |

---

### 5.3 System interface requirements

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-CC-EXT-5DV | PA, PT | Mycelium Bloom shall support External Relationships linking model elements to external web resources via IRIs when "a user creates a relationship targeting an external resource identified by an IRI." | SysML 7.3 |
| SSS-CC-EXT-OAW | PA, PT | Mycelium Bloom shall support defining Metadata Definitions and instantiating them as Metadata Usages for tool-specific and process-specific annotations on model elements when "a user or tool creates metadata on a model element." | SysML 7.27 |

Additional interface specifications between Mycelium Bloom, Mycelium Fabric, and Mycelium Forge are described in section 4.4 (Operational environment) and will be elaborated in the Interface Requirements Document (IRD).

### 5.4 Adaptation and missionization requirements

The Mycelium platform supports two project modes (Regular and Concurrent Design) configurable per project (see SSS-PA-MGMT-73C). The platform supports both SaaS and on-premise deployment models with configurable organization policies.

Additional adaptation and missionization requirements: TBD.

### 5.5 Computer resource requirements

#### 5.5.1 Computer hardware resource requirements

TBD — Minimum server specifications, browser hardware requirements.

#### 5.5.2 Computer hardware resource utilization requirements

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-CC-PERF-CDT | All | Mycelium Bloom shall load a model with up to 10,000 elements within a responsive timeframe (target TBD) when "a user opens a project containing up to 10,000 model elements." | - |
| SSS-CC-PERF-NGA | All | Mycelium Bloom shall reflect model edits in the UI within a responsive timeframe (target TBD) when "a user or a collaborating user commits a model change." | - |
| SSS-CC-PERF-EIU | All | Mycelium Bloom shall render diagrams with 100+ elements within a responsive timeframe (target TBD) when "a user opens a diagram view containing 100 or more graphical elements." | - |

#### 5.5.3 Computer software resource requirements

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-CC-WEB-1MV | All | The Mycelium platform shall be deployable as a cloud-native containerized service when "a system operator deploys the application using container orchestration tools." | - |

The platform depends on the following software components: PostgreSQL (persistence), Keycloak (identity management), RabbitMQ (messaging).

### 5.6 Security requirements

The complete role and permission model is defined in [Roles and Permissions](Roles-and-Permissions.md).

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-OA-AUTH-B7A | OA | Mycelium Bloom shall provide a configuration interface for authentication mechanisms (JWT, OIDC, LDAP, SAML) when "the Organization Administrator accesses the authentication settings interface." | - |
| SSS-VW-AC-R7Y | All | Mycelium Bloom shall prevent all create, modify, and delete operations on model elements when "the authenticated user has the Viewer role." | - |
| SSS-VW-AC-VKZ | All | Mycelium Bloom shall restrict project and view access to only those projects the Viewer has been granted access to when "a Viewer attempts to open a project." | - |
| SSS-IA-SEC-P5G | All | Mycelium Bloom shall present the installation administration interface only to users with the Installation Administrator role when "a user navigates to the application." | - |

Additional security requirements (data-at-rest encryption, audit logging, vulnerability management): TBD.

### 5.7 Safety requirements

Not applicable. The Mycelium platform is a web-based engineering tool and does not perform safety-critical functions.

### 5.8 Reliability and availability requirements

TBD — Availability targets, data durability, backup and recovery requirements.

### 5.9 Quality requirements

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-PT-UI-YJL | All | Mycelium Bloom shall present commonly used features first and advanced capabilities on demand (progressive disclosure) when "a user interacts with any feature area of the application." | - |

Additional quality requirements (accessibility, internationalization, usability standards): TBD.

### 5.10 Design requirements and constraints

| ID | Roles | Requirement | Ref |
|----|-------|-------------|-----|
| SSS-CC-STD-UZA | All | The Mycelium platform shall implement the SysML v2 metamodel (OMG formal/25-09-03) as its native data model when "any model element is created, stored, or retrieved." | SysML 8 |
| SSS-CC-STD-1OO | All | The Mycelium platform shall support the Kernel Modelling Language (KerML) as the underlying formalism for SysML v2 when "model elements are validated, serialized, or interpreted." | KerML 7 |
| SSS-CC-WEB-3YC | All | Mycelium Bloom shall be accessible through modern web browsers without requiring desktop installation when "a user navigates to the application URL using a supported browser." | - |
| SSS-CC-WEB-6NX | All | Mycelium Bloom shall provide a responsive interface optimized for desktop browsers when "a user accesses the application from a desktop-resolution display." | - |

### 5.11 Software operations requirements

Operations requirements for the SaaS Platform Operator and on-premise IT operations are described in [Roles and Permissions](Roles-and-Permissions.md), section "Platform Scope."

The system monitoring dashboard capability is defined in SSS-OA-SYS-IEJ (section 5.2.1.15).

Additional operations requirements (deployment procedures, configuration management, upgrade procedures): TBD.

### 5.12 Software maintenance requirements

TBD — Supportability, update mechanisms, backward compatibility, data migration between versions.

### 5.13 System and software observability requirements

TBD — Logging, distributed tracing, metrics exposition, health check endpoints.

---

## 6. Verification, validation and system integration

### 6.1 Verification and validation process requirements

TBD — V&V approach, independence requirements, test environment specifications.

### 6.2 Validation approach

TBD — Validation strategy against the requirements baseline, mission-representative scenarios, acceptance criteria.

### 6.3 Validation requirements

TBD — Specific validation conditions and success criteria per requirement.

### 6.4 Verification requirements

TBD — Installation and acceptance requirements, delivery format, integration support.

---

## 7. System models

TBD — System context diagrams, component decomposition, interface diagrams. May reference SysML v2 model artifacts.

## 8. SysML v2 and KerML concept coverage matrix

This appendix tracks every KerML and SysML v2 modelling concept (metaclass) against the SSS requirements that cover it.

**Columns.**

- **Concept** — the KerML or SysML v2 metaclass name as used in the OMG specifications (KerML formal/25-09-01, SysML v2 formal/25-09-03).
- **Scope** — whether the concept is In scope for Mycelium, Deferred (planned beyond early-phase MBSE), or Out (not intended to be surfaced).
- **Abstract syntax** — SSS requirement identifiers that cover the metaclass at the *abstract-syntax* level: representation, persistence, query, creation, modification, deletion, and validation. `TBC` where coverage is missing or has not yet been verified.
- **UX / notation** — SSS requirement identifiers that cover the *user-facing surface area* for the concept: browsers, tabular views, diagrams and diagram notation per SysML v2 §8.2.3, dashboards, property editors, tooltips, and similar. `TBC` where coverage is missing or has not yet been verified.

A concept is fully covered only when both its *Abstract syntax* and *UX / notation* cells list at least one requirement. Splitting the two prevents a single generic UX requirement from implicitly painting the matrix green, while still recording notation coverage alongside the semantics. Multiple requirement IDs are comma-separated. Coverage is derived from the *KerML/SysML spec reference* column of each requirement table in §5 and, where that column is empty, from the requirement text.

### 8.1 KerML — Root

| Concept | Scope | Abstract syntax | UX / notation |
| --- | --- | --- | --- |
| Element | In | TBC | TBC |
| Relationship | In | SSS-PA-TRACE-8ZB, SSS-PA-TRACE-V8K | TBC |
| AnnotatingElement | In | TBC | TBC |
| Annotation | In | TBC | TBC |
| Comment | In | SSS-PA-CMT-R4K, SSS-PA-CMT-M6J, SSS-PA-CMT-T9F, SSS-PA-CMT-K2B, SSS-PA-CMT-D5P, SSS-PA-CMT-N8V | SSS-PA-VIS-F8Q, SSS-PA-VIS-B2M, SSS-PA-VIS-T1J, SSS-PA-VIS-G5R |
| Documentation | In | SSS-PA-CMT-W7N, SSS-PA-CMT-H3D, SSS-PA-CMT-M6J | TBC |
| TextualRepresentation | In | SSS-PA-CMT-Y6L | TBC |
| Namespace | In | SSS-PA-PKG-H6T | TBC |
| Membership | In | SSS-PA-PKG-H6T | TBC |
| OwningMembership | In | TBC | TBC |
| Import | In | SSS-PA-PKG-D4N | TBC |
| NamespaceImport | In | TBC | TBC |
| MembershipImport | In | TBC | TBC |
| AliasMembership | In | SSS-PA-PKG-Q1M | TBC |

### 8.2 KerML — Core

| Concept | Scope | Abstract syntax | UX / notation |
| --- | --- | --- | --- |
| Feature | In | TBC | TBC |
| Type | In | TBC | TBC |
| Classifier | In | TBC | TBC |
| Specialization | In | SSS-PA-ELEM-M4J, SSS-PA-ELEM-R6F | TBC |
| Subclassification | In | SSS-PA-ELEM-M4J | TBC |
| FeatureTyping | In | TBC | TBC |
| Subsetting | In | SSS-PA-ELEM-D2N | TBC |
| Redefinition | In | SSS-PA-ELEM-H9W | TBC |
| Conjugation | In | TBC | TBC |
| FeatureMembership | In | TBC | TBC |
| TypeFeaturing | In | SSS-PA-ARCH-N5W | TBC |
| FeatureChaining | In | TBC | TBC |
| FeatureInverting | Deferred | TBC | TBC |
| Multiplicity | In | SSS-PA-ELEM-V7K | TBC |
| MultiplicityRange | In | SSS-PA-ELEM-V7K | TBC |
| Package | In | SSS-PA-PKG-R8W, SSS-PA-PKG-V2J | TBC |
| LibraryPackage | In | TBC | TBC |
| Metaclass | In | TBC | TBC |
| MetadataFeature | In | SSS-PA-META-K7R | TBC |

### 8.3 SysML v2 — Structure (Part, Item, Attribute, Reference)

| Concept | Scope | Abstract syntax | UX / notation |
| --- | --- | --- | --- |
| PartDefinition | In | SSS-PA-ARCH-JQH, SSS-PA-ARCH-TB2, SSS-PA-ELEM-K4T, SSS-PA-ELEM-R8V, SSS-PA-ELEM-T2N, SSS-PA-ELEM-D7M, SSS-PA-ELEM-W4F | SSS-PA-VIS-W3T, SSS-PA-NAV-8IB, SSS-PA-NAV-G5X |
| PartUsage | In | SSS-PA-ARCH-JQH, SSS-PA-ARCH-TB2, SSS-PA-VAR-K3T, SSS-PT-CDS-I22 | SSS-PA-VIS-W3T, SSS-PA-VIS-M2K, SSS-PA-VIS-R3F, SSS-PT-CDS-I22 |
| ItemDefinition | In | SSS-PA-ARCH-B2D, SSS-PA-GLOSS-T5R | SSS-PA-GLOSS-K2W, SSS-PA-GLOSS-M3J, SSS-PA-GLOSS-V9D, SSS-PA-GLOSS-F6B |
| ItemUsage | In | SSS-PA-ARCH-B2D | TBC |
| AttributeDefinition | In | SSS-PA-ARCH-97Z, SSS-PA-QU-H2V, SSS-PA-QU-K6F, SSS-PA-CONST-N7K, SSS-PA-CONST-D3V, SSS-PA-CONST-W8F, SSS-PA-META-K7R, SSS-PA-GLOSS-T5R | SSS-PA-QU-T3K, SSS-PA-QU-R7N, SSS-PA-QU-W5J, SSS-PA-QU-D8M, SSS-PA-CONST-D3V, SSS-PA-CONST-J5M, SSS-PA-CONST-R2H |
| AttributeUsage | In | SSS-PA-ARCH-97Z, SSS-PT-DATA-I9M, SSS-PT-DATA-OH2, SSS-PT-DATA-492, SSS-PA-QU-N9X, SSS-PT-COLLAB-8U9, SSS-PT-PUB-K4W, SSS-PT-PUB-R7N, SSS-PT-PUB-H8J | SSS-PA-NAV-ZRW, SSS-PA-HIST-K3R, SSS-PA-HIST-T6W, SSS-PA-HIST-V2P |
| ReferenceUsage | In | TBC | TBC |
| EnumerationDefinition | In | SSS-PA-ARCH-9W5 | TBC |
| EnumerationUsage | In | SSS-PA-ARCH-9W5 | TBC |

### 8.4 SysML v2 — Ports, Interfaces, Connections, Flows

| Concept | Scope | Abstract syntax | UX / notation |
| --- | --- | --- | --- |
| PortDefinition | In | SSS-PA-ARCH-5RR | SSS-PA-VIS-W3T |
| PortUsage | In | SSS-PA-ARCH-5RR, SSS-PA-ARCH-K7M, SSS-PA-VAR-K3T | SSS-PA-VIS-W3T |
| ConjugatedPortDefinition | In | SSS-PA-ARCH-K7M | TBC |
| InterfaceDefinition | In | SSS-PA-ARCH-IGA | SSS-PA-VIS-Q7K |
| InterfaceUsage | In | SSS-PA-ARCH-IGA | SSS-PA-VIS-Q7K |
| ConnectionDefinition | In | SSS-PA-ARCH-IGA | SSS-PA-VIS-W3T, SSS-PA-VIS-G8N |
| ConnectionUsage | In | SSS-PA-ARCH-IGA, SSS-PA-ARCH-Y2D | SSS-PA-VIS-W3T, SSS-PA-VIS-G8N |
| FlowConnectionDefinition | In | SSS-PA-BEH-PC7 | SSS-PA-VIS-W3T, SSS-PA-VIS-G8N |
| FlowConnectionUsage | In | SSS-PA-BEH-PC7, SSS-PA-BEH-Q4N, SSS-PA-BEH-D6L, SSS-PA-BEH-X9V | SSS-PA-VIS-W3T, SSS-PA-VIS-G8N |

### 8.5 SysML v2 — Actions and Control

| Concept | Scope | Abstract syntax | UX / notation |
| --- | --- | --- | --- |
| ActionDefinition | In | SSS-PA-BEH-N5Z | SSS-PA-VIS-SMC, SSS-PA-VIS-E4R, SSS-PA-VIS-M1Z |
| ActionUsage | In | SSS-PA-BEH-N5Z, SSS-PA-BEH-WG5, SSS-PA-BEH-H83 | SSS-PA-VIS-SMC, SSS-PA-VIS-E4R, SSS-PA-VIS-J6N, SSS-PA-VIS-M1Z |
| AcceptActionUsage | In | TBC | TBC |
| SendActionUsage | In | TBC | TBC |
| AssignmentActionUsage | In | TBC | TBC |
| IfActionUsage | In | TBC | TBC |
| WhileLoopActionUsage | In | TBC | TBC |
| ForLoopActionUsage | In | TBC | TBC |
| PerformActionUsage | In | SSS-PA-BEH-H83 | TBC |
| TerminateActionUsage | Deferred | TBC | TBC |
| ControlNode | In | TBC | TBC |
| DecisionNode | In | TBC | TBC |
| MergeNode | In | TBC | TBC |
| ForkNode | In | TBC | TBC |
| JoinNode | In | TBC | TBC |

### 8.6 SysML v2 — States and Transitions

| Concept | Scope | Abstract syntax | UX / notation |
| --- | --- | --- | --- |
| StateDefinition | In | SSS-PA-BEH-RPK, SSS-PT-DATA-492 | SSS-PA-VIS-DP2, SSS-PA-VIS-B8V |
| StateUsage | In | SSS-PA-BEH-RPK, SSS-PA-BEH-H83 | SSS-PA-VIS-DP2, SSS-PA-VIS-B8V, SSS-PA-VIS-F2C |
| ExhibitStateUsage | In | SSS-PA-BEH-H83 | TBC |
| TransitionUsage | In | SSS-PA-BEH-RPK | SSS-PA-VIS-B8V |
| EntryAction | In | TBC | TBC |
| DoAction | In | TBC | TBC |
| ExitAction | In | TBC | TBC |

### 8.7 SysML v2 — Calculations and Constraints

| Concept | Scope | Abstract syntax | UX / notation |
| --- | --- | --- | --- |
| CalculationDefinition | In | SSS-PT-ANALYSIS-4W2 | TBC |
| CalculationUsage | In | SSS-PT-ANALYSIS-4W2 | TBC |
| ConstraintDefinition | In | SSS-PA-AV-LSX, SSS-PT-ANALYSIS-NWL | TBC |
| ConstraintUsage | In | SSS-PA-AV-LSX, SSS-PT-ANALYSIS-NWL, SSS-PT-ANALYSIS-EAJ, SSS-PA-SCRIPT-K8B | TBC |
| AssumeConstraintUsage | In | TBC | TBC |
| RequireConstraintUsage | In | TBC | TBC |

### 8.8 SysML v2 — Requirements and Concerns

| Concept | Scope | Abstract syntax | UX / notation |
| --- | --- | --- | --- |
| RequirementDefinition | In | SSS-PA-REQ-QP0, SSS-PA-REQ-WD0, SSS-PA-REQ-T8K, SSS-PA-REQ-M3N, SSS-PA-REQ-H6W | SSS-PA-VIS-C3D, SSS-PA-IE-B5W |
| RequirementUsage | In | SSS-PA-REQ-QP0, SSS-PA-REQ-WD0, SSS-PA-REQ-DS6, SSS-PA-REQ-T8K, SSS-PA-REQ-M3N, SSS-PA-REQ-H6W, SSS-PA-REQ-V4J, SSS-PA-REQ-W9B | SSS-PA-VIS-C3D, SSS-PA-IE-B5W |
| ConcernDefinition | In | SSS-PA-REQ-SUC | TBC |
| ConcernUsage | In | SSS-PA-REQ-SUC | TBC |
| StakeholderMembership | In | SSS-PA-REQ-H6W | TBC |
| ActorMembership | In | SSS-PA-REQ-M3N | TBC |
| SubjectMembership | In | SSS-PA-REQ-T8K | TBC |
| FramedConcernMembership | In | SSS-PA-REQ-SUC | TBC |
| RequirementConstraintMembership | In | SSS-PA-REQ-DS6 | TBC |
| RequirementVerificationMembership | In | SSS-PA-REQ-W9B | TBC |
| SatisfyRequirementUsage | In | SSS-PA-TRACE-Q72 | TBC |

### 8.9 SysML v2 — Cases (Use, Analysis, Verification)

| Concept | Scope | Abstract syntax | UX / notation |
| --- | --- | --- | --- |
| CaseDefinition | In | TBC | TBC |
| CaseUsage | In | TBC | TBC |
| UseCaseDefinition | In | SSS-PA-BEH-IX9 | TBC |
| UseCaseUsage | In | SSS-PA-BEH-IX9, SSS-PA-BEH-T7P, SSS-PA-BEH-J3F | TBC |
| IncludeUseCaseUsage | In | SSS-PA-BEH-T7P | TBC |
| ExtendUseCaseUsage | In | SSS-PA-BEH-J3F | TBC |
| AnalysisCaseDefinition | In | SSS-PA-AV-QII | TBC |
| AnalysisCaseUsage | In | SSS-PA-AV-QII | TBC |
| VerificationCaseDefinition | In | SSS-PA-AV-UCQ | TBC |
| VerificationCaseUsage | In | SSS-PA-AV-UCQ, SSS-PA-REQ-W9B | TBC |

### 8.10 SysML v2 — Views, Viewpoints, Rendering

| Concept | Scope | Abstract syntax | UX / notation |
| --- | --- | --- | --- |
| ViewDefinition | In | SSS-PA-VIS-T2V | SSS-PA-VIS-T2V, SSS-PA-VIS-BB9, SSS-PA-VIS-JPW |
| ViewUsage | In | SSS-PA-VIS-T2V | SSS-PA-VIS-T2V, SSS-PA-VIS-BB9, SSS-PA-VIS-JPW |
| ViewpointDefinition | In | SSS-PA-VIS-T2V | SSS-PA-VIS-T2V |
| ViewpointUsage | In | SSS-PA-VIS-T2V | SSS-PA-VIS-T2V |
| RenderingDefinition | In | TBC | TBC |
| RenderingUsage | In | TBC | TBC |
| ExposeMembership | In | SSS-PA-VIS-K9R | TBC |

### 8.11 SysML v2 — Metadata

| Concept | Scope | Abstract syntax | UX / notation |
| --- | --- | --- | --- |
| MetadataDefinition | In | SSS-PA-META-K7R, SSS-CC-EXT-OAW | SSS-PA-META-R9V |
| MetadataUsage | In | SSS-PA-META-W3D, SSS-PA-META-N8F, SSS-PA-META-H2T, SSS-PA-META-D5J, SSS-PA-META-T4K, SSS-PA-META-M6W, SSS-PA-META-J1B, SSS-PA-META-V8G, SSS-CC-EXT-OAW, SSS-PT-PUB-B9G | SSS-PA-META-R9V, SSS-PA-META-T4K, SSS-PA-META-M6W, SSS-PA-META-V8G, SSS-PA-VIS-B4F |

### 8.12 SysML v2 — Occurrences and Individuals

| Concept | Scope | Abstract syntax | UX / notation |
| --- | --- | --- | --- |
| OccurrenceDefinition | In | TBC | TBC |
| OccurrenceUsage | In | TBC | TBC |
| LifeClass | Deferred | TBC | TBC |
| IndividualDefinition | In | TBC | TBC |
| IndividualUsage | In | TBC | TBC |

### 8.13 SysML v2 — Allocations

| Concept | Scope | Abstract syntax | UX / notation |
| --- | --- | --- | --- |
| AllocationDefinition | In | SSS-PA-TRACE-YWQ | TBC |
| AllocationUsage | In | SSS-PA-TRACE-YWQ | TBC |

### 8.14 SysML v2 — Packaging, Imports, Variants

| Concept | Scope | Abstract syntax | UX / notation |
| --- | --- | --- | --- |
| Package (SysML) | In | SSS-PA-PKG-R8W, SSS-PA-PKG-V2J | TBC |
| LibraryPackage (SysML) | In | SSS-PA-QU-G1W, SSS-PA-IE-OYJ | TBC |
| FilteredImport | In | SSS-PA-PKG-J3W | TBC |
| VariantMembership | In | SSS-PA-VAR-R7W, SSS-PA-VAR-J9K, SSS-PA-VAR-F1P | TBC |
| VariationMembership | In | SSS-PA-VAR-R7W | TBC |

### 8.15 SysML v2 — Requirement / trace relationships

| Concept | Scope | Abstract syntax | UX / notation |
| --- | --- | --- | --- |
| Dependency | In | SSS-PA-TRACE-V8K | TBC |
| DeriveRequirementUsage | In | SSS-PA-REQ-V4J | TBC |