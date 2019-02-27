---
title: Breaking changes in EF Core 3.0 - EF Core
author: divega
ms.date: 02/19/2019
ms.assetid: EE2878C9-71F9-4FA5-9BC4-60517C7C9830
uid: core/what-is-new/ef-core-3.0/breaking-changes
---

# Breaking changes included in EF Core 3.0 (in preview)

> [!IMPORTANT]
> Please note that the feature sets and schedules of future releases are always subject to change, and although we will try to keep this page up to date, it may not reflect our latest plans at all times.

## SQLite

* Microsoft.EntityFrameworkCore.Sqlite now depends on SQLitePCLRaw.bundle_e_sqlite3 instead of SQLitePCLRaw.bundle_green. This makes the version of SQLite used on iOS consistent with other platforms.
* Removed SqliteDbContextOptionsBuilder.SuppressForeignKeyEnforcement(). EF Core no longer sends `PRAGMA foreign_keys = 1` when a connection is opened. Foreign keys are enabled by default in SQLitePCLRaw.bundle_e_sqlite3. If you're not using that, you can enable foreign keys by specifying `Foreign Keys=True` in your connection string.

## QueryType and EntityType consolidation ([#14194](https://github.com/aspnet/EntityFrameworkCore/issues/14194))

[Query types](xref:core/modeling/query-types) were introduced in 2.1 as a means to query data that doesn't contain a primary key in a structured way. However it caused some confusion when deciding whether to configure something as an entity type or a query type. Threfore we have decided to bring the two closer together in the API.

A query type now becomes just an entity type without a primary key, but will have the same functionality as in previous versions. The following parts of the API are now obsolete:
* **`ModelBuilder.Query<>()`** - Instead `ModelBuilder.Entity<>().HasNoKey()` needs to be called to mark an entity type as having no keys. This would still not be configured by convention to avoid misconfiguration when a primary key is expected, but doesn't match the convention.
* **`DbQuery<>`** - Instead `DbSet<>` should be used.
* **`DbContext.Query<>()`** - Instead `DbContext.Set<>()` should be used.

## Improvements to the owned entity type configuration API ([#12444](https://github.com/aspnet/EntityFrameworkCore/issues/12444), [#9148](https://github.com/aspnet/EntityFrameworkCore/issues/9148), [#14153](https://github.com/aspnet/EntityFrameworkCore/issues/14153))

Now there is a fluent way to configure a navigation to the owner using `WithOwner()`:

```C#
modelBuilder.Entity<Order>.OwnsOne(e => e.Details).WithOwner(e => e.Order);
```

The configuration related to the relationship between owner and owned should now be chained after `WithOwner()` similarly to how other relationships are configured. While the configuration for the owned type itself would still be chained after `OwnsOne()/OwnsMany()` call:

```C#
modelBuilder.Entity<Order>.OwnsOne(e => e.Details, eb =>
    {
        eb.WithOwner()
            .HasForeignKey(e => e.AlternateId)
            .HasConstraintName("FK_OrderDetails");
            
        eb.ToTable("OrderDetails");
        eb.HasKey(e => e.AlternateId);
        eb.HasIndex(e => e.Id);

        eb.HasOne(e => e.Customer).WithOne();

        eb.HasData(
            new OrderDetails
            {
                AlternateId = 1,
                Id = -1
            });
    });
```

Additionally calling `Entity()`, `HasOne()` or `Set()` with an owned type target will now throws an exception.

## FK property convention does not match same name as principal anymore ([#13274](https://github.com/aspnet/EntityFrameworkCore/issues/13274))

Consider the following model
```C#
public class Customer
{
    public int CustomerId { get; set; }
    public ICollection<Order> Orders { get; set; }
}

public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
}

```
In previous versions of EF Core `CustomerId` property would be used for the foreign key by convention, but if `Order` is an owned type that would also make `CustomerId` the primary key and this is usually not the exceptation.

Now EF Core will not try to use properties for foreign keys by convention if they have the same name as the principal property. Principal type name + principal property name and navigation + principal property name patterns would still be used:
```C#
public class Customer
{
    public int Id { get; set; }
    public ICollection<Order> Orders { get; set; }
}

public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
}
```
```C#
public class Customer
{
    public int Id { get; set; }
    public ICollection<Order> Orders { get; set; }
}

public class Order
{
    public int Id { get; set; }
    public int BuyerId { get; set; }
    public Customer Buyer { get; set; }
}
```

## ToTable() on derived type throws an exception ([#11811](https://github.com/aspnet/EntityFrameworkCore/issues/11811))

Previously `ToTable()` called on a derived type would be just ignored as the only inheritance mapping strategy was TPH. In preparation for adding TPT and TPC support `ToTable()` called on a derived type will now throw an exception to avoid an unexpected mapping change in the future.

## ForSqlServerHasIndex replaced with HasIndex ([#12366](https://github.com/aspnet/EntityFrameworkCore/issues/12366))

`ForSqlServerHasIndex().ForSqlServerInclude()` provided a way to configure columns used with `INCLUDE`, now the same can be accomplished with `HasIndex().ForSqlServerInclude()`