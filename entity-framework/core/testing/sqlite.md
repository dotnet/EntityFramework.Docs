---
title: Testing with SQLite - EF Core
description: Using SQLite to test an Entity Framework Core application
author: ajcvickers
ms.date: 04/24/2020
uid: core/testing/sqlite
---

# Using SQLite to test an EF Core application

> [!WARNING]
> Using SQLite can be an effective way to test an EF Core application.
> However, problems can arise where SQLite behaves differently from other database systems.
> See [Testing code that uses EF Core](xref:core/testing/index) for a discussion of the issues and trade-offs.

This document builds uses on the concepts introduced in [Sample showing how to test applications that use EF Core](xref:core/testing/testing-sample).
The code examples shown here come from this sample.

## Using SQLite in-memory databases

Normally, SQLite creates databases as simple files and accesses the file in-process with your application.
This is very fast, especially when using a fast [SSD](https://en.wikipedia.org/wiki/Solid-state_drive).

SQLite can also use databases created purely in-memory.
This is easy to use with EF Core as long as you understand the in-memory database lifetime:

* The database is created when the connection to it is opened
* The database is deleted when the connection to it is closed

EF Core will use an already open connection when given one, and will never attempt to close it.
So the key to using EF Core with an in-memory SQLite database is to open the connection before passing it to EF.

The [sample](xref:core/testing/testing-sample) achieves this with the following code:

[!code-csharp[SqliteInMemory](../../../samples/core/Miscellaneous/Testing/ItemsWebApi/Tests/SqliteInMemoryItemsControllerTest.cs?name=SqliteInMemory)]

Notice:

* The `CreateInMemoryDatabase` method creates a SQLite in-memory database and opens the connection to it.
* The created `DbConnection` is extracted from the `ContextOptions` and saved.
* The connection is disposed when the test is disposed so that resources are not leaked.

> [!NOTE]
> [Issue #16103](https://github.com/dotnet/efcore/issues/16103) is tracking ways to make this connection management easier.
