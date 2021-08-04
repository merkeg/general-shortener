using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
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
        /// Name of the collection
        /// </summary>
        public const string Collection = "entries";
        
        /// <summary>
        /// Id of the Entry in the document.
        /// </summary>
        [BsonId]
        public ObjectId Id { get; set; }
        
        /// <summary>
        /// Entry slug
        /// </summary>
        public string Slug { get; set; }
        
        /// <summary>
        /// Deletion code of the resource if needed
        /// </summary>
        public string DeletionCode { get; set; }
        
        /// <summary>
        /// Type of the entry
        /// </summary>
        public EntryType Type { get; set; }
        
        /// <summary>
        /// Value of the Entry, if it isn't a file
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Entry metadata
        /// </summary>
        public EntryMetadata Meta { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public Entry()
        {
            this.Id = ObjectId.GenerateNewId();
            this.Meta = new EntryMetadata();
        }
    }

    /// <summary>
    /// Entry metadata
    /// </summary>
    public class EntryMetadata
    {
        /// <summary>
        /// If entry is a file, size of the file
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// Mimetype of the entry if entry is a file
        /// </summary>
        public string Mime { get; set; }
        
        /// <summary>
        /// Local filename of the entry
        /// </summary>
        public string Filename { get; set; }
        
        /// <summary>
        /// Original filename from the request
        /// </summary>
        public string OriginalFilename { get; set; }
        
        /// <summary>
        /// Owner of the entry
        /// </summary>
        public ObjectId Owner { get; set; }
    }
}