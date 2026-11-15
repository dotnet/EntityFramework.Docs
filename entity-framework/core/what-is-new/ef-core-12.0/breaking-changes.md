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

EF doesn't move or delete the previous snapshot. If the context's existing snapshot is in a different file, the project now contains two snapshot classes with the same name and doesn't compile until you delete the old file.

If a file with the snapshot's name already exists in the output directory and the context has no snapshot, such as the snapshot of another `DbContext`, the command fails instead of overwriting it. If the context does have a snapshot, an existing file in the output directory is overwritten with the updated snapshot.

With `--dry-run`, nothing is written.

#### Why

The `--output-dir` option is documented as the directory to put the generated files in, and having the snapshot elsewhere was surprising. It was hard to fix by hand: deleting the output folder to start over left a snapshot behind, and moving the migration files left the project with a snapshot in the wrong place. See [#24339](https://github.com/dotnet/efcore/issues/24339), [#28316](https://github.com/dotnet/efcore/issues/28316) and [#32738](https://github.com/dotnet/efcore/issues/32738).

#### Mitigations

- If you pass a different `--output-dir` for each batch of migrations, for example `Migrations/2025` and then `Migrations/2026`, delete the previous snapshot file after each `migrations add`, or use the same output directory for every migration. Omit `--output-dir` to keep new files beside the existing ones.
- If your project lists compile items explicitly, include the new snapshot file and remove the old one.
