﻿using Microsoft.EntityFrameworkCore;

namespace EFModeling.FluentAPI.Relational.DefaultSchema
{
    #region DefaultSchema
    class MyContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("blogging");
        }
    }
    #endregion

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }
    }
}
