using HV_NIX.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace HV_NIX.Controllers
{
    public class AdminOrderController : Controller
    {
        private readonly AppDbContext db = new AppDbContext();

        // ==============================
        // 📌 DANH SÁCH ĐƠN HÀNG
        // ==============================
        public ActionResult Index(string status, DateTime? from, DateTime? to)
        {
            var query = db.Orders
                          .Include("User")
                          .Include("Details.Product")
                          .AsQueryable();

            // Lọc trạng thái
            if (!String.IsNullOrEmpty(status))
                query = query.Where(x => x.Status == status);

            // Lọc ngày
            if (from.HasValue)
                query = query.Where(x =>
                    DbFunctions.TruncateTime(x.OrderDate) >= DbFunctions.TruncateTime(from));

            if (to.HasValue)
                query = query.Where(x =>
                    DbFunctions.TruncateTime(x.OrderDate) <= DbFunctions.TruncateTime(to));

            var list = query.OrderByDescending(x => x.OrderDate).ToList();

            return View(list);
        }

        // ==============================
        // 📌 CHI TIẾT ĐƠN HÀNG
        // ==============================
        public ActionResult Details(int id)
        {
            var order = db.Orders
                          .Include("User")
                          .Include("Details.Product")
                          .FirstOrDefault(x => x.OrderID == id);

            if (order == null)
                return HttpNotFound();

            return View(order);
        }

        // ==============================
        // 📌 CẬP NHẬT TRẠNG THÁI
        // ==============================
        [HttpPost]
        public ActionResult UpdateStatus(int id, string status)
        {
            var order = db.Orders.Find(id);
            if (order == null)
                return HttpNotFound();

            order.Status = status;

            if (status == "Paid" && !order.PaidDate.HasValue)
                order.PaidDate = DateTime.Now;

            db.SaveChanges();

            TempData["Success"] = "Cập nhật trạng thái thành công!";
            return RedirectToAction("Details", new { id });
        }

        // ==============================
        // 📌 XÓA ĐƠN HÀNG
        // ==============================
        public ActionResult Delete(int id)
        {
            var order = db.Orders
                .Include("Details")
                .FirstOrDefault(x => x.OrderID == id);

            if (order == null)
                return HttpNotFound();

            db.OrderDetails.RemoveRange(order.Details);
            db.Orders.Remove(order);
            db.SaveChanges();

            TempData["Success"] = "Đã xóa đơn thành công!";
            return RedirectToAction("Index");
        }

        // ==============================
        // 📌 API THỐNG KÊ
        // ==============================
        public JsonResult Stats()
        {
            var nowVN = DateTime.UtcNow.AddHours(7).Date;

            var data = new
            {
                totalOrders = db.Orders.Count(),
                paidOrders = db.Orders.Count(x => x.Status == "Paid"),
                pendingOrders = db.Orders.Count(x => x.Status == "Pending" || x.Status == "WaitingPayment"),
                todayRevenue = db.Orders
                    .Where(x => DbFunctions.TruncateTime(x.PaidDate) == nowVN)
                    .Sum(x => (decimal?)x.Total) ?? 0
            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
