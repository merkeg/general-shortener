using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using general_shortener.Attributes;
using general_shortener.Models;
using general_shortener.Models.Authentication;
using general_shortener.Models.Data;
using general_shortener.Models.Entry;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace general_shortener.Controllers
{
    /// <summary>
    /// Controller for all kinds of data
    /// </summary>
    [ApiController]
    [Route("entries")]
    public class EntryManagementController : Controller
    {
        /// <summary>
        /// Create a new entry
        /// </summary>
        /// <remarks>Send a file named "file" in body to upload a file. Only works if type is "file"</remarks>
        /// <returns></returns>
        [TypedAuthorize(Claim.entries_new)]
        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BaseResponse<ErrorResponse>),StatusCodes.Status401Unauthorized)]
        [Produces("application/json")]
        public BaseResponse<NewEntryResponseModel> NewEntry(NewEntryRequestModel entryRequestModel)
        {
            return null;
        }
        
        /// <summary>
        /// Get Entries
        /// </summary>
        [TypedAuthorize(Claim.entries_list)]
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<ErrorResponse>),StatusCodes.Status401Unauthorized)]
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
        [ProducesResponseType(typeof(BaseResponse<ErrorResponse>),StatusCodes.Status401Unauthorized)]
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
        [ProducesResponseType(typeof(BaseResponse<ErrorResponse>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse<ErrorResponse>), StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public BaseResponse<EmptyResponse> DeleteEntry(string slug)
        {
            return null;
        }
    }
}