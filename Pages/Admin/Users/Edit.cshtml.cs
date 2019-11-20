using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using southosting.Data;
using southosting.Authorization;

namespace southosting.Pages.Admin.Users
{
    [AuthorizeRoles()]
    public class EditModel : PageModel
    {
        private readonly SouthostingContext _context;
        private readonly UserManager<SouthostingUser> _userManager;

        public EditModel(SouthostingContext context, UserManager<SouthostingUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public List<SelectListItem> RoleOptions { get; set; }

        private SouthostingUser SouthostingUser { get; set; }

        public class InputModel
        {
            public string Id { get; set; }
            
            public string FirstName { get; set; }

            public string LastName { get; set; }

            public string Email { get; set; }

            public string Role { get; set; }

            public string FullName {
                get {
                    return FirstName + " " + LastName;
                }
            }
        }

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

            RoleOptions = new List<SelectListItem> {
                new SelectListItem { Value = Constants.StudentRole, Text = Constants.StudentRole },
                new SelectListItem { Value = Constants.LandlordRole, Text = Constants.LandlordRole },
                new SelectListItem { Value = Constants.AccommodationOfficerRole, Text = Constants.AccommodationOfficerRole },
                new SelectListItem { Value = Constants.AdministratorRole, Text = Constants.AdministratorRole }
            };

            Input = new InputModel {
                Id = SouthostingUser.Id,
                FirstName = SouthostingUser.FirstName,
                LastName = SouthostingUser.LastName,
                Email = SouthostingUser.Email,
                Role = (await _userManager.GetRolesAsync(SouthostingUser)).FirstOrDefault()
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            SouthostingUser = await _userManager.FindByIdAsync(id);

            SouthostingUser.FirstName = Input.FirstName;
            SouthostingUser.LastName = Input.LastName;
            SouthostingUser.Email = Input.Email;

            _context.Attach(SouthostingUser).State = EntityState.Modified;

            var oldRole = (await _userManager.GetRolesAsync(SouthostingUser)).FirstOrDefault();
            if (Input.Role != oldRole)
            {
                await _userManager.RemoveFromRoleAsync(SouthostingUser, oldRole);
                await _userManager.AddToRoleAsync(SouthostingUser, Input.Role);
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
