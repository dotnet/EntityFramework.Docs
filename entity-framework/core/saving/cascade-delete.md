---
title: Cascade Delete - EF Core
description: Configuring cascading behaviors triggered when an entity is deleted or severed from its principal/parent
author: SamMonoRT
ms.date: 08/10/2021
uid: core/saving/cascade-delete
---
# Cascade Delete

Entity Framework Core (EF Core) represents relationships using foreign keys. An entity with a foreign key is the child or dependent entity in the relationship. This entity's foreign key value must match the primary key value (or an alternate key value) of the related principal/parent entity.

If the principal/parent entity is deleted, then the foreign key values of the dependents/children will no longer match the primary or alternate key of _any_ principal/parent. This is an invalid state, and will cause a referential constraint violation in most databases.

There are two options to avoid this referential constraint violation:

1. Set the FK values to null
2. Also delete the dependent/child entities

The first option is only valid for optional relationships where the foreign key property (and the database column to which it is mapped) must be nullable.

The second option is valid for any kind of relationship and is known as "cascade delete".

> [!TIP]
> This document describes cascade deletes (and deleting orphans) from the perspective of updating the database. It makes heavy use of concepts introduced in [Change Tracking in EF Core](xref:core/change-tracking/index) and [Changing Foreign Keys and Navigations](xref:core/change-tracking/relationship-changes). Make sure to fully understand these concepts before tackling the material here.

> [!TIP]  
> You can run and debug into all the code in this document by [downloading the sample code from GitHub](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/CascadeDeletes).

## When cascading behaviors happen

Cascading deletes are needed when a dependent/child entity can no longer be associated with its current principal/parent. This can happen because the principal/parent is deleted, or it can happen when the principal/parent still exists but the dependent/child is no longer associated with it.

### Deleting a principal/parent

Consider this simple model where `Blog` is the principal/parent in a relationship with `Post`, which is the dependent/child. `Post.BlogId` is a foreign key property, the value of which must match the `Blog.Id` primary key of the blog to which the post belongs.

<!--
    public class Blog
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IList<Post> Posts { get; } = new List<Post>();
    }

    public class Post
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }
-->
[!code-csharp[Model](../../../samples/core/CascadeDeletes/IntroRequiredSamples.cs?name=Model)]

By convention, this relationship is configured as a required, since the `Post.BlogId` foreign key property is non-nullable. Required relationships are configured to use cascade deletes by default. See [Relationships](xref:core/modeling/relationships) for more information on modeling relationships.

When deleting a blog, all posts are cascade deleted. For example:

<!--
            using var context = new BlogsContext();

            var blog = context.Blogs.OrderBy(e => e.Name).Include(e => e.Posts).First();

            context.Remove(blog);
            
            context.SaveChanges();
-->
[!code-csharp[Deleting_principal_parent_1](../../../samples/core/CascadeDeletes/IntroRequiredSamples.cs?name=Deleting_principal_parent_1)]

SaveChanges generates the following SQL, using SQL Server as an example:

```sql
-- Executed DbCommand (1ms) [Parameters=[@p0='1'], CommandType='Text', CommandTimeout='30']
SET NOCOUNT ON;
DELETE FROM [Posts]
WHERE [Id] = @p0;
SELECT @@ROWCOUNT;

-- Executed DbCommand (0ms) [Parameters=[@p0='2'], CommandType='Text', CommandTimeout='30']
SET NOCOUNT ON;
DELETE FROM [Posts]
WHERE [Id] = @p0;
SELECT @@ROWCOUNT;

-- Executed DbCommand (2ms) [Parameters=[@p1='1'], CommandType='Text', CommandTimeout='30']
SET NOCOUNT ON;
DELETE FROM [Blogs]
WHERE [Id] = @p1;
SELECT @@ROWCOUNT;
```

### Severing a relationship

Rather than deleting the blog, we could instead sever the relationship between each post and its blog. This can be done by setting the reference navigation `Post.Blog` to null for each post:

<!--
            using var context = new BlogsContext();

            var blog = context.Blogs.OrderBy(e => e.Name).Include(e => e.Posts).First();

            foreach (var post in blog.Posts)
            {
                post.Blog = null;
            }
            
            context.SaveChanges();
