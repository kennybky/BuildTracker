namespace BuildTrackerApi.Models.Dtos
{
    public partial class ProductDeveloperDto
    {
        public int ProductId { get; set; }
        public int DeveloperId { get; set; }

        
        public virtual UserDto Developer { get; set; }
       
        public virtual ProductDto Product { get; set; }
    }
}
