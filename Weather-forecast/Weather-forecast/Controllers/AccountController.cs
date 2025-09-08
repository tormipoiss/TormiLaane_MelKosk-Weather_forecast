using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Weather_forecast.Models;
using Weather_forecast.ViewModels;

namespace Weather_forecast.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AccountController(UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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
                var account = new ApplicationUser()
                {
                    UserName = model.Username,
                    Email = model.Username,
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
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user == null) return RedirectToAction("Index", "Home");
                var check = await _userManager.CheckPasswordAsync(user, model.Password);
                if (!check)
                {
                    ViewBag.ErrorTile = "Invalid login";
                    ViewBag.ErrorMessage = "Username or password is incorrect";
                    return View("Error");
                }
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                ViewBag.ErrorTile = "Invalid login";
                ViewBag.ErrorMessage = "Username or password is incorrect";
                return View("Error");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
