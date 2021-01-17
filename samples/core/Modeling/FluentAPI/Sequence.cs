using Microsoft.EntityFrameworkCore;

namespace EFModeling.FluentAPI.Relational.Sequence
{
    internal class MyContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }

        #region Sequence
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasSequence<int>("OrderNumbers");

            modelBuilder.Entity<Order>()
                .Property(o => o.OrderNo)
                .HasDefaultValueSql("NEXT VALUE FOR shared.OrderNumbers");
        }
        #endregion
    }

    public class Order
    {
        public int OrderId { get; set; }
        public int OrderNo { get; set; }
        public string Url { get; set; }
    }
}
