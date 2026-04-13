# Mycelium Roles and Permissions

This document defines the role and permission model for the Mycelium platform. It covers both the application-level roles managed by the authorization service and the model-level ownership mechanism implemented using SysML v2 metadata.

## Core Principles

1. **Separation of identity and model semantics.** User identity, authentication and role assignment are managed by an external authorization service (e.g. Keycloak). The SysML v2 model contains no `User` or `Role` concepts.
2. **Ownership is in the model.** Element-level access control is expressed as SysML v2 metadata (`Owner` MetadataDefinition referencing an `Ownership` Usage). The model server enforces ownership constraints at runtime.
3. **Roles define capabilities, ownership defines scope.** A role determines what kinds of operations a user may perform. An `Ownership` determines which model elements those operations apply to.
4. **Read access is universal.** All authenticated users with access to a project can read all model elements. Write access is governed by role and ownership.
5. **Lock-free collaboration.** No user can lock a model or part of a model to prevent other users from working. Concurrent modifications are resolved through the commit and merge workflow. An optimistic approach is used to modify elements, the last owner to change an owned element was right. When an owner changes elements that no longer exist, the model server will ignore these modifications gracefully.
6. **Self-service by default.** Authenticated users can create Organizations and Projects without requiring administrator intervention, following a model of self-service resource creation.

---

## Scope Hierarchy

The permission model operates across four scopes.

```
Installation
  |-- Installation Administrator (super-admin across all orgs and users)
  └── Account (any authenticated user)
       ├── Can create Organizations
       └── Organization (tenant boundary)
            ├── Organization Administrator (Owner)
            ├── Organization Member
            ├── Outside Collaborator (per-project only)
            └── Project (private | org-visible | public)
                 ├── Project Administrator
                 ├── Participant (scoped by Ownership)
                 ├── Viewer
                 └── Branch Protection Rules
```

---

## Platform Scope

### SaaS Deployment

On SaaS, the Platform scope governs multi-tenant infrastructure and self-service account management. The SaaS deployment is not available for organisations outside of Starion.

#### Platform Operator

The Platform Operator is an internal role held by the team operating the SaaS infrastructure (i.e. Starion or a designated operations team). This role is not available to customers.

| Capability | Description |
|-----------|-------------|
| Monitor platform health | View infrastructure metrics, logs and alerts across all tenants |
| Perform platform maintenance | Database maintenance, backups, upgrades and migrations |
| Configure platform defaults | Set default authentication policies, retention rules and compliance settings |
| Manage billing and quotas | Configure storage, user and project limits per organization |
| Suspend organizations | Suspend or deactivate organizations for policy violations or non-payment |

The Platform Operator cannot access model content within any organization. Platform operations are infrastructure-level, not model-level. Organization provisioning and team management are self-service operations performed by users through their Accounts — the Platform Operator does not create organizations or assign members on behalf of customers.

#### mycelium Account

Any user who registers on mycelium receives an Account. An Account exists independently of any Organization and provides self-service capabilities.

| Capability | Description |
|-----------|-------------|
| Create organizations | Create a new Organization and become its Organization Administrator |
| Update and Delete organizations | as Organization Administrator update and delete existing Organization |
| Accept organization invitations | Join an existing Organization as a Member when invited |
| View own memberships | See all Organizations the user belongs to and their role in each |
| Manage own profile | Update personal profile information and authentication credentials |

A user needs to register with a valid email, used for account verification, and needs to provide a platform unique username. A user can create and maintain a profile comprised of a bio, company, location (country, city, etc.), website, links to social media accounts (linked-in, x, facebook) and a gravatar.

### Installation Administrator

The Installation Administrator (IA) is a super-admin role that manages all users and organizations across the entire Mycelium installation. This role exists in both SaaS and on-premise deployments.

- **On SaaS:** The Installation Administrator maps to the Platform Operator role (Starion internal). It is not available to customers.
- **On on-premise:** The first user to complete the initial setup is automatically assigned the Installation Administrator role. The IA can grant the same role to other users.

| Capability | Description |
|-----------|-------------|
| View all organizations | Display all organizations on the installation with name, creation date, member count, project count, and status |
| Manage organizations | Create, update, suspend, reactivate, and delete organizations |
| View all users | Display all user accounts across all organizations with username, email, memberships, roles, and status |
| Manage users | Create, update, deactivate, and delete user accounts across all organizations |
| Assign org memberships | Add and remove users to and from any organization with a specified role |
| View installation metrics | Display total users, organizations, projects, storage usage, and active sessions |
| View audit log | Display an immutable log of all installation-wide administrative actions |

