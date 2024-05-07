using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace EFQuerying.UserDefinedFunctionMapping;

#region Entities
public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }
    public int? Rating { get; set; }

    public List<Post> Posts { get; set; }
}

public class Post
{
    public int PostId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public int Rating { get; set; }
    public int BlogId { get; set; }

    public Blog Blog { get; set; }
    public List<Comment> Comments { get; set; }
}

public class Comment
{
    public int CommentId { get; set; }
    public string Text { get; set; }
    public int Likes { get; set; }
    public int PostId { get; set; }

    public Post Post { get; set; }
}
#endregion

public class BloggingContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }

    #region BasicFunctionDefinition
    public int ActivePostCountForBlog(int blogId)
        => throw new NotSupportedException();
    #endregion

    #region HasTranslationFunctionDefinition
    public double PercentageDifference(double first, int second)
        => throw new NotSupportedException();
    #endregion

    #region QueryableFunctionDefinition
    public IQueryable<Post> PostsWithPopularComments(int likeThreshold)
        => FromExpression(() => PostsWithPopularComments(likeThreshold));
    #endregion

    #region NullabilityPropagationFunctionDefinition
    public string ConcatStrings(string prm1, string prm2)
        => throw new InvalidOperationException();

    public string ConcatStringsOptimized(string prm1, string prm2)
        => throw new InvalidOperationException();
    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region EntityConfiguration
        modelBuilder.Entity<Blog>()
            .HasMany(b => b.Posts)
            .WithOne(p => p.Blog);

        modelBuilder.Entity<Post>()
            .HasMany(p => p.Comments)
            .WithOne(c => c.Post);
        #endregion

        modelBuilder.Entity<Blog>()
            .HasData(
                new Blog { BlogId = 1, Url = @"https://devblogs.microsoft.com/dotnet", Rating = 5 },
                new Blog { BlogId = 2, Url = @"https://mytravelblog.com/", Rating = 4 });

        modelBuilder.Entity<Post>()
            .HasData(
                new Post
                {
                    PostId = 1,
                    BlogId = 1,
                    Title = "What's new",
                    Content = "Lorem ipsum dolor sit amet",
                    Rating = 5
                },
                new Post
                {
                    PostId = 2,
                    BlogId = 2,
                    Title = "Around the World in Eighty Days",
                    Content = "consectetur adipiscing elit",
                    Rating = 5
                },
                new Post
                {
                    PostId = 3,
                    BlogId = 2,
                    Title = "Glamping *is* the way",
                    Content = "sed do eiusmod tempor incididunt",
                    Rating = 4
                },
                new Post
                {
                    PostId = 4,
                    BlogId = 2,
                    Title = "Travel in the time of pandemic",
                    Content = "ut labore et dolore magna aliqua",
                    Rating = 3
                });

        modelBuilder.Entity<Comment>()
            .HasData(
                new Comment { CommentId = 1, PostId = 1, Text = "Exciting!", Likes = 3 },
                new Comment
                {
                    CommentId = 2,
                    PostId = 1,
                    Text = "Dotnet is useless - why use C# when you can write super fast assembly code instead?",
                    Likes = 0
                },
                new Comment { CommentId = 3, PostId = 2, Text = "Didn't think you would make it!", Likes = 3 },
                new Comment { CommentId = 4, PostId = 2, Text = "Are you going to try 70 days next time?", Likes = 5 },
                new Comment { CommentId = 5, PostId = 2, Text = "Good thing the earth is round :)", Likes = 5 },
                new Comment { CommentId = 6, PostId = 3, Text = "I couldn't agree with you more", Likes = 2 });

        #region BasicFunctionConfiguration
        modelBuilder.HasDbFunction(typeof(BloggingContext).GetMethod(nameof(ActivePostCountForBlog), new[] { typeof(int) }))
            .HasName("CommentedPostCountForBlog");
        #endregion

        #region HasTranslationFunctionConfiguration
        // 100 * ABS(first - second) / ((first + second) / 2)
        modelBuilder.HasDbFunction(
                typeof(BloggingContext).GetMethod(nameof(PercentageDifference), new[] { typeof(double), typeof(int) }))
            .HasTranslation(
                args =>
                    new SqlBinaryExpression(
                        ExpressionType.Multiply,
                        new SqlConstantExpression(
                            Expression.Constant(100),
                            new IntTypeMapping("int", DbType.Int32)),
                        new SqlBinaryExpression(
                            ExpressionType.Divide,
                            new SqlFunctionExpression(
                                "ABS",
                                new SqlExpression[]
                                {
                                    new SqlBinaryExpression(
                                        ExpressionType.Subtract,
                                        args.First(),
                                        args.Skip(1).First(),
                                        args.First().Type,
                                        args.First().TypeMapping)
                                },
                                nullable: true,
                                argumentsPropagateNullability: new[] { true, true },
                                type: args.First().Type,
                                typeMapping: args.First().TypeMapping),
                            new SqlBinaryExpression(
                                ExpressionType.Divide,
                                new SqlBinaryExpression(
                                    ExpressionType.Add,
                                    args.First(),
                                    args.Skip(1).First(),
                                    args.First().Type,
                                    args.First().TypeMapping),
                                new SqlConstantExpression(
                                    Expression.Constant(2),
                                    new IntTypeMapping("int", DbType.Int32)),
                                args.First().Type,
                                args.First().TypeMapping),
                            args.First().Type,
                            args.First().TypeMapping),
                        args.First().Type,
                        args.First().TypeMapping));
        #endregion

        #region NullabilityPropagationModelConfiguration
        modelBuilder
            .HasDbFunction(typeof(BloggingContext).GetMethod(nameof(ConcatStrings), new[] { typeof(string), typeof(string) }))
            .HasName("ConcatStrings");

        modelBuilder.HasDbFunction(
            typeof(BloggingContext).GetMethod(nameof(ConcatStringsOptimized), new[] { typeof(string), typeof(string) }),
            b =>
            {
                b.HasName("ConcatStrings");
                b.HasParameter("prm1").PropagatesNullability();
                b.HasParameter("prm2").PropagatesNullability();
            });
        #endregion

        #region QueryableFunctionConfigurationHasDbFunction
        modelBuilder.Entity<Post>().ToTable("Posts");
        modelBuilder.HasDbFunction(typeof(BloggingContext).GetMethod(nameof(PostsWithPopularComments), new[] { typeof(int) }));
        #endregion
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            @"Server=(localdb)\mssqllocaldb;Database=EFQuerying.UserDefinedFunctionMapping;Trusted_Connection=True;ConnectRetryCount=0");
    }
}
