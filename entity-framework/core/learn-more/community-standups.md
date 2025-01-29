---
title: .NET Data Community Standups
description: Details and links for each episode of the .NET Data Community Standup
author: SamMonoRT
ms.date: 02/23/2024
uid: core/learn-more/community-standups
---

# .NET Data Community Standups

The .NET Data Community Standups are live-streamed monthly (roughly) on Wednesday (usually) to YouTube and Twitch. This is your opportunity to interact with the .NET Data team and community. All past episodes are [available on YouTube](https://aka.ms/efstandups) and are listed below with links to content from the shows.

[Comment on GitHub](https://github.com/dotnet/efcore/issues/22700) with ideas for guests, demos, or anything else you want to see.

## Summary

| Date         | Area                  | Title                                                                                    |
|--------------|-----------------------|------------------------------------------------------------------------------------------|
| Jan 22, 2025 | Tips                  | [Context pooling, FromSql and compiled queries](#Jan22_2025)                             |
| Nov 20, 2024 | Release               | [EF Core 9: Release extravaganza](#Nov20_2024)                                           |
| Jun 26, 2024 | SQL schemas           | [Improve your SQL schema and scripts with .NET based static code analysis](#June26_2024) |
| May 15, 2024 | Firebird              | [Harnessing the Power of Firebird in .NET](#May15_2024)                                  |
| Apr 17, 2024 | EF Core mapping       | [All about EF Core property mapping](#Apr17_2024)                                        |
| Mar 20, 2024 | GraphQL               | [Distributed Applications with Hot Chocolate 14, Aspire, and EF Core](#Mar20_2024)       |
| Mar 6, 2024  | Value generation      | [EF Core keys and value generation](#value-generation)                                   |
| Feb 21, 2024 | SQL translation       | [Window and binary functions using Zomp EF Core extensions](#window-functions)           |
| Feb 7, 2024  | Database concurrency  | [Database concurrency and EF Core: Beyond optimistic concurrency](#concurrency3)         |
| Jan 24, 2024 | Database concurrency  | [Database concurrency and EF Core: ASP.NET and Blazor](#concurrency2)                    |
| Jan 10, 2024 | Database concurrency  | [Database concurrency and EF Core](#concurrency1)                                        |
| Nov 29, 2023 | New features          | [A tour of what's new in EF8](#new8)                                                     |
| Oct 18, 2023 | Document databases    | [Introducing the MongoDB provider for EF Core](#mongo)                                   |
| Oct 4, 2023  | New features          | [Size doesn't matter: Smaller features in EF8](#smaller)                                 |
| Sep 20, 2023 | Complex types/DDD     | [Complex types as value objects in EF8](#complextypes)                                   |
| Jun 14, 2023 | SQLite                | [Synchronizing data between the cloud and the client (using SQLite)](#zumero)            |
| May 31, 2023 | Power Tools           | [New CLI edition of EF Core Power Tools](#power-tools)                                   |
| May 17, 2023 | Primitive collections | [Collections of primitive values in EF Core](#primitive-collections)                     |
| Apr 19, 2023 | EF internals          | [EF Core Internals - Model Building](#internals-models)                                  |
| Mar 22, 2023 | HierarchyId           | [Using hierarchical data in SQL Server and PostgreSQL with EF Core](#hierarchyid)        |
| Mar 8, 2023  | EF internals          | [EF Core internals: IQueryable, LINQ and the EF Core query pipeline](#internals-query)   |
| Feb 22, 2023 | MySQL                 | [MySQL and .NET: MySqlConnector and the Pomelo EF Core Provider](#mysql)                 |
| Feb 8, 2023  | SQLite                | [SQLite with .NET and EF Core](#sqlite)                                                  |
| Jan 11, 2023 | FAQ                   | [Entity Framework Core FAQs](#faq)                                                       |
| Dec 14, 2022 | EF Core plans         | [The Plan for Entity Framework Core 8](#plan8)                                           |
| Nov 16, 2022 | New features          | [A Whirlwind Tour of EF7](#new-ef7)                                                      |
| Nov 7, 2022  | Model building        | [EF7 Custom Model Conventions](#conventions)                                             |
| Oct 19, 2022 | Bulk updates          | [New EF Core 7.0 APIs (Bulk Update)](#bulkcud)                                           |
| Oct 5, 2022  | JSON mapping          | [JSON Columns](#json)                                                                    |
| Sep 21, 2022 | Azure Mobile Apps     | [Azure Mobile Apps and offline sync](#offline-sync)                                      |
| Aug 24, 2022 | SQL translation       | [New aggregate function support in EF Core 7](#ef7aggs)                                  |
| Aug 10, 2022 | WCF                   | [CoreWCF: Roadmap and Q&A](#wcf)                                                         |
| Jul 27, 2022 | ADO.NET               | [DbDataSource, a new System.Data abstraction](#dbdatasource)                             |
| Jul 13, 2022 | EF Interceptors       | [Intercept this EF7 Preview 6 Event](#ef7interceptors)                                   |
| Jun 29, 2022 | EF internals          | [DbContext Configuration and Lifetime - EF Core Architecture Part 2](#lifetimes)         |
| Jun 15, 2022 | TPH/TPT/TPC           | [TPH, TPT, and TPC Inheritance mapping with EF Core](#inheritance)                       |
| Jun 1, 2022  | EF internals          | [EF Core Architecture: Internal Dependency Injection](#internal-di)                      |
| May 18, 2022 | Testing               | [Testing EF Core Apps (part 2)](#testing2)                                               |
| May 4, 2022  | Pagination            | [Database Pagination](#pagination)                                                       |
| Apr 20, 2022 | SaveChanges           | [Performance Improvements to the EF7 Update Pipeline](#update-perf)                      |
| Apr 6, 2022  | Scaffolding           | [Database-first with T4 Templates in EF7: Early look](#t4)                               |
| Mar 9, 2022  | GraphQL/OData         | [GraphQL and OData: An In-Depth Discussion](#graphql-odata)                              |
| Feb 23, 2022 | Entity Framework      | [Celebrating 20 Years of .NET: Entity Framework](#20years)                               |
| Feb 9, 2022  | Versioning            | [Software version and "stuff"](#stuff)                                                   |
| Jan 26, 2022 | Testing               | [Testing EF Core Apps](#testing1)                                                        |
| Jan 12, 2022 | EF Core plans         | [The EF7 Plan](#ef7plan)                                                                 |
| Dec 1, 2021  | GraphQL               | [Hot Chocolate 12 and GraphQL 2021](#hotchoc12)                                          |
| Nov 17, 2021 | New features          | [Make History and Explore the Cosmos, an EF Core 6 Retrospective](#retro6)               |
| Nov 3, 2021  | Noda Time             | [Noda Time](#nodatime)                                                                   |
| Oct 6, 2021  | Temporal tables       | [SQL Server Temporal Tables and EF Core 6](#temporal)                                    |
| Oct 20, 2021 | Docs                  | [EF Core and ASP.NET Core from the ASP.NET documentation team](#aspnet)                  |
| Sep 22, 2021 | PostgreSQL            | [PostgreSQL and EF Core](#npgsql)                                                        |
| Aug 25, 2021 | Dapper                | [Dapper](#dapper)                                                                        |
| Aug 11, 2021 | SQL translation       | [EF Core's Global Query Filter](#query-filters)                                          |
| Jul 28, 2021 | OData                 | [OData](#odata)                                                                          |
| Jul 14, 2021 | SQL translation       | [Visualizing database query plans](#query-plans)                                         |
| Jun 16, 2021 | Azure Cosmos DB       | [Azure Cosmos DB and EF Core](#cosmos6)                                                  |
| Jun 2, 2021  | Model building        | [Introducing EF Core Compiled Models](#compiled-models)                                  |
| May 19, 2021 | GraphQL               | [Building Modern Apps with GraphQL](#graphql1)                                           |
| May 5, 2021  | Triggers              | [Triggers for EF Core](#triggers)                                                        |
| Apr 21, 2021 | Open source           | [Open Source Contributions: How to Add a Feature to EF Core](#contributing)              |
| Apr 7, 2021  | Azure SQL             | [Azure SQL for Cloud-Born Applications and Developers](#davide)                          |
| Mar 24, 2021 | EF Core Power Tools   | [EF Core Power Tools: the New Batch](#newbatch)                                          |
| Mar 10, 2021 | AMA                   | [Julie Lerman and EF Core Ask Me Anything (AMA)](#julie)                                 |
| Feb 24, 2021 | Performance           | [Performance Tuning an EF Core App](#jonp1)                                              |
| Feb 10, 2021 | Exception handling    | [Typed Exceptions for Entity Framework Core](#typed-exceptions)                          |
| Jan 27, 2021 | Database projects     | [Introducing MSBuild.Sdk.SqlProj](#sqlproj)                                              |
| Jan 13, 2021 | Survey                | [EF Core 6.0 Survey Results](#survey)                                                    |
| Dec 2, 2020  | LLBLGen Pro           | [LLBLGen designer and .NET data history](#frans)                                         |
| Nov 18, 2020 | New features          | [Special EF Core 5.0 Community Panel](#efcore5)                                          |
| Oct 28, 2020 | Collations            | [EF Core 5.0 Collations](#collations)                                                    |
| Oct 14, 2020 | Azure Cosmos DB       | [Cosmos DB: Repository Pattern .NET Wrapper SDK](#repository)                            |
| Sep 20, 2020 | Spatial types         | [Geographic Data with NetTopologySuite](#geo-nts)                                        |
| Sep 16, 2020 | Migrations            | [What's New with Migrations in EF Core 5.0](#migrations5)                                |
| Sep 2, 2020  | Offline sync          | [Sync your database with DotMim.Sync](#dotmin-sync)                                      |
| Aug 19, 2020 | Many-to-many          | [Many-to-Many in EF Core 5.0](#many-to-many)                                             |
| Aug 5, 2020  | EF in-depth           | [EF Core In Depth Video Series](#depth)                                                  |
| Jul 22, 2020 | Scaffolding           | [Using Scaffolding with Handlebars](#handlebars)                                         |
| Jun 24, 2020 | Blazor                | [EF Core in Blazor](#blazor)                                                             |
| Jun 10, 2020 | EF Core Power Tools   | [EF Core Power Tools](#power-tools1)                                                     |
| May 6, 2020  | Welcome!              | [Introducing the EF Core Community Standup](#one)                                        |

## 2025

<a name="Jan22_2025"></a>

### January 22: [Context pooling, FromSql and compiled queries](https://www.youtube.com/live/lAP2nlA6ijw?si=jq7XhqVwmLi_Yt7c)

Join us for another .NET Data Community Standup with Chris Woodruff where he talks about context pooling, FromSql and compiled queries.

Featuring:

- [Chris Woodruff](https://woodruff.dev/) (Special guest)
- [Jiri Cincura](https://www.tabsoverspaces.com/) (Host)
- [Shay Rojansky](https://www.roji.org/) (Host)

## 2024

<a name="Nov20_2024"></a>

### November 20: [EF Core 9: Release extravaganza](https://www.youtube.com/live/wG8D5HJMzjA?si=239r9dKJhBxilQ2e)

EF Core 9 was just released. Join us in this special session talking with industry experts about all topic EF Core.

Featuring:

- [Erik Ejlskov Jensen](https://erikej.github.io) (Special guest)
- [Julie Lerman](https://thedatafarm.com/) (Special guest)
- [Jiri Cincura](https://github.com/cincuranet) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

<a name="June26_2024"></a>

### June 26: [Improve your SQL schema and scripts with .NET based static code analysis](https://www.youtube.com/live/yIOFMHzaKGs?si=YuvPQNKpKomd0wue)

By harnessing the power of the .NET DacFX framework and the related .NET T-SQL parser, you can improve the quality of your database schema and scripts with static T-SQL analysis.

[@ErikEJ](https://github.com/ErikEJ) has recently revived this 15-year-old technology with support for .NET 6 and later, and has brought a number of third-party rules back from the grave.

In this EF Core community standup we will have a closer look - from generation of a simple HTML report with findings to managing and creating your own analyzer rules using modern .NET.

Featuring:

- [Erik Ejlskov Jensen](https://github.com/ErikEJ) (Special guest)
- [Jiri Cincura](https://github.com/cincuranet) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- Blog: [How to: Code analyze your SQL Server T-SQL scripts in Visual Studio](https://erikej.github.io/dacfx/codeanalysis/sqlserver/2024/04/02/dacfx-codeanalysis.html)
- Blog: [Create a Custom Static Code Analysis Rule for Azure SQL Database / SQL Server with .NET](https://erikej.github.io/dacfx/dotnet/2024/04/04/dacfx-rules.html)
- Product: [EF Core Power Tools - Visual Studio Marketplace](https://marketplace.visualstudio.com/items?itemName=ErikEJ.EFCorePowerTools&ssr=false#overview)

<a name="May15_2024"></a>

### May 15: [Harnessing the Power of Firebird in .NET](https://www.youtube.com/live/ouGIWUtzxRQ?si=qtZ1xqKjKe_E1MhI)

In this community talk, we delve into the integration of Firebird, an open-source SQL relational database management system, with .NET, a free, cross-platform, open-source developer platform. This session aims to provide an understanding of how these two powerful technologies can be combined to create robust, scalable, and efficient applications.

This talk is designed for developers of all levels interested in expanding their knowledge on database management and .NET development. Whether you're a seasoned developer or a beginner looking to broaden your skill set, this talk will provide valuable insights into the effective use of Firebird with .NET.

Join us for this enlightening session and discover how you can harness the power of Firebird in .NET to take your applications to the next level.

Featuring:

- [Jiri Cincura](https://github.com/cincuranet) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- Product: [Firebird](https://firebirdsql.org/)
- NuGet: [FirebirdSql.Data.FirebirdClient](https://www.nuget.org/packages/FirebirdSql.Data.FirebirdClient)
- Blog: [FbNetExternalEngine](https://www.tabsoverspaces.com/tools/fb-net-external-engine)
- Tool used in the stream: [Database.NET](https://fishcodelib.com/database.htm)
- Docs: [What's new in EF9, with runnable samples](https://aka.ms/ef9-new)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="Apr17_2024"></a>

### Apr 17: [All about EF Core property mapping](https://www.youtube.com/live/ouGIWUtzxRQ?si=qtZ1xqKjKe_E1MhI)

In this episode, Arthur Vickers and Jiri Cincura from the EF team discuss everything about mapping properties in EF Core. We’ll look at:

- The differences between properties and navigations.
- How EF decides to map a property, and whether it is mapped to a nullable column, has a max length defined, etc.
- When backing fields are used, and mapping fields directly.
- Shadow properties.
- Indexer properties.

And if we get time, we’ll look at how value converters influence all this!

Featuring:

- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Jiri Cincura](https://github.com/cincuranet) (Host)

Links:

- GitHub: [Code from the standup](https://github.com/ajcvickers/PropertyMapping)
- Docs: [What's new in EF9, with runnable samples](https://aka.ms/ef9-new)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="Mar20_2024"></a>

### Mar 20: [Building Distributed Applications with Hot Chocolate 14, Aspire, and Entity Framework](https://www.youtube.com/live/JjT9pg6oGsE?si=jFykuE06ZFeHGEDJ)

Building and debugging distributed systems challenges developers to balance complexity with the need for simplicity. Ideally, we aim for the simplicity of a monolith while benefiting from microservices' scalability and isolation. In todays meetup we will have an early look at what we are doing with Hot Chocolate 14 to integrate well with Aspire and bridge this gap, offering an approach that combines microservices' advantages with the ease of a monolith for our consumers. Also we will have a look at a lot of the new feature around Hot Chocolate 14 that will make it so much easier to build layered services with entity framework. Do not worry! We will start slow with just the simplest of services accessing a database and then go all the way in.

Featuring:

- [Michael Staib](https://github.com/michaelstaib) (Special guest)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)
- [Jiri Cincura](https://github.com/cincuranet) (Host)

Links:

- Product: [Hot Chocolate for GraphQL](https://chillicream.com/docs/hotchocolate)
- Docs: [Hot Chocolate and Entity Framework Core](https://chillicream.com/docs/hotchocolate/integrations/entity-framework)

<a name="value-generation"></a>

### Mar 6: [EF Core keys and value generation](https://www.youtube.com/live/_eNPkrTuLvE?si=RuZUFoLqF2QnwbvF)

In this episode, Arthur and members of the EF Team will explore generated property values and how value generation interacts with keys in the model. We will look at fully client-side value generation, as well as how to use identity columns, database sequences, or a hi-lo pattern for server-side generation. In addition, we’ll investigate how generated key values impact EF Core change tracking by determining if an entity will go into the `Added` state or not. And finally, if we have time, we’ll look at special considerations for TPC mapping, and customizations such as setting before and after save behaviors, and sentinel values.

Featuring:

- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- GitHub: [Code from the standup](https://github.com/ajcvickers/Keys)
- Docs: [What's new in EF9, with runnable samples](https://aka.ms/ef9-new)
- Docs: [What's new in EF8, with runnable samples](https://aka.ms/ef8-new)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

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
- Blog: [EF8 release post on the .NET Blog](https://aka.ms/ef8)
- Docs: [What's new in EF8, with runnable samples](https://aka.ms/ef8-new)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="concurrency3"></a>

### February 7: [Database concurrency and EF Core: Beyond optimistic concurrency](https://www.youtube.com/live/0eVTR5up2RY?si=pttR5NJ85_NWozhj)

In the 3rd installment of our series on concurrency, we'll dive deep into SQL isolation levels, how they work (and work differently!) across SQL Server and PostgreSQL. We'll hopefully learn a bit about how database locks, snapshots and isolation levels work together to bring some sanity into the world of concurrency.

Featuring:

- [Shay Rojansky](https://github.com/roji) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)

Links:

- Blog: [SQL Server Isolation Levels : A Series](https://sqlperformance.com/2014/07/t-sql-queries/isolation-levels)
- Blog: [Transaction isolation in PostgreSQL](https://www.postgresql.org/docs/current/transaction-iso.html)
- Blog: [EF8 release post on the .NET Blog](https://aka.ms/ef8)
- Docs: [What's new in EF8, with runnable samples](https://aka.ms/ef8-new)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="concurrency2"></a>

### January 24: [Database concurrency and EF Core: ASP.NET and Blazor](https://www.youtube.com/live/xVyYrtetDeA?si=Bx2s44mk_ayUnAzx)

In the last episode, we dug deep into the underpinnings of optimistic concurrency handling in EF Core. In this episode, we continue that journey to cover disconnected scenarios. That is, where the entity travels to a client and then back to the server before being updated in the database. We’ll look at the different patterns for doing updates like this in ASP.NET Core and Blazor apps and see how concurrency tokens work with each of these. We’ll also look at how `ExecuteUpdate` can be used with concurrency tokens, and take a look at ETag concurrency in Azure Cosmos DB.

Featuring:

- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- Docs: [Handling Concurrency Conflicts](xref:core/saving/concurrency)
- Blog: [EF8 release post on the .NET Blog](https://aka.ms/ef8)
- Docs: [What's new in EF8, with runnable samples](https://aka.ms/ef8-new)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="concurrency1"></a>

### January 10: [Database concurrency and EF Core](https://www.youtube.com/live/YfIM-gfJe4c?si=ZQNxpGEULZt3L43I)

What happens when one user is reading a record while another user is updating it? What if both users try to update the same record at the same time? In this episode of the .NET Data Community Standup, Shay and Arthur take a look at how different database isolation levels impact concurrency and performance, and then extend this to optimistic concurrency patterns supported by Entity Framework Core. This includes both manual concurrency tokens and automatic concurrency tokens like SQL Server’s “rowversion”. We’ll also dig into what happens in disconnected scenarios, and when to use the original value as opposed to the current value of the concurrency token. Finally, we’ll look at how EF Core handles optimistic concurrency with ETags when using a document database like Azure Cosmos DB.

Featuring:

- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- Docs: [Handling Concurrency Conflicts](xref:core/saving/concurrency)
- Blog: [EF8 release post on the .NET Blog](https://aka.ms/ef8)
- Docs: [What's new in EF8, with runnable samples](https://aka.ms/ef8-new)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

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

- [Erik Ejlskov Jensen](https://github.com/ErikEJ) (Special guest)
- [Julie Lerman](https://github.com/julielerman) (Special guest)
- [Jeremy Likness](https://github.com/JeremyLikness) (Alumnus)
- [Diego Vega](https://github.com/divega) (Alumnus)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)
- [Brice Lambson](https://github.com/bricelam) (Host)

Links:

- Blog: [EF8 release post on the .NET Blog](https://aka.ms/ef8)
- Docs: [What's new in EF8, with runnable samples](https://aka.ms/ef8-new)
- Video: [Entity Framework Core 8: Improved JSON, queryable collections, and more… | .NET Conf 2023](https://aka.ms/ef8-dotnetconf)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="mongo"></a>

### October 18: [Introducing the MongoDB provider for EF Core](https://www.youtube.com/live/Zat-ferrjro?si=UKIgjsB5RpnEqJUo)

In this episode of the .NET Data Community Standup, we talk to the engineers from MongoDB who have just released the first preview of an official EF Core provider. The EF team has collaborated closely with MongoDB on this provider, which now joins the Cosmos provider as flagship document database providers for EF Core. We will demo the provider, talk about what it can and cannot do, and of course answer all your questions!

Featuring:

- [James Kovacs](https://github.com/JamesKovacs) (Special guest)
- [Damien Guard](https://github.com/damieng) (Special guest)
- [Patrick Gilfether](https://www.linkedin.com/in/patrick-gilfether) (Special guest)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- Blog: [MongoDB Provider for Entity Framework Core Now Available in Public Preview](https://www.mongodb.com/blog/post/mongodb-provider-entity-framework-core-now-available-public-preview)
- Blog: [Trying out MongoDB with EF Core using Testcontainers](https://devblogs.microsoft.com/dotnet/efcore-mongodb/)
- Docs: [What's new in EF8, with runnable samples](https://aka.ms/ef8-new)
- Video: [Entity Framework Core 8: Improved JSON, queryable collections, and more… | .NET Conf 2023](https://aka.ms/ef8-dotnetconf)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

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

- Docs: [What's new in EF8, with runnable samples](https://aka.ms/ef8-new)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="complextypes"></a>

### September 20: [Complex types as value objects in EF8](https://www.youtube.com/live/H-soJYqWSds?si=tJzm-uqLL0ZJnsUh)

The Entity Framework team returns with new community standups after a summer break heads-down working on EF8. In this session, we’ll look at the new “complex types” feature in EF8. Complex types do not have a key or any identity outside of the object’s value, but can still be deconstructed by property to map to different columns in the database. This allows either reference or value types to be used as DDD value objects with much better fidelity than when owned types are used. We’ll show examples of using complex types and talk about and answer your questions on the behavioral differences between entity types, owned entity types, complex types, and primitive types.

Featuring:

- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- Docs: [Unicode and UTF-8 in SQL Server with EF Core](xref:core/providers/sql-server/columns#unicode-and-utf-8)
- Docs: [What's new in EF8, with runnable samples](https://aka.ms/ef8-new)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="zumero"></a>

### June 14: [Synchronizing data between the cloud and the client (using SQLite)](https://www.youtube.com/live/QTvFiiST-r4?si=9SOUVhwz75VplWad)

In this session, we'll invite Eric Sink to discuss a solution for keeping a synchronized copy of the DB (using SQLite) on the client, rather than asking data for the cloud every time. This is helpful for mobile scenarios where the connectivity may be poor, but the "rep and sync" approach can reduce the time the user spends waiting on the network.  Local writes are fast, sync happens in the background. We'll explore how this can be done efficiently, how to deal with conflict resolution, and other data sync-related concerns.
Featuring:

- [Eric Sink](https://github.com/ericsink) (Special guest)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- Product: [Zumero](https://www.zumero.com/)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

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
- Docs: [What's new in EF8, with runnable samples](https://aka.ms/ef8-new)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="primitive-collections"></a>

### May 17: [Collections of primitive values in EF Core](https://www.youtube.com/live/AUS2OZjsA2I?si=V8hR14VyBtHuvbmp)

In this episode of the .NET Data Community Standup, the .NET Data Access team dive into new support for collections of primitive values, just released in EF Core 8 Preview 4. Collections of a primitive type can now be used as properties of an entity type and will be mapped to a JSON column in the relational database. In addition, parameters of primitive values can be passed to the database. In either case, the native JSON processing capabilities of the database are then used to exact and manipulate the primitive values, just as if they were in a table. This opens up powerful query possibilities, as well as optimizations to common problems such as translating queries that use Contains.

Featuring:

- [Shay Rojansky](https://github.com/roji) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)

Links:

- Docs: [What's new in EF8, with runnable samples](https://aka.ms/ef8-new)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="internals-models"></a>

### Apr 19: [EF Core Internals – Model Building](https://www.youtube.com/live/FYz0rAxQkC8?si=GsLuBBByuQbMurYP)

In this session, the .NET Data Access team will dig into the EF Core internals for building a model. EF models are built using a combination of three mechanisms: conventions, mapping attributes, and the model builder API. We will explain each of these mechanisms and show how they interact. We’ll also cover how models are cached, and ways in which that caching can be controlled. And, as always, we’ll answer your questions live!
Featuring:

- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- Docs: [What's new in EF8, with runnable samples](https://aka.ms/ef8-new)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="hierarchyid"></a>

### Mar 22: [Using hierarchical data in SQL Server and PostgreSQL with EF Core](https://www.youtube.com/live/pmnHGWYpCfg?si=h_v40tZ7NEw_wZ6W)

Join the .NET Data Access Team to learn about mapping hierarchical data structures to relational databases using EF Core. We’ll look at “hierarchyid” on SQL Server/Azure SQL and “ltree” on PostgresSQL, and show how to map tree structures such as a family tree, file system, or organization structure. EF Core can then be used to write LINQ queries against the hierarchy to find ancestors and descendants in various ways, as well as perform manipulation of subtrees for updates. And, as always, we’ll be here to answer your questions!

Featuring:

- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)
- [Brice Lambson](https://github.com/bricelam) (Host)

Links:

- GitHub: [ajcvickers/HierarchySample](https://github.com/ajcvickers/HierarchySample)
- Docs: [What's new in EF8, with runnable samples](https://aka.ms/ef8-new)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="internals-query"></a>

### Mar 8: [EF Core internals: IQueryable, LINQ and the EF Core query pipeline](https://www.youtube.com/live/1Ld3dtnTrMw?si=xWhbADioxqquACBi)

In this standup, we'll dive deep under the hood to see how EF Core processes LINQ queries, translates them to SQL and executes them on your database. We'll introduce key concepts such as IQueryable and LINQ providers, and see how EF Core uses caching to make your querying lightning-fast. Join us to understand how the magic works!

Featuring:

- [Shay Rojansky](https://github.com/roji) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)

Links:

- Docs: [What's new in EF8, with runnable samples](https://aka.ms/ef8-new)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

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
- Docs: [What's new in EF8, with runnable samples](https://aka.ms/ef8-new)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

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
- Docs: [What's new in EF8, with runnable samples](https://aka.ms/ef8-new)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

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

- Docs: [What's new in EF7, with runnable samples](https://aka.ms/ef7-new)
- Docs: [EF8 plan](https://aka.ms/ef8-plan)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

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

- Docs: [What's new in EF7, with runnable samples](https://aka.ms/ef7-new)
- Docs: [EF8 plan](https://aka.ms/ef8-plan)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="new-ef7"></a>

### Nov 16: [A Whirlwind Tour of EF7](https://www.youtube.com/live/FAQH1H9K6ng?si=tVAkyQSE2mrXX7nO)

Join the .NET Data team as we take a whirlwind tour through new features of EF Core 7.0 (EF7). This includes JSON columns, bulk updates, TPC mapping, and more!

Featuring:

- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- Docs: [What's new in EF7, with runnable samples](https://aka.ms/ef7-new)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="conventions"></a>

### Nov 7: [EF7 Custom Model Conventions](https://www.youtube.com/live/6apfe1L1FhY?si=g6NuYVtFmLCy9a6i)

EF Core uses a metadata "model" to describe how the application's entity types are mapped to the underlying database. This model is built using a set of around 60 "conventions". The model built by conventions can then be customized using mapping attributes (aka "data annotations") and/or calls to the DbModelBuilder API in OnModelCreating. EF7 allows applications to remove or modify existing conventions, as well as create new conventions. Join us for this .NET Data Community Standup to learn how to take advantage of custom model building conventions in your EF Core 7.0 applications.

Featuring:

- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)
- [Jiachen Jiang](https://github.com/jcjiang) (Host)

Links:

- Docs: [What's new in EF7, with runnable samples](https://aka.ms/ef7-new)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)
- Docs: [Model Bulk Configuration](xref:core/modeling/bulk-configuration)

<a name="bulkcud"></a>

### Oct 19: [New EF Core 7.0 APIs (Bulk Update)](https://www.youtube.com/live/rrKhbiXydKs?si=S5nUIdSerRg8Vqtp)

EF Core 7.0 introduces the ExecuteUpdate and ExecuteDelete APIs, which allow you to easily use LINQ to express a database update or delete operation. These APIs can be far more efficient for applying changes to multiple rows based on a condition, e.g. “delete rows which have InActive set to true” (AKA bulk update scenarios). But in some scenarios, they also allow you to express changes over a single row in a much more succinct and simple way, simplifying your code. ExecuteUpdate and ExecuteDelete completely bypass EF’s change tracking and execute immediately, as opposed to when SaveChanges is called. Tune in to learn more and to explore the new possibilities that these new APIs bring!

Featuring:

- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)
- [Jiachen Jiang](https://github.com/jcjiang) (Host)

Links:

- Docs: [What's new in EF7, with runnable samples](https://aka.ms/ef7-new)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)
- Docs: [ExecuteUpdate and ExecuteDelete](xref:core/saving/execute-insert-update-delete)

<a name="json"></a>

### Oct 5: [JSON Columns](https://www.youtube.com/live/0W2vcYMoJ8g?si=NKwrjgz569kQHM_J)

JSON columns allow relational databases to directly store documents while retaining the overall relational structure of the data. EF7 contains provider-agnostic support for JSON columns, with an implementation for SQL Server.  The JSON in these columns can queried using LINQ, allowing filtering and sorting by the elements of the documents, as well as projection of elements out of the documents into results. In addition, EF7 supports element-level change tracking of the documents and partial updates for only the changed elements when SaveChanges is called.

Featuring:

- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)
- [Jiachen Jiang](https://github.com/jcjiang) (Host)

Links:

- Docs: [What's new in EF7, with runnable samples](https://aka.ms/ef7-new)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="offline-sync"></a>

### Sep 21: [Azure Mobile Apps and offline sync](https://www.youtube.com/live/FbV4VWf7PfQ?si=x2arUB7YLRS79d3E)

Learn how Azure Mobile Apps provides the tools and libraries you need to easily synchronize data between an Entity Framework Core-backed Web API and your mobile or desktop app.

Featuring:

- [Adrian Hall](https://github.com/adrianhall) (Special guest)
- [Jeremy Likness](https://github.com/JeremyLikness) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- Product: [About Azure Mobile Apps](/azure/developer/mobile-apps/azure-mobile-apps/overview)
- Blog: [Adrian Hall's Blog](https://adrianhall.github.io/)
- Docs: [What's new in EF7, with runnable samples](https://aka.ms/ef7-new)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="ef7aggs"></a>

### Aug 24: [New aggregate function support in EF Core 7](https://www.youtube.com/live/IfaURw5D1Qg?si=DbxZ6xflfnds9CN0)

Aggregate functions such as COUNT, MAX or AVG compute a single value from multiple rows. EF7 adds support for custom provider aggregate functions, adding support for many new translations. In this episode of the .NET Data Community Standup, we’ll explore these new translations across different EF providers, and see what they could mean for more efficient data loading from the database. We’ll also go into some tricky design problems and under the hood query translation details which the team discussed while working on this feature.

Featuring:

- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)
- [Jiachen Jiang](https://github.com/jcjiang) (Host)

Links:

- Docs: [What's new in EF7, with runnable samples](https://aka.ms/ef7-new)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="wcf"></a>

### Aug 10: [CoreWCF: Roadmap and Q&A](https://www.youtube.com/live/OvaYdycmb-U?si=ucnqZXAoXDSCMZqE)

CoreWCF is a port of the service side of Windows Communication Foundation (WCF) to .NET Core. The goal of this project is to enable existing WCF services to move to .NET Core. The CoreWCF team recently conducted a survey and will be talking about some of  the responses, answering questions, and sharing insights into their roadmap for the future.

Featuring:

- [Matt Connew](https://github.com/mconnew) (Special guest)
- [Mike Rousos](https://github.com/HongGit) (Special guest)
- [Sam Spencer](https://devblogs.microsoft.com/dotnet/author/samsp/) (Special guest)
- [Hong Li](https://github.com/HongGit) (Special guest)
- [Heather Arbon](https://github.com/HeathAr) (Special guest)
- [Jeremy Likness](https://github.com/JeremyLikness) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- GitHub: [CoreWCF](https://github.com/CoreWCF/CoreWCF)

<a name="dbdatasource"></a>

### Jul 27: [DbDataSource, a new System.Data abstraction](https://www.youtube.com/live/vRUtHeUpU44?si=jm73JkMidEcZHJZj)

In this episode of the .NET data community standup, we'll explore DbDataSource, a new abstraction being introduced to System.Data in .NET 7.0. DbDataSource unlocks easier integration of ADO.NET drivers with dependency injection, better management of advanced driver configuration, helps performance and more. Tune in to see improvements at the lower levels of the .NET data stack.

Featuring:

- [Jeremy Likness](https://github.com/JeremyLikness) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- Docs: [What's new in EF7, with runnable samples](https://aka.ms/ef7-new)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="ef7interceptors"></a>

### Jul 13: [Intercept this EF7 Preview 6 Event](https://www.youtube.com/live/EJs3sSetr2Q?si=V-NxDGrPLPAsV-3f)

EF7 includes a variety of new interceptors and events, as well as many improvements to the existing lifecycle hooks. In this session, Arthur and others from the .NET Data team will dive into this new functionality, including examples for intercepting materialization of entities, query expression trees, identity resolution when tracking, and optimistic concurrency violations. Join us for a fun-packed ride into the world of modifying EF behavior through interception with code galore!

Featuring:

- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- Blog: [Announcing Entity Framework Core 7 Preview 6: Performance Edition](https://devblogs.microsoft.com/dotnet/announcing-ef-core-7-preview6-performance-optimizations/)
- Docs: [What's new in EF7, with runnable samples](https://aka.ms/ef7-new)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="lifetimes"></a>

### Jun 29: [DbContext Configuration and Lifetime - EF Core Architecture Part 2](https://www.youtube.com/live/NPgFlqXPbK8?si=k1HbnuniXaivGlAS)

Arthur and others from the EF Team continue their deep dive into the architecture of Entity Framework Core. This is a code-driven discussion looking at how and why things are designed and implemented the way they are. In this session, we look at the initialization and configuration of a DbContext instance, including selection of the provider, creating DbSet instances, reading DbContextOptions, and DbContext pooling.

Featuring:

- [Jeremy Likness](https://github.com/JeremyLikness) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- Docs: [DbContext Lifetime, Configuration, and Initialization](xref:core/dbcontext-configuration/index)
- Docs: [DbContext pooling](xref:core/performance/advanced-performance-topics#dbcontext-pooling)
- Docs: [What's new in EF7, with runnable samples](https://aka.ms/ef7-new)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="inheritance"></a>

### Jun 15: [TPH, TPT, and TPC Inheritance mapping with EF Core](https://www.youtube.com/live/HaL6DKW1mrg?si=s82taQNe2QUlZHOA)

Type inheritance hierarchies as used in object-oriented programming do not map naturally to relational database schemas. In this live stream, we will look at different strategies for inheritance mapping when using EF Core with a relational database. This includes table-per-hierarchy (TPH) mapping, table-per-type (TPT) mapping, and table-per-concrete type (TPC) mapping, which is new in EF Core 7.0 preview 5. We will look at the consequences of the mapping on the generated SQL and consider the performance and storage implications. This will result in some guidance as to when each strategy should be used.

Featuring:

- [Jeremy Likness](https://github.com/JeremyLikness) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- Blog: [Announcing Entity Framework 7 Preview 5](https://devblogs.microsoft.com/dotnet/announcing-ef7-preview5/)
- Docs: [Inheritance](xref:core/modeling/inheritance)
- Docs: [What's new in EF7, with runnable samples](https://aka.ms/ef7-new)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="internal-di"></a>

### Jun 1: [EF Core Architecture: Internal Dependency Injection](https://www.youtube.com/live/pYhe-Mt0HzI?si=SF8S0NSQnpTS4zBO)

Join Arthur Vickers and others from the EF Team as we start a deep dive into the architecture of Entity Framework Core. This won’t be a formal talk, but rather a free-form discussion driven by the code where we look how and why things are designed and implemented the way they are. This week, we’ll start with the use of dependency injection (D.I.) and the “internal service provider.” This is the backbone of how EF Core works as a service-oriented architecture, where the core code, database providers, and plugins all implement services that work together to provide EF’s functionality.

Featuring:

- [Jeremy Likness](https://github.com/JeremyLikness) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- Docs: [What's new in EF7, with runnable samples](https://aka.ms/ef7-new)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="testing2"></a>

### May 18: [Testing EF Core Apps (part 2)](https://www.youtube.com/live/4JQUJk8muCc?si=U81q0c5gEwdpe-dN)

In this second part of testing EF Core Apps, guest Jon P Smith will focus on the actual code and approaches that you need to test applications using EF Core and will address some key "pain points."

Featuring:

- [Jon P Smith](https://github.com/JonPSmith) (Special guest)
- [Jeremy Likness](https://github.com/JeremyLikness) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- Blog: [New features for testing your Entity Framework Core 5 and above](https://www.thereformedprogrammer.net/new-features-for-unit-testing-your-entity-framework-core-5-code/)
- Docs: [Testing against your production database system](xref:core/testing/testing-with-the-database)
- GitHub: [JonPSmith/EfCore.TestSupport](https://github.com/JonPSmith/EfCore.TestSupport)
- Blog: [Announcing Entity Framework 7 Preview 4](https://devblogs.microsoft.com/dotnet/announcing-entity-framework-7-preview-4/)
- Docs: [What's new in EF7, with runnable samples](https://aka.ms/ef7-new)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="pagination"></a>

### May 4: [Database Pagination](https://www.youtube.com/live/DIKH-q-gJNU?si=0TuIPGEEI25ve4i9)

Lots of data? Time to page it. But what's the best approach? TAKE, SKIP, and OFFSET? What about keyset pagination? Join the .NET Data team when we page the author of "keyset pagination for EF Core" Mohammed Rahhal to discuss paging and answer your questions live.

Featuring:

- [Mohammad Rahhal](https://github.com/mrahhal) (Special guest)
- [Jeremy Likness](https://github.com/JeremyLikness) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- GitHub: [mrahhal/MR.EntityFrameworkCore.KeysetPagination](https://github.com/mrahhal/MR.EntityFrameworkCore.KeysetPagination)
- GitHub: [mrahhal/MR.AspNetCore.Pagination](https://github.com/mrahhal/MR.AspNetCore.Pagination)
- Blog: [Pagination Done the Right Way](https://www.slideshare.net/MarkusWinand/p2d2-pagination-done-the-postgresql-way)
- Blog: [We need tool support for keyset pagination](https://use-the-index-luke.com/no-offset)
- Docs: [Pagination](xref:core/querying/pagination)
- Docs: [What's new in EF7, with runnable samples](https://aka.ms/ef7-new)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="update-perf"></a>

### Apr 20: [Performance Improvements to the EF7 Update Pipeline](https://www.youtube.com/live/EXbuRVqxn2o?si=tL_J_GCbMi1fFPrb)

Join the Entity Framework team as they review optimizations to the update pipeline to improve performance. Come prepared with your EF7 questions as we plan to spend time in Q&A to answer your questions live.

Featuring:

- [Jeremy Likness](https://github.com/JeremyLikness) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- Docs: [What's new in EF7, with runnable samples](https://aka.ms/ef7-new)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="t4"></a>

### Apr 6: [Database-first with T4 Templates in EF7: Early look](https://www.youtube.com/live/x2nh1vZBsHE?si=JA7iA1PoiDid5lzm)

Prefer null setters? Property initializers? Constructor initialization? What about auto-generating database diagrams that render in markdown using Mermaid? Learn how to take control of your database scaffolding in this early look at an EF7 feature that's hot off the press

Featuring:

- [Brice Lambson](https://github.com/bricelam) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Jeremy Likness](https://github.com/JeremyLikness) (Host)

Links:

- Docs: [Code Generation and T4 Text Templates](/visualstudio/modeling/code-generation-and-t4-text-templates)
- GitHub: [mono/t4](https://github.com/mono/t4)
- GitHub: [JeremyLikness/MvpSummitTaskList](https://github.com/JeremyLikness/MvpSummitTaskList/)
- Docs: [What's new in EF7, with runnable samples](https://aka.ms/ef7-new)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="graphql-odata"></a>

### Mar 9: [GraphQL and OData: An In-Depth Discussion](https://www.youtube.com/live/t7nkdORzed4?si=-2ov3X6DfuhVokWT)

The creator of the GraphQL library HotChocolate, Michael Staib, and Microsoft engineer and OData expert Hassan Habib discuss the various capabilities of GraphQL and OData and how they empower developers to go beyond REST.

Featuring:

- [Michael Staib](https://github.com/michaelstaib) (Special guest)
- [Hassan Rezk Habib](https://github.com/hassanhabib) (Special guest)
- [Jeremy Likness](https://github.com/JeremyLikness) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- Product: [OData - the best way to REST](https://www.odata.org/)
- Product: [Hot Chocolate - Introduction](https://chillicream.com/docs/hotchocolate/v13)
- Blog: [The Future of OData NxT (Neo)](https://devblogs.microsoft.com/odata/the-future-of-odata-odata-nxt/)
- Docs: [The Planetary Docs Sample](xref:core/providers/cosmos/planetary-docs-sample)
- Docs: [Multi-tenancy](xref:core/miscellaneous/multitenancy)

<a name="20years"></a>

### Feb 23: [Celebrating 20 Years of .NET: Entity Framework](https://www.youtube.com/live/kiPHP0KuSzM?si=IbVA62S4yn57meAF)

As .NET celebrates its 20th birthday, Entity Framework approaches its 10-year anniversary of going open source. The Entity Framework team will celebrate these milestones by inviting back the engineers and program managers who helped shipped previous versions to share their stories, demos, and anecdotes. Join Tim Laverty, Rowan Miller, Danny Simmons, Diego Vega, and more for this special edition of the .NET Data community standup.

Featuring:

- [Danny Simmons](https://github.com/simmdan) (Alumnus)
- [Rowan Miller](https://github.com/rowanmiller/) (Alumnus)
- [Julie Lerman](https://github.com/julielerman) (Special guest)
- [Diego Vega](https://github.com/divega) (Alumnus)
- [Andrew Peters](https://github.com/anpete) (Alumnus)
- [Tim Laverty](https://github.com/timlaverty) (Alumnus)
- [Jeremy Likness](https://github.com/JeremyLikness) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- Historic links:
  - [The ADO.NET Entity Framework Overview](/previous-versions/aa697427(v=vs.80))
  - [ADO.NET vNext: The Entity Framework, LINQ and more](/archive/blogs/dataaccess/ado-net-vnext-the-entity-framework-linq-and-more)
  - [Entity Framework Vote of No Confidence Signatories](https://efvote.wufoo.com/reports/entity-framework-vote-of-no-confidence-signatories/#public)
  - [DP Advisory Council](/archive/blogs/dsimmons/dp-advisory-council)
  - [RTM is Finally Here!](/archive/blogs/adonet/rtm-is-finally-here)
  - [Update on LINQ to SQL and LINQ to Entities Roadmap](/archive/blogs/adonet/update-on-linq-to-sql-and-linq-to-entities-roadmap)
  - [Simple Code First with Entity Framework 4 - Magic Unicorn Feature CTP 4](https://www.hanselman.com/blog/simple-code-first-with-entity-framework-4-magic-unicorn-feature-ctp-4)
  - [Entity Framework and Open Source](/archive/blogs/adonet/entity-framework-and-open-source)
  - [Introducing ADO.NET Entity Framework](https://www.codemag.com/article/0711051/Introducing-ADO.NET-Entity-Framework)
  - [Code First Databases with Entity Framework (Magic Unicorn Edition)](https://hanselminutes.com/223/code-first-databases-with-entity-framework-magic-unicorn-edition)
  - [Visual Studio 2008 and .NET Framework 3.5 Service Pack 1 Beta](https://weblogs.asp.net/scottgu/visual-studio-2008-and-net-framework-3-5-service-pack-1-beta)
  - [EF5 Released](/archive/blogs/adonet/ef5-released)
  - [EF6 RTM Available](/archive/blogs/adonet/ef6-rtm-available)
  - [EF7 - New Platforms, New Data Stores](/archive/blogs/adonet/ef7-new-platforms-new-data-stores)
  - [ASP.NET 5 is dead - Introducing ASP.NET Core 1.0 and .NET Core 1.0](https://www.hanselman.com/blog/aspnet-5-is-dead-introducing-aspnet-core-10-and-net-core-10)
  - [Announcing Entity Framework Core 1.0](https://devblogs.microsoft.com/dotnet/entity-framework-core-1-0-0-available/)
  - [Announcing Entity Framework Core 2.0](https://devblogs.microsoft.com/dotnet/announcing-entity-framework-core-2-0/)
  - [Announcing Entity Framework Core 3.0 and Entity Framework 6.3 General Availability](https://devblogs.microsoft.com/dotnet/announcing-ef-core-3-0-and-ef-6-3-general-availability/)
  - [Announcing the Release of EF Core 5.0](https://devblogs.microsoft.com/dotnet/announcing-the-release-of-ef-core-5-0/)
  - [Get to Know EF Core 6](https://devblogs.microsoft.com/dotnet/get-to-know-ef-core-6/)
  - [Announcing Entity Framework 7 Preview 1](https://devblogs.microsoft.com/dotnet/announcing-entity-framework-7-preview-1/)

<a name="stuff"></a>

### Feb 9: [Software version and "stuff"](https://www.youtube.com/live/Os7Rpm3LBkE?si=g9uwjlSvNdAYVatk)

Join the .NET Data team and Jon Skeet in a discussion around software versioning, issues with versioning in .NET, and… stuff. Who knows what topics may come up? Join us live to find out and have your questions answered in real time.

Featuring:

- [Jon Skeet](https://github.com/jskeet/) (Special guest)
- [Jeremy Likness](https://github.com/JeremyLikness) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- Jon Skeet's blog:
  - [WHY I DON’T START VERSIONS AT 0.X ANY MORE](https://codeblog.jonskeet.uk/2019/10/20/why-i-dont-start-versions-at-0-x-any-more/)
  - [OPTIONS FOR .NET’S VERSIONING ISSUES](https://codeblog.jonskeet.uk/2019/10/25/options-for-nets-versioning-issues/)
  - [VERSIONING LIMITATIONS IN .NET](https://codeblog.jonskeet.uk/2019/06/30/versioning-limitations-in-net/)
- Docs: [What's new in EF Core 6, with runnable samples](xref:core/what-is-new/ef-core-6.0/whatsnew)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="testing1"></a>

### Jan 26: [Testing EF Core Apps](https://www.youtube.com/live/KO2aFuLqGkc?si=ivF7Ma9vIEplxaKW)

What’s the best way to run automated tests on an application that uses Entity Framework Core? Jon P Smith, author of the book “Entity Framework Core in Action” covers three ways to create automated tests for your code and looks at the EF Core test “pain points” and how to get around them.

Featuring:

- [Jon P Smith](https://github.com/JonPSmith) (Special guest)
- [Jeremy Likness](https://github.com/JeremyLikness) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- Blog: [New features for testing your Entity Framework Core 5 and above](https://www.thereformedprogrammer.net/new-features-for-unit-testing-your-entity-framework-core-5-code/)
- Blog: [Using PostgreSQL in dev: Part 1 – Installing PostgreSQL on Windows](https://www.thereformedprogrammer.net/using-postgresql-in-development-part-1-installing-postgresql-on-windows/)
- Blog: [Using PostgreSQL in dev: Part 2 – Testing against a PostgreSQL database](https://www.thereformedprogrammer.net/using-postgresql-in-dev-part-2-testing-against-a-postgresql-database/)
- GitHub: [JonPSmith/EfCore.TestSupport](https://github.com/JonPSmith/EfCore.TestSupport)
- Docs: [Testing EF Core Applications](xref:core/testing/index)
- Video: [Unicorns Kickin': The EF Core rap official video.](https://youtu.be/QoIky9WQ9Dg?si=nqINdARz6KTtQ16M)
- Video: [20 GOTO 10: A cautionary tale about using the GOTO statement.](https://youtu.be/qd85U5mVWgo?si=RG9zhrZqcmRlhVOl)
- Blog: [What's new in EF Core 6, with runnable samples](xref:core/what-is-new/ef-core-6.0/whatsnew)
- Blog: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="ef7plan"></a>

### Jan 12: [The EF7 Plan](https://www.youtube.com/live/nU-mtUtbHV4?si=j5YTFfxudv0DOIPh)

The plan for EF7 has been published. In this episode, the EF Core/.NET Data team will review the details of the plan and answer live questions about the roadmap.

Featuring:

- [Jeremy Likness](https://github.com/JeremyLikness) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)
- [Brice Lambson](https://github.com/bricelam) (Host)

Links:

- [Announcing the Plan for EF7](https://devblogs.microsoft.com/dotnet/announcing-the-plan-for-ef7/)
- [Plan for Entity Framework Core 7.0](xref:core/what-is-new/ef-core-7.0/plan)
- [Port from EF6 to EF Core](xref:efcore-and-ef6/porting/index)
- [What's new in EF Core 6, with runnable samples](xref:core/what-is-new/ef-core-6.0/whatsnew)
- [EF Core daily builds](https://aka.ms/ef-daily-builds)

## 2021

<a name="hotchoc12"></a>

### Dec 1: [Hot Chocolate 12 and GraphQL 2021](https://www.youtube.com/live/3_4nt2QQSeE?si=WT7ecz52hz-SP4Rr)

Hot Chocolate 12 allows for more schema-building options with deeper integrations into EF core. Hot Chocolate has already implemented the new GraphQL October 2021 spec, and we will explore the new capabilities. We now support the complete stream and defer spec and will look into these new data fetching capabilities.

Featuring:

- [Michael Staib](https://github.com/michaelstaib) (Special guest)
- [Jeremy Likness](https://github.com/JeremyLikness) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- Product: [Hot Chocolate for GraphQL](https://chillicream.com/docs/hotchocolate/v12)
- Docs: [Hot Chocolate and Entity Framework Core](https://chillicream.com/docs/hotchocolate/v12/integrations/entity-framework)
- Blog: [Get to Know EF Core 6](https://devblogs.microsoft.com/dotnet/get-to-know-ef-core-6/)
- Docs: [What's new in EF Core 6, with runnable samples](xref:core/what-is-new/ef-core-6.0/whatsnew)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="retro6"></a>

### Nov 17: [Make History and Explore the Cosmos, an EF Core 6 Retrospective](https://www.youtube.com/live/cx6IUURncgk?si=CqKbnHztzd-Pv91l)

EF Core 6 has been released with a long list of features from performance improvements, temporal tables support, CI/CD-friendly migration bundles, compiled models, query improvements, enhanced Azure Cosmos DB capabilities and much more. Join the EF Core team and our community panelists as we review what's new, share the EF Core 6 story and answer your questions live.

Featuring:

- [Julie Lerman](https://github.com/julielerman) (Special guest)
- [Jon P Smith](https://github.com/JonPSmith) (Special guest)
- [Erik Ejlskov Jensen](https://github.com/ErikEJ) (Special guest)
- [Jeremy Likness](https://github.com/JeremyLikness) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- Video: [New Blazor WebAssembly capabilities in .NET 6](https://www.youtube.com/watch?v=kesUNeBZ1Os)
- Video: [Modern data APIs with EF Core and GraphQL](https://www.youtube.com/watch?v=GBvTRcV4PVA)
- Blog: [Updating your ASP.NET Core / EF Core application to NET 6](https://www.thereformedprogrammer.net/updating-your-asp-net-core-ef-core-application-to-net-6/)
- Video: [Entity Framework Community Standup - Introducing EF Core Compiled Models](https://www.youtube.com/watch?v=XdhX3iLXAPk)
- Video: [Entity Framework Community Standup - SQL Server Temporal Tables and EF Core 6](https://www.youtube.com/watch?v=2aCgKM41NFw)
- Blog: [Taking the EF Core Azure Cosmos DB Provider for a Test Drive](https://devblogs.microsoft.com/dotnet/taking-the-ef-core-azure-cosmos-db-provider-for-a-test-drive/)
- Video: [EF Core 6 and Azure Cosmos DB](https://www.youtube.com/watch?v=zQC9D00pr6I)
- Video: [What's New in EF Core 6](https://youtu.be/_1fJeW4F3ts?si=5GJh-NSqIyyIfRQd)
- Docs: [What's new in EF Core 6, with runnable samples](xref:core/what-is-new/ef-core-6.0/whatsnew)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="nodatime"></a>

### Nov 3: [Noda Time](https://www.youtube.com/live/ZLJLfImuFqM?si=bv_mbh2qzAeICfFG)

Noda Time is an alternative date and time API for .NET. It helps you to think about your data more clearly, and express operations on that data more precisely.

Featuring:

- [Jon Skeet](https://github.com/jskeet/) (Special guest)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- Blog: [Noda Time](https://blog.nodatime.org/2011/08/what-wrong-with-datetime-anyway.html)
- Blog: [STORING UTC IS NOT A SILVER BULLET](https://codeblog.jonskeet.uk/2019/03/27/storing-utc-is-not-a-silver-bullet/)
- Product: [nodatime.org](https://nodatime.org/)
- GitHub: [nodatime/nodatime](https://github.com/nodatime/nodatime)
- Blog: [Date/Time Mapping with NodaTime in Npgsql](https://www.npgsql.org/efcore/mapping/nodatime.html?tabs=with-datasource)
- GitHub: [StevenRasmussen/EFCore.SqlServer.NodaTime](https://github.com/StevenRasmussen/EFCore.SqlServer.NodaTime)
- GitHub: [khellang/EFCore.Sqlite.NodaTime](https://github.com/khellang/EFCore.Sqlite.NodaTime)
- Docs: [What's new in EF Core 6, with runnable samples](xref:core/what-is-new/ef-core-6.0/whatsnew)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="temporal"></a>

### Oct 6: [SQL Server Temporal Tables and EF Core 6](https://www.youtube.com/live/2aCgKM41NFw?si=q__izjX1qL0GHyqe)

Learn about the new support in EF Core 6 for temporal tables, including creating them from migrations, transforming existing tables into temporal tables, querying historical data and point-in-time restore.

Featuring:

- [Jeremy Likness](https://github.com/JeremyLikness) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)

Links:

- GitHub: [ajcvickers/TemporalTables](https://github.com/ajcvickers/TemporalTables)
- Docs: [What's new in EF Core 6, with runnable samples](xref:core/what-is-new/ef-core-6.0/whatsnew)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="aspnet"></a>

### Oct 20: [EF Core and ASP.NET Core from the ASP.NET documentation team](https://www.youtube.com/live/2wdJ0xjGF2o?si=V7pOPwty_8Oi1i36)

Back to the basics: EF Core and ASP.NET Core from the documentation team.

Featuring:

- [Rick Anderson](https://github.com/Rick-Anderson) (Special guest)
- [Jeremy Likness](https://github.com/JeremyLikness) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- Docs: [Razor Pages with Entity Framework Core in ASP.NET Core - Tutorial](/aspnet/core/data/ef-rp/intro)
- Docs: [ASP.NET Core MVC with EF Core - Tutorial](/aspnet/core/data/ef-mvc)
- Blog: [Prime your flux capacitor: SQL Server temporal tables in EF Core 6.0](https://devblogs.microsoft.com/dotnet/prime-your-flux-capacitor-sql-server-temporal-tables-in-ef-core-6-0/)
- Docs: [What's new in EF Core 6, with runnable samples](xref:core/what-is-new/ef-core-6.0/whatsnew)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="npgsql"></a>

### Sep 22: [PostgreSQL and EF Core](https://www.youtube.com/live/Ya_cmZRwACM?si=FSx9ajf5fXrwz5t7)

PostgreSQL has some advanced capabilities not usually found in other relational databases. In this session we'll go over some of them, and see how EF Core makes these accessible.

Featuring:

- [Shay Rojansky](https://github.com/roji) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Brice Lambson](https://github.com/bricelam) (Host)

Links:

- Product: [Npgsql Entity Framework Core Provider](https://www.npgsql.org/efcore/index.html)
- Docs: [What's new in EF Core 6, with runnable samples](xref:core/what-is-new/ef-core-6.0/whatsnew)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="dapper"></a>

### Aug 25: [Dapper](https://www.youtube.com/live/txiQar6PqvA?si=wIEFckHrk2RD2z4z)

Dapper maintainers and key contributors Nick and Marc will show people the glorious power of Dapper and everything it can do, so they are fully informed and can choose something much better, like EF Core (their words).

Featuring:

- [Marc Gravell](https://github.com/mgravell)
- [Nick Craver](https://github.com/NickCraver)
- [Jeremy Likness](https://github.com/JeremyLikness) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- GitHub: [JeremyLikness/EFCosmosQuickstart](https://github.com/JeremyLikness/EFCosmosQuickstart)
- GitHub: [DapperLib/Dapper](https://github.com/DapperLib/Dapper)
- GitHub: [DapperLib/DapperAOT](https://github.com/DapperLib/DapperAOT)
- Product: [Dapper](https://dapperlib.github.io/Dapper/)
- GitHub: [SqlServer.Core: Performance-oriented SQL Server .NET driver](https://github.com/dotnet/datalab/issues/6)
- Blog: [Is the era of reflection-heavy C# libraries at an end?](https://blog.marcgravell.com/2021/05/is-era-of-reflection-heavy-c-libraries.html)
- Blog: [.NET Core, .NET 5; the exodus of .NET Framework?](https://blog.marcgravell.com/2020/01/net-core-net-5-exodus-of-net-framework.html)
- Docs: [What's new in EF Core 6, with runnable samples](xref:core/what-is-new/ef-core-6.0/whatsnew)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="query-filters"></a>

### Aug 11: [EF Core's Global Query Filter](https://www.youtube.com/live/Uy0c_DKGM-U?si=5iLoEM8ZdKjXXjdY)

Why do customers want "soft delete"? What's a good practice for handling multi-tenancy in EF Core apps? The reformed programmer, Jon P. Smith, shares how to use a powerful tool in the EF Core platform, global query filters, to handle scenarios like "soft delete."

Featuring:

- [Jon P Smith](https://github.com/JonPSmith) (Special guest)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- Docs: [Global Query Filters](xref:core/querying/filters)
- Blog: [EF Core In depth – Soft deleting data with Global Query Filters](https://www.thereformedprogrammer.net/ef-core-in-depth-soft-deleting-data-with-global-query-filters/)
- Blog: [Introducing the EfCore.SoftDeleteServices library to automate soft deletes](https://www.thereformedprogrammer.net/introducing-the-efcore-softdeleteservices-library-to-automate-soft-deletes/)
- Blog: [Part 2: Handling data authorization in ASP.NET Core and Entity Framework Core](https://www.thereformedprogrammer.net/part-2-handling-data-authorization-asp-net-core-and-entity-framework-core/)
- Blog: [Part 4: Building a robust and secure data authorization with EF Core](https://www.thereformedprogrammer.net/part-4-building-a-robust-and-secure-data-authorization-with-ef-core/)
- Docs: [What's new in EF Core 6, with runnable samples](xref:core/what-is-new/ef-core-6.0/whatsnew)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="odata"></a>

### Jul 28: [OData](https://www.youtube.com/live/Q3Ove-2Uh94?si=VPRzsiqO-S9nD5bs)

OData is one of the best protocols out there to supercharge your ASP.NET Core APIs with so many capabilities such as shaping, filtering, batching and ordering the data on the fly - it executes it's queries on the server which tremendously improves the performance on client side. Hassan talks all things OData on the Community Standup.

Featuring:

- [Hassan Rezk Habib](https://github.com/hassanhabib) (Special guest)
- [Jeremy Likness](https://github.com/JeremyLikness) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- Blog: [Up & Running w/ OData in ASP.NET 6](https://devblogs.microsoft.com/odata/up-running-w-odata-in-asp-net-6/)
- GitHub: [OData NxT - The Future of OData (Project Proposal)](https://github.com/OData/OData.Neo/issues/4)
- Video: [Enabling OData in ASP.NET 6.0](https://www.youtube.com/watch?v=w0Tj0VIUCtA)
- Video: [OData NxT 002: OData Queries Deep Dive](https://youtu.be/6AvFqhkALmg?si=KY5_pauTt_weCMRN)
- Docs: [What's new in EF Core 6, with runnable samples](xref:core/what-is-new/ef-core-6.0/whatsnew)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="query-plans"></a>

### Jul 14: [Visualizing database query plans](https://www.youtube.com/live/Zhy5antRDJk?si=avlq-ll147UyRcIt)

Learn what a query plan is, how to view EF Core query plan in LINQPad, find missing indexes, and improve performance of your queries.

Featuring:

- [Giorgi Dalakishvili](https://github.com/Giorgi)
- [Jeremy Likness](https://github.com/JeremyLikness) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- GitHub: [Giorgi/LINQPad.QueryPlanVisualizer](https://github.com/Giorgi/LINQPad.QueryPlanVisualizer)
- GitHub: [dalibo/pev2](https://github.com/dalibo/pev2/)
- GitHub: [JustinPealing/html-query-plan](https://github.com/JustinPealing/html-query-plan)
- Docs: [SQL Server: Execution plan overview](/sql/relational-databases/performance/execution-plans)
- Docs: [SQL Server: Logical and physical showplan operator reference](/sql/relational-databases/showplan-logical-and-physical-operators-reference)
- Docs: [SQL Server: Statistics](/sql/relational-databases/statistics/statistics)
- Blog: [Announcing Entity Framework Core 6.0 Preview 6: Configure Conventions](https://devblogs.microsoft.com/dotnet/announcing-entity-framework-core-6-0-preview-6-configure-conventions/)
- Docs: [What's new in EF Core 6, with runnable samples](xref:core/what-is-new/ef-core-6.0/whatsnew)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="cosmos6"></a>

### Jun 16: [Azure Cosmos DB and EF Core](https://www.youtube.com/live/nEqH_XfCfho?si=rq5L2ciDW37atihz)

EF Core may be an object relational mapper, but it also can ditch the relational and connect directly to the Azure Cosmos DB NoSQL engine. In this standup, the team will discuss the rationale behind a Cosmos DB provider, show code examples, discuss the major differences between SQL and NoSQL data and share the roadmap for future enhancements to the Azure Cosmos DB provider.

Featuring:

- [Jeremy Likness](https://github.com/JeremyLikness) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- GitHub: [JeremyLikness/PlanetaryDocs](https://github.com/JeremyLikness/PlanetaryDocs)
- Docs: [What's new in EF Core 6, with runnable samples](xref:core/what-is-new/ef-core-6.0/whatsnew)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="compiled-models"></a>

### Jun 2: [Introducing EF Core Compiled Models](https://www.youtube.com/live/XdhX3iLXAPk?si=DzUmXMMLWFh8XSXv)

EF Core creates an internal model for entity types and their database mappings. This model is typically built once when the application starts. Starting with EF Core 6.0, it is now possible to instead build this model at compile time. This can reduce startup time by up to 67% for large models. Join us for this Community Standup, where the team will introduce compiled models and demo their use.

Featuring:

- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)
- [Andriy Svyryd](https://github.com/AndriySvyryd) (Host)

Links:

- GitHub: [Reduce EF Core application startup time via compiled models](https://github.com/dotnet/efcore/issues/1906)
- Docs: [What's new in EF Core 6, with runnable samples](xref:core/what-is-new/ef-core-6.0/whatsnew)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="graphql1"></a>

### May 19: [Building Modern Apps with GraphQL](https://www.youtube.com/live/4k3WzW2ZdXs?si=DXqIxVNNlbMILNeT)

GraphQL is a great way to expose data whether you expose your business layer in a much richer way or just map your data models to get started quickly. GraphQL gives backend engineers the tools to mold the service layer without compromises and lets the frontend engineer decide what data he or she needs to build that awesome application. Learn the what, why, and see how to build apps with GraphQL.

Featuring:

- [Michael Staib](https://github.com/michaelstaib) (Special guest)
- [Jeremy Likness](https://github.com/JeremyLikness) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- Interview: [Deep Diving into EF Core: Q&A with Jeremy Likness](https://www.infoq.com/articles/deep-diving-ef-core-jeremy-likness/)
- Book: [Entity Framework Core in Action, Second Edition](https://www.manning.com/books/entity-framework-core-in-action-second-edition)
- Blog: [Azure Cosmos DB With EF Core on Blazor Server](https://blog.jeremylikness.com/blog/azure-cosmos-db-with-ef-core-on-blazor-server/)
- GitHub: [ChilliCream/graphql-platform](https://github.com/ChilliCream/graphql-platform)
- Product: [Hot Chocolate - Introduction](https://chillicream.com/docs/hotchocolate/v13)
- Docs: [What's new in EF Core 6, with runnable samples](xref:core/what-is-new/ef-core-6.0/whatsnew)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="triggers"></a>

### May 5: [Triggers for EF Core](https://www.youtube.com/live/Gjys0Yebobk?si=JcRRypyCPZxjho0k)

Guest Koen Bekkenutte will introduce us to the EFCore.Triggered project and discuss its advantages and how it differs from domain events and database triggers. See how it is implemented and how the project uses EF Core's ChangeTracker and SaveChanges interception. As always, the EF Core team and Koen will be responding to live Q&A.

Featuring:

- [Koen](https://github.com/koenbeuk)
- [Jeremy Likness](https://github.com/JeremyLikness) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- Blog: [Multi-tenancy with EF Core in Blazor Server Apps](https://blog.jeremylikness.com/blog/multitenancy-with-ef-core-in-blazor-server-apps/)
- Blog: [Triggers for Entity Framework Core](https://onthedrift.com/posts/efcore-triggered-part1/)
- GitHub: [koenbeuk/EntityFrameworkCore.Triggered](https://github.com/koenbeuk/EntityFrameworkCore.Triggered)
- Docs: [What's new in EF Core 6, with runnable samples](xref:core/what-is-new/ef-core-6.0/whatsnew)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="contributing"></a>

### Apr 21: [Open Source Contributions: How to Add a Feature to EF Core](https://www.youtube.com/live/9OMxy1wal1s?si=WSH5zxI8L23WLoy7)

Do you want to contribute to EF Core but are uncertain where to begin? We’re here to help! In this episode of the EF Core Community Standup, we will add an EF Core feature live. We’ll go through everything from forking the repo, adding the code and related tests, and generating the pull request. Learn how the codebase is organized, how to build and how to run tests and receive answers to your questions in real time direct from the team that brings you magic unicorns!

Featuring:

- [Erik Ejlskov Jensen](https://github.com/ErikEJ) (Special guest)
- [Jeremy Likness](https://github.com/JeremyLikness) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- Blog: [Entity Framework Core 5 — Table-per-Type and Table-per-Hierarchy](https://henriquesd.medium.com/entity-framework-core-5-table-per-type-and-table-per-hierarchy-215ff85e4ae8)
- Blog: [Integration testing with EF Core, part 1](https://dev.to/maxx_don/integration-testing-with-ef-core-part-1-1l40)
- Blog: [Look ma, no passwords - using Entity Framework Core with Azure Managed Identity, App Service/Functions and Azure SQL DB](https://erikej.github.io/efcore/azure/2021/04/20/efcore-managed-identity.html)
- Docs: [What's new in EF Core 6, with runnable samples](xref:core/what-is-new/ef-core-6.0/whatsnew)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="davide"></a>

### Apr 7: [Azure SQL for Cloud-Born Applications and Developers](https://www.youtube.com/live/GhIhwCafilk?si=gyyq5PgHv-qf8UV0)

Join Davide Mauri and the EF Core team for an interactive discussion about how Azure SQL helps cloud developers create cloud native solutions. From simple REST APIs to scalable backends, Azure SQL provides the required flexibility, scalability, and features to simplify the developer's life. Davide will cover topics ranging from JSON support and CI/CD to column-store, geospatial and graph models.

Featuring:

- [Davide Mauri](https://github.com/yorek) (Special guest)
- [Jeremy Likness](https://github.com/JeremyLikness) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)

Links:

- Docs: [Connection Resiliency](xref:core/miscellaneous/connection-resiliency)
- Docs: [What's new in EF Core 6, with runnable samples](xref:core/what-is-new/ef-core-6.0/whatsnew)
- Book: [Practical Azure SQL Database for Modern Developers: Building Applications in the Microsoft Cloud](https://www.amazon.com/Practical-Azure-Database-Modern-Developers/dp/1484263693/)
- GitHub: [yorek/azure-sql-db-samples](https://github.com/yorek/azure-sql-db-samples)
- GitHub: [azure-samples/azure-sql-db-dynamic-schema](https://github.com/azure-samples/azure-sql-db-dynamic-schema)
- Product: [TodoMVC](https://todomvc.com/)
- Product: [Todo-Backend](https://www.todobackend.com/)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="newbatch"></a>

### Mar 24: [EF Core Power Tools: the New Batch](https://www.youtube.com/live/3-Izu_qLDqY?si=Tx1Cbwvtq_GI_dmG)

Learn how to take advantage of the new advanced features recently added to EF Core Power Tools: - reverse engineer .dacpac files, renaming of tables, columns and even navigations, excluding columns, working with multiple DbContexts in the same project, map spatial columns and SQL Server stored procedures. The session will also include a look under the hood of EF Core Power Tools reverse engineering.

Featuring:

- [Erik Ejlskov Jensen](https://github.com/ErikEJ) (Special guest)
- [Jeremy Likness](https://github.com/JeremyLikness) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- GitHub: [ErikEJ/EFCorePowerTools](https://github.com/ErikEJ/EFCorePowerTools)
- Docs: [What's new in EF Core 6, with runnable samples](xref:core/what-is-new/ef-core-6.0/whatsnew)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="julie"></a>

### Mar 10: [Julie Lerman and EF Core Ask Me Anything (AMA)](https://www.youtube.com/live/oZVsZrFKp48?si=OqUUNoea5R5ds5rg)

Julie Lerman is an independent authority and community luminary for Entity Framework. She has been programming for nearly 30 years and has been an independent consultant since 1989. Julie is one of 150 Microsoft Regional Directors worldwide and has been awarded Microsoft MVP annually since 2003. Join Julie and the EF Core team for an "ask me anything" session: your questions answered in real-time!

Featuring:

- [Julie Lerman](https://github.com/julielerman) (Special guest)
- [Erik Ejlskov Jensen](https://github.com/ErikEJ) (Special guest)
- [Jeremy Likness](https://github.com/JeremyLikness) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- Product: [The Data Farm is Julie Lerman](https://thedatafarm.com/)
- Videos: [Julie Lerman on Pluralsight](https://www.pluralsight.com/authors/julie-lerman)
- Docs: [What's new in EF Core 6, with runnable samples](xref:core/what-is-new/ef-core-6.0/whatsnew)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="jonp1"></a>

### Feb 24: [Performance Tuning an EF Core App](https://www.youtube.com/live/VgNFFEqwZPU?si=3XQwMvO5kWMemi7T)

Guest Jon P Smith shows how he built a demo e-commerce book selling site that uses EF Core. He started with 700 books then scaled through 100,000 books to ½ million books. At each stage he compares the performance of each improvement, and the extra work to get that performance.

Featuring:

- [Jon P Smith](https://github.com/JonPSmith) (Special guest)
- [Jeremy Likness](https://github.com/JeremyLikness) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- Docs: [Introduction to Performance](xref:core/miscellaneous/performance/index)
- Blog: [Entity Framework Core 5 – Pitfalls To Avoid and Ideas to Try](https://blog.jetbrains.com/dotnet/2021/02/24/entity-framework-core-5-pitfalls-to-avoid-and-ideas-to-try/)
- Blog: [Five levels of performance tuning for an EF Core query](https://www.thereformedprogrammer.net/five-levels-of-performance-tuning-for-an-ef-core-query/)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="typed-exceptions"></a>

### Feb 10: [Typed Exceptions for Entity Framework Core](https://www.youtube.com/live/aUl5QfswNU4?si=zbglNkNEElISaVpb)

When using Entity Framework Core for data access all database exceptions are wrapped in DbUpdateException. If you need to know whether the exception was caused by a unique constraint, the value being too long, or the value missing for a required column, you need to dig into the concrete DbException subclass instance and check the error number to determine the exact cause. In this episode, learn how EntityFramework.Exceptions handles all the database-specific details and allows you to use typed exceptions for Entity Framework Core when your query violates database constraints.

Featuring:

- [Giorgi Dalakishvili](https://github.com/Giorgi) (Special guest)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- GitHub: [Giorgi/EntityFramework.Exceptions](https://github.com/Giorgi/EntityFramework.Exceptions)
- Docs: [The Plan for Entity Framework Core 6.0](https://devblogs.microsoft.com/dotnet/the-plan-for-entity-framework-core-6-0/)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="sqlproj"></a>

### Jan 27: [Introducing MSBuild.Sdk.SqlProj](https://www.youtube.com/live/lmHU1zD2mvA?si=75sWlZjkLDbakuzG)

MSBuild.Sdk.SqlProj is a new open source project that allows you to build SQL Server Database Projects (.sqlproj) using the new SDK-style projects cross-platform without the need to have Visual Studio installed. Jonathan will show why you would use it, how you can get started and walk you through its various features. He'll also show how you can contribute to the project.

Featuring:

- [Jonathan Mezach](https://github.com/jmezach) (Special guest)
- [Erik Ejlskov Jensen](https://github.com/ErikEJ) (Special guest)
- [Jeremy Likness](https://github.com/JeremyLikness) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- Blog: [Preview: The SQLite Llibrary as a .NET assembly](https://ericsink.com/entries/sqlite_llama_preview.html)
- GitHub: [rr-wfm/MSBuild.Sdk.SqlProj](https://github.com/rr-wfm/MSBuild.Sdk.SqlProj)
- Blog: [How to update a database’s schema without using EF Core’s migrate feature](https://www.thereformedprogrammer.net/how-to-update-a-databases-schema-without-using-ef-cores-migrate-feature/)
- Docs: [The Plan for Entity Framework Core 6.0](https://devblogs.microsoft.com/dotnet/the-plan-for-entity-framework-core-6-0/)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="survey"></a>

### Jan 13: [EF Core 6.0 Survey Results](https://www.youtube.com/live/IiAS61uVDqE?si=EPu3HUNiThsWm88f)

We asked, and you listened! Nearly 4,000 developers responded to the EF Core 2020 survey. In this edition of the community standup, senior program manager Jeremy Likness will review the survey results and discuss how they impact the plan for EF Core 6.0. The team will be available to answer your questions live.

Featuring:

- [Jeremy Likness](https://github.com/JeremyLikness) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- Blog: [HotChocolate: Introduction to GraphQL for ASP.NET Core (Part 1)](https://dev.to/jioophoenix/hotchocolate-introduction-to-graphql-for-asp-net-core-part-1-2e27)
- Blog: [Entity Framework Core Spotify data seed generator](https://driesdeboosere.dev/blog/entity-framework-core-spotify-data-seed-generator/#spotify-seed-data-generator-manual)
- Blog: [Entity Framework Core 5 free resources](https://erikej.github.io/efcore/2021/01/05/efcore-5-resources.html)
- Blog: [Announcing the Release of EF Core 5.0](https://devblogs.microsoft.com/dotnet/announcing-the-release-of-ef-core-5-0/)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

## 2020

<a name="frans"></a>

### Dec 2: [LLBLGen designer and .NET data history](https://www.youtube.com/live/notUk3yR0mc?si=mUHTlgHjXwCxAW1Z)

In this episode we welcome guest Frans Bouma, who will show us how to graphically model a domain for any .NET ORM with the LLBLGen Pro designer. Frans is also a venerable .NET data and SqlServer figure, so we will chat a lot about history and what's it's been like to make a commercial ORM in the Microsoft world.

Featuring:

- [Frans Bouma](https://github.com/FransBouma) (Special guest)
- [Jeremy Likness](https://github.com/JeremyLikness) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- Product: [LLBLGen Pro](https://www.llblgen.com/)
- Docs: [Announcing the Release of EF Core 5.0](https://devblogs.microsoft.com/dotnet/announcing-the-release-of-ef-core-5-0/)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="efcore5"></a>

### Nov 18: [Special EF Core 5.0 Community Panel](https://www.youtube.com/live/AkqRn2vr1lc?si=uAgpUza2w2k6HChb)

In this special edition of the EF Core Community Standup, we celebrate the release of EF Core 5.0 with a community panel. We'll welcome Entity Framework luminaries Diego Vega, Erik E Jensen, Jon P Smith and Julie Lerman to discuss their favorite features and answer your questions live.

Featuring:

- [Erik Ejlskov Jensen](https://github.com/ErikEJ) (Special guest)
- [Julie Lerman](https://github.com/julielerman) (Special guest)
- [Jon P Smith](https://github.com/JonPSmith) (Special guest)
- [Diego Vega](https://github.com/divega) (Alumnus)
- [Jeremy Likness](https://github.com/JeremyLikness) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)
- [Brice Lambson](https://github.com/bricelam) (Host)

Links:

- Docs: [Announcing the Release of EF Core 5.0](https://devblogs.microsoft.com/dotnet/announcing-the-release-of-ef-core-5-0/)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="collations"></a>

### Oct 28: [EF Core 5.0 Collations](https://www.youtube.com/live/OgMhLVa_VfA?si=bhjMpN1l_N9RYYYN)

In this community standup, we'll be showing new features around case sensitivity and collations in 5.0. We'll also provide a glimpse into how these features were designed, and what considerations and constraints guide the EF team - performance, cross-database support, usability and more. Come see how we design EF features under the hood!

Featuring:

- [Jeremy Likness](https://github.com/JeremyLikness) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- Docs: [Collations and Case Sensitivity](xref: core/miscellaneous/collations-and-case-sensitivity)
- Video: [Using Entity Framework Core with Azure SQL DB and Azure Cosmos DB | Azure Friday](https://www.youtube.com/watch?v=FFgS_k_Muk8)
- Blog: [Using Azure Identity with Azure SQL, Graph, and Entity Framework](https://devblogs.microsoft.com/azure-sdk/azure-identity-with-sql-graph-ef/)
- Blog: [The Curious Case of Commands and Cancellation](https://www.roji.org/db-commands-and-cancellation)
- Blog: [Setting the command timeout with the latest .NET SqlClient](https://erikej.github.io/sqlclient/2020/10/26/sqlclient-commandtimeout-preview.html)
- Blog: [Help Us Plan EF Core 6.0](https://devblogs.microsoft.com/dotnet/help-us-plan-ef-core-6-0/)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="repository"></a>

### Oct 14: [Cosmos DB: Repository Pattern .NET Wrapper SDK](https://www.youtube.com/live/Ejbgqq8vETM?si=kyCTJenwpGqQVtFC)

In this episode, fellow .NET TV host David Pine will join Jeremy Likness to discuss his Cosmos DB Repository pattern .NET SDK. The SDK aims to simplify the Cosmos DB .NET SDK by abstracting away some of the complexities, exposing an elegant API surface area, and providing C.R.U.D. operations through the IRepository interface.

As always, the EF Core team will be available to answer questions live.

Featuring:

- [David Pine](https://github.com/IEvangelist) (Special guest)
- [Jeremy Likness](https://github.com/JeremyLikness) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)
- [Brice Lambson](https://github.com/bricelam) (Host)

Links:

- Video: [The .NET Docs Show: Data & .NET](https://dev.to/dotnet/the-net-docs-show-data-net-1n42)
- Blog: [Announcing Entity Framework Core (EF Core) 5 RC2](https://devblogs.microsoft.com/dotnet/announcing-entity-framework-core-ef-core-5-rc2/)
- Blog: [Azure Cosmos DB Repository .NET SDK v.1.0.4](https://devblogs.microsoft.com/cosmosdb/azure-cosmos-db-repository-net-sdk-v-1-0-4/)
- GitHub: [IEvangelist/azure-cosmos-dotnet-repository](https://github.com/IEvangelist/azure-cosmos-dotnet-repository)
- GitHub: [IEvangelist/DotNetDocs.Show](https://github.com/IEvangelist/DotNetDocs.Show)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="geo-nts"></a>

### Sep 30: [Geographic Data with NetTopologySuite](https://www.youtube.com/live/IHslY5rrxD0?si=QpNNSZql1UsydmHz)

Joe Amenta joins us to discuss how to add spatial data to your applications to open a world of new relationships between data. He will demonstrate the "status quote" for querying spatial data from .NET Core and demo integrations with NetTopologSuite and EF Core.

Featuring:

- [Joe Amenta](https://github.com/airbreather) (Special guest)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Brice Lambson](https://github.com/bricelam) (Host)

Links:

- GitHub: [NetTopologySuite](https://github.com/NetTopologySuite)
- Blog: [Run EF Core Queries on SQL Server From Blazor WebAssembly](https://blog.jeremylikness.com/blog/run-efcore-queries-against-sql-from-blazor-webassembly/)
- Docs: [NetTopologySuite](https://nettopologysuite.github.io/NetTopologySuite/api/NetTopologySuite.html)
- GitHub: [locationtech/jts](https://github.com/locationtech/jts)
- Blog: [Microsoft.Data.Sqlite 5.0](https://www.bricelam.net/2020/09/23/microsoft-data-sqlite-5-0.html)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="migrations5"></a>

### Sep 16: [What's New with Migrations in EF Core 5.0](https://www.youtube.com/live/mSsGERmrhnE?si=mw1jCDxDbERDrfAi)

The Entity Framework Core team focused on major improvements to migrations for the EF Core 5.0 release. Learn about what's new and explore different migrations scenarios in this demo-heavy session. As always, the team will be standing by live to answer your questions!

Featuring:

- [Brice Lambson](https://github.com/bricelam) (Host)
- [Jeremy Likness](https://github.com/JeremyLikness) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- Blog: [Oracle EF Core 3.1 Production Release](https://medium.com/oracledevs/oracle-ef-core-3-1-production-release-9e470eaf3d03)
- Blog: [Announcing Entity Framework Core (EFCore) 5.0 RC1](https://devblogs.microsoft.com/dotnet/announcing-entity-framework-core-efcore-5-0-rc1/)
- Docs: [Migrations Overview](xref: core/managing-schemas/migrations/index)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="dotmin-sync"></a>

### Sep 2: [Sync your database with DotMim.Sync](https://www.youtube.com/live/_SHryJiblRo?si=rmlPMxFWPRYHXDvX)

The maintainer of open source software solution DotMim.Sync joins the EF Core team to discuss how the project helps keep local relational databases in sync.

Featuring:

- [Sébastien Pertus](https://github.com/Mimetis) (Special guest)
- [Jeremy Likness](https://github.com/JeremyLikness) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)
- [Brice Lambson](https://github.com/bricelam) (Host)

Links:

- GitHub: [Mimetis/Dotmim.Sync](https://github.com/Mimetis/Dotmim.Sync)
- Blog: [Big Plans for Big Data and .NET for Spark](https://devblogs.microsoft.com/dotnet/big-plans-for-big-data-and-net-for-spark/)
- Blog: [Tree Structure in EF Core: How to configure a self-referencing table and use it](https://habr.com/en/articles/516596/)
- Blog: [Using a Full Framework SQL Server Project in a .NET core project build.](https://shawtyds.wordpress.com/2020/08/26/using-a-full-framework-sql-server-project-in-a-net-core-project-build/)
- Blog: [Announcing Entity Framework Core (EF Core) 5.0 Preview 8](https://devblogs.microsoft.com/dotnet/announcing-entity-framework-core-ef-core-5-0-preview-8/)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="many-to-many"></a>

### Aug 19: [Many-to-Many in EF Core 5.0](https://www.youtube.com/live/W1sxepfIMRM?si=JSBiCbeA3jM0tSz8)

Join the team as they explore the latest many-to-many mapping features implemented for EF Core 5.0 including skip navigations and more!

Featuring:

- [Jeremy Likness](https://github.com/JeremyLikness) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Brice Lambson](https://github.com/bricelam) (Host)
- [Smit Patel](https://github.com/smitpatel) (Host)

Links:

- Blog: [Announcing Entity Framework Core (EF Core) 5.0 Preview 8](https://devblogs.microsoft.com/dotnet/announcing-entity-framework-core-ef-core-5-0-preview-8/)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="depth"></a>

### Aug 5: [EF Core In Depth Video Series](https://www.youtube.com/live/b-zTazj2vuI?si=A9lCvvHva3AomWe7)

Join members from the .NET teams for our community standup covering great community contributions for Framework, .NET Core, Languages, CLI, MSBuild, and more.

Featuring:

- [Philip Japikse](https://github.com/skimedic) (Special guest)
- [Robert Green](https://dotnet.microsoft.com/en-us/live/vs-toolbox) (Special guest)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)
- [Smit Patel](https://github.com/smitpatel) (Host)

Links:

- Video: [Entity Framework Core In-Depth](/shows/visual-studio-toolbox/entity-framework-core-in-depth-part-1)
- Video: [The Intersection of Microservices, Domain-Driven Design and Entity Framework Core](https://www.youtube.com/watch?v=DG8Qe7TJiIE)
- GitHub: [julielerman/dotnetconfms2020](https://github.com/julielerman/dotnetconfms2020)
- Blog: [How to call stored procedures with OUTPUT parameters with FromSqlRaw in EF Core](https://erikej.github.io/efcore/2020/08/03/ef-core-call-stored-procedures-out-parameters.html)
- Docs: [Getting Started with WPF](xref:core/get-started/wpf)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="handlebars"></a>

### Jul 22: [Using Scaffolding with Handlebars](https://www.youtube.com/live/6Ux7EpgiWXE?si=78XhoFrViXVkMXOL)

Join members from the .NET teams for our community standup covering great community contributions for Framework, .NET Core, Languages, CLI, MSBuild, and more.

Featuring:

- [Anthony Sneed](https://github.com/tonysneed) (Special guest)
- [Brice Lambson](https://github.com/bricelam) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- GitHib: [fiseni/QuerySpecification](https://github.com/fiseni/QuerySpecification)
- GitHub: [matteobortolazzo/couchdb-net](https://github.com/matteobortolazzo/couchdb-net)
- Blog: [All in One with OData $Batch](https://devblogs.microsoft.com/odata/all-in-one-with-odata-batch/)
- Blog: [Azure Cosmos DB - SQL API Geo-Replication Using EF Core](https://www.c-sharpcorner.com/article/azure-cosmos-db-sql-api-geo-replication-using-ef-core-part-three/)
- Blog: [Announcing Entity Framework Core EF Core 5.0 Preview 7](https://devblogs.microsoft.com/dotnet/announcing-entity-framework-core-ef-core-5-0-preview-7/)
- GitHub: [TrackableEntities/EntityFrameworkCore.Scaffolding.Handlebars](https://github.com/TrackableEntities/EntityFrameworkCore.Scaffolding.Handlebars)
- Blog: [Tony Sneed's Blog](https://blog.tonysneed.com/2018/05/27/customize-ef-core-scaffolding-with-handlebars-templates/)
- GitHub: [ErikEJ/EFCorePowerTools](https://github.com/ErikEJ/EFCorePowerTools/wiki)
- GitHub: [TrackableEntities/ef-core-community-handlebars](https://github.com/TrackableEntities/ef-core-community-handlebars)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="blazor"></a>

### Jun 24: [EF Core in Blazor](https://www.youtube.com/live/HNJYIqeBLQc?si=xvEObLRdY37_0L87)

Join members from the .NET teams for our community standup covering great community contributions for Framework, .NET Core, Languages, CLI, MSBuild, and more.

Featuring:

- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)
- [Brice Lambson](https://github.com/bricelam) (Host)

Links:

- Blog: [EF Core In depth – what happens when EF Core writes to the database?](https://www.thereformedprogrammer.net/ef-core-in-depth-what-happens-when-ef-core-writes-to-the-database/)
- Blog: [Build a Blazor WebAssembly Line of Business App Part 1: Intro and Data Access](https://blog.jeremylikness.com/blog/build-a-blazor-webassembly-line-of-business-app/)
- GitHub: [JeremyLikness/BlazorWasmEFCoreExample](https://github.com/JeremyLikness/BlazorWasmEFCoreExample)
- GitHub: [JeremyLikness/BlazorServerEFCoreExample](https://github.com/JeremyLikness/BlazorServerEFCoreExample)
- GitHub: [JeremyLikness/AzureBlazorCosmosWasm](https://github.com/JeremyLikness/AzureBlazorCosmosWasm)
- Video: [Entity Framework Core In-Depth](/shows/visual-studio-toolbox/entity-framework-core-in-depth-part-1)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="power-tools1"></a>

### Jun 10: [EF Core Power Tools](https://www.youtube.com/live/OWuP_qOYwsk?si=VoSGWW6CfR8-x46P)

Join members from the .NET teams for our community standup covering great community contributions for Framework, .NET Core, Languages, CLI, MSBuild, and more.

Featuring:

- [Erik Ejlskov Jensen](https://github.com/ErikEJ) (Special guest)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- Blog: [Azure AD Secured Serverless Cosmos DB from Blazor WebAssembly](https://blog.jeremylikness.com/blog/azure-ad-secured-serverless-cosmosdb-from-blazor-webassembly/)
- Blog: [HANDLING CONCURRENCY - AGGREGATE PATTERN AND EF CORE](https://www.kamilgrzybek.com/blog/posts/handling-concurrency-aggregate-pattern-ef-core)
- Blog: [Announcing Entity Framework Core 5.0 Preview 4](https://devblogs.microsoft.com/dotnet/announcing-entity-framework-core-5-0-preview-4/)
- Visual Studio Marketplace: [EF Core Power Tools](https://marketplace.visualstudio.com/items?itemName=ErikEJ.EFCorePowerTools)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)

<a name="one"></a>

### May 6: [Introducing the EF Core Community Standup](https://www.youtube.com/live/j1sGgfCxhp0?si=VQBMex-9-me9JhAB)

Join members from the .NET teams for our community standup covering great community contributions for Framework, .NET Core, Languages, CLI, MSBuild, and more.

Featuring:

- [Jeremy Likness](https://github.com/JeremyLikness) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)
- [Andriy Svyryd](https://github.com/AndriySvyryd) (Host)
- [lajones](https://github.com/lajones) (Host)
- [Josh Lane](https://github.com/jplane) (Special guest)

Links:

- Blog: [Entity Framework Core 3.0: A Foundation for the Future](https://www.codemag.com/article/1911062)
- Blog: [Event-driven integration #2 - Inferring events from EF Core changes](https://blog.codingmilitia.com/2020/04/20/aspnet-041-from-zero-to-overkill-event-driven-integration-inferring-events-from-efcore-changes/)
- Blog: [Logging in EF Core](https://www.tektutorialshub.com/entity-framework-core/logging-in-ef-core/)
- Blog: [How to Query SQL Server XML data type Columns in Entity Framework Core 3.x](https://www.robkennedy.com/2020/04/30/how-query-sql-server-xml-data-type-columns-in-entity-framework-core-3-x/)
- Blog: [How to pass a dynamic/variable list of values as SqlParameters with FromSqlRaw in EF Core](https://erikej.github.io/efcore/sqlserver/2020/04/20/use-dynamic-sqlparameters-with-fromsql.html)
- Blog: [Generate Entity Framework Core classes from a SQL Server database project - .dacpac file](https://erikej.github.io/efcore/sqlserver/2020/04/13/generate-efcore-classes-from-a-sql-server-database-project.html)
- Blog: [Entity Framework Core 5 vs SQLBulkCopy](https://www.michalbialecki.com/2020/05/03/entity-framework-core-5-vs-sqlbulkcopy-2/)
- Blog: [Microsoft, UPS and health care companies create app so you can donate masks to hospitals](https://eu.usatoday.com/story/money/2020/04/14/ups-microsoft-effort-lets-you-donate-masks-hospitals-need/2981932001/)
- Blog: [Announcing Entity Framework Core 5.0 Preview 3](https://devblogs.microsoft.com/dotnet/announcing-entity-framework-core-5-0-preview-3/)
- Docs: [EF Core daily builds](https://aka.ms/ef-daily-builds)
