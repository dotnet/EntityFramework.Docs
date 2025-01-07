using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public static class DbContextFactorySample
{
    public static void Ignore_parameterless_constructor_when_creating_DbContext_from_factory()
    {
        Console.WriteLine($">>>> Sample: {nameof(Ignore_parameterless_constructor_when_creating_DbContext_from_factory)}");
        Console.WriteLine();

        var services = new ServiceCollection()
            .AddDbContextFactory<SomeDbContext>(
                builder => builder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFCoreSample;ConnectRetryCount=0"))
            .BuildServiceProvider();

        var factory = services.GetService<IDbContextFactory<SomeDbContext>>();

        using var context = factory.CreateDbContext();

        Console.WriteLine();
    }

    public static void AddDbContextFactory_also_registers_scoped_DbContext_instance()
    {
        Console.WriteLine($">>>> Sample: {nameof(AddDbContextFactory_also_registers_scoped_DbContext_instance)}");
        Console.WriteLine();

        var services = new ServiceCollection();

        #region Registration
        var container = services
            .AddDbContextFactory<SomeDbContext>(
                builder => builder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFCoreSample;ConnectRetryCount=0"))
            .BuildServiceProvider();
        #endregion

        // Factory can be obtained from the root container
        #region ResolveFactory
        var factory = container.GetService<IDbContextFactory<SomeDbContext>>();
        using (var context = factory.CreateDbContext())
        {
            // Contexts obtained from the factory must be explicitly disposed
        }
        #endregion

        // DbContext can only be obtained from a container scope
        #region ResolveContext
        using (var scope = container.CreateScope())
        {
            var context = scope.ServiceProvider.GetService<SomeDbContext>();
            // Context is disposed when the scope is disposed
        }
        #endregion

        Console.WriteLine();
    }

    #region InjectContext
    private class MyController1
    {
        private readonly SomeDbContext _context;

        public MyController1(SomeDbContext context)
        {
            _context = context;
        }

        public async Task DoSomething()
        {
            var results = await _context.Blogs.ToListAsync();

            // Injected context is disposed when the request scope is disposed
        }
    }
    #endregion

    #region InjectFactory
    private class MyController2
    {
        private readonly IDbContextFactory<SomeDbContext> _contextFactory;

        public MyController2(IDbContextFactory<SomeDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task DoSomething()
        {
            using var context1 = _contextFactory.CreateDbContext();
            using var context2 = _contextFactory.CreateDbContext();

            var results1 = await context1.Blogs.ToListAsync();
            var results2 = await context2.Blogs.ToListAsync();

            // Contexts obtained from the factory must be explicitly disposed
        }
    }
    #endregion

    public class Blog
    {
        public int Id { get; set; }
    }

    #region Context
    public class SomeDbContext : DbContext
    {
        public SomeDbContext()
        {
        }

        public SomeDbContext(DbContextOptions<SomeDbContext> options)
            : base(options)
        {
        }

        public DbSet<Blog> Blogs { get; set; }
    }
    #endregion
}
