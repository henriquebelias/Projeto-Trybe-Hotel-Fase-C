using Microsoft.AspNetCore.Mvc;
using TrybeHotel.Dto;
using TrybeHotel.Models;
using TrybeHotel.Repository;
using TrybeHotel.Services;

namespace TrybeHotel.Controllers
{
    [ApiController]
    [Route("geo")]
    public class GeoController : Controller
    {
        private readonly IHotelRepository _repository;
        private readonly IGeoService _geoService;

        public GeoController(IHotelRepository repository, IGeoService geoService)
        {
            _repository = repository;
            _geoService = geoService;
        }

        // 11. Desenvolva o endpoint GET /geo/status
        [HttpGet]
        [Route("status")]
        public async Task<IActionResult> GetStatus()
        {
            var response = await _geoService.GetGeoStatus();
            return Ok(response);
        }

        // 12. Desenvolva o endpoint GET /geo/address
        [HttpGet]
        [Route("address")]
        public async Task<IActionResult> GetHotelsByLocation([FromBody] GeoDto address)
        {
            var response = await _geoService.GetHotelsByGeo(address, _repository);

            return Ok(response);
        }
    }
}
