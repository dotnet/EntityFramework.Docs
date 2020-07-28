---
title: What's New in EF Core 5.0
description: Overview of new features in EF Core 5.0
author: ajcvickers
ms.date: 07/20/2020
uid: core/what-is-new/ef-core-5.0/whatsnew
---

# What's New in EF Core 5.0

EF Core 5.0 is currently in development. This page will contain an overview of interesting changes introduced in each preview.

This page does not duplicate the [plan for EF Core 5.0](xref:core/what-is-new/ef-core-5.0/plan). The plan describes the overall themes for EF Core 5.0, including everything we are planning to include before shipping the final release.

We will add links from here to the official documentation as it is published.

## Preview 7

### DbContextFactory

EF Core 5.0 introduces `AddDbContextFactory` and `AddPooledDbContextFactory` to register a factory for creating DbContext instances in the application's dependency injection (D.I.) container. For example:

```csharp
services.AddDbContextFactory<SomeDbContext>(b =>
    b.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Test"));
```

Application services such as ASP.NET Core controllers can then depend on `IDbContextFactory<TContext>` in the service constructor. For example:

```csharp
public class MyController
{
    private readonly IDbContextFactory<SomeDbContext> _contextFactory;

    public MyController(IDbContextFactory<SomeDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }
}
```

DbContext instances can then be created and used as needed. For example:

```csharp
public void DoSomehing()
{
    using (var context = _contextFactory.CreateDbContext())
    {
        // ...            
    }
}
```

Note that the DbContext instances created in this way are _not_ managed by the application's service provider and therefore must be disposed by the application. This decoupling is very useful for Blazor applications, where using `IDbContextFactory` is recommended, but may also be useful in other scenarios.

DbContext instances can be pooled by calling `AddPooledDbContextFactory`. This pooling works the same way as for `AddDbContextPool`, and also has the same limitations.

