using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using QRCoder;
using System.Drawing;
using System.Text.Json;
using Weather_forecast.Data;
using Weather_forecast.Models;

namespace Weather_forecast.Services
{
    public class SaveDatabaseService
    {
        private readonly DatabaseContext _context;
        public SaveDatabaseService(DatabaseContext context)
        {
            _context = context;
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
