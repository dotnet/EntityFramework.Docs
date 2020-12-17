// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;

namespace Performance
{
    public class BatchTweakingContext : DbContext
    {
        #region BatchTweaking
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Server=(localdb)\mssqllocaldb;Database=Blogging;Integrated Security=True",
                o => o
                    .MinBatchSize(1)
                    .MaxBatchSize(100));
        }
        #endregion
    }
}
