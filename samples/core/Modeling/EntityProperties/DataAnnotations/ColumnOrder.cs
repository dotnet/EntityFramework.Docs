using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.EntityProperties.DataAnnotations.ColumnOrder;

internal class MyContext : DbContext
{
    public DbSet<Employee> Employees { get; set; }
}

#region snippet_ColumnAttribute
public class EntityBase
{
    [Column(Order = 0)]
    public int Id { get; set; }
}

public class PersonBase : EntityBase
{
    [Column(Order = 1)]
    public string FirstName { get; set; }

    [Column(Order = 2)]
    public string LastName { get; set; }
}

public class Employee : PersonBase
{
    public string Department { get; set; }
    public decimal AnnualSalary { get; set; }
}
#endregion