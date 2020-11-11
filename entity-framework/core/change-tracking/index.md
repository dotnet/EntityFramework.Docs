---
title: Change Tracking - EF Core
description: Overview of change tracking for EF Core  
author: ajcvickers
ms.date: 12/30/2020
uid: core/change-tracking/index
---

# Change Tracking in EF Core

Each <xref:Microsoft.EntityFrameworkCore.DbContext> instance tracks changes made to entities. These tracked entities in turn drive the changes to the database when <xref:Microsoft.EntityFrameworkCore.DbContext.SaveChanges%2A> or <xref:Microsoft.EntityFrameworkCore.DbContext.SaveChangesAsync%2A> is called.

This document presents an overview of Entity Framework Core (EF Core) change tracking and how it relates to queries and updates.

> [!TIP]  
> You can run and debug into all the code in this document by [downloading the sample code from GitHub](https://github.com/dotnet/EntityFramework.Docs/tree/master/samples/core/ChangeTracking/ChangeTrackingInEFCore).

## How to track entities

Entity instances become tracked when they are:

- Returned from a query executed against the database
- Explicitly attached to the DbContext by `Add`, `Attach`, `Update`, or similar methods
- Detected as new entities connected to existing tracked entities

Entity instances are no longer tracked when:

- The DbContext is disposed
- The change tracker is cleared (EF Core 5.0 and later)
- The entities are explicitly detached

DbContext is designed to represent a short-lived unit-of-work, as described in [DbContext Initialization and Configuration](xref:core/dbcontext-configuration/index). This means that disposing the DbContext is _the normal way_ to stop tracking entities. In other words, the lifetime of a DbContext should be:

1. Create the DbContext instance
2. Track some entities
3. Make some changes to the entities
4. Call SaveChanges to update the database
5. Dispose the DbContext instance

> [!TIP]
> It is not necessary to clear the change tracker or explicitly detach entity instances when taking this approach. However, if you do need to detach entities, then calling <xref:Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.Clear%2A?displayProperty=nameWithType> is more efficient than detaching entities one-by-one.

## Entity states

Every entity is is associated with a given <xref:Microsoft.EntityFrameworkCore.EntityState>:

- `Detached` entities are not being tracked by the <xref:Microsoft.EntityFrameworkCore.DbContext>.
- `Added` entities are new and have not yet been inserted into the database. This means they will be inserted when <xref:Microsoft.EntityFrameworkCore.DbContext.SaveChanges%2A> is called.
- `Unchanged` entities have _not_ been changed since they were queried from the database. All entities returned from queries are initially in this state.
- `Modified` entities have been changed since they were queried from the database. This means they will be updated when SaveChanges is called.
- `Deleted` entities exist in the database, but are marked to be deleted when SaveChanges is called.

EF Core tracks changes at the property level. For example, if only a single property value is modified, then a database update will change only that value. However, properties can only be marked as modified when the entity itself is in the Modified state. (Or, from an alternate perspective, the Modified state means that at least one property value has been changed.)

The following table summarizes the different states:

| Entity state     | Tracked by DbContext | Exists in database | Properties modified | Action on SaveChanges
|:-----------------|----------------------|--------------------|---------------------|-----------------------
| `Detached`       | No                   | -                  | -                   | -
| `Added`          | Yes                  | No                 | -                   | Insert
| `Unchanged`      | Yes                  | Yes                | No                  | -
| `Modified`       | Yes                  | Yes                | Yes                 | Update
| `Deleted`        | Yes                  | Yes                | -                   | Delete

> [!NOTE]
> This text uses relational database terms for clarity. NoSQL databases typically support similar operations but possibly with different names. Consult your database provider documentation for more information.

## Tracking from queries

EF Core change tracking works best when the same <xref:Microsoft.EntityFrameworkCore.DbContext> instance is used to both query for entities and update them by calling <xref:Microsoft.EntityFrameworkCore.DbContext.SaveChanges%2A>. This is because EF Core automatically tracks the state of queried entities and then detects any changes made to these entities when SaveChanges is called.

This approach has several advantages over explicit tracking, as described in the next section:

- It is simple. Entity states rarely need to be manipulated explicitly; EF Core takes care of state changes.
- Updates are limited to only those values that have actually changed.
- The values of [shadow properties](xref:core/modeling/shadow-properties) are preserved and used as needed. This is especially relevant when foreign keys are stored in shadow state.
- The original values of properties are preserved automatically and used for efficient updates.

### Simple query and update

For example, consider a simple blog/posts model:

<!--
public class Blog
{
    public int Id { get; set; }

    public string Name { get; set; }

    public ICollection<Post> Posts { get; } = new List<Post>();
}

public class Post
{
    public int Id { get; set; }
    
    public string Title { get; set; }
    public string Content { get; set; }
    
    public int? BlogId { get; set; }
    public Blog Blog { get; set; }
}
-->
[!code-csharp[Model](../../../samples/core/ChangeTracking/ChangeTrackingInEFCore/GeneratedKeysSamples.cs?name=Model)]

We can use this model to query for blogs and posts and then make some updates to the database:

<!--
            using var context = new BlogsContext();

            var blog = context.Blogs.Include(e => e.Posts).First(e => e.Name == ".NET Blog");

            blog.Name = ".NET Blog (Updated!)";

            foreach (var post in blog.Posts.Where(e => !e.Title.Contains("5.0")))
            {
                post.Title = post.Title.Replace("5", "5.0");
            }

            context.SaveChanges();
-->
[!code-csharp[Simple_query_and_update_1](../../../samples/core/ChangeTracking/ChangeTrackingInEFCore/GeneratedKeysSamples.cs?name=Simple_query_and_update_1)]

Calling SaveChanges results in the following database updates, using SQLite as an example database:

```sql
info: 12/21/2020 14:39:21.132 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command) 
      Executed DbCommand (0ms) [Parameters=[@p1='1' (DbType = String), @p0='.NET Blog (Updated!)' (Size = 20)], CommandType='Text', CommandTimeout='30']
      UPDATE "Blogs" SET "Name" = @p0
      WHERE "Id" = @p1;
      SELECT changes();
info: 12/21/2020 14:39:21.132 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command) 
      Executed DbCommand (0ms) [Parameters=[@p1='2' (DbType = String), @p0='Announcing F# 5.0' (Size = 17)], CommandType='Text', CommandTimeout='30']
      UPDATE "Posts" SET "Title" = @p0
      WHERE "Id" = @p1;
      SELECT changes();
```

The [change tracker debug view](xref:core/change-tracking/debug-views) is a great way visualize which entities are being tracked and what their states are. For example, inserting the following code into the sample above before calling SaveChanges:

<!--
                context.ChangeTracker.DetectChanges();
                Console.WriteLine(context.ChangeTracker.DebugView.LongView);
-->
[!code-csharp[Simple_query_and_update_2](../../../samples/core/ChangeTracking/ChangeTrackingInEFCore/GeneratedKeysSamples.cs?name=Simple_query_and_update_2)]

Generates the following output:

```output
Blog {Id: 1} Modified
  Id: 1 PK
  Name: '.NET Blog (Updated!)' Modified Originally '.NET Blog'
  Posts: [{Id: 1}, {Id: 2}, {Id: 3}]
Post {Id: 1} Unchanged
  Id: 1 PK
  BlogId: 1 FK
  Content: 'Announcing the release of EF Core 5.0, a full featured cross...'
  Title: 'Announcing the Release of EF Core 5.0'
  Blog: {Id: 1}
Post {Id: 2} Modified
  Id: 2 PK
  BlogId: 1 FK
  Content: 'F# 5 is the latest version of F#, the functional programming...'
  Title: 'Announcing F# 5.0' Modified Originally 'Announcing F# 5'
  Blog: {Id: 1}
```

Notice specifically:

- The `Blog.Name` property is marked as modified (`Name: '.NET Blog (Updated!)' Modified Originally '.NET Blog'`), and this results in the blog being in the `Modified` state.
- The `Post.Title` property of post 2 is marked as modified (`Title: 'Announcing F# 5.0' Modified Originally 'Announcing F# 5'`), and this results in this post being in the `Modified` state.
- The other property values of post 2 have not changed and are therefore not marked as modified. This is why these values are not included in the database update.
- The other post was not modified in any way. This is why it is still in the `Unchanged` state and are not included in the database update.

### Query then insert, update, and delete

Updates like those in the previous example can be combined with inserts and deletes in the same unit-of-work. For example:

<!--
            using var context = new BlogsContext();
            
            var blog = context.Blogs.Include(e => e.Posts).First(e => e.Name == ".NET Blog");

            // Modify property values
            blog.Name = ".NET Blog (Updated!)";

            // Insert a new Post
            blog.Posts.Add(new Post
            {
                Title = "What’s next for System.Text.Json?",
                Content = ".NET 5.0 was released recently and has come with many..."
            });
            
            // Mark an existing Post as Deleted

            var postToDelete = blog.Posts.Single(e => e.Title == "Announcing F# 5");
            context.Remove(postToDelete);
            
            context.ChangeTracker.DetectChanges();
            Console.WriteLine(context.ChangeTracker.DebugView.LongView);

            context.SaveChanges();
-->
[!code-csharp[Query_then_insert_update_and_delete_1](../../../samples/core/ChangeTracking/ChangeTrackingInEFCore/GeneratedKeysSamples.cs?name=Query_then_insert_update_and_delete_1)]

In this example:

- A blog and related posts are queried from the database and tracked.
- The `Blog.Name` property is changed.
- A new post is added to the collection of existing posts for the blog.
- An existing post is marked for deletion by calling <xref:Microsoft.EntityFrameworkCore.DbContext.Remove%2A?displayProperty=nameWithType>.

Looking again at the [change tracker debug view](xref:core/change-tracking/debug-views) before calling SaveChanges shows how EF Core is tracking these changes:

```output
Blog {Id: 1} Modified
  Id: 1 PK
  Name: '.NET Blog (Updated!)' Modified Originally '.NET Blog'
  Posts: [{Id: 1}, {Id: 2}, {Id: 3}, {Id: -2147482638}]
Post {Id: -2147482638} Added
  Id: -2147482638 PK Temporary
  BlogId: 1 FK
  Content: '.NET 5.0 was released recently and has come with many...'
  Title: 'What's next for System.Text.Json?'
  Blog: {Id: 1}
Post {Id: 1} Unchanged
  Id: 1 PK
  BlogId: 1 FK
  Content: 'Announcing the release of EF Core 5.0, a full featured cross...'
  Title: 'Announcing the Release of EF Core 5.0'
  Blog: {Id: 1}
Post {Id: 2} Deleted
  Id: 2 PK
  BlogId: 1 FK
  Content: 'F# 5 is the latest version of F#, the functional programming...'
  Title: 'Announcing F# 5'
  Blog: {Id: 1}
```

Notice that:

- The blog is marked as `Modified`. This will generate a database update.
- Post 2 is marked as `Deleted`. This will generate a database delete.
- A new post with a temporary ID is associated with blog 1 and is marked as `Added`. This will generate a database insert.

This results in the following database commands (using SQLite) when SaveChanges is called:

```sql
info: 12/28/2020 10:08:02.434 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command)
      Executed DbCommand (0ms) [Parameters=[@p1='1' (DbType = String), @p0='.NET Blog (Updated!)' (Size = 20)], CommandType='Text', CommandTimeout='30']
      UPDATE "Blogs" SET "Name" = @p0
      WHERE "Id" = @p1;
      SELECT changes();
info: 12/28/2020 10:08:02.435 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command)
      Executed DbCommand (0ms) [Parameters=[@p0='2' (DbType = String)], CommandType='Text', CommandTimeout='30']
      DELETE FROM "Posts"
      WHERE "Id" = @p0;
      SELECT changes();
info: 12/28/2020 10:08:02.436 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command)
      Executed DbCommand (0ms) [Parameters=[@p0='1' (DbType = String), @p1='.NET 5.0 was released recently and has come with many...' (Size = 56), @p2='What's next for System.Text.Json?' (Size = 33)], CommandType='Text', CommandTimeout='30']
      INSERT INTO "Posts" ("BlogId", "Content", "Title")
      VALUES (@p0, @p1, @p2);
      SELECT "Id"
      FROM "Posts"
      WHERE changes() = 1 AND "rowid" = last_insert_rowid();
```

See the following section on _Explicit tracking_ for more information on inserting and deleting entities. See [Change Detection and Notifications](xref:core/change-tracking/change-detection) for more information on how EF Core automatically detects changes like this.

> [!TIP]
> Call <xref:Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.HasChanges?displayProperty=nameWithType> to determine whether any changes have been made that will cause SaveChanges to make updates to the database. If HasChanges return false, then SaveChanges will be a no-op.

## Explicit tracking

Entities can be explicitly "attached" to a <xref:Microsoft.EntityFrameworkCore.DbContext> such that the context then tracks those entities. This is primarily useful when:

1. Creating new entities that will be inserted into the database.
2. Re-attaching disconnected entities that were previously queried by a _different_ DbContext instance.

The first of these will be needed by most applications, and is primary handled by the <xref:Microsoft.EntityFrameworkCore.DbContext.Add%2A?displayProperty=nameWithType> methods.

The second is only needed by applications that change entities or their relationships **_while the entities are not being tracked_**. For example, a web application may send entities to the web client where the user makes changes and sends the entities back. These entities are referred to as "disconnected" since they were originally queried from a DbContext, but were then disconnected from that context when sent to the client.

The web application must now re-attach these entities so that they are again tracked and indicate the changes that have been made such that <xref:Microsoft.EntityFrameworkCore.DbContext.SaveChanges%2A> can make appropriate updates to the database. This is primarily handled by the <xref:Microsoft.EntityFrameworkCore.DbContext.Attach%2A?displayProperty=nameWithType> and <xref:Microsoft.EntityFrameworkCore.DbContext.Update%2A?displayProperty=nameWithType> methods.

> [!TIP]
> Attaching entities to the _same DbContext instance_ that they were queried from should not normally be needed. Do not routinely perform a no-tracking query and then attach the returned entities to the context. This will be both slower and harder to get right than using a tracking query.

### Generated vs explicit key values

By default, integer and GUID [key properties](xref:core/modeling/keys) are configured to use [automatically generated key values](xref:core/modeling/generated-properties). This has a **major advantage for change tracking: an unset key value indicates that the entity is "new"**. That is, it has not yet been inserted into the database.

Two models are used in the following sections. The first is configured to **not** use generated key values:

<!--
    public class Blog
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        
        public string Name { get; set; }

        public IList<Post> Posts { get; } = new List<Post>();
    }

    public class Post
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }

        public int? BlogId { get; set; }
        public Blog Blog { get; set; }
    }
