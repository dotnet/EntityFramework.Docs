---
title: Efficient Updating - EF Core
description: Performance guide for efficient updating using Entity Framework Core
author: roji
ms.date: 12/1/2020
uid: core/performance/efficient-updating
---
# Efficient Updating

## Batching

EF Core helps minimize roundtrips by automatically batching together all updates in a single roundtrip. Consider the following:

[!code-csharp[Main](../../../samples/core/Performance/Other/Program.cs#SaveChangesBatching)]

The above loads a blog from the database, changes its URL, and then adds two new blogs; to apply this, two SQL INSERT statements and one UPDATE statement are sent to the database. Rather than sending them one by one, as Blog instances are added, EF Core tracks these changes internally, and executes them in a single roundtrip when <xref:Microsoft.EntityFrameworkCore.DbContext.SaveChanges%2A> is called.

The number of statements that EF batches in a single roundtrip depends on the database provider being used. For example, performance analysis has shown batching to be generally less efficient for SQL Server when less than 4 statements are involved. Similarly, the benefits of batching degrade after around 40 statements for SQL Server, so EF Core will by default only execute up to 42 statements in a single batch, and execute additional statements in separate roundtrips.

Users can also tweak these thresholds to achieve potentially higher performance - but benchmark carefully before modifying these:

[!code-csharp[Main](../../../samples/core/Performance/Other/BatchTweakingContext.cs#BatchTweaking)]

## Bulk updates

Let's assume you want to give all your employees a raise. A typical implementation for this in EF Core would look like the following:

[!code-csharp[Main](../../../samples/core/Performance/Other/Program.cs#UpdateWithoutBulk)]

While this is perfectly valid code, let's analyze what it does from a performance perspective:

* A database roundtrip is performed, to load all the relevant employees; note that this brings all the Employees' row data to the client, even if only the salary will be needed.
* EF Core's change tracking creates snapshots when loading the entities, and then compares those snapshots to the instances to find out which properties changed.
* Typically, a second database roundtrip is performed to save all the changes (note that some database providers split the changes into multiples roundtrips). Although this batching behavior is far better than doing a roundtrip for each update, EF Core still sends an UPDATE statement per employee, and the database must execute each statement separately.

Relational databases also support *bulk updates*, so the above could be rewritten as the following single SQL statement:

```sql
UPDATE [Employees] SET [Salary] = [Salary] + 1000;
```

This performs the entire operation in a single roundtrip, without loading or sending any actual data to the database, and without making use of EF's change tracking machinery, which imposes an additional overhead.

Unfortunately, EF doesn't currently provide APIs for performing bulk updates. Until these are introduced, you can use raw SQL to perform the operation where performance is sensitive:

[!code-csharp[Main](../../../samples/core/Performance/Other/Program.cs#UpdateWithBulk)]
