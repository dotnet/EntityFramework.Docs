using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;

namespace EFSaving.Transactions.ExternalDbTransaction
{
    public class Sample
    {
        public static void Run()
        {
            var connectionString = @"Server=(localdb)\mssqllocaldb;Database=EFSaving.Transactions;Trusted_Connection=True;ConnectRetryCount=0";

            using (var context = new BloggingContext(
                new DbContextOptionsBuilder<BloggingContext>()
                    .UseSqlServer(connectionString)
                    .Options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            var connection = new SqlConnection(connectionString);
            connection.Open();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // Run raw ADO.NET command in the transaction
                    var command = connection.CreateCommand();
                    command.Transaction = transaction;
                    command.CommandText = "DELETE FROM dbo.Blogs";
                    command.ExecuteNonQuery();

                    // Run an EF Core command in the transaction
                    var options = new DbContextOptionsBuilder<BloggingContext>()
                        .UseSqlServer(connection)
                        .Options;

                    using (var context = new BloggingContext(options))
                    {
                        context.Database.UseTransaction(transaction);
                        context.Blogs.Add(new Blog { Url = "http://blogs.msdn.com/dotnet" });
                        context.SaveChanges();
                    }

                    // Commit transaction if all commands succeed, transaction will auto-rollback
                    // when disposed if either commands fails
                    transaction.Commit();
                }
                catch (System.Exception)
                {
                    // TODO: Handle failure
                }
            }
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
