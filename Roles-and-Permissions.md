# Mycelium Roles and Permissions

This document defines the role and permission model for the Mycelium platform. It covers both the application-level roles managed by the authorization service and the model-level ownership mechanism implemented using SysML v2 metadata.

## Mycelium Fabric Roles and Permissions

### Core Principles

1. **Separation of identity and model semantics.** User identity, authentication and role assignment are managed by an external authorization service (e.g. Keycloak). The SysML v2 model contains no `User` or `Role` concepts.
2. **Ownership is in the model.** Element-level access control is expressed as SysML v2 metadata (`Owner` MetadataDefinition referencing an `Ownership` Usage). The model server enforces ownership constraints at runtime.
3. **Roles define capabilities, ownership defines scope.** A role determines what kinds of operations a user may perform. An `Ownership` determines which model elements those operations apply to.
4. **Read access is universal.** All authenticated users with access to a project can read all model elements. Write access is governed by role and ownership.
5. **Lock-free collaboration.** No user can lock a model or part of a model to prevent other users from working. Concurrent modifications are resolved through the commit and merge workflow. An optimistic approach is used to modify elements, the last owner to change an owned element was right. When an owner changes elements that no longer exist, the model server will ignore these modifications gracefully.
6. **Self-service by default.** Authenticated users can create Organizations and Projects without requiring administrator intervention, following a model of self-service resource creation.

---

### Scope Hierarchy

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

### Platform Scope

#### SaaS Deployment

On SaaS, the Platform scope governs multi-tenant infrastructure and self-service account management. The SaaS deployment is not available for organisations outside of Starion.

##### Platform Operator

The Platform Operator is an internal role held by the team operating the SaaS infrastructure (i.e. Starion or a designated operations team). This role is not available to customers.

| Capability | Description |
|-----------|-------------|
| Monitor platform health | View infrastructure metrics, logs and alerts across all tenants |
| Perform platform maintenance | Database maintenance, backups, upgrades and migrations |
| Configure platform defaults | Set default authentication policies, retention rules and compliance settings |
| Manage billing and quotas | Configure storage, user and project limits per organization |
| Suspend organizations | Suspend or deactivate organizations for policy violations or non-payment |

The Platform Operator cannot access model content within any organization. Platform operations are infrastructure-level, not model-level. Organization provisioning and team management are self-service operations performed by users through their Accounts — the Platform Operator does not create organizations or assign members on behalf of customers.

##### mycelium Account

Any user who registers on mycelium receives an Account. An Account exists independently of any Organization and provides self-service capabilities.

| Capability | Description |
|-----------|-------------|
| Create organizations | Create a new Organization and become its Organization Administrator |
| Update and Delete organizations | as Organization Administrator update and delete existing Organization |
| Accept organization invitations | Join an existing Organization as a Member when invited |
| View own memberships | See all Organizations the user belongs to and their role in each |
| Manage own profile | Update personal profile information and authentication credentials |

A user needs to register with a valid email, used for account verification, and needs to provide a platform unique username. A user can create and maintain a profile comprised of a bio, company, location (country, city, etc.), website, links to social media accounts (linked-in, x, facebook) and a gravatar.

#### Installation Administrator

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

#### On-Premise Deployment

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

### Organization Scope

An Organization is the tenant boundary. On SaaS each paying customer is an Organization. On-premise deployments typically have one Organization, though larger institutions (e.g. ESA with multiple directorates) may configure several.

An Organization has two roles: Organization Administrator and Organization Member.

#### Organization Administrator

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

#### Organization Member

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

#### Outside Collaborator

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

### Project Scope

A Project is the container for a versioned SysML v2 model. Project-level roles govern what a user can do within a specific project. A user may hold different roles in different projects.

#### Project Visibility

Projects have a visibility level, configured by the Project Administrator:

