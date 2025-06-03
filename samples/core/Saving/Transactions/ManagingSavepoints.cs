using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EFSaving.Transactions;

public class ManagingSavepoints
{
    public static async Task Run()
    {
        using (var setupContext = new BloggingContext())
        {
            await setupContext.Database.EnsureDeletedAsync();
            await setupContext.Database.EnsureCreatedAsync();
        }

        #region Savepoints
        using var context = new BloggingContext();
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            context.Blogs.Add(new Blog { Url = "https://devblogs.microsoft.com/dotnet/" });
            await context.SaveChangesAsync();

            await transaction.CreateSavepointAsync("BeforeMoreBlogs");

            context.Blogs.Add(new Blog { Url = "https://devblogs.microsoft.com/visualstudio/" });
            context.Blogs.Add(new Blog { Url = "https://devblogs.microsoft.com/aspnet/" });
            await context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            // If a failure occurred, we rollback to the savepoint and can continue the transaction
            await transaction.RollbackToSavepointAsync("BeforeMoreBlogs");

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
