using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TokenAuthSystemMVC.Areas.Identity.Data;

namespace TokenAuthSystemMVC.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
