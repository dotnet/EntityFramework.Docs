namespace PlanetaryDocs.Domain
{
    /// <summary>
    /// Represents a summary of a document.
    /// </summary>
    public class DocumentSummary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentSummary"/> class.
        /// </summary>
        public DocumentSummary()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentSummary"/> class
        /// and initializes it with the <see cref="Document"/>.
        /// </summary>
        /// <param name="doc">The <see cref="Document"/> to summarize.</param>
        public DocumentSummary(Document doc)
        {
            Uid = doc.Uid;
            Title = doc.Title;
            AuthorAlias = doc.AuthorAlias;
        }

        /// <summary>
        /// Gets or sets the unique id of the <see cref="Document"/>.
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the alias of the author.
        /// </summary>
        public string AuthorAlias { get; set; }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>The hash code of the document identifier.</returns>
        public override int GetHashCode() => Uid.GetHashCode();

        /// <summary>
        /// Implements equality.
        /// </summary>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>A value indicating whether the unique identifiers match.</returns>
        public override bool Equals(object obj) =>
            obj is DocumentSummary ds && ds.Uid == Uid;

        /// <summary>
        /// Gets the string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString() => $"Summary for {Uid} by {AuthorAlias}: {Title}.";
    }
}
