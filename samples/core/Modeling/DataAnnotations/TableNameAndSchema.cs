using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFModeling.DataAnnotations.Relational.TableNameAndSchema
{
    class MyContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
    }

    #region TableNameAndSchema
    [Table("blogs", Schema = "blogging")]
    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }
    }
    #endregion
}
