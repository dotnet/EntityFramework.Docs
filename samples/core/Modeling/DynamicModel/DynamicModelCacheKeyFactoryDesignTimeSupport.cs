using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EFModeling.DynamicModel;

#region DynamicModelDesignTimeSupport
public class DynamicModelCacheKeyFactoryDesignTimeSupport : IModelCacheKeyFactory
{
    public object Create(DbContext context, bool designTime)
        => context is DynamicContext dynamicContext
            ? (context.GetType(), dynamicContext.UseIntProperty, designTime)
            : (object)context.GetType();

    public object Create(DbContext context)
        => Create(context, false);
}
#endregion