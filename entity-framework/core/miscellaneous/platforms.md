---
title: Supported .NET implementations - EF Core
description: Information on supported platforms across Entity Framework Core versions
author: bricelam
ms.date: 12/14/2021
uid: core/miscellaneous/platforms
---

# .NET implementations supported by EF Core

We want EF Core to be available to developers on all modern .NET implementations, and we're still working towards that goal. While EF Core's support on .NET Core is covered by automated testing and many applications known to be using it successfully, Mono, Xamarin and UWP have some issues.

## Overview

The following table provides guidance for each .NET implementation:

EF Core                       | 3.1        | 5.0    | 6.0
----------------------------- | ---------- | ------ | ---
.NET & .NET Core              | 3.1        | 5.0    | 6.0
.NET Standard<sup>(1)</sup>   | 2.0        | 2.1    |
.NET Framework<sup>(2)</sup>  | 4.7.2      |        |
Mono                          | 5.4        | 6.4    |
Xamarin.iOS<sup>(3)</sup>     | 10.14      | 12.16  | (use .NET 6)
Xamarin.Mac<sup>(3)</sup>     | 3.8        | 5.16   | (use .NET 6)
Xamarin.Android<sup>(3)</sup> | 8.0        | 10.0   | (use .NET 6)
UWP<sup>(4)</sup>             | 10.0.16299 |        | (use [Windows App SDK](/windows/apps/windows-app-sdk/))
Unity<sup>(5)</sup>           | 2018.1     | 2021.2 | TBD
Tizen<sup>(6)</sup>           | 4.0        | 6.0    | (use .NET 6)

<sup>(1)</sup> .NET Standard has been superseded by a new approach to uniformity. For more information, see [The future of .NET Standard](https://devblogs.microsoft.com/dotnet/the-future-of-net-standard/).

<sup>(2)</sup> See the [.NET Framework](#net-framework) section below.

<sup>(3)</sup> There are issues and known limitations with Xamarin which may prevent some applications developed using EF Core from working correctly. Check the list of [active issues](https://github.com/dotnet/efcore/issues?q=is%3Aopen+is%3Aissue+label%3Aarea-xamarin) for workarounds.

<sup>(4)</sup> Install the [.NET Core UWP 6.x package](https://www.nuget.org/packages/Microsoft.NETCore.UniversalWindowsPlatform/). See the [Universal Windows Platform](#universal-windows-platform) section of this article.

<sup>(5)</sup> There are issues and known limitations with Unity. Check the list of [active issues](https://github.com/dotnet/efcore/issues?q=is%3Aopen+is%3Aissue+label%3Aarea-unity).

<sup>(6)</sup> EF Core compatibility with Tizen is largely unknown. If you've tried it, we'd love your feedback.

## .NET Framework

Applications that target .NET Framework may need changes to work with .NET Standard libraries:

Edit the project file and make sure the following entry appears in the initial property group:

```xml
<AutoGenerateBindingRedirects>true</AutoGenerateBindingRedirects>
```

For test projects, also make sure the following entry is present:

```xml
<GenerateBindingRedirectsOutputType>true</GenerateBindingRedirectsOutputType>
```

If you want to use an older version of Visual Studio, make sure you [upgrade the NuGet client to version 3.6.0](https://www.nuget.org/downloads) to work with .NET Standard 2.0 libraries.

We also recommend migrating from NuGet packages.config to PackageReference if possible. Add the following property to your project file:

```xml
<RestoreProjectStyle>PackageReference</RestoreProjectStyle>
```

## Universal Windows Platform

Earlier versions of EF Core and .NET UWP had numerous compatibility issues, especially with applications compiled with the .NET Native toolchain. The new .NET UWP version adds support for .NET Standard 2.0 and contains .NET Native 2.0, which fixes most of the compatibility issues previously reported. EF Core 2.0.1 has been tested more thoroughly with UWP but testing is not automated.

When using EF Core on UWP:

* To optimize query performance, avoid anonymous types in LINQ queries. Deploying a UWP application to the app store requires an application to be compiled with .NET Native. Queries with anonymous types have worse performance on .NET Native.

* To optimize `SaveChanges()` performance, use [ChangeTrackingStrategy.ChangingAndChangedNotifications](/dotnet/api/microsoft.entityframeworkcore.changetrackingstrategy) and implement [INotifyPropertyChanged](https://msdn.microsoft.com/library/system.componentmodel.inotifypropertychanged.aspx), [INotifyPropertyChanging](https://msdn.microsoft.com/library/system.componentmodel.inotifypropertychanging.aspx), and [INotifyCollectionChanged](https://msdn.microsoft.com/library/system.collections.specialized.inotifycollectionchanged.aspx) in your entity types.

## Report issues

For any combination that doesn't work as expected, we encourage creating new issues on the [EF Core issue tracker](https://github.com/dotnet/efcore/issues/new). For Xamarin-specific issues use the issue tracker for [Xamarin.Android](https://github.com/xamarin/xamarin-android/issues/new) or [Xamarin.iOS](https://github.com/xamarin/xamarin-macios/issues/new).
