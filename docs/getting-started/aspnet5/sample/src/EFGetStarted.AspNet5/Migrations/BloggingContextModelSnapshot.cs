using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using EFGetStarted.AspNet5.Models;
using Microsoft.Data.Entity.SqlServer.Metadata;

namespace EFGetStarted.AspNet5.Migrations
{
    [DbContext(typeof(BloggingContext))]
    partial class BloggingContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Annotation("ProductVersion", "7.0.0-beta7-15540")
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerIdentityStrategy.IdentityColumn);

            modelBuilder.Entity("EFGetStarted.AspNet5.Models.Blog", b =>
                {
                    b.Property<int>("BlogId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Url")
                        .Required();

                    b.Key("BlogId");
                });

            modelBuilder.Entity("EFGetStarted.AspNet5.Models.Post", b =>
                {
                    b.Property<int>("PostId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BlogId");

                    b.Property<string>("Content");

                    b.Property<string>("Title");

                    b.Key("PostId");
                });

            modelBuilder.Entity("EFGetStarted.AspNet5.Models.Post", b =>
                {
                    b.Reference("EFGetStarted.AspNet5.Models.Blog")
                        .InverseCollection()
                        .ForeignKey("BlogId");
                });
        }
    }
}
