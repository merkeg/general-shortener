namespace general_shortener.Models.Options
{
    /// <summary>
    /// Options model for MongoDB
    /// </summary>
    public class MongoDbOptions
    {
        /// <summary>
        /// Section in the Options
        /// </summary>
        public const string Section = "MongoDB";
        
        /// <summary>
        /// Connection string to connect to a mongo db server
        /// </summary>
        public string ConnectionString { get; set; }
        
        /// <summary>
        /// Name of the database
        /// </summary>
        public string DatabaseName { get; set; }
    }
}