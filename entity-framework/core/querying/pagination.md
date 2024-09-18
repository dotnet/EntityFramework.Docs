---
title: Pagination - EF Core
description: Writing paginating queries in Entity Framework Core
author: roji
ms.date: 12/19/2021
uid: core/querying/pagination
---
# Pagination

Pagination refers to retrieving results in pages, rather than all at once; this is typically done for large resultsets, where a user interface is shown that allows the user to navigate to the next or previous page of the results.

> [!WARNING]
> Regardless of the pagination method used, always make sure that your ordering is fully unique. For example, if results are ordered only by date, but there can be multiple results with the same date, then results could be skipped when paginating as they're ordered differently across two paginating queries. Ordering by both date and ID (or any other unique property or combination of properties) makes the ordering fully unique and avoids this problem. Note that relational databases do not apply any ordering by default, even on the primary key.

> [!NOTE]
> Azure Cosmos DB has its own mechanism for pagination, [see the dedicated documentation page](xref:core/providers/cosmos/querying#pagination).

## Offset pagination

A common way to implement pagination with databases is to use the `Skip` and `Take` LINQ operators (`OFFSET` and `LIMIT` in SQL). Given a page size of 10 results, the third page can be fetched with EF Core as follows:

[!code-csharp[Main](../../../samples/core/Querying/Pagination/Program.cs?name=OffsetPagination&highlight=4)]

Unfortunately, while this technique is very intuitive, it also has some severe shortcomings:

1. The database must still process the first 20 entries, even if they aren't returned to the application; this creates possibly significant computation load that increases with the number of rows being skipped.
2. If any updates occur concurrently, your pagination may end up skipping certain entries or showing them twice. For example, if an entry is removed as the user is moving from page 2 to 3, the whole resultset "shifts up", and one entry would be skipped.

## Keyset pagination

The recommended alternative to offset-based pagination - sometimes called *keyset pagination* or *seek-based pagination* - is to simply use a `WHERE` clause to skip rows, instead of an offset. This means remember the relevant values from the last entry fetched (instead of its offset), and to ask for the next rows after that row. For example, assuming the last entry in the last page we fetched had an ID value of 55, we'd simply do the following:

[!code-csharp[Main](../../../samples/core/Querying/Pagination/Program.cs?name=KeySetPagination&highlight=4)]

Assuming an index is defined on `PostId`, this query is very efficient, and also isn't sensitive to any concurrent changes happening in lower Id values.

Keyset pagination is appropriate for pagination interfaces where the user navigates forwards and backwards, but does not support random access, where the user can jump to any specific page. Random access pagination requires using offset pagination as explained above; because of the shortcomings of offset pagination, carefully consider if random access pagination really is required for your use case, or if next/previous page navigation is enough. If random access pagination is necessary, a robust implementation could use keyset pagination when navigation to the next/previous page, and offset navigation when jumping to any other page.

### Multiple pagination keys

When using keyset pagination, it's frequently necessary to order by more than one property. For example, the following query paginates by date and ID:

[!code-csharp[Main](../../../samples/core/Querying/Pagination/Program.cs?name=KeySetPaginationWithMultipleKeys&highlight=6)]

This ensures that the next page picks off exactly where the previous one ended. As more ordering keys are added, additional clauses can be added.

> [!NOTE]
> Most SQL databases support a simpler and more efficient version of the above, using *row values*: `WHERE (Date, Id) > (@lastDate, @lastId)`. EF Core does not currently support expressing this in LINQ queries, this is tracked by [#26822](https://github.com/dotnet/efcore/issues/26822).

## Indexes

As with any other query, proper indexing is vital for good performance: make sure to have indexes in place which correspond to your pagination ordering. If ordering by more than one column, an index over those multiple columns can be defined; this is called a *composite index*.

For more information, [see the documentation page on indexes](xref:core/modeling/indexes).

## Additional resources

* To learn more about the shortcomings of offset-based pagination and about keyset pagination, [see this post](https://use-the-index-luke.com/no-offset).
* [.NET Data Community Standup session](https://www.youtube.com/watch?v=DIKH-q-gJNU) where we discuss pagination and demo all the above concepts.
* [A technical deep dive presentation](https://www.slideshare.net/MarkusWinand/p2d2-pagination-done-the-postgresql-way) comparing offset and keyset pagination. While the content deals with the PostgreSQL database, the general information is valid for other relational databases as well.
* For extensions on top of EF Core which simplify keyset pagination, see [MR.EntityFrameworkCore.KeysetPagination](https://github.com/mrahhal/MR.EntityFrameworkCore.KeysetPagination) and [MR.AspNetCore.Pagination](https://github.com/mrahhal/MR.AspNetCore.Pagination).
