using System;
using BenchCommon;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BenchSql
{
    public class ProviderSql : BenchBase
    {
        public static void SetupSqlServer(IConfiguration config)
        {
            var opts = new DbContextOptionsBuilder<BlogContext>();
            opts.UseSqlServer(config.GetConnectionString("DefaultSql"));

            using var context = new BlogContextSQL(opts.Options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            context.SeedData();
        }

        public override bool AddRows(int addcount) => throw new NotImplementedException();
        public override bool DeleteRows(int rowcount) => throw new NotImplementedException();
        public override bool FindRows(int addcount) => throw new NotImplementedException();
        public override bool GetRows(int addcount) => throw new NotImplementedException();
        public override bool Initialise(int rowcount) => throw new NotImplementedException();
        public override bool Save(int rowcount) => throw new NotImplementedException();
        public override bool UpdateRows(int rowcount) => throw new NotImplementedException();
    }
}
