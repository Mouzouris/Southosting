using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using southosting.Data;
using southosting.Models;
using southosting.Logic;
using Microsoft.Extensions.Options;
using southosting.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.IO;
using System.ComponentModel.DataAnnotations;

namespace southosting.Pages.Adverts.Manage
{
    [AuthorizeRoles(Constants.LandlordRole)]
    public class CreateModel : PageModel
    {
        private readonly SouthostingContext _context;
        private readonly UserManager<SouthostingUser> _userManager;
        private readonly BlobStorage _blobConfig;


        public CreateModel(SouthostingContext context, 
                           UserManager<SouthostingUser> userManager,
                           IOptions<BlobStorage> blobConfig)
        {
            _context = context;
            _userManager = userManager;
            _blobConfig = blobConfig.Value;
        }

        public IActionResult OnGet()
        {
            Input = new Advert {};
            return Page();
        }

        [BindProperty]
        public Advert Input { get; set; }

        [BindProperty]
        [Required]
        public IFormCollection FirstUpload { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (FirstUpload == null)
            {
                ModelState.AddModelError("", "You must upload a file.");
                return Page();
            }

            if (string.IsNullOrEmpty(Input.Postcode))
            {
                 ModelState.AddModelError("", "You must provide a valid postcode.");
                 return Page();
            }

            if (User.IsInRole(Constants.LandlordRole) ||
                User.IsInRole(Constants.AdministratorRole))
            {
                
                var userId = _userManager.GetUserId(User);
                var advert = new Advert { Title = Input.Title,
                                        Description = Input.Description,
                                        Postcode = Input.Postcode,
                                        Submitted = Input.Submitted,
                                        Accepted = false,
                                        Comment = "",
                                        LandlordID = userId };

                _context.Advert.Add(advert);

                foreach (var formFile in FirstUpload.Files)
                {
                    var extension = Path.GetExtension(formFile.FileName);
                    var filename = Guid.NewGuid().ToString() + extension;
                    bool isUploaded;

                    if (FileHelpers.IsImage(formFile))
                    {
                        isUploaded = await FileHelpers.UploadFileToStorage(formFile, filename, _blobConfig);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unsupported media type.");
                        return Page();
                    }

                    if (isUploaded)
                    {
                        var upload = new Upload
                        {
                            ImagePath = Path.Join(_blobConfig.Url, _blobConfig.ImageContainer, filename),
                            ThumbnailImagePath = Path.Join(_blobConfig.Url, _blobConfig.ThumbnailContainer, filename),
                            InternalFileName = filename,
                            OriginalFileName = formFile.FileName,
                            AdvertID = advert.ID,
                            Advert = advert
                        };

                        _context.Upload.Add(upload);
                    }
                }

                await _context.SaveChangesAsync();

                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}