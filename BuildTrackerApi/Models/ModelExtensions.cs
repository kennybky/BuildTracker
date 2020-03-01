using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildTrackerApi.Models
{
    public static class ModelExtensions
    {

        public static IEnumerable<AppRole> GetRoles()
        {
            List<AppRole> list = new List<AppRole>();
            foreach(var value in Enum.GetValues(typeof(Role)).Cast<Role>())
            {
                AppRole role = new AppRole(value);
                list.Add(role);
            }
            return list;
        }
    }
}
