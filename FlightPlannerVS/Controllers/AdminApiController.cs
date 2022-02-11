using FlightPlannerVS.Models;
using FlightPlannerVS.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightPlannerVS.Controllers
{
    [Route("admin-api")]
    [ApiController]
    [Authorize]

    public class AdminApiController : ControllerBase
    {
        private static readonly object _locker = new();

        [HttpGet]
        [Route("Flights/{id}")]
        [Authorize]
        public IActionResult GetFlights(int id)
        {
            var flight = FlightStorage.GetFlight(id);

            if (flight == null) return NotFound();
            return Ok();
        }

        [HttpDelete]
        [Route("Flights/{id}")]
        [Authorize]
        public IActionResult DeleteFlights(int id)
        {
            FlightStorage.DeleteFlight(id);

            return Ok();
        } 

        [HttpPut, Authorize]
        [Route("flights")]
        public IActionResult PutFlights(AddFlightRequest request)
        {
            lock (_locker)
            {
                if (!FlightStorage.IsValid(request)) return BadRequest();

                if (FlightStorage.Exists(request)) return Conflict();

                var flight = FlightStorage.AddFlight(request);

                return Created("", flight);
            }
        }
    }
}
