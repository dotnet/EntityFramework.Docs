using ExampleSchema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("Multiple schema example.");

var tenants = new[] { "TenantA", "TenantB" };

// wiring this so sample is relevant for ASP.NET Core
var serviceProvider = new ServiceCollection()
            .AddLogging()
            .AddSingleton<TenantProvider>()
            .AddDbContextFactory<ContactContext>(
                opt => opt.UseSqlite("Data Source=schemaexample.sqlite")
            )
            .AddDbContextFactory<AlternateContactContext>(
                (sp, opt) => opt.UseSqlite("Data Source=schemaexample.sqlite")
            )
            .BuildServiceProvider();

var factory = serviceProvider.GetService<IDbContextFactory<ContactContext>>();

if (factory == null)
{
    throw new Exception("Factory should not be null.");
}

using (var ctx = factory.CreateDbContext())
{
    // create database first time
    if (ctx != null && ctx.Database.EnsureCreated())
    {
        var tenantAContacts = new[] { "Jane", "John", "Joe" };
        var tenantBContacts = new[] { "Diane", "DeeDee", "Dan" };
        ctx.TenantAContacts.AddRange(
            tenantAContacts.Select(c => new TenantAContact { Name = c }));
        ctx.TenantBContacts.AddRange(
            tenantBContacts.Select(c => new TenantBContact { Name = c }));
        ctx.SaveChanges();
    }
}

var provider = serviceProvider.GetService<TenantProvider>();

if (provider == null)
{
    throw new Exception("Provider should not be null.");
}

foreach (var tenant in tenants)
{
    Console.WriteLine($"Setting tenant to: {tenant}");
    provider.Tenant = tenant;
    using var ctx = factory.CreateDbContext();
    var contacts = ctx.Contacts.OrderBy(c => c.Name).Select(c => c.Name);
    var contactList = string.Join(", ", contacts);
    Console.WriteLine($"Loaded contacts: {contactList}");
}

Console.WriteLine("Alternate approach.");

var alternateFactory = serviceProvider
    .GetService<IDbContextFactory<AlternateContactContext>>();

if (alternateFactory == null)
{
    throw new Exception("Alternate factory should not be null.");
}

foreach (var tenant in tenants)
{
    Console.WriteLine($"Setting tenant to: {tenant}");
    provider.Tenant = tenant;
    using var ctx = alternateFactory.CreateDbContext();
    var contacts = ctx.Contacts.OrderBy(c => c.Name).Select(c => c.Name);
    var contactList = string.Join(", ", contacts);
    Console.WriteLine($"Loaded contacts: {contactList}");
}

namespace ExampleSchema
{
    /// <summary>
    /// Simple tenant service.
    /// </summary>
    public class TenantProvider
    {
        public string Tenant { get; set; } = string.Empty;
    }

    /// <summary>
    /// Contact.
    /// </summary>
    public class Contact
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// Derived class for tenant-specific support.
    /// </summary>
    public class TenantAContact : Contact { }

    /// <summary>
    /// Derived class for tenant-specific support.
    /// </summary>
    public class TenantBContact : Contact { }

    /// <summary>
    /// Using IQueryable.
    /// </summary>
    public class ContactContext : DbContext
    {
        private readonly string _tenant;

        public ContactContext(
            TenantProvider provider,
            DbContextOptions<ContactContext> opts)
            : base(opts) => _tenant = provider.Tenant;

        public DbSet<TenantAContact> TenantAContacts { get; set; } = null!;
        public DbSet<TenantBContact> TenantBContacts { get; set; } = null!;

        public IQueryable<Contact> Contacts => _tenant == "TenantA" ?
            TenantAContacts : TenantBContacts;

        // SQLite doesn't handle multiple schemas, but for SQLServer you
        // can uncomment the code below for TenantaA.Contact and TenantB.Contact
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<TenantAContact>()
            //    .ToTable(nameof(Contact), "TenantA");

            //modelBuilder.Entity<TenantBContact>()
            //    .ToTable(nameof(Contact), "TenantB");

            base.OnModelCreating(modelBuilder);
        }
    }

    /// <summary>
    /// Using multiple models with schema/name overrides.
    /// </summary>
    public class AlternateContactContext : DbContext
    {
        public string Tenant { get; set; } = null!;

        public AlternateContactContext(
            TenantProvider provider,
            DbContextOptions<AlternateContactContext> opts)
            : base(opts) => Tenant = provider.Tenant;

        public DbSet<Contact> Contacts { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
         =>
            // SQLite doesn't handle multiple schemas, so we use different
            // table names but the same API can be used with SQLServer for
            // same table name, different schema
            modelBuilder.Entity<Contact>().ToTable($"{Tenant}Contacts");

        /// <summary>
        /// Tell EF Core to cache the model by tenant.
        /// </summary>
        /// <param name="optionsBuilder">The options builder.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .ReplaceService<IModelCacheKeyFactory, TenantModelCacheKeyFactory>();
    }

    /// <summary>
    /// Implemented to support a model per tenant in the same DbContext.
    /// </summary>
    public class TenantModelCacheKeyFactory : IModelCacheKeyFactory
    {
        public object Create(DbContext context)
            => context is AlternateContactContext altContext
                ? (context.GetType(), altContext.Tenant)
                : (object)context.GetType();
    }
}
