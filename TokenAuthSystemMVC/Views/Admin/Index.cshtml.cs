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
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<UserModel> UserModel { get;set; } = default!;

        public async Task OnGetAsync()
        {
            UserModel = await _context.UserModel.ToListAsync();
        }
    }
}
