using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations.Infrastructure;
using EFGetStarted.AspNet5.Models;

namespace EFGetStarted.AspNet5.Migrations
{
    [ContextType(typeof(BloggingContext))]
    partial class BloggingContextModelSnapshot : ModelSnapshot
    {
        public override void BuildModel(ModelBuilder builder)
        {
            builder
                .Annotation("ProductVersion", "7.0.0-beta6-13815")
                .Annotation("SqlServer:ValueGenerationStrategy", "IdentityColumn");

            builder.Entity("EFGetStarted.AspNet5.Models.Blog", b =>
                {
                    b.Property<int>("BlogId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Url")
                        .Required();

                    b.Key("BlogId");
                });

            builder.Entity("EFGetStarted.AspNet5.Models.Post", b =>
                {
                    b.Property<int>("PostId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BlogId");

                    b.Property<string>("Content");

                    b.Property<string>("Title");

                    b.Key("PostId");
                });

            builder.Entity("EFGetStarted.AspNet5.Models.Post", b =>
                {
                    b.Reference("EFGetStarted.AspNet5.Models.Blog")
                        .InverseCollection()
                        .ForeignKey("BlogId");
                });
        }
    }
}
