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

Azure Cosmos DB supports _bulk execution_, which allows multiple document operations to be executed in parallel, significantly improving throughput when saving many entities at once. EF Core now supports enabling bulk execution:

```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder.UseCosmos(
        "<connection string>",
        databaseName: "OrdersDB",
        options => options.BulkExecutionEnabled());
```

For more information, see [Cosmos DB saving documentation](xref:core/providers/cosmos/saving#bulk-execution).

This feature was contributed by [@JoasE](https://github.com/JoasE) - many thanks!
