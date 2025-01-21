---
title: EF6 and EF Core - Using them in the Same Application
description: Guidance on using both Entity Framework Core and Entity Framework 6 in the same application
author: SamMonoRT
ms.date: 01/23/2019
uid: efcore-and-ef6/side-by-side
---
# Using EF Core and EF6 in the Same Application

It is possible to use EF Core and EF6 in the same application or library by installing both NuGet packages.

Some types have the same names in EF Core and EF6 and differ only by namespace, which may complicate using both EF Core and EF6 in the same code file. The ambiguity can be easily removed using namespace alias directives. For example:

```csharp
using Microsoft.EntityFrameworkCore; // use DbContext for EF Core
using EF6 = System.Data.Entity; // use EF6.DbContext for the EF6 version
```

If you are porting an existing application that has multiple EF models, you can choose to selectively port some of them to EF Core, and continue using EF6 for the others.
