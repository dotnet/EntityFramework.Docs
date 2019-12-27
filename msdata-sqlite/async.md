---
title: Async Limitations - Microsoft.Data.Sqlite
author: bricelam
ms.date: 11/22/2019
---
# Async Limitations

SQLite doesn't support asynchronous I/O. Async ADO.NET methods will execute synchronously in Microsoft.Data.Sqlite. Avoid calling them.

Instead, use [write-ahead logging <span class="docon docon-navigate-external" aria-hidden="true" />](https://www.sqlite.org/wal.html) to improve performance and concurrency.

[!code-csharp[](../samples/msdata-sqlite/AsyncSample/Program.cs?name=snippet_WAL)]

> [!TIP]
> Write-ahead logging is enabled by default on databases created using [Entity Framework Core](/ef/core/).
