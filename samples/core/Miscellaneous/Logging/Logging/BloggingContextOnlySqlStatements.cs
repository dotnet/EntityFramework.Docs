using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EFLogging
{
    public class BloggingContextOnlySqlStatements : DbContext
    {
        #region DefineSqlCommandLoggerFactory
        public static readonly LoggerFactory MyLoggerFactory
            = new LoggerFactory(new[] {new SqlCommandLoggingProvider()});
        #endregion

        public DbSet<Blog> Blogs { get; set; }

        #region RegisterSqlCommandLoggerFactory
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .UseLoggerFactory(MyLoggerFactory) // Warning: Do not create a new ILoggerFactory instance each time
                .UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=EFLogging;Trusted_Connection=True;ConnectRetryCount=0");
        #endregion
    }
}
