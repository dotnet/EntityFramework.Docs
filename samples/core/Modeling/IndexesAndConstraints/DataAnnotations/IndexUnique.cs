using Microsoft.EntityFrameworkCore;

namespace EFModeling.IndexesAndConstraints.DataAnnotations.IndexUnique;

internal class MyContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }
}

#region IndexUnique
[Index(nameof(Url), IsUnique = true)]
public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }
}
#endregion