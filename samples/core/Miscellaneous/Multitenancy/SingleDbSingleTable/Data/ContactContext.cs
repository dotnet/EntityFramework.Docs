using Common;
using Microsoft.EntityFrameworkCore;

namespace SingleDbSingleTable.Data
{
    public class ContactContext : DbContext
    {
        private readonly string _tenant = string.Empty;

        public ContactContext(
            DbContextOptions<ContactContext> opts,
            ITenantService service)
            : base(opts) => _tenant = service.Tenant;

        public DbSet<MultitenantContact> Contacts { get; set; } = null!;

        public void CheckAndSeed()
        {
            if (Database.EnsureCreated())
            {
                foreach (var contact in Contact.GeneratedContacts)
                {
                    var tenant = contact.IsUnicorn ? "TenantA" : "TenantB";
                    Contacts.Add(new MultitenantContact(contact, tenant));
                }

                SaveChanges();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<MultitenantContact>()
                .HasQueryFilter(mt => mt.Tenant == _tenant);
    }
}
