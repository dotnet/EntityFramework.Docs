using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using WebApplication1.Data;

namespace WebApplication1.Migrations;

#region snippet_DesignTimeFactory
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var connectionString = args.FirstOrDefault()
            ?? @"Server=(localdb)\mssqllocaldb;Database=WebApplication1;Trusted_Connection=True";

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(
                connectionString,
                sqlServer => sqlServer.MigrationsAssembly(typeof(ApplicationDbContextFactory).Assembly.GetName().Name))
            .Options;

        return new ApplicationDbContext(options);
    }
}
#endregion
