using BCrypt.Net;
using HV_NIX.Helpers;
using HV_NIX.Models;
using HV_NIX.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;

namespace HV_NIX.Controllers
{
    [AdminAuthorize]   // ⬅ CHẶN NGƯỜI DÙNG / USER KHÔNG CHO VÀO ADMIN
    public class AdminController : Controller
    {
        AppDbContext db = new AppDbContext();

        // ====================================
        // 📌 DASHBOARD
        // ====================================
        public ActionResult Dashboard()
        {
            DashboardVM vm = new DashboardVM();

            vm.TotalUsers = db.Users.Count();
            vm.TotalProducts = db.Products.Count();
            vm.TotalOrders = db.Orders.Count();

            vm.RevenueToday = db.Orders
                .Where(o => o.OrderDate >= DateTime.Today)
                .Sum(o => (decimal?)o.Total) ?? 0;

            vm.MaxRevenue = db.Orders.Max(o => (decimal?)o.Total) ?? 0;
            vm.MinRevenue = db.Orders.Min(o => (decimal?)o.Total) ?? 0;

            vm.BestProduct = db.OrderDetails
                .GroupBy(x => x.ProductID)
                .Select(g => new BestProductVM
                {
                    ProductID = g.Key,
                    Sold = g.Sum(x => x.Quantity),
                    ProductName = g.FirstOrDefault().Product.ProductName,
                    Thumbnail = g.FirstOrDefault().Product.Thumbnail
                })
                .OrderByDescending(x => x.Sold)
                .FirstOrDefault();

            return View(vm);
        }


        // ====================================
        // 📌 PRODUCTS
        // ====================================
        public ActionResult Products()
        {
            return RedirectToAction("Index", "AdminProduct");
        }

        public ActionResult CreateProduct()
        {
            ViewBag.Categories = db.Categories.ToList();
            return View();
        }

        [HttpPost]
        public ActionResult CreateProduct(Products p)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = db.Categories.ToList();
                return View(p);
            }

            db.Products.Add(p);
            db.SaveChanges();

            LogHelper.AddLog(db, Convert.ToInt32(Session["AdminID"]),
                "AddProduct", $"Thêm sản phẩm: {p.ProductName}");

