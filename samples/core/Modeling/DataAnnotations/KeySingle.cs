using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EFModeling.DataAnnotations.KeySingle
{
    class MyContext : DbContext
    {
        public DbSet<Car> Cars { get; set; }
    }

    #region KeySingle
    class Car
    {
        [Key]
        public string LicensePlate { get; set; }

        public string Make { get; set; }
        public string Model { get; set; }
    }
    #endregion
}
