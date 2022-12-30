---
title: Introduction to Performance - EF Core
description: Performance guide for efficiently using Entity Framework Core
author: roji
ms.date: 12/1/2020
uid: core/miscellaneous/performance/index
---
# Introduction to Performance

Database performance is a vast and complex topic, spanning an entire stack of components: the database, networking, the database driver, and data access layers such as EF Core. While high-level layers and O/RMs such as EF Core considerably simplify application development and improve maintainability, they can sometimes be opaque, hiding performance-critical internal details such as the SQL being executed. This section attempts to provide an overview of how to achieve good performance with EF Core, and how to avoid common pitfalls which can degrade application performance.

## Identify bottlenecks and measure, measure, measure

As always with performance, it's important not to rush into optimization without data showing a problem; as the great Donald Knuth once said, "Premature optimization is the root of all evil". The [performance diagnosis](xref:core/performance/performance-diagnosis) section discusses various ways to understand where your application is spending time in database logic, and how to pinpoint specific problematic areas. Once a slow query has been identified, solutions can be considered: is your database missing an index? Should you try out other querying patterns?

Always benchmark your code and possible alternatives yourself - the performance diagnosis section contains a sample benchmark with BenchmarkDotNet, which you can use as a template for your own benchmarks. Don't assume that general, public benchmarks apply as-is to your specific use-case; a variety of factors such as database latency, query complexity and actual data amounts in your tables can have a profound effect on which solution is best. For example, many public benchmarks are carried out in ideal networking conditions, where latency to the database is almost zero, and with extremely light queries which hardly require any processing (or disk I/O) on the database side. While these are valuable for comparing the runtime overheads of different data access layers, the differences they reveal usually prove to be negligible in a real-world application, where the database performs actual work and latency to the database is a significant performance factor.

## Aspects of data access performance

Overall data access performance can be broken down into the following broad categories:

* **Pure database performance**. With relational database, EF translates the application's LINQ queries into the SQL statements getting executed by the database; these SQL statements themselves can run more or less efficiently. The right index in the right place can make a world of difference in SQL performance, or rewriting your LINQ query may make EF generate a better SQL query.
* **Network data transfer**. As with any networking system, it's important to limit the amount of data going back and forth on the wire. This covers making sure that you only send and load data which you're actually going to need, but also avoiding the so-called "cartesian explosion" effect when loading related entities.
* **Network roundtrips**. Beyond the amount of data going back and forth, the network roundtrips, since the time taken for a query to execute in the database can be dwarfed by the time packets travel back and forth between your application and your database. Roundtrip overhead heavily depends on your environment; the further away your database server is, the higher the latency and the costlier each roundtrip. With the advent of the cloud, applications increasingly find themselves further away from the database, and "chatty" applications which perform too many roundtrips experience degraded performance. Therefore, it's important to understand exactly when your application contacts the database, how many roundtrips it performs, and whether that number can be minimized.
* **EF runtime overhead**. Finally, EF itself adds some runtime overhead to database operations: EF needs to compile your queries from LINQ to SQL (although that should normally be done only once), change tracking adds some overhead (but can be disabled), etc. In practice, the EF overhead for real-world applications is likely to be negligible in most cases, as query execution time in the database and network latency dominate the total time; but it's important to understand what your options are and how to avoid some pitfalls.

## Know what's happening under the hood

EF allows developers to concentrate on business logic by generating SQL, materializing results, and performing other tasks. Like any layer or abstraction, it also tends to hide what's happening under-the-hood, such as the actual SQL queries being executed. Performance isn't necessarily a critical aspect of every application out there, but in applications where it is, it is vital that the developer understand what EF is doing for them: inspect outgoing SQL queries, follow roundtrips to make sure the N+1 problem isn't occurring, etc.

## Cache outside the database

Finally, the most efficient way to interact with a database, is to not interact with it at all. In other words, if database access shows up as a performance bottleneck in your application, it may be worthwhile to cache certain results outside of the database, so as to minimize requests. Although caching adds complexity, it is an especially crucial part of any scalable application: while the application tier can be easy scaled by adding additional servers to handle increased load, scaling the database tier is usually far more complicated.
