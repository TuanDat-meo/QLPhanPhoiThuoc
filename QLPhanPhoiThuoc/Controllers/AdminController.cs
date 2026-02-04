using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLPhanPhoiThuoc.Models.EF;
using QLPhanPhoiThuoc.Models.Entities;
using System.Security.Claims;

namespace QLPhanPhoiThuoc.Controllers
{
    [Authorize(Roles = "Admin,NhanVien")]
    public class AdminController : Controller
    {
        private readonly BenhVienDbContext _benhVienContext;
        private readonly VNeIDDbContext _vneIdContext;

        public AdminController(BenhVienDbContext benhVienContext, VNeIDDbContext vneIdContext)
        {
            _benhVienContext = benhVienContext;
            _vneIdContext = vneIdContext;
        }

        // GET: /Admin/AdminIndex - Main admin page with dashboard
        public async Task<IActionResult> AdminIndex()
        {
            // Lấy thông tin admin từ claims
            var maNhanVien = User.FindFirstValue("MaNhanVien");
            var tenNhanVien = User.FindFirstValue("TenNhanVien");
            var chucVu = User.FindFirstValue("ChucVu");

            ViewBag.MaNhanVien = maNhanVien;
            ViewBag.TenNhanVien = tenNhanVien ?? "Admin";
            ViewBag.ChucVu = chucVu ?? "Quản trị viên";

            // Lấy số thông báo chưa đọc
            var soThongBaoChuaDoc = await _benhVienContext.ThongBaos
                .Where(t => t.NguoiNhan == maNhanVien && !t.DaDoc)
                .CountAsync();
            ViewBag.SoThongBaoChuaDoc = soThongBaoChuaDoc;

            return View();
        }

        // GET: /Admin/Dashboard - Load Dashboard section
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var maNhanVien = User.FindFirstValue("MaNhanVien");
            var ngayHienTai = DateTime.Now;

            // ==================== THỐNG KÊ DASHBOARD ====================

            // 1. Tổng số bệnh nhân
            var tongBenhNhan = await _benhVienContext.BenhNhans.CountAsync();
            ViewBag.TongBenhNhan = tongBenhNhan;

            // 2. Tổng số đơn thuốc
            var tongDonThuoc = await _benhVienContext.DonThuocs.CountAsync();
            ViewBag.TongDonThuoc = tongDonThuoc;

            // 3. Tổng doanh thu (từ hóa đơn đã thanh toán)
            var tongDoanhThu = await _benhVienContext.HoaDons
                .Where(h => h.TrangThaiThanhToan == "DaTra")
                .SumAsync(h => (decimal?)h.TongTien) ?? 0;
            ViewBag.TongDoanhThu = tongDoanhThu;

            // 4. Số thuốc sắp hết hạn (trong vòng 30 ngày)
            var thuocSapHetHan = await _benhVienContext.LoThuocs
                .Where(l => l.TrangThai == "ConHang" &&
                           l.HanSuDung <= ngayHienTai.AddDays(30) &&
                           l.HanSuDung >= ngayHienTai)
                .CountAsync();
            ViewBag.ThuocSapHetHan = thuocSapHetHan;

            // 5. Số bệnh nhân mới trong tháng
            var dauThang = new DateTime(ngayHienTai.Year, ngayHienTai.Month, 1);
            var benhNhanMoi = await _benhVienContext.BenhNhans
                .Where(b => b.NgayTao >= dauThang)
                .CountAsync();
            ViewBag.BenhNhanMoi = benhNhanMoi;

            // 6. Số đơn thuốc chưa cấp phát
            var donChuaCapPhat = await _benhVienContext.DonThuocs
                .Where(d => d.TrangThai == "ChoCapPhat")
                .CountAsync();
            ViewBag.DonChuaCapPhat = donChuaCapPhat;

            // 7. Tổng số nhân viên
            var tongNhanVien = await _benhVienContext.NhanViens
                .Where(n => n.TrangThai == "DangLamViec")
                .CountAsync();
            ViewBag.TongNhanVien = tongNhanVien;

            // 8. Số lượng thuốc trong kho
            var tongThuoc = await _benhVienContext.Thuocs
                .Where(t => t.TrangThai == "KichHoat")
                .CountAsync();
            ViewBag.TongThuoc = tongThuoc;

