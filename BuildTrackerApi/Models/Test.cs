﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BuildTrackerApi.Models
{
    public partial class Test
    {


        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public virtual User TestPerson { get; set; }

        [Required]
        public virtual Build Build { get; set; }

        public string Comments { get; set; }

        
        public DateTime? TestDate { get; set; }

        [Required]
        public Platform Platform { get; set; }
        
        [Required]
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
