using Microsoft.EntityFrameworkCore;

namespace OwnedEntities
{
    public class OwnedEntityContext : DbContext
    {
        public DbSet<Order> Order { get; set; }
        public DbSet<DetailedOrder> DetailedOrders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFOwnedEntity;Trusted_Connection=True;ConnectRetryCount=0");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region OwnsOne
            modelBuilder.Entity<Order>().OwnsOne(p => p.ShippingAddress);
            #endregion

            #region OwnsOneString
            modelBuilder.Entity<Order>().OwnsOne(typeof(StreetAddress), "ShippingAddress");
            #endregion

            #region ColumnNames
            modelBuilder.Entity<Order>().OwnsOne(
                o => o.ShippingAddress,
                sa =>
                {
                    sa.Property(p => p.Street).HasColumnName("ShipsToStreet");
                    sa.Property(p => p.City).HasColumnName("ShipsToCity");
                });
            #endregion

            #region OwnsOneNested
            modelBuilder.Entity<DetailedOrder>().OwnsOne(p => p.OrderDetails, od =>
            {
                od.OwnsOne(c => c.BillingAddress);
                od.OwnsOne(c => c.ShippingAddress);
            });
            #endregion

            #region OwnsOneTable
            modelBuilder.Entity<DetailedOrder>().OwnsOne(p => p.OrderDetails, od =>
            {
                od.OwnsOne(c => c.BillingAddress);
                od.OwnsOne(c => c.ShippingAddress);
                od.ToTable("OrderDetails");
            });
            #endregion

            #region OwnsMany
            modelBuilder.Entity<Distributor>().OwnsMany(p => p.ShippingCenters, a =>
            {
                a.HasForeignKey("DistributorId");
                a.Property<int>("Id");
                a.HasKey("DistributorId", "Id");
            });
            #endregion
        }
    }
}
