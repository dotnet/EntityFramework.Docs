---
title: What's New in EF Core 8
description: Overview of new features in EF Core 8
author: ajcvickers
ms.date: 03/11/2023
uid: core/what-is-new/ef-core-8
---

# What's New in EF Core 8

EF Core 8.0 (EF8) is the next release after EF Core 7.0 and is scheduled for release in November 2023. See [_Plan for Entity Framework Core 8_](xref:core/what-is-new/ef-core-8.0/plan) for details and [_Latest news and progress on .NET 8 and EF8_](https://github.com/dotnet/efcore/issues/29989) for progress on the plan.

EF8 is available as [daily builds](https://github.com/dotnet/efcore/blob/main/docs/DailyBuilds.md) which contain all the latest EF8 features and API tweaks. The samples here make use of these daily builds.

> [!TIP]
> You can run and debug into the samples by [downloading the sample code from GitHub](https://github.com/dotnet/EntityFramework.Docs). Each section links to the source code specific to that section.

EF8 previews currently target .NET 6, and can therefore be used with either [.NET 6 (LTS)](https://dotnet.microsoft.com/download/dotnet/6.0) or [.NET 7](https://dotnet.microsoft.com/download/dotnet/7.0). There is a good chance that EF8 will be changed to target .NET 8 before it's released.

## Sample model

Many of the examples below use a simple model with blogs, posts, tags, and authors:

[!code-csharp[BlogsModel](../../../../samples/core/Miscellaneous/NewInEFCore8/BlogsContext.cs?name=BlogsModel)]

Some of the examples also use aggregate types, which are mapped in different ways in different samples. There is one aggregate type for contacts:

[!code-csharp[ContactDetailsAggregate](../../../../samples/core/Miscellaneous/NewInEFCore8/BlogsContext.cs?name=ContactDetailsAggregate)]

And a second aggregate type for post metadata:

[!code-csharp[PostMetadataAggregate](../../../../samples/core/Miscellaneous/NewInEFCore8/BlogsContext.cs?name=PostMetadataAggregate)]

> [!TIP]
> The sample model can be found in [BlogsContext.cs](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Miscellaneous/NewInEFCore8/BlogsContext.cs).

## New in EF8 Preview 1

### Enhancements to JSON column support

EF8 includes improvements to the [JSON column mapping support introduced in EF7](xref:core/what-is-new/ef-core-7#json-columns).

> [!TIP]
> The code shown here comes from [JsonColumnsSample.cs](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Miscellaneous/NewInEFCore8/JsonColumnsSample.cs).

#### Translate element access into JSON arrays

EF8 supports indexing in JSON arrays when executing queries. For example, the following query checks whether the first two updates were made before a given date.

<!--
        var cutoff = DateOnly.FromDateTime(DateTime.UtcNow - TimeSpan.FromDays(365));
        var updatedPosts = await context.Posts
            .Where(
                p => p.Metadata!.Updates[0].UpdatedOn < cutoff
                     && p.Metadata!.Updates[1].UpdatedOn < cutoff)
            .ToListAsync();
-->
[!code-csharp[CollectionIndexPredicate](../../../../samples/core/Miscellaneous/NewInEFCore8/JsonColumnsSample.cs?name=CollectionIndexPredicate)]

This translates into the following SQL when using SQL Server:

```sql
SELECT [p].[Id], [p].[Archived], [p].[AuthorId], [p].[BlogId], [p].[Content], [p].[Discriminator], [p].[PublishedOn], [p].[Title], [p].[PromoText], [p].[Metadata]
FROM [Posts] AS [p]
WHERE CAST(JSON_VALUE([p].[Metadata],'$.Updates[0].UpdatedOn') AS date) < @__cutoff_0
  AND CAST(JSON_VALUE([p].[Metadata],'$.Updates[1].UpdatedOn') AS date) < @__cutoff_0
```

> [!NOTE]
> This query will succeed even if a given post does not have any updates, or only has a single update. In such a case, `JSON_VALUE` returns `NULL` and the predicate is not matched.

Indexing into JSON arrays can also be used to project elements from an array into the final results. For example, the following query projects out the `UpdatedOn` date for the first and second updates of each post.

<!--
        var postsAndRecentUpdatesNullable = await context.Posts
            .Select(p => new
            {
                p.Title,
                LatestUpdate = (DateOnly?)p.Metadata!.Updates[0].UpdatedOn,
                SecondLatestUpdate = (DateOnly?)p.Metadata.Updates[1].UpdatedOn
            })
            .ToListAsync();
-->
[!code-csharp[CollectionIndexProjectionNullable](../../../../samples/core/Miscellaneous/NewInEFCore8/JsonColumnsSample.cs?name=CollectionIndexProjectionNullable)]

This translates into the following SQL when using SQL Server:

```sql
SELECT [p].[Title],
       CAST(JSON_VALUE([p].[Metadata],'$.Updates[0].UpdatedOn') AS date) AS [LatestUpdate],
       CAST(JSON_VALUE([p].[Metadata],'$.Updates[1].UpdatedOn') AS date) AS [SecondLatestUpdate]
FROM [Posts] AS [p]
```

As noted above, `JSON_VALUE` returns null if the element of the array does not exist. This is handled in the query by casting the projected value to a nullable `DateOnly`. An alternative to casting the value is to filter the query results so that `JSON_VALUE` will never return null. For example:

<!--
        var postsAndRecentUpdates = await context.Posts
            .Where(p => p.Metadata!.Updates[0].UpdatedOn != null
                        && p.Metadata!.Updates[1].UpdatedOn != null)
            .Select(p => new
            {
                p.Title,
                LatestUpdate = p.Metadata!.Updates[0].UpdatedOn,
                SecondLatestUpdate = p.Metadata.Updates[1].UpdatedOn
            })
            .ToListAsync();
-->
[!code-csharp[CollectionIndexProjection](../../../../samples/core/Miscellaneous/NewInEFCore8/JsonColumnsSample.cs?name=CollectionIndexProjection)]

This translates into the following SQL when using SQL Server:

```sql
SELECT [p].[Title],
       CAST(JSON_VALUE([p].[Metadata],'$.Updates[0].UpdatedOn') AS date) AS [LatestUpdate],
       CAST(JSON_VALUE([p].[Metadata],'$.Updates[1].UpdatedOn') AS date) AS [SecondLatestUpdate]
FROM [Posts] AS [p]
      WHERE (CAST(JSON_VALUE([p].[Metadata],'$.Updates[0].UpdatedOn') AS date) IS NOT NULL)
        AND (CAST(JSON_VALUE([p].[Metadata],'$.Updates[1].UpdatedOn') AS date) IS NOT NULL)
```

### Raw SQL queries for unmapped types

EF7 introduced [raw SQL queries returning scalar types](xref:core/querying/sql-queries#querying-scalar-(non-entity)-types). This is enhanced in EF8 to include raw SQL queries returning any mappable CLR type, without including that type in the EF model.

> [!TIP]
> The code shown here comes from [RawSqlSample.cs](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Miscellaneous/NewInEFCore8/RawSqlSample.cs).

Queries using unmapped types are executed using <xref:Microsoft.EntityFrameworkCore.RelationalDatabaseFacadeExtensions.SqlQuery%2A> or <xref:Microsoft.EntityFrameworkCore.RelationalDatabaseFacadeExtensions.SqlQueryRaw%2A>. The former uses string interpolation to parameterize the query, which helps ensure that all non-constant values are parameterized. For example, consider the following database table:

```sql
CREATE TABLE [Posts] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(max) NOT NULL,
    [Content] nvarchar(max) NOT NULL,
    [PublishedOn] date NOT NULL,
    [BlogId] int NOT NULL,
);
```

`SqlQuery` can be used to query this table and return instances of a `BlogPost` type with properties corresponding to the columns in the table:

For example:

```csharp
public class BlogPost
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateOnly PublishedOn { get; set; }
    public int BlogId { get; set; }
}
```

For example:

<!--
        var start = new DateOnly(2022, 1, 1);
        var end = new DateOnly(2023, 1, 1);
        var postsIn2022 =
            await context.Database
                .SqlQuery<BlogPost>($"SELECT * FROM Posts as p WHERE p.PublishedOn >= {start} AND p.PublishedOn < {end}")
                .ToListAsync();
-->
[!code-csharp[SqlQueryAllColumns](../../../../samples/core/Miscellaneous/NewInEFCore8/RawSqlSample.cs?name=SqlQueryAllColumns)]

This query is parameterized and executed as:

```sql
SELECT * FROM Posts as p WHERE p.PublishedOn >= @p0 AND p.PublishedOn < @p1
```

The type used for query results can contain common mapping constructs supported by EF Core, such as parameterized constructors and mapping attributes. For example:

<!--
    public class BlogPost
    {
        public BlogPost(string blogTitle, string content, DateOnly publishedOn)
        {
            BlogTitle = blogTitle;
            Content = content;
            PublishedOn = publishedOn;
        }

        public int Id { get; private set; }

        [Column("Title")]
        public string BlogTitle { get; set; }

        public string Content { get; set; }
        public DateOnly PublishedOn { get; set; }
        public int BlogId { get; set; }
    }
-->
[!code-csharp[BlogPost](../../../../samples/core/Miscellaneous/NewInEFCore8/RawSqlSample.cs?name=BlogPost)]

> [!NOTE]
> Types used in this way do not have keys defined and cannot have relationships to other types. Types with relationships must be mapped in the model.

The type used must have a property for every value in the result set, but do not need to match any table in the database. For example, the following type represents only a subset of information for each post, and includes the blog name, which comes from the `Blogs` table:

<!--
    public class PostSummary
    {
        public string BlogName { get; set; }
        public string PostTitle { get; set; }
        public DateOnly PublishedOn { get; set; }
    }
-->
[!code-csharp[PostSummary](../../../../samples/core/Miscellaneous/NewInEFCore8/RawSqlSample.cs?name=PostSummary)]

And can be queried using `SqlQuery` in the same way as before:

<!--
        var summaries =
            await context.Database.SqlQuery<PostSummary>(
                    @$"SELECT b.Name AS BlogName, p.Title AS PostTitle, p.PublishedOn
                    FROM Posts AS p
                    INNER JOIN Blogs AS b ON p.BlogId = b.Id")
                .ToListAsync();
-->
[!code-csharp[SqlQueryJoin](../../../../samples/core/Miscellaneous/NewInEFCore8/RawSqlSample.cs?name=SqlQueryJoin)]

One nice feature of `SqlQuery` is that it returns an `IQueryable` which can be composed on using LINQ. For example, a 'Where' clause can be added to the query above:

<!--
        var summariesIn2022 =
            await context.Database.SqlQuery<PostSummary>(
                    @$"SELECT b.Name AS BlogName, p.Title AS PostTitle, p.PublishedOn
                    FROM Posts AS p
                    INNER JOIN Blogs AS b ON p.BlogId = b.Id")
                .Where(p => p.PublishedOn >= start && p.PublishedOn < end)
                .ToListAsync();
-->
[!code-csharp[SqlQueryJoinComposed](../../../../samples/core/Miscellaneous/NewInEFCore8/RawSqlSample.cs?name=SqlQueryJoinComposed)]

This is executed as:

```sql
SELECT [n].[BlogName], [n].[PostTitle], [n].[PublishedOn]
FROM (
         SELECT b.Name AS BlogName, p.Title AS PostTitle, p.PublishedOn
         FROM Posts AS p
                  INNER JOIN Blogs AS b ON p.BlogId = b.Id
     ) AS [n]
WHERE [n].[PublishedOn] >= @__cutoffDate_1 AND [n].[PublishedOn] < @__end_2
```

At this point it is worth remembering that all of the above can be done completely in LINQ without the need to write any SQL. This includes returning instances of an unmapped type like `PostSummary`. For example, the preceding query can be written in LINQ as:

```csharp
var summaries =
    await context.Posts.Select(
            p => new PostSummary
            {
                BlogName = p.Blog.Name,
                PostTitle = p.Title,
                PublishedOn = p.PublishedOn,
            })
        .Where(p => p.PublishedOn >= start && p.PublishedOn < end)
        .ToListAsync();
```

Which translates to much cleaner SQL:

```sql
SELECT [b].[Name] AS [BlogName], [p].[Title] AS [PostTitle], [p].[PublishedOn]
FROM [Posts] AS [p]
INNER JOIN [Blogs] AS [b] ON [p].[BlogId] = [b].[Id]
WHERE [p].[PublishedOn] >= @__start_0 AND [p].[PublishedOn] < @__end_1
```

> [!TIP]
> EF is able to generate cleaner SQL when it is responsible for the entire query than it is when composing over user-supplied SQL because, in the former case, the full semantics of the query is available to EF.

So far, all the queries have been executed directly against tables. `SqlQuery` can also be used to return results from a view without mapping the view type in the EF model. For example:

<!--
        var summariesFromView =
            await context.Database.SqlQuery<PostSummary>(
                    @$"SELECT * FROM PostAndBlogSummariesView")
                .Where(p => p.PublishedOn >= start && p.PublishedOn < end)
                .ToListAsync();
-->
[!code-csharp[SqlQueryView](../../../../samples/core/Miscellaneous/NewInEFCore8/RawSqlSample.cs?name=SqlQueryView)]

Likewise, `SqlQuery` can be used for the results of a function:

<!--
        var summariesFromFunc =
            await context.Database.SqlQuery<PostSummary>(
                    @$"SELECT * FROM GetPostsPublishedAfter({start})")
                .Where(p => p.PublishedOn < end)
                .ToListAsync();
-->
[!code-csharp[SqlQueryFunction](../../../../samples/core/Miscellaneous/NewInEFCore8/RawSqlSample.cs?name=SqlQueryFunction)]

The returned `IQueryable` can be composed upon when it is the result of a view or function, just like it can be for the result of a table query. Stored procedures can be also be executed using `SqlQuery`, but most databases do not support composing over them. For example:

<!--
        var summariesFromStoredProc =
            await context.Database.SqlQuery<PostSummary>(
                    @$"exec GetRecentPostSummariesProc")
                .ToListAsync();
-->
[!code-csharp[SqlQueryStoredProc](../../../../samples/core/Miscellaneous/NewInEFCore8/RawSqlSample.cs?name=SqlQueryStoredProc)]

### Lazy-loading for no-tracking queries

EF8 adds support for [lazy-loading of navigations](xref:core/querying/related-data/lazy) on entities that are not being tracked by the `DbContext`. This means a no-tracking query can be followed by lazy-loading of navigations on the entities returned by the no-tracking query.

> [!TIP]
> The code for the lazy-loading examples shown below comes from [LazyLoadingSample.cs](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Miscellaneous/NewInEFCore8/LazyLoadingSample.cs).

For example, consider a no-tracking query for blogs:

<!--
var blogs = await context.Blogs.AsNoTracking().ToListAsync();
-->
[!code-csharp[NoTrackingForBlogs](../../../../samples/core/Miscellaneous/NewInEFCore8/LazyLoadingSample.cs?name=NoTrackingForBlogs)]

If `Blog.Posts` is configured for lazy-loading, for example, using lazy-loading proxies, then accessing `Posts` will cause it to load from the database:

<!--
        Console.WriteLine();
        Console.Write("Choose a blog: ");
        if (int.TryParse(ReadLine(), out var blogId))
        {
            Console.WriteLine("Posts:");
            foreach (var post in blogs[blogId - 1].Posts)
            {
                Console.WriteLine($"  {post.Title}");
            }
        }
-->
[!code-csharp[ChooseABlog](../../../../samples/core/Miscellaneous/NewInEFCore8/LazyLoadingSample.cs?name=ChooseABlog)]

EF8 also reports whether or not a given navigation is loaded for entities not tracked by the context. For example:

<!--
        foreach (var blog in blogs)
        {
            if (context.Entry(blog).Collection(e => e.Posts).IsLoaded)
            {
                Console.WriteLine($" Posts for blog '{blog.Name}' are loaded.");
            }
        }
-->
[!code-csharp[IsLoaded](../../../../samples/core/Miscellaneous/NewInEFCore8/LazyLoadingSample.cs?name=IsLoaded)]

There are a few important considerations when using lazy-loading in this way:

- Lazy-loading will only succeed until the `DbContext` used to query the entity is disposed.
- Entities queried in this way a reference to their `DbContext`, even though they are not tracked by it. Care should be taken to avoid memory leaks if the entity instances will have long lifetimes.
- Explicitly detaching the entity by setting its state to `EntityState.Detached` severs the reference to the `DbContext` and lazy-loading will no longer work.
- Remember that all lazy-loading uses synchronous I/O, since there is no way to access a property in an asynchronous manner.

Lazy-loading from untracked entities works for both [lazy-loading proxies](xref:core/querying/related-data/lazy#lazy-loading-with-proxies) and [lazy-loading without proxies](xref:core/querying/related-data/lazy#lazy-loading-without-proxies).

### Explicit loading from untracked entities

EF8 supports loading of navigations on untracked entities even when the entity or navigation is not configured for lazy-loading. Unlike with lazy-loading, this [explicit loading](xref:core/querying/related-data/explicit) can be done asynchronously. For example:

<!--
            await context.Entry(blog).Collection(e => e.Posts).LoadAsync();
-->
[!code-csharp[ExplicitLoad](../../../../samples/core/Miscellaneous/NewInEFCore8/LazyLoadingSample.cs?name=ExplicitLoad)]

### Opt-out of lazy-loading for specific navigations

EF8 allows configuration of specific navigations to not lazy-load, even when everything else is set up to do so. For example, to configure the `Post.Author` navigation to not lazy-load, do the following:

<!--
        modelBuilder
            .Entity<Post>()
            .Navigation(p => p.Author)
            .EnableLazyLoading(false);
-->
[!code-csharp[NoLazyLoading](../../../../samples/core/Miscellaneous/NewInEFCore8/LazyLoadingSample.cs?name=NoLazyLoading)]

Disabling Lazy-loading like this works for both [lazy-loading proxies](xref:core/querying/related-data/lazy#lazy-loading-with-proxies) and [lazy-loading without proxies](xref:core/querying/related-data/lazy#lazy-loading-without-proxies).

Lazy-loading proxies work by overriding virtual navigation properties. In classic EF6 applications, a common source of bugs is forgetting to make a navigation virtual, since the navigation will then silently not lazy-load. Therefore, EF Core proxies throw by default when a navigation is not virtual.

This can be changed in EF8 to opt-in to the classic EF6 behavior such that a navigation can be made to not lazy-load simply by making the navigation non-virtual. This opt-in is configured as part of the call to `UseLazyLoadingProxies`. For example:

<!--
        optionsBuilder.UseLazyLoadingProxies(ignoreNonVirtualNavigations: true);
-->
[!code-csharp[IgnoreNonVirtualNavigations](../../../../samples/core/Miscellaneous/NewInEFCore8/LazyLoadingSample.cs?name=IgnoreNonVirtualNavigations)]

### Lookup tracked entities by primary, alternate, or foreign key

Internally, EF maintains data structures for finding tracked entities by primary, alternate, or foreign key. These data structures are used for efficient fixup between related entities when new entities are tracked or relationships change.

EF8 contains new public APIs so that applications can now use these data structures to efficiently lookup tracked entities. These APIs are accessed through the <xref:Microsoft.EntityFrameworkCore.ChangeTracking.LocalView%601> of the entity type. For example, to lookup a tracked entity by its primary key:

<!--
        var blogEntry = context.Blogs.Local.FindEntry(2)!;
-->
[!code-csharp[LookupByPrimaryKey](../../../../samples/core/Miscellaneous/NewInEFCore8/LookupByKeySample.cs?name=LookupByPrimaryKey)]

> [!TIP]
> The code shown here comes from [LookupByKeySample.cs](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Miscellaneous/NewInEFCore8/LookupByKeySample.cs).

The [`FindEntry`](https://github.com/dotnet/efcore/blob/81886272a761df8fafe4970b895b1e1fe35effb8/src/EFCore/ChangeTracking/LocalView.cs#L543) method returns either the <xref:Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry%601> for the tracked entity, or `null` if no entity with the given key is being tracked. Like all methods on `LocalView`, the database is never queried, even if the entity is not found. The returned entry contains the entity itself, as well as tracking information. For example:

<!--
        Console.WriteLine($"Blog '{blogEntry.Entity.Name}' with key {blogEntry.Entity.Id} is tracked in the '{blogEntry.State}' state.");
-->
[!code-csharp[UseEntry](../../../../samples/core/Miscellaneous/NewInEFCore8/LookupByKeySample.cs?name=UseEntry)]

Looking up an entity by anything other than a primary key requires that the property name be specified. For example, to look up by an alternate key:

<!--
        var siteEntry = context.Websites.Local.FindEntry(nameof(Website.Uri), new Uri("https://www.bricelam.net/"))!;
-->
[!code-csharp[LookupByAlternateKey](../../../../samples/core/Miscellaneous/NewInEFCore8/LookupByKeySample.cs?name=LookupByAlternateKey)]

Or to look up by a unique foreign key:

<!--
        var blogAtSiteEntry = context.Blogs.Local.FindEntry(nameof(Blog.SiteUri), new Uri("https://www.bricelam.net/"))!;
-->
[!code-csharp[LookupByUniqueForeignKey](../../../../samples/core/Miscellaneous/NewInEFCore8/LookupByKeySample.cs?name=LookupByUniqueForeignKey)]

So far, the lookups have always returned a single entry, or `null`. However, some lookups can return more than one entry, such as when looking up by a non-unique foreign key. The [`GetEntries`](https://github.com/dotnet/efcore/blob/81886272a761df8fafe4970b895b1e1fe35effb8/src/EFCore/ChangeTracking/LocalView.cs#L664) method should be used for these lookups. For example:

<!--
        var postEntries = context.Posts.Local.GetEntries(nameof(Post.BlogId), 2);
-->
[!code-csharp[LookupByForeignKey](../../../../samples/core/Miscellaneous/NewInEFCore8/LookupByKeySample.cs?name=LookupByForeignKey)]

In all these cases, the value being used for the lookup is either a primary key, alternate key, or foreign key value. EF uses its internal data structures for these lookups. However, lookups by value can also be used for the value of any property or combination of properties. For example, to find all archived posts:

<!--
        var archivedPostEntries = context.Posts.Local.GetEntries(nameof(Post.Archived), true);
-->
[!code-csharp[LookupByAnyProperty](../../../../samples/core/Miscellaneous/NewInEFCore8/LookupByKeySample.cs?name=LookupByAnyProperty)]

This lookup requires a scan of all tracked `Post` instances, and so will be less efficient than key lookups. However, it is usually still faster than naive queries using <xref:Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.Entries%60%601?displayProperty=nameWithType>.

Finally, it is also possible to perform lookups against composite keys, other combinations of multiple properties, or when the property type is not known at compile time. For example:

<!--
        var postTagEntry = context.Set<PostTag>().Local.FindEntryUntyped(new object[] { 4, "TagEF" });
-->
[!code-csharp[LookupByCompositePrimaryKey](../../../../samples/core/Miscellaneous/NewInEFCore8/LookupByKeySample.cs?name=LookupByCompositePrimaryKey)]

### Discriminator columns have max length

In EF8, string discriminator columns used for [TPH inheritance mapping](xref:core/modeling/inheritance) are now configured with a max length. This length is calculated as the smallest Fibonacci number that covers all defined discriminator values. For example, consider the following hierarchy:

```csharp
public abstract class Document
{
    public int Id { get; set; }
    public string Title { get; set; }
}

public abstract class Book : Document
{
    public string? Isbn { get; set; }
}

public class PaperbackEdition : Book
{
}

public class HardbackEdition : Book
{
}

public class Magazine : Document
{
    public int IssueNumber { get; set; }
}
```

With the convention of using the class names for discriminator values, the possible values here are "PaperbackEdition", "HardbackEdition", and "Magazine", and hence the discriminator column is configured for a max length of 21. For example, when using SQL Server:

```sql
CREATE TABLE [Documents] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(max) NOT NULL,
    [Discriminator] nvarchar(21) NOT NULL,
    [Isbn] nvarchar(max) NULL,
    [IssueNumber] int NULL,
    CONSTRAINT [PK_Documents] PRIMARY KEY ([Id]),
```

> [!TIP]
> Fibonacci numbers are used to limit the number of times a migration is generated to change the column length as new types are added to the hierarchy.

### DateOnly/TimeOnly supported on SQL Server

The <xref:System.DateOnly> and <xref:System.TimeOnly> types were introduced in .NET 6 and have been supported for several database providers (e.g. SQLite, MySQL, and PostgreSQL) since their introduction. For SQL Server, the recent release of a [Microsoft.Data.SqlClient](https://www.nuget.org/packages/Microsoft.Data.SqlClient/) package targeting .NET 6 has allowed [ErikEJ to add support for these types at the ADO.NET level](https://github.com/dotnet/SqlClient/pull/1813). This in turn paved the way for support in EF8 for `DateOnly` and `TimeOnly` as properties in entity types.

> [!TIP]
> `DateOnly` and `TimeOnly` can be used in EF Core 6 and 7 using the [ErikEJ.EntityFrameworkCore.SqlServer.DateOnlyTimeOnly](https://www.nuget.org/packages/ErikEJ.EntityFrameworkCore.SqlServer.DateOnlyTimeOnly) community package from [@ErikEJ](https://github.com/ErikEJ).

For example, consider the following EF model for British schools:

<!--
public class School
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateOnly Founded { get; set; }
    public List<Term> Terms { get; } = new();
    public List<OpeningHours> OpeningHours { get; } = new();
}

public class Term
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateOnly FirstDay { get; set; }
    public DateOnly LastDay { get; set; }
    public School School { get; set; } = null!;
}

[Owned]
public class OpeningHours
{
    public OpeningHours(DayOfWeek dayOfWeek, TimeOnly? opensAt, TimeOnly? closesAt)
    {
        DayOfWeek = dayOfWeek;
        OpensAt = opensAt;
        ClosesAt = closesAt;
    }

    public DayOfWeek DayOfWeek { get; private set; }
    public TimeOnly? OpensAt { get; set; }
    public TimeOnly? ClosesAt { get; set; }
}
-->
[!code-csharp[BritishSchools](../../../../samples/core/Miscellaneous/NewInEFCore8/DateOnlyTimeOnlySample.cs?name=BritishSchools)]

> [!TIP]
> The code shown here comes from [DateOnlyTimeOnlySample.cs](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Miscellaneous/NewInEFCore8/DateOnlyTimeOnlySample.cs).

> [!NOTE]
> This model represents only British schools and stores times as local (GMT) times. Handling different timezones would complicate this code significantly. Note that using `DateTimeOffset` would not help here, since opening and closing times have different offsets depending whether daylight saving time is active or not.

These entity types map to the following tables when using SQL Server. Notice that the `DateOnly` properties map to `date` columns, and the `TimeOnly` properties map to `time` columns.

```sql
CREATE TABLE [Schools] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Founded] date NOT NULL,
    CONSTRAINT [PK_Schools] PRIMARY KEY ([Id]));

CREATE TABLE [OpeningHours] (
    [SchoolId] int NOT NULL,
    [Id] int NOT NULL IDENTITY,
    [DayOfWeek] int NOT NULL,
    [OpensAt] time NULL,
    [ClosesAt] time NULL,
    CONSTRAINT [PK_OpeningHours] PRIMARY KEY ([SchoolId], [Id]),
    CONSTRAINT [FK_OpeningHours_Schools_SchoolId] FOREIGN KEY ([SchoolId]) REFERENCES [Schools] ([Id]) ON DELETE CASCADE);

CREATE TABLE [Term] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [FirstDay] date NOT NULL,
    [LastDay] date NOT NULL,
    [SchoolId] int NOT NULL,
    CONSTRAINT [PK_Term] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Term_Schools_SchoolId] FOREIGN KEY ([SchoolId]) REFERENCES [Schools] ([Id]) ON DELETE CASCADE);
```

Queries using `DateOnly` and `TimeOnly` work in the expected manner. For example, the following LINQ query finds schools that are currently open:

<!--
            openSchools = await context.Schools
                .Where(
                    s => s.Terms.Any(
                             t => t.FirstDay <= today
                                  && t.LastDay >= today)
                         && s.OpeningHours.Any(
                             o => o.DayOfWeek == dayOfWeek
                                  && o.OpensAt < time && o.ClosesAt >= time))
                .ToListAsync();
-->
[!code-csharp[OpenSchools](../../../../samples/core/Miscellaneous/NewInEFCore8/DateOnlyTimeOnlySample.cs?name=OpenSchools)]

This query translates to the following SQL, as shown by <xref:Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToQueryString%2A>:

```sql
DECLARE @__today_0 date = '2023-02-07';
DECLARE @__dayOfWeek_1 int = 2;
DECLARE @__time_2 time = '19:53:40.4798052';

SELECT [s].[Id], [s].[Founded], [s].[Name], [o0].[SchoolId], [o0].[Id], [o0].[ClosesAt], [o0].[DayOfWeek], [o0].[OpensAt]
FROM [Schools] AS [s]
LEFT JOIN [OpeningHours] AS [o0] ON [s].[Id] = [o0].[SchoolId]
WHERE EXISTS (
    SELECT 1
    FROM [Term] AS [t]
    WHERE [s].[Id] = [t].[SchoolId] AND [t].[FirstDay] <= @__today_0 AND [t].[LastDay] >= @__today_0) AND EXISTS (
    SELECT 1
    FROM [OpeningHours] AS [o]
    WHERE [s].[Id] = [o].[SchoolId] AND [o].[DayOfWeek] = @__dayOfWeek_1 AND [o].[OpensAt] < @__time_2 AND [o].[ClosesAt] >= @__time_2)
ORDER BY [s].[Id], [o0].[SchoolId]
```

`DateOnly` and `TimeOnly` can also be used in JSON columns. For example, `OpeningHours` can be saved as a JSON document, resulting in data that looks like this:

| Column       | Value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               |
|--------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Id           | 2                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   |
| Name         | Farr High School                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    |
| Founded      | 1964-05-01                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          |
| OpeningHours | <pre>[<br>  { "DayOfWeek": "Sunday", "ClosesAt": null, "OpensAt": null },<br>  { "DayOfWeek": "Monday", "ClosesAt": "15:35:00", "OpensAt": "08:45:00" },<br>  { "DayOfWeek": "Tuesday", "ClosesAt": "15:35:00", "OpensAt": "08:45:00" },<br>  { "DayOfWeek": "Wednesday", "ClosesAt": "15:35:00", "OpensAt": "08:45:00" },<br>  { "DayOfWeek": "Thursday", "ClosesAt": "15:35:00", "OpensAt": "08:45:00" },<br>  { "DayOfWeek": "Friday", "ClosesAt": "12:50:00", "OpensAt": "08:45:00" },<br>  { "DayOfWeek": "Saturday", "ClosesAt": null, "OpensAt": null }<br>] |

Combining two features from EF8, we can now query for opening hours by indexing into the JSON collection. For example:

<!--
            openSchools = await context.Schools
                .Where(
                    s => s.Terms.Any(
                             t => t.FirstDay <= today
                                  && t.LastDay >= today)
                         && s.OpeningHours[(int)dayOfWeek].OpensAt < time
                         && s.OpeningHours[(int)dayOfWeek].ClosesAt >= time)
                .ToListAsync();
-->
[!code-csharp[OpenSchoolsJson](../../../../samples/core/Miscellaneous/NewInEFCore8/DateOnlyTimeOnlySample.cs?name=OpenSchoolsJson)]

This query translates to the following SQL, as shown by <xref:Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToQueryString%2A>:

```sql
DECLARE @__today_0 date = '2023-02-07';
DECLARE @__dayOfWeek_1 int = 2;
DECLARE @__time_2 time = '20:14:34.7795877';

SELECT [s].[Id], [s].[Founded], [s].[Name], [s].[OpeningHours]
FROM [Schools] AS [s]
WHERE EXISTS (
    SELECT 1
    FROM [Term] AS [t]
    WHERE [s].[Id] = [t].[SchoolId] AND [t].[FirstDay] <= @__today_0
      AND [t].[LastDay] >= @__today_0)
      AND CAST(JSON_VALUE([s].[OpeningHours],'$[' + CAST(CAST(@__dayOfWeek_1 AS int) AS nvarchar(max)) + '].OpensAt') AS time) < @__time_2
      AND CAST(JSON_VALUE([s].[OpeningHours],'$[' + CAST(CAST(@__dayOfWeek_1 AS int) AS nvarchar(max)) + '].ClosesAt') AS time) >= @__time_2
```

Finally, updates and deletes can be accomplished with [tracking and SaveChanges](xref:core/saving/basic), or using [ExecuteUpdate/ExecuteDelete](xref:core/what-is-new/ef-core-7#executeupdate-and-executedelete-bulk-updates). For example:

<!--
        await context.Schools
            .Where(e => e.Terms.Any(t => t.LastDay.Year == 2022))
            .SelectMany(e => e.Terms)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.LastDay, t => t.LastDay.AddDays(1)));
-->
[!code-csharp[UpdateForSnowDay](../../../../samples/core/Miscellaneous/NewInEFCore8/DateOnlyTimeOnlySample.cs?name=UpdateForSnowDay)]

This update translates to the following SQL:

```sql
UPDATE [t0]
SET [t0].[LastDay] = DATEADD(day, CAST(1 AS int), [t0].[LastDay])
FROM [Schools] AS [s]
INNER JOIN [Term] AS [t0] ON [s].[Id] = [t0].[SchoolId]
WHERE EXISTS (
    SELECT 1
    FROM [Term] AS [t]
    WHERE [s].[Id] = [t].[SchoolId] AND DATEPART(year, [t].[LastDay]) = 2022)
```

### Reverse engineer Synapse and Dynamics 365 TDS

EF8 reverse engineering (a.k.a. scaffolding from an existing database) now supports [Synapse Serverless SQL Pool](/azure/synapse-analytics/sql/on-demand-workspace-overview) and [Dynamics 365 TDS Endpoint](/power-apps/developer/data-platform/dataverse-sql-query) databases.

> [!WARNING]
> These database systems have differences from normal SQL Server and Azure SQL databases. These differences mean that not all EF Core functionality is supported when writing queries against or performing other operations with these database systems.

### Smaller enhancements included in Preview 1

In addition to the enhancements described above, EF8 Preview 1 also [includes many smaller enhancements](https://github.com/dotnet/efcore/issues?q=is%3Aissue+label%3Atype-enhancement+milestone%3A8.0.0-preview1+is%3Aclosed). Some of these relate to the internal workings of EF Core, even excluding these, there are many that may be of interest to application developers. These include:

- [Translate ElementAt(OrDefault)](https://github.com/dotnet/efcore/issues/17066)
- [Translate ToString() on a string column](https://github.com/dotnet/efcore/issues/20839)
- [Generic overload of ConventionSetBuilder.Remove](https://github.com/dotnet/efcore/issues/29476)
- [Allow UseSequence and HiLo on non-key properties](https://github.com/dotnet/efcore/issues/29758)
- [Pass query tracking behavior to materialization interceptor](https://github.com/dotnet/efcore/issues/29910)
- [Use case-insensitive string key comparisons on SQL Server](https://github.com/dotnet/efcore/issues/27526)
- [Allow value converters to change the DbType](https://github.com/dotnet/efcore/issues/24771)
- [Resolve application services in EF services](https://github.com/dotnet/efcore/issues/13540)
- [Numeric rowersion properties automatically convert to binary](https://github.com/dotnet/efcore/issues/12434)
- [Allow transfer of ownership of DbConnection from application to DbContext](https://github.com/dotnet/efcore/issues/24199)
- [Provide more information when 'No DbContext was found' error is generated](https://github.com/dotnet/efcore/issues/18715)

## New in EF8 Preview 2

### JSON Columns for SQLite

EF7 introduced support for mapping to JSON columns when using Azure SQL/SQL Server. EF8 extends this support to SQLite databases. As for the SQL Server support, this includes

- Mapping of aggregates built from .NET types to JSON documents stored in SQLite columns
- Queries into JSON columns, such as filtering and sorting by the elements of the documents
- Queries that project elements out of the JSON document into results
- Updating and saving changes to JSON documents

The existing [documentation from What's New in EF7](xref:core/what-is-new/ef-core-7#json-columns) provides detailed information on JSON mapping, queries, and updates. This documentation now also applies to SQLite.

> [!TIP]
> The code shown in the EF7 documentation has been updated to also run on SQLite can can be found in [JsonColumnsSample.cs](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Miscellaneous/NewInEFCore8/JsonColumnsSample.cs).

#### Queries into JSON columns

Queries into JSON columns on SQLite use the `json_extract` function. For example, the "authors in Chigley" query from the documentation referenced above:

<!--
        var authorsInChigley = await context.Authors
            .Where(author => author.Contact.Address.City == "Chigley")
            .ToListAsync();
-->
[!code-csharp[AuthorsInChigley](../../../../samples/core/Miscellaneous/NewInEFCore8/JsonColumnsSample.cs?name=AuthorsInChigley)]

Is translated to the following SQL when using SQLite:

```sql
SELECT "a"."Id", "a"."Name", "a"."Contact"
FROM "Authors" AS "a"
WHERE json_extract("a"."Contact", '$.Address.City') = 'Chigley'
```

#### Updating JSON columns

For updates, EF uses the `json_set` function on SQLite. For example, when updating a single property in a document:

<!--
        var arthur = await context.Authors.SingleAsync(author => author.Name.StartsWith("Arthur"));

        arthur.Contact.Address.Country = "United Kingdom";

        await context.SaveChangesAsync();
-->
[!code-csharp[UpdateProperty](../../../../samples/core/Miscellaneous/NewInEFCore8/JsonColumnsSample.cs?name=UpdateProperty)]

EF generates the following parameters:

```text
info: 3/10/2023 10:51:33.127 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command)
      Executed DbCommand (0ms) [Parameters=[@p0='["United Kingdom"]' (Nullable = false) (Size = 18), @p1='4'], CommandType='Text', CommandTimeout='30']
```

Which use the `json_set` function on SQLite:

```sql
UPDATE "Authors" SET "Contact" = json_set("Contact", '$.Address.Country', json_extract(@p0, '$[0]'))
WHERE "Id" = @p1
RETURNING 1;
```

### SQL Server HierarchyId

Azure SQL and SQL Server have a special data type called [`hierarchyid`](/sql/t-sql/data-types/hierarchyid-data-type-method-reference) that is used to store [hierarchical data](/sql/relational-databases/hierarchical-data-sql-server). In this case, "hierarchical data" essentially means data that forms a tree structure, where each item can have a parent and/or children. Examples of such data are:

- An organizational structure
- A file system
- A set of tasks in a project
- A taxonomy of language terms
- A graph of links between Web pages

The database is then able to run queries against this data using its hierarchical structure. For example, a query can find ancestors and dependents of given items, or find all items at a certain depth in the hierarchy.

#### HierarchyId support in .NET and EF Core

Official support for the SQL Server `hierarchyid` type has only recently come to modern .NET platforms (i.e. ".NET Core"). This support is in the form of the [Microsoft.SqlServer.Types](https://www.nuget.org/packages/Microsoft.SqlServer.Types) NuGet package, which brings in low-level SQL Server-specific types. In this case, the low-level type is called `SqlHierarchyId`.

At the next level, a new [Microsoft.EntityFrameworkCore.SqlServer.Abstractions](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.SqlServer.Abstractions) package has been introduced, which includes a higher-level `HierarchyId` type intended for use in entity types.

> [!TIP]
> The `HierarchyId` type is more idiomatic to the norms of .NET than `SqlHierarchyId`, which is instead modeled after how .NET Framework types are hosted inside the SQL Server database engine.  `HierarchyId` is designed to work with EF Core, but it can also be used outside of EF Core in other applications. The `Microsoft.EntityFrameworkCore.SqlServer.Abstractions` package doesn't reference any other packages, and so has minimal impact on deployed application size and dependencies.

Use of `HierarchyId` for EF Core functionality such as queries and updates requires the [Microsoft.EntityFrameworkCore.SqlServer.HierarchyId](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.SqlServer.HierarchyId) package. This package brings in `Microsoft.EntityFrameworkCore.SqlServer.Abstractions` and `Microsoft.SqlServer.Types` as transitive dependencies, and so is often the only package needed. Once the package is installed, use of `HierarchyId` is enabled by calling `UseHierarchyId` as part of the application's call to `UseSqlServer`. For example:

```csharp
options.UseSqlServer(
    connectionString,
    x => x.UseHierarchyId());
```

> [!NOTE]
> Unofficial support for `hierarchyid` in EF Core has been available for many years via the [EntityFrameworkCore.SqlServer.HierarchyId](https://www.nuget.org/packages/EntityFrameworkCore.SqlServer.HierarchyId) package. This package has been maintained as a collaboration between the community and the EF team. Now that there is official support for `hierarchyid` in .NET, the code from this community package forms, with the permission of the original contributors, the basis for the official package described here. Many thanks to all those involved over the years, including [@aljones](https://github.com/aljones), [@cutig3r](https://github.com/cutig3r), [@huan086](https://github.com/huan086), [@kmataru](https://github.com/kmataru), [@mehdihaghshenas](https://github.com/mehdihaghshenas), and [@vyrotek](https://github.com/vyrotek)

#### Modeling hierarchies

The `HierarchyId` type can be used for properties of an entity type. For example, assume we want to model the paternal family tree of some fictional [halflings](https://en.wikipedia.org/wiki/Halfling). In the entity type for `Halfling`, a `HierarchyId` property can be used to locate each halfling in the family tree.

<!--
    public class Halfling
    {
        public Halfling(HierarchyId pathFromPatriarch, string name, int? yearOfBirth = null)
        {
            PathFromPatriarch = pathFromPatriarch;
            Name = name;
            YearOfBirth = yearOfBirth;
        }

        public int Id { get; private set; }
        public HierarchyId PathFromPatriarch { get; set; }
        public string Name { get; set; }
        public int? YearOfBirth { get; set; }
    }
-->
[!code-csharp[Halfling](../../../../samples/core/Miscellaneous/NewInEFCore8/HierarchyIdSample.cs?name=Halfling)]

> [!TIP]
> The code shown here and in the examples below comes from [HierarchyIdSample.cs](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Miscellaneous/NewInEFCore8/HierarchyIdSample.cs).

> [!TIP]
> If desired, `HierarchyId` is suitable for use as a key property type.

In this case, the family tree is rooted with the patriarch of the family. Each halfling can be traced from the patriarch down the tree using its `PathFromPatriarch` property. SQL Server uses a compact binary format for these paths, but it is common to parse to and from a human-readable string representation when when working with code. In this representation, the position at each level is separated by a `/` character. For example, consider the family tree in the diagram below:

![Halfling family tree](familytree.png)

In this tree:

- Balbo is at the root of the tree, represented by `/`.
- Balbo has five children, represented by `/1/`, `/2/`, `/3/`, `/4/`, and `/5/`.
- Balbo's first child, Mungo, also has five children, represented by `/1/1/`, `/1/2/`, `/1/3/`, `/1/4/`, and `/1/5/`. Notice that the `HierarchyId` for Balbo (`/1/`) is the prefix for all his children.
- Similarly, Balbo's third child, Ponto, has two children, represented by `/3/1/` and `/3/2/`. Again the each of these children is prefixed by the `HierarchyId` for Ponto, which is represented as `/3/`.
- And so on down the tree...

The following code inserts this family tree into a database using EF Core:

<!--
            await AddRangeAsync(
                new Halfling(HierarchyId.Parse("/"), "Balbo", 1167),
                new Halfling(HierarchyId.Parse("/1/"), "Mungo", 1207),
                new Halfling(HierarchyId.Parse("/2/"), "Pansy", 1212),
                new Halfling(HierarchyId.Parse("/3/"), "Ponto", 1216),
                new Halfling(HierarchyId.Parse("/4/"), "Largo", 1220),
                new Halfling(HierarchyId.Parse("/5/"), "Lily", 1222),
                new Halfling(HierarchyId.Parse("/1/1/"), "Bungo", 1246),
                new Halfling(HierarchyId.Parse("/1/2/"), "Belba", 1256),
                new Halfling(HierarchyId.Parse("/1/3/"), "Longo", 1260),
                new Halfling(HierarchyId.Parse("/1/4/"), "Linda", 1262),
                new Halfling(HierarchyId.Parse("/1/5/"), "Bingo", 1264),
                new Halfling(HierarchyId.Parse("/3/1/"), "Rosa", 1256),
                new Halfling(HierarchyId.Parse("/3/2/"), "Polo"),
                new Halfling(HierarchyId.Parse("/4/1/"), "Fosco", 1264),
                new Halfling(HierarchyId.Parse("/1/1/1/"), "Bilbo", 1290),
                new Halfling(HierarchyId.Parse("/1/3/1/"), "Otho", 1310),
                new Halfling(HierarchyId.Parse("/1/5/1/"), "Falco", 1303),
                new Halfling(HierarchyId.Parse("/3/2/1/"), "Posco", 1302),
                new Halfling(HierarchyId.Parse("/3/2/2/"), "Prisca", 1306),
                new Halfling(HierarchyId.Parse("/4/1/1/"), "Dora", 1302),
                new Halfling(HierarchyId.Parse("/4/1/2/"), "Drogo", 1308),
                new Halfling(HierarchyId.Parse("/4/1/3/"), "Dudo", 1311),
                new Halfling(HierarchyId.Parse("/1/3/1/1/"), "Lotho", 1310),
                new Halfling(HierarchyId.Parse("/1/5/1/1/"), "Poppy", 1344),
                new Halfling(HierarchyId.Parse("/3/2/1/1/"), "Ponto", 1346),
                new Halfling(HierarchyId.Parse("/3/2/1/2/"), "Porto", 1348),
                new Halfling(HierarchyId.Parse("/3/2/1/3/"), "Peony", 1350),
                new Halfling(HierarchyId.Parse("/4/1/2/1/"), "Frodo", 1368),
                new Halfling(HierarchyId.Parse("/4/1/3/1/"), "Daisy", 1350),
                new Halfling(HierarchyId.Parse("/3/2/1/1/1/"), "Angelica", 1381));

            await SaveChangesAsync();
-->
[!code-csharp[AddRangeAsync](../../../../samples/core/Miscellaneous/NewInEFCore8/HierarchyIdSample.cs?name=AddRangeAsync)]

> [!TIP]
> If needed, decimal values can be used to create new nodes between two existing nodes. For example, `/3/2.5/2/` goes between `/3/2/2/` and `/3/3/2/`.

#### Querying hierarchies

`HierarchyId` exposes several methods that can be used in LINQ queries.

| Method                                                           | Description                                                                                                                                                                |
|------------------------------------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `GetAncestor(int n)`                                             | Gets the node `n` levels up the hierarchical tree.                                                                                                                         |
| `GetDescendant(HierarchyId? child1, HierarchyId? child2)`        | Gets the value of a descendant node that is greater than `child1` and less than `child2`.                                                                                  |
| `GetLevel()`                                                     | Gets the level of this node in the hierarchical tree.                                                                                                                      |
| `GetReparentedValue(HierarchyId? oldRoot, HierarchyId? newRoot)` | Gets a value representing the location of a new node that has a path from `newRoot` equal to the path from `oldRoot` to this, effectively moving this to the new location. |
| `IsDescendantOf(HierarchyId? parent)`                            | Gets a value indicating whether this node is a descendant of `parent`.                                                                                                     |

In addition, the operators `==`, `!=`, `<`, `<=`, `>` and `>=` can be used.

The following are examples of using these methods in LINQ queries.

**Get entities at a given level in the tree**

The following query uses `GetLevel` to return all halflings at a given level in the family tree:

<!--
            var generation = await context.Halflings.Where(halfling => halfling.PathFromPatriarch.GetLevel() == level).ToListAsync();
-->
[!code-csharp[GetLevel](../../../../samples/core/Miscellaneous/NewInEFCore8/HierarchyIdSample.cs?name=GetLevel)]

This translates to the following SQL:

```sql
SELECT [h].[Id], [h].[Name], [h].[PathFromPatriarch], [h].[YearOfBirth]
FROM [Halflings] AS [h]
WHERE [h].[PathFromPatriarch].GetLevel() = @__level_0
```

Running this in a loop we can get the halflings for every generation:

```text
Generation 0: Balbo
Generation 1: Mungo, Pansy, Ponto, Largo, Lily
Generation 2: Bungo, Belba, Longo, Linda, Bingo, Rosa, Polo, Fosco
Generation 3: Bilbo, Otho, Falco, Posco, Prisca, Dora, Drogo, Dudo
Generation 4: Lotho, Poppy, Ponto, Porto, Peony, Frodo, Daisy
Generation 5: Angelica
```

**Get the direct ancestor of an entity**

The following query uses `GetAncestor` to find the direct ancestor of a halfling, given that halfling's name:

<!--
        async Task<Halfling?> FindDirectAncestor(string name)
            => await context.Halflings
                .SingleOrDefaultAsync(
                    ancestor => ancestor.PathFromPatriarch == context.Halflings
                        .Single(descendent => descendent.Name == name).PathFromPatriarch
                        .GetAncestor(1));
-->
[!code-csharp[FindDirectAncestor](../../../../samples/core/Miscellaneous/NewInEFCore8/HierarchyIdSample.cs?name=FindDirectAncestor)]

This translates to the following SQL:

```sql
SELECT TOP(2) [h].[Id], [h].[Name], [h].[PathFromPatriarch], [h].[YearOfBirth]
FROM [Halflings] AS [h]
WHERE [h].[PathFromPatriarch] = (
    SELECT TOP(1) [h0].[PathFromPatriarch]
    FROM [Halflings] AS [h0]
    WHERE [h0].[Name] = @__name_0).GetAncestor(1)
```

Running this query for the halfling "Bilbo" returns "Bungo".

**Get the direct descendents of an entity**

The following query also uses `GetAncestor`, but this time to find the direct descendents of a halfling, given that halfling's name:

<!--
        IQueryable<Halfling> FindDirectDescendents(string name)
            => context.Halflings.Where(
                descendent => descendent.PathFromPatriarch.GetAncestor(1) == context.Halflings
                    .Single(ancestor => ancestor.Name == name).PathFromPatriarch);
-->
[!code-csharp[FindDirectDescendents](../../../../samples/core/Miscellaneous/NewInEFCore8/HierarchyIdSample.cs?name=FindDirectDescendents)]

This translates to the following SQL:

```sql
SELECT [h].[Id], [h].[Name], [h].[PathFromPatriarch], [h].[YearOfBirth]
FROM [Halflings] AS [h]
WHERE [h].[PathFromPatriarch].GetAncestor(1) = (
    SELECT TOP(1) [h0].[PathFromPatriarch]
    FROM [Halflings] AS [h0]
    WHERE [h0].[Name] = @__name_0)
```

Running this query for the halfling "Mungo" returns "Bungo", "Belba", "Longo", and "Linda".

**Get all ancestors of an entity**

`GetAncestor` is useful for searching up or down a single level, or, indeed, a specified number of levels. On the other hand, `IsDescendantOf` is useful for finding all ancestors or dependents. For example, the following query uses `IsDescendantOf` to find the all the ancestors of a halfling, given that halfling's name:

<!--
        IQueryable<Halfling> FindAllAncestors(string name)
            => context.Halflings.Where(
                    ancestor => context.Halflings
                        .Single(
                            descendent =>
                                descendent.Name == name
                                && ancestor.Id != descendent.Id)
                        .PathFromPatriarch.IsDescendantOf(ancestor.PathFromPatriarch))
                .OrderByDescending(ancestor => ancestor.PathFromPatriarch.GetLevel());
-->
[!code-csharp[FindAllAncestors](../../../../samples/core/Miscellaneous/NewInEFCore8/HierarchyIdSample.cs?name=FindAllAncestors)]

> [!IMPORTANT]
> `IsDescendantOf` returns true for itself, which is why it is filtered out in the query above.

This translates to the following SQL:

```sql
SELECT [h].[Id], [h].[Name], [h].[PathFromPatriarch], [h].[YearOfBirth]
FROM [Halflings] AS [h]
WHERE (
    SELECT TOP(1) [h0].[PathFromPatriarch]
    FROM [Halflings] AS [h0]
    WHERE [h0].[Name] = @__name_0 AND [h].[Id] <> [h0].[Id]).IsDescendantOf([h].[PathFromPatriarch]) = CAST(1 AS bit)
ORDER BY [h].[PathFromPatriarch].GetLevel() DESC
```

Running this query for the halfling "Bilbo" returns "Bungo", "Mungo", and "Balbo".

**Get all descendents of an entity**

The following query also uses `IsDescendantOf`, but this time to all the descendents of a halfling, given that halfling's name:

<!--
        IQueryable<Halfling> FindAllDescendents(string name)
            => context.Halflings.Where(
                    descendent => descendent.PathFromPatriarch.IsDescendantOf(
                        context.Halflings
                            .Single(
                                ancestor =>
                                    ancestor.Name == name
                                    && descendent.Id != ancestor.Id)
                            .PathFromPatriarch))
                .OrderBy(descendent => descendent.PathFromPatriarch.GetLevel());
-->
[!code-csharp[FindAllDescendents](../../../../samples/core/Miscellaneous/NewInEFCore8/HierarchyIdSample.cs?name=FindAllDescendents)]

This translates to the following SQL:

```sql
SELECT [h].[Id], [h].[Name], [h].[PathFromPatriarch], [h].[YearOfBirth]
FROM [Halflings] AS [h]
WHERE [h].[PathFromPatriarch].IsDescendantOf((
    SELECT TOP(1) [h0].[PathFromPatriarch]
    FROM [Halflings] AS [h0]
    WHERE [h0].[Name] = @__name_0 AND [h].[Id] <> [h0].[Id])) = CAST(1 AS bit)
ORDER BY [h].[PathFromPatriarch].GetLevel()
```

Running this query for the halfling "Mungo" returns "Bungo", "Belba", "Longo", "Linda", "Bingo", "Bilbo", "Otho", "Falco", "Lotho", and "Poppy".

**Finding a common ancestor**

One of the most common questions asked about this particular family tree is, "who is the common ancestor of Frodo and Bilbo?" We can use `IsDescendantOf` to write such a query:

<!--
        async Task<Halfling?> FindCommonAncestor(Halfling first, Halfling second)
            => await context.Halflings
                .Where(
                    ancestor => first.PathFromPatriarch.IsDescendantOf(ancestor.PathFromPatriarch)
                                && second.PathFromPatriarch.IsDescendantOf(ancestor.PathFromPatriarch))
                .OrderByDescending(ancestor => ancestor.PathFromPatriarch.GetLevel())
                .FirstOrDefaultAsync();
-->
[!code-csharp[FindCommonAncestor](../../../../samples/core/Miscellaneous/NewInEFCore8/HierarchyIdSample.cs?name=FindCommonAncestor)]

This translates to the following SQL:

```sql
SELECT TOP(1) [h].[Id], [h].[Name], [h].[PathFromPatriarch], [h].[YearOfBirth]
FROM [Halflings] AS [h]
WHERE @__first_PathFromPatriarch_0.IsDescendantOf([h].[PathFromPatriarch]) = CAST(1 AS bit)
  AND @__second_PathFromPatriarch_1.IsDescendantOf([h].[PathFromPatriarch]) = CAST(1 AS bit)
ORDER BY [h].[PathFromPatriarch].GetLevel() DESC
```

Running this query with "Bilbo" and "Frodo" tells us that their common ancestor is "Balbo".

#### Updating hierarchies

The normal [change tracking](xref:core/change-tracking/index) and [SaveChanges](xref:core/saving/basic) mechanisms can be used to update `hierarchyid` columns.

**Re-parenting a sub-hierarchy**

For example, I'm sure we all remember the scandal of SR 1752 (a.k.a. "LongoGate") when DNA testing revealed that Longo was not in fact the son of Mungo, but actually the son of Ponto! One fallout from this scandal was that the family tree needed to be re-written. In particular, Longo and all his descendents needed to be re-parented from Mungo to Ponto. `GetReparentedValue` can be used to do this. For example, first "Longo" and all his descendents are queried:

<!--
        var longoAndDescendents = await context.Halflings.Where(
                descendent => descendent.PathFromPatriarch.IsDescendantOf(
                    context.Halflings.Single(ancestor => ancestor.Name == "Longo").PathFromPatriarch))
            .ToListAsync();
-->
[!code-csharp[LongoAndDescendents](../../../../samples/core/Miscellaneous/NewInEFCore8/HierarchyIdSample.cs?name=LongoAndDescendents)]

Then `GetReparentedValue` is used to update the `HierarchyId` for Longo and each descendent, followed by a call to `SaveChangesAsync`:

<!--
        foreach (var descendent in longoAndDescendents)
        {
            descendent.PathFromPatriarch
                = descendent.PathFromPatriarch.GetReparentedValue(
                    mungo.PathFromPatriarch, ponto.PathFromPatriarch)!;
        }

        await context.SaveChangesAsync();
-->
[!code-csharp[GetReparentedValue](../../../../samples/core/Miscellaneous/NewInEFCore8/HierarchyIdSample.cs?name=GetReparentedValue)]

This results in the following database update:

```sql
SET NOCOUNT ON;
UPDATE [Halflings] SET [PathFromPatriarch] = @p0
OUTPUT 1
WHERE [Id] = @p1;
UPDATE [Halflings] SET [PathFromPatriarch] = @p2
OUTPUT 1
WHERE [Id] = @p3;
UPDATE [Halflings] SET [PathFromPatriarch] = @p4
OUTPUT 1
WHERE [Id] = @p5;
```

Using these parameters:

```text
 @p1='9',
 @p0='0x7BC0' (Nullable = false) (Size = 2) (DbType = Object),
 @p3='16',
 @p2='0x7BD6' (Nullable = false) (Size = 2) (DbType = Object),
 @p5='23',
 @p4='0x7BD6B0' (Nullable = false) (Size = 3) (DbType = Object)
 ```

> [!NOTE]
> The parameters values for `HierarchyId` properties are sent to the database in their compact, binary format.

Following the update, querying for the descendents of "Mungo" returns "Bungo", "Belba", "Linda", "Bingo", "Bilbo", "Falco", and "Poppy", while querying for the descendents of "Ponto" returns "Longo", "Rosa", "Polo", "Otho", "Posco", "Prisca", "Lotho", "Ponto", "Porto", "Peony", and "Angelica".

### Smaller enhancements included in Preview 2

In addition to the enhancements described above, EF8 Preview 2 also includes some [smaller enhancements](https://github.com/dotnet/efcore/issues?q=is%3Aissue+label%3Atype-enhancement+milestone%3A8.0.0-preview2+is%3Aclosed):

- [Configuration to opt out of occasionally problematic SaveChanges optimizations](https://github.com/dotnet/efcore/issues/29916)
- [Add convention types for triggers](https://github.com/dotnet/efcore/issues/28687)
