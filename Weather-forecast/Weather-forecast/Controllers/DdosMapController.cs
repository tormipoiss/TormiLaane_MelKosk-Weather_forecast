using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Weather_forecast.Controllers
{
    [Authorize]
    public class DdosMapController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CheckPointThreatMap()
        {
            return View();
        }
        public IActionResult BitDefender()
        {
            return View();
        }
    }
}
