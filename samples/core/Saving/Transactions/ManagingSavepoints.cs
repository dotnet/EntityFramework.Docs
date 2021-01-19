using System;
using Microsoft.EntityFrameworkCore;

namespace EFSaving.Transactions
{
    public class ManagingSavepoints
    {
        public static void Run()
        {
            using (var setupContext = new BloggingContext())
            {
                setupContext.Database.EnsureDeleted();
                setupContext.Database.EnsureCreated();
            }

            #region Savepoints
            using var context = new BloggingContext();
            using var transaction = context.Database.BeginTransaction();

            try
            {
                context.Blogs.Add(new Blog { Url = "https://devblogs.microsoft.com/dotnet/" });
                context.SaveChanges();

                transaction.CreateSavepoint("BeforeMoreBlogs");

                context.Blogs.Add(new Blog { Url = "https://devblogs.microsoft.com/visualstudio/" });
                context.Blogs.Add(new Blog { Url = "https://devblogs.microsoft.com/aspnet/" });
                context.SaveChanges();

                transaction.Commit();
            }
            catch (Exception)
            {
                // If a failure occurred, we rollback to the savepoint and can continue the transaction
                transaction.RollbackToSavepoint("BeforeMoreBlogs");

                // TODO: Handle failure, possibly retry inserting blogs
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
