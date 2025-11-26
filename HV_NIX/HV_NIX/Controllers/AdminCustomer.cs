using System;
using System.Linq;
using System.Web.Mvc;
using HV_NIX.Models;
using HV_NIX.ViewModels;

namespace HV_NIX.Controllers
{
    public class AdminCustomerController : Controller
    {
        private readonly AppDbContext db = new AppDbContext();

        public ActionResult Index(string sort = "desc")
        {
            var data = db.Users
                .Select(user => new CustomerStatsVM
                {
                    UserID = user.UserID,

                    // Lấy tên từ UserInfo (nếu có)
                    FullName = user.UserInfo != null
                               ? user.UserInfo.FullName
                               : "(Không tên)",

                    Email = user.Email,

                    // Tổng số đơn của khách
                    TotalOrders = db.Orders.Count(o => o.UserID == user.UserID),

                    // Tổng tiền khách đã mua
                    TotalSpent = db.Orders
                                   .Where(o => o.UserID == user.UserID)
                                   .Sum(o => (decimal?)o.Total) ?? 0
                })
                .ToList();

            // =============== Lọc theo yêu cầu ===============
            switch (sort)
            {
                case "asc":
                    data = data.OrderBy(x => x.TotalSpent).ToList();
                    break;

                default: // desc
                    data = data.OrderByDescending(x => x.TotalSpent).ToList();
                    break;
            }

            ViewBag.Sort = sort;
            return View("Index", data);
        }
    }
}
