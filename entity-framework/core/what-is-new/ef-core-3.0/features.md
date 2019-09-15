---
title: New features in EF Core 3.0 - EF Core
author: divega
ms.date: 02/19/2019
ms.assetid: 2EBE2CCC-E52D-483F-834C-8877F5EB0C0C
uid: core/what-is-new/ef-core-3.0/features
---

# New features included in EF Core 3.0

The following list includes the major new features planned for EF Core 3.0.

EF Core 3.0 is a major release and also contains numerous [breaking changes](xref:core/what-is-new/ef-core-3.0/breaking-changes), which are API improvements that may have negative impact on existing applications.  

## LINQ improvements 

[Tracking Issue #12795](https://github.com/aspnet/EntityFrameworkCore/issues/12795)

LINQ enables you to write database queries without leaving your language of choice, taking advantage of rich type information to offer IntelliSense and compile-time type checking.
But LINQ also enables you to write an unlimited number of complicated queries containing arbitrary expressions (method calls or operations).
This has always been a significant challenge for LINQ providers.
In EF Core 3.0, we've rewrote our LINQ implementation to enable translating more expressions into SQL, to generate efficient queries in more cases, to prevent inefficient queries from going undetected, and to make it easier for us to intorduce new query capabilities and performance improvements in the future without breaking existing applications and providers.

### Client evaluation

The main design change in 3.0 has to do with how EF Core deals with expressions in LINQ queries that cannot be translated to SQL or parameters:

In the first few versions of EF Core, we solved that by figuring out what portions of a query could be translated to SQL, and then by allowing the rest of the query to execute in memory on the client.
This client-side execution can be desirable in some situations, but in many other cases it can result in inefficient queries that may not be identified until an application is deployed to production and queries are executed against large amounts of data.
For example, when in previous versions a filter in a `Where()` could not be translated to SQL, all rows would be transferred into the client's memory, and the filter would be evaluated in-memory. That can be acceptable if the database contains a small number of rows, but can cause the appliction to load large numbers of rows into memory if later on, the database contains a large number or rows.
In EF Core 3.0 client evaluation is restricted to the top-level projection (the last call to `Select()`) in the query and non-translatable expressions anywhere else in the query cause the query to fail with an error.

## Cosmos DB support 

[Tracking Issue #8443](https://github.com/aspnet/EntityFrameworkCore/issues/8443)

The Cosmos DB provider for EF Core enables developers familiar with the EF programing model to easily target Azure Cosmos DB as an application database.
The goal is to make some of the advantages of Cosmos DB, like global distribution, "always on" availability, elastic scalability, and low latency, even more accessible to .NET developers.
The provider will enable most EF Core features, like automatic change tracking, LINQ, and value conversions, against the SQL API in Cosmos DB.

## C# 8.0 support

[Tracking Issue #12047](https://github.com/aspnet/EntityFrameworkCore/issues/12047)
[Tracking Issue #10347](https://github.com/aspnet/EntityFrameworkCore/issues/10347)

With EF Core 3.0, customers can take advantage of some of the [new features coming in C# 8.0](https://blogs.msdn.microsoft.com/dotnet/2018/11/12/building-c-8-0/):

### Asynchronous streams
Asynchronous query results are now exposed using the new standard `IAsyncEnumerable<T>` interface and can be consumed using `await foreach`. 
### Nullable reference types 
When this new feature is enabled in your code, EF Core can reason about the nullability of properties of refrence types (either of primitive types like string or naviagion properties) to decide the requiredness (that is the nullability) of columns and relationships in the database.

## Interception
The new interception API in EF Core 3.0 allows programatically observing and modifying the outcome of low-level database operations that occur as part of the normal operation of EF Core, such as opening connections, initating transactions, and executing commands. 

## Reverse engineering of database views

[Tracking Issue #1679](https://github.com/aspnet/EntityFrameworkCore/issues/1679)

Entity types without keys (previously known as [query types](xref:core/modeling/query-types)) represent data that can be read from the database, but cannot be updated.
This characteristic makes them an excellent fit for mapping database views in most scenarios, so we automated the creation of entity types without keys when reverse engineering database views.

## Dependent entities sharing the table with the principal are now optional

[Tracking Issue #9005](https://github.com/aspnet/EntityFrameworkCore/issues/9005)

This feature will be introduced in EF Core 3.0-preview 4.

Consider the following model:
```C#
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

Starting with EF Core 3.0, if `OrderDetails` is owned by `Order` or explicitly mapped to the same table, it will be possible to add an `Order` without an `OrderDetails` and all of the `OrderDetails` properties, except the primary key will be mapped to nullable columns.

When querying, EF Core will set `OrderDetails` to `null` if any of its required properties doesn't have a value, or if it has no required properties besides the primary key and all properties are `null`.

## EF 6.3 on .NET Core

[Tracking Issue EF6#271](https://github.com/aspnet/EntityFramework6/issues/271)

Work on this feature has started but it isn't included in the current preview. 

We understand that many existing applications use previous versions of EF, and that porting them to EF Core only to take advantage of .NET Core can sometimes require a significant effort.
For that reason, we will be adapting the next version of EF 6 to run on .NET Core 3.0.
We are doing this to facilitate porting existing applications with minimal changes.
There are going to be some limitations. 
For example:
- It will require new providers to work with other databases besides the included SQL Server support on .NET Core
- Spatial support with SQL Server won't be enabled

Note also that there are no new features planned for EF 6 at this point.

# Postponed features

These two features where originally planned for EF Core 3.0, but they have been postponed to future releases

- Allowing ignoring parts of a model in migrations
- Property bag entities
