---
title: Migrations Overview - EF Core
author: bricelam
ms.author: bricelam
ms.date: 05/06/2020
uid: core/managing-schemas/migrations/index
---
# Migrations Overview

In real world projects, data models change as features get implemented: new entities or properties are added and removed, and database schemas needs to be changed accordingly to be kept in sync with the application. The migrations feature in EF Core provides a way to incrementally update the database schema to keep it in sync with the application's data model while preserving existing data in the database.

At a high level, migrations function in the following way:

* When a data model change is introduced, the developer uses EF Core tools to add a corresponding migration describing the updates necessary to keep the database schema in sync. EF Core compares the current model against a snapshot of the old model to determine the differences, and generates migration source files; the files can be tracked in your project's source control like any other source file.
* Once a new migration has been generated, it can be applied to a database in various ways. EF Core records all applied migrations in a special history table, allowing it to know which migrations have been applied and which haven't.

The rest of this page is a step-by-step beginner's guide for using migrations. Consult the other pages in this section for more in-depth information.

## Getting started

Let's assume you've just completed your first EF Core application, which contains the following simple model:

```c#
public class Blog
{
    public int Id { get; set; }
    public string Name { get; set; }
}
```

During development, you may have used the [Create and Drop APIs](xref:core/managing-schemas/ensure-created) to iterate quickly, changing your model as needed; but now that your application is going to production, you need a way to safely evolve the schema without dropping the entire database.

### Install the tools

First, you'll have to install the [EF Core command-line tools](xref:core/miscellaneous/cli/index):

* We generally recommend using the [.NET Core CLI tools](xref:core/miscellaneous/cli/dotnet), which work on all platforms.
* If you're more comfortable working inside Visual Studio or have experience with EF6 migrations, you can also use the [Package Manager Console tools](xref:core/miscellaneous/cli/powershell).

### Create your first migration

You're now ready to add your first migration! Instruct EF Core to create a migration named **InitialCreate**:

#### [.NET Core CLI](#tab/dotnet-core-cli)

```dotnetcli
dotnet ef migrations add InitialCreate
```

#### [Visual Studio](#tab/vs)

``` powershell
Add-Migration InitialCreate
```

***

EF Core will create a directory called **Migrations** in your project, and generate some files. It's a good idea to inspect what exactly EF Core generated - and possibly amend it - but we'll skip over that for now.

### Create a migration SQL script

You now want to create your first production database and apply your migration there. In production scenarios, you typically need to create the database yourself, e.g. by using a cloud service user interface, or asking your DBA. Once an empty database is available, you can generate a *SQL script* from the migration we've just created:

#### [.NET Core CLI](#tab/dotnet-core-cli)

```dotnetcli
dotnet ef migrations script -o InitialCreate.sql
```
#### [Visual Studio](#tab/vs)

``` powershell
Script-Migration -Output InitialCreate.sql
```

***

You now have an **InitialCreate.sql** script which can be executed against your empty database to create the schema corresponding to your model. If you're using SQL Server, the script should resemble the following:

```sql
IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

CREATE TABLE [Blogs] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NULL,
    CONSTRAINT [PK_Blogs] PRIMARY KEY ([Id])
);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20200604212813_InitialCreate', N'3.1.4');

GO
```

The script contains three parts:

1. The migrations history table is created; this table will store all migrations applied to this database. This only happens in your very first migration.
2. Our `Blogs` table is created, based on our code model. This is the actual migration action.
3. A row is inserted into the migrations history table, to record that the **InitialCreate** migration was applied.

### Apply the script to your database

After we've examined the SQL script and are satisfied with it, we can execute it on our empty database. How this is done varies across databases, but with SQL Server you can use `sqlcmd`:

```console
sqlcmd -S <server> -U <user> -d <database> -i InitialCreate.sql
```

Congratulations, you've just applied your first migration! If you now connect to your database with a tool such as SQL Server Management Studio, you should see your table.

### Evolving your model

A few days have passed, and you're asked to add a creation timestamp to your blogs. You've done the necessary changes to your application, and your model now looks like this:

```c#
public class Blog
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedTimestamp { get; set; }
}
```

Your model and your production database are now out of sync - we must add a new column to your database schema. Let's create a new migration for this:


#### [.NET Core CLI](#tab/dotnet-core-cli)

```dotnetcli
dotnet ef migrations add AddBlogCreatedTimestamp
```

#### [Visual Studio](#tab/vs)

``` powershell
Add-Migration AddBlogCreatedTimestamp
```

***

Note that we give migrations a descriptive name, to make it easier to understand the project history later.

Since this isn't the project's first migration, EF Core now compares your updated model against a snapshot of the old model, before the column was added; the model snapshot is one of the files generated by EF Core when you add a migration, and is checked into source control. Based on that comparison, EF Core detects that a column has been added, and adds the appropriate migration.

We now want to generate a SQL script, just like before. However, the **InitialCreate** migration has already been applied to our database, so our new script should contain only the schema changes since that point:

#### [.NET Core CLI](#tab/dotnet-core-cli)

```dotnetcli
dotnet ef migrations script -o AddBlogCreatedTimestamp.sql InitialCreate
```
#### [Visual Studio](#tab/vs)

``` powershell
Script-Migration -Output AddBlogCreatedTimestamp.sql InitialCreate
```

***

Note the new parameter, which instructs EF Core to generate a script *since* the given migration. Our new SQL script should look the following:


```sql
ALTER TABLE [Blogs] ADD [CreatedTimestamp] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20200604214444_AddBlogCreatedTimestamp', N'3.1.4');

GO
```

The script first adds our new column to the database, and then records that the migration has been applied in the migrations history table.

### Next steps

The above was only a brief introduction to migrations. Please consult the other documentation pages to learn more about [managing migrations](xref:core/managing-schemas/migrations/managing), [applying them](xref:core/managing-schemas/migrations/applying), and other aspects. The [.NET Core CLI tool reference](xref:core/miscellaneous/cli/index) also contains useful information on the different commands
