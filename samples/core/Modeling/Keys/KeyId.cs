using Microsoft.EntityFrameworkCore;

namespace EFModeling.Keys.KeyId;

internal class MyContext : DbContext
{
    public DbSet<Car> Cars { get; set; }
    public DbSet<Truck> Trucks { get; set; }
}

#region KeyId
internal class Car
{
    public string Id { get; set; }

    public string Make { get; set; }
    public string Model { get; set; }
}

internal class Truck
{
    public string TruckId { get; set; }

    public string Make { get; set; }
    public string Model { get; set; }
}
#endregion