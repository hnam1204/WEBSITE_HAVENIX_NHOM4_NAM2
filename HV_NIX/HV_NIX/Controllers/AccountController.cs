using HV_NIX.Filters;
using HV_NIX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace HV_NIX.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext db = new AppDbContext();

        // ============================
        // ✔ REGISTER (GET)
        // ============================
        [HttpGet]
        public ActionResult Register()
        {
            if (Session["UserID"] != null)
                return RedirectToAction("Index", "Home");

            return View();
        }

        // ============================
        // ✔ REGISTER (POST)
        // ============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(string fullname, string phone, string address,
                                    string email, string password, string confirmPassword)
        {
            if (string.IsNullOrEmpty(fullname) || string.IsNullOrEmpty(phone) ||
                string.IsNullOrEmpty(address) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "⚠️ Vui lòng nhập đầy đủ thông tin.";
                return View();
            }

            if (password != confirmPassword)
            {
                ViewBag.Error = "⚠️ Mật khẩu xác nhận không khớp.";
                return View();
            }

            if (db.Users.Any(u => u.Email == email))
            {
                ViewBag.Error = "⚠️ Email đã tồn tại.";
                return View();
            }

            // Tạo user
            var user = new Users
            {
                Email = email,
                PasswordHash = password,
                CreatedAt = DateTime.Now,
                Role = "User"
            };
            db.Users.Add(user);
            db.SaveChanges();

            // Tạo UserInfo
            var info = new UserInfo
            {
                UserID = user.UserID,
                FullName = fullname,
                Phone = phone,
                Address = address
            };
            db.UserInfos.Add(info);
            db.SaveChanges();

            TempData["Success"] = " Chào Mừng Đến Với Cửa Hàng Quần Áo";

            // 👉 CHUYỂN ĐẾN TRANG LOGIN THAY VÌ VÀO HOME
            return RedirectToAction("Login", "Account");
        }

        // ============================
        // ✔ LOGIN (GET)
        // ============================
        [HttpGet]
        public ActionResult Login()
        {
            if (Session["UserID"] != null)
                return RedirectToAction("Index", "Home");

            return View();
        }
        // ============================
        // ✔ LOGIN (POST)
        // ============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            var user = db.Users.FirstOrDefault(u => u.Email == email && u.PasswordHash == password);
            if (user == null)
            {
                ViewBag.Error = "❌ Sai email hoặc mật khẩu.";
                return View();
            }

            // ===========================
            // ✔ Nếu là ADMIN → Vào ADMIN Dashboard luôn
            // ===========================
            if (user.Role == "Admin")
            {
                Session["AdminID"] = user.UserID;
                Session["AdminName"] = user.Email;

                return RedirectToAction("Dashboard", "Admin");
            }

            // ===========================
            // ✔ Nếu là USER → login bình thường
            // ===========================
            Session["UserID"] = user.UserID;
            Session["UserName"] = db.UserInfos.FirstOrDefault(i => i.UserID == user.UserID)?.FullName;

            // MERGE GIỎ HÀNG (giữ nguyên code cũ của bạn)
            var guestCart = Session["Cart"] as List<SessionCartItem>;
            if (guestCart != null && guestCart.Any())
            {
                var cartDb = db.Carts.FirstOrDefault(c => c.UserID == user.UserID);
                if (cartDb == null)
                {
                    cartDb = new Cart
                    {
                        UserID = user.UserID,
                        CreatedAt = DateTime.Now
                    };
                    db.Carts.Add(cartDb);
                    db.SaveChanges();
                }

                foreach (var g in guestCart)
                {
                    var itemDb = db.CartItems.FirstOrDefault(x =>
                        x.CartID == cartDb.CartID &&
                        x.ProductID == g.ProductID &&
                        x.Size == g.Size
                    );

                    if (itemDb == null)
                    {
                        db.CartItems.Add(new CartItems
                        {
                            CartID = cartDb.CartID,
                            ProductID = g.ProductID,
                            Quantity = g.Quantity,
                            Price = g.Price,
                            Size = g.Size
                        });
                    }
                    else itemDb.Quantity += g.Quantity;
                }

                db.SaveChanges();
                Session["Cart"] = null;
            }

            return RedirectToAction("Index", "Home");
        }




        // ============================
        // ✔ LOGOUT
        // ============================
        public ActionResult Logout()
        {
            Session["UserID"] = null;
            Session["UserRole"] = null;
            Session["UserName"] = null;

            return RedirectToAction("Index", "Home");
        }


        // ============================
        // ✔ CHECK LOGIN (AJAX)
        // ============================
        [HttpGet]
        public ActionResult CheckLogin()
        {
            return Json(new { isLoggedIn = Session["UserID"] != null }, JsonRequestBehavior.AllowGet);

        }

        // ============================
        // ✔ MY PROFILE (GET)
        // ============================
        [UserAuthorize]
        [HttpGet]
        public ActionResult MyProfile()
        {
            int uid = (int)Session["UserID"];

            var info = db.UserInfos.FirstOrDefault(i => i.UserID == uid);

            if (info == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin người dùng.";
                return RedirectToAction("Index", "Home");
            }

            return View(info);
        }

        // ============================
        // ✔ MY PROFILE (POST)
        // ============================
        [UserAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MyProfile(UserInfo model)
        {
            int uid = (int)Session["UserID"];

            var info = db.UserInfos.FirstOrDefault(i => i.UserID == uid);

            if (info == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin người dùng.";
                return RedirectToAction("Index", "Home");
            }

            // Cập nhật
            info.FullName = model.FullName;
            info.Phone = model.Phone;
            info.Address = model.Address;

            db.SaveChanges();

            TempData["Success"] = "Cập nhật hồ sơ thành công!";
            return RedirectToAction("MyProfile");
        }


        // ============================
        // ✔ LẤY LỊCH SỬ ĐƠN HÀNG
        // ============================
        [HttpGet]
        public ActionResult OrderHistory()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login");

            int uid = Convert.ToInt32(Session["UserID"]);

            var orders = db.Orders
                .Where(o => o.UserID == uid)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(orders);
        }
        // ============================
        // ✔ TRANG CHI TIẾT ĐƠN HÀNG (FULL PAGE)
        // ============================
        public ActionResult OrderDetail(int id)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login");

            int uid = (int)Session["UserID"];

            var order = db.Orders.FirstOrDefault(o => o.OrderID == id && o.UserID == uid);
            if (order == null)
                return HttpNotFound();

            var details = db.OrderDetails
                .Where(d => d.OrderID == id)
                .ToList();

            ViewBag.Details = details;
            ViewBag.Info = db.UserInfos.FirstOrDefault(i => i.UserID == uid);

            return View(order);
        }

        // ============================
        // ✔ LẤY CHI TIẾT ĐƠN HÀNG + REVIEW
        // ============================
        [HttpGet]
        public JsonResult GetOrderDetail(int orderId)
        {
            if (Session["UserID"] == null)
                return Json(new { error = "NotLogin" }, JsonRequestBehavior.AllowGet);

            int uid = Convert.ToInt32(Session["UserID"]);

            var order = db.Orders.FirstOrDefault(o => o.OrderID == orderId && o.UserID == uid);
            if (order == null)
                return Json(new { error = "Không tìm thấy đơn hàng" }, JsonRequestBehavior.AllowGet);

            var details = db.OrderDetails
                .Where(d => d.OrderID == orderId)
                .Select(d => new
                {
                    d.ProductID,
                    d.Size,                               
                    ProductName = d.Product.ProductName,
                    ImageURL = d.Product.Thumbnail,
                    d.Quantity,
                    d.Price,
                    HasReview = db.Reviews.Any(r =>
                        r.ProductID == d.ProductID &&
                        r.OrderID == orderId &&
                        r.UserID == uid)
                })
                .ToList();

            return Json(new
            {
                order.OrderID,
                Date = order.OrderDate.ToString("dd/MM/yyyy HH:mm"),
                Total = order.Total,
                order.Status,
                Details = details
            }, JsonRequestBehavior.AllowGet);
        }
        // ============================
        // ✔ GỬI REVIEW
        // ============================
        [HttpPost]
        [ValidateAntiForgeryToken] // BẮT BUỘC
        public JsonResult SubmitReview(int ProductID, int OrderID, int Rating, string Comment)
        {
            if (Session["UserID"] == null)
                return Json(new { success = false, message = "Vui lòng đăng nhập" });

            int uid = Convert.ToInt32(Session["UserID"]);

            // Kiểm tra đơn có thuộc user không
            var order = db.Orders.FirstOrDefault(o => o.OrderID == OrderID && o.UserID == uid);
            if (order == null)
                return Json(new { success = false, message = "Bạn không có quyền đánh giá đơn này." });

            // Chỉ cho phép Paid hoặc Completed
            if (order.Status != "Completed" && order.Status != "Paid")
                return Json(new { success = false, message = "Đơn hàng chưa hoàn thành!" });

            // Sản phẩm không nằm trong đơn
            bool isInOrder = db.OrderDetails.Any(od => od.OrderID == OrderID && od.ProductID == ProductID);
            if (!isInOrder)
                return Json(new { success = false, message = "Sản phẩm không thuộc đơn!" });

            // Đã đánh giá?
            bool existed = db.Reviews.Any(r =>
                r.ProductID == ProductID &&
                r.OrderID == OrderID &&
                r.UserID == uid
            );
            if (existed)
                return Json(new { success = false, message = "Bạn đã đánh giá sản phẩm này rồi!" });

            if (Rating < 1 || Rating > 5)
                return Json(new { success = false, message = "Số sao không hợp lệ!" });

            // Lưu review
            db.Reviews.Add(new Reviews
            {
                ProductID = ProductID,
                OrderID = OrderID,
                UserID = uid,
                Rating = Rating,
                Comment = Comment ?? "",
                ReviewDate = DateTime.Now
            });

            db.SaveChanges();

            return Json(new { success = true });
        }
        // ============================
        // ⭐ TRANG ĐÁNH GIÁ ĐƠN HÀNG
        // ============================
        public ActionResult ReviewOrder(int id)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login");

            int uid = (int)Session["UserID"];

            // Lấy đơn hàng của người dùng
            var order = db.Orders
                .FirstOrDefault(o => o.OrderID == id && o.UserID == uid);

            if (order == null)
                return HttpNotFound();

            // Lấy danh sách sản phẩm trong đơn
            var items = db.OrderDetails
                .Where(d => d.OrderID == id)
                .ToList();

            // Tải sản phẩm theo ProductID
            foreach (var it in items)
            {
                it.Product = db.Products.FirstOrDefault(p => p.ProductID == it.ProductID);
            }

            // ⭐ THÊM: Lấy danh sách review mà user đã đánh giá
            var reviewed = db.Reviews
                .Where(r => r.OrderID == id && r.UserID == uid)
                .ToList();

            ViewBag.Items = items;
            ViewBag.Reviewed = reviewed;

            return View(order);
        }
        // ============================
        // ⭐ TRANG 'Đánh giá của tôi'
        // ============================
        public ActionResult MyReviews()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login");

            int uid = (int)Session["UserID"];

            var list = db.Reviews
     .Where(r => r.UserID == uid)
     .OrderByDescending(r => r.ReviewDate)
     .Select(r => new
     {
         r.ReviewID,
         r.ProductID,
         r.OrderID,
         r.Rating,
         r.Comment,
         r.ReviewDate,
         ProductName = r.Product.ProductName,
         Thumbnail = r.Product.Thumbnail
     })
     .ToList()
     .Select(r => new Reviews
     {
         ReviewID = r.ReviewID,
         ProductID = r.ProductID,
         OrderID = r.OrderID,
         Rating = r.Rating,
         Comment = r.Comment,
         ReviewDate = r.ReviewDate,
         Product = new Products
         {
             ProductID = r.ProductID,
             ProductName = r.ProductName,
             Thumbnail = r.Thumbnail
         }
     })
     .ToList();


            // Load product cho từng review
            foreach (var r in list)
            {
                r.Product = db.Products.FirstOrDefault(p => p.ProductID == r.ProductID);
            }

            return View(list);
        }

        // ============================
        // 📌 DANH SÁCH THÔNG BÁO
        // ============================
        public ActionResult Notifications()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login");

            int uid = (int)Session["UserID"];

            var list = db.Notifications
                .Where(n => n.UserID == uid)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();

            return View(list);
        }

        // ============================
        // 📌 ĐÁNH DẤU ĐÃ ĐỌC
        // ============================
        public ActionResult MarkRead(int id)
        {
            var noti = db.Notifications.FirstOrDefault(n => n.NotificationID == id);

            if (noti != null)
            {
                noti.IsRead = true;
                db.SaveChanges();
            }

            return RedirectToAction("Notifications");
        }
    }
}
