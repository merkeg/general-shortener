using general_shortener.Models.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace general_shortener.Controllers
{
    /// <summary>
    /// Webroot controller
    /// </summary>
    [ApiController]
    [Route("/")]
    public class RootController : Controller
    {
        private readonly HttpOptions _options;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        public RootController(IOptions<HttpOptions> options)
        {
            _options = options.Value;
        }
        
        /// <summary>
        /// Gets the Webroot page or redirects to a user specified location
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status302Found)]
        public IActionResult GetIndex()
        {
            if (string.IsNullOrEmpty(_options.Redirect.Root))
            {
                return Ok("No redirect on Webroot given");
            }
            else
            {
                return Redirect(_options.Redirect.Root);
            }
        }
    }
}