using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using southosting.Data;
using southosting.Authorization;

namespace southosting.Pages.Admin.Users
{
    [AuthorizeRoles()]
    public class DeleteModel : PageModel
    {
        private readonly SouthostingContext _context;
        private readonly UserManager<SouthostingUser> _userManager;

        public DeleteModel(SouthostingContext context, UserManager<SouthostingUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public SouthostingUser SouthostingUser { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            SouthostingUser = await _userManager.FindByIdAsync(id);

            if (SouthostingUser == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            SouthostingUser = await _userManager.FindByIdAsync(id);

            if (SouthostingUser != null)
            {
                await _userManager.DeleteAsync(SouthostingUser);
            }

            return RedirectToPage("./Index");
        }
    }
}
