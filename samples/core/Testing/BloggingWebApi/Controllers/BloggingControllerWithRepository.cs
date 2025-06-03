using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
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
    public async Task<Blog> GetBlog(string name)
        => await _repository.GetBlogByNameAsync(name);
    #endregion

    [HttpGet]
    public IAsyncEnumerable<Blog> GetAllBlogs()
        => _repository.GetAllBlogsAsync();

    [HttpPost]
    public async Task AddBlog(string name, string url)
    {
        _repository.AddBlog(new Blog { Name = name, Url = url });
        await _repository.SaveChangesAsync();
    }

    [HttpPost]
    public async Task<ActionResult> UpdateBlogUrl(string name, string url)
    {
        var blog = await _repository.GetBlogByNameAsync(name);
        if (blog is null)
        {
            return NotFound();
        }

        blog.Url = url;
        await _repository.SaveChangesAsync();

        return Ok();
    }
}
