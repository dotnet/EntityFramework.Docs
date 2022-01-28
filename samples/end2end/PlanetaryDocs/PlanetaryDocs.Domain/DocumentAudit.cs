using System;
using System.Text.Json;

namespace PlanetaryDocs.Domain
{
    /// <summary>
    /// Represents a snapshot of the document.
    /// </summary>
    public class DocumentAudit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentAudit"/> class.
        /// </summary>
        public DocumentAudit()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentAudit"/> class
        /// and configures it with the <see cref="Document"/> passed in.
        /// </summary>
        /// <param name="document">The document to audit.</param>
        public DocumentAudit(Document document)
        {
            Id = Guid.NewGuid();
            Uid = document.Uid;
            Document = JsonSerializer.Serialize(document);
            Timestamp = DateTimeOffset.UtcNow;
        }

        /// <summary>
        /// Gets or sets a unique identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the document.
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the audit.
        /// </summary>
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the JSON serialized snapshot.
        /// </summary>
        public string Document { get; set; }

        /// <summary>
        /// Deserializes the snapshot.
        /// </summary>
        /// <returns>The <see cref="Document"/> snapshot.</returns>
        public Document GetDocumentSnapshot() =>
            JsonSerializer.Deserialize<Document>(Document);
    }
}
