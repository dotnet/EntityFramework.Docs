using Microsoft.EntityFrameworkCore;

namespace EFModeling.Configuring.FluentAPI.Samples.Relational.ComputedColumn
{
    class MyContext : DbContext
    {
        public DbSet<Person> People { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>()
                .Property(p => p.DisplayName)
                .HasComputedColumnSql("[LastName] + ', ' + [FirstName]");
        }
    }

    public class Person
    {
        public int PersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
    }
}
