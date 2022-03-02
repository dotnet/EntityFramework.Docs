using Microsoft.EntityFrameworkCore;

namespace EFModeling.GeneratedProperties.FluentAPI.ComputedColumn;

internal class MyContext : DbContext
{
    public DbSet<Person> People { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region DefaultComputedColumn
        modelBuilder.Entity<Person>()
            .Property(p => p.DisplayName)
            .HasComputedColumnSql("[LastName] + ', ' + [FirstName]");
        #endregion

        #region StoredComputedColumn
        modelBuilder.Entity<Person>()
            .Property(p => p.NameLength)
            .HasComputedColumnSql("LEN([LastName]) + LEN([FirstName])", stored: true);
        #endregion
    }
}

public class Person
{
    public int PersonId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string DisplayName { get; set; }
    public int NameLength { get; set; }
}