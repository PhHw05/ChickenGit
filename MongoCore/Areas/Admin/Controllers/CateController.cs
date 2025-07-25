using Microsoft.AspNetCore.Mvc;
using MongoCore.Models.ModelViews;
using MongoCore.Models.Repositoties;
using MongoDB.Bson;
using System;
using System.Linq;

namespace MongoCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CateController : Controller
    {
        private readonly CateRepository _repo;

        public CateController(CateRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public IActionResult Index()
        {
            var data = _repo.GetAll();
            Console.WriteLine($"Index: Loaded {data.Count} categories");
            return View(data);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Title = "Thêm Danh mục mới";
            return View(new CateM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CateM cate)
        {
            Console.WriteLine($"Create POST: nameCate={cate.nameCate ?? "null"}, _id={cate._id.ToString() ?? "null"}");
            if (cate == null)
            {
                Console.WriteLine("CateM is null");
                return BadRequest("Invalid category data.");
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine("ModelState Errors: " + string.Join(", ", errors));
            }
            else
            {
                Console.WriteLine("ModelState is valid");
                if (!string.IsNullOrEmpty(cate.nameCate))
                {
                    var existingCate = _repo.GetByName(name: cate.nameCate);
                    if (existingCate != null)
                    {
                        ModelState.AddModelError("nameCate", "Tên danh mục đã tồn tại.");
                        Console.WriteLine("Duplicate nameCate found");
                    }
                    else if (_repo.Add(cate))
                    {
                        Console.WriteLine("Add successful, redirecting to Index");
                        TempData["SuccessMessage"] = "Thêm danh mục thành công!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        Console.WriteLine("Add failed");
                        ModelState.AddModelError("", "Không thể thêm danh mục do lỗi hệ thống.");
                    }
                }
                else
                {
                    ModelState.AddModelError("nameCate", "Tên danh mục không được để trống.");
                    Console.WriteLine("nameCate is null or empty");
                }
            }

            ViewBag.Title = "Thêm Danh mục mới";
            return View(cate);
        }

        /// <summary>
        /// GET: Hiển thị form chỉnh sửa
        /// </summary>
        [HttpGet]
        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id) || !ObjectId.TryParse(id, out _))
            {
                Console.WriteLine($"Edit GET: Invalid ID={id}");
                return BadRequest("ID không hợp lệ.");
            }

            var cate = _repo.GetById(id);
            if (cate == null)
            {
                Console.WriteLine($"Edit GET: Category not found with ID={id}");
                return NotFound("Không tìm thấy danh mục.");
            }

            ViewBag.Title = "Chỉnh sửa Danh mục";
            return View(cate);
        }

        /// <summary>
        /// POST: Lưu thông tin chỉnh sửa
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CateM cate)
        {
            Console.WriteLine($"Edit POST: _id={cate?._id.ToString() ?? "null"}, nameCate={cate?.nameCate ?? "null"}");
            if (cate == null)
            {
                Console.WriteLine("CateM is null");
                return BadRequest("Invalid category data.");
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine("ModelState Errors: " + string.Join(", ", errors));
            }
            else
            {
                Console.WriteLine("ModelState is valid");
                if (_repo.Update(cate))
                {
                    Console.WriteLine("Update successful, redirecting to Index");
                    TempData["SuccessMessage"] = "Cập nhật danh mục thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    Console.WriteLine("Update failed");
                    ModelState.AddModelError("", "Không thể cập nhật danh mục do lỗi hệ thống.");
                }
            }

            ViewBag.Title = "Chỉnh sửa Danh mục";
            return View(cate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id) || !ObjectId.TryParse(id, out _))
            {
                Console.WriteLine($"Delete: Invalid ID={id}");
                return BadRequest("ID không hợp lệ.");
            }

            if (_repo.Delete(id))
            {
                TempData["SuccessMessage"] = "Xóa danh mục thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xóa danh mục do lỗi hệ thống.";
            }

            return RedirectToAction("Index");
        }
    }
}
