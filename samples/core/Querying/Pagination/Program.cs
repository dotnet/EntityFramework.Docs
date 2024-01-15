using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EFQuerying.Pagination;

internal class Program
{
    private static void Main()
    {
        using (var context = new BloggingContext())
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        using (var context = new BloggingContext())
        {
            #region OffsetPagination
            const int position = 20;
            var nextPage = context.Posts
                .OrderBy(b => b.PostId)
                .Skip(position)
                .Take(10)
                .ToList();
            #endregion
        }

        using (var context = new BloggingContext())
        {
            #region KeySetPagination
            const int lastId = 55;
            var nextPage = context.Posts
                .OrderBy(b => b.PostId)
                .Where(b => b.PostId > lastId)
                .Take(10)
                .ToList();
            #endregion
        }

        using (var context = new BloggingContext())
        {
            #region KeySetPaginationWithMultipleKeys
            var lastDate = new DateTime(2020, 1, 1);
            const int lastId = 55;
            var nextPage = context.Posts
                .OrderBy(b => b.Date)
                .ThenBy(b => b.PostId)
                .Where(b => b.Date > lastDate || (b.Date == lastDate && b.PostId > lastId))
                .Take(10)
                .ToList();
            #endregion
        }
    }
}