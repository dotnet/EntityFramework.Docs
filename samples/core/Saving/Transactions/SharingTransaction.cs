using System;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EFSaving.Transactions
{
    public class SharingTransaction
    {
        public static void Run()
        {
            var connectionString =
                @"Server=(localdb)\mssqllocaldb;Database=EFSaving.Transactions;Trusted_Connection=True;ConnectRetryCount=0";

            using (var context = new BloggingContext(
                new DbContextOptionsBuilder<BloggingContext>()
                    .UseSqlServer(connectionString)
                    .Options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            #region Transaction
            using var connection = new SqlConnection(connectionString);
            var options = new DbContextOptionsBuilder<BloggingContext>()
                .UseSqlServer(connection)
                .Options;

            using var context1 = new BloggingContext(options);
            using var transaction = context1.Database.BeginTransaction();
            try
            {
                context1.Blogs.Add(new Blog { Url = "http://blogs.msdn.com/dotnet" });
                context1.SaveChanges();

                using (var context2 = new BloggingContext(options))
                {
                    context2.Database.UseTransaction(transaction.GetDbTransaction());

                    var blogs = context2.Blogs
                        .OrderBy(b => b.Url)
                        .ToList();
                }

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

        #region Context
        public class BloggingContext : DbContext
        {
            public BloggingContext(DbContextOptions<BloggingContext> options)
                : base(options)
            {
            }

            public DbSet<Blog> Blogs { get; set; }
        }
        #endregion

        public class Blog
        {
            public int BlogId { get; set; }
            public string Url { get; set; }
        }
    }
}
