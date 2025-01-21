---
title: Breaking changes in EF Core 7.0 (EF7) - EF Core
description: Complete list of breaking changes introduced in Entity Framework Core 7.0 (EF7)
author: SamMonoRT
ms.date: 09/20/2023
uid: core/what-is-new/ef-core-7.0/breaking-changes
---

# Breaking changes in EF Core 7.0 (EF7)

This page documents API and behavior changes that have the potential to break existing applications updating from EF Core 6 to EF Core 7. Make sure to review earlier breaking changes if updating from an earlier version of EF Core:

- [Breaking changes in EF Core 6](xref:core/what-is-new/ef-core-6.0/breaking-changes)

## Target Framework

EF Core 7.0 targets .NET 6. This means that existing applications that target .NET 6 can continue to do so. Applications targeting older .NET, .NET Core, and .NET Framework versions will need to target .NET 6 or .NET 7 to use EF Core 7.0.

## Summary

| **Breaking change**                                                                                                                      | **Impact** |
|:-----------------------------------------------------------------------------------------------------------------------------------------|------------|
| [`Encrypt` defaults to `true` for SQL Server connections](#encrypt-true)                                                                 | High       |
| [Some warnings will again throw exceptions by default](#warnings-as-errors)                                                              | High       |
| [SQL Server tables with triggers or certain computed columns now require special EF Core configuration](#sqlserver-tables-with-triggers) | High       |
| [SQLite tables with AFTER triggers and virtual tables now require special EF Core configuration](#sqlitetriggers)                        | High       |
| [Orphaned dependents of optional relationships are not automatically deleted](#optional-deletes)                                         | Medium     |
| [Cascade delete is configured between tables when using TPT mapping with SQL Server](#tpt-cascade-delete)                                | Medium     |
| [Higher chance of busy/locked errors on SQLite when not using write-ahead logging](#sqliteretries)                                       | Medium     |
| [Key properties may need to be configured with a provider value comparer](#provider-value-comparer)                                      | Low        |
| [Check constraints and other table facets are now configured on the table](#table-configuration)                                         | Low        |
| [Navigations from new entities to deleted entities are not fixed up](#deleted-fixup)                                                     | Low        |
| [Using `FromSqlRaw` and related methods from the wrong provider throws](#use-the-correct-method)                                         | Low        |
| [Scaffolded `OnConfiguring` no longer calls `IsConfigured`](#is-configured)                                                              | Low        |

## High-impact changes

<a name="encrypt-true"></a>

### `Encrypt` defaults to `true` for SQL Server connections

[Tracking Issue: SqlClient #1210](https://github.com/dotnet/SqlClient/pull/1210)

> [!IMPORTANT]
> This is a severe breaking change in the [Microsoft.Data.SqlClient](https://www.nuget.org/packages/Microsoft.Data.SqlClient/) package. **There is nothing that can be done in EF Core to revert or mitigate this change.** Please direct feedback to the [Microsoft.Data.SqlClient GitHub Repo](https://github.com/dotnet/SqlClient) or contact a [Microsoft Support Professional](https://support.serviceshub.microsoft.com/supportforbusiness/onboarding?origin=/supportforbusiness/create) for additional questions or help.

#### Old behavior

[SqlClient connection strings](/sql/connect/ado-net/connection-string-syntax) use `Encrypt=False` by default. This allows connections on development machines where the local server does not have a valid certificate.

#### New behavior

[SqlClient connection strings](/sql/connect/ado-net/connection-string-syntax) use `Encrypt=True` by default. This means that:

- The server must be configured with a valid certificate
- The client must trust this certificate

If these conditions are not met, then a `SqlException` will be thrown. For example:

> A connection was successfully established with the server, but then an error occurred during the login process. (provider: SSL Provider, error: 0 - The certificate chain was issued by an authority that is not trusted.)

#### Why

This change was made to ensure that, by default, either the connection is secure or the application will fail to connect.

#### Mitigations

There are three ways to proceed:

1. [Install a valid certificate on the server](/sql/database-engine/configure-windows/enable-encrypted-connections-to-the-database-engine). Note that this is an involved process and requires obtaining a certificate and ensuring it is signed by an authority trusted by the client.
2. If the server has a certificate, but it is not trusted by the client, then `TrustServerCertificate=True` to allow bypassing the normal trust mechanism.
3. Explicitly add `Encrypt=False` to the connection string.

> [!WARNING]
> Options 2 and 3 both leave the server in a potentially insecure state.

<a name="warnings-as-errors"></a>

### Some warnings throw exceptions by default again

[Tracking Issue #29069](https://github.com/dotnet/efcore/issues/29069)

#### Old behavior

In EF Core 6.0, a bug in the SQL Server provider meant that some warnings that are configured to throw exceptions by default were instead being logged but not throwing exceptions. These warnings are:

| EventId                                                                                                                                      | Description                                                                                                     |
|----------------------------------------------------------------------------------------------------------------------------------------------|-----------------------------------------------------------------------------------------------------------------|
| <xref:Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.AmbientTransactionWarning?displayProperty=nameWithType>                    | An application may have expected an ambient transaction to be used when it was actually ignored.                |
| <xref:Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.IndexPropertiesBothMappedAndNotMappedToTable?displayProperty=nameWithType> | An index specifies properties some of which are mapped and some of which are not mapped to a column in a table. |
| <xref:Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.IndexPropertiesMappedToNonOverlappingTables?displayProperty=nameWithType>  | An index specifies properties which map to columns on non-overlapping tables.                                   |
| <xref:Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.ForeignKeyPropertiesMappedToUnrelatedTables?displayProperty=nameWithType>  | A foreign key specifies properties which don't map to the related tables.                                       |

#### New behavior

Starting with EF Core 7.0, these warnings again, by default, result in an exception being thrown.

#### Why

These are issues that very likely indicate an error in the application code that should be fixed.

#### Mitigations

Fix the underlying issue that is the reason for the warning.

Alternately, the warning level can be changed so that it is [logged only or suppressed entirely](xref:core/logging-events-diagnostics/extensions-logging#configuration-for-specific-messages). For example:

```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder
        .ConfigureWarnings(b => b.Ignore(RelationalEventId.AmbientTransactionWarning));
```

<a name="sqlserver-tables-with-triggers"></a>

### SQL Server tables with triggers or certain computed columns now require special EF Core configuration

[Tracking Issue #27372](https://github.com/dotnet/efcore/issues/27372)

#### Old behavior

Previous versions of the SQL Server provider saved changes via a less efficient technique which always worked.

#### New behavior

By default, EF Core now saves changes via a significantly more efficient technique; unfortunately, this technique is not supported on SQL Server if the target table has database triggers, or certain types of computed columns. See the [SQL Server documentation](/sql/t-sql/queries/output-clause-transact-sql#remarks) for more details.

#### Why

The performance improvements linked to the new method are significant enough that it's important to bring them to users by default. At the same time, we estimate usage of database triggers or the affected computed columns in EF Core applications to be low enough that the negative breaking change consequences are outweighed by the performance gain.

#### Mitigations

Starting with EF Core 8.0, the use or not of the "OUTPUT" clause can be configured explicitly. For example:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Blog>()
        .ToTable(tb => tb.UseSqlOutputClause(false));
}
```

In EF7 or later, If the target table has a trigger, then you can let EF Core know this, and EF will revert to the previous, less efficient technique. This can be done by configuring the corresponding entity type as follows:

[!code-csharp[Main](../../../../samples/core/SqlServer/Misc/TriggersContext.cs?name=TriggerConfiguration&highlight=4)]

Note that doing this doesn't actually make EF Core create or manage the trigger in any way - it currently only informs EF Core that triggers are present on the table. As a result, any trigger name can be used. Specifying a trigger can be used to revert the old behavior _even if there isn't actually a trigger in the table_.

If most or all of your tables have triggers, you can opt out of using the newer, efficient technique for all your model's tables by using the following model building convention:

[!code-csharp[Main](../../../../samples/core/SqlServer/Misc/TriggersContext.cs?name=BlankTriggerAddingConvention)]

Use the convention on your `DbContext` by overriding `ConfigureConventions`:

[!code-csharp[Main](../../../../samples/core/SqlServer/Misc/TriggersContext.cs?name=ConfigureConventions)]

This effectively calls `HasTrigger` on all your model's tables, instead of you having to do it manually for each and every table.

<a name="sqlitetriggers"></a>

### SQLite tables with AFTER triggers and virtual tables now require special EF Core configuration

[Tracking Issue #29916](https://github.com/dotnet/efcore/issues/29916)

#### Old behavior

Previous versions of the SQLite provider saved changes via a less efficient technique which always worked.

#### New behavior

By default, EF Core now saves changes via a more efficient technique, using the RETURNING clause. Unfortunately, this technique is not supported on SQLite if target table is has database AFTER triggers, is virtual, or if older versions of SQLite are being used. See the [SQLite documentation](https://www.sqlite.org/lang_returning.html) for more details.

#### Why

The simplifications and performance improvements linked to the new method are significant enough that it's important to bring them to users by default. At the same time, we estimate usage of database triggers and virtual tables in EF Core applications to be low enough that the negative breaking change consequences are outweighed by the performance gain.

#### Mitigations

In EF Core 8.0, the `UseSqlReturningClause` method has been introduced to explicitly revert back to the older, less efficient SQL. For example:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Blog>()
        .ToTable(tb => tb.UseSqlReturningClause(false));
}
```

If you are still using EF Core 7.0, then it's possible to revert to the old mechanism for the entire application by inserting the following code in your context configuration:

```c#
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder
        .UseSqlite(...)
        .ReplaceService<IUpdateSqlGenerator, SqliteLegacyUpdateSqlGenerator>();
```

## Medium-impact changes

<a name="optional-deletes"></a>

### Orphaned dependents of optional relationships are not automatically deleted

[Tracking Issue #27217](https://github.com/dotnet/efcore/issues/27217)

#### Old behavior

A [relationship is optional](xref:core/modeling/relationships) if its foreign key is nullable. Setting the foreign key to null allows the dependent entity exist without any related principal entity. Optional relationships can be configured to use [cascade deletes](xref:core/saving/cascade-delete), although this is not the default.

An optional dependent can be severed from its principal by either setting its foreign key to null, or clearing the navigation to or from it. In EF Core 6.0, this would cause the dependent to be deleted when the relationship was configured for cascade delete.

#### New behavior

Starting with EF Core 7.0, the dependent is no longer deleted. Note that if the principal is deleted, then the dependent will still be deleted since cascade deletes are configured for the relationship.

#### Why

The dependent can exist without any relationship to a principal, so severing the relationship should not cause the entity to be deleted.

#### Mitigations

The dependent can be explicitly deleted:

```csharp
context.Remove(blog);
```

Or `SaveChanges` can be overridden or intercepted to delete dependents with no principal reference. For example:

```csharp
context.SavingChanges += (c, _) =>
    {
        foreach (var entry in ((DbContext)c!).ChangeTracker
            .Entries<Blog>()
            .Where(e => e.State == EntityState.Modified))
        {
            if (entry.Reference(e => e.Author).CurrentValue == null)
            {
                entry.State = EntityState.Deleted;
            }
        }
    };
```

<a name="tpt-cascade-delete"></a>

### Cascade delete is configured between tables when using TPT mapping with SQL Server

[Tracking Issue #28532](https://github.com/dotnet/efcore/issues/28532)

#### Old behavior

When [mapping an inheritance hierarchy using the TPT strategy](xref:core/modeling/inheritance), the base table must contain a row for every entity saved, regardless of the actual type of that entity. Deleting the row in the base table should delete rows in all the other tables. EF Core configures a [cascade deletes](xref:core/saving/cascade-delete) for this.

In EF Core 6.0, a bug in the SQL Server database provider meant that these cascade deletes were not being created.

#### New behavior

Starting with EF Core 7.0, the cascade deletes are now being created for SQL Server just as they always were for other databases.

#### Why

Cascade deletes from the base table to the sub-tables in TPT allow an entity to be deleted by deleting its row in the base table.

#### Mitigations

In most cases, this change should not cause any issues. However, SQL Server is very restrictive when there are multiple cascade behaviors configured between tables. This means that if there is an existing cascading relationship between tables in the TPT mapping, then SQL Server may generate the following error:

> Microsoft.Data.SqlClient.SqlException: The DELETE statement conflicted with the REFERENCE constraint "FK_Blogs_People_OwnerId". The conflict occurred in database "Scratch", table "dbo.Blogs", column 'OwnerId'. The statement has been terminated.

For example, this model creates a cycle of cascading relationships:

```csharp
[Table("FeaturedPosts")]
public class FeaturedPost : Post
{
    public int ReferencePostId { get; set; }
    public Post ReferencePost { get; set; } = null!;
}

[Table("Posts")]
public class Post
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
}
```

One of these will need to be configured to not use cascade deletes on the server. For example, to change the explicit relationship:

```csharp
modelBuilder
    .Entity<FeaturedPost>()
    .HasOne(e => e.ReferencePost)
    .WithMany()
    .OnDelete(DeleteBehavior.ClientCascade);
```

Or to change the implicit relationship created for the TPT mapping:

```csharp
modelBuilder
    .Entity<FeaturedPost>()
    .HasOne<Post>()
    .WithOne()
    .HasForeignKey<FeaturedPost>(e => e.Id)
    .OnDelete(DeleteBehavior.ClientCascade);
```

<a name="sqliteretries"></a>

### Higher chance of busy/locked errors on SQLite when not using write-ahead logging

#### Old behavior

Previous versions of the SQLite provider saved changes via a less efficient technique which was able to automatically retry when the table was locked/busy and write-ahead logging (WAL) was not enabled.

#### New behavior

By default, EF Core now saves changes via a more efficient technique, using the RETURNING clause. Unfortunately, this technique is not able to automatically retry when busy/locked. In a multi-threaded application (like a web app) not using write-ahead logging, it is common to encounter these errors.

#### Why

The simplifications and performance improvements linked to the new method are significant enough that it's important to bring them to users by default. Databases created by EF Core also enable write-ahead logging by default. The SQLite team also recommends enabling write-ahead logging by default.

#### Mitigations

If possible, you should enable write-ahead logging on your database. If your database was created by EF, this should already be the case. If not, you can enable write-ahead logging by executing the following command.

```sql
PRAGMA journal_mode = 'wal';
```

If, for some reason, you can't enable write-ahead logging, it's possible to revert to the old mechanism for the entire application by inserting the following code in your context configuration:

##### [EF Core 7.0](#tab/v7)

```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder
        .UseSqlite(...)
        .ReplaceService<IUpdateSqlGenerator, SqliteLegacyUpdateSqlGenerator>();
```

##### [EF Core 8.0 and above](#tab/v8)

```csharp
protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
{
    configurationBuilder.Conventions.Add(_ => new DoNotUseReturningClauseConvention());
}
```

```csharp
class DoNotUseReturningClauseConvention : IModelFinalizingConvention
{
    public void ProcessModelFinalizing(
        IConventionModelBuilder modelBuilder,
        IConventionContext<IConventionModelBuilder> context)
    {
        foreach (var entityType in modelBuilder.Metadata.GetEntityTypes())
        {
            entityType.UseSqlReturningClause(false);
        }
    }
}
```

---

## Low-impact changes

<a name="provider-value-comparer"></a>

### Key properties may need to be configured with a provider value comparer

[Tracking Issue #27738](https://github.com/dotnet/efcore/issues/27738)

#### Old behavior

In EF Core 6.0, key values taken directly from the properties of entity types were used for comparison of key values when saving changes. This would make use of any [custom value comparer](xref:core/modeling/value-comparers) configured on these properties.

#### New behavior

Starting with EF Core 7.0, database values are used for these comparisons. This "just works" for the vast majority of cases. However, if the properties were using a custom comparer, and that comparer cannot be applied to the database values, then a "provider value comparer" may be needed, as shown below.

#### Why

Various entity-splitting and table-splitting can result in multiple properties mapped to the same database column, and vice-versa. This requires values to be compared after conversion to value that will be used in the database.

#### Mitigations

Configure a provider value comparer. For example, consider the case where a value object is being used as a key, and the comparer for that key uses case-insensitive string comparisons:

```csharp
var blogKeyComparer = new ValueComparer<BlogKey>(
    (l, r) => string.Equals(l.Id, r.Id, StringComparison.OrdinalIgnoreCase),
    v => v.Id.ToUpper().GetHashCode(),
    v => v);

var blogKeyConverter = new ValueConverter<BlogKey, string>(
    v => v.Id,
    v => new BlogKey(v));

modelBuilder.Entity<Blog>()
    .Property(e => e.Id).HasConversion(
        blogKeyConverter, blogKeyComparer);
```

The database values (strings) cannot directly use the comparer defined for `BlogKey` types. Therefore, a provider comparer for case-insensitive string comparisons must be configured:

```csharp
var caseInsensitiveComparer = new ValueComparer<string>(
    (l, r) => string.Equals(l, r, StringComparison.OrdinalIgnoreCase),
    v => v.ToUpper().GetHashCode(),
    v => v);

var blogKeyComparer = new ValueComparer<BlogKey>(
    (l, r) => string.Equals(l.Id, r.Id, StringComparison.OrdinalIgnoreCase),
    v => v.Id.ToUpper().GetHashCode(),
    v => v);

var blogKeyConverter = new ValueConverter<BlogKey, string>(
    v => v.Id,
    v => new BlogKey(v));

modelBuilder.Entity<Blog>()
    .Property(e => e.Id).HasConversion(
        blogKeyConverter, blogKeyComparer, caseInsensitiveComparer);
```

<a name="table-configuration"></a>

### Check constraints and other table facets are now configured on the table

[Tracking Issue #28205](https://github.com/dotnet/efcore/issues/28205)

#### Old behavior

In EF Core 6.0, `HasCheckConstraint`, `HasComment`, and `IsMemoryOptimized` were called directly on the entity type builder. For example:

```csharp
modelBuilder
    .Entity<Blog>()
    .HasCheckConstraint("CK_Blog_TooFewBits", "Id > 1023");

modelBuilder
    .Entity<Blog>()
    .HasComment("It's my table, and I'll delete it if I want to.");

modelBuilder
    .Entity<Blog>()
    .IsMemoryOptimized();
```

#### New behavior

Starting with EF Core 7.0, these methods are instead called on the table builder:

```csharp
modelBuilder
    .Entity<Blog>()
    .ToTable(b => b.HasCheckConstraint("CK_Blog_TooFewBits", "Id > 1023"));

modelBuilder
    .Entity<Blog>()
    .ToTable(b => b.HasComment("It's my table, and I'll delete it if I want to."));

modelBuilder
    .Entity<Blog>()
    .ToTable(b => b.IsMemoryOptimized());
```

The existing methods have been marked as `Obsolete`. They currently have the same behavior as the new methods, but will be removed in a future release.

#### Why

These facets apply to tables only. They will not be applied to any mapped views, functions, or stored procedures.

#### Mitigations

Use the table builder methods, as shown above.

<a name="deleted-fixup"></a>

### Navigations from new entities to deleted entities are not fixed up

[Tracking Issue #28249](https://github.com/dotnet/efcore/issues/28249)

#### Old behavior

In EF Core 6.0, when a new entity is tracked either from a [tracking query](xref:core/querying/tracking) or by [attaching it](xref:core/change-tracking/explicit-tracking) to the `DbContext`, then navigations to and from related entities in the [`Deleted` state](xref:core/change-tracking/index#entity-states) are [fixed up](xref:core/change-tracking/relationship-changes#relationship-fixup).

#### New behavior

Starting with EF Core 7.0, navigations to and from `Deleted` entities are not fixed up.

#### Why

Once an entity is marked as `Deleted` it rarely makes sense to associate it with non-deleted entities.

#### Mitigations

Query or attach entities before marking entities as `Deleted`, or manually set navigation properties to and from the deleted entity.

<a name="use-the-correct-method"></a>

### Using `FromSqlRaw` and related methods from the wrong provider throws use-the-correct-method

[Tracking Issue #26502](https://github.com/dotnet/efcore/issues/26502)

#### Old behavior

In EF Core 6.0, using the Azure Cosmos DB <xref:Microsoft.EntityFrameworkCore.CosmosQueryableExtensions.FromSqlRaw*> extension method when using a relational provider, or the relational <xref:Microsoft.EntityFrameworkCore.RelationalQueryableExtensions.FromSqlRaw*> extension method when using the Azure Cosmos DB provider could silently fail. Likewise, using relational methods on the in-memory provider is a silent no-op.

#### New behavior

Starting with EF Core 7.0, using an extension method designed for one provider on a different provider will throw an exception.

#### Why

The correct extension method must be used for it to function correctly in all situations.

#### Mitigations

Use the correct extension method for the provider being used. If multiple providers are referenced, then call the extension method as a static method. For example:

```csharp
var result = await CosmosQueryableExtensions.FromSqlRaw(context.Blogs, "SELECT ...").ToListAsync();
```

Or:

```csharp
var result = await RelationalQueryableExtensions.FromSqlRaw(context.Blogs, "SELECT ...").ToListAsync();
```

<a name="is-configured"></a>

### Scaffolded `OnConfiguring` no longer calls `IsConfigured`

[Tracking Issue #4274](https://github.com/dotnet/EntityFramework.Docs/issues/4274)

#### Old behavior

In EF Core 6.0, the `DbContext` type scaffolded from an existing database contained a call to `IsConfigured`. For example:

```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    if (!optionsBuilder.IsConfigured)
    {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        optionsBuilder.UseNpgsql("MySecretConnectionString");
    }
}
```

#### New behavior

Starting with EF Core 7.0, the call to `IsConfigured` is no longer included.

#### Why

There are very limited scenarios where the database provider is configured inside your DbContext in some cases, but only if the context is not configured already. Instead, leaving `OnConfiguring` here makes it more likely that a connection string containing sensitive information is left in the code, despite the compile-time warning. Thus the extra safely and cleaner code from removing this was deemed worthwhile, especially given that the `--no-onconfiguring` (.NET CLI) or `-NoOnConfiguring` (Visual Studio Package Manager Console) flag can be used to prevent scaffolding of the `OnConfiguring` method, and that customizable templates exist to add back `IsConfigured` if it is really needed.

#### Mitigations

Either:

- Use the `--no-onconfiguring` (.NET CLI) or `-NoOnConfiguring` (Visual Studio Package Manager Console) argument when scaffolding from an existing database.
- [Customize the T4 templates](xref:core/managing-schemas/scaffolding/templates) to add back the call to `IsConfigured`.
