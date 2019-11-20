using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using southosting.Data;
using southosting.Authorization;

namespace southosting.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<SouthostingUser> _signInManager;
        private readonly UserManager<SouthostingUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<SouthostingUser> userManager,
            SignInManager<SouthostingUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;

            RoleOptions = new List<SelectListItem> {
                new SelectListItem { Value = Constants.StudentRole, Text = Constants.StudentRole },
                new SelectListItem { Value = Constants.LandlordRole, Text = Constants.LandlordRole }
            };
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public List<SelectListItem> RoleOptions { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [Display(Name = "Role")]
            public string Role { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/Adverts");
            if (ModelState.IsValid)
            {
                var user = new SouthostingUser { FirstName = Input.FirstName,
                                                 LastName = Input.LastName,
                                                 UserName = Input.Email, 
                                                 Email = Input.Email };
                var role = Input.Role;
                if (role != Constants.StudentRole && role != Constants.LandlordRole) {
                    // they've sent a different role
                    ModelState.AddModelError(string.Empty, "Invalid role. Pick from: Student, Landlord.");
                    return Page();
                }

                var result = await _userManager.CreateAsync(user, Input.Password);
                var roleResult = await _userManager.AddToRoleAsync(user, role);
                if (result.Succeeded && roleResult.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
