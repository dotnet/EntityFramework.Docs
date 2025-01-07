using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;

[MemoryDiagnoser]
public class DynamicallyConstructedQueries
{
    private int _blogNumber;
    private bool _addWhereClause = true;

    [GlobalSetup]
    public static async Task GlobalSetup()
    {
        using var context = new BloggingContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }

    #region ExpressionApiWithConstant
    [Benchmark]
    public async Task<int> ExpressionApiWithConstant()
    {
        var url = "blog" + Interlocked.Increment(ref _blogNumber);
        using var context = new BloggingContext();

        IQueryable<Blog> query = context.Blogs;

        if (_addWhereClause)
        {
            var blogParam = Expression.Parameter(typeof(Blog), "b");
            var whereLambda = Expression.Lambda<Func<Blog, bool>>(
                Expression.Equal(
                    Expression.MakeMemberAccess(
                        blogParam,
                        typeof(Blog).GetMember(nameof(Blog.Url)).Single()),
                    Expression.Constant(url)),
                blogParam);

            query = query.Where(whereLambda);
        }

        return await query.CountAsync();
    }
    #endregion

    #region ExpressionApiWithParameter
    [Benchmark]
    public async Task<int> ExpressionApiWithParameter()
    {
        var url = "blog" + Interlocked.Increment(ref _blogNumber);
        using var context = new BloggingContext();

        IQueryable<Blog> query = context.Blogs;

        if (_addWhereClause)
        {
            var blogParam = Expression.Parameter(typeof(Blog), "b");

            // This creates a lambda expression whose body is identical to the url captured closure variable in the non-dynamic query:
            // blogs.Where(b => b.Url == url)
            // This dynamically creates an expression node which EF can properly recognize and parameterize in the database query.
            // We then extract that body and use it in our dynamically-constructed query.
            Expression<Func<string>> urlParameterLambda = () => url;
            var urlParamExpression = urlParameterLambda.Body;

            var whereLambda = Expression.Lambda<Func<Blog, bool>>(
                Expression.Equal(
                    Expression.MakeMemberAccess(
                        blogParam,
                        typeof(Blog).GetMember(nameof(Blog.Url)).Single()),
                    urlParamExpression),
                blogParam);

            query = query.Where(whereLambda);
        }

        return await query.CountAsync();
    }
    #endregion

    #region SimpleWithParameter
    [Benchmark]
    public async Task<int> SimpleWithParameter()
    {
        var url = "blog" + Interlocked.Increment(ref _blogNumber);

        using var context = new BloggingContext();

        IQueryable<Blog> query = context.Blogs;

        if (_addWhereClause)
        {
            Expression<Func<Blog, bool>> whereLambda = b => b.Url == url;

            query = query.Where(whereLambda);
        }

        return await query.CountAsync();
    }
    #endregion

    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Blogging;Trusted_Connection=True;ConnectRetryCount=0");
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }
        public int Rating { get; set; }
        public List<Post> Posts { get; set; }
    }

    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }
}
