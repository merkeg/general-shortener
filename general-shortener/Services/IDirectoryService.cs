using System.Threading.Tasks;
using general_shortener.Models.Entry;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public string SaveFile(IFormFile file, string fileName);

        /// <summary>
        /// Try to guess the file mimetype
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string GuessMimetype(string fileName);

        /// <summary>
        /// Handle a file stream
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="forceDownload">Force media download</param>
        public Task<IActionResult> HandleFileStream(Entry entry, HttpRequest request, HttpResponse response, bool forceDownload = false);

        /// <summary>
        /// Deletes a file
        /// </summary>
        /// <param name="fileName"></param>
        public void DeleteFile(string fileName);

        /// <summary>
        /// Deletes a file
        /// </summary>
        /// <param name="entry"></param>
        public void DeleteFile(Entry entry);

    }
}