using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.GeneratedProperties.DataAnnotations.ValueGeneratedOnAdd;

internal class MyContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }
}

#region ValueGeneratedOnAdd
public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime Inserted { get; set; }
}
#endregion