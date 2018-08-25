---
title: Upgrading from EF Core 1.0 RC1 to RC2 - EF Core
author: rowanmiller
ms.date: 10/27/2016
ms.assetid: 6d75b229-cc79-4d08-88cd-3a1c1b24d88f
uid: core/miscellaneous/rc1-rc2-upgrade
---
# Upgrading from EF Core 1.0 RC1 to 1.0 RC2

This article provides guidance for moving an application built with the RC1 packages to RC2.

## Package Names and Versions

Between RC1 and RC2, we changed from "Entity Framework 7" to "Entity Framework Core". You can read more about the reasons for the change in [this post by Scott Hanselman](http://www.hanselman.com/blog/ASPNET5IsDeadIntroducingASPNETCore10AndNETCore10.aspx). Because of this change, our package names changed from `EntityFramework.*` to `Microsoft.EntityFrameworkCore.*` and our versions from `7.0.0-rc1-final` to `1.0.0-rc2-final` (or `1.0.0-preview1-final` for tooling).

**You will need to completely remove the RC1 packages and then install the RC2 ones.** Here is the mapping for some common packages.

| RC1 Package                                               | RC2 Equivalent                                                       |
|:----------------------------------------------------------|:---------------------------------------------------------------------|
| EntityFramework.MicrosoftSqlServer        7.0.0-rc1-final | Microsoft.EntityFrameworkCore.SqlServer         1.0.0-rc2-final      |
| EntityFramework.SQLite                    7.0.0-rc1-final | Microsoft.EntityFrameworkCore.Sqlite            1.0.0-rc2-final      |
| EntityFramework7.Npgsql                   3.1.0-rc1-3     | NpgSql.EntityFrameworkCore.Postgres             <to be advised>      |
| EntityFramework.SqlServerCompact35        7.0.0-rc1-final | EntityFrameworkCore.SqlServerCompact35          1.0.0-rc2-final      |
| EntityFramework.SqlServerCompact40        7.0.0-rc1-final | EntityFrameworkCore.SqlServerCompact40          1.0.0-rc2-final      |
| EntityFramework.InMemory                  7.0.0-rc1-final | Microsoft.EntityFrameworkCore.InMemory          1.0.0-rc2-final      |
| EntityFramework.IBMDataServer             7.0.0-beta1     | Not yet available for RC2                                            |
| EntityFramework.Commands                  7.0.0-rc1-final | Microsoft.EntityFrameworkCore.Tools             1.0.0-preview1-final |
| EntityFramework.MicrosoftSqlServer.Design 7.0.0-rc1-final | Microsoft.EntityFrameworkCore.SqlServer.Design  1.0.0-rc2-final      |

## Namespaces

Along with package names, namespaces changed from `Microsoft.Data.Entity.*` to `Microsoft.EntityFrameworkCore.*`. You can handle this change with a find/replace of `using Microsoft.Data.Entity` with `using Microsoft.EntityFrameworkCore`.

## Table Naming Convention Changes

A significant functional change we took in RC2 was to use the name of the `DbSet<TEntity>` property for a given entity as the table name it maps to, rather than just the class name. You can read more about this change in [the related announcement issue](https://github.com/aspnet/Announcements/issues/167).

For existing RC1 applications, we recommend adding the following code to the start of your `OnModelCreating` method to keep the RC1 naming strategy:

``` csharp
foreach (var entity in modelBuilder.Model.GetEntityTypes())
{
    entity.Relational().TableName = entity.DisplayName();
}
```

If you want to adopt the new naming strategy, we would recommend successfully completing the rest of the upgrade steps and then removing the code and creating a migration to apply the table renames.

## AddDbContext / Startup.cs Changes (ASP.NET Core Projects Only)

In RC1, you had to add Entity Framework services to the application service provider - in `Startup.ConfigureServices(...)`:

``` csharp
services.AddEntityFramework()
  .AddSqlServer()
  .AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(Configuration["ConnectionStrings:DefaultConnection"]));
```

In RC2, you can remove the calls to `AddEntityFramework()`, `AddSqlServer()`, etc.:

``` csharp
services.AddDbContext<ApplicationDbContext>(options =>
  options.UseSqlServer(Configuration["ConnectionStrings:DefaultConnection"]));
```

You also need to add a constructor, to your derived context, that takes context options and passes them to the base constructor. This is needed because we removed some of the scary magic that snuck them in behind the scenes:

``` csharp
public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
{
}
```

## Passing in an IServiceProvider

If you have RC1 code that passes an `IServiceProvider` to the context, this has now moved to `DbContextOptions`, rather than being a separate constructor parameter. Use `DbContextOptionsBuilder.UseInternalServiceProvider(...)` to set the service provider.

### Testing

The most common scenario for doing this was to control the scope of an InMemory database when testing. See the updated [Testing](testing/index.md) article for an example of doing this with RC2.

### Resolving Internal Services from Application Service Provider (ASP.NET Core Projects Only)

If you have an ASP.NET Core application and you want EF to resolve internal services from the application service provider, there is an overload of `AddDbContext` that allows you to configure this:

``` csharp
services.AddEntityFrameworkSqlServer()
  .AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
    options.UseSqlServer(Configuration["ConnectionStrings:DefaultConnection"])
           .UseInternalServiceProvider(serviceProvider)); );
```

> [!WARNING]  
> We recommend allowing EF to internally manage its own services, unless you have a reason to combine the internal EF services into your application service provider. The main reason you may want to do this is to use your application service provider to replace services that EF uses internally

## DNX Commands => .NET CLI (ASP.NET Core Projects Only)

If you previously used the `dnx ef` commands for ASP.NET 5 projects, these have now moved to `dotnet ef` commands. The same command syntax still applies. You can use `dotnet ef --help` for syntax information.

The way commands are registered has changed in RC2, due to DNX being replaced by .NET CLI. Commands are now registered in a `tools` section in `project.json`:

``` json
"tools": {
  "Microsoft.EntityFrameworkCore.Tools": {
    "version": "1.0.0-preview1-final",
    "imports": [
      "portable-net45+win8+dnxcore50",
      "portable-net45+win8"
    ]
  }
}
```

> [!TIP]  
> If you use Visual Studio, you can now use Package Manager Console to run EF commands for ASP.NET Core projects (this was not supported in RC1). You still need to register the commands in the `tools` section of `project.json` to do this.

## Package Manager Commands Require PowerShell 5

If you use the Entity Framework commands in Package Manager Console in Visual Studio, then you will need to ensure you have PowerShell 5 installed. This is a temporary requirement that will be removed in the next release (see [issue #5327](https://github.com/aspnet/EntityFramework/issues/5327) for more details).

## Using "imports" in project.json

Some of EF Core's dependencies do not support .NET Standard yet. EF Core in .NET Standard and .NET Core projects may require adding "imports" to project.json as a temporary workaround.

When adding EF, NuGet restore will display this error message:

``` Console
Package Ix-Async 1.2.5 is not compatible with netcoreapp1.0 (.NETCoreApp,Version=v1.0). Package Ix-Async 1.2.5 supports:
  - net40 (.NETFramework,Version=v4.0)
  - net45 (.NETFramework,Version=v4.5)
  - portable-net45+win8+wp8 (.NETPortable,Version=v0.0,Profile=Profile78)
Package Remotion.Linq 2.0.2 is not compatible with netcoreapp1.0 (.NETCoreApp,Version=v1.0). Package Remotion.Linq 2.0.2 supports:
  - net35 (.NETFramework,Version=v3.5)
  - net40 (.NETFramework,Version=v4.0)
  - net45 (.NETFramework,Version=v4.5)
  - portable-net45+win8+wp8+wpa81 (.NETPortable,Version=v0.0,Profile=Profile259)
```

The workaround is to manually import the portable profile "portable-net451+win8". This forces NuGet to treat this binaries that match this provide as a compatible framework with .NET Standard, even though they are not. Although "portable-net451+win8" is not 100% compatible with .NET Standard, it is compatible enough for the transition from PCL to .NET Standard. Imports can be removed when EF's dependencies eventually upgrade to .NET Standard.

Multiple frameworks can be added to "imports" in array syntax. Other imports may be necessary if you add additional libraries to your project.

``` json
{
  "frameworks": {
    "netcoreapp1.0": {
      "imports": ["dnxcore50", "portable-net451+win8"]
    }
  }
}
```

See [Issue #5176](https://github.com/aspnet/EntityFramework/issues/5176).