-->
[!code-csharp[Model](../../../samples/core/ChangeTracking/ChangeTrackingInEFCore/ExplicitKeysSamples.cs?name=Model)]

Non-generated (i.e. explicitly set) key values are shown first in each example because everything is very explicit and easy to follow. This is then followed by an example where generated key values are used:

<!--
public class Blog
{
    public int Id { get; set; }

    public string Name { get; set; }

    public ICollection<Post> Posts { get; } = new List<Post>();
}

public class Post
{
    public int Id { get; set; }
    
    public string Title { get; set; }
    public string Content { get; set; }
    
    public int? BlogId { get; set; }
    public Blog Blog { get; set; }
}
-->
[!code-csharp[Model](../../../samples/core/ChangeTracking/ChangeTrackingInEFCore/GeneratedKeysSamples.cs?name=Model)]

Notice that the key properties need no additional configuration here since using generated key values is the [default for simple integer keys](xref:core/modeling/generated-properties).

### Inserting new entities

#### Explicit key values

For an entity must be tracked in the `Added` state to be inserted by <xref:Microsoft.EntityFrameworkCore.DbContext.SaveChanges%2A>, as described earlier. Entities are typically put in the Added state by calling one of <xref:Microsoft.EntityFrameworkCore.DbContext.Add%2A?displayProperty=nameWithType>, <xref:Microsoft.EntityFrameworkCore.DbContext.AddRange%2A?displayProperty=nameWithType>, <xref:Microsoft.EntityFrameworkCore.DbContext.AddAsync%2A?displayProperty=nameWithType>, <xref:Microsoft.EntityFrameworkCore.DbContext.AddRangeAsync%2A?displayProperty=nameWithType>, or equivalent methods on <xref:Microsoft.EntityFrameworkCore.DbSet%601>.

