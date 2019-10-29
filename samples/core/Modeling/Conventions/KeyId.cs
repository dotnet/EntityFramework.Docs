using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EFModeling.Conventions.KeyId
{
    class MyContext : DbContext
    {
        public DbSet<Car> Cars { get; set; }
    }

    #region KeyId
    class Car
    {
        public string Id { get; set; }

        public string Make { get; set; }
        public string Model { get; set; }
    }
    #endregion
}