-->
[!code-csharp[Severing_a_relationship_1](../../../samples/core/CascadeDeletes/IntroRequiredSamples.cs?name=Severing_a_relationship_1)]

The relationship can also be severed by removing each post from the `Blog.Posts` collection navigation:

<!--
            using var context = new BlogsContext();

            var blog = context.Blogs.OrderBy(e => e.Name).Include(e => e.Posts).First();

            blog.Posts.Clear();
            
            context.SaveChanges();
-->
[!code-csharp[Severing_a_relationship_2](../../../samples/core/CascadeDeletes/IntroRequiredSamples.cs?name=Severing_a_relationship_2)]

In either case the result is the same: the blog is not deleted, but the posts that are no longer associated with any blog are deleted:

```sql
-- Executed DbCommand (1ms) [Parameters=[@p0='1'], CommandType='Text', CommandTimeout='30']
SET NOCOUNT ON;
DELETE FROM [Posts]
WHERE [Id] = @p0;
SELECT @@ROWCOUNT;

-- Executed DbCommand (0ms) [Parameters=[@p0='2'], CommandType='Text', CommandTimeout='30']
SET NOCOUNT ON;
DELETE FROM [Posts]
WHERE [Id] = @p0;
SELECT @@ROWCOUNT;
```

Deleting entities that are no longer associated with any principal/dependent is known as "deleting orphans".

> [!TIP]
> Cascade delete and deleting orphans are closely related. Both result in deleting dependent/child entities when the relationship to their required principal/parent is severed. For cascade delete, this severing happens because the principal/parent is itself deleted. For orphans, the principal/parent entity still exists, but is no longer related to the dependent/child entities.  

## Where cascading behaviors happen

Cascading behaviors can be applied to:

- Entities tracked by the current <xref:Microsoft.EntityFrameworkCore.DbContext>
- Entities in the database that have not been loaded into the context

### Cascade delete of tracked entities

EF Core always applies configured cascading behaviors to tracked entities. This means that if the application loads all relevant dependent/child entities into the DbContext, as is shown in the examples above, then cascading behaviors will be correctly applied regardless of how the database is configured.

> [!TIP]
> The exact timing of when cascading behaviors happen to tracked entities can be controlled using <xref:Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.CascadeDeleteTiming?displayProperty=nameWithType> and <xref:Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.DeleteOrphansTiming?displayProperty=nameWithType>. See [Changing Foreign Keys and Navigations](xref:core/change-tracking/relationship-changes) for more information.

### Cascade delete in the database

Many database systems also offer cascading behaviors that are triggered when an entity is deleted in the database. EF Core configures these behaviors based on the cascade delete behavior in the EF Core model when a database is created using <xref:Microsoft.EntityFrameworkCore.Infrastructure.DatabaseFacade.EnsureCreated*> or EF Core migrations. For example, using the model above, the following table is created for posts when using SQL Server:

```sql
CREATE TABLE [Posts] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(max) NULL,
    [Content] nvarchar(max) NULL,
    [BlogId] int NOT NULL,
    CONSTRAINT [PK_Posts] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Posts_Blogs_BlogId] FOREIGN KEY ([BlogId]) REFERENCES [Blogs] ([Id]) ON DELETE CASCADE
);
```

Notice that the foreign key constraint defining the relationship between blogs and posts is configured with `ON DELETE CASCADE`.

If we know that the database is configured like this, then we can delete a blog _without first loading posts_ and the database will take care of deleting all the posts that were related to that blog. For example:

<!--
            using var context = new BlogsContext();

            var blog = context.Blogs.OrderBy(e => e.Name).First();

            context.Remove(blog);
            
            context.SaveChanges();
-->
[!code-csharp[Where_cascading_behaviors_happen_1](../../../samples/core/CascadeDeletes/IntroRequiredSamples.cs?name=Where_cascading_behaviors_happen_1)]

Notice that there is no `Include` for posts, so they are not loaded. SaveChanges in this case will delete just the blog, since that's the only entity being tracked:

```sql
-- Executed DbCommand (6ms) [Parameters=[@p0='1'], CommandType='Text', CommandTimeout='30']
SET NOCOUNT ON;
DELETE FROM [Blogs]
WHERE [Id] = @p0;
SELECT @@ROWCOUNT;
```

