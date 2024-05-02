using Microsoft.EntityFrameworkCore;

namespace EFModeling.TableSplitting;

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
        modelBuilder.Entity<DetailedOrder>(
            dob =>
            {
                dob.ToTable("Orders");
                dob.Property(o => o.Status).HasColumnName("Status");
            });

        modelBuilder.Entity<Order>(
            ob =>
            {
                ob.ToTable("Orders");
                ob.Property(o => o.Status).HasColumnName("Status");
                ob.HasOne(o => o.DetailedOrder).WithOne()
                    .HasForeignKey<DetailedOrder>(o => o.Id);
                ob.Navigation(o => o.DetailedOrder).IsRequired();
            });
        #endregion

        #region ConcurrencyToken
        modelBuilder.Entity<Order>()
            .Property<byte[]>("Version").IsRowVersion().HasColumnName("Version");

        modelBuilder.Entity<DetailedOrder>()
            .Property(o => o.Version).IsRowVersion().HasColumnName("Version");
        #endregion
    }
}
