using System.Collections.Generic;
using System.Threading.Tasks;

namespace EF.Testing.BusinessLogic;

#region IBloggingRepository
public interface IBloggingRepository
{
    Task<Blog> GetBlogByNameAsync(string name);

    IAsyncEnumerable<Blog> GetAllBlogsAsync();

    void AddBlog(Blog blog);

    Task SaveChangesAsync();
}
#endregion
