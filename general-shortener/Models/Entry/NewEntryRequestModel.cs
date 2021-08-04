using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// ReSharper disable All
#pragma warning disable 1591

namespace general_shortener.Models.Entry
{
    /// <summary>
    /// New entry request model
    /// </summary>
    public class NewEntryRequestModel
    {
        /// <summary>
        /// Type of the new entry
        /// </summary>
        [Required]
        [EnumDataType(typeof(EntryType))]
        [JsonConverter(typeof(StringEnumConverter))]
        public EntryType Type { get; set; }
        
        /// <summary>
        /// Value of the new entry
        /// </summary>
        [MaxLength(1024)]
        public string Value { get; set; }
        
        /// <summary>
        /// Custom slug for entry, if entry with specified slug already exists, it will be overwritten and gives an 200 instead of an 201
        /// </summary>
        [Range(1, 128)]
        public string Slug { get; set; }

        /// <summary>
        /// File to upload
        /// </summary>
        private IFormFile File { get; set; }
    }


    public enum EntryType
    {
        url,
        file,
        text
    }
}