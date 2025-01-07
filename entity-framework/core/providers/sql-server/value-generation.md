---
title: Microsoft SQL Server Database Provider - Value Generation - EF Core
description: Value Generation Patterns Specific to the SQL Server Entity Framework Core Database Provider
author: roji
ms.date: 11/15/2021
uid: core/providers/sql-server/value-generation
---
# SQL Server Value Generation

This page details value generation configuration  and patterns that are specific to the SQL Server provider. It's recommended to first read [the general page on value generation](xref:core/modeling/generated-properties).

## IDENTITY columns

By convention, numeric columns that are configured to have their values generated on add are set up as [SQL Server IDENTITY columns](/sql/t-sql/statements/create-table-transact-sql-identity-property).

### Seed and increment

By default, IDENTITY columns start off at 1 (the seed), and increment by 1 each time a row is added (the increment). You can configure a different seed and increment as follows:

[!code-csharp[Main](../../../../samples/core/SqlServer/ValueGeneration/IdentityOptionsContext.cs?name=IdentityOptions&highlight=5)]

### Inserting explicit values into IDENTITY columns

By default, SQL Server doesn't allow inserting explicit values into IDENTITY columns. To do so, you must manually enable `IDENTITY_INSERT` before calling `SaveChangesAsync()`, as follows:

[!code-csharp[Main](../../../../samples/core/SqlServer/ValueGeneration/ExplicitIdentityValues.cs?name=ExplicitIdentityValues)]

> [!NOTE]
> We have a [feature request](https://github.com/aspnet/EntityFramework/issues/703) on our backlog to do this automatically within the SQL Server provider.

## Sequences

As an alternative to IDENTITY columns, you can use standard sequences. This can be useful in various scenarios; for example, you may want to have multiple columns drawing their default values from a single sequence.

SQL Server allows you to create sequences and use them as detailed in [the general page on sequences](xref:core/modeling/sequences). It's up to you to configure your properties to use sequences via `HasDefaultValueSql()`.

## GUIDs

For GUID primary keys, the provider automatically generates optimal sequential values, similar to SQL Server's [NEWSEQUENTIALID](/sql/t-sql/functions/newsequentialid-transact-sql) function. Generating the value on the client is more efficient in some scenarios, i.e. an extra database round trip isn't needed to get the database-generated value, when a dependent is also being inserted that references that key.

To have EF generate the same sequential GUID values for non-key properties, configure them as follows:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Blog>().Property(b => b.Guid).HasValueGenerator(typeof(SequentialGuidValueGenerator));
}
```

## Rowversions

SQL Server has the [`rowversion`](/sql/t-sql/data-types/rowversion-transact-sql) data type, which automatically changes whenever the row is updated. This makes it very useful as a concurrency token, for managing cases where the same row is simultaneously updated by multiple transactions.

To fully understand concurrency tokens and how to use them, read the dedicated page on [concurrency conflicts](xref:core/saving/concurrency). To map a `byte[]` property to a `rowversion` column, configure it as follows:

### [Data Annotations](#tab/data-annotations)

```c#
public class Person
{
    public int PersonId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    [Timestamp]
    public byte[] Version { get; set; }
}
```

### [Fluent API](#tab/fluent-api)

```c#
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Person>()
        .Property(p => p.Version)
        .IsRowVersion();
}
```

***
