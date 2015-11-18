using Microsoft.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace EFModeling.Configuring.DataAnnotations.Samples.Concurrency
{
    class MyContext : DbContext
    {
        public DbSet<Person> People { get; set; }
    }

    public class Person
    {
        public int PersonId { get; set; }
        [ConcurrencyCheck]
        public string SocialSecurityNumber { get; set; }
        public string Name { get; set; }
    }
}
