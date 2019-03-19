using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuildTrackerApi.Models
{
    public partial class Product
    {
        public Product()
        {
            Builds = new HashSet<Build>();
            ProductDevelopers = new HashSet<ProductDeveloper>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; internal set; }
        public string Name { get; internal set; }

        public virtual ICollection<Build> Builds { get; internal set; }
        public virtual ICollection<ProductDeveloper> ProductDevelopers { get; internal set; }
    }
}
