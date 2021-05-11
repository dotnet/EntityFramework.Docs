using Microsoft.EntityFrameworkCore;
using WebApp;

namespace WithContextFactory
{
    public class MyController
    {
        #region Construct
        private readonly IDesignTimeDbContextFactory<ApplicationDbContext> _contextFactory;

        public MyController(IDesignTimeDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        #endregion

        #region DoSomething
        public void DoSomething()
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                // ...
            }
        }
        #endregion
    }
}
