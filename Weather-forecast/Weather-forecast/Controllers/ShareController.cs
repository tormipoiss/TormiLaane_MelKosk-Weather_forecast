using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Text.Json;
using Weather_forecast.Data;
using Weather_forecast.Models;
using Weather_forecast.Services;
using Weather_forecast.ViewModels;
using static QRCoder.PayloadGenerator;

namespace Weather_forecast.Controllers
{
    public class ShareController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly WeatherAPIHandler _weatherAPIHandler;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly QrCodeService _qrCodeService;

        public ShareController(ILogger<HomeController> logger, WeatherAPIHandler weatherAPIHandler, UserManager<ApplicationUser> userManager, DatabaseContext context, QrCodeService qrCodeService)
        {
            _logger = logger;
            _weatherAPIHandler = weatherAPIHandler;
            _userManager = userManager;
            _context = context;
            _qrCodeService = qrCodeService;

        }
        [HttpGet("Home/DeleteSharedLink")] // Should be post but im too lazy to add a form to just delete a shared link
        [Authorize]
        public async Task<IActionResult> DeleteSharedLink(Guid shareToken)
        {
            if (!Request.Headers.TryGetValue("Referer", out var referer) || referer.Count != 1)
            {
                return RedirectToAction("Statistics");
            }
            if (!referer.First().EndsWith("/Home/Statistics"))
            {
                return RedirectToAction("Statistics");
            }
            var uid = _userManager.GetUserId(User);
            if (uid == null)
            {
                return View("~/Views/Home/Index.cshtml");
            }
            var share = await _context.Shares.FirstOrDefaultAsync(x => x.ShareToken == shareToken.ToString());
            if (share == null)
            {
                return RedirectToAction("Statistics");
            }
            if (share.UserId != uid)
            {
                return View("~/Views/Home/Index.cshtml");
            }
            _context.Shares.Remove(share);
            await _context.SaveChangesAsync();
            return RedirectToAction("Statistics");
        }


        [HttpGet("Home/Shared")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSharedForecastView(string city, Guid shareToken, Guid uid, bool metric, string? foreCastDate, bool? displayMultipleDays, int? DayAmount)
        {
            if (city == null)
            {
                return View("~/Views/Home/Index.cshtml");
            }
            var exists = await _context.Shares.FirstOrDefaultAsync(x => x.ShareToken == shareToken.ToString());
            if (exists == null)
            {
                return View("~/Views/Home/Index.cshtml");
            }
            if (exists.City != city)
            {
                return View("~/Views/Home/Index.cshtml");
            }
            var usr = await _context.Users.FirstOrDefaultAsync(x => x.Id == uid.ToString());
            if (usr == null)
            {
                return View("~/Views/Home/Index.cshtml");
            }
            if (User.Identity != null && !User.Identity.IsAuthenticated)
            {
                exists.ViewCount++;
            }
            await _context.SaveChangesAsync();
            var result = await _weatherAPIHandler.FetchDataAsync(city, metric);
            ViewBag.SharedUrl = true;
            if (result == null)
            {
                ViewBag.error = true;
                return View("~/Views/Home/Index.cshtml");
            }
            if (metric)
            {
                result.Units = new()
                {
                    KmOrMile = "km",
                    COrF = "C",
                    MmOrInches = "mm"
                };
            }
            else
            {
                result.Units = new()
                {
                    KmOrMile = "miles",
                    COrF = "F",
                    MmOrInches = "inches"
                };
            }
            if (foreCastDate == null)
            {
                result.ForecastDate = DateTime.Now;
            }
            else
            {
                result.ForecastDate = DateTime.Parse(foreCastDate);
            }
            if (displayMultipleDays == true)
            {
                result.DisplayMultipleDays = true;
                result.DayAmount = DayAmount;
            }
            else
            {
                result.DisplayMultipleDays = false;
            }
            result.QrCodeBase64 = _qrCodeService.CreateSharedForecastQRCodeAsB64(new ShareDto() { City = city, Metric = metric, UID = uid, ShareToken = shareToken, Date = result.ForecastDate });
            return View("~/Views/Home/Index.cshtml", result);
        }
        [HttpPost("Home/ShareLink")]
        [Authorize]
        public async Task<IActionResult> CreateSharedForecast(string city, Guid shareToken, Guid uid, bool metric, string? foreCastDate, bool? displayMultipleDays, int? DayAmount)
        {
            var exists = await _context.Shares.FirstOrDefaultAsync(x => x.ShareToken == shareToken.ToString());
            if (exists != null)
            {
                return BadRequest();
            }
            var userExistsId = _userManager.GetUserId(User);
            if (userExistsId == null)
            {
                ViewBag.error = true;
                return View("~/Views/Home/Index.cshtml");
            }
            if (userExistsId != uid.ToString())
            {
                ViewBag.error = true;
                return View("~/Views/Home/Index.cshtml");
            }
            DateTime? forecastDateFixed = new DateTime();
            if (foreCastDate == null)
            {
                forecastDateFixed = null;
            }
            else
            {
                forecastDateFixed = DateTime.Parse(foreCastDate);
            }
            if (displayMultipleDays == true)
            {
                await _context.Shares.AddAsync(new() { City = city, ShareToken = shareToken.ToString(), UserId = uid.ToString(), Metric = metric, forecastDate = forecastDateFixed, isMultipleDayForecast = true, DayAmount = DayAmount });
            }
            else
            {
                await _context.Shares.AddAsync(new() { City = city, ShareToken = shareToken.ToString(), UserId = uid.ToString(), Metric = metric, forecastDate = forecastDateFixed });
            }
            await _context.SaveChangesAsync();
            //var existingUserByName = await _userManager.FindByNameAsync(User.Identity.Name);
            return Ok();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
