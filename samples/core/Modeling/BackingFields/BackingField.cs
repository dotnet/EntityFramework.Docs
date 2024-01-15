using Microsoft.EntityFrameworkCore;

namespace EFModeling.BackingFields.BackingField;

internal class MyContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }
}

#region Sample
public class Blog
{
    public int BlogId { get; set; }

    public string Url { get; set; }
}
#endregion
