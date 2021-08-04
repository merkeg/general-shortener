using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace general_shortener.Models.Entry
{
    /// <summary>
    /// Entry model
    /// </summary>
    public class Entry
    {
        /// <summary>
        /// Id of the Entry in the document.
        /// </summary>
        [BsonId]
        public int Id { get; set; }
        
        /// <summary>
        /// Entry slug
        /// </summary>
        public string Slug { get; set; }
        
        /// <summary>
        /// Type of the entry
        /// </summary>
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
        
        /// <summary>
        /// Owner of the entry
        /// </summary>
        public int Owner { get; set; }
    }
}