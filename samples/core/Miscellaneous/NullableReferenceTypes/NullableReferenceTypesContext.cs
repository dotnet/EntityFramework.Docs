using Microsoft.EntityFrameworkCore;

namespace NullableReferenceTypes
{
    #region Context
    public class NullableReferenceTypesContext : DbContext
    {
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Order> Orders => Set<Order>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=EFNullableReferenceTypes;Trusted_Connection=True;ConnectRetryCount=0");
    }
    #endregion
}
