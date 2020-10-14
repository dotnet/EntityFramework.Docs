using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace EFSaving.Transactions
{
    public class ExternalDbTransaction
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
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            await using var transaction = connection.BeginTransaction();
            try
            {
                // Run raw ADO.NET command in the transaction
                var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText = "DELETE FROM dbo.Blogs";
                await command.ExecuteNonQueryAsync();

                // Run an EF Core command in the transaction
                var options = new DbContextOptionsBuilder<BloggingContext>()
                    .UseSqlServer(connection)
                    .Options;

                await using (var context = new BloggingContext(options))
                {
                    context.Database.UseTransaction(transaction);
                    context.Blogs.Add(new Blog { Url = "http://blogs.msdn.com/dotnet" });
                    await context.SaveChangesAsync();
                }

                // Commit transaction if all commands succeed, transaction will auto-rollback
                // when disposed if either commands fails
                await transaction.CommitAsync();
            }
            catch (System.Exception)
            {
                // TODO: Handle failure
            }
            #endregion
        }

        public class BloggingContext : DbContext
        {
            public BloggingContext(DbContextOptions<BloggingContext> options)
                : base(options)
            { }

            public DbSet<Blog> Blogs { get; set; }
        }


        public class Blog
        {
            public int BlogId { get; set; }
            public string Url { get; set; }
        }
    }
}
