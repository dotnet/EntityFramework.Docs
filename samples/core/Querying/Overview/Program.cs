using System.Linq;

namespace EFQuerying.Overview
{
    class Program
    {
        static void Main(string[] args)
        {
            #region LoadingAllData
            using (var context = new BloggingContext())
            {
                var blogs = context.Blogs.ToList();
            }
            #endregion

            #region LoadingSingleEntity
            using (var context = new BloggingContext())
            {
                var blog = context.Blogs
                    .Single(b => b.BlogId == 1);
            }
            #endregion

            #region Filtering
            using (var context = new BloggingContext())
            {
                var blogs = context.Blogs
                    .Where(b => b.Url.Contains("dotnet"))
                    .ToList();
            }
            #endregion
        }
    }
}
