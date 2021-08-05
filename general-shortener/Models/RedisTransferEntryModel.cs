using general_shortener.Models.Entry;

namespace general_shortener.Models
{
    /// <summary>
    /// Old entry model, transfer model
    /// </summary>
    public class RedisTransferEntryModel
    {
        /// <summary>
        /// Type
        /// </summary>
        public EntryType Type;
        
        /// <summary>
        /// Value
        /// </summary>
        public string Value;
        
        /// <summary>
        /// Mimetype
        /// </summary>
        public string Mime;
        
        /// <summary>
        /// Size
        /// </summary>
        public long Size;
        
        /// <summary>
        /// DeletionCode
        /// </summary>
        public string DeletionCode;
    }
}