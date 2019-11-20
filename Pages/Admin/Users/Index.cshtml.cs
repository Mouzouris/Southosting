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
    public class IndexModel : PageModel
    {
        private readonly SouthostingContext _context;
        private readonly UserManager<SouthostingUser> _userManager;

        public IndexModel(SouthostingContext context, UserManager<SouthostingUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public IList<InputModel> Input { get;set; }

        public class InputModel
        {
            public string Id { get; set; }

            public string Name { get; set; }

            public string Email { get; set; }

            public string Roles { get; set; }
        }

        public async Task OnGetAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            Input = new List<InputModel>();
            foreach (var user in users)
            {
                Input.Add(new InputModel {
                    Id = user.Id,
                    Name = user.FullName,
                    Email = user.Email,
                    Roles = string.Join(", ", await _userManager.GetRolesAsync(user))
                });
            }
        }
    }
}
