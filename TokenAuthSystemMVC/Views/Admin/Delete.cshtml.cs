using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TokenAuthSystemMVC.Data;
using TokenAuthSystemMVC.Models;

namespace TokenAuthSystemMVC.Views.Admin
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public UserModel UserModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usermodel = await _context.UserModel.FirstOrDefaultAsync(m => m.Id == id);

            if (usermodel == null)
            {
                return NotFound();
            }
            else
            {
                UserModel = usermodel;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usermodel = await _context.UserModel.FindAsync(id);
            if (usermodel != null)
            {
                UserModel = usermodel;
                _context.UserModel.Remove(UserModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
