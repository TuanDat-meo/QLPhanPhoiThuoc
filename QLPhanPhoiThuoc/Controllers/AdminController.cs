using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLPhanPhoiThuoc.Models.EF;
using System.Security.Claims;

namespace QLPhanPhoiThuoc.Controllers
{
    [Authorize(Roles = "QuanTriVien")] // Chỉ Admin mới vào được
    public class AdminController : Controller
    {
        private readonly BenhVienDbContext _context;

        public AdminController(BenhVienDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Index - Dashboard quản trị
        public async Task<IActionResult> Index()
        {
            var username = User.Identity.Name;
            var fullName = User.FindFirst("FullName")?.Value ?? username;

            // Thống kê tổng quan
            ViewBag.TongSoThuoc = await _context.Thuocs.CountAsync();
            ViewBag.TongSoBenhNhan = await _context.BenhNhans.CountAsync();
            ViewBag.TongSoNhanVien = await _context.NhanViens.CountAsync();
            ViewBag.DonThuocHomNay = await _context.DonThuocs
                .Where(d => d.NgayKeDon.Date == DateTime.Today)
                .CountAsync();

            // Thuốc sắp hết (tồn kho < tồn kho tối thiểu)
            // FIX: SoLuongTon is now mapped in BenhVienDbContext (see the added
            //      Property + HasDefaultValue in the LoThuoc configuration).
            ViewBag.ThuocSapHet = await _context.LoThuocs
                .Include(l => l.Thuoc)
                .Where(l => l.SoLuongCon < l.Thuoc.TonKhoToiThieu && l.SoLuongCon > 0)
                .CountAsync();

            ViewBag.UserName = fullName;
            return View();
        }

        // GET: Admin/QuanLyThuoc
        public async Task<IActionResult> QuanLyThuoc()
        {
            var thuocs = await _context.Thuocs
                .Where(t => t.TrangThai == "KichHoat")
                .OrderBy(t => t.TenThuoc)
                .ToListAsync();

            return View(thuocs);
        }

        // GET: Admin/QuanLyNhanVien
        public async Task<IActionResult> QuanLyNhanVien()
        {
            var nhanViens = await _context.NhanViens
                .Include(n => n.KhoaPhong)
                .Where(n => n.TrangThai == "DangLamViec")
                .OrderBy(n => n.TenNhanVien)
                .ToListAsync();

            return View(nhanViens);
        }

        // GET: Admin/QuanLyBenhNhan
        public async Task<IActionResult> QuanLyBenhNhan()
        {
            var benhNhans = await _context.BenhNhans
                .Where(b => b.TrangThai == "HoatDong")
                .OrderByDescending(b => b.NgayTao)
                .Take(100)
                .ToListAsync();

            return View(benhNhans);
        }

        // GET: Admin/BaoCao
        public IActionResult BaoCao()
        {
            return View();
        }

        // GET: Admin/CauHinh
        public IActionResult CauHinh()
        {
            return View();
        }

        // API: Lấy thống kê nhanh
        [HttpGet]
        public async Task<IActionResult> GetThongKe()
        {
            var thongKe = new
            {
                TongThuoc = await _context.Thuocs.CountAsync(),
                TongBenhNhan = await _context.BenhNhans.CountAsync(),
                TongNhanVien = await _context.NhanViens.CountAsync(),
                DonThuocThang = await _context.DonThuocs
                    .Where(d => d.NgayKeDon.Month == DateTime.Now.Month)
                    .CountAsync(),
                DoanhThuThang = await _context.HoaDons
                    .Where(h => h.NgayTaoHoaDon.Month == DateTime.Now.Month)
                    .SumAsync(h => (decimal?)h.TongTien) ?? 0
            };

            return Json(thongKe);
        }
    }
}