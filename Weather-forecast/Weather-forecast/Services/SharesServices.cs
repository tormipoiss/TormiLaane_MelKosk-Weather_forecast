using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using QRCoder;
using System.Drawing;
using System.Text.Json;
using System.Threading.Tasks;
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
        public List<CityShare>? GetUserShares(string? uid)
        {
            return _context.Shares.Where(x => x.UserId == uid).ToList();
        }
        public async Task<CityShare?> GetShareByShareToken(Guid shareToken)
        {
            return await _context.Shares.FirstOrDefaultAsync(x => x.ShareToken == shareToken.ToString());
        }
        public void Remove(CityShare? share)
        {
            _context.Shares.Remove(share);
            _context.SaveChanges();
        }
        public async Task AddAsync(CityShare share)
        {
            await _context.Shares.AddAsync(share);
            await _context.SaveChangesAsync();
        }
        public async Task<CityShare?> GetAlreadySharedShare(string? uid, string cityName)
        {
            return await _context.Shares.Where(u => u.UserId == uid).FirstOrDefaultAsync(x => x.City == cityName);
        }
    }
}
