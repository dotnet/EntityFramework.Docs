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

### EF Core 2.2

This release will include many bug fixes, and relatively small number of new features. This release is planned for end of year 2018. Details about this release are included in [What's new in EF Core 2.2](xref:core/what-is-new/ef-core-2.2). 

### EF Core 3.0

We plan to have major EF Core release aligned with .NET Core 3.0 and ASP.NET 3.0, but we haven't completed the [release planning process](#release-planning-process) for it.

Use [this query in our issue tracker](https://github.com/aspnet/EntityFrameworkCore/issues?q=is%3Aopen+is%3Aissue+milestone%3A3.0.0+sort%3Areactions-%2B1-desc) to see work items tentatively assigned to 3.0.

## Schedule

The schedule for EF Core is in-sync with the [.NET Core schedule](https://github.com/dotnet/core/blob/master/roadmap.md) and [ASP.NET Core schedule](https://github.com/aspnet/Home/wiki/Roadmap).

## Backlog

We use the [Backlog Milestone](https://github.com/aspnet/EntityFrameworkCore/issues?q=is%3Aopen+is%3Aissue+milestone%3ABacklog+sort%3Areactions-%2B1-desc) in our issue tracker to maintain a list of issues that we expect to work on someday, or that we think someone from the community could tackle.
Customers are welcome to submit comments and votes on these issues.
Anyone looking to work on any of these issues is encouraged to start a discussion on how to approach it.  

If we intend to resolve an issue in a specific timeframe, we'll assign it to a specific release milestone rather than the backlog milestone.
We routinely move issues from the backlog to specific release milestones as part of our [planning](#release-planning-process).

We'll likely close an issue if we don't plan to ever address it.
But any issue that we close can be reconsidered at a later point if we get new information about it.

All that said, there is never a guarantee that we will add any given feature in a specific version of EF Core. 
As in all software projects, priorities, release schedules, and available resources can change at any point.

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

4. **Is the feature an extensibility point?** We tend to favor extensibility points over normal features because they enable developers to hook behaviors optimized for their scenarios and compensate for any missing functionality. 

5. **What is the synergy of the feature when used in combination with other products?** We favor features that enable or significantly improve the experience of using EF Core with other products, such as .NET Core, the latest version of Visual Studio, Microsoft Azure, etc.

6. **What are the skills of the people available to work on a feature, and how to best leverage these resources?** Each member of the EF team and our community contributors have different levels of experience in different areas, so we have to plan accordingly. Even if we wanted to have "all hands on deck" to work on a specific feature like GroupBy translations, or many-to-many, that wouldn't be practical.

As mentioned before, the process evolves on every release, and in the future we'll try to add more opportunities for members of the community to provide inputs into release plans.
For example, we would like to make it easier to review draft designs of the features and of the release plan itself.