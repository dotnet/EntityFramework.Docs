---
title: Compare EF Core & EF6 
author: rowanmiller
ms.author: divega
ms.date: 10/27/2016

ms.assetid: a6b9cd22-6803-4c6c-a4d4-21147c0a81cb
uid: efcore-and-ef6/index
---

# Compare EF Core & EF6

There are two versions of Entity Framework, Entity Framework Core and Entity Framework 6.

## Entity Framework 6

Entity Framework 6 (EF6) is a tried and tested data access technology with many years of features and stabilization. It first released in 2008, as part of .NET Framework 3.5 SP1 and Visual Studio 2008 SP1. Starting with the EF4.1 release it has shipped as the [EntityFramework NuGet package](https://www.nuget.org/packages/EntityFramework/) - currently the most popular package on NuGet.org.

EF6 continues to be a supported product, and will continue to see bug fixes and minor improvements for some time to come.

## Entity Framework Core

Entity Framework Core (EF Core) is a lightweight, extensible, and cross-platform version of Entity Framework. EF Core introduces many improvements and new features when compared with EF6. At the same time, EF Core is a new code base and not as mature as EF6.

EF Core keeps the developer experience from EF6, and most of the top-level APIs remain the same too, so EF Core will feel very familiar to folks who have used EF6. At the same time, EF Core is built over a completely new set of core components. This means EF Core doesn't automatically inherit all the features from EF6. Some of these features will show up in future releases (such as lazy loading and connection resiliency), other less commonly used features will not be implemented in EF Core.

The new, extensible, and lightweight core has also allowed us to add some features to EF Core that will not be implemented in EF6 (such as alternate keys and mixed client/database evaluation in LINQ queries).
