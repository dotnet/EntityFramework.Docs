---
title: What's New in EF Core 5.0
description: Overview of new features in EF Core 5.0
author: ajcvickers
ms.date: 03/30/2020
uid: core/what-is-new/ef-core-5.0/whatsnew.md
---

# What's New in EF Core 5.0

EF Core 5.0 is currently in development.
This page will contain an overview of interesting changes introduced in each preview.

This page does not duplicate the [plan for EF Core 5.0](plan.md).
The plan describes the overall themes for EF Core 5.0, including everything we are planning to include before shipping the final release.

We will add links from here to the official documentation as it is published.

## Preview 3

### Filtered Include

The Include method now supports filtering of the entities included.
For example:

```CSharp
var blogs = context.Blogs
    .Include(e => e.Posts.Where(p => p.Title.Contains("Cheese")))
    .ToList();
```

This query will return blogs together with each associated post, but only when the post title contains "Cheese".

Skip and Take can also be used to reduce the number of included entities.
For example:
 
```CSharp
var blogs = context.Blogs
    .Include(e => e.Posts.OrderByDescending(post => post.Title).Take(5)))
    .ToList();
```
This query will return blogs with at most five posts included on each blog.

See the [Include documentation](xref:core/querying/related-data#filtered-include) for full details.

### New ModelBuilder API for navigation properties

Navigation properties are primarily configured when [defining relationships](xref:core/modeling/relationships).
However, the new `Navigation` method can be used in the cases where navigation properties need additional configuration.
For example, to set a backing field for the navigation when the field would not be found by convention:

```CSharp
modelBuilder.Entity<Blog>().Navigation(e => e.Posts).HasField("_myposts");
```

Note that the `Navigation` API does not replace relationship configuration.
Instead it allows additional configuration of navigation properties in already discovered or defined relationships.

Documentation is tracked by issue [#2302](https://github.com/dotnet/EntityFramework.Docs/issues/2302).

### New command-line parameters for namespaces and connection strings 

Migrations and scaffolding now allow namespaces to be specified on the command line.
For example, to reverse engineer a database putting the context and model classes in different namespaces: 

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

For performance reasons, EF doesn't do additional null-checks when reading values from the database.
This can result in exceptions that are hard to root-cause when an unexpected null is encountered.

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

The partition key to use for a given query can now be specified in the query.
For example:

```CSharp
await context.Set<Customer>()
             .WithPartitionKey(myPartitionKey)
             .FirstAsync();
```

Documentation is tracked by issue [#2199](https://github.com/dotnet/EntityFramework.Docs/issues/2199).

### Support for the SQL Server DATALENGTH function

This can be accessed using the new `EF.Functions.DataLength` method.
For example:
```CSharp
var count = context.Orders.Count(c => 100 < EF.Functions.DataLength(c.OrderDate));
``` 

## Preview 2

### Use a C# attribute to specify a property backing field

A C# attribute can now be used to specify the backing field for a property.
This attribute allows EF Core to still write to and read from the backing field as would normally happen, even when the backing field cannot be found automatically.
For example:

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

EF Core uses a discriminator column for [TPH mapping of an inheritance hierarchy](/ef/core/modeling/inheritance).
Some performance enhancements are possible so long as EF Core knows all possible values for the discriminator.
EF Core 5.0 now implements these enhancements.

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

This feature adds functionality similar to `Database.Log` in EF6.
That is, it provides a simple way to get logs from EF Core without the need to configure any kind of external logging framework.

Preliminary documentation is included in the [EF weekly status for December 5, 2019](https://github.com/dotnet/efcore/issues/15403#issuecomment-562332863).

Additional documentation is tracked by issue [#2085](https://github.com/dotnet/EntityFramework.Docs/issues/2085).

### Simple way to get generated SQL

EF Core 5.0 introduces the `ToQueryString` extension method, which will return the SQL that EF Core will generate when executing a LINQ query.

Preliminary documentation is included in the [EF weekly status for January 9, 2020](https://github.com/dotnet/efcore/issues/19549#issuecomment-572823246).

Additional documentation is tracked by issue [#1331](https://github.com/dotnet/EntityFramework.Docs/issues/1331).

### Use a C# attribute to indicate that an entity has no key

An entity type can now be configured as having no key using the new `KeylessAttribute`.
For example:

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

It is now easier to create a DbContext instance without any connection or connection string.
Also, the connection or connection string can now be mutated on the context instance.
This feature allows the same context instance to dynamically connect to different databases.

Documentation is tracked by issue [#2075](https://github.com/dotnet/EntityFramework.Docs/issues/2075).

### Change-tracking proxies

EF Core can now generate runtime proxies that automatically implement [INotifyPropertyChanging](https://docs.microsoft.com/dotnet/api/system.componentmodel.inotifypropertychanging?view=netcore-3.1) and [INotifyPropertyChanged](https://docs.microsoft.com/dotnet/api/system.componentmodel.inotifypropertychanged?view=netcore-3.1).
These then report value changes on entity properties directly to EF Core, avoiding the need to scan for changes.
However, proxies come with their own set of limitations, so they are not for everyone.

Documentation is tracked by issue [#2076](https://github.com/dotnet/EntityFramework.Docs/issues/2076).

### Enhanced debug views

Debug views are an easy way to look at the internals of EF Core when debugging issues.
A debug view for the Model was implemented some time ago.
For EF Core 5.0, we have made the model view easier to read and added a new debug view for tracked entities in the state manager.

Preliminary documentation is included in the [EF weekly status for December 12, 2019](https://github.com/dotnet/efcore/issues/15403#issuecomment-565196206).

Additional documentation is tracked by issue [#2086](https://github.com/dotnet/EntityFramework.Docs/issues/2086).

### Improved handling of database null semantics

Relational databases typically treat NULL as an unknown value and therefore not equal to any other NULL.
While C# treats null as a defined value, which compares equal to any other null.
EF Core by default translates queries so that they use C# null semantics.
EF Core 5.0 greatly improves the efficiency of these translations.

Documentation is tracked by issue [#1612](https://github.com/dotnet/EntityFramework.Docs/issues/1612).

### Indexer properties

EF Core 5.0 supports mapping of C# indexer properties.
These properties allow entities to act as property bags where columns are mapped to named properties in the bag.

Documentation is tracked by issue [#2018](https://github.com/dotnet/EntityFramework.Docs/issues/2018).

### Generation of check constraints for enum mappings

EF Core 5.0 Migrations can now generate CHECK constraints for enum property mappings.
For example:

```SQL
MyEnumColumn VARCHAR(10) NOT NULL CHECK (MyEnumColumn IN ('Useful', 'Useless', 'Unknown'))
```

Documentation is tracked by issue [#2082](https://github.com/dotnet/EntityFramework.Docs/issues/2082).

### IsRelational

A new `IsRelational` method has been added in addition to the existing `IsSqlServer`, `IsSqlite`, and `IsInMemory`.
This method can be used to test if the DbContext is using any relational database provider.
For example:

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

The Azure Cosmos DB database provider now supports optimistic concurrency using ETags.
Use the model builder in OnModelCreating to configure an ETag:

```CSharp
builder.Entity<Customer>().Property(c => c.ETag).IsEtagConcurrency();
```

SaveChanges will then throw an `DbUpdateConcurrencyException` on a concurrency conflict, which [can be handled](https://docs.microsoft.com/ef/core/saving/concurrency) to implement retries, etc.

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

Queries using `Reverse` are now translated.
For example:

```CSharp
context.Employees.OrderBy(e => e.EmployeeID).Reverse()
```

Documentation is tracked by issue [#2079](https://github.com/dotnet/EntityFramework.Docs/issues/2079).

### Query translation for bitwise operators

Queries using bitwise operators are now translated in more cases
For example:

```CSharp
context.Orders.Where(o => ~o.OrderID == negatedId)
```

Documentation is tracked by issue [#2079](https://github.com/dotnet/EntityFramework.Docs/issues/2079).

### Query translation for strings on Cosmos

Queries that use the string methods Contains, StartsWith, and EndsWith are now translated when using the Azure Cosmos DB provider.

Documentation is tracked by issue [#2079](https://github.com/dotnet/EntityFramework.Docs/issues/2079).
