using System;
using System.Collections.Generic;

namespace PlanetaryDocs.Domain
{
    /// <summary>
    /// A document item.
    /// </summary>
    public class Document
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the published date.
        /// </summary>
        public DateTime PublishDate { get; set; }

        /// <summary>
        /// Gets or sets the markdown content.
        /// </summary>
        public string Markdown { get; set; }

        /// <summary>
        /// Gets or sets the generated html.
        /// </summary>
        public string Html { get; set; }

        /// <summary>
        /// Gets or sets the author's alias.
        /// </summary>
        public string AuthorAlias { get; set; }

        /// <summary>
        /// Gets or sets the list of related tags.
        /// </summary>
        public List<string> Tags { get; set; }
            = new List<string>();

        /// <summary>
        /// Gets or sets the concurrency token.
        /// </summary>
        public string ETag { get; set; }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>The hash code of the unique identifier.</returns>
        public override int GetHashCode() => Uid.GetHashCode();

        /// <summary>
        /// Implements equality.
        /// </summary>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>A value indicating whether the unique identifiers match.</returns>
        public override bool Equals(object obj) =>
            obj is Document document && document.Uid == Uid;

        /// <summary>
        /// Gets the string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString() =>
            $"Document {Uid} by {AuthorAlias} with {Tags.Count} tags: {Title}.";
    }
}
