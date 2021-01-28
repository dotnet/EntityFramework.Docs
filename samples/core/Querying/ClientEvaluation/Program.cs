using System;
using System.Linq;

namespace EFQuerying.ClientEvaluation
{
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

        private static void Main(string[] args)
        {
            using (var context = new BloggingContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            using (var context = new BloggingContext())
            {
                #region ClientProjection
                var blogs = context.Blogs
                    .OrderByDescending(blog => blog.Rating)
                    .Select(
                        blog => new { Id = blog.BlogId, Url = StandardizeUrl(blog.Url) })
                    .ToList();
                #endregion
            }

            using (var context = new BloggingContext())
            {
                try
                {
                    #region ClientWhere
                    var blogs = context.Blogs
                        .Where(blog => StandardizeUrl(blog.Url).Contains("dotnet"))
                        .ToList();
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
                    .AsEnumerable()
                    .Where(blog => StandardizeUrl(blog.Url).Contains("dotnet"))
                    .ToList();
                #endregion
            }
        }
    }
}
