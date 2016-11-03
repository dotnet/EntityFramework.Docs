---
title: Raw SQL Queries
author: rowanmiller
ms.author: rowmil
manager: rowanmiller
ms.date: 10/27/2016
ms.topic: article
ms.assetid: 70aae9b5-8743-4557-9c5d-239f688bf418
ms.prod: entity-framework
uid: core/querying/raw-sql
---
# Raw SQL Queries

> [!NOTE]
> This documentation is for EF Core. For EF6.x, see [Entity Framework 6](../../ef6/index.md).

Entity Framework Core allows you to drop down to raw SQL queries when working with a relational database. This can be useful if the query you want to perform can't be expressed using LINQ, or if using a LINQ query is resulting in inefficient SQL being sent to the database.

> [!TIP]
> You can view this article's [sample](https://github.com/aspnet/EntityFramework.Docs/tree/master/samples/core/Querying) on GitHub.

## Limitations

There are a couple of limitations to be aware of when using raw SQL queries:
* SQL queries can only be used to return entity types that are part of your model. There is an enhancement on our backlog to [enable returning ad-hoc types from raw SQL queries](https://github.com/aspnet/EntityFramework/issues/1862).

* The SQL query must return data for all properties of the entity type.

* The column names in the result set must match the column names that properties are mapped to. Note this is different from EF6.x where property/column mapping was ignored for raw SQL queries and result set column names had to match the property names.

* The SQL query cannot contain related data. However, in many cases you can compose on top of the query using the `Include` operator to return related data (see [Including related data](#including-related-data)).

## Basic raw SQL queries

You can use the *FromSql* extension method to begin a LINQ query based on a raw SQL query.

<!-- [!code-csharp[Main](samples/core/Querying/Querying/RawSQL/Sample.cs)] -->
````csharp
var blogs = context.Blogs
    .FromSql("SELECT * FROM dbo.Blogs")
    .ToList();
````

Raw SQL queries can be used to execute a stored procedure.

<!-- [!code-csharp[Main](samples/core/Querying/Querying/RawSQL/Sample.cs)] -->
````csharp
var blogs = context.Blogs
    .FromSql("EXECUTE dbo.GetMostPopularBlogs")
    .ToList();
````

## Passing parameters

As with any API that accepts SQL, it is important to parameterize any user input to protect against a SQL injection attack. You can include parameter placeholders in the SQL query string and then supply parameter values as additional arguments. Any parameter values you supply will automatically be converted to a `DbParameter`.

The following example passes a single parameter to a stored procedure. While this may look like `String.Format` syntax, the supplied value is wrapped in a parameter and the generated parameter name inserted where the `{0}` placeholder was specified.

<!-- [!code-csharp[Main](samples/core/Querying/Querying/RawSQL/Sample.cs)] -->
````csharp
   var user = "johndoe";

   var blogs = context.Blogs
.FromSql("EXECUTE dbo.GetMostPopularBlogsForUser {0}", user)
.ToList();
````

You can also construct a DbParameter and supply it as a parameter value. This allows you to use named parameters in the SQL query string

<!-- [!code-csharp[Main](samples/core/Querying/Querying/RawSQL/Sample.cs)] -->
````csharp
var user = "johndoe";

var blogs = context.Blogs
    .FromSql("EXECUTE dbo.GetMostPopularBlogsForUser {0}", user)
    .ToList();
````

## Composing with LINQ

If the SQL query can be composed on in the database, then you can compose on top of the initial raw SQL query using LINQ operators. SQL queries that can be composed on being with the `SELECT` keyword.

The following example uses a raw SQL query that selects from a Table-Valued Function (TVF) and then composes on it using LINQ to perform filtering and sorting.

<!-- [!code-csharp[Main](samples/core/Querying/Querying/RawSQL/Sample.cs)] -->
````csharp
   var searchTerm = ".NET";

   var blogs = context.Blogs
.FromSql("SELECT * FROM dbo.SearchBlogs {0}", searchTerm)
.Where(b => b.Rating > 3)
.OrderByDescending(b => b.Rating)
.ToList();
````

### Including related data

Composing with LINQ operators can be used to include related data in the query.

<!-- [!code-csharp[Main](samples/core/Querying/Querying/RawSQL/Sample.cs)] -->
````csharp
var user = new SqlParameter("user", "johndoe");

var blogs = context.Blogs
    .FromSql("EXECUTE dbo.GetMostPopularBlogsForUser @user", user)
    .ToList();
````
