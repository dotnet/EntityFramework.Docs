---
title: Entity Framework Core Roadmap
author: divega
ms.date: 02/20/2018
ms.assetid: 834C9729-7F6E-4355-917D-DE3EE9FE149E
uid: core/what-is-new/roadmap
---

# Entity Framework Core Roadmap

> [!IMPORTANT]
> Please note that the feature sets and schedules of future releases are always subject to change, and although we will try to keep this page up to date, it may not reflect our latest plans at all times.

### EF Core 3.0

With EF Core 2.2 out the door, our main focus is now EF Core 3.0, which will be aligned with the .NET Core 3.0 and ASP.NET 3.0 releases.

We haven't completed any new features yet, so the [EF Core 3.0 Preview 1 packages published to the NuGet Gallery](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/3.0.0-preview.18572.1) on December 2018 only contain [bug fixes, minor improvements, and changes we made in preparation for the 3.0 work](https://github.com/aspnet/EntityFrameworkCore/issues?q=is%3Aissue+milestone%3A3.0.0+is%3Aclosed+label%3Aclosed-fixed).

In fact, we still need to refine our [release planning](#release-planning-process) for 3.0, to make sure we have the right set of features that can be completed in the allotted time.
We will share more information as we get more clarity, but here are some high-level themes and features we itend to work on:

- **LINQ improvements ([#12795](https://github.com/aspnet/EntityFrameworkCore/issues/12795))**: LINQ enables you to write database queries without leaving your language of choice, taking advantage of rich type information to get IntelliSense and compile-time type checking.
  But LINQ also enables you to write an unlimited number of complicated queries, and that has always been a huge challenge for LINQ providers.
  In the first few versions of EF Core, we solved that in part by figuring out what portions of a query could be translated to SQL, and then by allowing the rest of the query to execute in memory on the client.
  This client-side execution can be desirable in some situations, but in many other cases it can result in inefficient queries that may not be identified until an application is deployed to production.
  In EF Core 3.0, we are planning to make profound changes to how our LINQ implementation works, and how we test it.
  The goals are to make it more robust (for example, to avoid breaking queries in patch releases), to be able to translate more expressions correctly into SQL, to generate efficient queries in more cases, and to prevent inefficient queries from going undetected.

- **Cosmos DB support ([#8443](https://github.com/aspnet/EntityFrameworkCore/issues/8443))**: We're working on a Cosmos DB provider for EF Core, to enable developers familiar with the EF programing model to easily target Azure Cosmos DB as an application database.
  The goal is to make some of the advantages of Cosmos DB, like global distribution, “always on” availability, elastic scalability, and low latency, even more accessible to .NET developers.
  The provider will enable most EF Core features, like automatic change tracking, LINQ, and value conversions, against the SQL API in Cosmos DB. We started this effort before EF Core 2.2, and [we have made some preview versions of the provider available](https://blogs.msdn.microsoft.com/dotnet/2018/10/17/announcing-entity-framework-core-2-2-preview-3/).
  The new plan is to continue developing the provider alongside EF Core 3.0.   

- **C# 8.0 support ([#12047](https://github.com/aspnet/EntityFrameworkCore/issues/12047))**: We want our customers to take advantage of some of the [new features coming in C# 8.0](https://blogs.msdn.microsoft.com/dotnet/2018/11/12/building-c-8-0/) like async streams (including await for each) and nullable reference types while using EF Core.

- **Reverse engineering database views into query types ([#1679](https://github.com/aspnet/EntityFrameworkCore/issues/1679))**: In EF Core 2.1, we added support for query types, which can represent data that can be read from the database, but cannot be updated.
  Query types are a great fit for mapping database views, so in EF Core 3.0, we would like to automate the creation of query types for database views.

- **Property bag entities ([#13610](https://github.com/aspnet/EntityFrameworkCore/issues/13610) and [#9914](https://github.com/aspnet/EntityFrameworkCore/issues/9914))**: This feature is about enabling entities that store data in indexed properties instead of regular properties, and also about being able to use instances of the same .NET class (potentially something as simple as a `Dictionary<string, object>`) to represent different entity types in the same EF Core model.
  This feature is a stepping stone to support many-to-many relationships without a join entity, which is one of the most requested improvements for EF Core.

- **EF 6.3 on .NET Core ([EF6 #271](https://github.com/aspnet/EntityFramework6/issues/271))**: We understand that many existing applications use previous versions of EF, and that porting them to EF Core only to take advantage of .NET Core can sometimes require a significant effort.
  For that reason, we will be adapting the next version of EF 6 to run on .NET Core 3.0.
  We are doing this to facilitate porting existing applications with minimal changes.
  There are going to be some limitations (for example, it will require new providers, spatial support with SQL Server won't be enabled), and there are no new features planned for EF 6.

In the meantime, you can use [this query in our issue tracker](https://github.com/aspnet/EntityFrameworkCore/issues?q=is%3Aopen+is%3Aissue+milestone%3A3.0.0+sort%3Areactions-%2B1-desc) to see work items tentatively assigned to 3.0.

## Schedule

The schedule for EF Core is in-sync with the [.NET Core schedule](https://github.com/dotnet/core/blob/master/roadmap.md) and [ASP.NET Core schedule](https://github.com/aspnet/Home/wiki/Roadmap).

## Backlog

The [Backlog Milestone](https://github.com/aspnet/EntityFrameworkCore/issues?q=is%3Aopen+is%3Aissue+milestone%3ABacklog+sort%3Areactions-%2B1-desc) in our issue tracker contains issues that we expect to work on someday, or that we think someone from the community could tackle.
Customers are welcome to submit comments and votes on these issues.
Contributors looking to work on any of these issues are encouraged to first start a discussion on how to approach them.

There is never a guarantee that we will work on any given feature in a specific version of EF Core.
As in all software projects, priorities, release schedules, and available resources can change at any point.
But if we intend to resolve an issue in a specific timeframe, we'll assign it to a release milestone instead of the backlog milestone.
We routinely move issues between the backlog and release milestones as part of our [release planning process](#release-planning-process).

We'll likely close an issue if we don't plan to ever address it.
But we can reconsider an issue that we previously closed if we get new information about it.

## Release planning process

We often get questions about how we choose specific features to go into a particular release.
Our backlog certainly doesn't automatically translate into release plans.
The presence of a feature in EF6 also doesn't automatically mean that the feature needs to be implemented in EF Core.

It's difficult to detail the whole process we follow to plan a release.
Much of it is discussing the specific features, opportunities and priorities, and the process itself also evolves with every release.
However, we can summarize the common questions we try to answer when deciding what to work on next:

1. **How many developers we think will use the feature and how much better will it make their applications/experience?** To answer this, we collect feedback from many sources — Comments and votes on issues is one of those sources.

2. **What are the workarounds people can use if we don't implement this feature yet?** For example, many developers can map a join table to work around lack of native many-to-many support. Obviously, not all developers want to do it, but many can, and that counts as a factor in our decision.

3. **Does implementing this feature evolve the architecture of EF Core such that it moves us closer to implementing other features?** We tend to favor features that act as building blocks for other features. For example, property bag entities can help us move towards many-to-many support, and entity constructors enabled our lazy loading support. 

4. **Is the feature an extensibility point?** We tend to favor extensibility points over normal features because they enable developers to hook their own behaviors and compensate for any missing functionality. 

5. **What is the synergy of the feature when used in combination with other products?** We favor features that enable or significantly improve the experience of using EF Core with other products, such as .NET Core, the latest version of Visual Studio, Microsoft Azure, etc.

6. **What are the skills of the people available to work on a feature, and how to best leverage these resources?** Each member of the EF team and our community contributors has different levels of experience in different areas, so we have to plan accordingly. Even if we wanted to have "all hands on deck" to work on a specific feature like GroupBy translations, or many-to-many, that wouldn't be practical.

As mentioned before, the process evolves on every release.
In the future we'll try to add more opportunities for members of the community to provide inputs into our release plans.
For example, we would like to make it easier to review draft designs of the features and of the release plan itself.
