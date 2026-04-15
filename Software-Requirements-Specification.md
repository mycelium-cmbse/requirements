# Mycelium Software Requirements Specification (SRS)

This document defines the software requirements specification for the Mycelium platform in accordance with ECSS-E-ST-40C Rev.1 Annex D. It is the supplier's (Starion) response to the Software System Specification (SSS) and describes the functional and non-functional requirements applicable to the Mycelium software items. Together with the Interface Control Document (ICD), it constitutes a major part of the Technical Specification (TS).

Each requirement uses the form:

> **\<Component\> shall** \<active verb\> **when** "\<condition\>"

Where component is one of: Mycelium Bloom, Mycelium Fabric, or Mycelium Forge.

Requirements are identified using the convention `SRS-<area>-<alpha-numeric>`.

This document is the primary input for the Preliminary Design Review (PDR).

---

## 1. Introduction

This Software Requirements Specification (SRS) elaborates the requirements defined in the [Software System Specification (SSS)](Software-System-Specification.md) into detailed, implementation-oriented requirements for the Mycelium platform. It addresses the three Mycelium components:

- **Mycelium Bloom** — the end-user web application (ASP.NET Blazor)
- **Mycelium Fabric** — the backend server (ASP.NET, CarterCommunity)
- **Mycelium Forge** — the package registry for SysML v2 model libraries

---

## 2. Applicable and reference documents

### 2.1 Applicable documents

| ID | Document |
|----|----------|
| AD-01 | ECSS-E-ST-40C Rev.1 — Space engineering: Software (30 April 2025) |
| AD-02 | OMG SysML v2 — Systems Modeling Language, version 2.0 (formal/25-09-03) |
| AD-03 | OMG KerML — Kernel Modeling Language, version 1.0 (formal/25-09-03) |
| AD-04 | OMG Systems Modelling API and Services, version 1.0 (formal/25-09-04) |
| AD-05 | [Software System Specification (SSS)](Software-System-Specification.md) |

### 2.2 Reference documents

| ID | Document |
|----|----------|
| RD-01 | [Roles and Permissions](Roles-and-Permissions.md) — Mycelium role and permission model |
| RD-02 | ECSS-E-TM-10-25A — Technical Memorandum: Engineering design model data exchange |
| RD-03 | ISO/IEC 80000 series — Quantities and units |
| RD-04 | SysML v2 Quantities and Units Domain Library (normative, section 9.8 of AD-02) |

---

## 3. Terms, definitions and abbreviated terms

