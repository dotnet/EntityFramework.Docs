using Microsoft.EntityFrameworkCore;

namespace WebApp
{
    public static class UseNewForWebApp
    {
        public static void Example()
        {
            #region UseNewForWebApp
            var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Test")
                .Options;

            using var context = new ApplicationDbContext(contextOptions);
            #endregion
        }
    }
}
