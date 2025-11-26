using System.Web.Mvc;

namespace HV_NIX.Models
{
    public class AdminAuthorize : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Kiểm tra admin login
            var adminId = filterContext.HttpContext.Session["AdminID"];

            if (adminId == null)
            {
                // Nếu chưa phải admin → về trang đăng nhập chính
                filterContext.Result = new RedirectResult("/Account/Login");
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
