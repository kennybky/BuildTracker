using System.Collections.Generic;

namespace BuildTrackerMobile.Models
{
    public partial class Product
    {

        public Product()
        {
            Builds = new HashSet<Build>();
            ProductDevelopers = new HashSet<ProductDeveloper>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Build> Builds { get; set; }
        public virtual ICollection<ProductDeveloper> ProductDevelopers { get; set; }
    }
}