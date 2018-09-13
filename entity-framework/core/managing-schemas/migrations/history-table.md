---
title: Custom Migrations History Table - EF Core
author: bricelam
ms.author: bricelam
ms.date: 11/07/2017
---
Custom Migrations History Table
===============================
By default, EF Core keeps track of which migrations have been applied to the database by recording them in a table named
`__EFMigrationsHistory`. For various reasons, you may want to customize this table to better suit your needs.

> [!IMPORTANT]
> If you customize the Migrations history table *after* applying migrations, you are responsible for updating the
> existing table in the database.

Schema and table name
----------------------
You can change the schema and table name using the `MigrationsHistoryTable()` method in `OnConfiguring()` (or
`ConfigureServices()` on ASP.NET Core). Here is an example using the SQL Server EF Core provider.

``` csharp
protected override void OnConfiguring(DbContextOptionsBuilder options)
    => options.UseSqlServer(
        connectionString,
        x => x.MigrationsHistoryTable("__MyMigrationsHistory", "mySchema"));
```

Other changes
-------------
To configure additional aspects of the table, override and replace the provider-specific
`IHistoryRepository` service. Here is an example of changing the MigrationId column name to *Id* on SQL Server.

``` csharp
protected override void OnConfiguring(DbContextOptionsBuilder options)
    => options
        .UseSqlServer(connectionString)
        .ReplaceService<IHistoryRepository, MyHistoryRepository>();
```

> [!WARNING]
> `SqlServerHistoryRepository` is inside an internal namespace and may change in future releases.

``` csharp
class MyHistoryRepository : SqlServerHistoryRepository
{
    public MyHistoryRepository(HistoryRepositoryDependencies dependencies)
        : base(dependencies)
    {
    }

    protected override void ConfigureTable(EntityTypeBuilder<HistoryRow> history)
    {
        base.ConfigureTable(history);

        history.Property(h => h.MigrationId).HasColumnName("Id");
    }
}
```
