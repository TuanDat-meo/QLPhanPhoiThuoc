using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLPhanPhoiThuoc.Models.EF;
using QLPhanPhoiThuoc.Models;
using QLPhanPhoiThuoc.Models.Entities;

namespace QLPhanPhoiThuoc.Controllers.Admin
{
    [Route("Admin/[controller]")]
    public class PhieuNhapController : Controller
    {
        private readonly BenhVienDbContext _context;

        public PhieuNhapController(BenhVienDbContext context)
        {
            _context = context;
        }

        // GET: Admin/PhieuNhap/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            // Lấy mã nhân viên từ session/claims - Thay đổi logic này theo hệ thống của bạn
            ViewBag.MaNhanVien = User.Identity?.Name ?? "NV001";
            // Hoặc nếu bạn lưu trong Session:
            // ViewBag.MaNhanVien = HttpContext.Session.GetString("MaNhanVien") ?? "NV001";

            // Trỏ trực tiếp đến đường dẫn file trong thư mục Kho
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("~/Views/Kho/PhieuNhap_Create.cshtml");
            }
            return View("~/Views/Kho/PhieuNhap_Create.cshtml");
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] PhieuNhapDTO phieuNhapDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (!ModelState.IsValid) return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });

                // LẤY MÃ NHÂN VIÊN THỰC TẾ TỪ USERNAME 'admin'
                var currentUsername = User.Identity.Name;
                // Sử dụng _context.TaiKhoan (theo DbContext của bạn)
                var account = await _context.TaiKhoans
                    .FirstOrDefaultAsync(t => t.Username == currentUsername);

                if (account == null || string.IsNullOrEmpty(account.MaNhanVien))
                {
                    return Json(new { success = false, message = "Lỗi: Tài khoản đăng nhập chưa liên kết với Mã nhân viên!" });
                }

                var phieuNhap = new PhieuNhap
                {
                    MaPhieuNhap = phieuNhapDto.MaPhieuNhap,
                    MaNCC = phieuNhapDto.MaNCC,
                    MaKho = phieuNhapDto.MaKho,
                    NgayNhap = phieuNhapDto.NgayNhap,
                    SoHoaDon = phieuNhapDto.SoHoaDon ?? "",
                    NhanVienNhap = account.MaNhanVien, // ✅ Dùng account.MaNhanVien đã tìm thấy
                    GhiChu = phieuNhapDto.GhiChu ?? "",
                    TrangThai = "DangNhap",
                    NgayTao = DateTime.Now,
                    ChiTietPhieuNhaps = new List<ChiTietPhieuNhap>()
                };

                decimal tongTien = 0;
                foreach (var item in phieuNhapDto.ChiTiet)
                {
                    var trackedLo = await _context.LoThuoc
                        .FirstOrDefaultAsync(l => l.MaThuoc == item.MaThuoc && l.MaKho == phieuNhapDto.MaKho && l.SoLo == item.SoLo);

                    string finalMaLo;
                    if (trackedLo != null)
                    {
                        trackedLo.SoLuongNhap += item.SoLuong;
                        trackedLo.SoLuongCon += item.SoLuong;
                        finalMaLo = trackedLo.MaLo;
                    }
                    else
                    {
                        var newLo = new LoThuoc
                        {
                            MaLo = "LO" + DateTime.Now.Ticks.ToString().Substring(10),
                            MaThuoc = item.MaThuoc,
                            MaKho = phieuNhapDto.MaKho,
                            SoLo = item.SoLo,
                            HanSuDung = item.HanSuDung,
                            SoLuongNhap = item.SoLuong,
                            SoLuongCon = item.SoLuong,
                            TrangThai = "ConHang",
                            NgayTao = DateTime.Now
                        };
                        _context.LoThuoc.Add(newLo);
                        finalMaLo = newLo.MaLo;
                    }

                    phieuNhap.ChiTietPhieuNhaps.Add(new ChiTietPhieuNhap
                    {
                        MaPhieuNhap = phieuNhap.MaPhieuNhap,
                        MaLo = finalMaLo,
                        SoLuongNhap = item.SoLuong,
                        DonGiaNhap = item.DonGia
                    });
                    tongTien += item.SoLuong * item.DonGia;
                }

                phieuNhap.TongTien = tongTien;
                _context.PhieuNhap.Add(phieuNhap);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Json(new { success = true, message = "Tạo phiếu nhập thành công!" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Json(new { success = false, message = "Lỗi: " + (ex.InnerException?.Message ?? ex.Message) });
            }
        }

        [HttpGet("GetNhaCungCap")]
        public async Task<IActionResult> GetNhaCungCap()
        {
            var data = await _context.NhaCungCap.Where(n => n.TrangThai == "HoatDong").Select(n => new { maNCC = n.MaNCC, tenNCC = n.TenNCC }).ToListAsync();
            return Json(data);
        }

        [HttpGet("GetKho")]
        public async Task<IActionResult> GetKho()
        {
            var data = await _context.Kho.Where(k => k.TrangThai == "HoatDong").Select(k => new { maKho = k.MaKho, tenKho = k.TenKho }).ToListAsync();
            return Json(data);
        }

        [HttpGet("GetThuoc")]
        public async Task<IActionResult> GetThuoc()
        {
            var data = await _context.Thuoc.Where(t => t.TrangThai == "KichHoat").Select(t => new { maThuoc = t.MaThuoc, tenThuoc = t.TenThuoc, donViTinh = t.DonViTinh, giaNhap = t.GiaNhap }).ToListAsync();
            return Json(data);
        }

        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var phieuNhaps = await _context.PhieuNhap.Include(p => p.NhaCungCap).Include(p => p.Kho).Include(p => p.NhanVien).OrderByDescending(p => p.NgayNhap).ToListAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest") return PartialView(phieuNhaps);
            return View(phieuNhaps);
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            var phieuNhap = await _context.PhieuNhap.Include(p => p.NhaCungCap).Include(p => p.Kho).Include(p => p.NhanVien).Include(p => p.ChiTietPhieuNhaps).ThenInclude(c => c.LoThuoc).ThenInclude(l => l!.Thuoc).FirstOrDefaultAsync(p => p.MaPhieuNhap == id);
            if (phieuNhap == null) return NotFound();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest") return PartialView(phieuNhap);
            return View(phieuNhap);
        }

        [HttpGet("GetNhanVien")]
        public async Task<IActionResult> GetNhanVien()
        {
            var data = await _context.NhanVien
                .Where(n => n.TrangThai == "DangLamViec")
                .Select(n => new { maNV = n.MaNhanVien, tenNV = n.TenNhanVien })
                .ToListAsync();
            return Json(data);
        }
    }

    public class PhieuNhapDTO
    {
        public required string MaPhieuNhap { get; set; }
        public required string MaNCC { get; set; }
        public required string MaKho { get; set; }
        public DateTime NgayNhap { get; set; }
        public string? SoHoaDon { get; set; }
        public required string NhanVienNhap { get; set; }
        public string? GhiChu { get; set; }
        public List<ChiTietNhapDTO> ChiTiet { get; set; } = new List<ChiTietNhapDTO>();
    }

    public class ChiTietNhapDTO
    {
        public required string MaThuoc { get; set; }
        public required string SoLo { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public DateTime HanSuDung { get; set; }
    }
}