using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using FlightPlannerVS.Models;

namespace FlightPlannerVS.Storage
{
    public static class FlightStorage
    {
        private static List<Flight> _flights = new List<Flight>();
        private static int _id = 0;
        public static readonly object _flightLock = new object();

        public static Flight AddFlight(AddFlightRequest request)
        {
            lock (_flightLock)
            {
                var flight = new Flight
                {
                    From = request.From,
                    To = request.To,
                    ArrivalTime = request.ArrivalTime,
                    DepartureTime = request.DepartureTime,
                    Carrier = request.Carrier,
                    Id = ++_id
                };

                _flights.Add(flight);

                return flight;
            }
        }

        public static Flight ConvertToFlight(AddFlightRequest request)
        {
            var flight = new Flight
            {
                From = request.From,
                To = request.To,
                ArrivalTime = request.ArrivalTime,
                DepartureTime = request.DepartureTime,
                Carrier = request.Carrier
            };

            return flight;
        }

        public static Flight GetFlight(int id)
        {
            lock (_flightLock)
            {
                return _flights.SingleOrDefault(flight => flight.Id == id);
            }
        }

        public static void DeleteFlight(int id)
        {
            lock (_flightLock)
            {
                var flight = GetFlight(id);

                if (flight != null)
                    _flights.Remove(flight);
            }
        }

        public static List<Airport> FindAirports(string userInput)
        {
            userInput = userInput.ToLower().Trim() ;

            var fromAirport = _flights.Where(f => f.From.AirportName.ToLower().Trim().Contains(userInput)
                                                  || f.From.Country.ToLower().Trim().Contains(userInput)
                                                  || f.From.City.ToLower().Trim().Contains(userInput)).Select(a => a.From).ToList();

            var ToAirport = _flights.Where(f => f.To.AirportName.ToLower().Trim().Contains(userInput)
                                                  || f.To.Country.ToLower().Trim().Contains(userInput)
                                                  || f.To.City.ToLower().Trim().Contains(userInput)).Select(a => a.From).ToList();

            return fromAirport.Concat(ToAirport).ToList();
        }

        public static void ClearFlights()
        {
            _flights.Clear();
            _id = 0;
        }

        public static bool Exists(AddFlightRequest request)
        {
            lock (_flightLock)
            {
                return _flights.Any(f => f.Carrier.ToLower().Trim() == request.Carrier.ToLower().Trim()
                                         && f.DepartureTime == request.DepartureTime &&
                                         f.ArrivalTime == request.ArrivalTime
                                         && f.From.AirportName.ToLower().Trim() ==
                                         request.From.AirportName.ToLower().Trim()
                                         && f.To.AirportName.ToLower().Trim() ==
                                         request.To.AirportName.ToLower().Trim());
            }
        }

        public static bool IsValid(AddFlightRequest request)
        {
            if (request == null) return false;

            if (string.IsNullOrEmpty(request.ArrivalTime) 
                || string.IsNullOrEmpty(request.Carrier) 
                || string.IsNullOrEmpty(request.DepartureTime))
            {
                return false;
            }

            if (request.From == null || request.To == null) return false;

            if (string.IsNullOrEmpty(request.From.AirportName) || string.IsNullOrEmpty(request.From.City) || string.IsNullOrEmpty(request.From.Country))
            {
                return false;
            }

            if (string.IsNullOrEmpty(request.To.AirportName) || string.IsNullOrEmpty(request.To.City) || string.IsNullOrEmpty(request.To.Country))
            {
                return false;
            }

            if (request.From.Country.ToLower().Trim() == request.To.Country.ToLower().Trim() 
                && request.From.City.ToLower().Trim() == request.To.City.ToLower().Trim() &&
                request.From.AirportName.ToLower().Trim() == request.To.AirportName.ToLower().Trim())
            {
                return false;
            }

            var arrivalTime = DateTime.Parse(request.ArrivalTime);
            var departureTime = DateTime.Parse(request.DepartureTime);

            return arrivalTime > departureTime;
        }

        public static PageResult SearchFlights(SearchFlightRequest request)
        {
            var foundFlights = _flights.Where(flight =>
                flight.From.AirportName.ToLower().Trim() == request.From.ToLower().Trim() &&
                flight.To.AirportName.ToLower().Trim() == request.To.ToLower().Trim() &&
                DateTime.Parse(flight.DepartureTime).Date == DateTime.Parse(request.DepartureDate)).ToList();

            return new PageResult(foundFlights);
        }
    }
}
