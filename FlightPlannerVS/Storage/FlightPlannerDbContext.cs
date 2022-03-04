using System;
using FlightPlannerVS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FlightPlannerVS.Storage
{
    public class FlightPlannerDbContext : DbContext
    {
       public DbSet<Flight> Flights { get; set; }

       public DbSet<Airport> Airports { get; set; }

       protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
       {
           IConfigurationRoot configuration = new ConfigurationBuilder()
               .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
               .AddJsonFile("appsettings.json")
               .Build();
           optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
       }
    }
}