> [!TIP]
> In the context of change tracking, all these methods work in the same way. See [Additional Change Tracking Features](xref:core/change-tracking/miscellaneous) for more information.

For example, to start tracking a new blog:

<!--
            context.Add(
                new Blog
                {
                    Id = 1,
                    Name = ".NET Blog",
                });
-->
[!code-csharp[Inserting_new_entities_1](../../../samples/core/ChangeTracking/ChangeTrackingInEFCore/ExplicitKeysSamples.cs?name=Inserting_new_entities_1)]

Inspecting the [change tracker debug view](xref:core/change-tracking/debug-views) following this call shows that the context is tracking the new entity in the `Added` state:

```output
Blog {Id: 1} Added
  Id: 1 PK
  Name: '.NET Blog'
  Posts: []
```

However, the Add methods don't just work on an individual entity. They actually start tracking an _entire graph of related entities_, putting them all to the `Added` state. For example, to insert a new blog and associated new posts:

<!--
            context.Add(
                new Blog
                {
                    Id = 1,
                    Name = ".NET Blog",
                    Posts =
                    {
                        new Post
                        {
                            Id = 1,
                            Title = "Announcing the Release of EF Core 5.0",
                            Content = "Announcing the release of EF Core 5.0, a full featured cross-platform..."
                        },
                        new Post
                        {
                            Id = 2,
                            Title = "Announcing F# 5",
                            Content = "F# 5 is the latest version of F#, the functional programming language..."
                        }
                    }
                });
-->
[!code-csharp[Inserting_new_entities_2](../../../samples/core/ChangeTracking/ChangeTrackingInEFCore/ExplicitKeysSamples.cs?name=Inserting_new_entities_2)]

The context is now tracking all these entities as `Added`:

```output
Blog {Id: 1} Added
  Id: 1 PK
  Name: '.NET Blog'
  Posts: [{Id: 1}, {Id: 2}]
Post {Id: 1} Added
  Id: 1 PK
  BlogId: 1 FK
  Content: 'Announcing the release of EF Core 5.0, a full featured cross...'
  Title: 'Announcing the Release of EF Core 5.0'
  Blog: {Id: 1}
Post {Id: 2} Added
  Id: 2 PK
  BlogId: 1 FK
  Content: 'F# 5 is the latest version of F#, the functional programming...'
  Title: 'Announcing F# 5'
  Blog: {Id: 1}
```

Notice that explicit values have been set for the `Id` key properties in the examples above. This is because the model here has been configured to use explicitly set key values, rather than automatically generated key values. When not using generated keys, the key properties must be explicitly in this way _before_ calling `Add`. These key values are then inserted when SaveChanges is called. For example, when using SQLite:

```sql
info: 12/28/2020 10:53:54.530 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command)
      Executed DbCommand (0ms) [Parameters=[@p0='1' (DbType = String), @p1='.NET Blog' (Size = 9)], CommandType='Text', CommandTimeout='30']
      INSERT INTO "Blogs" ("Id", "Name")
      VALUES (@p0, @p1);
info: 12/28/2020 10:53:54.530 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command)
      Executed DbCommand (0ms) [Parameters=[@p2='1' (DbType = String), @p3='1' (DbType = String), @p4='Announcing the release of EF Core 5.0, a full featured cross-platform...' (Size = 72), @p5='Announcing the Release of EF Core 5.0' (Size = 37)], CommandType='Text', CommandTimeout='30']
      INSERT INTO "Posts" ("Id", "BlogId", "Content", "Title")
      VALUES (@p2, @p3, @p4, @p5);
info: 12/28/2020 10:53:54.531 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command)
      Executed DbCommand (0ms) [Parameters=[@p0='2' (DbType = String), @p1='1' (DbType = String), @p2='F# 5 is the latest version of F#, the functional programming language...' (Size = 72), @p3='Announcing F# 5' (Size = 15)], CommandType='Text', CommandTimeout='30']
      INSERT INTO "Posts" ("Id", "BlogId", "Content", "Title")
      VALUES (@p0, @p1, @p2, @p3);
```

