using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using QRCoder;
using System.Drawing;
using System.Text.Json;
using Weather_forecast.Data;
using Weather_forecast.Models;

namespace Weather_forecast.Services
{
    public class SharesServices
    {
        private readonly DatabaseContext _context;
        public SharesServices(DatabaseContext context)
        {
            _context = context;
        }
    }
}
