using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BuildTrackerMobile.Models
{
    public partial class Test
    {
        public int Id { get; set; }
        
        public virtual User TestPerson { get; set; }
        
        public virtual Build Build { get; set; }

        public string Comments { get; set; }

        public DateTime? TestDate { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Platform Platform { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public TestType Type { get; set; }
    }

    public enum TestType
    {
        UnitTest = 0,
        Developer = 1,
        Acceptance = 2,
        System = 3,
        Interface = 4,
        Beta = 5,
        Integration = 6
    }
}