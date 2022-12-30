using Microsoft.EntityFrameworkCore;

namespace EFModeling.EntityTypes.DataAnnotations.TableComment;

internal class MyContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }
}

#region TableComment
[Comment("Blogs managed on the website")]
public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }
}
#endregion