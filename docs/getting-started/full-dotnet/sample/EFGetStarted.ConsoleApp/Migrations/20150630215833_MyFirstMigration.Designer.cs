using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations.Infrastructure;
using EFGetStarted.ConsoleApp;

namespace EFGetStarted.ConsoleApp.Migrations
{
    [ContextType(typeof(BloggingContext))]
    partial class MyFirstMigration
    {
        public override string Id
        {
            get { return "20150630215833_MyFirstMigration"; }
        }

        public override string ProductVersion
        {
            get { return "7.0.0-beta6-13815"; }
        }

        public override void BuildTargetModel(ModelBuilder builder)
        {
            builder
                .Annotation("ProductVersion", "7.0.0-beta6-13815")
                .Annotation("SqlServer:ValueGenerationStrategy", "IdentityColumn");

            builder.Entity("EFGetStarted.ConsoleApp.Blog", b =>
                {
                    b.Property<int>("BlogId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Url")
                        .Required();

                    b.Key("BlogId");
                });

            builder.Entity("EFGetStarted.ConsoleApp.Post", b =>
                {
                    b.Property<int>("PostId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BlogId");

                    b.Property<string>("Content");

                    b.Property<string>("Title");

                    b.Key("PostId");
                });

            builder.Entity("EFGetStarted.ConsoleApp.Post", b =>
                {
                    b.Reference("EFGetStarted.ConsoleApp.Blog")
                        .InverseCollection()
                        .ForeignKey("BlogId");
                });
        }
    }
}
