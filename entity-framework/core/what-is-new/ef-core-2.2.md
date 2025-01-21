---
title: What is new in EF Core 2.2 - EF Core
description: Changes and improvements in Entity Framework Core 2.2
author: SamMonoRT
ms.date: 11/14/2018
uid: core/what-is-new/ef-core-2.2
---

# New features in EF Core 2.2

## Spatial data support

Spatial data can be used to represent the physical location and shape of objects.
Many databases can natively store, index, and query spatial data.
Common scenarios include querying for objects within a given distance, and testing if a polygon contains a given location.
EF Core 2.2 now supports working with spatial data from various databases using types from the [NetTopologySuite](https://github.com/NetTopologySuite/NetTopologySuite) (NTS) library.

Spatial data support is implemented as a series of provider-specific extension packages.
Each of these packages contributes mappings for NTS types and methods, and the corresponding spatial types and functions in the database.
Such provider extensions are now available for [SQL Server](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.SqlServer.NetTopologySuite/), [SQLite](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Sqlite.NetTopologySuite/), and [PostgreSQL](https://www.nuget.org/packages/Npgsql.EntityFrameworkCore.PostgreSQL.NetTopologySuite/) (from the [Npgsql project](https://www.npgsql.org/)).
Spatial types can be used directly with the [EF Core in-memory provider](xref:core/providers/in-memory/index) without additional extensions.

Once the provider extension is installed, you can add properties of supported types to your entities. For example:

```csharp
using NetTopologySuite.Geometries;

namespace MyApp
{
  public class Friend
  {
    [Key]
    public string Name { get; set; }

    [Required]
    public Point Location { get; set; }
  }
}
```

You can then persist entities with spatial data:

```csharp
using (var context = new MyDbContext())
{
    context.Add(
        new Friend
        {
            Name = "Bill",
            Location = new Point(-122.34877, 47.6233355) {SRID = 4326 }
        });
    await context.SaveChangesAsync();
}
```

And you can execute database queries based on spatial data and operations:

```csharp
var nearestFriends =
    await (from f in context.Friends
    orderby f.Location.Distance(myLocation) descending
    select f).Take(5).ToListAsync();
```

For more information on this feature, see the [spatial types documentation](xref:core/modeling/spatial).

## Collections of owned entities

EF Core 2.0 added the ability to model ownership in one-to-one associations.
EF Core 2.2 extends the ability to express ownership to one-to-many associations.
Ownership helps constrain how entities are used.

For example, owned entities:

- Can only ever appear on navigation properties of other entity types.
- Are automatically loaded, and can only be tracked by a DbContext alongside their owner.

In relational databases, owned collections are mapped to separate tables from the owner, just like regular one-to-many associations.
But in document-oriented databases, we plan to nest owned entities (in owned collections or references) within the same document as the owner.

You can use the feature by calling the new OwnsMany() API:

```csharp
modelBuilder.Entity<Customer>().OwnsMany(c => c.Addresses);
```

For more information, see the [updated owned entities documentation](xref:core/modeling/owned-entities#collections-of-owned-types).

## Query tags

This feature simplifies the correlation of LINQ queries in code with generated SQL queries captured in logs.

To take advantage of query tags, you annotate a LINQ query using the new TagWith() method.
Using the spatial query from a previous example:

```csharp
var nearestFriends =
    await (from f in context.Friends.TagWith(@"This is my spatial query!")
    orderby f.Location.Distance(myLocation) descending
    select f).Take(5).ToListAsync();
```

This LINQ query will produce the following SQL output:

```sql
-- This is my spatial query!

SELECT TOP(@__p_1) [f].[Name], [f].[Location]
FROM [Friends] AS [f]
ORDER BY [f].[Location].STDistance(@__myLocation_0) DESC
```

For more information, see the [query tags documentation](xref:core/querying/tags).
