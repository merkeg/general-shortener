﻿using Newtonsoft.Json;

namespace general_shortener.Models.Entry
{
    /// <summary>
    /// Response model
    /// </summary>
    public class NewEntryResponseModel
    {
        /// <summary>
        /// Url to access the resouce
        /// </summary>
        public string Url { get; set; }
        
        /// <summary>
        /// Url to delete the entry with an easy get request
        /// </summary>
        [JsonProperty(PropertyName = "deletion_url")]
        public string DeletionUrl { get; set; }
    }
}