using Microsoft.AspNetCore.Mvc;
using MongoCore.Models.ModelViews;
using MongoCore.Models.Repositoties;
using System.Text.Json;

public class ProfileController : Controller
{
    private readonly UserRepository _userRepo;
    private readonly IWebHostEnvironment _env;

    public ProfileController(UserRepository userRepo, IWebHostEnvironment env)
    {
        _userRepo = userRepo;
        _env = env;
    }

    public IActionResult Profile()
    {
        var userJson = HttpContext.Session.GetString("CurrentUser");
        if (string.IsNullOrEmpty(userJson)) return RedirectToAction("Login", "Login");

        var user = JsonSerializer.Deserialize<UserM>(userJson);
        return View(user);
    }

    [HttpPost]
    public IActionResult UpdateProfile(UserM user, IFormFile ImageFile)
    {
        if (!ModelState.IsValid)
        {
            return View("Profile", user);
        }

        var existingUser = _userRepo.GetById(user._id.ToString());
        if (existingUser == null)
        {
            return NotFound();
        }

        if (ImageFile != null && ImageFile.Length > 0)
        {
            string uploadFolder = Path.Combine(_env.WebRootPath, "images");
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
            string filePath = Path.Combine(uploadFolder, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                ImageFile.CopyTo(fileStream);
            }

            user.Image = "/images/" + fileName;
        }
        else
        {
            user.Image = existingUser.Image;
        }

        user.Role = existingUser.Role;
        user.idUser = existingUser.idUser;

        _userRepo.Update(user);

        HttpContext.Session.SetString("CurrentUser", JsonSerializer.Serialize(user));

        TempData["Success"] = "Cập nhật thành công!";
        return RedirectToAction("Profile");
    }
}
