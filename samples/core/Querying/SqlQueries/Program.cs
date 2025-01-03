﻿using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EFQuerying.RawSQL;

#pragma warning disable EF1002 // Risk of vulnerability to SQL injection.
internal class Program
{
    private static void Main(string[] args)
    {
        using (var context = new BloggingContext())
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.Database.ExecuteSqlRaw(
                @"create function [dbo].[SearchBlogs] (@searchTerm nvarchar(max))
                          returns @found table
                          (
                              BlogId int not null,
                              Url nvarchar(max),
                              Rating int
                          )
                          as
                          begin
                              insert into @found
                              select * from dbo.Blogs as b
                              where exists (
                                  select 1
                                  from [Post] as [p]
                                  where ([b].[BlogId] = [p].[BlogId]) and (charindex(@searchTerm, [p].[Title]) > 0))

                                  return
                          end");

            context.Database.ExecuteSqlRaw(
                @"create procedure [dbo].[GetMostPopularBlogs] as
                          begin
                              select * from dbo.Blogs order by Rating
                          end");

            context.Database.ExecuteSqlRaw(
                @"create procedure [dbo].[GetMostPopularBlogsForUser] @filterByUser nvarchar(max) as
                          begin
                              select * from dbo.Blogs order by Rating
                          end");
        }

        using (var context = new BloggingContext())
        {
            var blogs = context.Blogs
                .FromSql($"SELECT * FROM dbo.Blogs")
                .ToList();
        }

        using (var context = new BloggingContext())
        {
            var blogs = context.Blogs
                .FromSql($"EXECUTE dbo.GetMostPopularBlogs")
                .ToList();
        }

        using (var context = new BloggingContext())
        {
            var user = "johndoe";

            var blogs = context.Blogs
                .FromSql($"EXECUTE dbo.GetMostPopularBlogsForUser {user}")
                .ToList();
        }

        using (var context = new BloggingContext())
        {
            var user = new SqliteParameter("user", "johndoe");

            var blogs = context.Blogs
                .FromSql($"EXECUTE dbo.GetMostPopularBlogsForUser @filterByUser={user}")
                .ToList();
        }

        using (var context = new BloggingContext())
        {
            var user = new SqliteParameter("user", "johndoe");

            var blogs = context.Blogs
                .FromSql($"EXECUTE dbo.GetMostPopularBlogsForUser {user}")
                .ToList();
        }

        using (var context = new BloggingContext())
        {
            var columnName = "Url";
            var columnValue = new SqliteParameter("columnValue", "http://SomeURL");

            var blogs = context.Blogs
                .FromSqlRaw($"SELECT * FROM [Blogs] WHERE {columnName} = @columnValue", columnValue)
                .ToList();
        }

        using (var context = new BloggingContext())
        {
            var searchTerm = "Lorem ipsum";

            var blogs = context.Blogs
                .FromSql($"SELECT * FROM dbo.SearchBlogs({searchTerm})")
                .Where(b => b.Rating > 3)
                .OrderByDescending(b => b.Rating)
                .ToList();
        }

        using (var context = new BloggingContext())
        {
            var searchTerm = "Lorem ipsum";

            var blogs = context.Blogs
                .FromSql($"SELECT * FROM dbo.SearchBlogs({searchTerm})")
                .Include(b => b.Posts)
                .ToList();
        }

        using (var context = new BloggingContext())
        {
            var searchTerm = "Lorem ipsum";

            var blogs = context.Blogs
                .FromSql($"SELECT * FROM dbo.SearchBlogs({searchTerm})")
                .AsNoTracking()
                .ToList();
        }

        using (var context = new BloggingContext())
        {
            var ids = context.Database
                .SqlQuery<int>($"SELECT [BlogId] FROM [Blogs]")
                .ToList();
        }

        using (var context = new BloggingContext())
        {
            var overAverageIds = context.Database
                .SqlQuery<int>($"SELECT [BlogId] AS [Value] FROM [Blogs]")
                .Where(id => id > context.Blogs.Average(b => b.BlogId))
                .ToList();
        }

        using (var context = new BloggingContext())
        {
            var rowsModified = context.Database.ExecuteSql($"UPDATE [Blogs] SET [Url] = NULL");
        }
    }
}
