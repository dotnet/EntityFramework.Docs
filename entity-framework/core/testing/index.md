---
title: Testing code that uses EF Core - EF Core
description: Different approaches to testing applications that use Entity Framework Core
author: ajcvickers
ms.date: 04/22/2020
uid: core/testing/index
---
# Testing code that uses EF Core

Testing code that accesses a database requires either:

* Running queries and updates against the same database system used in production.
* Running queries and updates against some other easier to manage database system.
* Using test doubles or some other mechanism to avoid using a database at all.

This document outlines the trade-offs involved in each of these choices and shows how EF Core can be used with each approach.

> [!TIP]
> See [EF Core testing sample](xref:core/testing/testing-sample) for code demonstrating the concepts introduced here.

## All database providers are not equal

It is very important to understand that EF Core is not designed to abstract every aspect of the underlying database system.
Instead, EF Core is a common set of patterns and concepts that can be used with any database system.
EF Core database providers then layer database-specific behavior and functionality over this common framework.
This allows each database system to do what it does best while still maintaining commonality, where appropriate, with other database systems.

Fundamentally, this means that switching out the database provider will change EF Core behavior and the application can't be expected to function correctly unless it explicitly accounts for any differences in behavior.
That being said, in many cases doing this will work because there is a high degree of commonality amongst relational databases.
This is good and bad.
Good because moving between database systems can be relatively easy.
Bad because it can give a false sense of security if the application is not fully tested against the new database system.

## Approach 1: Production database system

As described in the previous section, the only way to be sure you are testing what runs in production is to use the same database system.
For example, if the deployed application uses SQL Azure, then testing should also be done against SQL Azure.

However, having every developer run tests against SQL Azure while actively working on the code would be both slow and expensive.
This illustrates the main trade-off involved throughout these approaches: when is it appropriate to deviate from the production database system so as to improve test efficiency?

Luckily, in this case the answer is quite easy: use local or on-premises SQL Server for developer testing.
SQL Azure and SQL Server are extremely similar, so testing against SQL Server is usually a reasonable trade-off.
That being said, it is still wise to run tests against SQL Azure itself before going into production.

### LocalDB

All the major database systems have some form of "Developer Edition" for local testing.
SQL Server also has a feature called [LocalDB](/sql/database-engine/configure-windows/sql-server-express-localdb).
The primary advantage of LocalDB is that it spins up the database instance on demand.
This avoids having a database service running on your machine even when you're not running tests.

LocalDB is not without its issues:

* It doesn't support everything that [SQL Server Developer Edition](/sql/sql-server/editions-and-components-of-sql-server-version-15?view=sql-server-ver15&preserve-view=true) does.
* It isn't available on Linux.
* It can cause lag on first test run as the service is spun up.

Personally, I've never found it a problem having a database service running on my dev machine and I would generally recommend using Developer Edition instead.
However, LocalDB may be appropriate for some people, especially on less powerful dev machines.

[Running SQL Server](/sql/linux/quickstart-install-connect-docker) (or any other database system) in a Docker container (or similar) is another way to avoid running the database system directly on your development machine.

## Approach 2: SQLite

EF Core tests the SQL Server provider primarily by running it against a local SQL Server instance.
These tests run tens of thousands of queries in a couple of minutes on a fast machine.
This illustrates that using the real database system can be a performant solution.
It is a myth that using some lighter-weight database is the only way to run tests quickly.

That being said, what if for whatever reason you can't run tests against something close to your production database system?
The next best choice is to use something with similar functionality.
This usually means another relational database, for which [SQLite](https://sqlite.org/index.html) is the obvious choice.

SQLite is a good choice because:

* It runs in-process with your application and so has low overhead.
* It uses simple, automatically created files for databases, and so doesn't require database management.
* It has an in-memory mode that avoids even the file creation.

However, remember that:

* SQLite inevitably doesn't support everything that your production database system does.
* SQLite will behave differently than your production database system for some queries.

So if you do use SQLite for some testing, make sure to also test against your real database system.

See [Testing with SQLite](xref:core/testing/sqlite) for EF Core specific guidance.

## Approach 3: The EF Core in-memory database

EF Core comes with an in-memory database that we use for internal testing of EF Core itself.
This database is in general **not suitable for testing applications that use EF Core**. Specifically:

* It is not a relational database.
* It doesn't support transactions.
* It cannot run raw SQL queries.
* It is not optimized for performance.

None of this is very important when testing EF Core internals because we use it specifically where the database is irrelevant to the test.
On the other hand, these things tend to be very important when testing an application that uses EF Core.

## Unit testing

Consider testing a piece of business logic that might need to use some data from a database, but is not inherently testing the database interactions.
One option is to use a [test double](https://en.wikipedia.org/wiki/Test_double) such as a mock or fake.

We use test doubles for internal testing of EF Core.
However, we never try to mock DbContext or IQueryable.
Doing so is difficult, cumbersome, and fragile.
**Don't do it.**

Instead we use the EF in-memory database when unit testing something that uses DbContext.
In this case using the EF in-memory database is appropriate because the test is not dependent on database behavior.
Just don't do this to test actual database queries or updates.

The [EF Core testing sample](xref:core/testing/testing-sample) demonstrates tests using the EF in-memory database, as well as SQL Server and SQLite.
