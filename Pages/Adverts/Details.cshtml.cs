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

namespace southosting.Pages.Adverts
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly southosting.Data.SouthostingContext _context;

        public DetailsModel(southosting.Data.SouthostingContext context)
        {
            _context = context;
        }

        public Advert Advert { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return RedirectToPage("Index");
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
    }
}
