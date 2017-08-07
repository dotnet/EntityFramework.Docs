---
title: EF Core | Platform Support | Microsoft Docs
author: rowanmiller
ms.author: divega
ms.date: 03/13/2017
ms.assetid: bfce70e5-7e14-47d3-87b2-e0b93352e955
ms.technology: entity-framework-core
uid: core/platforms/index
---

# Platform Support

We want EF Core to be available anywhere you can write .NET code, and we're still working towards that goal. The following table provides guidance for each platform where we want to enable EF Core.

EF Core 2.0 targets and therefore requires .NET platforms that support [.NET Standard 2.0](https://docs.microsoft.com/en-us/dotnet/standard/net-standard).

| Platform | Status | 1.x requires | 2.x requires
|-|-|-|-
| **.NET Core** (ASP.NET Core, Console, etc.) | **Fully supported and recommended:** Covered by automated testing and many applications known to be using it successfully. | Same version [.NET Core SDK](https://www.microsoft.com/net/core/) | Same version [.NET Core SDK](https://www.microsoft.com/net/core/)
| **.NET Framework** (WinForms, WPF, ASP.NET, Console, etc.) | **Fully supported and recommended:**  Covered by automated testing and many applications known to be using it successfully. EF 6 also available in this platform (see [Compare EF Core & EF6.x](../../efcore-and-ef6/index.md) to choose the right technology). | .NET Framework 4.5.1 | .NET Framework 4.6.1
| **Mono & Xamarin** | **In progress – issues may be encountered:** Ad-hoc testing has been performed by the EF Core team and customers. Early adopters have reported some success but [issues have been encountered](https://github.com/aspnet/entityframework/issues?q=is%3Aopen+is%3Aissue+label%3Aarea-xamarin) and others will likely be uncovered as testing continues. | Mono 4.6 | Mono 5.0
| **Universal Windows Platform** |  **In progress – issues may be encountered:** Ad-hoc testing has been performed by the EF Core team and customers. [Significant issues](https://github.com/aspnet/entityframework/issues?utf8=%E2%9C%93&q=is%3Aopen%20is%3Aissue%20label%3Aarea-uwp%20) reported when compiled with .NET Native toolchain, which is typically used during a release build, and is a requirement for deploying to the Windows Store (if you are not using .NET Native, or just want to experiment, many of the issues will not affect you). | UWP 5.3.1 | UWP 6.0 <sup>(1)</sup>

<sup>(1)</sup> Upcoming UWP 6.0 adds support for .NET Standard 2.0. We have started testing EF Core 2.0 with it and we will have more information to share about running on this platform soon.

For any combination that doesn’t work as expected, we encourage creating new issues in the [EF Core issue tracker](https://github.com/aspnet/entityframework/issues/new), and for Xamarin related issues, the [Xamarin issue tracker](https://bugzilla.xamarin.com/newbug).
