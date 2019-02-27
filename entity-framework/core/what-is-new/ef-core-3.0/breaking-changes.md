---
title: Breaking changes in EF Core 3.0 - EF Core
author: divega
ms.date: 02/19/2019
ms.assetid: EE2878C9-71F9-4FA5-9BC4-60517C7C9830
uid: core/what-is-new/ef-core-3.0/breaking-changes
---

# Breaking changes included in EF Core 3.0 (in preview)

> [!IMPORTANT]
> Please note that the feature sets and schedules of future releases are always subject to change, and although we will try to keep this page up to date, it may not reflect our latest plans at all times.

## SQLite

* Microsoft.EntityFrameworkCore.Sqlite now depends on SQLitePCLRaw.bundle_e_sqlite3 instead of SQLitePCLRaw.bundle_green. This makes the version of SQLite used on iOS consistent with other platforms.
* Removed SqliteDbContextOptionsBuilder.SuppressForeignKeyEnforcement(). EF Core no longer sends `PRAGMA foreign_keys = 1` when a connection is opened. Foreign keys are enabled by default in SQLitePCLRaw.bundle_e_sqlite3. If you're not using that, you can enable foreign keys by specifying `Foreign Keys=True` in your connection string.
