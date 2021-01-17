using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Performance
{
    public class ExtensionsLoggingContext : DbContext
    {
        #region ExtensionsLogging
        private static ILoggerFactory ContextLoggerFactory
            => LoggerFactory.Create(b => b.AddConsole().AddFilter("", LogLevel.Information));

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Blogging;Integrated Security=True")
                .UseLoggerFactory(ContextLoggerFactory);
        }
        #endregion
    }
}
