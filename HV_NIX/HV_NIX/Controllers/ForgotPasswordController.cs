using HV_NIX.Models;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;

namespace HV_NIX.Controllers
{
    public class ForgotPasswordController : Controller
    {
        private readonly AppDbContext db = new AppDbContext();

        // ================================
        // 1️⃣ GIAO DIỆN NHẬP EMAIL
        // ================================
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        // ================================
        // 2️⃣ NHẬN EMAIL & GỬI MÃ KHÔI PHỤC
        // ================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string email)
        {
            var user = db.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                ViewBag.Error = " Email không tồn tại trong hệ thống.";
                return View();
            }

            // Tạo mã reset
            string code = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();

            // Lưu vào PasswordResets
            db.PasswordResets.Add(new PasswordReset
            {
                UserID = user.UserID,
                ResetCode = code,
                ExpiredAt = DateTime.Now.AddMinutes(10), // hết hạn sau 10 phút
                Used = false
            });
            db.SaveChanges();

            // Gửi email
            SendEmail(
     email,
     "Mã khôi phục mật khẩu - Havenix",
 $@"
HAVENIX_STORE

Mã khôi phục của bạn:{code}
Mã có hiệu lực trong vòng 10 phút, vui lòng không chia sẻ mã này cho bất kỳ ai.

───────────────────────────────
"
 );
            TempData["Success"] = " Mã khôi phục đã được gửi vào email!";
            return RedirectToAction("Verify");
        }

        // ================================
        // 3️⃣ XÁC NHẬN MÃ
        // ================================
        [HttpGet]
        public ActionResult Verify()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Verify(string code)
        {
            var reset = db.PasswordResets
                .Where(r => r.ResetCode == code && !r.Used)
                .OrderByDescending(r => r.PasswordResetID)
                .FirstOrDefault();

            if (reset == null)
            {
                ViewBag.Error = " Mã không hợp lệ.";
                return View();
            }

            if (reset.ExpiredAt < DateTime.Now)
            {
                ViewBag.Error = " Mã đã hết hạn.";
                return View();
            }

            Session["ResetUserID"] = reset.UserID;
            reset.Used = true;
            db.SaveChanges();

            return RedirectToAction("ResetPassword");
        }

        // ================================
        // 4️⃣ NHẬP MẬT KHẨU MỚI
        // ================================
        [HttpGet]
        public ActionResult ResetPassword()
        {
            if (Session["ResetUserID"] == null)
                return RedirectToAction("Index");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(string password, string confirmPassword)
        {
            if (Session["ResetUserID"] == null)
                return RedirectToAction("Index");

            if (password != confirmPassword)
            {
                ViewBag.Error = " Mật khẩu xác nhận không trùng khớp!";
                return View();
            }

            int uid = (int)Session["ResetUserID"];
            var user = db.Users.FirstOrDefault(u => u.UserID == uid);

            if (user == null)
            {
                ViewBag.Error = "Không tìm thấy người dùng.";
                return View();
            }

            user.PasswordHash = password;
            db.SaveChanges();

            Session.Remove("ResetUserID");
            TempData["Success"] = " Đổi mật khẩu thành công!";

            return RedirectToAction("Login", "Account");
        }

        // ================================
        // 5️⃣ HÀM GỬI EMAIL
        // ================================
        private void SendEmail(string to, string subject, string body)
        {
            var mail = new MailMessage();
            mail.To.Add(to);
            mail.From = new MailAddress("yourEmail@gmail.com");
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = false;

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                Credentials = new NetworkCredential("tkgeminiplus@gmail.com", "ijsz mlwy dcau svyr"),
                EnableSsl = true
            };

            smtp.Send(mail);
        }
    }
}
