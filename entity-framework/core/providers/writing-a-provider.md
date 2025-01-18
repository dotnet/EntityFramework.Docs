---
title: Writing a Database Provider - EF Core
description: Information on writing a new Entity Framework Core provider
author: SamMonoRT
ms.date: 10/27/2016
uid: core/providers/writing-a-provider
---

# Writing a Database Provider

For information about writing an Entity Framework Core database provider, see [So you want to write an EF Core provider](https://blog.oneunicorn.com/2016/11/11/so-you-want-to-write-an-ef-core-provider/) by [Arthur Vickers](https://github.com/ajcvickers).

> [!NOTE]
> These posts have not been updated since EF Core 1.1 and there have been significant changes since that time.
[Issue 681](https://github.com/dotnet/EntityFramework.Docs/issues/681) is tracking updates to this documentation.

The EF Core codebase is open source and contains several database providers that can be used as a reference. You can find the source code at <https://github.com/dotnet/efcore>. It may also be helpful to look at the code for commonly used third-party providers, such as [Npgsql](https://github.com/npgsql/Npgsql.EntityFrameworkCore.PostgreSQL), [Pomelo MySQL](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql), and [SQL Server Compact](https://github.com/ErikEJ/EntityFramework.SqlServerCompact). In particular, these projects are set up to extend from and run functional tests that we publish on NuGet. This kind of setup is strongly recommended.

## Community standup videos

The EF team produces "community standup" videos, where we discuss various aspects of .NET and data access. Some of these have explored EF internals, and can be a good way to understand the general architecture of EF when writing a provider:

* [How to Add a Feature to EF Core](https://www.youtube.com/watch?v=9OMxy1wal1s&list=PLdo4fOcmZ0oX0ObHwBrJ0vJpZ7PiYMqeA)
* [Internal Dependency Injection](https://www.youtube.com/watch?v=pYhe-Mt0HzI&list=PLdo4fOcmZ0oX0ObHwBrJ0vJpZ7PiYMqeA)
* [DbContext Configuration and Lifetime](https://www.youtube.com/watch?v=NPgFlqXPbK8&list=PLdo4fOcmZ0oX0ObHwBrJ0vJpZ7PiYMqeA)
* [IQueryable, LINQ and the EF Core query pipeline](https://www.youtube.com/watch?v=1Ld3dtnTrMw&list=PLdo4fOcmZ0oX0ObHwBrJ0vJpZ7PiYMqeA)
* [Model Building](https://www.youtube.com/watch?v=FYz0rAxQkC8&list=PLdo4fOcmZ0oX0ObHwBrJ0vJpZ7PiYMqeA)

## Keeping up-to-date with provider changes

Starting with work after the 2.1 release, we have created a [log of changes](xref:core/providers/provider-log) that may need corresponding changes in provider code. This is intended to help when updating an existing provider to work with a new version of EF Core.

Prior to 2.1, we used the [`providers-beware`](https://github.com/dotnet/efcore/labels/providers-beware) and [`providers-fyi`](https://github.com/dotnet/efcore/labels/providers-fyi) labels on our GitHub issues and pull requests for a similar purpose. We will continue to use these labels on issues to give an indication which work items in a given release may also require work to be done in providers. A `providers-beware` label typically means that the implementation of a work item may break providers, while a `providers-fyi` label typically means that providers will not be broken, but code may need to be changed anyway, for example, to enable new functionality.

## Suggested naming of third party providers

We suggest using the following naming for NuGet packages. This is consistent with the names of packages delivered by the EF Core team.

`<Optional project/company name>.EntityFrameworkCore.<Database engine name>`

For example:

* `Microsoft.EntityFrameworkCore.SqlServer`
* `Npgsql.EntityFrameworkCore.PostgreSQL`
* `EntityFrameworkCore.SqlServerCompact40`

## The EF Core specification tests

EF Core provides a specification test suite project, which all providers are encouraged to implement. The project contains tests which ensure that the provider function correctly, e.g. by executing various LINQ queries and ensuring that the correct results are returned. This test suite is used by EF Core's own providers (such as SQL Server, SQLite, and Azure Cosmos DB) as the primary regression testing mechanism, and are continuously updated and improved as new features are added to EF Core. By implementing these tests for other, 3rd-party providers, you can ensure that your database provider works correctly and implements all the latest EF Core features. Note that the test suite is quite large, as it covers the entire EF Core feature set; you don't have to implement everything - it's perfectly fine to cherry-pick certain test classes, and incrementally improve your coverage with time.

To start using the specification tests, follow these steps:

* Create an xUnit test project in your provider solution. We suggest the name `<Provider>.FunctionalTests` for consistency, so if your provider is called `AcmeSoftware.EntityFramework.AcmeDb`, call the test project `AcmeSoftware.EntityFramework.AcmeDb.FunctionalTests`.
* From your new test project, reference the EF specification tests, which are published as regular Nuget packages. For relational providers, the specification test suite is [Microsoft.EntityFrameworkCore.Relational.Specification.Tests](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Relational.Specification.Tests), for non-relational provider, use [Microsoft.EntityFrameworkCore.Specification.Tests](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Specification.Tests)).
* Pick a test class from the EF specification tests, and extend it from a corresponding test class in your own project. The available test classes can be seen in the [EF source code](https://github.com/dotnet/efcore/tree/main/test/EFCore.Relational.Specification.Tests). Your class should be named based on the EF test class, with your provider name inserted where appropriate. For example, `NorthwindWhereQueryRelationalTestBase` (which is a good place to start), would be extended by `NorthwindWhereQueryAcmeDbTest`.
* At the very beginning, you'll have a bit of test infrastructure to implement - once that's done once, things will become easier. For example, for `NorthwindWhereQueryAcmeDbTest` you'll have to implement `NorthwindWhereQueryAcmeDbFixture`, which will require a `AcmeDbNorthwindTestStoreFactory`, which would need a Northwind.sql script to seed the AcmeDb version of the Northwind database. We strongly suggest keeping another EF Core provider's test suite open nearby, and following what it does. For example, the SQL Server implementation of the specification tests is visible [here](https://github.com/dotnet/efcore/tree/main/test/EFCore.SqlServer.FunctionalTests).
* Once the infrastructure for the test class is done, you'll start seeing some green tests on it. You can investigate the failing tests, or temporarily skip them for later investigation. In this way you can add more and more test classes.
* At some point, when you've extended most of the upstream test classes, you can also create `AcmeDbComplianceTest`, which extends `RelationalComplianceTestBase`. This test class will fail if your own test project doesn't extend an EF Core test class - it's a great way to know whether your test suite is complete, and also whether EF added a new test class in a new version. You can also opt out of extending specific test classes if they're not ready (or not relevant).

### SQL assertions

When implementing the specification tests, you have the option of additionally asserting the SQL that EF Core produces. This isn't mandatory: the specification test implementation already checks that your provider returned the expected rows from the database, so if it passes, your provider is likely doing the right thing. However, asserting that the SQL is what you expect it to be can add additional coverage in some cases, especially where some SQL construct is being used that's specific to your database.

For example, here's the `Where_simple` test in [NorthwindWhereQuerySqlServerTest](https://github.com/dotnet/efcore/blob/main/test/EFCore.SqlServer.FunctionalTests/Query/NorthwindWhereQuerySqlServerTest.cs) test class:

```c#
public override async Task Where_simple(bool async)
{
    await base.Where_simple(async);

    AssertSql(
        @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c]
WHERE [c].[City] = N'London'");
}
```

The base invocation runs the specification test, which executes the LINQ query and verifies that the results are correct. In addition, `AssertSql` ensures that the SQL matched the baseline included in the test.

#### Inspecting SQL assertion failures

If a SQL assertion fails, xunit typically reports only the fragment of the SQL which did not match. To see the full SQL produced by your provider, you can have the test infrastructure output the complete new baseline when a test fails, so that you can inspect it and possibly replace the old one. To do this, pass the `ITestOutputHelper` provided by xunit to the `TestSqlLoggerFactory` of the test fixture:

```c#
public NorthwindWhereQuerySqlServerTest(
    NorthwindQuerySqlServerFixture<NoopModelCustomizer> fixture,
    ITestOutputHelper testOutputHelper)
    : base(fixture)
{
    ClearLog();
    Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
}
```

In the EF Core test suites, we usually keep the above line commented out in the constructor, so that we can easily uncomment it any time a SQL assertion fails.

#### Bulk update of baselines

If you use SQL assertions a lot, you'll have many SQL baselines in your test suite. In some cases, a small change - either in your provider or in EF Core itself - may cause a large number of these assertions to fail for some reason, e.g. parentheses were added somewhere. If that happens, manually correcting all the affected baselines can be a very tedious and time-consuming process.

The EF Core test infrastructure has a feature which automatically updates all failing baselines with the new ones. Simply set the `EF_TEST_REWRITE_BASELINES` environment variable to `1` and run the tests, and the test source files should get updated. It's recommend to commit before doing this, and then use git to inspect the test diff, to make sure the new baselines make sense. Once you're satisfied, you can commit those changes as well.
