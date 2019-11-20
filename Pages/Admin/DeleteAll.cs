using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using southosting.Data;
using southosting.Models;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using southosting.Logic;
using southosting.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace southosting.Pages.Admin
{
    public class DeleteAllModel : PageModel
    {
        public readonly SouthostingContext _context;
        public readonly UserManager<SouthostingUser> _userManager;

        public DeleteAllModel(SouthostingContext context, UserManager<SouthostingUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public void OnGet() {}

        public async Task<IActionResult> OnPostAsync()
        {
            var adverts = await _context.Advert.ToListAsync();
            foreach (var advert in adverts) {
                _context.Advert.Remove(advert);
            }
            await _context.SaveChangesAsync();

            var thisUserId = _userManager.GetUserId(User);
            var users = await _userManager.Users
                                          .Where(u => u.Id != thisUserId) // don't delete the current user
                                          .ToListAsync();
            foreach (var user in users) {
                await _userManager.DeleteAsync(user);
            }

            return RedirectToPage("./Index");
        }
    }
}