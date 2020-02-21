---
title: What's New in EF Core 5.0
author: ajcvickers
ms.date: 01/29/2020
uid: core/what-is-new/ef-core-5.0/whatsnew.md
---

# What's New in EF Core 5.0

EF Core 5.0 is currently in development.
This page will contain an overview of interesting changes introduced in each preview.
The first preview of EF Core 5.0 is tentatively expected in in the first quarter of 2020.

This page does not duplicate the [plan for EF Core 5.0](plan.md).
The plan describes the overall themes for EF Core 5.0, including everything we are planning to include before shipping the final release.

We will add links from here to the official documentation as it is published.

## Preview 1 (Not yet shipped)

### Simple logging

This feature adds functionality similar to `Database.Log` in EF6.
That is, it provides a simple way to get logs from EF Core without the need to configure any kind of external logging framework.

Preliminary documentation is included in the [EF weekly status for December 5, 2019](https://github.com/dotnet/efcore/issues/15403#issuecomment-562332863).

Additional documentation is tracked by issue [#2085](https://github.com/dotnet/EntityFramework.Docs/issues/2085).

### Simple way to get generated SQL

EF Core 5.0 introduces the `ToQueryString` extension method which will return the SQL that EF Core will generate when executing a LINQ query.

Preliminary documentation is included in the [EF weekly status for January 9, 2020](https://github.com/dotnet/efcore/issues/19549#issuecomment-572823246).

Additional documentation is tracked by issue [#1331](https://github.com/dotnet/EntityFramework.Docs/issues/1331).

### Enhanced debug views

Debug views are an easy way to look at the internals of EF Core when debugging issues.
A debug view for the Model was implemented some time ago.
For EF Core 5.0, we have made the model view easier to read and added a new debug view for tracked entities in the state manager.

Preliminary documentation is included in the [EF weekly status for December 12, 2019](https://github.com/dotnet/efcore/issues/15403#issuecomment-565196206).

Additional documentation is tracked by issue [#2086](https://github.com/dotnet/EntityFramework.Docs/issues/2086).

### Connection or connection string can be changed on initialized DbContext

It is now easier to create a DbContext instance without any connection or connection string.
Also, the connection or connection string can now be mutated on the context instance.
This allows the same context instance to dynamically connect to different databases.

Documentation is tracked by issue [#2075](https://github.com/dotnet/EntityFramework.Docs/issues/2075).

### Change-tracking proxies

EF Core can now generate runtime proxies that automatically implement [INotifyPropertyChanging](https://docs.microsoft.com/dotnet/api/system.componentmodel.inotifypropertychanging?view=netcore-3.1) and [INotifyPropertyChanged](https://docs.microsoft.com/dotnet/api/system.componentmodel.inotifypropertychanged?view=netcore-3.1).
These then report value changes on entity properties directly to EF Core, avoiding the need to scan for changes.
However, proxies come with their own set of limitations, so they are not for everyone.

Documentation is tracked by issue [#2076](https://github.com/dotnet/EntityFramework.Docs/issues/2076).

### Improved handling of database null semantics

Relational databases typically treat NULL as an unknown value and therefore not equal to any other NULL.
C#, on the other hand, treats null as a defined value which compares equal to any other null.
EF Core by default translates queries so that they use C# null semantics.
EF Core 5.0 greatly improves the efficiency of these translations.

Documentation is tracked by issue [#1612](https://github.com/dotnet/EntityFramework.Docs/issues/1612).

### Indexer properties

EF Core 5.0 supports mapping of C# indexer properties.
This allows entities to act as property bags where columns are mapped to named properties in the bag.

Documentation is tracked by issue [#2018](https://github.com/dotnet/EntityFramework.Docs/issues/2018).

### Generation of check constraints for enum mappings

EF Core 5.0 Migrations can now generate CHECK constraints for enum property mappings.
For example:

```SQL
MyEnumColumn VARCHAR(10) NOT NULL CHECK (MyEnumColumn IN('Useful', 'Useless', 'Unknown'))
```

Documentation is tracked by issue [#2082](https://github.com/dotnet/EntityFramework.Docs/issues/2082).

### Query translations for more DateTime constructs

Queries containing new DataTime construction are now translated.
Also, the SQL Server function DateDiffWeek is now mapped.

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
