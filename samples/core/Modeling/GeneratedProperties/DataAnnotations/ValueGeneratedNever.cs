using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.GeneratedProperties.DataAnnotations.ValueGeneratedNever;

internal class MyContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }
}

#region ValueGeneratedNever
public class Blog
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int BlogId { get; set; }

    public string Url { get; set; }
}
#endregion