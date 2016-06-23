using Microsoft.EntityFrameworkCore;

namespace EFModeling.Configuring.FluentAPI.Samples.Relational.SequenceUsed
{
    class MyContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasSequence<int>("OrderNumbers", schema: "shared")
                .StartsAt(1000)
                .IncrementsBy(5);

            modelBuilder.Entity<Order>()
                .Property(o => o.OrderNo)
                .HasDefaultValueSql("NEXT VALUE FOR shared.OrderNumbers");
        }
    }

    public class Order
    {
        public int OrderId { get; set; }
        public int OrderNo { get; set; }
        public string Url { get; set; }
    }
}
