using Microsoft.EntityFrameworkCore;

namespace EFModeling.Configuring.FluentAPI.Samples.KeySingle
{
    class MyContext : DbContext
    {
        public DbSet<Car> Cars { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Car>()
                .HasKey(c => c.LicensePlate);
        }
    }

    class Car
    {
        public string LicensePlate { get; set; }

        public string Make { get; set; }
        public string Model { get; set; }
    }
}
