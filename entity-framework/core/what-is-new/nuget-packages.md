---
title: EF Core NuGet Packages
description: Overview of the different Entity Framework Core NuGet packages
author: SamMonoRT
ms.date: 10/21/2024
uid: core/what-is-new/nuget-packages
---

# EF Core NuGet Packages

Entity Framework Core (EF Core) is shipped as [NuGet](https://www.nuget.org/profiles/EntityFramework) packages. The packages needed by an application depends on:

- The type of database system being used (SQL Server, SQLite, etc.)
- The EF Core features needed

The usual process for installing packages is:

- Decide on a database provider and install the appropriate package ([see below](#database-providers))
- Also install [Microsoft.EntityFrameworkCore](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/), and [Microsoft.EntityFrameworkCore.Relational](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Relational/) if using a relational database provider. This helps ensure that consistent versions are being used, and also means that NuGet will let you know when new package versions are shipped.
- Optionally, decide which kind of tooling you need and install the appropriate packages for that ([see below](#tools))

See [Getting started tutorial for Entity Framework Core](xref:core/get-started/overview/first-app) for help getting started with EF Core.

## Package versions

Make sure to install the same version of all EF Core packages shipped by Microsoft. For example, if version 5.0.3 of Microsoft.EntityFrameworkCore.SqlServer is installed, then all other Microsoft.EntityFrameworkCore.* packages must also be at 5.0.3.

Also make sure that any external packages are compatible with the version of EF Core being used. In particular, check that external database provider supports the version of EF Core you are using. New major versions of EF Core usually require an updated database provider.

> [!WARNING]
> NuGet does not enforce consistent package versions. Always carefully check which package versions you are referencing in your `.csproj` file or equivalent.

## Database providers

EF Core supports different database systems through the use of "database providers". Each system has its own database provider, which is shipped as NuGet package. Applications should install one or more of these provider packages.

Common database providers are listed in the table below. See [database providers](xref:core/providers/index) for a more comprehensive list of available providers.

| Database system                   | Package
|:----------------------------------|----------------------
| SQL Server and SQL Azure          | [Microsoft.EntityFrameworkCore.SqlServer](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.SqlServer)
| SQLite                            | [Microsoft.EntityFrameworkCore.Sqlite](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Sqlite)
| Azure Cosmos DB                   | [Microsoft.EntityFrameworkCore.Cosmos](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Cosmos)
| PostgreSQL                        | [Npgsql.EntityFrameworkCore.PostgreSQL](https://www.nuget.org/packages/Npgsql.EntityFrameworkCore.PostgreSQL/)*
| MySQL                             | [Pomelo.EntityFrameworkCore.MySql](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/)*
| EF Core in-memory database**      | [Microsoft.EntityFrameworkCore.InMemory](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.InMemory)

*These are popular, high quality, open-source providers developed and shipped by the community. The other providers listed are shipped by Microsoft.

**Carefully consider whether to use the in-memory provider. It is not designed for production use, and also may not be [the best solution for testing](xref:core/testing/index).

## Tools

Use of tooling for [EF Core migrations](xref:core/managing-schemas/migrations/index) and [reverse engineering (scaffolding) from an existing database](xref:core/managing-schemas/scaffolding) requires installation of the appropriate tooling package:

- [dotnet-ef](https://www.nuget.org/packages/dotnet-ef/) for cross-platform command line tooling
- [Microsoft.EntityFrameworkCore.Tasks](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Tasks/) for MSBuild tasks allowing build-time integration.
- [Microsoft.EntityFrameworkCore.Tools](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Tools/) for PowerShell tooling that works in the Visual Studio [Package Manager Console](/nuget/consume-packages/install-use-packages-powershell)

See [Entity Framework Core Tools Reference](xref:core/cli/index) for more information on using EF Core tooling, including how to correctly install the `dotnet-ef` tool in a project or globally.

> [!TIP]
> By default, the Microsoft.EntityFrameworkCore.Design package is installed in such a way that it will not be deployed with your application. This also means that its types cannot be transitively used in other projects. Use a normal `PackageReference` in your `.csproj` file or equivalent if you need access to the types in this package. See [Design-time services](xref:core/cli/services) for more information.

## Extension packages

There are many [extensions for EF Core](xref:core/extensions/index) published both by Microsoft and third parties as NuGet packages. Commonly used packages include:

| Functionality                                | Package | Additional dependencies
|:---------------------------------------------|---------|------------------------
| Proxies for lazy-loading and change tracking | [Microsoft.EntityFrameworkCore.Proxies](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Proxies/) | [Castle.Core](https://www.nuget.org/packages/Castle.Core/)
| Spatial support for SQL Server               | [Microsoft.EntityFrameworkCore.SqlServer.NetTopologySuite](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.SqlServer.NetTopologySuite/) | [NetTopologySuite](https://www.nuget.org/packages/NetTopologySuite/) and [NetTopologySuite.IO.SqlServerBytes](https://www.nuget.org/packages/NetTopologySuite.IO.SqlServerBytes/)
| Spatial support for SQLite                   | [Microsoft.EntityFrameworkCore.Sqlite.NetTopologySuite](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Sqlite.NetTopologySuite/) | [NetTopologySuite](https://www.nuget.org/packages/NetTopologySuite/) and [NetTopologySuite.IO.SpatiaLite](https://www.nuget.org/packages/NetTopologySuite.IO.SpatiaLite/)
| Spatial support for PostgreSQL               | [Npgsql.EntityFrameworkCore.PostgreSQL.NetTopologySuite](https://www.nuget.org/packages/Npgsql.EntityFrameworkCore.PostgreSQL.NetTopologySuite) | [NetTopologySuite](https://www.nuget.org/packages/NetTopologySuite/) and [NetTopologySuite.IO.PostGIS](https://www.nuget.org/packages/NetTopologySuite.IO.PostGIS/) (via [Npgsql.NetTopologySuite](https://www.nuget.org/packages/Npgsql.NetTopologySuite/))
| Spatial support for MySQL                    | [Pomelo.EntityFrameworkCore.MySql.NetTopologySuite](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql.NetTopologySuite) | [NetTopologySuite](https://www.nuget.org/packages/NetTopologySuite/)

## Other packages

Other EF Core packages are pulled in as dependencies of the database provider package. However, you may want to add explicit package references for these packages so that NuGet will provide notifications when new versions are released.

| Functionality                                              | Package
|:-----------------------------------------------------------|--------
| EF Core basic functionality                                | [Microsoft.EntityFrameworkCore](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/)
| Common relational database functionality                   | [Microsoft.EntityFrameworkCore.Relational](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Relational/)
| Lightweight package for EF Core attributes, etc.           | [Microsoft.EntityFrameworkCore.Abstractions](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Abstractions/)
| Roslyn code analyzers for EF Core usage                    | [Microsoft.EntityFrameworkCore.Analyzers](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Analyzers/)
| EF Core SQLite provider without a native SQLite dependency | [Microsoft.EntityFrameworkCore.Sqlite.Core](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Sqlite.Core/)
| Design-time functionality implementation shared by EF tools| [Microsoft.EntityFrameworkCore.Design](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Design/)

## Packages for database provider testing

The following packages are used to test database providers built in external GitHub repositories. See [efcore.pg](https://github.com/npgsql/efcore.pg) and [Pomelo.EntityFrameworkCore.MySql](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql) for examples. Applications should not install these packages.

| Functionality                                              | Package
|:-----------------------------------------------------------|--------
| Tests for any database provider                            | [Microsoft.EntityFrameworkCore.Specification.Tests](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Specification.Tests/)
| Tests for relational database providers                    | [Microsoft.EntityFrameworkCore.Relational.Specification.Tests](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Relational.Specification.Tests/)

## Obsolete packages

Do **not** install the following obsolete packages, and remove them if they are currently installed in your projects:

- Microsoft.EntityFrameworkCore.Relational.Design
- Microsoft.EntityFrameworkCore.Tools.DotNet
- Microsoft.EntityFrameworkCore.SqlServer.Design
- Microsoft.EntityFrameworkCore.Sqlite.Design
- Microsoft.EntityFrameworkCore.Relational.Design.Specification.Tests
