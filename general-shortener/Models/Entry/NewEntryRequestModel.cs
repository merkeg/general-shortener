using System.ComponentModel.DataAnnotations;
using general_shortener.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public EntryType type { get; set; }
        
        /// <summary>
        /// Value of the new entry
        /// </summary>
        [MaxLength(1024)]
        public string value { get; set; }
        
        /// <summary>
        /// Custom slug for entry
        /// </summary>
        [Range(1, 128)]
        public string slug { get; set; }

        /// <summary>
        /// File to upload
        /// </summary>
        public IFormFile file { get; set; }
    }


    public enum EntryType
    {
        url,
        file,
        text
    }
}