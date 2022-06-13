---
title: Breaking changes in EF Core 7.0 - EF Core
description: Complete list of breaking changes introduced in Entity Framework Core 7.0
author: ajcvickers
ms.date: 12/15/2021
uid: core/what-is-new/ef-core-7.0/breaking-changes
---

# Breaking changes in EF Core 7.0

API and behavior changes have the potential to break existing applications updating to EF Core 7.0.0 will be documented here.

## Summary

| **Breaking change**                                                                                          | **Impact**  |
|:------------------------------------------------------------------------------------------------------------ | ----------- |
| [SQL Server tables with triggers now require special EF Core configuration](#sqlserver-tables-with-triggers) | High        |

## High-impact changes

<a name="sqlserver-tables-with-triggers"></a>

### SQL Server tables with triggers now require special EF Core configuration

[Tracking Issue #27372](https://github.com/dotnet/efcore/issues/27372)

#### Old behavior

Previous versions of the SQL Server saved changes via a less efficient technique which always worked.

#### New behavior

By default, EF Core now saves changes via a significantly more efficient technique; unfortunately, this technique is not supported on SQL Server if the target table has database triggers.

#### Why

The performance improvements linked to the new method are significant enough that it's important to bring them to users by default. At the same time, we estimate usage of database triggers in EF Core applications to be low enough that the negative breaking change consequences are outweighed by the performance gain.

#### Mitigations

You can let EF Core know that the target table has a trigger; doing so will revert to the previous, less efficient technique. This can be done by configuring the corresponding entity type as follows:

[!code-csharp[Main](../../../../samples/core/SqlServer/Misc/TriggersContext.cs?name=TriggerConfiguration&highlight=4)]

Note that doing this doesn't actually make EF Core create or manage the trigger in any way - it currently only informs EF Core that triggers are present on the table. As a result, any trigger name can be used.
