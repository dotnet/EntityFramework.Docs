using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


    public class PrivacyModel : PageModel
    {
    private readonly ApplicationDbContext _context;


    public PrivacyModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public void OnGet()
        {
        }
    }

