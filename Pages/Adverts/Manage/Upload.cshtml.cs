using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using southosting.Data;
using southosting.Logic;
using southosting.Models;
using southosting.Authorization;
using System.ComponentModel.DataAnnotations;
// using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.Extensions.Options;

namespace southosting.Pages.Adverts.Manage
{
    [AuthorizeRoles(Constants.LandlordRole)]
    public class UploadModel : PageModel
    {
        private readonly SouthostingContext _context;
        private readonly UserManager<SouthostingUser> _userManager;
        private readonly BlobStorage _blobConfig;

        public UploadModel(SouthostingContext context, 
                           UserManager<SouthostingUser> userManager,
                           IOptions<BlobStorage> blobConfig)
        {
            _context = context;
            _userManager = userManager;
            _blobConfig = blobConfig.Value;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel {
            [Required]
            [Display(Name = "Choose Images")]
            public IFormCollection Image { get; set; }
        }

        public IList<Upload> Uploads { get; set; }

        public Advert Advert { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            ModelState.Clear();
            if (id == null)
            {
                return RedirectToPage("./Index");
            }

            Advert = _context.Advert.FirstOrDefault(m => m.ID == id);

            if (Advert == null)
            {
                return NotFound();
            }

            if (_blobConfig.AccountKey == string.Empty || _blobConfig.AccountName == string.Empty)
            {
                ModelState.AddModelError("", "Can't retrieve blob storage setttings.");
                return Page();
            }

            if (_blobConfig.ImageContainer == string.Empty)
            {
                ModelState.AddModelError("" ,"No image container name.");
                return Page();
            }

            Input = new InputModel {
                Image = null
            };
            Uploads = await _context.Upload
                                 .Where(u => u.AdvertID == id)
                                 .AsNoTracking()
                                 .ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Advert = _context.Advert.FirstOrDefault(m => m.ID == id);
            if (Advert == null)
            {
                return NotFound();
            }

            foreach (var formFile in Input.Image.Files)
            {
                // var formFile = Input.Image;
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
                        AdvertID = Advert.ID,
                        Advert = Advert
                    };

                    _context.Upload.Add(upload);
                    await _context.SaveChangesAsync();
                }
            }
            
            Uploads = await _context.Upload
                                 .Where(u => u.AdvertID == id)
                                 .AsNoTracking()
                                 .ToListAsync();
            return Page();
        }


        public async Task<IActionResult> OnPostDeleteAsync(int id, int UploadId)
        {
            var upload = await _context.Upload.FindAsync(UploadId);

            if (upload != null) {
                _context.Upload.Remove(upload);
                await _context.SaveChangesAsync();
                if (upload.InternalFileName != null)
                {
                    await FileHelpers.DeleteFileAsync(upload.InternalFileName, _blobConfig);
                }
                Console.WriteLine("Deleted upload " + UploadId);
            }
            
            Uploads = await _context.Upload
                                 .Where(u => u.AdvertID == id)
                                 .AsNoTracking()
                                 .ToListAsync();
            return RedirectToPage();
        }
    }
}