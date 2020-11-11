using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace EFModeling.FluentAPI.IndexerProperty
{
    class MyContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        #region ShadowProperty
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>().IndexerProperty<DateTime>("LastUpdated");
        }
        #endregion
    }

    public class Blog
    {
        private readonly Dictionary<string, object>  _data = new Dictionary<string, object>();
        public int BlogId { get; set; }
        public object this[string key]
        {
            get => _data[key];
            set => _data[key] = value;

        }
    }
}
