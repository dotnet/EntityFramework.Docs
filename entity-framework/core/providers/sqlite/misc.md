---
title: Miscellaneous - SQLite Database Provider - EF Core
description: Miscellaneous information for the SQLite database provider
author: roji
ms.date: 11/27/2025
uid: core/providers/sqlite/misc
---
# Miscellaneous notes for SQLite

## SaveChanges and the RETURNING clause

When EF Core saves changes to the database, it does so with an optimized technique using the SQL [RETURNING clause](https://sqlite.org/lang_returning.html). Unfortunately, the RETURNING clause has some limitations; it cannot be used with virtual tables or tables with certain trigger types, for example.

If you run into a limitation related to the use of the RETURNING clause, you can disable it on a specific table via <xref:Microsoft.EntityFrameworkCore.SqliteEntityTypeExtensions.UseSqlReturningClause*>:

```c#
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Blog>()
        .ToTable(tb => tb.UseSqlReturningClause(false));
}
```

Doing this will make EF switch to an older, less efficient technique for updating the table.

If most or all of your tables have triggers, you can configure this for all your model's tables by using the following [model building convention](xref:core/modeling/bulk-configuration#conventions):

```c#
public class NoOutputClauseConvention : IModelFinalizingConvention
{
    public virtual void ProcessModelFinalizing(
        IConventionModelBuilder modelBuilder,
        IConventionContext<IConventionModelBuilder> context)
    {
        foreach (var entityType in modelBuilder.Metadata.GetEntityTypes())
        {
            var table = StoreObjectIdentifier.Create(entityType, StoreObjectType.Table);
            if (table is not null)
            {
                entityType.Builder.UseSqlReturningClause(false);
            }

            foreach (var fragment in entityType.GetMappingFragments(StoreObjectType.Table))
            {
                entityType.Builder.UseSqlReturningClause(false, fragment.StoreObject);
            }
        }
    }
}
```

This effectively calls <xref:Microsoft.EntityFrameworkCore.SqliteEntityTypeExtensions.UseSqlReturningClause*> on all your model's tables, instead of you having to do it manually for each and every table.