            return RedirectToAction("Products");
        }

        public ActionResult EditProduct(int id)
        {
            var p = db.Products.Find(id);
            if (p == null) return HttpNotFound();

            ViewBag.Categories = db.Categories.ToList();
            return View(p);
        }

        [HttpPost]
        public ActionResult EditProduct(Products p)
        {
            db.Entry(p).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            LogHelper.AddLog(db, Convert.ToInt32(Session["AdminID"]),
                "EditProduct", $"Sửa sản phẩm: {p.ProductName}");

            return RedirectToAction("Products");
        }

        public ActionResult DeleteProduct(int id)
        {
            var p = db.Products.Find(id);
            if (p == null) return HttpNotFound();

            db.Products.Remove(p);
            db.SaveChanges();

            LogHelper.AddLog(db, Convert.ToInt32(Session["AdminID"]),
                "DeleteProduct", $"Xoá sản phẩm: {p.ProductName}");

            return RedirectToAction("Products");
        }


        // ====================================
        // 📌 CUSTOMERS
        // ====================================
        public ActionResult Customers()
        {
            return RedirectToAction("Index", "AdminCustomer");
        }


        // ====================================
        // 📌 ADMIN ACCOUNTS
        // ====================================
        public ActionResult Accounts()
        {
            return View(db.Users.OrderBy(u => u.UserID).ToList());
        }

        public ActionResult CreateAccount()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateAccount(Users u)
        {
            if (!u.Email.ToLower().EndsWith("@admin.vn"))
                u.Email = u.Email + "@admin.vn";

            u.Role = "Admin";
            u.CreatedAt = DateTime.Now;

            // Lưu mật khẩu dạng thường
            u.PasswordHash = u.PasswordHash;

            db.Users.Add(u);
            db.SaveChanges();

            TempData["Success"] = "Tạo admin thành công!";
            return RedirectToAction("Accounts");
        }

        public ActionResult EditAccount(int id)
        {
            var u = db.Users.Find(id);
            return View(u);
        }

        [HttpPost]
        public ActionResult EditAccount(Users model)
        {
            var u = db.Users.Find(model.UserID);
            if (u == null) return HttpNotFound();

            int adminId = Convert.ToInt32(Session["AdminID"]);

            // ❗ NGĂN ADMIN SỬA CHÍNH MÌNH
            if (u.UserID == adminId)
            {
                TempData["Error"] = "❌ Bạn không thể chỉnh sửa tài khoản đang đăng nhập!";
                return RedirectToAction("Accounts");
            }

            // Cập nhật email & quyền
            u.Email = model.Email;
            u.Role = model.Role;

            // Nếu có nhập mật khẩu mới → lưu plaintext
            if (!string.IsNullOrEmpty(model.PasswordHash))
                u.PasswordHash = model.PasswordHash;

            db.SaveChanges();

            TempData["Success"] = "✔ Cập nhật tài khoản thành công!";
            return RedirectToAction("Accounts");
        }
        // ====================================
        // 📌 DELETE ADMIN
        // ====================================
        public ActionResult DeleteAccount(int id)
        {
            var user = db.Users.Find(id);
            if (user == null) return HttpNotFound();

            // ❗ Không cho xóa admin cấp cao (tuỳ bạn giữ lại)
            if (user.Role == "Admin")
            {
                TempData["Error"] = "Không thể xóa Admin tại mục này!";
                return RedirectToAction("Accounts");
            }

            // ❗ Kiểm tra có đơn hàng
            bool hasOrders = db.Orders.Any(o => o.UserID == id);

            if (hasOrders)
            {
                TempData["Error"] = "❌ Không thể xóa! Tài khoản này đã có đơn hàng.";
                return RedirectToAction("Accounts");
            }

            // ❗ Xóa UserInfo (nếu có)
            var info = db.UserInfos.FirstOrDefault(x => x.UserID == id);
            if (info != null)
                db.UserInfos.Remove(info);

            // ❗ Xóa tài khoản
            db.Users.Remove(user);
            db.SaveChanges();

            TempData["Success"] = "✔ Xóa tài khoản thành công!";
            return RedirectToAction("Accounts");
        }


        [HttpPost]
        public ActionResult ConfirmDeleteAdmin(AdminCheckVM form)
        {
            var deletingUser = db.Users.Find(form.DeleteUserID);
            if (deletingUser == null) return HttpNotFound();

            var superAdmins = db.Users
                .Where(u => u.Role == "Admin")
                .OrderBy(u => u.UserID)
                .Take(3)
                .ToList();

            bool passwordCorrect = superAdmins.Any(
                admin => BCrypt.Net.BCrypt.Verify(form.ConfirmPassword, admin.PasswordHash)
            );

            if (!passwordCorrect)
            {
                TempData["Error"] = "Sai mật khẩu admin cấp cao!";
                return RedirectToAction("DeleteAccount", new { id = form.DeleteUserID });
            }

            db.Users.Remove(deletingUser);
            db.SaveChanges();

            LogHelper.AddLog(db, Convert.ToInt32(Session["AdminID"]),
                "DeleteAdmin", $"Xoá admin: {deletingUser.Email}");

            TempData["Success"] = "Đã xoá tài khoản admin!";
            return RedirectToAction("Accounts");
        }

        // ====================================
        // 📌 ORDERS
        // ====================================
        public ActionResult Orders()
        {
            return View(db.Orders.ToList());
        }

        public ActionResult OrderDetails(int id)
        {
            var details = db.OrderDetails
                .Where(x => x.OrderID == id)
                .Select(x => new
                {
                    x.OrderDetailID,
                    x.ProductID,
                    x.Product.ProductName,
                    x.Product.Thumbnail,
                    x.Quantity,
                    x.Price
                }).ToList();

            ViewBag.OrderID = id;

            return View("~/Views/AdminOrder/Details.cshtml", details);
        }


        // ====================================
        // 📌 ACTIVITY LOGS
        // ====================================
        public ActionResult Logs(string type)
        {
            var logs = db.ActivityLogs.OrderByDescending(x => x.CreatedAt);

            if (!string.IsNullOrEmpty(type))
                logs = logs.Where(x => x.Action == type)
                           .OrderByDescending(x => x.CreatedAt);

            ViewBag.ActionTypes = db.ActivityLogs
                                    .Select(x => x.Action)
                                    .Distinct()
                                    .OrderBy(x => x)
                                    .ToList();

            ViewBag.SelectedType = type;

            return View(logs.ToList());
        }
        // ===============================================
        // 📌 API: Doanh thu 7 ngày gần nhất
        // ===============================================
        public JsonResult RevenueLast7Days()
        {
            var data = db.Orders
                .GroupBy(o => DbFunctions.TruncateTime(o.OrderDate))
                .Select(g => new
                {
                    Date = g.Key,
                    Total = g.Sum(x => x.Total)
                })
                .OrderBy(x => x.Date)
                .Take(7)
                .ToList()
                .Select(x => new {
                    Date = x.Date.Value.ToString("dd/MM"),
                    Total = x.Total
                });

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult OrdersLast7Days()
        {
            var today = DateTime.Today;

            var data = Enumerable.Range(0, 7)
                .Select(i => today.AddDays(-i))
                .Select(date => new
                {
                    Date = date.ToString("dd/MM"),
                    Count = db.Orders
                        .Count(o => DbFunctions.TruncateTime(o.OrderDate) == date)
                })
                .OrderBy(x => DateTime.ParseExact(x.Date, "dd/MM", null))
                .ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult TopProductsLast7Days()
        {
            var today = DateTime.Today;

            var list = new List<object>();

            for (int i = 0; i < 7; i++)
            {
                var date = today.AddDays(-i);

                var products = db.OrderDetails
                    .Where(od => DbFunctions.TruncateTime(od.Order.OrderDate) == date)
                    .GroupBy(od => od.ProductID)
                    .Select(g => new
                    {
                        ProductID = g.Key,
                        Name = g.FirstOrDefault().Product.ProductName,
                        Sold = g.Sum(x => x.Quantity)
                    })
                    .OrderByDescending(x => x.Sold)
                    .Take(5)
                    .ToList();

                list.Add(new
                {
                    Date = date.ToString("dd/MM"),
                    Products = products
                });
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        // ====================================
        // 📌 NOTIFICATIONS
        // ====================================
        public ActionResult Notifications()
        {
            var list = db.Notifications
                .OrderByDescending(n => n.CreatedAt)
                .ToList();

            return View(list);
        }


        // ============================
        // 📌 GET – TẠO THÔNG BÁO
        // ============================
        public ActionResult CreateNotification()
        {
            ViewBag.Users = db.Users.ToList();   // LẤY TẤT CẢ USER
            return View();
        }

        // ============================
        // 📌 POST – GỬI THÔNG BÁO
        // ============================
        [HttpPost]
        public ActionResult CreateNotification(Notification n, string SendTo)
        {
            // Validate
            if (string.IsNullOrWhiteSpace(n.Title) || string.IsNullOrWhiteSpace(n.Message))
            {
                TempData["Error"] = "⚠️ Tiêu đề và nội dung không được để trống!";
                return RedirectToAction("CreateNotification");
            }

            // GỬI CHO TẤT CẢ
            if (SendTo == "All")
            {
                var users = db.Users.Where(u => u.Role == "User").ToList();

                foreach (var u in users)
                {
                    db.Notifications.Add(new Notification
                    {
                        UserID = u.UserID,
                        Title = n.Title,
                        Message = n.Message,
                        CreatedAt = DateTime.Now,
                        IsRead = false
                    });
                }

                db.SaveChanges();
                TempData["Success"] = "Đã gửi thông báo đến tất cả người dùng!";
                return RedirectToAction("Notifications");
            }

            // GỬI CHO 1 NGƯỜI
            if (n.UserID == null)
            {
                TempData["Error"] = "⚠️ Vui lòng chọn người dùng!";
                return RedirectToAction("CreateNotification");
            }

            bool exists = db.Users.Any(x => x.UserID == n.UserID);
            if (!exists)
            {
                TempData["Error"] = "⚠️ Người dùng không tồn tại!";
                return RedirectToAction("CreateNotification");
            }

            n.CreatedAt = DateTime.Now;
            n.IsRead = false;

            db.Notifications.Add(n);
            db.SaveChanges();

            TempData["Success"] = "🎉 Đã gửi thông báo!";
            return RedirectToAction("Notifications");
        }

        public ActionResult Logout()
        {
            // Xóa toàn bộ session admin
            Session["AdminID"] = null;
            Session["AdminName"] = null;

            TempData["Success"] = "Đăng xuất thành công!";

            // Chuyển về trang đăng nhập Admin
            return RedirectToAction("Login", "Account");
        }
        public ActionResult Reviews()
        {
            var list = db.Reviews
                .OrderByDescending(r => r.ReviewDate)
                .ToList();

            return View(list);
        }
        [HttpPost]
        public ActionResult ReplyReview(int id, string reply)
        {
            var rv = db.Reviews.Find(id);
            if (rv == null)
                return HttpNotFound();

            rv.AdminReply = reply;
            db.SaveChanges();

            TempData["Success"] = "Đã trả lời đánh giá.";
            return RedirectToAction("Reviews");
        }
        public ActionResult DeleteReview(int id)
        {
            var rv = db.Reviews.Find(id);
            if (rv == null)
                return HttpNotFound();

            db.Reviews.Remove(rv);
            db.SaveChanges();

            TempData["Success"] = "Đã xóa đánh giá.";
            return RedirectToAction("Reviews");
        }

    }
}
