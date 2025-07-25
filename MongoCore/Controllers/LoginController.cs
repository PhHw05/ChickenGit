using Microsoft.AspNetCore.Mvc;
using MongoCore.Models.ModelViews;
using System.Threading.Tasks;
using System.Text.Json;
using MongoDB.Driver;
using Microsoft.AspNetCore.Http;
using BCrypt.Net; // Thêm dòng này để sử dụng BCrypt
using System.Linq; // Cần cho .Any() và .FirstOrDefault()
using System; // Cần cho ArgumentNullException, Console.WriteLine

namespace MongoCore.Controllers
{
    public class LoginController : Controller
    {
        private readonly IMongoCollection<UserM> _usersCollection;

        public LoginController(IMongoDatabase database)
        {
            _usersCollection = database.GetCollection<UserM>("User"); // Đảm bảo tên collection là "User"
        }

        // --- Action để hiển thị form đăng nhập (GET) ---
        [HttpGet]
        public IActionResult Login()
        {
            var currentUserJson = HttpContext.Session.GetString("CurrentUser");
            if (!string.IsNullOrEmpty(currentUserJson))
            {
                UserM currentUser = null;
                try
                {
                    currentUser = JsonSerializer.Deserialize<UserM>(currentUserJson);
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Lỗi deserialize UserM trong Login GET: {ex.Message}");
                }

                if (currentUser != null && currentUser.Role == "Admin")
                {
                    return RedirectToAction("Index", "Admin", new { area = "Admin" }); // Chuyển hướng đến Admin Dashboard
                }
                return RedirectToAction("Index", "Home"); // Chuyển hướng đến trang chủ
            }
            return View(new LoginViewModel());
        }

        // --- Action để xử lý form đăng nhập (POST) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var email = model.Email.Trim();
                var password = model.Password.Trim();

                var user = await _usersCollection.Find(u => u.Email == email).FirstOrDefaultAsync();

                if (user != null)
                {
                    if (BCrypt.Net.BCrypt.Verify(password, user.Password))
                    {
                        var userJson = JsonSerializer.Serialize(user);
                        HttpContext.Session.SetString("CurrentUser", userJson);

                        if (user.Role == "Admin")
                        {
                            return RedirectToAction("Index", "Admin", new { area = "Admin" });
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
                }
            }
            return View(model);
        }

        // --- Action Đăng Ký Người Dùng Mới (GET) ---
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        // --- Action Đăng Ký Người Dùng Mới (POST) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem email đã tồn tại chưa
                var existingUser = await _usersCollection.Find(u => u.Email == model.Email.Trim()).FirstOrDefaultAsync();
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng.");
                    return View(model);
                }

                // Tìm idUser lớn nhất để gán idUser mới
                var lastUser = await _usersCollection.Find(_ => true)
                                                    .SortByDescending(u => u.idUser)
                                                    .FirstOrDefaultAsync();
                int nextId = (lastUser?.idUser ?? 0) + 1;

                var newUser = new UserM
                {
                    idUser = nextId,
                    nameUser = model.Name.Trim(),
                    Email = model.Email.Trim(),
                    Password = BCrypt.Net.BCrypt.HashPassword(model.Password.Trim()), // Băm mật khẩu
                    Image = "noimage.png", // Giá trị mặc định
                    Role = "User" // Mặc định là User khi đăng ký, cần thay đổi thủ công thành "Admin" trong DB nếu muốn
                };

                await _usersCollection.InsertOneAsync(newUser);

                TempData["SuccessMessage"] = "Đăng ký tài khoản thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }
            return View(model);
        }

        // --- Action đăng xuất ---
        [HttpGet]
        public IActionResult Logout()
        {
            // Xóa thông tin người dùng khỏi Session
            HttpContext.Session.Remove("CurrentUser");

            // Chuyển hướng về trang chủ thay vì trang đăng nhập
            return RedirectToAction("Index", "Home"); // <--- ĐÃ CHỈNH SỬA TẠI ĐÂY
        }

        // Action TẠM THỜI để băm mật khẩu (chỉ dùng trong dev, xóa khi triển khai thật)
        [HttpGet]
        public IActionResult HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return Content("Vui lòng cung cấp mật khẩu để băm. Ví dụ: /Login/HashPassword?password=123");
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            return Content($"Mật khẩu gốc: \"{password}\"<br/>Mật khẩu đã băm (hashed): \"{hashedPassword}\"");
        }
    }
}