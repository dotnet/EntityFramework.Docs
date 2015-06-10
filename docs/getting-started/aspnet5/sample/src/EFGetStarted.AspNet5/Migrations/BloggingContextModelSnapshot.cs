using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Metadata.Builders;
using Microsoft.Data.Entity.Relational.Migrations.Infrastructure;
using EFGetStarted.AspNet5.Models;

namespace EFGetStarted.AspNet5.Migrations
{
    [ContextType(typeof(BloggingContext))]
    partial class BloggingContextModelSnapshot : ModelSnapshot
    {
        public override IModel Model
        {
            get
            {
                var builder = new BasicModelBuilder()
                    .Annotation("SqlServer:ValueGeneration", "Sequence");
                
                builder.Entity("EFGetStarted.AspNet5.Models.Blog", b =>
                    {
                        b.Property<int>("BlogId")
                            .GenerateValueOnAdd()
                            .Annotation("OriginalValueIndex", 0)
                            .Annotation("SqlServer:ValueGeneration", "Default");
                        b.Property<string>("Url")
                            .Annotation("OriginalValueIndex", 1);
                        b.Key("BlogId");
                    });
                
                builder.Entity("EFGetStarted.AspNet5.Models.Post", b =>
                    {
                        b.Property<int>("BlogId")
                            .Annotation("OriginalValueIndex", 0);
                        b.Property<string>("Content")
                            .Annotation("OriginalValueIndex", 1);
                        b.Property<int>("PostId")
                            .GenerateValueOnAdd()
                            .Annotation("OriginalValueIndex", 2)
                            .Annotation("SqlServer:ValueGeneration", "Default");
                        b.Property<string>("Title")
                            .Annotation("OriginalValueIndex", 3);
                        b.Key("PostId");
                    });
                
                builder.Entity("EFGetStarted.AspNet5.Models.Post", b =>
                    {
                        b.ForeignKey("EFGetStarted.AspNet5.Models.Blog", "BlogId");
                    });
                
                return builder.Model;
            }
        }
    }
}
