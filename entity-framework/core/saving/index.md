---
title: Saving Data - EF Core
description: Overview of saving data with Entity Framework Core
author: ajcvickers
ms.date: 10/27/2016
uid: core/saving/index
---
# Saving Data

Each context instance has a `ChangeTracker` that is responsible for keeping track of changes that need to be written to the database. As you make changes to instances of your entity classes, these changes are recorded in the `ChangeTracker` and then written to the database when you call `SaveChanges`. The database provider is responsible for translating the changes into database-specific operations (for example, `INSERT`, `UPDATE`, and `DELETE` commands for a relational database).