This would result in an exception if the foreign key constraint in the database is not configured for cascade deletes. However, in this case the posts are deleted by the database because it has been configured with `ON DELETE CASCADE` when it was created.

> [!NOTE]
> Databases don't typically have any way to automatically delete orphans. This is because while EF Core represents relationships using navigations as well of foreign keys, databases have only foreign keys and no navigations. This means that it is usually not possible to sever a relationship without loading both sides into the DbContext.

> [!NOTE]
> The EF Core in-memory database does not currently support cascade deletes in the database.

> [!WARNING]
> Do not configure cascade delete in the database when soft-deleting entities. This may cause entities to be accidentally really deleted instead of soft-deleted.

### Database cascade limitations

Some databases, most notably SQL Server, have limitations on the cascade behaviors that form cycles. For example, consider the following model:

<!--
    public class Blog
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public IList<Post> Posts { get; } = new List<Post>();
        
        public int OwnerId { get; set; }
        public Person Owner { get; set; }
    }

    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set; }
        
        public int AuthorId { get; set; }
        public Person Author { get; set; }
    }

    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public IList<Post> Posts { get; } = new List<Post>();

        public Blog OwnedBlog { get; set; }
    }
-->
[!code-csharp[Model](../../../samples/core/CascadeDeletes/WithDatabaseCycleSamples.cs?name=Model)]

This model has three relationships, all required and therefore configured to cascade delete by convention:

- Deleting a blog will cascade delete all the related posts
- Deleting the author of posts will cause the authored posts to be cascade deleted
- Deleting the owner of a blog will cause the blog to be cascade deleted

This is all reasonable (if a bit draconian in blog management policies!) but attempting to create a SQL Server database with these cascades configured results in the following exception:

> Microsoft.Data.SqlClient.SqlException (0x80131904): Introducing FOREIGN KEY constraint 'FK_Posts_Person_AuthorId' on table 'Posts' may cause cycles or multiple cascade paths. Specify ON DELETE NO ACTION or ON UPDATE NO ACTION, or modify other FOREIGN KEY constraints.

There are two ways to handle this situation:

1. Change one or more of the relationships to not cascade delete.
2. Configure the database without one or more of these cascade deletes, then ensure all dependent entities are loaded so that EF Core can perform the cascading behavior.

Taking the first approach with our example, we could make the post-blog relationship optional by giving it a nullable foreign key property:

<!--
            public int? BlogId { get; set; }
-->
[!code-csharp[NullableBlogId](../../../samples/core/CascadeDeletes/OptionalDependentsSamples.cs?name=NullableBlogId)]

An optional relationship allows the post to exist without a blog, which means cascade delete will no longer be configured by default. This means there is no longer a cycle in cascading actions, and the database can be created without error on SQL Server.

Taking the second approach instead, we can keep the blog-owner relationship required and configured for cascade delete, but make this configuration only apply to tracked entities, not the database:

<!--
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Blog>()
                .HasOne(e => e.Owner)
                .WithOne(e => e.OwnedBlog)
                .OnDelete(DeleteBehavior.ClientCascade);
        }
-->
[!code-csharp[OnModelCreating](../../../samples/core/CascadeDeletes/WithDatabaseCycleSamples.cs?name=OnModelCreating)]

Now what happens if we load both a person and the blog they own, then delete the person?

<!--
            using var context = new BlogsContext();

            var owner = context.People.Single(e => e.Name == "ajcvickers");
            var blog = context.Blogs.Single(e => e.Owner == owner);

            context.Remove(owner);
            
            context.SaveChanges();
-->
[!code-csharp[Database_cascade_limitations_1](../../../samples/core/CascadeDeletes/WithDatabaseCycleSamples.cs?name=Database_cascade_limitations_1)]

EF Core will cascade the delete of the owner so that the blog is also deleted:

```sql
-- Executed DbCommand (8ms) [Parameters=[@p0='1'], CommandType='Text', CommandTimeout='30']
SET NOCOUNT ON;
DELETE FROM [Blogs]
WHERE [Id] = @p0;
SELECT @@ROWCOUNT;

-- Executed DbCommand (2ms) [Parameters=[@p1='1'], CommandType='Text', CommandTimeout='30']
SET NOCOUNT ON;
DELETE FROM [People]
WHERE [Id] = @p1;
SELECT @@ROWCOUNT;
```

