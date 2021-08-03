using System.ComponentModel.DataAnnotations;
using general_shortener.Models.Entry;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace general_shortener.Models.Data
{
    /// <summary>
    /// Entry info response model
    /// </summary>
    public class EntryInfoResponseModel
    {
        /// <summary>
        /// Resource slug
        /// </summary>
        public string Slug { get; set; }
        
        /// <summary>
        /// Type of the entry
        /// </summary>
        [Required]
        [EnumDataType(typeof(EntryType))]
        [JsonConverter(typeof(StringEnumConverter))]
        public EntryType Type { get; set; }
        
        /// <summary>
        /// If entry is a file, size of the file
        /// </summary>
        public uint Size { get; set; }
        
        /// <summary>
        /// Deletion code of the resource if needed
        /// </summary>
        public string DeletionCode { get; set; }
        
        /// <summary>
        /// Mimetype of the entry if entry is a file
        /// </summary>
        public string Mime { get; set; }
    }
}