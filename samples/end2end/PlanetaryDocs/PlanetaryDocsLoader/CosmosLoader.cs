using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlanetaryDocs.DataAccess;
using PlanetaryDocs.Domain;

namespace PlanetaryDocsLoader
{
    /// <summary>
    /// Saves the <see cref="Document"/> instances to Azure Cosmos DB.
    /// </summary>
    public static class CosmosLoader
    {
        /// <summary>
        /// Loads documents to the database.
        /// </summary>
        /// <param name="docsList">The <see cref="Document"/> list.</param>
        /// <param name="endPoint">The Azure Cosmos DB endpoint.</param>
        /// <param name="accessKey">The Azure Cosmos DB access key.</param>
        /// <returns>An asynchronous task.</returns>
        public static async Task LoadDocumentsAsync(
            IEnumerable<Document> docsList,
            string endPoint,
            string accessKey)
        {
            Console.Clear();
            Console.WriteLine("Initializing database...");

            string alias = string.Empty,
                altAlias = string.Empty,
                tag = string.Empty,
                text = string.Empty,
                uid = string.Empty;

            var testOnly = docsList == null;

            var sc = new ServiceCollection();
            sc.AddDbContextFactory<DocsContext>(
                options => options.UseCosmos(
                    endPoint, accessKey, nameof(DocsContext)));
            sc.AddSingleton<IDocumentService, DocumentService>();
            var sp = sc.BuildServiceProvider().CreateScope().ServiceProvider;
            var factory = sp.GetRequiredService<IDbContextFactory<DocsContext>>();
            var service = sp.GetRequiredService<IDocumentService>();

            using (var context = factory.CreateDbContext())
            {
                if (testOnly)
                {
                    docsList = await context.Documents.ToListAsync();
                }
                else
                {
                    await context.Database.EnsureDeletedAsync();
                    await context.Database.EnsureCreatedAsync();
                }
            }

            Console.WriteLine("Database created. Populating...");

            var (left, right) = Console.GetCursorPosition();
            var emptyLine = new string(' ', Console.BufferWidth - 1);
            var current = 1;
            var total = docsList.Count();
            var progress = new ProgressBar(total);
            foreach (var doc in docsList)
            {
                Console.SetCursorPosition(left, right);
                Console.WriteLine(emptyLine);
                Console.WriteLine(emptyLine);
                Console.SetCursorPosition(left, right);
                Console.WriteLine(progress.Advance());
                Console.WriteLine($"{current++}/{total}:\tInsert {doc.Uid}");
                if (!testOnly)
                {
                    await service.InsertDocumentAsync(doc);
                }

                if (doc.Tags.Count > 0 && string.IsNullOrWhiteSpace(alias))
                {
                    alias = doc.AuthorAlias;
                    tag = doc.Tags[0];
                    text = doc.Title;
                    uid = doc.Uid;
                }
                else if (
                    string.IsNullOrWhiteSpace(altAlias) &&
                    doc.AuthorAlias != alias)
                {
                    altAlias = doc.AuthorAlias;
                }
            }

            Console.WriteLine("The way has been prepared.");
            Console.WriteLine("Running tests...");

            await TestLoadDocumentAsync(service, uid);
            await TestUpdateDocumentAsync(service, uid, alias, altAlias);
            await TestSearchTagsAsync(service, tag);
            await TestSearchAuthorsAsync(service, alias);
            await TestQueryAsync(service, tag, alias, text);
            var auditId = await TestDeleteDocumentAsync(service, uid);
            await TestRestoreDocumentAsync(service, auditId, uid);

            Console.WriteLine("All done!");
        }

        /// <summary>
        /// Load a <see cref="Document"/>.
        /// </summary>
        /// <param name="service">The <see cref="IDocumentService"/>.</param>
        /// <param name="uid">The unique identifier.</param>
        /// <returns>An asynchronous task.</returns>
        private static async Task TestLoadDocumentAsync(
            IDocumentService service, string uid)
        {
            var start = TestStart();
            var doc = await service.LoadDocumentAsync(uid);
            Console.WriteLine("Loaded document:");
            Console.WriteLine($"Title:\t{doc.Title}");
            Console.WriteLine($"Description:\t{doc.Description}");
            Console.Write("Tags:\t");
            Console.WriteLine(string.Join(", ", doc.Tags));
            TestEnd(start);
        }

