using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoCore.Models.Repositoties;
using MongoCore.Models.Entities; // Đảm bảo CateM, ProM nằm trong đây nếu chúng là Entities
using MongoCore.Models.ModelViews; // Đảm bảo UserM nằm trong đây nếu nó là ModelViews

var builder = WebApplication.CreateBuilder(args);

// --- Cấu hình MongoDB ---
builder.Services.AddSingleton<IMongoClient>(s =>
{
    var connectionString = builder.Configuration.GetConnectionString("MongoDBConnection");
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("MongoDBConnection connection string is not configured in appsettings.json.");
    }
    return new MongoClient(connectionString);
});

// Đăng ký IMongoDatabase là Singleton để tránh lỗi "Cannot resolve scoped service from root provider"
builder.Services.AddSingleton<IMongoDatabase>(s =>
{
    var client = s.GetRequiredService<IMongoClient>();
    var databaseName = builder.Configuration.GetValue<string>("MongoDB:DatabaseName");
    if (string.IsNullOrEmpty(databaseName))
    {
        throw new InvalidOperationException("MongoDB:DatabaseName is not configured in appsettings.json.");
    }
    return client.GetDatabase(databaseName);
});

// Đăng ký các collection cụ thể (lấy từ IMongoDatabase đã đăng ký)
// Vì IMongoDatabase là Singleton, các collection cũng có thể là Singleton
builder.Services.AddSingleton(s => s.GetRequiredService<IMongoDatabase>().GetCollection<CateM>("Category"));
builder.Services.AddSingleton(s => s.GetRequiredService<IMongoDatabase>().GetCollection<ProM>("Product"));
builder.Services.AddSingleton(s => s.GetRequiredService<IMongoDatabase>().GetCollection<UserM>("User"));


// Đăng ký repository (thường là Scoped hoặc Transient tùy vào logic của Repository)
builder.Services.AddScoped<CateRepository>();
builder.Services.AddScoped<ProRepository>();
builder.Services.AddScoped<UserRepository>();

// Thêm dịch vụ MVC
builder.Services.AddControllersWithViews();

// --- Thêm dịch vụ Session ---
builder.Services.AddDistributedMemoryCache(); // Cần thiết cho Session trong ASP.NET Core
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian session hết hạn sau 30 phút không hoạt động
    options.Cookie.HttpOnly = true; // Cookie chỉ truy cập được bởi server (giảm thiểu tấn công XSS)
    options.Cookie.IsEssential = true; // Cookie là cần thiết cho ứng dụng hoạt động (chấp nhận GDPR)
    options.Cookie.Name = "BonHubSession"; // Tên của session cookie
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// app.UseHttpsRedirection(); // Đã comment bởi bạn
app.UseStaticFiles();

// Order is important: UseRouting must be before UseSession, UseAuthentication, UseAuthorization
app.UseRouting();

// Sử dụng Session Middleware. Phải đặt TRƯỚC app.UseAuthorization()
app.UseSession();

app.UseAuthorization();

// --- Cấu hình Routing cho Areas và Controller ---
// Quan trọng: Route cho Area phải được định nghĩa TRƯỚC route mặc định
app.MapControllerRoute(
    name: "admin",
    pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}" // Mặc định là AdminController
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();