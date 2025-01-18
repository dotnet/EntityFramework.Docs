---
title: Overview of Entity Framework 6 - EF6
description: Overview of Entity Framework 6
author: SamMonoRT
ms.date: 10/23/2016
uid: ef6/index
---
# Entity Framework 6
Entity Framework 6 (EF6) is a tried and tested object-relational mapper (O/RM) for .NET with many years of feature development and stabilization.

As an O/RM, EF6 reduces the impedance mismatch between the relational and object-oriented worlds, enabling developers to write applications that interact with data stored in relational databases using strongly-typed .NET objects that represent the application's domain, and eliminating the need for a large portion of the data access "plumbing" code that they usually need to write.

EF6 implements many popular O/RM features:
- Mapping of [POCO](xref:ef6/resources/glossary#poco) entity classes which do not depend on any EF types
- Automatic change tracking
- Identity resolution and Unit of Work
- Eager, lazy and explicit loading
- Translation of strongly-typed queries using [LINQ (Language INtegrated Query)](https://aka.ms/AA6hsvu)
- Rich mapping capabilities, including support for:
  - One-to-one, one-to-many and many-to-many relationships
  - Inheritance (table per hierarchy, table per type and table per concrete class)
  - Complex types
  - Stored procedures
- A visual designer to create entity models.
- A "Code First" experience to create entity models by writing code.
- Models can either be generated from existing databases and then hand-edited, or they can be created from scratch and then used to generate new databases.
- Integration with .NET Framework application models, including ASP.NET, and through databinding, with WPF and WinForms.
- Database connectivity based on ADO.NET and numerous [providers](xref:ef6/fundamentals/providers/index) available to connect to SQL Server, Oracle, MySQL, SQLite, PostgreSQL, DB2, etc.

## Should I use EF6 or EF Core?

EF Core is a more modern, lightweight and extensible version of Entity Framework that has very similar capabilities and benefits to EF6.
EF Core is a complete rewrite and contains many new features not available in EF6, although it also still lacks some of the most advanced mapping capabilities of EF6.
Consider using EF Core in new applications if the feature set matches your requirements.
[Compare EF Core & EF6](xref:efcore-and-ef6/index) examines this choice in greater detail.

## [Get Started](xref:ef6/get-started)

Add the EntityFramework NuGet package to your project or install the [Entity Framework Tools for Visual Studio](https://aka.ms/AA6i8c5). Then watch videos, read tutorials, and advanced documentation to help you make the most of EF6.

## Past Entity Framework Versions

This is the documentation for the latest version of Entity Framework 6, although much of it also applies to past releases.
Check out [What's New](xref:ef6/what-is-new/index) and [Past Releases](xref:ef6/what-is-new/past-releases) for a complete list of EF releases and the features they introduced.
