---
title: EF Core | Supported .NET implementations | Microsoft Docs
author: rowanmiller
ms.author: divega
ms.date: 08/30/2017
ms.technology: entity-framework-core
uid: core/platforms/index
---

# .NET implementations supported by EF Core

We want EF Core to be available anywhere you can write .NET code, and we're still working towards that goal. The following table provides guidance for each .NET implementation where we want to enable EF Core.

EF Core 2.0 targets [.NET Standard 2.0](https://docs.microsoft.com/en-us/dotnet/standard/net-standard) and therefore requires .NET implementations that support it.

| .NET implementation | Status | 1.x requires | 2.x requires
|-|-|-|-
| **.NET Core** ([ASP.NET Core](../get-started/aspnetcore/index.md), [Console](../get-started/netcore/index.md), etc.) | **Fully supported and recommended:** Covered by automated testing and many applications known to be using it successfully. | [.NET Core SDK 1.x](https://www.microsoft.com/net/core/) | [.NET Core SDK 2.x](https://www.microsoft.com/net/core/)
| **.NET Framework** (WinForms, WPF, ASP.NET, [Console](../get-started/full-dotnet/index.md), etc.) | **Fully supported and recommended:**  Covered by automated testing and many applications known to be using it successfully. EF 6 also available in this platform (see [Compare EF Core & EF6](../../efcore-and-ef6/index.md) to choose the right technology). | .NET Framework 4.5.1 | .NET Framework 4.6.1
| **Mono & Xamarin** | **In progress – issues may be encountered:** Ad-hoc testing has been performed by the EF Core team and customers. Early adopters have reported some success but [issues have been encountered](https://github.com/aspnet/entityframework/issues?q=is%3Aopen+is%3Aissue+label%3Aarea-xamarin) and others will likely be uncovered as testing continues. | Mono 4.6 <br/> Xamarin.iOS 10 <br/> Xamarin.Mac 3 <br/> Xamarin.Android 7 | Mono 5.4 <br/> Xamarin.iOS 10.14 <br/> Xamarin.Mac 3.8 <br/> Xamarin.Android 7.5
| [**Universal Windows Platform**](../get-started/uwp/index.md) | **In progress – issues may be encountered:** Ad-hoc testing has been performed by the EF Core team and customers. [Significant issues](https://github.com/aspnet/entityframework/issues?utf8=%E2%9C%93&q=is%3Aopen%20is%3Aissue%20label%3Aarea-uwp%20) have been reported when compiled with .NET Native toolchain, which is typically used during a release build, and is a requirement for deploying to the Windows Store (if you are not using .NET Native, or just want to experiment, many of the issues will not affect you). | [.NET UWP 5.2.2 package](https://www.nuget.org/packages/Microsoft.NETCore.UniversalWindowsPlatform/5.2.2) | .NET UWP 6.0.0 package <sup>(1)</sup>

<sup>(1)</sup> This upcoming version of .NET UWP adds support for .NET Standard 2.0. We have started testing EF Core 2.0 with it and we have found some issues that will require a patch release. Better support is expected in EF Core 2.0.1. We will have more information to share about running on this platform soon.

For any combination that doesn’t work as expected, we encourage creating new issues in the [EF Core issue tracker](https://github.com/aspnet/entityframework/issues/new), and for Xamarin related issues, the [Xamarin issue tracker](https://bugzilla.xamarin.com/newbug).
