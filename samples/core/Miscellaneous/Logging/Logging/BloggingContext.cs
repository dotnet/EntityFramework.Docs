using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EFLogging
{
    public class BloggingContext : DbContext
    {
        #region DefineLoggerFactory
        public static readonly Lazy<ILoggerFactory> MyLoggerFactory = new Lazy<ILoggerFactory>(() =>
            {
                IServiceCollection serviceCollection = new ServiceCollection();
                serviceCollection.AddLogging(builder => builder.AddConsole());
                return serviceCollection.BuildServiceProvider().GetService<ILoggerFactory>();
            });
        #endregion

        public DbSet<Blog> Blogs { get; set; }

        #region RegisterLoggerFactory
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .UseLoggerFactory(MyLoggerFactory.Value) // Warning: Do not create a new ILoggerFactory instance each time
                .UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=EFLogging;Trusted_Connection=True;ConnectRetryCount=0");
        #endregion
    }
}