See [SSS section 3](Software-System-Specification.md#3-terms-definitions-and-abbreviated-terms) for terms and abbreviations common to both documents. Additional terms specific to this SRS:

| Term | Definition |
|------|-----------|
| AttributeDefinition | A KerML/SysML abstract syntax metaclass representing a reusable type for data characteristics. All QUDV library concepts (quantity values, measurement units, scales, quantity kinds) are modeled as AttributeDefinitions. |
| AttributeUsage | A KerML/SysML abstract syntax metaclass representing a specific occurrence of an AttributeDefinition on a model element. |
| Domain Library | A normative SysML v2 model library providing reusable models of fundamental concepts. Domain libraries are instances of the abstract syntax, not extensions to it. |
| Specialization | A KerML relationship where one type inherits and extends another. Used extensively in the QUDV library to create quantity-specific types from abstract base types. |

---

## 4. Software overview

### 4.1 Function and purpose

The Mycelium platform provides a collaborative, web-based SysML v2 MBSE environment for early-phase system design and concurrent design processes. It replaces CDP4-COMET's ECSS-E-TM-10-25-based, desktop-first approach with a cloud-native, SysML v2-native architecture supporting lock-free collaboration, ownership-based access control, and near real-time model change notification.

### 4.2 Environmental considerations

- **Target runtime:** Linux containers (Docker/Kubernetes) for Mycelium Fabric and Mycelium Forge; modern web browsers for Mycelium Bloom
- **Hardware environment:** Cloud infrastructure or on-premise servers with container orchestration
- **Operating environment:** .NET 10, PostgreSQL, Keycloak, RabbitMQ

### 4.3 Relation to other systems

Mycelium operates within a broader engineering ecosystem:

- **Keycloak** — external identity provider for authentication and authorization (OIDC, SAML, LDAP, JWT)
- **PostgreSQL** — persistent storage for model data and functional data
- **RabbitMQ** — message broker for inter-service communication
- **External tools** — domain-specific engineering tools integrating via the OMG Systems Modelling API (REST/HTTP)
- **CDP4-COMET** — predecessor system; models can be migrated via the ECSS-E-TM-10-25 to SysML v2 converter

### 4.4 Constraints

- The SysML v2 abstract syntax (KerML + SysML, Chapters 7-8 of AD-02 and AD-03) defines the metamodel. All model content is expressed as instances of abstract syntax metaclasses.
- The SysML v2 Domain Libraries (Chapter 9 of AD-02) are normative model libraries — instances of the abstract syntax, not extensions to it. This applies to the Quantities and Units library, the Geometry library, and all other domain libraries.
- The OMG Systems Modelling API (AD-04) defines the REST/HTTP interface. Mycelium Fabric implements the REST/HTTP PSM.
- SysML v2 textual notation is generated read-only; editing and parsing of textual notation is not supported.

---

## 5. Requirements

### 5.1 General

Requirements are identified using the convention `SRS-<area>-<alpha-numeric>`. Each requirement traces to one or more SSS requirements (forward traceability is provided in section 7).

### 5.2 Functional requirements

#### 5.2.1 Quantities, Units, Dimensions and Values (QUDV)

##### 5.2.1.1 Architectural approach

In SysML v2, QUDV is not part of the abstract syntax metamodel (Chapter 8). It is entirely implemented as the **Quantities and Units Domain Library** (Section 9.8 of AD-02) — a normative model library where every QUDV concept is an instance of the standard KerML/SysML abstract syntax metaclasses, specifically `AttributeDefinition` and `AttributeUsage`.

The following table shows how QUDV concepts map to the abstract syntax:

| QUDV library concept | Abstract syntax metaclass | Specializes | Purpose |
|---------------------|--------------------------|-------------|---------|
| ScalarQuantityValue | AttributeDefinition | Base::DataValue | Abstract base for scalar quantity values; contains `num : Real` and `mRef : ScalarMeasurementReference` |
| VectorQuantityValue | AttributeDefinition | TensorQuantityValue | Value of a vector quantity (order 1) |
| TensorQuantityValue | AttributeDefinition | Base::DataValue | Most general tensor representation (order n) |
| MeasurementUnit | AttributeDefinition | ScalarMeasurementReference | A real scalar quantity defined by convention (per VIM); has `unitConversion` and `unitPowerFactors` |
| SimpleUnit | AttributeDefinition | MeasurementUnit | A base unit that references itself with exponent 1 |
| DerivedUnit | AttributeDefinition | MeasurementUnit | A unit depending on powers of other measurement units |
| MeasurementScale | AttributeDefinition | ScalarMeasurementReference | Non-ratio scale measurement (ordinal, interval, cyclic, logarithmic) |
| OrdinalScale | AttributeDefinition | MeasurementScale | Scale where only ordering is meaningful |
| IntervalScale | AttributeDefinition | MeasurementScale | Scale where intervals are meaningful (bound) |
| CyclicRatioScale | AttributeDefinition | MeasurementScale | Periodic ratio scale (e.g. degrees 0-360) |
| LogarithmicScale | AttributeDefinition | MeasurementScale | Scale defined by v = f * log_b(x/x_ref)^a |
| QuantityDimension | AttributeDefinition | Base::DataValue | Product of powers of base quantities for dimensional analysis |
| SystemOfQuantities | AttributeDefinition | Base::DataValue | Collection of base and derived quantities (e.g. ISQ) |
| SystemOfUnits | AttributeDefinition | Base::DataValue | Collection of base and derived units (e.g. SI) |
| UnitConversion | AttributeDefinition | Base::DataValue | Linear conversion with `conversionFactor` and `referenceUnit` |
| CoordinateFrame | AttributeDefinition | VectorMeasurementReference | Reference for quantifying vector spaces |

Specific quantities and units are further specializations:

| Library element | Type | Specializes | Example |
|----------------|------|-------------|---------|
| LengthValue | AttributeDefinition | ScalarQuantityValue | Quantity type for length |
| LengthUnit | AttributeDefinition | SimpleUnit | Unit type for length |
| MassValue | AttributeDefinition | ScalarQuantityValue | Quantity type for mass |
| MassUnit | AttributeDefinition | SimpleUnit | Unit type for mass |
| metre | AttributeUsage | LengthUnit | SI base unit for length |
| kilogram | AttributeUsage | MassUnit | SI base unit for mass |
| kilometre | AttributeUsage | LengthUnit | SI prefixed unit (prefix: kilo) |

##### 5.2.1.2 Key design principle: type independence from units

The type of a quantity value (e.g. `PowerValue`) is independent from the choice of measurement unit (e.g. Watt vs milliWatt). Both `{num=1.5, mRef=Watt}` and `{num=1500, mRef=MilliWatt}` are typed by the same `PowerValue` AttributeDefinition. This enables:

- Automatic unit conversion between compatible units
- Dimensional analysis and type checking in constraint expressions
- Consistent typing regardless of unit preference

##### 5.2.1.3 Standard library structure

The Quantities and Units Domain Library is organized into packages:

| Package | Content | Standard |
|---------|---------|----------|
| ISQBase | Base quantities, general concepts | ISO/IEC 80000 |
| ISQ | 11 sub-packages covering all ISQ quantity kinds | ISO/IEC 80000-3 through 80000-13 |
| SI | SI base and derived measurement units | ISO/IEC 80000 |
| SIPrefixes | Decimal prefixes (kilo, mega, nano...) and binary prefixes (kibi, mebi...) | ISO/IEC 80000 |
| USCustomary | US customary units with conversion factors to SI | NIST SP-811 |
| Time | Gregorian calendar, UTC, ISO 8601 representations | ISO 8601 |

##### 5.2.1.4 QUDV requirements

| ID | Component | Requirement | Traces to SSS |
|----|-----------|-------------|---------------|
| SRS-QUDV-K4R | Fabric | Mycelium Fabric shall store all QUDV library concepts (quantity values, measurement units, measurement scales, quantity kinds, unit conversions) as AttributeDefinitions and AttributeUsages conforming to the SysML v2 abstract syntax when "model data containing QUDV constructs is persisted." | SSS-PA-ARCH-97Z |
| SRS-QUDV-W7N | Fabric | Mycelium Fabric shall import the normative SysML v2 Quantities and Units Domain Library packages (ISQBase, ISQ, SI, SIPrefixes, USCustomary, Time) as project-importable library content when "the platform is initialized or a library update is deployed." | SSS-PA-QU-G1W |
| SRS-QUDV-M3J | Fabric | Mycelium Fabric shall resolve specialization hierarchies of AttributeDefinitions to determine QUDV classification (quantity value, measurement unit, measurement scale, quantity kind) when "a client queries QUDV library content by type." | SSS-PA-QU-T3K, SSS-PA-QU-R7N, SSS-PA-QU-W5J, SSS-PA-QU-D8M |
| SRS-QUDV-P8D | Fabric | Mycelium Fabric shall evaluate unit conversion factors between compatible MeasurementUnit AttributeDefinitions using the `unitConversion` and `unitPowerFactors` features when "a unit conversion is requested." | SSS-PA-ARCH-97Z |
| SRS-QUDV-V2H | Bloom | Mycelium Bloom shall present QUDV library content in tabular browsers by filtering AttributeDefinitions according to their specialization hierarchy (e.g. all specializations of MeasurementUnit, all specializations of ScalarQuantityValue) when "a user opens a QUDV tabular browser." | SSS-PA-QU-T3K, SSS-PA-QU-R7N, SSS-PA-QU-W5J, SSS-PA-QU-D8M |
| SRS-QUDV-F6T | Bloom | Mycelium Bloom shall present custom QUDV creation dialogs that create new AttributeDefinitions specializing the appropriate QUDV base type (e.g. specializing MeasurementUnit for a new unit, specializing ScalarQuantityValue for a new quantity kind) when "a user creates a custom quantity, unit, or scale." | SSS-PA-QU-H2V, SSS-PA-QU-K6F, SSS-PA-QU-B4P |
| SRS-QUDV-B9L | Bloom | Mycelium Bloom shall create an AttributeUsage typed by the dragged AttributeDefinition on the target element, with the `mRef` feature defaulting to the AttributeDefinition's associated MeasurementUnit, when "a user drops an Attribute Definition from a QUDV browser onto an element Definition or Usage." | SSS-PA-QU-N9X |
| SRS-QUDV-J1G | Bloom | Mycelium Bloom shall present unit selection constrained to compatible MeasurementUnit specializations (e.g. only LengthUnit subtypes for a LengthValue attribute) when "a user edits the measurement reference of an attribute value." | SSS-PA-ARCH-97Z |
| SRS-QUDV-A5X | Bloom | Mycelium Bloom shall display converted attribute values in the user's preferred unit while preserving the stored value and unit when "a user views an attribute value and has a different preferred unit configured." | SSS-PA-ARCH-97Z |

#### 5.2.2 Model element management

TBD — Detailed requirements for CRUD operations on all Definition and Usage types.

#### 5.2.3 Namespace and package management

TBD — Detailed requirements for package hierarchy, imports, visibility, and alias management.

#### 5.2.4 Version control and branching

TBD — Detailed requirements for commit, branch, merge, tag, and diff operations.

#### 5.2.5 Concurrent design support

TBD — Detailed requirements for ownership enforcement, publication workflow, and real-time notifications.

#### 5.2.6 Organization and user management

TBD — Detailed requirements for installation-wide and organization-scoped user management.

#### 5.2.7 Diagram editing

TBD — Detailed requirements for all diagram types (Interconnection, Action Flow, State Transition, Sequence, General, Grid).

#### 5.2.8 Query execution

TBD — Detailed requirements for the query interface and execution engine.

### 5.3 Performance requirements

TBD — Detailed performance targets (response times, throughput, capacity) derived from SSS-CC-PERF requirements.

### 5.4 Interface requirements

TBD — Detailed interface specifications for Bloom-Fabric, Bloom-Forge, Fabric-Keycloak, Fabric-PostgreSQL, Fabric-RabbitMQ, and the external Systems Modelling API.

### 5.5 Operational requirements

TBD — Operational modes (Regular, Concurrent Design), mode transitions, deployment scenarios (SaaS, on-premise).

### 5.6 Resources requirements

TBD — Hardware resource requirements, sizing and timing constraints, software dependencies.

### 5.7 Design requirements and implementation constraints

TBD — Technology stack constraints (C#, .NET 10, Blazor, Carter, PostgreSQL), coding standards, design patterns.

### 5.8 Security and privacy requirements

TBD — Authentication flows, authorization enforcement, ownership-based access control implementation, data encryption, audit logging. See [Roles and Permissions](Roles-and-Permissions.md).

### 5.9 Portability requirements

TBD — Container portability, database portability, browser compatibility.

### 5.10 Software quality requirements

TBD — Usability, progressive disclosure, accessibility, internationalization.

### 5.11 Software reliability requirements

TBD — Availability targets, data durability, fault tolerance, graceful degradation.

### 5.12 Software maintainability requirements

TBD — Modularity, testability, backward compatibility, data migration between versions.

### 5.13 Software safety requirements

Not applicable. The Mycelium platform is a web-based engineering tool and does not perform safety-critical functions.

### 5.14 Software configuration and delivery requirements

TBD — Container image delivery, Helm charts, configuration management, versioning scheme.

### 5.15 Data definition and database requirements

TBD — Database schema generation from SysML v2 metamodel, data formats (JSON, XMI, MessagePack, KPAR), model interchange.

### 5.16 Human factors related requirements

TBD — Role-aware interface adaptation, drag-and-drop interactions, progressive disclosure, workspace customization.

### 5.17 Adaptation and installation requirements

TBD — SaaS vs on-premise configuration, multi-tenancy, organization bootstrapping, first-user setup.

---

## 6. Validation requirements

TBD — Validation approach per requirement, validation matrix (requirements to validation method correlation).

---

## 7. Traceability

TBD — Forward traceability (SSS → SRS) and backward traceability (SRS → SSS) matrices.

---

## 8. Logical model description

TBD — Top-down description of the logical model including:
- Data model (SysML v2 abstract syntax as implemented)
- Application function model (Bloom, Fabric, Forge component interactions)
- Behavioral model (user workflows, concurrent design sessions)
- Diagrams and walkthroughs
