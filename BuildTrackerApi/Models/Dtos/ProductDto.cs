using System.Collections.Generic;

namespace BuildTrackerApi.Models.Dtos
{
    public partial class ProductDto
    {

        public ProductDto()
        {
            Builds = new HashSet<BuildDto>();
            ProductDevelopers = new HashSet<ProductDeveloperDto>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<BuildDto> Builds { get; set; }
        public virtual ICollection<ProductDeveloperDto> ProductDevelopers { get; set; }
    }
}