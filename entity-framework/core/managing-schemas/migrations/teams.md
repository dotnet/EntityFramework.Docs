---
title: Migrations in Team Environments - EF Core
description: Best practices for managing migrations and resolving conflicts in team environments with Entity Framework Core
author: SamMonoRT
ms.date: 08/05/2026
uid: core/managing-schemas/migrations/teams
---
# Migrations in Team Environments

When working with Migrations in team environments, various problems can arise when migrations are added by multiple developers around the same time; note that migrations aren't simply SQL scripts but also include a snapshot of the model at the time of that migration.

For example, imagine developer A and B both create work branches at the same time, and generate a migration in their branches. If developer A merges their branch and then developer B does the same, the latest migration (developer B's) will have a context snapshot that does not include the changes from developer A's migration. This can cause various forms of corruption in later migrations.

As a result, it is highly recommended to coordinate in advance and to avoid working concurrently on migrations in multiple branches when possible.

Migrations form an ordered sequence. Each migration's designer metadata represents the model at that point in the sequence and is used when the migration is removed. Don't resolve parallel migrations by sorting or renaming their files: the later migration would still contain metadata that doesn't include the other branch's changes.

## Detecting diverged migration trees

> [!NOTE]
> This feature is being introduced in EF Core 11 from preview-3 onwards.

Starting with EF 11, the model snapshot records the ID of the latest migration. This means that if two developers each create a migration on separate branches, merging those branches will produce a source control conflict in the model snapshot file — since both branches modify the latest migration ID. This conflict is an important signal: it tells you that the migration trees have diverged, and one of them must be discarded before proceeding.

To resolve this, follow the steps in [Resolving diverged migration trees](#resolving-diverged-migration-trees) below: abort the merge, remove your migration (keeping your model changes), merge your teammate's changes, and then re-add your migration.

EF Core 10 and earlier don't record the latest migration ID in the model snapshot, so a source control system may merge the snapshot without reporting this conflict. The migration trees are still diverged and must be resolved using the same workflow.

## Resolving diverged migration trees

If, when merging a branch, a diverged migration tree is detected, resolve it by re-creating your migration. Follow these steps:

1. Abort the merge and return to your working directory before the merge.
2. Remove your migration, but keep the model changes that produced it. Source control can be used to remove only the generated migration files and restore the pre-migration snapshot.
3. Merge your teammate's changes into your working directory.
4. Re-add your migration so it is based on the merged model snapshot.

After doing this, your migration is cleanly based on top of any migrations that have been added in the other branch, and its context snapshot contains all previous changes. Your migration can now be safely shared with the rest of the team.

Don't run `dotnet ef migrations remove` (or `Remove-Migration`) after parallel migrations have already been merged into an invalid sequence. The command restores the model represented by the preceding migration's designer metadata, which may not contain the other branch's changes. Use source control to return to a coherent pre-merge state, and then follow the steps above.

## Revert migration changes in source control

Reverting a source control commit doesn't change any database. Before removing migration code, choose one of these approaches:

* If the migration hasn't been applied to a shared database, remove the migration and then revert the model changes.
* If the migration has been applied, migrate the database to an earlier migration while the migration code is still available, or deploy a new corrective migration. Keep application and database deployment compatible throughout the rollback.

Don't remove migration source that is still recorded in a shared database. If the code was already reverted, check out or restore the commit containing the migration to generate and test the rollback, and then commit a coherent migration sequence.
