using HV_NIX.Models;
using System.Linq;
using System.Web.Mvc;

namespace HV_NIX.Controllers
{
    public class ShippingController : Controller
    {
        private readonly AppDbContext db = new AppDbContext();

        // 📌 Danh sách địa chỉ
        public ActionResult Index()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Account");

            int uid = (int)Session["UserID"];

            var list = db.ShippingAddresses
                         .Where(x => x.UserID == uid)
                         .OrderByDescending(x => x.IsDefault)
                         .ToList();

            return View(list);
        }

        // 📌 Thêm địa chỉ mới
        [HttpPost]
        public ActionResult Add(string name, string phone, string address, string city, bool? isDefault)
        {
            int uid = (int)Session["UserID"];
            bool setDefault = isDefault ?? false;

            if (setDefault)
            {
                var oldDefaults = db.ShippingAddresses.Where(x => x.UserID == uid && x.IsDefault);
                foreach (var d in oldDefaults) d.IsDefault = false;
            }

            db.ShippingAddresses.Add(new ShippingAddress
            {
                UserID = uid,
                ReceiverName = name,
                Phone = phone,
                AddressLine = address,
                City = city,
                IsDefault = setDefault
            });

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // 📌 Đặt làm mặc định
        public ActionResult SetDefault(int id)
        {
            int uid = (int)Session["UserID"];

            var mine = db.ShippingAddresses.FirstOrDefault(a => a.AddressID == id && a.UserID == uid);
            if (mine == null) return RedirectToAction("Index");

            // Hủy mặc định cũ
            var list = db.ShippingAddresses.Where(a => a.UserID == uid);
            foreach (var a in list) a.IsDefault = false;

            mine.IsDefault = true;
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        // 📌 GET: Edit
        public ActionResult Edit(int id)
        {
            int uid = (int)Session["UserID"];

            var item = db.ShippingAddresses.FirstOrDefault(a => a.AddressID == id && a.UserID == uid);
            if (item == null)
                return RedirectToAction("Index");

            return View(item);
        }

        // 📌 POST: Edit
        [HttpPost]
        public ActionResult Edit(int AddressID, string name, string phone, string address, string city, bool isDefault = false)
        {
            int uid = (int)Session["UserID"];

            var item = db.ShippingAddresses.FirstOrDefault(a => a.AddressID == AddressID && a.UserID == uid);
            if (item == null)
                return RedirectToAction("Index");

            // Nếu đặt mặc định → bỏ mặc định cũ
            if (isDefault)
            {
                var oldDefaults = db.ShippingAddresses.Where(x => x.UserID == uid && x.IsDefault);
                foreach (var d in oldDefaults) d.IsDefault = false;
            }

            item.ReceiverName = name;
            item.Phone = phone;
            item.AddressLine = address;
            item.City = city;
            item.IsDefault = isDefault;

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // 📌 Xóa địa chỉ
        public ActionResult Delete(int id)
        {
            int uid = (int)Session["UserID"];

            var item = db.ShippingAddresses.FirstOrDefault(a => a.AddressID == id && a.UserID == uid);
            if (item != null)
            {
                db.ShippingAddresses.Remove(item);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
