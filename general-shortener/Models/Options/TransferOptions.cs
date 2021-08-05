namespace general_shortener.Models.Options
{
    /// <summary>
    /// Transfer options
    /// </summary>
    public class TransferOptions
    {
        /// <summary>
        /// Section
        /// </summary>
        public const string Section = "Transfer";
        
        /// <summary>
        /// Transfer data?
        /// </summary>
        public bool Transfer { get; set; }
        
        /// <summary>
        /// Redis transfer configuration
        /// </summary>
        public TransferRedisOptions Redis { get; set; }
    }

    /// <summary>
    /// Transfer redis options
    /// </summary>
    public class TransferRedisOptions
    {
        
        /// <summary>
        /// Redis Host
        /// </summary>
        public string Host { get; set; }
        
        /// <summary>
        /// Redis port
        /// </summary>
        public string Port { get; set; }
        
        
        /// <summary>
        /// Redis password
        /// </summary>
        public string Password { get; set; }
    }
}