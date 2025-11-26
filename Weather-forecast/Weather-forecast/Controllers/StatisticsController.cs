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
    public class StatisticsController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SharesServices _sharesServices;
        private readonly SaveDatabaseService _saveDatabaseService;
        private readonly CitiesServices _citiesServices;

        public StatisticsController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, SharesServices sharesServices, SaveDatabaseService saveDatabaseService, CitiesServices citiesServices)
        {
            _logger = logger;
            _userManager = userManager;
            _sharesServices = sharesServices;
            _saveDatabaseService = saveDatabaseService;
            _citiesServices = citiesServices;
        }

        [HttpGet("Home/Statistics")]
        [Authorize]
        public IActionResult GetAllStatistics()
        {
            var uid = _userManager.GetUserId(User);
            if (uid == null)
            {
                return View("~/Views/Home/Index.cshtml");
            }
            var allShares = _sharesServices.GetUserShares(uid);
            if (allShares.Count == 0)
            {
                ViewBag.ShowNoSharedLinksAlert = true;
                return View("~/Views/Home/Index.cshtml");
            }
            return View("~/Views/Home/ShareLinkStatistics.cshtml", new LinkShares() { Shares = allShares });
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
            var share = await _sharesServices.GetShareByShareToken(shareToken);
            if (share == null)
            {
                return RedirectToAction("Statistics");
            }
            if (share.UserId != uid)
            {
                return View("~/Views/Home/Index.cshtml");
            }
            _sharesServices.Remove(share);
            await _saveDatabaseService.SaveChangesAsync();
            var allShares = _sharesServices.GetUserShares(uid);
            if (allShares.Count == 0)
            {
                return View("~/Views/Home/Index.cshtml");
            }
            return Redirect("https://localhost:5001/Home/Statistics");
        }

        [HttpPost("Home/ShareLink")]
        [Authorize]
        public async Task<IActionResult> ConfirmShare(string city, Guid shareToken, Guid uid, bool metric, string? foreCastDate, bool? displayMultipleDays, int? DayAmount)
        {
            var exists = await _sharesServices.GetShareByShareToken(shareToken);
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
                await _sharesServices.AddAsync(new() { City = city, ShareToken = shareToken.ToString(), UserId = uid.ToString(), Metric = metric, forecastDate = forecastDateFixed, isMultipleDayForecast = true, DayAmount = DayAmount });
            }
            else
            {
                await _sharesServices.AddAsync(new() { City = city, ShareToken = shareToken.ToString(), UserId = uid.ToString(), Metric = metric, forecastDate = forecastDateFixed });
            }
            await _saveDatabaseService.SaveChangesAsync();
            //var existingUserByName = await _userManager.FindByNameAsync(User.Identity.Name);
            return Ok();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet("Home/History")]
        [Authorize]
        public async Task<IActionResult> History()
        {
            var uid = _userManager.GetUserId(User);
            CityAndApi historyCities = new CityAndApi();
            historyCities.Cities = _citiesServices.GetCitiesByUserID(uid);
            return View("~/Views/Home/History.cshtml", historyCities);
        }
    }
}
