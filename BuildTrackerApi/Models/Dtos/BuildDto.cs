using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BuildTrackerApi.Models.Dtos
{
    public partial class BuildDto
    {
        public BuildDto()
        {
            Tests = new HashSet<TestDto>();
        }

        public int Id { get; set; }

        [Required]
        public string Version { get; set; }

        [Required]
        public string ProductName { get; set; }


        public DateTime? BuildDate { get; set; }


        public virtual UserDto BuildPerson { get; set; }


        public virtual UserDto UpdatePerson { get; set; }


        public string BuildNumber { get; set; }

        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        public Platform Platform { get; set; }

        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        public BuildType Type { get; set; }

        public DateTime? LastUpdate { get; set; }

        public virtual ICollection<TestDto> Tests { get; set; }
    }
}
