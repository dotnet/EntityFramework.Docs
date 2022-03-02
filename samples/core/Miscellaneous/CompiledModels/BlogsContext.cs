using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CompiledModelTest;

public class BlogsContext : DbContext
{


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var currentDirectory = Environment.CurrentDirectory;
        var location = currentDirectory.Substring(
            0, currentDirectory.IndexOf("CompiledModels", StringComparison.Ordinal) + "CompiledModels".Length);

        optionsBuilder
            //.UseModel(MyCompiledModels.BlogsContextModel.Instance)
            .EnableServiceProviderCaching(false)
            .UseSqlite(@$"Data Source={Path.Combine(location, "compiled_model.db")}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog0000>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0000>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0000>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0000>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0001>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0001>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0001>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0001>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0002>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0002>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0002>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0002>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0003>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0003>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0003>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0003>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0004>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0004>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0004>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0004>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0005>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0005>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0005>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0005>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0006>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0006>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0006>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0006>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0007>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0007>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0007>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0007>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0008>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0008>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0008>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0008>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0009>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0009>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0009>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0009>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0010>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0010>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0010>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0010>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0011>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0011>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0011>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0011>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0012>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0012>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0012>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0012>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0013>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0013>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0013>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0013>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0014>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0014>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0014>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0014>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0015>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0015>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0015>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0015>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0016>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0016>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0016>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0016>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0017>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0017>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0017>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0017>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0018>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0018>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0018>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0018>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0019>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0019>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0019>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0019>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0020>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0020>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0020>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0020>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0021>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0021>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0021>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0021>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0022>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0022>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0022>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0022>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0023>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0023>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0023>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0023>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0024>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0024>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0024>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0024>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0025>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0025>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0025>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0025>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0026>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0026>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0026>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0026>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0027>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0027>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0027>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0027>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0028>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0028>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0028>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0028>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0029>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0029>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0029>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0029>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0030>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0030>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0030>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0030>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0031>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0031>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0031>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0031>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0032>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0032>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0032>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0032>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0033>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0033>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0033>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0033>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0034>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0034>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0034>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0034>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0035>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0035>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0035>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0035>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0036>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0036>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0036>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0036>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0037>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0037>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0037>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0037>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0038>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0038>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0038>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0038>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0039>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0039>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0039>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0039>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0040>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0040>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0040>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0040>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0041>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0041>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0041>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0041>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0042>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0042>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0042>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0042>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0043>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0043>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0043>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0043>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0044>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0044>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0044>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0044>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0045>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0045>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0045>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0045>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0046>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0046>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0046>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0046>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0047>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0047>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0047>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0047>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0048>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0048>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0048>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0048>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0049>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0049>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0049>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0049>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0050>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0050>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0050>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0050>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0051>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0051>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0051>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0051>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0052>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0052>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0052>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0052>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0053>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0053>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0053>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0053>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0054>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0054>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0054>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0054>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0055>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0055>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0055>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0055>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0056>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0056>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0056>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0056>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0057>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0057>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0057>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0057>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0058>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0058>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0058>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0058>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0059>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0059>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0059>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0059>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0060>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0060>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0060>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0060>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0061>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0061>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0061>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0061>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0062>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0062>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0062>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0062>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0063>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0063>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0063>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0063>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0064>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0064>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0064>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0064>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0065>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0065>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0065>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0065>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0066>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0066>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0066>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0066>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0067>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0067>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0067>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0067>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0068>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0068>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0068>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0068>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0069>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0069>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0069>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0069>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0070>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0070>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0070>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0070>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0071>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0071>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0071>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0071>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0072>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0072>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0072>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0072>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0073>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0073>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0073>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0073>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0074>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0074>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0074>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0074>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0075>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0075>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0075>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0075>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0076>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0076>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0076>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0076>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0077>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0077>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0077>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0077>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0078>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0078>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0078>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0078>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0079>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0079>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0079>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0079>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0080>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0080>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0080>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0080>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0081>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0081>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0081>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0081>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0082>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0082>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0082>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0082>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0083>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0083>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0083>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0083>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0084>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0084>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0084>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0084>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0085>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0085>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0085>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0085>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0086>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0086>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0086>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0086>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0087>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0087>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0087>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0087>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0088>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0088>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0088>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0088>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);

        modelBuilder.Entity<Blog0089>().HasMany(e => e.Posts00).WithOne(e => e.Blog00);
        modelBuilder.Entity<Blog0089>().HasMany(e => e.Posts01).WithOne(e => e.Blog01);
        modelBuilder.Entity<Blog0089>().HasMany(e => e.Posts02).WithOne(e => e.Blog02);
        modelBuilder.Entity<Blog0089>().HasMany(e => e.Posts03).WithOne(e => e.Blog03);
    }
}