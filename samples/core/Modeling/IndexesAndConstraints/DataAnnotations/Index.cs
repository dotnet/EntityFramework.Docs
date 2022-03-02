using Microsoft.EntityFrameworkCore;

namespace EFModeling.IndexesAndConstraints.DataAnnotations.Index;

internal class MyContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }
}

#region Index
[Index(nameof(Url))]
public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }
}
#endregion