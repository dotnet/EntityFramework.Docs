---
title: ExecuteUpdate and ExecuteDelete - EF Core
description: Using ExecuteUpdate and ExecuteDelete to save changes with Entity Framework Core
author: roji
ms.date: 4/30/2023
uid: core/saving/execute-insert-update-delete
---
# ExecuteUpdate and ExecuteDelete

> [!NOTE]
> This feature was introduced in EF Core 7.0.

<xref:Microsoft.EntityFrameworkCore.RelationalQueryableExtensions.ExecuteUpdate*> and <xref:Microsoft.EntityFrameworkCore.RelationalQueryableExtensions.ExecuteDelete*> are a way to save data to the database without using EF's traditional change tracking and <xref:Microsoft.EntityFrameworkCore.DbContext.SaveChanges> method. For an introductory comparison of these two techniques, see the [Overview page](xref:core/saving/index) on saving data.

## ExecuteDelete

Let's assume that you need to delete all Blogs with a rating below a certain threshold. The traditional <xref:Microsoft.EntityFrameworkCore.DbContext.SaveChanges> approach requires you to do the following:

```c#
await foreach (var blog in context.Blogs.Where(b => b.Rating < 3).AsAsyncEnumerable())
{
    context.Blogs.Remove(blog);
}

await context.SaveChangesAsync();
```

This is quite an inefficient way to perform this task: we query the database for all Blogs matching our filter, and then we query, materialize and track all those instances; the number of matching entities could be huge. We then tell EF's change tracker that each Blog needs to be removed, and apply those changes by calling <xref:Microsoft.EntityFrameworkCore.DbContext.SaveChanges>, which generates a `DELETE` statement for each and every one of them.

Here is the same task performed via the <xref:Microsoft.EntityFrameworkCore.RelationalQueryableExtensions.ExecuteDelete*> API:

```c#
await context.Blogs.Where(b => b.Rating < 3).ExecuteDeleteAsync();
```

This uses the familiar LINQ operators to determine which Blogs should be affected - just as if we were querying them - and then tells EF to execute a SQL `DELETE` against the database:

```sql
DELETE FROM [b]
FROM [Blogs] AS [b]
WHERE [b].[Rating] < 3
```

Aside from being simpler and shorter, this executes very efficiently in the database, without loading any data from the database or involving EF's change tracker. Note that you can use arbitrary LINQ operators to select which Blogs you'd like to delete - these are translated to SQL for execution at the database, just as if you were querying those Blogs out.

## ExecuteUpdate

Rather than deleting these Blogs, what if we wanted to change a property to indicate that they should be hidden instead? <xref:Microsoft.EntityFrameworkCore.RelationalQueryableExtensions.ExecuteUpdate*> provides a similar way to express a SQL `UPDATE` statement:

```c#
await context.Blogs
    .Where(b => b.Rating < 3)
    .ExecuteUpdateAsync(setters => setters.SetProperty(b => b.IsVisible, false));
```

Like with `ExecuteDelete`, we first use LINQ to determine which Blogs should be affected; but with `ExecuteUpdate` we also need to express the change to be applied to the matching Blogs. This is done by calling `SetProperty` within the `ExecuteUpdate` call, and providing it with two arguments: the property to be changed (`IsVisible`), and the new value it should have (`false`). This causes the following SQL to be executed:

```sql
UPDATE [b]
SET [b].[IsVisible] = CAST(0 AS bit)
FROM [Blogs] AS [b]
WHERE [b].[Rating] < 3
```

### Updating multiple properties

`ExecuteUpdate` allows updating multiple properties in a single invocation. For example, to both set `IsVisible` to false and to set `Rating` to zero, simply chain additional `SetProperty` calls together:

```c#
await context.Blogs
    .Where(b => b.Rating < 3)
    .ExecuteUpdateAsync(setters => setters
        .SetProperty(b => b.IsVisible, false)
        .SetProperty(b => b.Rating, 0));
```

This executes the following SQL:

```sql
UPDATE [b]
SET [b].[Rating] = 0,
    [b].[IsVisible] = CAST(0 AS bit)
FROM [Blogs] AS [b]
WHERE [b].[Rating] < 3
```

### Referencing the existing property value

The above examples updated the property to a new constant value. `ExecuteUpdate` also allows referencing the existing property value when calculating the new value; for example, to increase the rating of all matching Blogs by one, use the following:

```c#
await context.Blogs
    .Where(b => b.Rating < 3)
    .ExecuteUpdateAsync(setters => setters.SetProperty(b => b.Rating, b => b.Rating + 1));
```

Note that the second argument to `SetProperty` is now a lambda function, and not a constant as before. Its `b` parameter represents the Blog being updated; within that lambda, `b.Rating` thus contains the rating before any change occurred. This executes the following SQL:

```sql
UPDATE [b]
SET [b].[Rating] = [b].[Rating] + 1
FROM [Blogs] AS [b]
WHERE [b].[Rating] < 3
```

### Navigations and related entities

`ExecuteUpdate` does not currently support referencing navigations within the `SetProperty` lambda. For example, let's say we want to update all the Blogs' ratings so that each Blog's new rating is the average of all its Posts' ratings. We may try to use `ExecuteUpdate` as follows:

```c#
await context.Blogs.ExecuteUpdateAsync(
    setters => setters.SetProperty(b => b.Rating, b => b.Posts.Average(p => p.Rating)));
```

However, EF does allow performing this operation by first using `Select` to calculate the average rating and project it to an anonymous type, and then using `ExecuteUpdate` over that:

```c#
await context.Blogs
    .Select(b => new { Blog = b, NewRating = b.Posts.Average(p => p.Rating) })
    .ExecuteUpdateAsync(setters => setters.SetProperty(b => b.Blog.Rating, b => b.NewRating));
```