However, if the blog is not loaded when the owner is deleted:

<!--
                using var context = new BlogsContext();

                var owner = context.People.Single(e => e.Name == "ajcvickers");

                context.Remove(owner);
            
                context.SaveChanges();
-->
[!code-csharp[Database_cascade_limitations_2](../../../samples/core/CascadeDeletes/WithDatabaseCycleSamples.cs?name=Database_cascade_limitations_2)]

Then an exception will be thrown due to violation of the foreign key constraint in the database:

> Microsoft.Data.SqlClient.SqlException: The DELETE statement conflicted with the REFERENCE constraint "FK_Blogs_People_OwnerId". The conflict occurred in database "Scratch", table "dbo.Blogs", column 'OwnerId'.
The statement has been terminated.

## Cascading nulls

Optional relationships have nullable foreign key properties mapped to nullable database columns. This means that the foreign key value can be set to null when the current principal/parent is deleted or is severed from the dependent/child.

Let's look again at the examples from [When cascading behaviors happen](#when-cascading-behaviors-happen), but this time with an optional relationship represented by a nullable `Post.BlogId` foreign key property:

<!--
            public int? BlogId { get; set; }
-->
[!code-csharp[NullableBlogId](../../../samples/core/CascadeDeletes/OptionalDependentsSamples.cs?name=NullableBlogId)]

This foreign key property will be set to null for each post when its related blog is deleted. For example, this code, which is the same as before:

<!--
            using var context = new BlogsContext();

            var blog = context.Blogs.OrderBy(e => e.Name).Include(e => e.Posts).First();

            context.Remove(blog);
            
            context.SaveChanges();
-->
[!code-csharp[Deleting_principal_parent_1b](../../../samples/core/CascadeDeletes/IntroOptionalSamples.cs?name=Deleting_principal_parent_1b)]

Will now result in the following database updates when SaveChanges is called:

```sql
-- Executed DbCommand (2ms) [Parameters=[@p1='1', @p0=NULL (DbType = Int32)], CommandType='Text', CommandTimeout='30']
SET NOCOUNT ON;
UPDATE [Posts] SET [BlogId] = @p0
WHERE [Id] = @p1;
SELECT @@ROWCOUNT;

-- Executed DbCommand (0ms) [Parameters=[@p1='2', @p0=NULL (DbType = Int32)], CommandType='Text', CommandTimeout='30']
SET NOCOUNT ON;
UPDATE [Posts] SET [BlogId] = @p0
WHERE [Id] = @p1;
SELECT @@ROWCOUNT;

-- Executed DbCommand (1ms) [Parameters=[@p2='1'], CommandType='Text', CommandTimeout='30']
SET NOCOUNT ON;
DELETE FROM [Blogs]
WHERE [Id] = @p2;
SELECT @@ROWCOUNT;
```

Likewise, if the relationship is severed using either of the examples from above:

<!--
            using var context = new BlogsContext();

            var blog = context.Blogs.OrderBy(e => e.Name).Include(e => e.Posts).First();

            foreach (var post in blog.Posts)
            {
                post.Blog = null;
            }
            
            context.SaveChanges();
-->
[!code-csharp[Severing_a_relationship_1b](../../../samples/core/CascadeDeletes/IntroOptionalSamples.cs?name=Severing_a_relationship_1b)]

Or:

<!--
            using var context = new BlogsContext();

            var blog = context.Blogs.OrderBy(e => e.Name).Include(e => e.Posts).First();

            blog.Posts.Clear();
            
            context.SaveChanges();
-->
[!code-csharp[Severing_a_relationship_2b](../../../samples/core/CascadeDeletes/IntroOptionalSamples.cs?name=Severing_a_relationship_2b)]

Then the posts are updated with null foreign key values when SaveChanges is called:

