using Graduation_Project.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Graduation_Project.BLL.Services.Implementations;
namespace Graduation_Project.Api.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        // GET: api/search?query=shirt
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            var result = await _searchService.SearchAsync(query);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            return Ok(result.Data);
        }
    }
}