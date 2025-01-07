using System;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EFSaving.Transactions;

public class CommitableTransaction
{
    public static async Task Run()
    {
        var connectionString =
            @"Server=(localdb)\mssqllocaldb;Database=EFSaving.Transactions;Trusted_Connection=True;ConnectRetryCount=0";

        using (var context = new BloggingContext(
                   new DbContextOptionsBuilder<BloggingContext>()
                       .UseSqlServer(connectionString)
                       .Options))
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }

        #region Transaction
        using (var transaction = new CommittableTransaction(
                   new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
        {
            var connection = new SqlConnection(connectionString);

            try
            {
                var options = new DbContextOptionsBuilder<BloggingContext>()
                    .UseSqlServer(connection)
                    .Options;

                using (var context = new BloggingContext(options))
                {
                    await context.Database.OpenConnectionAsync();
                    context.Database.EnlistTransaction(transaction);

                    // Run raw ADO.NET command in the transaction
                    var command = connection.CreateCommand();
                    command.CommandText = "DELETE FROM dbo.Blogs";
                    await command.ExecuteNonQueryAsync();

                    // Run an EF Core command in the transaction
                    context.Blogs.Add(new Blog { Url = "http://blogs.msdn.com/dotnet" });
                    await context.SaveChangesAsync();
                    await context.Database.CloseConnectionAsync();
                }

                // Commit transaction if all commands succeed, transaction will auto-rollback
                // when disposed if either commands fails
                transaction.Commit();
            }
            catch (Exception)
            {
                // TODO: Handle failure
            }
        }
        #endregion
    }

    public class BloggingContext : DbContext
    {
        public BloggingContext(DbContextOptions<BloggingContext> options)
            : base(options)
        {
        }

        public DbSet<Blog> Blogs { get; set; }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }
    }
}
