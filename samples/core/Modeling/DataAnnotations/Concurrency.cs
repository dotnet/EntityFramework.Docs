using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.DataAnnotations.Concurrency
{
    internal class MyContext : DbContext
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
