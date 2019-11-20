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

namespace southosting.Pages.Admin
{
    [AuthorizeRoles()]
    public class IndexModel : PageModel
    {
        private readonly SouthostingContext _context;

        public IndexModel(SouthostingContext context)
        {
            _context = context;
        }

        public void OnGet()
        {}
    }
}