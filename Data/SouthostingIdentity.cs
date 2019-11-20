using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace southosting.Data
{
    public class SouthostingUser : IdentityUser
    {
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public string FullName {
            get {
                return FirstName + " " + LastName;
            }
        }
    }

    public class SouthostingRole : IdentityRole
    {
        public string Title { get; set; }
    }
}