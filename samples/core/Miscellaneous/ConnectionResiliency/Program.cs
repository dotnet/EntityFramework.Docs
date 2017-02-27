using Microsoft.EntityFrameworkCore;

namespace ConnectionResiliency
{
    class Program 
    {
        static void Main(string[] args)
        {
            using (var db = new BloggingContext())
            {
                db.Database.EnsureCreated();
            }

            #region Sample
            using (var db = new BloggingContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        db.Blogs.Add(new Blog { Url = "http://blogs.msdn.com/dotnet" });
                        db.SaveChanges();

                        db.Blogs.Add(new Blog { Url = "http://blogs.msdn.com/visualstudio" });
                        db.SaveChanges();

                        transaction.Commit();
                    }
                });
            }
            #endregion
        }
    }

    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        #region OnConfiguring
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=EFMiscellanous.ConnectionResiliency;Trusted_Connection=True;",
                    options => options.EnableRetryOnFailure());
        }
        #endregion
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }
    }
}
