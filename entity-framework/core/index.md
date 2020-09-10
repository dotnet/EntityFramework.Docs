---
title: Overview of Entity Framework Core - EF Core
description: General introductory overiew of Entity Framework Core
author: rowanmiller
ms.date: 10/27/2016
uid: core/index
---

# Entity Framework Core

Entity Framework (EF) Core is a lightweight, extensible, [open source](https://github.com/aspnet/EntityFrameworkCore) and cross-platform version of the popular Entity Framework data access technology.

EF Core can serve as an object-relational mapper (O/RM), enabling .NET developers to work with a database using .NET objects, and eliminating the need for most of the data-access code they usually need to write.

EF Core supports many database engines, see [Database Providers](xref:core/providers/index) for details.

## The Model

With EF Core, data access is performed using a model. A model is made up of entity classes and a context object that represents a session with the database, allowing you to query and save data. See [Creating a Model](xref:core/modeling/index) to learn more.

You can generate a model from an existing database, hand code a model to match your database, or use [EF Migrations](xref:core/managing-schemas/migrations/index) to create a database from your model, and then evolve it as your model changes over time.

[!code-csharp[Main](../../samples/core/Intro/Model.cs)]

## Querying

Instances of your entity classes are retrieved from the database using Language Integrated Query (LINQ). See [Querying Data](xref:core/querying/index) to learn more.

[!code-csharp[Main](../../samples/core/Intro/Program.cs#Querying)]

## Saving Data

Data is created, deleted, and modified in the database using instances of your entity classes. See [Saving Data](xref:core/saving/index) to learn more.

[!code-csharp[Main](../../samples/core/Intro/Program.cs#SavingData)]

## Next steps

For introductory tutorials, see [Getting Started with Entity Framework Core](xref:core/get-started/index).
