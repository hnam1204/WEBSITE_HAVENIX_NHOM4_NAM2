namespace HV_NIX.Migrations
{
    using HV_NIX.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<HV_NIX.Models.AppDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;               // KHÔNG BẬT AUTO, tránh lỗi DB
            AutomaticMigrationDataLossAllowed = false;
        }

        protected override void Seed(HV_NIX.Models.AppDbContext context)
        {
            // ===========================
            // 🔹 SEED DANH MỤC
            // ===========================
            if (!context.Categories.Any())
            {
                context.Categories.AddOrUpdate(
                    c => c.CategoryName,
                    new Category { CategoryName = "Áo Thun" },
                    new Category { CategoryName = "Áo Khoác" },
                    new Category { CategoryName = "Quần Dài" },
                    new Category { CategoryName = "Áo Khoác Nỉ" },
                    new Category { CategoryName = "Quần Ngắn" }
                );
            }

            // ===========================
            // 🔹 SEED ADMIN
            // ===========================
            if (!context.Users.Any(u => u.Email == "admin@gmail.com"))
            {
                context.Users.AddOrUpdate(
                    u => u.Email,
                    new Users
                    {
                        Email = "admin@gmail.com",
                        PasswordHash = "123456",   // NẾU bạn muốn hash, tôi hash lại cho bạn
                        Role = "Admin",
                        CreatedAt = DateTime.Now
                    }
                );
            }

            context.SaveChanges();
        }
    }
}
