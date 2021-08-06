// ReSharper disable InconsistentNaming
#pragma warning disable 1591
namespace general_shortener.Models.Authentication
{
    public class ApiKeyModel
    {
        /// <summary>
        /// Id of the Api key in the document.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// The actual key
        /// </summary>
        public string Token { get; set; }
        
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