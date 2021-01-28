using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.DataAnnotations.KeySingle
{
    internal class MyContext : DbContext
    {
        public DbSet<Car> Cars { get; set; }
    }

    #region KeySingle
    internal class Car
    {
        [Key]
        public string LicensePlate { get; set; }

        public string Make { get; set; }
        public string Model { get; set; }
    }
    #endregion
}
