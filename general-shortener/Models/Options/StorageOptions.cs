namespace general_shortener.Models.Options
{
    /// <summary>
    /// Storage option model class
    /// </summary>
    public class StorageOptions
    {
        /// <summary>
        /// Section in the settings
        /// </summary>
        public const string Section = "Storage";
        
        /// <summary>
        /// Storage driver
        /// </summary>
        public string Driver { get; set; }
        
        /// <summary>
        /// Local storage path
        /// </summary>
        public string Path { get; set; }
    }
}