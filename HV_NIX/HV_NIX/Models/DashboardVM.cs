using HV_NIX.ViewModels;
using System;

namespace HV_NIX.Models
{
    public class DashboardVM
    {
        public int TotalUsers { get; set; }
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public decimal RevenueToday { get; set; }

        public decimal MaxRevenue { get; set; }
        public decimal MinRevenue { get; set; }

        public BestProductVM BestProduct { get; set; }
    }
}
