using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.EntityProperties.DataAnnotations.ColumnName;

internal class MyContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }
}

#region ColumnName
public class Blog
{
    [Column("blog_id")]
    public int BlogId { get; set; }

    public string Url { get; set; }
}
#endregion