using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFQuerying.Async
{
    public class Sample
    {
        #region Sample
        public async Task<List<Blog>> GetBlogsAsync()
        {
            using (var context = new BloggingContext())
            {
                return await context.Blogs.ToListAsync();
            }
        }
        #endregion
    }
}
