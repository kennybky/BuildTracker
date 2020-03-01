using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildTrackerApi.Models
{
    public class AppRole: IdentityRole<int>
    {

        public AppRole(string role): base(role)
        {

        }

        public AppRole(): base()
        {

        }

        public AppRole(Role role) : this(role.ToString())
        {

        }

    }
}
