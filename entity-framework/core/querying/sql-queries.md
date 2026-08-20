---
title: SQL Queries - EF Core
description: Using SQL queries in Entity Framework Core
author: SamMonoRT
ms.date: 09/19/2022
uid: core/querying/sql-queries
---
# SQL Queries

Entity Framework Core allows you to drop down to SQL queries when working with a relational database. SQL queries are useful if the query you want can't be expressed using LINQ, or if a LINQ query causes EF to generate inefficient SQL. SQL queries can return regular entity types or [keyless entity types](xref:core/modeling/keyless-entity-types) that are part of your model.

## Basic SQL queries

You can use <xref:Microsoft.EntityFrameworkCore.RelationalQueryableExtensions.FromSql*> to begin a LINQ query based on a SQL query:

```csharp
var blogs = await context.Blogs
    .FromSql($"SELECT * FROM dbo.Blogs")
    .ToListAsync();
```

> [!NOTE]
>
> <xref:Microsoft.EntityFrameworkCore.RelationalQueryableExtensions.FromSql*> was introduced in EF Core 7.0. When using older versions, use <xref:Microsoft.EntityFrameworkCore.RelationalQueryableExtensions.FromSqlInterpolated*> instead.

SQL queries can be used to execute a stored procedure which returns entity data:

```csharp
var blogs = await context.Blogs
    .FromSql($"EXECUTE dbo.GetMostPopularBlogs")
    .ToListAsync();
```

> [!NOTE]
>
> <xref:Microsoft.EntityFrameworkCore.RelationalQueryableExtensions.FromSql*> can only be used directly on a `DbSet`. It cannot be composed over an arbitrary LINQ query.

## Passing parameters

> [!WARNING]
> **Pay close attention to parameterization when using SQL queries**
>
> When introducing any user-provided values into a SQL query, care must be taken to avoid SQL injection attacks. SQL injection occurs when a program integrates a user-provided string value into a SQL query, and the user-provided value is crafted to terminate the string and perform another malicious SQL operation. To learn more about SQL injection, [see this page](/sql/relational-databases/security/sql-injection).
>
> The <xref:Microsoft.EntityFrameworkCore.RelationalQueryableExtensions.FromSql*> and <xref:Microsoft.EntityFrameworkCore.RelationalQueryableExtensions.FromSqlInterpolated*> methods are safe against SQL injection, and always integrate parameter data as a separate SQL parameter. However, the <xref:Microsoft.EntityFrameworkCore.RelationalQueryableExtensions.FromSqlRaw*> method can be vulnerable to SQL injection attacks, if improperly used. See below for more details.

The following example passes a single parameter to a stored procedure by including a parameter placeholder in the SQL query string and providing an additional argument:

```csharp
var user = "johndoe";

var blogs = await context.Blogs
    .FromSql($"EXECUTE dbo.GetMostPopularBlogsForUser {user}")
    .ToListAsync();
```

While this syntax may look like regular C# [string interpolation](/dotnet/csharp/language-reference/tokens/interpolated), the supplied value is wrapped in a `DbParameter` and the generated parameter name inserted where the `{0}` placeholder was specified. This makes <xref:Microsoft.EntityFrameworkCore.RelationalQueryableExtensions.FromSql*> safe from SQL injection attacks, and sends the value efficiently and correctly to the database.

When executing stored procedures, it can be useful to use named parameters in the SQL query string, especially when the stored procedure has optional parameters:

```csharp
var user = new SqlParameter("user", "johndoe");

var blogs = await context.Blogs
    .FromSql($"EXECUTE dbo.GetMostPopularBlogsForUser @filterByUser={user}")
    .ToListAsync();
```

If you need more control over the database parameter being sent, you can also construct a `DbParameter` and supply it as a parameter value. This allows you to set the precise database type of the parameter, or facets such as its size, precision or length:

```csharp
var user = new SqlParameter("user", "johndoe");

var blogs = await context.Blogs
    .FromSql($"EXECUTE dbo.GetMostPopularBlogsForUser {user}")
    .ToListAsync();
```

> [!NOTE]
>
> The parameters you pass must exactly match the stored procedure definition. Pay special attention to the ordering of the parameters, taking care to not miss or misplace any of them - or consider using named parameter notation. Also, make sure the parameter types correspond, and that their facets (size, precision, scale) are set as needed.

