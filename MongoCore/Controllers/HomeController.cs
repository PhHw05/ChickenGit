using Microsoft.AspNetCore.Mvc;
using MongoCore.Models.Repositoties;
using MongoCore.Models.ModelViews;
using System.Linq;
using System.Collections.Generic;
using MongoCore.Models.Helpers; // Thêm using để dùng SessionExtensions

namespace MongoCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProRepository _proRepo;
        private readonly CateRepository _cateRepo;

        public HomeController(ProRepository proRepo, CateRepository cateRepo)
        {
            _proRepo = proRepo;
            _cateRepo = cateRepo;
        }

        public IActionResult Index()
        {
            ViewBag.Categories = _cateRepo.GetAll();
            List<ProM> hotProducts = _proRepo.GetAll().Take(16).ToList();

            // Lấy danh sách sản phẩm đã xem từ session (nếu có)
            var viewed = HttpContext.Session.GetObject<List<ProM>>("viewedProducts") ?? new List<ProM>();
            ViewBag.Viewed = viewed;

            return View(hotProducts);
        }

        [HttpGet("/xem-san-pham/{id}")]
        public IActionResult ViewProduct(string id)
        {
            var product = _proRepo.GetById(id);
            if (product == null) return NotFound();

            var viewed = HttpContext.Session.GetObject<List<ProM>>("viewedProducts") ?? new List<ProM>();

            if (!viewed.Any(p => p._id == product._id))
            {
                viewed.Insert(0, product); // Thêm vào đầu danh sách
                if (viewed.Count > 10) viewed.RemoveAt(viewed.Count - 1); // Giới hạn 10 sản phẩm
            }

            HttpContext.Session.SetObject("viewedProducts", viewed);

            return RedirectToAction("Detail", "Pro", new { id = product._id.ToString() });
        }

        [HttpPost]
        public IActionResult ClearViewed()
        {
            HttpContext.Session.Remove("viewedProducts");
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
