using System.Collections.Generic;

namespace PlanetaryDocs.Domain
{
    /// <summary>
    /// A tag.
    /// </summary>
    public class Tag : IDocSummaries
    {
        /// <summary>
        /// Gets or sets the name of the tag.
        /// </summary>
        public string TagName { get; set; }

        /// <summary>
        /// Gets or sets a summary of documents with the tag.
        /// </summary>
        public List<DocumentSummary> Documents { get; set; }
            = new List<DocumentSummary>();

        /// <summary>
        /// Gets or sets the concurrency token.
        /// </summary>
        public string ETag { get; set; }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>The hash code of the tag name.</returns>
        public override int GetHashCode() => TagName.GetHashCode();

        /// <summary>
        /// Implements equality.
        /// </summary>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>A value indicating whether the tag names match.</returns>
        public override bool Equals(object obj) =>
            obj is Tag tag && tag.TagName == TagName;

        /// <summary>
        /// Gets the string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString() =>
            $"Tag {TagName} tagged by {Documents.Count} documents.";
    }
}
