using Microsoft.EntityFrameworkCore;
using MySQL.Data.EntityFrameworkCore.Extensions;

namespace EFGetStarted.AspNetCore.ExistingDb.Models
{
	public partial class BloggingContext : DbContext
	{
		//private readonly string _connectionString;

		//public BloggingContext(string connectionString)
		//{
		//	_connectionString = connectionString;
		//}

		public BloggingContext(DbContextOptions<BloggingContext> options)
			: base(options)
		{
		}

		//protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		//{
		//	//optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=MyDatabase;Trusted_Connection=True;");

		//	if (!string.IsNullOrEmpty(_connectionString))
		//		optionsBuilder.UseMySQL(_connectionString);
		//}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			var is_mysql = Database.GetDbConnection().GetType().Name == "MySqlConnection";

			modelBuilder.Entity<Blog>(entity =>
			{
				entity.Property(e => e.Url).IsRequired();
			});
			modelBuilder.Entity<Blog>().ToTable("Blog");

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

				if (is_mysql)//fixes column mapping for MySql
				{
					entity.Property(e => e.Key).HasColumnName("SourceKey");
					entity.Property(e => e.HashMD5).HasColumnName("hashMD5");
					entity.Property(e => e.HashSHA256).HasColumnName("hashSHA256");

					modelBuilder.Entity<Hashes>().ToTable("Hashes");
				}
			});
		}

		public virtual DbSet<Blog> Blog { get; set; }
		public virtual DbSet<Post> Post { get; set; }
		public virtual DbSet<Hashes> Hashes { get; set; }
	}
}
