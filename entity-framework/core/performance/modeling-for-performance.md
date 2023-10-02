---
title: Modeling for Performance - EF Core
description: Modeling efficiently when using Entity Framework Core
author: roji
ms.date: 10/10/2022
uid: core/performance/modeling-for-performance
---
# Modeling for Performance

In many cases, the way you model can have a profound impact on the performance of your application; while a properly normalized and "correct" model is usually a good starting point, in real-world applications some pragmatic compromises can go a long way for achieving good performance. Since it's quite difficult to change your model once an application is running in production, it's worth keeping performance in mind when creating the initial model.

## Denormalization and caching

*Denormalization* is the practice of adding redundant data to your schema, usually in order to eliminate joins when querying. For example, for a model with Blogs and Posts, where each Post has a Rating, you may be required to frequently show the average rating of the Blog. The simple approach to this would group the Posts by their Blog, and calculate the average as part of the query; but this requires a costly join between the two tables. Denormalization would add the calculated average of all posts to a new column on Blog, so that it is immediately accessible, without joining or calculating.

The above can be viewed as a form of *caching* - aggregate information from the Posts is cached on their Blog; and like with any caching, the problem is how to keep the cached value up to date with the data it's caching. In many cases, it's OK for the cached data to lag for a bit; for example, in the example above, it's usually reasonable for the blog's average rating to not be completely up to date at any given point. If that's the case, you can have it recalculated every now and then; otherwise, a more elaborate system must be set up to keep the cached values up to date.

The following details some techniques for denormalization and caching in EF Core, and points to the relevant sections in the documentation.

### Stored computed columns

If the data to be cached is a product of other columns in the same table, then a [stored computed column](xref:core/modeling/generated-properties#computed-columns) can be a perfect solution. For example, a `Customer` may have `FirstName` and `LastName` columns, but we may need to search by the customer's *full name*. A stored computed column is automatically maintained by the database - which recalculates it whenever the row is changed - and you can even define an index over it to speed up queries.

### Update cache columns when inputs change

If your cached column needs to reference inputs from outside the table's row, you cannot use computed columns. However, it is still possible to recalculate the column whenever its input changes; for example, you could recalculate the average Blog's rating every time a Post is changed, added or removed. Be sure to identify the exact conditions when recalculation is needed, otherwise your cached value will go out of sync.

One way to do this, is to perform the update yourself, via the regular EF Core API. `SaveChanges` [Events](xref:core/logging-events-diagnostics/events) or [interceptors](xref:core/logging-events-diagnostics/interceptors#savechanges-interception) can be used to automatically check if any Posts are being updated, and to perform the recalculation that way. Note that this typically entails additional database roundtrips, as additional commands must be sent.

For more perf-sensitive applications, database triggers can be defined to automatically perform the recalculation in the database. This saves the extra database roundtrips, automatically occurs within the same transaction as the main update, and can be simpler to set up. EF doesn't provide any specific API for creating or maintaining triggers, but it's perfectly fine to [create an empty migration and add the trigger definition via raw SQL](xref:core/managing-schemas/migrations/managing#arbitrary-changes-via-raw-sql).

### Materialized/indexed views

Materialized (or indexed) views are similar to regular views, except that their data is stored on disk ("materialized"), rather than calculated every time the view is queried. Such views are conceptually similar to stored computed columns, as they cache the results of potentially expensive calculations; however, they cache an entire query's resultset instead of a single column. Materialized views can be queried just like any regular table, and since they are cached on disk, such queries execute very quickly and cheaply without having to constantly perform the expensive calculations of the query which defines the view.

Specific support for materialized views varies across databases. In some databases (e.g. [PostgreSQL](https://www.postgresql.org/docs/current/rules-materializedviews.html)), materialized views must be manually refreshed in order for their values to be synchronized with their underlying tables. This is typically done via a timer - in cases where some data lag is acceptable - or via a trigger or stored procedure call in specific conditions. [SQL Server Indexed Views](/sql/relational-databases/views/create-indexed-views), on the other hand, are automatically updated as their underlying tables are modified; this ensures that the view always shows the latest data, at the cost of slower updates. In addition, SQL Server Index Views have various restrictions on what they support; consult the [documentation](/sql/relational-databases/views/create-indexed-views) for more information.

EF doesn't currently provide any specific API for creating or maintaining views, materialized/indexed or otherwise; but it's perfectly fine to [create an empty migration and add the view definition via raw SQL](xref:core/managing-schemas/migrations/managing#arbitrary-changes-via-raw-sql).

## Inheritance mapping

It's recommended to read [the dedicated page on inheritance](xref:core/modeling/inheritance) before continuing with this section.

EF Core currently supports three techniques for mapping an inheritance model to a relational database:

* **Table-per-hierarchy** (TPH), in which an entire .NET hierarchy of classes is mapped to a single database table.
* **Table-per-type** (TPT), in which each type in the .NET hierarchy is mapped to a different table in the database.
* **Table-per-concrete-type** (TPC), in which each concrete type in the .NET hierarchy is mapped to a different table in the database, where each table contains columns for all properties of the corresponding type.

The choice of inheritance mapping technique can have a considerable impact on application performance - it's recommended to carefully measure before committing to a choice.

Intuitively, TPT might seem like the "cleaner" technique; a separate table for each .NET type makes the database schema look similar to the .NET type hierarchy. In addition, since TPH must represent the entire hierarchy in a single table, rows have *all* columns regardless of the type actually being held in the row, and unrelated columns are always empty and unused. Aside from seeming to be an "unclean" mapping technique, many believe that these empty columns take up considerable space in the database and may hurt performance as well.

> [!TIP]
> If your database system supports it (e.g. SQL Server), then consider using "sparse columns" for TPH columns that will be rarely populated.

However, measuring shows that TPT is in most cases the inferior mapping technique from a performance standpoint; where all data in TPH comes from a single table, TPT queries must join together multiple tables, and joins are one of the primary sources of performance issues in relational databases. Databases also generally tend to deal well with empty columns, and features such as [SQL Server sparse columns](/sql/relational-databases/tables/use-sparse-columns) can reduce this overhead even further.

TPC has similar performance characteristics to TPH, but is slightly slower when selecting entities of all types as this involves several tables. However, TPC really excels when querying for entities of a single leaf type - the query only uses a single table and needs no filtering.

For a concrete example, [see this benchmark](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Benchmarks/Inheritance.cs) which sets up a simple model with a 7-type hierarchy; 5000 rows are seeded for each type - totalling 35000 rows - and the benchmark simply loads all rows from the database:

| Method |     Mean |   Error |   StdDev |     Gen 0 |     Gen 1 | Allocated |
|------- |---------:|--------:|---------:|----------:|----------:|----------:|
|    TPH | 149.0 ms | 3.38 ms |  9.80 ms | 4000.0000 | 1000.0000 |     40 MB |
|    TPT | 312.9 ms | 6.17 ms | 10.81 ms | 9000.0000 | 3000.0000 |     75 MB |
|    TPC | 158.2 ms | 3.24 ms |  8.88 ms | 5000.0000 | 2000.0000 |     46 MB |

As can be seen, TPH and TPC are considerably more efficient than TPT for this scenario. Note that actual results always depend on the specific query being executed and the number of tables in the hierarchy, so other queries may show a different performance gap; you're encouraged to use this benchmark code as a template for testing other queries.
