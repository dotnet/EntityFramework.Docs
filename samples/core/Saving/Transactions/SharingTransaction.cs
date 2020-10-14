using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Data.SqlClient;

namespace EFSaving.Transactions
{
    public class SharingTransaction
    {
        public static async Task RunAsync()
        {
            var connectionString = @"Server=(localdb)\mssqllocaldb;Database=EFSaving.Transactions;Trusted_Connection=True;ConnectRetryCount=0";

            await using (var context = new BloggingContext(
                new DbContextOptionsBuilder<BloggingContext>()
                    .UseSqlServer(connectionString)
                    .Options))
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();
            }

            #region Transaction
            var options = new DbContextOptionsBuilder<BloggingContext>()
                .UseSqlServer(new SqlConnection(connectionString))
                .Options;

            await using var context1 = new BloggingContext(options);
            await using var transaction = await context1.Database.BeginTransactionAsync();
            try
            {
                context1.Blogs.Add(new Blog { Url = "http://blogs.msdn.com/dotnet" });
                await context1.SaveChangesAsync();

                await using (var context2 = new BloggingContext(options))
                {
                    context2.Database.UseTransaction(transaction.GetDbTransaction());

                    var blogs = await context2.Blogs
                        .OrderBy(b => b.Url)
                        .ToListAsync();
                }

                // Commit transaction if all commands succeed, transaction will auto-rollback
                // when disposed if either commands fails
                await transaction.CommitAsync();
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
            { }

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
