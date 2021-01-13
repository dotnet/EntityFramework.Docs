// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;

namespace SqlServer.ValueGeneration
{
    public class IdentityOptionsContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        #region IdentityOptions
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>()
                .Property(b => b.BlogId)
                .UseIdentityColumn(seed: 10, increment: 10);
        }
        #endregion
    }
}
