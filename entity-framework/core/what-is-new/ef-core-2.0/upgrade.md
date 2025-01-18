---
title: Upgrading from previous versions to EF Core 2 - EF Core
description: Instructions and notes for upgrading to Entity Framework Core 2.0
author: SamMonoRT
ms.date: 10/25/2021
uid: core/what-is-new/ef-core-2.0/upgrade
---

# Upgrading applications from previous versions to EF Core 2.0

We have taken the opportunity to significantly refine our existing APIs and behaviors in 2.0. There are a few improvements that can require modifying existing application code, although we believe that for the majority of applications the impact will be low, in most cases requiring just recompilation and minimal guided changes to replace obsolete APIs.

Updating an existing application to EF Core 2.0 may require:

1. Upgrading the target .NET implementation of the application to one that supports .NET Standard 2.0. See [Supported .NET Implementations](xref:core/miscellaneous/platforms) for more details.

2. Identify a provider for the target database which is compatible with EF Core 2.0. See [EF Core 2.0 requires a 2.0 database provider](#ef-core-20-requires-a-20-database-provider) below.

3. Upgrading all the EF Core packages (runtime and tooling) to 2.0. Refer to [Installing EF Core](xref:core/get-started/overview/install) for more details.

4. Make any necessary code changes to compensate for the breaking changes described in the rest of this document.

## ASP.NET Core now includes EF Core

Applications targeting ASP.NET Core 2.0 can use EF Core 2.0 without additional dependencies besides third party database providers. However, applications targeting previous versions of ASP.NET Core need to upgrade to ASP.NET Core 2.0 in order to use EF Core 2.0. For more details on upgrading ASP.NET Core applications to 2.0 see [the ASP.NET Core documentation on the subject](/aspnet/core/migration/1x-to-2x/).

## New way of getting application services in ASP.NET Core

The recommended pattern for ASP.NET Core web applications has been updated for 2.0 in a way that broke the design-time logic EF Core used in 1.x. Previously at design-time, EF Core would try to invoke `Startup.ConfigureServices` directly in order to access the application's service provider. In ASP.NET Core 2.0, Configuration is initialized outside of the `Startup` class. Applications using EF Core typically access their connection string from Configuration, so `Startup` by itself is no longer sufficient. If you upgrade an ASP.NET Core 1.x application, you may receive the following error when using the EF Core tools.

> No parameterless constructor was found on 'ApplicationContext'. Either add a parameterless constructor to 'ApplicationContext' or add an implementation of 'IDesignTimeDbContextFactory&lt;ApplicationContext&gt;' in the same assembly as 'ApplicationContext'

A new design-time hook has been added in ASP.NET Core 2.0's default template. The static `Program.BuildWebHost` method enables EF Core to access the application's service provider at design time. If you are upgrading an ASP.NET Core 1.x application, you will need to update the `Program` class to resemble the following.

```csharp
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace AspNetCoreDotNetCore2._0App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
```

The adoption of this new pattern when updating applications to 2.0 is highly recommended and is required in order for product features like Entity Framework Core Migrations to work. The other common alternative is to [implement *IDesignTimeDbContextFactory\<TContext>*](xref:core/cli/dbcontext-creation#from-a-design-time-factory).

## IDbContextFactory renamed

In order to support diverse application patterns and give users more control over how their `DbContext` is used at design time, we have, in the past, provided the `IDbContextFactory<TContext>` interface. At design-time, the EF Core tools will discover implementations of this interface in your project and use it to create `DbContext` objects.

This interface had a very general name which mislead some users to try re-using it for other `DbContext`-creating scenarios. They were caught off guard when the EF Tools then tried to use their implementation at design-time and caused commands like `Update-Database` or `dotnet ef database update` to fail.

In order to communicate the strong design-time semantics of this interface, we have renamed it to `IDesignTimeDbContextFactory<TContext>`.

For the 2.0 release the `IDbContextFactory<TContext>` still exists but is marked as obsolete.

## DbContextFactoryOptions removed

Because of the ASP.NET Core 2.0 changes described above, we found that `DbContextFactoryOptions` was no longer needed on the new `IDesignTimeDbContextFactory<TContext>` interface. Here are the alternatives you should be using instead.

| DbContextFactoryOptions | Alternative                                                  |
|:------------------------|:-------------------------------------------------------------|
| ApplicationBasePath     | AppContext.BaseDirectory                                     |
| ContentRootPath         | Directory.GetCurrentDirectory()                              |
| EnvironmentName         | Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") |

## Design-time working directory changed

The ASP.NET Core 2.0 changes also required the working directory used by `dotnet ef` to align with the working directory used by Visual Studio when running your application. One observable side effect of this is that SQLite filenames are now relative to the project directory and not the output directory like they used to be.

## EF Core 2.0 requires a 2.0 database provider

For EF Core 2.0 we have made many simplifications and improvements in the way database providers work. This means that 1.0.x and 1.1.x providers will not work with EF Core 2.0.

The SQL Server and SQLite providers are shipped by the EF team and 2.0 versions will be available as part of the 2.0 release. The open-source third party providers for [SQL Compact](https://github.com/ErikEJ/EntityFramework.SqlServerCompact), [PostgreSQL](https://github.com/npgsql/Npgsql.EntityFrameworkCore.PostgreSQL), and [MySQL](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql) are being updated for 2.0. For all other providers, please contact the provider writer.

## Logging and Diagnostics events have changed

Note: these changes should not impact most application code.

The event IDs for messages sent to an [ILogger](/dotnet/api/microsoft.extensions.logging.ilogger) have changed in 2.0. The event IDs are now unique across EF Core code. These messages now also follow the standard pattern for structured logging used by, for example, MVC.

Logger categories have also changed. There is now a well-known set of categories accessed through <xref:Microsoft.EntityFrameworkCore.DbLoggerCategory>.

`DiagnosticSource` events now use the same event ID names as the corresponding `ILogger` messages. The event payloads are all nominal types derived from [EventData](/dotnet/api/microsoft.entityframeworkcore.diagnostics.eventdata).

Event IDs, payload types, and categories are documented in the [CoreEventId](/dotnet/api/microsoft.entityframeworkcore.diagnostics.coreeventid) and the [RelationalEventId](/dotnet/api/microsoft.entityframeworkcore.diagnostics.relationaleventid) classes.

IDs have also moved from Microsoft.EntityFrameworkCore.Infrastructure to the new Microsoft.EntityFrameworkCore.Diagnostics namespace.

## EF Core relational metadata API changes

EF Core 2.0 will now build a different [IModel](/dotnet/api/microsoft.entityframeworkcore.metadata.imodel) for each different provider being used. This is usually transparent to the application. This has facilitated a simplification of lower-level metadata APIs such that any access to *common relational metadata concepts* is always made through a call to `.Relational` instead of `.SqlServer`, `.Sqlite`, etc. For example, 1.1.x code like this:

```csharp
var tableName = context.Model.FindEntityType(typeof(User)).SqlServer().TableName;
```

Should now be written like this:

```csharp
var tableName = context.Model.FindEntityType(typeof(User)).Relational().TableName;
```

Instead of using methods like `ForSqlServerToTable`, extension methods are now available to write conditional code based on the current provider in use. For example:

```csharp
modelBuilder.Entity<User>().ToTable(
    Database.IsSqlServer() ? "SqlServerName" : "OtherName");
```

Note that this change only applies to APIs/metadata that is defined for *all* relational providers. The API and metadata remains the same when it is specific to only a single provider. For example, clustered indexes are specific to SQL Sever, so `ForSqlServerIsClustered` and  `.SqlServer().IsClustered()` must still be used.

## Don’t take control of the EF service provider

EF Core uses an internal `IServiceProvider` (a dependency injection container) for its internal implementation. Applications should allow EF Core to create and manage this provider except in special cases. Strongly consider removing any calls to `UseInternalServiceProvider`. If an application does need to call `UseInternalServiceProvider`, then please consider [filing an issue](https://github.com/dotnet/efcore/issues) so we can investigate other ways to handle your scenario.

Calling `AddEntityFramework`, `AddEntityFrameworkSqlServer`, etc. is not required by application code unless `UseInternalServiceProvider` is also called. Remove any existing calls to `AddEntityFramework` or `AddEntityFrameworkSqlServer`, etc. `AddDbContext` should still be used in the same way as before.

## In-memory databases must be named

The global unnamed in-memory database has been removed and instead all in-memory databases must be named. For example:

```csharp
optionsBuilder.UseInMemoryDatabase("MyDatabase");
```

This creates/uses a database with the name “MyDatabase”. If `UseInMemoryDatabase` is called again with the same name, then the same in-memory database will be used, allowing it to be shared by multiple context instances.

## In-memory provider 'Include' operation no longer returns results if the included navigation is required but its value is null

When trying to include a required navigation and the included navigation is null, the query no longer returns result for the entity on which the Include operation is applied. To avoid this problem, either provide a value for the required navigation or change the navigation to be optional.

```csharp
public class Person
{
    public int Id { get; set; }
    public Language NativeLanguage { get; set;} // required navigation
    public Person Sibling { get; set; } // optional navigation
}
...
var person = new Person();
context.People.Add(person);
await context.SaveChangesAsync();
...

// returns one result
await context.People.ToListAsync();

// returns no results because 'NativeLanguage' navigation is required but has not been provided
await context.People.Include(p => p.NativeLanguage).ToListAsync(); 

// returns one result because 'Sibling' navigation is optional so it doesn't have to be provided
await context.People.Include(p => p.Sibling).ToListAsync();
```

## Read-only API changes

`IsReadOnlyBeforeSave`, `IsReadOnlyAfterSave`, and `IsStoreGeneratedAlways` have been obsoleted and replaced with [BeforeSaveBehavior](/dotnet/api/microsoft.entityframeworkcore.metadata.iproperty.beforesavebehavior) and [AfterSaveBehavior](/dotnet/api/microsoft.entityframeworkcore.metadata.iproperty.aftersavebehavior). These behaviors apply to any property (not only store-generated properties) and determine how the value of the property should be used when inserting into a database row (`BeforeSaveBehavior`) or when updating an existing database row (`AfterSaveBehavior`).

Properties marked as [ValueGenerated.OnAddOrUpdate](/dotnet/api/microsoft.entityframeworkcore.metadata.valuegenerated) (for example, for computed columns) will by default ignore any value currently set on the property. This means that a store-generated value will always be obtained regardless of whether any value has been set or modified on the tracked entity. This can be changed by setting a different `Before\AfterSaveBehavior`.

## New ClientSetNull delete behavior

In previous releases, [DeleteBehavior.Restrict](/dotnet/api/microsoft.entityframeworkcore.deletebehavior) had a behavior for entities tracked by the context that more closed matched `SetNull` semantics. In EF Core 2.0, a new `ClientSetNull` behavior has been introduced as the default for optional relationships. This behavior has `SetNull` semantics for tracked entities and `Restrict` behavior for databases created using EF Core. In our experience, these are the most expected/useful behaviors for tracked entities and the database. `DeleteBehavior.Restrict` is now honored for tracked entities when set for optional relationships.

## Provider design-time packages removed

The `Microsoft.EntityFrameworkCore.Relational.Design` package has been removed. It's contents were consolidated into `Microsoft.EntityFrameworkCore.Relational` and `Microsoft.EntityFrameworkCore.Design`.

This propagates into the provider design-time packages. Those packages (`Microsoft.EntityFrameworkCore.Sqlite.Design`, `Microsoft.EntityFrameworkCore.SqlServer.Design`, etc.) were removed and their contents consolidated into the main provider packages.

To enable `Scaffold-DbContext` or `dotnet ef dbcontext scaffold` in EF Core 2.0, you only need to reference the single provider package:

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer"
    Version="2.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools"
    Version="2.0.0"
    PrivateAssets="All" />
<DotNetCliToolReference Include="Microsoft.EntityFrameworkCore.Tools.DotNet"
    Version="2.0.0" />
```
