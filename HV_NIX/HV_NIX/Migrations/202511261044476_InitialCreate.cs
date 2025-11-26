namespace HV_NIX.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ActivityLogs",
                c => new
                    {
                        LogID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(),
                        Action = c.String(nullable: false, maxLength: 500),
                        Description = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.LogID)
                .ForeignKey("dbo.Users", t => t.UserID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserID = c.Int(nullable: false, identity: true),
                        Email = c.String(nullable: false, maxLength: 150),
                        PasswordHash = c.String(nullable: false),
                        Role = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserID);
            
            CreateTable(
                "dbo.Carts",
                c => new
                    {
                        CartID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.CartID)
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.CartItems",
                c => new
                    {
                        CartItemID = c.Int(nullable: false, identity: true),
                        CartID = c.Int(nullable: false),
                        ProductID = c.Int(nullable: false),
                        Size = c.String(),
                        Quantity = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.CartItemID)
                .ForeignKey("dbo.Carts", t => t.CartID, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductID, cascadeDelete: true)
                .Index(t => t.CartID)
                .Index(t => t.ProductID);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        ProductID = c.Int(nullable: false, identity: true),
                        ProductName = c.String(),
                        CategoryID = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Thumbnail = c.String(),
                        Image1 = c.String(),
                        Image2 = c.String(),
                        Image3 = c.String(),
                        Description = c.String(),
                        Stock = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProductID)
                .ForeignKey("dbo.Categories", t => t.CategoryID, cascadeDelete: true)
                .Index(t => t.CategoryID);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        CategoryID = c.Int(nullable: false, identity: true),
                        CategoryName = c.String(),
                    })
                .PrimaryKey(t => t.CategoryID);
            
            CreateTable(
                "dbo.Reviews",
                c => new
                    {
                        ReviewID = c.Int(nullable: false, identity: true),
                        ProductID = c.Int(nullable: false),
                        UserID = c.Int(nullable: false),
                        OrderID = c.Int(nullable: false),
                        Rating = c.Int(nullable: false),
                        Comment = c.String(),
                        ReviewDate = c.DateTime(nullable: false),
                        AdminReply = c.String(),
                    })
                .PrimaryKey(t => t.ReviewID)
                .ForeignKey("dbo.Orders", t => t.OrderID)
                .ForeignKey("dbo.Products", t => t.ProductID)
                .ForeignKey("dbo.Users", t => t.UserID)
                .Index(t => t.ProductID)
                .Index(t => t.UserID)
                .Index(t => t.OrderID);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        OrderID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        OrderDate = c.DateTime(nullable: false),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PaymentMethod = c.String(),
                        PaidDate = c.DateTime(),
                        Status = c.String(),
                        PaymentCode = c.String(),
                    })
                .PrimaryKey(t => t.OrderID)
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .ForeignKey("dbo.UserInfoes", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.OrderDetails",
                c => new
                    {
                        OrderDetailID = c.Int(nullable: false, identity: true),
                        OrderID = c.Int(nullable: false),
                        ProductID = c.Int(nullable: false),
                        Size = c.String(),
                        Quantity = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.OrderDetailID)
                .ForeignKey("dbo.Orders", t => t.OrderID, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductID, cascadeDelete: true)
                .Index(t => t.OrderID)
                .Index(t => t.ProductID);
            
            CreateTable(
                "dbo.UserInfoes",
                c => new
                    {
                        UserID = c.Int(nullable: false),
                        FullName = c.String(),
                        Phone = c.String(),
                        Address = c.String(),
                        City = c.String(),
                        Email = c.String(),
                        PaymentMethod = c.String(),
                        Province = c.String(),
                        District = c.String(),
                        Ward = c.String(),
                    })
                .PrimaryKey(t => t.UserID)
                .ForeignKey("dbo.Users", t => t.UserID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.Notifications",
                c => new
                    {
                        NotificationID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(),
                        Title = c.String(),
                        Message = c.String(),
                        IsRead = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.NotificationID)
                .ForeignKey("dbo.Users", t => t.UserID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.PasswordResets",
                c => new
                    {
                        PasswordResetID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        Token = c.String(),
                        ExpiredAt = c.DateTime(nullable: false),
                        Used = c.Boolean(nullable: false),
                        ResetCode = c.String(nullable: false, maxLength: 10),
                    })
                .PrimaryKey(t => t.PasswordResetID)
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.ShippingAddresses",
                c => new
                    {
                        AddressID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        ReceiverName = c.String(nullable: false),
                        Phone = c.String(nullable: false),
                        AddressLine = c.String(nullable: false),
                        City = c.String(nullable: false),
                        IsDefault = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.AddressID)
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.Location_District",
                c => new
                    {
                        DistrictID = c.Int(nullable: false, identity: true),
                        ProvinceID = c.Int(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.DistrictID);
            
            CreateTable(
                "dbo.Location_Province",
                c => new
                    {
                        ProvinceID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.ProvinceID);
            
            CreateTable(
                "dbo.Location_Ward",
                c => new
                    {
                        WardID = c.Int(nullable: false, identity: true),
                        DistrictID = c.Int(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.WardID);
            
            CreateTable(
                "dbo.OrderHistories",
                c => new
                    {
                        OrderHistoryID = c.Int(nullable: false, identity: true),
                        OrderID = c.Int(nullable: false),
                        Status = c.String(),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.OrderHistoryID)
                .ForeignKey("dbo.Orders", t => t.OrderID, cascadeDelete: true)
                .Index(t => t.OrderID);
            
            CreateTable(
                "dbo.PaymentMethods",
                c => new
                    {
                        MethodID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        MethodType = c.String(),
                        Provider = c.String(),
                        AccountNumber = c.String(),
                        IsDefault = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.MethodID)
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PaymentMethods", "UserID", "dbo.Users");
            DropForeignKey("dbo.OrderHistories", "OrderID", "dbo.Orders");
            DropForeignKey("dbo.ActivityLogs", "UserID", "dbo.Users");
            DropForeignKey("dbo.ShippingAddresses", "UserID", "dbo.Users");
            DropForeignKey("dbo.PasswordResets", "UserID", "dbo.Users");
            DropForeignKey("dbo.Notifications", "UserID", "dbo.Users");
            DropForeignKey("dbo.Carts", "UserID", "dbo.Users");
            DropForeignKey("dbo.CartItems", "ProductID", "dbo.Products");
            DropForeignKey("dbo.Reviews", "UserID", "dbo.Users");
            DropForeignKey("dbo.Reviews", "ProductID", "dbo.Products");
            DropForeignKey("dbo.Reviews", "OrderID", "dbo.Orders");
            DropForeignKey("dbo.Orders", "UserID", "dbo.UserInfoes");
            DropForeignKey("dbo.UserInfoes", "UserID", "dbo.Users");
            DropForeignKey("dbo.Orders", "UserID", "dbo.Users");
            DropForeignKey("dbo.OrderDetails", "ProductID", "dbo.Products");
            DropForeignKey("dbo.OrderDetails", "OrderID", "dbo.Orders");
            DropForeignKey("dbo.Products", "CategoryID", "dbo.Categories");
            DropForeignKey("dbo.CartItems", "CartID", "dbo.Carts");
            DropIndex("dbo.PaymentMethods", new[] { "UserID" });
            DropIndex("dbo.OrderHistories", new[] { "OrderID" });
            DropIndex("dbo.ShippingAddresses", new[] { "UserID" });
            DropIndex("dbo.PasswordResets", new[] { "UserID" });
            DropIndex("dbo.Notifications", new[] { "UserID" });
            DropIndex("dbo.UserInfoes", new[] { "UserID" });
            DropIndex("dbo.OrderDetails", new[] { "ProductID" });
            DropIndex("dbo.OrderDetails", new[] { "OrderID" });
            DropIndex("dbo.Orders", new[] { "UserID" });
            DropIndex("dbo.Reviews", new[] { "OrderID" });
            DropIndex("dbo.Reviews", new[] { "UserID" });
            DropIndex("dbo.Reviews", new[] { "ProductID" });
            DropIndex("dbo.Products", new[] { "CategoryID" });
            DropIndex("dbo.CartItems", new[] { "ProductID" });
            DropIndex("dbo.CartItems", new[] { "CartID" });
            DropIndex("dbo.Carts", new[] { "UserID" });
            DropIndex("dbo.ActivityLogs", new[] { "UserID" });
            DropTable("dbo.PaymentMethods");
            DropTable("dbo.OrderHistories");
            DropTable("dbo.Location_Ward");
            DropTable("dbo.Location_Province");
            DropTable("dbo.Location_District");
            DropTable("dbo.ShippingAddresses");
            DropTable("dbo.PasswordResets");
            DropTable("dbo.Notifications");
            DropTable("dbo.UserInfoes");
            DropTable("dbo.OrderDetails");
            DropTable("dbo.Orders");
            DropTable("dbo.Reviews");
            DropTable("dbo.Categories");
            DropTable("dbo.Products");
            DropTable("dbo.CartItems");
            DropTable("dbo.Carts");
            DropTable("dbo.Users");
            DropTable("dbo.ActivityLogs");
        }
    }
}
