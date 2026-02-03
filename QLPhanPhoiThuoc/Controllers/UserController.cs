using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLPhanPhoiThuoc.Models.EF;
using QLPhanPhoiThuoc.Models.Entities;
using System.Security.Claims;

namespace QLPhanPhoiThuoc.Controllers
{
    [Authorize(Roles = "BenhNhan")] // Chỉ User / Bệnh nhân mới vào được
    public class UserController : Controller
    {
        private readonly BenhVienDbContext _context;
        private readonly VNeIDDbContext _vneIDContext;

        public UserController(BenhVienDbContext context, VNeIDDbContext vneIDContext)
        {
            _context = context;
            _vneIDContext = vneIDContext;
        }

        // ---------------------------------------------------------------------------
        // Helper: tìm BenhNhan từ tài khoản đang đăng nhập.
        // FIX: TaiKhoan không có Email.  Đăng ký Register đã gán
        //      TaiKhoan.MaNhanVien = BenhNhan.MaBenhNhan, nên join qua đó.
        // ---------------------------------------------------------------------------
        private async Task<(TaiKhoan taiKhoan, BenhNhan benhNhan)> GetCurrentPatientAsync()
        {
            var username = User.Identity.Name;

            // FIX: TenDangNhap → Username
            var taiKhoan = await _context.TaiKhoans
                .FirstOrDefaultAsync(t => t.Username == username);

            if (taiKhoan == null) return (null, null);

            // FIX: lookup via MaNhanVien (the FK that links TaiKhoan → BenhNhan)
            var benhNhan = await _context.BenhNhans
                .Include(b => b.DonThuocs)
                .Include(b => b.HoaDons)
                .FirstOrDefaultAsync(b => b.MaBenhNhan == taiKhoan.MaNhanVien);

            return (taiKhoan, benhNhan);
        }

        // GET: User/Index - Dashboard bệnh nhân
        public async Task<IActionResult> Index()
        {
            var username = User.Identity.Name;
            var fullName = User.FindFirst("FullName")?.Value ?? username;

            var (taiKhoan, benhNhan) = await GetCurrentPatientAsync();

            if (taiKhoan == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Thống kê
            ViewBag.TongDonThuoc = benhNhan?.DonThuocs.Count ?? 0;
            ViewBag.DonThuocChuaNhan = benhNhan?.DonThuocs.Count(d => d.TrangThai == "ChoLayThuoc") ?? 0;
            ViewBag.TongHoaDon = benhNhan?.HoaDons.Count ?? 0;
            ViewBag.TongTienNo = benhNhan?.HoaDons
                .Where(h => h.TrangThaiThanhToan != "DaTra")
                .Sum(h => h.TienBenhNhanCanTra - h.TienDaTra) ?? 0;

            ViewBag.UserName = fullName;
            ViewBag.BenhNhan = benhNhan;

            return View();
        }

        // GET: User/DonThuocCuaToi
        public async Task<IActionResult> DonThuocCuaToi()
        {
            var (taiKhoan, benhNhan) = await GetCurrentPatientAsync();

            if (taiKhoan == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (benhNhan == null)
            {
                ViewBag.Message = "Không tìm thấy thông tin bệnh nhân.";
                return View(new List<QLPhanPhoiThuoc.Models.Entities.DonThuoc>());
            }

            var donThuocs = await _context.DonThuocs
                .Include(d => d.BenhNhan)
                .Include(d => d.ChanDoan)
                .Include(d => d.ChiTietDonThuocs)
                    .ThenInclude(ct => ct.Thuoc)
                .Where(d => d.MaBenhNhan == benhNhan.MaBenhNhan)
                .OrderByDescending(d => d.NgayKeDon)
                .ToListAsync();

            ViewBag.UserName = User.FindFirst("FullName")?.Value ?? User.Identity.Name;
            return View(donThuocs);
        }

        // GET: User/LichSuKham
        public async Task<IActionResult> LichSuKham()
        {
            var (taiKhoan, benhNhan) = await GetCurrentPatientAsync();

            if (taiKhoan == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (benhNhan == null)
            {
                ViewBag.Message = "Không tìm thấy thông tin bệnh nhân.";
                return View(new List<QLPhanPhoiThuoc.Models.Entities.ChanDoan>());
            }

            // FIX: NgayKham → NgayChanDoan  (actual column name in ChanDoan entity)
            var lichSuKham = await _context.ChanDoans
                .Include(c => c.BenhNhan)
                .Include(c => c.NhanVien)
                .Where(c => c.MaBenhNhan == benhNhan.MaBenhNhan)
                .OrderByDescending(c => c.NgayChanDoan)   // FIX
                .ToListAsync();

            ViewBag.UserName = User.FindFirst("FullName")?.Value ?? User.Identity.Name;
            return View(lichSuKham);
        }

        // GET: User/ThongTinCaNhan
        public async Task<IActionResult> ThongTinCaNhan()
        {
            var username = User.Identity.Name;

            // FIX: TenDangNhap → Username
            var taiKhoan = await _context.TaiKhoans
                .FirstOrDefaultAsync(t => t.Username == username);

            if (taiKhoan == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // FIX: TheBHYT → TheBHYTs  (the navigation property is a collection)
            var benhNhan = await _context.BenhNhans
                .Include(b => b.TheBHYTs)                          // FIX
                .FirstOrDefaultAsync(b => b.MaBenhNhan == taiKhoan.MaNhanVien);

            // Lấy thông tin từ VNeID nếu có CCCD
            if (benhNhan != null && !string.IsNullOrEmpty(benhNhan.CCCD))
            {
                var congDan = await _vneIDContext.CongDans
                    .FirstOrDefaultAsync(c => c.SoDinhDanh == benhNhan.CCCD);

                ViewBag.CongDan = congDan;

                var theBHYTMock = await _vneIDContext.TheBHYTMocks
                    .FirstOrDefaultAsync(t => t.SoDinhDanh == benhNhan.CCCD && t.TrangThai == "ConHan");

                ViewBag.TheBHYTMock = theBHYTMock;
            }

            ViewBag.UserName = User.FindFirst("FullName")?.Value ?? username;
            ViewBag.TaiKhoan = taiKhoan;
            return View(benhNhan);
        }

        // GET: User/HoaDonCuaToi
        public async Task<IActionResult> HoaDonCuaToi()
        {
            var (taiKhoan, benhNhan) = await GetCurrentPatientAsync();

            if (taiKhoan == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (benhNhan == null)
            {
                ViewBag.Message = "Không tìm thấy thông tin bệnh nhân.";
                return View(new List<QLPhanPhoiThuoc.Models.Entities.HoaDon>());
            }

            var hoaDons = await _context.HoaDons
                .Include(h => h.BenhNhan)
                .Include(h => h.DonThuoc)
                .Include(h => h.PhieuThuTiens)
                .Where(h => h.MaBenhNhan == benhNhan.MaBenhNhan)
                .OrderByDescending(h => h.NgayTaoHoaDon)
                .ToListAsync();

            ViewBag.UserName = User.FindFirst("FullName")?.Value ?? User.Identity.Name;
            return View(hoaDons);
        }
    }
}