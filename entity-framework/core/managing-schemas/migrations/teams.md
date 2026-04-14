---
title: Migrations in Team Environments - EF Core
description: Best practices for managing migrations and resolving conflicts in team environments with Entity Framework Core
author: SamMonoRT
ms.date: 02/18/2026
uid: core/managing-schemas/migrations/teams
---
# Migrations in Team Environments

When working with Migrations in team environments, various problems can arise when migrations are added by multiple developers around the same time; note that migrations aren't simply SQL scripts but also include a snapshot of the model at the time of that migration.

For example, imagine developer A and B both create work branches at the same time, and generate a migration in their branches. If developer A merges their branch and then developer B does the same, the latest migration (developer B's) will have a context snapshot that does not include the changes from developer A's migration. This can cause various forms of corruption in later migrations.

As a result, it is highly recommended to coordinate in advance and to avoid working concurrently on migrations in multiple branches when possible.

## Detecting diverged migration trees

> [!NOTE]
> This feature is being introduced in EF Core 11 from preview-3 onwards.

Starting with EF 11, the model snapshot records the ID of the latest migration. This means that if two developers each create a migration on separate branches, merging those branches will produce a source control conflict in the model snapshot file — since both branches modify the latest migration ID. This conflict is an important signal: it tells you that the migration trees have diverged, and one of them must be discarded before proceeding.

To resolve this, follow the steps in [Resolving diverged migration trees](#resolving-diverged-migration-trees) below: abort the merge, remove your migration (keeping your model changes), merge your teammate's changes, and then re-add your migration.

## Resolving diverged migration trees

If, when merging a branch, a diverged migration tree is detected, resolve it by re-creating your migration. Follow these steps:

1. Abort the merge and rollback to your working directory before the merge
2. Remove your migration (but keep your model changes)
3. Merge your teammate's changes into your working directory
4. Re-add your migration

After doing this, your migration is cleanly based on top of any migrations that have been added in the other branch, and its context snapshot contains all previous changes. Your migration can now be safely shared with the rest of the team.
