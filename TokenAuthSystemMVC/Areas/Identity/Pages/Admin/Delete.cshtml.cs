using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TokenAuthSystemMVC.Areas.Identity.Data;
using TokenAuthSystemMVC.Data;

namespace TokenAuthSystemMVC.Areas.Identity.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DeleteModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public ApplicationUser UserModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var usermodel = await _context.UserModel.FirstOrDefaultAsync(m => m.Id == id);
            var userModel = await _userManager.FindByIdAsync(id);

            if (userModel == null)
            {
                return NotFound();
            }
            else
            {
                UserModel = userModel;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usermodel = await _userManager.FindByIdAsync(id);
            if (usermodel != null)
            {
                UserModel = usermodel;
                await _userManager.DeleteAsync(UserModel);
            }

            return RedirectToPage("./Index");
        }
    }
}
