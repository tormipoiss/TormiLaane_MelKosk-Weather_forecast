using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using QRCoder;
using System.Drawing;
using System.Text.Json;
using Weather_forecast.Data;
using Weather_forecast.Models;

namespace Weather_forecast.Services
{
    public class UserService
    {
        private readonly DatabaseContext _context;
        public UserService(DatabaseContext context)
        {
            _context = context;
        }
        public async Task<ApplicationUser?> GetUserByID(Guid uid)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == uid.ToString());
        }
    }
}
