namespace general_shortener.Models.Entry
{
    /// <summary>
    /// Get entry request model
    /// </summary>
    public class GetEntryRequestModel
    {
        
        /// <summary>
        /// Download the file directly
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public bool? download { get; set; }
    }
}