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

## 2024

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

### February 7: [Database concurrency and EF Core: Beyond optimistic concurrency](https://www.youtube.com/live/0eVTR5up2RY?si=pttR5NJ85_NWozhj)

In the 3rd installment of our series on concurrency, we'll dive deep into SQL isolation levels, how they work (and work differently!) across SQL Server and PostgreSQL. We'll hopefully learn a bit about how database locks, snapshots and isolation levels work together to bring some sanity into the world of concurrency.

Featuring:

- [Shay Rojansky](https://github.com/roji) (Host)
- [Arthur Vickers](https://github.com/ajcvickers) (Host)

Links:

- [SQL Server Isolation Levels : A Series](https://sqlperformance.com/2014/07/t-sql-queries/isolation-levels)
- [Transaction isolation in PostgreSQL](https://www.postgresql.org/docs/current/transaction-iso.html)

### January 24: [Database concurrency and EF Core: ASP.NET and Blazor](https://www.youtube.com/live/xVyYrtetDeA?si=Bx2s44mk_ayUnAzx)

In the last episode, we dug deep into the underpinnings of optimistic concurrency handling in EF Core. In this episode, we continue that journey to cover disconnected scenarios. That is, where the entity travels to a client and then back to the server before being updated in the database. We’ll look at the different patterns for doing updates like this in ASP.NET Core and Blazor apps and see how concurrency tokens work with each of these. We’ll also look at how `ExecuteUpdate` can be used with concurrency tokens, and take a look at ETag concurrency in Azure Cosmos DB.

Featuring:

- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- [Handling Concurrency Conflicts](xref:core/saving/concurrency)

### January 10: [Database concurrency and EF Core](https://www.youtube.com/live/YfIM-gfJe4c?si=ZQNxpGEULZt3L43I)

What happens when one user is reading a record while another user is updating it? What if both users try to update the same record at the same time? In this episode of the .NET Data Community Standup, Shay and Arthur take a look at how different database isolation levels impact concurrency and performance, and then extend this to optimistic concurrency patterns supported by Entity Framework Core. This includes both manual concurrency tokens and automatic concurrency tokens like SQL Server’s “rowversion”. We’ll also dig into what happens in disconnected scenarios, and when to use the original value as opposed to the current value of the concurrency token. Finally, we’ll look at how EF Core handles optimistic concurrency with ETags when using a document database like Azure Cosmos DB.

Featuring:

- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- [Handling Concurrency Conflicts](xref:core/saving/concurrency)

## 2023

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
- [Erik Ejlskov Jensen](https://github.com/JeremyLikness) (Special guest)
- [Julie Lerman](https://github.com/julielerman) (Special guest)
- [Diego Vega](https://github.com/divega) (Alumnus)
- [Brice Lambson](https://github.com/bricelam) (Team member)

Links:

- [EF8 release post on the .NET Blog](https://aka.ms/ef8)
- [What's new in EF8, with runnable samples](https://aka.ms/ef8-new)
- [Entity Framework Core 8: Improved JSON, queryable collections, and more… | .NET Conf 2023](https://aka.ms/ef8-dotnetconf)
- [EF Core daily builds](https://aka.ms/ef-daily-builds)

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

### September 20: [Complex types as value objects in EF8](https://www.youtube.com/live/H-soJYqWSds?si=tJzm-uqLL0ZJnsUh)

The Entity Framework team returns with new community standups after a summer break heads-down working on EF8. In this session, we’ll look at the new “complex types” feature in EF8. Complex types do not have a key or any identity outside of the object’s value, but can still be deconstructed by property to map to different columns in the database. This allows either reference or value types to be used as DDD value objects with much better fidelity than when owned types are used. We’ll show examples of using complex types and talk about and answer your questions on the behavioral differences between entity types, owned entity types, complex types, and primitive types.

Featuring:

- [Arthur Vickers](https://github.com/ajcvickers) (Host)
- [Shay Rojansky](https://github.com/roji) (Host)

Links:

- [Unicode and UTF-8 in SQL Server with EF Core](xref:core/providers/sql-server/columns#unicode-and-utf-8)
- [What's new in EF8, with runnable samples](https://aka.ms/ef8-new)
- [EF Core daily builds](https://aka.ms/ef-daily-builds)
