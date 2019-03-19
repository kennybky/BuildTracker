using System;
using System.Collections.Generic;

namespace BuildTrackerApi.Models
{
    public partial class ProductDeveloper
    {
        public int ProductId { get; internal set; }
        public int DeveloperId { get; internal set; }

        public virtual User Developer { get; internal set; }
        public virtual Product Product { get; internal set; }
    }
}
