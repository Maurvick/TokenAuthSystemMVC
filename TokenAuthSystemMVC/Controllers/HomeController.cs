using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
            if (!_jwtTokenProvider.IsTokenValid())
            {
                return Redirect("/Identity/Account/Login");
            }

            // Get the expiration time from the ClaimsPrincipal
            ClaimsPrincipal user = HttpContext.User;

            DateTime expiresUtc;

            string? expValue = user.FindFirstValue("exp");

            if (expValue != null && long.TryParse(expValue, out long expUnixTime))
            {
                expiresUtc = DateTimeOffset.FromUnixTimeSeconds(expUnixTime).UtcDateTime;
            }
            else
            {
                // Handle the case where "exp" is null or not convertible to long
                expiresUtc = DateTime.MinValue;
            }

            // Calculate the remaining time until expiration
            TimeSpan timeUntilExpiration = expiresUtc - DateTime.UtcNow;

            ViewData["TokenValidUntil"] = $"Token expires in: {timeUntilExpiration.TotalMinutes} minutes";

            string token = HttpContext.Session.GetString("Token") ?? "";

            ViewData["UserToken"] = _jwtTokenProvider.GetToken(token, 50);
            ViewData["UserId"] = _userManager.GetUserId(User);
            ViewData["IsTokenValid"] = _jwtTokenProvider.IsTokenValid();

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
