using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using QRCoder;
using System.Drawing;
using System.Text.Json;
using Weather_forecast.Data;
using Weather_forecast.Models;

namespace Weather_forecast.Services
{
    public class SearchHistoryServices
    {
        private readonly DatabaseContext _context;
        public SearchHistoryServices(DatabaseContext context)
        {
            _context = context;
        }
        public History? GetUserHistory(string? uid)
        {
            return _context.SearchHistory.FirstOrDefault(History => History.UserId == uid);
        }
    }
}
