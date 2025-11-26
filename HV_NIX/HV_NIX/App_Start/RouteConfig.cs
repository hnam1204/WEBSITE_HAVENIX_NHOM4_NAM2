using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HV_NIX
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // ⚡ Route cho Category phải đặt TRƯỚC Default
            routes.MapRoute(
                name: "ProductCategory",
                url: "Product/Category/{id}",
                defaults: new { controller = "Product", action = "Category", id = UrlParameter.Optional }
            );

            // ⚡ Route Default để cuối cùng
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}