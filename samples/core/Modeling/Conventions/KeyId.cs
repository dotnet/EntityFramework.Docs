using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EFModeling.Conventions.KeyId
{
    class MyContext : DbContext
    {
        public DbSet<Car> Cars { get; set; }
        public DbSet<Truck> Trucks { get; set; }
    }

    #region KeyId
    class Car
    {
        public string Id { get; set; }

        public string Make { get; set; }
        public string Model { get; set; }
    }

    class Truck
    {
        public string TruckId { get; set; }

        public string Make { get; set; }
        public string Model { get; set; }
    }
    #endregion
}
