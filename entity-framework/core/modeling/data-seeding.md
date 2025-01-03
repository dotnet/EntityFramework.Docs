---
title: Data Seeding - EF Core
description: Using data seeding to populate a database with an initial set of data using Entity Framework Core
author: AndriySvyryd
ms.date: 10/10/2024
uid: core/modeling/data-seeding
---

# Data Seeding

Data seeding is the process of populating a database with an initial set of data.

There are several ways this can be accomplished in EF Core:

* [Configuration options data seeding (`UseSeeding`)](#use-seeding-method)
* [Custom initialization logic](#custom-initialization-logic)
* [Model managed data (`HasData`)](#model-seed-data)
* [Manual migration customization](#manual-migration-customization)

<a name="use-seeding-method"></a>

## Configuration options `UseSeeding` and `UseAsyncSeeding` methods

EF 9 introduced `UseSeeding` and `UseAsyncSeeding` methods, which provide a convenient way of seeding the database with initial data. These methods aim to improve the experience of using custom initialization logic (explained below). They provide one clear location where all the data seeding code can be placed. Moreover, the code inside `UseSeeding` and `UseAsyncSeeding` methods is protected by the [migration locking mechanism](/ef/core/what-is-new/ef-core-9.0/whatsnew#concurrent-migrations) to prevent concurrency issues.

The new seeding methods are called as part of [`EnsureCreated`](xref:Microsoft.EntityFrameworkCore.Storage.IDatabaseCreator.EnsureCreated) operation, [`Migrate`](/dotnet/api/microsoft.entityframeworkcore.relationaldatabasefacadeextensions.migrate) and `dotnet ef database update` command, even if there are no model changes and no migrations were applied.

> [!TIP]
> Using `UseSeeding` and `UseAsyncSeeding` is the recommended way of seeding the database with initial data when working with EF Core.

These methods can be set up in the [options configuration step](/ef/core/dbcontext-configuration/#dbcontextoptions). Here is an example:

[!code-csharp[ContextOptionSeeding](../../../samples/core/Modeling/DataSeeding/DataSeedingContext.cs?name=ContextOptionSeeding)]

> [!NOTE]
> `UseSeeding` is called from the `EnsureCreated` method, and `UseAsyncSeeding` is called from the `EnsureCreatedAsync` method. When using this feature, it is recommended to implement both `UseSeeding` and `UseAsyncSeeding` methods using similar logic, even if the code using EF is asynchronous. EF Core tooling currently relies on the synchronous version of the method and will not seed the database correctly if the `UseSeeding` method is not implemented.

<a name="custom-initialization-logic"></a>

## Custom initialization logic

A straightforward and powerful way to perform data seeding is to use [`DbContext.SaveChangesAsync()`](xref:core/saving/index) before the main application logic begins execution. It is recommended to use `UseSeeding` and `UseAsyncSeeding` for that purpose, however sometimes using these methods is not a good solution. An example scenario is when seeding requires using two different contexts in one transaction. Below is a code sample performing custom initialization in the application directly:

[!code-csharp[Main](../../../samples/core/Modeling/DataSeeding/Program.cs?name=CustomSeeding)]

> [!WARNING]
> The seeding code should not be part of the normal app execution as this can cause concurrency issues when multiple instances are running and would also require the app having permission to modify the database schema.

Depending on the constraints of your deployment the initialization code can be executed in different ways:

* Running the initialization app locally
* Deploying the initialization app with the main app, invoking the initialization routine and disabling or removing the initialization app.

This can usually be automated by using [publish profiles](/aspnet/core/host-and-deploy/visual-studio-publish-profiles).

<a name="model-seed-data"></a>

## Model managed data

Data can also be associated with an entity type as part of the model configuration. Then, EF Core [migrations](xref:core/managing-schemas/migrations/index) can automatically compute what insert, update or delete operations need to be applied when upgrading the database to a new version of the model.

> [!WARNING]
> Migrations only considers model changes when determining what operation should be performed to get the managed data into the desired state. Thus any changes to the data performed outside of migrations might be lost or cause an error.

As an example, this will configure managed data for a `Country` in `OnModelCreating`:

[!code-csharp[CountrySeed](../../../samples/core/Modeling/DataSeeding/ManagingDataContext.cs?name=CountrySeed)]

To add entities that have a relationship the foreign key values need to be specified:

[!code-csharp[CitySeed](../../../samples/core/Modeling/DataSeeding/ManagingDataContext.cs?name=CitySeed)]

When managing data for many-to-many navigations, the join entity needs to be configured explicitly. If the entity type has any properties in shadow state (e.g. the `LanguageCountry` join entity below), an anonymous class can be used to provide the values:

[!code-csharp[LanguageSeed](../../../samples/core/Modeling/DataSeeding/ManagingDataContext.cs?name=LanguageSeed)]

Owned entity types can be configured in a similar fashion:

[!code-csharp[LanguageDetailsSeed](../../../samples/core/Modeling/DataSeeding/ManagingDataContext.cs?name=LanguageDetailsSeed)]

See the [full sample project](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Modeling/DataSeeding) for more context.

Once the data has been added to the model, [migrations](xref:core/managing-schemas/migrations/index) should be used to apply the changes.

> [!TIP]
> If you need to apply migrations as part of an automated deployment you can [create a SQL script](xref:core/managing-schemas/migrations/applying#sql-scripts) that can be previewed before execution.

Alternatively, you can use `context.Database.EnsureCreatedAsync()` to create a new database containing the managed data, for example for a test database or when using the in-memory provider or any non-relational database. Note that if the database already exists, `EnsureCreatedAsync()` will neither update the schema nor managed data in the database. For relational databases you shouldn't call `EnsureCreatedAsync()` if you plan to use Migrations.

> [!NOTE]
> Populating the database using the `HasData` method used to be referred to as "data seeding". This naming sets incorrect expectations, as the feature has a number of limitations and is only appropriate for specific types of data. That is why we decided to rename it to "model managed data". `UseSeeding` and `UseAsyncSeeding` methods should be used for general purpose data seeding.

### Limitations of model managed data

This type of data is managed by migrations and the script to update the data that's already in the database needs to be generated without connecting to the database. This imposes some restrictions:

* The primary key value needs to be specified even if it's usually generated by the database. It will be used to detect data changes between migrations.
* Previously inserted data will be removed if the primary key is changed in any way.

Therefore this feature is most useful for static data that's not expected to change outside of migrations and does not depend on anything else in the database, for example ZIP codes.

If your scenario includes any of the following it is recommended to use `UseSeeding` and `UseAsyncSeeding` methods described in the first section:

* Temporary data for testing
* Data that depends on database state
* Data that is large (seeding data gets captured in migration snapshots, and large data can quickly lead to huge files and degraded performance).
* Data that needs key values to be generated by the database, including entities that use alternate keys as the identity
* Data that requires custom transformation (that is not handled by [value conversions](xref:core/modeling/value-conversions)), such as some password hashing
* Data that requires calls to external API, such as ASP.NET Core Identity roles and users creation
* Data that isn't fixed and deterministic, such as seeding to `DateTime.Now`.

<a name="manual-migration-customization"></a>

## Manual migration customization

When a migration is added the changes to the data specified with `HasData` are transformed to calls to `InsertData()`, `UpdateData()`, and `DeleteData()`. One way of working around some of the limitations of `HasData` is to manually add these calls or [custom operations](xref:core/managing-schemas/migrations/operations) to the migration instead.

[!code-csharp[CustomInsert](../../../samples/core/Modeling/DataSeeding/Migrations/20241016041555_Initial.cs?name=CustomInsert)]
