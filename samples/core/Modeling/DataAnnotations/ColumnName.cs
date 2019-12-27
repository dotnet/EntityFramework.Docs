using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFModeling.DataAnnotations.Relational.ColumnName
{
    class MyContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
    }

    #region ColumnName
    public class Blog
    {
        [Column("blog_id")]
        public int BlogId { get; set; }
        public string Url { get; set; }
    }
    #endregion
}
