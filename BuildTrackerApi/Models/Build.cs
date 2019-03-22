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
        [Required]
        public  DateTime BuildDate { get; internal set; }
        [Required]
        public virtual User BuildPerson { get; internal set; }

        [Required]
        public virtual User UpdatePerson { get; internal set; }

        [Required]
        public string BuildNumber { get; internal set; }
        [Required]
        public string Platform { get; internal set; }
        [Required]
        public BuildType Type  { get; internal set; }
        [Required]
        public DateTime LastUpdate { get; internal set; }
    }

    public enum BuildType
    {
        TEST = 0, ALPHA = 1, BETA = 2, PRODUCTION = 3,
    }
}