This executes the following SQL:

```sql
UPDATE [b]
SET [b].[Rating] = CAST((
    SELECT AVG(CAST([p].[Rating] AS float))
    FROM [Post] AS [p]
    WHERE [b].[Id] = [p].[BlogId]) AS int)
FROM [Blogs] AS [b]
```

## Change tracking

Users familiar with `SaveChanges` are used to performing multiple changes, and then calling `SaveChanges` to apply all these changes to the database; this is made possible by EF's change tracker, which accumulates - or tracks - these changes.

`ExecuteUpdate` and `ExecuteDelete` work quite differently: they take effect immediately, at the point in which they are invoked. This means that while a single `ExecuteUpdate` or `ExecuteDelete` operation can affect many rows, it isn't possible to accumulate multiple such operations and apply them at once, e.g. when calling `SaveChanges`. In fact, the functions are completely unaware of EF's change tracker, and have no interaction with it whatsoever. This has several important consequences.

Consider the following code:

```c#
// 1. Query the blog with the name `SomeBlog`. Since EF queries are tracking by default, the Blog is now tracked by EF's change tracker.
var blog = await context.Blogs.SingleAsync(b => b.Name == "SomeBlog");

// 2. Increase the rating of all blogs in the database by one. This executes immediately.
await context.Blogs.ExecuteUpdateAsync(setters => setters.SetProperty(b => b.Rating, b => b.Rating + 1));

// 3. Increase the rating of `SomeBlog` by two. This modifies the .NET `Rating` property and is not yet persisted to the database.
blog.Rating += 2;

// 4. Persist tracked changes to the database.
await context.SaveChangesAsync();
```

Crucially, when `ExecuteUpdate` is invoked and all Blogs are updated in the database, EF's change tracker is **not** updated, and the tracked .NET instance still has its original rating value, from the point at which it was queried. Let's assume that the Blog's rating was originally 5; after the 3rd line executes, the rating in the database is now 6 (because of the `ExecuteUpdate`), whereas the rating in the tracked .NET instance is 7. When `SaveChanges` is called, EF detects that the new value 7 is different from the original value 5, and persists that change. The change performed by `ExecuteUpdate` is overwritten and not taken into account.

As a result, it is usually a good idea to avoid mixing both tracked `SaveChanges` modifications and untracked modifications via `ExecuteUpdate`/`ExecuteDelete`.

## Transactions

Continuing on the above, it's important to understand that `ExecuteUpdate` and `ExecuteDelete` do not implicitly start a transaction they're invoked. Consider the following code:

```c#
await context.Blogs.ExecuteUpdateAsync(/* some update */);
await context.Blogs.ExecuteUpdateAsync(/* another update */);

var blog = await context.Blogs.SingleAsync(b => b.Name == "SomeBlog");
blog.Rating += 2;
await context.SaveChangesAsync();
```

Each `ExecuteUpdate` call causes a single SQL `UPDATE` to be sent to the database. Since no transaction is created, if any sort of failure prevents the second `ExecuteUpdate` from completing successfully, the effects of the first one are still persisted to the database. In fact, the four operations above - two invocations of `ExecuteUpdate`, a query and `SaveChanges` - each executes within its own transaction. To wrap multiple operations in a single transaction, explicitly start a transaction with <xref:Microsoft.EntityFrameworkCore.Infrastructure.DatabaseFacade>:

```c#
using (var transaction = context.Database.BeginTransaction())
{
    context.Blogs.ExecuteUpdate(/* some update */);
    context.Blogs.ExecuteUpdate(/* another update */);

    ...
}
```

For more information about transaction handling, see [Using Transactions](xref:core/saving/transactions).

## Concurrency control and rows affected

`SaveChanges` provides automatic [Concurrency Control](xref:core/saving/concurrency), using a concurrency token to ensure that a row wasn't changed between the moment you loaded it and the moment you save changes to it. Since `ExecuteUpdate` and `ExecuteDelete` do not interact with the change tracker, they cannot automatically apply concurrency control.

However, both these methods do return the number of rows that were affected by the operation; this can come particularly handy for implementing concurrency control yourself:

```c#
// (load the ID and concurrency token for a Blog in the database)

var numUpdated = await context.Blogs
    .Where(b => b.Id == id && b.ConcurrencyToken == concurrencyToken)
    .ExecuteUpdateAsync(/* ... */);
if (numUpdated == 0)
{
    throw new Exception("Update failed!");
}
```

In this code, we use a LINQ `Where` operator to apply an update to a specific Blog, and only if its concurrency token has a specific value (e.g. the one we saw when querying the Blog from the database). We then check how many rows were actually updated by `ExecuteUpdate`; if the result is zero, no rows were updated and the concurrency token was likely changed as a result of a concurrent update.

## Limitations

* Only updating and deleting is currently supported; insertion must be done via <xref:Microsoft.EntityFrameworkCore.DbSet`1.Add*?displayProperty=nameWithType> and <xref:Microsoft.EntityFrameworkCore.DbContext.SaveChanges>.
* While the SQL UPDATE and DELETE statements allow retrieving original column values for the rows affected, this isn't currently supported by `ExecuteUpdate` and `ExecuteDelete`.
* Multiple invocations of these methods cannot be batched. Each invocation performs its own roundtrip to the database.
* Databases typically allow only a single table to be modified with UPDATE or DELETE.
* These methods currently only work with relational database providers.

## Additional resources

* [.NET Data Access Community Standup session](https://www.youtube.com/watch?v=rrKhbiXydKs&list=PLdo4fOcmZ0oX-DBuRG4u58ZTAJgBAeQ-t&index=33) where we discuss `ExecuteUpdate` and `ExecuteDelete`.
