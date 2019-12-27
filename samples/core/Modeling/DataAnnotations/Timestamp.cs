using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EFModeling.DataAnnotations.Timestamp
{
    class MyContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
    }

    #region Timestamp
    public class Blog
    {
        public int BlogId { get; set; }

        public string Url { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
    #endregion
}
