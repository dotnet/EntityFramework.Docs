---
title: Raw SQL Queries - EF Core
description: Using raw SQL for queries in Entity Framework Core
author: smitpatel
ms.date: 10/08/2019
uid: core/querying/raw-sql
---
# Raw SQL Queries

Entity Framework Core allows you to drop down to raw SQL queries when working with a relational database. Raw SQL queries are useful if the query you want can't be expressed using LINQ. Raw SQL queries are also used if using a LINQ query is resulting in an inefficient SQL query. Raw SQL queries can return regular entity types or [keyless entity types](xref:core/modeling/keyless-entity-types) that are part of your model.

> [!TIP]
> You can view this article's [sample](https://github.com/dotnet/EntityFramework.Docs/tree/master/samples/core/Querying/RawSQL) on GitHub.

## Basic raw SQL queries

You can use the `FromSqlRaw` extension method to begin a LINQ query based on a raw SQL query. `FromSqlRaw` can only be used on query roots, that is directly on the `DbSet<>`.

[!code-csharp[Main](../../../samples/core/Querying/RawSQL/Program.cs#FromSqlRaw)]

Raw SQL queries can be used to execute a stored procedure.

[!code-csharp[Main](../../../samples/core/Querying/RawSQL/Program.cs#FromSqlRawStoredProcedure)]

## Passing parameters

> [!WARNING]
> **Always use parameterization for raw SQL queries**
>
> When introducing any user-provided values into a raw SQL query, care must be taken to avoid SQL injection attacks. In addition to validating that such values don't contain invalid characters, always use parameterization which sends the values separate from the SQL text.
>
> In particular, never pass a concatenated or interpolated string (`$""`) with non-validated user-provided values into `FromSqlRaw` or `ExecuteSqlRaw`. The `FromSqlInterpolated` and `ExecuteSqlInterpolated` methods allow using string interpolation syntax in a way that protects against SQL injection attacks.

The following example passes a single parameter to a stored procedure by including a parameter placeholder in the SQL query string and providing an additional argument. While this syntax may look like `String.Format` syntax, the supplied value is wrapped in a `DbParameter` and the generated parameter name inserted where the `{0}` placeholder was specified.

[!code-csharp[Main](../../../samples/core/Querying/RawSQL/Program.cs#FromSqlRawStoredProcedureParameter)]

`FromSqlInterpolated` is similar to `FromSqlRaw` but allows you to use string interpolation syntax. Just like `FromSqlRaw`, `FromSqlInterpolated` can only be used on query roots. As with the previous example, the value is converted to a `DbParameter` and isn't vulnerable to SQL injection.

> [!NOTE]
> Prior to version 3.0, `FromSqlRaw` and `FromSqlInterpolated` were two overloads named `FromSql`. For more information, see the [previous versions section](#previous-versions).

[!code-csharp[Main](../../../samples/core/Querying/RawSQL/Program.cs#FromSqlInterpolatedStoredProcedureParameter)]

You can also construct a DbParameter and supply it as a parameter value. Since a regular SQL parameter placeholder is used, rather than a string placeholder, `FromSqlRaw` can be safely used:

[!code-csharp[Main](../../../samples/core/Querying/RawSQL/Program.cs#FromSqlRawStoredProcedureSqlParameter)]

`FromSqlRaw` allows you to use named parameters in the SQL query string, which is useful when a stored procedure has optional parameters:

[!code-csharp[Main](../../../samples/core/Querying/RawSQL/Program.cs#FromSqlRawStoredProcedureNamedSqlParameter)]

> [!NOTE]
> **Parameter Ordering**
> Entity Framework Core passes parameters based on the order of the `SqlParameter[]` array. When passing multiple `SqlParameter`s, the ordering in the SQL string must match the order of the parameters in the stored procedure's definition. Failure to do this may result in type conversion exceptions and/or unexpected behavior when the procedure is executed.

## Composing with LINQ

You can compose on top of the initial raw SQL query using LINQ operators. EF Core will treat it as subquery and compose over it in the database. The following example uses a raw SQL query that selects from a Table-Valued Function (TVF). And then composes on it using LINQ to do filtering and sorting.

[!code-csharp[Main](../../../samples/core/Querying/RawSQL/Program.cs#FromSqlInterpolatedComposed)]

Above query generates following SQL:

```sql
SELECT [b].[BlogId], [b].[OwnerId], [b].[Rating], [b].[Url]
FROM (
    SELECT * FROM dbo.SearchBlogs(@p0)
) AS [b]
WHERE [b].[Rating] > 3
ORDER BY [b].[Rating] DESC
```

### Including related data

The `Include` method can be used to include related data, just like with any other LINQ query:

[!code-csharp[Main](../../../samples/core/Querying/RawSQL/Program.cs#FromSqlInterpolatedInclude)]

Composing with LINQ requires your raw SQL query to be composable since EF Core will treat the supplied SQL as a subquery. SQL queries that can be composed on begin with the `SELECT` keyword. Further, SQL passed shouldn't contain any characters or options that aren't valid on a subquery, such as:

- A trailing semicolon
- On SQL Server, a trailing query-level hint (for example, `OPTION (HASH JOIN)`)
- On SQL Server, an `ORDER BY` clause that isn't used with `OFFSET 0` OR `TOP 100 PERCENT` in the `SELECT` clause

SQL Server doesn't allow composing over stored procedure calls, so any attempt to apply additional query operators to such a call will result in invalid SQL. Use `AsEnumerable` or `AsAsyncEnumerable` method right after `FromSqlRaw` or `FromSqlInterpolated` methods to make sure that EF Core doesn't try to compose over a stored procedure.

## Change Tracking

Queries that use the `FromSqlRaw` or `FromSqlInterpolated` methods follow the exact same change tracking rules as any other LINQ query in EF Core. For example, if the query projects entity types, the results will be tracked by default.

The following example uses a raw SQL query that selects from a Table-Valued Function (TVF), then disables change tracking with the call to `AsNoTracking`:

[!code-csharp[Main](../../../samples/core/Querying/RawSQL/Program.cs#FromSqlInterpolatedAsNoTracking)]

## Limitations

There are a few limitations to be aware of when using raw SQL queries:

- The SQL query must return data for all properties of the entity type.
- The column names in the result set must match the column names that properties are mapped to. Note this behavior is different from EF6. EF6 ignored property to column mapping for raw SQL queries and result set column names had to match the property names.
- The SQL query can't contain related data. However, in many cases you can compose on top of the query using the `Include` operator to return related data (see [Including related data](#including-related-data)).

## Previous versions

EF Core version 2.2 and earlier had two overloads of method named `FromSql`, which behaved in the same way as the newer `FromSqlRaw` and `FromSqlInterpolated`. It was easy to accidentally call the raw string method when the intent was to call the interpolated string method, and the other way around. Calling wrong overload accidentally could result in queries not being parameterized when they should have been.
