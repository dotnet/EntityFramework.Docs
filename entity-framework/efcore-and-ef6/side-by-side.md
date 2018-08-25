---
title: EF6 and EF Core - Using them in the Same Application
author: rowanmiller
ms.date: 10/27/2016
ms.assetid: a06e3c35-110c-4294-a1e2-32d2c31c90a7
uid: efcore-and-ef6/side-by-side
---
# Using EF Core and EF6 in the Same Application

It is possible to use EF Core and EF6 in the same .NET Framework application or library by installing both NuGet packages.

Some types have the same names in EF Core and EF6 and differ only by namespace, which may complicate using both EF Core and EF6 in the same code file. The ambiguity can be easily removed using namespace alias directives. For example:

``` csharp
using Microsoft.EntityFrameworkCore; // use DbContext for EF Core
using EF6 = System.Data.Entity; // use EF6.DbContext for the EF6 version
```

If you are porting an existing application that has multiple EF models, you can choose to selectively port some of them to EF Core, and continue using EF6 for the others.
