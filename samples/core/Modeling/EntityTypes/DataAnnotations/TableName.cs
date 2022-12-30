using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.EntityTypes.DataAnnotations.TableName;

internal class MyContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }
}

#region TableName
[Table("blogs")]
public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }
}
#endregion