using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFModeling.DataAnnotations.Relational.TableAndSchema
{
    class MyContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
    }

    [Table("blogs", Schema = "blogging")]
    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }
    }
}
