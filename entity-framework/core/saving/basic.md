---
title: Basic Save - EF Core
description: Basic information on adding, updating and removing data with Entity Framework Core
author: ajcvickers
ms.date: 10/27/2016
uid: core/saving/basic
---
# Basic Save

Learn how to add, modify, and remove data using your context and entity classes.

> [!TIP]
> You can view this article's [sample](https://github.com/dotnet/EntityFramework.Docs/tree/master/samples/core/Saving/Basics/) on GitHub.

## Adding Data

Use the *DbSet.Add* method to add new instances of your entity classes. The data will be inserted in the database when you call *SaveChanges*.

[!code-csharp[Main](../../../samples/core/Saving/Basics/Sample.cs#Add)]

> [!TIP]
> The Add, Attach, and Update methods all work on the full graph of entities passed to them, as described in the [Related Data](xref:core/saving/related-data) section. Alternately, the EntityEntry.State property can be used to set the state of just a single entity. For example, `context.Entry(blog).State = EntityState.Modified`.

## Updating Data

EF will automatically detect changes made to an existing entity that is tracked by the context. This includes entities that you load/query from the database, and entities that were previously added and saved to the database.

Simply modify the values assigned to properties and then call *SaveChanges*.

[!code-csharp[Main](../../../samples/core/Saving/Basics/Sample.cs#Update)]

## Deleting Data

Use the *DbSet.Remove* method to delete instances of your entity classes.

If the entity already exists in the database, it will be deleted during *SaveChanges*. If the entity has not yet been saved to the database (that is, it is tracked as added) then it will be removed from the context and will no longer be inserted when *SaveChanges* is called.

[!code-csharp[Main](../../../samples/core/Saving/Basics/Sample.cs#Remove)]

## Multiple Operations in a single SaveChanges

You can combine multiple Add/Update/Remove operations into a single call to *SaveChanges*.

> [!NOTE]
> For most database providers, *SaveChanges* is transactional. This means  all the operations will either succeed or fail and the operations will never be left partially applied.

[!code-csharp[Main](../../../samples/core/Saving/Basics/Sample.cs#MultipleOperations)]
