using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.EntityTypes;

internal class Program
{
    private static async Task Main(string[] args)
    {
        using var context = new MyContextWithFunctionMapping();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        await context.Database.ExecuteSqlRawAsync(
            @"CREATE FUNCTION dbo.BlogsWithMultiplePosts()
                        RETURNS TABLE
                        AS
                        RETURN
                        (
                            SELECT b.Url, COUNT(p.BlogId) AS PostCount
                            FROM Blogs AS b
                            JOIN Posts AS p ON b.BlogId = p.BlogId
                            GROUP BY b.BlogId, b.Url
                            HAVING COUNT(p.BlogId) > 1
                        )");

        #region ToFunctionQuery
        var query = from b in context.Set<BlogWithMultiplePosts>()
                    where b.PostCount > 3
                    select new { b.Url, b.PostCount };
        #endregion
        var result = await query.ToListAsync();
    }
}
