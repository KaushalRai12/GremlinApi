// Controllers/PlaceController.cs
using GremlinApi;
using GremlinApi.Models;
using GremlinAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace GremlinAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlaceController : ControllerBase
    {
        private readonly IGraphService _graphService;

        public PlaceController(IGraphService graphService)
        {
            _graphService = graphService;
        }

        [HttpPost]
        public async Task<IActionResult> AddPlace([FromBody] Place place)
        {
            await _graphService.AddPlaceAsync(place);
            return Ok();
        }
    }
}
