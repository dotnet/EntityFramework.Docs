using Microsoft.EntityFrameworkCore;

namespace EFModeling.FluentAPI.KeySingle
{
    class MyContext : DbContext
    {
        public DbSet<Car> Cars { get; set; }

        #region KeySingle
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Car>()
                .HasKey(c => c.LicensePlate);
        }
        #endregion
    }

    class Car
    {
        public string LicensePlate { get; set; }

        public string Make { get; set; }
        public string Model { get; set; }
    }
}
