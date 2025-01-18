---
title: What is new in EF Core 2.1 - EF Core
description: Changes and improvements in Entity Framework Core 2.1
author: SamMonoRT
ms.date: 02/20/2018
uid: core/what-is-new/ef-core-2.1
---

# New features in EF Core 2.1

Besides numerous bug fixes and small functional and performance enhancements, EF Core 2.1 includes some compelling new features:

## Lazy loading

EF Core now contains the necessary building blocks for anyone to author entity classes that can load their navigation properties on demand. We have also created a new package, Microsoft.EntityFrameworkCore.Proxies, that leverages those building blocks to produce lazy loading proxy classes based on minimally modified entity classes (for example, classes with virtual navigation properties).

Read the [section on lazy loading](xref:core/querying/related-data#lazy-loading) for more information about this topic.

## Parameters in entity constructors

As one of the required building blocks for lazy loading, we enabled the creation of entities that take parameters in their constructors. You can use parameters to inject property values, lazy loading delegates, and services.

Read the [section on entity constructor with parameters](xref:core/modeling/constructors) for more information about this topic.

## Value conversions

Until now, EF Core could only map properties of types natively supported by the underlying database provider. Values were copied back and forth between columns and properties without any transformation. Starting with EF Core 2.1, value conversions can be applied to transform the values obtained from columns before they are applied to properties, and vice versa. We have a number of conversions that can be applied by convention as necessary, as well as an explicit configuration API that allows registering custom conversions between columns and properties. Some of the application of this feature are:

- Storing enums as strings
- Mapping unsigned integers with SQL Server
- Automatic encryption and decryption of property values

Read the [section on value conversions](xref:core/modeling/value-conversions) for more information about this topic.

## LINQ GroupBy translation

Before version 2.1, in EF Core the GroupBy LINQ operator would always be evaluated in memory. We now support translating it to the SQL GROUP BY clause in most common cases.

This example shows a query with GroupBy used to compute various aggregate functions:

```csharp
var query = context.Orders
    .GroupBy(o => new { o.CustomerId, o.EmployeeId })
    .Select(g => new
        {
          g.Key.CustomerId,
          g.Key.EmployeeId,
          Sum = g.Sum(o => o.Amount),
          Min = g.Min(o => o.Amount),
          Max = g.Max(o => o.Amount),
          Avg = g.Average(o => o.Amount)
        });
```

The corresponding SQL translation looks like this:

```sql
SELECT [o].[CustomerId], [o].[EmployeeId],
    SUM([o].[Amount]), MIN([o].[Amount]), MAX([o].[Amount]), AVG([o].[Amount])
FROM [Orders] AS [o]
GROUP BY [o].[CustomerId], [o].[EmployeeId];
```

## Data Seeding

With the new release it will be possible to provide initial data to populate a database. Unlike in EF6, seeding data is associated to an entity type as part of the model configuration. Then EF Core migrations can automatically compute what insert, update or delete operations need to be applied when upgrading the database to a new version of the model.

As an example, you can use this to configure seed data for a Post in `OnModelCreating`:

```csharp
modelBuilder.Entity<Post>().HasData(new Post{ Id = 1, Text = "Hello World!" });
```

Read the [section on data seeding](xref:core/modeling/data-seeding) for more information about this topic.

## Query types

An EF Core model can now include query types. Unlike entity types, query types do not have keys defined on them and cannot be inserted, deleted or updated (that is, they are read-only), but they can be returned directly by queries. Some of the usage scenarios for query types are:

- Mapping to views without primary keys
- Mapping to tables without primary keys
- Mapping to queries defined in the model
- Serving as the return type for `FromSql()` queries

Read the [section on query types](xref:core/modeling/keyless-entity-types) for more information about this topic.

## Include for derived types

It will be now possible to specify navigation properties only defined on derived types when writing expressions for the `Include` method. For the strongly typed version of `Include`, we support using either an explicit cast or the `as` operator. We also now support referencing the names of navigation property defined on derived types in the string version of `Include`:

```csharp
var option1 = context.People.Include(p => ((Student)p).School);
var option2 = context.People.Include(p => (p as Student).School);
var option3 = context.People.Include("School");
```

Read the [section on Include with derived types](xref:core/querying/related-data#include-on-derived-types) for more information about this topic.

## System.Transactions support

We have added the ability to work with System.Transactions features such as TransactionScope. This will work on both .NET Framework and .NET Core when using database providers that support it.

Read the [section on System.Transactions](xref:core/saving/transactions#using-systemtransactions) for more information about this topic.

## Better column ordering in initial migration

Based on customer feedback, we have updated migrations to initially generate columns for tables in the same order as properties are declared in classes. Note that EF Core cannot change order when new members are added after the initial table creation.

## Optimization of correlated subqueries

We have improved our query translation to avoid executing "N + 1" SQL queries in many common scenarios in which the usage of a navigation property in the projection leads to joining data from the root query with data from a correlated subquery. The optimization requires buffering the results from the subquery, and we require that you modify the query to opt-in the new behavior.

As an example, the following query normally gets translated into one query for Customers, plus N (where "N" is the number of customers returned) separate queries for Orders:

```csharp
var query = context.Customers.Select(
    c => c.Orders.Where(o => o.Amount  > 100).Select(o => o.Amount));
```

By including `ToListAsync()` in the right place, you indicate that buffering is appropriate for the Orders, which enable the optimization:

```csharp
var query = context.Customers.Select(
    c => c.Orders.Where(o => o.Amount  > 100).Select(o => o.Amount).ToList());
```

Note that this query will be translated to only two SQL queries: One for Customers and the next one for Orders.

## [Owned] attribute

It is now possible to configure [owned entity types](xref:core/modeling/owned-entities) by simply annotating the type with `[Owned]` and then making sure the owner entity is added to the model:

```csharp
[Owned]
public class StreetAddress
{
    public string Street { get; set; }
    public string City { get; set; }
}

public class Order
{
    public int Id { get; set; }
    public StreetAddress ShippingAddress { get; set; }
}
```

## Command-line tool dotnet-ef included in .NET Core SDK

The _dotnet-ef_ commands are now part of the .NET Core SDK, therefore it will no longer be necessary to use DotNetCliToolReference in the project to be able to use migrations or to scaffold a DbContext from an existing database.

See the section on [installing the tools](xref:core/cli/dotnet#installing-the-tools) for more details on how to enable command line tools for different versions of the .NET Core SDK and EF Core.

## Microsoft.EntityFrameworkCore.Abstractions package

The new package contains attributes and interfaces that you can use in your projects to light up EF Core features without taking a dependency on EF Core as a whole. For example, the [Owned] attribute and the ILazyLoader interface are located here.

## State change events

New `Tracked` And `StateChanged` events on `ChangeTracker` can be used to write logic that reacts to entities entering the DbContext or changing their state.

## Raw SQL parameter analyzer

A new code analyzer is included with EF Core that detects potentially unsafe usages of our raw-SQL APIs, like `FromSql` or `ExecuteSqlCommand`. For example, for the following query, you will see a warning because _minAge_ is not parameterized:

```csharp
var sql = $"SELECT * FROM People WHERE Age > {minAge}";
var query = context.People.FromSql(sql);
```

## Database provider compatibility

It is recommended that you use EF Core 2.1 with providers that have been updated or at least tested to work with EF Core 2.1.

> [!TIP]
> If you find any unexpected incompatibility or any issue in the new features, or if you have feedback on them, please report it using [our issue tracker](https://github.com/dotnet/efcore/issues/new).
