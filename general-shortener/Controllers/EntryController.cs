﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using general_shortener.Extensions;
using general_shortener.Models;
using general_shortener.Models.Entry;
using general_shortener.Models.Options;
using general_shortener.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace general_shortener.Controllers
{
    /// <summary>
    /// Controller for entries
    /// </summary>
    [ApiController]
    [Route("/")]
    public class EntryController : Controller
    {
        private readonly IDirectoryService _directoryService;
        private readonly IMongoCollection<Entry> _entries;
        private readonly HttpOptions _options;

        /// <summary>
        /// Entry controller constructor
        /// </summary>
        /// <param name="directoryService"></param>
        /// <param name="mongoDatabase"></param>
        /// <param name="options"></param>
        public EntryController(IDirectoryService directoryService, IMongoDatabase mongoDatabase, IOptions<HttpOptions> options)
        {
            _directoryService = directoryService;
            this._entries = mongoDatabase.GetCollection<Entry>(Entry.Collection);
            _options = options.Value;
        }

        /// <summary>
        /// Get an entry from the given slug
        /// </summary>
        /// <remarks>
        /// This endpoint will directly show or download a file or redirect you
        /// </remarks>
        /// <param name="slug">Slug of the entry you want to get</param>
        /// <param name="requestModel"></param>
        [HttpGet("{slug}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status302Found)]
        [ProducesResponseType(typeof(BaseResponse<MessageResponse>), StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public async Task<IActionResult> GetEntry(string slug, [FromQuery] GetEntryRequestModel requestModel)
        {
            List<Entry> entries = this._entries.Find(f => f.Slug == slug).ToList();
            if (entries.Count == 0)
            {
                if (string.IsNullOrEmpty(_options.Redirect.NotFound))
                {
                    return NotFound(this.ConstructErrorResponse("Entry with given slug not found"));
                }
                return Redirect(_options.Redirect.NotFound);
            }
                
            
            Entry entry = entries.First();
            EntryType type = entry.Type;

            if (type == EntryType.url)
                return Redirect(entry.Value);

            if (type == EntryType.file)
            {
                return await this._directoryService.HandleFileStream(entry, Request, Response, requestModel.download ?? false);
                return new EmptyResult();
            }

            if (type == EntryType.text)
            {
                if (!requestModel.download ?? true)
                {
                    return View(!requestModel.raw??true ? "TextView" : "RawTextView", entry);
                }
                else
                {
                    return await this._directoryService.HandleFileStream(entry, Request, Response, true);
                }
            }
                
                

            return Ok();
        }
        

        /// <summary>
        /// Delete an existing entry without authentication
        /// </summary>
        /// <param name="slug">Slug of the entry you want to delete</param>
        /// <param name="deletionCode">Deletion code of the slug you got on the creation of the entry</param>
        [HttpGet("{slug}/{deletionCode}")]
        [ProducesResponseType(typeof(BaseResponse<MessageResponse>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<MessageResponse>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(BaseResponse<MessageResponse>), StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public async Task<IActionResult> DeleteEntryCode(string slug, string deletionCode)
        {
            List<Entry> entries = this._entries.Find(f => f.Slug == slug).ToList();
            if (entries.Count == 0) 
                return BadRequest(this.ConstructErrorResponse("Entry with given slug not found"));
            Entry entry = entries.First();
            
            if(deletionCode != entry.DeletionCode)
                return Unauthorized(this.ConstructErrorResponse("Deletion code wrong"));
        
            await this._entries.DeleteOneAsync(f => f.Slug == entry.Slug);
            this._directoryService.DeleteFile(entry);
            if (string.IsNullOrEmpty(_options.Redirect.Deletion))
            {
                return Ok(this.ConstructSuccessResponse(new MessageResponse()
                {
                    Message = "Entry deleted"
                }));
            }
            return Redirect(_options.Redirect.Deletion);
        }

    }
}