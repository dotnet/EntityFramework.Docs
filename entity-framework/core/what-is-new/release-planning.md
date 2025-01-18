---
title: EF Core release planning
description: Information on how Entity Framework Core planning and releasing is done
author: SamMonoRT
ms.date: 01/28/2020
uid: core/what-is-new/release-planning
---

# Release planning process

We often get questions about how we choose specific features to go into a particular release.
This document outlines the process we use.
The process is continually evolving as we find better ways to plan, but the general ideas remain the same.

## Different kinds of releases

Different kinds of release contain different kinds of changes.
This in turn means that the release planning is different for different kinds of release.

### Patch releases

Patch releases change only the "patch" part of the version.
For example, EF Core 3.1.**1** is a release that patches issues found in EF Core 3.1.**0**.

Patch releases are intended to fix critical customer bugs.
This means patch releases do not include new features.
API changes are not allowed in patch releases except in special circumstances.

The bar to make a change in a patch release is very high.
This is because it is critical that patch releases do not introduce new bugs.
Therefore, the decision process emphasizes high value and low risk.

We are more likely to patch an issue if:

* It is impacting multiple customers
* It is a regression from a previous release
* The failure causes data corruption

We are less likely to patch an issue if:

* There are reasonable workarounds
* The fix has high risk of breaking something else
* The bug is in a corner-case

This bar gradually rises through the lifetime of a [long-term support (LTS)](https://dotnet.microsoft.com/platform/support/policy/dotnet-core) release. This is because LTS releases emphasize stability.

The final decision about whether or not to patch an issue is made by the .NET Directors at Microsoft.

### Major releases

Major releases change the EF "major" version number.
For example, EF Core **3**.0.0 is a major release that makes a big step forward over EF Core 2.2.x.

Major releases:

* Are intended to improve the quality and features of the previous release
* Typically contain bug fixes and new features
  * Some of the new features may be fundamental changes to the way EF Core works
* Typically include intentional breaking changes
  * Breaking changes are necessary part of evolving EF Core as we learn
  * However, we think very carefully about making any breaking change because of the potential customer impact. We may have been too aggressive with breaking changes in the past. Going forward, we will strive to minimize changes that break applications, and to reduce changes that break database providers and extensions.
* Have many prerelease previews pushed to NuGet

## Planning for major/minor releases

### GitHub issue tracking

GitHub ([https://github.com/dotnet/efcore](https://github.com/dotnet/efcore)) is the source-of-truth for all EF Core planning.

Issues on GitHub have:

* A state
  * [Open](https://github.com/dotnet/efcore/issues) issues have not been addressed.
  * [Closed](https://github.com/dotnet/efcore/issues?q=is%3Aissue+is%3Aclosed) issues have been addressed.
    * All issues that have been fixed are [tagged with closed-fixed](https://github.com/dotnet/efcore/issues?q=is%3Aissue+label%3Aclosed-fixed+is%3Aclosed). An issue tagged with closed-fixed is fixed and merged, but may not have been released.
    * Other `closed-` labels indicate other reasons for closing an issue. For example, duplicates are tagged with closed-duplicate.
* A type
  * [Bugs](https://github.com/dotnet/efcore/issues?q=is%3Aissue+is%3Aopen+label%3Atype-bug) represent bugs.
  * [Enhancements](https://github.com/dotnet/efcore/issues?q=is%3Aissue+is%3Aopen+label%3Atype-enhancement) represent new features or better functionality in existing features.
* A milestone
  * [Issues with no milestone](https://github.com/dotnet/efcore/issues?q=is%3Aopen+is%3Aissue+no%3Amilestone) are being considered by the team. The decision on what to do with the issue has not yet been made or a change in the decision is being considered.
  * [Issues in the Backlog milestone](https://github.com/dotnet/efcore/issues?q=is%3Aopen+is%3Aissue+milestone%3ABacklog) are items that the EF team will consider working on in a future release
    * Issues in the backlog may be [tagged with consider-for-next-release](https://github.com/dotnet/efcore/issues?q=is%3Aissue+is%3Aopen+label%3Aconsider-for-next-release) indicating that this work item is high on the list for the next release.
  * Open issues in a versioned milestone are items that the team plans to work on in that version. For example, [these are the issues we plan to work on for EF Core 5.0](https://github.com/dotnet/efcore/issues?q=is%3Aopen+is%3Aissue+milestone%3A5.0.0).
  * Closed issues in a versioned milestone are issues that are completed for that version. Note that the version may not yet have been released. For example, [these are the issues completed for EF Core 3.0](https://github.com/dotnet/efcore/issues?q=is%3Aissue+milestone%3A3.0.0+is%3Aclosed).
* Votes!
  * Voting is the best way to indicate that an issue is important to you.
  * To vote, just add a "thumbs-up" 👍 to the issue. For example, [these are the top-voted issues](https://github.com/dotnet/efcore/issues?q=is%3Aissue+is%3Aopen+sort%3Areactions-%2B1-desc)
  * Please also comment describing specific reasons you need the feature if you feel this adds value. Commenting "+1" or similar does not add value.

### The planning process

The planning process is more involved than just taking the top-most requested features from the backlog.
This is because we gather feedback from multiple stakeholders in multiple ways.
We then shape a release based on:

* Input from customers
* Input from other stakeholders
* Strategic direction
* Resources available
* Schedule

Some of the questions we ask are:

1. **How many developers we think will use the feature and how much better will it make their applications or experience?** To answer this question, we collect feedback from many sources — Comments and votes on issues is one of those sources. Specific engagements with important customers is another.

2. **What are the workarounds people can use if we don't implement this feature yet?** For example, many developers can map a join table to work around lack of native many-to-many support. Obviously, not all developers want to do it, but many can, and that counts as a factor in our decision.

3. **Does implementing this feature evolve the architecture of EF Core such that it moves us closer to implementing other features?** We tend to favor features that act as building blocks for other features. For instance, property bag entities can help us move towards many-to-many support, and entity constructors enabled our lazy loading support.

4. **Is the feature an extensibility point?** We tend to favor extensibility points over normal features because they enable developers to hook their own behaviors and compensate for any missing functionality.

5. **What is the synergy of the feature when used in combination with other products?** We favor features that enable or significantly improve the experience of using EF Core with other products, such as .NET Core, the latest version of Visual Studio, Microsoft Azure, and so on.

6. **What are the skills of the people available to work on a feature, and how can we best leverage these resources?** Each member of the EF team and our community contributors has different levels of experience in different areas, so we have to plan accordingly. Even if we wanted to have "all hands on deck" to work on a specific feature like GroupBy translations, or many-to-many, that wouldn't be practical.