| Visibility | Description | Use case |
|------------|-------------|----------|
| **Private** | Only explicitly invited users can access the project | Default. Most engineering models are confidential |
| **Organization-visible** | All Organization Members can view the project in read-only mode; explicit invitation is required for write roles (Participant, Project Administrator) | Sharing reference models, completed study results or standard libraries within the organization |
| **Public** (SaaS only) | Any authenticated Account holder can view the project in read-only mode | Open-source model libraries, educational models, standard reference architectures (e.g. SysML v2 Quantities and Units) |

#### Project Administrator

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

#### Participant

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

#### Viewer

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

### Branch Protection Rules

The Project Administrator can configure protection rules on any branch. Branch protection enforces quality gates and review workflows before changes are integrated.

| Rule | Description | Default |
|------|-------------|---------|
| Require ownership metadata | All committed elements must have an Owner annotation | Off |
| Require model validation pass | Commits to this branch must pass configured model validation rules (e.g. required attributes filled, no dangling references, constraint evaluations pass) before being accepted | Off |
| Restrict merge access | Only Project Administrators can merge into this branch | On (for default branch) |
| Require review | Merges require approval from at least N designated Reviewers before being accepted | Off |
| Restrict direct commits | Changes must be made on a separate branch and merged in; direct commits to the protected branch are not allowed | Off |

#### Reviewer Designation

A Reviewer is not a separate role, it is a capability flag that any project participant can assign to another project participant for a specific branch. When a branch requires review:

- Designated Reviewers are notified when a merge is proposed
- Each Reviewer can approve or request changes
- The merge is blocked until the required number of approvals is reached
- This supports design review workflows where the study lead (Project Administrator) requires domain experts or stakeholders to sign off before changes enter the baseline

---

### Ownership

`Ownership` is the mechanism that provides fine-grained, element-level access control within a project. It is modeled as SysML v2 content within the project itself, or as an import from another project, which may be a library.

#### Model Structure

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

#### Ownership Enforcement

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

#### Concurrent Design Mode vs Regular Mode

Projects can operate in one of two modes, configured by the Project Administrator:

| Behavior | Regular Mode | Concurrent Design Mode |
|----------|-------------|------------------------|
| Ownership enforcement | Not applicable | Strict — violations are blocked by the model server |
| Publication workflow | Not applicable | Required — OwnedValues must be published before they overwrite shared attribute values |
| ParameterSubscription | Not applicable | Expected — the interface prompts users to subscribe to cross-domain inputs |
| Iteration tagging | Not applicable — tags can be created at any time | Expected — the interface supports structured iteration tagging for session tracking |

---

### Version Control Permissions

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

### Role Assignment Rules

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


## Mycelium Forge Roles and Permissions

This document defines the role and permission model for **Mycelium Forge**, the MBSE artefact sharing platform. It is a working document: it is the authority on the intended model while it is being agreed, after which the settled parts move into `design.md` §13 and into SSS requirements.

Forge is a **registry**: a scope holds packages, and a package holds immutable versions (`design.md` §8).

| Concept | Definition | Identity |
|---|---|---|
| **Scope** | Resolves to an Account or Organization slug | `@starion` |
| **Package** | The container for the versions of one artefact | `@starion/ECSS-MM-THE` |
| **PackageVersion** | Immutable once published | `@starion/ECSS-MM-THE 1.4.2` |

There is no container between Scope and Package. `SSS-FG-AUTH-S2B` fixes the identifier at `@<scope>/<package-name>`, which leaves no third segment, and every `usage[]` IRI in an already-published kpar depends on that shape.

---

### Core principles

1. **Forge owns its identity registry.** Per DD-20 Forge ships its own OIDC provider (Keycloak) and does not depend on Fabric's authentication or authorization. An external identity provider may be federated as configuration, but Forge is deployable without one.
2. **Roles define capabilities; visibility defines reach.** A role determines which operations a principal may perform on a package. Whether a principal can *see* the package at all is governed by the package's visibility, not by a role assignment.
3. **Self-service by default.** Authenticated users create Organizations and publish packages without administrator intervention.
4. **Principals are not only people.** An API key is a first-class principal with its own authority, because CI/CD pipelines may publish (`SSS-FG-REG-Y2L`).
5. **Immutability is not a permission.** No role can edit a published version. `SSS-FG-REG-I3C` freezes `{package, version}`, and `SSS-FG-AUTH-M3C` freezes metadata at publish time. Correction means publishing a new version.

