using System.Linq;
using EFModeling.Conventions.EntityTypes;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.Conventions
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using (var context = new MyContextWithFunctionMapping())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                context.Database.ExecuteSqlRaw(
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
                var result = query.ToList();
            }
        }
    }
}
