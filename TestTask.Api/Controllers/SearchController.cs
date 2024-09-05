using Microsoft.AspNetCore.Mvc;
using TestTask.Application.Common.Interfaces;
using TestTask.Application.Models.RequestModels;

namespace TestTask.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }


        [HttpGet]
        public IActionResult Index() => Ok("Search web service");


        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] SearchRequest request, CancellationToken cancellationToken)
        {
            if (request == null || string.IsNullOrEmpty(request.Origin) || string.IsNullOrEmpty(request.Destination))
            {
                return BadRequest("Origin, Destination, and Date are required.");
            }

            var response = await _searchService.SearchAsync(request, cancellationToken);
            return Ok(response);
        }


        [HttpGet("availability")]
        public async Task<IActionResult> CheckAvailability(CancellationToken cancellationToken)
        {
            var isAvailable = await _searchService.IsAvailableAsync(cancellationToken);
            if (isAvailable)
            {
                return Ok("Service is available.");
            }
            return StatusCode(503, "Service is unavailable.");
        }
    }
}
