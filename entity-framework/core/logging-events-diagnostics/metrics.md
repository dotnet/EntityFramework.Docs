---
title: Metrics - EF Core
description: Tracking EF Core performance and diagnosing anomalies with .NET metrics
author: cincuranet
ms.date: 06/11/2024
uid: core/logging-events-diagnostics/metrics
---

# Metrics in EF Core

Entity Framework Core (EF Core) exposes continuous numeric metrics which can provide a good indication of your program's health. These metrics can be used for the following purposes:

* Track general database load in realtime as the application is running
* Expose problematic coding practices which can lead to degraded performance
* Track down and isolate anomalous program behavior

## Metrics

EF Core reports metrics via the standard <xref:System.Diagnostics.Metrics?displayProperty=nameWithType> API. `Microsoft.EntityFrameworkCore` is the name of the meter. It's recommended to read [.NET documentation on metrics](/dotnet/core/diagnostics/metrics).

> [!NOTE]
> This feature is being introduced in EF Core 9.0 (in preview). [See event counters below](#event-counters-legacy) for older versions of EF Core.

### Metrics and their meaning

* [`microsoft.entityframeworkcore.active_dbcontexts`](#metric-microsoftentityframeworkcoreactive_dbcontexts)
* [`microsoft.entityframeworkcore.queries`](#metric-microsoftentityframeworkcorequeries)
* [`microsoft.entityframeworkcore.savechanges`](#metric-microsoftentityframeworkcoresavechanges)
* [`microsoft.entityframeworkcore.compiled_query_cache_hits`](#metric-microsoftentityframeworkcorecompiled_query_cache_hits)
* [`microsoft.entityframeworkcore.compiled_query_cache_misses`](#metric-microsoftentityframeworkcorecompiled_query_cache_misses)
* [`microsoft.entityframeworkcore.execution_strategy_operation_failures`](#metric-microsoftentityframeworkcoreexecution_strategy_operation_failures)
* [`microsoft.entityframeworkcore.optimistic_concurrency_failures`](#metric-microsoftentityframeworkcoreoptimistic_concurrency_failures)

#### Metric: `microsoft.entityframeworkcore.active_dbcontexts`

| Name     | Instrument Type | Unit (UCUM) | Description    |
| -------- | --------------- | ----------- | -------------- |
| `microsoft.entityframeworkcore.active_dbcontexts` | ObservableUpDownCounter | `{dbcontext}` | Number of currently active `DbContext` instances. |

Available starting in: Entity Framework Core 9.0.

#### Metric: `microsoft.entityframeworkcore.queries`

| Name     | Instrument Type | Unit (UCUM) | Description    |
| -------- | --------------- | ----------- | -------------- |
| `microsoft.entityframeworkcore.queries` | ObservableCounter | `{query}` | Cumulative count of queries executed. |

Available starting in: Entity Framework Core 9.0.

#### Metric: `microsoft.entityframeworkcore.savechanges`

| Name     | Instrument Type | Unit (UCUM) | Description    |
| -------- | --------------- | ----------- | -------------- |
| `microsoft.entityframeworkcore.savechanges` | ObservableCounter | `{savechanges}` | Cumulative count of changes saved. |

Available starting in: Entity Framework Core 9.0.

#### Metric: `microsoft.entityframeworkcore.compiled_query_cache_hits`

| Name     | Instrument Type | Unit (UCUM) | Description    |
| -------- | --------------- | ----------- | -------------- |
| `microsoft.entityframeworkcore.compiled_query_cache_hits` | ObservableCounter | `{hits}` | Cumulative count of hits for the compiled query cache. |

Available starting in: Entity Framework Core 9.0.

#### Metric: `microsoft.entityframeworkcore.compiled_query_cache_misses`

| Name     | Instrument Type | Unit (UCUM) | Description    |
| -------- | --------------- | ----------- | -------------- |
| `microsoft.entityframeworkcore.compiled_query_cache_misses` | ObservableCounter | `{misses}` | Cumulative count of misses for the compiled query cache. |

Available starting in: Entity Framework Core 9.0.

#### Metric: `microsoft.entityframeworkcore.execution_strategy_operation_failures`

| Name     | Instrument Type | Unit (UCUM) | Description    |
| -------- | --------------- | ----------- | -------------- |
| `microsoft.entityframeworkcore.execution_strategy_operation_failures` | ObservableCounter | `{failure}` | Cumulative number of failed operation executed by an `IExecutionStrategy`. |

Available starting in: Entity Framework Core 9.0.

#### Metric: `microsoft.entityframeworkcore.optimistic_concurrency_failures`

| Name     | Instrument Type | Unit (UCUM) | Description    |
| -------- | --------------- | ----------- | -------------- |
| `microsoft.entityframeworkcore.optimistic_concurrency_failures` | ObservableCounter | `{failure}` | Cumulative number of optimistic concurrency failures. |

Available starting in: Entity Framework Core 9.0.

## Event Counters (legacy)

EF Core reports metrics via the standard .NET event counters feature; it's recommended to read [this blog post](https://devblogs.microsoft.com/dotnet/introducing-diagnostics-improvements-in-net-core-3-0/) for a quick overview of how counters work.

### Attach to a process using dotnet-counters

The [dotnet-counters tool](/dotnet/core/diagnostics/dotnet-counters) can be used to attach to a running process and report EF Core event counters regularly; nothing special needs to be done in the program for these counters to be available.

First, install the `dotnet-counters` tool: `dotnet tool install --global dotnet-counters`.

Next, find the process ID (PID) of the .NET process running your EF Core application:

#### [Windows](#tab/windows)

1. Open the Windows Task Manager by right-clicking on the task bar and selecting "Task Manager".
2. Make sure that the "More details" option is selected at the bottom of the window.
3. In the Processes tab, right-click a column and make sure that the PID column is enabled.
4. Locate your application in the process list, and get its process ID from the PID column.

#### [Linux or macOS](#tab/fluent-api)

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

### Counters and their meaning

Counter name                                                                      | Description
--------------------------------------------------------------------------------  | ----
Active DbContexts <br /> (`active-db-contexts`)                                          | The number of active, undisposed DbContext instances currently in your application. If this number grows continuously, you may have a leak because DbContext instances aren't being properly disposed. Note that if [context pooling](xref:core/performance/advanced-performance-topics#dbcontext-pooling) is enabled, this number includes pooled DbContext instances not currently in use.
Execution Strategy Operation Failures <br /> (`total-execution-strategy-operation-failures` and  `execution-strategy-operation-failures-per-second`) | The number of times a database operation failed to execute. If a retrying execution strategy is enabled, this includes each individual failure within multiple attempts on the same operation. This can be used to detect transient issues with your infrastructure.
Optimistic Concurrency Failures <br /> (`total-optimistic-concurrency-failures` and `optimistic-concurrency-failures-per-second`) | The number of times `SaveChanges` failed because of an optimistic concurrency error, because data in the data store was changed since your code loaded it. This corresponds to a <xref:Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException> being thrown.
Queries <br /> (`total-queries` and `queries-per-second`)                                | The number of queries executed.
Query Cache Hit Rate (%) <br /> (`compiled-query-cache-hit-rate`)                        | The ratio of query cache hits to misses. The first time a given LINQ query is executed by EF Core (excluding parameters), it must be compiled in what is a relatively heavy process. In a normal application, all queries are reused, and the query cache hit rate should be stable at 100% after an initial warmup period. If this number is less than 100% over time, you may experience degraded perf due to repeated compilations, which could be a result of suboptimal dynamic query generation.
SaveChanges <br />(`total-save-changes` and `save-changes-per-second`)                                                                       | The number of times `SaveChanges` has been called. Note that `SaveChanges` saves multiple changes in a single batch, so this doesn't necessarily represent each individual update done on a single entity.

### Additional resources

* [.NET documentation on event counters](/dotnet/core/diagnostics/event-counters)
