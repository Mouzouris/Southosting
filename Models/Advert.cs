using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using southosting.Data;

namespace southosting.Models
{
    public class Advert
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public string Title { get; set; }
        
        [Required]
        public string Description { get; set; }
        
        [Required, MinLength(3), MaxLength(8)]
        public string Postcode { get; set; }
        
        public bool Submitted { get; set; }
        
        public bool Accepted { get; set; }

        public string Comment { get; set; }

        public string LandlordID { get; set; }
        
        [ForeignKey("LandlordID")]
        public SouthostingUser Landlord { get; set; }

        public IList<Upload> Uploads { get; set; }
    }
}