Documentation is tracked by issue [#2523](https://github.com/dotnet/EntityFramework.Docs/issues/2523).

### Reset DbContext state

EF Core 5.0 introduces `ChangeTracker.Clear()` which clears the DbContext of all tracked entities. This should usually not be needed when using the best practice of creating a new, short-lived context instance for each unit-of-work. However, if there is a need to reset the state of a DbContext instance, then using the new `Clear()` method is more performant and robust than mass-detaching all entities.  

Documentation is tracked by issue [#2524](https://github.com/dotnet/EntityFramework.Docs/issues/2524).

### New pattern for store-generated defaults

EF Core allows an explicit value to be set for a column that may also have default value constraint. EF Core uses the CLR default of type property type as a sentinel for this; if the value is not the CLR default, then it is inserted, otherwise the database default is used.

This creates problems for types where the CLR default is not a good sentinel--most notably, `bool` properties. EF Core 5.0 now allows the backing field to be nullable for cases like this. For example:

```csharp
public class Blog
{
    private bool? _isValid;

    public bool IsValid
    {
        get => _isValid ?? false;
        set => _isValid = value;
    }
}
```

Note that the backing field is nullable, but the publicly exposed property is not. This allows the sentinel value to be `null` without impacting the public surface of the entity type. In this case, if the `IsValid` is never set, then the database default will be used since the backing field remains null. If either `true` or `false` are set, then this value is saved explicitly to the database.

Documentation is tracked by issue [#2525](https://github.com/dotnet/EntityFramework.Docs/issues/2525).

### Cosmos partition keys

EF Core allows the Cosmos partition key is included in the EF model. For example:

```csharp
modelBuilder.Entity<Customer>().HasPartitionKey(b => b.AlternateKey)
```

Starting with preview 7, the partition key is included in the entity type's PK and is used to improved performance in some queries.

Documentation is tracked by issue [#2471](https://github.com/dotnet/EntityFramework.Docs/issues/2471).

### Cosmos configuration

EF Core 5.0 improves configuration of Cosmos and Cosmos connections.

Previously, EF Core required the end-point and key to be specified explicitly when connecting to a Cosmos database. EF Core 5.0 allows use of a connection string instead. In addition, EF Core 5.0 allows the WebProxy instance to be explicitly set. For example:

```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder
        .UseCosmos("my-cosmos-connection-string", "MyDb",
            cosmosOptionsBuilder =>
            {
                cosmosOptionsBuilder.WebProxy(myProxyInstance);
            });
```

Many other timeout values, limits, etc. can now also be configured. For example:

```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder
        .UseCosmos("my-cosmos-connection-string", "MyDb",
            cosmosOptionsBuilder =>
            {
                cosmosOptionsBuilder.LimitToEndpoint();
                cosmosOptionsBuilder.RequestTimeout(requestTimeout);
                cosmosOptionsBuilder.OpenTcpConnectionTimeout(timeout);
                cosmosOptionsBuilder.IdleTcpConnectionTimeout(timeout);
                cosmosOptionsBuilder.GatewayModeMaxConnectionLimit(connectionLimit);
                cosmosOptionsBuilder.MaxTcpConnectionsPerEndpoint(connectionLimit);
                cosmosOptionsBuilder.MaxRequestsPerTcpConnection(requestLimit);
            });
```

Finally, the default connection mode is now `ConnectionMode.Gateway`, which is generally more compatible.

Documentation is tracked by issue [#2471](https://github.com/dotnet/EntityFramework.Docs/issues/2471).

### Scaffold-DbContext now singularizes

Previously when scaffolding a DbContext from an existing database, EF Core will create entity type names that match the table names in the database. For example, tables `People` and `Addresses` resulted in entity types named `People` and `Addresses`.

In previous releases, this behavior was configurable through registration of a pluralization service. Now in EF Core 5.0, the [Humanizer](https://www.nuget.org/packages/Humanizer.Core/) package is used as a default pluralization service. This means tables `People` and `Addresses` will now be reverse engineered to entity types named `Person` and `Address`.

### Savepoints

EF Core now supports [savepoints](/SQL/t-sql/language-elements/save-transaction-transact-sql?view=sql-server-ver15#remarks) for greater control over transactions that execute multiple operations.

Savepoints can be manually created, released, and rolled back. For example:

```csharp
context.Database.CreateSavepoint("MySavePoint"); 
```

In addition, EF Core will now roll back to the last savepoint when executing `SaveChanges` fails. This allows SaveChanges to be re-tried without re-trying the entire transaction.

Documentation is tracked by issue [#2429](https://github.com/dotnet/EntityFramework.Docs/issues/2429).

## Preview 6

### Split queries for related collections

Starting with EF Core 3.0, EF Core always generates a single SQL query for each LINQ query. This ensures consistency of the data returned within the constraints of the transaction mode in use. However, this can become very slow when the query uses `Include` or a projection to bring back multiple related collections.

EF Core 5.0 now allows a single LINQ query including related collections to be split into multiple SQL queries. This can significantly improve performance, but can result in inconsistency in the results returned if the data changes between the two queries. Serializable or snapshot transactions can be used to mitigate this and achieve consistency with split queries, but that may bring other performance costs and behavioral difference.

#### Split queries with Include

For example, consider a query that pulls in two levels of related collections using `Include`:

```CSharp
var artists = context.Artists
    .Include(e => e.Albums).ThenInclude(e => e.Tags)
    .ToList();
```

By default, EF Core will generate the following SQL when using the SQLite provider:

```sql
SELECT "a"."Id", "a"."Name", "t0"."Id", "t0"."ArtistId", "t0"."Title", "t0"."Id0", "t0"."AlbumId", "t0"."Name"
FROM "Artists" AS "a"
LEFT JOIN (
    SELECT "a0"."Id", "a0"."ArtistId", "a0"."Title", "t"."Id" AS "Id0", "t"."AlbumId", "t"."Name"
    FROM "Album" AS "a0"
    LEFT JOIN "Tag" AS "t" ON "a0"."Id" = "t"."AlbumId"
) AS "t0" ON "a"."Id" = "t0"."ArtistId"
ORDER BY "a"."Id", "t0"."Id", "t0"."Id0"
```

The new `AsSplitQuery` API can be used to change this behavior. For example:

```CSharp
var artists = context.Artists
    .AsSplitQuery()
    .Include(e => e.Albums).ThenInclude(e => e.Tags)
    .ToList();
```

AsSplitQuery is available for all relational database providers and can be used anywhere in the query, just like AsNoTracking. EF Core will now generate the following three SQL queries:

```sql
SELECT "a"."Id", "a"."Name"
FROM "Artists" AS "a"
ORDER BY "a"."Id"

SELECT "a0"."Id", "a0"."ArtistId", "a0"."Title", "a"."Id"
FROM "Artists" AS "a"
INNER JOIN "Album" AS "a0" ON "a"."Id" = "a0"."ArtistId"
ORDER BY "a"."Id", "a0"."Id"

SELECT "t"."Id", "t"."AlbumId", "t"."Name", "a"."Id", "a0"."Id"
FROM "Artists" AS "a"
INNER JOIN "Album" AS "a0" ON "a"."Id" = "a0"."ArtistId"
INNER JOIN "Tag" AS "t" ON "a0"."Id" = "t"."AlbumId"
ORDER BY "a"."Id", "a0"."Id"
```

All operations on the query root are supported. This includes OrderBy/Skip/Take, Join operations, FirstOrDefault and similar single result selecting operations.

Note that filtered Includes with OrderBy/Skip/Take are not supported in preview 6, but are available in the daily builds and will be included in preview 7.

#### Split queries with collection projections

`AsSplitQuery` can also be used when collections are loaded in projections. For example:

```CSharp
context.Artists
    .AsSplitQuery()
    .Select(e => new
    {
        Artist = e,
        Albums = e.Albums,
    }).ToList();
```

This LINQ query generates the following two SQL queries when using the SQLite provider:

```sql
SELECT "a"."Id", "a"."Name"
FROM "Artists" AS "a"
ORDER BY "a"."Id"

SELECT "a0"."Id", "a0"."ArtistId", "a0"."Title", "a"."Id"
FROM "Artists" AS "a"
INNER JOIN "Album" AS "a0" ON "a"."Id" = "a0"."ArtistId"
ORDER BY "a"."Id"
```

Note that only materialization of the collection is supported. Any composition after `e.Albums` in above case won't result in a split query. Improvements in this area are tracked by [#21234](https://github.com/dotnet/efcore/issues/21234).

### IndexAttribute

The new IndexAttribute can be placed on an entity type to specify an index for a single column. For example:

```CSharp
[Index(nameof(FullName), IsUnique = true)]
public class User
{
    public int Id { get; set; }
    
    [MaxLength(128)]
    public string FullName { get; set; }
}
```

For SQL Server, Migrations will then generate the following SQL:

```sql
CREATE UNIQUE INDEX [IX_Users_FullName]
    ON [Users] ([FullName])
    WHERE [FullName] IS NOT NULL;
```

IndexAttribute can also be used to specify an index spanning multiple columns. For example:

```CSharp
[Index(nameof(FirstName), nameof(LastName), IsUnique = true)]
public class User
{
    public int Id { get; set; }
    
    [MaxLength(64)]
    public string FirstName { get; set; }

    [MaxLength(64)]
    public string LastName { get; set; }
}
```

For SQL Server, this results in:

```sql
CREATE UNIQUE INDEX [IX_Users_FirstName_LastName]
    ON [Users] ([FirstName], [LastName])
    WHERE [FirstName] IS NOT NULL AND [LastName] IS NOT NULL;
```

Documentation is tracked by issue [#2407](https://github.com/dotnet/EntityFramework.Docs/issues/2407).

### Improved query translation exceptions

We are continuing to improve the exception messages generated when query translation fails. For example, this query uses the unmapped property `IsSigned`:

```CSharp
var artists = context.Artists.Where(e => e.IsSigned).ToList();
```

EF Core will throw the following exception indicating that translation failed because `IsSigned` is not mapped:

> Unhandled exception. System.InvalidOperationException: The LINQ expression 'DbSet<Artist>()
>    .Where(a => a.IsSigned)' could not be translated. Additional information: Translation of member 'IsSigned' on entity type 'Artist' failed. Possibly the specified member is not mapped. Either rewrite the query in a form that can be translated, or switch to client evaluation explicitly by inserting a call to either AsEnumerable(), AsAsyncEnumerable(), ToList(), or ToListAsync(). See https://go.microsoft.com/fwlink/?linkid=2101038 for more information.

Similarly, better exception messages are now generated when attempting to translate string comparisons with culture-dependent semantics. For example, this query attempts to use `StringComparison.CurrentCulture`:

```CSharp
var artists = context.Artists
    .Where(e => e.Name.Equals("The Unicorns", StringComparison.CurrentCulture))
    .ToList();
```

EF Core will now throw the following exception:

> Unhandled exception. System.InvalidOperationException: The LINQ expression 'DbSet<Artist>()
>      .Where(a => a.Name.Equals(
>          value: "The Unicorns", 
>          comparisonType: CurrentCulture))' could not be translated. Additional information: Translation of 'string.Equals' method which takes 'StringComparison' argument is not supported. See https://go.microsoft.com/fwlink/?linkid=2129535 for more information. Either rewrite the query in a form that can be translated, or switch to client evaluation explicitly by inserting a call to either AsEnumerable(), AsAsyncEnumerable(), ToList(), or ToListAsync(). See https://go.microsoft.com/fwlink/?linkid=2101038 for more information.

### Specify transaction ID

This feature was contributed from the community by [@Marusyk](https://github.com/Marusyk). Many thanks for the contribution!

EF Core exposes a transaction ID for correlation of transactions across calls. This ID typically set by EF Core when a transaction is started. If the application starts the transaction instead, then this feature allows the application to explicitly set the transaction ID so it is correlated correctly everywhere it is used. For example:

```CSharp
using (context.Database.UseTransaction(myTransaction, myId))
{
   ...
}
```

### IPAddress mapping

This feature was contributed from the community by [@ralmsdeveloper](https://github.com/ralmsdeveloper). Many thanks for the contribution!

The standard .NET [IPAddress class](/dotnet/api/system.net.ipaddress) is now automatically mapped to a string column for databases that do not already have native support. For example, consider mapping this entity type:

```CSharp
public class Host
{
    public int Id { get; set; }
    public IPAddress Address { get; set; }
}
```

On SQL Server, this will result in Migrations creating the following table:

```sql
CREATE TABLE [Host] (
    [Id] int NOT NULL,
    [Address] nvarchar(45) NULL,
    CONSTRAINT [PK_Host] PRIMARY KEY ([Id]));
``` 

Entities can then be added in the normal way:

```CSharp
context.AddRange(
    new Host { Address = IPAddress.Parse("127.0.0.1")},
    new Host { Address = IPAddress.Parse("0000:0000:0000:0000:0000:0000:0000:0001")});
``` 

And the resulting SQL will insert the normalized IPv4 or IPv6 address:

```sql
Executed DbCommand (14ms) [Parameters=[@p0='1', @p1='127.0.0.1' (Size = 45), @p2='2', @p3='::1' (Size = 45)], CommandType='Text', CommandTimeout='30']
      SET NOCOUNT ON;
      INSERT INTO [Host] ([Id], [Address])
      VALUES (@p0, @p1), (@p2, @p3);
```

### Exclude OnConfiguring when scaffolding

When a DbContext is scaffolded from an existing database, EF Core by default creates an OnConfiguring overload with a connection string so that the context is immediately usable. However, this is not useful if you already have a partial class with OnConfiguring, or if you are configuring the context some other way.

To address this, the scaffolding commands can now be instructed to omit generation of OnConfiguring. For example:

```
dotnet ef dbcontext scaffold "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Chinook" Microsoft.EntityFrameworkCore.SqlServer --no-onconfiguring
```

Or in the Package Manager Console:

```
Scaffold-DbContext 'Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Chinook' Microsoft.EntityFrameworkCore.SqlServer -NoOnConfiguring 
``` 

Note that we recommend using [a named connection string and secure storage like User Secrets](/core/managing-schemas/scaffolding?tabs=vs#configuration-and-user-secrets).

### Translations for FirstOrDefault on strings

This feature was contributed from the community by [@dvoreckyaa](https://github.com/dvoreckyaa). Many thanks for the contribution!

FirstOrDefault and similar operators for characters in strings are now translated. For example, this LINQ query:

```CSharp
context.Customers.Where(c => c.ContactName.FirstOrDefault() == 'A').ToList();
```

Will be translated to the following SQL when using SQL Server:

```sql
SELECT [c].[Id], [c].[ContactName]
FROM [Customer] AS [c]
WHERE SUBSTRING([c].[ContactName], 1, 1) = N'A'
```

### Simplify case blocks

EF Core now generates better queries with CASE blocks. For example, this LINQ query: 

```CSharp
context.Weapons
    .OrderBy(w => w.Name.CompareTo("Marcus' Lancer") == 0)
    .ThenBy(w => w.Id)
```

Was on SQL Server formally translated to:

```sql
SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM [Weapons] AS [w]
ORDER BY CASE
    WHEN (CASE
        WHEN [w].[Name] = N'Marcus'' Lancer' THEN 0
        WHEN [w].[Name] > N'Marcus'' Lancer' THEN 1
        WHEN [w].[Name] < N'Marcus'' Lancer' THEN -1
    END = 0) AND CASE
        WHEN [w].[Name] = N'Marcus'' Lancer' THEN 0
        WHEN [w].[Name] > N'Marcus'' Lancer' THEN 1
        WHEN [w].[Name] < N'Marcus'' Lancer' THEN -1
    END IS NOT NULL THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END, [w].[Id]");
``` 

But is now translated to:

```sql
SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM [Weapons] AS [w]
ORDER BY CASE
    WHEN ([w].[Name] = N'Marcus'' Lancer') AND [w].[Name] IS NOT NULL THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END, [w].[Id]");
``` 

## Preview 5

### Database collations

The default collation for a database can now be specified in the EF model. This will flow through to generated migrations to set the collation when the database is created. For example:

```CSharp
modelBuilder.UseCollation("German_PhoneBook_CI_AS");
```

Migrations then generates the following to create the database on SQL Server:

```sql
CREATE DATABASE [Test]
COLLATE German_PhoneBook_CI_AS;
```

The collation to use for specific database columns can also be specified. For example:

```CSharp
 modelBuilder
     .Entity<User>()
     .Property(e => e.Name)
     .UseCollation("German_PhoneBook_CI_AS");
```

For those not using migrations, collations are now reverse-engineered from the database when scaffolding a DbContext.

Finally, the `EF.Functions.Collate()` allows for ad-hoc queries using different collations. For example:

```CSharp
context.Users.Single(e => EF.Functions.Collate(e.Name, "French_CI_AS") == "Jean-Michel Jarre");
```

This will generate the following query for SQL Server:

```sql
SELECT TOP(2) [u].[Id], [u].[Name]
FROM [Users] AS [u]
WHERE [u].[Name] COLLATE French_CI_AS = N'Jean-Michel Jarre'
```

Note that ad-hoc collations should be used with care as they can negatively impact database performance.

Documentation is tracked by issue [#2273](https://github.com/dotnet/EntityFramework.Docs/issues/2273).

### Flow arguments into IDesignTimeDbContextFactory

Arguments are now flowed from the command line into the `CreateDbContext` method of [IDesignTimeDbContextFactory](/dotnet/api/microsoft.entityframeworkcore.design.idesigntimedbcontextfactory-1?view=efcore-3.1). For example, to indicate this is a dev build, a custom argument (e.g. `dev`) can be passed on the command line:

```
dotnet ef migrations add two --verbose --dev
``` 

This argument will then flow into the factory, where it can be used to control how the context is created and initialized. For example:

```CSharp
public class MyDbContextFactory : IDesignTimeDbContextFactory<SomeDbContext>
{
    public SomeDbContext CreateDbContext(string[] args) 
        => new SomeDbContext(args.Contains("--dev"));
}
```

Documentation is tracked by issue [#2419](https://github.com/dotnet/EntityFramework.Docs/issues/2419).

### No-tracking queries with identity resolution

No-tracking queries can now be configured to perform identity resolution. For example, the following query will create a new Blog instance for each Post, even if each Blog has the same primary key. 

```CSharp
context.Posts.AsNoTracking().Include(e => e.Blog).ToList();
```

However, at the expense of usually being slightly slower and always using more memory, this query can be changed to ensure only a single Blog instance is created:

```CSharp
context.Posts.AsNoTracking().PerformIdentityResolution().Include(e => e.Blog).ToList();
```

Note that this is only useful for no-tracking queries since all tracking queries already exhibit this behavior. Also, following API review, the `PerformIdentityResolution` syntax will be changed. See [#19877](https://github.com/dotnet/efcore/issues/19877#issuecomment-637371073).

Documentation is tracked by issue [#1895](https://github.com/dotnet/EntityFramework.Docs/issues/1895).

### Stored (persisted) computed columns

Most databases allow computed column values to be stored after computation. While this takes up disk space, the computed column is calculated only once on update, instead of each time its value is retrieved. This also allows the column to be indexed for some databases.

EF Core 5.0 allows computed columns to be configured as stored. For example:
 
```CSharp
modelBuilder
    .Entity<User>()
    .Property(e => e.SomethingComputed)
    .HasComputedColumnSql("my sql", stored: true);
```

### SQLite computed columns

EF Core now supports computed columns in SQLite databases.

## Preview 4

### Configure database precision/scale in model

Precision and scale for a property can now be specified using the model builder. For example:

```CSharp
modelBuilder
    .Entity<Blog>()
    .Property(b => b.Numeric)
    .HasPrecision(16, 4);
```

Precision and scale can still be set via the full database type, such as "decimal(16,4)". 

Documentation is tracked by issue [#527](https://github.com/dotnet/EntityFramework.Docs/issues/527).

### Specify SQL Server index fill factor

The fill factor can now be specified when creating an index on SQL Server. For example:

```CSharp
modelBuilder
    .Entity<Customer>()
    .HasIndex(e => e.Name)
    .HasFillFactor(90);
```

## Preview 3

### Filtered Include

The Include method now supports filtering of the entities included. For example:

```CSharp
var blogs = context.Blogs
    .Include(e => e.Posts.Where(p => p.Title.Contains("Cheese")))
    .ToList();
```

This query will return blogs together with each associated post, but only when the post title contains "Cheese".

Skip and Take can also be used to reduce the number of included entities. For example:
 
```CSharp
var blogs = context.Blogs
    .Include(e => e.Posts.OrderByDescending(post => post.Title).Take(5)))
    .ToList();
```
This query will return blogs with at most five posts included on each blog.

See the [Include documentation](xref:core/querying/related-data#filtered-include) for full details.

### New ModelBuilder API for navigation properties

Navigation properties are primarily configured when [defining relationships](xref:core/modeling/relationships). However, the new `Navigation` method can be used in the cases where navigation properties need additional configuration. For example, to set a backing field for the navigation when the field would not be found by convention:

```CSharp
modelBuilder.Entity<Blog>().Navigation(e => e.Posts).HasField("_myposts");
```

Note that the `Navigation` API does not replace relationship configuration. Instead it allows additional configuration of navigation properties in already discovered or defined relationships.

See the [Configuring Navigation Properties documentation](xref:core/modeling/relationships#configuring-navigation-properties).

### New command-line parameters for namespaces and connection strings 

Migrations and scaffolding now allow namespaces to be specified on the command line. For example, to reverse engineer a database putting the context and model classes in different namespaces: 

```
dotnet ef dbcontext scaffold "connection string" Microsoft.EntityFrameworkCore.SqlServer --context-namespace "My.Context" --namespace "My.Model"
```

See the [Migrations](xref:core/managing-schemas/migrations/index#namespaces) and [Reverse Engineering](xref:core/managing-schemas/scaffolding#directories-and-namespaces) documentation for full details.

---
Also, a connection string can now be passed to the `database-update` command:

```
dotnet ef database update --connection "connection string"
```

See the [Tools documentation](xref:core/miscellaneous/cli/dotnet#dotnet-ef-database-update) for full details.

Equivalent parameters have also been added to the PowerShell commands used in the VS Package Manager Console.

### EnableDetailedErrors has returned

For performance reasons, EF doesn't do additional null-checks when reading values from the database. This can result in exceptions that are hard to root-cause when an unexpected null is encountered.

Using `EnableDetailedErrors` will add extra null checking to queries such that, for a small performance overhead, these errors are easier to trace back to a root cause.  

For example:
```CSharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder
        .EnableDetailedErrors()
        .EnableSensitiveDataLogging() // Often also useful with EnableDetailedErrors 
        .UseSqlServer(Your.SqlServerConnectionString);
```

Documentation is tracked by issue [#955](https://github.com/dotnet/EntityFramework.Docs/issues/955).

### Cosmos partition keys

The partition key to use for a given query can now be specified in the query. For example:

```CSharp
await context.Set<Customer>()
             .WithPartitionKey(myPartitionKey)
             .FirstAsync();
```

Documentation is tracked by issue [#2199](https://github.com/dotnet/EntityFramework.Docs/issues/2199).

### Support for the SQL Server DATALENGTH function

This can be accessed using the new `EF.Functions.DataLength` method. For example:
```CSharp
var count = context.Orders.Count(c => 100 < EF.Functions.DataLength(c.OrderDate));
``` 

## Preview 2

### Use a C# attribute to specify a property backing field

A C# attribute can now be used to specify the backing field for a property. This attribute allows EF Core to still write to and read from the backing field as would normally happen, even when the backing field cannot be found automatically. For example:

```CSharp
public class Blog
{
    private string _mainTitle;

    public int Id { get; set; }

    [BackingField(nameof(_mainTitle))]
    public string Title
    {
        get => _mainTitle;
        set => _mainTitle = value;
    }
}
```

Documentation is tracked by issue [#2230](https://github.com/dotnet/EntityFramework.Docs/issues/2230).

### Complete discriminator mapping

EF Core uses a discriminator column for [TPH mapping of an inheritance hierarchy](/ef/core/modeling/inheritance). Some performance enhancements are possible so long as EF Core knows all possible values for the discriminator. EF Core 5.0 now implements these enhancements.

For example, previous versions of EF Core would always generate this SQL for a query returning all types in a hierarchy:

```sql
SELECT [a].[Id], [a].[Discriminator], [a].[Name]
FROM [Animal] AS [a]
WHERE [a].[Discriminator] IN (N'Animal', N'Cat', N'Dog', N'Human')
```

EF Core 5.0 will now generate the following when a complete discriminator mapping is configured:

```sql
SELECT [a].[Id], [a].[Discriminator], [a].[Name]
FROM [Animal] AS [a]
```

It will be the default behavior starting with preview 3.

### Performance improvements in Microsoft.Data.Sqlite

We have made two performance improvements for SQLIte:

* Retrieving binary and string data with GetBytes, GetChars, and GetTextReader is now more efficient by making use of SqliteBlob and streams.
* Initialization of SqliteConnection is now lazy.

These improvements are in the ADO.NET Microsoft.Data.Sqlite provider and hence also improve performance outside of EF Core.

## Preview 1

### Simple logging

This feature adds functionality similar to `Database.Log` in EF6. That is, it provides a simple way to get logs from EF Core without the need to configure any kind of external logging framework.

Preliminary documentation is included in the [EF weekly status for December 5, 2019](https://github.com/dotnet/efcore/issues/15403#issuecomment-562332863).

Additional documentation is tracked by issue [#2085](https://github.com/dotnet/EntityFramework.Docs/issues/2085).

### Simple way to get generated SQL

EF Core 5.0 introduces the `ToQueryString` extension method, which will return the SQL that EF Core will generate when executing a LINQ query.

Preliminary documentation is included in the [EF weekly status for January 9, 2020](https://github.com/dotnet/efcore/issues/19549#issuecomment-572823246).

Additional documentation is tracked by issue [#1331](https://github.com/dotnet/EntityFramework.Docs/issues/1331).

### Use a C# attribute to indicate that an entity has no key

An entity type can now be configured as having no key using the new `KeylessAttribute`. For example:

```CSharp
[Keyless]
public class Address
{
    public string Street { get; set; }
    public string City { get; set; }
    public int Zip { get; set; }
}
```

Documentation is tracked by issue [#2186](https://github.com/dotnet/EntityFramework.Docs/issues/2186).

### Connection or connection string can be changed on initialized DbContext

It is now easier to create a DbContext instance without any connection or connection string. Also, the connection or connection string can now be mutated on the context instance. This feature allows the same context instance to dynamically connect to different databases.

Documentation is tracked by issue [#2075](https://github.com/dotnet/EntityFramework.Docs/issues/2075).

### Change-tracking proxies

EF Core can now generate runtime proxies that automatically implement [INotifyPropertyChanging](/dotnet/api/system.componentmodel.inotifypropertychanging?view=netcore-3.1) and [INotifyPropertyChanged](/dotnet/api/system.componentmodel.inotifypropertychanged?view=netcore-3.1). These then report value changes on entity properties directly to EF Core, avoiding the need to scan for changes. However, proxies come with their own set of limitations, so they are not for everyone.

Documentation is tracked by issue [#2076](https://github.com/dotnet/EntityFramework.Docs/issues/2076).

### Enhanced debug views

Debug views are an easy way to look at the internals of EF Core when debugging issues. A debug view for the Model was implemented some time ago. For EF Core 5.0, we have made the model view easier to read and added a new debug view for tracked entities in the state manager.

Preliminary documentation is included in the [EF weekly status for December 12, 2019](https://github.com/dotnet/efcore/issues/15403#issuecomment-565196206).

Additional documentation is tracked by issue [#2086](https://github.com/dotnet/EntityFramework.Docs/issues/2086).

### Improved handling of database null semantics

Relational databases typically treat NULL as an unknown value and therefore not equal to any other NULL. While C# treats null as a defined value, which compares equal to any other null. EF Core by default translates queries so that they use C# null semantics. EF Core 5.0 greatly improves the efficiency of these translations.

Documentation is tracked by issue [#1612](https://github.com/dotnet/EntityFramework.Docs/issues/1612).

### Indexer properties

EF Core 5.0 supports mapping of C# indexer properties. These properties allow entities to act as property bags where columns are mapped to named properties in the bag.

Documentation is tracked by issue [#2018](https://github.com/dotnet/EntityFramework.Docs/issues/2018).

### Generation of check constraints for enum mappings

EF Core 5.0 Migrations can now generate CHECK constraints for enum property mappings. For example:

```SQL
MyEnumColumn VARCHAR(10) NOT NULL CHECK (MyEnumColumn IN ('Useful', 'Useless', 'Unknown'))
```

Documentation is tracked by issue [#2082](https://github.com/dotnet/EntityFramework.Docs/issues/2082).

### IsRelational

A new `IsRelational` method has been added in addition to the existing `IsSqlServer`, `IsSqlite`, and `IsInMemory`. This method can be used to test if the DbContext is using any relational database provider. For example:

```CSharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    if (Database.IsRelational())
    {
        // Do relational-specific model configuration.
    }
}
```

Documentation is tracked by issue [#2185](https://github.com/dotnet/EntityFramework.Docs/issues/2185).

### Cosmos optimistic concurrency with ETags

The Azure Cosmos DB database provider now supports optimistic concurrency using ETags. Use the model builder in OnModelCreating to configure an ETag:

```CSharp
builder.Entity<Customer>().Property(c => c.ETag).IsEtagConcurrency();
```

SaveChanges will then throw an `DbUpdateConcurrencyException` on a concurrency conflict, which [can be handled](/ef/core/saving/concurrency) to implement retries, etc.

Documentation is tracked by issue [#2099](https://github.com/dotnet/EntityFramework.Docs/issues/2099).

### Query translations for more DateTime constructs

Queries containing new DateTime construction are now translated.

In addition, the following SQL Server functions are now mapped:

* DateDiffWeek
* DateFromParts

For example:

```CSharp
var count = context.Orders.Count(c => date > EF.Functions.DateFromParts(DateTime.Now.Year, 12, 25));

```

Documentation is tracked by issue [#2079](https://github.com/dotnet/EntityFramework.Docs/issues/2079).

### Query translations for more byte array constructs

Queries using Contains, Length, SequenceEqual, etc. on byte[] properties are now translated to SQL.

Preliminary documentation is included in the [EF weekly status for December 5, 2019](https://github.com/dotnet/efcore/issues/15403#issuecomment-562332863).

Additional documentation is tracked by issue [#2079](https://github.com/dotnet/EntityFramework.Docs/issues/2079).

### Query translation for Reverse

Queries using `Reverse` are now translated. For example:

```CSharp
context.Employees.OrderBy(e => e.EmployeeID).Reverse()
```

Documentation is tracked by issue [#2079](https://github.com/dotnet/EntityFramework.Docs/issues/2079).

### Query translation for bitwise operators

Queries using bitwise operators are now translated in more cases For example:

```CSharp
context.Orders.Where(o => ~o.OrderID == negatedId)
```

Documentation is tracked by issue [#2079](https://github.com/dotnet/EntityFramework.Docs/issues/2079).

### Query translation for strings on Cosmos

Queries that use the string methods Contains, StartsWith, and EndsWith are now translated when using the Azure Cosmos DB provider.

Documentation is tracked by issue [#2079](https://github.com/dotnet/EntityFramework.Docs/issues/2079).
