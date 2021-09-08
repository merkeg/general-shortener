using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using App.Metrics;
using general_shortener.Metrics;
using general_shortener.Models.Entry;
using general_shortener.Models.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace general_shortener.Services
{
    /// <inheritdoc />
    public class DirectoryService: IDirectoryService
    {

        /// <summary>
        /// Streamable data not to download
        /// </summary>
        public static readonly string[] StreamableMedia = {"image/*", "video/*", "application/pdf"};
        
        private readonly StorageOptions _options;
        private readonly ILogger<DirectoryService> _logger;
        private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;

        /// <summary>
        /// Directory service class
        /// </summary>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        /// <param name="metrics"></param>
        public DirectoryService(IOptions<StorageOptions> options, ILogger<DirectoryService> logger)
        {
            _options = options.Value;
            _logger = logger;
            _fileExtensionContentTypeProvider = new FileExtensionContentTypeProvider();
            logger.LogInformation($"File storage location is at {_options.Path}");
            
        }
        
        /// <inheritdoc />
        public string SaveFile(IFormFile file, string fileName)
        {
            string extension = Path.GetExtension(file.FileName);
            fileName = fileName + extension;
            string filePath = Path.Combine(this._options.Path, fileName);
            FileStream stream = new FileStream(filePath, FileMode.Create);
            file.CopyTo(stream);
            stream.Close();
            return fileName;
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

        /// <inheritdoc />
        public async Task<IActionResult> HandleFileStream(Entry entry, HttpRequest request, HttpResponse response, bool forceDownload = false)
        {
            bool download = true;
            
            response.Clear();
            response.ContentType = entry.Meta.Mime;
            response.ContentLength = entry.Meta.Size;
            response.Headers.Add("Accept-Ranges", "bytes");

            if (!forceDownload)
            {
                foreach (string streamableMedium in StreamableMedia)
                {
                    if (Regex.IsMatch(response.ContentType, streamableMedium))
                    {
                        download = false;
                        break;
                    }
                }
            }
            if(download)
                response.Headers.Add("Content-disposition", $"attachment; filename={entry.Meta.OriginalFilename??entry.Meta.Filename}");

            Stream stream = null;

            switch (entry.Type)
            {
                case EntryType.url:
                    break;
                case EntryType.file:
                    string filePath = Path.Combine(this._options.Path, entry.Meta.Filename);
                    if (File.Exists(filePath))
                    {
                        stream = new MemoryStream(await File.ReadAllBytesAsync(filePath));
                    }
                    else
                    {
                        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                    }
                    
                    break;
            }
            return new FileStreamResult(stream, entry.Meta.Mime);
        }

        /// <inheritdoc />
        public void DeleteFile(string fileName)
        {
            try
            {
                File.Delete(Path.Combine(this._options.Path, fileName));
            }
            catch (Exception)
            {
                _logger.LogError("Error deleting file: " + fileName);
            }
            
        }

        /// <inheritdoc />
        public void DeleteFile(Entry entry)
        {
            DeleteFile(entry.Meta.Filename);
        }
        
    }
}