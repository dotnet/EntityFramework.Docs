---
title: Testing with the EF In-Memory Database - EF Core
description: Using the EF in-memory database to test an Entity Framework Core application
author: ajcvickers
ms.date: 10/27/2016
uid: core/testing/in-memory
---

# Testing with the EF In-Memory Database

> [!WARNING]
> The EF in-memory database often behaves differently than relational databases.
> Only use the EF in-memory database after fully understanding the issues and trade-offs involved, as discussed in [Testing code that uses EF Core](xref:core/testing/index).

> [!TIP]
> SQLite is a relational provider and can also use in-memory databases.
> Consider using this for testing to more closely match common relational database behaviors.
> This is covered in [Using SQLite to test an EF Core application](xref:core/testing/sqlite).

The information on this page now lives in other locations:

* See [Testing code that uses EF Core](xref:core/testing/index) for general information on testing with the EF in-memory database.
* See [Sample showing how to test applications that use EF Core](xref:core/testing/testing-sample) for a sample using the EF in-memory database.
* See [The EF in-memory database provider](xref:core/providers/in-memory/index) for general information about the EF in-memory database.
