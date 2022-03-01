---
title: Port from EF6 to EF Core - Hybrid Approach
description: How to port from EF6 to EF Core when you iterate your domain model and database separately and use mapping to connect the two.
author: jeremylikness
ms.alias: jeliknes
ms.date: 12/09/2021
uid: efcore-and-ef6/porting/port-hybrid
---

# Port from EF6 to EF Core - the Hybrid Approach

Two common approaches are to generate your database from code and use migrations, or generate your entities from the database using reverse-engineering. In the hybrid approach, you don't generate anything. Instead, you let the database and codebase evolve and use model configuration to keep the two in sync. This page contains some tips for success using the hybrid approach:

1. First, read the guides for [code as source of truth](xref:efcore-and-ef6/porting/port-code) and [database as source of truth](xref:efcore-and-ef6/porting/port-database) to familiarize yourself with some of the considerations to be aware of.
1. Because you won't be using migrations, there is no need to model sequences, non-primary indexes, constraints and index filters.
1. An integration test suite is valuable in this approach for validating a proper handoff between code and the database as the code and database evolve.
1. One approach to test that your mappings are correct is to generate a dummy database using a "throwaway" migration, then use a tool to compare the generated database to the actual database. You can quickly flag differences in schema and act on them.
1. If you prefer, you can consider generating partial classes from the database and use extensions to those classes to configure your custom code.
