using Microsoft.AspNetCore.Mvc; // Dùng cho ASP.NET Core MVC
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http; // Cần cho HttpContext.Session.GetString
using System.Text.Json; // Cần để deserialize JSON

namespace MongoCore.Models.ModelViews
{
    public class AdminAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Kiểm tra HTTP Context và Session có tồn tại không
            if (filterContext.HttpContext == null || filterContext.HttpContext.Session == null)
            {
                // Có thể log lỗi hoặc chuyển hướng đến trang lỗi
                filterContext.Result = new RedirectResult("/Error");
                return;
            }

            // Lấy thông tin người dùng từ Session
            UserM user = null;
            // HttpContext.Session.GetString là phương thức đồng bộ, phù hợp trong OnActionExecuting
            var currentUserJson = filterContext.HttpContext.Session.GetString("CurrentUser");

            if (!string.IsNullOrEmpty(currentUserJson))
            {
                try
                {
                    user = JsonSerializer.Deserialize<UserM>(currentUserJson);
                }
                catch (JsonException ex)
                {
                    // Ghi log lỗi deserialize nếu có. Trong môi trường thực tế, bạn nên dùng ILogger.
                    Console.WriteLine($"Lỗi deserialize UserM từ session trong AdminAuthorizeAttribute: {ex.Message}");
                }
            }

            // Kiểm tra nếu user không tồn tại hoặc user không có quyền admin (Role != "Admin")
            // Lưu ý: role của bạn là string, nên so sánh bằng chuỗi
            if (user == null || user.Role != "Admin") // Giả định "Admin" là chuỗi đại diện cho vai trò quản trị
            {
                // Chuyển hướng về trang đăng nhập
                filterContext.Result = new RedirectResult("/Login/Login");
                return; // Quan trọng: dừng xử lý pipeline khi chuyển hướng
            }

            base.OnActionExecuting(filterContext);
        }
    }
}