        /// <summary>
        /// Updates a <see cref="Document"/>.
        /// </summary>
        /// <param name="service">The <see cref="IDocumentService"/>.</param>
        /// <param name="uid">The unique identifier.</param>
        /// <param name="author">An author to update.</param>
        /// <param name="altAuthor">An alternate author.</param>
        /// <returns>An asyncronous task.</returns>
        private static async Task TestUpdateDocumentAsync(
            IDocumentService service,
            string uid,
            string author,
            string altAuthor)
        {
            var start = TestStart();
            var doc = await service.LoadDocumentAsync(uid);
            Console.WriteLine($"Updating doc with author {doc.AuthorAlias}...");
            var newAuthor = doc.AuthorAlias == author ?
                altAuthor : author;
            Console.WriteLine($"New author = {newAuthor}.");
            var originalAuthor = doc.AuthorAlias;
            var existsBeforeOriginal = (await service.QueryDocumentsAsync(
                string.Empty,
                originalAuthor,
                string.Empty)).Any(d => d.Uid == doc.Uid);
            var existsBeforeNew = (await service.QueryDocumentsAsync(
                string.Empty,
                newAuthor,
                string.Empty)).Any(d => d.Uid == doc.Uid);
            doc.AuthorAlias = newAuthor;
            doc.Title = $"(new) {doc.Title}";
            doc.Tags.RemoveAt(0);
            await service.UpdateDocumentAsync(doc);
            var updatedDoc = await service.LoadDocumentAsync(doc.Uid);
            var existsAfterOriginal = (await service.QueryDocumentsAsync(
                string.Empty,
                originalAuthor,
                string.Empty)).Any(d => d.Uid == doc.Uid);
            var existsAfterNew = (await service.QueryDocumentsAsync(
                string.Empty,
                newAuthor,
                string.Empty)).Any(d => d.Uid == doc.Uid);
            Console.WriteLine("Updated document.");
            Console.WriteLine($"Author after update: {updatedDoc.AuthorAlias}.");
            Console.WriteLine($"{originalAuthor} before: {existsBeforeOriginal} after: {existsAfterOriginal}");
            Console.WriteLine($"{newAuthor} before: {existsBeforeNew} after: {existsAfterNew}");
            TestEnd(start);
        }

        /// <summary>
        /// Search authors.
        /// </summary>
        /// <param name="service">The <see cref="IDocumentService"/>.</param>
        /// <param name="alias">The alias to search.</param>
        /// <returns>An asynchronous tsk.</returns>
        private static async Task TestSearchAuthorsAsync(
            IDocumentService service, string alias)
        {
            var start = TestStart();
            Console.WriteLine("Testing author search...");
            for (var x = 1; x < alias.Length - 1; x++)
            {
                var search = alias[..x];
                var results = await service.SearchAuthorsAsync(search);
                var resultText = string.Join(',', results);
                Console.WriteLine($"{search}=>{resultText}");
            }

            TestEnd(start);
        }

        /// <summary>
        /// Test tag search.
        /// </summary>
        /// <param name="service">The <see cref="IDocumentService"/>.</param>
        /// <param name="tag">The tag search.</param>
        /// <returns>An asynchronous task.</returns>
        private static async Task TestSearchTagsAsync(
            IDocumentService service, string tag)
        {
            var start = TestStart();
            Console.WriteLine("Testing tag search...");
            for (var x = 1; x < tag.Length - 1; x++)
            {
                var search = tag[..x];
                var results = await service.SearchTagsAsync(search);
                var resultText = string.Join(',', results);
                Console.WriteLine($"{search}=>{resultText}");
            }

            TestEnd(start);
        }

