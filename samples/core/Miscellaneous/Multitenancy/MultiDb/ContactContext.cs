using Common;
using Microsoft.EntityFrameworkCore;

namespace MultiDb
{
    public class ContactContext : DbContext
    {
        private readonly ITenantService _tenantService;
        private readonly IConfiguration _configuration;

        public ContactContext(
            DbContextOptions<ContactContext> opts,
            IConfiguration config,
            ITenantService service)
            : base(opts)
        {
            _tenantService = service;
            _configuration = config;
        }        

        public DbSet<Contact> Contacts { get; set; } = null!;

        public void CheckAndSeed()
        {
            if (Database.EnsureCreated())
            {
                foreach (var contact in Contact.GeneratedContacts)
                {
                    var isTenantA = _tenantService.Tenant == "TenantA";
                    if (isTenantA == contact.IsUnicorn)
                    {
                        Contacts.Add(contact);
                    }
                }

                SaveChanges();
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var tenant = _tenantService.Tenant;
            var connectionStr = _configuration.GetConnectionString(tenant);
            optionsBuilder.UseSqlite(connectionStr);
        }
    }
}
