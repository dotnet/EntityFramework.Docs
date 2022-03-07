using Common;

namespace SingleDbSingleTable.Data
{
    public class MultitenantContact : Contact
    {
        public MultitenantContact() { }

        public MultitenantContact(Contact contact, string tenant)
        {
            IsUnicorn = contact.IsUnicorn;
            Name = contact.Name;
            Tenant = tenant;
        }
        public string Tenant { get; set; } = null!;
    }
}
