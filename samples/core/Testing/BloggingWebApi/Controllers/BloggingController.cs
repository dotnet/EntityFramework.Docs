using System.Data;
using System.Linq;
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
    public ActionResult<Blog> GetBlog(string name)
    {
        var blog = _context.Blogs.FirstOrDefault(b => b.Name == name);
        return blog is null ? NotFound() : blog;
    }
    #endregion

    [HttpGet]
    public ActionResult<Blog[]> GetAllBlogs()
        => _context.Blogs.OrderBy(b => b.Name).ToArray();

    #region AddBlog
    [HttpPost]
    public ActionResult AddBlog(string name, string url)
    {
        _context.Blogs.Add(new Blog { Name = name, Url = url });
        _context.SaveChanges();

        return Ok();
    }
    #endregion

    #region UpdateBlogUrl
    [HttpPost]
    public ActionResult UpdateBlogUrl(string name, string url)
    {
        // Note: it isn't usually necessary to start a transaction for updating. This is done here for illustration purposes only.
        using var transaction = _context.Database.BeginTransaction(IsolationLevel.Serializable);

        var blog = _context.Blogs.FirstOrDefault(b => b.Name == name);
        if (blog is null)
        {
            return NotFound();
        }

        blog.Url = url;
        _context.SaveChanges();

        transaction.Commit();
        return Ok();
    }
    #endregion
}