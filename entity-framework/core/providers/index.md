---
title: Database Providers - EF Core
author: rowanmiller
ms.author: divega
ms.date: 2/23/2018
ms.assetid: 14fffb6c-a687-4881-a094-af4a1359a296
ms.technology: entity-framework-core
uid: core/providers/index
---

# Database Providers

Entity Framework Core can access many different databases through plug-in libraries called database providers.

## Current providers
> [!IMPORTANT]  
> EF Core providers are built by a variety of sources. Not all providers are maintained as part of the [Entity Framework Core Project](https://github.com/aspnet/EntityFrameworkCore). When considering a provider, be sure to evaluate quality, licensing, support, etc. to ensure they meet your requirements. Also make sure you review each provider's documentation for detailed version compatibility information.

| NuGet Package                                                                                                        | Supported database engines | Maintainer / Vendor                                                           | Notes / Requirements             | Useful links                                                                                                                                                              |
|:---------------------------------------------------------------------------------------------------------------------|:---------------------------|:------------------------------------------------------------------------------|:---------------------------------|:--------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| [Microsoft.EntityFrameworkCore.SqlServer](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.SqlServer)    | SQL Server 2008 onwards    | [EF Core Project](https://github.com/aspnet/EntityFrameworkCore/) (Microsoft) |                                  | [docs](xref:core/providers/sql-server/index)                                                                                                                              |
| [Microsoft.EntityFrameworkCore.Sqlite](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Sqlite)          | SQLite 3.7 onwards         | [EF Core Project](https://github.com/aspnet/EntityFrameworkCore/) (Microsoft) |                                  | [docs](xref:core/providers/sqlite/index)                                                                                                                                  |
| [Microsoft.EntityFrameworkCore.InMemory](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.InMemory)      | EF Core in-memory database | [EF Core Project](https://github.com/aspnet/EntityFrameworkCore/) (Microsoft) | For testing only                 | [docs](xref:core/providers/in-memory/index)                                                                                                                               |
| [Npgsql.EntityFrameworkCore.PostgreSQL](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.SqlServer)      | PostgreSQL                 | [Npgsql Development Team](https://github.com/npgsql)                          |                                  | [docs](http://www.npgsql.org/efcore/index.html)                                                                                                                           |
| [Pomelo.EntityFrameworkCore.MySql](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql)                  | MySQL, MariaDB             | [Pomelo Foundation Project](https://github.com/PomeloFoundation)              |                                  | [readme](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/blob/master/README.md)                                                                      |
| [Pomelo.EntityFrameworkCore.MyCat](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MyCat)                  | MyCAT Server               | [Pomelo Foundation Project](https://github.com/PomeloFoundation)              | Pre-release, Up to EF Core 1.1   | [readme](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MyCat/blob/master/README.md)                                                                      |
| [EntityFrameworkCore.SqlServerCompact40](https://www.nuget.org/packages/EntityFrameworkCore.SqlServerCompact40)      | SQL Server Compact 4.0     | [Erik Ejlskov Jensen](https://github.com/ErikEJ/)                             | .NET Framework                   | [wiki](https://github.com/ErikEJ/EntityFramework.SqlServerCompact/wiki/Using-EF-Core-with-SQL-Server-Compact-in-Traditional-.NET-Applications)                            |
| [EntityFrameworkCore.SqlServerCompact35](https://www.nuget.org/packages/EntityFrameworkCore.SqlServerCompact35)      | SQL Server Compact 3.5     | [Erik Ejlskov Jensen](https://github.com/ErikEJ/)                             | .NET Framework                   | [wiki](https://github.com/ErikEJ/EntityFramework.SqlServerCompact/wiki/Using-EF-Core-with-SQL-Server-Compact-in-Traditional-.NET-Applications)                            |
| [MySql.Data.EntityFrameworkCore](https://www.nuget.org/packages/MySql.Data.EntityFrameworkCore)                      | MySQL                      | [MySQL project](http://dev.mysql.com) (Oracle)                                | Pre-release                      | [docs](https://dev.mysql.com/doc/connector-net/en/)                                                                                                                       |
| [FirebirdSql.EntityFrameworkCore.Firebird](https://www.nuget.org/packages/FirebirdSql.EntityFrameworkCore.Firebird/) | Firebird 2.5 and 3.x       | [Jiri Cincura](https://github.com/cincuranet)                                 | EF Core 2.0 onwards, Pre-release | [blog](https://www.tabsoverspaces.com/233653-preview-of-entity-framework-core-2-0-support-for-firebird-and-firebirdclient-6-0/)                                           |
| [EntityFrameworkCore.FirebirdSql](https://www.nuget.org/packages/EntityFrameworkCore.FirebirdSql/)                   | Firebird 2.5 and 3.x       | [Rafael Almeida](https://github.com/ralmsdeveloper)                           | EF Core 2.0 onwards              | [wiki](https://github.com/ralmsdeveloper/EntityFrameworkCore.FirebirdSQL/wiki)                                                                                            |
| [IBM.EntityFrameworkCore](https://www.nuget.org/packages/IBM.EntityFrameworkCore)                                    | Db2, Informix              | [IBM](https://ibm.com)                                                        | Up to EF Core 1.1, Windows       | [FAQ](https://www.ibm.com/developerworks/community/blogs/96960515-2ea1-4391-8170-b0515d08e4da/entry/Instructions_for_downloading_and_using_DB2_NET_Core_provider_package) |
| [IBM.EntityFrameworkCore-lnx](https://www.nuget.org/packages/IBM.EntityFrameworkCore-lnx)                            | Db2, Informix              | [IBM](https://ibm.com)                                                        | Up to EF Core 1.1, Linux         | [FAQ](https://www.ibm.com/developerworks/community/blogs/96960515-2ea1-4391-8170-b0515d08e4da/entry/Instructions_for_downloading_and_using_DB2_NET_Core_provider_package) |
| [Devart.Data.Oracle.EFCore](https://www.nuget.org/packages/Devart.Data.Oracle.EFCore/)                               | Oracle 9.2.0.4 onwards     | [DevArt](https://www.devart.com/)                                             | Paid                             | [docs](https://www.devart.com/dotconnect/oracle/docs/)                                                                                                                    |
| [Devart.Data.PostgreSql.EFCore](https://www.nuget.org/packages/Devart.Data.PostgreSql.EFCore/)                       | PostgreSQL 8.0 onwards     | [DevArt](https://www.devart.com/)                                             | Paid                             | [docs](https://www.devart.com/dotconnect/postgresql/docs/)                                                                                                                |
| [Devart.Data.SQLite.EFCore](https://www.nuget.org/packages/Devart.Data.SQLite.EFCore/)                               | SQLite 3 onwards           | [DevArt](https://www.devart.com/)                                             | Paid                             | [docs](https://www.devart.com/dotconnect/sqlite/docs/)                                                                                                                    |
| [Devart.Data.MySql.EFCore](https://www.nuget.org/packages/Devart.Data.MySql.EFCore/)                                 | MySQL 5 onwards            | [DevArt](https://www.devart.com/)                                             | Paid                             | [docs](https://www.devart.com/dotconnect/mysql/docs/)                                                                                                                     |
| [EntityFrameworkCore.Jet](https://www.nuget.org/packages/EntityFrameworkCore.Jet/)                                   | Microsoft Access files     | [Bubi](https://github.com/bubibubi)                                           | EF Core 2.0, .NET Framework      | [readme](https://github.com/bubibubi/EntityFrameworkCore.Jet/blob/master/docs/README.md)                                                                                  |

## Future Providers

### Cosmos DB

We have been developing an EF Core provider for the DocumentDB API in Cosmos DB. This will be the first complete document-oriented database provider we have produced, and the learnings from this exercise are going to inform improvements in the design of the subsequent release after 2.1. The current plan is to publish an early preview of the provider in the 2.1 timeframe.

### Oracle
The Oracle .NET team has announced they are planning to release a first-party provider for EF Core 2.0 approximately in the third quarter of 2018. See their [statement of direction for .NET Core and Entity Framework Core](http://www.oracle.com/technetwork/topics/dotnet/tech-info/odpnet-dotnet-ef-core-sod-4395108.pdf) for more information.
Please direct any questions about this provider, including the release timeline, to the [Oracle Community Site](https://community.oracle.com/).

In the meanwhile, the EF team has produced a [sample EF Core provider for Oracle databases](https://github.com/aspnet/EntityFrameworkCore/blob/dev/samples/OracleProvider/README.md). The purpose of the project is not to produce an EF Core provider owned by Microsoft, but to help us identify gaps in EF Core's relational and base functionality which we need to address in order to better support Oracle, and to jumpstart the development of other Oracle providers for EF Core by either Oracle or third parties.

We will consider contributions that improve the sample implementation. We would also welcome and encourage a community effort to create an open-source Oracle provider for EF Core, using the sample as a starting point.

## Adding a database provider to your application

Most database providers for EF Core are distributed as NuGet packages. This means they can be installed using the `dotnet` tool in the command line:

``` console
dotnet add package provider_package_name
```

Or in Visual Studio, using NuGet's Package Manager Console:

``` powershell
install-package provider_package_name
```

Once installed, you will configure the provider in your `DbContext`, either in the `OnConfiguring` method or in the `AddDbContext` method if you are using a dependency injection container. E.g. the following line configures the SQL Server provider with the passed connection string:

``` csharp
optionsBuilder.UseSqlServer(
    "Server=(localdb)\mssqllocaldb;Database=MyDatabase;Trusted_Connection=True;");
```  

Database providers can extend EF Core to enable functionality unique to specific databases. Some concepts are common to most databases, and are included in the primary EF Core components. Such concepts include expressing queries in LINQ, transactions, and tracking changes to objects once they are loaded from the database. Some concepts are specific to a particular provider. For example, the SQL Server provider allows you to [configure memory-optimized tables](xref:core/providers/sql-server/memory-optimized-tables) (a feature specific to SQL Server). Other concepts are specific to a class of providers. For example, EF Core providers for relational databases build on the common `Microsoft.EntityFrameworkCore.Relational` library, which provides APIs for configuring table and column mappings, foreign key constraints, etc.
Providers are usually distributed as NuGet packages.

> [!IMPORTANT]  
> When a new patch version of EF Core is released, it often includes updates to the `Microsoft.EntityFrameworkCore.Relational` package. When you add a relational database provider, this package becomes a transitive dependency of your application. But many providers are released independently from EF Core and may not be updated to depend on the newer patch version of that package. In order to make sure you will get all bug fixes, it is recommended that you add the patch version of `Microsoft.EntityFrameworkCore.Relational` as a direct dependency of your application.
