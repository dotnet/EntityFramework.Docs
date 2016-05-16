using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EFModeling.Configuring.DataAnnotations.Samples.MaxLength
{
    class MyContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        [MaxLength(500)]
        public string Url { get; set; }
    }
}
