using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HV_NIX.Models
{
    public class Orders
    {
        [Key]
        public int OrderID { get; set; }

        public int UserID { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal Total { get; set; }

        public string PaymentMethod { get; set; }

        public DateTime? PaidDate { get; set; }

        public string Status { get; set; }

        public string PaymentCode { get; set; }

        // ===== NAVIGATION PROPERTIES =====

        // ✔ Lấy Email từ bảng Users
        public virtual Users User { get; set; }

        // ✔ THÊM DÒNG NÀY — lấy FullName, Phone, Address...
        public virtual UserInfo UserInfo { get; set; }

        public virtual ICollection<OrderDetails> Details { get; set; }

        public virtual ICollection<Reviews> Reviews { get; set; }
    }
}
