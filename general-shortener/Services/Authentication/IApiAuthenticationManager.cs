using general_shortener.Models.Authentication;

namespace general_shortener.Services.Authentication
{
    /// <summary>
    /// Base interface for Api authentication
    /// </summary>
    public interface IApiAuthenticationManager
    {
        /// <summary>
        /// Verifies an api key
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Key information or null if key is not valid</returns>
        public ApiKeyModel VerifyKey(string key);
    }
}