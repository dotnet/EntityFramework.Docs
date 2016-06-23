using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EFQuerying.ClientEval
{
    public class Sample
    {
        public static string StandardizeUrl(string url)
        {
            url = url.ToLower();

            if (!url.StartsWith("http://"))
            {
                url = string.Concat("http://", url);
            }

            return url;
        }

        public static void Run()
        {
            using (var context = new BloggingContext())
            {
                var blogs = context.Blogs
                    .OrderByDescending(blog => blog.Rating)
                    .Select(blog => new
                    {
                        Id = blog.BlogId,
                        Url = StandardizeUrl(blog.Url)
                    })
                    .ToList();
            }

            using (var context = new BloggingContext())
            {
                var blogs = context.Blogs
                    .Where(blog => StandardizeUrl(blog.Url).Contains("dotnet"))
                    .ToList();
            }
        }
    }
}
