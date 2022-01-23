---
title: Choosing a testing strategy - EF Core
description: Different approaches to testing applications that use Entity Framework Core
author: roji
ms.date: 11/07/2021
uid: core/testing/choosing-a-testing-strategy
---
# Choosing a testing strategy

As discussed in the [Overview](xref:core/testing/index), we'll refer to tests which involve your production database system as *integration tests*, and tests which employ [test doubles](https://martinfowler.com/bliki/TestDouble.html) as *unit tests*.

Integration tests frequently involve the following difficulties:

1. In many cases, it's simply not possible or practical to test against the actual external dependency. For example, your application may interact with some service that cannot be easily tested against (because of rate limiting, or the lack of a testing environment).
2. Even when it's possible to involve the real dependency, this may be exceedingly slow: running a large amount of tests against a cloud service may cause tests to take too long. Testing should be part of the developer's everyday workflow, so it's important that tests run quickly.
3. Executing tests against an external resource may involve isolation issues, where tests interfere with one another. For example, multiple tests running in parallel against a database may modify data and cause each other to fail in various ways. Since unit tests run in-memory and don't use shared resources, they're isolated from each other and don't cause interference.

However, unit tests do not guarantee that your program works correctly as a whole; while each component may work well in isolation, various issues may arise when integrating them together. For example, a component may execute some query against a database, expecting certain behavior, while the actual database system behaves differently. Such issues are only uncovered in integration testing, making them an important part of any testing strategy. In addition, since unit tests interact with internal components, they must be reworked as the program is refactored and internal APIs change; integration tests, in contrast, are more stable as they test public, external API contracts.

## Integration testing

Because of the above difficulties with integration tests, developers are frequently urged to concentrate on unit tests first, and have a robust unit test suite which they can run frequently on their machines; integration tests, in contrast, are supposed to be executed much less frequently, and in many cases also provide far less coverage. We recommend giving more thought to integration tests, and suggest that databases may actually be far less affected by the above problems than people tend to think:

1. Most databases can nowadays be easily installed on the developer's machine. Container-based technologies such as Docker can make this very easy, and technologies such as [Github Workspaces](https://docs.github.com/en/codespaces/overview) and [Dev Container](https://code.visualstudio.com/docs/remote/create-dev-container) set up your entire development environment for you (including the database). When using SQL Server, it's also possible to test against [LocalDB](/sql/database-engine/configure-windows/sql-server-express-localdb) on Windows, or easily set up a Docker image on Linux.
2. Testing against a local database - with a reasonable test dataset - is usually extremely fast: communication is completely local, and test data is typically buffered in memory on the database side. EF Core itself contains over 30,000 tests against SQL Server alone; these complete reliably in a few minutes, execute in CI on every single commit, and are very frequently executed by developers locally. Some developers turn to an in-memory database (a "fake") in the belief that this is needed for speed - this is almost never actually the case.
3. Isolation is indeed a hurdle when running tests against a real database, as tests may modify data and interfere with one another. However, there are various techniques to provide isolation in database testing scenarios; we concentrate on these in the [integration testing docs](xref:core/testing/integration-testing)).

The above is not meant to disparage unit testing or to argue against writing them. For one thing, unit tests are necessary for some scenarios which cannot be tested with integration tests, such as simulating database failure. However, in our experience, users frequently shy away from integration testing for the above reasons, believing it's slow, hard or unreliable, when that isn't necessarily the case. [The dedicated page on integration testing](xref:core/testing/integration-testing) aims to address this, providing guidelines and samples for writing fast, isolated database integration tests.

## Unit testing

