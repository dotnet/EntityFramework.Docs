using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using PlanetaryDocs.Domain;

namespace PlanetaryDocs.DataAccess
{
    /// <summary>
    /// <see cref="DbContext"/> implementation for Planetary Docs.
    /// </summary>
    public sealed class DocsContext : DbContext
    {
        /// <summary>
        /// Name of the partition key shadow property.
        /// </summary>
        public const string PartitionKey = nameof(PartitionKey);

        /// <summary>
        /// Name of the container for metadata.
        /// </summary>
        private const string Meta = nameof(Meta);

        /// <summary>
        /// Initializes a new instance of the <see cref="DocsContext"/> class.
        /// </summary>
        /// <param name="options">The configuration options.</param>
        public DocsContext(DbContextOptions<DocsContext> options)
            : base(options) =>
                SavingChanges += DocsContext_SavingChanges;

        /// <summary>
        /// Gets or sets the audits collection.
        /// </summary>
        public DbSet<DocumentAudit> Audits { get; set; }

        /// <summary>
        /// Gets or sets the documents collection.
        /// </summary>
        public DbSet<Document> Documents { get; set; }

        /// <summary>
        /// Gets or sets the tags collection.
        /// </summary>
        public DbSet<Tag> Tags { get; set; }

        /// <summary>
        /// Gets or sets the authors collection.
        /// </summary>
        public DbSet<Author> Authors { get; set; }

        /// <summary>
        /// Determines the custom partition key for meta (author, tag).
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <returns>The partition key (type).</returns>
        public static string ComputePartitionKey<T>()
            where T : class, IDocSummaries => typeof(T).Name;

        /// <summary>
        /// Unhook events on disposal.
        /// </summary>
        public override void Dispose()
        {
            SavingChanges -= DocsContext_SavingChanges;
            base.Dispose();
        }

        /// <summary>
        /// Asynchronous disposal.
        /// </summary>
        /// <returns>The asynchronous task.</returns>
        public override ValueTask DisposeAsync()
        {
            SavingChanges -= DocsContext_SavingChanges;
            return base.DisposeAsync();
        }

        /// <summary>
        /// Generic strategy to load a <see cref="Author"/> or
        /// <see cref="Tag"/>.
        /// </summary>
        /// <typeparam name="T">The type to load.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>The matching item.</returns>
        public async ValueTask<T> FindMetaAsync<T>(string key)
            where T : class, IDocSummaries
        {
            var partitionKey = ComputePartitionKey<T>();
            try
            {
                return await FindAsync<T>(key, partitionKey);
            }
            catch (CosmosException ce)
            {
                if (ce.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }

                throw;
            }
        }

        /// <summary>
        /// Set the partition key shadow property.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="entity">The entity.</param>
        public void SetPartitionKey<T>(T entity)
            where T : class, IDocSummaries =>
                Entry(entity).Property(PartitionKey).CurrentValue =
                    ComputePartitionKey<T>();

        /// <summary>
        /// Migrate from EF Core 5 to EF Core 6.
        /// </summary>
        /// <param name="checkId">Document to check.</param>
        /// <returns>The asynchronous task.</returns>
        public async Task CheckAndMigrateTagsAsync(string checkId)
        {
            bool migrated = true;

            try
            {
                var doc = await Documents.FindAsync(checkId);
            }
            catch (JsonSerializationException)
            {
                migrated = false;
            }

            if (migrated)
            {
                return;
            }

            var docs = await Documents.FromSqlRaw(
                "select c.id, c.Uid, c.AuthorAlias, c.Description, c.Html, c.Markdown, c.PublishDate, c.Title, STRINGTOARRAY(c.Tags) as Tags from c").ToListAsync();
            foreach (var doc in docs)
            {
                Entry(doc).State = EntityState.Modified;
            }

            await SaveChangesAsync();
        }

        /// <summary>
        /// Configure the model that maps the domain to the backend.
        /// </summary>
        /// <param name="modelBuilder">The API for model configuration.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DocumentAudit>()
                .HasNoDiscriminator()
                .ToContainer(nameof(Audits))
                .HasPartitionKey(da => da.Uid)
                .HasKey(da => new { da.Id, da.Uid });

            var docModel = modelBuilder.Entity<Document>();

            docModel.ToContainer(nameof(Documents))
                .HasNoDiscriminator()
                .HasKey(d => d.Uid);

            docModel.HasPartitionKey(d => d.Uid)
                .Property(p => p.ETag)
                .IsETagConcurrency();

            var tagModel = modelBuilder.Entity<Tag>();

            tagModel.Property<string>(PartitionKey);

            tagModel.HasPartitionKey(PartitionKey);

            tagModel.ToContainer(Meta)
                .HasKey(nameof(Tag.TagName), PartitionKey);

            tagModel.Property(t => t.ETag)
                .IsETagConcurrency();

            var authorModel = modelBuilder.Entity<Author>();

            authorModel.Property<string>(PartitionKey);
            authorModel.HasPartitionKey(PartitionKey);

            authorModel.ToContainer(Meta)
                .HasKey(nameof(Author.Alias), PartitionKey);

            authorModel.Property(a => a.ETag)
                .IsETagConcurrency();

            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Intercepts saving changes to store audits.
        /// </summary>
        /// <param name="sender">The sending context.</param>
        /// <param name="e">The change arguments.</param>
        private void DocsContext_SavingChanges(
            object sender,
            SavingChangesEventArgs e)
        {
            var entries = ChangeTracker.Entries<Document>()
                .Where(
                    e => e.State == EntityState.Added ||
                    e.State == EntityState.Modified)
                .Select(e => e.Entity)
                .ToList();

            foreach (var docEntry in entries)
            {
                Audits.Add(new DocumentAudit(docEntry));
            }
        }
    }
}
