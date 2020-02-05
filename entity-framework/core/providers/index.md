---
title: Database Providers - EF Core
author: ajcvickers
ms.date: 12/17/2019
uid: core/providers/index
---

# Database Providers

Entity Framework Core can access many different databases through plug-in libraries called database providers.

## Current providers

> [!IMPORTANT]  
> EF Core providers are built by a variety of sources. Not all providers are maintained as part of the [Entity Framework Core Project](https://github.com/aspnet/EntityFrameworkCore). When considering a provider, be sure to evaluate quality, licensing, support, etc. to ensure they meet your requirements. Also make sure you review each provider's documentation for detailed version compatibility information.

> [!IMPORTANT]  
> EF Core providers typically work across minor versions, but not across major versions. For example, a provider released for EF Core 2.1 should work with EF Core 2.2, but will not work with EF Core 3.0. 

| NuGet Package                                                                                                        | Supported database engines | Maintainer / Vendor                                                           | Notes / Requirements | Built for version | Useful links                                                                                                                                                                                       |
|:---------------------------------------------------------------------------------------------------------------------|:---------------------------|:------------------------------------------------------------------------------|:---------------------|:------------------|:---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| [Microsoft.EntityFrameworkCore.SqlServer](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.SqlServer)    | SQL Server 2012 onwards    | [EF Core Project](https://github.com/aspnet/EntityFrameworkCore/) (Microsoft) |                      | 3.1               | [docs](xref:core/providers/sql-server/index)                                                                                                                                                       |
| [Microsoft.EntityFrameworkCore.Sqlite](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Sqlite)          | SQLite 3.7 onwards         | [EF Core Project](https://github.com/aspnet/EntityFrameworkCore/) (Microsoft) |                      | 3.1               | [docs](xref:core/providers/sqlite/index)                                                                                                                                                           |
| [Microsoft.EntityFrameworkCore.InMemory](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.InMemory)      | EF Core in-memory database | [EF Core Project](https://github.com/aspnet/EntityFrameworkCore/) (Microsoft) | [Limitations](xref:core/miscellaneous/testing/in-memory)                 | 3.1               | [docs](xref:core/providers/in-memory/index)                                                                                                                                                        |
| [Microsoft.EntityFrameworkCore.Cosmos](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Cosmos)          | Azure Cosmos DB SQL API    | [EF Core Project](https://github.com/aspnet/EntityFrameworkCore/) (Microsoft) |                      | 3.1               | [docs](xref:core/providers/cosmos/index)                                                                                                                                                           |
| [Npgsql.EntityFrameworkCore.PostgreSQL](https://www.nuget.org/packages/Npgsql.EntityFrameworkCore.PostgreSQL)        | PostgreSQL                 | [Npgsql Development Team](https://github.com/npgsql)                          |                      | 3.1               | [docs](https://www.npgsql.org/efcore/index.html)                                                                                                                                                   |
| [Pomelo.EntityFrameworkCore.MySql](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql)                  | MySQL, MariaDB             | [Pomelo Foundation Project](https://github.com/PomeloFoundation)              |                      | 3.1               | [readme](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/blob/master/README.md)                                                                                               |
| [Devart.Data.MySql.EFCore](https://www.nuget.org/packages/Devart.Data.MySql.EFCore/)                                 | MySQL 5 onwards            | [DevArt](https://www.devart.com/)                                             | Paid                 | 3.0               | [docs](https://www.devart.com/dotconnect/mysql/docs/)                                                                                                                                              |
| [Devart.Data.Oracle.EFCore](https://www.nuget.org/packages/Devart.Data.Oracle.EFCore/)                               | Oracle DB 9.2.0.4 onwards  | [DevArt](https://www.devart.com/)                                             | Paid                 | 3.0               | [docs](https://www.devart.com/dotconnect/oracle/docs/)                                                                                                                                             |
| [Devart.Data.PostgreSql.EFCore](https://www.nuget.org/packages/Devart.Data.PostgreSql.EFCore/)                       | PostgreSQL 8.0 onwards     | [DevArt](https://www.devart.com/)                                             | Paid                 | 3.0               | [docs](https://www.devart.com/dotconnect/postgresql/docs/)                                                                                                                                         |
| [Devart.Data.SQLite.EFCore](https://www.nuget.org/packages/Devart.Data.SQLite.EFCore/)                               | SQLite 3 onwards           | [DevArt](https://www.devart.com/)                                             | Paid                 | 3.0               | [docs](https://www.devart.com/dotconnect/sqlite/docs/)                                                                                                                                             |
| [FileContextCore](https://www.nuget.org/packages/FileContextCore/)                                                   | Stores data in files       | [Morris Janatzek](https://github.com/morrisjdev)                              | For development purposes | 3.0               | [readme](https://github.com/morrisjdev/FileContextCore/blob/master/README.md)                                                                                                                                              |
| [EntityFrameworkCore.Jet](https://www.nuget.org/packages/EntityFrameworkCore.Jet/)                                   | Microsoft Access files     | [Bubi](https://github.com/bubibubi)                                           | .NET Framework       | 2.2               | [readme](https://github.com/bubibubi/EntityFrameworkCore.Jet/blob/master/docs/README.md)                                                                                                           |
| [EntityFrameworkCore.SqlServerCompact35](https://www.nuget.org/packages/EntityFrameworkCore.SqlServerCompact35)      | SQL Server Compact 3.5     | [Erik Ejlskov Jensen](https://github.com/ErikEJ/)                             | .NET Framework       | 2.2               | [wiki](https://github.com/ErikEJ/EntityFramework.SqlServerCompact/wiki/Using-EF-Core-with-SQL-Server-Compact-in-Traditional-.NET-Applications)                                                     |
| [EntityFrameworkCore.SqlServerCompact40](https://www.nuget.org/packages/EntityFrameworkCore.SqlServerCompact40)      | SQL Server Compact 4.0     | [Erik Ejlskov Jensen](https://github.com/ErikEJ/)                             | .NET Framework       | 2.2               | [wiki](https://github.com/ErikEJ/EntityFramework.SqlServerCompact/wiki/Using-EF-Core-with-SQL-Server-Compact-in-Traditional-.NET-Applications)                                                     |
| [FirebirdSql.EntityFrameworkCore.Firebird](https://www.nuget.org/packages/FirebirdSql.EntityFrameworkCore.Firebird/) | Firebird 2.5 and 3.x       | [Jiří Činčura](https://github.com/cincuranet)                                 |                      | 2.2               | [docs](https://github.com/cincuranet/FirebirdSql.Data.FirebirdClient/blob/master/Provider/docs/entity-framework-core.md)                                                                           |
| [Teradata.EntityFrameworkCore](https://www.nuget.org/packages/Teradata.EntityFrameworkCore/)                         | Teradata Database 16.10 onwards | [Teradata](https://downloads.teradata.com/download/connectivity/net-data-provider-for-teradata) | | 2.2               |[website](https://www.nuget.org/packages/Teradata.EntityFrameworkCore/)                                                                                                                            |
| [EntityFrameworkCore.FirebirdSql](https://www.nuget.org/packages/EntityFrameworkCore.FirebirdSql/)                   | Firebird 2.5 and 3.x       | [Rafael Almeida](https://github.com/ralmsdeveloper)                           |                      | 2.1               | [wiki](https://github.com/ralmsdeveloper/EntityFrameworkCore.FirebirdSQL/wiki)                                                                                                                     |
| [EntityFrameworkCore.OpenEdge](https://www.nuget.org/packages/EntityFrameworkCore.OpenEdge/)                         | Progress OpenEdge          | [Alex Wiese](https://github.com/alexwiese)                                    |                      | 2.1               | [readme](https://github.com/alexwiese/EntityFrameworkCore.OpenEdge/blob/master/README.md)                                                                                                          |
| [MySql.Data.EntityFrameworkCore](https://www.nuget.org/packages/MySql.Data.EntityFrameworkCore)                      | MySQL                      | [MySQL project](https://dev.mysql.com) (Oracle)                               |                      | 2.1               | [docs](https://dev.mysql.com/doc/connector-net/en/connector-net-entityframework-core.html)                                                                                                         |
| [Oracle.EntityFrameworkCore](https://www.nuget.org/packages/Oracle.EntityFrameworkCore/)                             | Oracle DB 11.2 onwards     | [Oracle](https://www.oracle.com/technetwork/topics/dotnet/)                   |                      | 2.1               | [website](https://www.oracle.com/technetwork/topics/dotnet/)                                                                                                                                       |
| [IBM.EntityFrameworkCore](https://www.nuget.org/packages/IBM.EntityFrameworkCore)                                    | Db2, Informix              | [IBM](https://ibm.com)                                                        | Windows version      | 2.0               | [blog](https://www.ibm.com/developerworks/community/blogs/96960515-2ea1-4391-8170-b0515d08e4da/entry/Creating_Entity_Data_Model_using_IBM_Data_Server_providers_for_Entity_Framework_Core?lang=en) |
| [IBM.EntityFrameworkCore-lnx](https://www.nuget.org/packages/IBM.EntityFrameworkCore-lnx)                            | Db2, Informix              | [IBM](https://ibm.com)                                                        | Linux version        | 2.0               | [blog](https://www.ibm.com/developerworks/community/blogs/96960515-2ea1-4391-8170-b0515d08e4da/entry/Creating_Entity_Data_Model_using_IBM_Data_Server_providers_for_Entity_Framework_Core?lang=en) |
| [IBM.EntityFrameworkCore-osx](https://www.nuget.org/packages/IBM.EntityFrameworkCore-osx)                            | Db2, Informix              | [IBM](https://ibm.com)                                                        | macOS version        | 2.0               | [blog](https://www.ibm.com/developerworks/community/blogs/96960515-2ea1-4391-8170-b0515d08e4da/entry/Creating_Entity_Data_Model_using_IBM_Data_Server_providers_for_Entity_Framework_Core?lang=en) |
| [Pomelo.EntityFrameworkCore.MyCat](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MyCat)                  | MyCAT Server               | [Pomelo Foundation Project](https://github.com/PomeloFoundation)              | Prerelease only      | 1.1               | [readme](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MyCat/blob/master/README.md)                                                                                               |

## Adding a database provider to your application

Most database providers for EF Core are distributed as NuGet packages, and can be installed as follows:

## [.NET Core CLI](#tab/dotnet-core-cli)

```dotnetcli
dotnet add package provider_package_name
```

## [Visual Studio](#tab/vs)

``` powershell
install-package provider_package_name
```

***

Once installed, you will configure the provider in your `DbContext`, either in the `OnConfiguring` method or in the `AddDbContext` method if you are using a dependency injection container.
For example, the following line configures the SQL Server provider with the passed connection string:

``` csharp
optionsBuilder.UseSqlServer(
    "Server=(localdb)\mssqllocaldb;Database=MyDatabase;Trusted_Connection=True;");
```  

Database providers can extend EF Core to enable functionality unique to specific databases.
Some concepts are common to most databases, and are included in the primary EF Core components.
Such concepts include expressing queries in LINQ, transactions, and tracking changes to objects once they are loaded from the database.
Some concepts are specific to a particular provider.
For example, the SQL Server provider allows you to [configure memory-optimized tables](xref:core/providers/sql-server/memory-optimized-tables) (a feature specific to SQL Server).
Other concepts are specific to a class of providers.
For example, EF Core providers for relational databases build on the common `Microsoft.EntityFrameworkCore.Relational` library, which provides APIs for configuring table and column mappings, foreign key constraints, etc.
Providers are usually distributed as NuGet packages.

> [!IMPORTANT]  
> When a new patch version of EF Core is released, it often includes updates to the `Microsoft.EntityFrameworkCore.Relational` package.
> When you add a relational database provider, this package becomes a transitive dependency of your application.
> But many providers are released independently from EF Core and may not be updated to depend on the newer patch version of that package.
> In order to make sure you will get all bug fixes, it is recommended that you add the patch version of `Microsoft.EntityFrameworkCore.Relational` as a direct dependency of your application.
