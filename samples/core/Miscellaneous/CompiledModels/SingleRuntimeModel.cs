using CompiledModelTest;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SingleRuntimeModel;

#region RuntimeModel
[DbContext(typeof(BlogsContext))]
internal partial class BlogsContextModel : RuntimeModel
{
    private static BlogsContextModel _instance;
    public static IModel Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BlogsContextModel();
                _instance.Initialize();
                _instance.Customize();
            }

            return _instance;
        }
    }

    partial void Initialize();

    partial void Customize();
}
#endregion
