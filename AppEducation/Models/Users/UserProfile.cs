using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AppEducation.Models.Users {

    public class UserProfile
    {
        [Key]
        [Column("Id")]
        public long UserProfileId {get;set;}

        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Birthday { get; set; }
        public string Job { get; set; }
        public bool Sex { get; set; }
        public string PhoneNumber { get; set; }

        public string UserId {get;set;}
        public AppUser User { get; set;}
        
    }
}