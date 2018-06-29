---
title: "Customizing the migrations history table - EF6"
author: divega
ms.date: "2016-10-23"
ms.prod: "entity-framework"
ms.author: divega
ms.manager: avickers
ms.technology: entity-framework-6
ms.topic: "article"
ms.assetid: ed5518f0-a9a6-454e-9e98-a4fa7748c8d0
caps.latest.revision: 3
---
# Customizing the migrations history table
> **EF6 Onwards Only** - The features, APIs, etc. discussed in this page were introduced in Entity Framework 6. If you are using an earlier version, some or all of the information does not apply.

> [!NOTE]
> This article assumes you know how to use Code First Migrations in basic scenarios. If you don’t, then you’ll need to read [Code First Migrations](~/ef6/modeling/code-first/migrations/index.md) before continuing.

## What is Migrations History Table?

Migrations history table is a table used by Code First Migrations to store details about migrations applied to the database. By default the name of the table in the database is \_\_MigrationHistory and it is created when applying the first migration do the database. In Entity Framework 5 this table was a system table if the application used Microsoft Sql Server database. This has changed in Entity Framework 6 however and the migrations history table is no longer marked a system table.

## Why customize Migrations History Table?

Migrations history table is supposed to be used solely by Code First Migrations and changing it manually can break migrations. However sometimes the default configuration is not suitable and the table needs to be customized, for instance:

-   You need to change names and/or facets of the columns to enable a 3<sup>rd</sup> party Migrations provider
-   You want to change the name of the table
-   You need to use a non-default schema for the \_\_MigrationHistory table
-   You need to store additional data for a given version of the context and therefore you need to add an additional column to the table

## Words of precaution

Changing the migration history table is powerful but you need to be careful to not overdo it. EF runtime currently does not check whether the customized migrations history table is compatible with the runtime. If it is not your application may break at runtime or behave in unpredictable ways. This is even more important if you use multiple contexts per database in which case multiple contexts can use the same migration history table to store information about migrations.

## How to customize Migrations History Table?

Before you start you need to know that you can customize the migrations history table only before you apply the first migration. Now, to the code.

First, you will need to create a class derived from System.Data.Entity.Migrations.History.HistoryContext class. The HistoryContext class is derived from the DbContext class so configuring the migrations history table is very similar to configuring EF models with fluent API. You just need to override the OnModelCreating method and use fluent API to configure the table.

>[!NOTE]
> Typically when you configure EF models you don’t need to call base.OnModelCreating() from the overridden OnModelCreating method since the DbContext.OnModelCreating() has empty body. This is not the case when configuring the migrations history table. In this case the first thing to do in your OnModelCreating() override is to actually call base.OnModelCreating(). This will configure the migrations history table in the default way which you then tweak in the overriding method.

Let’s say you want to rename the migrations history table and put it to a custom schema called “admin”. In addition your DBA would like you to rename the MigrationId column to Migration\_ID.  You could achieve this by creating the following class derived from HistoryContext:

``` csharp
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.Entity.Migrations.History;

    namespace CustomizableMigrationsHistoryTableSample
    {
        public class MyHistoryContext : HistoryContext
        {
            public MyHistoryContext(DbConnection dbConnection, string defaultSchema)
                : base(dbConnection, defaultSchema)
            {
            }

            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);
                modelBuilder.Entity<HistoryRow>().ToTable(tableName: "MigrationHistory", schemaName: "admin");
                modelBuilder.Entity<HistoryRow>().Property(p => p.MigrationId).HasColumnName("Migration_ID");
            }
        }
    }
```

Once your custom HistoryContext is ready you need to make EF aware of it by registering it via [code-based configuration](http://msdn.com/data/jj680699):

``` csharp
    using System.Data.Entity;

    namespace CustomizableMigrationsHistoryTableSample
    {
        public class ModelConfiguration : DbConfiguration
        {
            public ModelConfiguration()
            {
                this.SetHistoryContext("System.Data.SqlClient",
                    (connection, defaultSchema) => new MyHistoryContext(connection, defaultSchema));
            }
        }
    }
```

That’s pretty much it. Now you can go to the Package Manager Console, Enable-Migrations, Add-Migration and finally Update-Database. This should result in adding to the database a migrations history table configured according to the details you specified in your HistoryContext derived class.

![Database](~/ef6/media/database.png)
