using Microsoft.EntityFrameworkCore;

namespace EFModeling.FluentAPI.Relational.ComputedColumn
{
    class MyContext : DbContext
    {
        public DbSet<Person> People { get; set; }

        #region ComputedColumn
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>()
                .Property(p => p.DisplayName)
                .HasComputedColumnSql("[LastName] + ', ' + [FirstName]");
        }
        #endregion
    }

    public class Person
    {
        public int PersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
    }
}