---

### Principals

| Principal | Description | Authentication |
|---|---|---|
| **Anonymous** | An unauthenticated visitor or crawler | None. `SSS-FG-REG-W9J` requires the web interface to be reachable unauthenticated, and DD-01 depends on public pages being crawlable and CDN-cacheable |
| **Account** | A registered person. Exists at installation level, independently of any Organization | OIDC against Forge's own provider (DD-20) |
| **API key** | A machine credential issued by an Account, acting with that Account's authority narrowed to an explicit set of operations | Bearer credential, hashed at rest, revealed once at issuance (`SSS-FG-REG-Y2L`) |

An API key never exceeds the authority of the Account that issued it. Where the two differ, the narrower applies. Revoking an Account's access revokes every key it issued.

---

### Scope hierarchy

```
Installation
  ├── Installation Administrator (super-admin; seeded from configuration)
  ├── Platform Operator (SaaS only; infrastructure, no package content)
  └── Account (any authenticated user)
       ├── owns a personal Scope           →  @alice/…
       ├── can create Organizations
       └── Organization (tenant boundary)
            ├── Organization Administrator
            ├── Organization Member
            └── owns an organization Scope →  @starion/…

Scope (@alice or @starion)
  └── Package  (private | organization-visible | public)
       ├── Owner
       ├── Maintainer
       ├── Reader        (meaningful only where visibility restricts)
       └── PackageVersion (immutable)
```

**An Account is a namespace in its own right.** `design.md` §8.2 resolves a scope to an Account *or* an Organization slug, and §8's model has `Account "1" --> "0..1" Scope`. An individual publishes to `@alice/…` without belonging to any Organization.

---

### Installation scope

#### Installation Administrator

The Installation Administrator is a super-admin over the whole installation. It exists in both SaaS and on-premise deployments.

**Bootstrap is from configuration, not from whoever arrives first.** Per DD-20 and `F1-05`, the seeded administrator is supplied as deployment configuration.

| Capability | Description |
|---|---|
| View all organizations | Name, creation date, member count, package count, status |
| Manage organizations | Create, suspend, reactivate and delete organizations |
| View all accounts | Username, email, memberships, roles, status |
| Manage accounts | Deactivate and delete accounts; grant and revoke the Installation Administrator role |
| Assign organization memberships | Add and remove accounts to and from any organization with a specified role |
| Reserve and release scope slugs | Including refusing a slug that collides with a proxied upstream scope (§5.1.2, `F1-06`) |
| Configure mirroring | Scope routing to an upstream, upstream credentials, bulk pre-warm, air-gapped bundle import and export (§5.1, DD-16) |
| View installation metrics | Accounts, organizations, packages, storage usage, active sessions |
| View the audit log | The append-only, tamper-evident record of privileged operations (`SSS-FG-AUTH-R9J`) |

**The Installation Administrator does not gain read access to private packages by virtue of the role.** Administration is over accounts, organizations and the installation, not over package content. Where an operator genuinely needs content access — a legal hold, an incident — it is an explicit, audited grant rather than an ambient capability.

#### Platform Operator — SaaS only

The Platform Operator is held by the team operating the SaaS infrastructure. It is not available to customers, and the SaaS deployment is not offered to organisations outside Starion.

| Capability | Description |
|---|---|
| Monitor platform health | Infrastructure metrics, logs and alerts across all tenants |
| Perform platform maintenance | Backups, upgrades, schema migrations (DD-18) |
| Configure platform defaults | Authentication policy, retention, compliance settings |
| Manage billing and quotas | Storage, account and package limits per organization |
| Suspend organizations | For policy violation or non-payment |

**Platform Operator and Installation Administrator are distinct roles, not two names for one.** The Platform Operator acts on infrastructure and never on package content or account records; the Installation Administrator acts on accounts, organizations and scopes and never on infrastructure.

#### On-premise deployment

The Platform scope does not exist inside the application on-premise. Its responsibilities are the customer IT function's:

