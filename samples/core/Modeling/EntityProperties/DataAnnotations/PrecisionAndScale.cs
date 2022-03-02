using System;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.EntityProperties.DataAnnotations.PrecisionAndScale;

internal class MyContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }
}

#region PrecisionAndScale
public class Blog
{
    public int BlogId { get; set; }
    [Precision(14, 2)]
    public decimal Score { get; set; }
    [Precision(3)]
    public DateTime LastUpdated { get; set; }
}
#endregion