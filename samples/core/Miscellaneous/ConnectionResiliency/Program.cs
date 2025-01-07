using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.EntityFrameworkCore;

namespace EFConnectionResiliency;

public class Program
{
    public static async Task Main(string[] args)
    {
        using (var db = new BloggingContext())
        {
            await db.Database.EnsureDeletedAsync();
            await db.Database.EnsureCreatedAsync();
        }

        await ExecuteWithManualTransaction();

        await ExecuteWithManualAmbientTransaction();

        await ExecuteInTransactionWithVerification();

        await ExecuteInTransactionWithTracking();
    }

    private static async Task ExecuteWithManualTransaction()
    {
        #region ManualTransaction

        using var db = new BloggingContext();
        var strategy = db.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(
            async () =>
            {
                using var context = new BloggingContext();
                await using var transaction = await context.Database.BeginTransactionAsync();

                context.Blogs.Add(new Blog { Url = "http://blogs.msdn.com/dotnet" });
                await context.SaveChangesAsync();

                context.Blogs.Add(new Blog { Url = "http://blogs.msdn.com/visualstudio" });
                await context.SaveChangesAsync();

                await transaction.CommitAsync();
            });

        #endregion
    }

    private static async Task ExecuteWithManualAmbientTransaction()
    {
        #region AmbientTransaction

        using var context1 = new BloggingContext();
        context1.Blogs.Add(new Blog { Url = "http://blogs.msdn.com/visualstudio" });

        var strategy = context1.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(
            async () =>
            {
                using var context2 = new BloggingContext();
                using var transaction = new TransactionScope();

                context2.Blogs.Add(new Blog { Url = "http://blogs.msdn.com/dotnet" });
                await context2.SaveChangesAsync();

                await context1.SaveChangesAsync();

                transaction.Complete();
            });

        #endregion
    }

    private static async Task ExecuteInTransactionWithVerification()
    {
        #region Verification

        using var db = new BloggingContext();
        var strategy = db.Database.CreateExecutionStrategy();

        var blogToAdd = new Blog { Url = "http://blogs.msdn.com/dotnet" };
        db.Blogs.Add(blogToAdd);

        await strategy.ExecuteInTransactionAsync(
            db,
            operation: (context, cancellationToken) => context.SaveChangesAsync(acceptAllChangesOnSuccess: false, cancellationToken),
            verifySucceeded: (context, cancellationToken) => context.Blogs.AsNoTracking().AnyAsync(b => b.BlogId == blogToAdd.BlogId, cancellationToken));

        db.ChangeTracker.AcceptAllChanges();

        #endregion
    }

    private static async Task ExecuteInTransactionWithTracking()
    {
        #region Tracking

        using var db = new BloggingContext();
        var strategy = db.Database.CreateExecutionStrategy();

        db.Blogs.Add(new Blog { Url = "http://blogs.msdn.com/dotnet" });

        var transaction = new TransactionRow { Id = Guid.NewGuid() };
        db.Transactions.Add(transaction);

        await strategy.ExecuteInTransactionAsync(
            db,
            operation: (context, cancellationToken) => context.SaveChangesAsync(acceptAllChangesOnSuccess: false, cancellationToken),
            verifySucceeded: (context, cancellationToken) => context.Transactions.AsNoTracking().AnyAsync(t => t.Id == transaction.Id, cancellationToken));

        db.ChangeTracker.AcceptAllChanges();
        db.Transactions.Remove(transaction);
        await db.SaveChangesAsync();

        #endregion
    }
}

public class BloggingContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<TransactionRow> Transactions { get; set; }

    #region OnConfiguring
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSqlServer(
                @"Server=(localdb)\mssqllocaldb;Database=EFMiscellanous.ConnectionResiliency;Trusted_Connection=True;ConnectRetryCount=0",
                options => options.EnableRetryOnFailure());
    }
    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>().Property(b => b.BlogId).UseHiLo();
    }
}

public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }
}

public class TransactionRow
{
    public Guid Id { get; set; }
}
