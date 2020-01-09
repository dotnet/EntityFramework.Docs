using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EFModeling.DataAnnotations.Concurrency
{
    class MyContext : DbContext
    {
        public DbSet<Person> People { get; set; }
    }

    #region Concurrency
    public class Person
    {
        public int PersonId { get; set; }

        [ConcurrencyCheck]
        public string LastName { get; set; }

        public string FirstName { get; set; }
    }
    #endregion
}
