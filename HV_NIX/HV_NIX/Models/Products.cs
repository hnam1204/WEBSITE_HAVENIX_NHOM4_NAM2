using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HV_NIX.Models
{
    public class Products
    {
        [Key]
        public int ProductID { get; set; }

        public string ProductName { get; set; }

        public int CategoryID { get; set; }
        public decimal Price { get; set; }

        public string Thumbnail { get; set; }
        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public string Image3 { get; set; }
        public string Description { get; set; }
        public int Stock { get; set; }

        [ForeignKey("CategoryID")]
        public virtual Category Category { get; set; }
        public virtual ICollection<Reviews> Reviews { get; set; }
    }
}