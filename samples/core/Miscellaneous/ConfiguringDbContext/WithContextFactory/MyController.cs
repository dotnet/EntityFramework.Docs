using Microsoft.EntityFrameworkCore;
using WebApp;

namespace WithContextFactory;

public class MyController
{
    #region Construct
    readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public MyController(IDbContextFactory<ApplicationDbContext> contextFactory) => _contextFactory = contextFactory;
    #endregion

    #region DoSomething
    public void DoSomething()
    {
        using ApplicationDbContext context = _contextFactory.CreateDbContext();
        // ...
    }
    #endregion
}
