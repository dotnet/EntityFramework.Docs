using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EFQuerying.ClientEvaluation;

internal class Program
{
    #region ClientMethod
    public static string StandardizeUrl(string url)
    {
        url = url.ToLower();

        if (!url.StartsWith("http://"))
        {
            url = string.Concat("http://", url);
        }

        return url;
    }
    #endregion

    private static async Task Main(string[] args)
    {
        using (var context = new BloggingContext())
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }

        using (var context = new BloggingContext())
        {
            #region ClientProjection
            var blogs = await context.Blogs
                .OrderByDescending(blog => blog.Rating)
                .Select(
                    blog => new { Id = blog.BlogId, Url = StandardizeUrl(blog.Url) })
                .ToListAsync();
            #endregion
        }

        using (var context = new BloggingContext())
        {
            try
            {
                #region ClientWhere
                var blogs = await context.Blogs
                    .Where(blog => StandardizeUrl(blog.Url).Contains("dotnet"))
                    .ToListAsync();
                #endregion
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        using (var context = new BloggingContext())
        {
            #region ExplicitClientEvaluation
            var blogs = context.Blogs
                .AsAsyncEnumerable()
                .Where(blog => StandardizeUrl(blog.Url).Contains("dotnet"))
                .ToListAsync();
            #endregion
        }
    }
}
