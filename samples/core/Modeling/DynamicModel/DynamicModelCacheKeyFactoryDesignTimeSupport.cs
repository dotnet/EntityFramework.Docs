using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EFModeling.DynamicModel
{
    #region DynamicModelDesignTimeSupport
    public class DynamicModelCacheKeyFactory : IModelCacheKeyFactory
    {
        public object Create(DbContext context)
            => context is DynamicContext dynamicContext
                ? (context.GetType(), dynamicContext.UseIntProperty)
                : (object)context.GetType();

        public object Create(DbContext context)
            => Create(context, false);
    }
    #endregion
}
