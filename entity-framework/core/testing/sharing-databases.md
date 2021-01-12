---
title: Sharing databases between tests - EF Core
description: Sample showing how to share a database between multiple tests
author: ajcvickers
ms.date: 04/25/2020
uid: core/testing/sharing-databases
---

# Sharing databases between tests

The [EF Core testing sample](xref:core/testing/testing-sample) showed how to test applications against different database systems.
For that sample, each test created a new database.
This is a good pattern when using SQLite or the EF in-memory database, but it can involve significant overhead when using other database systems.

This sample builds on the previous sample by moving database creation into a test fixture.
This allows a single SQL Server database to be created and seeded only once for all tests.

> [!TIP]
> Make sure to work through the [EF Core testing sample](xref:core/testing/testing-sample) before continuing here.

It's not difficult to write multiple tests against the same database.
The trick is doing it in a way that the tests don't trip over each other as they run.
This requires understanding:

* How to safely share objects between tests
* When the test framework runs tests in parallel
* How to keep the database in a clean state for every test

## The fixture

We will use a test fixture for sharing objects between tests.
The [XUnit documentation](https://xunit.net/docs/shared-context.html) states that a fixture should be used "when you want to create a single test context and share it among all the tests in the class, and have it cleaned up after all the tests in the class have finished."

> [!TIP]
> This sample uses [XUnit](https://xunit.net/), but similar concepts exist in other testing frameworks, including [NUnit](https://nunit.org/).

This means that we need to move database creation and seeding to a fixture class.
Here's what it looks like:

[!code-csharp[SharedDatabaseFixture](../../../samples/core/Miscellaneous/Testing/ItemsWebApi/SharedDatabaseTests/SharedDatabaseFixture.cs?name=SharedDatabaseFixture)]

For now, notice how the constructor:

* Creates a single database connection for the lifetime of the fixture
* Creates and seeds that database by calling the `Seed` method

Ignore the locking for now; we will come back to it later.

> [!TIP]
> The creation and seeding code does not need to be async.
> Making it async will complicate the code and will not improve performance or throughput of tests.

The database is created by first deleting any existing database and then creating a new database.
This ensures that the database matches the current EF model even if it has been changed since the last test run.

> [!TIP]
> It can be faster to "clean" the existing database using something like [respawn](https://jimmybogard.com/tag/respawn/) rather than re-create it each time.
> However, care must be taken to ensure that the database schema is up-to-date with the EF model when doing this.

The database connection is disposed when the fixture is disposed.
You may also consider deleting the test database at this point.
However, this will require additional locking and reference counting if the fixture is being shared by multiple test classes.
Also, it is often useful to have the test database still available for debugging failed tests.

## Using the fixture

XUnit has a common pattern for associating a test fixture with a class of tests:

[!code-csharp[UsingTheFixture](../../../samples/core/Miscellaneous/Testing/ItemsWebApi/SharedDatabaseTests/SharedDatabaseTest.cs?name=UsingTheFixture)]

XUnit will now create a single fixture instance and pass it to each instance of the test class.
(Remember from the first [testing sample](xref:core/testing/testing-sample) that XUnit creates a new test class instance every time it runs a test.)
This means that the database will be created and seeded once and then each test will use this database.

Note that tests within a single class will not be run in parallel.
This means it is safe for each test to use the same database connection, even though the `DbConnection` object is not thread-safe.

## Maintaining database state

Tests often need to mutate the test data with inserts, updates, and deletes.
But these changes will then impact other tests which are expecting a clean, seeded database.

This can be dealt with by running mutating tests inside a transaction.
For example:

[!code-csharp[CanAddItem](../../../samples/core/Miscellaneous/Testing/ItemsWebApi/SharedDatabaseTests/SharedDatabaseTest.cs?name=CanAddItem)]

Notice that the transaction is created as the test starts and disposed when it is finished.
Disposing the transaction causes it to be rolled back, so none of the changes will be seen by other tests.

The helper method for creating a context (see the fixture code above) accepts this transaction and opts the DbContext into using it.

## Sharing the fixture

You may have noticed locking code around database creation and seeding.
This is not needed for this sample since only one class of tests use the fixture, so only a single fixture instance is created.

However, you may want to use the same fixture with multiple classes of tests.
XUnit will create one fixture instance for each of these classes.
These may be used by different threads running tests in parallel.
Therefore, it is important to have appropriate locking to ensure only one thread does the database creation and seeding.

> [!TIP]
> A simple `lock` is fine here.
> There is no need to attempt anything more complex, such as any lock-free patterns.
