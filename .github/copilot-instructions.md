# Copilot Instructions for EntityFramework.Docs

## Repository Overview

This repository contains the **Entity Framework Core (EF Core) and Entity Framework 6 (EF6) documentation** published at <https://learn.microsoft.com/ef/>. It is a **documentation-focused repository** with markdown files (305 .md files) and C# code samples (~89 sample projects). The repo is ~122 MB with documentation (~12 MB) and samples (~110 MB).

**Key Technologies:**
- **Documentation**: Markdown, DocFX, Microsoft Learn publishing platform
- **Samples**: .NET 10.0 (SDK 10.0.100+), C#, Entity Framework Core/EF6, SQL Server, SQLite, Cosmos DB
- **Tooling**: Node.js 20.x, markdownlint-cli, .NET SDK

## Critical Build & Validation Instructions

### Prerequisites
- **.NET SDK 10.0.100+** is required (check with `dotnet --version`)
- **Node.js 20.x** for markdown linting (check with `node --version`)

### Markdown Linting (ALWAYS RUN BEFORE COMMITTING)

**Command:**
```bash
npm i -g markdownlint-cli
markdownlint "**/*.md" -i "entity-framework/ef6/"
```

**Configuration:** `.markdownlint.json` at repo root
**Key Rules:** MD046 (fenced code blocks), MD025 (single H1 with front matter)
**CI Workflow:** `.github/workflows/markdownlint.yml` runs on all PRs with markdown changes
**IMPORTANT:** EF6 documentation (`entity-framework/ef6/`) is excluded from linting

### Building Code Samples

**Location:** All samples are in `samples/` directory
- `samples/core/` - EF Core samples (89 projects, Samples.sln solution file)
- `samples/end2end/PlanetaryDocs/` - Complete end-to-end Blazor application

**Building Individual Samples:**
```bash
cd samples/end2end/PlanetaryDocs
dotnet build
```

**Result:** Builds successfully in ~16 seconds with some expected warnings (CS0618, BL0007)

**Building All Core Samples:**
```bash
cd samples/core
dotnet build
```

**KNOWN ISSUE:** Building `samples/core/Samples.sln` may fail intermittently with NuGet package download errors from Azure DevOps feeds ("Resource temporarily unavailable"). This is a transient network issue with preview package feeds (10.0.0-preview packages). **Retry the build** if this occurs. Individual samples build reliably.

**Testing Samples:**
```bash
cd samples/core
dotnet build
```
**CI Workflow:** `.github/workflows/build-samples.yml`
- Runs on PRs to `live` branch with changes to `samples/core/**` or `samples/end2end/**`
- Uses .NET 10.0 SDK on ubuntu-24.04
- Builds `samples/core` and `samples/end2end/PlanetaryDocs`

### NuGet Configuration
**File:** `samples/NuGet.Config` specifies only nuget.org as package source
**Global Config:** `samples/global.json` pins SDK to 10.0.100

## Repository Structure

### Documentation Files
```
entity-framework/
├── docfx.json           # DocFX configuration for building documentation
├── core/                # EF Core documentation (~200 files)
│   ├── index.md
│   ├── get-started/
│   ├── modeling/
│   ├── querying/
│   ├── saving/
│   ├── change-tracking/
│   ├── managing-schemas/
│   ├── providers/       # Database provider documentation
│   ├── performance/
│   ├── testing/
│   ├── what-is-new/     # Version-specific features
│   └── ...
├── ef6/                 # EF6 documentation (~100 files, excluded from markdownlint, generally not to be touched)
├── efcore-and-ef6/      # Comparison and porting guides
└── breadcrumb/          # Navigation breadcrumbs

Root files:
├── .markdownlint.json              # Markdown linting rules
├── .openpublishing.publish.config.json  # Microsoft Learn publishing config
├── .openpublishing.redirection.json     # URL redirections
├── README.md                       # Main repository README
├── CONTRIBUTING.md                 # Contribution guidelines
└── .github/
    ├── workflows/
    │   ├── markdownlint.yml        # Markdown lint CI
    │   └── build-samples.yml       # Sample build CI
    └── dependabot.yml              # Weekly NuGet updates
```

