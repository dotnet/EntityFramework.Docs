---
title: What's New in EF Core 7.0
description: Overview of new features in EF Core 7.0
author: ajcvickers
ms.date: 08/12/2022
uid: core/what-is-new/ef-core-7
---

# What's New in EF Core 7.0

EF Core 7.0 (EF7) is the next release after EF Core 6.0 and is scheduled for release in November 2022. See [_Plan for Entity Framework Core 7.0_](xref:core/what-is-new/ef-core-7.0/plan) for details and [_.NET Data Biweekly Updates (2022)_](https://github.com/dotnet/efcore/issues/27185) for progress on the plan.

EF7 is currently in preview. The latest release on NuGet is [EF Core 7.0 Preview 7](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/7.0.0-preview.7.22376.2).

EF7 is also available as [daily builds](https://github.com/dotnet/efcore/blob/main/docs/DailyBuilds.md) which contain all the latest EF7 features and API tweaks. The samples here make use of these daily builds.

> [!TIP]
> You can run and debug into the samples by [downloading the sample code from GitHub](https://github.com/dotnet/EntityFramework.Docs). Each section links to the source code specific to that section.

EF7 targets .NET 6, and so can be used with either [.NET 6 (LTS)](https://dotnet.microsoft.com/download/dotnet/6.0) or [.NET 7](https://dotnet.microsoft.com/download/dotnet/7.0).

## ExecuteUpdate and ExecuteDelete (Bulk updates)

By default, EF Core [tracks changes to entities](xref:core/change-tracking/index), and then [sends updates to the database](xref:core/saving/index) when one of the `SaveChanges` methods is called. Changes are only sent for properties and relationships that have actually changed. Also, the tracked entities remain in sync with the changes sent to the database. This mechanism is an efficient and convenient way to send general-purpose inserts, updates, and deletes to the database. These changes are also batched to reduce the number of database round-trips.

However, it is sometimes useful to execute update or delete commands on the database without involving the change tracker. EF7 enables this with the new `ExecuteUpdate` and `ExecuteDelete` methods. These methods are applied to a LINQ query and will update or delete entities in the database based on the results of that query. Many entities can be updated with a single command and the entities are not loaded into memory, which means this can result in more efficient updates and deletes.

However, keep in mind that:

- The specific changes to make must be specified explicitly; they are not automatically detected by EF Core.
- Any tracked entities will not be kept in sync.
- Additional commands may need to be sent in the correct order so as not to violate database constraints. For example deleting dependents before a principal can be deleted.

All of this means that the `ExecuteUpdate` and `ExecuteDelete` methods complement, rather than replace, the existing `SaveChanges` mechanism.

### Sample model

The examples below use a simple model with blogs, posts, tags, and authors:

<!--
public class Blog
{
    public Blog(string name)
    {
        Name = name;
    }

    public int Id { get; private set; }
    public string Name { get; set; }
    public List<Post> Posts { get; } = new();
}

public class Post
{
    public Post(string title, string content, DateTime publishedOn)
    {
        Title = title;
        Content = content;
        PublishedOn = publishedOn;
    }

    public int Id { get; private set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime PublishedOn { get; set; }
    public Blog Blog { get; set; } = null!;
    public List<Tag> Tags { get; } = new();
    public Author? Author { get; set; }
}

public class FeaturedPost : Post
{
    public FeaturedPost(string title, string content, DateTime publishedOn, string promoText)
        : base(title, content, publishedOn)
    {
        PromoText = promoText;
    }

    public string PromoText { get; set; }
}

public class Tag
{
    public Tag(string text)
    {
        Text = text;
    }

    public int Id { get; private set; }
    public string Text { get; set; }
    public List<Post> Posts { get; } = new();
}

public class Author
{
    public Author(string name)
    {
        Name = name;
    }

    public int Id { get; private set; }
    public string Name { get; set; }
    public List<Post> Posts { get; } = new();
}
-->
[!code-csharp[BlogsModel](../../../../samples/core/Miscellaneous/NewInEFCore7/BlogsContext.cs?name=BlogsModel)]

> [!TIP]
> The sample model can be found in [BlogsContext.cs](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Miscellaneous/NewInEFCore7/BlogsContext.cs).

### Basic `ExecuteDelete` examples

> [!TIP]
> The code shown here comes from [ExecuteDeleteSample.cs](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Miscellaneous/NewInEFCore7/ExecuteDeleteSample.cs).

Calling `ExecuteDelete` or `ExecuteDeleteAsync` on a `DbSet` immediately deletes all entities of that `DbSet` from the database. For example, to delete all `Tag` entities:

<!--
        await context.Tags.ExecuteDeleteAsync();
-->
[!code-csharp[DeleteAllTags](../../../../samples/core/Miscellaneous/NewInEFCore7/ExecuteDeleteSample.cs?name=DeleteAllTags)]

This executes the following SQL when using SQL Server:

```sql
DELETE FROM [t]
FROM [Tags] AS [t]
```

More interestingly, the query can contain a filter. For example:

<!--
        await context.Tags.Where(t => t.Text.Contains(".NET")).ExecuteDeleteAsync();
-->
[!code-csharp[DeleteTagsContainingDotNet](../../../../samples/core/Miscellaneous/NewInEFCore7/ExecuteDeleteSample.cs?name=DeleteTagsContainingDotNet)]

This executes the following SQL:

```sql
DELETE FROM [t]
FROM [Tags] AS [t]
WHERE [t].[Text] LIKE N'%.NET%'
```

The query can also use more complex filters, including navigations to other types. For example, to delete tags only from old blog posts:

<!--
        await context.Tags.Where(t => t.Posts.All(e => e.PublishedOn.Year < 2022)).ExecuteDeleteAsync();
-->
[!code-csharp[DeleteTagsFromOldPosts](../../../../samples/core/Miscellaneous/NewInEFCore7/ExecuteDeleteSample.cs?name=DeleteTagsFromOldPosts)]

Which executes:

```sql
DELETE FROM [t]
FROM [Tags] AS [t]
WHERE NOT EXISTS (
    SELECT 1
    FROM [PostTag] AS [p]
    INNER JOIN [Posts] AS [p0] ON [p].[PostsId] = [p0].[Id]
    WHERE [t].[Id] = [p].[TagsId] AND NOT (DATEPART(year, [p0].[PublishedOn]) < 2022))
```

### Basic `ExecuteUpdate` examples

> [!TIP]
> The code shown here comes from [ExecuteUpdateSample.cs](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Miscellaneous/NewInEFCore7/ExecuteUpdateSample.cs).

`ExecuteUpdate` and `ExecuteUpdateAsync` behave in a very similar way to the `ExecuteDelete` methods. The main difference is that an update requires knowing _which_ properties to update, and _how_ to update them. This is achieved using one or more calls to `SetProperty`. For example, to update the `Name` of every blog:

<!--
        await context.Blogs.ExecuteUpdateAsync(
            s => s.SetProperty(b => b.Name, b => b.Name + " *Featured!*"));
-->
[!code-csharp[UpdateAllBlogs](../../../../samples/core/Miscellaneous/NewInEFCore7/ExecuteUpdateSample.cs?name=UpdateAllBlogs)]

The first parameter of `SetProperty` specifies which property to update; in this case, `Blog.Name`. The second parameter specifies how the new value should be calculated; in this case, by taking the existing value and appending `"*Featured!*"`. The resulting SQL is:

```sql
UPDATE [b]
    SET [b].[Name] = [b].[Name] + N' *Featured!*'
FROM [Blogs] AS [b]
```

As with `ExecuteDelete`, the query can be used to filter which entities are updated. In addition, multiple calls to `SetProperty` can be used to update more than one property on the target entity. For example, to update the `Title` and `Content` of all posts published before 2022:

<!--
        await context.Posts
            .Where(p => p.PublishedOn.Year < 2022)
            .ExecuteUpdateAsync(
                s => s.SetProperty(b => b.Title, b => b.Title + " (" + b.PublishedOn.Year + ")")
                    .SetProperty(b => b.Content, b => b.Content + " ( This content was published in " + b.PublishedOn.Year + ")"));
-->
[!code-csharp[UpdateOldPosts](../../../../samples/core/Miscellaneous/NewInEFCore7/ExecuteUpdateSample.cs?name=UpdateOldPosts)]

In this case the generated SQL is a bit more complicated:

```sql
UPDATE [p]
    SET [p].[Content] = (([p].[Content] + N' ( This content was published in ') + COALESCE(CAST(DATEPART(year, [p].[PublishedOn]) AS nvarchar(max)), N'')) + N')',
    [p].[Title] = (([p].[Title] + N' (') + COALESCE(CAST(DATEPART(year, [p].[PublishedOn]) AS nvarchar(max)), N'')) + N')'
FROM [Posts] AS [p]
WHERE DATEPART(year, [p].[PublishedOn]) < 2022
```

Finally, again as with `ExecuteDelete`, the filter can reference other tables. For example, to update all tags from old posts:

<!--
        await context.Tags
            .Where(t => t.Posts.All(e => e.PublishedOn.Year < 2022))
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.Text, t => t.Text + " (old)"));
-->
[!code-csharp[UpdateTagsOnOldPosts](../../../../samples/core/Miscellaneous/NewInEFCore7/ExecuteUpdateSample.cs?name=UpdateTagsOnOldPosts)]

Which generates:

```sql
UPDATE [t]
    SET [t].[Text] = [t].[Text] + N' (old)'
FROM [Tags] AS [t]
WHERE NOT EXISTS (
    SELECT 1
    FROM [PostTag] AS [p]
    INNER JOIN [Posts] AS [p0] ON [p].[PostsId] = [p0].[Id]
    WHERE [t].[Id] = [p].[TagsId] AND NOT (DATEPART(year, [p0].[PublishedOn]) < 2022))
```

### Inheritance and multiple tables

`ExecuteUpdate` and `ExecuteDelete` can only act on a single table. This has implications when working with different [inheritance mapping strategies](xref:core/modeling/inheritance). Generally, there are no problems when using the TPH mapping strategy, since there is only one table to modify. For example, deleting all `FeaturedPost` entities:

<!--
        await context.Set<FeaturedPost>().ExecuteDeleteAsync();
-->
[!code-csharp[DeleteFeaturedPosts](../../../../samples/core/Miscellaneous/NewInEFCore7/ExecuteDeleteSample.cs?name=DeleteFeaturedPosts)]

Generates the following SQL when using TPH mapping:

```sql
DELETE FROM [p]
FROM [Posts] AS [p]
WHERE [p].[Discriminator] = N'FeaturedPost'
```

There are also no issues for this case when using the TPC mapping strategy, since again only changes to a single table are needed:

```sql
DELETE FROM [f]
FROM [FeaturedPosts] AS [f]
```

However, attempting this when using the TPT mapping strategy will fail since it would require deleting rows from two different tables.

Adding a filter to the query often means the operation will fail with both the TPC and TPT strategies. This is again because the rows may need to be deleted from multiple tables. For example, this query:

<!--
        await context.Posts.Where(p => p.Author.Name.StartsWith("Arthur")).ExecuteDeleteAsync();
-->
[!code-csharp[DeletePostsForGivenAuthor](../../../../samples/core/Miscellaneous/NewInEFCore7/ExecuteDeleteSample.cs?name=DeletePostsForGivenAuthor)]

Generates the following SQL when using TPH:

```sql
DELETE FROM [p]
FROM [Posts] AS [p]
LEFT JOIN [Authors] AS [a] ON [p].[AuthorId] = [a].[Id]
WHERE [a].[Name] IS NOT NULL AND ([a].[Name] LIKE N'Arthur%')
```

But fails when using TPC or TPT.

> [!TIP]
> [Issue #10879](https://github.com/dotnet/efcore/issues/28520) tracks adding support for automatically sending multiple commands in these scenarios. Vote for this issue if it's something you would like to see implemented.

### `ExecuteDelete` and relationships

As mentioned above, it may be necessary to delete or update dependent entities before the principal of a relationship can be deleted. For example, each `Post` is a dependent of its associated `Author`. This means that an author cannot be deleted if a post still references it; doing so will violate the foreign key constraint in the database. For example, attempting this:

```csharp
await context.Authors.ExecuteDeleteAsync();
```

Will result in the following exception on SQL Server:

> Microsoft.Data.SqlClient.SqlException (0x80131904): The DELETE statement conflicted with the REFERENCE constraint "FK_Posts_Authors_AuthorId". The conflict occurred in database "TphBlogsContext", table "dbo.Posts", column 'AuthorId'.
The statement has been terminated.

To fix this, we must first either delete the posts, or sever the relationship between each post and its author by setting `AuthorId` foreign key property to null. For example, using the delete option:

<!--
        await context.Posts.ExecuteDeleteAsync();
        await context.Authors.ExecuteDeleteAsync();
-->
[!code-csharp[DeleteAllAuthors](../../../../samples/core/Miscellaneous/NewInEFCore7/ExecuteDeleteSample.cs?name=DeleteAllAuthors)]

This results in two separate commands; the first to delete the dependents:

```sql
DELETE FROM [p]
FROM [Posts] AS [p]
```

And the second to delete the principals:

```sql
DELETE FROM [a]
FROM [Authors] AS [a]
```

> [!IMPORTANT]
> Multiple `ExecuteDelete` and `ExecuteUpdate` commands will not be contained in a single transaction by default. However, the [DbContext transaction APIs](xref:core/saving/transactions) can be used in the normal way to wrap these commands in a transaction.

> [!TIP]
> Sending these commands in a single round-trip depends on [Issue #10879](https://github.com/dotnet/efcore/issues/10879). Vote for this issue if it's something you would like to see implemented.

Configuring [cascade deletes](xref:core/saving/cascade-delete) in the database can be very useful here. In our model, the relationship between `Blog` and `Post` is required, which causes EF Core to configure a cascade delete by convention.  This means when a blog is deleted in the database, then all its dependent posts will also be deleted. It then follows that to delete all blogs and posts we need only delete the blogs:

<!--
        await context.Blogs.ExecuteDeleteAsync();
-->
[!code-csharp[DeleteAllBlogsAndPosts](../../../../samples/core/Miscellaneous/NewInEFCore7/ExecuteDeleteSample.cs?name=DeleteAllBlogsAndPosts)]

This results in the following SQL:

```sql
DELETE FROM [b]
FROM [Blogs] AS [b]
```

Which, as it is deleting a blog, will also cause all related posts to be deleted by the configured cascade delete.

## Table-per-concrete-type (TPC) inheritance mapping

By default, EF Core maps an inheritance hierarchy of .NET types to a single database table. This is known as the [table-per-hierarchy (TPH)](xref:core/modeling/inheritance#table-per-hierarchy-and-discriminator-configuration) mapping strategy. EF Core 5.0 introduced the [table-per-type (TPT)](xref:core/modeling/inheritance#table-per-type-configuration) strategy, which supports mapping each .NET type to a different database table. EF7 introduces the table-per-concrete-type (TPC) strategy. TPC also maps .NET types to different tables, but in a way that addresses some common performance issues with the TPT strategy.

> [!TIP]
> The code shown here comes from [TpcInheritanceSample.cs](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Miscellaneous/NewInEFCore7/TpcInheritanceSample.cs).

> [!TIP]
> The EF Team demonstrated and talked in depth about TPC mapping in an episode of the .NET Data Community Standup. As with [all Community Standup episodes](https://aka.ms/efstandups), you can [watch the TPC episode now on YouTube](https://youtu.be/HaL6DKW1mrg).

### TPC database schema

The TPC strategy is similar to the TPT strategy except that a different table is created for every _concrete_ type in the hierarchy, but tables are **not** created for _abstract_ types--hence the name “table-per-concrete-type”. As with TPT, the table itself indicates the type of the object saved. However, unlike TPT mapping, each table contains columns for every property in the concrete type and its base types. TPC database schemas are denormalized.

For example, consider mapping this hierarchy:

<!--
    public abstract class Animal
    {
        protected Animal(string name)
        {
            Name = name;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public abstract string Species { get; }

        public Food? Food { get; set; }
    }

    public abstract class Pet : Animal
    {
        protected Pet(string name)
            : base(name)
        {
        }

        public string? Vet { get; set; }

        public ICollection<Human> Humans { get; } = new List<Human>();
    }

    public class FarmAnimal : Animal
    {
        public FarmAnimal(string name, string species)
            : base(name)
        {
            Species = species;
        }

        public override string Species { get; }

        [Precision(18, 2)]
        public decimal Value { get; set; }

        public override string ToString()
            => $"Farm animal '{Name}' ({Species}/{Id}) worth {Value:C} eats {Food?.ToString() ?? "<Unknown>"}";
    }

    public class Cat : Pet
    {
        public Cat(string name, string educationLevel)
            : base(name)
        {
            EducationLevel = educationLevel;
        }

        public string EducationLevel { get; set; }
        public override string Species => "Felis catus";

        public override string ToString()
            => $"Cat '{Name}' ({Species}/{Id}) with education '{EducationLevel}' eats {Food?.ToString() ?? "<Unknown>"}";
    }

    public class Dog : Pet
    {
        public Dog(string name, string favoriteToy)
            : base(name)
        {
            FavoriteToy = favoriteToy;
        }

        public string FavoriteToy { get; set; }
        public override string Species => "Canis familiaris";

        public override string ToString()
            => $"Dog '{Name}' ({Species}/{Id}) with favorite toy '{FavoriteToy}' eats {Food?.ToString() ?? "<Unknown>"}";
    }

    public class Human : Animal
    {
        public Human(string name)
            : base(name)
        {
        }

        public override string Species => "Homo sapiens";

        public Animal? FavoriteAnimal { get; set; }
        public ICollection<Pet> Pets { get; } = new List<Pet>();

        public override string ToString()
            => $"Human '{Name}' ({Species}/{Id}) with favorite animal '{FavoriteAnimal?.Name ?? "<Unknown>"}'" +
               $" eats {Food?.ToString() ?? "<Unknown>"}";
    }
-->
[!code-csharp[AnimalsHierarchy](../../../../samples/core/Miscellaneous/NewInEFCore7/TpcInheritanceSample.cs?name=AnimalsHierarchy)]

When using SQL Server, the tables created for this hierarchy are:

```sql
CREATE TABLE [Cats] (
    [Id] int NOT NULL DEFAULT (NEXT VALUE FOR [AnimalSequence]),
    [Name] nvarchar(max) NOT NULL,
    [FoodId] uniqueidentifier NULL,
    [Vet] nvarchar(max) NULL,
    [EducationLevel] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Cats] PRIMARY KEY ([Id]));

CREATE TABLE [Dogs] (
    [Id] int NOT NULL DEFAULT (NEXT VALUE FOR [AnimalSequence]),
    [Name] nvarchar(max) NOT NULL,
    [FoodId] uniqueidentifier NULL,
    [Vet] nvarchar(max) NULL,
    [FavoriteToy] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Dogs] PRIMARY KEY ([Id]));

CREATE TABLE [FarmAnimals] (
    [Id] int NOT NULL DEFAULT (NEXT VALUE FOR [AnimalSequence]),
    [Name] nvarchar(max) NOT NULL,
    [FoodId] uniqueidentifier NULL,
    [Value] decimal(18,2) NOT NULL,
    [Species] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_FarmAnimals] PRIMARY KEY ([Id]));

CREATE TABLE [Humans] (
    [Id] int NOT NULL DEFAULT (NEXT VALUE FOR [AnimalSequence]),
    [Name] nvarchar(max) NOT NULL,
    [FoodId] uniqueidentifier NULL,
    [FavoriteAnimalId] int NULL,
    CONSTRAINT [PK_Humans] PRIMARY KEY ([Id]));
```

Notice that:

- There are no tables for the `Animal` or `Pet` types, since these are `abstract` in the object model. Remember that C# does not allow instances of abstract types, and there is therefore no situation where an abstract type instance will be saved to the database.
- The mapping of properties in base types is repeated for each concrete type. For example, every table has a `Name` column, and both Cats and Dogs have a `Vet` column.

- Saving some data into this database results in the following:

**Cats table**

| Id  | Name   | FoodId                               | Vet                  | EducationLevel |
|:----|:-------|:-------------------------------------|:---------------------|:---------------|
| 1   | Alice  | 99ca3e98-b26d-4a0c-d4ae-08da7aca624f | Pengelly             | MBA            |
| 2   | Mac    | 99ca3e98-b26d-4a0c-d4ae-08da7aca624f | Pengelly             | Preschool      |
| 8   | Baxter | 5dc5019e-6f72-454b-d4b0-08da7aca624f | Bothell Pet Hospital | BSc            |

**Dogs table**

| Id  | Name  | FoodId                               | Vet      | FavoriteToy  |
|:----|:------|:-------------------------------------|:---------|:-------------|
| 3   | Toast | 011aaf6f-d588-4fad-d4ac-08da7aca624f | Pengelly | Mr. Squirrel |

**FarmAnimals table**

| Id  | Name  | FoodId                               | Value  | Species                |
|:----|:------|:-------------------------------------|:-------|:-----------------------|
| 4   | Clyde | 1d495075-f527-4498-d4af-08da7aca624f | 100.00 | Equus africanus asinus |

**Humans table**

| Id  | Name   | FoodId                               | FavoriteAnimalId                                                           |
|:----|:-------|:-------------------------------------|:---------------------------------------------------------------------------|
| 5   | Wendy  | 5418fd81-7660-432f-d4b1-08da7aca624f | -2147482646 // See [#28654](https://github.com/dotnet/efcore/issues/28654) |
| 6   | Arthur | 59b495d4-0414-46bf-d4ad-08da7aca624f | -2147482644 // See [#28654](https://github.com/dotnet/efcore/issues/28654) |
| 9   | Katie  | null                                 | -2147482640 // See [#28654](https://github.com/dotnet/efcore/issues/28654) |

Notice that, unlike with TPT mapping, all the information for a single object is contained in a single table. And, unlike with TPH mapping, there is no combination of column and row in any table where that is never used by the model. We'll see below how these characteristics can be important for queries and storage.

### Configuring TPC inheritance

All types in an inheritance hierarchy must be explicitly included in the model when mapping the hierarchy with EF Core. This can be done by creating `DbSet` properties on your `DbContext` for every type:

<!--
        public DbSet<Animal> Animals => Set<Animal>();
        public DbSet<Pet> Pets => Set<Pet>();
        public DbSet<FarmAnimal> FarmAnimals => Set<FarmAnimal>();
        public DbSet<Cat> Cats => Set<Cat>();
        public DbSet<Dog> Dogs => Set<Dog>();
        public DbSet<Human> Humans => Set<Human>();
-->
[!code-csharp[AnimalSets](../../../../samples/core/Miscellaneous/NewInEFCore7/TpcInheritanceSample.cs?name=AnimalSets)]

Or by using the `Entity` method in `OnModelCreating`:

<!--
            modelBuilder.Entity<Animal>();
            modelBuilder.Entity<Pet>();
            modelBuilder.Entity<Cat>();
            modelBuilder.Entity<Dog>();
            modelBuilder.Entity<FarmAnimal>();
            modelBuilder.Entity<Human>();
-->
[!code-csharp[AnimalsInModelBuilder](../../../../samples/core/Miscellaneous/NewInEFCore7/TpcInheritanceSample.cs?name=AnimalsInModelBuilder)]

> [!IMPORTANT]
> This is different from the legacy EF6 behavior, where derived types of mapped base types would be automatically discovered if they were contained in the same assembly.

Nothing else needs to be done to map the hierarchy as TPH, since it is the default strategy. However, starting with EF7, TPH can made explicit by calling `UseTphMappingStrategy` on the base type of the hierarchy:

<!--
            modelBuilder.Entity<Animal>().UseTphMappingStrategy();
-->
[!code-csharp[UseTphMappingStrategy](../../../../samples/core/Miscellaneous/NewInEFCore7/TpcInheritanceSample.cs?name=UseTphMappingStrategy)]

To use TPT instead, change this to `UseTptMappingStrategy`:

<!--
            modelBuilder.Entity<Animal>().UseTptMappingStrategy();
-->
[!code-csharp[UseTptMappingStrategy](../../../../samples/core/Miscellaneous/NewInEFCore7/TpcInheritanceSample.cs?name=UseTptMappingStrategy)]

Likewise, `UseTpcMappingStrategy` is used to configure TPC:

<!--
            modelBuilder.Entity<Animal>().UseTpcMappingStrategy();
-->
[!code-csharp[UseTpcMappingStrategy](../../../../samples/core/Miscellaneous/NewInEFCore7/TpcInheritanceSample.cs?name=UseTpcMappingStrategy)]

In each case, the table name used for each type is taken from the `DbSet` property name on your `DbContext`, or [can be configured](xref:core/modeling/entity-types#table-name) using the `ToTable` builder method, or the `[Table]` attribute.

### TPC query performance

For queries, the TPC strategy is an improvement over TPT because it ensures that the information for a given entity instance is always stored in a single table. This means the TPC strategy can be useful when the mapped hierarchy is large and has many concrete (usually leaf) types, each with a large number of properties, and where only a small subset of types are used in most queries.

The SQL generated for three simple LINQ queries can be used to observe where TPC does well when compared to TPH and TPT. These queries are:

1. A query that returns entities of all types in the hierarchy:

   ```csharp
   context.Animals.ToList();
   ```

2. A query that returns entities from a subset of types in the hierarchy:

   ```csharp
   context.Pets.ToList();
   ```

3. A query that returns only entities from a single leaf type in the hierarchy:

   ```csharp
   context.Cats.ToList();
   ```

#### TPH queries

When using TPH, all three queries only query a single table, but with different filters on the discriminator column:

1. TPH SQL returning entities of all types in the hierarchy:

   ```sql
   SELECT [a].[Id], [a].[Discriminator], [a].[FoodId], [a].[Name], [a].[Species], [a].[Value], [a].[FavoriteAnimalId], [a].[Vet], [a].[EducationLevel], [a].[FavoriteToy]
   FROM [Animals] AS [a]
   ```

2. TPH SQL returning entities from a subset of types in the hierarchy:

   ```sql
   SELECT [a].[Id], [a].[Discriminator], [a].[FoodId], [a].[Name], [a].[Vet], [a].[EducationLevel], [a].[FavoriteToy]
   FROM [Animals] AS [a]
   WHERE [a].[Discriminator] IN (N'Cat', N'Dog')
   ```

3. TPH SQL returning only entities from a single leaf type in the hierarchy:

   ```sql
   SELECT [a].[Id], [a].[Discriminator], [a].[FoodId], [a].[Name], [a].[Vet], [a].[EducationLevel]
   FROM [Animals] AS [a]
   WHERE [a].[Discriminator] = N'Cat'
   ```

All these queries should perform well, especially with an appropriate database index on the discriminator column.

#### TPT queries

When using TPT, all of these queries require joining multiple tables, since the data for any given concrete type is split across many tables:

1. TPT SQL returning entities of all types in the hierarchy:

   ```sql
   SELECT [a].[Id], [a].[FoodId], [a].[Name], [f].[Species], [f].[Value], [h].[FavoriteAnimalId], [p].[Vet], [c].[EducationLevel], [d].[FavoriteToy], CASE
       WHEN [d].[Id] IS NOT NULL THEN N'Dog'
       WHEN [c].[Id] IS NOT NULL THEN N'Cat'
       WHEN [h].[Id] IS NOT NULL THEN N'Human'
       WHEN [f].[Id] IS NOT NULL THEN N'FarmAnimal'
   END AS [Discriminator]
   FROM [Animals] AS [a]
   LEFT JOIN [FarmAnimals] AS [f] ON [a].[Id] = [f].[Id]
   LEFT JOIN [Humans] AS [h] ON [a].[Id] = [h].[Id]
   LEFT JOIN [Pets] AS [p] ON [a].[Id] = [p].[Id]
   LEFT JOIN [Cats] AS [c] ON [a].[Id] = [c].[Id]
   LEFT JOIN [Dogs] AS [d] ON [a].[Id] = [d].[Id]
   ```

2. TPT SQL returning entities from a subset of types in the hierarchy:

   ```sql
   SELECT [a].[Id], [a].[FoodId], [a].[Name], [p].[Vet], [c].[EducationLevel], [d].[FavoriteToy], CASE
       WHEN [d].[Id] IS NOT NULL THEN N'Dog'
       WHEN [c].[Id] IS NOT NULL THEN N'Cat'
   END AS [Discriminator]
   FROM [Animals] AS [a]
   INNER JOIN [Pets] AS [p] ON [a].[Id] = [p].[Id]
   LEFT JOIN [Cats] AS [c] ON [a].[Id] = [c].[Id]
   LEFT JOIN [Dogs] AS [d] ON [a].[Id] = [d].[Id]
   ```

3. TPT SQL returning only entities from a single leaf type in the hierarchy:

   ```sql
   SELECT [a].[Id], [a].[FoodId], [a].[Name], [p].[Vet], [c].[EducationLevel]
   FROM [Animals] AS [a]
   INNER JOIN [Pets] AS [p] ON [a].[Id] = [p].[Id]
   INNER JOIN [Cats] AS [c] ON [a].[Id] = [c].[Id]
   ```

> [!NOTE]
> EF Core uses “discriminator synthesis” to determine which table the data comes from, and hence the correct type to use. This works because the LEFT JOIN returns nulls for the dependent ID column (the “sub-tables”) which aren’t the correct type. So for a dog, `[d].[Id]` will be non-null, and all the other (concrete) IDs will be null.

All of these queries can suffer from performance issues due to the table joins. This is why TPT is never a good choice for query performance.

#### TPC queries

TPC improves over TPT for all of these queries because the number of tables that need to be queried is reduced. In addition, the results from each table are combined using `UNION ALL`, which can be considerably faster than a table join, since it does not need to perform any matching between rows or de-duplication of rows.

1. TPC SQL returning entities of all types in the hierarchy:

   ```sql
   SELECT [f].[Id], [f].[FoodId], [f].[Name], [f].[Species], [f].[Value], NULL AS [FavoriteAnimalId], NULL AS [Vet], NULL AS [EducationLevel], NULL AS [FavoriteToy], N'FarmAnimal' AS [Discriminator]
   FROM [FarmAnimals] AS [f]
   UNION ALL
   SELECT [h].[Id], [h].[FoodId], [h].[Name], NULL AS [Species], NULL AS [Value], [h].[FavoriteAnimalId], NULL AS [Vet], NULL AS [EducationLevel], NULL AS [FavoriteToy], N'Human' AS [Discriminator]
   FROM [Humans] AS [h]
   UNION ALL
   SELECT [c].[Id], [c].[FoodId], [c].[Name], NULL AS [Species], NULL AS [Value], NULL AS [FavoriteAnimalId], [c].[Vet], [c].[EducationLevel], NULL AS [FavoriteToy], N'Cat' AS [Discriminator]
   FROM [Cats] AS [c]
   UNION ALL
   SELECT [d].[Id], [d].[FoodId], [d].[Name], NULL AS [Species], NULL AS [Value], NULL AS [FavoriteAnimalId], [d].[Vet], NULL AS [EducationLevel], [d].[FavoriteToy], N'Dog' AS [Discriminator]
   FROM [Dogs] AS [d]
   ```

2. TPC SQL returning entities from a subset of types in the hierarchy:

   ```sql
   SELECT [c].[Id], [c].[FoodId], [c].[Name], [c].[Vet], [c].[EducationLevel], NULL AS [FavoriteToy], N'Cat' AS [Discriminator]
   FROM [Cats] AS [c]
   UNION ALL
   SELECT [d].[Id], [d].[FoodId], [d].[Name], [d].[Vet], NULL AS [EducationLevel], [d].[FavoriteToy], N'Dog' AS [Discriminator]
   FROM [Dogs] AS [d]
   ```

3. TPC SQL returning only entities from a single leaf type in the hierarchy:

   ```sql
   SELECT [c].[Id], [c].[FoodId], [c].[Name], [c].[Vet], [c].[EducationLevel]
   FROM [Cats] AS [c]
   ```

Even though TPC is better than TPT for all of these queries, the TPH queries are still better when returning instances of multiple types. This is one of the reasons that TPH is the default strategy used by EF Core.

As the SQL for query #3 shows, TPC really excels when querying for entities of a single leaf type. The query only uses a single table and needs no filtering.

### TPC inserts and updates

TPC also performs well when saving a new entity, since this requires inserting only a single row into a single table. This is also true for TPH. With TPT, rows must be inserted into many tables, which will perform less well.

The same is often true for updates, although in this case if all columns being updated are in the same table, even for TPT, then the difference may not be significant.

### Space considerations

Both TPT and TPC can use less storage than TPH when there are many subtypes with many properties that are often not used. This is because every row in the TPH table must store a `NULL` for each of these unused properties. In practice, this is rarely an issue, but it could be worth considering when storing large amounts of data with these characteristics.

> [!TIP]
> If your database system supports it (e.g. SQL Server), then consider using "sparse columns" for TPH columns that will be rarely populated.

### Key generation

The inheritance mapping strategy chosen has consequences for how primary key values are generated and managed. Keys in TPH are easy, since each entity instance is represented by a single row in a single table. Any kind of key value generation can be used, and no additional constraints are needed.

For the TPT strategy, there is always a row in the table mapped to the base type of the hierarchy. Any kind of key generation can be used on this row, and the keys for other tables are linked to this table using foreign key constraints.

Things get a bit more complicated for TPC. First, it’s important to understand that EF Core requires that all entities in a hierarchy must have a unique key value, even if the entities have different types. So, using our example model, a Dog cannot have the same Id key value as a Cat. Second, unlike TPT, there is no common table that can act as the single place where key values live and can be generated. This means a simple `Identity` column cannot be used.

For databases that support sequences, key values can be generated by using a single sequence referenced in the default constraint for each table. This is the strategy used in the TPC tables shown above, where each table has the following:

```sql
[Id] int NOT NULL DEFAULT (NEXT VALUE FOR [AnimalSequence])
```

`AnimalSequence` is a database sequence created by EF Core. This strategy is used by default for TPC hierarchies when using the EF Core database provider for SQL Server. Database providers for other databases that support sequences should have a similar default. Other key generation strategies that use sequences, such as Hi-Lo patterns, may also be used with TPC.

SQLite does not support sequences, and hence integer key value generation is not supported when using SQLite with the TPC strategy. However, client-side generation or globally unique keys--for example, GUID keys--are supported on any database, including SQLite.

### Foreign key constraints

The TPC mapping strategy creates a denormalized SQL schema--this is one reason why some database purists are against it. For example, consider the foreign key column `FavoriteAnimalId`. The value in this column must match the primary key value of some animal. This can be enforced in the database with a simple FK constraint when using TPH or TPT. For example:

```sql
CONSTRAINT [FK_Animals_Animals_FavoriteAnimalId] FOREIGN KEY ([FavoriteAnimalId]) REFERENCES [Animals] ([Id])
```

But when using TPC, the primary key for an animal is stored in the table for the concrete type of that animal. For example, a cat's primary key is stored in the `Cats.Id` column, while a dog's primary key is stored in the `Dogs.Id` column, and so on. This means an FK constraint cannot be created for this relationship.

In practice, this is not a problem as long as the application does not attempt to insert invalid data. For example, if all the data is inserted by EF Core and uses navigations to relate entities, then it is guaranteed that the FK column will contain valid PK value at all times.

### Summary and guidance

In summary, TPC is a good mapping strategy to use when your code will mostly query for entities of a single leaf type. This is because the storage requirements are smaller, and there is no discriminator column that may need an index. Inserts and updates are also efficient.

That being said, TPH is usually fine for most applications, and is a good default for a wide range of scenarios, so don't add the complexity of TPC if you don't need it. Specifically, if your code will mostly query for entities of many types, such as writing queries against the base type, then lean towards TPH over TPC.

Use TPT only if constrained to do so by external factors.
