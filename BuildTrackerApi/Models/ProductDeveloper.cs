using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuildTrackerApi.Models
{
    public partial class ProductDeveloper
    {
        public int ProductId { get; set; }
        public int DeveloperId { get;  set; }

        [ForeignKey("DeveloperId")]
        public virtual User Developer { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get;  set; }
    }
}
