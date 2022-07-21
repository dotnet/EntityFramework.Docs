﻿using Microsoft.EntityFrameworkCore;

namespace EFModeling.IndexesAndConstraints.DataAnnotations.IndexDescending;

internal class MyContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }
}

#region IndexDescending
[Index(nameof(Url), nameof(Rating), IsDescending = new bool[0])]
public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }
    public int Rating { get; set; }
}
#endregion
