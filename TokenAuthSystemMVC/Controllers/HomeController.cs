using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics;
using System.Security.Claims;
using TokenAuthSystemMVC.Areas.Identity.Data;
using TokenAuthSystemMVC.Models;
using TokenAuthSystemMVC.Services;

namespace TokenAuthSystemMVC.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenProvider _jwtTokenProvider; 

        public HomeController(
            UserManager<ApplicationUser> userManager,
            IJwtTokenProvider tokenService,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _jwtTokenProvider = tokenService;
            _roleManager = roleManager;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            if (!_jwtTokenProvider.IsTokenValid())
            {
                return Redirect("/Identity/Account/Login");
            }

            ClaimsPrincipal user = HttpContext.User;

            // Get the expiration time from the ClaimsPrincipal
            DateTime expiresUtc = user.FindFirstValue("exp") != null ?
                DateTimeOffset.FromUnixTimeSeconds(long.Parse(user.FindFirstValue("exp"))).UtcDateTime :
                DateTime.MinValue;

            // Calculate the remaining time until expiration
            TimeSpan timeUntilExpiration = expiresUtc - DateTime.UtcNow;

            // Now you can use 'timeUntilExpiration' as needed
            ViewData["TokenValidUntil"] = $"Token expires in: {timeUntilExpiration.TotalMinutes} minutes";

            string token = HttpContext.Session.GetString("Token") ?? "";
            var userId = _userManager.GetUserId(User);

            ViewData["UserToken"] = _jwtTokenProvider.GetToken(token, 50);
            ViewData["UserId"] = userId;
            ViewData["IsTokenValid"] = _jwtTokenProvider.IsTokenValid();

            ViewData["IsAdmin"] = User.IsInRole(UserRoles.Admin);

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
