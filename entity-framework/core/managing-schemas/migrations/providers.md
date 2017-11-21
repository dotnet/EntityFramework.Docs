---
title: Migrations with Multiple Providers - EF Core
author: bricelam
ms.author: bricelam
ms.date: 11/8/2017
ms.technology: entity-framework-core
---
Migrations with Multiple Providers
==================================
The [EF Core Tools][1] only scaffold migrations for the active provider. Sometimes, however, you may want to use more
than one provider (for example Microsoft SQL Server and SQLite) with your DbContext. There are two ways to handle
this with Migrations. You can maintain two sets of migrations--one for each provider--or merge them into a single set
that can work on both.

Two migration sets
------------------
In the first approach, you generate two migrations for each model change.

One way to do this is to put each migration set [in a separate assembly][2] and manually switch the active provider (and
migrations assembly) between adding the two migrations.

Another approach that makes working with the tools easier is to create a new type derives from your DbContext and
overrides the active provider. This type is used at design time when adding or applying migrations.

``` csharp
class MySqliteDbContext : MyDbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=my.db");
}
```

> [!NOTE]
> Since each migration set uses its own DbContext types, this approach doesn't require using a separate migrations
> assembly.

When adding new migration, specify the context types.

``` powershell
Add-Migration InitialCreate -Context MyDbContext -OutputDir Migrations\SqlServerMigrations
Add-Migration InitialCreate -Context MySqliteDbContext -OutputDir Migrations\SqliteMigrations
```
``` Console
dotnet ef migrations add InitialCreate --context MyDbContext --output-dir Migrations/SqlServerMigrations
dotnet ef migrations add InitialCreate --context MySqliteDbContext --output-dir Migrations/SqliteMigrations
```

> [!TIP]
> You don't need to specify the output directory for subsequent migrations since they are created as siblings to the
> last one.

One migration set
-----------------
If you don't like having two sets of migrations, you can manually combine them into a single set that can be applied to
both providers.

Annotations can coexist since a provider ignores any annotations that it doesn't understand. For example, a primary
key column that works with both Microsoft SQL Server and SQLite might look like this.

``` csharp
Id = table.Column<int>(nullable: false)
    .Annotation("SqlServer:ValueGenerationStrategy",
        SqlServerValueGenerationStrategy.IdentityColumn)
    .Annotation("Sqlite:Autoincrement", true),
```

If operations can only be applied on one provider (or they're differently between providers), use the `ActiveProvider`
property to tell which provider is active.

``` csharp
if (migrationBuilder.ActiveProvider == "Microsoft.EntityFrameworkCore.SqlServer")
{
    migrationBuilder.CreateSequence(
        name: "EntityFrameworkHiLoSequence");
}
```


  [1]: ../../miscellaneous/cli/index.md
  [2]: projects.md
