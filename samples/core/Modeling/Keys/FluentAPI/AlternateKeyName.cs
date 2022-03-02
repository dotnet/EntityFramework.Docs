using Microsoft.EntityFrameworkCore;

namespace EFModeling.Keys.FluentAPI.AlternateKeyName;

internal class MyContext : DbContext
{
    public DbSet<Car> Cars { get; set; }

    #region AlternateKeyName
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Car>()
            .HasAlternateKey(c => c.LicensePlate)
            .HasName("AlternateKey_LicensePlate");
    }
    #endregion
}

internal class Car
{
    public int CarId { get; set; }
    public string LicensePlate { get; set; }
    public string Make { get; set; }
    public string Model { get; set; }
}