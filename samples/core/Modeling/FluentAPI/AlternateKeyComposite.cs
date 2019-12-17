using Microsoft.EntityFrameworkCore;

namespace EFModeling.FluentAPI.AlternateKeyComposite
{
    class MyContext : DbContext
    {
        public DbSet<Car> Cars { get; set; }

        #region AlternateKeyComposite
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Car>()
                .HasAlternateKey(c => new { c.State, c.LicensePlate });
        }
        #endregion
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
