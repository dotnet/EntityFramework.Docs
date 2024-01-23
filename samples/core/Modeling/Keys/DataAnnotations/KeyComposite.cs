using Microsoft.EntityFrameworkCore;

namespace EFModeling.Keys.DataAnnotations.KeyComposite;

class MyContext : DbContext
{
    public DbSet<Car> Cars { get; set; }
}

#region KeyComposite
[PrimaryKey(nameof(State), nameof(LicensePlate))]
class Car
{
    public string State { get; set; }
    public string LicensePlate { get; set; }

    public string Make { get; set; }
    public string Model { get; set; }
}
#endregion