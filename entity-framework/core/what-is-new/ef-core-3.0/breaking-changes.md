---
title: Breaking changes in EF Core 3.0 - EF Core
author: divega
ms.date: 02/19/2019
ms.assetid: EE2878C9-71F9-4FA5-9BC4-60517C7C9830
uid: core/what-is-new/ef-core-3.0/breaking-changes
---

# Breaking changes included in EF Core 3.0 (currently in preview)

> [!IMPORTANT]
> Please note that the feature sets and schedules of future releases are always subject to change, and although we will try to keep this page up to date, it may not reflect our latest plans at all times.

The following API and behavior changes have the potential to break applications developed for EF Core 2.2.x when upgrading them to 3.0.0.
Changes that we expect to only impact database providers are documented under [provider changes](../../providers/provider-log.md).
Breaks in new features introduced from one 3.0 preview to another 3.0 preview aren't documented here.

## LINQ queries are no longer evaluated on the client

[Tracking Issue #14935](https://github.com/aspnet/EntityFrameworkCore/issues/14935)
[Also see issue #12795](https://github.com/aspnet/EntityFrameworkCore/issues/12795)

This change is introduced in EF Core 3.0-preview 4.

**Old behavior**

Before 3.0, when EF Core couldn't convert an expression that was part of a query to either SQL or a parameter, it automatically evaluated the expression on the client.
By default, client evaluation of potentially expensive expressions only triggered a warning.

**New behavior**

Starting with 3.0, EF Core only allows expressions in the top-level projection (the last `Select()` call in the query) to be evaluated on the client.
When expressions in any other part of the query can't be converted to either SQL or a parameter, an exception is thrown.

**Why**

Automatic client evaluation of queries allows many queries to be executed even if important parts of them can't be translated.
This behavior can result in unexpected and potentially damaging behavior that may only become evident in production.
For example, a condition in a `Where()` call which can't be translated can cause all rows from the table to be transferred from the database server, and the filter to be applied on the client.
This situation can easily go undetected if the table contains only a few rows in development, but hit hard when the application moves to production, where the table may contain millions of rows.
Client evaluation warnings also proved too easy to ignore during development.

Besides this, automatic client evaluation can lead to issues in which improving query translation for specific expressions caused unintended breaking changes between releases.

**Mitigations**

If a query can't be fully translated, then either rewrite the query in a form that can be translated, or use `AsEnumerable()`, `ToList()`, or similar to explicitly bring data back to the client where it can then be further processed using LINQ-to-Objects.

## Entity Framework Core is no longer part of the ASP.NET Core shared framework

[Tracking Issue Announcements#325](https://github.com/aspnet/Announcements/issues/325)

This change is introduced in ASP.NET Core 3.0-preview 1. 

**Old behavior**

Before ASP.NET Core 3.0, when you added a package reference to `Microsoft.AspNetCore.App` or `Microsoft.AspNetCore.All`, it would include EF Core and some of the EF Core data providers like the SQL Server provider.

**New behavior**

Starting in 3.0, the ASP.NET Core shared framework doesn't include EF Core or any EF Core data providers.

**Why**

Before this change, getting EF Core required different steps depending on whether the application targeted ASP.NET Core and SQL Server or not. 
Also, upgrading ASP.NET Core forced the upgrade of EF Core and the SQL Server provider, which isn't always desirable.

With this change, the experience of getting EF Core is the same across all providers, supported .NET implementations and application types.
Developers can also now control exactly when EF Core and EF Core data providers are upgraded.

**Mitigations**

To use EF Core in an ASP.NET Core 3.0 application or any other supported application, explicitly add a package reference to the EF Core database provider that your application will use.

## The EF Core command-line tool, dotnet ef, is no longer part of the .NET Core SDK

[Tracking Issue #14016](https://github.com/aspnet/EntityFrameworkCore/issues/14016)

This change is introduced in EF Core 3.0-preview 4 and the corresponding version of the .NET Core SDK.

**Old behavior**

Before 3.0, the `dotnet ef` tool was included in the .NET Core SDK and was readily available to use from the command line from any project without requiring extra steps. 

**New behavior**

Starting in 3.0, the .NET SDK does not include the `dotnet ef` tool, so before you can use it you have to explicitly install it as a local or global tool. 

**Why**

This change allows us to distribute and update `dotnet ef` as a regular .NET CLI tool on NuGet, consistent with the fact that the EF Core 3.0 is also always distributed as a NuGet package.

**Mitigations**

To be able to manage migrations or scaffold a `DbContext`, install `dotnet-ef` using the `dotnet tool install` command.
For example, to install it as a global tool, you can type this command:

  ``` console
  $ dotnet tool install --global dotnet-ef --version <exact-version>
  ```

You can also obtain it a local tool when you restore the dependencies of a project that declares it as a tooling dependency using a [tool manifest file](https://github.com/dotnet/cli/issues/10288).

## FromSql, ExecuteSql, and ExecuteSqlAsync have been renamed

[Tracking Issue #10996](https://github.com/aspnet/EntityFrameworkCore/issues/10996)

This change is introduced in EF Core 3.0-preview 4.

**Old behavior**

Before EF Core 3.0, these method names were overloaded to work with either a normal string or a string that should be interpolated into SQL and parameters.

**New behavior**

Starting with EF Core 3.0, use `FromSqlRaw`, `ExecuteSqlRaw`, and `ExecuteSqlRawAsync` to create a parameterized query where the parameters are passed separately from the query string.
For example:

```C#
context.Products.FromSqlRaw(
    "SELECT * FROM Products WHERE Name = {0}",
    product.Name);
```

Use `FromSqlInterpolated`, `ExecuteSqlInterpolated`, and `ExecuteSqlInterpolatedAsync` to create a parameterized query where the parameters are passed as part of an interpolated query string.
For example:

```C#
context.Products.FromSqlInterpolated(
    $"SELECT * FROM Products WHERE Name = {product.Name}");
```

Note that both of the queries above will produce the same parameterized SQL with the same SQL parameters.

**Why**

Method overloads like this make it very easy to accidentally call the raw string method when the intent was to call the interpolated string method, and the other way around.
This could result in queries not being parameterized when they should have been.

**Mitigations**

Switch to use the new method names.

## FromSql methods can only be specified on query roots

[Tracking Issue #15704](https://github.com/aspnet/EntityFrameworkCore/issues/15704)

This change is introduced in EF Core 3.0-preview 6.

**Old behavior**

Before EF Core 3.0, the `FromSql` method could be specified anywhere in the query.

**New behavior**

Starting with EF Core 3.0, the new `FromSqlRaw` and `FromSqlInterpolated` methods (which replace `FromSql`) can only be specified on query roots, i.e. directly on the `DbSet<>`. Attempting to specify them anywhere else will result in a compilation error.

**Why**

Specifying `FromSql` anywhere other than on a `DbSet` had no added meaning or added value, and could cause ambiguity in certain scenarios.

**Mitigations**

`FromSql` invocations should be moved to be directly on the `DbSet` to which they apply.

## Query execution is logged at Debug level

[Tracking Issue #14523](https://github.com/aspnet/EntityFrameworkCore/issues/14523)

This change is introduced in EF Core 3.0-preview 3.

**Old behavior**

Before EF Core 3.0, execution of queries and other commands was logged at the `Info` level.

**New behavior**

Starting with EF Core 3.0, logging of command/SQL execution is at the `Debug` level.

**Why**

This change was made to reduce the noise at the `Info` log level.

**Mitigations**

This logging event is defined by `RelationalEventId.CommandExecuting` with event ID 20100.
To log SQL at the `Info` level again, explicitly configure the level in `OnConfiguring` or `AddDbContext`.
For example:
```C#
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder
        .UseSqlServer(connectionString)
        .ConfigureWarnings(c => c.Log((RelationalEventId.CommandExecuting, LogLevel.Info)));
```

## Temporary key values are no longer set onto entity instances

[Tracking Issue #12378](https://github.com/aspnet/EntityFrameworkCore/issues/12378)

This change is introduced in EF Core 3.0-preview 2.

**Old behavior**

Before EF Core 3.0, temporary values were assigned to all key properties that would later have a real value generated by the database.
Usually these temporary values were large negative numbers.

**New behavior**

Starting with 3.0, EF Core stores the temporary key value as part of the entity's tracking information, and leaves the key property itself unchanged.

**Why**

This change was made to prevent temporary key values from erroneously becoming permanent when an entity that has been previously tracked by some `DbContext` instance is moved to a different `DbContext` instance. 

**Mitigations**

Applications that assign primary key values onto foreign keys to form associations between entities may depend on the old behavior if the primary keys are store-generated and belong to entities in the `Added` state.
This can be avoided by:
* Not using store-generated keys.
* Setting navigation properties to form relationships instead of setting foreign key values.
* Obtain the actual temporary key values from the entity's tracking information.
For example, `context.Entry(blog).Property(e => e.Id).CurrentValue` will return the temporary value even though `blog.Id` itself hasn't been set.

## DetectChanges honors store-generated key values

[Tracking Issue #14616](https://github.com/aspnet/EntityFrameworkCore/issues/14616)

This change is introduced in EF Core 3.0-preview 3.

**Old behavior**

Before EF Core 3.0, an untracked entity found by `DetectChanges` would be tracked in the `Added` state and inserted as a new row when `SaveChanges` is called.

**New behavior**

Starting with EF Core 3.0, if an entity is using generated key values and some key value is set, then the entity will be tracked in the `Modified` state.
This means that a row for the entity is assumed to exist and it will be updated when `SaveChanges` is called.
If the key value isn't set, or if the entity type isn't using generated keys, then the new entity will still be tracked as `Added` as in previous versions.

**Why**

This change was made to make it easier and more consistent to work with disconnected entity graphs while using store-generated keys.

**Mitigations**

This change can break an application if an entity type is configured to use generated keys but key values are explicitly set for new instances.
The fix is to explicitly configure the key properties to not use generated values.
For example, with the fluent API:

```C#
modelBuilder
    .Entity<Blog>()
    .Property(e => e.Id)
    .ValueGeneratedNever();
```

Or with data annotations:

```C#
[DatabaseGenerated(DatabaseGeneratedOption.None)]
public string Id { get; set; }
```

## Cascade deletions now happen immediately by default

[Tracking Issue #10114](https://github.com/aspnet/EntityFrameworkCore/issues/10114)

This change is introduced in EF Core 3.0-preview 3.

**Old behavior**

Before 3.0, EF Core applied cascading actions (deleting dependent entities when a required principal is deleted or when the relationship to a required principal is severed) did not happen until SaveChanges was called.

**New behavior**

Starting with 3.0, EF Core applies cascading actions as soon as the triggering condition is detected.
For example, calling `context.Remove()` to delete a principal entity will result in all tracked related required dependents also being set to `Deleted` immediately.

**Why**

This change was made to improve the experience for data binding and auditing scenarios where it is important to understand which entities will be deleted _before_ `SaveChanges` is called.

**Mitigations**

The previous behavior can be restored through settings on `context.ChangedTracker`.
For example:

```C#
context.ChangeTracker.CascadeDeleteTiming = CascadeTiming.OnSaveChanges;
context.ChangeTracker.DeleteOrphansTiming = CascadeTiming.OnSaveChanges;
```

## DeleteBehavior.Restrict has cleaner semantics

[Tracking Issue #12661](https://github.com/aspnet/EntityFrameworkCore/issues/12661)

This change is introduced in EF Core 3.0-preview 5.

**Old behavior**

Before 3.0, `DeleteBehavior.Restrict` created foreign keys in the database with `Restrict` semantics, but also changed internal fixup in a non-obvious way.

**New behavior**

Starting with 3.0, `DeleteBehavior.Restrict` ensures that foreign keys are created with `Restrict` semantics--that is, no cascades; throw on constraint violation--without also impacting EF internal fixup.

**Why**

This change was made to improve the experience for using `DeleteBehavior` in an intuitive manner, without unexpected side-effects.

**Mitigations**

The previous behavior can be restored by using `DeleteBehavior.ClientNoAction`.

## Query types are consolidated with entity types

[Tracking Issue #14194](https://github.com/aspnet/EntityFrameworkCore/issues/14194)

This change is introduced in EF Core 3.0-preview 3.

**Old behavior**

Before EF Core 3.0, [query types](xref:core/modeling/query-types) were a means to query data that doesn't define a primary key in a structured way.
That is, a query type was used for mapping entity types without keys (more likely from a view, but possibly from a table) while a regular entity type was used when a key was available (more likely from a table, but possibly from a view).

**New behavior**

A query type now becomes just an entity type without a primary key.
Keyless entity types have the same functionality as query types in previous versions.

**Why**

This change was made to reduce the confusion around the purpose of query types.
Specifically, they are keyless entity types and they are inherently read-only because of this, but they should not be used just because an entity type needs to be read-only.
Likewise, they are often mapped to views, but this is only because views often don't define keys.

**Mitigations**

The following parts of the API are now obsolete:
* **`ModelBuilder.Query<>()`** - Instead `ModelBuilder.Entity<>().HasNoKey()` needs to be called to mark an entity type as having no keys.
This would still not be configured by convention to avoid misconfiguration when a primary key is expected, but doesn't match the convention.
* **`DbQuery<>`** - Instead `DbSet<>` should be used.
* **`DbContext.Query<>()`** - Instead `DbContext.Set<>()` should be used.

## Configuration API for owned type relationships has changed

[Tracking Issue #12444](https://github.com/aspnet/EntityFrameworkCore/issues/12444)
[Tracking Issue #9148](https://github.com/aspnet/EntityFrameworkCore/issues/9148)
[Tracking Issue #14153](https://github.com/aspnet/EntityFrameworkCore/issues/14153)

This change is introduced in EF Core 3.0-preview 3.

**Old behavior**

Before EF Core 3.0, configuration of the owned relationship was performed directly after the `OwnsOne` or `OwnsMany` call. 

**New behavior**

Starting with EF Core 3.0, there is now fluent API to configure a navigation property to the owner using `WithOwner()`.
For example:

```C#
modelBuilder.Entity<Order>.OwnsOne(e => e.Details).WithOwner(e => e.Order);
```

The configuration related to the relationship between owner and owned should now be chained after `WithOwner()` similarly to how other relationships are configured.
While the configuration for the owned type itself would still be chained after `OwnsOne()/OwnsMany()`.
For example:

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

Additionally calling `Entity()`, `HasOne()`, or `Set()` with an owned type target will now throw an exception.

**Why**

This change was made to create a cleaner separation between configuring the owned type itself and the _relationship to_ the owned type.
This in turn removes ambiguity and confusion around methods like `HasForeignKey`.

**Mitigations**

Change configuration of owned type relationships to use the new API surface as shown in the example above.

## Dependent entities sharing the table with the principal are now optional

[Tracking Issue #9005](https://github.com/aspnet/EntityFrameworkCore/issues/9005)

This change is introduced in EF Core 3.0-preview 4.

**Old behavior**

Consider the following model:
```C#
public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public OrderDetails Details { get; set; }
}

public class OrderDetails
{
    public int Id { get; set; }
    public string ShippingAddress { get; set; }
}
```
Before EF Core 3.0, if `OrderDetails` is owned by `Order` or explicitly mapped to the same table then an `OrderDetails` instance was always required when adding a new `Order`.


**New behavior**

Starting with 3.0, EF Core allows to add an `Order` without an `OrderDetails` and maps all of the `OrderDetails` properties except the primary key to nullable columns.
When querying EF Core sets `OrderDetails` to `null` if any of its required properties doesn't have a value or if it has no required properties besides the primary key and all properties are `null`.

**Mitigations**

If your model has a table sharing dependent with all optional columns, but the navigation pointing to it is not expected to be `null` then the application should be modified to handle cases when the navigation is `null`. If this is not possible a required property should be added to the entity type or at least one property should have a non-`null` value assigned to it.

## All entities sharing a table with a concurrency token column have to map it to a property

[Tracking Issue #14154](https://github.com/aspnet/EntityFrameworkCore/issues/14154)

This change is introduced in EF Core 3.0-preview 4.

**Old behavior**

Consider the following model:
```C#
public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public byte[] Version { get; set; }
    public OrderDetails Details { get; set; }
}

public class OrderDetails
{
    public int Id { get; set; }
    public string ShippingAddress { get; set; }
}

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Order>()
        .Property(o => o.Version).IsRowVersion().HasColumnName("Version");
}
```
Before EF Core 3.0, if `OrderDetails` is owned by `Order` or explicitly mapped to the same table then updating just `OrderDetails` will not update `Version` value on client and the next update will fail.


**New behavior**

Starting with 3.0, EF Core propagates the new `Version` value to `Order` if it owns `OrderDetails`. Otherwise an exception is thrown during model validation.

**Why**

This change was made to avoid a stale concurrency token value when only one of the entities mapped to the same table is updated.

**Mitigations**

All entities sharing the table have to include a property that is mapped to the concurrency token column. It's possible the create one in shadow-state:
```C#
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<OrderDetails>()
        .Property<byte[]>("Version").IsRowVersion().HasColumnName("Version");
}
```

## Inherited properties from unmapped types are now mapped to a single column for all derived types

[Tracking Issue #13998](https://github.com/aspnet/EntityFrameworkCore/issues/13998)

This change is introduced in EF Core 3.0-preview 4.

**Old behavior**

Consider the following model:
```C#
public abstract class EntityBase
{
    public int Id { get; set; }
}

public abstract class OrderBase : EntityBase
{
    public int ShippingAddress { get; set; }
}

public class BulkOrder : OrderBase
{
}

public class Order : OrderBase
{
}

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Ignore<OrderBase>();
    modelBuilder.Entity<EntityBase>();
    modelBuilder.Entity<BulkOrder>();
    modelBuilder.Entity<Order>();
}
```

Before EF Core 3.0, the `ShippingAddress` property would be mapped to separate columns for `BulkOrder` and `Order` by default.

**New behavior**

Starting with 3.0, EF Core only creates one column for `ShippingAddress`.

**Why**

The old behavoir was unexpected.

**Mitigations**

The property can still be explicitly mapped to separate column on the derived types:

```C#
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Ignore<OrderBase>();
    modelBuilder.Entity<EntityBase>();
    modelBuilder.Entity<BulkOrder>()
        .Property(o => o.ShippingAddress).HasColumnName("BulkShippingAddress");
    modelBuilder.Entity<Order>()
        .Property(o => o.ShippingAddress).HasColumnName("ShippingAddress");
}
```

## The foreign key property convention no longer matches same name as the principal property

[Tracking Issue #13274](https://github.com/aspnet/EntityFrameworkCore/issues/13274)

This change is introduced in EF Core 3.0-preview 3.

**Old behavior**

Consider the following model:
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
Before EF Core 3.0, the `CustomerId` property would be used for the foreign key by convention.
However, if `Order` is an owned type, then this would also make `CustomerId` the primary key and this isn't usually the expectation.

**New behavior**

Starting with 3.0, EF Core doesn't try to use properties for foreign keys by convention if they have the same name as the principal property.
Principal type name concatenated with principal property name, and navigation name concatenated with principal property name patterns are still matched.
For example:

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

**Why**

This change was made to avoid erroneously defining a primary key property on the owned type.

**Mitigations**

If the property was intended to be the foreign key, and hence part of the primary key, then explicitly configure it as such.

## Database connection is now closed if not used anymore before the TransactionScope has been completed

[Tracking Issue #14218](https://github.com/aspnet/EntityFrameworkCore/issues/14218)

This change is introduced in EF Core 3.0-preview 4.

**Old behavior**

Before EF Core 3.0, if the context opens the connection inside a `TransactionScope`, the connection remains open while the current `TransactionScope` is active.

```C#
using (new TransactionScope())
{
    using (AdventureWorks context = new AdventureWorks())
    {
        context.ProductCategories.Add(new ProductCategory());
        context.SaveChanges();

        // Old behavior: Connection is still open at this point
		
        var categories = context.ProductCategories().ToList();
    }
}
```

**New behavior**

Starting with 3.0, EF Core closes the connection as soon as it's done using it.

**Why**

This change allows to use multiple contexts in the same `TransactionScope`. The new behavior also matches EF6.

**Mitigations**

If the connection needs to remain open explicit call to `OpenConnection()` will ensure that EF Core doesn't close it prematurely:

```C#
using (new TransactionScope())
{
    using (AdventureWorks context = new AdventureWorks())
    {
        context.Database.OpenConnection();
        context.ProductCategories.Add(new ProductCategory());
        context.SaveChanges();
		
        var categories = context.ProductCategories().ToList();
        context.Database.CloseConnection();
    }
}
```

## Each property uses independent in-memory integer key generation

[Tracking Issue #6872](https://github.com/aspnet/EntityFrameworkCore/issues/6872)

This change is introduced in EF Core 3.0-preview 4.

**Old behavior**

Before EF Core 3.0, one shared value generator was used for all in-memory integer key properties.

**New behavior**

Starting with EF Core 3.0, each integer key property gets its own value generator when using the in-memory database.
Also, if the database is deleted, then key generation is reset for all tables.

**Why**

This change was made to align in-memory key generation more closely to real database key generation and to improve the ability to isolate tests from each other when using the in-memory database.

**Mitigations**

This can break an application that is relying on specific in-memory key values to be set.
Consider instead not relying on specific key values, or updating to match the new behavior.

## Backing fields are used by default

[Tracking Issue #12430](https://github.com/aspnet/EntityFrameworkCore/issues/12430)

This change is introduced in EF Core 3.0-preview 2.

**Old behavior**

Before 3.0, even if the backing field for a property was known, EF Core would still by default read and write the property value using the property getter and setter methods.
The exception to this was query execution, where the backing field would be set directly if known.

**New behavior**

Starting with EF Core 3.0, if the backing field for a property is known, then EF Core will always read and write that property using the backing field.
This could cause an application break if the application is relying on additional behavior coded into the getter or setter methods.

**Why**

This change was made to prevent EF Core from erroneously triggering business logic by default when performing database operations involving the entities.

**Mitigations**

The pre-3.0 behavior can be restored through configuration of the property access mode on `ModelBuilder`.
For example:

```C#
modelBuilder.UsePropertyAccessMode(PropertyAccessMode.PreferFieldDuringConstruction);
```

## Throw if multiple compatible backing fields are found

[Tracking Issue #12523](https://github.com/aspnet/EntityFrameworkCore/issues/12523)

This change is introduced in EF Core 3.0-preview 4.

**Old behavior**

Before EF Core 3.0, if multiple fields matched the rules for finding the backing field of a property, then one field would be chosen based on some precedence order.
This could cause the wrong field to be used in ambiguous cases.

**New behavior**

Starting with EF Core 3.0, if multiple fields are matched to the same property, then an exception is thrown.

**Why**

This change was made to avoid silently using one field over another when only one can be correct.

**Mitigations**

Properties with ambiguous backing fields must have the field to use specified explicitly.
For example, using the fluent API:

```C#
modelBuilder
    .Entity<Blog>()
    .Property(e => e.Id)
    .HasField("_id");
```

## Field-only property names should match the field name

This change is introduced in EF Core 3.0-preview 4.

**Old behavior**

Before EF Core 3.0, a property could be specified by a string value and if no property with that name was found on the CLR type then EF Core would try to match it to a field using convention rules.
```C#
private class Blog
{
    private int _id;
    public string Name { get; set; }
}
```
```C#
modelBuilder
    .Entity<Blog>()
    .Property("Id");
```

**New behavior**

Starting with EF Core 3.0, a field-only property must match the field name exactly.

```C#
modelBuilder
    .Entity<Blog>()
    .Property("_id");
```

**Why**

This change was made to avoid using the same field for two properties named similarly, it also makes the matching rules for field-only properties the same as for properties mapped to CLR properties.

**Mitigations**

Field-only properties must be named the same as the field they are mapped to.
In a later preview of EF Core 3.0, we plan to re-enable explicitly configuring a field name that is different from the property name:

```C#
modelBuilder
    .Entity<Blog>()
    .Property("Id")
    .HasField("_id");
```

## AddDbContext/AddDbContextPool no longer call AddLogging and AddMemoryCache

[Tracking Issue #14756](https://github.com/aspnet/EntityFrameworkCore/issues/14756)

This change is introduced in EF Core 3.0-preview 4.

**Old behavior**

Before EF Core 3.0, calling `AddDbContext` or `AddDbContextPool` would also register logging and memory caching services with D.I through calls to [AddLogging](https://docs.microsoft.com/dotnet/api/microsoft.extensions.dependencyinjection.loggingservicecollectionextensions.addlogging) and [AddMemoryCache](https://docs.microsoft.com/dotnet/api/microsoft.extensions.dependencyinjection.memorycacheservicecollectionextensions.addmemorycache).

**New behavior**

Starting with EF Core 3.0, `AddDbContext` and `AddDbContextPool` will no longer register these services with Dependency Injection (DI).

**Why**

EF Core 3.0 does not require that these services are in the application's DI container. However, if `ILoggerFactory` is registered in the application's DI container, then it will still be used by EF Core.

**Mitigations**

If your application needs these services, then register them explicitly with the DI container using  [AddLogging](https://docs.microsoft.com/dotnet/api/microsoft.extensions.dependencyinjection.loggingservicecollectionextensions.addlogging) or [AddMemoryCache](https://docs.microsoft.com/dotnet/api/microsoft.extensions.dependencyinjection.memorycacheservicecollectionextensions.addmemorycache).

## DbContext.Entry now performs a local DetectChanges

[Tracking Issue #13552](https://github.com/aspnet/EntityFrameworkCore/issues/13552)

This change is introduced in EF Core 3.0-preview 3.

**Old behavior**

Before EF Core 3.0, calling `DbContext.Entry` would cause changes to be detected for all tracked entities.
This ensured that the state exposed in the `EntityEntry` was up-to-date.

**New behavior**

Starting with EF Core 3.0, calling `DbContext.Entry` will now only attempt to detect changes in the given entity and any tracked principal entities related to it.
This means that changes elsewhere may not have been detected by calling this method, which could have implications on application state.

Note that if `ChangeTracker.AutoDetectChangesEnabled` is set to `false` then even this local change detection will be disabled.

Other methods that cause change detection--for example `ChangeTracker.Entries` and `SaveChanges`--still cause a full `DetectChanges` of all tracked entities.

**Why**

This change was made to improve the default performance of using `context.Entry`.

**Mitigations**

Call `ChgangeTracker.DetectChanges()` explicitly before calling `Entry` to ensure the pre-3.0 behavior.

## String and byte array keys are not client-generated by default

[Tracking Issue #14617](https://github.com/aspnet/EntityFrameworkCore/issues/14617)

This change is introduced in EF Core 3.0-preview 4.

**Old behavior**

Before EF Core 3.0, `string` and `byte[]` key properties could be used without explicitly setting a non-null value.
In such a case, the key value would be generated on the client as a GUID, serialized to bytes for `byte[]`.

**New behavior**

Starting with EF Core 3.0 an exception will be thrown indicating that no key value has been set.

**Why**

This change was made because client-generated `string`/`byte[]` values generally aren't useful, and the default behavior made it hard to reason about generated key values in a common way.

**Mitigations**

The pre-3.0 behavior can be obtained by explicitly specifying that the key properties should use generated values if no other non-null value is set.
For example, with the fluent API:

```C#
modelBuilder
    .Entity<Blog>()
    .Property(e => e.Id)
    .ValueGeneratedOnAdd();
```

Or with data annotations:

```C#
[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
public string Id { get; set; }
```

## ILoggerFactory is now a scoped service

[Tracking Issue #14698](https://github.com/aspnet/EntityFrameworkCore/issues/14698)

This change is introduced in EF Core 3.0-preview 3.

**Old behavior**

Before EF Core 3.0, `ILoggerFactory` was registered as a singleton service.

**New behavior**

Starting with EF Core 3.0, `ILoggerFactory` is now registered as scoped.

**Why**

This change was made to allow association of a logger with a `DbContext` instance, which enables other functionality and removes some cases of pathological behavior such as an explosion of internal service providers.

**Mitigations**

This change should not impact application code unless it is registering and using custom services on the EF Core internal service provider.
This isn't common.
In these cases, most things will still work, but any singleton service that was depending on `ILoggerFactory` will need to be changed to obtain the `ILoggerFactory` in a different way.

If you run into situations like this, please file an issue at on the [EF Core GitHub issue tracker](https://github.com/aspnet/EntityFrameworkCore/issues) to let us know how you are using `ILoggerFactory` such that we can better understand how not to break this again in the future.

## IDbContextOptionsExtensionWithDebugInfo merged into IDbContextOptionsExtension

[Tracking Issue #13552](https://github.com/aspnet/EntityFrameworkCore/issues/13552)

This change is introduced in EF Core 3.0-preview 3.

**Old behavior**

`IDbContextOptionsExtensionWithDebugInfo` was an additional optional interface extended from `IDbContextOptionsExtension` to avoid making a breaking change to the interface during the 2.x release cycle.

**New behavior**

The interfaces are now merged together into `IDbContextOptionsExtension`.

**Why**

This change was made because the interfaces are conceptually one.

**Mitigations**

Any implementations of `IDbContextOptionsExtension` will need to be updated to support the new member.

## Lazy-loading proxies no longer assume navigation properties are fully loaded

[Tracking Issue #12780](https://github.com/aspnet/EntityFrameworkCore/issues/12780)

This change is introduced in EF Core 3.0-preview 4.

**Old behavior**

Before EF Core 3.0, once a `DbContext` was disposed there was no way of knowing if a given navigation property on an entity obtained from that context was fully loaded or not.
Proxies would instead assume that a reference navigation is loaded if it has a non-null value, and that a collection navigation is loaded if it isn't empty.
In these cases, attempting to lazy-load would be a no-op.

**New behavior**

Starting with EF Core 3.0, proxies keep track of whether or not a navigation property is loaded.
This means attempting to access a navigation property that is loaded after the context has been disposed will always be a no-op, even when the loaded navigation is empty or null.
Conversely, attempting to access a navigation property that isn't loaded will throw an exception if the context is disposed even if the navigation property is a non-empty collection.
If this situation arises, it means the application code is attempting to use lazy-loading at an invalid time, and the application should be changed to not do this.

**Why**

This change was made to make the behavior consistent and correct when attempting to lazy-load on a disposed `DbContext` instance.

**Mitigations**

Update application code to not attempt lazy-loading with a disposed context, or configure this to be a no-op as described in the exception message.

## Excessive creation of internal service providers is now an error by default

[Tracking Issue #10236](https://github.com/aspnet/EntityFrameworkCore/issues/10236)

This change is introduced in EF Core 3.0-preview 3.

**Old behavior**

Before EF Core 3.0, a warning would be logged for an application creating a pathological number of internal service providers.

**New behavior**

Starting with EF Core 3.0, this warning is now considered and error and an exception is thrown. 

**Why**

This change was made to drive better application code through exposing this pathological case more explicitly.

**Mitigations**

The most appropriate cause of action on encountering this error is to understand the root cause and stop creating so many internal service providers.
However, the error can be converted back to a warning (or ignored) via configuration on the `DbContextOptionsBuilder`.
For example:

```C#
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder
        .ConfigureWarnings(w => w.Log(CoreEventId.ManyServiceProvidersCreatedWarning));
}
```

## New behavior for HasOne/HasMany called with a single string

[Tracking Issue #9171](https://github.com/aspnet/EntityFrameworkCore/issues/9171)

This change is introduced in EF Core 3.0-preview 4.

**Old behavior**

Before EF Core 3.0, code calling `HasOne` or `HasMany` with a single string was interpreted in a confusing way.
For example:
```C#
modelBuilder.Entity<Samurai>().HasOne("Entrance").WithOne();
```

The code looks like it is relating `Samurai` to some other entity type using the `Entrance` navigation property, which may be private.

In reality, this code attempts to create a relationship to some entity type called `Entrance` with no navigation property.

**New behavior**

Starting with EF Core 3.0, the code above now does what it looked like it should have been doing before.

**Why**

The old behavior was very confusing, especially when reading the configuration code and looking for errors.

**Mitigations**

This will only break applications that are explicitly configuring relationships using strings for type names and without specifying the navigation property explicitly.
This is not common.
The previous behavior can be obtained through explicitly passing `null` for the navigation property name.
For example:

```C#
modelBuilder.Entity<Samurai>().HasOne("Some.Entity.Type.Name", null).WithOne();
```

## The return type for several async methods has been changed from Task to ValueTask

[Tracking Issue #15184](https://github.com/aspnet/EntityFrameworkCore/issues/15184)

This change is introduced in EF Core 3.0-preview 4.

**Old behavior**

The following async methods previously returned a `Task<T>`:

* `DbContext.FindAsync()`
* `DbSet.FindAsync()`
* `DbContext.AddAsync()`
* `DbSet.AddAsync()`
* `ValueGenerator.NextValueAsync()` (and deriving classes)

**New behavior**

The aforementioned methods now return a `ValueTask<T>` over the same `T` as before.

**Why**

This change reduces the number of heap allocations incurred when invoking these methods, improving general performance.

**Mitigations**

Applications simply awaiting the above APIs only need to be recompiled - no source changes are necessary.
A more complex usage (e.g. passing the returned `Task` to `Task.WhenAny()`) typically require that the returned `ValueTask<T>` be converted to a `Task<T>` by calling `AsTask()` on it.
Note that this negates the allocation reduction that this change brings.

## The Relational:TypeMapping annotation is now just TypeMapping

[Tracking Issue #9913](https://github.com/aspnet/EntityFrameworkCore/issues/9913)

This change is introduced in EF Core 3.0-preview 2.

**Old behavior**

The annotation name for type mapping annotations was "Relational:TypeMapping".

**New behavior**

The annotation name for type mapping annotations is now "TypeMapping".

**Why**

Type mappings are now used for more than just relational database providers.

**Mitigations**

This will only break applications that access the type mapping directly as an annotation, which isn't common.
The most appropriate action to fix is to use API surface to access type mappings rather than using the annotation directly.

## ToTable on a derived type throws an exception 

[Tracking Issue #11811](https://github.com/aspnet/EntityFrameworkCore/issues/11811)

This change is introduced in EF Core 3.0-preview 3.

**Old behavior**

Before EF Core 3.0, `ToTable()` called on a derived type would be ignored since only inheritance mapping strategy was TPH where this isn't valid. 

**New behavior**

Starting with EF Core 3.0 and in preparation for adding TPT and TPC support in a later release, `ToTable()` called on a derived type will now throw an exception to avoid an unexpected mapping change in the future.

**Why**

Currently it isn't valid to map a derived type to a different table.
This change avoids breaking in the future when it becomes a valid thing to do.

**Mitigations**

Remove any attempts to map derived types to other tables.

## ForSqlServerHasIndex replaced with HasIndex 

[Tracking Issue #12366](https://github.com/aspnet/EntityFrameworkCore/issues/12366)

This change is introduced in EF Core 3.0-preview 3.

**Old behavior**

Before EF Core 3.0, `ForSqlServerHasIndex().ForSqlServerInclude()` provided a way to configure columns used with `INCLUDE`.

**New behavior**

Starting with EF Core 3.0, using `Include` on an index is now supported at the relational level.
Use `HasIndex().ForSqlServerInclude()`.

**Why**

This change was made to consolidate the API for indexes with `Include` into one place for all database providers.

**Mitigations**

Use the new API, as shown above.

## Metadata API changes

[Tracking Issue #214](https://github.com/aspnet/EntityFrameworkCore/issues/214)

This change is introduced in EF Core 3.0-preview 4.

**New behavior**

The following properties were converted to extension methods:

* `IEntityType.QueryFilter` -> `GetQueryFilter()`
* `IEntityType.DefiningQuery` -> `GetDefiningQuery()`
* `IProperty.IsShadowProperty` -> `IsShadowProperty()`
* `IProperty.BeforeSaveBehavior` -> `GetBeforeSaveBehavior()`
* `IProperty.AfterSaveBehavior` -> `GetAfterSaveBehavior()`

**Why**

This change simplifies the implementation of the aforementioned interfaces.

**Mitigations**

Use the new extension methods.

## Provider-specific Metadata API changes

[Tracking Issue #214](https://github.com/aspnet/EntityFrameworkCore/issues/214)

This change is introduced in EF Core 3.0-preview 6.

**New behavior**

The provider-specific extension methods will be flattened out:

* `IProperty.Relational().ColumnName` -> `IProperty.GetColumnName()`
* `IEntityType.SqlServer().IsMemoryOptimized` -> `IEntityType.GetSqlServerIsMemoryOptimized()`
* `PropertyBuilder.UseSqlServerIdentityColumn()` -> `PropertyBuilder.ForSqlServerUseIdentityColumn()`

**Why**

This change simplifies the implementation of the aforementioned extension methods.

**Mitigations**

Use the new extension methods.

## EF Core no longer sends pragma for SQLite FK enforcement

[Tracking Issue #12151](https://github.com/aspnet/EntityFrameworkCore/issues/12151)

This change is introduced in EF Core 3.0-preview 3.

**Old behavior**

Before EF Core 3.0, EF Core would send `PRAGMA foreign_keys = 1` when a connection to SQLite is opened.

**New behavior**

Starting with EF Core 3.0, EF Core no longer sends `PRAGMA foreign_keys = 1` when a connection to SQLite is opened.

**Why**

This change was made because EF Core uses `SQLitePCLRaw.bundle_e_sqlite3` by default, which in turn means that FK enforcement is switched on by default and doesn't need to be explicitly enabled each time a connection is opened.

**Mitigations**

Foreign keys are enabled by default in SQLitePCLRaw.bundle_e_sqlite3, which is used by default for EF Core.
For other cases, foreign keys can be enabled by specifying `Foreign Keys=True` in your connection string.

## Microsoft.EntityFrameworkCore.Sqlite now depends on SQLitePCLRaw.bundle_e_sqlite3

**Old behavior**

Before EF Core 3.0, EF Core used `SQLitePCLRaw.bundle_green`.

**New behavior**

Starting with EF Core 3.0, EF Core uses `SQLitePCLRaw.bundle_e_sqlite3`.

**Why**

This change was made so that the version of SQLite used on iOS consistent with other platforms.

**Mitigations**

To use the native SQLite version on iOS, configure `Microsoft.Data.Sqlite` to use a different `SQLitePCLRaw` bundle.

## Guid values are now stored as TEXT on SQLite

[Tracking Issue #15078](https://github.com/aspnet/EntityFrameworkCore/issues/15078)

This change is introduced in EF Core 3.0-preview 4.

**Old behavior**

Guid values were previously sored as BLOB values on SQLite.

**New behavior**

Guid values are now stored as TEXT.

**Why**

The binary format of Guids is not standardized. Storing the values as TEXT makes the database more compatible with other technologies.

**Mitigations**

You can migrate existing databases to the new format by executing SQL like the following.

``` sql
UPDATE MyTable
SET GuidColumn = hex(substr(GuidColumn, 4, 1)) ||
                 hex(substr(GuidColumn, 3, 1)) ||
                 hex(substr(GuidColumn, 2, 1)) ||
                 hex(substr(GuidColumn, 1, 1)) || '-' ||
                 hex(substr(GuidColumn, 6, 1)) ||
                 hex(substr(GuidColumn, 5, 1)) || '-' ||
                 hex(substr(GuidColumn, 8, 1)) ||
                 hex(substr(GuidColumn, 7, 1)) || '-' ||
                 hex(substr(GuidColumn, 9, 2)) || '-' ||
                 hex(substr(GuidColumn, 11, 6))
WHERE typeof(GuidColumn) == 'blob';
```

In EF Core, you could also continue using the previous behavior by configuring a value converter on these properties.

``` csharp
modelBuilder
    .Entity<MyEntity>()
    .Property(e => e.GuidProperty)
    .HasConversion(
        g => g.ToByteArray(),
        b => new Guid(b));
```

Microsoft.Data.Sqlite remains capable of reading Guid values from both BLOB and TEXT columns; however, since the default format for parameters and constants has changed you'll likely need to take action for most scenarios involving Guids.

## Char values are now stored as TEXT on SQLite

[Tracking Issue #15020](https://github.com/aspnet/EntityFrameworkCore/issues/15020)

This change is introduced in EF Core 3.0-preview 4.

**Old behavior**

Char values were previously sored as INTEGER values on SQLite. For example, a char value of *A* was stored as the integer value 65.

**New behavior**

Char values are now stored as TEXT.

**Why**

Storing the values as TEXT is more natural and makes the database more compatible with other technologies.

**Mitigations**

You can migrate existing databases to the new format by executing SQL like the following.

``` sql
UPDATE MyTable
SET CharColumn = char(CharColumn)
WHERE typeof(CharColumn) = 'integer';
```

In EF Core, you could also continue using the previous behavior by configuring a value converter on these properties.

``` csharp
modelBuilder
    .Entity<MyEntity>()
    .Property(e => e.CharProperty)
    .HasConversion(
        c => (long)c,
        i => (char)i);
```

Microsoft.Data.Sqlite also remains capable of reading character values from both INTEGER and TEXT columns, so certain scenarios may not require any action.

## Migration IDs are now generated using the invariant culture's calendar

[Tracking Issue #12978](https://github.com/aspnet/EntityFrameworkCore/issues/12978)

This change is introduced in EF Core 3.0-preview 4.

**Old behavior**

Migration IDs were inadvertently generated using the current culture's calendar.

**New behavior**

Migration IDs are now always generated using the invariant culture's calendar (Gregorian).

**Why**

The order of migrations is important when updating the database or resolving merge conflicts. Using the invariant calendar avoids ordering issues that can result from team members having different system calendars.

**Mitigations**

This change affects anyone using a non-Gregorian calendar where the year is greater than the Gregorian calendar (like the Thai Buddhist calendar). Existing migration IDs will need to be updated so that new migrations are ordered after existing migrations.

The migration ID can be found in the Migration attribute in the migrations' designer files.

``` diff
 [DbContext(typeof(MyDbContext))]
-[Migration("25620318122820_MyMigration")]
+[Migration("20190318122820_MyMigration")]
 partial class MyMigration
 {
```

The Migrations history table also needs to be updated.

``` sql
UPDATE __EFMigrationsHistory
SET MigrationId = CONCAT(LEFT(MigrationId, 4)  - 543, SUBSTRING(MigrationId, 4, 150))
```

## LogQueryPossibleExceptionWithAggregateOperator has been renamed

[Tracking Issue #10985](https://github.com/aspnet/EntityFrameworkCore/issues/10985)

This change is introduced in EF Core 3.0-preview 4.

**Change**

`RelationalEventId.LogQueryPossibleExceptionWithAggregateOperator` has been renamed to `RelationalEventId.LogQueryPossibleExceptionWithAggregateOperatorWarning`.

**Why**

Aligns the naming of this warning event with all other warning events.

**Mitigations**

Use the new name. (Note that the event ID number has not changed.)

## Clarify API for foreign key constraint names

[Tracking Issue #10730](https://github.com/aspnet/EntityFrameworkCore/issues/10730)

This change is introduced in EF Core 3.0-preview 4.

**Old behavior**

Before EF Core 3.0, foreign key constraint names were referred to as simply the "name". For example:

```C#
var constraintName = myForeignKey.Name;
```

**New behavior**

Starting with EF Core 3.0, foreign key constraint names are now referred to as the "constraint name". For example:

```C#
var constraintName = myForeignKey.ConstraintName;
```

**Why**

This change brings consistency to naming in this area, and also clarifies that this is the name of the foreign key constraint, and not the column or property name that the foreign key is defined on.

**Mitigations**

Use the new name.
