using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using southosting.Data;
using southosting.Models;
using southosting.Authorization;

namespace southosting.Pages.Adverts.Manage
{
    [Authorize(Policy = "ElevatedRights")]
    public class IndexModel : PageModel
    {
        private readonly SouthostingContext _context;
        private readonly UserManager<SouthostingUser> _userManager;

        public IndexModel(SouthostingContext context, UserManager<SouthostingUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<Advert> Advert { get;set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (User.IsInRole(Constants.StudentRole))
            {
                return RedirectToPage("/"); // students can't manage anything
            } 
            else if (User.IsInRole(Constants.LandlordRole))
            {
                Advert = await _context.Advert
                    .Where(a => a.LandlordID == user.Id)  // owned by this user
                    .Include(a => a.Landlord)
                    .Include(a => a.Uploads)
                    .ToListAsync();
            }
            else if (User.IsInRole(Constants.AccommodationOfficerRole))
            {
                Advert = await _context.Advert
                    .Where(a => a.Submitted && !a.Accepted)  // submitted but not accepted
                    .Include(a => a.Landlord)
                    .Include(a => a.Uploads)
                    .ToListAsync();
            }
            else
            {
                // admin, so show everthing
                Advert = await _context.Advert
                    .Include(a => a.Landlord)
                    .Include(a => a.Uploads)
                    .ToListAsync();
            }
            
            return Page();
        }
    }
}
