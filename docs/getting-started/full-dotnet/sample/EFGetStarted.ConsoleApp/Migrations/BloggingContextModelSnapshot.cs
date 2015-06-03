using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Metadata.Builders;
using Microsoft.Data.Entity.Relational.Migrations.Infrastructure;
using EFGetStarted.ConsoleApp;

namespace EFGetStarted.ConsoleApp.Migrations
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
                
                builder.Entity("EFGetStarted.ConsoleApp.Blog", b =>
                    {
                        b.Property<int>("BlogId")
                            .GenerateValueOnAdd()
                            .Annotation("OriginalValueIndex", 0)
                            .Annotation("SqlServer:ValueGeneration", "Default");
                        b.Property<string>("Url")
                            .Annotation("OriginalValueIndex", 1);
                        b.Key("BlogId");
                    });
                
                builder.Entity("EFGetStarted.ConsoleApp.Post", b =>
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
                
                builder.Entity("EFGetStarted.ConsoleApp.Post", b =>
                    {
                        b.ForeignKey("EFGetStarted.ConsoleApp.Blog", "BlogId");
                    });
                
                return builder.Model;
            }
        }
    }
}