```sql
-- Executed DbCommand (2ms) [Parameters=[@p1='1', @p0=NULL (DbType = Int32)], CommandType='Text', CommandTimeout='30']
SET NOCOUNT ON;
UPDATE [Posts] SET [BlogId] = @p0
WHERE [Id] = @p1;
SELECT @@ROWCOUNT;

-- Executed DbCommand (0ms) [Parameters=[@p1='2', @p0=NULL (DbType = Int32)], CommandType='Text', CommandTimeout='30']
SET NOCOUNT ON;
UPDATE [Posts] SET [BlogId] = @p0
WHERE [Id] = @p1;
SELECT @@ROWCOUNT;
```

See [Changing Foreign Keys and Navigations](xref:core/change-tracking/relationship-changes) for more information on how EF Core manages foreign keys and navigations as their values are changed.

> [!NOTE]
> The fixup of relationships like this has been the default behavior of Entity Framework since the first version in 2008. Prior to EF Core it didn't have a name and was not possible to change. It is now known as `ClientSetNull` as described in the next section.

Databases can also be configured to cascade nulls like this when a principal/parent in an optional relationship is deleted. However, this is much less common than using cascading deletes in the database. Using cascading deletes and cascading nulls in the database at the same time will almost always result in relationship cycles when using SQL Server. See the next section for more information on configuring cascading nulls.

## Configuring cascading behaviors

> [!TIP]
> Be sure to read sections above before coming here. The configuration options will likely not make sense if the preceding material is not understood.

Cascade behaviors are configured per relationship using the <xref:Microsoft.EntityFrameworkCore.Metadata.Builders.ReferenceCollectionBuilder.OnDelete*> method in <xref:Microsoft.EntityFrameworkCore.DbContext.OnModelCreating*>. For example:

<!--
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Blog>()
                .HasOne(e => e.Owner)
                .WithOne(e => e.OwnedBlog)
                .OnDelete(DeleteBehavior.ClientCascade);
        }
-->
[!code-csharp[OnModelCreating](../../../samples/core/CascadeDeletes/WithDatabaseCycleSamples.cs?name=OnModelCreating)]

See [Relationships](xref:core/modeling/relationships) for more information on configuring relationships between entity types.

`OnDelete` accepts a value from the, admittedly confusing, <xref:Microsoft.EntityFrameworkCore.DeleteBehavior> enum. This enum defines both the behavior of EF Core on tracked entities, and the configuration of cascade delete in the database when EF is used to create the schema.

### Impact on database schema

The following table shows the result of each `OnDelete` value on the foreign key constraint created by EF Core migrations or <xref:Microsoft.EntityFrameworkCore.Infrastructure.DatabaseFacade.EnsureCreated*>.

| DeleteBehavior        | Impact on database schema
|:----------------------|--------------------------
| Cascade               | ON DELETE CASCADE
| Restrict              | ON DELETE RESTRICT
| NoAction              | database default
| SetNull               | ON DELETE SET NULL
| ClientSetNull         | database default
| ClientCascade         | database default
| ClientNoAction        | database default

The behaviors of `ON DELETE NO ACTION` (the database default) and `ON DELETE RESTRICT` in relational databases are typically either identical or very similar. Despite what `NO ACTION` may imply, both of these options cause referential constraints to be enforced. The difference, when there is one, is _when_ the database checks the constraints.  Check your database documentation for the specific differences between `ON DELETE NO ACTION` and `ON DELETE RESTRICT` on your database system.

SQL Server doesn't support `ON DELETE RESTRICT`, so `ON DELETE NO ACTION` is used instead.

The only values that will cause cascading behaviors on the database are `Cascade` and `SetNull`. All other values will configure the database to not cascade any changes.

### Impact on SaveChanges behavior

The tables in the following sections cover what happens to dependent/child entities when the principal/parent is deleted, or its relationship to the dependent/child entities is severed. Each table covers one of:

- Optional (nullable FK) and required (non-nullable FK) relationships
- When dependents/children are loaded and tracked by the DbContext and when they exist only in the database

#### Required relationship with dependents/children loaded

