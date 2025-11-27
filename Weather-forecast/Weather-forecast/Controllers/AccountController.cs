using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;
using Weather_forecast.Data;
using Weather_forecast.Models;
using Weather_forecast.Services;
using Weather_forecast.ViewModels;

namespace Weather_forecast.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserHistoryService _userHistoryService;
        private readonly SaveDatabaseService _saveDatabaseService;
        public AccountController(UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager, DatabaseContext context, UserHistoryService userHistoryService, SaveDatabaseService saveDatabaseService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userHistoryService = userHistoryService;
            _saveDatabaseService = saveDatabaseService;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(Account model)
        {
            if (ModelState.IsValid)
            {
                var existingUserByName = await _userManager.FindByNameAsync(model.Username);
                if (existingUserByName != null)
                {
                    ModelState.AddModelError("Name", "This username is already taken., Please choose another one!");
                    return View(model);
                }
                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("Name", "Password and confirm password do not match.");
                    return View(model);
                }
                var account = new ApplicationUser()
                {
                    UserName = model.Username,
                    Email = model.Username,
                    GlobalMetric = true
                };
                var result = await _userManager.CreateAsync(account, model.Password);
                if (result.Succeeded)
                {
                    return View("RegisterSuccess");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(Login model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Username);
                if (user == null)
                {
                    ViewBag.ErrorTile = "Invalid login";
                    ViewBag.ErrorMessage = "Username or password is incorrect";
                    return View("Error");
                }
                var check = await _userManager.CheckPasswordAsync(user, model.Password);
                if (!check)
                {
                    ViewBag.ErrorTile = "Invalid login";
                    ViewBag.ErrorMessage = "Username or password is incorrect";
                    return View("Error");
                }
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    var uid = _userManager.GetUserId(User);
                    History? testHistory = _userHistoryService.GetUserHistory(uid);
                    if (testHistory != default)
                    {
                        ViewData["showHistory"] = "true";
                    }
                    return View("~/Views/Home/Index.cshtml");
                }
                ViewBag.ErrorTile = "Invalid login";
                ViewBag.ErrorMessage = "Username or password is incorrect";
                return View("Error");
            }
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            ViewData["showHistory"] = "false";
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SettingsChanged(AccountSettings settings)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            user.GlobalMetric = settings.GlobalMetric;
            await _saveDatabaseService.SaveChangesAsync();
            return View("AccountSettings",settings);
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> AccountSettings()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            AccountSettings settings = new()
            {
                GlobalMetric = user.GlobalMetric,
            };
            return View(settings);
        }
    }
}