| SaaS Platform Operator responsibility | On-premise equivalent |
|---|---|
| Monitor platform health | Container orchestration dashboards, log aggregation, APM |
| Perform platform maintenance | Backups and migrations via deployment pipelines; the migrator is an explicit invocation (DD-18) |
| Configure platform defaults | Environment variables, configuration files or Helm values |
| Manage billing and quotas | Not applicable — the customer manages its own capacity |
| Suspend organizations | Installation Administrator, in the application |

---

### Organization scope

An Organization is the tenant boundary and owns a Scope. On SaaS each paying customer is an Organization.

#### Organization Administrator

The Account that creates an Organization becomes its Administrator. Multiple Accounts may hold the role, and **at least one must exist at all times**.

| Capability | Description |
|---|---|
| Manage organization settings | Display name, description, profile |
| Invite and remove members | Invitations are accepted, not imposed |
| Manage organization roles | Assign and revoke Administrator and Member |
| Configure publishing policy | Whether Members may create new packages in the organization scope (enabled by default) |
| Transfer administration | Transfer the role to another member, on that member's acceptance |
| Delete any package in the scope | Subject to the deletion policy below |
| Configure default package visibility | The visibility new packages receive unless overridden |

**The Organization Administrator does not automatically hold a package role.** Package access is granted per package. The role may optionally be configured to carry implicit *read* access across the organization's packages for audit purposes; it never carries implicit write.

**An Organization does not own its members' Accounts.** An Account exists at installation level and is provisioned on first login (DD-20, `F1-05`). An Organization controls membership — who belongs and with what role — not existence. Removing a member from an Organization does not deactivate their Account, and no Organization Administrator can create or delete one.

#### Organization Member

| Capability | Description |
|---|---|
| Publish to the organization scope | Creating a new package where policy permits, becoming its Owner |
| View the organization package list | Public, organization-visible, and private packages the member holds a role on |
| View the member list | Other members of the organization |
| Accept package invitations | Take up an Owner, Maintainer or Reader role when granted |
| Leave the organization | Subject to the Owner invariant below |

Members cannot manage roles or memberships, delete packages they do not own, or read private packages they hold no role on.

---

### Package scope

A Package is the container for versions of a kpar or other MBSE artefact (§9). It is the unit of visibility, ownership and collaboration.

#### Visibility

Visibility is an attribute of the Package, set by an Owner.

| Visibility | Who may read | Use |
|---|---|---|
| **Private** | Only principals holding an explicit role on the package | Default. Most MBSE artefacts are confidential |
| **Organization-visible** | All members of the owning Organization, read-only; write requires an explicit role | Sharing within the organisation |
| **Public** | **Anyone, including unauthenticated visitors and crawlers** | Publishing to the community |

**Public means anonymous.**  `SSS-FG-REG-W9J` requires unauthenticated reach, and DD-01 and §7.2 rest on public pages being linkable, crawlable and cacheable at a CDN.

**Visibility ships in the first release.** It is not a later addition: search (`E-02`), qualified-name resolution (`E-03`), artefact serving (`C-01`) and mirror replication (§5.1.6) each carry an authorisation dimension from the outset, and `A-01`'s baseline schema carries the attribute.

**New packages are private by default.** An Organization Administrator may set a different default for their organisation; the installation default is private. The two failure modes are not symmetric — an accidental publication cannot be recalled once crawlers, CDN edges, mirrors and downstream copies have taken it, whereas an accidentally private package is corrected in one action.

**Private and organization-visible artefacts are not cached at a CDN.** DD-22 sets `Cache-Control: public, max-age=31536000, immutable` on artefact responses, which is correct only for public packages: the artefact URL is `@scope/name/version/artifact` and therefore guessable, so a shared edge would serve private bytes to anyone who asked for them. Non-public artefacts are served from origin under `Cache-Control: private, no-store`. DD-22's economics are unaffected — its argument rests on *popular* artefacts absorbing origin load, and a non-public package has a small, known audience by construction. The content hash remains the `ETag` on both paths.

