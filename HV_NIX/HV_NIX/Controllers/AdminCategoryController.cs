using HV_NIX.Helpers;
using HV_NIX.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace HV_NIX.Controllers
{
    public class AdminCategoryController : Controller
    {
        private readonly AppDbContext db = new AppDbContext();

        // ============================
        // 📌 DANH SÁCH DANH MỤC
        // ============================
        public ActionResult Index()
        {
            var categories = db.Categories.ToList();
            return View(categories);
        }

        // ============================
        // 📌 GET: THÊM DANH MỤC
        // ============================
        public ActionResult Create()
        {
            return View();
        }

        // ============================
        // 📌 POST: THÊM DANH MỤC
        // ============================
        [HttpPost]
        public ActionResult Create(Category model)
        {
            if (!ModelState.IsValid)
                return View(model);

            db.Categories.Add(model);
            db.SaveChanges();

            // ✔ FIXED — dùng UserID = null cho log hệ thống
            LogHelper.AddLog(db, null, "AddCategory", $"Thêm danh mục: {model.CategoryName}");

            TempData["Success"] = "Đã thêm danh mục!";
            return RedirectToAction("Index");
        }

        // ============================
        // 📌 GET: SỬA DANH MỤC
        // ============================
        public ActionResult Edit(int id)
        {
            var cat = db.Categories.Find(id);
            if (cat == null) return HttpNotFound();

            return View(cat);
        }

        // ============================
        // 📌 POST: SỬA DANH MỤC
        // ============================
        [HttpPost]
        public ActionResult Edit(Category update)
        {
            var cat = db.Categories.Find(update.CategoryID);
            if (cat == null) return HttpNotFound();

            cat.CategoryName = update.CategoryName;
            db.SaveChanges();

            // ✔ FIXED
            LogHelper.AddLog(db, null, "EditCategory", $"Sửa danh mục: {update.CategoryName}");

            TempData["Success"] = "Cập nhật thành công!";
            return RedirectToAction("Index");
        }

        // ============================
        // 📌 XOÁ DANH MỤC
        // ============================
        public ActionResult Delete(int id)
        {
            var cat = db.Categories.Find(id);
            if (cat == null) return HttpNotFound();

            // kiểm tra nếu category có product
            bool hasProducts = db.Products.Any(p => p.CategoryID == id);

            if (hasProducts)
            {
                TempData["Error"] = "Không thể xoá danh mục vì đang có sản phẩm bên trong!";
                return RedirectToAction("Index");
            }

            db.Categories.Remove(cat);
            db.SaveChanges();

            TempData["Success"] = "Đã xoá danh mục!";
            return RedirectToAction("Index");
        }
    }
}
