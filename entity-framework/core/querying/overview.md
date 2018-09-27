---
title: How Queries Work - EF Core
author: rowanmiller
ms.date: 09/26/2018
ms.assetid: de2e34cd-659b-4cab-b5ed-7a979c6bf120
uid: core/querying/overview
---

# How Queries Work

Entity Framework Core uses Language Integrated Query (LINQ) to query data from the database. LINQ allows you to use C# (or your .NET language of choice) to write strongly typed queries based on your derived context and entity classes.

## The life of a query

The following is a high level overview of the process each query goes through.

1. The LINQ query is processed by Entity Framework Core to build a representation that is ready to be processed by the database provider
   1. The result is cached so that this processing does not need to be done every time the query is executed
2. The result is passed to the database provider
   1. The database provider identifies which parts of the query can be evaluated in the database
   2. These parts of the query are translated to database specific query language (for example, SQL for a relational database)
   3. One or more queries are sent to the database and the result set returned (results are values from the database, not entity instances)
3. For each item in the result set
   1. If this is a tracking query, EF checks if the data represents an entity already in the change tracker for the context instance
      * If so, the existing entity is returned
      * If not, a new entity is created, change tracking is setup, and the new entity is returned
   2. If this is a no-tracking query, EF checks if the data represents an entity already in the result set for this query
      * If so, the existing entity is returned <sup>(1)</sup>
      * If not, a new entity is created and returned

<sup>(1)</sup> No tracking queries use weak references to keep track of entities that have already been returned. If a previous result with the same identity goes out of scope, and garbage collection runs, you may get a new entity instance.

## When queries are executed

When you call LINQ operators, you are simply building up an in-memory representation of the query. The query is only sent to the database when the results are consumed.

The most common operations that result in the query being sent to the database are:
* Iterating the results in a `for` loop
* Using an operator such as `ToList`, `ToArray`, `Single`, `Count`
* Databinding the results of a query to a UI

> [!WARNING]  
> **Always validate user input:** While EF Core protects against SQL injection attacks by using parameters and escaping literals in queries, it does not validate inputs. Appropriate validation, per the application's requirements, should be performed before values from untrusted sources are used in LINQ queries, assigned to entity properties, or passed to other EF Core APIs. This includes any user input used to dynamically construct queries. Even when using LINQ, if you are accepting user input to build expressions, you need to make sure that only intended expressions can be constructed.
