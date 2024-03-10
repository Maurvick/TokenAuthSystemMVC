using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using TokenAuthSystemMvc.Server.Models;
using TokenAuthSystemMVC.Areas.Identity.Data;
using TokenAuthSystemMVC.Models;
using TokenAuthSystemMVC.Services;

namespace TokenAuthSystemMVC.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenProvider _jwtTokenProvider; 

        public HomeController(
            UserManager<ApplicationUser> userManager,
            IJwtTokenProvider jwtTokenProvider)
        {
            _userManager = userManager;
            _jwtTokenProvider = jwtTokenProvider;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            if (_jwtTokenProvider.IsTokenValid() is not true)
            {
                return Redirect("/Identity/Account/Login");
            }

            string token = HttpContext.Session.GetString("Token") ?? "";

            ViewData["TokenValidUntil"] = $"Token expires in: {_jwtTokenProvider.GetTokenExpirationTime()} minutes";
            ViewData["UserToken"] = _jwtTokenProvider.GetToken(token, 50);
            ViewData["UserId"] = _userManager.GetUserId(User);
            ViewData["IsTokenValid"] = _jwtTokenProvider.IsTokenValid();
            ViewData["UserRole"] = User.IsInRole(UserRoleModel.Admin);

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
