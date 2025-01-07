using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.DataSeeding;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        using (var context = new ManagingDataContext())
        {
            await context.Database.EnsureCreatedAsync();

            await foreach (var country in context.Countries.Include(b => b.OfficialLanguages).AsAsyncEnumerable())
            {
                Console.WriteLine($"{country.Name} official language(s):");

                if (!country.OfficialLanguages.Any())
                {
                    Console.WriteLine("\tNone");
                }
                foreach (var language in country.OfficialLanguages)
                {
                    Console.WriteLine($"\t{language.Name} - phenomes count: {language.Details.PhonemesCount}");
                }
            }
        }

        #region CustomSeeding
        using (var context = new DataSeedingContext())
        {
            await context.Database.EnsureCreatedAsync();

            var testBlog = await context.Blogs.FirstOrDefaultAsync(b => b.Url == "http://test.com");
            if (testBlog == null)
            {
                context.Blogs.Add(new Blog { Url = "http://test.com" });
                await context.SaveChangesAsync();
            }
        }
        #endregion

        using (var context = new DataSeedingContext())
        {
            await foreach (var blog in context.Blogs.Include(b => b.Posts).AsAsyncEnumerable())
            {
                Console.WriteLine($"Blog {blog.Url}");

                foreach (var post in blog.Posts)
                {
                    Console.WriteLine($"\t{post.Title}: {post.Content}");
                }
            }
        }
    }
}
