using System;
using System.ComponentModel.DataAnnotations;

namespace HV_NIX.Models
{
    public class ActivityLog
    {
        [Key]
        public int LogID { get; set; }

        public int? UserID { get; set; }

        [Required]
        [StringLength(500)]
        public string Action { get; set; }

        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Users User { get; set; }
    }
}
