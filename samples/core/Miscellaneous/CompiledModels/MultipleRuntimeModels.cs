using System;
using System.Collections.Concurrent;
using CompiledModelTest;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

#pragma warning disable 219, 612, 618
#nullable disable

namespace MultipleRuntimeModels;

#region RuntimeModelCache
public static class RuntimeModelCache
{
    private static readonly ConcurrentDictionary<string, IModel> _runtimeModels
        = new();

    public static IModel GetOrCreateModel(string connectionString)
        => _runtimeModels.GetOrAdd(
            connectionString, cs =>
            {
                if (cs.Contains("X"))
                {
                    return BlogsContextModel1.Instance;
                }

                if (cs.Contains("Y"))
                {
                    return BlogsContextModel2.Instance;
                }

                throw new InvalidOperationException("No appropriate compiled model found.");
            });
}
#endregion

[DbContext(typeof(BlogsContext))]
partial class BlogsContextModel1 : RuntimeModel
{
    private static BlogsContextModel1 _instance;
    public static IModel Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BlogsContextModel1();
                _instance.Initialize();
                _instance.Customize();
            }

            return _instance;
        }
    }

    partial void Initialize();

    partial void Customize();
}
    
[DbContext(typeof(BlogsContext))]
partial class BlogsContextModel2 : RuntimeModel
{
    private static BlogsContextModel2 _instance;
    public static IModel Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BlogsContextModel2();
                _instance.Initialize();
                _instance.Customize();
            }

            return _instance;
        }
    }

    partial void Initialize();

    partial void Customize();
}