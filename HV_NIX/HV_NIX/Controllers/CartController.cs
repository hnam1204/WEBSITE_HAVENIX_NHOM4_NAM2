using HV_NIX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace HV_NIX.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext db = new AppDbContext();

        // Tạo Cart DB cho user
        private Cart GetOrCreateCart(int userId)
        {
            var cart = db.Carts.FirstOrDefault(c => c.UserID == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserID = userId,
                    CreatedAt = DateTime.Now
                };
                db.Carts.Add(cart);
                db.SaveChanges();
            }
            return cart;
        }

        // ==============================
        // ➕ THÊM SẢN PHẨM (CÓ SIZE)
        // ==============================
        [HttpPost]
        public JsonResult Add(int productId, string size, int quantity = 1)
        {
            size = (size ?? "").Trim().ToUpper();
            var product = db.Products.Find(productId);
            if (product == null)
                return Json(new { success = false });

            // ===============================
            // ▶ Guest Cart (Session)
            // ===============================
            if (Session["UserID"] == null)
            {
                var guest = Session["Cart"] as List<SessionCartItem> ?? new List<SessionCartItem>();

                var item = guest.FirstOrDefault(x => x.ProductID == productId && x.Size == size);

                if (item == null)
                {
                    guest.Add(new SessionCartItem
                    {
                        ProductID = productId,
                        Size = size,
                        Quantity = quantity,
                        Price = product.Price
                    });
                }
                else item.Quantity += quantity;

                Session["Cart"] = guest;

                return Json(new { success = true, totalItems = guest.Sum(x => x.Quantity) });
            }

            // ===============================
            // ▶ User Cart (Database)
            // ===============================
            int uid = (int)Session["UserID"];
            var cartDb = GetOrCreateCart(uid);

            var itemDb = db.CartItems.FirstOrDefault(
                x => x.CartID == cartDb.CartID &&
                     x.ProductID == productId &&
                     x.Size.ToUpper() == size
            );

            if (itemDb == null)
            {
                db.CartItems.Add(new CartItems
                {
                    CartID = cartDb.CartID,
                    ProductID = productId,
                    Size = size,
                    Quantity = quantity,
                    Price = product.Price
                });
            }
            else
            {
                itemDb.Quantity += quantity;
            }

            db.SaveChanges();

            int count = db.CartItems.Where(x => x.CartID == cartDb.CartID)
                                    .Sum(x => (int?)x.Quantity) ?? 0;

            return Json(new { success = true, totalItems = count });
        }

        // ==============================
        // 🛒 INDEX CART
        // ==============================
        public ActionResult Index()
        {
            // ============================
            // 🟦 KHÁCH VÃNG LAI (SESSION)
            // ============================
            if (Session["UserID"] == null)
            {
                var guest = Session["Cart"] as List<SessionCartItem> ?? new List<SessionCartItem>();

                if (!guest.Any())
                {
                    ViewBag.TotalAmount = 0;
                    return View("GuestCart", guest);
                }

                // Lấy danh sách ProductID
                var productIds = guest.Select(x => x.ProductID).Distinct().ToList();

                // Load toàn bộ product 1 lần → KHÔNG query trong View
                var products = db.Products
                                 .Where(p => productIds.Contains(p.ProductID))
                                 .ToDictionary(p => p.ProductID);

                ViewBag.Products = products;

                // Tổng tiền
                ViewBag.TotalAmount = guest.Sum(x => x.Price * x.Quantity);

                return View("GuestCart", guest);
            }

            // ============================
            // 🟩 USER ĐĂNG NHẬP
            // ============================
            int uid = (int)Session["UserID"];
            var cart = GetOrCreateCart(uid);

            var items = db.CartItems
                          .Include("Product")
                          .Where(x => x.CartID == cart.CartID)
                          .ToList();

            ViewBag.TotalAmount = items.Sum(x => x.Price * x.Quantity);

            return View(items);  // Trả về Views/Cart/Index.cshtml
        }




        // ==============================
        // 🔄 CẬP NHẬT SỐ LƯỢNG (AJAX)
        // ==============================
        [HttpPost]
        public JsonResult UpdateQuantity(int id, int quantity)
        {
            var item = db.CartItems.Find(id);

            if (item == null)
                return Json(new { success = false });

            if (quantity <= 0)
                quantity = 1;

            item.Quantity = quantity;
            db.SaveChanges();

            decimal itemTotal = item.Price * item.Quantity;

            decimal totalAmount = db.CartItems
                .Where(x => x.CartID == item.CartID)
                .Sum(x => x.Price * x.Quantity);

            return Json(new
            {
                success = true,
                itemTotal = itemTotal,
                totalAmount = totalAmount
            });
        }
        [HttpPost]
        public JsonResult UpdateQuantitySession(int productId, string size, int quantity)
        {
            var cart = Session["Cart"] as List<SessionCartItem> ?? new List<SessionCartItem>();

            var item = cart.FirstOrDefault(x => x.ProductID == productId && x.Size == size);

            if (item == null)
                return Json(new { success = false });

            if (quantity < 1) quantity = 1;

            item.Quantity = quantity;
            Session["Cart"] = cart;

            return Json(new
            {
                success = true,
                itemTotal = item.Price * item.Quantity,
                totalAmount = cart.Sum(x => x.Price * x.Quantity)
            });
        }
        [HttpPost]
        public JsonResult RemoveSession(int productId, string size)
        {
            var cart = Session["Cart"] as List<SessionCartItem> ?? new List<SessionCartItem>();

            var item = cart.FirstOrDefault(x => x.ProductID == productId && x.Size == size);
            if (item == null)
                return Json(new { success = false });

            cart.Remove(item);

            Session["Cart"] = cart;

            decimal totalAmount = cart.Sum(x => x.Price * x.Quantity);

            return Json(new
            {
                success = true,
                totalAmount = totalAmount
            });
        }
        public JsonResult GetCartCount()
        {
            // Guest
            if (Session["UserID"] == null)
            {
                var guest = Session["Cart"] as List<SessionCartItem> ?? new List<SessionCartItem>();
                int guestCount = guest.Sum(x => x.Quantity);
                return Json(new { totalItems = guestCount }, JsonRequestBehavior.AllowGet);
            }

            // User đã đăng nhập
            int uid = (int)Session["UserID"];
            var cart = GetOrCreateCart(uid);

            int count = db.CartItems
                          .Where(x => x.CartID == cart.CartID)
                          .Sum(x => (int?)x.Quantity) ?? 0;

            return Json(new { totalItems = count }, JsonRequestBehavior.AllowGet);
        }


        // ==============================
        // ❌ XÓA ITEM
        // ==============================
        public ActionResult Remove(int id)
        {
            var item = db.CartItems.Find(id);
            if (item != null)
            {
                db.CartItems.Remove(item);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // ==============================
        // 🧹 CLEAR CART
        // ==============================
        public ActionResult Clear()
        {
            if (Session["UserID"] == null)
            {
                Session["Cart"] = null;
                return RedirectToAction("Index");
            }

            int uid = (int)Session["UserID"];
            var cart = GetOrCreateCart(uid);

            var items = db.CartItems.Where(i => i.CartID == cart.CartID);

            db.CartItems.RemoveRange(items);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // ==============================
        // Checkout
        // ==============================
        public ActionResult Checkout()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Account");

            int uid = (int)Session["UserID"];
            var cart = GetOrCreateCart(uid);

            var items = db.CartItems.Where(x => x.CartID == cart.CartID).ToList();

            if (!items.Any())
                return RedirectToAction("Index");

            ViewBag.TotalAmount = items.Sum(x => x.Price * x.Quantity);
            ViewBag.Info = db.UserInfos.FirstOrDefault(x => x.UserID == uid);

            return View(items);
        }

        // ==============================
        // Submit Checkout
        // ==============================
        [HttpPost]
        public ActionResult SubmitCheckout(string paymentMethod,
                                           string FullName,
                                           string Email,
                                           string Phone,
                                           string Address,
                                           string District)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Account");

            int uid = (int)Session["UserID"];
            var cart = GetOrCreateCart(uid);

            var items = db.CartItems.Where(x => x.CartID == cart.CartID).ToList();

            if (!items.Any())
                return RedirectToAction("Index");

            // Cập nhật thông tin giao hàng
            var info = db.UserInfos.FirstOrDefault(x => x.UserID == uid);
            if (info == null)
            {
                info = new UserInfo
                {
                    UserID = uid,
                    FullName = FullName,
                    Email = Email,
                    Phone = Phone,
                    Address = Address,
                    District = District
                };
                db.UserInfos.Add(info);
            }
            else
            {
                info.FullName = FullName;
                info.Email = Email;
                info.Phone = Phone;
                info.Address = Address;
                info.District = District;
            }
            db.SaveChanges();

            // Tạo đơn hàng
            var order = new Orders
            {
                UserID = uid,
                OrderDate = DateTime.Now,
                Total = items.Sum(x => x.Price * x.Quantity),
                PaymentMethod = paymentMethod,
                Status = paymentMethod == "COD" ? "Pending" : "WaitingPayment"
            };

            db.Orders.Add(order);
            db.SaveChanges();

            // Lưu chi tiết đơn hàng có size
            foreach (var i in items)
            {
                db.OrderDetails.Add(new OrderDetails
                {
                    OrderID = order.OrderID,
                    ProductID = i.ProductID,
                    Size = i.Size,
                    Quantity = i.Quantity,
                    Price = i.Price
                });
            }

            db.SaveChanges();

            db.CartItems.RemoveRange(items);
            db.SaveChanges();

            if (paymentMethod == "COD")
                return RedirectToAction("ThankYou", new { id = order.OrderID });

            return RedirectToAction("PaymentQR", new { id = order.OrderID });
        }

        // ==============================
        // QR PAGE
        // ==============================
        public ActionResult PaymentQR(int id)
        {
            var order = db.Orders.Find(id);
            if (order == null) return RedirectToAction("Index");

            // Nếu đơn chưa có mã => tạo mới
            if (string.IsNullOrEmpty(order.PaymentCode))
            {
                string code = Guid.NewGuid().ToString().Substring(0, 6);
                order.PaymentCode = $"HAVENIX-{code}-ORDER{order.OrderID}";
                db.SaveChanges();
            }

            // Tạo QR
            string qrUrl =
                $"https://qr.sepay.vn/img?acc=338558120406&bank=MB&amount={order.Total}&des={order.PaymentCode}";

            ViewBag.OrderID = order.OrderID;
            ViewBag.Amount = order.Total;
            ViewBag.QR = qrUrl;
            ViewBag.Content = order.PaymentCode;

            return View(order);
        }


        // ============================
        // 🟢 KIỂM TRA TRẠNG THÁI ĐƠN
        // ============================
        public JsonResult CheckPaymentStatus(int id)
        {
            var order = db.Orders.Find(id);
            if (order == null)
                return Json(new { status = "NotFound" }, JsonRequestBehavior.AllowGet);

            return Json(new { status = order.Status }, JsonRequestBehavior.AllowGet);
        }


        // ============================
        // 🟢 XÁC NHẬN THANH TOÁN (WEBHOOK / AJAX)
        // ============================
        [HttpPost]
        public JsonResult ConfirmPayment(string content, decimal amount)
        {
            var order = db.Orders.FirstOrDefault(o => o.PaymentCode == content);

            if (order == null)
                return Json(new { success = false, message = "Không tìm thấy đơn theo mã nội dung" });

            // Đối chiếu số tiền (option)
            if (amount < order.Total)
                return Json(new { success = false, message = "Số tiền không khớp" });

            // Cập nhật đơn hàng
            order.Status = "Paid";
            order.PaidDate = DateTime.Now;  // ✔ sửa đúng tên property
            db.SaveChanges();

            return Json(new { success = true });
        }



        // THANK YOU PAGE
        public ActionResult ThankYou(int id)
        {
            var order = db.Orders.Find(id);
            return View(order);
        }
    }
}
