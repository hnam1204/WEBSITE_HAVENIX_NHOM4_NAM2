using HV_NIX.Models;
using System.ComponentModel.DataAnnotations;
namespace HV_NIX.Models
{
    public class CartItems
    {
        [Key]
        public int CartItemID { get; set; }   // ONLY Identity

        public int CartID { get; set; }
        public int ProductID { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public virtual Cart Cart { get; set; }
        public virtual Products Product { get; set; }
    }
}