using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EFQuerying.Overview;

internal class Program
{
    private static async Task Main(string[] args)
    {
        using (var context = new BloggingContext())
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }

        #region LoadingAllData
        using (var context = new BloggingContext())
        {
            var blogs = await context.Blogs.ToListAsync();
        }
        #endregion

        #region LoadingSingleEntity
        using (var context = new BloggingContext())
        {
            var blog = await context.Blogs
                .SingleAsync(b => b.BlogId == 1);
        }
        #endregion

        #region Filtering
        using (var context = new BloggingContext())
        {
            var blogs = await context.Blogs
                .Where(b => b.Url.Contains("dotnet"))
                .ToListAsync();
        }
        #endregion
    }
}
