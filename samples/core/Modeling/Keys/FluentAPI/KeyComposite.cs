using Microsoft.EntityFrameworkCore;

namespace EFModeling.Keys.FluentAPI.KeyComposite;

internal class MyContext : DbContext
{
    public DbSet<Car> Cars { get; set; }

    #region KeyComposite
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Car>()
            .HasKey(c => new { c.State, c.LicensePlate });
    }
    #endregion
}

internal class Car
{
    public string State { get; set; }
    public string LicensePlate { get; set; }

    public string Make { get; set; }
    public string Model { get; set; }
}