using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EFGetStarted.Migrations;

[DbContext(typeof(BloggingContext))]
internal class BloggingContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder
            .HasAnnotation("ProductVersion", "3.0.0-rc1.19456.14");

        modelBuilder.Entity(
            "EFGetStarted.Blog", b =>
            {
                b.Property<int>("BlogId")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("INTEGER");

                b.Property<string>("Url")
                    .HasColumnType("TEXT");

                b.HasKey("BlogId");

                b.ToTable("Blogs");
            });

        modelBuilder.Entity(
            "EFGetStarted.Post", b =>
            {
                b.Property<int>("PostId")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("INTEGER");

                b.Property<int>("BlogId")
                    .HasColumnType("INTEGER");

                b.Property<string>("Content")
                    .HasColumnType("TEXT");

                b.Property<string>("Title")
                    .HasColumnType("TEXT");

                b.HasKey("PostId");

                b.HasIndex("BlogId");

                b.ToTable("Posts");
            });

        modelBuilder.Entity(
            "EFGetStarted.Post", b =>
            {
                b.HasOne("EFGetStarted.Blog", "Blog")
                    .WithMany("Posts")
                    .HasForeignKey("BlogId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });
#pragma warning restore 612, 618
    }
}