The Installation Administrator cannot access model content within any project. Installation-level operations are administrative, not model-level. If installation-level model access is required for audit purposes, this can be configured via the Organization Administrator's audit policy setting.

### On-Premise Deployment

In on-premise deployments the Platform scope does not exist within the application. The responsibilities that the SaaS Platform Operator handles are instead performed by the customer's IT operations team using external tooling:

| SaaS Platform Operator responsibility | On-premise equivalent |
|---------------------------------------|----------------------|
| Monitor platform health | Customer IT monitors via container orchestration dashboards, log aggregation and APM tools |
| Perform platform maintenance | Customer IT performs database backups, migrations and application upgrades via deployment pipelines |
| Configure platform defaults | Customer IT configures defaults via environment variables, configuration files or Helm values |
| Manage billing and quotas | Not applicable — the customer manages their own infrastructure capacity |
| Suspend organizations | Customer IT can deactivate organizations via direct database administration or CLI tooling |

The first user to complete the on-premise installation is assigned the Installation Administrator role and bootstraps the initial Organization (becoming its Organization Administrator). Additional Organizations can be created by any authenticated user, following the same self-service model as SaaS.

---

## Organization Scope

An Organization is the tenant boundary. On SaaS each paying customer is an Organization. On-premise deployments typically have one Organization, though larger institutions (e.g. ESA with multiple directorates) may configure several.

An Organization has two roles: Organization Administrator and Organization Member.

### Organization Administrator

The Organization Administrator is the owner of the Organization. The user who creates an Organization automatically becomes its Organization Administrator. Multiple users can hold this role. At least one Organization Administrator must exist per Organization.

| Capability | Description |
|-----------|-------------|
| Manage user accounts | Create, update, deactivate and delete user accounts within the organization |
| Invite users | Invite users to join the organization as Members |
| Manage organization-level roles | Assign and revoke Organization Administrator and Organization Member roles |
| Configure member permissions | Configure whether Organization Members can create projects (enabled by default) |
| Create projects | Create new projects with metadata and a default branch |
| Delete any project | Delete any project within the organization, with configurable deletion policies |
| Transfer organization ownership | Transfer the Organization Administrator role to another member |

The Organization Administrator does not automatically have Project Administrator access to all projects. Project-level access must be explicitly granted. The Organization Administrator can optionally configure a policy to grant themselves implicit read access to all projects within the organization for audit purposes.

### Organization Member

The Organization Member is a regular user within an Organization. Members can create and participate in projects.

| Capability | Description |
|-----------|-------------|
| Create projects | Create new projects and become their Project Administrator (if permitted by org settings) |
| View organization project list | See all projects they have been granted access to |
| Accept project invitations | Join a project when invited by a Project Administrator |
| View organization member list | See other members of the organization |
| Leave organization | Remove themselves from the organization |

Organization Members cannot:

