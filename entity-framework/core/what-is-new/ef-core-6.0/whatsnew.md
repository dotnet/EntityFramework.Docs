---
title: What's New in EF Core 6.0
description: Overview of new features in EF Core 6.0
author: ajcvickers
ms.date: 03/08/2021
uid: core/what-is-new/ef-core-6.0/whatsnew
---

# What's New in EF Core 6.0

EF Core 6.0 is currently in development. This contains an overview of interesting changes introduced in each preview.

This page does not duplicate the [plan for EF Core 6.0](xref:core/what-is-new/ef-core-6.0/plan). The plan describes the overall themes for EF Core 6.0, including everything we are planning to include before shipping the final release.

> [!TIP]
> You can run and debug into all the preview 1 samples shown below by [downloading the sample code from GitHub](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Miscellaneous/NewInEFCore6).

## EF Core 6.0 Preview 2

### Preserve synchronization context in SaveChangesAsync

GitHub Issue: [#23971](https://github.com/dotnet/efcore/issues/23971).

We [changed the EF Core code in the 5.0 release](https://github.com/dotnet/efcore/issues/10164) to set <xref:System.Threading.Tasks.Task.ConfigureAwait%2A?displayProperty=nameWithType> to `false` in all places where we `await` async code. This is generally a better choice for EF Core usage. However, <xref:System.Data.Entity.DbContext.SaveChangesAsync%2A> is a special case because EF Core will set generated values into tracked entities after the async database operation is complete. These changes may then trigger notifications which, for example, may have to run on the U.I. thread. Therefore, we are reverting this change in EF Core 6.0 for the <xref:System.Data.Entity.DbContext.SaveChangesAsync%2A> method only.

### Translate String.Concat with multiple arguments

GitHub Issue: [#23859](https://github.com/dotnet/efcore/issues/23859). This feature was contributed by [@wmeints](https://github.com/wmeints).

Starting with EF Core 6.0, calls to <xref:System.String.Concat%2A?displayProperty=nameWithType> with multiple arguments are now translated to SQL. For example, the following query:

<!--
        var shards = context.Shards
            .Where(e => string.Concat(e.Token1, e.Token2, e.Token3) != e.TokensProcessed).ToList();
-->
[!code-csharp[StringConcat](../../../../samples/core/Miscellaneous/NewInEFCore6/StringConcatSample.cs?name=StringConcat)]

Will be translated to the following SQL when using SQL Server:

```sql
SELECT [s].[Id], [s].[Token1], [s].[Token2], [s].[Token3], [s].[TokensProcessed]
FROM [Shards] AS [s]
WHERE ((COALESCE([s].[Token1], N'') + (COALESCE([s].[Token2], N'') + COALESCE([s].[Token3], N''))) <> [s].[TokensProcessed]) OR [s].[TokensProcessed] IS NULL
```

### Smoother integration with System.Linq.Async

GitHub Issue: [#24041](https://github.com/dotnet/efcore/issues/24041).

The [System.Linq.Async](https://www.nuget.org/packages/System.Linq.Async/) package adds client-side async LINQ processing. Using this package with previous versions of EF Core was cumbersome due to a namespace clash for the async LINQ methods. In EF Core 6.0 we have taken advantage of C# pattern matching for <xref:System.Collections.Generic.IAsyncEnumerable%601> such that the exposed EF Core <xref:Microsoft.EntityFrameworkCore.DbSet%601> does not need to implement the interface directly.

Note that most applications do not need to use System.Linq.Async since EF Core queries are usually fully translated on the server.

### More flexible free-text search

GitHub Issue: [#23921](https://github.com/dotnet/efcore/issues/23921).

In EF Core 6.0, we have relaxed the parameter requirements for <xref:Microsoft.EntityFrameworkCore.SqlServerDbFunctionsExtensions.FreeText(Microsoft.EntityFrameworkCore.DbFunctions,System.String,System.String)> and <xref:Microsoft.EntityFrameworkCore.SqlServerDbFunctionsExtensions.Contains%2A>. This allows these functions to be used with binary columns, or with columns mapped using a value converter. For example, consider an entity type with a `Name` property defined as a value object:

<!--
    public class Customer
    {
        public int Id { get; set; }

        public Name Name{ get; set; }
    }

    public class Name
    {
        public string First { get; set; }
        public string MiddleInitial { get; set; }
        public string Last { get; set; }
    }
-->
[!code-csharp[EntityType](../../../../samples/core/Miscellaneous/NewInEFCore6/ContainsFreeTextSample.cs?name=EntityType)]

This is mapped to JSON in the database:

<!--
            modelBuilder.Entity<Customer>()
                .Property(e => e.Name)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, null),
                    v => JsonSerializer.Deserialize<Name>(v, null));
-->
[!code-csharp[ConfigureCompositeValueObject](../../../../samples/core/Miscellaneous/NewInEFCore6/ContainsFreeTextSample.cs?name=ConfigureCompositeValueObject)]

A query can now be executed using `Contains` or `FreeText` even though the type of the property is `Name` not `string`. For example:

<!--
        var result = context.Customers.Where(e => EF.Functions.Contains(e.Name, "Martin")).ToList();
-->
[!code-csharp[Query](../../../../samples/core/Miscellaneous/NewInEFCore6/ContainsFreeTextSample.cs?name=Query)]

This generates the following SQL, when using SQL Server:

```sql
SELECT [c].[Id], [c].[Name]
FROM [Customers] AS [c]
WHERE CONTAINS([c].[Name], N'Martin')
```

## EF Core 6.0 Preview 1

### UnicodeAttribute

GitHub Issue: [#19794](https://github.com/dotnet/efcore/issues/19794). This feature was contributed by [@RaymondHuy](https://github.com/RaymondHuy).

Starting with EF Core 6.0, a string property can now be mapped to a non-Unicode column using a mapping attribute _without specifying the database type directly_. For example, consider a `Book` entity type with a property for the [International Standard Book Number (ISBN)](https://en.wikipedia.org/wiki/International_Standard_Book_Number) in the form "ISBN 978-3-16-148410-0":

<!--
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }

        [Unicode(false)]
        [MaxLength(22)]
        public string Isbn { get; set; }
    }
-->
[!code-csharp[BookEntityType](../../../../samples/core/Miscellaneous/NewInEFCore6/UnicodeAttributeSample.cs?name=BookEntityType)]

Since ISBNs cannot contain any non-unicode characters, the `Unicode` attribute will cause a non-Unicode string type to be used. In addition, `MaxLength` is used to limit the size of the database column. For example, when using SQL Server, this results in a database column of `varchar(22)`:

```sql
CREATE TABLE [Book] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(max) NULL,
    [Isbn] varchar(22) NULL,
    CONSTRAINT [PK_Book] PRIMARY KEY ([Id]));
```

> [!NOTE]
> EF Core maps string properties to Unicode columns by default. `UnicodeAttribute` is ignored when the database system supports only Unicode types.

### PrecisionAttribute

GitHub Issue: [#17914](https://github.com/dotnet/efcore/issues/17914). This feature was contributed by [@RaymondHuy](https://github.com/RaymondHuy).

The precision and scale of a database column can now be configured using mapping attributes _without specifying the database type directly_. For example, consider a `Product` entity type with a decimal `Price` property:

<!--
    public class Product
    {
        public int Id { get; set; }

        [Precision(precision: 10, scale: 2)]
        public decimal Price { get; set; }
    }
-->
[!code-csharp[ProductEntityType](../../../../samples/core/Miscellaneous/NewInEFCore6/PrecisionAttributeSample.cs?name=ProductEntityType)]

EF Core will map this property to a database column with precision 10 and scale 2. For example, on SQL Server:

```sql
CREATE TABLE [Product] (
    [Id] int NOT NULL IDENTITY,
    [Price] decimal(10,2) NOT NULL,
    CONSTRAINT [PK_Product] PRIMARY KEY ([Id]));
```

### EntityTypeConfigurationAttribute

GitHub Issue: [#23163](https://github.com/dotnet/efcore/issues/23163). This feature was contributed by [@KaloyanIT](https://github.com/KaloyanIT).

<xref:Microsoft.EntityFrameworkCore.IEntityTypeConfiguration%601> instances allow <xref:Microsoft.EntityFrameworkCore.ModelBuilder> configuration for each entity type to be contained in its own configuration class. For example:

<!--
public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder
            .Property(e => e.Isbn)
            .IsUnicode(false)
            .HasMaxLength(22);
    }
}
-->
[!code-csharp[BookConfiguration](../../../../samples/core/Miscellaneous/NewInEFCore6/EntityTypeConfigurationAttributeSample.cs?name=BookConfiguration)]

Normally, this configuration class must be instantiated and called into from <xref:Microsoft.EntityFrameworkCore.DbContext.OnModelCreating%2A?displayProperty=nameWithType>. For example:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    new BookConfiguration().Configure(modelBuilder.Entity<Book>());
}
```

Starting with EF Core 6.0, an `EntityTypeConfigurationAttribute` can be placed on the entity type such that EF Core can find and use appropriate configuration. For example:

<!--
[EntityTypeConfiguration(typeof(BookConfiguration))]
public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Isbn { get; set; }
}
-->
[!code-csharp[BookEntityType](../../../../samples/core/Miscellaneous/NewInEFCore6/EntityTypeConfigurationAttributeSample.cs?name=BookEntityType)]

This attribute means that EF Core will use the specified `IEntityTypeConfiguration` implementation whenever the `Book` entity type is included in a model. The entity type is included in a model using one of the normal mechanisms. For example, by creating a <xref:Microsoft.EntityFrameworkCore.DbSet%601> property for the entity type:

<!--
public class BooksContext : DbContext
{
    public DbSet<Book> Books { get; set; }

    //...
-->
[!code-csharp[DbContext](../../../../samples/core/Miscellaneous/NewInEFCore6/EntityTypeConfigurationAttributeSample.cs?name=DbContext)]

Or by registering it in <xref:Microsoft.EntityFrameworkCore.DbContext.OnModelCreating%2A>:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Book>();
}
```

> [!NOTE]
> `EntityTypeConfigurationAttribute` types will not be automatically discovered in an assembly. Entity types must be added to the model before the attribute will be discovered on that entity type.

### Translate ToString on SQLite

GitHub Issue: [#17223](https://github.com/dotnet/efcore/issues/17223). This feature was contributed by [@ralmsdeveloper](https://github.com/ralmsdeveloper).

Calls to <xref:System.Object.ToString> are now translated to SQL when using the SQLite database provider. This can be useful for text searches involving non-string columns. For example, consider a `User` entity type that stores phone numbers as numeric values:

<!--
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public long PhoneNumber { get; set; }
    }
-->
[!code-csharp[UserEntityType](../../../../samples/core/Miscellaneous/NewInEFCore6/ToStringTranslationSample.cs?name=UserEntityType)]

`ToString` can be used to convert the number to a string in the database. We can then use this string with a function such as `LIKE` to find numbers that match a pattern. For example, to find all numbers containing 555:

<!--
var users = context.Users.Where(u => EF.Functions.Like(u.PhoneNumber.ToString(), "%555%")).ToList();
-->
[!code-csharp[Query](../../../../samples/core/Miscellaneous/NewInEFCore6/ToStringTranslationSample.cs?name=Query)]

This translates to the following SQL when using a SQLite database:

```sql
SELECT "u"."Id", "u"."PhoneNumber", "u"."Username"
FROM "Users" AS "u"
WHERE CAST("u"."PhoneNumber" AS TEXT) LIKE '%555%'
```

Note that translation of <xref:System.Object.ToString> for SQL Server is already supported in EF Core 5.0, and may also be supported by other database providers.

### EF.Functions.Random

GitHub Issue: [#16141](https://github.com/dotnet/efcore/issues/16141). This feature was contributed by [@RaymondHuy](https://github.com/RaymondHuy).

`EF.Functions.Random` maps to a database function returning a pseudo-random number between 0 and 1 exclusive. Translations have been implemented in the EF Core repo for SQL Server, SQLite, and Cosmos. For example, consider a `User` entity type with a `Popularity` property:

<!--
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public int Popularity { get; set; }
    }
-->
[!code-csharp[UserEntityType](../../../../samples/core/Miscellaneous/NewInEFCore6/RandomFunctionSample.cs?name=UserEntityType)]

`Popularity` can have values from 1 to 5 inclusive. Using `EF.Functions.Random` we can write a query to return all users with a randomly chosen popularity:

<!--
var users = context.Users.Where(u => u.Popularity == (int)(EF.Functions.Random() * 5.0) + 1).ToList();
-->
[!code-csharp[Query](../../../../samples/core/Miscellaneous/NewInEFCore6/RandomFunctionSample.cs?name=Query)]

This translates to the following SQL when using a SQL Server database:

```sql
SELECT [u].[Id], [u].[Popularity], [u].[Username]
FROM [Users] AS [u]
WHERE [u].[Popularity] = (CAST((RAND() * 5.0E0) AS int) + 1)
```

### Support for SQL Server sparse columns

GitHub Issue: [#8023](https://github.com/dotnet/efcore/issues/8023).

SQL Server [sparse columns](/sql/relational-databases/tables/use-sparse-columns) are ordinary columns that are optimized to store null values. This can be useful when using [TPH inheritance mapping](xref:core/modeling/inheritance) where properties of a rarely used subtype will result in null column values for most rows in the table. For example, consider a `ForumModerator` class that extends from `ForumUser`:

<!--
    public class ForumUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
    }

    public class ForumModerator : ForumUser
    {
        public string ForumName { get; set; }
    }
-->
[!code-csharp[UserEntityType](../../../../samples/core/Miscellaneous/NewInEFCore6/SparseColumnsSample.cs?name=UserEntityType)]

There may be millions of users, with only a handful of these being moderators. This means mapping the `ForumName` as sparse might make sense here. This can now be configured using `IsSparse` in <xref:Microsoft.EntityFrameworkCore.DbContext.OnModelCreating%2A>. For example:

<!--
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<ForumModerator>()
                .Property(e => e.ForumName)
                .IsSparse();
        }
-->
[!code-csharp[OnModelCreating](../../../../samples/core/Miscellaneous/NewInEFCore6/SparseColumnsSample.cs?name=OnModelCreating)]

EF Core migrations will then mark the column as sparse. For example:

```sql
CREATE TABLE [ForumUser] (
    [Id] int NOT NULL IDENTITY,
    [Username] nvarchar(max) NULL,
    [Discriminator] nvarchar(max) NOT NULL,
    [ForumName] nvarchar(max) SPARSE NULL,
    CONSTRAINT [PK_ForumUser] PRIMARY KEY ([Id]));
```

> [!NOTE]
> Sparse columns have limitations. Make sure to read the [SQL Server sparse columns documentation](/sql/relational-databases/tables/use-sparse-columns) to ensure that sparse columns are the right choice for your scenario.

### In-memory database: validate required properties are not null

GitHub Issue: [#10613](https://github.com/dotnet/efcore/issues/10613). This feature was contributed by [@fagnercarvalho](https://github.com/fagnercarvalho).

The EF Core in-memory database will now throw an exception if an attempt is made to save a null value for a property marked as required. For example, consider a `User` type with a required `Username` property:

<!--
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }
    }
-->
[!code-csharp[UserEntityType](../../../../samples/core/Miscellaneous/NewInEFCore6/InMemoryRequiredPropertiesSample.cs?name=UserEntityType)]

Attempting to save an entity with a null `Username` will result in the following exception:

> Microsoft.EntityFrameworkCore.DbUpdateException: Required properties '{'Username'}' are missing for the instance of entity type 'User' with the key value '{Id: 1}'.

This validation can be disabled if necessary. For example:

<!--
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .LogTo(Console.WriteLine, new[] { InMemoryEventId.ChangesSaved })
                .UseInMemoryDatabase("UserContextWithNullCheckingDisabled")
                .EnableNullabilityCheck(false);
        }
