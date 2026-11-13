---
title: Breaking changes in EF Core 12 (EF12) - EF Core
description: List of breaking changes introduced in Entity Framework Core 12 (EF12)
author: ajcvickers
ms.date: 11/08/2026
uid: core/what-is-new/ef-core-12.0/breaking-changes
---

# Breaking changes in EF Core 12 (EF12)

This page documents API and behavior changes that have the potential to break existing applications updating from EF Core 11 to EF Core 12. Make sure to review earlier breaking changes if updating from an earlier version of EF Core:

- [Breaking changes in EF Core 11](xref:core/what-is-new/ef-core-11.0/breaking-changes)
- [Breaking changes in EF Core 10](xref:core/what-is-new/ef-core-10.0/breaking-changes)
- [Breaking changes in EF Core 9](xref:core/what-is-new/ef-core-9.0/breaking-changes)
- [Breaking changes in EF Core 8](xref:core/what-is-new/ef-core-8.0/breaking-changes)

## Summary

| **Breaking change**                                                                                  | **Impact** |
|:---------------------------------------------------------------------------------------------------- | -----------|
| [`migrations add --output-dir` now also places the model snapshot](#migrations-snapshot-output-dir) | Low        |

## Low-impact changes

<a name="migrations-snapshot-output-dir"></a>

### `migrations add --output-dir` now also places the model snapshot

[Tracking Issue #24339](https://github.com/dotnet/efcore/issues/24339)

#### Old behavior

`dotnet ef migrations add --output-dir <dir>` (and `Add-Migration -OutputDir <dir>`) put the migration and its designer file in `<dir>`, but the model snapshot (`<Context>ModelSnapshot.cs`) was written to the directory of the existing snapshot. When there was no existing snapshot, or it couldn't be found, the snapshot was written to a directory derived from its namespace. This could be outside `<dir>`, and for a namespace containing dots it was a folder tree such as `my/project/Db/Migrations`.

For example, after migrations had been added to `Migrations/2025`, `dotnet ef migrations add Next -o Migrations/2026` created the new migration in `Migrations/2026` and left the snapshot in `Migrations/2025`.

#### New behavior

When `--output-dir` is specified, the model snapshot is written to that directory as well, together with the migration and designer files. This applies to relative paths, to paths outside the project directory, to absolute paths, and when `--namespace` is also specified.

If the context already has a snapshot in a different file, the new snapshot is written first and the old file is then deleted, so that only one snapshot remains. The old file is deleted only when exactly one file in the project or the output directory declares the snapshot class. If there are several, or the existing snapshot can't be found, nothing is deleted and a warning is reported. If a file with the snapshot's name already exists in the output directory but declares a different class, such as the snapshot of another `DbContext`, the command fails instead of overwriting it.

With `--dry-run`, the planned move is reported but nothing is changed.

The path of a deleted snapshot is returned in the new `PreviousSnapshotFile` property of <xref:Microsoft.EntityFrameworkCore.Migrations.Design.MigrationFiles>, and in the JSON output of the command.

#### Why

The `--output-dir` option is documented as the directory to put the generated files in, and having the snapshot elsewhere was surprising. It was hard to fix by hand: deleting the output folder to start over left a snapshot behind, and moving the migration files left the project with a snapshot in the wrong place. See [#24339](https://github.com/dotnet/efcore/issues/24339), [#28316](https://github.com/dotnet/efcore/issues/28316) and [#32738](https://github.com/dotnet/efcore/issues/32738).

#### Mitigations

- If you pass a different `--output-dir` for each batch of migrations, for example `Migrations/2025` and then `Migrations/2026`, the snapshot now moves each time. Use the same output directory for every migration, or omit `--output-dir` after the first one to keep new files beside the existing ones.
- The snapshot's path changes when it moves. Make sure the new path is committed and, if your project lists compile items explicitly, included in the project. The Package Manager Console updates these project items for you.
- If the snapshot is outside the project directory, pass the same `--output-dir` to every `migrations add` command. Otherwise EF can't find the existing snapshot, warns, and writes a second one inside the project.
