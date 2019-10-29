using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EFModeling.Conventions.KeyTypeNameId
{
    class MyContext : DbContext
    {
        public DbSet<Car> Cars { get; set; }
    }

    #region KeyId
    class Car
    {
        public string CarId { get; set; }

        public string Make { get; set; }
        public string Model { get; set; }
    }
    #endregion
}
