namespace WebApp.Controllers;

#region MyController
public class MyController
{
    readonly ApplicationDbContext _context;

    public MyController(ApplicationDbContext context) => _context = context;
}
#endregion