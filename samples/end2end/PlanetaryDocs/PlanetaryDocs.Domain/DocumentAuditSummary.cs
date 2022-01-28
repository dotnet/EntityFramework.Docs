using System;

namespace PlanetaryDocs.Domain
{
    /// <summary>
    /// Simple class representing a document snapshot.
    /// </summary>
    public class DocumentAuditSummary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentAuditSummary"/> class.
        /// </summary>
        public DocumentAuditSummary()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentAuditSummary"/> class
        /// and initializes it with the <see cref="DocumentAudit"/>.
        /// </summary>
        /// <param name="documentAudit">The <see cref="DocumentAudit"/> to summarize.</param>
        public DocumentAuditSummary(DocumentAudit documentAudit)
        {
            Id = documentAudit.Id;
            Uid = documentAudit.Uid;
            Timestamp = documentAudit.Timestamp;
            var doc = documentAudit.GetDocumentSnapshot();
            Alias = doc.AuthorAlias;
            Title = doc.Title;
        }

        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the audit event.
        /// </summary>
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the document.
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// Gets or sets the author alias for the snapshot.
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Gets or sets the title at the time of the snapshot.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>The hash code of the <see cref="Id"/>.</returns>
        public override int GetHashCode() => Id.GetHashCode();

        /// <summary>
        /// Implement equality.
        /// </summary>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>A value indicating whether the object is a <see cref="DocumentAuditSummary"/> with the same <see cref="Id"/>.</returns>
        public override bool Equals(object obj) =>
            obj is DocumentAuditSummary das &&
            das.Id == Id;

        /// <summary>
        /// The string representation.
        /// </summary>
        /// <returns>The string representation of the instance.</returns>
        public override string ToString() => $"Summary for audit {Id} with document {Uid}.";
    }
}