**Visibility and unlisting are orthogonal, not two points on one scale.** `SSS-FG-REG-U4D` unlisting hides a version from search and resolution while *still serving direct downloads*; it is a deprecation signal. A package may be public-and-unlisted or private-and-listed. Conflating the two is the likeliest implementation error in this area.

#### Owner

The Account that first publishes a package name becomes its Owner. Multiple Owners may exist.

| Capability | Description |
|---|---|
| Publish a version | Subject to `SSS-FG-REG-S2B` monotonic SemVer and `I3C` immutability |
| Unlist and relist a version | `SSS-FG-REG-U4D` |
| Set visibility | Private, organization-visible or public |
| Manage the package team | Grant and revoke Owner, Maintainer and Reader |
| Transfer ownership | Effective only on the recipient's explicit acceptance (`SSS-FG-AUTH-T5E`) |
| Manage package settings | Description, licence, links — within the limits of frozen metadata (`M3C`) |
| Delete the package | Subject to the deletion policy below |

**A package always retains at least one individual-Account Owner** (`SSS-FG-AUTH-O4D`). An Organization may hold ownership, but an Organization Owner alone does not satisfy the invariant (`P7G`). Any operation that would leave a package without an individual Owner — the last Owner leaving, being removed, or the Organization being deleted — is refused, not silently repaired.

#### Maintainer

| Capability | Description |
|---|---|
| Publish a version | As Owner |
| Unlist and relist a version | As Owner |
| Read the package | Regardless of visibility |

A Maintainer cannot change visibility, alter the team, transfer ownership or delete the package.

#### Reader

An explicit read grant on a package whose visibility would otherwise exclude the principal. **A Reader role on a public package is meaningless and should not be assignable** — public packages are readable by everyone, including anonymous visitors, so the grant would express nothing.

| Capability | Description |
|---|---|
| Read package metadata and versions | Manifest, version list, dependency graph (`SSS-FG-REG-M8H`) |
| Download artefacts | `SSS-FG-REG-D6F` |

#### Anonymous and unauthenticated access

An anonymous visitor may read metadata for, search, resolve names within, and download artefacts from **public packages only** (`SSS-FG-REG-W9J`, `F1-04`). Private and organization-visible packages are absent from search results and from qualified-name resolution, and are indistinguishable from packages that do not exist — see *How visibility propagates*.

#### Publishing authority

Publishing is authorised against the **scope**, not inherited from the Organization role (§8.2, `B-03`, `SSS-FG-AUTH-G6F`):

| Case | Who may publish |
|---|---|
| A new package in a personal scope `@alice/…` | The Account owning that scope, or one of its API keys |
| A new package in an organization scope `@starion/…` | An Organization Administrator, or a Member where the organization's publishing policy permits |
| A new version of an existing package | An Owner or Maintainer of that package |

The scope is **declared at publish time and authorised**, never derived from the credential, because an Account may hold publishing rights in several scopes and must be able to say which one a publication targets.

#### Deletion and erasure

**A published version is never hard-deleted by a user.** `SSS-FG-REG-U4D` unlisting is the only withdrawal available: the version leaves search and resolution and continues to serve direct downloads. This follows from `I3C` immutability and from the `usage[]` IRIs that point at published versions — a hard delete breaks resolution permanently and silently for every dependant, which is precisely the failure §8.2's hash fallback exists to survive.

| Action | Who | Condition |
|---|---|---|
| Unlist or relist a version | Owner, Maintainer | Always available |
| Delete a package | Owner; Organization Administrator of the owning scope | Only while no version has been downloaded and no dependants exist — DD-19's `usage[]` graph supplies the check. Otherwise the operation degrades to unlisting every version, and says so rather than failing silently |
| Erase a package or version | Installation Administrator | Audited. Reserved for accidental disclosure of confidential material, or a lawful erasure request |

Erasure is deliberately an administrator operation rather than a self-service one. It is the escape hatch every registry eventually needs — a credential committed into an artefact, a model published from the wrong scope — and making it self-service turns a rare, considered act into an ordinary button.