| DeleteBehavior    | On deleting principal/parent             | On severing from principal/parent
|:------------------|------------------------------------------|----------------------------------------
| Cascade           | Dependents deleted by EF Core            | Dependents deleted by EF Core
| Restrict          | `InvalidOperationException`              | `InvalidOperationException`
| NoAction          | `InvalidOperationException`              | `InvalidOperationException`
| SetNull           | `SqlException` on creating database      | `SqlException` on creating database
| ClientSetNull     | `InvalidOperationException`              | `InvalidOperationException`
| ClientCascade     | Dependents deleted by EF Core            | Dependents deleted by EF Core
| ClientNoAction    | `DbUpdateException`                      | `InvalidOperationException`

Notes:

- The default for required relationships like this is `Cascade`.
- Using anything other than cascade delete for required relationships will result in an exception when SaveChanges is called.
  - Typically, this is an `InvalidOperationException` from EF Core since the invalid state is detected in the loaded children/dependents.
  - `ClientNoAction` forces EF Core to not check fixup dependents before sending them to the database, so in this case the database throws an exception, which is then wrapped in a `DbUpdateException` by SaveChanges.
  - `SetNull` is rejected when creating the database since the foreign key column is not nullable.
- Since dependents/children are loaded, they are always deleted by EF Core, and never left for the database to delete.

#### Required relationship with dependents/children not loaded

| DeleteBehavior    | On deleting principal/parent             | On severing from principal/parent
|:------------------|------------------------------------------|----------------------------------------
| Cascade           | Dependents deleted by database           | N/A
| Restrict          | `DbUpdateException`                      | N/A
| NoAction          | `DbUpdateException`                      | N/A
| SetNull           | `SqlException` on creating database      | N/A
| ClientSetNull     | `DbUpdateException`                      | N/A
| ClientCascade     | `DbUpdateException`                      | N/A
| ClientNoAction    | `DbUpdateException`                      | N/A

Notes:

- Severing a relationship is not valid here since the dependents/children are not loaded.
- The default for required relationships like this is `Cascade`.
- Using anything other than cascade delete for required relationships will result in an exception when SaveChanges is called.
  - Typically, this is a `DbUpdateException` because the dependents/children are not loaded, and hence the invalid state can only be detected by the database. SaveChanges then wraps the database exception in a `DbUpdateException`.
  - `SetNull` is rejected when creating the database since the foreign key column is not nullable.

#### Optional relationship with dependents/children loaded

| DeleteBehavior    | On deleting principal/parent             | On severing from principal/parent
|:------------------|------------------------------------------|----------------------------------------
| Cascade           | Dependents deleted by EF Core            | Dependents deleted by EF Core
| Restrict          | Dependent FKs set to null by EF Core     | Dependent FKs set to null by EF Core
| NoAction          | Dependent FKs set to null by EF Core     | Dependent FKs set to null by EF Core
| SetNull           | Dependent FKs set to null by EF Core     | Dependent FKs set to null by EF Core
| ClientSetNull     | Dependent FKs set to null by EF Core     | Dependent FKs set to null by EF Core
| ClientCascade     | Dependents deleted by EF Core            | Dependents deleted by EF Core
| ClientNoAction    | `DbUpdateException`                      | Dependent FKs set to null by EF Core

Notes:

- The default for optional relationships like this is `ClientSetNull`.
- Dependents/children are never deleted unless `Cascade` or `ClientCascade` are configured.
- All other values cause the dependent FKs to be set to null by EF Core...
  - ...except `ClientNoAction` which tells EF Core not to touch the foreign keys of dependents/children when the principal/parent is deleted. The database therefore throws an exception, which is wrapped as a `DbUpdateException` by SaveChanges.

#### Optional relationship with dependents/children not loaded

| DeleteBehavior    | On deleting principal/parent             | On severing from principal/parent
|:------------------|------------------------------------------|----------------------------------------
| Cascade           | Dependents deleted by database           | N/A
| Restrict          | `DbUpdateException`                      | N/A
| NoAction          | `DbUpdateException`                      | N/A
| SetNull           | Dependent FKs set to null by database    | N/A
| ClientSetNull     | `DbUpdateException`                      | N/A
| ClientCascade     | `DbUpdateException`                      | N/A
| ClientNoAction    | `DbUpdateException`                      | N/A

Notes:

- Severing a relationship is not valid here since the dependents/children are not loaded.
- The default for optional relationships like this is `ClientSetNull`.
- Dependents/children must be loaded to avoid a database exception unless the database has been configured to cascade either deletes or nulls.