            // ==================== BIỂU ĐỒ DOANH THU 7 NGÀY ====================
            var doanhThu7Ngay = new List<decimal>();
            var ngay7Ngay = new List<string>();

            for (int i = 6; i >= 0; i--)
            {
                var ngay = ngayHienTai.AddDays(-i);
                var doanhThuNgay = await _benhVienContext.HoaDons
                    .Where(h => h.NgayTaoHoaDon.Date == ngay.Date &&
                               h.TrangThaiThanhToan == "DaTra")
                    .SumAsync(h => (decimal?)h.TongTien) ?? 0;

                doanhThu7Ngay.Add(doanhThuNgay / 1000000); // Chuyển sang triệu đồng
                ngay7Ngay.Add(ngay.ToString("dd/MM"));
            }

            ViewBag.DoanhThu7Ngay = doanhThu7Ngay;
            ViewBag.Ngay7Ngay = ngay7Ngay;

            // ==================== PHÂN LOẠI BỆNH NHÂN (BHYT vs Viện phí) ====================
            var benhNhanBHYT = await _benhVienContext.TheBHYTs
                .Where(t => t.TrangThai == "ConHan")
                .Select(t => t.MaBenhNhan)
                .Distinct()
                .CountAsync();

            var tongBenhNhanHoatDong = await _benhVienContext.BenhNhans.CountAsync();
            var benhNhanVienPhi = tongBenhNhanHoatDong - benhNhanBHYT;

            ViewBag.BenhNhanBHYT = benhNhanBHYT;
            ViewBag.BenhNhanVienPhi = benhNhanVienPhi;

            // ==================== TOP 5 THUỐC BÁN CHẠY ====================
            var top5ThuocBanChay = await _benhVienContext.ChiTietDonThuocs
                .GroupBy(ct => new { ct.MaThuoc, ct.Thuoc.TenThuoc })
                .Select(g => new
                {
                    TenThuoc = g.Key.TenThuoc,
                    SoLuong = g.Sum(ct => ct.SoLuong)
                })
                .OrderByDescending(x => x.SoLuong)
                .Take(5)
                .ToListAsync();

            ViewBag.Top5ThuocBanChay = top5ThuocBanChay;

            // ==================== HOẠT ĐỘNG GẦN ĐÂY ====================
            var hoatDongGanDay = await _benhVienContext.DonThuocs
                .OrderByDescending(d => d.NgayKeDon)
                .Take(5)
                .Select(d => new
                {
                    d.MaDonThuoc,
                    d.NgayKeDon,
                    d.LoaiDon,
                    BenhNhan = d.BenhNhan.TenBenhNhan,
                    BacSi = d.NhanVien.TenNhanVien,
                    d.TrangThai
                })
                .ToListAsync();

            ViewBag.HoatDongGanDay = hoatDongGanDay;

            // ==================== THỐNG KÊ KHO ====================
            var thuocSapHetTonKho = await _benhVienContext.LoThuocs
                .Where(l => l.TrangThai == "ConHang")
                .GroupBy(l => new { l.MaThuoc, l.Thuoc.TenThuoc, l.Thuoc.TonKhoToiThieu })
                .Select(g => new
                {
                    TenThuoc = g.Key.TenThuoc,
                    TonKhoHienTai = g.Sum(l => l.SoLuongCon),
                    TonKhoToiThieu = g.Key.TonKhoToiThieu
                })
                .Where(x => x.TonKhoHienTai <= x.TonKhoToiThieu)
                .OrderBy(x => x.TonKhoHienTai)
                .Take(5)
                .ToListAsync();

            ViewBag.ThuocSapHetTonKho = thuocSapHetTonKho;

            // Nếu là AJAX request, trả về PartialView
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_Dashboard");
            }

