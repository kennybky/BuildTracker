using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuildTrackerApi.Models
{
    public partial class User : IdentityUser<int>
    {
        public User()
        {
            ProductDevelopers = new HashSet<ProductDeveloper>();
        }

        
        public Role? Role { get; internal set; } 

        public virtual ICollection<ProductDeveloper> ProductDevelopers { get; internal set; }

        [PersonalData]
        public string FirstName { get; internal set; }
        [PersonalData]
        public string LastName { get; internal set; }



        //public byte[] PasswordHash { get; internal set; }
        //public byte[] PasswordSalt { get; internal set; }

    }

    
    public enum Role
    {
       USER = 0, DEVELOPER = 1, PROJECT_MANAGER = 2, ADMIN = 3
    }
}
