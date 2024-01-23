using System;
using System.Collections.Concurrent;
using CompiledModelTest;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MultipleRuntimeModels;

#region RuntimeModelCache
public static class RuntimeModelCache
{
    static readonly ConcurrentDictionary<string, IModel> _runtimeModels
        = new();

    public static IModel GetOrCreateModel(string connectionString)
        => _runtimeModels.GetOrAdd(
            connectionString, cs => cs.Contains('X')
                    ? BlogsContextModel1.Instance
                    : cs.Contains('Y') ? BlogsContextModel2.Instance : throw new InvalidOperationException("No appropriate compiled model found."));
}
#endregion

[DbContext(typeof(BlogsContext))]
partial class BlogsContextModel1 : RuntimeModel
{
    static BlogsContextModel1? _instance;
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
    static BlogsContextModel2? _instance;
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
