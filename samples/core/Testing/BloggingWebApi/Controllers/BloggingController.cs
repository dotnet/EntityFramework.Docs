using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using EF.Testing.BusinessLogic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EF.Testing.BloggingWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class BloggingController : ControllerBase
{
    private readonly BloggingContext _context;

    public BloggingController(BloggingContext context)
        => _context = context;

    #region GetBlog
    [HttpGet]
    public async Task<ActionResult<Blog>> GetBlog(string name)
    {
        var blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Name == name);
        return blog is null ? NotFound() : blog;
    }
    #endregion

    [HttpGet]
    public IAsyncEnumerable<Blog> GetAllBlogs()
        => _context.Blogs.OrderBy(b => b.Name).AsAsyncEnumerable();

    #region AddBlog
    [HttpPost]
    public async Task<ActionResult> AddBlog(string name, string url)
    {
        _context.Blogs.Add(new Blog { Name = name, Url = url });
        await _context.SaveChangesAsync();

        return Ok();
    }
    #endregion

    #region UpdateBlogUrl
    [HttpPost]
    public async Task<ActionResult> UpdateBlogUrl(string name, string url)
    {
        // Note: it isn't usually necessary to start a transaction for updating. This is done here for illustration purposes only.
        await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

        var blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Name == name);
        if (blog is null)
        {
            return NotFound();
        }

        blog.Url = url;
        await _context.SaveChangesAsync();

        await transaction.CommitAsync();
        return Ok();
    }
    #endregion
}
