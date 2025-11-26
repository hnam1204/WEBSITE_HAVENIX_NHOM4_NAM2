using System.ComponentModel.DataAnnotations;

namespace HV_NIX.Models
{
    public class Location_District
    {
        [Key]
        public int DistrictID { get; set; }
        public int ProvinceID { get; set; }
        public string Name { get; set; }
    }
}
