using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace HV_NIX.Models   // <-- PHẢI CÓ DÒNG NÀY
{
    public class Notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NotificationID { get; set; }   // ✔ Chỉ identity cho cột này
        public int? UserID { get; set; }   // Cho phép NULL khi gửi cho tất cả
        public virtual Users User { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}