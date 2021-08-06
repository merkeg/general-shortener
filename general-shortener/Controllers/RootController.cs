using general_shortener.Models.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace general_shortener.Controllers
{
    [ApiController]
    [Route("/")]
    public class RootController : Controller
    {
        private readonly HttpOptions _options;

        public RootController(IOptions<HttpOptions> options)
        {
            _options = options.Value;
        }
        
        [HttpGet]
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