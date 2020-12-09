using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFModeling.DataAnnotations.Relational.ColumnComment
{
    class MyContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
    }

    #region ColumnComment
    public class Blog
    {
        public int BlogId { get; set; }
        [Comment("The URL of the blog")]
        public string Url { get; set; }
    }
    #endregion
}
