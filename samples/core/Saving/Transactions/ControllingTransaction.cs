using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EFSaving.Transactions
{
    public class ControllingTransaction
    {
        public static void Run()
        {
            using (var setupContext = new BloggingContext())
            {
                setupContext.Database.EnsureDeleted();
                setupContext.Database.EnsureCreated();
            }

            #region Transaction
            using var context = new BloggingContext();
            using var transaction = context.Database.BeginTransaction();

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
            #endregion
        }

        public class BloggingContext : DbContext
        {
            public DbSet<Blog> Blogs { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=EFSaving.Transactions;Trusted_Connection=True;ConnectRetryCount=0");
            }
        }

        public class Blog
        {
            public int BlogId { get; set; }
            public string Url { get; set; }
        }
    }
}
