using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HV_NIX.Models;

namespace HV_NIX.Models
{
        public class CustomerStatsVM
        {
            public int UserID { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }

            // Tổng số đơn
            public int TotalOrders { get; set; }

            // Tổng tiền khách đã mua
            public decimal TotalSpent { get; set; }
        }
    }