All of these entities are tracked in the `Unchanged` after SaveChanges completes, since these entities now exist in the database:

```output
Blog {Id: 1} Unchanged
  Id: 1 PK
  Name: '.NET Blog'
  Posts: [{Id: 1}, {Id: 2}]
Post {Id: 1} Unchanged
  Id: 1 PK
  BlogId: 1 FK
  Content: 'Announcing the release of EF Core 5.0, a full featured cross...'
  Title: 'Announcing the Release of EF Core 5.0'
  Blog: {Id: 1}
Post {Id: 2} Unchanged
  Id: 2 PK
  BlogId: 1 FK
  Content: 'F# 5 is the latest version of F#, the functional programming...'
  Title: 'Announcing F# 5'
  Blog: {Id: 1}
```

#### Generated key values

As mentioned above, integer and GUID [key properties](xref:core/modeling/keys) are configured to use [automatically generated key values](xref:core/modeling/generated-properties) by default. This means that the application _must not set any key value explicitly_. For example, to insert a new blog and posts all with generated key values:

<!--
            context.Add(
                new Blog
                {
                    Name = ".NET Blog",
                    Posts =
                    {
                        new Post
                        {
                            Title = "Announcing the Release of EF Core 5.0",
                            Content = "Announcing the release of EF Core 5.0, a full featured cross-platform..."
                        },
                        new Post
                        {
                            Title = "Announcing F# 5",
                            Content = "F# 5 is the latest version of F#, the functional programming language..."
                        }
                    }
                });
-->
[!code-csharp[Inserting_new_entities_3](../../../samples/core/ChangeTracking/ChangeTrackingInEFCore/GeneratedKeysSamples.cs?name=Inserting_new_entities_3)]

As with explicit key values, the context is now tracking all these entities as `Added`:

```output
Blog {Id: -2147482644} Added
  Id: -2147482644 PK Temporary
  Name: '.NET Blog'
  Posts: [{Id: -2147482637}, {Id: -2147482636}]
Post {Id: -2147482637} Added
  Id: -2147482637 PK Temporary
  BlogId: -2147482644 FK Temporary
  Content: 'Announcing the release of EF Core 5.0, a full featured cross...'
  Title: 'Announcing the Release of EF Core 5.0'
  Blog: {Id: -2147482644}
Post {Id: -2147482636} Added
  Id: -2147482636 PK Temporary
  BlogId: -2147482644 FK Temporary
  Content: 'F# 5 is the latest version of F#, the functional programming...'
  Title: 'Announcing F# 5'
  Blog: {Id: -2147482644}
```

Notice in this case that [temporary key values](xref:core/change-tracking/miscellaneous) have been generated for each entity. These values are used by EF Core until SaveChanges is called, at which point real key values are read back from the database. For example, when using SQLite:

```sql
info: 12/28/2020 11:00:01.991 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command)
      Executed DbCommand (0ms) [Parameters=[@p0='.NET Blog' (Size = 9)], CommandType='Text', CommandTimeout='30']
      INSERT INTO "Blogs" ("Name")
      VALUES (@p0);
      SELECT "Id"
      FROM "Blogs"
      WHERE changes() = 1 AND "rowid" = last_insert_rowid();
info: 12/28/2020 11:00:01.991 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command)
      Executed DbCommand (0ms) [Parameters=[@p1='1' (DbType = String), @p2='Announcing the release of EF Core 5.0, a full featured cross-platform...' (Size = 72), @p3='Announcing the Release of EF Core 5.0' (Size = 37)], CommandType='Text', CommandTimeout='30']
      INSERT INTO "Posts" ("BlogId", "Content", "Title")
      VALUES (@p1, @p2, @p3);
      SELECT "Id"
      FROM "Posts"
      WHERE changes() = 1 AND "rowid" = last_insert_rowid();
info: 12/28/2020 11:00:01.992 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command)
      Executed DbCommand (0ms) [Parameters=[@p0='1' (DbType = String), @p1='F# 5 is the latest version of F#, the functional programming language...' (Size = 72), @p2='Announcing F# 5' (Size = 15)], CommandType='Text', CommandTimeout='30']
      INSERT INTO "Posts" ("BlogId", "Content", "Title")
      VALUES (@p0, @p1, @p2);
      SELECT "Id"
      FROM "Posts"
      WHERE changes() = 1 AND "rowid" = last_insert_rowid();
```

After SaveChanges completes, all of the entities have been updated with their real key values and are tracked in the `Unchanged` state since they now match the state in the database:

```output
Blog {Id: 1} Unchanged
  Id: 1 PK
  Name: '.NET Blog'
  Posts: [{Id: 1}, {Id: 2}]
Post {Id: 1} Unchanged
  Id: 1 PK
  BlogId: 1 FK
  Content: 'Announcing the release of EF Core 5.0, a full featured cross...'
  Title: 'Announcing the Release of EF Core 5.0'
  Blog: {Id: 1}
Post {Id: 2} Unchanged
  Id: 2 PK
  BlogId: 1 FK
  Content: 'F# 5 is the latest version of F#, the functional programming...'
  Title: 'Announcing F# 5'
  Blog: {Id: 1}
```

This is exactly the same end-state as the previous example that used explicit key values.

> [!TIP]
> An explicit key value can still be set even when using generated key values. EF Core will then attempt to insert using this key value. Some database configurations, including SQL Server with Identity columns, do not support such inserts and will throw.

### Attaching existing entities

#### Explicit key values

Entities returned from queries are tracked in the `Unchanged` state. As described earlier, the `Unchanged` state means that the entity has not been modified since it was queried. A disconnected entity, perhaps returned from a web client in an HTTP request, can be put into this state using either <xref:Microsoft.EntityFrameworkCore.DbContext.Attach%2A?displayProperty=nameWithType>, <xref:Microsoft.EntityFrameworkCore.DbContext.AttachRange%2A?displayProperty=nameWithType>, or the equivalent methods on <xref:Microsoft.EntityFrameworkCore.DbSet%601>. For example, to start tracking an existing blog:

<!--
            context.Attach(
                new Blog
                {
                    Id = 1,
                    Name = ".NET Blog",
                });
-->
[!code-csharp[Attaching_existing_entities_1](../../../samples/core/ChangeTracking/ChangeTrackingInEFCore/ExplicitKeysSamples.cs?name=Attaching_existing_entities_1)]

> [!NOTE]
> The examples here are creating entities explicitly with `new` for simplicity. Normally the entity instances will have come from another source, such as being deserialized from a client, or being created from data in an HTTP Post.

Inspecting the [change tracker debug view](xref:core/change-tracking/debug-views) following this call shows that the entity is tracked in the `Unchanged` state:

```output
Blog {Id: 1} Unchanged
  Id: 1 PK
  Name: '.NET Blog'
  Posts: []
```

