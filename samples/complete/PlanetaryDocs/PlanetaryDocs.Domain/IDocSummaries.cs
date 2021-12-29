using System.Collections.Generic;

namespace PlanetaryDocs.Domain
{
    /// <summary>
    /// Indicates classes with summaries.
    /// </summary>
    public interface IDocSummaries
    {
        /// <summary>
        /// Gets the list of summaries.
        /// </summary>
        List<DocumentSummary> Documents { get; }
    }
}
