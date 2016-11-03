---
title: Which One Is Right for You
author: rowanmiller
ms.author: rowmil
manager: rowanmiller
ms.date: 10/27/2016
ms.topic: article
ms.assetid: 288f825b-b3e6-4096-971b-d0a1cb96770e
ms.prod: entity-framework
uid: efcore-and-ef6/choosing
---
# Which One Is Right for You

The following information will help you choose between Entity Framework Core and Entity Framework 6.x.

## Guidance for new applications

Because EF Core is a new product, and still lacks some critical O/RM features, EF6.x will still be the most suitable choice for many applications.

**These are the types of applications we would recommend using EF Core for.**

* New applications that do not require features that are not yet implemented in EF Core. Review [Feature Comparison](features.md) to see if EF Core may be a suitable choice for your application.

* Applications that target .NET Core, such as Universal Windows Platform (UWP) and ASP.NET Core applications. These applications can not use EF6.x as it requires the Full .NET Framework (i.e. .NET Framework 4.5).

## Guidance for existing EF6.x applications

Because of the fundamental changes in EF Core we do not recommend attempting to move an EF6.x application to EF Core unless you have a compelling reason to make the change. If you want to move to EF Core to make use of new features, then make sure you are aware of its limitations before you start. Review [Feature Comparison](features.md) to see if EF Core may be a suitable choice for your application.

**You should view the move from EF6.x to EF Core as a port rather than an upgrade.** See [Porting from EF6.x to EF Core](porting/index.md) for more information.
