---
title: Basic SaveChanges - EF Core
description: Basic information on adding, updating and removing data using SaveChanges with Entity Framework Core
author: SamMonoRT
ms.date: 4/30/2023
uid: core/saving/basic
---
# Basic SaveChanges

<xref:Microsoft.EntityFrameworkCore.DbContext.SaveChanges?displayProperty=nameWithType> is one of two techniques for saving changes to the database with EF. With this method, you perform one or more *tracked changes* (add, update, delete), and then apply those changes by calling the `SaveChanges` method. As an alternative, <xref:Microsoft.EntityFrameworkCore.RelationalQueryableExtensions.ExecuteUpdate*> and <xref:Microsoft.EntityFrameworkCore.RelationalQueryableExtensions.ExecuteDelete*> can be used without involving the change tracker. For an introductory comparison of these two techniques, see the [Overview page](xref:core/saving/index) on saving data.

> [!TIP]
> You can view this article's [sample](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Saving/Basics/) on GitHub.

## Adding Data

Use the <xref:Microsoft.EntityFrameworkCore.DbSet`1.Add*?displayProperty=nameWithType> method to add new instances of your entity classes. The data will be inserted into the database when you call <xref:Microsoft.EntityFrameworkCore.DbContext.SaveChanges?displayProperty=nameWithType>:

[!code-csharp[Main](../../../samples/core/Saving/Basics/Sample.cs#Add)]

> [!TIP]
> The `Add`, `Attach`, and `Update` methods all work on the full graph of entities passed to them, as described in the [Related Data](xref:core/saving/related-data) section. Alternately, the EntityEntry.State property can be used to set the state of just a single entity. For example, `context.Entry(blog).State = EntityState.Modified`.

## Updating Data

EF automatically detects changes made to an existing entity that is tracked by the context. This includes entities that you load/query from the database, and entities that were previously added and saved to the database.

Simply modify the values assigned to properties and then call `SaveChanges`:

[!code-csharp[Main](../../../samples/core/Saving/Basics/Sample.cs#Update)]

## Deleting Data

Use the <xref:Microsoft.EntityFrameworkCore.DbSet`1.Remove*?displayProperty=nameWithType> method to delete instances of your entity classes:

[!code-csharp[Main](../../../samples/core/Saving/Basics/Sample.cs#Remove)]

If the entity already exists in the database, it will be deleted during `SaveChanges`. If the entity has not yet been saved to the database (that is, it is tracked as added) then it will be removed from the context and will no longer be inserted when `SaveChanges` is called.

## Multiple Operations in a single SaveChanges

You can combine multiple Add/Update/Remove operations into a single call to `SaveChanges`:

[!code-csharp[Main](../../../samples/core/Saving/Basics/Sample.cs#MultipleOperations)]

> [!NOTE]
> For most database providers, `SaveChanges` is transactional. This means all the operations either succeed or fail and the operations are never be left partially applied.
