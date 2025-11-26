using HV_NIX.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace HV_NIX.Controllers
{
    public class AdminBankController : Controller
    {
        private readonly AppDbContext db = new AppDbContext();

        // ==============================
        // 📌 TRANG XÁC MINH THANH TOÁN QR
        // ==============================
        public ActionResult Index(DateTime? from, DateTime? to, decimal? min, decimal? max)
        {
            var query = db.Orders
                          .Include(o => o.User)        // Email
                          .Include(o => o.UserInfo)    // FullName + Phone
                          .Where(o => o.Status == "WaitingPayment")
                          .AsQueryable();

            // Lọc ngày
            if (from.HasValue)
                query = query.Where(x => x.OrderDate >= from.Value);

            if (to.HasValue)
                query = query.Where(x => x.OrderDate <= to.Value);

            // Lọc số tiền
            if (min.HasValue)
                query = query.Where(x => x.Total >= min.Value);

            if (max.HasValue)
                query = query.Where(x => x.Total <= max.Value);

            var result = query
                .OrderByDescending(x => x.OrderDate)
                .ToList();

            return View(result);
        }

        // ==============================
        // 📌 XÁC MINH ĐƠN QR
        // ==============================
        public ActionResult MarkPaid(int id)
        {
            var order = db.Orders.Find(id);
            if (order == null) return HttpNotFound();

            order.Status = "Paid";
            order.PaidDate = DateTime.Now;
            db.SaveChanges();

            TempData["Success"] = $"Đã xác nhận thanh toán cho đơn hàng #{id}";
            return RedirectToAction("Index");
        }

        // ==============================
        // 📌 LỊCH SỬ XÁC MINH
        // ==============================
        public ActionResult History()
        {
            var paid = db.Orders
                .Include(o => o.User)
                .Include(o => o.UserInfo)
                .Where(x => x.Status == "Paid")
                .OrderByDescending(x => x.PaidDate)
                .Take(300)
                .ToList();

            return View(paid);
        }

        // ==============================
        // 📌 API ĐẾM ĐƠN CHỜ
        // ==============================
        public JsonResult NewOrderCount()
        {
            int waiting = db.Orders.Count(x => x.Status == "WaitingPayment");
            return Json(new { waiting }, JsonRequestBehavior.AllowGet);
        }
    }
}
