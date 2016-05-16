using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EFModeling.Conventions.Samples.KeyTypeNameId
{
    class MyContext : DbContext
    {
        public DbSet<Car> Cars { get; set; }
    }

    class Car
    {
        public string CarId { get; set; }

        public string Make { get; set; }
        public string Model { get; set; }
    }
}
