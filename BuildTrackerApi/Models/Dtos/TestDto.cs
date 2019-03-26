using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BuildTrackerApi.Models.Dtos
{
    public partial class TestDto
    {
        public int Id { get; set; }
        
        public virtual UserDto TestPerson { get; set; }
        
        public virtual BuildDto Build { get; set; }

        public string Comments { get; set; }

        public DateTime? TestDate { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Platform Platform { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public TestType Type { get; set; }
    }
}