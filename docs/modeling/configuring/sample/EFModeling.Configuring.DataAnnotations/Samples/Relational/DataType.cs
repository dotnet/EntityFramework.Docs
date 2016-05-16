using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFModeling.Configuring.DataAnnotations.Samples.Relational.DataType
{
    class MyContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string Url { get; set; }
    }
}
