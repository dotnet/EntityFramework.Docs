using Microsoft.EntityFrameworkCore;

namespace NullableReferenceTypes
{
    #region Context
    public class NullableReferenceTypesContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFNullableReferenceTypes;Trusted_Connection=True;ConnectRetryCount=0");
    }
    #endregion
}
