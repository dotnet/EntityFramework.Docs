---
title: Miscellaneous - Microsoft SQL Server Database Provider - EF Core
description: Miscellaneous for the Microsoft SQL Server database provider
author: roji
ms.date: 06/07/2021
uid: core/providers/sql-server/misc
---
# Miscellaneous notes for SQL Server

## SaveChanges and database triggers

Starting with EF Core 7.0, EF Core saves changes to the database with significantly optimized SQL; unfortunately, this technique is not supported on SQL Server if the target table has database triggers. For more information on this SQL Server limitation, see the documentation on the [OUTPUT clause](/sql/t-sql/queries/output-clause-transact-sql).

You can let EF Core know that the target table has a trigger; doing so will revert to the previous, less efficient technique. This can be done by configuring the corresponding entity type as follows:

[!code-csharp[Main](../../../../samples/core/SqlServer/Misc/TriggersContext.cs?name=TriggerConfiguration&highlight=4)]

Note that doing this doesn't actually make EF Core create or manage the trigger in any way - it currently only informs EF Core that triggers are present on the table. As a result, any trigger name can be used.
