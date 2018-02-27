---
title: Supported .NET implementations - EF Core
author: rowanmiller
ms.author: divega
ms.date: 08/30/2017
ms.technology: entity-framework-core
uid: core/platforms/index
---

# .NET implementations supported by EF Core

We want EF Core to be available anywhere you can write .NET code, and we're still working towards that goal. While EF Core's support on .NET Core and .NET Framework is covered by automated testing and many applications known to be using it successfully, Mono, Xamarin and UWP have some issues.

The following table provides guidance for each .NET implementation:

| .NET implementation                                                                                                  | Status                                                             | EF Core 1.x requirements                                                                                | EF Core 2.x requirements <sup>(1)</sup>                                                                 |
|:---------------------------------------------------------------------------------------------------------------------|:-------------------------------------------------------------------|:--------------------------------------------------------------------------------------------------------|:--------------------------------------------------------------------------------------------------------|
| **.NET Core** ([ASP.NET Core](../get-started/aspnetcore/index.md), [Console](../get-started/netcore/index.md), etc.) | Fully supported and recommended                                    | [.NET Core SDK 1.x](https://www.microsoft.com/net/core/)                                                | [.NET Core SDK 2.x](https://www.microsoft.com/net/core/)                                                |
| **.NET Framework** (WinForms, WPF, ASP.NET, [Console](../get-started/full-dotnet/index.md), etc.)                    | Fully supported and recommended. EF6 also available <sup>(2)</sup> | .NET Framework 4.5.1                                                                                    | .NET Framework 4.6.1                                                                                    |
| **Mono & Xamarin**                                                                                                   | In progress <sup>(3)</sup>                                         | Mono 4.6 <br/> Xamarin.iOS 10 <br/> Xamarin.Mac 3 <br/> Xamarin.Android 7                               | Mono 5.4 <br/> Xamarin.iOS 10.14 <br/> Xamarin.Mac 3.8 <br/> Xamarin.Android 7.5                        |
| [**Universal Windows Platform**](../get-started/uwp/index.md)                                                        | EF Core 2.0.1 recommended <sup>(4)</sup>                           | [.NET Core UWP 5.x package](https://www.nuget.org/packages/Microsoft.NETCore.UniversalWindowsPlatform/) | [.NET Core UWP 6.x package](https://www.nuget.org/packages/Microsoft.NETCore.UniversalWindowsPlatform/) |

<sup>(1)</sup> EF Core 2.0 targets and therefore requires .NET implementations that support [.NET Standard 2.0](https://docs.microsoft.com/dotnet/standard/net-standard).

<sup>(2)</sup> See [Compare EF Core & EF6](../../efcore-and-ef6/index.md) to choose the right technology.

<sup>(3)</sup> There are issues and known limitations with Xamarin which may prevent some applications developed using EF Core 2.0 from working correctly. Check the list of [active issues]([](https://github.com/aspnet/entityframeworkCore/issues?q=is%3Aopen+is%3Aissue+label%3Aarea-xamarin) for workarounds.

<sup>(4)</sup> Earlier versions of EF Core and .NET UWP had numerous compatibility issues, especially with applications compiled with the .NET Native toolchain. The new .NET UWP version adds support for .NET Standard 2.0 and contains .NET Native 2.0, which fixes most of the compatibility issues previously reported. EF Core 2.0.1 has been tested more thoroughly with UWP but testing is not automated.

For any combination that doesn’t work as expected, we encourage creating new issues on the [EF Core issue tracker](https://github.com/aspnet/entityframeworkcore/issues/new). For Xamarin-specific issues use the issue tracker for [Xamarin.Android](https://github.com/xamarin/xamarin-android/issues/new) or [Xamarin.iOS](https://github.com/xamarin/xamarin-macios/issues/new).
