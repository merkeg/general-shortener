using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using general_shortener.Attributes;
using general_shortener.Extensions;
using general_shortener.Filters;
using general_shortener.Models;
using general_shortener.Models.Authentication;
using general_shortener.Models.Data;
using general_shortener.Models.Entry;
using general_shortener.Services;
using general_shortener.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace general_shortener.Controllers
{
    /// <summary>
    /// Controller for all kinds of data
    /// </summary>
    [ApiController]
    [Route("entries")]
    public class EntryManagementController : Controller
    {
        private readonly ILogger<EntryManagementController> _logger;
        private readonly IDirectoryService _directoryService;

        private readonly IMongoCollection<Entry> _entries;
        private readonly string _baseUrl;
        
        
        /// <summary>
        /// Constructor
        /// </summary>
        public EntryManagementController(IMongoDatabase mongoDatabase, ILogger<EntryManagementController> logger, IConfiguration configuration, IDirectoryService directoryService)
        {
            _logger = logger;
            _directoryService = directoryService;
            this._entries = mongoDatabase.GetCollection<Entry>(Entry.Collection);
            this._baseUrl = configuration.GetValue<string>("BaseUrl");
        }
        
        /// <summary>
        /// Create a new entry
        /// </summary>
        /// <remarks>Send a file named "file" in body to upload a file. Only works if type is "file"</remarks>
        /// <returns></returns>
        [TypedAuthorize(Claim.entries_new)]
        [HttpPost()]
        [ProducesResponseType(typeof(BaseResponse<NewEntryResponseModel>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BaseResponse<MessageResponse>),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Produces("application/json")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> NewEntry([FromForm] NewEntryRequestModel entryRequestModel)
        {

            if (entryRequestModel.type == EntryType.file && entryRequestModel.file == null)
                return BadRequest(this.ConstructErrorResponse("File must be given on type 'file'"));
            
            if(entryRequestModel.type == EntryType.file && !string.IsNullOrEmpty(entryRequestModel.value))
                return BadRequest(this.ConstructErrorResponse("'value' is not allowed on type 'file'"));
            
            if (entryRequestModel.type is EntryType.text or EntryType.url && string.IsNullOrEmpty(entryRequestModel.value))
                return BadRequest(this.ConstructErrorResponse("Value must be specified on types 'text' and 'url'"));
            
            if(entryRequestModel.type == EntryType.url && !entryRequestModel.value.ValidateUri())
                return BadRequest(this.ConstructErrorResponse("Value is not a valid URI (http(s))"));

            string slug = entryRequestModel.slug;
            string deletionCode = StringUtils.CreateSlug(10);
            
            while (slug == null)
            {
                slug = StringUtils.CreateSlug(6);
                List<Entry> entries = this._entries.Find(f => f.Slug == slug).ToList();
                if (entries.Count != 0)
                {
                    slug = null;
                }
                
            }
            
            this._logger.LogDebug($"New entry request, using slug '{slug}'");

            Entry entry = new Entry()
            {
                Slug = slug,
                Type = entryRequestModel.type,
                DeletionCode = deletionCode,
                Value = entryRequestModel.value,
            };

            if (entryRequestModel.type == EntryType.file)
            {
                IFormFile file = entryRequestModel.file;
                entry.Meta.Filename = this._directoryService.SaveFile(file, slug);
                entry.Meta.OriginalFilename = file.FileName;
                entry.Meta.Mime = this._directoryService.GuessMimetype(file.FileName);
                entry.Meta.Size = file.Length;
            }

            await this._entries.ReplaceOneAsync( filter: f => f.Slug == slug, options: new ReplaceOptions() {IsUpsert = true}, replacement: entry);
            string accessUrl = Flurl.Url.Combine(this._baseUrl, entry.Slug);
            string deletionUrl = Flurl.Url.Combine(accessUrl, entry.DeletionCode);
            
            return Created(accessUrl, this.ConstructSuccessResponse(new NewEntryResponseModel()
            {
                Url = accessUrl,
                DeletionUrl = deletionUrl
            }));
            
        }
        
        /// <summary>
        /// Get Entries
        /// </summary>
        [TypedAuthorize(Claim.entries_list)]
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<MessageResponse>),StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse<MessageResponse>),StatusCodes.Status403Forbidden)]
        [Produces("application/json")]
        public BaseResponse<EntryInfoResponseModel[]> GetEntries([FromQuery] EntriesRequestModel requestModel)
        {
            return null;
        }
        
        /// <summary>
        /// Get information about a resource
        /// </summary>
        /// <param name="slug">The entry you want to get information from</param>
        
        [TypedAuthorize(Claim.entry_info)]
        [HttpGet("{slug}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<MessageResponse>),StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse<MessageResponse>),StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(BaseResponse<MessageResponse>), StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public BaseResponse<EntryInfoResponseModel> GetEntryInfo(string slug)
        {
            return null;
        }
        
        /// <summary>
        /// Delete an existing entry
        /// </summary>
        /// <param name="slug">Slug of the entry you want to delete</param>
        [TypedAuthorize(Claim.entries_delete)]
        [HttpDelete("{slug}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<MessageResponse>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse<MessageResponse>),StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(BaseResponse<MessageResponse>), StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public BaseResponse<EmptyResponse> DeleteEntry(string slug)
        {
            return null;
        }
    }
}