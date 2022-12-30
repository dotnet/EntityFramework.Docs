using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.EntityProperties.DataAnnotations.Required;

internal class MyContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }
}

#region Required
public class Blog
{
    public int BlogId { get; set; }

    [Required]
    public string Url { get; set; }
}
#endregion