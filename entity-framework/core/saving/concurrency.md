---
title: Handling Concurrency - EF Core
author: rowanmiller
ms.author: divega

ms.date: 11/27/2017

ms.technology: entity-framework-core

uid: core/saving/concurrency
---
# Handling Concurrency

If a property is configured as a concurrency token:

* EF will verify the property value has not been updated after it was fetched.
* The check occurs when saving changes to that record.

You can view this article's [sample](https://github.com/aspnet/EntityFramework.Docs/tree/master/samples/core/Saving/Saving/Concurrency/) on GitHub.

## How concurrency handling works in EF Core

For a detailed description of how concurrency handling works in Entity Framework Core, see [Concurrency Tokens](xref:core/modeling/concurrency).

## Resolving concurrency conflicts

Resolving a concurrency conflict involves using an algorithm to merge the pending changes from the current user with the values in the database. The merging approach will vary based on your application. A common approach is to display the values to the user and have them decide the correct values to be stored in the database.

**There are three sets of values available to help resolve a concurrency conflict.**

* **Current values** are the values that the application was attempting to write to the database.

* **Original values** are the values that were originally retrieved from the database, before any edits were made.

* **Database values** are the values currently stored in the database.

To handle a concurrency conflict:

* Catch `DbUpdateConcurrencyException` during `SaveChanges`.
* Use `DbUpdateConcurrencyException.Entries` to prepare a new set of changes for the affected entities.
* Retry the `SaveChanges` operation.

In the following example, `Person.FirstName` and `Person.LastName` are setup as concurrency tokens. There is a `// TODO:` comment in the location where you include application specific logic to choose the value to be saved.

[!code-csharp[Main](../../../samples/core/Saving/Saving/Concurrency/Sample.cs?highlight=60-63)]
