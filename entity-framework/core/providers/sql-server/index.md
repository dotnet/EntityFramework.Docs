---
title: Microsoft SQL Server Database Provider - EF Core
description: Documentation for the database provider that allows Entity Framework Core to be used with Microsoft SQL Server
author: AndriySvyryd
ms.date: 11/15/2021
uid: core/providers/sql-server/index
---
# Microsoft SQL Server EF Core Database Provider

This database provider allows Entity Framework Core to be used with Microsoft SQL Server (including Azure SQL and Azure Synapse Analytics). The provider is maintained as part of the [Entity Framework Core Project](https://github.com/dotnet/efcore).

## Install

Install the [Microsoft.EntityFrameworkCore.SqlServer NuGet package](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.SqlServer/).

### [.NET CLI](#tab/dotnet-core-cli)

```dotnetcli
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

### [Visual Studio](#tab/vs)

```powershell
Install-Package Microsoft.EntityFrameworkCore.SqlServer
```

***

## Usage and configuration

Once your project references the nuget package, configure EF for SQL Server as follows:

### [SQL Server (on-premises)](#tab/sqlserver)

```c#
public class MyContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("<CONNECTION STRING>");
    }
}
```

When using EF with dependency injection (e.g. ASP.NET), use the following:

```c#
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MyContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyContext")));
```

### [Azure SQL](#tab/azure-sql)

```c#
public class MyContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseAzureSql("<CONNECTION STRING>");
    }
}
```

When using EF with dependency injection (e.g. ASP.NET), use the following:

```c#
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MyContext>(options =>
    options.UseAzureSql(builder.Configuration.GetConnectionString("MyContext")));
```

> [!NOTE]
> <xref:Microsoft.EntityFrameworkCore.SqlServerDbContextOptionsExtensions.UseAzureSql*> was introduced in EF Core 9.0. When using an older version, use <xref:Microsoft.EntityFrameworkCore.SqlServerDbContextOptionsExtensions.UseSqlServer*> instead.

### [Azure Synapse Analytics](#tab/azure-synapse)

```c#
public class MyContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseAzureSynapse("<CONNECTION STRING>");
    }
}
```

When using EF with dependency injection (e.g. ASP.NET), use the following:

```c#
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MyContext>(options =>
    options.UseAzureSynapse(builder.Configuration.GetConnectionString("MyContext")));
```

> [!NOTE]
> <xref:Microsoft.EntityFrameworkCore.SqlServerDbContextOptionsExtensions.UseAzureSynapse*> was introduced in EF Core 9.0. When using an older version, use <xref:Microsoft.EntityFrameworkCore.SqlServerDbContextOptionsExtensions.UseSqlServer*> instead.

***

The EF SQL Server provider uses [Microsoft.Data.SqlClient](/sql/connect/ado-net/overview-sqlclient-driver) as its underlying ADO.NET provider. For more information on the connection strings accepted by SqlClient, [see this page](/sql/connect/ado-net/connection-string-syntax).

## Compatibility level

You can optionally configure EF with the compatibility level of your database; higher compatibility levels allow for newer features, and configuring EF accordingly makes it use those features. If you do not explicitly configure a compatibility level, a reasonable default will be chosen that may not take advantage of the newest features. As a result, it's recommended to explicitly configure the compatibility level you'd like to have.

Note that this only covers EF's own configuration of the compatibility level - affecting e.g. the SQL it generates - but does not affect the compatibility level configured in your actual database. Databases hosted on newer versions of SQL Server may still be configured with lower compatibility levels, causing them to not support the latest features - so you may need to change the compatibility level in your database as well. For more information on compatibility levels, [see the documentation](/sql/relational-databases/databases/view-or-change-the-compatibility-level-of-a-database).

To configure EF with a compatibility level, use `UseCompatibilityLevel()` as follows:

```c#
optionsBuilder.UseSqlServer("<CONNECTION STRING>", o => o.UseCompatibilityLevel(170));
```

## Connection resiliency

EF includes functionality for automatically retrying failed database commands; for more information, [see the documentation](xref:core/miscellaneous/connection-resiliency). When using <xref:Microsoft.EntityFrameworkCore.SqlServerDbContextOptionsExtensions.UseAzureSql*> and <xref:Microsoft.EntityFrameworkCore.SqlServerDbContextOptionsExtensions.UseAzureSynapse*>, connection resiliency is automatically set up with the appropriate settings specific for those databases. Otherwise, when using <xref:Microsoft.EntityFrameworkCore.SqlServerDbContextOptionsExtensions.UseSqlServer*>, configure the provider with <xref:Microsoft.EntityFrameworkCore.Infrastructure.SqlEngineDbContextOptionsBuilder.EnableRetryOnFailure*> as shown in the connection resiliency documentation.

In some cases, <xref:Microsoft.EntityFrameworkCore.SqlServerDbContextOptionsExtensions.UseSqlServer*> may be called in code that you cannot control. Starting with EF 9, to enable connection resiliency in such scenarios, call `ConfigureSqlEngine(c => c.EnableRetryOnFailureByDefault())` beforehand (this is not necessary with <xref:Microsoft.EntityFrameworkCore.SqlServerDbContextOptionsExtensions.UseAzureSql*> and <xref:Microsoft.EntityFrameworkCore.SqlServerDbContextOptionsExtensions.UseAzureSynapse*>).

## Notes and caveats

* The Microsoft.Data.SqlClient package ships more frequently than the EF Core provider. If you would like to take advantage of new features and bug fixes, you can add a direct package reference to the latest version of Microsoft.Data.SqlClient.
* The EF SQL Server provider uses Microsoft.Data.SqlClient, and not the older System.Data.Client; if your project takes a direct dependency on SqlClient, make sure it references the Microsoft.Data.SqlClient package. For more information on the differences between Microsoft.Data.SqlClient and System.Data.SqlClient, [see this blog post](https://devblogs.microsoft.com/dotnet/introducing-the-new-microsoftdatasqlclient).

## Supported database engines

* Microsoft SQL Server (2019 onwards)
* Azure SQL Database
* Azure Synapse Analytics
