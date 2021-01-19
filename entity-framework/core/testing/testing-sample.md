---
title: EF Core testing sample - EF Core
description: Sample showing how to test applications which use Entity Framework Core
author: ajcvickers
ms.date: 04/22/2020
uid: core/testing/testing-sample
no-loc: [Item, Tag, Items, Tags, items, tags]
---

# EF Core testing sample

> [!TIP]
> The code in this document can be found on GitHub as a [runnable sample](https://github.com/dotnet/EntityFramework.Docs/tree/master/samples/core/Miscellaneous/Testing/ItemsWebApi/).
> Note that some of these tests **are expected to fail**. The reasons for this are explained below.

This doc walks through a sample for testing code that uses EF Core.

## The application

The [sample](https://github.com/dotnet/EntityFramework.Docs/tree/master/samples/core/Miscellaneous/Testing/ItemsWebApi/) contains two projects:

- ItemsWebApi: A very simple [Web API backed by ASP.NET Core](/aspnet/core/tutorials/first-web-api) with a single controller
- Tests: An [XUnit](https://xunit.net/) test project to test the controller

### The model and business rules

The model backing this API has two entity types: Items and Tags.

- Items have a case-sensitive name and a collection of Tags.
- Each Tag has a label and a count representing the number of times it has been applied to the Item.
- Each Item should only have one Tag with a given label.
  - If an item is tagged with the same label more than once, then the count on the existing tag with that label is incremented instead of a new tag being created.
- Deleting an Item should delete all associated Tags.

#### The Item entity type

The `Item` entity type:

[!code-csharp[ItemEntityType](../../../samples/core/Miscellaneous/Testing/ItemsWebApi/ItemsWebApi/Item.cs?name=ItemEntityType)]

And its configuration in `DbContext.OnModelCreating`:

[!code-csharp[ConfigureItem](../../../samples/core/Miscellaneous/Testing/ItemsWebApi/ItemsWebApi/ItemsContext.cs?name=ConfigureItem)]

Notice that entity type constrains the way it can be used to reflect the domain model and business rules. In particular:

- The primary key is mapped directly to the `_id` field and not exposed publicly
  - EF detects and uses the private constructor accepting the primary key value and name.
- The `Name` property is read-only and set only in the constructor.
- Tags are exposed as a `IReadOnlyList<Tag>` to prevent arbitrary modification.
  - EF associates the `Tags` property with the `_tags` backing field by matching their names.
  - The `AddTag` method takes a tag label and implements the business rule described above.
    That is, a tag is only added for new labels.
    Otherwise the count on an existing label is incremented.
- The `Tags` navigation property is configured for a many-to-one relationship
  - There is no need for a navigation property from Tag to Item, so it is not included.
  - Also, Tag does not define a foreign key property.
    Instead, EF will create and manage a property in shadow-state.

#### The Tag entity type

The `Tag` entity type:

[!code-csharp[TagEntityType](../../../samples/core/Miscellaneous/Testing/ItemsWebApi/ItemsWebApi/Tag.cs?name=TagEntityType)]

And its configuration in `DbContext.OnModelCreating`:

[!code-csharp[ConfigureTag](../../../samples/core/Miscellaneous/Testing/ItemsWebApi/ItemsWebApi/ItemsContext.cs?name=ConfigureTag)]

Similarly to Item, Tag hides its primary key and makes the `Label` property read-only.

### The ItemsController

The Web API controller is pretty basic.
It gets a `DbContext` from the dependency injection container through constructor injection:

[!code-csharp[Constructor](../../../samples/core/Miscellaneous/Testing/ItemsWebApi/ItemsWebApi/Controllers/ItemsController.cs?name=Constructor)]

It has methods to get all Items or an Item with a given name:

[!code-csharp[Get](../../../samples/core/Miscellaneous/Testing/ItemsWebApi/ItemsWebApi/Controllers/ItemsController.cs?name=Get)]

It has a method to add a new Item:

[!code-csharp[PostItem](../../../samples/core/Miscellaneous/Testing/ItemsWebApi/ItemsWebApi/Controllers/ItemsController.cs?name=PostItem)]

A method to tag an Item with a label:

[!code-csharp[PostTag](../../../samples/core/Miscellaneous/Testing/ItemsWebApi/ItemsWebApi/Controllers/ItemsController.cs?name=PostTag)]

And a method to delete an Item and all associated Tags:

[!code-csharp[DeleteItem](../../../samples/core/Miscellaneous/Testing/ItemsWebApi/ItemsWebApi/Controllers/ItemsController.cs?name=DeleteItem)]

Most validation and error handling have been removed to reduce clutter.

## The Tests

The tests are organized to run with multiple database provider configurations:

- The SQL Server provider, which is the provider used by the application
- The SQLite provider
- The SQLite provider using in-memory SQLite databases
- The EF in-memory database provider

This is achieved by putting all the tests in a base class, then inheriting from this to test with each provider.

> [!TIP]
> You will need to change the SQL Server connection string if you're not using LocalDB.
> See [Testing with SQLite](xref:core/testing/sqlite) for guidance on using SQLite for in-memory testing.

The following two tests are expected to fail:

- `Can_remove_item_and_all_associated_tags` when running with the EF in-memory database provider
- `Can_add_item_differing_only_by_case` when running with the SQL Server provider

This is covered in more detail below.

### Setting up and seeding the database

XUnit, like most testing frameworks, will create a new test class instance for each test run.
Also, XUnit will not run tests within a given test class in parallel.
This means that we can set up and configure the database in the test constructor and it will be in a well-known state for each test.

> [!TIP]
> This sample recreates the database for each test.
> This works well for SQLite and EF in-memory database testing but can involve significant overhead with other database systems, including SQL Server.
> Approaches for reducing this overhead are covered in [Sharing databases across tests](xref:core/testing/sharing-databases).

When each test is run:

- DbContextOptions are configured for the provider in use and passed to the base class constructor
  - These options are stored in a property and used throughout the tests for creating DbContext instances
- A Seed method is called to create and seed the database
  - The Seed method ensures the database is clean by deleting it and then re-creating it
  - Some well-known test entities are created and saved to the database

[!code-csharp[Seeding](../../../samples/core/Miscellaneous/Testing/ItemsWebApi/Tests/ItemsControllerTest.cs?name=Seeding)]

Each concrete test class then inherits from this.
For example:

[!code-csharp[SqliteItemsControllerTest](../../../samples/core/Miscellaneous/Testing/ItemsWebApi/Tests/SqliteItemsControllerTest.cs?name=SqliteItemsControllerTest)]

### Test structure

Even though the application uses dependency injection, the tests do not.
It would be fine to use dependency injection here, but the additional code it requires has little value.
Instead, a DbContext is created using `new` and then directly passed as the dependency to the controller.

Each test then executes the method under test on the controller and asserts the results are as expected.
For example:

[!code-csharp[CanGetItems](../../../samples/core/Miscellaneous/Testing/ItemsWebApi/Tests/ItemsControllerTest.cs?name=CanGetItems)]

Notice that different DbContext instances are used to seed the database and run the tests.
This ensures that the test is not using (or tripping over) entities tracked by the context when seeding.
It also better matches what happens in web apps and services.

Tests that mutate the database create a second DbContext instance in the test for similar reasons.
That is, creating a new, clean, context and then reading into it from the database to ensure that the changes were saved to the database.
For example:

[!code-csharp[CanAddItem](../../../samples/core/Miscellaneous/Testing/ItemsWebApi/Tests/ItemsControllerTest.cs?name=CanAddItem)]

Two slightly more involved tests cover the business logic around adding tags.

[!code-csharp[CanAddTag](../../../samples/core/Miscellaneous/Testing/ItemsWebApi/Tests/ItemsControllerTest.cs?name=CanAddTag)]

[!code-csharp[CanUpTagCount](../../../samples/core/Miscellaneous/Testing/ItemsWebApi/Tests/ItemsControllerTest.cs?name=CanUpTagCount)]

## Issues using different database providers

Testing with a different database system than is used in the production application can lead to problems.
These are covered at the conceptual level in [Testing code that uses EF Core](xref:core/testing/index).
The sections below cover two examples of such issues demonstrated by the tests in this sample.

### Test passes when the application is broken

One of the requirements for our application is that "Items have a case-sensitive name and a collection of Tags."
This is pretty simple to test:

[!code-csharp[CanAddItemCaseInsensitive](../../../samples/core/Miscellaneous/Testing/ItemsWebApi/Tests/ItemsControllerTest.cs?name=CanAddItemCaseInsensitive)]

Running this test against the EF in-memory database indicates that everything is fine.
Everything still looks fine when using SQLite.
But the test fails when run against SQL Server!

```output
System.InvalidOperationException : Sequence contains more than one element
   at System.Linq.ThrowHelper.ThrowMoreThanOneElementException()
   at System.Linq.Enumerable.Single[TSource](IEnumerable`1 source)
   at Microsoft.EntityFrameworkCore.Query.Internal.QueryCompiler.Execute[TResult](Expression query)
   at Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryProvider.Execute[TResult](Expression expression)
   at System.Linq.Queryable.Single[TSource](IQueryable`1 source, Expression`1 predicate)
   at Tests.ItemsControllerTest.Can_add_item_differing_only_by_case()
```

This is because both the EF in-memory database and the SQLite database are case-sensitive by default.
SQL Server, on the other hand, is case-insensitive!

EF Core, by design, does not change these behaviors because forcing a change in case-sensitivity can have a big performance impact.

Once we know this is a problem we can fix the application and compensate in tests.
However, the point here is that this bug could be missed if only testing with the EF in-memory database or SQLite providers.

### Test fails when the application is correct

Another of the requirements for our application is that "deleting an Item should delete all associated Tags."
Again, easy to test:

[!code-csharp[DeleteItem](../../../samples/core/Miscellaneous/Testing/ItemsWebApi/Tests/ItemsControllerTest.cs?name=DeleteItem)]

This test passes on SQL Server and SQLite, but fails with the EF in-memory database!

```output
Assert.False() Failure
Expected: False
Actual:   True
   at Tests.ItemsControllerTest.Can_remove_item_and_all_associated_tags()
```

In this case, the application is working correctly because SQL Server supports [cascade deletes](xref:core/saving/cascade-delete).
SQLite also supports cascade deletes, as do most relational databases, so testing this on SQLite works.
On the other hand, the EF in-memory database [does not support cascade deletes](https://github.com/dotnet/efcore/issues/3924).
This means that this part of the application cannot be tested with the EF in-memory database provider.
