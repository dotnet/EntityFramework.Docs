using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EFSaving.Transactions;

public class ControllingTransaction
{
    public static async Task Run()
    {
        using (var setupContext = new BloggingContext())
        {
            await setupContext.Database.EnsureDeletedAsync();
            await setupContext.Database.EnsureCreatedAsync();
        }

        #region Transaction
        using var context = new BloggingContext();
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            context.Blogs.Add(new Blog { Url = "http://blogs.msdn.com/dotnet" });
            await context.SaveChangesAsync();

            context.Blogs.Add(new Blog { Url = "http://blogs.msdn.com/visualstudio" });
            await context.SaveChangesAsync();

            var blogs = await context.Blogs
                .OrderBy(b => b.Url)
                .ToListAsync();

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
