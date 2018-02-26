---
title: What is new in EF Core 2.1 - EF Core
author: divega
ms.author: divega

ms.date: 2/20/2018

ms.assetid: 585F90A3-4D5A-4DD1-92D8-5243B14E0FEC
ms.technology: entity-framework-core

uid: core/what-is-new/ef-core-2.1
---

# New features in EF Core 2.1
> [!NOTE]  
> This release is still in preview.

In this release we have fixed more than a hundred product bugs and added numerous small improvements. Here are some of the new major features:

## Lazy loading
EF Core now contains the necessary building blocks for anyone to write entity classes that can load their navigation properties on demand. We have also created a new package, Microsoft.EntityFrameworkCore.Proxies, that leverages those building blocks to produce lazy loading proxy classes based on minimally modified entity classes (e.g. classes with virtual navigation properties).

## Parameters in entity constructors
As one of the required building blocks for lazy loading, we enabled the creation of entities that take parameters in their constructors. You can use parameters to inject property values, lazy loading delegates, and services.

## Value conversions
Until now, EF Core could only map properties of types natively supported by the underlying database provider. Values were copied back and forth between columns and properties without any transformation. Starting with EF Core 2.1, value conversions can be applied to transform the values obtained from columns before they are applied to properties, and vice versa. We have a number of conversions that can be applied by convention as necessary, as well as an explicit configuration API that allows registering delegates for the conversions between columns and properties. Some of the application of this feature are:

- Storing enums as strings
- Mapping unsigned integers with SQL Server
- Automatic encryption and decryption of property values

Read the [section on value conversions](xref:core/modeling/value-conversions) for more information.  

## LINQ GroupBy translation
Before EF Core 2.1, the GroupBy LINQ operator would always be evaluated in memory. We now support translating it to the SQL GROUP BY clause in most common cases.

## Data Seeding
With the new release it will be possible to provide initial data to populate a database. Unlike in EF6, in EF Core, seeding data is associated to an entity type as part of the model configuration. Then EF Core migrations can automatically compute what insert, update or delete operations need to be applied when upgrading the database to a new version of the model.

As an example, you can use this to configure seed data for a Post in `OnModelCreating`:

``` csharp
modelBuilder.Entity<Post>().SeedData(new Post{ Id = 1, Text = "Hello World!" });
```

## Query types
An EF Core model can now include query types. Unlike entity types, query types do not have keys defined on them and cannot be inserted, deleted or updated (i.e. they are read-only), but they can be returned directly by queries. Some of the usage scenarios for query types are:

- Mapping to views without primary keys
- Mapping to tables without primary keys
- Mapping to queries defined in the model
- Serving as the return type for FromSql() queries

## Include for derived types
It will be now possible to specify navigation properties only defined on derived types when writing expressions for the `Include` method. For the strongly typed version of `Include`, we support using either an explicit cast or the `as` operator. We also now support referencing the names of navigation property defined on derived types in the string version of `Include`:

``` csharp
var option1 = context.People.Include(p => ((Student)p).School);
var option2 = context.People.Include(p => (p as Student).School);
var option3 = context.People.Include("School");
```

## System.Transactions support
We have added the ability to work with System.Transactions features such as TransactionScope. This will work on both .NET Framework and .NET Core when using database providers that support it.

## Better column ordering in initial migration
Based on customer feedback, we have updated migrations to initially generate columns for tables in the same order as properties are declared in classes. Note that we cannot change order when new members are added after the initial table creation.

## Optimization of correlated subqueries
We have improved our query translation to avoid executing "N + 1" SQL queries in many common scenarios in which the usage of a navigation property in the projection leads to joining data from the root query with data from a correlated subquery. The optimization requires buffering the results form the subquery, and we require that you modify the query to opt-in the new behavior.

As an example, the following query normally gets translated into one query for Customers, plus N (where "N" is the number of customers returned) separate queries for Orders:

``` csharp
var query = context.Customers.Select(c => c.Orders.Where(o => o.Amount  > 100).Select(o => o.Amount));
```

By including `ToList()` in the right place, you can give up the streaming of the Orders and enable the optimization:

``` csharp
var query = context.Customers.Select(c => c.Orders.Where(o => o.Amount  > 100).Select(o => o.Amount).ToList());
```

Nota that this query will be translated to only two SQL queries: One for Customers and the next one for Orders.

``` csharp
var query = context.Customers.Select(c => c.Orders.Where(o => o.Amount  > 100).Select(o => o.Amount).ToList())
```
