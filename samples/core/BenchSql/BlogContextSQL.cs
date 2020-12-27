using BenchCommon;
using Microsoft.EntityFrameworkCore;

namespace BenchSql
{
    public class BlogContextSQL : BlogContext
    {
        private static DbContextOptions<BlogContext> _options = null;

        public BlogContextSQL() : base(_options ?? throw new System.InvalidOperationException("WTF: options should have been setup on init!"))
        { }
        public BlogContextSQL(DbContextOptions<BlogContext> options) : base(options) => _options = options;

    }
}