### Dynamic SQL and parameters

<xref:Microsoft.EntityFrameworkCore.RelationalQueryableExtensions.FromSql*> and its parameterization should be used wherever possible. However, there are certain scenarios where SQL needs to be dynamically pieced together, and database parameters cannot be used. For example, let's assume that a C# variable holds the name of the a property to be filtered by. It may be tempting to use a SQL query such as the following:

```c#
var propertyName = "User";
var propertyValue = "johndoe";

var blogs = await context.Blogs
    .FromSql($"SELECT * FROM [Blogs] WHERE {propertyName} = {propertyValue}")
    .ToListAsync();
```

This code doesn't work, since databases do not allow parameterizing column names (or any other part of the schema).

First, it's important to consider the implications of dynamically constructing a query - via SQL or otherwise. Accepting a column name from a user may allow them to choose a column that isn't indexed, making the query run extremely slowly and overload your database; or it may allow them to choose a column containing data you don't want exposed. Except for truly dynamic scenarios, it's usually better to have two queries for two column names, rather than using parameterization to collapse them into a single query.

If you've decided you do want to dynamically construct your SQL, you'll have to use <xref:Microsoft.EntityFrameworkCore.RelationalQueryableExtensions.FromSqlRaw*>, which allows interpolating variable data directly into the SQL string, instead of using a database parameter:

```csharp
var columnName = "Url";
var columnValue = new SqlParameter("columnValue", "http://SomeURL");

var blogs = await context.Blogs
    .FromSqlRaw($"SELECT * FROM [Blogs] WHERE {columnName} = @columnValue", columnValue)
    .ToListAsync();
```

In the above code, the column name is inserted directly into the SQL, using C# string interpolation. It is your responsibility to make sure this string value is safe, sanitizing it if it comes from an unsafe origin; this means detecting special characters such as semicolons, comments, and other SQL constructs, and either escaping them properly or rejecting such inputs.

On the other hand, the column value is sent via a `DbParameter`, and is therefore safe in the face of SQL injection.

> [!WARNING]
>
> Be very careful when using <xref:Microsoft.EntityFrameworkCore.RelationalQueryableExtensions.FromSqlRaw*>, and always make sure values are either from a safe origin, or are properly sanitized. SQL injection attacks can have disastrous consequences for your application.

## Composing with LINQ

You can compose on top of the initial SQL query using LINQ operators; EF Core will treat your SQL as a subquery and compose over it in the database. The following example uses a SQL query that selects from a Table-Valued Function (TVF). And then composes on it using LINQ to do filtering and sorting.

```csharp
var searchTerm = "Lorem ipsum";

var blogs = await context.Blogs
    .FromSql($"SELECT * FROM dbo.SearchBlogs({searchTerm})")
    .Where(b => b.Rating > 3)
    .OrderByDescending(b => b.Rating)
    .ToListAsync();
```

The above query generates the following SQL:

```sql
SELECT [b].[BlogId], [b].[OwnerId], [b].[Rating], [b].[Url]
FROM (
    SELECT * FROM dbo.SearchBlogs(@p0)
) AS [b]
WHERE [b].[Rating] > 3
ORDER BY [b].[Rating] DESC
```

### Including related data

The [`Include`](xref:core/querying/related-data/eager) operator can be used to load related data, just like with any other LINQ query:

```csharp
var searchTerm = "Lorem ipsum";

var blogs = await context.Blogs
    .FromSql($"SELECT * FROM dbo.SearchBlogs({searchTerm})")
    .Include(b => b.Posts)
    .ToListAsync();
```

Composing with LINQ requires your SQL query to be composable, since EF Core will treat the supplied SQL as a subquery. Composable SQL queries generally begin with the `SELECT` keyword, and cannot contain SQL features that aren't valid in a subquery, such as:

- A trailing semicolon
- On SQL Server, a trailing query-level hint (for example, `OPTION (HASH JOIN)`)
- On SQL Server, an `ORDER BY` clause that isn't used with `OFFSET 0` OR `TOP 100 PERCENT` in the `SELECT` clause

