using Microsoft.EntityFrameworkCore;

namespace EFModeling.Configuring.FluentAPI.Samples.Relational.Sequence
{
    class MyContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasSequence<int>("OrderNumbers");
        }
    }

    public class Order
    {
        public int OrderId { get; set; }
        public int OrderNo { get; set; }
        public string Url { get; set; }
    }
}
