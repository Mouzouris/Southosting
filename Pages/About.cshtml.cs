﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace southosting.Pages
{
    [AllowAnonymous]
    public class AboutModel : PageModel
    {
        public void OnGet()
        {}
    }
}
