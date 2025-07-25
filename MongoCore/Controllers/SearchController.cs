using Microsoft.AspNetCore.Mvc;
using MongoCore.Models.Repositoties;
using MongoCore.Models.ModelViews;
using System.Collections.Generic; // Cần cho List<T>
using System.Linq; // Cần cho LINQ (ToList)
using System.Threading.Tasks; // Cần cho Task

namespace MongoCore.Controllers
{
    // Controller này ở cấp độ gốc (không trong Area)
    public class SearchController : Controller
    {
        private readonly ProRepository _proRepo;

        // Inject ProRepository
        public SearchController(ProRepository proRepo)
        {
            _proRepo = proRepo;
        }

        // Action xử lý yêu cầu tìm kiếm AJAX
        [HttpGet]
        public IActionResult Search(string q) // 'q' là tên tham số query từ JavaScript
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return Json(new List<ProM>()); // Trả về JSON rỗng nếu không có query
            }

            // Gọi phương thức SearchByName từ ProRepository
            var results = _proRepo.SearchByName(q);

            // Trả về kết quả dưới dạng JSON.
            // HttpClient side (scripts.js) sẽ nhận JSON này.
            return Json(results);
        }

        // Nếu bạn muốn một trang kết quả tìm kiếm đầy đủ, bạn có thể thêm action này
        // public IActionResult Results(string q)
        // {
        //     ViewBag.Title = $"Kết quả tìm kiếm cho: {q}";
        //     var results = _proRepo.SearchByName(q);
        //     return View(results); // Trả về một View riêng cho trang kết quả tìm kiếm
        // }
    }
}