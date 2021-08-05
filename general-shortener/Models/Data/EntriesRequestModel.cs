using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace general_shortener.Models.Data
{
    /// <summary>
    /// Entry list request model
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class EntriesRequestModel
    {
        /// <summary>
        /// Amount of items shown each request
        /// </summary>
        [Range(1, 100)] 
        public int? limit { get; set; }
        
        /// <summary>
        /// Data offset
        /// </summary>
        [Range(0, Int32.MaxValue)]
        public int? offset { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public EntriesRequestModel()
        {
            this.limit = 10;
        }
    }
}