            return View("_Dashboard");
        }

        // ==================== API ENDPOINTS CHO DASHBOARD ====================

        // API: Lấy thống kê tổng quan
        [HttpGet]
        public async Task<IActionResult> GetDashboardStats()
        {
            var stats = new
            {
                tongBenhNhan = await _benhVienContext.BenhNhans.CountAsync(),
                tongDonThuoc = await _benhVienContext.DonThuocs.CountAsync(),
                tongDoanhThu = await _benhVienContext.HoaDons
                    .Where(h => h.TrangThaiThanhToan == "DaTra")
                    .SumAsync(h => (decimal?)h.TongTien) ?? 0,
                tongNhanVien = await _benhVienContext.NhanViens
                    .Where(n => n.TrangThai == "DangLamViec")
                    .CountAsync()
            };

            return Json(stats);
        }

        // API: Lấy dữ liệu biểu đồ doanh thu theo tháng
        [HttpGet]
        public async Task<IActionResult> GetRevenueByMonth(int year)
        {
            var revenueByMonth = new List<decimal>();

            for (int month = 1; month <= 12; month++)
            {
                var dauThang = new DateTime(year, month, 1);
                var cuoiThang = dauThang.AddMonths(1).AddDays(-1);

                var doanhThu = await _benhVienContext.HoaDons
                    .Where(h => h.NgayTaoHoaDon >= dauThang &&
                               h.NgayTaoHoaDon <= cuoiThang &&
                               h.TrangThaiThanhToan == "DaTra")
                    .SumAsync(h => (decimal?)h.TongTien) ?? 0;

                revenueByMonth.Add(doanhThu / 1000000); // Triệu đồng
            }

            return Json(new { months = new[] { "T1", "T2", "T3", "T4", "T5", "T6", "T7", "T8", "T9", "T10", "T11", "T12" }, revenue = revenueByMonth });
        }

        // API: Lấy danh sách thuốc cần nhập thêm
        [HttpGet]
        public async Task<IActionResult> GetLowStockMedicines()
        {
            var lowStockMeds = await _benhVienContext.LoThuocs
                .Where(l => l.TrangThai == "ConHang")
                .GroupBy(l => new { l.MaThuoc, l.Thuoc.TenThuoc, l.Thuoc.TonKhoToiThieu })
                .Select(g => new
                {
                    maThuoc = g.Key.MaThuoc,
                    tenThuoc = g.Key.TenThuoc,
                    tonKhoHienTai = g.Sum(l => l.SoLuongCon),
                    tonKhoToiThieu = g.Key.TonKhoToiThieu
                })
                .Where(x => x.tonKhoHienTai <= x.tonKhoToiThieu)
                .OrderBy(x => x.tonKhoHienTai)
                .ToListAsync();

            return Json(lowStockMeds);
        }

        // API: Lấy danh sách thuốc sắp hết hạn
        [HttpGet]
        public async Task<IActionResult> GetExpiringMedicines(int days = 30)
        {
            var ngayHienTai = DateTime.Now;
            var expiringMeds = await _benhVienContext.LoThuocs
                .Where(l => l.TrangThai == "ConHang" &&
                           l.HanSuDung <= ngayHienTai.AddDays(days) &&
                           l.HanSuDung >= ngayHienTai)
                .Include(l => l.Thuoc)
                .Include(l => l.Kho)
                .OrderBy(l => l.HanSuDung)
                .Select(l => new
                {
                    maLo = l.MaLo,
                    tenThuoc = l.Thuoc.TenThuoc,
                    soLo = l.SoLo,
                    ngayHetHan = l.HanSuDung,
                    soLuongCon = l.SoLuongCon,
                    tenKho = l.Kho.TenKho,
                    soNgayConLai = (l.HanSuDung - ngayHienTai).Days
                })
                .ToListAsync();

            return Json(expiringMeds);
        }

        // API: Lấy thống kê bệnh nhân theo tháng
        [HttpGet]
        public async Task<IActionResult> GetPatientStatsByMonth(int year)
        {
            var patientsByMonth = new List<int>();

            for (int month = 1; month <= 12; month++)
            {
                var dauThang = new DateTime(year, month, 1);
                var cuoiThang = dauThang.AddMonths(1).AddDays(-1);

                var soBenhNhan = await _benhVienContext.BenhNhans
                    .Where(b => b.NgayTao >= dauThang && b.NgayTao <= cuoiThang)
                    .CountAsync();

                patientsByMonth.Add(soBenhNhan);
            }

            return Json(new { months = new[] { "T1", "T2", "T3", "T4", "T5", "T6", "T7", "T8", "T9", "T10", "T11", "T12" }, patients = patientsByMonth });
        }
    }
}