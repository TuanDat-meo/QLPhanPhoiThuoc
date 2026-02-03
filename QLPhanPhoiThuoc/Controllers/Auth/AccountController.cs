using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLPhanPhoiThuoc.Models.EF;
using QLPhanPhoiThuoc.Models.Entities;
using QLPhanPhoiThuoc.Models.Entities.VNeID;
using QLPhanPhoiThuoc.Models.ViewModels;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;                       // FIX: replaced Newtonsoft

namespace QLPhanPhoiThuoc.Controllers.Auth
{
    // DTO used by LookupVNeID to receive the CCCD string from JSON body.
    // Without this wrapper ASP.NET Core cannot bind a bare string and throws
    // "cannot convert from 'string' to 'System.IO.BinaryReader'".
    public class CccdRequest
    {
        public string Cccd { get; set; }
    }

    public class AccountController : Controller
    {
        private readonly BenhVienDbContext _benhVienContext;
        private readonly VNeIDDbContext _vneIDContext;

        public AccountController(BenhVienDbContext benhVienContext, VNeIDDbContext vneIDContext)
        {
            _benhVienContext = benhVienContext;
            _vneIDContext = vneIDContext;
        }

        // ==================== LOGIN ====================
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // FIX: TenDangNhap → Username  (actual column name in DbContext)
                var taiKhoan = await _benhVienContext.TaiKhoans
                    .Include(t => t.NhanVien)
                    .FirstOrDefaultAsync(t => t.Username == model.Username);

                if (taiKhoan == null)
                {
                    ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng.");
                    return View(model);
                }

                // Kiểm tra trạng thái tài khoản
                if (taiKhoan.TrangThai == "BiKhoa")
                {
                    ModelState.AddModelError(string.Empty, "Tài khoản đã bị khóa. Vui lòng liên hệ quản trị viên.");
                    return View(model);
                }

                // FIX: MatKhauHash → PasswordHash
                if (!VerifyPassword(model.Password, taiKhoan.PasswordHash))
                {
                    ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng.");
                    return View(model);
                }

                // FIX: removed taiKhoan.LanDangNhapCuoi — field does not exist in DbContext.
                // If you need login-time tracking, add the property to the entity and migration first.
                await _benhVienContext.SaveChangesAsync();

