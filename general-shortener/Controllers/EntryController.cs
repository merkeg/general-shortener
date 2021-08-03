using general_shortener.Models;
using general_shortener.Models.Entry;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace general_shortener.Controllers
{
    /// <summary>
    /// Controller for entries
    /// </summary>
    [ApiController]
    [Route("/")]
    public class EntryController : Controller
    {
        /// <summary>
        /// Get an entry from the given slug
        /// </summary>
        /// <remarks>
        /// This endpoint will directly show or download a file or redirect you
        /// </remarks>
        /// <param name="slug">Slug of the entry you want to get</param>
        [HttpGet("{slug}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status302Found)]
        [ProducesResponseType(typeof(BaseResponse<ErrorResponse>), StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public IActionResult GetEntry(string slug)
        {
            return Ok();
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