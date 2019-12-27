using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFModeling.DataAnnotations.IgnoreProperty
{
    class MyContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
    }

    #region IgnoreProperty
    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }

        [NotMapped]
        public DateTime LoadedFromDatabase { get; set; }
    }
    #endregion
}
