using System.Collections.Generic;
using FlightPlannerVS.Models;
using FlightPlannerVS.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PageResult = FlightPlannerVS.Models.PageResult;

namespace FlightPlannerVS.Controllers
{
    [Route("api")]
    [ApiController]
    public class CustomerApiController : ControllerBase
    {
        [HttpGet]
        [Route("Airports")]
        public IActionResult SearchAirports(string search)
        {
            var airports = FlightStorage.FindAirports(search);

            return Ok(airports);
        }

        [HttpPost]
        [Route("flights/search")]
        public IActionResult SearchFlight(SearchFlightRequest search)
        {
            if (search.From == search.To)
            {
                return BadRequest();
            }

            return Ok(FlightStorage.SearchFlights(search));
        }

        [HttpGet]
        [Route("flights/{id}")]
        public IActionResult SearchFlights(int id)
        {
            var flight = FlightStorage.GetFlight(id);

            if (flight == null) return NotFound();

            return Ok(flight);
        }
    }
}
