using Microsoft.EntityFrameworkCore;

namespace WebApp;

#region ApplicationDbContext
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
}
#endregion