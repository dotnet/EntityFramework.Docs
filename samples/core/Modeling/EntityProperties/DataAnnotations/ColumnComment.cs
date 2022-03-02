using Microsoft.EntityFrameworkCore;

namespace EFModeling.EntityProperties.DataAnnotations.ColumnComment;

internal class MyContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }
}

#region ColumnComment
public class Blog
{
    public int BlogId { get; set; }

    [Comment("The URL of the blog")]
    public string Url { get; set; }
}
#endregion