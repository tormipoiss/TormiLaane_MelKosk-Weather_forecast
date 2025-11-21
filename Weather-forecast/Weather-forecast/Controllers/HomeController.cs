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
    public class HomeController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, DatabaseContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            var uid = _userManager.GetUserId(User);
            History? testHistory = _context.SearchHistory.FirstOrDefault(History => History.UserId == uid);
            if (testHistory != default)
            {
                ViewData["showHistory"] = "true";
            }
            return View();
        }

        [HttpGet("Home/History")]
        [Authorize]
        public async Task<IActionResult> History()
        {
            var uid = _userManager.GetUserId(User);
            CityAndApi historyCities = new CityAndApi();
            historyCities.Cities = _context.Cities.Where(City => City.HistoryUserId == uid).ToList();
            return View(historyCities);
        }
        [HttpGet("Home/Statistics")]
        [Authorize]
        public IActionResult ShareLinkStatistics()
        {
            var uid = _userManager.GetUserId(User);
            if(uid == null)
            {
                return View("~/Views/Home/Index.cshtml");
            }
            var allShares = _context.Shares.Where(x=>x.UserId==uid).ToList();
            if(allShares.Count == 0)
            {
                ViewBag.ShowNoSharedLinksAlert = true;
                return View("~/Views/Home/Index.cshtml");
            }
            return View(new LinkShares() { Shares=allShares});
        }
        [HttpGet("Home/DeleteSharedLink")] // Should be post but im too lazy to add a form to just delete a shared link
        [Authorize]
        public async Task<IActionResult> DeleteSharedLink(Guid shareToken)
        {
            if (!Request.Headers.TryGetValue("Referer",out var referer) || referer.Count != 1)
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

        [HttpPost("Home/ShareLink")]
        [Authorize]
        public async Task<IActionResult> ConfirmShare(string city, Guid shareToken, Guid uid, bool metric, string? foreCastDate, bool? displayMultipleDays, int? DayAmount)
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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
