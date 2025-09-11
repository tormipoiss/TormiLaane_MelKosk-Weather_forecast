using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Weather_forecast.Data;
using Weather_forecast.Models;

namespace Weather_forecast.Controllers
{
    public class WeatherController : Controller
    {
        private readonly DatabaseContext _context;

        public WeatherController(DatabaseContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult SearchWeatherByCity(History model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Home/Index.cshtml", model);
            }
            if (model.City == null || model.City == "")
            {
                return View("~/Views/Home/Index.cshtml");
            }
            SaveCityToHistory(model.City);
            return View("~/Views/Home/Index.cshtml");
        }

        [HttpPost]
        public IActionResult SaveCityToHistory(string city)
        {
            var history = new History();
            //ulong newId = 0;
            //if (_context.SearchHistory.Any())
            //{
            //    newId = _context.SearchHistory.Max(x => x.Id) + 1;
            //}
            //else { newId = 1; }
            //history.Id = newId;
            history.City = city;
            history.DateAndTimeOfSearch = DateTime.Now;
            _context.SearchHistory.Add(history);
            _context.SaveChanges();
            return View("~/Views/Home/Index.cshtml");
        }
    }
}
