using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TokenAuthSystemMVC.Areas.Identity.Data;
using TokenAuthSystemMVC.Data;
using TokenAuthSystemMVC.Models;

namespace TokenAuthSystemMVC.Areas.Identity.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(ApplicationDbContext context, 
            UserManager<ApplicationUser> userManger)
        {
            _context = context;
            _userManager = userManger;
        }

        public IList<ApplicationUser> Users { get;set; } = default!;

        public void OnGetAsync()
        {
            Users = _userManager.Users.ToList();
        }
    }
}
