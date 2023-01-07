﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using App.Metrics;
using general_shortener.Extensions;
using general_shortener.Metrics;
using general_shortener.Models;
using general_shortener.Models.Entry;
using general_shortener.Models.Logging;
using general_shortener.Models.Options;
using general_shortener.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly IMetrics _metrics;
        private readonly IMongoCollection<Entry> _entries;
        private readonly HttpOptions _options;
        private readonly ILogger<EntryController> _logger;

        /// <summary>
        /// Entry controller constructor
        /// </summary>
        /// <param name="directoryService"></param>
        /// <param name="mongoDatabase"></param>
        /// <param name="options"></param>
        /// <param name="metrics"></param>
        /// <param name="logger"></param>
        public EntryController(IDirectoryService directoryService, IMongoDatabase mongoDatabase, IOptions<HttpOptions> options, IMetrics metrics, ILogger<EntryController> logger)
        {
            _directoryService = directoryService;
            _metrics = metrics;
            _logger = logger;
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
                    _logger.LogEntryInfo(new EntryInfoLoggingModel()
                    {
                        Status = HttpStatusCode.NotFound,
                        Id = slug
                    });
                    return NotFound(this.ConstructErrorResponse("Entry with given slug not found"));
                }
                _logger.LogEntryInfo(new EntryInfoLoggingModel()
                {
                    Status = HttpStatusCode.Redirect,
                    Id = slug
                });
                return Redirect(_options.Redirect.NotFound);
            }
                
            
            Entry entry = entries.First();
            EntryType type = entry.Type;

            this._metrics.Measure.Counter.Increment(MetricsRegistry.EntryServed, type.ToString());
            
            if (type == EntryType.url)
            {
                _logger.LogEntryInfo(new EntryInfoLoggingModel()
                {
                    Status = HttpStatusCode.Redirect,
                    Id = slug,
                    Type = type,
                    Value = entry.Value,
                    Size = entry.Meta.Size,
                    Owner = entry.Meta.Owner.ToString()
                });
                return Redirect(entry.Value);
            }
                

            if (type == EntryType.file)
            {
                _logger.LogEntryInfo(new EntryInfoLoggingModel()
                {
                    Status = HttpStatusCode.Redirect,
                    Id = slug,
                    Type = type,
                    Mimetype = entry.Meta.Mime,
                    Size = entry.Meta.Size,
                    Owner = entry.Meta.Owner.ToString()
                });
                _metrics.Measure.Counter.Increment(MetricsRegistry.EntryMimetypeServed, entry.Meta.Mime.ToLower());
                return await this._directoryService.HandleFileStream(entry, Request, Response, requestModel.download ?? false);
            }

            if (type == EntryType.text)
            {
                _logger.LogEntryInfo(new EntryInfoLoggingModel()
                {
                    Status = HttpStatusCode.Redirect,
                    Id = slug,
                    Type = type,
                    Size = entry.Meta.Size,
                    Owner = entry.Meta.Owner.ToString()
                });
                if (!requestModel.download ?? true)
                {
                    return View(!requestModel.raw??true ? "TextView" : "RawTextView", entry);
                }
                return await this._directoryService.HandleFileStream(entry, Request, Response, true);
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