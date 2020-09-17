---
title: Loading Related Data - EF Core
description: Different strategies for loading related data with Entity Framework Core
author: roji
ms.date: 9/11/2020
ms.assetid: f9fb64e2-6699-4d70-a773-592918c04c19
uid: core/querying/related-data
---
# Loading Related Data

Entity Framework Core allows you to use the navigation properties in your model to load related entities. There are three common O/RM patterns used to load related data.

* **[Eager loading](xref:core/querying/related-data/eager)** means that the related data is loaded from the database as part of the initial query.
* **[Explicit loading](xref:core/querying/related-data/explicit)** means that the related data is explicitly loaded from the database at a later time.
* **[Lazy loading](xref:core/querying/related-data/lazy)** means that the related data is transparently loaded from the database when the navigation property is accessed.

For more information on each of these loading approaches, see [Eager, explicit, and lazy loading](/aspnet/core/data/ef-rp/read-related-data).

> [!TIP]
> You can view the [samples](https://github.com/dotnet/EntityFramework.Docs/tree/master/samples/core/Querying) under this section on GitHub.
