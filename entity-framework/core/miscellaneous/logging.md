---
title: Logging - EF Core
author: rowanmiller
ms.date: 10/27/2016
ms.assetid: f6e35c6d-45b7-4258-be1d-87c1bb67438d
uid: core/miscellaneous/logging
---
# Logging

> [!TIP]  
> You can view this article's [sample](https://github.com/aspnet/EntityFramework.Docs/tree/master/samples/core/Miscellaneous/Logging) on GitHub.

## ASP.NET Core applications

EF Core integrates automatically with the logging mechanisms of ASP.NET Core whenever `AddDbContext` or `AddDbContextPool` is used. Therefore, when using ASP.NET Core, logging should be configured as described in the [ASP.NET Core documentation](https://docs.microsoft.com/aspnet/core/fundamentals/logging?tabs=aspnetcore2x).

## Other applications

EF Core logging requires an ILoggerFactory which is itself configured with one or more logging providers. Common providers are shipped in the following packages:

* [Microsoft.Extensions.Logging.Console](https://www.nuget.org/packages/Microsoft.Extensions.Logging.Console/): A simple console logger.
* [Microsoft.Extensions.Logging.AzureAppServices](https://www.nuget.org/packages/Microsoft.Extensions.Logging.AzureAppServices/): Supports Azure App Services 'Diagnostics logs' and 'Log stream' features.
* [Microsoft.Extensions.Logging.Debug](https://www.nuget.org/packages/Microsoft.Extensions.Logging.Debug/): Logs to a debugger monitor using System.Diagnostics.Debug.WriteLine().
* [Microsoft.Extensions.Logging.EventLog](https://www.nuget.org/packages/Microsoft.Extensions.Logging.EventLog/): Logs to Windows Event Log.
* [Microsoft.Extensions.Logging.EventSource](https://www.nuget.org/packages/Microsoft.Extensions.Logging.EventSource/): Supports EventSource/EventListener.
* [Microsoft.Extensions.Logging.TraceSource](https://www.nuget.org/packages/Microsoft.Extensions.Logging.TraceSource/): Logs to a trace listener using `System.Diagnostics.TraceSource.TraceEvent()`.

After installing the appropriate package(s), the application should create a singleton/global instance of a LoggerFactory. For example, using the console logger:

### [Version 3.0](#tab/v3)

[!code-csharp[Main](../../../samples/core/Miscellaneous/Logging/Logging/BloggingContext.cs#DefineLoggerFactory)]

### [Version 2.x](#tab/v2)

> [!NOTE]
> The following code sample uses a `ConsoleLoggerProvider` constructor that has been obsoleted in version 2.2 and replaced in 3.0. It is safe to ignore and suppress the warnings when using 2.2.

``` csharp
public static readonly LoggerFactory MyLoggerFactory
    = new LoggerFactory(new[] { new ConsoleLoggerProvider((_, __) => true, true) });
```

***

This singleton/global instance should then be registered with EF Core on the `DbContextOptionsBuilder`. For example:

[!code-csharp[Main](../../../samples/core/Miscellaneous/Logging/Logging/BloggingContext.cs#RegisterLoggerFactory)]

> [!WARNING]
> It is very important that applications do not create a new ILoggerFactory instance for each context instance. Doing so will result in a memory leak and poor performance.

## Filtering what is logged

The application can control what is logged by configuring a filter on the ILoggerProvider. For example:

### [Version 3.0](#tab/v3)

[!code-csharp[Main](../../../samples/core/Miscellaneous/Logging/Logging/BloggingContextWithFiltering.cs#DefineLoggerFactory)]

### [Version 2.x](#tab/v2)

> [!NOTE]
> The following code sample uses a `ConsoleLoggerProvider` constructor that has been obsoleted in version 2.2 and replaced in 3.0. It is safe to ignore and suppress the warnings when using 2.2.

``` csharp
public static readonly LoggerFactory MyLoggerFactory
    = new LoggerFactory(new[]
    {
        new ConsoleLoggerProvider((category, level)
            => category == DbLoggerCategory.Database.Command.Name
               && level == LogLevel.Information, true)
    });
```

***

In this example, the log is filtered to only return messages:

* in the 'Microsoft.EntityFrameworkCore.Database.Command' category
* at the 'Information' level

For EF Core, logger categories are defined in the `DbLoggerCategory` class to make it easy to find categories, but these resolve to simple strings.

More details on the underlying logging infrastructure can be found in the [ASP.NET Core logging documentation](https://docs.microsoft.com/aspnet/core/fundamentals/logging?tabs=aspnetcore2x).
