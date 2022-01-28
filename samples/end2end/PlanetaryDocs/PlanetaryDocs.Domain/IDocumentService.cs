using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlanetaryDocs.Domain
{
    /// <summary>
    /// Data service definition.
    /// </summary>
    public interface IDocumentService
    {
        /// <summary>
        /// Add a new document.
        /// </summary>
        /// <param name="document">The <see cref="Document"/> to add.</param>
        /// <returns>An asynchronous <see cref="Task"/>.</returns>
        Task InsertDocumentAsync(Document document);

        /// <summary>
        /// Updates an existing document.
        /// </summary>
        /// <param name="document">The <see cref="Document"/> to update.</param>
        /// <returns>An asynchronous <see cref="Task"/>.</returns>
        Task UpdateDocumentAsync(Document document);

        /// <summary>
        /// Searches tags.
        /// </summary>
        /// <param name="searchText">Text to search on.</param>
        /// <returns>The list of matching tags.</returns>
        Task<List<string>> SearchTagsAsync(string searchText);

        /// <summary>
        /// Searches author aliases.
        /// </summary>
        /// <param name="searchText">Text to search on.</param>
        /// <returns>The list of matching aliases.</returns>
        Task<List<string>> SearchAuthorsAsync(string searchText);

        /// <summary>
        /// Query to obtain a <see cref="Document"/> list.
        /// </summary>
        /// <param name="searchText">Text to look for.</param>
        /// <param name="authorAlias">Restrict to an author.</param>
        /// <param name="tag">Restrict to a tag.</param>
        /// <returns>The matching <see cref="Document"/> list.</returns>
        Task<List<DocumentSummary>> QueryDocumentsAsync(
            string searchText,
            string authorAlias,
            string tag);

        /// <summary>
        /// Loads a single <see cref="Document"/>.
        /// </summary>
        /// <param name="uid">The unique identifier.</param>
        /// <returns>The <see cref="Document"/>.</returns>
        Task<Document> LoadDocumentAsync(string uid);

        /// <summary>
        /// Retrieves the audit history of the <see cref="Document"/>.
        /// </summary>
        /// <param name="uid">The unique identifier of the <see cref="Document"/>.</param>
        /// <returns>The list of audit entries.</returns>
        Task<List<DocumentAuditSummary>> LoadDocumentHistoryAsync(string uid);

        /// <summary>
        /// Loads a specific snapshot.
        /// </summary>
        /// <param name="guid">The unique identifier of the snapshot.</param>
        /// <param name="uid">The unique identifier of the document.</param>
        /// <returns>The document snapshot.</returns>
        Task<Document> LoadDocumentSnapshotAsync(Guid guid, string uid);

        /// <summary>
        /// Deletes a document.
        /// </summary>
        /// <param name="uid">The unique identifier.</param>
        /// <returns>The asynchronous task.</returns>
        Task DeleteDocumentAsync(string uid);

        /// <summary>
        /// Restores a version of the deleted document.
        /// </summary>
        /// <param name="id">The id of the audit.</param>
        /// <param name="uid">The unique identifiers of the document.</param>
        /// <returns>The restored document.</returns>
        Task<Document> RestoreDocumentAsync(Guid id, string uid);
    }
}
