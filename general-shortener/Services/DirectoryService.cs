using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using general_shortener.Models.Entry;
using general_shortener.Models.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;

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
            file.CopyTo(new FileStream(filePath, FileMode.Create));
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
        public async Task HandleFileStream(Entry entry, HttpRequest request, HttpResponse response, bool forceDownload = false)
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
                response.Headers.Add("Content-disposition", $"attachment; filename={entry.Meta.OriginalFilename}");

            string range = request.Headers["range"];
            if (string.IsNullOrEmpty(range))
            {
                range = "bytes=0-";
            }

            string[] positions = range.Replace("bytes=", "").Split("-", StringSplitOptions.RemoveEmptyEntries);

            long start = long.Parse(positions[0]);
            long total = entry.Meta.Size;
            long end = positions.Length > 1 ? long.Parse(positions[1]) : total > 1024 ? 1024 : total - 1;
            long chunksize = end - start + 1;
            
            response.Headers.Add("Content-Range", $"bytes {start}-{end}/{total}");
            await response.SendFileAsync(Path.Combine(this._options.Path, entry.Meta.Filename));
        }

        /// <inheritdoc />
        public void DeleteFile(string fileName)
        {
            try
            {
                File.Delete(Path.Combine(this._options.Path, fileName));
            }
            catch (Exception e)
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