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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SearchHistoryServices _searchHistoryServices;

        public HomeController(UserManager<ApplicationUser> userManager, SearchHistoryServices searchHistoryServices)
        {
            _userManager = userManager;
            _searchHistoryServices = searchHistoryServices;
        }

        public IActionResult Index()
        {
            var uid = _userManager.GetUserId(User);
            History? testHistory = _searchHistoryServices.GetUserHistory(uid);
            if (testHistory != default)
            {
                ViewData["showHistory"] = "true";
            }
            return View();
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
