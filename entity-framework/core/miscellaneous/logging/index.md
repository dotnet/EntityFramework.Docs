---
title: Overview of logging and interception - EF Core
description: Overview of logging, events, interceptors, and diagnostics for EF Core  
author: ajcvickers
ms.date: 10/01/2020
uid: core/miscellaneous/logging/index
---
# Overview of logging and interception

EF Core contains several mechanisms for generating logs, responding to events, and obtaining diagnostics. Each of these are tailored to different situations, and it is important to select the best mechanism for the task in hand, even when multiple mechanisms could work. For example, a database interceptor could be used to log SQL, but this is better handled by one of the logging-specific mechanisms. This page presents an overview of the different mechanisms and describes when each should be used.

## Simple logging

> [!NOTE]
> This feature was added in EF Core 5.0.

EF Core generates log messages for operations such as executing a query or saving changes to the database. These can be accessed from any application type using [LogTo](/dotnet/api/microsoft.entityframeworkcore.dbcontextoptionsbuilder.logto) when configuring a <xref:Microsoft.EntityFrameworkCore.DbContext> instance. For example:

<!--
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.LogTo(Console.WriteLine);
-->
[!code-csharp[LogToConsole](../../../../samples/core/Miscellaneous/Collations/Program.cs?name=LogToConsole)]

This is a similar concept to [DbContext.Database.Log](/dotnet/api/system.data.entity.database.log) in EF6.

See [Simple Logging](xref:core/miscellaneous/logging/simple-logging) for more information.

## Logging with Microsoft.Extensions.Logging

TODO

## Events

TODO

## Interceptors

TODO

## Diagnostic listeners

TODO

## Event sources

TODO
