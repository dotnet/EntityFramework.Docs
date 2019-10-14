using Microsoft.EntityFrameworkCore;

namespace EFModeling.OwnedEntities
{
    public class OwnedEntityContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFOwnedEntity;Trusted_Connection=True;ConnectRetryCount=0");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region OwnsOne
            modelBuilder.Entity<Order>().OwnsOne(p => p.ShippingAddress);
            #endregion

            // Uncomment following line and it starts working
            #region OwnsOne
            //// modelBuilder.Entity<Order2>().OwnsOne(p => p.ShippingAddress);
            #endregion


        }
    }
}
