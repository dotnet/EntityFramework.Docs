using Microsoft.EntityFrameworkCore;

namespace EFModeling.IndexesAndConstraints.DataAnnotations.IndexComposite;

internal class MyContext : DbContext
{
    public DbSet<Person> People { get; set; }
}

#region Composite
[Index(nameof(FirstName), nameof(LastName))]
public class Person
{
    public int PersonId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
#endregion