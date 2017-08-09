---
title: EF6 and EF Core | Using them in the Same Application | Microsoft Docs
author: rowanmiller
ms.author: divega
ms.date: 10/27/2016

ms.assetid: a06e3c35-110c-4294-a1e2-32d2c31c90a7
uid: efcore-and-ef6/side-by-side
---
# Using EF Core and EF6 in the Same Application

It is possible to use EF Core and EF6 in the same application. EF Core and EF6 have the same type names that differ only by namespace, so this may complicate code that attempts to use both EF Core and EF6 in the same code file.

If you are porting an existing application that has multiple EF models, then you can selectively port some of them to EF Core, and continue using EF6 for the others.