Just like `Add`, `Attach` actually sets an entire graph of connected entities to the `Unchanged` state. For example, to attach an existing blog and associated existing posts:

<!--
            context.Attach(
                new Blog
                {
                    Id = 1,
                    Name = ".NET Blog",
                    Posts =
                    {
                        new Post
                        {
                            Id = 1,
                            Title = "Announcing the Release of EF Core 5.0",
                            Content = "Announcing the release of EF Core 5.0, a full featured cross-platform..."
                        },
                        new Post
                        {
                            Id = 2,
                            Title = "Announcing F# 5",
                            Content = "F# 5 is the latest version of F#, the functional programming language..."
                        }
                    }
                });
-->
[!code-csharp[Attaching_existing_entities_2](../../../samples/core/ChangeTracking/ChangeTrackingInEFCore/ExplicitKeysSamples.cs?name=Attaching_existing_entities_2)]

The context is now tracking all these entities as `Unchanged`:

```output
Blog {Id: 1} Unchanged
  Id: 1 PK
  Name: '.NET Blog'
  Posts: [{Id: 1}, {Id: 2}]
Post {Id: 1} Unchanged
  Id: 1 PK
  BlogId: 1 FK
  Content: 'Announcing the release of EF Core 5.0, a full featured cross...'
  Title: 'Announcing the Release of EF Core 5.0'
  Blog: {Id: 1}
Post {Id: 2} Unchanged
  Id: 2 PK
  BlogId: 1 FK
  Content: 'F# 5 is the latest version of F#, the functional programming...'
  Title: 'Announcing F# 5'
  Blog: {Id: 1}
```

Calling SaveChanges at this point will have no effect. All the entities are marked as `Unchanged`, so there is nothing to update in the database.

#### Generated key values

As mentioned above, integer and GUID [key properties](xref:core/modeling/keys) are configured to use [automatically generated key values](xref:core/modeling/generated-properties) by default. This has a major advantage when working with disconnected entities: an unset key value indicates that the entity has not yet been inserted into the database. This allows the change tracker to automatically detect new entities and put them in the `Added` state. For example, consider attaching this graph of a blog and posts:

```c#
            context.Attach(
                new Blog
                {
                    Id = 1,
                    Name = ".NET Blog",
                    Posts =
                    {
                        new Post
                        {
                            Id = 1,
                            Title = "Announcing the Release of EF Core 5.0",
                            Content = "Announcing the release of EF Core 5.0, a full featured cross-platform..."
                        },
                        new Post
                        {
                            Id = 2,
                            Title = "Announcing F# 5",
                            Content = "F# 5 is the latest version of F#, the functional programming language..."
                        },
                        new Post
                        {
                            Title = "Announcing .NET 5.0",
                            Content = ".NET 5.0 includes many enhancements, including single file applications, more..."
                        },
                    }
                });
```

<!--
            context.Attach(
                new Blog
                {
                    Id = 1,
                    Name = ".NET Blog",
                    Posts =
                    {
                        new Post
                        {
                            Id = 1,
                            Title = "Announcing the Release of EF Core 5.0",
                            Content = "Announcing the release of EF Core 5.0, a full featured cross-platform..."
                        },
                        new Post
                        {
                            Id = 2,
                            Title = "Announcing F# 5",
                            Content = "F# 5 is the latest version of F#, the functional programming language..."
                        },
                        new Post
                        {
                            Title = "Announcing .NET 5.0",
                            Content = ".NET 5.0 includes many enhancements, including single file applications, more..."
                        },
                    }
                });
-->
[!code-csharp[Attaching_existing_entities_3](../../../samples/core/ChangeTracking/ChangeTrackingInEFCore/GeneratedKeysSamples.cs?name=Attaching_existing_entities_3)]

The blog has a key value of 1, indicating that it already exists in the database. Two of the posts also have key values set, but the third does not. EF Core will see this key value as 0, the CLR default for an integer. This results in EF Core marking the new entity as `Added` instead of `Unchanged`:

```output
Blog {Id: 1} Unchanged
  Id: 1 PK
  Name: '.NET Blog'
  Posts: [{Id: 1}, {Id: 2}, {Id: -2147482636}]
Post {Id: -2147482636} Added
  Id: -2147482636 PK Temporary
  BlogId: 1 FK
  Content: '.NET 5.0 includes many enhancements, including single file a...'
  Title: 'Announcing .NET 5.0'
  Blog: {Id: 1}
Post {Id: 1} Unchanged
  Id: 1 PK
  BlogId: 1 FK
  Content: 'Announcing the release of EF Core 5.0, a full featured cross...'
  Title: 'Announcing the Release of EF Core 5.0'
  Blog: {Id: 1}
Post {Id: 2} Unchanged
  Id: 2 PK
  BlogId: 1 FK
  Content: 'F# 5 is the latest version of F#, the functional programming...'
```

Calling SaveChanges at this point does nothing with the `Unchanged` entities, but inserts the new entity into the database. For example, when using SQLite:

```sql
info: 12/28/2020 11:27:57.807 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command)
      Executed DbCommand (0ms) [Parameters=[@p0='1' (DbType = String), @p1='.NET 5.0 includes many enhancements, including single file applications, more...' (Size = 80), @p2='Announcing .NET 5.0' (Size = 19)], CommandType='Text', CommandTimeout='30']
      INSERT INTO "Posts" ("BlogId", "Content", "Title")
      VALUES (@p0, @p1, @p2);
      SELECT "Id"
      FROM "Posts"
      WHERE changes() = 1 AND "rowid" = last_insert_rowid();
```

The important point to notice here is that, with generated key values, EF Core is able to **automatically distinguish new from existing entities in a disconnected graph**. In a nutshell, when using generated keys, EF Core will always insert an entity when that entity has no key value set.

### Updating existing entities

#### Explicit key values

<xref:Microsoft.EntityFrameworkCore.DbContext.Update%2A?displayProperty=nameWithType>, <xref:Microsoft.EntityFrameworkCore.DbContext.UpdateRange%2A?displayProperty=nameWithType>, and the equivalent methods on <xref:Microsoft.EntityFrameworkCore.DbSet%601> behave exactly as the `Attach` methods described above, except that entities are put into the `Modfied` instead of the `Unchanged` state. For example, to start tracking an existing blog as `Modified`:

<!--
            context.Update(
                new Blog
                {
                    Id = 1,
                    Name = ".NET Blog",
                });
