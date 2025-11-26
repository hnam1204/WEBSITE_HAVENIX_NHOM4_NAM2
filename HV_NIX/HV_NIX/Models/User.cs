using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HV_NIX.Models
{
    public class Users
    {
        [Key]
        public int UserID { get; set; }

        [Required, MaxLength(150)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }
        public string Role { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // ======================
        // 🔹 Navigation Properties
        // ======================

        // 1-1 UserInfo
        public virtual UserInfo UserInfo { get; set; }

        // 1-n Orders
        public virtual ICollection<Orders> Orders { get; set; }

        // 1-n Notifications
        public virtual ICollection<Notification> Notifications { get; set; }

        // 1-n PasswordReset
        public virtual ICollection<PasswordReset> PasswordResets { get; set; }

        // 1-n Reviews
        public virtual ICollection<Reviews> Reviews { get; set; }

        // 1-n ShippingAddress
        public virtual ICollection<ShippingAddress> ShippingAddresses { get; set; }

        // 1-n Cart (nếu 1 user có nhiều cart—nhưng hệ thống bạn dùng 1 user = 1 cart)
        public virtual ICollection<Cart> Carts { get; set; }
    }
}
