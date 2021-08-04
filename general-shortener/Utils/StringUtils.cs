using System;
using System.Text;

namespace general_shortener.Utils
{
    /// <summary>
    /// String utilities
    /// </summary>
    public static class StringUtils
    {

        /// <summary>
        /// Validate an string as an uri
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static bool ValidateUri(this string uri)
        {
            Uri uriResult;
            bool result = Uri.TryCreate(uri, UriKind.Absolute, out uriResult) 
                          && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            return result;
        }

        /// <summary>
        /// Generates a new slug with specific length
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string CreateSlug(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }

    }
}