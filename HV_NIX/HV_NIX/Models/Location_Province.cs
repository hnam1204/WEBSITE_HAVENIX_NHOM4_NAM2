using System.ComponentModel.DataAnnotations;

namespace HV_NIX.Models
{
    public class Location_Province
    {
        [Key]
        public int ProvinceID { get; set; }
        public string Name { get; set; }
    }
}