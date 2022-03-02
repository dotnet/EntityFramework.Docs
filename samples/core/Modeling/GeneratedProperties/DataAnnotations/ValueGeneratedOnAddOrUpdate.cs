using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.GeneratedProperties.DataAnnotations.ValueGeneratedOnAddOrUpdate;

internal class MyContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }
}

#region ValueGeneratedOnAddOrUpdate
public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime LastUpdated { get; set; }
}
#endregion