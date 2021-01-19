using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EFModeling.DataSeeding.Migrations
{
    [DbContext(typeof(DataSeedingContext))]
    internal class DataSeedingContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.0-rtm-30799")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity(
                "EFModeling.DataSeeding.Blog", b =>
                {
                    b.Property<int>("BlogId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Url")
                        .IsRequired();

                    b.HasKey("BlogId");

                    b.ToTable("Blogs");

                    b.HasData(
                        new { BlogId = 1, Url = "http://sample.com" }
                    );
                });

            modelBuilder.Entity(
                "EFModeling.DataSeeding.Post", b =>
                {
                    b.Property<int>("PostId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BlogId");

                    b.Property<string>("Content");

                    b.Property<string>("Title");

                    b.HasKey("PostId");

                    b.HasIndex("BlogId");

                    b.ToTable("Posts");

                    b.HasData(
                        new { PostId = 1, BlogId = 1, Content = "Test 1", Title = "First post" },
                        new { PostId = 2, BlogId = 1, Content = "Test 2", Title = "Second post" }
                    );
                });

            modelBuilder.Entity(
                "EFModeling.DataSeeding.Post", b =>
                {
                    b.HasOne("EFModeling.DataSeeding.Blog", "Blog")
                        .WithMany("Posts")
                        .HasForeignKey("BlogId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.OwnsOne(
                        "EFModeling.DataSeeding.Name", "AuthorName", b1 =>
                        {
                            b1.Property<int?>("PostId")
                                .ValueGeneratedOnAdd()
                                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                            b1.Property<string>("First");

                            b1.Property<string>("Last");

                            b1.ToTable("Posts");

                            b1.HasOne("EFModeling.DataSeeding.Post")
                                .WithOne("AuthorName")
                                .HasForeignKey("EFModeling.DataSeeding.Name", "PostId")
                                .OnDelete(DeleteBehavior.Cascade);

                            b1.HasData(
                                new { PostId = 1, First = "Andriy", Last = "Svyryd" },
                                new { PostId = 2, First = "Diego", Last = "Vega" }
                            );
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
