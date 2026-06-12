# requirements

This repository contains the `Requirements` for the Mycelium collaborative MBSE ecosystem — a next-generation, SysML v2 native, web-first Model-Based Systems Engineering platform evolving from CDP4-COMET.

## Repository contents

The core contents are:

- `Software-System-Specification.md` — Software System Specification (SSS) per ECSS-E-ST-40C Annex B
- `Software-System-Specification-Justification.md` - Justifications for selected requirements to provide extra contaxt for these requirements.
- `Software-System-Specification - Project.md` — Software System Specification (SSS) per ECSS-E-ST-40C Annex B, including the High priority requirements that will be implemented in contract 4000151333/26/NL/GP/mdu.
- `Roles-and-Permissions.md` — Role and permission model supporting the SSS

The following folders contain reference documentation

- `cdp4-comet/` — CDP4-COMET user manual reference
- `Reference Documentation/` — third-party specification documents (see notice below)

## Scripting

The `scripting` folder contains script(s) to conver the markdown into other formats:
  - `Mdtodocx.cs`: a c# script to conver a `.md` file into a `.docx` file

To conver the SSS into a docx file execute the following command from the root of the repository:

```
dotnet run scripting/MdToDocx.cs -- "Software-System-Specification.md"
dotnet run scripting/MdToDocx.cs -- "Software-System-Specification - Project.md"
```

## Third-Party Reference Documentation

The `Reference Documentation/` folder contains materials **that are not authored or owned by Starion**. They are copyrighted works of the **Object Management Group (OMG)** and the **European Cooperation for Space Standardization (ECSS)**, included in this repository solely as offline reference and search aids for the Mycelium requirements work.

- The files are textified extractions of the original PDFs. They are **not redistributed, modified in substance, or claimed as Starion work product**.
- Anyone needing an authoritative copy of any of these documents must obtain it directly from OMG (https://www.omg.org) or ECSS (https://ecss.nl).
- Full per-file attribution is provided in [`THIRD-PARTY-NOTICES.md`](THIRD-PARTY-NOTICES.md).

## Ownership

All content in this repository **other than** the files under `Reference Documentation/` is © Starion Group.
