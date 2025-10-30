---
title: Testing against your Production Database System - EF Core
description: Techniques for testing EF Core applications against your production database system
author: roji
ms.date: 1/24/2022
uid: core/testing/testing-with-the-database
---
# Testing against your production database system

In this page, we discuss techniques for writing automated tests which involve the database system against which the application runs in production. Alternate testing approaches exist, where the production database system is swapped out by test doubles; see the [testing overview page](xref:core/testing/index) for more information. Note that testing against a different database than what is used in production (e.g. Sqlite) is not covered here, since the different database is used as a test double; this approach is covered in [Testing without your production database system](xref:core/testing/testing-without-the-database).

The main hurdle with testing which involves a real database is to ensure proper test isolation, so that tests running in parallel (or even in serial) don't interfere with each other. The full sample code for the below can be viewed [here](https://github.com/dotnet/EntityFramework.Docs/blob/main/samples/core/Testing/TestingWithTheDatabase).

> [!TIP]
> This page shows [xUnit](https://xunit.net/) techniques, but similar concepts exist in other testing frameworks, including [NUnit](https://nunit.org/).

## Setting up your database system

Most database systems nowadays can be easily installed, both in CI environments and on developer machines. While it's frequently easy enough to install the database via the regular installation mechanism, ready-to-use Docker images are available for most major databases and can make installation particularly easy in CI. For the developer environment, [GitHub Workspaces](https://docs.github.com/en/codespaces/overview), [Dev Container](https://code.visualstudio.com/docs/remote/create-dev-container) can set up all needed services and dependencies - including the database. While this requires an initial investment in setup, once that's done you have a working testing environment and can concentrate on more important things.

In certain cases, databases have a special edition or version which can be helpful for testing. When using SQL Server, [LocalDB](/sql/database-engine/configure-windows/sql-server-express-localdb) can be used to run tests locally with virtually no setup at all, spinning up the database instance on demand and possibly saving resources on less powerful developer machines. However, LocalDB is not without its issues:

* It doesn't support everything that [SQL Server Developer Edition](/sql/sql-server/editions-and-components-of-sql-server-version-15#-editions) does.
* It's only available on Windows.
* It can cause lag on first test run as the service is spun up.

We generally recommend installing SQL Server Developer edition rather than LocalDB, since it provides the full SQL Server feature set and is generally very easy to do.

When using a cloud database, it's usually appropriate to test against a local version of the database, both to improve speed and to decrease costs. For example, when using SQL Azure in production, you can test against a locally-installed SQL Server - the two are extremely similar (though it's still wise to run tests against SQL Azure itself before going into production). When using Azure Cosmos DB, [the Azure Cosmos DB emulator](/azure/cosmos-db/local-emulator) is a useful tool both for developing locally and for running tests.

## Creating, seeding and managing a test database

Once your database is installed, you're ready to start using it in your tests. In most simple cases, your test suite has a single database that's shared between multiple tests across multiple test classes, so we need some logic to make sure the database is created and seeded exactly once during the lifetime of the test run.

