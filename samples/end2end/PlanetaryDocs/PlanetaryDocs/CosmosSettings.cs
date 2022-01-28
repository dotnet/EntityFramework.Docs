namespace PlanetaryDocs
{
    /// <summary>
    /// Settings to connect to Cosmos DB.
    /// </summary>
    public class CosmosSettings
    {
        /// <summary>
        /// Gets or sets the endpoint.
        /// </summary>
        public string EndPoint { get; set; }

        /// <summary>
        /// Gets or sets the access key.
        /// </summary>
        public string AccessKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether startup should check for migrations.
        /// </summary>
        public bool EnableMigration { get; set; }

        /// <summary>
        /// Gets or sets the id of the document to check for migration.
        /// </summary>
        public string DocumentToCheck { get; set; }
    }
}
