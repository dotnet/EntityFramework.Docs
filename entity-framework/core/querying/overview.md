---
title: How Query Works
author: rowanmiller
ms.author: rowmil
manager: rowanmiller
ms.date: 10/27/2016
ms.topic: article
ms.assetid: de2e34cd-659b-4cab-b5ed-7a979c6bf120
ms.prod: entity-framework
uid: core/querying/overview
---
# How Query Works

> [!NOTE]
> This documentation is for EF Core. For EF6.x, see [Entity Framework 6](../../ef6/index.md).

Entity Framework Core uses Language Integrate Query (LINQ) to query data from the database. LINQ allows you to use C# (or your .NET language of choice) to write strongly typed queries based on your derived context and entity classes.

## The life of a query

The following is a high level overview of the process each query goes through.

1. The LINQ query is processed by Entity Framework Core to build a representation that is ready to be processed by the database provider

   1. The result is cached so that this processing does not need to be done every time the query is executed

2. The result is passed to the database provider

   1. The database provider identifies which parts of the query can be evaluated in the database

   2. These parts of the query are translated to database specific query language (e.g. SQL for a relational database)

   3. One or more queries are sent to the database and the result set returned (results are values from the database, not entity instances)

3. For each item in the result set

   1. If this is a tracking query, EF checks if the data represents an entity already in the change tracker for the context instance

      * If so, the existing entity is returned

      * If not, a new entity is created, change tracking is setup, and the new entity is returned

   2. If this is a no-tracking query, EF checks if the data represents an entity already in the result set for this query

      * If so, the existing entity is returned

      * If not, a new entity is created and returned

## When queries are executed

When you call LINQ operators, you are simply building up an in-memory representation of the query. The query is only sent to the database when the results are consumed.

The most common operations that result in the query being sent to the database are:
* Iterating the results in a `for` loop

* Using an operator such as `ToList`, `ToArray`, `Single`, `Count`

* Databinding the results of a query to a UI