SQL Server doesn't allow composing over stored procedure calls, so any attempt to apply additional query operators to such a call will result in invalid SQL. Use <xref:System.Linq.Enumerable.AsEnumerable*> or <xref:Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AsAsyncEnumerable*> right after <xref:Microsoft.EntityFrameworkCore.RelationalQueryableExtensions.FromSql*> or <xref:Microsoft.EntityFrameworkCore.RelationalQueryableExtensions.FromSqlRaw*> to make sure that EF Core doesn't try to compose over a stored procedure.

## Change tracking

Queries that use <xref:Microsoft.EntityFrameworkCore.RelationalQueryableExtensions.FromSql*> or <xref:Microsoft.EntityFrameworkCore.RelationalQueryableExtensions.FromSqlRaw*> follow the exact same change tracking rules as any other LINQ query in EF Core. For example, if the query projects entity types, the results are tracked by default.

The following example uses a SQL query that selects from a Table-Valued Function (TVF), then disables change tracking with the call to [`AsNoTracking`](xref:core/querying/tracking#no-tracking-queries):

```csharp
var searchTerm = "Lorem ipsum";

var blogs = await context.Blogs
    .FromSql($"SELECT * FROM dbo.SearchBlogs({searchTerm})")
    .AsNoTracking()
    .ToListAsync();
```

## Querying scalar (non-entity) types

While <xref:Microsoft.EntityFrameworkCore.RelationalQueryableExtensions.FromSql*> is useful for querying entities defined in your model, <xref:Microsoft.EntityFrameworkCore.RelationalDatabaseFacadeExtensions.SqlQuery*> allows you to easily query for scalar, non-entity types via SQL, without needing to drop down to lower-level data access APIs. For example, the following query fetches all the IDs from the `Blogs` table:

### [SQL Server](#tab/sqlserver)

```c#
var ids = await context.Database
    .SqlQuery<int>($"SELECT [BlogId] FROM [Blogs]")
    .ToListAsync();
```

### [SQLite](#tab/sqlite)

```c#
var ids = await context.Database
    .SqlQuery<int>($"""
                    SELECT "BlogId" FROM "Blogs"
                    """)
    .ToListAsync();
```

### [PostgreSQL](#tab/postgres)

```c#
var ids = await context.Database
    .SqlQuery<int>($"""
                    SELECT "BlogId" FROM "Blogs"
                    """)
    .ToListAsync();
```

***

You can also compose LINQ operators over your SQL query. However, since your SQL becomes a subquery whose output column needs to be referenced by the SQL EF adds, you must name the output column `Value`. For example, the following query returns the IDs which are above the ID average:

### [SQL Server](#tab/sqlserver)

```c#
var overAverageIds = await context.Database
    .SqlQuery<int>($"SELECT [BlogId] AS [Value] FROM [Blogs]")
    .Where(id => id > context.Blogs.Average(b => b.BlogId))
    .ToListAsync();
```

### [SQLite](#tab/sqlite)

```c#
var overAverageIds = await context.Database
    .SqlQuery<int>($"""
                    SELECT "BlogId" AS "Value" FROM "Blogs"
                    """)
    .Where(id => id > context.Blogs.Average(b => b.BlogId))
    .ToListAsync();
```

### [PostgreSQL](#tab/postgres)

```c#
var overAverageIds = await context.Database
    .SqlQuery<int>($"""
                    SELECT "BlogId" AS "Value" FROM "Blogs"
                    """)
    .Where(id => id > context.Blogs.Average(b => b.BlogId))
    .ToListAsync();
```

***

<xref:Microsoft.EntityFrameworkCore.RelationalDatabaseFacadeExtensions.SqlQuery*> can be used with any scalar type supported by your database provider. If you'd like to use a type not supported by your database provider, you can use [pre-convention configuration](xref:core/modeling/bulk-configuration#pre-convention-configuration) to define a value conversion for it.

<xref:Microsoft.EntityFrameworkCore.RelationalDatabaseFacadeExtensions.SqlQuery*> isn't limited to scalar types. You can also project each row of the result set into an arbitrary CLR type that isn't part of your EF model, as long as the type has a property for every column in the result set. See [Querying unmapped types](#querying-unmapped-types) below for more details.

<xref:Microsoft.EntityFrameworkCore.RelationalDatabaseFacadeExtensions.SqlQueryRaw*> allows for dynamic construction of SQL queries, just like <xref:Microsoft.EntityFrameworkCore.RelationalQueryableExtensions.FromSqlRaw*> does for entity types.

## Querying unmapped types

> [!NOTE]
> Returning unmapped types from <xref:Microsoft.EntityFrameworkCore.RelationalDatabaseFacadeExtensions.SqlQuery*> was introduced in EF Core 8.0. Support for unmapped types that contain nested [complex types](xref:core/modeling/complex-types) was added in a later release.

In addition to scalar types, <xref:Microsoft.EntityFrameworkCore.RelationalDatabaseFacadeExtensions.SqlQuery*> can populate instances of a mappable CLR type from the result set of a SQL query, without including that type in the EF model. The type used for the query results can contain common mapping constructs supported by EF Core, such as parameterized constructors and mapping attributes. For example, given the following type:

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

The type can be queried using <xref:Microsoft.EntityFrameworkCore.RelationalDatabaseFacadeExtensions.SqlQuery*> in the same way as scalar types:

```csharp
var start = new DateOnly(2022, 1, 1);
var end = new DateOnly(2023, 1, 1);
var posts = await context.Database
    .SqlQuery<BlogPost>($"SELECT * FROM Posts AS p WHERE p.PublishedOn >= {start} AND p.PublishedOn < {end}")
    .ToListAsync();
```

The type used must have a property for every value in the result set, but it doesn't need to match any table in the database - it can, for example, represent a subset of columns, or the result of a join across multiple tables.

> [!NOTE]
> Unmapped types used in this way don't have keys defined and cannot have relationships to other types. Types with relationships must be mapped in the model.

### Unmapped complex types

The unmapped type returned by <xref:Microsoft.EntityFrameworkCore.RelationalDatabaseFacadeExtensions.SqlQuery*> can also contain nested [complex types](xref:core/modeling/complex-types), allowing you to group several columns from the result set into a nested value object. For example, consider an `Address` complex type that is nested within a `Customer` result type:

```csharp
[ComplexType]
public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Address Address { get; set; }
}

public class Address
{
    public string Line1 { get; set; }
    public string City { get; set; }
    public string PostCode { get; set; }
    public string Country { get; set; }
}
```

The nested complex type properties are populated from the result set using the same column-name conventions used when mapping complex types in the model, where each nested property maps to a column named with the complex property name and the nested property name joined by an underscore (for example, `Address_City`):

```csharp
var customers = await context.Database
    .SqlQuery<Customer>(
        $"""
         SELECT [Id], [Name], [Address_Line1], [Address_City], [Address_PostCode], [Address_Country]
         FROM [Customers]
         """)
    .ToListAsync();
```

As with complex types in the model, the nested type doesn't have a key and cannot define relationships to other types.

## Executing non-querying SQL

In some scenarios, it may be necessary to execute SQL which does not return any data, typically for modifying data in the database or calling a stored procedure which doesn't return any result sets. This can be done via <xref:Microsoft.EntityFrameworkCore.RelationalDatabaseFacadeExtensions.ExecuteSql*>:

```csharp
using (var context = new BloggingContext())
{
    var rowsModified = context.Database.ExecuteSql($"UPDATE [Blogs] SET [Url] = NULL");
}
```

This executes the provided SQL and returns the number of rows modified. <xref:Microsoft.EntityFrameworkCore.RelationalDatabaseFacadeExtensions.ExecuteSql*> protects against SQL injection by using safe parameterization, just like <xref:Microsoft.EntityFrameworkCore.RelationalQueryableExtensions.FromSql*>, and <xref:Microsoft.EntityFrameworkCore.RelationalDatabaseFacadeExtensions.ExecuteSqlRaw*> allows for dynamic construction of SQL queries, just like <xref:Microsoft.EntityFrameworkCore.RelationalQueryableExtensions.FromSqlRaw*> does for queries.

## Limitations

There are a few limitations to be aware of when returning entity types from SQL queries:

- The SQL query must return data for all properties of the entity type.
- The column names in the result set must match the column names that properties are mapped to. Note that this behavior is different from EF6; EF6 ignored property-to-column mapping for SQL queries, and result set column names had to match those property names.
- The SQL query can't contain related data. However, in many cases you can compose on top of the query using the `Include` operator to return related data (see [Including related data](#including-related-data)).
