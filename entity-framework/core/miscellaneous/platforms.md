---
title: Supported .NET implementations - EF Core
description: Information on supported platforms across Entity Framework Core versions
author: SamMonoRT
ms.date: 12/13/2022
uid: core/miscellaneous/platforms
---

# .NET implementations supported by EF Core

We want EF Core to be available to developers on all modern .NET implementations. EF Core's support on .NET for Windows, Linux, and macOS is covered by automated testing and many applications are known to be using it successfully, other platforms that leverage trimming and ahead-of-time (AoT) compilation like iOS, Wasm, and Unity have some limitations that we are working to address.

Several older .NET implementations are no longer supported. See the sections below for more guidance.

| EF Core           | .NET & .NET Core | .NET Standard | .NET Framework |
|-------------------|------------------|---------------|----------------|
| **9.0**           | 8.0              |               |                |
| **8.0**           | 8.0              |               |                |
| ~~**7.0**~~ (EOL) | 6.0              |               |                |
| ~~**6.0**~~ (EOL) | 6.0              |               |                |
| ~~**5.0**~~ (EOL) | 5.0              | 2.1           |                |
| ~~**3.1**~~ (EOL) | 3.1              | 2.0           | 4.7.2          |

## .NET

EF Core is a .NET library. EF Core version versions 8.0 and 9.0 both target .NET version 8.0. In general, we target the latest [LTS release](/lifecycle/products/microsoft-net-and-net-core) of .NET. This enables you to upgrade to a newer, STS release of EF Core to take advantage of the latest features without having to upgrade your entire app. There may be exceptions to this, however, as runtime features sometimes get added that require us to depend on the latest version of .NET.

.NET supports multiple platforms including Windows, Linux, macOS, iOS, Android, and Wasm. For more details on which version are supported, see the [.NET Supported OS Policy](https://github.com/dotnet/core/blob/main/os-lifecycle-policy.md).

## .NET Core

The last release of .NET Core was version 3.1. It was renamed to just .NET in version 5.0. Note, version 4.0 was skipped to avoid confusion with .NET Framework--the original, Windows-only implementation. .NET continues to support multiple platforms including Windows, Linux, and macOS.

## .NET Standard

.NET Standard has been superseded by a new approach to uniformity. For more information, see [The future of .NET Standard](https://devblogs.microsoft.com/dotnet/the-future-of-net-standard/). The last version of EF Core that supported .NET Standard was version 5.0.

## .NET Framework

The last version of EF Core that supported .NET Framework was version 3.1. We recommend using .NET instead which continues to support WinForms and WPF applications. The [.NET Upgrade Assistant](/dotnet/core/porting/upgrade-assistant-overview) can help you with the migration process.

## Xamarin

The last version of EF Core that supported Xamarin was version 5.0. We recommend using .NET and [.NET MAUI](/dotnet/maui/) instead. .NET supports multiple platforms including Android, iOS, macOS, and Windows. .NET MAUI is an evolution of the Xamarin.Forms UI framework. Xamarin.Android, Xamarin.iOS, Xamarin.Mac are now integrated directly into .NET (starting with .NET 6) as .NET for Android, .NET for iOS, and .NET for macOS. If you're building with these project types today, they should be upgraded to .NET SDK-style projects for continued support. For more information about upgrading Xamarin projects to .NET, see the [Upgrade from Xamarin to .NET & .NET MAUI](/dotnet/maui/migration) documentation.

## Universal Windows Platform

The last version of EF Core that supported UWP was version 3.1. We recommend using .NET and the [Windows App SDK](/windows/apps/windows-app-sdk/) instead.

## Unity

Unity currently only supports .NET Standard libraries. The last version of EF Core that supported .NET Standard was version 5.0. Unity is currently working towards an implementation that uses .NET. For more information, see [Unity and .NET, what’s next?](https://blog.unity.com/technology/unity-and-net-whats-next)

## Tizen

Tizen is an open source operating system that runs on various Samsung devices including phones, tablets, watches, TVs, cameras, and appliances. [Tizen .NET](https://developer.samsung.com/tizen/About-Tizen.NET/Tizen.NET.html) enables you to develop apps for it using .NET and .NET MAUI. EF Core compatibility with Tizen is largely unknown. If you've tried it, we'd love your feedback.