-->
[!code-csharp[Updating_existing_entities_1(](../../../samples/core/ChangeTracking/ChangeTrackingInEFCore/ExplicitKeysSamples.cs?name=Updating_existing_entities_1()]

Inspecting the [change tracker debug view](xref:core/change-tracking/debug-views) following this call shows that the context is tracking this entity in the `Modified` state:

```output
Blog {Id: 1} Modified
  Id: 1 PK
  Name: '.NET Blog' Modified
  Posts: []
```

Just like with `Add` and `Attach`, `Update` actually marks an _entire graph_ of related entities as `Modified`. For example, to attach an existing blog and associated existing posts as `Modified`:

<!--
            context.Update(
                new Blog
                {
                    Id = 1,
                    Name = ".NET Blog",
                    Posts =
                    {
                        new Post
                        {
                            Id = 1,
                            Title = "Announcing the Release of EF Core 5.0",
                            Content = "Announcing the release of EF Core 5.0, a full featured cross-platform..."
                        },
                        new Post
                        {
                            Id = 2,
                            Title = "Announcing F# 5",
                            Content = "F# 5 is the latest version of F#, the functional programming language..."
                        }
                    }
                });
-->
[!code-csharp[Updating_existing_entities_2(](../../../samples/core/ChangeTracking/ChangeTrackingInEFCore/ExplicitKeysSamples.cs?name=Updating_existing_entities_2()]

The context is now tracking all these entities as `Modified`:

```output
Blog {Id: 1} Modified
  Id: 1 PK
  Name: '.NET Blog' Modified
  Posts: [{Id: 1}, {Id: 2}]
Post {Id: 1} Modified
  Id: 1 PK
  BlogId: 1 FK Modified Originally <null>
  Content: 'Announcing the release of EF Core 5.0, a full featured cross...' Modified
  Title: 'Announcing the Release of EF Core 5.0' Modified
  Blog: {Id: 1}
Post {Id: 2} Modified
  Id: 2 PK
  BlogId: 1 FK Modified Originally <null>
  Content: 'F# 5 is the latest version of F#, the functional programming...' Modified
  Title: 'Announcing F# 5' Modified
  Blog: {Id: 1}
```

Calling SaveChanges at this point will cause updates to be sent to the database for all these entities. For example, when using SQLite:

```sql
info: 12/28/2020 11:35:17.916 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command)
      Executed DbCommand (0ms) [Parameters=[@p1='1' (DbType = String), @p0='.NET Blog' (Size = 9)], CommandType='Text', CommandTimeout='30']
      UPDATE "Blogs" SET "Name" = @p0
      WHERE "Id" = @p1;
      SELECT changes();
info: 12/28/2020 11:35:17.916 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command)
      Executed DbCommand (0ms) [Parameters=[@p3='1' (DbType = String), @p0='1' (DbType = String), @p1='Announcing the release of EF Core 5.0, a full featured cross-platform...' (Size = 72), @p2='Announcing the Release of EF Core 5.0' (Size = 37)], CommandType='Text', CommandTimeout='30']
      UPDATE "Posts" SET "BlogId" = @p0, "Content" = @p1, "Title" = @p2
      WHERE "Id" = @p3;
      SELECT changes();
info: 12/28/2020 11:35:17.916 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command)
      Executed DbCommand (0ms) [Parameters=[@p3='2' (DbType = String), @p0='1' (DbType = String), @p1='F# 5 is the latest version of F#, the functional programming language...' (Size = 72), @p2='Announcing F# 5' (Size = 15)], CommandType='Text', CommandTimeout='30']
      UPDATE "Posts" SET "BlogId" = @p0, "Content" = @p1, "Title" = @p2
      WHERE "Id" = @p3;
      SELECT changes();
```

#### Generated key values

As with `Attach`, generated key values have the same major benefit for `Update`: an unset key value indicates that the entity is new and has not yet been inserted into the database. As with `Attach`, this allows the DbContext to automatically detect new entities and put them in the `Added` state. For example, consider calling `Update` with this graph of a blog and posts:

<!--
            context.Update(
                new Blog
                {
                    Id = 1,
                    Name = ".NET Blog",
                    Posts =
                    {
                        new Post
                        {
                            Id = 1,
                            Title = "Announcing the Release of EF Core 5.0",
                            Content = "Announcing the release of EF Core 5.0, a full featured cross-platform..."
                        },
                        new Post
                        {
                            Id = 2,
                            Title = "Announcing F# 5",
                            Content = "F# 5 is the latest version of F#, the functional programming language..."
                        }
                    }
                });
-->
[!code-csharp[Updating_existing_entities_3(](../../../samples/core/ChangeTracking/ChangeTrackingInEFCore/GeneratedKeysSamples.cs?name=Updating_existing_entities_3()]

As with the `Attach` example, the post with no key value set is detected as new and set to the `Added` state. The other entities are marked as `Modified`:  

```output
Blog {Id: 1} Modified
  Id: 1 PK
  Name: '.NET Blog' Modified
  Posts: [{Id: 1}, {Id: 2}, {Id: -2147482633}]
Post {Id: -2147482633} Added
  Id: -2147482633 PK Temporary
  BlogId: 1 FK
  Content: '.NET 5.0 includes many enhancements, including single file a...'
  Title: 'Announcing .NET 5.0'
  Blog: {Id: 1}
Post {Id: 1} Modified
  Id: 1 PK
  BlogId: 1 FK Modified Originally <null>
  Content: 'Announcing the release of EF Core 5.0, a full featured cross...' Modified
  Title: 'Announcing the Release of EF Core 5.0' Modified
  Blog: {Id: 1}
Post {Id: 2} Modified
  Id: 2 PK
  BlogId: 1 FK Modified Originally <null>
  Content: 'F# 5 is the latest version of F#, the functional programming...' Modified
  Title: 'Announcing F# 5' Modified
  Blog: {Id: 1}
```

Calling `SaveChanges` at this point will cause updates to be sent to the database for all the existing entities, while the new entity is inserted. For example, when using SQLite:

```sql
info: 12/28/2020 11:44:37.839 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command)
      Executed DbCommand (0ms) [Parameters=[@p1='1' (DbType = String), @p0='.NET Blog' (Size = 9)], CommandType='Text', CommandTimeout='30']
      UPDATE "Blogs" SET "Name" = @p0
      WHERE "Id" = @p1;
      SELECT changes();
info: 12/28/2020 11:44:37.840 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command)
      Executed DbCommand (0ms) [Parameters=[@p3='1' (DbType = String), @p0='1' (DbType = String), @p1='Announcing the release of EF Core 5.0, a full featured cross-platform...' (Size = 72), @p2='Announcing the Release of EF Core 5.0' (Size = 37)], CommandType='Text', CommandTimeout='30']
      UPDATE "Posts" SET "BlogId" = @p0, "Content" = @p1, "Title" = @p2
      WHERE "Id" = @p3;
      SELECT changes();
info: 12/28/2020 11:44:37.840 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command)
      Executed DbCommand (0ms) [Parameters=[@p3='2' (DbType = String), @p0='1' (DbType = String), @p1='F# 5 is the latest version of F#, the functional programming language...' (Size = 72), @p2='Announcing F# 5' (Size = 15)], CommandType='Text', CommandTimeout='30']
      UPDATE "Posts" SET "BlogId" = @p0, "Content" = @p1, "Title" = @p2
      WHERE "Id" = @p3;
      SELECT changes();
info: 12/28/2020 11:44:37.840 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command)
      Executed DbCommand (0ms) [Parameters=[@p0='1' (DbType = String), @p1='.NET 5.0 includes many enhancements, including single file applications, more...' (Size = 80), @p2='Announcing .NET 5.0' (Size = 19)], CommandType='Text', CommandTimeout='30']
      INSERT INTO "Posts" ("BlogId", "Content", "Title")
      VALUES (@p0, @p1, @p2);
      SELECT "Id"
      FROM "Posts"
      WHERE changes() = 1 AND "rowid" = last_insert_rowid();
```

This is a very easy way to generate updates and inserts from a disconnected graph. However, it results in updates or inserts being sent to the database for every property of every tracked entity, even when some property values may not have been changed. Don't be too scared by this; for many applications with small graphs, this can be an easy and pragmatic way of generating updates. That being said, other more complex patterns can sometimes result in more efficient updates, as described in [Identity Resolution in EF Core](xref:core/change-tracking/identity-resolution).

### Deleting existing entities

For an entity to be deleted by SaveChanges it must be tracked in the `Deleted` state, as described earlier. Entities are typically put in the `Deleted` state by calling one of <xref:Microsoft.EntityFrameworkCore.DbContext.Remove%2A?displayProperty=nameWithType>, <xref:Microsoft.EntityFrameworkCore.DbContext.RemoveRange%2A?displayProperty=nameWithType>, or the equivalent methods <xref:Microsoft.EntityFrameworkCore.DbSet%601>. For example, to mark an existing post as `Deleted`:

<!--
            context.Remove(
                new Post
                {
                    Id = 2
                });
-->
[!code-csharp[Deleting_existing_entities_1(](../../../samples/core/ChangeTracking/ChangeTrackingInEFCore/ExplicitKeysSamples.cs?name=Deleting_existing_entities_1()]

> [!NOTE]
> The examples here are creating entities explicitly with `new` for simplicity. Normally the entity instances will have come from another source, such as being deserialized from a client, or being created from data in an HTTP Post.

Inspecting the [change tracker debug view](xref:core/change-tracking/debug-views) following this call shows that the context is tracking the entity in the `Deleted` state:

```output
Post {Id: 2} Deleted
  Id: 2 PK
  BlogId: <null> FK
  Content: <null>
  Title: <null>
  Blog: <null>
```

This entity will be deleted when SaveChanges is called. For example, when using SQLite:

```sql
info: 12/28/2020 12:05:20.313 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command)
      Executed DbCommand (0ms) [Parameters=[@p0='2' (DbType = String)], CommandType='Text', CommandTimeout='30']
      DELETE FROM "Posts"
      WHERE "Id" = @p0;
      SELECT changes();
```

After SaveChanges completes, the deleted entity is detached from the DbContext since it no longer exists in the database. The debug view is therefore empty because no entities are being tracked.

### Deleting dependent/child entities

Deleting dependent/child entities from a graph is more straightforward than deleting principal/parent entities. See the next section and [Changing Foreign Keys and Navigations](xref:core/change-tracking/relationship-changes) for more information.

As mentioned above, it is unusual to call `Remove` on an entity created with `new`. Further, unlike `Add`, `Attach` and `Update`, it is uncommon to call `Remove` on an entity that isn't already tracked in the `Unchanged` or `Modified` state. Instead it is typical to track a single entity or graph of related entities, and then call `Remove` on the entities that should be deleted. This graph of tracked entities is typically created by either:

1. Running a query for the entities
2. Or using the `Attach` or `Update` methods on a graph of disconnected entities, as described in the preceding sections.

For example, the code in the previous section is more likely obtain a post from a client and then do something like this:

<!--
            context.Attach(post);
            context.Remove(post);
-->
[!code-csharp[Deleting_dependent_child_entities_1(](../../../samples/core/ChangeTracking/ChangeTrackingInEFCore/ExplicitKeysSamples.cs?name=Deleting_dependent_child_entities_1()]

This behaves exactly the same way as the previous example, since calling `Remove` on an un-tracked entity causes it to first be attached and then marked as `Deleted`.

In more realistic examples, a graph of entities is first attached, and then some of those entities are marked as deleted. For example:

<!--
            // Attach a blog and associated posts
            context.Attach(blog);
            
            // Mark one post as Deleted
            context.Remove(blog.Posts[1]);
-->
[!code-csharp[Deleting_dependent_child_entities_2(](../../../samples/core/ChangeTracking/ChangeTrackingInEFCore/ExplicitKeysSamples.cs?name=Deleting_dependent_child_entities_2()]

All entities are marked as `Unchanged`, except the one on which `Remove` was called:

```output
Blog {Id: 1} Unchanged
  Id: 1 PK
  Name: '.NET Blog'
  Posts: [{Id: 1}, {Id: 2}]
Post {Id: 1} Unchanged
  Id: 1 PK
  BlogId: 1 FK
  Content: 'Announcing the release of EF Core 5.0, a full featured cross...'
  Title: 'Announcing the Release of EF Core 5.0'
  Blog: {Id: 1}
Post {Id: 2} Deleted
  Id: 2 PK
  BlogId: 1 FK
  Content: 'F# 5 is the latest version of F#, the functional programming...'
  Title: 'Announcing F# 5'
  Blog: {Id: 1}
```

This entity will be deleted when SaveChanges is called. For example, when using SQLite:

```sql
info: 12/28/2020 12:27:10.873 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command)
      Executed DbCommand (0ms) [Parameters=[@p0='2' (DbType = String)], CommandType='Text', CommandTimeout='30']
      DELETE FROM "Posts"
      WHERE "Id" = @p0;
      SELECT changes();
```

After SaveChanges completes, the deleted entity is detached from the DbContext since it no longer exists in the database. Other entities remain in the `Unchanged` state:

```output
Blog {Id: 1} Unchanged
  Id: 1 PK
  Name: '.NET Blog'
  Posts: [{Id: 1}]
Post {Id: 1} Unchanged
  Id: 1 PK
  BlogId: 1 FK
  Content: 'Announcing the release of EF Core 5.0, a full featured cross...'
  Title: 'Announcing the Release of EF Core 5.0'
  Blog: {Id: 1}
```

### Deleting principal/parent entities

Each relationship that connects two entity types has a principal or parent end, and a dependent or child end. For the purpose of this discussion, the dependent/child entity is the one with the foreign key property. In a one-to-many relationship, the principal/parent is on the "one" side, and the dependent/child is on the "many" side. See [Relationships](xref:core/modeling/relationships) for more information.

In the preceding examples we were deleting a post, which is a dependent/child entity in the blog-posts one-to-many relationship. This is relatively straightforward since removal of a dependent/child entity does not have any impact on other entities. On the other hand, deleting a principal/parent entity must also impact any dependent/child entities. Not doing so would leave a foreign key value referencing a primary key value that no longer exists. This is an invalid model state and results in a referential constraint error in most databases.

This invalid model state can be handled in two ways:

1. Setting FK values to null. This indicates that the dependents/children are no longer related to any principal/parent. This is the default for optional relationships where the foreign key must be nullable. Setting the FK to null is not valid for required relationships, where the foreign key is typically non-nullable.
2. Deleting the the dependents/children. This is the default for required relationships, and is also valid for optional relationships.

See [Changing Foreign Keys and Navigations](xref:core/change-tracking/relationship-changes) for detailed information on change tracking and relationships.

#### Optional relationships

The `Post.BlogId` foreign key property is nullable in the model we have been using. This means the relationship is optional, and hence the default behavior of EF Core is to set `BlogId` foreign key properties to null when the blog is deleted. For example:

<!--
            // Attach a blog and associated posts
            context.Attach(blog);
            
            // Mark one post as Deleted
            context.Remove(blog.Posts[1]);
-->
[!code-csharp[Deleting_principal_parent_entities_1(](../../../samples/core/ChangeTracking/ChangeTrackingInEFCore/ExplicitKeysSamples.cs?name=Deleting_principal_parent_entities_1()]

Inspecting the [change tracker debug view](xref:core/change-tracking/debug-views) following the call to `Remove` shows that, as expected, the blog is now marked as `Deleted`:

```output
Blog {Id: 1} Deleted
  Id: 1 PK
  Name: '.NET Blog'
  Posts: [{Id: 1}, {Id: 2}]
Post {Id: 1} Modified
  Id: 1 PK
  BlogId: <null> FK Modified Originally 1
  Content: 'Announcing the release of EF Core 5.0, a full featured cross...'
  Title: 'Announcing the Release of EF Core 5.0'
  Blog: <null>
Post {Id: 2} Modified
  Id: 2 PK
  BlogId: <null> FK Modified Originally 1
  Content: 'F# 5 is the latest version of F#, the functional programming...'
  Title: 'Announcing F# 5'
  Blog: <null>
```

More interestingly, all the related posts are now marked as `Modified`. This is because the foreign key property in each entity has been set to null. Calling SaveChanges updates the foreign key value for each post to null in the database, before then deleting the blog:

```sql
info: 12/28/2020 12:51:15.011 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command)
      Executed DbCommand (0ms) [Parameters=[@p1='1' (DbType = String), @p0=NULL], CommandType='Text', CommandTimeout='30']
      UPDATE "Posts" SET "BlogId" = @p0
      WHERE "Id" = @p1;
      SELECT changes();
info: 12/28/2020 12:51:15.011 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command)
      Executed DbCommand (0ms) [Parameters=[@p1='2' (DbType = String), @p0=NULL], CommandType='Text', CommandTimeout='30']
      UPDATE "Posts" SET "BlogId" = @p0
      WHERE "Id" = @p1;
      SELECT changes();
info: 12/28/2020 12:51:15.012 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command)
      Executed DbCommand (0ms) [Parameters=[@p2='1' (DbType = String)], CommandType='Text', CommandTimeout='30']
      DELETE FROM "Blogs"
      WHERE "Id" = @p2;
      SELECT changes();
```

After SaveChanges completes, the deleted entity is detached from the DbContext since it no longer exists in the database. Other entities are now marked as `Unchanged` with null foreign key values, which matches the state of the database:

```output
Post {Id: 1} Unchanged
  Id: 1 PK
  BlogId: <null> FK
  Content: 'Announcing the release of EF Core 5.0, a full featured cross...'
  Title: 'Announcing the Release of EF Core 5.0'
  Blog: <null>
Post {Id: 2} Unchanged
  Id: 2 PK
  BlogId: <null> FK
  Content: 'F# 5 is the latest version of F#, the functional programming...'
  Title: 'Announcing F# 5'
  Blog: <null>
```

#### Required relationships

If the `Post.BlogId` foreign key property is non-nullable, then the relationship between blogs and posts becomes "required". In this situation, EF Core will, by default, delete dependent/child entities when the principal/parent is deleted. For example, deleting a blog with related posts as in the previous example:

<!--
            // Attach a blog and associated posts
            context.Attach(blog);
            
            // Mark one post as Deleted
            context.Remove(blog.Posts[1]);
-->
[!code-csharp[Deleting_principal_parent_entities_1(](../../../samples/core/ChangeTracking/ChangeTrackingInEFCore/ExplicitKeysRequiredSamples.cs?name=Deleting_principal_parent_entities_1()]

Inspecting the [change tracker debug view](xref:core/change-tracking/debug-views) following the call to `Remove` shows that, as expected, the blog is again marked as `Deleted`:

```output
Blog {Id: 1} Deleted
  Id: 1 PK
  Name: '.NET Blog'
  Posts: [{Id: 1}, {Id: 2}]
Post {Id: 1} Deleted
  Id: 1 PK
  BlogId: 1 FK
  Content: 'Announcing the release of EF Core 5.0, a full featured cross...'
  Title: 'Announcing the Release of EF Core 5.0'
  Blog: {Id: 1}
Post {Id: 2} Deleted
  Id: 2 PK
  BlogId: 1 FK
  Content: 'F# 5 is the latest version of F#, the functional programming...'
  Title: 'Announcing F# 5'
  Blog: {Id: 1}
```

More interestingly in this case is that all related posts have also been marked as `Deleted`. Calling SaveChanges causes the blog and all related posts to be deleted from the database:

```sql
info: 12/28/2020 12:58:04.111 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command)
      Executed DbCommand (0ms) [Parameters=[@p0='1' (DbType = String)], CommandType='Text', CommandTimeout='30']
      DELETE FROM "Posts"
      WHERE "Id" = @p0;
      SELECT changes();
info: 12/28/2020 12:58:04.112 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command)
      Executed DbCommand (0ms) [Parameters=[@p0='2' (DbType = String)], CommandType='Text', CommandTimeout='30']
      DELETE FROM "Posts"
      WHERE "Id" = @p0;
      SELECT changes();
info: 12/28/2020 12:58:04.112 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command)
      Executed DbCommand (0ms) [Parameters=[@p1='1' (DbType = String)], CommandType='Text', CommandTimeout='30']
      DELETE FROM "Blogs"
      WHERE "Id" = @p1;
```

After SaveChanges completes, all the deleted entities are detached from the DbContext since they no longer exist in the database. Output from the debug view is therefore empty.

> [!NOTE]
> This document only scratches the surface on working with relationships in EF Core. See
See [Relationships](xref:core/modeling/relationships) for more information on modeling relationships, and [Changing Foreign Keys and Navigations](xref:core/change-tracking/relationship-changes) for more information on updating/deleting dependent/child entities when calling SaveChanges.  
