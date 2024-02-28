---
title: .NET Data Community Standups
description: Details and links for each episode of the .NET Data/EF Community Standup
author: ajcvickers
ms.date: 02/23/2024
uid: core/learn-more/community-standups
---

# .NET Data Community Standups

The .NET Data Community Standups are live-streamed every other Wednesday to Twitch, YouTube, and Twitter. This is your opportunity to interact with the .NET Data team and community. All past episodes are [available on YouTube](https://aka.ms/efstandups) and are listed below with links to content from the shows.

[Comment on GitHub](https://github.com/dotnet/efcore/issues/22700) with ideas for guests, demos, or anything else you want to see.

## Summary

| Date         | Area                  | Title                                                                                  |
|--------------|-----------------------|----------------------------------------------------------------------------------------|
| Feb 21, 2024 | SQL translation       | [Window and binary functions using Zomp EF Core extensions](#window-functions)         |
| Feb 7, 2024  | Database concurrency  | [Database concurrency and EF Core: Beyond optimistic concurrency](#concurrency3)       |
| Jan 24, 2024 | Database concurrency  | [Database concurrency and EF Core: ASP.NET and Blazor](#concurrency2)                  |
| Jan 10, 2024 | Database concurrency  | [Database concurrency and EF Core](#concurrency1)                                      |
| Nov 29, 2023 | New features          | [A tour of what's new in EF8](#new8)                                                   |
| Oct 18, 2023 | Document databases    | [Introducing the MongoDB provider for EF Core](#mongo)                                 |
| Oct 4, 2023  | New features          | [Size doesn't matter: Smaller features in EF8](#smaller)                               |
| Sep 20, 2023 | Complex types/DDD     | [Complex types as value objects in EF8](#complextypes)                                 |
| Jun 14, 2023 | SQLite                | [Synchronizing data between the cloud and the client (using SQLite)](#zumero)          |
| May 31, 2023 | Power Tools           | [New CLI edition of EF Core Power Tools](#power-tools)                                 |
| May 17, 2023 | Primitive collections | [Collections of primitive values in EF Core](#primitive-collections)                   |
| Apr 19, 2023 | EF internals          | [EF Core Internals – Model Building](#internals-models)                                |
| Mar 22, 2023 | HierarchyId           | [Using hierarchical data in SQL Server and PostgreSQL with EF Core](#hierarchyid)      |
| Mar 8, 2023  | EF internals          | [EF Core internals: IQueryable, LINQ and the EF Core query pipeline](#internals-query) |
| Feb 22, 2023 | MySQL                 | [MySQL and .NET: MySqlConnector and the Pomelo EF Core Provider](#mysql)               |
| Feb 8, 2023  | SQLite                | [SQLite with .NET and EF Core](#sqlite)                                                |
| Jan 11, 2023 | FAQ                   | [Entity Framework Core FAQs](#faq)                                                     |
| Dec 14, 2022 | EF Core plans         | [The Plan for Entity Framework Core 8](#plan8)                                         |

## 2024

<a name="window-functions"></a>

### February 21: [Window and binary functions using Zomp EF Core extensions](https://www.youtube.com/live/Z9SkvUuU9Sc?si=xX3rRG3Xr3R9Zj5o)

Window functions are one of the most powerful features of SQL. Did you know that you can use Window Functions in EF Core today instead of writing raw SQL? Victor Irzak joins Shay and Arthur to explore real use cases of window functions with EF Core.

Featuring:

- [Victor Irzak](https://github.com/virzak) (Special guest)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- GutHub: [Zomp EF Core Extensions](https://github.com/zompinc/efcore-extensions)
- NuGet:
  - [Zomp.EFCore.WindowFunctions.SqlServer](https://www.nuget.org/packages/Zomp.EFCore.WindowFunctions.SqlServer)
  - [Zomp.EFCore.WindowFunctions.Npgsql](https://www.nuget.org/packages/Zomp.EFCore.WindowFunctions.Npgsql)
  - [Zomp.EFCore.WindowFunctions.Sqlite](https://www.nuget.org/packages/Zomp.EFCore.WindowFunctions.Sqlite)

<a name="concurrency3"></a>

### February 7: [Database concurrency and EF Core: Beyond optimistic concurrency](https://www.youtube.com/live/0eVTR5up2RY?si=pttR5NJ85_NWozhj)

In the 3rd installment of our series on concurrency, we'll dive deep into SQL isolation levels, how they work (and work differently!) across SQL Server and PostgreSQL. We'll hopefully learn a bit about how database locks, snapshots and isolation levels work together to bring some sanity into the world of concurrency.

Featuring:

- [Shay Rojansky](https://github.com/roji) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)

Links:

- [SQL Server Isolation Levels : A Series](https://sqlperformance.com/2014/07/t-sql-queries/isolation-levels)
- [Transaction isolation in PostgreSQL](https://www.postgresql.org/docs/current/transaction-iso.html)

<a name="concurrency2"></a>

### January 24: [Database concurrency and EF Core: ASP.NET and Blazor](https://www.youtube.com/live/xVyYrtetDeA?si=Bx2s44mk_ayUnAzx)

In the last episode, we dug deep into the underpinnings of optimistic concurrency handling in EF Core. In this episode, we continue that journey to cover disconnected scenarios. That is, where the entity travels to a client and then back to the server before being updated in the database. We’ll look at the different patterns for doing updates like this in ASP.NET Core and Blazor apps and see how concurrency tokens work with each of these. We’ll also look at how `ExecuteUpdate` can be used with concurrency tokens, and take a look at ETag concurrency in Azure Cosmos DB.

Featuring:

- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- [Handling Concurrency Conflicts](xref:core/saving/concurrency)

<a name="concurrency1"></a>

### January 10: [Database concurrency and EF Core](https://www.youtube.com/live/YfIM-gfJe4c?si=ZQNxpGEULZt3L43I)

What happens when one user is reading a record while another user is updating it? What if both users try to update the same record at the same time? In this episode of the .NET Data Community Standup, Shay and Arthur take a look at how different database isolation levels impact concurrency and performance, and then extend this to optimistic concurrency patterns supported by Entity Framework Core. This includes both manual concurrency tokens and automatic concurrency tokens like SQL Server’s “rowversion”. We’ll also dig into what happens in disconnected scenarios, and when to use the original value as opposed to the current value of the concurrency token. Finally, we’ll look at how EF Core handles optimistic concurrency with ETags when using a document database like Azure Cosmos DB.

Featuring:

- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- [Handling Concurrency Conflicts](xref:core/saving/concurrency)

## 2023

<a name="new8"></a>

### November 29: [A tour of what's new in EF8](https://www.youtube.com/live/5HapqzoxJ60?si=DXxh9jo8X16-rETm)

Arthur and Shay from the EF team are joined by members of the community to demo some of the key new features available now in Entity Framework Core 8. For example:

- Primitive collections
- Complex types
- HierarchyId in .NET and EF Core
- Raw SQL queries for unmapped types
- Lazy-loading enhancements
- Lookup tracked entities by PK/AK/FK
- DateOnly/TimeOnly supported on SQL Server
- Sentinel values and database defaults

Featuring:

- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)
- [Jeremy Likness](https://github.com/JeremyLikness) (Alumnus)
- [Erik Ejlskov Jensen](https://github.com/ErikEJ) (Special guest)
- [Julie Lerman](https://github.com/julielerman) (Special guest)
- [Diego Vega](https://github.com/divega) (Alumnus)
- [Brice Lambson](https://github.com/bricelam) (Team member)

Links:

- [EF8 release post on the .NET Blog](https://aka.ms/ef8)
- [What's new in EF8, with runnable samples](https://aka.ms/ef8-new)
- [Entity Framework Core 8: Improved JSON, queryable collections, and more… | .NET Conf 2023](https://aka.ms/ef8-dotnetconf)
- [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="mongo"></a>

### October 18: [Introducing the MongoDB provider for EF Core](https://www.youtube.com/live/Zat-ferrjro?si=UKIgjsB5RpnEqJUo)

In this episode of the .NET Data Community Standup, we talk to the engineers from MongoDB who have just released the first preview of an official EF Core provider. The EF team has collaborated closely with MongoDB on this provider, which now joins the Cosmos provider as flagship document database providers for EF Core. We will demo the provider, talk about what it can and cannot do, and of course answer all your questions!

Featuring:

- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)
- [James Kovacs](https://github.com/JamesKovacs) (Special guest)
- [Damien Guard](https://github.com/damieng) (Special guest)
- Patrick Gilfether (Special guest)

Links:

- [MongoDB Provider for Entity Framework Core Now Available in Public Preview](https://www.mongodb.com/blog/post/mongodb-provider-entity-framework-core-now-available-public-preview)
- [Trying out MongoDB with EF Core using Testcontainers](https://devblogs.microsoft.com/dotnet/efcore-mongodb/)
- [What's new in EF8, with runnable samples](https://aka.ms/ef8-new)
- [Entity Framework Core 8: Improved JSON, queryable collections, and more… | .NET Conf 2023](https://aka.ms/ef8-dotnetconf)
- [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="smaller"></a>

### October 4: [Size doesn't matter: Smaller features in EF8](https://www.youtube.com/live/GGDv_p4LAL8?si=WfswHDLiYnMMe4Hm)

In addition to the big features such as primitive collections and complex types, EF8 contains many smaller features and minor improvements. In this session, Arthur and Shay from the EF team will take a look at a few of these smaller features, including:

- Sentinels to control when the database generates a value
- Massive performance improvements for IN and EXISTS in LINQ queries
- DateOnly/TimeOnly on SQL Server
- Raw SQL queries for unmapped types
- More flexible ExecuteUpdate
- Numeric SQL rowversion properties
- Cleaner SQL through parentheses elimination

Featuring:

- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- [What's new in EF8, with runnable samples](https://aka.ms/ef8-new)
- [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="complextypes"></a>

### September 20: [Complex types as value objects in EF8](https://www.youtube.com/live/H-soJYqWSds?si=tJzm-uqLL0ZJnsUh)

The Entity Framework team returns with new community standups after a summer break heads-down working on EF8. In this session, we’ll look at the new “complex types” feature in EF8. Complex types do not have a key or any identity outside of the object’s value, but can still be deconstructed by property to map to different columns in the database. This allows either reference or value types to be used as DDD value objects with much better fidelity than when owned types are used. We’ll show examples of using complex types and talk about and answer your questions on the behavioral differences between entity types, owned entity types, complex types, and primitive types.

Featuring:

- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- [Unicode and UTF-8 in SQL Server with EF Core](xref:core/providers/sql-server/columns#unicode-and-utf-8)
- [What's new in EF8, with runnable samples](https://aka.ms/ef8-new)
- [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="zumero"></a>

### June 14: [Synchronizing data between the cloud and the client (using SQLite)](https://www.youtube.com/live/QTvFiiST-r4?si=9SOUVhwz75VplWad)

In this session, we'll invite Eric Sink to discuss a solution for keeping a synchronized copy of the DB (using SQLite) on the client, rather than asking data for the cloud every time. This is helpful for mobile scenarios where the connectivity may be poor, but the "rep and sync" approach can reduce the time the user spends waiting on the network.  Local writes are fast, sync happens in the background. We'll explore how this can be done efficiently, how to deal with conflict resolution, and other data sync-related concerns.
Featuring:

- [Eric Sink](https://github.com/ericsink) (Special guest)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- [Zumero](https://www.zumero.com/)

<a name="power-tools"></a>

### May 31: [New CLI edition of EF Core Power Tools](https://www.youtube.com/live/vGvBAoP3nVY?si=VhwBYYdc0F3OSgOu)

In today's standup, Erik will demonstrate the new CLI edition of EF Core Power Tools and show how he used a number of community NuGet packages to improve the user experience when creating a CLI tool.

Featuring:

- [Erik Ejlskov Jensen](https://github.com/ErikEJ) (Special guest)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Brice Lambson](https://github.com/bricelam) (Host)

Links:

- Visual Studio Marketplace: [EF Core Power Tools](https://marketplace.visualstudio.com/items?itemName=ErikEJ.EFCorePowerTools&ssr=false#review-details)
- GitHub: [ErikEJ/EFCorePowerTools](https://github.com/ErikEJ/EFCorePowerTools)
- [What's new in EF8, with runnable samples](https://aka.ms/ef8-new)
- [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="primitive-collections"></a>

### May 17: [Collections of primitive values in EF Core](https://www.youtube.com/live/AUS2OZjsA2I?si=V8hR14VyBtHuvbmp)

In this episode of the .NET Data Community Standup, the .NET Data Access team dive into new support for collections of primitive values, just released in EF Core 8 Preview 4. Collections of a primitive type can now be used as properties of an entity type and will be mapped to a JSON column in the relational database. In addition, parameters of primitive values can be passed to the database. In either case, the native JSON processing capabilities of the database are then used to exact and manipulate the primitive values, just as if they were in a table. This opens up powerful query possibilities, as well as optimizations to common problems such as translating queries that use Contains.

Featuring:

- [Shay Rojansky](https://github.com/roji) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)

Links:

- [What's new in EF8, with runnable samples](https://aka.ms/ef8-new)
- [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="internals-models"></a>

### Apr 19: [EF Core Internals – Model Building](https://www.youtube.com/live/FYz0rAxQkC8?si=GsLuBBByuQbMurYP)

In this session, the .NET Data Access team will dig into the EF Core internals for building a model. EF models are built using a combination of three mechanisms: conventions, mapping attributes, and the model builder API. We will explain each of these mechanisms and show how they interact. We’ll also cover how models are cached, and ways in which that caching can be controlled. And, as always, we’ll answer your questions live!
Featuring:

- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- [What's new in EF8, with runnable samples](https://aka.ms/ef8-new)
- [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="hierarchyid"></a>

### Mar 22: [Using hierarchical data in SQL Server and PostgreSQL with EF Core](https://www.youtube.com/live/pmnHGWYpCfg?si=h_v40tZ7NEw_wZ6W)

Join the .NET Data Access Team to learn about mapping hierarchical data structures to relational databases using EF Core. We’ll look at “hierarchyid” on SQL Server/Azure SQL and “ltree” on PostgresSQL, and show how to map tree structures such as a family tree, file system, or organization structure. EF Core can then be used to write LINQ queries against the hierarchy to find ancestors and descendants in various ways, as well as perform manipulation of subtrees for updates. And, as always, we’ll be here to answer your questions!

Featuring:

- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)
- [Brice Lambson](https://github.com/bricelam) (Host)

Links:

- [Sample code from this standup](https://github.com/ajcvickers/HierarchySample)
- [What's new in EF8, with runnable samples](https://aka.ms/ef8-new)
- [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="internals-query"></a>

### Mar 8: [EF Core internals: IQueryable, LINQ and the EF Core query pipeline](https://www.youtube.com/live/1Ld3dtnTrMw?si=xWhbADioxqquACBi)

In this standup, we'll dive deep under the hood to see how EF Core processes LINQ queries, translates them to SQL and executes them on your database. We'll introduce key concepts such as IQueryable and LINQ providers, and see how EF Core uses caching to make your querying lightning-fast. Join us to understand how the magic works!

Featuring:

- [Shay Rojansky](https://github.com/roji) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)

Links:

- [What's new in EF8, with runnable samples](https://aka.ms/ef8-new)
- [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="mysql"></a>

### Feb 22: [MySQL and .NET: MySqlConnector and the Pomelo EF Core Provider](https://www.youtube.com/live/XSEYNocnzlI?si=WdBnhu_kDewO1TbN)

This week, we'll be hosting Bradley Grainger and Laurents Meyer, authors of the open-source MySQL ADO.NET driver and of the Pomelo EF Core provider. Both these components work together to provide a 1st-class MySQL data access experience in .NET. We'll discuss some of the specifities of MySQL as a database, what it's like to work on open source data projects in .NET, and whatever else comes to mind!

Featuring:

- [Bradley Grainger](https://github.com/bgrainger) (Special guest)
- [Laurents Meyer](https://github.com/lauxjpn) (Special guest)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)
- [Jiachen Jiang](https://github.com/jcjiang) (Host)

Links:

- GitHub:
  - [Pomelo.EntityFrameworkCore.MySql](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql)
  - [MySqlConnector](https://github.com/mysql-net/MySqlConnector)
- NuGet:
  - [Pomelo.EntityFrameworkCore.MySql](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/)
  - [Pomelo.EntityFrameworkCore.MySql.Json.Microsoft](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql.Json.Microsoft/)
  - [Pomelo.EntityFrameworkCore.MySql.Json.Newtonsoft](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql.Json.Newtonsoft/)
  - [Pomelo.EntityFrameworkCore.MySql.Json.NetTopologySuite](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql.NetTopologySuite/)
  - [MySqlConnector](https://www.nuget.org/packages/MySqlConnector/)

<a name="sqlite"></a>

### Feb 8: [SQLite with .NET and EF Core](https://www.youtube.com/live/n3Tt0F4g4is?si=aKzl5sr13mSo8DkC)

This week, Eric Sink (creator of SQLitePCL.raw) and Brice Lambson (creator of Microsoft.Data.Sqlite) join the .NET Data Access Team to discuss everything SQLite on .NET. We’ll start down low digging into different ways to get the SQLite native binaries using Eric’s packages. Then we’ll move up the stack to look at the basics of SQLite on .NET with Brice’s ADO.NET provider. Going higher still, we’ll look at how to get the most from SQLite with the EF Core provider. And, as always, we’ll be there to answer any questions you have about SQLite on .NET.

Featuring:

- [Eric Sink](https://github.com/ericsink) (Special guest)
- [Brice Lambson](https://github.com/bricelam) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)

Links:

- GitHub:
  - [SQLitePCL.raw](https://github.com/ericsink/SQLitePCL.raw)
  - [Microsoft.Data.Sqlite](https://github.com/dotnet/efcore) (Contained in the EF Core repo.)
- NuGet:
  - [SQLitePCLRaw.bundle_e_sqlite3](https://www.nuget.org/packages/SQLitePCLRaw.bundle_e_sqlite3)
  - [Other SQLitePCLRaw packages](https://www.nuget.org/profiles/SQLitePCLRaw)
  - [Microsoft.Data.Sqlite](https://www.nuget.org/packages/Microsoft.Data.Sqlite)
  - [Microsoft.Data.Sqlite.Core](https://www.nuget.org/packages/Microsoft.Data.Sqlite.Core)

<a name="faq"></a>

### Jan 11: [Entity Framework Core FAQs](https://www.youtube.com/live/2MZNbPT8Q2E?si=TvG40n04AanO9vxj)

Join the Microsoft .NET Data team to get answers to many frequently asked questions about EF Core. For example:

- What's the difference between EF Core and EF6?
- What NuGet packages to I need to install to use EF Core?
- Should I always use no-tracking queries?
- Why does EF Core sometimes ignore Includes?
- What's wrong with using owned types for value objects?
- Should I create a repository over EF Core?
- Why does the in-memory database not work in my tests?
- How do I call stored procedures from EF Core?
- How do I see the SQL for a LINQ query?

Featuring:

- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)
- [Jiachen Jiang](https://github.com/jcjiang) (Host)

Links:

- [What's new in EF7, with runnable samples](https://aka.ms/ef7-new)
- [EF8 plan](https://aka.ms/ef8-plan)
- [EF Core daily builds](https://aka.ms/ef-daily-builds)

## 2022

<a name="plan8"></a>

### Dec 14: [The Plan for Entity Framework Core 8](https://www.youtube.com/live/-zoXAeDfNBY?si=Pr6ucas7j58gIyql)

The plan has been published for EF Core 8 and other data access work in .NET 8. This includes work in five major themes:

- Highly requested features
- Cloud native and devices
- Performance
- Visual tooling
- Developer experience

Join the .NET Data Team for a discussion of what’s in and what’s out, and to get your questions answered.

Featuring:

- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)
- [Jiachen Jiang](https://github.com/jcjiang) (Host)

Links:

- [What's new in EF7, with runnable samples](https://aka.ms/ef7-new)
- [EF8 plan](https://aka.ms/ef8-plan)
- [EF Core daily builds](https://aka.ms/ef-daily-builds)
