using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TokenAuthSystemMVC.Areas.Identity.Data;
using TokenAuthSystemMVC.Services;

namespace TokenAuthSystemMVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [Authorize(Roles = UserRoles.Admin)]
        public IActionResult AdminPage()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }
    }
}
