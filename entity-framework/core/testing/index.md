---
title: Overview of testing applications that use EF Core - EF Core
description: Overview of testing applications that use Entity Framework Core
author: roji
ms.date: 01/17/2021
uid: core/testing/index
---
# Testing EF Core Applications

Testing is an important concern to almost all application types - it allows you to be sure your application works correctly, and makes it instantly known if its behavior regresses in the future. Since testing may affect how your code is architected, it's highly recommended to plan for a testing early and to ensure good coverage as your application evolves. This introductory section provides a quick overview of various testing strategies for applications using EF Core.

## Involving the database (or not)

When writing tests for your EF Core application, one basic decision you need to make is whether your tests will involve your production database system - just as your application does - or whether your tests will run against a [test double](https://martinfowler.com/bliki/TestDouble.html), which replaces your production database system. Two prominent examples of test doubles in the EF Core context are [SQLite in-memory mode](xref:core/testing/choosing-a-testing-strategy#sqlite-as-a-database-fake), and the [InMemory provider](xref:core/testing/choosing-a-testing-strategy#inmemory-as-a-database-fake). For simplicity's sake, we'll use the term *integration tests* to refer to tests which involve your production database systems, and *unit tests* to refer to tests which use a test double.

For an in-depth comparison and analysis of integration and unit testing approaches in EF Core applications, see [Choosing a testing strategy](xref:core/testing/choosing-a-testing-strategy). Below is a short point-by-point summary to help you get up to speed with the different options:

* Developers frequently turn to unit testing because they believe testing against their production database system is difficult or slow. This isn't always true in our experience, and we suggest giving this approach a chance: [Integration testing](xref:core/testing/integration-testing) provides techniques for reliable, fast integration testing. Writing at least some integration tests is usually necessary in any case - to make sure your application actually works against your production database - and unit testing can be limited in what it allow you to test (see below).
* The [InMemory provider](xref:core/testing/choosing-a-testing-strategy#inmemory-as-a-database-fake) will not behave like your real database in many important ways. Some features cannot be tested with it at all (e.g. transactions, raw SQL..), some others may behave differently than your production database (e.g. case-sensitivity in queries). While InMemory can work for simple, constrained query scenarios, it is highly limited and we discourage its use.
  * Mocking `DbSet` for querying is complex and difficult, and suffers from the same disadvantages as the InMemory approach; we discourage this as well.
* [SQLite in-memory mode](xref:core/testing/choosing-a-testing-strategy#sqlite-as-a-database-fake) offers better compatibility with production relational databases, since SQLite is itself a full-fledged relational database. However, there will still be some important discrepancies between SQLite and your production database, and some features cannot be tested at all (e.g. provider-specific methods on EF.Functions).
* For a unit testing approach that allows you to use a reliable test double for all the functionality of your production database system, it's possible to introduce a [repository layer](xref:core/testing/choosing-a-testing-strategy#repository-pattern) in your application. This allows you to exclude EF Core entirely from testing and to fully mock the repository; however, this alters the architecture of your application in a way which could be significant.

## Further reading

For more in-depth information, see [Choosing a testing strategy](xref:core/testing/choosing-a-testing-strategy). For implementation guidelines and code samples, see [Integration testing](xref:core/testing/integration-testing) and [Unit testing](xref:core/testing/unit-testing).
