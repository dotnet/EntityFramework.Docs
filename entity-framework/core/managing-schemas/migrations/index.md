---
title: Migrations - EF Core
author: bricelam
ms.author: bricelam
ms.date: 10/30/2017
ms.technology: entity-framework-core
---
Migrations
==========
Migrations provide a way to incrementally apply schema changes to the database to keep it in sync with your EF Core
model while preserving existing data in the database.

Creating the database
---------------------
After you've [defined your initial model][1], it's time to create the database. To do this, add an initial migration.
Install the [EF Core Tools][2] and run the appropriate command.

``` powershell
Add-Migration InitialCreate
```
``` Console
dotnet ef migrations add InitialCreate
```

Three files are added to your project under the **Migrations** directory:

* **00000000000000_InitialCreate.cs**--The main migrations file. Contains the operations necessary to apply the
  migration (in `Up()`) and to revert it (in `Down()`).
* **00000000000000_InitialCreate.Designer.cs**--The migrations metadata file. Contains information used by EF.
* **MyContextModelSnapshot.cs**--A snapshot of your current model. Used to determine what changed when adding the next
  migration.

The timestamp in the filename helps keep them ordered chronologically so you can see the progression of changes.

> [!TIP]
> You are free to move Migrations files and change their namespace. New migrations are created as siblings of the last
> migration.

Next, apply the migration to the database to create the schema.

``` powershell
Update-Database
```
``` Console
dotnet ef database update
```

Adding another migration
------------------------
After making changes to your EF Core model, the database schema will be out of sync. To bring it up to date, add another
migration. The migration name can be used like a commit message in a version control system. For example, if I made
changes to save customer reviews of products, I might choose something like *AddProductReviews*.

``` powershell
Add-Migration AddProductReviews
```
``` Console
dotnet ef migrations add AddProductReviews
```

Once the migration is scaffolded, you should review it for accuracy and add any additional operations required to apply
it correctly. For example, your migration might contain the following operations:

``` csharp
migrationBuilder.DropColumn(
    name: "FirstName",
    table: "Customer");

migrationBuilder.DropColumn(
    name: "LastName",
    table: "Customer");

migrationBuilder.AddColumn<string>(
    name: "Name",
    table: "Customer",
    nullable: true);
```

While these operations make the database schema compatible, they don't preserve the existing customer names. To make
it better, rewrite it as follows.

``` csharp
migrationBuilder.AddColumn<string>(
    name: "Name",
    table: "Customer",
    nullable: true);

migrationBuilder.Sql(
@"
    UPDATE Customer
    SET Name = FirstName + ' ' + LastName;
");

migrationBuilder.DropColumn(
    name: "FirstName",
    table: "Customer");

migrationBuilder.DropColumn(
    name: "LastName",
    table: "Customer");
```

> [!TIP]
> Adding a new migration warns when an operation is scaffolded that may result in data loss (like dropping a column). Be
> sure to especially review these migrations for accuracy.

Apply the migration to the database using the appropriate command.

``` powershell
Update-Database
```
``` Console
dotnet ef database update
```

Removing a migration
--------------------
Sometimes you add a migration and realize you need to make additional changes to your EF Core model before applying it.
To remove the last migration, use this command.

``` powershell
Remove-Migration
```
``` Console
dotnet ef migrations remove
```

After removing it, you can make the additional model changes and add it again.

Reverting a migration
---------------------
If you already applied a migration (or several migrations) to the database but need to revert it, you can use the same
command to apply migrations, but specify the name of the migration you want to roll back to.

``` powershell
Update-Database LastGoodMigration
```
``` Console
dotnet ef database update LastGoodMigration
```

Empty migrations
----------------
Sometimes it's useful to add a migration without making any model changes. In this case, adding a new migration creates
an empty one. You can customize this migration to perform operations that don't directly relate to the EF Core model.
Some things you might want to manage this way are:

* Full-Text Search
* Functions
* Stored procedures
* Triggers
* Views
* etc.

Generating a SQL script
-----------------------
When debugging your migrations or deploying them to a production database, it's useful to generate a SQL script. The
script can then be further reviewed for accuracy and tuned to fit the needs of a production database. The script can
also be used in conjunction with a deployment technology. The basic command is as follows.

``` powershell
Script-Migration
```
``` Console
dotnet ef migrations script
```

There are several options to this command.

The **from** migration should be the last migration applied to the database before running the script. If no migrations
have been applied, specify `0` (this is the default).

The **to** migration is the last migration that will be applied to the database after running the script. This defaults
to the last migration in your project.

An **idempotent** script can optionally be generated. This script only applies migrations if they haven't already been
applied to the database. This is useful if you don't exactly know what the last migration applied to the database was or
if you are deploying to multiple databases that may each be at a different migration.

Applying migrations at runtime
------------------------------
Some apps may want to apply migrations at runtime during startup or first run. Do this using the `Migrate()` method.

Caution, this approach isn't for everyone. While it's great for apps with a local database, most applications will
require more robust deployment strategy like generating SQL scripts.

``` csharp
myDbContext.Database.Migrate();
```

> [!WARNING]
> Don't call `EnsureCreated()` before `Migrate()`. `EnsureCreated()` bypasses Migrations to create the schema and
> cause `Migrate()` to fail.

> [!NOTE]
> This method builds on top of the `IMigrator` service, which can be used for more advanced scenarios. Use
> `DbContext.GetService<IMigrator>()` to access it.


  [1]: ../../modeling/index.md
  [2]: ../../miscellaneous/cli/index.md
