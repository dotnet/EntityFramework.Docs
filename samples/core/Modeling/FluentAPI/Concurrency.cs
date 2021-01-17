using Microsoft.EntityFrameworkCore;

namespace EFModeling.FluentAPI.Concurrency
{
    internal class MyContext : DbContext
    {
        public DbSet<Person> People { get; set; }

        #region Concurrency
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>()
                .Property(p => p.LastName)
                .IsConcurrencyToken();
        }
        #endregion
    }

    public class Person
    {
        public int PersonId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
    }
}