-->
[!code-csharp[OnConfiguring](../../../../samples/core/Miscellaneous/NewInEFCore6/InMemoryRequiredPropertiesSample.cs?name=OnConfiguring)]

### Improved SQL Server translation for IsNullOrWhitespace

GitHub Issue: [#22916](https://github.com/dotnet/efcore/issues/22916). This feature was contributed by [@Marusyk](https://github.com/Marusyk).

Consider the following query:

<!--
        var users = context.Users.Where(
            e => string.IsNullOrWhiteSpace(e.FirstName)
                 || string.IsNullOrWhiteSpace(e.LastName)).ToList();
-->
[!code-csharp[Query](../../../../samples/core/Miscellaneous/NewInEFCore6/IsNullOrWhitespaceSample.cs?name=Query)]

Before EF Core 6.0, this was translated to the following on SQL Server:

```sql
SELECT [u].[Id], [u].[FirstName], [u].[LastName]
FROM [Users] AS [u]
WHERE ([u].[FirstName] IS NULL OR (LTRIM(RTRIM([u].[FirstName])) = N'')) OR ([u].[LastName] IS NULL OR (LTRIM(RTRIM([u].[LastName])) = N''))
```

This translation has been improved for EF Core 6.0 to:

```sql
SELECT [u].[Id], [u].[FirstName], [u].[LastName]
FROM [Users] AS [u]
WHERE ([u].[FirstName] IS NULL OR ([u].[FirstName] = N'')) OR ([u].[LastName] IS NULL OR ([u].[LastName] = N''))
```

### Database comments are scaffolded to code comments

GitHub Issue: [#19113](https://github.com/dotnet/efcore/issues/19113). This feature was contributed by [@ErikEJ](https://github.com/ErikEJ).

Comments on SQL tables and columns are now scaffolded into the entity types created when [reverse-engineering an EF Core model](xref:core/managing-schemas/scaffolding) from an existing SQL Server database. For example:

```csharp
/// <summary>
/// The Blog table.
/// </summary>
public partial class Blog
{
    /// <summary>
    /// The primary key.
    /// </summary>
    [Key]
    public int Id { get; set; }
}
```

## Microsoft.Data.Sqlite 6.0 Preview 1

> [!TIP]
> You can run and debug into all the preview 1 samples shown below by [downloading the sample code from GitHub](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Miscellaneous/NewInEFCore6).

### Savepoints API

GitHub Issue: [#20228](https://github.com/dotnet/efcore/issues/20228).

We have been standardizing on [a common API for savepoints in ADO.NET providers](https://github.com/dotnet/runtime/issues/33397). Microsoft.Data.Sqlite now supports this API, including:

- <xref:System.Data.Common.DbTransaction.Save(System.String)> to create a savepoint in the transaction
- <xref:System.Data.Common.DbTransaction.Rollback(System.String)> to roll back to a previous savepoint
- <xref:System.Data.Common.DbTransaction.Release(System.String)> to release a savepoint

Using a savepoint allows part of a transaction to be rolled back without rolling back the entire transaction. For example, the code below:

- Creates a transaction
- Sends an update to the database
- Creates a savepoint
- Sends another update to the database
- Rolls back to the savepoint previous created
- Commits the transaction

<!--
        using var connection = new SqliteConnection("DataSource=test.db");
        connection.Open();

        using var transaction = connection.BeginTransaction();

        using (var command = connection.CreateCommand())
        {
            command.CommandText = @"UPDATE Users SET Username = 'ajcvickers' WHERE Id = 1";
            command.ExecuteNonQuery();
        }

        transaction.Save("MySavepoint");

        using (var command = connection.CreateCommand())
        {
            command.CommandText = @"UPDATE Users SET Username = 'wfvickers' WHERE Id = 2";
            command.ExecuteNonQuery();
        }

        transaction.Rollback("MySavepoint");

        transaction.Commit();
-->
[!code-csharp[PerformUpdates](../../../../samples/core/Miscellaneous/NewInEFCore6/SqliteSamples.cs?name=PerformUpdates)]

This will result in the first update being committed to the database, while the second update is not committed since the savepoint was rolled back before committing the transaction.

### Command timeout in the connection string

GitHub Issue: [#22505](https://github.com/dotnet/efcore/issues/22505). This feature was contributed by [@nmichels](https://github.com/nmichels).

ADO.NET providers support two distinct timeouts:

- The connection timeout, which determines the maximum time to wait when making a connection to the database.
- The command timeout, which determines the maximum time to wait for a command to complete executing.

The command timeout can be set from code using <xref:System.Data.Common.DbCommand.CommandTimeout?displayProperty=nameWithType>. Many providers are now also exposing this command timeout in the connection string. Microsoft.Data.Sqlite is following this trend with the `Command Timeout` connection string keyword. For example, `"Command Timeout=60;DataSource=test.db"` will use 60 seconds as the default timeout for commands created by the connection.

> [!TIP]
> Sqlite treats `Default Timeout` as a synonym for `Command Timeout` and so can be used instead if preferred.
