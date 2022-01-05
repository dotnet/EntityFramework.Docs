using ExampleSchema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("Multiple database example.");

var tenants = new[] { "TenantA", "TenantB" };

// wiring this so sample is relevant for ASP.NET Core
var serviceProvider = new ServiceCollection()
            .AddLogging()
            .AddSingleton<TenantProvider>()
            .AddDbContextFactory<ContactContext>()
            .BuildServiceProvider();

var factory = serviceProvider.GetService<IDbContextFactory<ContactContext>>();

if (factory == null)
{
    throw new Exception("Factory should not be null.");
}

var tenantProvider = serviceProvider.GetService<TenantProvider>();

if (tenantProvider == null)
{
    throw new Exception("Tenant provider should not be null.");
}

// populate tenant "a" database
tenantProvider.Tenant = "TenantA";
using (var ctx = factory.CreateDbContext())
{
    // create database first time
    if (ctx != null && ctx.Database.EnsureCreated())
    {
        var tenantAContacts = new[] { "Jane", "John", "Joe" };
        ctx.Contacts.AddRange(
            tenantAContacts.Select(c => new Contact { Name = c }));
        ctx.SaveChanges();
    }
}

// populate tenant "a" database
tenantProvider.Tenant = "TenantB";
using (var ctx = factory.CreateDbContext())
{
    // create database first time
    if (ctx != null && ctx.Database.EnsureCreated())
    {
        var tenantBContacts = new[] { "Diane", "DeeDee", "Dan" };
        ctx.Contacts.AddRange(
            tenantBContacts.Select(c => new Contact { Name = c }));
        ctx.SaveChanges();
    }
}

foreach (var tenant in tenants)
{
    Console.WriteLine($"Setting tenant to: {tenant}");
    tenantProvider.Tenant = tenant;
    using var ctx = factory.CreateDbContext();
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
    /// Using single DbContext. You can inject any service to obtain the connection
    /// string without having to hardcode it as in this example.
    /// </summary>
    public class ContactContext : DbContext
    {
        public string Tenant { get; set; } = null!;

        public ContactContext(
            TenantProvider provider,
            DbContextOptions<ContactContext> opts)
            : base(opts) => Tenant = provider.Tenant;

        public DbSet<Contact> Contacts { get; set; } = null!;

        /// <summary>
        /// Tell EF Core to use the right database.
        /// </summary>
        /// <param name="optionsBuilder">The options builder.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .UseSqlite($"Data Source={Tenant}contacts.sqlite");
    }
}
