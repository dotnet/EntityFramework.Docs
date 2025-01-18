---
title: .NET events - EF Core
description: .NET events defined by EF Core
author: SamMonoRT
ms.date: 10/15/2020
uid: core/logging-events-diagnostics/events
---

# .NET Events in EF Core

> [!TIP]
> You can [download the events sample](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Miscellaneous/Events) from GitHub.

Entity Framework Core (EF Core) exposes [.NET events](/dotnet/standard/events/) to act as callbacks when certain things happen in the EF Core code. Events are simpler than [interceptors](xref:core/logging-events-diagnostics/interceptors) and allow more flexible registration. However, they are sync only and so cannot perform non-blocking async I/O.

Events are registered per `DbContext` instance. Use a [diagnostic listener](xref:core/logging-events-diagnostics/diagnostic-listeners) to get the same information but for all DbContext instances in the process.

## Events raised by EF Core

The following events are raised by EF Core:

| Event | When raised
|:------|-------
| <xref:Microsoft.EntityFrameworkCore.DbContext.SavingChanges?displayProperty=nameWithType> | At the start of <xref:Microsoft.EntityFrameworkCore.DbContext.SaveChanges*> or <xref:Microsoft.EntityFrameworkCore.DbContext.SaveChangesAsync*>
| <xref:Microsoft.EntityFrameworkCore.DbContext.SavedChanges?displayProperty=nameWithType> | At the end of a successful <xref:Microsoft.EntityFrameworkCore.DbContext.SaveChanges*> or <xref:Microsoft.EntityFrameworkCore.DbContext.SaveChangesAsync*>
| <xref:Microsoft.EntityFrameworkCore.DbContext.SaveChangesFailed?displayProperty=nameWithType> | At the end of a failed <xref:Microsoft.EntityFrameworkCore.DbContext.SaveChanges*> or <xref:Microsoft.EntityFrameworkCore.DbContext.SaveChangesAsync*>
| <xref:Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.Tracked?displayProperty=nameWithType> | When an entity is tracked by the context
| <xref:Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.StateChanged?displayProperty=nameWithType> | When a tracked entity changes its state

### Example: Timestamp state changes

Each entity tracked by a DbContext has an <xref:Microsoft.EntityFrameworkCore.EntityState>. For example, the `Added` state indicates that the entity will be inserted into the database.

This example uses the <xref:Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.Tracked> and <xref:Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.StateChanged> events to detect when an entity changes state. It then stamps the entity with the current time indicating when this change happened. This results in timestamps indicating when the entity was inserted, deleted, and/or last updated.

The entity types in this example implement an interface that defines the timestamp properties:

<!--
public interface IHasTimestamps
{
    DateTime? Added { get; set; }
    DateTime? Deleted { get; set; }
    DateTime? Modified { get; set; }
}
-->
[!code-csharp[IHasTimestamps](../../../samples/core/Miscellaneous/Events/Program.cs?name=IHasTimestamps)]

A method on the application's DbContext can then set timestamps for any entity that implements this interface:

<!--
    private static void UpdateTimestamps(object sender, EntityEntryEventArgs e)
    {
        if (e.Entry.Entity is IHasTimestamps entityWithTimestamps)
        {
            switch (e.Entry.State)
            {
                case EntityState.Deleted:
                    entityWithTimestamps.Deleted = DateTime.UtcNow;
                    Console.WriteLine($"Stamped for delete: {e.Entry.Entity}");
                    break;
                case EntityState.Modified:
                    entityWithTimestamps.Modified = DateTime.UtcNow;
                    Console.WriteLine($"Stamped for update: {e.Entry.Entity}");
                    break;
                case EntityState.Added:
                    entityWithTimestamps.Added = DateTime.UtcNow;
                    Console.WriteLine($"Stamped for insert: {e.Entry.Entity}");
                    break;
            }
        }
    }
-->
[!code-csharp[UpdateTimestamps](../../../samples/core/Miscellaneous/Events/Program.cs?name=UpdateTimestamps)]

This method has the appropriate signature to use as an event handler for both the `Tracked` and `StateChanged` events. The handler is registered for both events in the DbContext constructor. Note that events can be attached to a DbContext at any time; it is not required that this happen in the context constructor.

<!--
    public BlogsContext()
    {
        ChangeTracker.StateChanged += UpdateTimestamps;
        ChangeTracker.Tracked += UpdateTimestamps;
    }
-->
[!code-csharp[ContextConstructor](../../../samples/core/Miscellaneous/Events/Program.cs?name=ContextConstructor)]

Both events are needed because new entities fire `Tracked` events when they are first tracked. `StateChanged` events are only fired for entities that change state while they are _already_ being tracked.

The [sample](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Miscellaneous/Events) for this example contains a simple console application that makes changes to the blogging database:

<!--
        using (var context = new BlogsContext())
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.Add(
                new Blog
                {
                    Id = 1,
                    Name = "EF Blog",
                    Posts =
                    {
                        new Post { Id = 1, Title = "EF Core 3.1!" },
                        new Post { Id = 2, Title = "EF Core 5.0!" }
                    }
                });

            context.SaveChanges();
        }

        using (var context = new BlogsContext())
        {
            var blog = context.Blogs.Include(e => e.Posts).Single();

            blog.Name = "EF Core Blog";
            context.Remove(blog.Posts.First());
            blog.Posts.Add(new Post { Id = 3, Title = "EF Core 6.0!" });

            context.SaveChanges();
        }
-->
[!code-csharp[Demonstration](../../../samples/core/Miscellaneous/Events/Program.cs?name=Demonstration)]

The output from this code shows the state changes happening and the timestamps being applied:

```output
Stamped for insert: Blog 1 Added on: 10/15/2020 11:01:26 PM
Stamped for insert: Post 1 Added on: 10/15/2020 11:01:26 PM
Stamped for insert: Post 2 Added on: 10/15/2020 11:01:26 PM
Stamped for delete: Post 1 Added on: 10/15/2020 11:01:26 PM Deleted on: 10/15/2020 11:01:26 PM
Stamped for update: Blog 1 Added on: 10/15/2020 11:01:26 PM Modified on: 10/15/2020 11:01:26 PM
Stamped for insert: Post 3 Added on: 10/15/2020 11:01:26 PM
```
