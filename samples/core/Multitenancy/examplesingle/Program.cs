// See https://aka.ms/new-console-template for more information
using ExampleSingle;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("Single database and table example.");

// grab some types as sample data
#pragma warning disable CS8601 // Where clause guarantees non-nullable
var types = AppDomain.CurrentDomain.GetAssemblies()
    .SelectMany(a => a.ExportedTypes)
    .Select(t => new { t.Namespace, t.Name })
    .Where(t => !string.IsNullOrEmpty(t.Namespace))
    .Select(t => new MultitenantType
    {
        Tenant = t.Namespace,
        TypeName = t.Name
    })
    .ToList();
#pragma warning restore CS8601 // Possible null reference assignment.

// unique "tenants" = namespaces
var tenants = types.Select(t => t.Tenant).Distinct()
    .OrderBy(t => t).ToList<string>();

var provider = new TenantProvider { Tenant = tenants[0] };

// using this so the sample is relevant to ASP.NET Core apps
var serviceProvider = new ServiceCollection()
            .AddLogging()
            .AddSingleton<TenantProvider>()
            .AddDbContextFactory<SingleTenantContext>(
                opt => opt.UseSqlite("Data Source=singleexample.sqlite")
            )
            .BuildServiceProvider();

// prep the database. We're using "namespace" as tenant.
var tenantProvider = serviceProvider.GetRequiredService<TenantProvider>();
tenantProvider.Tenant = tenants[0];

var factory = serviceProvider
    .GetRequiredService<IDbContextFactory<SingleTenantContext>>();

using (var ctx = factory.CreateDbContext())
{
    ctx.Database.EnsureDeleted();
    ctx.Database.EnsureCreated();
    ctx.MultitenantTypes.AddRange(types);
    ctx.SaveChanges();
}

var done = false;

while (!done)
{
    for (var x = 0; x < tenants.Count; x++)
    {
        Console.WriteLine($"{x} - {tenants[x]}");
    }

    Console.WriteLine("Choose your tenant, or enter to exit.");

    var tenantIdx = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(tenantIdx))
    {
        done = true;
        continue;
    }

    if (int.TryParse(tenantIdx, out var idx) && idx >= 0 && idx < tenants.Count)
    {
        tenantProvider.Tenant = tenants[idx];

        Console.WriteLine($"Set tenant to {tenants[idx]}. Press ENTER to query.");
        Console.ReadLine();

        using var ctx = factory.CreateDbContext();

        foreach (var type in ctx.MultitenantTypes)
        {
            Console.Write($"{type.Tenant}.{type.TypeName}\t");
        }

        Console.WriteLine();
        Console.WriteLine("Press ENTER to continue.");
        Console.ReadLine();
    }
}

namespace ExampleSingle
{
    /// <summary>
    /// Simple tenant service.
    /// </summary>
    public class TenantProvider
    {
        public string Tenant { get; set; } = string.Empty;
    }

    public class MultitenantType
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Tenant { get; set; } = string.Empty;

        public string? TypeName { get; set; }
    }

    public class SingleTenantContext : DbContext
    {
        private readonly string _tenant = string.Empty;

        public SingleTenantContext(
            DbContextOptions<SingleTenantContext> opts,
            TenantProvider provider)
            : base(opts) => _tenant = provider.Tenant;
        public DbSet<MultitenantType> MultitenantTypes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<MultitenantType>()
                    .HasQueryFilter(mt => mt.Tenant == _tenant);
    }
}
