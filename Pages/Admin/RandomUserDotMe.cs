using Newtonsoft.Json;
using southosting.Data;
using southosting.Models;
using System.Collections.Generic;
using System;

namespace southosting.Pages.Admin
{
    public class RandomUserDotMeModel
    {
        [JsonProperty("results")]
        public List<RandomUserDotMeResult> Results { get; set; }
    }

    public class RandomUserDotMeName
    {
        [JsonProperty("first")]
        public string FirstName { get; set; }
    
        [JsonProperty("last")]
        public string LastName { get; set; }
    }

    public class RandomUserDotMeLocation
    {
        [JsonProperty("street")]
        public string Street { get; set; }

        [JsonProperty("postcode")]
        public string Postcode { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }
    }

    public class RandomUserDotMeResult
    {
        [JsonProperty("name")]
        public RandomUserDotMeName Name { get; set; }

        [JsonProperty("location")]
        public RandomUserDotMeLocation Address { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        public SouthostingUser GetUser()
        {
            return new SouthostingUser { FirstName = Name.FirstName,
                                         LastName = Name.LastName,
                                         UserName = Email,
                                         Email = Email };
        }

        public Advert GetAdvert(string LandlordId, bool Submit, bool Accept)
        {
            return new Advert { Title = Address.Street,
                                Description = Address.Street,
                                Postcode = Address.Postcode, 
                                Submitted = Submit,
                                Accepted = Submit ? Accept : false,
                                Comment = "",
                                LandlordID = LandlordId };
        }
    }
}