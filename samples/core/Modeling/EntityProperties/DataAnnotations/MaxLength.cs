using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.EntityProperties.DataAnnotations.MaxLength;

internal class MyContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }
}

#region MaxLength
public class Blog
{
    public int BlogId { get; set; }

    [MaxLength(500)]
    public string Url { get; set; }
}
#endregion