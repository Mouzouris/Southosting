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
    [AuthorizeRoles()]
    public class GenerateModel : PageModel
    {
        private readonly SouthostingContext _context;
        private readonly UserManager<SouthostingUser> _userManager;
        private readonly AppSettings _appSettings;
        private readonly ILogger<GenerateModel> _logger;
        private readonly Random random;


        public GenerateModel(SouthostingContext context, 
                             UserManager<SouthostingUser> userManager,
                             IOptions<AppSettings> appSettings,
                             ILogger<GenerateModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _appSettings = appSettings.Value;
            _logger = logger;
            random = new Random();

            Input = new InputModel {
                Adverts = 0,
                Landlords = 0,
                Students = 0,

                CreatedAdverts = new List<Advert>(),
                CreatedLandlords = new List<SouthostingUser>(),
                CreatedStudents = new List<SouthostingUser>()           
            };
        }   

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public int Students { get; set; }

            public int Landlords { get; set; }

            public int Adverts { get; set; }

            [Display(Name = "Advert Image URL")]
            public string AdvertPlaceholder { get; set; }

            public List<SouthostingUser> CreatedStudents { get; set; }

            public List<SouthostingUser> CreatedLandlords { get; set; }

            public List<Advert> CreatedAdverts { get; set; }
        }

        public void OnGet()
        {
            
        }

        private async Task<string> GetRandomUserId(string Role = null)
        {
            int offset;
            if (Role == null)
            {
                var users = await _context.Users.ToListAsync();
                offset = random.Next(users.Count());
                return users[offset].Id;
            }
            else
            {
                var users = await _userManager.GetUsersInRoleAsync(Role);
                offset = random.Next(users.Count());
                return users[offset].Id;
            }

        }

        [HttpGet]
        private List<RandomUserDotMeResult> GetRandomData(int count = 1)
        {
            if (count == 0) return new List<RandomUserDotMeResult>();
            string url = "https://randomuser.me/api/?nat=gb&results=" + count;
            HttpClient http = new HttpClient();
            var response = http.GetAsync(url).Result;
            response.EnsureSuccessStatusCode();

            var result = response.Content.ReadAsStringAsync().Result;
            var model = JsonConvert.DeserializeObject<RandomUserDotMeModel>(result);
            return model.Results;
        }

        private async Task<List<SouthostingUser>> CreateUsers(string Role, int N)
        {
            List<SouthostingUser> created = new List<SouthostingUser>();
            List<RandomUserDotMeResult> results = GetRandomData(N);
            foreach (RandomUserDotMeResult result in results)
            {
                var user = result.GetUser();
                var userResult = await _userManager.CreateAsync(user, _appSettings.DefaultPassword);
                var roleResult = await _userManager.AddToRoleAsync(user, Role);
                if (userResult.Succeeded && roleResult.Succeeded)
                {
                    created.Add(user);
                }
                else
                {
                    _logger.LogError("Failed to create a student user.");
                }
            }
            return created;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // fetch and populate data
            Input.CreatedStudents = await CreateUsers(Constants.StudentRole, Input.Students);
            Input.CreatedLandlords = await CreateUsers(Constants.LandlordRole, Input.Landlords);
            Input.CreatedAdverts = new List<Advert>();

            // create adverts
            if (Input.Adverts > 0 && string.IsNullOrEmpty(Input.AdvertPlaceholder))
            {
                ModelState.AddModelError("Input.AdvertPlaceholder", "A default advert image must be given if adverts are to be created.");
                return Page();
            }

            List<RandomUserDotMeResult> results = GetRandomData(Input.Adverts);

            foreach (RandomUserDotMeResult result in results)
            {
                var userId = await GetRandomUserId(Constants.LandlordRole);
                var submit = random.NextDouble() >= 0.33; // 2/3 times
                var accept = random.NextDouble() >= 0.5; // half of the submit times
                // => 1/3 not submitted, 1/3 not moderated, 1/3 accepted
                var advert = result.GetAdvert(userId, submit, accept);
                _context.Advert.Add(advert);

                // create an upload with the required image
                var upload = ModelCreator.GetUrlUpload(Input.AdvertPlaceholder, advert, "placeholder");
                _context.Upload.Add(upload);

                Input.CreatedAdverts.Add(advert);
            }
            await _context.SaveChangesAsync();
            
            return Page();
        }
    }
}