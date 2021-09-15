# CompiledModelsDemo

1. Run the benchmark
2. Compile a model - `dotnet ef dbcontext optimize --output-dir MyCompiledModels --namespace MyCompiledModels`
3. Update OnConfiguring to use the compiled model
4. Comment out the caching in `BlogsContextModel` as shown below.
5. Run benchmark again

```csharp
public static IModel Instance
{
    get
    {
        //if (_instance == null)
        {
            _instance = new BlogsContextModel();
            _instance.Initialize();
            _instance.Customize();
        }

        return _instance;
    }
}
```
