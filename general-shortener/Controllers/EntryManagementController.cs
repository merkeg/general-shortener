﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using general_shortener.Attributes;
using general_shortener.Extensions;
using general_shortener.Models;
using general_shortener.Models.Authentication;
using general_shortener.Models.Data;
using general_shortener.Models.Entry;
using general_shortener.Services;
using general_shortener.Utils;
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
                List<Entry> entries = (await this._entries.FindAsync(f => f.Slug == slug)).ToList();
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
            
            var entriesInDb = (await (await this._entries.FindAsync(f => f.Slug == slug)).ToListAsync());

            if (entriesInDb.Count != 0)
            {
                var oldEntry = entriesInDb.First();
                if (oldEntry.Type == EntryType.file)
                {
                    this._directoryService.DeleteFile(oldEntry);
                }
                entry.Id = oldEntry.Id;
            }

            if (entryRequestModel.type == EntryType.file)
            {
                IFormFile file = entryRequestModel.file;
                entry.Meta.Filename = this._directoryService.SaveFile(file, slug);
                entry.Meta.OriginalFilename = file.FileName;
                entry.Meta.Mime = this._directoryService.GuessMimetype(file.FileName);
                entry.Meta.Size = file.Length;
            }

            if (entryRequestModel.type == EntryType.text)
            {
                entry.Meta.Mime = "text/plain";
                entry.Meta.Size = entryRequestModel.value!.Length;
                entry.Meta.OriginalFilename = slug + ".txt";
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
        [ProducesResponseType(typeof(BaseResponse<EntryInfoResponseModel[]>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<MessageResponse>),StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse<MessageResponse>),StatusCodes.Status403Forbidden)]
        [Produces("application/json")]
        public async Task<IActionResult> GetEntries([FromQuery] EntriesRequestModel requestModel)
        {
            List<Entry> entries = await (await this._entries.FindAsync(_ => true, new FindOptions<Entry>()
            {
                Limit = requestModel.limit,
                Skip = requestModel.offset
            })).ToListAsync();

            List<EntryInfoResponseModel> entriesResponse = new List<EntryInfoResponseModel>();

            foreach (Entry entry in entries)
            {
                entriesResponse.Add(new EntryInfoResponseModel()
                {
                    Slug = entry.Slug,
                    Type = entry.Type,
                    DeletionCode = entry.DeletionCode,
                    Mime = entry.Meta.Mime,
                    Size = entry.Meta.Size,
                    Value = entry.Value
                });
            }
            
            return Ok(this.ConstructSuccessResponse(entriesResponse.ToArray()));
        }

        /// <summary>
        /// Get information about a resource
        /// </summary>
        /// <param name="slug">The entry you want to get information from</param>
        
        [TypedAuthorize(Claim.entry_info)]
        [HttpGet("{slug}")]
        [ProducesResponseType(typeof(BaseResponse<EntryInfoResponseModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<MessageResponse>),StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse<MessageResponse>),StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(BaseResponse<MessageResponse>), StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public async Task<IActionResult> GetEntryInfo(string slug)
        {
            if (string.IsNullOrEmpty(slug))
                return BadRequest(this.ConstructErrorResponse("Invalid slug"));

            List<Entry> entries = (await (await this._entries.FindAsync(f => f.Slug == slug)).ToListAsync());
            
            if(entries.Count == 0)
                return NotFound(this.ConstructErrorResponse("Entry with given slug not found"));

            Entry entry = entries.First();
            
            return Ok(this.ConstructSuccessResponse(new EntryInfoResponseModel()
            {
                Slug = entry.Slug,
                Type = entry.Type,
                DeletionCode = entry.DeletionCode,
                Mime = entry.Meta.Mime,
                Size = entry.Meta.Size,
                Value = entry.Value
            }));
        }
        
        /// <summary>
        /// Delete an existing entry
        /// </summary>
        /// <param name="slug">Slug of the entry you want to delete</param>
        [TypedAuthorize(Claim.entries_delete)]
        [HttpDelete("{slug}")]
        [ProducesResponseType(typeof(BaseResponse<MessageResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<MessageResponse>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse<MessageResponse>),StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(BaseResponse<MessageResponse>), StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public async Task<IActionResult> DeleteEntry(string slug)
        {
            if (string.IsNullOrEmpty(slug))
                return BadRequest(this.ConstructErrorResponse("Invalid slug"));

            List<Entry> entries = await (await this._entries.FindAsync(f => f.Slug == slug)).ToListAsync();
            
            if(entries.Count == 0)
                return NotFound(this.ConstructErrorResponse("Entry with given slug not found"));

            Entry entry = entries.First();

            if(entry.Type == EntryType.file)
                this._directoryService.DeleteFile(entry);
            
            await this._entries.DeleteOneAsync(f => f.Slug == entry.Slug);
            
            return Ok(this.ConstructSuccessResponse(new MessageResponse()
            {
                Message = $"Entry with slug '{entry.Slug}' successfully deleted"
            }));
        }
    }
}