Destructive actions are confirmed on their own page requiring the package name to be typed (§7.4, `G-07`) and are recorded in the audit log (`SSS-FG-AUTH-R9J`).

---

### API keys

| Capability | Held by |
|---|---|
| Issue an API key | Any Account, for itself |
| Scope a key to operations | The issuing Account, at issuance — publish, unlist, read |
| Revoke a key | The issuing Account; the Installation Administrator for any key |
| List own keys | The issuing Account — metadata and prefix only, never the secret |

A key's secret is displayed once, at issuance, and is stored only as a hash (`SSS-FG-REG-Y2L`, `F1-02`). A key is not a principal that can be granted package roles in its own right; it derives every permission from its issuing Account at the time of use.

---

### Role assignment rules

1. Every authenticated user has an Account, in both SaaS and on-premise deployments.
2. An Account exists at installation level and is not owned by any Organization.
3. An Account may belong to multiple Organizations and hold a different role in each.
4. An Account holds at most one role per package: Owner, Maintainer or Reader.
5. The Organization Administrator role implies no package role. Package access is granted per package.
6. Every package retains at least one individual-Account Owner at all times (`SSS-FG-AUTH-O4D`, `P7G`).
7. Every Organization retains at least one Organization Administrator at all times.
8. The Account creating an Organization becomes its Administrator; the Account first publishing a package name becomes its Owner.
9. Ownership transfer, organization invitations and package role grants take effect only on the recipient's explicit acceptance (`SSS-FG-AUTH-T5E`).
10. The Platform Operator role grants no access to any package content or account record.
11. An API key never exceeds the authority of its issuing Account.

---

### How visibility propagates

Visibility is an attribute of the Package, but four mechanisms elsewhere in the design read package content and must honour it. Each is recorded here because each was specified before visibility existed, and each would otherwise default to the pre-visibility behaviour.

#### Search and qualified-name resolution

Both filter to what the requester may read. **A requester cannot distinguish "does not exist" from "exists but is not yours"** — package lookup (`D-02`) and qualified-name resolution (`E-03`) return the same response in either case, because the existence of a private package name may itself be sensitive for a defence or space programme.

One residual oracle is accepted rather than concealed: publishing to a name already taken privately within the same scope must fail, and that failure reveals the name is taken. It is bounded — names are per-scope, so `@alice/foo` does not block `@bob/foo`, and a principal publishing into a scope generally holds a role in it.

#### The content-hash fallback

§8.2 permits serving a byte-identical copy from another scope when the declared version cannot be served. That candidate set is **restricted to artefacts the requester is authorised to read**. Where the only byte-identical copies are invisible to the requester, resolution fails exactly as if none existed.

The filter is on the **requester's** authorisation, not on the artefact's visibility. Filtering the other way would make identical content an oracle for the existence of private packages. §8.2 also has Forge report the substitution to the caller; that report names only a scope the caller can already see.

This degrades availability, which §8.2 anticipates when it calls the mechanism "an availability fallback, not a resolution rule".

#### Mirror replication

A mirror replicates exactly what its upstream credential (§5.1.7) is entitled to read. §5.1.6's promise that a mirror searches the whole upstream catalogue therefore reads: **the whole upstream catalogue visible to this installation's credential**. A mirror configured with an anonymous or public-scope credential replicates public packages only; an organisation mirroring its own private packages to an on-premise instance supplies a credential that can see them. Proxied scopes remain read-only (`P4-02`).

#### Artefact caching

Non-public artefacts bypass the CDN — see *Visibility*, above.

---

### Verified publishers — deferred

`design.md` §13.1 defers verified publishers beyond the first release, scoped to publisher identity. Two things are settled in advance so the deferral does not become a design gap:

- **It is an attribute of the Scope, not a role.** Verification asserts that a namespace is who it claims to be — that `@esa` is the European Space Agency. It grants no capability, so modelling it as a role would misrepresent it.
- **It needs a granting authority.** The Installation Administrator on-premise; Starion on SaaS. DD-20 notes there is no external authority to vouch for a scope, so the grant is an operational act with an audit entry, not an automated check.

---