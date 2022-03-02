using System.Collections.Generic;
using System.Linq;

namespace EF.Testing.BusinessLogic;

#region BloggingRepository
public class BloggingRepository : IBloggingRepository
{
    private readonly BloggingContext _context;

    public BloggingRepository(BloggingContext context)
        => _context = context;

    public Blog GetBlogByName(string name)
        => _context.Blogs.FirstOrDefault(b => b.Name == name);
    #endregion

    public IEnumerable<Blog> GetAllBlogs()
        => _context.Blogs;

    public void AddBlog(Blog blog)
        => _context.Add(blog);

    public void SaveChanges()
        => _context.SaveChanges();
}