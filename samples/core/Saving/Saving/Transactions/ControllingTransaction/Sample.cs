using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace EFSaving.Transactions.ControllingTransaction
{
    public class Sample
    {
        public static void Run()
        {
            using (var context = new BloggingContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            using (var context = new BloggingContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.Blogs.Add(new Blog { Url = "http://blogs.msdn.com/dotnet" });
                        context.SaveChanges();

                        context.Blogs.Add(new Blog { Url = "http://blogs.msdn.com/visualstudio" });
                        context.SaveChanges();

                        var blogs = context.Blogs
                            .OrderBy(b => b.Url)
                            .ToList();

                        // Commit transaction if all commands succeed, transaction will auto-rollback
                        // when disposed if either commands fails
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        // TODO: Handle failure
                    }
                }
            }
        }

        public class BloggingContext : DbContext
        {
            public DbSet<Blog> Blogs { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFSaving.Transactions;Trusted_Connection=True;ConnectRetryCount=0");
            }
        }

        public class Blog
        {
            public int BlogId { get; set; }
            public string Url { get; set; }
        }
    }
}
