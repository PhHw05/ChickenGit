using Microsoft.AspNetCore.Mvc;
using MongoCore.Models.ModelViews;
using MongoDB.Driver;
using System.Linq;

namespace MongoCore.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly IMongoCollection<UserM> _userCollection;

        public UserController(IMongoClient client)
        {
            var database = client.GetDatabase("YourDatabaseName");
            _userCollection = database.GetCollection<UserM>("User");
        }

        public IActionResult Index()
        {
            var users = _userCollection.Find(_ => true).ToList();
            ViewBag.Title = "Danh sách người dùng";
            return View(users);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Thêm người dùng";
            ViewBag.RoleList = new List<string> { "Admin", "User" };
            return View();
        }

        [HttpPost]
        public IActionResult Create(UserM user, IFormFile imageFile)
        {
            if (_userCollection.Find(u => u.Email == user.Email).Any())
            {
                ModelState.AddModelError("Email", "Email đã được sử dụng");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.RoleList = new List<string> { "Admin", "User" };
                return View(user);
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Path.GetFileName(imageFile.FileName);
                var filePath = Path.Combine("wwwroot/uploads", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(stream);
                }
                user.Image = "/uploads/" + fileName;
            }

            int maxId = _userCollection.AsQueryable().OrderByDescending(u => u.idUser).Select(u => u.idUser).FirstOrDefault();
            user.idUser = maxId + 1;

            _userCollection.InsertOne(user);
            return RedirectToAction("Index");
        }

        public IActionResult Edit(string id)
        {
            var user = _userCollection.Find(u => u._id == MongoDB.Bson.ObjectId.Parse(id)).FirstOrDefault();
            if (user == null) return NotFound();

            ViewBag.Title = "Chỉnh sửa người dùng";
            ViewBag.RoleList = new List<string> { "Admin", "User" };
            return View(user);
        }

        [HttpPost]
        public IActionResult Edit(string id, UserM updatedUser, IFormFile imageFile)
        {
            var existingUser = _userCollection.Find(u => u._id == MongoDB.Bson.ObjectId.Parse(id)).FirstOrDefault();
            if (existingUser == null) return NotFound();

            var emailUsed = _userCollection.Find(u => u.Email == updatedUser.Email && u._id != existingUser._id).Any();
            if (emailUsed)
            {
                ModelState.AddModelError("Email", "Email đã được sử dụng");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.RoleList = new List<string> { "Admin", "User" };
                return View(updatedUser);
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Path.GetFileName(imageFile.FileName);
                var filePath = Path.Combine("wwwroot/uploads", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(stream);
                }
                updatedUser.Image = "/uploads/" + fileName;
            }
            else
            {
                updatedUser.Image = existingUser.Image;
            }

            updatedUser._id = existingUser._id;
            updatedUser.idUser = existingUser.idUser;

            _userCollection.ReplaceOne(u => u._id == existingUser._id, updatedUser);
            return RedirectToAction("Index");
        }
    }
}