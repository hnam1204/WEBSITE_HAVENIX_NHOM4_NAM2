using System.ComponentModel.DataAnnotations;

namespace HV_NIX.Models
{
    public class Location_Ward
    {
        [Key]
        public int WardID { get; set; }
        public int DistrictID { get; set; }
        public string Name { get; set; }
    }
}
