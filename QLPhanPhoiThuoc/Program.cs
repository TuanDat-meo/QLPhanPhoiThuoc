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

// Add Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add Controllers with Views
builder.Services.AddControllersWithViews(options =>
{
    // Add global filters if needed
    // options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});

// Add Razor Pages
builder.Services.AddRazorPages();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Add Memory Cache
builder.Services.AddMemoryCache();

// Add Response Compression
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
    app.UseHsts();
}

// 2. HTTPS Redirection (commented out for development)
// app.UseHttpsRedirection();

// 3. Static Files
app.UseStaticFiles();

// 4. Response Compression
app.UseResponseCompression();

// 5. Routing
app.UseRouting();

// 6. CORS
app.UseCors("AllowAll");

// 7. Session
app.UseSession();

// 8. Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

//// 9. Map Controllers with Area support
//app.MapControllerRoute(
//    name: "areas",
//    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();