using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EFModeling.DataAnnotations.MaxLength
{
    class MyContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
    }

    #region MaxLength
    public class Blog
    {
        public int BlogId { get; set; }
        [MaxLength(500)]
        public string Url { get; set; }
    }
    #endregion
}
