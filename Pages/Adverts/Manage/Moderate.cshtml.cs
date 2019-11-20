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
using System.ComponentModel.DataAnnotations;


namespace southosting.Pages.Adverts.Manage
{
    [AuthorizeRoles(Constants.AccommodationOfficerRole)]
    public class ModerateModel : PageModel
    {
        private readonly southosting.Data.SouthostingContext _context;

        public ModerateModel(southosting.Data.SouthostingContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Advert Advert { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Moderation")]
            public bool Accept { get; set; }

            [MinLength(50), MaxLength(200)]
            [Display(Name = "Comment")]
            public string Comment { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return RedirectToPage("./Index");
            }

            Advert = await _context.Advert
                .Include(a => a.Landlord)
                .Include(a => a.Uploads)
                .FirstOrDefaultAsync(m => m.ID == id);

            Input = new InputModel {
                Accept = false,
                Comment = string.Empty
            };

            if (Advert == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            Advert = await _context.Advert.Include(a => a.Landlord)
                                          .Include(a => a.Uploads)
                                          .FirstOrDefaultAsync(m => m.ID == id);

            if (!Input.Accept && string.IsNullOrEmpty(Input.Comment))
            {
                ModelState.AddModelError("Input.Comment", "If the advert is being rejected, a comment must be provided.");
                return Page();
            }

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Comments should have between 50 and 200 characters.");
                return Page();
            }


            if (Advert != null)
            {
                Advert.Accepted = Input.Accept;
                Advert.Submitted = Input.Accept; // if it's rejected it should become unsubmitted
                Advert.Comment = Input.Accept ? "" : Input.Comment;
                _context.Attach(Advert).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
