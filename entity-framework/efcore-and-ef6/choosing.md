---
title: EF Core and EF6 - Which One Is Right for You
author: rowanmiller
ms.author: divega
ms.date: 10/27/2016

ms.assetid: 288f825b-b3e6-4096-971b-d0a1cb96770e
uid: efcore-and-ef6/choosing
---
# EF Core and EF6: Which One Is Right for You

The following information will help you choose between Entity Framework Core and Entity Framework 6.

## Guidance for new applications

Consider using EF Core for new applications if you want to take advantage of the all the capabilities of EF Core and your application does not require any features that are not yet implemented in EF Core.

EF6 requires .NET Framework 4.0 (or a later version) and is only supported on Windows (that is, EF6 does not currently run on .NET Core and is not supported in other operating systems), but it is still a viable choice for new applications as long those constraints are acceptable and the application does not require new features in EF Core that are not available to EF6.

Review [Feature Comparison](features.md) to see if EF Core may be a suitable choice for your application.

## Guidance for existing EF6 applications

Because of the fundamental changes in EF Core we do not recommend attempting to move an EF6 application to EF Core unless you have a compelling reason to make the change. If you want to move to EF Core to make use of new features, then make sure you are aware of its limitations before you start. Review [Feature Comparison](features.md) to see if EF Core may be a suitable choice for your application.

**You should view the move from EF6 to EF Core as a port rather than an upgrade.** See [Porting from EF6 to EF Core](porting/index.md) for more information.
