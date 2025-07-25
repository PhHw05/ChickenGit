using Microsoft.AspNetCore.Mvc;
using MongoCore.Models.ModelViews;
using MongoCore.Models.Repositoties;
using MongoDB.Bson;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MongoCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProController : Controller
    {
        private readonly ProRepository _repo;
        private readonly CateRepository _cateRepo;

        public ProController(ProRepository repo, CateRepository cateRepo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _cateRepo = cateRepo ?? throw new ArgumentNullException(nameof(cateRepo));
        }

        public IActionResult Index()
        {
            var data = _repo.GetAll();
            Console.WriteLine($"Index: Loaded {data.Count} products");
            var cateList = _cateRepo.GetAll();
            Console.WriteLine($"Index: Loaded {cateList?.Count ?? 0} categories, First item: {cateList?.FirstOrDefault()?.nameCate ?? "null"}");
            ViewBag.CateList = cateList ?? new List<CateM>();
            return View(data);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Title = "Thêm Sản phẩm mới";
            var cateList = _cateRepo.GetAll();
            Console.WriteLine($"Create GET: Loaded {cateList?.Count ?? 0} categories, First item: {cateList?.FirstOrDefault()?.nameCate ?? "null"}");
            if (cateList == null || !cateList.Any())
            {
                Console.WriteLine("Create GET: No categories found, setting empty list");
                ViewBag.CateList = new List<CateM>();
            }
            else
            {
                ViewBag.CateList = cateList;
            }
            var pro = new ProM();
            ViewBag.NextId = _repo.GetNextId();
            return View(pro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProM pro, IFormFile Image)
        {
            Console.WriteLine($"Create POST: namePro={pro?.namePro ?? "null"}, _id={pro?._id.ToString() ?? "null"}, CateId={pro?.CateId}");
            ModelState.Remove("Image");
            if (pro == null)
            {
                Console.WriteLine("ProM is null");
                TempData["ErrorMessage"] = "Dữ liệu sản phẩm không hợp lệ.";
                return View(pro);
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine("ModelState Errors: " + string.Join(", ", errors));
            }
            else
            {
                Console.WriteLine("ModelState is valid");
                try
                {
                    var imageDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    if (!Directory.Exists(imageDir))
                    {
                        Directory.CreateDirectory(imageDir);
                    }

                    if (Image != null && Image.Length > 0)
                    {
                        var fileExtension = Path.GetExtension(Image.FileName);
                        var fileName = $"{ObjectId.GenerateNewId()}{fileExtension}";
                        var filePath = Path.Combine(imageDir, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await Image.CopyToAsync(stream);
                        }
                        pro.Image = $"/images/{fileName}";
                    }
                    else
                    {
                        pro.Image = "/images/noimage.png";
                    }

                    if (_repo.Add(pro))
                    {
                        Console.WriteLine("Add successful, redirecting to Index");
                        TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        Console.WriteLine("Add failed");
                        TempData["ErrorMessage"] = "Không thể thêm sản phẩm do lỗi hệ thống.";
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in Create: {ex.Message}\nStackTrace: {ex.StackTrace}");
                    TempData["ErrorMessage"] = "Lỗi hệ thống khi thêm sản phẩm.";
                }
            }

            ViewBag.Title = "Thêm Sản phẩm mới";
            var cateList = _cateRepo.GetAll();
            Console.WriteLine($"Create POST: Loaded {cateList?.Count ?? 0} categories, First item: {cateList?.FirstOrDefault()?.nameCate ?? "null"}");
            ViewBag.CateList = cateList ?? new List<CateM>();
            ViewBag.NextId = _repo.GetNextId();
            return View(pro);
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id) || !ObjectId.TryParse(id, out _))
            {
                Console.WriteLine($"Edit GET: Invalid ID={id}");
                return BadRequest("ID không hợp lệ.");
            }

            var pro = _repo.GetById(id);
            if (pro == null)
            {
                Console.WriteLine($"Edit GET: Product not found with ID={id}");
                return NotFound("Không tìm thấy sản phẩm.");
            }

            ViewBag.Title = "Chỉnh sửa Sản phẩm";
            var cateList = _cateRepo.GetAll();
            Console.WriteLine($"Edit GET: Loaded {cateList?.Count ?? 0} categories, First item: {cateList?.FirstOrDefault()?.nameCate ?? "null"}");
            ViewBag.CateList = cateList ?? new List<CateM>();
            return View(pro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProM pro, IFormFile Image)
        {
            Console.WriteLine($"Edit POST: _id={pro?._id.ToString() ?? "null"}, namePro={pro?.namePro ?? "null"}, CateId={pro?.CateId}");
            ModelState.Remove("Image");
            if (pro == null)
            {
                Console.WriteLine("ProM is null");
                TempData["ErrorMessage"] = "Dữ liệu sản phẩm không hợp lệ.";
                return View(pro);
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine("ModelState Errors: " + string.Join(", ", errors));
            }
            else
            {
                Console.WriteLine("ModelState is valid");
                try
                {
                    var existing = _repo.GetById(pro._id.ToString());
                    if (existing == null)
                    {
                        Console.WriteLine($"Edit POST: Product not found with ID={pro._id}");
                        return NotFound("Không tìm thấy sản phẩm.");
                    }

                    var imageDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    if (!Directory.Exists(imageDir))
                    {
                        Directory.CreateDirectory(imageDir);
                    }

                    if (Image != null && Image.Length > 0)
                    {
                        var fileExtension = Path.GetExtension(Image.FileName);
                        var fileName = $"{pro._id}{fileExtension}";
                        var filePath = Path.Combine(imageDir, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await Image.CopyToAsync(stream);
                        }
                        pro.Image = $"/images/{fileName}";
                    }
                    else
                    {
                        pro.Image = existing.Image ?? "/images/noimage.png";
                    }

                    if (_repo.Update(pro))
                    {
                        Console.WriteLine("Update successful, redirecting to Index");
                        TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        Console.WriteLine("Update failed");
                        TempData["ErrorMessage"] = "Không thể cập nhật sản phẩm do lỗi hệ thống.";
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in Edit: {ex.Message}\nStackTrace: {ex.StackTrace}");
                    TempData["ErrorMessage"] = "Lỗi hệ thống khi cập nhật sản phẩm.";
                }
            }

            ViewBag.Title = "Chỉnh sửa Sản phẩm";
            var cateList = _cateRepo.GetAll();
            Console.WriteLine($"Edit POST: Loaded {cateList?.Count ?? 0} categories, First item: {cateList?.FirstOrDefault()?.nameCate ?? "null"}");
            ViewBag.CateList = cateList ?? new List<CateM>();
            return View(pro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id) || !ObjectId.TryParse(id, out _))
            {
                Console.WriteLine($"Delete: Invalid ID={id}");
                TempData["ErrorMessage"] = "ID không hợp lệ.";
                return RedirectToAction("Index");
            }

            if (_repo.Delete(id))
            {
                Console.WriteLine($"Delete successful: ID={id}");
                TempData["SuccessMessage"] = "Xóa sản phẩm thành công!";
            }
            else
            {
                Console.WriteLine($"Delete failed: ID={id}");
                TempData["ErrorMessage"] = "Không thể xóa sản phẩm do lỗi hệ thống.";
            }

            return RedirectToAction("Index");
        }
    }
}