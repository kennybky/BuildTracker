using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BuildTrackerMobile.Models
{
    public partial class Build
    {
        public Build()
        {
            Tests = new HashSet<Test>();
        }

        public int Id { get; set; }

     
        public string Version { get; set; }
        public string ProductName { get; set; }


        public DateTime? BuildDate { get; set; }


        public virtual User BuildPerson { get; set; }


        public virtual User UpdatePerson { get; set; }


        public string BuildNumber { get; set; }

        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        public Platform Platform { get; set; }

        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        public BuildType Type { get; set; }

        public DateTime? LastUpdate { get; set; }

        public virtual ICollection<Test> Tests { get; set; }
    }

    public enum BuildType
    {
        Test = 0,
        Alpha = 1,
        Beta = 2,
        Production = 3
    }

    public enum Platform
    {
        Android = 0,
        IOS = 1,
        Windows = 2,
        Mac = 3,
        Chromebook = 4,
        IPhone = 5,
        WinPhone = 6,
        Linux = 7,
        Ubuntu = 8,
        Windows_Legacy = 9
    }
}
