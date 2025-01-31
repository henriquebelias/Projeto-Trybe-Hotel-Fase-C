using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TrybeHotel.Dto;
using TrybeHotel.Models;
using TrybeHotel.Repository;

namespace TrybeHotel.Controllers
{
    [ApiController]
    [Route("booking")]
    public class BookingController : Controller
    {
        private readonly IBookingRepository _repository;

        public BookingController(IBookingRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = "Client")]
        public IActionResult Add([FromBody] BookingDtoInsert bookingInsert)
        {
            try
            {
                var token = HttpContext.User.Identity as ClaimsIdentity;
                string? email = token
                    ?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)
                    ?.Value;

                return Created("", _repository.Add(bookingInsert, email!));
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet("{Bookingid}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = "Client")]
        public IActionResult GetBooking(int Bookingid)
        {
            try
            {
                var token = HttpContext.User.Identity as ClaimsIdentity;
                string? email = token
                    ?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)
                    ?.Value;

                var booking = _repository.GetBooking(Bookingid, email!);

                return Ok(booking);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }
    }
}
