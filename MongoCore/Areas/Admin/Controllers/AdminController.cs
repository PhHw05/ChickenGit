using Microsoft.AspNetCore.Mvc;
using MongoCore.Models.ModelViews;

namespace MongoCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Title = "Admin Dashboard";
            return View("Dashboard");
        }
    }
}