using System.IO;
using general_shortener.Models.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace general_shortener.Services
{
    /// <inheritdoc />
    public class DirectoryService: IDirectoryService
    {
        private readonly StorageOptions _options;
        private readonly ILogger<DirectoryService> _logger;
        private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;
        
        /// <summary>
        /// Directory service class
        /// </summary>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        public DirectoryService(IOptions<StorageOptions> options, ILogger<DirectoryService> logger)
        {
            _options = options.Value;
            _logger = logger;
            _fileExtensionContentTypeProvider = new FileExtensionContentTypeProvider();
            logger.LogInformation($"File storage location is at {_options.Path}");
            
        }
        
        /// <inheritdoc />
        public void SaveFile(IFormFile file, string fileName)
        {
            string extension = Path.GetExtension(file.FileName);
            fileName = fileName + extension;
            string filePath = Path.Combine(this._options.Path, fileName);
            file.CopyTo(new FileStream(filePath, FileMode.Create));
        }

        /// <inheritdoc />
        public string GuessMimetype(string fileName)
        {
            string contentType;
            if(!this._fileExtensionContentTypeProvider.TryGetContentType(fileName, out contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }
    }
}