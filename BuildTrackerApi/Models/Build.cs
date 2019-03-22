using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuildTrackerApi.Models
{
    public partial class Build
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Version { get; internal set; }

        public string ProductName { get; set; }
        [Required]
        public virtual Product Product { get; internal set; }
       
        public  DateTime? BuildDate { get; internal set; }
        [Required]
        public virtual User BuildPerson { get; internal set; }

        [Required]
        public virtual User UpdatePerson { get; internal set; }

        [Required]
        public string BuildNumber { get; internal set; }

        [Required]
        public Platform Platform { get; internal set; }

       
        [Required]
        public BuildType Type  { get; internal set; }
       
        public DateTime? LastUpdate { get; internal set; }

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
