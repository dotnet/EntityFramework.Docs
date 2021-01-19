---
title: Modeling for Performance - EF Core
description: Modeling efficiently when using Entity Framework Core
author: roji
ms.date: 12/1/2020
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

### Materialized views

Materialized views are similar to regular views, except that their data is stored on disk ("materialized"), rather than calculated every time when the view is queried. This tool is useful when you don't want to simply add a single cache column to an existing database, but rather want to cache the entire resultset of a complicated and expensive query's results, just as if it were a regular table; these results can then be queried very cheaply without any computation or joins happening. Unlike computed columns, materialized views aren't automatically updated when their underlying tables change - they must be manually refreshed. If the cached data can lag, refreshing the view can be done via a timer; another option is to set up database triggers to review a materialized view once certain database events occur.

EF doesn't currently provide any specific API for creating or maintaining views, materialized or otherwise; but it's perfectly fine to [create an empty migration and add the view definition via raw SQL](xref:core/managing-schemas/migrations/managing#arbitrary-changes-via-raw-sql).

## Inheritance mapping

It's recommended to read [the dedicated page on inheritance](xref:core/modeling/inheritance) before continuing with this section.

EF Core currently supports two techniques for mapping an inheritance model to a relational database:

* **Table-per-hierarchy** (TPH), in which an entire .NET hierarchy of classes is mapped to a single database table
* **Table-per-type** (TPT), in which each type in the .NET hierarchy is mapped to a different table in the database.

The choice of inheritance mapping technique can have a considerable impact on application performance - it's recommended to carefully measure before committing to a choice.

People sometimes choose TPT because it appears to be the "cleaner" technique; a separate table for each .NET type makes the database schema look similar to the .NET type hierarchy. In addition, since TPH must represent the entire hierarchy in a single table, rows have *all* columns regardless of the type actually being held in the row, and unrelated columns are always empty and unused. Aside from seeming to be an "unclean" mapping technique, many believe that these empty columns take up considerable space in the database and may hurt performance as well.

However, measuring shows that TPT is in most cases the inferior mapping technique from a performance standpoint; where all data in TPH comes from a single table, TPT queries must join together multiple tables, and joins are one of the primary sources of performance issues in relational databases. Databases also generally tend to deal well with empty columns, and features such as [SQL Server sparse columns](/sql/relational-databases/tables/use-sparse-columns) can reduce this overhead even further.

For a concrete example, [see this benchmark](https://github.com/dotnet/EntityFramework.Docs/tree/master/samples/core/Benchmarks/Inheritance.cs) which sets up a simple model with a 7-type hierarchy; 5000 rows are seeded for each type - totalling 35000 rows - and the benchmark simply loads all rows from the database:

| Method |     Mean |   Error |  StdDev |     Gen 0 |     Gen 1 |     Gen 2 | Allocated |
|------- |---------:|--------:|--------:|----------:|----------:|----------:|----------:|
|    TPH | 132.3 ms | 2.29 ms | 2.03 ms | 8000.0000 | 3000.0000 | 1250.0000 |  44.49 MB |
|    TPT | 201.3 ms | 3.32 ms | 3.10 ms | 9000.0000 | 4000.0000 |         - |  61.84 MB |

As can be seen, TPH is considerably more efficient than TPT for this scenario. Note that actual results always depend on the specific query being executed and the number of tables in the hierarchy, so other queries may show a different performance gap; you're encouraged to use this benchmark code as a template for testing other queries.
