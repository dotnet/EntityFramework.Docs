using System.Collections.Generic;

namespace PlanetaryDocs.Domain
{
    /// <summary>
    /// A document author.
    /// </summary>
    public class Author : IDocSummaries
    {
        /// <summary>
        /// Gets or sets the alias.
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Gets the list of documents by this author.
        /// </summary>
        public List<DocumentSummary> Documents { get; }
            = new List<DocumentSummary>();

        /// <summary>
        /// Gets or sets the concurrency tag.
        /// </summary>
        public string ETag { get; set; }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>The hashcode of the alias.</returns>
        public override int GetHashCode() => Alias.GetHashCode();

        /// <summary>
        /// Implements equality.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>A value indicating whether the aliases match.</returns>
        public override bool Equals(object obj) =>
            obj is Author author && author.Alias == Alias;

        /// <summary>
        /// Gets the string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString() =>
            $"Author {Alias} has {Documents.Count} documents.";
    }
}
