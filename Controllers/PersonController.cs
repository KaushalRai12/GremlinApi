// Controllers/PersonController.cs
using Microsoft.AspNetCore.Mvc;
using GremlinAPI.Services;
using GremlinApi.Models;

namespace GremlinAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly IGraphService _graphService;

        public PersonController(IGraphService graphService)
        {
            _graphService = graphService;
        }

        [HttpPost]
        public async Task<IActionResult> AddPerson([FromBody] Person person)
        {
            await _graphService.AddPersonAsync(person);
            return Ok();
        }

        [HttpPost("{personId}/livesAt/{placeId}")]
        public async Task<IActionResult> ConnectPersonToPlace(string personId, string placeId)
        {
            await _graphService.ConnectPersonToPlaceAsync(personId, placeId);
            return Ok();
        }
    }
}