### Code Samples Structure
```
samples/
├── global.json          # .NET SDK version (10.0.100)
├── NuGet.Config         # Package sources (nuget.org only)
├── core/
│   ├── Samples.sln      # Solution with all core samples
│   ├── .editorconfig    # Code style rules
│   ├── GetStarted/
│   ├── Modeling/
│   ├── Querying/
│   ├── Saving/
│   ├── Testing/
│   └── ...              # 89 total project files
└── end2end/
    └── PlanetaryDocs/   # Complete Blazor app with Cosmos DB
        ├── PlanetaryDocs.sln
        ├── PlanetaryDocs/           # Blazor web app
        ├── PlanetaryDocs.Domain/    # Domain models
        ├── PlanetaryDocs.DataAccess/  # EF Core data layer
        └── Tests/DomainTests/       # xUnit tests (129 tests)
```

## Making Changes

### Documentation Changes

**File Format:** DocFX-flavored Markdown (DFM), superset of GitHub-flavored Markdown (GFM)
**Style Guide:** <https://learn.microsoft.com/contribute/dotnet/dotnet-style-guide>

**Code Snippet Syntax:**
- Reference external code files (preferred): `[!code-csharp[Main](../../../samples/core/saving/Program.cs)]`
- With line range: `[!code-csharp[Main](../../../samples/core/saving/Program.cs?range=1-10)]`
- With C# region: `[!code-csharp[Main](../../../samples/core/saving/Program.cs?name=snippet_Example)]`
- With highlighting: `[!code-csharp[Main](../../../samples/core/saving/Program.cs?highlight=1-3,10)]`

**Static Content:** Images and files in `_static/` folders within each documentation area

**ALWAYS:**
1. Ensure code snippets reference actual sample files in `samples/` directory
2. Run `markdownlint "**/*.md" -i "entity-framework/ef6/"` before committing
3. Match folder structure: docs in `entity-framework/core/` align with samples in `samples/core/`
4. When referencing an API, use docfx `<xref>` rather than code fencing to link to the API documentation.
5. When adding, removing or renaming pages, update the `entity-framework/toc.yml` file to make the changes appear in the doc site's table of contents.

### Sample Code Changes

**ALWAYS:**
1. Ensure samples build successfully: `cd samples/core && dotnet build`
2. Follow existing code style (see `samples/core/.editorconfig`)
3. Use C# regions (`#region snippet_Name`) for code referenced in documentation

**Common Sample Patterns:**
- Console applications showing specific EF Core features
- Each sample folder typically has one `.csproj` file
- Samples use in-memory SQLite or SQL Server LocalDB

## VS Code Configuration

**Recommended Extension:** `docsmsft.docs-authoring-pack` (see `.vscode/extensions.json`)

**Custom Dictionary Words** (`.vscode/settings.json`):
- dbcontext, LINQ, navigations, queryable, savepoints, subquery, etc.

## Testing Documentation Locally with DocFX

**DocFX** creates a locally hosted version of the documentation site (without Microsoft Learn styling).

**Requirements:**
- Windows: .NET Framework + DocFX tool
- macOS/Linux: Mono + DocFX

**Commands (not regularly used for PRs):**
```bash
# Download DocFX from https://github.com/dotnet/docfx/releases
# Add to PATH, then from repo root:
docfx entity-framework/docfx.json -t default --serve
# View at http://localhost:8080
```

**Note:** DocFX is optional for most contributions. Focus on markdown linting and sample builds.

## Common Pitfalls & Solutions

**Problem:** `samples/core/Samples.sln` build fails with "Resource temporarily unavailable" NuGet errors
**Solution:** This is a transient network issue with Azure DevOps preview package feeds. Retry the build. If persistent, build individual sample projects instead of the full solution.

**Problem:** Markdownlint errors on legitimate markdown
**Solution:** Check `.markdownlint.json` for disabled rules. Some rules (MD028, MD033, MD036, MD041) are intentionally disabled for documentation needs.

**Problem:** Code snippet not rendering in documentation
**Solution:** Verify the referenced file path is correct relative to the .md file location. Ensure C# region names match exactly (case-sensitive).

**Problem:** Changes to EF6 docs trigger linting warnings
**Solution:** EF6 docs (`entity-framework/ef6/`) are excluded from linting. This is intentional due to legacy content.

## Trust These Instructions

These instructions have been validated by running actual builds, tests, and linting on the repository. Only perform additional exploration if:
- Information is incomplete or unclear
- Instructions are found to be incorrect
- You need details about a specific undocumented area

For quick fixes (typos, grammar), simple markdown edits without building samples are acceptable.
