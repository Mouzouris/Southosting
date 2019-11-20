using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.IO;
using southosting.Models;

namespace southosting.Logic
{
    public class IFormCollectionWrapper
    {
        [Required]
        [Display(Name = "Choose images")]
        public IFormCollection File { get; set; }
    }

    public class PostcodeWrapper
    {
        [Required, MinLength(3), MaxLength(8)]
        public string Postcode { get; set; }

        public bool ShowPostcode { get; set; } = false;
    }

    public class ModelCreator
    {
        public static Upload GetUrlUpload(string url, Advert advert, string filename = null)
        {
            if (filename == null) filename = Path.GetFileName(url);
            return new Upload {
                ImagePath = url,
                ThumbnailImagePath = url,
                InternalFileName = filename,
                OriginalFileName = filename,
                AdvertID = advert.ID,
                Advert = advert
            };
        }
    }
}