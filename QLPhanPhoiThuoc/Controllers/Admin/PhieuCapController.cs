using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLPhanPhoiThuoc.Models.EF;
using QLPhanPhoiThuoc.Models;
using QLPhanPhoiThuoc.Models.Entities;

namespace QLPhanPhoiThuoc.Controllers.Admin
{
    [Route("Admin/[controller]")]
    public class PhieuCapController : Controller
    {
        private readonly BenhVienDbContext _context;

        public PhieuCapController(BenhVienDbContext context)
        {
            _context = context;
        }

        // GET: Admin/PhieuCap/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            // Lấy mã nhân viên từ session/claims - Thay đổi logic này theo hệ thống của bạn
            ViewBag.MaNhanVien = User.Identity?.Name ?? "NV001";
            // Hoặc nếu bạn lưu trong Session:
            // ViewBag.MaNhanVien = HttpContext.Session.GetString("MaNhanVien") ?? "NV001";

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("~/Views/Kho/PhieuCap_Create.cshtml");
            }
            return View("~/Views/Kho/PhieuCap_Create.cshtml");
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] PhieuCapDTO phieuCapDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (!ModelState.IsValid) return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });

                // 1. LẤY MÃ NHÂN VIÊN THỰC TẾ TỪ USERNAME ĐANG ĐĂNG NHẬP
                var currentUsername = User.Identity.Name;
                var account = await _context.TaiKhoans
                    .FirstOrDefaultAsync(t => t.Username == currentUsername);

                if (account == null || string.IsNullOrEmpty(account.MaNhanVien))
                {
                    return Json(new { success = false, message = "Lỗi: Tài khoản chưa liên kết với Mã nhân viên!" });
                }

                // 2. KHỞI TẠO PHIẾU CẤP PHÁT
                // Trong hàm [HttpPost("Create")] của PhieuCapController.cs

                var phieuCap = new PhieuCapPhat
                {
                    MaPhieuCap = phieuCapDto.MaPhieuCap,
                    MaBenhNhan = phieuCapDto.MaBenhNhan,
                    MaKho = phieuCapDto.MaKho,
                    NgayCap = phieuCapDto.NgayCap,
                    NhanVienCap = account.MaNhanVien,
                    // ✅ SỬA TẠI ĐÂY: Nếu chuỗi rỗng thì gán NULL
                    MaDonThuoc = string.IsNullOrWhiteSpace(phieuCapDto.MaDonThuoc) ? null : phieuCapDto.MaDonThuoc,
                    GhiChu = phieuCapDto.GhiChu ?? "",
                    TrangThai = "DangXuLy",
                    NgayTao = DateTime.Now,
                    ChiTietPhieuCaps = new List<ChiTietPhieuCap>()
                };

                decimal tongTien = 0;

                // 3. XỬ LÝ CHI TIẾT VÀ TRỪ TỒN KHO
                foreach (var item in phieuCapDto.ChiTiet)
                {
                    var loThuoc = await _context.LoThuoc.FirstOrDefaultAsync(l => l.MaLo == item.MaLo);

                    if (loThuoc == null)
                        return Json(new { success = false, message = $"Không tìm thấy lô thuốc {item.MaLo}!" });

                    if (loThuoc.SoLuongCon < item.SoLuong)
                        return Json(new { success = false, message = $"Số lượng tồn kho không đủ (Lô: {item.MaLo})!" });

                    // Trừ tồn kho
                    loThuoc.SoLuongCon -= item.SoLuong;
                    if (loThuoc.SoLuongCon == 0) loThuoc.TrangThai = "HetHang";

                    // Thêm chi tiết
                    phieuCap.ChiTietPhieuCaps.Add(new ChiTietPhieuCap
                    {
                        MaPhieuCap = phieuCap.MaPhieuCap,
                        MaLo = item.MaLo,
                        SoLuongCap = item.SoLuong,
                        DonGiaCap = item.DonGia
                    });

                    tongTien += item.SoLuong * item.DonGia;
                }

                phieuCap.TongTien = tongTien;

                // 4. LƯU VÀO DATABASE
                _context.PhieuCapPhat.Add(phieuCap);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Json(new { success = true, message = "Tạo phiếu xuất thành công!" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // Trả về lỗi chi tiết nhất để debug nếu vẫn lỗi
                var innerMsg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return Json(new { success = false, message = "Lỗi CSDL: " + innerMsg });
            }
        }

        [HttpGet("GetKho")]
        public async Task<IActionResult> GetKho()
        {
            var data = await _context.Kho.Where(k => k.TrangThai == "HoatDong").Select(k => new { maKho = k.MaKho, tenKho = k.TenKho }).ToListAsync();
            return Json(data);
        }

        [HttpGet("GetLoThuocByKho/{maKho}")]
        public async Task<IActionResult> GetLoThuocByKho(string maKho)
        {
            var data = await _context.LoThuoc.Where(l => l.MaKho == maKho && l.SoLuongCon > 0 && l.HanSuDung > DateTime.Now)
                .Include(l => l.Thuoc).Select(l => new { maLo = l.MaLo, maThuoc = l.MaThuoc, tenThuoc = l.Thuoc != null ? l.Thuoc.TenThuoc : "", soLo = l.SoLo, soLuongCon = l.SoLuongCon, hanSuDung = l.HanSuDung, giaXuat = l.Thuoc != null ? l.Thuoc.GiaXuat : 0 }).ToListAsync();
            return Json(data);
        }

        [HttpGet("SearchPatient")]
        public async Task<IActionResult> SearchPatient(string keyword)
        {
            try
            {
                if (string.IsNullOrEmpty(keyword)) return Json(null);

                // Tìm chính xác theo CCCD hoặc Mã bệnh nhân
                var patient = await _context.BenhNhan
                    .Where(b => b.CCCD == keyword || b.MaBenhNhan == keyword)
                    .Select(b => new {
                        maBenhNhan = b.MaBenhNhan,
                        tenBenhNhan = b.TenBenhNhan,
                        cccd = b.CCCD,
                        ngaySinh = b.NgaySinh
                    })
                    .FirstOrDefaultAsync();

                return Json(patient);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var data = await _context.PhieuCapPhat.Include(p => p.BenhNhan).Include(p => p.Kho).Include(p => p.NhanVien).OrderByDescending(p => p.NgayCap).ToListAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest") return PartialView(data);
            return View(data);
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            var data = await _context.PhieuCapPhat.Include(p => p.BenhNhan).Include(p => p.Kho).Include(p => p.NhanVien).Include(p => p.ChiTietPhieuCaps).ThenInclude(c => c.LoThuoc).ThenInclude(l => l!.Thuoc).FirstOrDefaultAsync(p => p.MaPhieuCap == id);
            if (data == null) return NotFound();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest") return PartialView(data);
            return View(data);
        }

        [HttpPost("UpdateStatus/{id}")]
        public async Task<IActionResult> UpdateStatus(string id, [FromBody] StatusUpdateDTO dto)
        {
            try
            {
                var p = await _context.PhieuCapPhat.FindAsync(id);
                if (p == null) return Json(new { success = false });
                p.TrangThai = dto.TrangThai;
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex) { return Json(new { success = false, message = ex.Message }); }
        }
    }

    public class PhieuCapDTO
    {
        public required string MaPhieuCap { get; set; }
        public required string MaBenhNhan { get; set; }
        public required string MaKho { get; set; }
        public DateTime NgayCap { get; set; }
        public required string NhanVienCap { get; set; }
        public string? MaDonThuoc { get; set; }
        public string? GhiChu { get; set; }
        public List<ChiTietCapDTO> ChiTiet { get; set; } = new List<ChiTietCapDTO>();
    }

    public class ChiTietCapDTO
    {
        public required string MaLo { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
    }

    public class StatusUpdateDTO { public required string TrangThai { get; set; } }
}