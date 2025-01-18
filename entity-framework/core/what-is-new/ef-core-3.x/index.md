---
title: New features in Entity Framework Core 3.x - EF Core
description: Changes and improvements in Entity Framework Core 3.x
author: SamMonoRT
ms.date: 09/05/2020
uid: core/what-is-new/ef-core-3.x/index
---

# New features in Entity Framework Core 3.x

The following list includes the major new features in EF Core 3.x

As a major release, EF Core 3.x also contains several [breaking changes](xref:core/what-is-new/ef-core-3.x/breaking-changes), which are API improvements that may have negative impact on existing applications.

## LINQ overhaul

LINQ enables you to write database queries using your .NET language of choice, taking advantage of rich type information to offer IntelliSense and compile-time type checking.
But LINQ also enables you to write an unlimited number of complicated queries containing arbitrary expressions (method calls or operations).
How to handle all those combinations is the main challenge for LINQ providers.

In EF Core 3.x, we rearchitected our LINQ provider to enable translating more query patterns into SQL, generating efficient queries in more cases, and preventing inefficient queries from going undetected. The new LINQ provider is the foundation over which we'll be able to offer new query capabilities and performance improvements in future releases, without breaking existing applications and data providers.

### Restricted client evaluation

The most important design change has to do with how we handle LINQ expressions that cannot be converted to parameters or translated to SQL.

In previous versions, EF Core identified what portions of a query could be translated to SQL, and executed the rest of the query on the client.
This type of client-side execution is desirable in some situations, but in many other cases it can result in inefficient queries.

For example, if EF Core 2.2 couldn't translate a predicate in a `Where()` call, it executed an SQL statement without a filter, transferred all the rows from the database, and then filtered them in-memory:

```csharp
var specialCustomers = context.Customers
    .Where(c => c.Name.StartsWith(n) && IsSpecialCustomer(c));
```

That may be acceptable if the database contains a small number of rows but can result in significant performance issues or even application failure if the database contains a large number of rows.

In EF Core 3.x, we've restricted client evaluation to only happen on the top-level projection (essentially, the last call to `Select()`).
When EF Core 3.x detects expressions that can't be translated anywhere else in the query, it throws a runtime exception.

To evaluate a predicate condition on the client as in the previous example, developers now need to explicitly switch evaluation of the query to LINQ to Objects:

```csharp
var specialCustomers = context.Customers
    .Where(c => c.Name.StartsWith(n))
    .AsEnumerable() // switches to LINQ to Objects
    .Where(c => IsSpecialCustomer(c));
```

