---
title: User-defined function mapping - EF Core
description: Mapping user-defined functions to database functions
author: maumar
ms.date: 11/23/2020
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

This function definition can now be associated with user-defined function in the model configuration:

[!code-csharp[Main](../../../samples/core/Querying/UserDefinedFunctionMapping/Model.cs#BasicFunctionConfiguration)]

By default, EF Core tries to map CLR function to a user-defined function with the same name. If the names differ, we can use `HasName` to provide the correct name for the user-defined function we want to map to.

Now, executing the following query:

[!code-csharp[Main](../../../samples/core/Querying/UserDefinedFunctionMapping/Program.cs#BasicQuery)]

Will produce this SQL:

```sql
SELECT [b].[BlogId], [b].[Rating], [b].[Url]
FROM [Blogs] AS [b]
WHERE [dbo].[CommentedPostCountForBlog]([b].[BlogId]) > 1
```

## Mapping a method to a custom SQL

EF Core also allows for user-defined functions that get converted to a specific SQL. The SQL expression is provided using `HasTranslation` method during user-defined function configuration.

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

## Configuring nullability of user-defined function based on its arguments

If the user-defined function can only return `null` when one or more of its arguments are `null`, EFCore provides way to specify that, resulting in more performant SQL. It can be done by adding a `PropagatesNullability()` call to the relevant function parameters model configuration.

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
> This optimization should only be used if the function can only return `null` when it's parameters are `null`.

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
> A queryable function must be mapped to a table-valued function and can't make use of `HasTranslation`.

When the function is mapped, the following query:

[!code-csharp[Main](../../../samples/core/Querying/UserDefinedFunctionMapping/Program.cs#TableValuedFunctionQuery)]

Produces:

```sql
SELECT [p].[PostId], [p].[BlogId], [p].[Content], [p].[Rating], [p].[Title]
FROM [dbo].[PostsWithPopularComments](@likeThreshold) AS [p]
ORDER BY [p].[Rating]
```