Unlike integration tests, unit tests never involve the actual database system used in production, but rather swap it with a [test double](https://en.wikipedia.org/wiki/Test_double). Below are some common methods for unit testing EF Core applications with test doubles:

1. Use SQLite (in-memory mode) as a database fake, replacing your production database system.
2. Use the EF Core InMemory provider as a database fake, replacing your production database system.
3. Mock or stub out `DbContext` and `DbSet`.
4. Introduce a repository layer between EF Core and your application code, and mock or stub that layer.

Below, we'll explore what each method means, and compare it with the others. We recommend reading through the different methods to gain a full understanding, but if you're looking for a quick suggestion, we highly recommend the 4th method: introducing a repository layer.

### SQLite as a database fake

One possible unit testing approach is to swap your production database (e.g. SQL Server) with SQLite, effectively using it as a testing "fake". Aside from ease of setup, SQLite has a an [in-memory database](https://sqlite.org/inmemorydb.html) feature which is especially useful for unit testing: each test is naturally isolated in its own database, and no actual files need to be managed.

However, before doing this, it's important to understand that in EF Core, different database providers behave differently - EF Core does not attempt to abstract every aspect of the underlying database system. Fundamentally, this means that testing against SQLite does not guarantee the same results as against SQL Server, or any other database. Here are some examples of possible behavioral differences:

* The same LINQ query may return different results on different providers. For example, SQL Server does case-insensitive string comparison by default, whereas SQLite is case-sensitive. This can make your unit tests pass where a test against your actual database would fail (or vice versa).
* Some queries which work on SQL Server simply aren't supported on SQLite, because the exact SQL support in these two database differs.
* If your query happens to use a provider-specific method such as SQL Server's [`EF.Functions.DateDiffDay`](xref:core/providers/sql-server/functions#date-and-time-functions), that query will fail on SQLite, and cannot be tested.
* Raw SQL may work, or it may fail or return different results, depending on exactly what is being done. SQL dialects are different in many ways across databases.

Compared to running integration tests against your production database system, it's relatively easy to get started with SQLite, and so many users do. Unfortunately, the above limitations tend to eventually become problematic when unit testing EF Core applications, even if they don't seem to be at the beginning. As a result, we recommend either concentrating on integration tests, or implementing unit tests via the repository pattern as discussed below.

For information on how to use SQLite for unit testing, see the [unit testing page](xref:core/testing/unit-testing#sqlite-in-memory).

### InMemory as a database fake

As an alternative to SQLite, EF Core also comes with an InMemory provider. Although this provider was originally designed to support internal testing of EF Core itself, some developers use it as a database fake when unit testing EF Core using EF Core. Doing so is **highly discouraged**: as a database fake, InMemory has the same issues as SQLite (see above), but in addition has the following additional limitations:

* The InMemory provider generally supports less query types than the SQLite provider, since it isn't a relational database. More queries will fail or behave differently in comparison to your production database.
* Transactions are not supported.
* Raw SQL is completely unsupported. Compare this with SQLite, where it's possible to use raw SQL, as long as that SQL works in the same way on SQLite and your production database.
* The InMemory provider has not been optimized for performance, and will generally work slower than SQLite in in-memory mode.

In summary, InMemory has all the disadvantages of SQLite, along with a few more - and offers no advantages in return. If you are looking for a simple, in-memory database fake, use SQLite instead of the InMemory provider; but consider using the repository pattern instead as described below.

For information on how to use InMemory for unit testing, see the [unit testing page](xref:core/testing/unit-testing#inmemory).

### Mocking or stubbing DbContext and DbSet

This approach typically uses a mock framework to create a test double of `DbContext` and `DbSet`, and tests against those doubles. Mocking `DbContext` can be a good approach for testing various *non-query* functionality, such as calls to <xref:Microsoft.EntityFrameworkCore.DbContext.Add%2A> or <xref:Microsoft.EntityFrameworkCore.DbContext.SaveChanges>, allowing you to verify that your code called them in write scenarios.

However, properly mocking `DbSet` *query* functionality is not possible, since queries are expressed via LINQ operators, which are static extension method calls over `IQueryable`. As a result, when some people talk about "mocking `DbSet`", what they really mean is that they stub `DbSet` out with an in-memory collection, and then evaluate query operators against that collection in memory, just like a simple `IEnumerable`. This isn't the same as actually mocking or stubbing the database, which would involve stubbing out query **end results**, rather than query inputs (i.e. the database table, as represented by a `DbSet`).

Since only the `DbSet` itself is stubbed and the query is evaluated in-memory, this approach ends up being very similar to using the EF Core InMemory provider: both techniques execute query operators in .NET over an in-memory collection. As a result, this technique suffers from the same drawbacks as well: queries will behave differently (e.g. around case sensitivity) or will simply fail (e.g. because of provider-specific methods), raw SQL won't work and transactions will be ignored at best. As a result, this technique should generally be avoided for testing any query code.

### Repository pattern

The approaches above attempted to either swap EF Core's production database provider with a fake testing provider, or to stub out `DbSet` itself with an in-memory collection. These techniques are similar in that they evaluate the program's LINQ queries - either in SQLite or in memory - and this is ultimately the source of the difficulties outlined above: a query designed to run against a specific production database cannot run elsewhere without issues.

For proper, reliable unit testing, consider introducing a [repository layer](https://martinfowler.com/eaaCatalog/repository.html) which mediates between your application code and EF Core. The production implementation of the repository contains the actual LINQ queries and executes them via EF Core. In testing, the repository abstraction is directly stubbed or mocked without needing any actual LINQ queries, effectively removing EF Core from your testing stack altogether and allowing tests to focus on application code alone.

The following diagram compares the database fake pattern (SQLite/InMemory) with the repository pattern:

![Comparison of fake provider with repository pattern](_static/fake-provider-and-repository-pattern.png)

Since LINQ queries are no longer part of testing, you can directly provide query results to your application. Put another way, the previous approaches roughly allow stubbing out *query inputs* (e.g. replacing SQL Server tables with an in-memory one), but then still execute the actual query operators in-memory; the repository pattern, in contrast, allows you to stub out *query outputs* directly, allowing for far more powerful and focused unit testing.

One possible disadvantage of the repository pattern is that it requires you to architect your application accordingly: application code can no longer execute LINQ queries directly, but must instead call into a repository method, which executes the query (and is stubbed in testing). This can make for a somewhat heavier application architecture, but can also have other advantages: all data access code is concentrated in one place rather than being spread around the application, and if your application needs to support more than one database, the repository abstraction can be very helpful for tweaking queries across providers.

For an example of a unit testing with a repository, see the [unit testing page](xref:core/testing/unit-testing#repository-pattern).

## Overall comparison

The following table provides a quick, comparative view of the different testing techniques, and shows which functionality can be tested under which approach:

Feature                             | InMemory     | SQLite in-memory          | Mock DbContext | Repository pattern | Integration testing
----------------------------------- | ------------ | ------------------------- | -------------- | ------------------ | -------------------
Test double type                    | Fake         | Fake                      | Mock/stub      | Mock/stub          | Real, no double
Raw SQL?                            | No           | Depends                   | No             | Yes                | Yes
Transactions?                       | No (ignored) | Yes                       | Yes            | Yes                | Yes
Provider-specific translations?     | No           | No                        | No             | Yes                | Yes
Exact query behavior?               | Depends      | Depends                   | Depends        | Yes                | Yes
Requires special code architecture? | No           | No                        | No             | Yes                | No

## Summary

* We highly recommend that developers have good test coverage of their application running against their actual production database system. This provides confidence that the application actually works in production, and with proper design, tests can execute reliably and quickly. Since integration tests are required in any case, it's a good idea to start there, and add unit tests later, based on need.
* For unit testing, we recommend implementing the repository pattern, allowing you to properly stub or mock out your data access layer above EF Core, rather than via a fake EF Core provider (Sqlite/InMemory) or by mocking `DbSet`.
* If the repository pattern isn't a viable option for some reason, consider using SQLite in-memory databases.
* Avoid the InMemory provider for testing purposes - this is discouraged and only supported for legacy applications.
* Avoid mocking `DbSet` for querying purposes.
