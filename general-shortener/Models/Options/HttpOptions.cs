using System.Diagnostics.CodeAnalysis;

namespace general_shortener.Models.Options
{
    /// <summary>
    /// Http options
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class HttpOptions
    {
        /// <summary>
        /// Section
        /// </summary>
        public const string Section = "HTTP";
        
        /// <summary>
        /// Redirect options
        /// </summary>
        public RedirectOptions Redirect { get; set; }
    }

    /// <summary>
    /// Redirect options
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class RedirectOptions
    {
        /// <summary>
        /// Not found redirect
        /// </summary>
        public string NotFound { get; set; }
        
        /// <summary>
        /// Root redirect
        /// </summary>
        public string Root { get; set; }
        
        /// <summary>
        /// Deletion redirect
        /// </summary>
        public string Deletion { get; set; }
    }
}