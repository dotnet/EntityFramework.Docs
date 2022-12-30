using System.Collections.Generic;

namespace EF.Testing.BusinessLogic;

#region IBloggingRepository
public interface IBloggingRepository
{
    Blog GetBlogByName(string name);

    IEnumerable<Blog> GetAllBlogs();

    void AddBlog(Blog blog);

    void SaveChanges();
}
#endregion