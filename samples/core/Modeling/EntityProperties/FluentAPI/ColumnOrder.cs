using Microsoft.EntityFrameworkCore;

namespace EFModeling.EntityProperties.FluentAPI.ColumnOrder;

internal class MyContext : DbContext
{
    public DbSet<Employee> Employees { get; set; }

    #region snippet_HasColumnOrder
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(x =>
        {
            x.Property(b => b.Id)
                .HasColumnOrder(0);

            x.Property(b => b.FirstName)
                .HasColumnOrder(1);

            x.Property(b => b.LastName)
                .HasColumnOrder(2);
        });
    }
    #endregion
}

public class EntityBase
{
    public int Id { get; set; }
}

public class PersonBase : EntityBase
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

public class Employee : PersonBase
{
    public string Department { get; set; }
    public decimal AnnualSalary { get; set; }
}