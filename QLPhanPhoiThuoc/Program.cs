using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using QLPhanPhoiThuoc.Models.EF;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<BenhVienDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BenhVienConnection")));

builder.Services.AddDbContext<VNeIDDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("VNeIDConnection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
    });

// 3. Add Session (nếu cần)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 4. Add Controllers với Views
builder.Services.AddControllersWithViews(options =>
{
    // Add global filters nếu cần
    // options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});

// 5. Add Razor Pages (nếu dùng)
builder.Services.AddRazorPages();

// 6. Add CORS (nếu có API)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// 7. Add HttpContextAccessor để truy cập HttpContext trong services
builder.Services.AddHttpContextAccessor();

// 8. Add Memory Cache
builder.Services.AddMemoryCache();

// 9. Add Response Compression (optional - tối ưu performance)
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

// ==================== BUILD APP ====================

var app = builder.Build();

// ==================== CONFIGURE MIDDLEWARE PIPELINE ====================

// 1. Exception Handling
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    
}

// 2. HTTPS Redirection
//app.UseHttpsRedirection();

// 3. Static Files
app.UseStaticFiles();

// 4. Response Compression
app.UseResponseCompression();

// 5. Routing
app.UseRouting();

// 6. CORS (nếu đã add)
app.UseCors("AllowAll");

app.UseSession();

// 8. Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

// Area routes (nếu có Admin area)
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// API routes (nếu có)
app.MapControllerRoute(
    name: "api",
    pattern: "api/{controller}/{action}/{id?}");

// Razor Pages (nếu dùng)
app.MapRazorPages();

// ==================== RUN APP ====================

app.Run();