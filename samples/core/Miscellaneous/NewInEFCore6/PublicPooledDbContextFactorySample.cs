using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

public static class PublicPooledDbContextFactorySample
{
    public static void Can_create_pooled_DbContext_factory()
    {
        Console.WriteLine($">>>> Sample: {nameof(Can_create_pooled_DbContext_factory)}");
        Console.WriteLine();

        #region CreatePool
        var options = new DbContextOptionsBuilder<SomeDbContext>()
            .EnableSensitiveDataLogging()
            .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFCoreSample;ConnectRetryCount=0")
            .Options;

        var factory = new PooledDbContextFactory<SomeDbContext>(options);
        #endregion

        #region UsePool
        for (var i = 0; i < 2; i++)
        {
            using var context1 = factory.CreateDbContext();
            Console.WriteLine($"Created DbContext with ID {context1.ContextId}");

            using var context2 = factory.CreateDbContext();
            Console.WriteLine($"Created DbContext with ID {context2.ContextId}");
        }
        #endregion

        Console.WriteLine();
    }

    public class SomeDbContext : DbContext
    {
        public SomeDbContext(DbContextOptions<SomeDbContext> options)
            : base(options)
        {
        }

        public override void Dispose()
        {
            base.Dispose();

            Console.WriteLine($"Disposing DbContext with ID {ContextId}");
        }
    }
}
