using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFModeling.Configuring.DataAnnotations.Samples.Relational.Column
{
    class MyContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
    }

    public class Blog
    {
        [Column("blog_id")]
        public int BlogId { get; set; }
        public string Url { get; set; }
    }
}
