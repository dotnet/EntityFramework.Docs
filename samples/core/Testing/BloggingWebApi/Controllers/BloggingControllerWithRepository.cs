using System;
using System.Data;
using System.Linq;
using EF.Testing.BusinessLogic;
using Microsoft.AspNetCore.Mvc;

namespace EF.Testing.BloggingWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class BloggingControllerWithRepository : ControllerBase
{
    #region BloggingControllerWithRepository
    private readonly IBloggingRepository _repository;

    public BloggingControllerWithRepository(IBloggingRepository repository)
        => _repository = repository;

    [HttpGet]
    public Blog GetBlog(string name)
        => _repository.GetBlogByName(name);
    #endregion

    [HttpGet]
    public ActionResult<Blog[]> GetAllBlogs()
        => _repository.GetAllBlogs().ToArray();

    [HttpPost]
    public ActionResult AddBlog(string name, string url)
    {
        _repository.AddBlog(new Blog { Name = name, Url = url });
        _repository.SaveChanges();

        return Ok();
    }

    [HttpPost]
    public ActionResult UpdateBlogUrl(string name, string url)
    {
        var blog = _repository.GetBlogByName(name);
        if (blog is null)
        {
            return NotFound();
        }

        blog.Url = url;
        _repository.SaveChanges();

        return Ok();
    }
}