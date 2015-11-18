using Microsoft.Data.Entity;

namespace EFModeling.Configuring.FluentAPI.Samples.Relational.AlternateKeyName
{
    class MyContext : DbContext
    {
        public DbSet<Car> Cars { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Car>()
                .HasAlternateKey(c => c.LicensePlate)
                .HasName("AlteranteKey_LicensePlate");
        }
    }

    class Car
    {
        public int CarId { get; set; }
        public string LicensePlate { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
    }
}
