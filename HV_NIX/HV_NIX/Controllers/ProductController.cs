using HV_NIX.Models;
using System;
using System.Data.Entity;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace HV_NIX.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext db = new AppDbContext();

        // ============================
        // 📌 LIST SẢN PHẨM + PHÂN TRANG
        // ============================
        public ActionResult Index(int page = 1, int pageSize = 6)
        {
            int total = db.Products.Count();
            int totalPages = (int)Math.Ceiling((double)total / pageSize);

            var products = db.Products
                .OrderByDescending(p => p.ProductID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;

            return View(products);
        }

        // ============================
        // 📌 CHI TIẾT SẢN PHẨM
        // ============================
        public ActionResult Details(int id)
        {
            var product = db.Products
                           .Include(p => p.Category)
                           .FirstOrDefault(p => p.ProductID == id);

            if (product == null)
                return HttpNotFound();

            var related = db.Products
                .Where(p => p.CategoryID == product.CategoryID && p.ProductID != id)
                .Take(4)
                .ToList();

            ViewBag.RelatedProducts = related;

            return View(product);
        }

        // ============================
        // 📌 LỌC THEO DANH MỤC CategoryID
        // ============================
        public ActionResult Category(int id)
        {
            var category = db.Categories.FirstOrDefault(c => c.CategoryID == id);

            if (category == null)
                return HttpNotFound("Danh mục không tồn tại!");

            var products = db.Products
                .Where(p => p.CategoryID == id)
                .OrderBy(p => p.ProductID).ToList();

            ViewBag.CategoryName = category.CategoryName;

            return View("Category", products);
        }

        // ============================
        // 📌 GET: CREATE
        // ============================
        public ActionResult Create()
        {
            ViewBag.Categories = db.Categories.ToList();
            return View();
        }

        // ============================
        // 📌 POST: CREATE
        // ============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Products product,
                                   HttpPostedFileBase ThumbnailFile,
                                   HttpPostedFileBase Image1File,
                                   HttpPostedFileBase Image2File)
        {
            if (product.CategoryID <= 0)
            {
                ModelState.AddModelError("", "Vui lòng chọn danh mục.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = db.Categories.ToList();
                return View(product);
            }

            string folderPath = Server.MapPath("~/Content/Images/");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            // 📌 Upload Thumbnail
            if (ThumbnailFile != null && ThumbnailFile.ContentLength > 0)
            {
                string fileName = Guid.NewGuid() + Path.GetExtension(ThumbnailFile.FileName);
                ThumbnailFile.SaveAs(Path.Combine(folderPath, fileName));
                product.Thumbnail = fileName;
            }

            // 📌 Upload Image1
            if (Image1File != null && Image1File.ContentLength > 0)
            {
                string fileName = Guid.NewGuid() + Path.GetExtension(Image1File.FileName);
                Image1File.SaveAs(Path.Combine(folderPath, fileName));
                product.Image1 = fileName;
            }

            // 📌 Upload Image2
            if (Image2File != null && Image2File.ContentLength > 0)
            {
                string fileName = Guid.NewGuid() + Path.GetExtension(Image2File.FileName);
                Image2File.SaveAs(Path.Combine(folderPath, fileName));
                product.Image2 = fileName;
            }

            db.Products.Add(product);
            db.SaveChanges();

            TempData["Success"] = "Thêm sản phẩm thành công!";
            return RedirectToAction("Index");
        }

        // ============================
        // 📌 GET: EDIT
        // ============================
        public ActionResult Edit(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
                return HttpNotFound();

            ViewBag.Categories = db.Categories.ToList();
            return View(product);
        }

        // ============================
        // 📌 POST: EDIT (ĐÃ FIX STOCK)
        // ============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Products model,
                                 HttpPostedFileBase ThumbnailFile,
                                 HttpPostedFileBase Image1File,
                                 HttpPostedFileBase Image2File)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = db.Categories.ToList();
                return View(model);
            }

            var product = db.Products.Find(model.ProductID);
            if (product == null)
                return HttpNotFound();

            // 📌 UPDATE THÔNG TIN
            product.ProductName = model.ProductName;
            product.Description = model.Description;
            product.Price = model.Price;
            product.CategoryID = model.CategoryID;

            // ❗ XÓA DÒNG LỖI: product.Stock = model.Stock;

            string folderPath = Server.MapPath("~/Content/Images/");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            // ===== Thumbnail =====
            if (ThumbnailFile != null && ThumbnailFile.ContentLength > 0)
            {
                if (!string.IsNullOrEmpty(product.Thumbnail))
                {
                    string oldFile = Path.Combine(folderPath, product.Thumbnail);
                    if (System.IO.File.Exists(oldFile))
                        System.IO.File.Delete(oldFile);
                }

                string fileName = Guid.NewGuid() + Path.GetExtension(ThumbnailFile.FileName);
                ThumbnailFile.SaveAs(Path.Combine(folderPath, fileName));
                product.Thumbnail = fileName;
            }

            // ===== Image1 =====
            if (Image1File != null && Image1File.ContentLength > 0)
            {
                if (!string.IsNullOrEmpty(product.Image1))
                {
                    string oldFile = Path.Combine(folderPath, product.Image1);
                    if (System.IO.File.Exists(oldFile))
                        System.IO.File.Delete(oldFile);
                }

                string fileName = Guid.NewGuid() + Path.GetExtension(Image1File.FileName);
                Image1File.SaveAs(Path.Combine(folderPath, fileName));
                product.Image1 = fileName;
            }

            // ===== Image2 =====
            if (Image2File != null && Image2File.ContentLength > 0)
            {
                if (!string.IsNullOrEmpty(product.Image2))
                {
                    string oldFile = Path.Combine(folderPath, product.Image2);
                    if (System.IO.File.Exists(oldFile))
                        System.IO.File.Delete(oldFile);
                }

                string fileName = Guid.NewGuid() + Path.GetExtension(Image2File.FileName);
                Image2File.SaveAs(Path.Combine(folderPath, fileName));
                product.Image2 = fileName;
            }

            db.SaveChanges();
            TempData["Success"] = "✔ Cập nhật sản phẩm thành công!";
            return RedirectToAction("Index");
        }

        // ============================
        // 📌 GET: DELETE
        // ============================
        public ActionResult Delete(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
                return HttpNotFound();

            return View(product);
        }

        // ============================
        // 📌 POST: DELETE
        // ============================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var product = db.Products.Find(id);

            if (product == null)
                return HttpNotFound();

            string folderPath = Server.MapPath("~/Content/Images/");

            void DeleteFile(string fileName)
            {
                if (!string.IsNullOrEmpty(fileName))
                {
                    string path = Path.Combine(folderPath, fileName);
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                }
            }

            DeleteFile(product.Thumbnail);
            DeleteFile(product.Image1);
            DeleteFile(product.Image2);

            db.Products.Remove(product);
            db.SaveChanges();

            TempData["Success"] = "✔ Đã xóa sản phẩm thành công!";
            return RedirectToAction("Index");
        }
    }
}
