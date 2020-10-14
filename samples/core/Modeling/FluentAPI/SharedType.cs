using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace EFModeling.FluentAPI.SharedType
{
    #region SharedType
    class MyContext : DbContext
    {
        public DbSet<Dictionary<string, object>> Blogs => Set<Dictionary<string, object>>("Blog");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.SharedTypeEntity<Dictionary<string, object>>("Blog", bb =>
            {
                bb.Property<int>("BlogId");
                bb.Property<string>("Url");
                bb.Property<DateTime>("LastUpdated");
            });
        }
    }
    #endregion
}
