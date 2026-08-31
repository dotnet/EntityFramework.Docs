using System;
using CompiledModelTest;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

#pragma warning disable 219, 612, 618, EF1001
#nullable disable

namespace SingleRuntimeModel;

#region RuntimeModel
[DbContext(typeof(BlogsContext))]
partial class BlogsContextModel : RuntimeModel
{
    private BlogsContextModel()
        : base(skipDetectChanges: false, modelId: Guid.NewGuid(), entityTypeCount: 0)
    {
    }

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