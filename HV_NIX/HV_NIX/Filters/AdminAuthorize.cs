using System.Web;
using System.Web.Mvc;

namespace HV_NIX.Filters
{
    public class AdminAuthorize : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase ctx)
        {
            // Chỉ cần có AdminID là admin đã login
            return ctx.Session["AdminID"] != null;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // Nếu chưa login admin → chuyển về login của trang chính
            filterContext.Result = new RedirectResult("/Account/Login");
        }
    }
}
