using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFModeling.Configuring.DataAnnotations.Samples.ValueGeneratedOnAdd
{
    class MyContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
    }

    #region Sample
    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime Inserted { get; set; }
    }
    #endregion
}
