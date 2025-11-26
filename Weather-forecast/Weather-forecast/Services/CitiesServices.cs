using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using QRCoder;
using System.Drawing;
using System.Text.Json;
using Weather_forecast.Data;
using Weather_forecast.Models;

namespace Weather_forecast.Services
{
    public class CitiesServices
    {
        private readonly DatabaseContext _context;
        public CitiesServices(DatabaseContext context)
        {
            _context = context;
        }
        public List<City> GetCitiesByUserID(string? uid)
        {
            return  _context.Cities.Where(City => City.HistoryUserId == uid).ToList();
        }
    }
}
