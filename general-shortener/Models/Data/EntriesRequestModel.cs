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
        public uint? limit { get; set; }
        
        /// <summary>
        /// Data offset
        /// </summary>
        public uint offset { get; set; }
    }
}