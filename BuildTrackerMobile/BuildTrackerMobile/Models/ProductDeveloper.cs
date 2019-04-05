using System;

namespace BuildTrackerMobile.Models
{
    public partial class ProductDeveloper
    {
       
        public int ProductId { get; set; }
       
        public int DeveloperId { get; set; }

        
        public virtual User Developer { get; set; }
       
        public virtual Product Product { get; set; }
    }
}
