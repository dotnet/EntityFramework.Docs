using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EF.Testing.BusinessLogic;

#region BloggingRepository
public class BloggingRepository : IBloggingRepository
{
    private readonly BloggingContext _context;

    public BloggingRepository(BloggingContext context)
        => _context = context;

    public async Task<Blog> GetBlogByNameAsync(string name)
        => await _context.Blogs.FirstOrDefaultAsync(b => b.Name == name);
    #endregion

    public IAsyncEnumerable<Blog> GetAllBlogsAsync()
        => _context.Blogs.AsAsyncEnumerable();

    public void AddBlog(Blog blog)
        => _context.Add(blog);

    public async Task SaveChangesAsync()
        => await _context.SaveChangesAsync();
}
