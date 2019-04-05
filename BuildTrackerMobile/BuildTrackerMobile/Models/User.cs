using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildTrackerMobile.Models
{
    public partial class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public bool AccountConfirmed { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Role? Role { get; set; }
    }

    public enum Role
    {
        USER = 0,
        DEVELOPER = 1,
        PROJECT_MANAGER = 2,
        ADMIN = 3
    }
}