- Manage other user accounts or roles
- Delete projects they did not create (unless they are that project's Project Administrator)
- Access projects they have not been invited to (unless project visibility permits it)

### Outside Collaborator

An Outside Collaborator is a user who has access to specific projects within an Organization but is not a member of that Organization. This enables cross-organizational collaboration — for example, granting a customer representative, external consultant or partner engineer access to a specific study project without giving them visibility into the rest of the organization.

| Capability | Description |
|-----------|-------------|
| Access granted projects | Participate in specific projects with an assigned project-level role |
| View project content | See model elements, diagrams and dashboards within granted projects |

Outside Collaborators:

- Are granted access by a Project Administrator of the specific project
- Can hold any project-level role except Project Administrator (i.e. Participant or Viewer)
- Cannot see other projects, organization members or organization settings
- Cannot create new projects within the organization
- Are visible to the Organization Administrator for audit purposes

---

## Project Scope

A Project is the container for a versioned SysML v2 model. Project-level roles govern what a user can do within a specific project. A user may hold different roles in different projects.

### Project Visibility

Projects have a visibility level, configured by the Project Administrator:

| Visibility | Description | Use case |
|------------|-------------|----------|
| **Private** | Only explicitly invited users can access the project | Default. Most engineering models are confidential |
| **Organization-visible** | All Organization Members can view the project in read-only mode; explicit invitation is required for write roles (Participant, Project Administrator) | Sharing reference models, completed study results or standard libraries within the organization |
| **Public** (SaaS only) | Any authenticated Account holder can view the project in read-only mode | Open-source model libraries, educational models, standard reference architectures (e.g. SysML v2 Quantities and Units) |

### Project Administrator

The Project Administrator is responsible for the structure, integrity and team composition of a project. This role is typically held by the study lead or chief systems engineer. The user who creates a project automatically becomes its Project Administrator.  Multiple users can hold this role. At least one Project Administrator must exist per Project.

| Capability | Description |
|-----------|-------------|
| Manage project settings | Update project name, description, applicable license, default branch and visibility |
| Manage project team | Invite, Add and remove users (including Outside Collaborators), assign project-level roles and Ownerships |
| Transfer project administration | Set and or Transfer the Project Administrator role to another team member |
| Manage Ownerships | Create, rename and remove Ownership Usages within the project package |
| Reassign element ownership | Change the Owner metadata on any model element to a different Ownership |
| Create model elements | Create any model element (becomes owner via assigned Ownership) |
| Modify any model element | Modify any model element regardless of ownership (ownership override) |
| Delete any model element | Delete any model element regardless of ownership (ownership override) |
| Create commits | Create commits on any branch |
| Create branches | Create new branches from any commit |
| Delete branches | Delete any branch except the default branch |
| Merge branches | Merge any branch into any other branch, including the default branch |
| Create tags | Create immutable tags on commits |
| Delete tags | Delete tags |
| Configure project mode | Set the project to Regular or Concurrent Design mode |
| Manage publication workflow | Initiate and manage publication cycles in Concurrent Design mode |
| Configure branch protection | Set branch protection rules on any branch |
| Designate reviewers | Assign Participants or Viewers as Reviewers for protected branches |

### Participant

The Participant is a subject matter specialist who creates, modifies and manages model elements within their assigned Ownership. A Participant may be assigned to one or more Ownerships within a project.

| Capability | Description |
|-----------|-------------|
| Create model elements | Create any model element; the element is automatically annotated with the Participant's active Ownership as Owner |
| Modify owned model elements | Modify model elements where the Owner metadata matches the Participant's assigned Ownership |
| Delete owned model elements | Delete model elements where the Owner metadata matches the Participant's assigned Ownership |
| Create commits | Create commits containing changes to owned elements on any branch (subject to branch protection rules) |
| Create branches | Create new branches from any commit |
| Delete own branches | Delete branches that the Participant created, provided they are not the default branch |
| Merge into non-default branches | Merge branches into non-default branches that the Participant created |
| Subscribe to parameters | Create ParameterSubscription metadata on AttributeUsages owned by other Ownerships |
| Publish values | Publish OwnedValues to the shared model during the publication workflow |
| View all model elements | Read any model element in any branch or commit |
| Execute queries | Create, save and execute queries against the model |
| View dashboards | Access validation, convergence and design driver dashboards |
| Review merges | Approve or request changes on merges to protected branches (when designated as a Reviewer) |

The Participant cannot:

- Modify or delete elements owned by another Ownership
- Merge into the default branch (unless branch protection rules permit it)
- Create or delete tags
- Reassign element ownership
- Manage project team membership or Ownerships

### Viewer

The Viewer is a non-editing observer of the model and its evolution. Typical Viewers include study leads without modelling responsibilities, customer representatives, quality assurance personnel and management.

| Capability | Description |
|-----------|-------------|
| View all model elements | Read any model element, diagram, view or dashboard in read-only mode |
| Browse model hierarchy | Navigate the model tree, search and filter elements |
| View diagrams and views | Open any diagram or view in read-only mode |
| Observe design sessions | Join concurrent design sessions as a non-editing observer |
| View version history | Browse commits, view diffs between commits, inspect historical model state |
| View dashboards | Access validation, convergence and design driver dashboards in read-only mode |
| Execute saved queries | Run previously saved queries in read-only mode |
| Export views and reports | Export diagrams, views and reports to standard formats (e.g. PDF, image) |
| Review merges | Approve or request changes on merges to protected branches (when designated as a Reviewer) |

The Viewer cannot:

- Create, modify or delete any model element
- Create commits, branches or tags
- Subscribe to parameters or publish values
- Modify project settings or team membership

---

## Branch Protection Rules

The Project Administrator can configure protection rules on any branch. Branch protection enforces quality gates and review workflows before changes are integrated.

| Rule | Description | Default |
|------|-------------|---------|
| Require ownership metadata | All committed elements must have an Owner annotation | Off |
| Require model validation pass | Commits to this branch must pass configured model validation rules (e.g. required attributes filled, no dangling references, constraint evaluations pass) before being accepted | Off |
| Restrict merge access | Only Project Administrators can merge into this branch | On (for default branch) |
| Require review | Merges require approval from at least N designated Reviewers before being accepted | Off |
| Restrict direct commits | Changes must be made on a separate branch and merged in; direct commits to the protected branch are not allowed | Off |

### Reviewer Designation

A Reviewer is not a separate role, it is a capability flag that any project participant can assign to another project participant for a specific branch. When a branch requires review:

- Designated Reviewers are notified when a merge is proposed
- Each Reviewer can approve or request changes
- The merge is blocked until the required number of approvals is reached
- This supports design review workflows where the study lead (Project Administrator) requires domain experts or stakeholders to sign off before changes enter the baseline

---

## Ownership

`Ownership` is the mechanism that provides fine-grained, element-level access control within a project. It is modeled as SysML v2 content within the project itself, or as an import from another project, which may be a library.

### Model Structure

The following SysML v2 elements are defined in the Concurrent Design library and instantiated per project:

| Element | Type | Location | Description |
|---------|------|----------|-------------|
| `Ownership` | ItemDefinition | Concurrent Design Library | Defines the concept with name, shortname and description |
| Ownership usages | ItemUsage | Project package | One usage per active Ownership in the project (e.g. "System", "Thermal", "Power") |
| `Owner` | MetadataDefinition | Concurrent Design Library | References an Ownership Usage; annotated onto model elements |
| `ParameterSubscription` | MetadataDefinition | Concurrent Design Library | Annotated onto AttributeUsages to indicate cross-domain parameter usage |
| `PublicationDefinition` | ItemDefinition | Concurrent Design Library | Records publication events with timestamp |
| `PublishedIn` | MetadataDefinition | Concurrent Design Library | Annotated onto AttributeUsages to record old vs new value and associated publication |
| `OwnedValue` | MetadataDefinition | Concurrent Design Library | Captures the value an owner provides prior to publication |

### Ownership Enforcement

When a user attempts to create, delete or modify a model element, the model server evaluates:

1. **User identity** — resolved via the authorization service
2. **Project role** — the user's role in this project (Project Administrator, Participant, Viewer)
3. **Ownership assignment** — which Ownership Usage(s) the user represents in this project
4. **Element ownership** — the Owner metadata on the target element

The enforcement rules are:

| Actor | Condition | Result |
|-------|-----------|--------|
| Project Administrator | Any element | Allowed |
| Participant | Owner metadata matches their Ownership | Allowed |
| Participant | Owner metadata does not match their Ownership | Denied |
| Participant | Element has no Owner metadata | Allowed (the Participant's Ownership is assigned as Owner upon modification) |
| Viewer | Any element | Denied |

### Concurrent Design Mode vs Regular Mode

Projects can operate in one of two modes, configured by the Project Administrator:

| Behavior | Regular Mode | Concurrent Design Mode |
|----------|-------------|------------------------|
| Ownership enforcement | Not applicable | Strict — violations are blocked by the model server |
| Publication workflow | Not applicable | Required — OwnedValues must be published before they overwrite shared attribute values |
| ParameterSubscription | Not applicable | Expected — the interface prompts users to subscribe to cross-domain inputs |
| Iteration tagging | Not applicable — tags can be created at any time | Expected — the interface supports structured iteration tagging for session tracking |

---

## Version Control Permissions

| Operation | Project Administrator | Participant | Viewer |
|-----------|:--------------------:|:-----------:|:------:|
| Read any branch or commit | Yes | Yes | Yes |
| Create commit | Yes | Yes (owned elements only) | No |
| Create branch | Yes | Yes | No |
| Delete branch (non-default) | Yes | Own branches only | No |
| Merge into default branch | Yes | No (unless branch protection permits) | No |
| Merge into non-default branch | Yes | Own branches only | No |
| Create tag | Yes | No | No |
| Delete tag | Yes | No | No |
| Diff commits | Yes | Yes | Yes |
| View historical commit | Yes | Yes | Yes |
| Approve merge (as Reviewer) | Yes | When designated | When designated |

---

## Role Assignment Rules

1. Every authenticated user has a mycelium Account, irrespective of the SaaS or on-premise setup.
2. A user may belong to multiple Organizations and hold a different organization-level role in each (Organization Administrator or Organization Member).
3. A user may hold exactly one project-level role per project (Project Administrator, Participant, or Viewer).
4. A user may be assigned to one or more Ownerships within a project.
5. When a Participant is assigned to multiple Ownerships, they may switch their active Ownership. The active Ownership determines which Owner metadata is applied to newly created elements.
6. The Organization Administrator role does not imply any project-level role. Project access must be explicitly granted.
7. Every project must have at least one Project Administrator at all times.
8. The Platform Operator role (SaaS only) does not grant access to any Organization or project content.
9. Outside Collaborators can hold Participant or Viewer roles but not Project Administrator.
10. The user who creates an Organization becomes its Organization Administrator. The user who creates a Project becomes its Project Administrator.