                // FIX: TenDangNhap → Username, VaiTro → Role
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, taiKhoan.Username),
                    new Claim(ClaimTypes.NameIdentifier, taiKhoan.MaTaiKhoan),
                    new Claim(ClaimTypes.Role, taiKhoan.Role),
                    new Claim("FullName", taiKhoan.NhanVien?.TenNhanVien ?? "User"),
                    new Claim("MaNhanVien", taiKhoan.MaNhanVien ?? "")
                };

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

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                // FIX: VaiTro → Role
                if (taiKhoan.Role == "QuanTriVien")
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    return RedirectToAction("Index", "User");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi đăng nhập: " + ex.Message);
                return View(model);
            }
        }

        // ==================== REGISTER ====================
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            return View();
        }

        // FIX: [FromBody] string cccd  →  [FromBody] CccdRequest request
        // A bare "string" cannot be bound from a JSON body; wrapping it in a DTO
        // is the standard ASP.NET Core pattern and eliminates the BinaryReader error.
        [HttpPost]
        [AllowAnonymous]
        [Route("Account/LookupVNeID")]
        public async Task<IActionResult> LookupVNeID([FromBody] CccdRequest request)
        {
            try
            {
                var cccd = request?.Cccd;

                if (string.IsNullOrEmpty(cccd) || cccd.Length != 12)
                {
                    return Json(new VNeIDLookupResult
                    {
                        Success = false,
                        Message = "Số CCCD phải có 12 chữ số"
                    });
                }

                var congDan = await _vneIDContext.CongDans
                    .FirstOrDefaultAsync(c => c.SoDinhDanh == cccd);

                if (congDan == null)
                {
                    return Json(new VNeIDLookupResult
                    {
                        Success = false,
                        Message = "Không tìm thấy thông tin công dân với số CCCD này"
                    });
                }

                // FIX: Newtonsoft.Json.JsonConvert.SerializeObject → System.Text.Json.JsonSerializer.Serialize
                var lichSu = new LichSuTraCuu
                {
                    MaTraCuu = "TC" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                    SoDinhDanh = cccd,
                    LoaiTraCuu = "ThongTinCongDan",
                    HeThongTraCuu = "QLThuocBenhVien",
                    IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = Request.Headers["User-Agent"].ToString(),
                    NgayGioTraCuu = DateTime.Now,
                    KetQua = "ThanhCong",
                    DuLieuTraVe = JsonSerializer.Serialize(congDan)   // FIX
                };

                _vneIDContext.LichSuTraCuus.Add(lichSu);
                await _vneIDContext.SaveChangesAsync();

                return Json(new VNeIDLookupResult
                {
                    Success = true,
                    Message = "Tra cứu thành công",
                    Data = new VNeIDCongDanData
                    {
                        SoDinhDanh = congDan.SoDinhDanh,
                        HoTen = congDan.HoTen,
                        NgaySinh = congDan.NgaySinh,
                        GioiTinh = congDan.GioiTinh,
                        QueQuan = congDan.QueQuan,
                        NoiThuongTru = congDan.NoiThuongTru,
                        DiaChiHienTai = congDan.DiaChiHienTai,
                        DanToc = congDan.DanToc,
                        TonGiao = congDan.TonGiao
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new VNeIDLookupResult
                {
                    Success = false,
                    Message = "Lỗi khi tra cứu: " + ex.Message
                });
            }
        }

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
                // FIX: TenDangNhap → Username
                var existingUser = await _benhVienContext.TaiKhoans
                    .FirstOrDefaultAsync(t => t.Username == model.Username);

                if (existingUser != null)
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập đã được sử dụng");
                    return View(model);
                }

                var existingBN = await _benhVienContext.BenhNhans
                    .FirstOrDefaultAsync(b => b.CCCD == model.CCCD);

                if (existingBN != null)
                {
                    ModelState.AddModelError("CCCD", "Số CCCD này đã được đăng ký tài khoản");
                    return View(model);
                }

                // FIX: HoTen → TenBenhNhan (actual column).
                //      NgaySinh does not exist on BenhNhan in the current schema —
                //      removed here.  If you need it, add the property + migration.
                var maBenhNhan = "BN" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var benhNhan = new BenhNhan
                {
                    MaBenhNhan = maBenhNhan,
                    TenBenhNhan = model.HoTen,          // FIX: was HoTen
                    // NgaySinh  = model.NgaySinh,       // FIX: removed — not in schema
                    GioiTinh = model.GioiTinh,
                    DiaChi = model.DiaChi,
                    SoDienThoai = model.SoDienThoai,
                    CCCD = model.CCCD,
                    Email = model.Email,          // BenhNhan.Email exists; store it here
                    NhomMau = model.NhomMau,
                    NgheNghiep = model.NgheNghiep,
                    TienSuDiUng = model.TienSuDiUng,
                    TrangThai = "HoatDong",
                    NgayTao = DateTime.Now
                };

                _benhVienContext.BenhNhans.Add(benhNhan);

                // FIX: TenDangNhap → Username, MatKhauHash → PasswordHash, VaiTro → Role.
                //      TaiKhoan has no Email column; it is stored on BenhNhan (above).
                var maTaiKhoan = "TK" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var taiKhoan = new TaiKhoan
                {
                    MaTaiKhoan = maTaiKhoan,
                    MaNhanVien = maBenhNhan,           // links account → patient record
                    Username = model.Username,       // FIX
                    PasswordHash = HashPassword(model.Password), // FIX
                    // Email removed — not on TaiKhoan
                    Role = "BenhNhan",           // FIX
                    TrangThai = "KichHoat",
                    NgayTao = DateTime.Now
                };

                _benhVienContext.TaiKhoans.Add(taiKhoan);
                await _benhVienContext.SaveChangesAsync();

                TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi đăng ký: " + ex.Message);
                return View(model);
            }
        }

        // ==================== FORGOT PASSWORD ====================
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

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
                // FIX: TaiKhoan has no Email.  Look up the account via
                //      BenhNhan.Email → MaBenhNhan → TaiKhoan.MaNhanVien.
                var benhNhan = await _benhVienContext.BenhNhans
                    .FirstOrDefaultAsync(b => b.Email == model.Email);

                TaiKhoan taiKhoan = null;
                if (benhNhan != null)
                {
                    taiKhoan = await _benhVienContext.TaiKhoans
                        .FirstOrDefaultAsync(t => t.MaNhanVien == benhNhan.MaBenhNhan);
                }

                if (taiKhoan == null)
                {
                    // Không tiết lộ email không tồn tại (security best practice)
                    TempData["SuccessMessage"] = "Nếu email này tồn tại, chúng tôi đã gửi hướng dẫn đặt lại mật khẩu.";
                    return RedirectToAction("Login");
                }

                var token = GeneratePasswordResetToken();

                TempData["ResetToken"] = token;
                TempData["ResetEmail"] = model.Email;

                // TODO: Gửi email với link reset password
                // var resetLink = Url.Action("ResetPassword", "Account", new { token, email = model.Email }, Request.Scheme);
                // await _emailService.SendEmailAsync(model.Email, "Đặt lại mật khẩu", resetLink);

                TempData["SuccessMessage"] = "Link đặt lại mật khẩu đã được gửi đến email của bạn.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi: " + ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login");
            }

            var model = new ResetPasswordViewModel
            {
                Token = token,
                Email = email
            };

            return View(model);
        }

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
                // FIX: same Email lookup pattern as ForgotPassword
                var benhNhan = await _benhVienContext.BenhNhans
                    .FirstOrDefaultAsync(b => b.Email == model.Email);

                TaiKhoan taiKhoan = null;
                if (benhNhan != null)
                {
                    taiKhoan = await _benhVienContext.TaiKhoans
                        .FirstOrDefaultAsync(t => t.MaNhanVien == benhNhan.MaBenhNhan);
                }

                if (taiKhoan == null)
                {
                    ModelState.AddModelError(string.Empty, "Token không hợp lệ");
                    return View(model);
                }

                // TODO: Validate token (check expiry, etc.)

                // FIX: MatKhauHash → PasswordHash
                taiKhoan.PasswordHash = HashPassword(model.NewPassword);
                await _benhVienContext.SaveChangesAsync();

                TempData["SuccessMessage"] = "Đặt lại mật khẩu thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi: " + ex.Message);
                return View(model);
            }
        }

        // ==================== LOGOUT ====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // ==================== HELPER METHODS ====================
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            var hash = HashPassword(password);
            return hash == hashedPassword;
        }

        private string GeneratePasswordResetToken()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var bytes = new byte[32];
                rng.GetBytes(bytes);
                return Convert.ToBase64String(bytes);
            }
        }
    }
}