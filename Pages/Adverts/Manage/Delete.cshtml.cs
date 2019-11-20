using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using southosting.Data;
using southosting.Models;
using southosting.Authorization;

namespace southosting.Pages.Adverts.Manage
{
    [AuthorizeRoles(Constants.LandlordRole)]
    public class DeleteModel : PageModel
    {
        private readonly southosting.Data.SouthostingContext _context;

        public DeleteModel(southosting.Data.SouthostingContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Advert Advert { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Advert = await _context.Advert
                .Include(a => a.Landlord)
                .Include(a => a.Uploads)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (Advert == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Advert = await _context.Advert.FindAsync(id);

            if (Advert != null)
            {
                _context.Advert.Remove(Advert);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
