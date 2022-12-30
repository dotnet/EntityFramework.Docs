using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using PlanetaryDocs.Domain;

namespace PlanetaryDocs.DataAccess
{
    /// <summary>
    /// Data access implementation.
    /// </summary>
    public class DocumentService : IDocumentService
    {
        /// <summary>
        /// Factory to generate <see cref="DocsContext"/> instances.
        /// </summary>
        private readonly IDbContextFactory<DocsContext> factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentService"/> class.
        /// </summary>
        /// <param name="factory">The factory instance.</param>
        public DocumentService(IDbContextFactory<DocsContext> factory) =>
            this.factory = factory;

        /// <summary>
        /// Add a new document.
        /// </summary>
        /// <param name="document">The <see cref="Document"/> to add.</param>
        /// <returns>An asynchronous <see cref="Task"/>.</returns>
        public async Task InsertDocumentAsync(Document document)
        {
            using var context = factory.CreateDbContext();

            await HandleMetaAsync(context, document);

            context.Add(document);

            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Loads a single <see cref="Document"/>.
        /// </summary>
        /// <param name="uid">The unique identifier.</param>
        /// <returns>The <see cref="Document"/>.</returns>
        public async Task<Document> LoadDocumentAsync(string uid)
        {
            using var context = factory.CreateDbContext();
            return await context.Documents
                    .WithPartitionKey(uid)
                    .SingleOrDefaultAsync(d => d.Uid == uid);
        }

        /// <summary>
        /// Query to obtain a <see cref="Document"/> list.
        /// </summary>
        /// <param name="searchText">Text to look for.</param>
        /// <param name="authorAlias">Restrict to an author.</param>
        /// <param name="tag">Restrict to a tag.</param>
        /// <returns>The matching <see cref="Document"/> list.</returns>
        public async Task<List<DocumentSummary>> QueryDocumentsAsync(
            string searchText,
            string authorAlias,
            string tag)
        {
            using var context = factory.CreateDbContext();

            var result = new HashSet<DocumentSummary>();

            var partialResults = false;

            if (!string.IsNullOrWhiteSpace(authorAlias))
            {
                partialResults = true;
                var author = await context.FindMetaAsync<Author>(authorAlias);
                foreach (var ds in author.Documents)
                {
                    result.Add(ds);
                }
            }

            if (!string.IsNullOrWhiteSpace(tag))
            {
                var tagEntity = await context.FindMetaAsync<Tag>(tag);

                var resultSet =
                    Enumerable.Empty<DocumentSummary>();

                // alias _AND_ tag
                if (partialResults)
                {
                    resultSet = result.Intersect(tagEntity.Documents);
                }
                else
                {
                    resultSet = tagEntity.Documents;
                }

                result.Clear();

                foreach (var docSummary in resultSet)
                {
                    result.Add(docSummary);
                }

                partialResults = true;
            }

            // nothing more to do?
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return result.OrderBy(r => r.Title).ToList();
            }

            // no list to filter further
            if (partialResults && result.Count < 1)
            {
                return result.ToList();
            }

            // find documents that match
            var documents = await context.Documents.Where(
                d => d.Title.Contains(searchText) ||
                d.Description.Contains(searchText) ||
                d.Markdown.Contains(searchText))
                .ToListAsync();

            // now only intersect with alias/tag constraints
            if (partialResults)
            {
                var uids = result.Select(ds => ds.Uid).ToList();
                documents = documents.Where(d => uids.Contains(d.Uid))
                    .ToList();
            }

            return documents.Select(d => new DocumentSummary(d))
                 .OrderBy(ds => ds.Title).ToList();
        }

        /// <summary>
        /// Searches author aliases.
        /// </summary>
        /// <param name="searchText">Text to search on.</param>
        /// <returns>The list of matching aliases.</returns>
        public async Task<List<string>> SearchAuthorsAsync(string searchText)
        {
            using var context = factory.CreateDbContext();
            var partitionKey = DocsContext.ComputePartitionKey<Author>();
            return (await context.Authors
                .WithPartitionKey(partitionKey)
                .Select(a => a.Alias)
                .ToListAsync())
                .Where(
                    a => a.Contains(searchText, System.StringComparison.InvariantCultureIgnoreCase))
                .OrderBy(a => a)
                .ToList();
        }