        /// <summary>
        /// Test document queries.
        /// </summary>
        /// <param name="service">The <see cref="IDocumentService"/>.</param>
        /// <param name="tag">The tag to use.</param>
        /// <param name="alias">The alias to use.</param>
        /// <param name="text">The text to use.</param>
        /// <returns>An asynchronous task.</returns>
        private static async Task TestQueryAsync(
            IDocumentService service,
            string tag,
            string alias,
            string text)
        {
            var startTime = TestStart();
            Console.WriteLine("Testing query...");

            var textParts = text.Split(' ');
            var start = new Random().Next(0, textParts.Length - 2);
            var search = $"{textParts[start]} {textParts[start + 1]}";

            Console.WriteLine($"Using tag={tag} alias={alias} text={search}");

            foreach (var useTag in new[] { true, false })
            {
                foreach (var useAlias in new[] { true, false })
                {
                    foreach (var useText in new[] { true, false })
                    {
                        Console.WriteLine($"tag={useTag} alias={useAlias} text={useText}");
                        var results = await service.QueryDocumentsAsync(
                            useText ? search : string.Empty,
                            useAlias ? alias : string.Empty,
                            useTag ? tag : string.Empty);
                        foreach (var item in results)
                        {
                            Console.WriteLine($"Found {item.Uid} by {item.AuthorAlias}: '{item.Title}'");
                        }
                    }
                }
            }

            TestEnd(startTime);
        }

        /// <summary>
        /// Test document delete.
        /// </summary>
        /// <param name="service">The <see cref="IDocumentService"/>.</param>
        /// <param name="uid">The unique identifier.</param>
        /// <returns>An audit identifier to restore later.</returns>
        private static async Task<Guid> TestDeleteDocumentAsync(
            IDocumentService service, string uid)
        {
            var start = TestStart();
            var docToDelete = await service.LoadDocumentAsync(uid);
            Console.WriteLine($"Document to delete: {uid}");
            var audits = await service.LoadDocumentHistoryAsync(uid);
            var history = audits.OrderByDescending(a => a.Timestamp)
                .Select(a => a.Id)
                .FirstOrDefault();
            await service.DeleteDocumentAsync(uid);
            var document = await service.LoadDocumentAsync(uid);
            if (document == null)
            {
                Console.WriteLine("SUCCESS! Checking dependencies.");
                var authorDocs = await service.QueryDocumentsAsync(
                    string.Empty,
                    docToDelete.AuthorAlias,
                    string.Empty);
                Console.WriteLine($"Author '{docToDelete.AuthorAlias}' has {authorDocs.Count} docs.");
                var docExists = authorDocs.Any(ds => ds.Uid == uid);
                Console.WriteLine($"Author has document in summary: {docExists}");
                foreach (var tag in docToDelete.Tags)
                {
                    var tagDocs = await service.QueryDocumentsAsync(
                        string.Empty,
                        string.Empty,
                        tag);
                    Console.WriteLine($"Tag '{tag}' has {tagDocs.Count} docs.");
                    docExists = tagDocs.Any(ds => ds.Uid == uid);
                    Console.WriteLine($"Tag has document in summary: {docExists}");
                }
            }
            else
            {
                Console.WriteLine("I've done something wrong. Back to the labs!");
            }

            TestEnd(start);
            return history;
        }

        /// <summary>
        /// Test restoration of the document.
        /// </summary>
        /// <param name="service">The <see cref="IDocumentService"/>.</param>
        /// <param name="auditId">The id of the <see cref="DocumentAudit"/>.</param>
        /// <param name="uid">The unique identifier of the <see cref="Document"/>.</param>
        /// <returns>An asynchronous task.</returns>
        private static async Task TestRestoreDocumentAsync(
            IDocumentService service,
            Guid auditId,
            string uid)
        {
            var start = TestStart();
            var doc = await service.LoadDocumentAsync(uid);
            Console.WriteLine($"Doc '{uid}' exists = {doc != null}");
            var restoredDoc = await service.RestoreDocumentAsync(auditId, uid);
            Console.WriteLine($"Doc '{uid}' restored = {restoredDoc != null}");
            TestEnd(start);
        }

        /// <summary>
        /// Mark test start.
        /// </summary>
        /// <param name="test">The test.</param>
        private static long TestStart([CallerMemberName] string test = null)
        {
            Console.WriteLine($"{Environment.NewLine}*** Starting Test '{test}'***");
            return DateTime.UtcNow.Ticks;
        }

        /// <summary>
        /// Mark test end.
        /// </summary>
        /// <param name="start">The start time.</param>
        /// <param name="test">The test.</param>
        private static void TestEnd(long start, [CallerMemberName] string test = null)
        {
            var time = TimeSpan.FromTicks(DateTime.UtcNow.Ticks - start);
            Console.WriteLine($"==== Test time: {time}");
            Console.WriteLine($"*** Ended Test '{test}'");
        }
    }
}
