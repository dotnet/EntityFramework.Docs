---
title: User-defined function mapping - EF Core
description: Mapping user-defined functions to database functions
author: SamMonoRT
ms.date: 08/19/2026
uid: core/querying/user-defined-function-mapping
---
# User-defined function mapping

EF Core allows for using user-defined SQL functions in queries. To do that, the functions need to be mapped to a CLR method during model configuration. When translating the LINQ query to SQL, the user-defined function is called instead of the CLR function it has been mapped to.

## Mapping a method to a SQL function

To illustrate how user-defined function mapping works, let's define the following entities:

[!code-csharp[Main](../../../samples/core/Querying/UserDefinedFunctionMapping/Model.cs#Entities)]

And the following model configuration:

[!code-csharp[Main](../../../samples/core/Querying/UserDefinedFunctionMapping/Model.cs#EntityConfiguration)]

Blog can have many posts and each post can have many comments.

Next, create the user-defined function `CommentedPostCountForBlog`, which returns the count of posts with at least one comment for a given blog, based on the blog `Id`:

```sql
CREATE FUNCTION dbo.CommentedPostCountForBlog(@id int)
RETURNS int
AS
BEGIN
    RETURN (SELECT COUNT(*)
        FROM [Posts] AS [p]
        WHERE ([p].[BlogId] = @id) AND ((
            SELECT COUNT(*)
            FROM [Comments] AS [c]
            WHERE [p].[PostId] = [c].[PostId]) > 0));
END
```

To use this function in EF Core, we define the following CLR method, which we map to the user-defined function:

[!code-csharp[Main](../../../samples/core/Querying/UserDefinedFunctionMapping/Model.cs#BasicFunctionDefinition)]

The body of the CLR method is not important. The method will not be invoked client-side, unless EF Core can't translate its arguments. If the arguments can be translated, EF Core only cares about the method signature.

> [!NOTE]
> In the example, the method is defined on `DbContext`, but it can also be defined as a static method inside other classes.

This function definition can now be associated with a user-defined function in the model configuration:

[!code-csharp[Main](../../../samples/core/Querying/UserDefinedFunctionMapping/Model.cs#BasicFunctionConfiguration)]

The lambda overload of <xref:Microsoft.EntityFrameworkCore.RelationalModelBuilderExtensions.HasDbFunction*> avoids manually looking up the `MethodInfo`. The `default` argument values are only used to identify the method; they are never sent to the database.

By default, EF Core maps the CLR method to a database function with the same name in the default schema. Use <xref:Microsoft.EntityFrameworkCore.Metadata.Builders.DbFunctionBuilderBase.HasName*> and <xref:Microsoft.EntityFrameworkCore.Metadata.Builders.DbFunctionBuilderBase.HasSchema*> when the name or schema differs.

Now, executing the following query:

[!code-csharp[Main](../../../samples/core/Querying/UserDefinedFunctionMapping/Program.cs#BasicQuery)]

Will produce this SQL:

```sql
SELECT [b].[BlogId], [b].[Rating], [b].[Url]
FROM [Blogs] AS [b]
WHERE [dbo].[CommentedPostCountForBlog]([b].[BlogId]) > 1
```

### Mapping a method to a built-in function

EF Core considers a mapped function to be user-defined by default. Some databases distinguish built-in and user-defined functions when generating SQL. For example, SQL Server requires user-defined functions to be schema-qualified, but built-in functions aren't schema-qualified.

Use `IsBuiltIn` to map a CLR method to a built-in function:

[!code-csharp[Main](../../../samples/core/Querying/UserDefinedFunctionMapping/Model.cs#BuiltInFunctionDefinition)]

[!code-csharp[Main](../../../samples/core/Querying/UserDefinedFunctionMapping/Model.cs#BuiltInFunctionConfiguration)]

The <xref:Microsoft.EntityFrameworkCore.DbFunctionAttribute.IsBuiltIn> property provides the same configuration when using an attribute:

```csharp
[DbFunction(Name = "ISDATE", IsBuiltIn = true)]
```

## Mapping a function using DbFunctionAttribute

Instead of registering a function in `OnModelCreating`, a static method declared on the `DbContext` can be mapped directly by applying <xref:Microsoft.EntityFrameworkCore.DbFunctionAttribute>. The attribute's `Name`, `Schema`, `IsBuiltIn`, and `IsNullable` properties configure the corresponding characteristics of the database function; these are the same characteristics configured by the `HasName`, `HasSchema`, `IsBuiltIn`, and `IsNullable` fluent API methods when using `HasDbFunction`. Attributed methods on the context are discovered and registered automatically; attributed methods on other classes must still be registered with `HasDbFunction`. Call `HasDbFunction` for an automatically registered method only when a builder is needed for additional fluent configuration, as in the store-type example below.

For example, the following method uses `DbFunctionAttribute` to map SQL Server's built-in `JSON_VALUE` function. Because `IsBuiltIn` is `true`, EF Core emits the function name without a schema.

[!code-csharp[Main](../../../samples/core/Querying/UserDefinedFunctionMapping/Model.cs#JsonFunctionDefinition)]

### Configuring store types

Use <xref:Microsoft.EntityFrameworkCore.Metadata.Builders.DbFunctionBuilder.HasStoreType*> to configure a function's return store type and <xref:Microsoft.EntityFrameworkCore.Metadata.Builders.DbFunctionParameterBuilder.HasStoreType*> to configure a parameter's store type. This is particularly useful when the CLR parameter type has no native database mapping.

In this example, `JsonEntity.Metadata` is a dictionary stored as `nvarchar(max)` through a value converter. The `json` function parameter has the same store type, while the result uses the `nvarchar(4000)` type returned by `JSON_VALUE`:

[!code-csharp[Main](../../../samples/core/Querying/UserDefinedFunctionMapping/Model.cs#JsonFunctionConfiguration)]

The function can then be used with the converted property:

[!code-csharp[Main](../../../samples/core/Querying/UserDefinedFunctionMapping/Program.cs#JsonFunctionQuery)]

```sql
SELECT JSON_VALUE([j].[Metadata], N'$.Filter')
FROM [JsonEntities] AS [j]
```

The value converter is taken from the expression passed as the function argument. Therefore, this pattern works for a mapped property such as `JsonEntity.Metadata`, but configuring the parameter store type does not make arbitrary dictionary values translatable. To use an in-memory dictionary, serialize it and pass the resulting string to a separately mapped method whose CLR parameter is `string`.

## Mapping a method to a custom SQL

EF Core also allows a CLR method to be translated directly to a SQL expression rather than a database function. The SQL expression is provided using <xref:Microsoft.EntityFrameworkCore.Metadata.Builders.DbFunctionBuilder.HasTranslation*> during function configuration.

In the example below, we'll create a function that computes percentage difference between two integers.

The CLR method is as follows:

[!code-csharp[Main](../../../samples/core/Querying/UserDefinedFunctionMapping/Model.cs#HasTranslationFunctionDefinition)]

The function definition is as follows:

[!code-csharp[Main](../../../samples/core/Querying/UserDefinedFunctionMapping/Model.cs#HasTranslationFunctionConfiguration)]

Once we define the function, it can be used in the query. Instead of calling database function, EF Core will translate the method body directly into SQL based on the SQL expression tree constructed from the HasTranslation. The following LINQ query:

[!code-csharp[Main](../../../samples/core/Querying/UserDefinedFunctionMapping/Program.cs#HasTranslationQuery)]

Produces the following SQL:

```sql
SELECT 100 * (ABS(CAST([p].[BlogId] AS float) - 3) / ((CAST([p].[BlogId] AS float) + 3) / 2))
FROM [Posts] AS [p]
```

> [!CAUTION]
> `HasTranslation` works with the SQL expression tree, not SQL text. The translation must construct valid <xref:Microsoft.EntityFrameworkCore.Query.SqlExpressions.SqlExpression> objects with the correct type mappings, nullability, and argument nullability propagation. Incorrect metadata can produce invalid SQL or incorrect query results, and the expression types used by a translation may be specific to a database provider. Use this low-level API only after understanding the provider's SQL expression tree; prefer a regular function mapping or an existing provider translation when possible.

## Configuring nullability of user-defined function based on its arguments

If nullability propagates from a function argument—that is, the function returns `null` whenever that argument is `null`—EF Core can generate more efficient SQL. Configure this by calling <xref:Microsoft.EntityFrameworkCore.Metadata.Builders.DbFunctionParameterBuilder.PropagatesNullability*> for the relevant parameters. For more information about how EF Core compensates for SQL's three-valued logic, see [Query null semantics](xref:core/querying/null-comparisons).

To illustrate this, define user function `ConcatStrings`:

```sql
CREATE FUNCTION [dbo].[ConcatStrings] (@prm1 nvarchar(max), @prm2 nvarchar(max))
RETURNS nvarchar(max)
AS
BEGIN
    RETURN @prm1 + @prm2;
END
```

and two CLR methods that map to it:

[!code-csharp[Main](../../../samples/core/Querying/UserDefinedFunctionMapping/Model.cs#NullabilityPropagationFunctionDefinition)]

The model configuration (inside `OnModelCreating` method) is as follows:

[!code-csharp[Main](../../../samples/core/Querying/UserDefinedFunctionMapping/Model.cs#NullabilityPropagationModelConfiguration)]

The first function is configured in the standard way. The second function is configured to take advantage of the nullability propagation optimization, providing more information on how the function behaves around null parameters.

When issuing the following queries:

[!code-csharp[Main](../../../samples/core/Querying/UserDefinedFunctionMapping/Program.cs#NullabilityPropagationExamples)]

We get this SQL:

```sql
SELECT [b].[BlogId], [b].[Rating], [b].[Url]
FROM [Blogs] AS [b]
WHERE ([dbo].[ConcatStrings]([b].[Url], CONVERT(VARCHAR(11), [b].[Rating])) <> N'Lorem ipsum...') OR [dbo].[ConcatStrings]([b].[Url], CONVERT(VARCHAR(11), [b].[Rating])) IS NULL

SELECT [b].[BlogId], [b].[Rating], [b].[Url]
FROM [Blogs] AS [b]
WHERE ([dbo].[ConcatStrings]([b].[Url], CONVERT(VARCHAR(11), [b].[Rating])) <> N'Lorem ipsum...') OR ([b].[Url] IS NULL OR [b].[Rating] IS NULL)
```

The second query doesn't need to re-evaluate the function itself to test its nullability.

> [!NOTE]
> Only configure nullability propagation when the function can return `null` solely because one or more of the configured parameters are `null`.

## Mapping a queryable function to a table-valued function

EF Core also supports mapping to a table-valued function using a user-defined CLR method returning an `IQueryable` of entity types, allowing EF Core to map TVFs with parameters. The process is similar to mapping a scalar user-defined function to a SQL function: we need a TVF in the database, a CLR function that is used in the LINQ queries, and a mapping between the two.

As an example, we'll use a table-valued function that returns all posts having at least one comment that meets a given "Like" threshold:

```sql
CREATE FUNCTION dbo.PostsWithPopularComments(@likeThreshold int)
RETURNS TABLE
AS
RETURN
(
    SELECT [p].[PostId], [p].[BlogId], [p].[Content], [p].[Rating], [p].[Title]
    FROM [Posts] AS [p]
    WHERE (
        SELECT COUNT(*)
        FROM [Comments] AS [c]
        WHERE ([p].[PostId] = [c].[PostId]) AND ([c].[Likes] >= @likeThreshold)) > 0
)
```

The CLR method signature is as follows:

[!code-csharp[Main](../../../samples/core/Querying/UserDefinedFunctionMapping/Model.cs#QueryableFunctionDefinition)]

> [!TIP]
> The `FromExpression` call in the CLR function body allows for the function to be used instead of a regular DbSet.

And below is the mapping:

[!code-csharp[Main](../../../samples/core/Querying/UserDefinedFunctionMapping/Model.cs#QueryableFunctionConfigurationHasDbFunction)]

> [!NOTE]
> A queryable function must be mapped to a table-valued function. `HasTranslation` supports scalar functions only and can't be used for a table-valued function.

When the function is mapped, the following query:

[!code-csharp[Main](../../../samples/core/Querying/UserDefinedFunctionMapping/Program.cs#TableValuedFunctionQuery)]

Produces:

```sql
SELECT [p].[PostId], [p].[BlogId], [p].[Content], [p].[Rating], [p].[Title]
FROM [dbo].[PostsWithPopularComments](@likeThreshold) AS [p]
ORDER BY [p].[Rating]
```