When using Xunit, this can be done via a [class fixture](https://xunit.net/docs/shared-context#class-fixture), which represents the database and is shared across multiple test runs:

[!code-csharp[Main](../../../samples/core/Testing/TestingWithTheDatabase/TestDatabaseFixture.cs?name=TestDatabaseFixture)]

When the above fixture is instantiated, it uses <xref:Microsoft.EntityFrameworkCore.Storage.IDatabaseCreator.EnsureDeleted> to drop the database (in case it exists from a previous run), and then <xref:Microsoft.EntityFrameworkCore.Storage.IDatabaseCreator.EnsureCreated> to create it with your latest model configuration ([see the docs for these APIs](xref:core/managing-schemas/ensure-created)). Once the database is created, the fixture seeds it with some data our tests can use. It's worth spending some time thinking about your seed data, since changing it later for a new test may cause existing tests to fail.

To use the fixture in a test class, simply implement `IClassFixture` over your fixture type, and xUnit will inject it into your constructor:

[!code-csharp[Main](../../../samples/core/Testing/TestingWithTheDatabase/BloggingControllerTest.cs?name=UsingTheFixture)]

Your test class now has a `Fixture` property which can be used by tests to create a fully functional context instance:

[!code-csharp[Main](../../../samples/core/Testing/TestingWithTheDatabase/BloggingControllerTest.cs?name=GetBlog&highlight=4)]

Finally, you may have noticed some locking in the fixture's creation logic above. If the fixture is only used in a single test class, it is guaranteed to be instantiated exactly once by xUnit; but it's common to use the same database fixture in multiple test classes. xUnit does provide [collection fixtures](https://xunit.net/docs/shared-context#collection-fixture), but that mechanism prevents your test classes from running in parallel, which is important for test performance. To safely manage this with an xUnit class fixture, we take a simple lock around database creation and seeding, and use a static flag to make sure we never have to do it twice.

## Tests which modify data

The above example showed a read-only test, which is the easy case from a test isolation standpoint: since nothing is being modified, test interference isn't possible. In contrast, tests which modify data are more problematic, since they may interfere with one another. One common technique to isolate writing tests is to wrap the test in a transaction, and to have that transaction rolled back at the end of the test. Since nothing is actually committed to the database, other tests don't see any modifications and interference is avoided.

Here's a controller method which adds a Blog to our database:

[!code-csharp[Main](../../../samples/core/Testing/BloggingWebApi/Controllers/BloggingController.cs?name=AddBlog)]

We can test this method with the following:

[!code-csharp[Main](../../../samples/core/Testing/TestingWithTheDatabase/BloggingControllerTest.cs?name=AddBlog&highlight=5,10)]

Some notes on the test code above:

* We start a transaction to make sure the changes below aren't committed to the database, and don't interfere with other tests. Since the transaction is never committed, it is implicitly rolled back at the end of the test when the context instance is disposed.
* After making the updates we want, we clear the context instance's change tracker with <xref:Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.Clear*?displayProperty=nameWithType>, to make sure we actually load the blog from the database below. We could use two context instances instead, but we'd then have to make sure the same transaction is used by both instances.
* You may even want to start the transaction in the fixture's `CreateContext`, so that tests receive a context instance that's already in a transaction, and ready for updates. This can help prevent cases where the transaction is accidentally forgotten, leading to test interference which can be hard to debug. You may also want to separate read-only and write tests in different test classes as well.

## Tests which explicitly manage transactions

There is one final category of tests which presents an additional difficulty: tests which modify data and also explicitly manage transactions. Because databases do not typically support nested transactions, it isn't possible to use transactions for isolation as above, since they need to be used by actual product code. While these tests tend to be more rare, it's necessary to handle them in a special way: you must clean up your database to its original state after each test, and parallelization must be disabled so that these tests don't interfere with each other.

Let's examine the following controller method as an example:

[!code-csharp[Main](../../../samples/core/Testing/BloggingWebApi/Controllers/BloggingController.cs?name=UpdateBlogUrl&highlight=5)]

Let's assume that for some reason, the method requires a serializable transaction to be used (this isn't typically the case). As a result, we cannot use a transaction to guarantee test isolation. Since the test will actually commit changes to the database, we'll define another fixture with its own, separate database, to make sure we don't interfere with the other tests already shown above:

[!code-csharp[Main](../../../samples/core/Testing/TestingWithTheDatabase/TransactionalTestDatabaseFixture.cs?name=TransactionalTestDatabaseFixture)]

This fixture is similar to the one used above, but notably contains a `Cleanup` method; we'll call this after every test to ensure that the database is reset to its starting state.

If this fixture will only be used by a single test class, we can reference it as a class fixture as above - xUnit doesn't parallelize tests within the same class (read more about test collections and parallelization in the [xUnit docs](https://xunit.net/docs/running-tests-in-parallel.html)). If, however, we want to share this fixture between multiple classes, we must make sure these classes don't run in parallel, to avoid any interference. To do that, we will use this as an xUnit [collection fixture](https://xunit.net/docs/shared-context#collection-fixture) rather than as a [class fixture](https://xunit.net/docs/shared-context#class-fixture).

First, we define a *test collection*, which references our fixture and will be used by all transactional test classes which require it:

[!code-csharp[Main](../../../samples/core/Testing/TestingWithTheDatabase/TransactionalTestDatabaseFixture.cs?name=CollectionDefinition)]

We now reference the test collection in our test class, and accept the fixture in the constructor as before:

[!code-csharp[Main](../../../samples/core/Testing/TestingWithTheDatabase/TransactionalBloggingControllerTest.cs?name=UsingTheFixture&highlight=1,4)]

Finally, we make our test class disposable, arranging for the fixture's `Cleanup` method to be called after each test:

[!code-csharp[Main](../../../samples/core/Testing/TestingWithTheDatabase/TransactionalBloggingControllerTest.cs?name=Dispose)]

Note that since xUnit only ever instantiates the collection fixture once, there is no need for us to use locking around database creation and seeding as we did above.

The full sample code for the above can be viewed [here](https://github.com/dotnet/EntityFramework.Docs/blob/main/samples/core/Testing/TestingWithTheDatabase/TransactionalBloggingControllerTest.cs).

> [!TIP]
> If you have multiple test classes with tests which modify the database, you can still run them in parallel by having different fixtures, each referencing its own database. Creating and using many test databases isn't problematic and should be done whenever it's helpful.

## Efficient database creation

In the samples above, we used <xref:Microsoft.EntityFrameworkCore.Storage.IDatabaseCreator.EnsureDeleted> and <xref:Microsoft.EntityFrameworkCore.Storage.IDatabaseCreator.EnsureCreated> before running tests, to make sure we have an up-to-date test database. These operations can be a bit slow in certain databases, which can be a problem as you iterate over code changes and re-run tests over and over. If that's the case, you may want to temporarily comment out `EnsureDeleted` in your fixture's constructor: this will reuse the same database across test runs.

The disadvantage of this approach is that if you change your EF Core model, your database schema won't be up to date, and tests may fail. As a result, we only recommend doing this temporarily during the development cycle.

## Efficient database cleanup

We saw above that when changes are actually committed to the database, we must clean up the database between every test to avoid interference. In the transactional test sample above, we did this by using EF Core APIs to delete the table's contents:

[!code-csharp[Main](../../../samples/core/Testing/TestingWithTheDatabase/TransactionalTestDatabaseFixture.cs?name=Cleanup)]

This typically isn't the most efficient way to clear out a table. If test speed is a concern, you may want to use raw SQL to delete the table instead:

```sql
DELETE FROM [Blogs];
```

You may also want to consider using the [respawn](https://github.com/jbogard/respawn) package, which efficiently clears out a database. In addition, it does not require you to specify the tables to be cleared, and so your cleanup code does not need to be updated as tables are added to your model.

## Summary

* When testing against a real database, it's worth distinguishing between the following test categories:
  * Read-only tests are relatively simple, and can always execute in parallel against the same database without having to worry about isolation.
  * Write tests are more problematic, but transactions can be used to make sure they're properly isolated.
  * Transactional tests are the most problematic, requiring logic to reset the database back to its original state, as well as disabling parallelization.
* Separating these test categories out into separate classes may avoid confusion and accidental interference between tests.
* Give some thought up-front to your seeded test data, and try to write your tests in a way that won't break too often if that seed data changes.
* Use multiple databases to parallelize tests which modify the database, and possibly also to allow different seed data configurations.
* If test speed is a concern, you may want to look at more efficient techniques for creating your test database, and for cleaning its data between runs.
* Always keep test parallelization and isolation in mind.
