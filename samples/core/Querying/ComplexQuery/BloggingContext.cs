using Microsoft.EntityFrameworkCore;

namespace EFQuerying.ComplexQuery;

public class BloggingContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Post> Posts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>()
            .HasMany(b => b.Posts)
            .WithOne(p => p.Blog)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<PostTag>()
            .HasOne(pt => pt.Post)
            .WithMany(p => p.Tags)
            .HasForeignKey(pt => pt.PostId);

        modelBuilder.Entity<PostTag>()
            .HasOne(pt => pt.Tag)
            .WithMany(t => t.Posts)
            .HasForeignKey(pt => pt.TagId);

        modelBuilder.Entity<Blog>()
            .HasData(
                new Blog
                {
                    BlogId = 1, Url = @"https://devblogs.microsoft.com/dotnet", Rating = 5, OwnerId = 1,
                },
                new Blog { BlogId = 2, Url = @"https://mytravelblog.com/", Rating = 4, OwnerId = 3 });

        modelBuilder.Entity<Post>()
            .HasData(
                new Post
                {
                    PostId = 1,
                    BlogId = 1,
                    Title = "What's new",
                    Content = "Lorem ipsum dolor sit amet",
                    Rating = 5,
                    AuthorId = 1
                },
                new Post
                {
                    PostId = 2,
                    BlogId = 2,
                    Title = "Around the World in Eighty Days",
                    Content = "consectetur adipiscing elit",
                    Rating = 5,
                    AuthorId = 2
                },
                new Post
                {
                    PostId = 3,
                    BlogId = 2,
                    Title = "Glamping *is* the way",
                    Content = "sed do eiusmod tempor incididunt",
                    Rating = 4,
                    AuthorId = 3
                },
                new Post
                {
                    PostId = 4,
                    BlogId = 2,
                    Title = "Travel in the time of pandemic",
                    Content = "ut labore et dolore magna aliqua",
                    Rating = 3,
                    AuthorId = 3
                });

        modelBuilder.Entity<Person>()
            .HasData(
                new Person { PersonId = 1, Name = "Dotnet Blog Admin", PhotoId = 1 },
                new Person { PersonId = 2, Name = "Phileas Fogg", PhotoId = 2 },
                new Person { PersonId = 3, Name = "Jane Doe", PhotoId = 3 });

        modelBuilder.Entity<PersonPhoto>()
            .HasData(
                new PersonPhoto { PersonPhotoId = 1, Caption = "SN", Photo = new byte[] { 0x00, 0x01 } },
                new PersonPhoto { PersonPhotoId = 2, Caption = "PF", Photo = new byte[] { 0x01, 0x02, 0x03 } },
                new PersonPhoto { PersonPhotoId = 3, Caption = "JD", Photo = new byte[] { 0x01, 0x01, 0x01 } });

        modelBuilder.Entity<Tag>()
            .HasData(
                new Tag { TagId = "general" },
                new Tag { TagId = "classic" },
                new Tag { TagId = "opinion" },
                new Tag { TagId = "informative" });

        modelBuilder.Entity<PostTag>()
            .HasData(
                new PostTag { PostTagId = 1, PostId = 1, TagId = "general" },
                new PostTag { PostTagId = 2, PostId = 1, TagId = "informative" },
                new PostTag { PostTagId = 3, PostId = 2, TagId = "classic" },
                new PostTag { PostTagId = 4, PostId = 3, TagId = "opinion" },
                new PostTag { PostTagId = 5, PostId = 4, TagId = "opinion" },
                new PostTag { PostTagId = 6, PostId = 4, TagId = "informative" });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            @"Server=(localdb)\mssqllocaldb;Database=EFQuerying.ComplexQuery;Trusted_Connection=True;ConnectRetryCount=0");
    }
}
