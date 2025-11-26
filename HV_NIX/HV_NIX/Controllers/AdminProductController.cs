using HV_NIX.Helpers;
using HV_NIX.Models;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace HV_NIX.Controllers
{
    public class AdminProductController : Controller
    {
        private readonly AppDbContext db = new AppDbContext();

        // ============================
        // 📌 DANH SÁCH SẢN PHẨM
        // ============================
        public ActionResult Index()
        {
            var products = db.Products.Include("Category").ToList(); // FIX
            return View(products);
        }

        // ============================
        // 📌 GET: TẠO SẢN PHẨM
        // ============================
        public ActionResult Create()
        {
            ViewBag.Categories = db.Categories.ToList();
            return View();
        }

        // ============================
        // 📌 POST: TẠO SẢN PHẨM
        // ============================
        [HttpPost]
        public ActionResult Create(
            Products product,
            HttpPostedFileBase ThumbnailFile,
            HttpPostedFileBase Image1File,
            HttpPostedFileBase Image2File)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = db.Categories.ToList();
                return View(product);
            }

            // Upload ảnh (3 loại)
            product.Thumbnail = SaveImage(ThumbnailFile);
            product.Image1 = SaveImage(Image1File);
            product.Image2 = SaveImage(Image2File);

            db.Products.Add(product);
            db.SaveChanges();

            LogHelper.AddLog(db, null, "AddProduct", $"Thêm sản phẩm: {product.ProductName}");
            TempData["Success"] = "Thêm sản phẩm thành công!";

            return RedirectToAction("Index");
        }

        // ============================
        // 📌 GET: SỬA SẢN PHẨM
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
        // 📌 POST: SỬA SẢN PHẨM
        // ============================
        [HttpPost]
        public ActionResult Edit(
            Products updated,
            HttpPostedFileBase ThumbnailFile,
            HttpPostedFileBase Image1File,
            HttpPostedFileBase Image2File)
        {
            var product = db.Products.Find(updated.ProductID);
            if (product == null)
                return HttpNotFound();

            // Cập nhật text
            product.ProductName = updated.ProductName;
            product.CategoryID = updated.CategoryID;
            product.Price = updated.Price;
            product.Description = updated.Description;

            // Cập nhật ảnh mới
            product.Thumbnail = UpdateImage(ThumbnailFile, product.Thumbnail);
            product.Image1 = UpdateImage(Image1File, product.Image1);
            product.Image2 = UpdateImage(Image2File, product.Image2);

            db.SaveChanges();

            LogHelper.AddLog(db, null, "EditProduct", $"Sửa sản phẩm: {product.ProductName}");
            TempData["Success"] = "Cập nhật sản phẩm thành công!";

            return RedirectToAction("Index");
        }

        // ============================
        // 📌 XOÁ SẢN PHẨM
        // ============================
        public ActionResult Delete(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
                return HttpNotFound();

            // ❗ Check: Sản phẩm có review không?
            bool hasReviews = db.Reviews.Any(r => r.ProductID == id);

            if (hasReviews)
            {
                TempData["Error"] = "Không thể xóa sản phẩm vì đang có đánh giá của khách hàng!";
                return RedirectToAction("Index");
            }

            // ❗ Check: Sản phẩm có trong đơn hàng không?
            bool hasOrderDetails = db.OrderDetails.Any(o => o.ProductID == id);

            if (hasOrderDetails)
            {
                TempData["Error"] = "Không thể xóa sản phẩm vì đã xuất hiện trong đơn hàng!";
                return RedirectToAction("Index");
            }

            // ❗ Check: Sản phẩm có trong giỏ hàng chưa checkout không?
            bool hasCartItems = db.CartItems.Any(c => c.ProductID == id);

            if (hasCartItems)
            {
                TempData["Error"] = "Không thể xóa sản phẩm vì đang có trong giỏ hàng của người dùng!";
                return RedirectToAction("Index");
            }

            // >>> Nếu qua 3 bước trên → sản phẩm an toàn để xoá

            DeleteImage(product.Thumbnail);
            DeleteImage(product.Image1);
            DeleteImage(product.Image2);

            db.Products.Remove(product);
            db.SaveChanges();

            LogHelper.AddLog(db, null, "DeleteProduct", $"Xóa sản phẩm: {product.ProductName}");
            TempData["Success"] = "Xóa sản phẩm thành công!";

            return RedirectToAction("Index");
        }


        // ============================
        // 📌 HÀM HỖ TRỢ UPLOAD ẢNH
        // ============================
        private string SaveImage(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
                return null;

            string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            string path = Server.MapPath("~/Content/Images/" + fileName);

            file.SaveAs(path);
            return fileName;
        }

        private string UpdateImage(HttpPostedFileBase newFile, string oldFile)
        {
            if (newFile != null && newFile.ContentLength > 0)
            {
                // Xóa ảnh cũ
                if (!string.IsNullOrEmpty(oldFile))
                {
                    string oldPath = Server.MapPath("~/Content/Images/" + oldFile);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                // Lưu ảnh mới
                string fileName = Guid.NewGuid() + Path.GetExtension(newFile.FileName);
                string savePath = Server.MapPath("~/Content/Images/" + fileName);
                newFile.SaveAs(savePath);

                return fileName;
            }

            return oldFile; // Không upload ảnh → giữ tên cũ
        }


        private void DeleteImage(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                string path = Server.MapPath("~/Content/Images/" + fileName);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
            }
        }
    }
}
