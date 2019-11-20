using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using southosting.Data;
using southosting.Models;
using southosting.Authorization;
using System.ComponentModel.DataAnnotations;

namespace southosting.Pages.Adverts.Manage
{
    [AuthorizeRoles(Constants.LandlordRole)]
    public class EditModel : PageModel
    {
        private readonly southosting.Data.SouthostingContext _context;

        public EditModel(southosting.Data.SouthostingContext context)
        {
            _context = context;
        }

        private Advert Advert { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public int ID { get; set; }

            [Required]
            public string Title { get; set; }

            [Required]
            public string Description { get; set; }

            [Required, MinLength(3), MaxLength(8)]
            public string Postcode { get; set; }

            [Display(Name = "Submit now")]
            public bool Submitted { get; set; }
        }

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

            Input = new InputModel
            {
                ID = Advert.ID,
                Title = Advert.Title,
                Description = Advert.Description,
                Postcode = Advert.Postcode,
                Submitted = Advert.Submitted
            };

            if (Advert == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            Console.WriteLine(Input.Postcode);
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var advert = await _context.Advert.FindAsync(id);

            if (advert != null)
            {
                advert.Title = Input.Title;
                advert.Description = Input.Description;
                advert.Postcode = Input.Postcode;
                advert.Submitted = Input.Submitted;
                advert.Accepted = false; // needs to be reevaluated if things have changed
                advert.Comment = ""; // if we've edited it, there should be a new comment
                _context.Attach(advert).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }

        private bool AdvertExists(int id)
        {
            return _context.Advert.Any(e => e.ID == id);
        }
    }
}
