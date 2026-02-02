---
title: What's New in EF Core 11
description: Overview of new features in EF Core 11
author: roji
ms.date: 02/02/2026
uid: core/what-is-new/ef-core-11.0/whatsnew
---

# What's New in EF Core 11

EF Core 11.0 (EF11) is the next release after EF Core 10.0 and is currently in development.

> [!TIP]
> You can run and debug into the samples by [downloading the sample code from GitHub](https://github.com/dotnet/EntityFramework.Docs). Each section below links to the source code specific to that section.

EF11 requires the .NET 11 SDK to build and requires the .NET 11 runtime to run. EF11 will not run on earlier .NET versions, and will not run on .NET Framework.

## Complex types

<a name="complex-types-tpt-tpc"></a>

### Complex types and JSON columns on entity types with TPT/TPC inheritance

EF Core has supported [complex types](xref:core/modeling/value-properties#complex-types) (previously called owned types) and JSON columns for several versions, allowing you to model and persist nested, structured data within your entities. However, until now these features could not be used on entities with TPT (table-per-type) or TPC (table-per-concrete-type) inheritance.

Starting with EF 11, you can now use complex types and JSON columns on entity types with TPT and TPC inheritance mappings. For example, consider the following entity types with a TPT inheritance strategy:

```csharp
public abstract class Animal
{
    public int Id { get; set; }
    public string Name { get; set; }
    public required AnimalDetails Details { get; set; }
}

public class Dog : Animal
{
    public string Breed { get; set; }
}

public class Cat : Animal
{
    public bool IsIndoor { get; set; }
}

[ComplexType]
public class AnimalDetails
{
    public DateTime BirthDate { get; set; }
    public string? Veterinarian { get; set; }
}
```

With the following TPT mapping configuration:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Animal>().UseTptMappingStrategy();
}
```

EF 11 now properly supports this scenario, creating the `Details_BirthDate` and `Details_Veterinarian` columns on the `Animal` table for the complex type properties.

Similarly, JSON columns are now supported with TPT/TPC. The following configuration maps the `Details` property to a JSON column:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Animal>()
        .UseTptMappingStrategy()
        .ComplexProperty(a => a.Details, b => b.ToJson());
}
```

This enhancement removes a significant limitation when modeling complex domain hierarchies, allowing you to combine the flexibility of TPT/TPC inheritance with the power of complex types and JSON columns.

For more information on inheritance mapping strategies, see [Inheritance](xref:core/modeling/inheritance).

## Cosmos DB

<a name="cosmos-bulk-execution"></a>

### Bulk execution

Azure Cosmos DB supports _bulk execution_, which allows multiple document operations to be executed in parallel and across DbContext instances, significantly improving throughput when saving many entities at once. EF Core now supports enabling bulk execution:

```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder.UseCosmos(
        "<connection string>",
        databaseName: "OrdersDB",
        options => options.BulkExecutionEnabled());
```

For more information, see [Cosmos DB saving documentation](xref:core/providers/cosmos/saving#bulk-execution).

This feature was contributed by [@JoasE](https://github.com/JoasE) - many thanks!

<a name="cosmos-session-tokens"></a>

### Session token management

Azure Cosmos DB uses session tokens to track read-your-writes consistency within a session. When running in an environment with multiple instances (e.g., with round-robin load balancing), you may need to manually manage session tokens to ensure consistency across requests.

EF Core now provides APIs to retrieve and set session tokens on a `DbContext`. To enable manual session token management, configure the <xref:Microsoft.EntityFrameworkCore.Cosmos.Infrastructure.SessionTokenManagementMode>:

```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder.UseCosmos(
        "<connection string>",
        databaseName: "OrdersDB",
        options => options.SessionTokenManagementMode(SessionTokenManagementMode.SemiAutomatic));
```

You can then retrieve and use session tokens:

```csharp
// After performing operations, retrieve the session token
var sessionToken = context.Database.GetSessionToken();

// Later, in a different context instance, apply the session token before reading
context.Database.UseSessionToken(sessionToken);
var result = await context.Documents.FindAsync(id);
```

For more information, see [Cosmos DB saving documentation](xref:core/providers/cosmos/saving#session-token-management).

This feature was contributed by [@JoasE](https://github.com/JoasE) - many thanks!

## Migrations

<a name="migrations-add-and-apply"></a>

### Create and apply migrations in one step

The `dotnet ef database update` command now supports creating and applying a migration in a single step using the new `--add` option. This uses Roslyn to compile the migration at runtime, enabling scenarios like .NET Aspire and containerized applications where recompilation isn't possible:

```dotnetcli
dotnet ef database update InitialCreate --add
```

This command scaffolds a new migration named `InitialCreate`, compiles it using Roslyn, and immediately applies it to the database. The migration files are still saved to disk for source control and future recompilation. The same options available for `dotnet ef migrations add` can be used:

```dotnetcli
dotnet ef database update AddProducts --add --output-dir Migrations/Products --namespace MyApp.Migrations
```

In PowerShell, use the `-Add` parameter:

```powershell
Update-Database -Migration InitialCreate -Add
```
