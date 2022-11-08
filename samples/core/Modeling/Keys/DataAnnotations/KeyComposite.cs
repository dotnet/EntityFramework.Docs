using Microsoft.EntityFrameworkCore;

namespace EFModeling.Keys.DataAnnotations.KeyComposite;

internal class MyContext : DbContext
{
    public DbSet<Car> Cars { get; set; }
}

#region KeyComposite
[PrimaryKey(nameof(State), nameof(LicensePlate))]
internal class Car
{
    public string State { get; set; }
    public string LicensePlate { get; set; }

    public string Make { get; set; }
    public string Model { get; set; }
}
#endregion