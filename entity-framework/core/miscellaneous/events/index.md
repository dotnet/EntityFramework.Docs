---
title: Overview of logging and interception - EF Core
description: Overview of logging, events, interceptors, and diagnostics for EF Core  
author: ajcvickers
ms.date: 10/01/2020
uid: core/miscellaneous/events/index
---
# Overview of logging and interception

Entity Framework Core (EF Core) contains several mechanisms for generating logs, responding to events, and obtaining diagnostics. Each of these are tailored to different situations, and it is important to select the best mechanism for the task in hand, even when multiple mechanisms could work. For example, a database interceptor could be used to log SQL, but this is better handled by one of the logging-specific mechanisms. This page presents an overview of each of these mechanisms and describes when each should be used.

## Simple logging

> [!NOTE]
> This feature was added in EF Core 5.0.

EF Core logs can be accessed from any type of application through use of [LogTo](https://github.com/dotnet/efcore/blob/ec3df8fd7e4ea4ebeebfa747619cef37b23ab2c6/src/EFCore/DbContextOptionsBuilder.cs#L135) <!-- Issue #2748 <xref:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.LogTo%2A> --> when [configuring a DbContext instance](xref:core/miscellaneous/configuring-dbcontext). This configuration is commonly done in an override of <xref:Microsoft.EntityFrameworkCore.DbContext.OnConfiguring%2A?displayProperty=nameWithType>. For example:

<!--
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.LogTo(Console.WriteLine);
-->
[!code-csharp[LogToConsole](../../../../samples/core/Miscellaneous/Logging/SimpleLogging/Program.cs?name=LogToConsole)]

This concept is similar to <xref:System.Data.Entity.Database.Log?displayProperty=nameWithType> in EF6.

See [Simple Logging](xref:core/miscellaneous/events/simple-logging) for more information.
