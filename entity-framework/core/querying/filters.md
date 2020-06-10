---
title: Global Query Filters - EF Core
author: anpete
ms.date: 11/03/2017
uid: core/querying/filters
---
# Global Query Filters

> [!NOTE]
> This feature was introduced in EF Core 2.0.

Global query filters are LINQ query predicates (a boolean expression typically passed to the LINQ *Where* query operator) applied to Entity Types in the metadata model (usually in *OnModelCreating*). Such filters are automatically applied to any LINQ queries involving those Entity Types, including Entity Types referenced indirectly, such as through the use of Include or direct navigation property references. Some common applications of this feature are:

* **Soft delete** - An Entity Type defines an *IsDeleted* property.
* **Multi-tenancy** - An Entity Type defines a *TenantId* property.

## Example

The following example shows how to use Global Query Filters to implement soft-delete and multi-tenancy query behaviors in a simple blogging model.

> [!TIP]
> You can view [multi-tenancy sample](https://github.com/dotnet/EntityFramework.Docs/tree/master/samples/core/QueryFilters) and [samples using navigations](https://github.com/dotnet/EntityFramework.Docs/tree/master/samples/core/QueryFiltersNavigations) on GitHub. 

First, define the entities:

[!code-csharp[Main](../../../samples/core/QueryFilters/Program.cs#Entities)]

Note the declaration of a _tenantId_ field on the _Blog_ entity. This will be used to associate each Blog instance with a specific tenant. Also defined is an _IsDeleted_ property on the _Post_ entity type. This is used to keep track of whether a _Post_ instance has been "soft-deleted". That is, the instance is marked as deleted without physically removing the underlying data.

Next, configure the query filters in _OnModelCreating_ using the `HasQueryFilter` API.

[!code-csharp[Main](../../../samples/core/QueryFilters/Program.cs#Configuration)]

The predicate expressions passed to the _HasQueryFilter_ calls will now automatically be applied to any LINQ queries for those types.

> [!TIP]
> Note the use of a DbContext instance level field: `_tenantId` used to set the current tenant. Model-level filters will use the value from the correct context instance (that is, the instance that is executing the query).

> [!NOTE]
> It is currently not possible to define multiple query filters on the same entity - only the last one will be applied. However, you can define a single filter with multiple conditions using the logical _AND_ operator ([`&&` in C#](https://docs.microsoft.com/dotnet/csharp/language-reference/operators/boolean-logical-operators#conditional-logical-and-operator-)).

## Use of navigations

Navigations can be used when defining global query filters. They are applied recursively - when navigations used in query filters are translated, query filters defined on referenced entities are also applied, potentially adding more navigations.

> [!NOTE]
> Currently EF Core does not detect cycles in global query filter definitions, so you should be careful when defining them. If specified incorrectly, this could lead to infinite loops during query translation.

## Accessing entity with query filter using reqiured navigation

> [!CAUTION]
> Using required navigation to access entity which has global query filter defined may lead to unexpected results. 

Required navigation expects the related entity to always be present. If required related entity is filtered out by the query filter, the parent entity could end up in unexpected state. This may result in returning fewer elements than expected. 

To illustrate the problem, we can use the `Blog` and `Post` entities specified above and the following _OnModelCreating_ method:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Blog>().HasMany(b => b.Posts).WithOne(p => p.Blog).IsRequired();
    modelBuilder.Entity<Blog>().HasQueryFilter(b => b.Url.Contains("fish"));
}
```

The model can be seeded with the following data:

[!code-csharp[Main](../../../samples/core/QueryFiltersNavigations/Program.cs#SeedData)]

The problem can be observed when executing two queries:

[!code-csharp[Main](../../../samples/core/QueryFiltersNavigations/Program.cs#Queries)]

With this setup, the first query returns all 6 `Post`s, however the second query only returns 3. This happens because _Include_ method in the second query loads the related `Blog` entities. Since the navigation between `Blog` and `Post` is required, EF Core uses `INNER JOIN` when constructing the query:

```SQL
SELECT [p].[PostId], [p].[BlogId], [p].[Content], [p].[IsDeleted], [p].[Title], [t].[BlogId], [t].[Name], [t].[Url]
FROM [Post] AS [p]
INNER JOIN (
    SELECT [b].[BlogId], [b].[Name], [b].[Url]
    FROM [Blogs] AS [b]
    WHERE CHARINDEX(N'fish', [b].[Url]) > 0
) AS [t] ON [p].[BlogId] = [t].[BlogId]
```

Use of the `INNER JOIN` filters out all `Post`s whose related `Blog`s have been removed by a global query filter. 

It can be addressed by using optional navigation instead of required. 
This way the first query stays the same as before, however the second query will now generate `LEFT JOIN` and return 6 results.

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Blog>().HasMany(b => b.Posts).WithOne(p => p.Blog).IsRequired(false);
    modelBuilder.Entity<Blog>().HasQueryFilter(b => b.Url.Contains("fish"));
}
```

Alternative approach is to specify consistent filters on both `Blog` and `Post` entities.
This way matching filters are applied to both `Blog` and `Post`. `Post`s that could end up in unexpected state are removed and both queries return 3 results. 

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Blog>().HasMany(b => b.Posts).WithOne(p => p.Blog).IsRequired();
    modelBuilder.Entity<Blog>().HasQueryFilter(b => b.Url.Contains("fish"));
    modelBuilder.Entity<Post>().HasQueryFilter(p => p.Blog.Url.Contains("fish"));
}
```

## Disabling Filters

Filters may be disabled for individual LINQ queries by using the `IgnoreQueryFilters()` operator.

[!code-csharp[Main](../../../samples/core/QueryFilters/Program.cs#IgnoreFilters)]

## Limitations

Global query filters have the following limitations:

* Filters can only be defined for the root Entity Type of an inheritance hierarchy.
