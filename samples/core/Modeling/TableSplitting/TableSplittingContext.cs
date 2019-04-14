using Microsoft.EntityFrameworkCore;

namespace TableSplitting
{
    public class TableSplittingContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<DetailedOrder> DetailedOrders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFTableSplitting;Trusted_Connection=True;ConnectRetryCount=0");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region TableSplitting
            modelBuilder.Entity<DetailedOrder>()
                .ToTable("Orders")
                .HasBaseType((string)null)
                .Ignore(o => o.DetailedOrder);

            modelBuilder.Entity<Order>()
                .ToTable("Orders")
                .HasOne(o => o.DetailedOrder).WithOne()
                .HasForeignKey<Order>(o => o.Id);
            #endregion

            #region ConcurrencyToken
            modelBuilder.Entity<Order>()
                .Property<byte[]>("Version").IsRowVersion().HasColumnName("Version");

            modelBuilder.Entity<DetailedOrder>()
                .Property(o => o.Version).IsRowVersion().HasColumnName("Version");
            #endregion
        }
    }
}
