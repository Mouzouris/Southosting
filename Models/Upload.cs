using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace southosting.Models
{
    public class Upload
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]        
        public int Id { get; set; }

        public string ImagePath { get; set; }

        public string ThumbnailImagePath { get; set; }

        public string InternalFileName { get; set; }

        public string OriginalFileName { get; set; }

        [Display(Name = "Uploaded (UTC)")]
        [DisplayFormat(DataFormatString = "{0:F}")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]        
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public int AdvertID { get; set; }
        public Advert Advert { get; set; }
    }
}
