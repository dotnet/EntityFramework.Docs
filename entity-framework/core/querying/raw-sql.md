---
title: Raw SQL Queries - EF Core
author: rowanmiller
ms.date: 10/27/2016
ms.assetid: 70aae9b5-8743-4557-9c5d-239f688bf418
uid: core/querying/raw-sql
---
# Raw SQL Queries

Entity Framework Core allows you to drop down to raw SQL queries when working with a relational database. This can be useful if the query you want to perform can't be expressed using LINQ, or if using a LINQ query is resulting in inefficient SQL queries. Raw SQL queries can return entity types or, starting with EF Core 2.1, [query types](xref:core/modeling/query-types) that are part of your model.

> [!TIP]  
> You can view this article's [sample](https://github.com/aspnet/EntityFramework.Docs/tree/master/samples/core/Querying) on GitHub.

## Basic raw SQL queries

You can use the *FromSql* extension method to begin a LINQ query based on a raw SQL query.

<!-- [!code-csharp[Main](samples/core/Querying/Querying/RawSQL/Sample.cs)] -->
``` csharp
var blogs = context.Blogs
    .FromSql("SELECT * FROM dbo.Blogs")
    .ToList();
```

Raw SQL queries can be used to execute a stored procedure.

<!-- [!code-csharp[Main](samples/core/Querying/Querying/RawSQL/Sample.cs)] -->
``` csharp
var blogs = context.Blogs
    .FromSql("EXECUTE dbo.GetMostPopularBlogs")
    .ToList();
```

## Passing parameters

As with any API that accepts SQL, it is important to parameterize any user input to protect against a SQL injection attack. You can include parameter placeholders in the SQL query string and then supply parameter values as additional arguments. Any parameter values you supply will automatically be converted to a `DbParameter`.

The following example passes a single parameter to a stored procedure. While this may look like `String.Format` syntax, the supplied value is wrapped in a parameter and the generated parameter name inserted where the `{0}` placeholder was specified.

<!-- [!code-csharp[Main](samples/core/Querying/Querying/RawSQL/Sample.cs)] -->
``` csharp
var user = "johndoe";

var blogs = context.Blogs
    .FromSql("EXECUTE dbo.GetMostPopularBlogsForUser {0}", user)
    .ToList();
```

This is the same query but using string interpolation syntax, which is supported in EF Core 2.0 and above:

<!-- [!code-csharp[Main](samples/core/Querying/Querying/RawSQL/Sample.cs)] -->
``` csharp
var user = "johndoe";

var blogs = context.Blogs
    .FromSql($"EXECUTE dbo.GetMostPopularBlogsForUser {user}")
    .ToList();
```

You can also construct a DbParameter and supply it as a parameter value. This allows you to use named parameters in the SQL query string.

<!-- [!code-csharp[Main](samples/core/Querying/Querying/RawSQL/Sample.cs)] -->
``` csharp
var user = new SqlParameter("user", "johndoe");

var blogs = context.Blogs
    .FromSql("EXECUTE dbo.GetMostPopularBlogsForUser @user", user)
    .ToList();
```

## Composing with LINQ

If the SQL query can be composed on in the database, then you can compose on top of the initial raw SQL query using LINQ operators. SQL queries that can be composed on begin with the `SELECT` keyword.

The following example uses a raw SQL query that selects from a Table-Valued Function (TVF) and then composes on it using LINQ to perform filtering and sorting.

<!-- [!code-csharp[Main](samples/core/Querying/Querying/RawSQL/Sample.cs)] -->
``` csharp
var searchTerm = ".NET";

var blogs = context.Blogs
    .FromSql($"SELECT * FROM dbo.SearchBlogs({searchTerm})")
    .Where(b => b.Rating > 3)
    .OrderByDescending(b => b.Rating)
    .ToList();
```

## Change Tracking

Queries that use the `FromSql()` follow the exact same change tracking rules as any other LINQ query in EF Core. For example, if the query projects entity types, the results will be tracked by default.  

The following example uses a raw SQL query that selects from a Table-Valued Function (TVF), then disables change tracking with teh call to .AsNoTracking():

<!-- [!code-csharp[Main](samples/core/Querying/Querying/RawSQL/Sample.cs)] -->
``` csharp
var searchTerm = ".NET";

var blogs = context.Query<SearchBlogsDto>()
    .FromSql($"SELECT * FROM dbo.SearchBlogs({searchTerm})")
    .AsNoTracking()
    .ToList();
```

## Including related data

The `Include()` method can be used to include related data, just like with any other LINQ query:

<!-- [!code-csharp[Main](samples/core/Querying/Querying/RawSQL/Sample.cs)] -->
``` csharp
var searchTerm = ".NET";

var blogs = context.Blogs
    .FromSql($"SELECT * FROM dbo.SearchBlogs({searchTerm})")
    .Include(b => b.Posts)
    .ToList();
```

## Limitations

There are a few limitations to be aware of when using raw SQL queries:

* The SQL query must return data for all properties of the entity or query type.

* The column names in the result set must match the column names that properties are mapped to. Note this is different from EF6 where property/column mapping was ignored for raw SQL queries and result set column names had to match the property names.

* The SQL query cannot contain related data. However, in many cases you can compose on top of the query using the `Include` operator to return related data (see [Including related data](#including-related-data)).

* `SELECT` statements passed to this method should generally be composable: If EF Core needs to evaluate additional query operators on the server (for example, to translate LINQ operators applied after `FromSql`), the supplied SQL will be treated as a subquery. This means that the SQL passed should not contain any characters or options that are not valid on a subquery, such as:
  * a trailing semicolon
  * On SQL Server, a trailing query-level hint (for example, `OPTION (HASH JOIN)`)
  * On SQL Server, an `ORDER BY` clause that is not accompanied of `TOP 100 PERCENT` in the `SELECT` clause

* SQL statements other than `SELECT` are recognized automatically as non-composable. As a consequence, the full results of stored procedures are always returned to the client and any LINQ operators applied after `FromSql` are evaluated in-memory.

> [!WARNING]  
> **Always use parameterization for raw SQL queries:** In addition to validating user input, always use parameterization for any values used in a raw SQL query/command. APIs that accept a raw SQL string such as `FromSql` and `ExecuteSqlCommand` allow values to be easily passed as parameters. Overloads of `FromSql` and `ExecuteSqlCommand` that accept FormattableString also allow using string interpolation syntaxt in a way that helps protect against SQL injection attacks. 
> 
> If you are using string concatenation or interpolation to dynamically build any part of the query string, or passing user input to statements or stored procedures that can execute those inputs as dynamic SQL, then you are responsible for validating any input to protect against SQL injection attacks.
