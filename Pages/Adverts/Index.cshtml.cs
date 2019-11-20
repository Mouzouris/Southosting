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
    public class IndexModel : PageModel
    {
        private readonly SouthostingContext _context;

        public IndexModel(SouthostingContext context)
        {
            _context = context;
        }

        public IList<Advert> Advert { get;set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Advert = await _context.Advert
                .Where(a => a.Submitted && a.Accepted)
                .Include(a => a.Landlord)
                .Include(a => a.Uploads)
                .ToListAsync();
            return Page();
        }
    }
}
