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

        public AccountController(BenhVienDbContext benhVienContext, VNeIDDbContext vneIdContext)
        {
            _benhVienContext = benhVienContext;
            _vneIdContext = vneIdContext;
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

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Kiểm tra username đã tồn tại
                var existingUser = await _benhVienContext.TaiKhoans
                    .FirstOrDefaultAsync(t => t.Username == model.Username);

                if (existingUser != null)
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại");
                    return View(model);
                }

                // Kiểm tra email đã tồn tại
                var existingEmail = await _benhVienContext.BenhNhans
                    .FirstOrDefaultAsync(b => b.Email == model.Email);

                if (existingEmail != null)
                {
                    ModelState.AddModelError("Email", "Email đã được sử dụng");
                    return View(model);
                }

                // Kiểm tra CCCD đã tồn tại
                var existingCCCD = await _benhVienContext.BenhNhans
                    .FirstOrDefaultAsync(b => b.CCCD == model.CCCD);

                if (existingCCCD != null)
                {
                    ModelState.AddModelError("CCCD", "Số CCCD đã được đăng ký");
                    return View(model);
                }

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

                _benhVienContext.TaiKhoans.Add(taiKhoan);
                await _benhVienContext.SaveChangesAsync();

                // Tạo bệnh nhân mới
                var benhNhan = new BenhNhan
                {
                    MaBenhNhan = GenerateId("BN"),
                    MaTaiKhoan = taiKhoan.MaTaiKhoan,
                    TenBenhNhan = model.HoTen,
                    NgaySinh = model.NgaySinh,
                    GioiTinh = model.GioiTinh,
                    CCCD = model.CCCD,
                    DiaChi = model.DiaChi,
                    SoDienThoai = model.SoDienThoai,
                    Email = model.Email,
                    NhomMau = model.NhomMau ?? "Chưa xác định",
                    NgheNghiep = model.NgheNghiep ?? "",
                    TienSuDiUng = model.TienSuDiUng ?? "",
                    NgayTao = DateTime.Now
                };

                _benhVienContext.BenhNhans.Add(benhNhan);
                await _benhVienContext.SaveChangesAsync();

                TempData["SuccessMessage"] = "Đăng ký tài khoản thành công! Vui lòng đăng nhập.";
                return RedirectToAction(nameof(Login));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi trong quá trình đăng ký: " + ex.Message);
                return View(model);
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