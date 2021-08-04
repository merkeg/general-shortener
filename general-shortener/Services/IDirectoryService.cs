using Microsoft.AspNetCore.Http;

namespace general_shortener.Services
{
    /// <summary>
    /// Directory service interface
    /// </summary>
    public interface IDirectoryService
    {
        /// <summary>
        /// Save a file to a configured location.
        /// Replace an already existing file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileName"></param>
        public void SaveFile(IFormFile file, string fileName);

        /// <summary>
        /// Try to guess the file mimetype
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string GuessMimetype(string fileName);
    }
}