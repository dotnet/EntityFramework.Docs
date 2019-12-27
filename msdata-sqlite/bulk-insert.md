---
title: Bulk Insert - Microsoft.Data.Sqlite
author: bricelam
ms.date: 11/22/2019
---
# Bulk Insert

SQLite doesn't have any special way to bulk insert data. To get optimal performance when inserting or updating data, ensure that you do the following.

1. Use a transaction.
2. Reuse the same parameterized command. Subsequent executions will reuse the compilation of the first one.

[!code-csharp[](../samples/msdata-sqlite/BulkInsertSample/Program.cs?name=snippet_BulkInsert)]