        /// <summary>
        /// Searches tags.
        /// </summary>
        /// <param name="searchText">Text to search on.</param>
        /// <returns>The list of matching tags.</returns>
        public async Task<List<string>> SearchTagsAsync(string searchText)
        {
            using var context = factory.CreateDbContext();
            var partitionKey = DocsContext.ComputePartitionKey<Tag>();
            var toSearch = searchText.Trim();
            return (await context.Tags
                .WithPartitionKey(partitionKey)
                .Select(t => t.TagName)
                .ToListAsync())
                .Where(t => t.Contains(
                    searchText,
                    System.StringComparison.InvariantCultureIgnoreCase))
                .OrderBy(t => t)
                .ToList();
        }

        /// <summary>
        /// Updates an existing document.
        /// </summary>
        /// <param name="document">The <see cref="Document"/> to update.</param>
        /// <returns>An asynchronous <see cref="Task"/>.</returns>
        public async Task UpdateDocumentAsync(Document document)
        {
            using var context = factory.CreateDbContext();

            await HandleMetaAsync(context, document);

            context.Update(document);

            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves the audit history of the <see cref="Document"/>.
        /// </summary>
        /// <param name="uid">The unique identifier of the <see cref="Document"/>.</param>
        /// <returns>The list of audit entries.</returns>
        public async Task<List<DocumentAuditSummary>> LoadDocumentHistoryAsync(string uid)
        {
            using var context = factory.CreateDbContext();
            return (await context.Audits
                .WithPartitionKey(uid)
                .Where(da => da.Uid == uid)
                .ToListAsync())
                .Select(da => new DocumentAuditSummary(da))
                .OrderBy(das => das.Timestamp)
                .ToList();
        }

        /// <summary>
        /// Loads a specific snapshot.
        /// </summary>
        /// <param name="guid">The unique identifier of the snapshot.</param>
        /// <param name="uid">The unique identifier of the document.</param>
        /// <returns>The document snapshot.</returns>
        public async Task<Document> LoadDocumentSnapshotAsync(System.Guid guid, string uid)
        {
            using var context = factory.CreateDbContext();
            try
            {
                var audit = await context.FindAsync<DocumentAudit>(guid, uid);
                return audit.GetDocumentSnapshot();
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
        /// Deletes a document.
        /// </summary>
        /// <param name="uid">The unique identifier.</param>
        /// <returns>The asynchronous task.</returns>
        public async Task DeleteDocumentAsync(string uid)
        {
            using var context = factory.CreateDbContext();
            var docToDelete = await LoadDocumentAsync(uid);
            var author = await context.FindMetaAsync<Author>(docToDelete.AuthorAlias);
            var summary = author.Documents.Find(d => d.Uid == uid);
            if (summary != null)
            {
                author.Documents.Remove(summary);
                context.Update(author);
            }

            foreach (var tag in docToDelete.Tags)
            {
                var tagEntity = await context.FindMetaAsync<Tag>(tag);
                var tagSummary = tagEntity.Documents.Find(d => d.Uid == uid);
                if (tagSummary != null)
                {
                    tagEntity.Documents.Remove(tagSummary);
                    context.Update(tagEntity);
                }
            }

            context.Remove(docToDelete);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Restores a version of the deleted document.
        /// </summary>
        /// <param name="id">The id of the audit.</param>
        /// <param name="uid">The unique identifiers of the document.</param>
        /// <returns>The restored document.</returns>
        public async Task<Document> RestoreDocumentAsync(Guid id, string uid)
        {
            var snapshot = await LoadDocumentSnapshotAsync(id, uid);
            await InsertDocumentAsync(snapshot);
            return await LoadDocumentAsync(uid);
        }

        /// <summary>
        /// Gets the document without tracking for comparisons.
        /// </summary>
        /// <param name="context">The <see cref="DocsContext"/>.</param>
        /// <param name="document">The <see cref="Document"/> to reference.</param>
        /// <returns>The <see cref="Document"/> instance.</returns>
        private static async Task<Document> LoadDocNoTrackingAsync(
        DocsContext context, Document document) =>
            await context.Documents
                .WithPartitionKey(document.Uid)
                .AsNoTracking()
                .SingleOrDefaultAsync(d => d.Uid == document.Uid);

        /// <summary>
        /// Keeps tags and authors in sync.
        /// </summary>
        /// <param name="context">The <see cref="DocsContext"/>.</param>
        /// <param name="document">The <see cref="Document"/> to sync.</param>
        /// <returns>An asynchronous task.</returns>
        private static async Task HandleMetaAsync(
            DocsContext context,
            Document document)
        {
            var authorChanged =
                await CheckAuthorChangedAsync(context, document);
            await HandleTagsAsync(context, document, authorChanged);
        }

        /// <summary>
        /// Keeps tags in sync.
        /// </summary>
        /// <param name="context">The <see cref="DocsContext"/>.</param>
        /// <param name="document">The <see cref="Document"/>.</param>
        /// <param name="authorChanged">A value indicating whether the author was updated.</param>
        /// <returns>An asynchronous task.</returns>
        private static async Task HandleTagsAsync(
            DocsContext context,
            Document document,
            bool authorChanged)
        {
            var refDoc = await LoadDocNoTrackingAsync(context, document);

            // did the title change?
            var updatedTitle = refDoc != null && refDoc.Title != document.Title;

            // tags removed need summary taken away
            if (refDoc != null)
            {
                var removed = refDoc.Tags.Where(
                    t => !document.Tags.Any(dt => dt == t));

                foreach (var removedTag in removed)
                {
                    var tag = await context.FindMetaAsync<Tag>(removedTag);

                    if (tag != null)
                    {
                        var docSummary =
                            tag.Documents.Find(
                                d => d.Uid == document.Uid);

                        if (docSummary != null)
                        {
                            tag.Documents.Remove(docSummary);
                            context.Entry(tag).State = EntityState.Modified;
                        }
                    }
                }
            }

            // figure out new tags
            var tagsAdded = refDoc == null ?
                document.Tags : document.Tags.Where(
                    t => !refDoc.Tags.Any(rt => rt == t));

            // do existing tags need title updated?
            if (updatedTitle || authorChanged)
            {
                // added ones will be handled later
                var tagsToChange = document.Tags.Except(tagsAdded);

                foreach (var tagName in tagsToChange)
                {
                    var tag = await context.FindMetaAsync<Tag>(tagName);
                    var ds = tag.Documents.SingleOrDefault(ds => ds.Uid == document.Uid);
                    if (ds != null)
                    {
                        ds.Title = document.Title;
                        ds.AuthorAlias = document.AuthorAlias;
                        context.Entry(tag).State = EntityState.Modified;
                    }
                }
            }

            // brand new tags (for the document)
            foreach (var tagAdded in tagsAdded)
            {
                var tag = await context.FindMetaAsync<Tag>(tagAdded);

                // new tag (overall)
                if (tag == null)
                {
                    tag = new Tag { TagName = tagAdded };
                    context.SetPartitionKey(tag);
                    context.Add(tag);
                }
                else
                {
                    context.Entry(tag).State = EntityState.Modified;
                }

                // either way, add the document summary
                tag.Documents.Add(new DocumentSummary(document));
            }
        }

        /// <summary>
        /// Handles sync of authors.
        /// </summary>
        /// <param name="context">The <see cref="DocsContext"/>.</param>
        /// <param name="document">The <see cref="Document"/>.</param>
        /// <returns>A value indicating whether the author changed.</returns>
        private static async Task<bool> CheckAuthorChangedAsync(
            DocsContext context, Document document)
        {
            var changed = false;
            var refDoc = await LoadDocNoTrackingAsync(context, document);

            // did the title change?
            if (refDoc != null && refDoc.AuthorAlias == document.AuthorAlias
                && refDoc.Title != document.Title)
            {
                var author = await context.FindMetaAsync<Author>(document.AuthorAlias);
                var docSummary = author.Documents.Single(ds => ds.Uid == document.Uid);
                docSummary.Title = document.Title;
                context.Entry(author).State = EntityState.Modified;
            }

            // did the author change? (always true for a new document)
            if (refDoc == null || refDoc.AuthorAlias != document.AuthorAlias)
            {
                if (refDoc != null)
                {
                    changed = true;
                    var oldAuthor = refDoc.AuthorAlias;
                    var author = await context.FindMetaAsync<Author>(oldAuthor);
                    var doc = author.Documents.SingleOrDefault(
                        d => d.Uid == document.Uid);
                    if (doc != null)
                    {
                        author.Documents.Remove(doc);
                        context.Entry(author).State = EntityState.Modified;
                    }
                }

                var newAuthor = await context.FindMetaAsync<Author>(
                    document.AuthorAlias);

                if (newAuthor == null)
                {
                    newAuthor = new Author { Alias = document.AuthorAlias };
                    context.SetPartitionKey(newAuthor);
                    context.Add(newAuthor);
                }
                else
                {
                    context.Entry(newAuthor).State = EntityState.Modified;
                }

                newAuthor.Documents.Add(new DocumentSummary(document));
            }

            return changed;
        }
    }
}
