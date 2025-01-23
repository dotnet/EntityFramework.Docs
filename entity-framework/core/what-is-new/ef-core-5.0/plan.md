---
title: Plan for Entity Framework Core 5.0
description: The features planned for Entity Framework Core 5.0
author: SamMonoRT
ms.date: 08/22/2020
uid: core/what-is-new/ef-core-5.0/plan
---

# Plan for Entity Framework Core 5.0

> [!IMPORTANT]
> EF Core 5.0 [has now been released](xref:core/what-is-new/index). This page remains as a historical record of the plan.

As described in the [planning process](xref:core/what-is-new/release-planning), we have gathered input from stakeholders into a tentative plan for the EF Core 5.0 release.

> [!IMPORTANT]
> This plan is still a work-in-progress. Nothing here is a commitment. This plan is a starting point that will evolve as we learn more. Some things not currently planned for 5.0 may get pulled in. Some things currently planned for 5.0 may get punted out.

## General information

### Version number and release date

EF Core 5.0 is currently scheduled for release at [the same time as .NET 5.0](https://devblogs.microsoft.com/dotnet/introducing-net-5/). The version "5.0" was chosen to align with .NET 5.0.

### Supported platforms

EF Core 5.0 is planned to run on any .NET Standard 2.1 platform, including .NET 5.0. This is part of the more general .NET wide [convergence of platforms to .NET Core](https://devblogs.microsoft.com/dotnet/introducing-net-5/).

EF Core 5.0 will not run on .NET Framework.

### Breaking changes

EF Core 5.0 will contain some [breaking changes](xref:core/what-is-new/ef-core-5.0/breaking-changes), but these will be much less severe than was the case for EF Core 3.0. Our goal is to allow the vast majority of applications to update without breaking.

It is expected that there will be some breaking changes for database providers, especially around TPT support. However, we expect the work to update a provider for 5.0 will be less than was required to update for 3.0.

## Themes

We have extracted a few major areas or themes which will form the basis for the large investments in EF Core 5.0.

## Fully transparent many-to-many mapping by convention

Lead developers: @smitpatel, @AndriySvyryd, and @lajones

Tracked by [#10508](https://github.com/dotnet/efcore/issues/10508)

T-shirt size: L

Status: Done

Many-to-many is the [most requested feature](https://github.com/dotnet/efcore/issues/1368) (~506 votes) on the GitHub backlog.

Support for many-to-many relationships can be broken down into three major areas:

* Skip navigation properties--covered by the next theme.
* Property-bag entity types. These allow a standard CLR type (e.g. `Dictionary`) to be used for entity instances such that an explicit CLR type is not needed for each entity type. Tracked by [#9914](https://github.com/dotnet/efcore/issues/9914).
* Sugar for easy configuration of many-to-many relationships.

In addition to the skip navigation support, we are now pulling these other areas of many-to-many into EF Core 5.0 so as to provide a complete experience.

## Many-to-many navigation properties (a.k.a "skip navigations")

Lead developers: @smitpatel and @AndriySvyryd

Tracked by [#19003](https://github.com/dotnet/efcore/issues/19003)

T-shirt size: L

Status: Done

As described in the first theme, many-to-many support has multiple aspects.
This theme specifically tracks use of skip navigations.
We believe that the most significant blocker for those wanting many-to-many support is not being able to use the "natural" relationships, without referring to the join table, in business logic such as queries.
The join table entity type may still exist, but it should not get in the way of business logic.

## Table-per-type (TPT) inheritance mapping

Lead developer: @AndriySvyryd and @smitpatel

Tracked by [#2266](https://github.com/dotnet/efcore/issues/2266)

T-shirt size: XL

Status: Done

We're doing TPT because it is both a highly requested feature (~289 votes; 3rd overall) and because it requires some low-level changes that we feel are appropriate for the foundational nature of the overall .NET 5 plan. We expect this to result in breaking changes for database providers, although these should be much less severe than the changes required for 3.0.

## Filtered Include

Lead developer: @maumar

Tracked by [#1833](https://github.com/dotnet/efcore/issues/1833)

T-shirt size: M

Status: Done

Filtered Include is a highly-requested feature (~376 votes; 2nd overall) that isn't a huge amount of work, and that we believe will unblock or make easier many scenarios that currently require model-level filters or more complex queries.

## Split Include

Lead developer: @smitpatel

Tracked by [#20892](https://github.com/dotnet/efcore/issues/20892)

T-shirt size: L

Status: Done

EF Core 3.0 changed the default behavior to create a single SQL query for a given LINQ query.
This caused large performance regressions for queries that use Include for multiple collections.

In EF Core 5.0, we are retaining the new default behavior.
However, EF Core 5.0 will now allow generation of multiple queries for collection Includes where having a single query is causing bad performance.

## Required one-to-one dependents

Lead developers: @AndriySvyryd and @smitpatel

Tracked by [#12100](https://github.com/dotnet/efcore/issues/12100)

T-shirt size: M

Status: Done

In EF Core 3.0, all dependents, including owned types are optional (e.g. Person.Address can be null). In EF Core 5.0, dependents can be configured as required.

## Rationalize ToTable, ToQuery, ToView, FromSql, etc

Lead developers: @AndriySvyryd and @smitpatel

Tracked by [#17270](https://github.com/dotnet/efcore/issues/17270)

T-shirt size: L

Status: Done

We have made progress in previous releases towards supporting raw SQL, keyless types, and related areas. However, there are both gaps and inconsistencies in the way everything works together as a whole. The goal for 5.0 is to fix these and create a good experience for defining, migrating, and using different types of entities and their associated queries and database artifacts. This may also involve updates to the compiled query API.

Note that this item may result in some application-level breaking changes since some of the functionality we currently have is too permissive such that it can quickly lead people into pits of failure. We will likely end up blocking some of this functionality together with guidance on what to do instead.

## General query enhancements

Lead developers: @smitpatel and @maumar

Tracked by [issues labeled with `area-query` in the 5.0 milestone](https://github.com/dotnet/efcore/issues?utf8=%E2%9C%93&q=is%3Aissue+label%3Aarea-query+milestone%3A5.0.0+)

T-shirt size: XL

Status: Done

The query translation code was extensively rewritten for EF Core 3.0. The query code is generally in a much more robust state because of this. For 5.0 we aren't planning on making major query changes, outside those needed to support TPT and skip navigation properties. However, there is still significant work needed to fix some technical debt left over from the 3.0 overhaul. We also plan to fix many bugs and implement small enhancements to further improve the overall query experience.

## Migrations and deployment experience

Lead developers: @bricelam

Tracked by [#19587](https://github.com/dotnet/efcore/issues/19587)

T-shirt size: L

Status: Scoped/Done

Scoping: The [migrations bundles feature](https://github.com/dotnet/efcore/issues/19693) has been deferred until after the EF Core 5.0 release. However, several other [targeted improvements related to migrations](https://github.com/dotnet/efcore/issues/19587#issuecomment-668794460) will be included in EF Core 5.0

Currently, many developers migrate their databases at application startup time. This is easy but is not recommended because:

* Multiple threads/processes/servers may attempt to migrate the database concurrently
* Applications may try to access inconsistent state while this is happening
* Usually the database permissions to modify the schema should not be granted for application execution
* It's hard to revert back to a clean state if something goes wrong

We want to deliver a better experience here that allows an easy way to migrate the database at deployment time. This should:

* Work on Linux, Mac, and Windows
* Be a good experience on the command line
* Support scenarios with containers
* Work with commonly used real-world deployment tools/flows
* Integrate into at least Visual Studio

The result is likely to be many small improvements in EF Core (for example, better Migrations on SQLite), together with guidance and longer-term collaborations with other teams to improve end-to-end experiences that go beyond just EF.

## EF Core platforms experience

Lead developers: @roji and @bricelam

Tracked by [#19588](https://github.com/dotnet/efcore/issues/19588)

T-shirt size: L

Status: Scope/Done

Scoping: Platform guidance and samples are published for Blazor, Xamarin, WinForms, and WPF. Xamarin and other AOT/linker work is now planned for EF Core 6.0.

> [!IMPORTANT]
> Xamarin.Android, Xamarin.iOS, Xamarin.Mac are now integrated directly into .NET (starting with .NET 6) as .NET for Android, .NET for iOS, and .NET for macOS. If you're building with these project types today, they should be upgraded to .NET SDK-style projects for continued support. For more information about upgrading Xamarin projects to .NET, see the [Upgrade from Xamarin to .NET & .NET MAUI](/dotnet/maui/migration) documentation.

We have good guidance for using EF Core in traditional MVC-like web applications. Guidance for other platforms and application models is either missing or out-of-date. For EF Core 5.0, we plan to investigate, improve, and document the experience of using EF Core with:

* Blazor
* Xamarin, including using the AOT/linker story
* WinForms/WPF/WinUI and possibly other U.I. frameworks

This is likely to be many small improvements in EF Core, together with guidance and longer-term collaborations with other teams to improve end-to-end experiences that go beyond just EF.

Specific areas we plan to look at are:

* Deployment, including the experience for using EF tooling such as for Migrations
* Application models, including Xamarin and Blazor, and probably others
* SQLite experiences, including the spatial experience and table rebuilds
* AOT and linking experiences
* Diagnostics integration, including perf counters

## Performance

Lead developer: @roji

Tracked by [issues labeled with `area-perf` in the 5.0 milestone](https://github.com/dotnet/efcore/issues?utf8=%E2%9C%93&q=is%3Aissue+label%3Aarea-perf+milestone%3A5.0.0+)

T-shirt size: L

Status: Scoped/Done

Scoping: Major performance improvements in the Npgsql provider are complete. Other performance work is now planned for EF Core 6.0.

For EF Core, we plan to improve our suite of performance benchmarks and make directed performance improvements to the runtime. In addition, we plan to complete the new ADO.NET batching API which was prototyped during the 3.0 release cycle. Also at the ADO.NET layer, we plan additional performance improvements to the Npgsql provider.

As part of this work we also plan to add ADO.NET/EF Core performance counters and other diagnostics as appropriate.

## Architectural/contributor documentation

Lead documenter: @ajcvickers

Tracked by [#1920](https://github.com/dotnet/EntityFramework.Docs/issues/1920)

T-shirt size: L

Status: Cut

The idea here is to make it easier to understand what is going on in the internals of EF Core. This can be useful to anyone using EF Core, but the primary motivation is to make it easier for external people to:

* Contribute to the EF Core code
* Create database providers
* Build other extensions

Update: Unfortunately, this plan was too ambitious.
We still believe this is important, but unfortunately it won't land with EF Core 5.0.

## Microsoft.Data.Sqlite documentation

Lead documenter: @bricelam

Tracked by [#1675](https://github.com/dotnet/EntityFramework.Docs/issues/1675)

T-shirt size: M

Status: Completed. The new documentation is [live on Microsoft Learn](/dotnet/standard/data/sqlite/?tabs=netcore-cli).

The EF Team also owns the Microsoft.Data.Sqlite ADO.NET provider. We plan to fully document this provider as part of the 5.0 release.

## General documentation

Lead documenter: @ajcvickers

Tracked by [issues in the documentation repo in the 5.0 milestone](https://github.com/dotnet/EntityFramework.Docs/issues?utf8=%E2%9C%93&q=is%3Aissue+milestone%3A5.0.0+)

T-shirt size: L

Status: In-progress

We are already in the process of updating documentation for the 3.0 and 3.1 releases. We are also working on:

* An overhaul of the getting started articles to make them more approachable/easier to follow
* Reorganization of articles to make things easier to find and to add cross-references
* Adding more details and clarifications to existing articles
* Updating the samples and adding more examples

## Fixing bugs

Tracked by [issues labeled with `type-bug` in the 5.0 milestone](https://github.com/dotnet/efcore/issues?utf8=%E2%9C%93&q=is%3Aissue+milestone%3A5.0.0+label%3Atype-bug+)

Developers: @roji, @maumar, @bricelam, @smitpatel, @AndriySvyryd, @ajcvickers

T-shirt size: L

Status: In-progress

At the time of writing, we have 135 bugs triaged to be fixed in the 5.0 release (with 62 already fixed), but there is significant overlap with the _General query enhancements_ section above.

The incoming rate (issues that end up as work in a milestone) was about 23 issues per month over the course of the 3.0 release. Not all of these will need to be fixed in 5.0. As a rough estimate we plan to fix an additional 150 issues in the 5.0 time frame.

## Small enhancements

Tracked by [issues labeled with `type-enhancement` in the 5.0 milestone](https://github.com/dotnet/efcore/issues?utf8=%E2%9C%93&q=is%3Aissue+milestone%3A5.0.0+label%3Atype-enhancement+)

Developers: @roji, @maumar, @bricelam, @smitpatel, @AndriySvyryd, @ajcvickers

T-shirt size: L

Status: Done

In addition to the bigger features outlined above, we also have many smaller improvements scheduled for 5.0 to fix "paper-cuts". Note that many of these enhancements are also covered by the more general themes outlined above.

## Below-the-line

Tracked by [issues labeled with `consider-for-next-release`](https://github.com/dotnet/efcore/issues?q=is%3Aopen+is%3Aissue+label%3Aconsider-for-next-release)

These are bug fixes and enhancements that are **not** currently scheduled for the 5.0 release, but we will look at as stretch goals depending on the progress made on the work above.

In addition, we always consider the [most voted issues](https://github.com/dotnet/efcore/issues?q=is%3Aissue+is%3Aopen+sort%3Areactions-%2B1-desc) when planning. Cutting any of these issues from a release is always painful, but we do need a realistic plan for the resources we have.

## Suggestions

Your feedback on planning is important. The best way to indicate the importance of an issue is to vote (thumbs-up) for that issue on GitHub. This data will then feed into the [planning process](xref:core/what-is-new/release-planning) for the next release.
