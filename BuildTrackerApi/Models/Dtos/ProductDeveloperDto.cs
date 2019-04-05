using System.ComponentModel.DataAnnotations;

namespace BuildTrackerApi.Models.Dtos
{
    public partial class ProductDeveloperDto
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int DeveloperId { get; set; }

        
        public virtual UserDto Developer { get; set; }
       
        public virtual ProductDto Product { get; set; }
    }
}
