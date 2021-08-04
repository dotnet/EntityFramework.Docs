using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public static class DbContextFactoryHandlesTwoConstructorsSample
{
    public static void Ignore_parameterless_constructor_when_creating_DbContext_from_factory()
    {
        Console.WriteLine($">>>> Sample: {nameof(Ignore_parameterless_constructor_when_creating_DbContext_from_factory)}");
        Console.WriteLine();

        var services = new ServiceCollection()
            .AddDbContextFactory<SomeDbContext>(
                builder => builder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFCoreSample"))
            .BuildServiceProvider();

        var factory = services.GetService<IDbContextFactory<SomeDbContext>>();

        using var context = factory.CreateDbContext();

        Console.WriteLine();
    }

    public class SomeDbContext : DbContext
    {
        public SomeDbContext()
        {
        }

        public SomeDbContext(DbContextOptions<SomeDbContext> options)
            : base(options)
        {
            Console.WriteLine($"Factory is using 'SomeDbContext(DbContextOptions<SomeDbContext> options)' constructor");
        }
    }
}
