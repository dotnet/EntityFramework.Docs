using System;
using System.Linq;
using System.Transactions;
using Microsoft.EntityFrameworkCore;

namespace EFConnectionResiliency
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var db = new BloggingContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }

            ExecuteWithManualTransaction();

            ExecuteWithManualAmbientTransaction();

            ExecuteInTransactionWithVerification();

            ExecuteInTransactionWithTracking();
        }

        private static void ExecuteWithManualTransaction()
        {
            #region ManualTransaction
            using (var db = new BloggingContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(
                    () =>
                    {
                        using (var context = new BloggingContext())
                        {
                            using (var transaction = context.Database.BeginTransaction())
                            {
                                context.Blogs.Add(new Blog { Url = "http://blogs.msdn.com/dotnet" });
                                context.SaveChanges();

                                context.Blogs.Add(new Blog { Url = "http://blogs.msdn.com/visualstudio" });
                                context.SaveChanges();

                                transaction.Commit();
                            }
                        }
                    });
            }
            #endregion
        }

        private static void ExecuteWithManualAmbientTransaction()
        {
            #region AmbientTransaction
            using (var context1 = new BloggingContext())
            {
                context1.Blogs.Add(new Blog { Url = "http://blogs.msdn.com/visualstudio" });

                var strategy = context1.Database.CreateExecutionStrategy();

                strategy.Execute(
                    () =>
                    {
                        using (var context2 = new BloggingContext())
                        {
                            using (var transaction = new TransactionScope())
                            {
                                context2.Blogs.Add(new Blog { Url = "http://blogs.msdn.com/dotnet" });
                                context2.SaveChanges();

                                context1.SaveChanges();

                                transaction.Complete();
                            }
                        }
                    });
            }
            #endregion
        }

        private static void ExecuteInTransactionWithVerification()
        {
            #region Verification
            using (var db = new BloggingContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                var blogToAdd = new Blog { Url = "http://blogs.msdn.com/dotnet" };
                db.Blogs.Add(blogToAdd);

                strategy.ExecuteInTransaction(
                    db,
                    operation: context => { context.SaveChanges(acceptAllChangesOnSuccess: false); },
                    verifySucceeded: context => context.Blogs.AsNoTracking().Any(b => b.BlogId == blogToAdd.BlogId));

                db.ChangeTracker.AcceptAllChanges();
            }
            #endregion
        }

        private static void ExecuteInTransactionWithTracking()
        {
            #region Tracking
            using (var db = new BloggingContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                db.Blogs.Add(new Blog { Url = "http://blogs.msdn.com/dotnet" });

                var transaction = new TransactionRow { Id = Guid.NewGuid() };
                db.Transactions.Add(transaction);

                strategy.ExecuteInTransaction(
                    db,
                    operation: context => { context.SaveChanges(acceptAllChangesOnSuccess: false); },
                    verifySucceeded: context => context.Transactions.AsNoTracking().Any(t => t.Id == transaction.Id));

                db.ChangeTracker.AcceptAllChanges();
                db.Transactions.Remove(transaction);
                db.SaveChanges();
            }
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
}
