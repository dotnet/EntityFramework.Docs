using Microsoft.AspNetCore.Mvc.RazorPages;


public class MyModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public MyModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public void OnGet()
    {
    }
}

