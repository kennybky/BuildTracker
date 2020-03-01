using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace BuildTrackerApi.Models
{
    public partial class User : IdentityUser<int>
    {
        public User()
        {
            ProductDevelopers = new HashSet<ProductDeveloper>();
            Builds = new HashSet<Build>();
            BuildUpdates = new HashSet<Build>();
            Tests = new HashSet<Test>();
        }

        

        public virtual ICollection<ProductDeveloper> ProductDevelopers { get; set; }

        [PersonalData]
        public string FirstName { get; internal set; }
        [PersonalData]
        public string LastName { get; internal set; }

        public bool AccountConfirmed { get; internal set; }

        

        

        [IgnoreDataMember]
        public virtual ICollection<Build> Builds { get; set; }
        [IgnoreDataMember]
        public virtual ICollection<Test> Tests { get; set; }
        [IgnoreDataMember]
        public virtual ICollection<Build> BuildUpdates { get; set; }
    }

    
    public enum Role
    {
        USER = 0,
        DEVELOPER = 1,
        PROJECT_MANAGER = 2,
        ADMIN = 3
    }
}
