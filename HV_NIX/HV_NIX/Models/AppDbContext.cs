using System.Data.Entity;

namespace HV_NIX.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("ShopPAEntities")
        {
            // KHÔNG DROP DATABASE NỮA, GIỮ NGUYÊN DỮ LIỆU ADMIN
            Database.SetInitializer<AppDbContext>(null);
        }

        // =======================
        // 📌 DB SETS
        // =======================
        public DbSet<Users> Users { get; set; }
        public DbSet<UserInfo> UserInfos { get; set; }

        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItems> CartItems { get; set; }

        public DbSet<ActivityLog> ActivityLogs { get; set; }

        public DbSet<Orders> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }

        public DbSet<Products> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<PasswordReset> PasswordResets { get; set; }
        public DbSet<Reviews> Reviews { get; set; }
        public DbSet<ShippingAddress> ShippingAddresses { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }

        public DbSet<Location_Province> Location_Provinces { get; set; }
        public DbSet<Location_District> Location_Districts { get; set; }
        public DbSet<Location_Ward> Location_Wards { get; set; }

        public DbSet<Notification> Notifications { get; set; }
        public DbSet<OrderHistory> OrderHistories { get; set; }

        // =======================
        // ⚙ MODEL CONFIG
        // =======================
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // ------------------------
            // 🔥 FIX REVIEW RELATION
            // ------------------------

            // Review – User
            modelBuilder.Entity<Reviews>()
                .HasRequired(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserID)
                .WillCascadeOnDelete(false);

            // Review – Product
            modelBuilder.Entity<Reviews>()
                .HasRequired(r => r.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProductID)
                .WillCascadeOnDelete(false);

            // Review – Order
            modelBuilder.Entity<Reviews>()
                .HasRequired(r => r.Order)
                .WithMany(o => o.Reviews)
                .HasForeignKey(r => r.OrderID)
                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}
