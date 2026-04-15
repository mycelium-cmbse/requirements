# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This repository does not contain source-code. It contains reference documentation for CDP4-COMET and SysML version 2 and requirements for the mycelium platform.

mycelium is a next-generation web-based SysML v2 Model-Based Systems Engineering (MBSE) platform, evolving from the existing CDP4-COMET collaborative model based engineering tool. It targets early-phase system design and concurrent design processes, replacing CDP4-COMET's desktop-first approach with a cloud-native, web-first architecture. The components that make up mycelium are:
  - **Mycelium Bloom**: the web application that exposes the user front end.
  - **Mycelium Fabric**:  the server backend
  - **Mycelium Forge**: package manager

Key differentiators from CDP4-COMET:
- SysML v2 native (replacing ECSS-E-TM-10-25 data model)
- Cloud-native web-first (ASP.NET Blazor frontend instead of WPF desktop)
- Self-service organization and project creation (SaaS and on-premise)

## Architecture

The system has five main components:

1. **C# SDK** - Auto-generated C# classes/interfaces mirroring the SysML v2 metamodel. Handles JSON/XMI/MessagePack serialization and provides a REST HTTP client for communicating with the model server.

2. **Model Server (Mycelium Fabric)** - ASP.NET backend using the CarterCommunity API framework. Implements the OMG Systems Modelling API and Services specification. Stateless horizontally scalable service with PostgreSQL as the source of truth (auto-generated schemas from the SysML v2 metamodel). Concurrency handled by Git-style commit-based optimistic concurrency at the database layer, not by an in-memory actor framework. Real-time notifications via SignalR backed by a RabbitMQ backplane. Background jobs via `IHostedService` and RabbitMQ. Authentication via Keycloak supporting JWT, OIDC, LDAP, SAML.

3. **Web Application (Mycelium Bloom)** - ASP.NET Blazor frontend (full-stack C#). Includes model browser, diagram editors, property editors, validation dashboards, and collaborative views. Real-time notifications via SignalR. Selectively exposes SysML v2 concepts relevant to early-phase design.

4. **Package Registry (Mycelium Forge)** - Registry for SysML v2 model libraries and reusable packages. Hosts standard libraries (Quantities and Units, standard view definitions) and handles publishing, discovery, versioning, and download of SysML v2 packages. Runs alongside Fabric in the same container environment; Project Administrators import Forge packages into their projects via Bloom, after which Fabric makes them available for querying and specialization.

5. **ECSS-E-TM-10-25 to SysML v2 Converter** - Migration tool for existing CDP4-COMET models.

## Technology Stack

- **Language:** C#
- **Backend:** ASP.NET, CarterCommunity
- **Frontend:** ASP.NET Blazor
- **Real-time:** SignalR
- **Messaging:** RabbitMQ (via Mercurio)
- **Serialization:** JSON, XMI, MessagePack
- **Database:** PostgreSQL (auto-generated schemas from SysML v2 UML model)
- **Auth:** Keycloak (JWT, OIDC, LDAP, SAML)
- **Deployment:** Linux containers (Docker/Kubernetes); Fabric and Forge share the same runtime stack (.NET 10, PostgreSQL, Keycloak, RabbitMQ)

## Standards Compliance

- Kernel Modelling Language (KerML)
- SysML v2 (OMG formal/25-09-03)
- OMG Systems Modelling API and Services (formal/25-09-04)

## Repository Structure

- `proposal/` - ESA proposal documents (technical and implementation)
- `cdp4-comet/` - CDP4-COMET user manual reference
- `Reference Documentation/` - SysML v2 and KerML specification documents (textified PDFs)
- `Software-System-Specification.md` - Software System Specification (SSS) per ECSS-E-ST-40C Annex B — customer requirements, reviewed at SRR
- `Software-Requirements-Specification.md` - Software Requirements Specification (SRS) per ECSS-E-ST-40C Annex D — supplier's detailed specification, reviewed at PDR
- `Roles-and-Permissions.md` - Role and permission model (referenced by the SSS, contributes to SSS section 5.6 Security and SSS section 5.9 Quality)

## Domain Context

- **CDP4-COMET**: The predecessor tool built on ECSS-E-TM-10-25, used for 20+ years in concurrent design (especially in ESA/space domain)
- **SysML v2**: The OMG systems modeling language standard that Mycelium natively implements
- **KerML**: The kernel modeling language underlying SysML v2
- **ECSS-E-TM-10-25**: European space standards technical memorandum that CDP4-COMET's data model is based on
- **SysML2.NET**: Existing C# implementation of SysML v2 by Starion (https://github.com/STARIONGROUP/SysML2.NET), serves as foundation for the SDK

## Roles and Terminology

The platform uses a hierarchical role model. Use the correct terminology consistently:

- **Installation Administrator** - Super-admin managing all users and organizations across the entire installation; on SaaS maps to Platform Operator, on on-premise assigned to the first user at setup
- **Account** - An authenticated user on the Mycelium platform
- **Organization Administrator** - Owner of an Organization; manages users, auth, security within the org
- **Organization Member** - Regular user within an Organization; can create projects if permitted
- **Outside Collaborator** - User with access to specific projects without org membership
- **Project Administrator** - Owner of a Project; manages structure, team, branches, tags, merges
- **Participant** - Subject matter specialist who creates/modifies model elements within their assigned Ownership
- **Viewer** - Read-only observer of the model

**Ownership** (not "Domain of Expertise") is the SysML v2 metadata based  mechanism in mycelium for element-level access control. It is modeled as an `Owner` MetadataDefinition referencing an `Ownership` ItemUsage.

## Key Design Principles

- The web application focuses on exposing SysML v2 concepts relevant to early-phase design, while supporting future extension to a comprehensive SysML v2 implementation
- Lock-free collaboration: no user can lock a model or part of a model; optimistic concurrency via commit and merge
- Ownership-based access control: element-level permissions expressed as SysML v2 metadata in the model itself
- Near real-time model change notifications for all connected users via SignalR
- Self-service: users can create Organizations and Projects without administrator intervention
- Support for both SaaS (multi-tenant) and on-premise (single-tenant) deployment
- Projects can operate in Regular mode or Concurrent Design mode (strict ownership enforcement, publication workflow)

## Requirements Writing Conventions

Requirements use the form:
> **\<Component\> shall** \<active verb\> **when** "\<condition\>"

Where component is: Mycelium Bloom (frontend), Mycelium Fabric (backend), or Mycelium Forge (package registry).

- Every verb must describe something the software actively does (renders, creates, displays, enforces, prevents, delivers, persists, imports, exports)
- Do not use "shall allow" — it is satisfied by not objecting
- Use "shall provide operations to", "shall provide an interface for", "shall support", "shall display", "shall enforce", "shall prevent" instead
- SSS requirement IDs follow the convention `SSS-<role>-<area>-<number>` with role prefixes: OA (Organization Administrator), PA (Project Administrator), PT (Participant), VW (Viewer), CC (Cross-Cutting), FB (Fabric), FG (Forge)
- SRS requirement IDs follow the convention `SRS-<area>-<number>`
- Documents follow ECSS-E-ST-40C Rev.1: SSS (Annex B) for customer requirements reviewed at SRR, SRS (Annex D) for supplier requirements reviewed at PDR
- The SSS is organized by ECSS Annex B DRD structure: sections 1-4 (introduction/context), section 5 (specific requirements with capabilities organized by component: 5.2.1 Bloom, 5.2.2 Fabric, 5.2.3 Forge), sections 5.3-5.13 (non-functional requirements), section 6 (V&V), section 7 (system models)
