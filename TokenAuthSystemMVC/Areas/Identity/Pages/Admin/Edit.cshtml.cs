using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TokenAuthSystemMvc.Server.Models;
using TokenAuthSystemMVC.Areas.Identity.Data;
using TokenAuthSystemMVC.Data;
using TokenAuthSystemMVC.Models;

namespace TokenAuthSystemMVC.Areas.Identity.Pages.Admin
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<EditModel> _logger;

        public EditModel(ApplicationDbContext context, ILogger<EditModel> logger,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        [BindProperty]
        public ApplicationUser UserModel { get; set; } = default!;
        [BindProperty]
        public IList<string> UserRoles { get; set; } = default!;
        [Display(Name = "Give admin permissions.")]
        public bool IsAdmin { get; set; } = default!;
        private ApplicationUser CurrentUser { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // var usermodel =  await _context.UserModel.FirstOrDefaultAsync(m => m.Id == id);
            var CurrentUser = await _userManager.FindByIdAsync(id);

            if (CurrentUser == null)
            {
                _logger.LogInformation($"User with id: {id} not found.");
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(CurrentUser);

            UserModel = CurrentUser;
            UserRoles = userRoles;

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }


            //_context.Attach(UserModel).State = EntityState.Modified;

            if (IsAdmin)
            {
                await _userManager.AddToRoleAsync(CurrentUser, UserRoleModel.Admin);
            }

            //try
            //{
            //    await _context.SaveChangesAsync();
            //}
            //catch (DbUpdateConcurrencyException)
            //{
            //    var isExists = await UserModelExists(UserModel.Id);
            //    if (!isExists)
            //    {
            //        return NotFound();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

            return RedirectToPage("./Index");
        }

        //private async Task<bool> UserModelExists(string id)
        //{
        //    return await _userManager.FindByIdAsync(id) != null ? true : false;
        //    // return _context.UserModel.Any(e => e.Id == id);
        //}
    }
}