See the [breaking changes documentation](xref:core/what-is-new/ef-core-3.x/breaking-changes#linq-queries-are-no-longer-evaluated-on-the-client) for more details about how this can affect existing applications.

### Single SQL statement per LINQ query

Another aspect of the design that changed significantly in 3.x is that we now always generate a single SQL statement per LINQ query. In previous versions, we used to generate multiple SQL statements in certain cases, translated `Include()` calls on collection navigation properties and translated queries that followed certain patterns with subqueries. Although this was in some cases convenient, and for `Include()` it even helped avoid sending redundant data over the wire, the implementation was complex, and it resulted in some extremely inefficient behaviors (N+1 queries). There were situations in which the data returned across multiple queries was potentially inconsistent.

Similarly to client evaluation, if EF Core 3.x can't translate a LINQ query into a single SQL statement, it throws a runtime exception. But we made EF Core capable of translating many of the common patterns that used to generate multiple queries to a single query with JOINs.

## Azure Cosmos DB support

The Azure Cosmos DB provider for EF Core enables developers familiar with the EF programming model to easily target Azure Cosmos DB as an application database. The goal is to make some of the advantages of Azure Cosmos DB, like global distribution, "always on" availability, elastic scalability, and low latency, even more accessible to .NET developers. The provider enables most EF Core features, like automatic change tracking, LINQ, and value conversions, against Azure Cosmos DB for NoSQL.

See the [Azure Cosmos DB provider documentation](xref:core/providers/cosmos/index) for more details.

## C# 8.0 support

EF Core 3.x takes advantage of a couple of the [new features in C# 8.0](/dotnet/csharp/whats-new/csharp-8):

### Asynchronous streams

Asynchronous query results are now exposed using the new standard `IAsyncEnumerable<T>` interface and can be consumed using `await foreach`.

```csharp
var orders =
    from o in context.Orders
    where o.Status == OrderStatus.Pending
    select o;

await foreach(var o in orders.AsAsyncEnumerable())
{
    Process(o);
}
```

See the [asynchronous streams in the C# documentation](/dotnet/csharp/whats-new/csharp-8#asynchronous-streams) for more details.

### Nullable reference types

When this new feature is enabled in your code, EF Core examines the nullability of reference type properties and applies it to corresponding columns and relationships in the database: properties of non-nullable references types are treated as if they had the `[Required]` data annotation attribute.

For example, in the following class, properties marked as of type `string?` will be configured as optional, whereas `string` will be configured as required:

```csharp
public class Customer
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? MiddleName { get; set; }
}
```

See [Working with nullable reference types](xref:core/miscellaneous/nullable-reference-types) in the EF Core documentation for more details.

## Interception of database operations

The new interception API in EF Core 3.x allows providing custom logic to be invoked automatically whenever low-level database operations occur as part of the normal operation of EF Core. For example, when opening connections, committing transactions, or executing commands.

Similarly to the interception features that existed in EF 6, interceptors allow you to intercept operations before or after they happen. When you intercept them before they happen, you are allowed to by-pass execution and supply alternate results from the interception logic.

For example, to manipulate command text, you can create a `DbCommandInterceptor`:

```csharp
public class HintCommandInterceptor : DbCommandInterceptor
{
    public override InterceptionResult<DbDataReader> ReaderExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result)
    {
        // Manipulate the command text, etc. here...
        command.CommandText += " OPTION (OPTIMIZE FOR UNKNOWN)";
        return result;
    }
}
```

And register it with your `DbContext`:

```csharp
services.AddDbContext(b => b
    .UseSqlServer(connectionString)
    .AddInterceptors(new HintCommandInterceptor()));
```

## Reverse engineering of database views

Query types, which represent data that can be read from the database but not updated, have been renamed to [keyless entity types](xref:core/modeling/keyless-entity-types).
As they are an excellent fit for mapping database views in most scenarios, EF Core now automatically creates keyless entity types when reverse engineering database views.

For example, using the [dotnet ef command-line tool](xref:core/cli/dotnet) you can type:

```dotnetcli
dotnet ef dbcontext scaffold "Server=(localdb)\mssqllocaldb;Database=Blogging;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer
```

And the tool will now automatically scaffold types for views and tables without keys:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Names>(entity =>
    {
        entity.HasNoKey();
        entity.ToView("Names");
    });

    modelBuilder.Entity<Things>(entity =>
    {
        entity.HasNoKey();
    });
}
```

## Dependent entities sharing the table with the principal are now optional

Starting with EF Core 3.x, if `OrderDetails` is owned by `Order` or explicitly mapped to the same table, it will be possible to add an `Order` without an `OrderDetails` and all of the `OrderDetails` properties, except the primary key will be mapped to nullable columns.

When querying, EF Core will set `OrderDetails` to `null` if any of its required properties doesn't have a value, or if it has no required properties besides the primary key and all properties are `null`.

```csharp
public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public OrderDetails Details { get; set; }
}

[Owned]
public class OrderDetails
{
    public int Id { get; set; }
    public string ShippingAddress { get; set; }
}
```

## EF 6.3 on .NET Core

This isn't really an EF Core 3.x feature, but we think it is important to many of our current customers.

We understand that many existing applications use previous versions of EF, and that porting them to EF Core only to take advantage of .NET Core can require a significant effort.
For that reason, we decided to port the newest version of EF 6 to run on .NET Core 3.x.

For more details, see [what's new in EF 6](xref:ef6/what-is-new/index).

## Postponed features

Some features originally planned for EF Core 3.x were postponed to future releases:

- Ability to ignore parts of a model in migrations, tracked as [#2725](https://github.com/dotnet/efcore/issues/2725).
- Property bag entities, tracked as two separate issues: [#9914](https://github.com/dotnet/efcore/issues/9914) about shared-type entities and [#13610](https://github.com/dotnet/efcore/issues/13610) about indexed property mapping support.
