using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLPhanPhoiThuoc.Models.EF;
using QLPhanPhoiThuoc.Models.Entities;
using QLPhanPhoiThuoc.Models.ViewModels;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace QLPhanPhoiThuoc.Controllers
{
    public class AccountController : Controller
    {
        private readonly BenhVienDbContext _benhVienContext;
        private readonly VNeIDDbContext _vneIdContext;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            BenhVienDbContext benhVienContext,
            VNeIDDbContext vneIdContext,
            ILogger<AccountController> logger)
        {
            _benhVienContext = benhVienContext;
            _vneIdContext = vneIdContext;
            _logger = logger;
        }

        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            // Nếu đã đăng nhập, chuyển đến trang chủ
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToHomePage();
            }

            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Tìm tài khoản trong database
                var taiKhoan = await _benhVienContext.TaiKhoans
                    .FirstOrDefaultAsync(t => t.Username == model.Username && t.TrangThai == "HoatDong");

                if (taiKhoan == null)
                {
                    ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không chính xác");
                    return View(model);
                }

                // Kiểm tra mật khẩu
                string hashedPassword = HashPassword(model.Password);
                if (taiKhoan.PasswordHash != hashedPassword)
                {
                    ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không chính xác");
                    return View(model);
                }

                // Tạo claims cho user
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, taiKhoan.MaTaiKhoan),
                    new Claim(ClaimTypes.Name, taiKhoan.Username),
                    new Claim(ClaimTypes.Role, taiKhoan.Role)
                };

                // Nếu là user (bệnh nhân), thêm thông tin bệnh nhân
                if (taiKhoan.Role == "User")
                {
                    var benhNhan = await _benhVienContext.BenhNhans
                        .FirstOrDefaultAsync(b => b.MaTaiKhoan == taiKhoan.MaTaiKhoan);

                    if (benhNhan != null)
                    {
                        claims.Add(new Claim("MaBenhNhan", benhNhan.MaBenhNhan));
                        claims.Add(new Claim("TenBenhNhan", benhNhan.TenBenhNhan ?? ""));
                    }
                }
                // Nếu là admin/nhân viên, thêm thông tin nhân viên
                else if (taiKhoan.Role == "Admin" || taiKhoan.Role == "NhanVien")
                {
                    var nhanVien = await _benhVienContext.NhanViens
                        .FirstOrDefaultAsync(n => n.MaTaiKhoan == taiKhoan.MaTaiKhoan);

                    if (nhanVien != null)
                    {
                        claims.Add(new Claim("MaNhanVien", nhanVien.MaNhanVien));
                        claims.Add(new Claim("TenNhanVien", nhanVien.TenNhanVien ?? ""));
                        claims.Add(new Claim("ChucVu", nhanVien.ChucVu ?? ""));
                    }
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(8)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // Cập nhật thời gian đăng nhập cuối
                taiKhoan.NgayCapNhat = DateTime.Now;
                await _benhVienContext.SaveChangesAsync();

                // Chuyển hướng dựa trên role
                return RedirectToHomePage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đăng nhập với username: {Username}", model.Username);
                ModelState.AddModelError("", "Đã xảy ra lỗi trong quá trình đăng nhập: " + ex.Message);
                return View(model);
            }
        }

        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register - Xử lý đăng ký 3 bước
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            _logger.LogInformation("Bắt đầu xử lý đăng ký cho CCCD: {CCCD}", model.CCCD);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState không hợp lệ. Errors: {Errors}",
                    string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                return View(model);
            }

            try
            {
                _logger.LogInformation("Kiểm tra username đã tồn tại: {Username}", model.Username);

                // Kiểm tra username đã tồn tại
                var existingUser = await _benhVienContext.TaiKhoans
                    .FirstOrDefaultAsync(t => t.Username == model.Username);

                if (existingUser != null)
                {
                    _logger.LogWarning("Username đã tồn tại: {Username}", model.Username);
                    ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại");
                    return View(model);
                }

                _logger.LogInformation("Kiểm tra email đã tồn tại: {Email}", model.Email);

                // Kiểm tra email đã tồn tại
                var existingEmail = await _benhVienContext.BenhNhans
                    .FirstOrDefaultAsync(b => b.Email == model.Email);

                if (existingEmail != null)
                {
                    _logger.LogWarning("Email đã được sử dụng: {Email}", model.Email);
                    ModelState.AddModelError("Email", "Email đã được sử dụng");
                    return View(model);
                }

                _logger.LogInformation("Kiểm tra CCCD đã tồn tại: {CCCD}", model.CCCD);

                // Kiểm tra CCCD đã tồn tại
                var existingCCCD = await _benhVienContext.BenhNhans
                    .FirstOrDefaultAsync(b => b.CCCD == model.CCCD);

                if (existingCCCD != null)
                {
                    _logger.LogWarning("CCCD đã được đăng ký: {CCCD}", model.CCCD);
                    ModelState.AddModelError("CCCD", "Số CCCD đã được đăng ký");
                    return View(model);
                }

                _logger.LogInformation("Tạo tài khoản mới cho username: {Username}", model.Username);

                // Tạo tài khoản mới
                var taiKhoan = new TaiKhoan
                {
                    MaTaiKhoan = GenerateId("TK"),
                    Username = model.Username,
                    PasswordHash = HashPassword(model.Password),
                    Role = "User",
                    TrangThai = "HoatDong",
                    NgayTao = DateTime.Now,
                    NgayCapNhat = DateTime.Now
                };

                _logger.LogInformation("Thêm tài khoản vào database: {MaTaiKhoan}", taiKhoan.MaTaiKhoan);
                _benhVienContext.TaiKhoans.Add(taiKhoan);

                try
                {
                    await _benhVienContext.SaveChangesAsync();
                    _logger.LogInformation("Đã lưu tài khoản thành công: {MaTaiKhoan}", taiKhoan.MaTaiKhoan);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi lưu tài khoản: {MaTaiKhoan}", taiKhoan.MaTaiKhoan);
                    throw;
                }

                _logger.LogInformation("Tạo bệnh nhân mới cho CCCD: {CCCD}", model.CCCD);

                // Tạo bệnh nhân mới
                var benhNhan = new BenhNhan
                {
                    MaBenhNhan = GenerateId("BN"),
                    MaTaiKhoan = taiKhoan.MaTaiKhoan,
                    TenBenhNhan = model.HoTen,
                    NgaySinh = model.NgaySinh,
                    GioiTinh = model.GioiTinh,
                    CCCD = model.CCCD,
                    DiaChi = model.DiaChi ?? "",
                    SoDienThoai = model.SoDienThoai,
                    Email = model.Email,
                    NhomMau = model.NhomMau ?? "Chưa xác định",
                    NgheNghiep = model.NgheNghiep ?? "",
                    TienSuDiUng = model.TienSuDiUng ?? "",
                    TrangThai = "HoatDong",
                    LoaiBenhNhan = "NgoaiTru",
                    NgayTao = DateTime.Now
                };

                // Note: Không set Avatar vì cột này không tồn tại trong database

                _logger.LogInformation("Thêm bệnh nhân vào database: {MaBenhNhan}", benhNhan.MaBenhNhan);
                _benhVienContext.BenhNhans.Add(benhNhan);

                try
                {
                    await _benhVienContext.SaveChangesAsync();
                    _logger.LogInformation("Đã lưu bệnh nhân thành công: {MaBenhNhan}", benhNhan.MaBenhNhan);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi lưu bệnh nhân: {MaBenhNhan}", benhNhan.MaBenhNhan);
                    throw;
                }

                _logger.LogInformation("Đăng ký thành công cho username: {Username}", model.Username);
                TempData["SuccessMessage"] = "Đăng ký tài khoản thành công! Vui lòng đăng nhập.";
                return RedirectToAction(nameof(Login));
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Lỗi database khi đăng ký cho CCCD: {CCCD}. InnerException: {InnerException}",
                    model.CCCD, dbEx.InnerException?.Message);

                ModelState.AddModelError("", $"Lỗi database: {dbEx.InnerException?.Message ?? dbEx.Message}");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định khi đăng ký cho CCCD: {CCCD}", model.CCCD);

                ModelState.AddModelError("", "Đã xảy ra lỗi trong quá trình đăng ký: " + ex.Message);
                return View(model);
            }
        }

        // API: Tra cứu thông tin VNeID theo CCCD
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LookupVNeID([FromBody] string cccd)
        {
            try
            {
                _logger.LogInformation("Tra cứu VNeID cho CCCD: {CCCD}", cccd);

                if (string.IsNullOrEmpty(cccd) || cccd.Length != 12)
                {
                    _logger.LogWarning("CCCD không hợp lệ: {CCCD}", cccd);
                    return Json(new { success = false, message = "Số CCCD không hợp lệ" });
                }

                // Tìm trong VNeID database
                var congDan = await _vneIdContext.CongDans
                    .FirstOrDefaultAsync(c => c.SoDinhDanh == cccd);

                if (congDan == null)
                {
                    _logger.LogWarning("Không tìm thấy công dân với CCCD: {CCCD}", cccd);
                    return Json(new { success = false, message = "Không tìm thấy thông tin công dân với số CCCD này" });
                }

                _logger.LogInformation("Tìm thấy công dân: {HoTen}", congDan.HoTen);

                // Trả về thông tin
                return Json(new
                {
                    success = true,
                    data = new
                    {
                        soDinhDanh = congDan.SoDinhDanh,
                        hoTen = congDan.HoTen,
                        ngaySinh = congDan.NgaySinh.ToString("yyyy-MM-dd"),
                        gioiTinh = congDan.GioiTinh,
                        queQuan = congDan.QueQuan,
                        noiThuongTru = congDan.NoiThuongTru,
                        diaChiHienTai = congDan.DiaChiHienTai
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tra cứu VNeID cho CCCD: {CCCD}", cccd);
                return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message });
            }
        }

        // GET: /Account/ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Tìm bệnh nhân có email này
                var benhNhan = await _benhVienContext.BenhNhans
                    .Include(b => b.TaiKhoan)
                    .FirstOrDefaultAsync(b => b.Email == model.Email);

                if (benhNhan == null || benhNhan.TaiKhoan == null)
                {
                    // Không tiết lộ email có tồn tại hay không
                    TempData["SuccessMessage"] = "Nếu email tồn tại trong hệ thống, chúng tôi đã gửi hướng dẫn đặt lại mật khẩu đến email của bạn.";
                    return RedirectToAction(nameof(Login));
                }

                // TODO: Implement email sending logic here
                // Tạm thời chỉ hiển thị thông báo

                TempData["SuccessMessage"] = "Hướng dẫn đặt lại mật khẩu đã được gửi đến email của bạn.";
                return RedirectToAction(nameof(Login));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý quên mật khẩu cho email: {Email}", model.Email);
                ModelState.AddModelError("", "Đã xảy ra lỗi: " + ex.Message);
                return View(model);
            }
        }

        // GET: /Account/ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token = "")
        {
            var model = new ResetPasswordViewModel
            {
                Token = token
            };
            return View(model);
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // TODO: Verify token logic here
                // Tìm tài khoản dựa trên email hoặc số điện thoại
                var benhNhan = await _benhVienContext.BenhNhans
                    .Include(b => b.TaiKhoan)
                    .FirstOrDefaultAsync(b => b.Email == model.EmailOrPhone || b.SoDienThoai == model.EmailOrPhone);

                if (benhNhan == null || benhNhan.TaiKhoan == null)
                {
                    ModelState.AddModelError("", "Không tìm thấy tài khoản");
                    return View(model);
                }

                // Cập nhật mật khẩu
                benhNhan.TaiKhoan.PasswordHash = HashPassword(model.NewPassword);
                benhNhan.TaiKhoan.NgayCapNhat = DateTime.Now;

                await _benhVienContext.SaveChangesAsync();

                TempData["SuccessMessage"] = "Đặt lại mật khẩu thành công! Vui lòng đăng nhập.";
                return RedirectToAction(nameof(Login));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đặt lại mật khẩu");
                ModelState.AddModelError("", "Đã xảy ra lỗi: " + ex.Message);
                return View(model);
            }
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        // GET: /Account/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        #region Helper Methods

        // Chuyển hướng đến trang chủ dựa trên role
        private IActionResult RedirectToHomePage()
        {
            var role = User.FindFirstValue(ClaimTypes.Role);

            return role switch
            {
                "Admin" => RedirectToAction("AdminIndex", "Admin"),
                "User" => RedirectToAction("UserIndex", "User"),
                "NhanVien" => RedirectToAction("AdminIndex", "Admin"),
                _ => RedirectToAction("Login", "Account")
            };
        }

        // Hash mật khẩu bằng SHA256
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        // Tạo ID tự động
        private string GenerateId(string prefix)
        {
            return $"{prefix}{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";
        }

        #endregion
    }
}