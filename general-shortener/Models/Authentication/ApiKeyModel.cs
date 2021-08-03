// ReSharper disable InconsistentNaming
#pragma warning disable 1591
namespace general_shortener.Models.Authentication
{
    public class ApiKeyModel
    {
        /// <summary>
        /// The actual key
        /// </summary>
        public string Key { get; set; }
        
        /// <summary>
        /// Description (or source) of the api key
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Claims for the endpoints
        /// </summary>
        public Claim[] Claims { get; set; }
    }

    public enum Claim
    {
        entries_new,
        entries_delete,
        entries_list,
        entry_info
    }
}