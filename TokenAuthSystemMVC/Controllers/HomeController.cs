using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TokenAuthSystemMVC.Areas.Identity.Data;
using TokenAuthSystemMVC.Interfaces;
using TokenAuthSystemMVC.Models;

namespace TokenAuthSystemMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService; 

        public HomeController(
            UserManager<ApplicationUser> userManager,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public IActionResult Index()
        {
            string token = HttpContext.Session.GetString("Token") ?? "None.";
            ViewData["UserToken"] = _tokenService.GetToken(token, 50);

            ViewData["UserId"] = _userManager.GetUserId(User) ?? "None.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel 
            { 
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier 
            });
        }
    }
}
