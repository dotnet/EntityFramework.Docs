---
title: Overview of testing applications that use EF Core - EF Core
description: Overview of testing applications that use Entity Framework Core
author: roji
ms.date: 01/17/2021
uid: core/testing/index
---
# Testing EF Core Applications

Testing is an important concern to almost all application types - it allows you to be sure your application works correctly, and makes it instantly known if its behavior regresses in the future. Since testing may affect how your code is architected, it's highly recommended to plan for testing early and to ensure good coverage as your application evolves. This introductory section provides a quick overview of various testing strategies for applications using EF Core.

## Involving the database (or not)

When writing tests for your EF Core application, one basic decision you need to make is whether your tests will involve your production database system - just as your application does - or whether your tests will run against a [test double](https://martinfowler.com/bliki/TestDouble.html), which replaces your production database system. Two prominent examples of test doubles in the EF Core context are [SQLite in-memory mode](xref:core/testing/choosing-a-testing-strategy#sqlite-as-a-database-fake), and the [in-memory provider](xref:core/testing/choosing-a-testing-strategy#inmemory-as-a-database-fake).

For an in-depth comparison and analysis of the different approaches, see [Choosing a testing strategy](xref:core/testing/choosing-a-testing-strategy). Below is a short point-by-point summary to help you get up to speed with the different options:

* Developers frequently avoid testing against their production database system because they believe this is difficult or slow. This isn't always true in our experience, and we suggest giving this approach a chance: [Testing against your production database system](xref:core/testing/testing-with-the-database) provides techniques for doing this reliably and efficiently. Writing at least some tests against your database is usually necessary in any case - to make sure your application actually works against your production database - and tests not involving the database can be limited in what they allow you to test (see below).
* The [in-memory provider](xref:core/testing/choosing-a-testing-strategy#inmemory-as-a-database-fake) will not behave like your real database in many important ways. Some features cannot be tested with it at all (e.g. transactions, raw SQL..), while other features may behave differently than your production database (e.g. case-sensitivity in queries). While in-memory can work for simple, constrained query scenarios, it is highly limited and we discourage its use.
  * Mocking `DbSet` for querying is complex and difficult, and suffers from the same disadvantages as the in-memory approach; we discourage this as well.
* [SQLite in-memory mode](xref:core/testing/choosing-a-testing-strategy#sqlite-as-a-database-fake) offers better compatibility with production relational databases, since SQLite is itself a full-fledged relational database. However, there will still be some important discrepancies between SQLite and your production database, and some features cannot be tested at all (e.g. provider-specific methods on EF.Functions).
* For a testing approach that allows you to use a reliable test double for all the functionality of your production database system, it's possible to introduce a [repository layer](xref:core/testing/choosing-a-testing-strategy#repository-pattern) in your application. This allows you to exclude EF Core entirely from testing and to fully mock the repository; however, this alters the architecture of your application in a way which could be significant, and involves more implementation and maintenance costs.

## Further reading

For more in-depth information, see [Choosing a testing strategy](xref:core/testing/choosing-a-testing-strategy). For implementation guidelines and code samples, see [Testing against your production database system](xref:core/testing/testing-with-the-database) and [Testing without your production database system](xref:core/testing/testing-without-the-database).
