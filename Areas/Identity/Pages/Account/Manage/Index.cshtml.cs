using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using southosting.Data;

namespace southosting.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<SouthostingUser> _userManager;
        private readonly SignInManager<SouthostingUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public IndexModel(
            UserManager<SouthostingUser> userManager,
            SignInManager<SouthostingUser> signInManager,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public ViewModel View { get; set; }

        public class ViewModel
        {
            public string Username { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Roles { get; set; }
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var userName = await _userManager.GetUserNameAsync(user);

            View = new ViewModel {
                Username = userName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = string.Join(" ", await _userManager.GetRolesAsync(user))
            };

            return Page();
        }

        public IActionResult OnPostAsync()
        {
            return RedirectToPage();
        }
    }
}
