using System;
using System.Collections.Generic;
using System.Linq;
using FlightPlannerVS.Models;
using FlightPlannerVS.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightPlannerVS.Controllers
{
    [Route("api")]
    [ApiController]
    public class CustomerApiController : ControllerBase
    {
        private readonly FlightPlannerDbContext _context;
        
        public CustomerApiController(FlightPlannerDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("Airports")]
        public IActionResult SearchAirports(string search)
        {
            lock (FlightStorage._flightLock)
            {
                var airport = new List<Airport>();
                airport.AddRange(_context.Flights.Select(f => f.From));
                airport.AddRange(_context.Flights.Select(f => f.To));

                var searchResultAirports = airport.Where(f => f.AirportName.ToUpper().Contains(search.Trim().ToUpper())
                                                              || f.Country.ToUpper().Contains(search.Trim().ToUpper())
                                                              || f.City.ToUpper().Contains(search.Trim().ToUpper()));

                if (searchResultAirports.Any()) return Ok(searchResultAirports);

                return NotFound();
            }
        }

        [HttpPost]
        [Route("flights/search")]
        public IActionResult SearchFlight(SearchFlightRequest search)
        {
            if (!IsValidRequest(search)) return BadRequest();

            lock (FlightStorage._flightLock)
            {
                var flightSearchResult = _context.Flights
                    .Include(f => f.To)
                    .Include(f => f.From)
                    .ToList()
                    .Where(l => l.To.AirportName == search.To
                                && l.From.AirportName == search.From
                                && DateTime.Parse(l.DepartureTime).Date == DateTime.Parse(search.DepartureDate))
                    .ToList();

                var pageResult = new PageResult<Flight>
                {
                    Page = 0,
                    TotalItems = flightSearchResult.Count,
                    Items = flightSearchResult
                };

                return Ok(pageResult);
            }
        }

        public static bool IsValidRequest(SearchFlightRequest request)
        {
            if (request.From == null || request.To == null || request.DepartureDate == null) return false;

            if (request.From == request.To) return false;

            return true;
        }

        [HttpGet]
        [Route("flights/{id}")]
        public IActionResult SearchFlights(int id)
        {
            lock (FlightStorage._flightLock)
            {
                var flight = _context.Flights
                    .Include(f => f.From)
                    .Include(f => f.To)
                    .SingleOrDefault(f => f.Id == id);

                if (flight != null) return Ok(flight);

                return NotFound();
            }
        }
    }

    internal class PageResult<T>
    {
        public int Page { get; set; }
        public int TotalItems { get; set; }
        public List<Flight> Items { get; set; }
    }
}
