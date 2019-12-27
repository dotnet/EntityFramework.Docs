using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EFModeling.DynamicModel
{
    #region DynamicModel
    public class DynamicModelCacheKeyFactory : IModelCacheKeyFactory
    {
        public object Create(DbContext context)
            => context is DynamicContext dynamicContext
                ? (context.GetType(), dynamicContext.UseIntProperty)
                : (object)context.GetType();
    }
    #endregion
}
