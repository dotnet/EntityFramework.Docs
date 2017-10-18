using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EFModeling.Samples.DynamicModel
{

    #region Class
    public class DynamicModelCacheKeyFactory : IModelCacheKeyFactory
    {
        public object Create(DbContext context)
        {
            if (context is DynamicContext dynamicContext)
            {
                return (context.GetType(), dynamicContext.IgnoreIntProperty);
            }
            return context.GetType();
        }
    }
    #endregion
}
