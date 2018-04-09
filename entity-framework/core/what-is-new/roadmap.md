---
title: Entity Framework Core Roadmap
author: divega
ms.author: divega

ms.date: 02/20/2018

ms.assetid: 834C9729-7F6E-4355-917D-DE3EE9FE149E
ms.technology: entity-framework-core

uid: core/what-is-new/roadmap
---

# Entity Framework Core Roadmap

> [!IMPORTANT]
> Please note that the feature sets and schedules of future releases are always subject to change, and although we will try to keep this page up to date, it may not reflect our latest plans at all times.

The second preview of EF Core 2.1 was released on April 2018. You can now find more information about this release in [What is new in EF Core 2.1](xref:core/what-is-new/ef-core-2.1).

We intend to release additional previews of EF Core 2.1 monthly, and a final release on the second calendar quarter of 2018.

We have not completed the [release planning](#release-planning-process) for the next release after 2.1.

## Schedule

The schedule for EF Core is in-sync with the [.NET Core schedule](https://github.com/dotnet/core/blob/master/roadmap.md) and [ASP.NET Core schedule](https://github.com/aspnet/Home/wiki/Roadmap).

## Backlog

We use the [Backlog Milestone](https://github.com/aspnet/EntityFrameworkCore/issues?q=is%3Aopen+is%3Aissue+milestone%3ABacklog+sort%3Areactions-%2B1-desc) in our issue tracker to maintain a detailed list of issues and features. Customers can comment and up-vote these.

We tend to leave issues open that we reasonably expect we will work on at some point, or that someone from the community could tackle, but that does not imply the intent to resolve them in a specific timeframe until we assign them to a specific milestone as part of our [release planning process](#release-planning-process).

If we do not plan to ever implement a feature, we will likely close the issue. An issue that we closed can be reconsidered at a later point if we obtain new information about it.

All that said, we don’t have enough information about the future to be able to say that feature X will be resolved by time/release Y. As in all software projects, priorities, release schedules, and available resources can change at any point.

## Release planning process

We often get questions about how we choose specific features to go into a particular release. Our backlog certainly does not automatically translate into release plans. The presence of a feature in EF6 also does not automatically mean that the feature needs to be implemented in EF Core.

It is difficult to detail here the whole process we follow to plan a release, partly because a lot of it is discussing the specific features, opportunities and priorities, and partly because the process itself usually evolves with every release. However, it is relatively easy to summarize the common questions we try to answer when deciding what to work on next:

1. **How many developers we think will use the feature and how much better will it make their applications/experience?** We aggregate feedback from many sources into this — Comments and votes on issues is one of those sources.

2. **What are the workarounds people can use if we don’t implement this feature yet?** For example, many developers are able to map a join table in order to work around lack of native many-to-many support. Obviously, not all developers can do this, but many can, and this is a factor which counts.

3. **Does implementing this feature evolve the architecture of EF Core such that it moves us closer to implementing other features?** We tend to favor features that act as building blocks for other features—for example, the table splitting that was done for owned types helps us move towards TPT support.

4. **Is the feature an extensibility point?** We tend to favor extensibility points because they enable developers to more easily hook in their own behaviors and get some of the missing functionality that way. We’re planning to do some of this as a start to the lazy loading work.

5. **What is the synergy of the feature when used in combination with other products?** We tend to favor features that allow EF Core to be used with other products or to significantly improve the experience of using other products, such as .NET Core, the latest version of Visual Studio, Microsoft Azure, etc.

6. **What are the capabilities of the people available to work on a feature, and how to best leverage these resources?** Each member of the EF team and even our community contributors have different levels of experience in different areas and we have to plan accordingly. Even if we wanted to have “all hands on deck” to work on a specific feature like GroupBy translations, or many-to-many, that wouldn’t be practical.

As mentioned before, this process evolves on every release, and in the future we would like to add more opportunities for members of the developer community to provide inputs into release plans, e.g. by making it easier to review proposed drafts of the features and of the release plan itself.
