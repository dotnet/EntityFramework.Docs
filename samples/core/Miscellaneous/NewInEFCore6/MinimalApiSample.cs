﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public static class MinimalApiSample
{
    public static void Add_a_DbContext_and_provider()
    {
        Console.WriteLine($">>>> Sample: {nameof(Add_a_DbContext_and_provider)}");

        SqliteMinimal(null);
        SqliteNormal(null);

        SqlServerMinimal(null);
        SqlServerNormal(null);
    }

    static void SqliteMinimal(string[] args)
    {
        #region SqliteMinimal
        FakeWebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSqlite<MyDbContext>("Data Source=mydatabase.db");
        #endregion
    }

    static void SqliteNormal(string[] args)
    {
        #region SqliteNormal
        FakeWebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<MyDbContext>(
            options => options.UseSqlite("Data Source=mydatabase.db"));
        #endregion
    }

    static void SqlServerMinimal(string[] args)
    {
        #region SqlServerMinimal
        FakeWebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSqlServer<MyDbContext>(@"Server=(localdb)\mssqllocaldb;Database=MyDatabase");
        #endregion
    }

    static void SqlServerNormal(string[] args)
    {
        #region SqlServerNormal
        FakeWebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<MyDbContext>(
            options => options.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=MyDatabase"));
        #endregion
    }

    // This is a fake implementation of WebApplicationBuilder  uses to show the EF Core minimal APIs
    class FakeWebApplicationBuilder
    {
        public ServiceCollection Services { get; } = new();
    }

    // This is a fake implementation of WebApplication uses to show the EF Core minimal APIs
    static class WebApplication
    {
        public static FakeWebApplicationBuilder CreateBuilder(string[] args)
            => new();
    }

    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options)
            : base(options)
        {
        }
    }
}
