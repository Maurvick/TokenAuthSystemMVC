using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TokenAuthSystemMVC.Areas.Identity.Data;
using TokenAuthSystemMVC.Data;
using TokenAuthSystemMVC.Models;

namespace TokenAuthSystemMVC.Areas.Identity.Pages.Admin
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateModel(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public UserModel UserModel { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            // TODO: Check if user admin

            //_userManager.CreateAsync();
            // _context.UserModel.Add(UserModel);
            // await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
