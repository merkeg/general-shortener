using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using general_shortener.Extensions;
using general_shortener.Models;
using general_shortener.Models.Entry;
using general_shortener.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        /// <summary>
        /// Entry controller constructor
        /// </summary>
        /// <param name="directoryService"></param>
        /// <param name="mongoDatabase"></param>
        public EntryController(IDirectoryService directoryService, IMongoDatabase mongoDatabase)
        {
            _directoryService = directoryService;
            this._entries = mongoDatabase.GetCollection<Entry>(Entry.Collection);
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
        [ProducesResponseType(typeof(BaseResponse<ErrorResponse>), StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public async Task<IActionResult> GetEntry(string slug, [FromQuery] GetEntryRequestModel requestModel)
        {
            List<Entry> entries = this._entries.Find(f => f.Slug == slug).ToList();
            if (entries.Count == 0) 
                return BadRequest(this.ConstructErrorResponse("Entry with given slug not found"));
            
            Entry entry = entries.First();
            EntryType type = entry.Type;

            if (type == EntryType.url)
                return Redirect(entry.Value);

            if (type == EntryType.file)
                await this._directoryService.HandleFileStream(entry, Request, Response, requestModel.download ?? false);

            return new EmptyResult();
        }
        

        /// <summary>
        /// Delete an existing entry without authentication
        /// </summary>
        /// <param name="slug">Slug of the entry you want to delete</param>
        /// <param name="deletionCode">Deletion code of the slug you got on the creation of the entry</param>
        [HttpGet("{slug}/{deletionCode}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<ErrorResponse>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(BaseResponse<ErrorResponse>), StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public BaseResponse<EmptyResponse> DeleteEntryCode(string slug, string deletionCode)
        {
            return null;
        }

    }
}