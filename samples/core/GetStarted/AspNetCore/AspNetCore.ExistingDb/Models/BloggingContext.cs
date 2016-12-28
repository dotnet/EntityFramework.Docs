using Microsoft.EntityFrameworkCore;

namespace EFGetStarted.AspNetCore.ExistingDb.Models
{
	public partial class BloggingContext : DbContext
	{
		public BloggingContext(DbContextOptions<BloggingContext> options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Blog>(entity =>
			{
				entity.Property(e => e.Url).IsRequired();
			});

			modelBuilder.Entity<Post>(entity =>
			{
				entity.HasOne(d => d.Blog)
					.WithMany(p => p.Post)
					.HasForeignKey(d => d.BlogId);
			});

			modelBuilder.Entity<Hashes>(entity =>
			{
				entity.Property(e => e.Key).IsRequired();
				entity.Property(e => e.HashMD5).IsRequired();
				entity.Property(e => e.HashSHA256).IsRequired();
			});
		}

		public virtual DbSet<Blog> Blog { get; set; }
		public virtual DbSet<Post> Post { get; set; }
		public virtual DbSet<Hashes> Hashes { get; set; }
	}
}
