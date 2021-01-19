---
title: Event Counters - EF Core
description: Tracking EF Core performance and diagnosing anomalies with .NET event counters
author: roji
ms.date: 11/17/2020
uid: core/logging-events-diagnostics/event-counters
---

# Event Counters

> [!NOTE]
> This feature was added in EF Core 5.0.

Entity Framework Core (EF Core) exposes continuous numeric metrics which can provide a good indication of your program's health. These metrics can be used for the following purposes:

* Track general database load in realtime as the application is running
* Expose problematic coding practices which can lead to degraded performance
* Track down and isolate anomalous program behavior

EF Core reports metrics via the standard .NET event counters feature; it's recommended to read [this blog post](https://devblogs.microsoft.com/dotnet/introducing-diagnostics-improvements-in-net-core-3-0/) for a quick overview of how counters work.

## Attach to a process using dotnet-counters

The [dotnet-counters tool](https://docs.microsoft.com/dotnet/core/diagnostics/dotnet-counters) can be used to attach to a running process and report EF Core event counters regularly; nothing special needs to be done in the program for these counters to be available.

First, install the `dotnet-counters` tool: `dotnet tool install --global dotnet-counters`.

Next, find the process ID (PID) of the .NET process running your EF Core application:

### [Windows](#tab/windows)

1. Open the Windows Task Manager by right-clicking on the task bar and selecting "Task Manager".
2. Make sure that the "More details" option is selected at the bottom of the window.
3. In the Processes tab, right-click a column and make sure that the PID column is enabled.
4. Locate your application in the process list, and get its process ID from the PID column.

### [Linux or macOS](#tab/fluent-api)

1. Use the `ps` command to list all processes running for your user.
2. Locate your application in the process list, and get its process ID from the PID column.

***

Inside your .NET application, the process ID is available as `Process.GetCurrentProcess().Id`; this can be useful for printing the PID upon startup.

Finally, launch `dotnet-counters` as follows:

```console
dotnet counters monitor Microsoft.EntityFrameworkCore -p <PID>
```

`dotnet-counters` will now attach to your running process and start reporting continuous counter data:

```console
Press p to pause, r to resume, q to quit.
 Status: Running

[Microsoft.EntityFrameworkCore]
    Active DbContexts                                               1
    Execution Strategy Operation Failures (Count / 1 sec)           0
    Execution Strategy Operation Failures (Total)                   0
    Optimistic Concurrency Failures (Count / 1 sec)                 0
    Optimistic Concurrency Failures (Total)                         0
    Queries (Count / 1 sec)                                         1
    Queries (Total)                                               189
    Query Cache Hit Rate (%)                                      100
    SaveChanges (Count / 1 sec)                                     0
    SaveChanges (Total)                                             0
```

## Counters and their meaning

Counter name                          | Description
------------------------------------- | ----
Active DbContexts                     | The number of active, undisposed DbContext instances currently in your application. If this number grows continuously, you may have a leak because DbContext instances aren't being properly disposed. Note that if [context pooling](xref:core/performance/advanced-performance-topics#dbcontext-pooling) is enabled, this number includes pooled DbContext instances not currently in use.
Execution Strategy Operation Failures | The number of times a database operation failed to execute. If a retrying execution strategy is enabled, this includes each individual failure within multiple attempts on the same operation. This can be used to detect transient issues with your infrastructure.
Optimistic Concurrency Failures       | The number of times `SaveChanges` failed because of an optimistic concurrency error, because data in the data store was changed since your code loaded it. This corresponds to a <xref:Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException> being thrown.
Queries                               | The number of queries executed.
Query Cache Hit Rate (%)              | The ratio of query cache hits to misses. The first time a given LINQ query is executed by EF Core (excluding parameters), it must be compiled in what is a relatively heavy process. In a normal application, all queries are reused, and the query cache hit rate should be stable at 100% after an initial warmup period. If this number is less than 100% over time, you may experience degraded perf due to repeated compilations, which could be a result of suboptimal dynamic query generation.
SaveChanges                           | The number of times `SaveChanges` has been called. Note that `SaveChanges` saves multiple changes in a single batch, so this doesn't necessarily represent each individual update done on a single entity.

## Additional resources

* [.NET documentation on event counters](https://docs.microsoft.com/dotnet/core/diagnostics/event-counters)
