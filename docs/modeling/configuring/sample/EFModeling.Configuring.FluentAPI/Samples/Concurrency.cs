using Microsoft.EntityFrameworkCore;

namespace EFModeling.Configuring.FluentAPI.Samples.Concurrency
{
    class MyContext : DbContext
    {
        public DbSet<Person> People { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>()
                .Property(p => p.SocialSecurityNumber)
                .IsConcurrencyToken();
        }
    }

    public class Person
    {
        public int PersonId { get; set; }
        public string SocialSecurityNumber { get; set; }
        public string Name { get; set; }
    }
}
