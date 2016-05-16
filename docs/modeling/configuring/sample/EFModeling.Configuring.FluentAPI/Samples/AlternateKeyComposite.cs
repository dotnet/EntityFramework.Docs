using Microsoft.EntityFrameworkCore;

namespace EFModeling.Configuring.FluentAPI.Samples.AlternateKeyComposite
{
    class MyContext : DbContext
    {
        public DbSet<Car> Cars { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Car>()
                .HasAlternateKey(c => new { c.State, c.LicensePlate });
        }
    }

    class Car
    {
        public int CarId { get; set; }
        public string State { get; set; }
        public string LicensePlate { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
    }
}
