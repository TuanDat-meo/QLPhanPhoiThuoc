using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLPhanPhoiThuoc.Models.EF;
using QLPhanPhoiThuoc.Models.Entities;
using System.Security.Claims;

namespace QLPhanPhoiThuoc.Controllers
{
    [Authorize(Roles = "Admin,NhanVien,BacSi,DuocSi")]
    public class DonThuocController : Controller
    {
        private readonly BenhVienDbContext _benhVienContext;
        private readonly ILogger<DonThuocController> _logger;

        public DonThuocController(BenhVienDbContext benhVienContext, ILogger<DonThuocController> logger)
        {
            _benhVienContext = benhVienContext;
            _logger = logger;
        }

        // ==================== QUẢN LÝ ĐƠN THUỐC ====================

        // GET: /DonThuoc/QuanLyDonThuoc - Trang quản lý đơn thuốc
        [HttpGet]
        public async Task<IActionResult> QuanLyDonThuoc()
        {
            var maNhanVien = User.FindFirstValue("MaNhanVien");
            var chucVu = User.FindFirstValue("ChucVu");

            ViewBag.MaNhanVien = maNhanVien;
            ViewBag.ChucVu = chucVu;

            // Load dữ liệu ban đầu
            await LoadQuanLyDonThuocData();

            // Kiểm tra nếu là AJAX request thì trả về partial view
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView();
            }

            return View();
        }

        // GET: /DonThuoc/CapPhatThuoc - Trang cấp phát thuốc
        [HttpGet]
        [Authorize(Roles = "Admin,DuocSi")]
        public async Task<IActionResult> CapPhatThuoc()
        {
            var maNhanVien = User.FindFirstValue("MaNhanVien");
            var chucVu = User.FindFirstValue("ChucVu");

            ViewBag.MaNhanVien = maNhanVien;
            ViewBag.ChucVu = chucVu;

            await LoadCapPhatThuocData();

            // Kiểm tra nếu là AJAX request thì trả về partial view
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView();
            }

            return View();
        }

        // GET: /DonThuoc/DanhSachDonThuoc - Load danh sách đơn thuốc
        [HttpGet]
        public async Task<IActionResult> DanhSachDonThuoc(string loaiDon = "", string trangThai = "",
            DateTime? tuNgay = null, DateTime? denNgay = null, string timKiem = "")
        {
            try
            {
                var maNhanVien = User.FindFirstValue("MaNhanVien");
                var chucVu = User.FindFirstValue("ChucVu");

                var query = _benhVienContext.DonThuocs
                    .Include(d => d.BenhNhan)
                    .Include(d => d.NhanVien)
                    .Include(d => d.ChanDoan)
                    .Include(d => d.ChiTietDonThuocs)
                        .ThenInclude(ct => ct.Thuoc)
                    .AsQueryable();

                // Kiểm tra quyền truy cập
                if (chucVu != "Admin" && chucVu != "DuocSi")
                {
                    query = query.Where(d => d.MaNhanVien == maNhanVien);
                }

                // Lọc theo loại đơn
                if (!string.IsNullOrEmpty(loaiDon))
                {
                    query = query.Where(d => d.LoaiDon == loaiDon);
                }

                // Lọc theo trạng thái
                if (!string.IsNullOrEmpty(trangThai))
                {
                    query = query.Where(d => d.TrangThai == trangThai);
                }

                // Lọc theo khoảng thời gian
                if (tuNgay.HasValue)
                {
                    query = query.Where(d => d.NgayKeDon >= tuNgay.Value);
                }

                if (denNgay.HasValue)
                {
                    var denNgayEnd = denNgay.Value.Date.AddDays(1).AddSeconds(-1);
                    query = query.Where(d => d.NgayKeDon <= denNgayEnd);
                }

                // Tìm kiếm
                if (!string.IsNullOrEmpty(timKiem))
                {
                    query = query.Where(d =>
                        d.MaDonThuoc.Contains(timKiem) ||
                        d.BenhNhan.TenBenhNhan.Contains(timKiem) ||
                        d.BenhNhan.MaBenhNhan.Contains(timKiem) ||
                        d.NhanVien.TenNhanVien.Contains(timKiem)
                    );
                }

                var donThuocs = await query
                    .OrderByDescending(d => d.NgayKeDon)
                    .Select(d => new
                    {
                        d.MaDonThuoc,
                        d.NgayKeDon,
                        d.LoaiDon,
                        d.TrangThai,
                        BenhNhan = d.BenhNhan.TenBenhNhan,
                        MaBenhNhan = d.BenhNhan.MaBenhNhan,
                        BacSi = d.NhanVien.TenNhanVien,
                        MaBacSi = d.NhanVien.MaNhanVien,
                        SoLoaiThuoc = d.ChiTietDonThuocs.Count(),
                        TongSoLuong = d.ChiTietDonThuocs.Sum(ct => ct.SoLuong),
                        ChanDoan = d.ChanDoan != null ? d.ChanDoan.ChanDoanSoBo : "",
                        d.MaChanDoan
                    })
                    .ToListAsync();

                return Json(new { success = true, data = donThuocs });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading danh sách đơn thuốc");
                return Json(new { success = false, message = "Lỗi khi tải danh sách đơn thuốc: " + ex.Message });
            }
        }

        // GET: /DonThuoc/ChiTietDonThuoc - Xem chi tiết đơn thuốc
        [HttpGet]
        public async Task<IActionResult> ChiTietDonThuoc(string maDonThuoc)
        {
            try
            {
                var maNhanVien = User.FindFirstValue("MaNhanVien");
                var chucVu = User.FindFirstValue("ChucVu");

                var donThuoc = await _benhVienContext.DonThuocs
                    .Include(d => d.BenhNhan)
                        .ThenInclude(b => b.TheBHYTs)
                    .Include(d => d.NhanVien)
                    .Include(d => d.ChanDoan)
                    .Include(d => d.ChiTietDonThuocs)
                        .ThenInclude(ct => ct.Thuoc)
                    .FirstOrDefaultAsync(d => d.MaDonThuoc == maDonThuoc);

                if (donThuoc == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn thuốc" });
                }

                // Kiểm tra quyền truy cập
                if (chucVu != "Admin" && chucVu != "DuocSi" && donThuoc.MaNhanVien != maNhanVien)
                {
                    return Json(new { success = false, message = "Bạn không có quyền xem đơn thuốc này" });
                }

                // Tính tổng tiền
                decimal tongTien = 0;
                foreach (var ct in donThuoc.ChiTietDonThuocs)
                {
                    tongTien += ct.Thuoc.GiaXuat * ct.SoLuong;
                }

                // Kiểm tra BHYT - Lấy thẻ còn hạn đầu tiên
                var theBHYT = donThuoc.BenhNhan.TheBHYTs?
                    .Where(t => t.NgayHetHan >= DateTime.Now && t.TrangThai == "ConHan")
                    .OrderByDescending(t => t.NgayHetHan)
                    .FirstOrDefault();

                decimal tienBHYT = 0;
                decimal tienBNTra = tongTien;

                if (theBHYT != null)
                {
                    // Tính tiền BHYT chi trả
                    foreach (var ct in donThuoc.ChiTietDonThuocs)
                    {
                        if (ct.Thuoc.LaThuocBHYT == "Yes")
                        {
                            var giaThuoc = ct.Thuoc.GiaXuat * ct.SoLuong;
                            tienBHYT += giaThuoc * (ct.Thuoc.TyLeBHYTChiTra / 100) * (theBHYT.MucHuong / 100);
                        }
                    }
                    tienBNTra = tongTien - tienBHYT;
                }

                var result = new
                {
                    donThuoc.MaDonThuoc,
                    donThuoc.NgayKeDon,
                    donThuoc.LoaiDon,
                    donThuoc.TrangThai,
                    BenhNhan = new
                    {
                        donThuoc.BenhNhan.MaBenhNhan,
                        donThuoc.BenhNhan.TenBenhNhan,
                        donThuoc.BenhNhan.NgaySinh,
                        donThuoc.BenhNhan.GioiTinh,
                        donThuoc.BenhNhan.SoDienThoai,
                        donThuoc.BenhNhan.DiaChi,
                        CoTheBHYT = theBHYT != null,
                        SoTheBHYT = theBHYT?.SoTheBHYT,
                        MucHuong = theBHYT?.MucHuong,
                        NgayHetHanBHYT = theBHYT?.NgayHetHan
                    },
                    BacSi = new
                    {
                        donThuoc.NhanVien.MaNhanVien,
                        donThuoc.NhanVien.TenNhanVien,
                        donThuoc.NhanVien.ChucVu
                    },
                    ChanDoan = donThuoc.ChanDoan != null ? donThuoc.ChanDoan.ChanDoanSoBo : "",
                    GhiChu = donThuoc.ChanDoan != null ? donThuoc.ChanDoan.GhiChu : "",
                    ChiTiet = donThuoc.ChiTietDonThuocs.Select(ct => new
                    {
                        ct.MaDonThuoc,
                        ct.MaThuoc,
                        TenThuoc = ct.Thuoc.TenThuoc,
                        HamLuong = ct.Thuoc.HamLuong,
                        DonViTinh = ct.Thuoc.DonViTinh,
                        ct.SoLuong,
                        DonGia = ct.Thuoc.GiaXuat,
                        ThanhTien = ct.Thuoc.GiaXuat * ct.SoLuong,
                        ct.LieuDung,
                        ct.SoNgayDung,
                        ct.CachDung,
                        ct.GhiChu,
                        LaThuocBHYT = ct.Thuoc.LaThuocBHYT == "Yes",
                        TyLeBHYT = ct.Thuoc.TyLeBHYTChiTra
                    }).ToList(),
                    TongTien = tongTien,
                    TienBHYTChiTra = tienBHYT,
                    TienBenhNhanTra = tienBNTra
                };

                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading chi tiết đơn thuốc {MaDonThuoc}", maDonThuoc);
                return Json(new { success = false, message = "Lỗi khi tải chi tiết đơn thuốc: " + ex.Message });
            }
        }

        // POST: /DonThuoc/TaoDonThuoc - Tạo đơn thuốc mới
        [HttpPost]
        [Authorize(Roles = "Admin,BacSi")]
        public async Task<IActionResult> TaoDonThuoc([FromBody] TaoDonThuocModel model)
        {
            try
            {
                // Validate dữ liệu đầu vào
                if (string.IsNullOrEmpty(model.MaBenhNhan))
                {
                    return Json(new { success = false, message = "Vui lòng chọn bệnh nhân" });
                }

                if (string.IsNullOrEmpty(model.LoaiDon))
                {
                    return Json(new { success = false, message = "Vui lòng chọn loại đơn" });
                }

                if (model.ChiTiet == null || !model.ChiTiet.Any())
                {
                    return Json(new { success = false, message = "Vui lòng thêm ít nhất một loại thuốc" });
                }

                // Kiểm tra tồn kho trước khi tạo đơn
                var thuocKhongDu = new List<string>();
                foreach (var item in model.ChiTiet)
                {
                    var tonKho = await GetTonKhoThuoc(item.MaThuoc);

                    if (tonKho < item.SoLuong)
                    {
                        var thuoc = await _benhVienContext.Thuocs.FindAsync(item.MaThuoc);
                        thuocKhongDu.Add($"{thuoc?.TenThuoc} (Tồn: {tonKho}, Cần: {item.SoLuong})");
                    }
                }

                if (thuocKhongDu.Any())
                {
                    return Json(new
                    {
                        success = false,
                        message = "Một số thuốc không đủ số lượng trong kho",
                        details = thuocKhongDu
                    });
                }

                var maNhanVien = User.FindFirstValue("MaNhanVien");

                // Tạo mã đơn thuốc tự động
                var maDonThuoc = await GenerateMaDonThuoc();

                var donThuoc = new DonThuoc
                {
                    MaDonThuoc = maDonThuoc,
                    MaBenhNhan = model.MaBenhNhan,
                    MaNhanVien = maNhanVien,
                    NgayKeDon = DateTime.Now,
                    LoaiDon = model.LoaiDon,
                    MaChanDoan = model.MaChanDoan,
                    TrangThai = "ChoCapPhat",
                    NgayTao = DateTime.Now
                };

                _benhVienContext.DonThuocs.Add(donThuoc);

                // Thêm chi tiết đơn thuốc
                foreach (var item in model.ChiTiet)
                {
                    var chiTiet = new ChiTietDonThuoc
                    {
                        MaDonThuoc = maDonThuoc,
                        MaThuoc = item.MaThuoc,
                        SoLuong = item.SoLuong,
                        LieuDung = item.LieuDung,
                        SoNgayDung = item.SoNgayDung,
                        CachDung = item.CachDung,
                        GhiChu = item.GhiChu
                    };

                    _benhVienContext.ChiTietDonThuocs.Add(chiTiet);
                }

                await _benhVienContext.SaveChangesAsync();

                _logger.LogInformation("Đã tạo đơn thuốc mới: {MaDonThuoc} cho bệnh nhân {MaBenhNhan}", maDonThuoc, model.MaBenhNhan);

                return Json(new
                {
                    success = true,
                    message = "Tạo đơn thuốc thành công",
                    maDonThuoc = maDonThuoc
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating đơn thuốc");
                return Json(new { success = false, message = "Lỗi khi tạo đơn thuốc: " + ex.Message });
            }
        }

        // POST: /DonThuoc/CapNhatDonThuoc - Cập nhật đơn thuốc
        [HttpPost]
        [Authorize(Roles = "Admin,BacSi")]
        public async Task<IActionResult> CapNhatDonThuoc([FromBody] CapNhatDonThuocModel model)
        {
            try
            {
                var maNhanVien = User.FindFirstValue("MaNhanVien");
                var chucVu = User.FindFirstValue("ChucVu");

                var donThuoc = await _benhVienContext.DonThuocs
                    .Include(d => d.ChiTietDonThuocs)
                    .FirstOrDefaultAsync(d => d.MaDonThuoc == model.MaDonThuoc);

                if (donThuoc == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn thuốc" });
                }

                // Kiểm tra quyền
                if (chucVu != "Admin" && donThuoc.MaNhanVien != maNhanVien)
                {
                    return Json(new { success = false, message = "Bạn không có quyền sửa đơn thuốc này" });
                }

                // Chỉ cho phép sửa khi đơn chưa cấp phát
                if (donThuoc.TrangThai != "ChoCapPhat")
                {
                    return Json(new { success = false, message = "Chỉ có thể sửa đơn thuốc chưa cấp phát" });
                }

                // Validate
                if (model.ChiTiet == null || !model.ChiTiet.Any())
                {
                    return Json(new { success = false, message = "Vui lòng thêm ít nhất một loại thuốc" });
                }

                // Kiểm tra tồn kho
                var thuocKhongDu = new List<string>();
                foreach (var item in model.ChiTiet)
                {
                    var tonKho = await GetTonKhoThuoc(item.MaThuoc);

                    if (tonKho < item.SoLuong)
                    {
                        var thuoc = await _benhVienContext.Thuocs.FindAsync(item.MaThuoc);
                        thuocKhongDu.Add($"{thuoc?.TenThuoc} (Tồn: {tonKho}, Cần: {item.SoLuong})");
                    }
                }

                if (thuocKhongDu.Any())
                {
                    return Json(new
                    {
                        success = false,
                        message = "Một số thuốc không đủ số lượng trong kho",
                        details = thuocKhongDu
                    });
                }

                // Cập nhật thông tin đơn
                donThuoc.LoaiDon = model.LoaiDon;
                donThuoc.MaChanDoan = model.MaChanDoan;

                // Xóa chi tiết cũ
                _benhVienContext.ChiTietDonThuocs.RemoveRange(donThuoc.ChiTietDonThuocs);

                // Thêm chi tiết mới
                foreach (var item in model.ChiTiet)
                {
                    var chiTiet = new ChiTietDonThuoc
                    {
                        MaDonThuoc = model.MaDonThuoc,
                        MaThuoc = item.MaThuoc,
                        SoLuong = item.SoLuong,
                        LieuDung = item.LieuDung,
                        SoNgayDung = item.SoNgayDung,
                        CachDung = item.CachDung,
                        GhiChu = item.GhiChu
                    };

                    _benhVienContext.ChiTietDonThuocs.Add(chiTiet);
                }

                await _benhVienContext.SaveChangesAsync();

                _logger.LogInformation("Đã cập nhật đơn thuốc: {MaDonThuoc}", model.MaDonThuoc);

                return Json(new { success = true, message = "Cập nhật đơn thuốc thành công" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating đơn thuốc {MaDonThuoc}", model.MaDonThuoc);
                return Json(new { success = false, message = "Lỗi khi cập nhật đơn thuốc: " + ex.Message });
            }
        }

        // POST: /DonThuoc/HuyDonThuoc - Hủy đơn thuốc
        [HttpPost]
        [Authorize(Roles = "Admin,BacSi")]
        public async Task<IActionResult> HuyDonThuoc(string maDonThuoc, string lyDoHuy = "")
        {
            try
            {
                var maNhanVien = User.FindFirstValue("MaNhanVien");
                var chucVu = User.FindFirstValue("ChucVu");

                var donThuoc = await _benhVienContext.DonThuocs
                    .FirstOrDefaultAsync(d => d.MaDonThuoc == maDonThuoc);

                if (donThuoc == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn thuốc" });
                }

                // Kiểm tra quyền
                if (chucVu != "Admin" && donThuoc.MaNhanVien != maNhanVien)
                {
                    return Json(new { success = false, message = "Bạn không có quyền hủy đơn thuốc này" });
                }

                // Chỉ cho phép hủy khi đơn chưa cấp phát
                if (donThuoc.TrangThai == "DaCapPhat")
                {
                    return Json(new { success = false, message = "Không thể hủy đơn thuốc đã cấp phát" });
                }

                donThuoc.TrangThai = "DaHuy";

                await _benhVienContext.SaveChangesAsync();

                _logger.LogInformation("Đã hủy đơn thuốc: {MaDonThuoc}, Lý do: {LyDo}", maDonThuoc, lyDoHuy);

                return Json(new { success = true, message = "Hủy đơn thuốc thành công" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error canceling đơn thuốc {MaDonThuoc}", maDonThuoc);
                return Json(new { success = false, message = "Lỗi khi hủy đơn thuốc: " + ex.Message });
            }
        }

        // GET: /DonThuoc/GetDonThuocToEdit - Lấy thông tin đơn để sửa
        [HttpGet]
        [Authorize(Roles = "Admin,BacSi")]
        public async Task<IActionResult> GetDonThuocToEdit(string maDonThuoc)
        {
            try
            {
                var maNhanVien = User.FindFirstValue("MaNhanVien");
                var chucVu = User.FindFirstValue("ChucVu");

                var donThuoc = await _benhVienContext.DonThuocs
                    .Include(d => d.BenhNhan)
                    .Include(d => d.ChiTietDonThuocs)
                        .ThenInclude(ct => ct.Thuoc)
                    .FirstOrDefaultAsync(d => d.MaDonThuoc == maDonThuoc);

                if (donThuoc == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn thuốc" });
                }

                // Kiểm tra quyền
                if (chucVu != "Admin" && donThuoc.MaNhanVien != maNhanVien)
                {
                    return Json(new { success = false, message = "Bạn không có quyền sửa đơn thuốc này" });
                }

                if (donThuoc.TrangThai != "ChoCapPhat")
                {
                    return Json(new { success = false, message = "Chỉ có thể sửa đơn thuốc chưa cấp phát" });
                }

                var result = new
                {
                    donThuoc.MaDonThuoc,
                    donThuoc.MaBenhNhan,
                    TenBenhNhan = donThuoc.BenhNhan.TenBenhNhan,
                    donThuoc.LoaiDon,
                    donThuoc.MaChanDoan,
                    ChiTiet = donThuoc.ChiTietDonThuocs.Select(ct => new
                    {
                        ct.MaThuoc,
                        TenThuoc = ct.Thuoc.TenThuoc,
                        ct.SoLuong,
                        ct.LieuDung,
                        ct.SoNgayDung,
                        ct.CachDung,
                        ct.GhiChu
                    }).ToList()
                };

                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting đơn thuốc to edit {MaDonThuoc}", maDonThuoc);
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // ==================== CẤP PHÁT THUỐC ====================

        // POST: /DonThuoc/CapPhatThuoc - Cấp phát thuốc
        [HttpPost]
        [Authorize(Roles = "Admin,DuocSi")]
        public async Task<IActionResult> CapPhatThuoc([FromBody] CapPhatThuocModel model)
        {
            using var transaction = await _benhVienContext.Database.BeginTransactionAsync();
            try
            {
                var donThuoc = await _benhVienContext.DonThuocs
                    .Include(d => d.BenhNhan)
                        .ThenInclude(b => b.TheBHYTs)
                    .Include(d => d.ChiTietDonThuocs)
                        .ThenInclude(ct => ct.Thuoc)
                    .FirstOrDefaultAsync(d => d.MaDonThuoc == model.MaDonThuoc);

                if (donThuoc == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn thuốc" });
                }

                if (donThuoc.TrangThai != "ChoCapPhat")
                {
                    return Json(new { success = false, message = "Đơn thuốc này đã được xử lý" });
                }

                // Kiểm tra tồn kho và lấy lô thuốc
                var thuocKhongDu = new List<string>();
                var danhSachLo = new Dictionary<string, List<LoThuoc>>();

                foreach (var chiTiet in donThuoc.ChiTietDonThuocs)
                {
                    var loThuocs = await _benhVienContext.LoThuocs
                        .Where(l => l.MaThuoc == chiTiet.MaThuoc &&
                                    l.HanSuDung > DateTime.Now &&
                                    l.TrangThai == "ConHang")
                        .OrderBy(l => l.HanSuDung)
                        .ToListAsync();

                    // Tính tồn kho từng lô
                    var loConTon = new List<LoThuoc>();
                    foreach (var lo in loThuocs)
                    {
                        var daNhap = await _benhVienContext.ChiTietPhieuNhaps
                            .Where(ct => ct.MaLo == lo.MaLo)
                            .SumAsync(ct => ct.SoLuongNhap);

                        var daCap = await _benhVienContext.ChiTietPhieuCaps
                            .Where(ct => ct.MaLo == lo.MaLo)
                            .SumAsync(ct => ct.SoLuongCap);

                        var tonLo = daNhap - daCap;
                        if (tonLo > 0)
                        {
                            // Sử dụng property tạm để lưu số lượng tồn
                            lo.SoLuongNhap = tonLo;
                            loConTon.Add(lo);
                        }
                    }

                    var tongTon = loConTon.Sum(l => l.SoLuongNhap);
                    if (tongTon < chiTiet.SoLuong)
                    {
                        thuocKhongDu.Add($"{chiTiet.Thuoc.TenThuoc} (Tồn: {tongTon}, Cần: {chiTiet.SoLuong})");
                    }
                    else
                    {
                        danhSachLo[chiTiet.MaThuoc] = loConTon;
                    }
                }

                if (thuocKhongDu.Any())
                {
                    return Json(new
                    {
                        success = false,
                        message = "Không đủ thuốc để cấp phát",
                        details = thuocKhongDu
                    });
                }

                // Lấy kho mặc định (có thể cấu hình)
                var khoMacDinh = await _benhVienContext.Khos
                    .Where(k => k.TrangThai == "HoatDong")
                    .FirstOrDefaultAsync();

                if (khoMacDinh == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy kho hoạt động" });
                }

                // Tạo phiếu cấp phát
                var maPhieuCap = await GenerateMaPhieuCapPhat();
                var phieuCap = new PhieuCapPhat
                {
                    MaPhieuCap = maPhieuCap,
                    MaDonThuoc = model.MaDonThuoc,
                    MaBenhNhan = donThuoc.MaBenhNhan,
                    MaKho = khoMacDinh.MaKho,
                    NhanVienCap = User.FindFirstValue("MaNhanVien"),
                    NgayCap = DateTime.Now,
                    TongTien = 0, // Sẽ tính sau
                    TrangThai = "DaCapPhat",
                    GhiChu = model.GhiChu,
                    NgayTao = DateTime.Now
                };

                _benhVienContext.PhieuCapPhats.Add(phieuCap);

                decimal tongTienPhieu = 0;

                // Cấp phát từng loại thuốc
                foreach (var chiTiet in donThuoc.ChiTietDonThuocs)
                {
                    var soLuongCanCap = chiTiet.SoLuong;
                    var loThuocs = danhSachLo[chiTiet.MaThuoc];

                    foreach (var lo in loThuocs)
                    {
                        if (soLuongCanCap <= 0) break;

                        var soLuongCap = Math.Min(soLuongCanCap, lo.SoLuongNhap);

                        var chiTietCap = new ChiTietPhieuCap
                        {
                            MaPhieuCap = maPhieuCap,
                            MaLo = lo.MaLo,
                            SoLuongCap = soLuongCap,
                            DonGiaCap = chiTiet.Thuoc.GiaXuat
                        };

                        _benhVienContext.ChiTietPhieuCaps.Add(chiTietCap);
                        tongTienPhieu += soLuongCap * chiTiet.Thuoc.GiaXuat;
                        soLuongCanCap -= soLuongCap;
                    }
                }

                phieuCap.TongTien = tongTienPhieu;

                // Cập nhật trạng thái đơn thuốc
                donThuoc.TrangThai = "DaCapPhat";

                // Tạo hóa đơn
                var maHoaDon = await GenerateMaHoaDon();
                decimal tongTien = tongTienPhieu;
                decimal tienBHYT = 0;

                // Lấy thẻ BHYT còn hạn
                var theBHYT = donThuoc.BenhNhan.TheBHYTs?
                    .Where(t => t.NgayHetHan >= DateTime.Now && t.TrangThai == "ConHan")
                    .OrderByDescending(t => t.NgayHetHan)
                    .FirstOrDefault();

                if (theBHYT != null)
                {
                    foreach (var chiTiet in donThuoc.ChiTietDonThuocs)
                    {
                        if (chiTiet.Thuoc.LaThuocBHYT == "Yes")
                        {
                            var thanhTien = chiTiet.Thuoc.GiaXuat * chiTiet.SoLuong;
                            tienBHYT += thanhTien * (chiTiet.Thuoc.TyLeBHYTChiTra / 100) * (theBHYT.MucHuong / 100);
                        }
                    }
                }

                var hoaDon = new HoaDon
                {
                    MaHoaDon = maHoaDon,
                    MaBenhNhan = donThuoc.MaBenhNhan,
                    MaDonThuoc = donThuoc.MaDonThuoc,
                    NgayTaoHoaDon = DateTime.Now,
                    TongTien = tongTien,
                    TienBHYTChiTra = tienBHYT,
                    TienBenhNhanCanTra = tongTien - tienBHYT,
                    TienDaTra = 0,
                    HinhThucThanhToan = "",
                    TrangThaiThanhToan = "ChuaTra",
                    NgayTao = DateTime.Now
                };

                _benhVienContext.HoaDons.Add(hoaDon);

                await _benhVienContext.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Đã cấp phát thuốc cho đơn: {MaDonThuoc}, Phiếu cấp: {MaPhieuCap}",
                    model.MaDonThuoc, maPhieuCap);

                return Json(new
                {
                    success = true,
                    message = "Cấp phát thuốc thành công",
                    maPhieuCap = maPhieuCap,
                    maHoaDon = maHoaDon,
                    tongTien = tongTien,
                    tienBHYT = tienBHYT,
                    tienBNTra = tongTien - tienBHYT
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error cấp phát thuốc cho đơn {MaDonThuoc}", model.MaDonThuoc);
                return Json(new { success = false, message = "Lỗi khi cấp phát thuốc: " + ex.Message });
            }
        }

        // GET: /DonThuoc/KiemTraTonKho - Kiểm tra tồn kho
        [HttpPost]
        public async Task<IActionResult> KiemTraTonKho([FromBody] KiemTraTonKhoModel model)
        {
            try
            {
                var result = new List<object>();

                foreach (var item in model.DanhSach)
                {
                    var thuoc = await _benhVienContext.Thuocs.FindAsync(item.MaThuoc);
                    if (thuoc == null) continue;

                    var tonKho = await GetTonKhoThuoc(item.MaThuoc);
                    var conLai = tonKho - item.SoLuong;

                    result.Add(new
                    {
                        item.MaThuoc,
                        TenThuoc = thuoc.TenThuoc,
                        TonKho = tonKho,
                        SoLuongYeuCau = item.SoLuong,
                        ConLai = conLai,
                        DuThuoc = conLai >= 0,
                        CanCanhBao = conLai < thuoc.TonKhoToiThieu
                    });
                }

                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error kiểm tra tồn kho");
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // GET: /DonThuoc/ThongKeDonThuoc - Thống kê đơn thuốc
        [HttpGet]
        public async Task<IActionResult> ThongKeDonThuoc(DateTime? tuNgay, DateTime? denNgay)
        {
            try
            {
                var query = _benhVienContext.DonThuocs.AsQueryable();

                if (tuNgay.HasValue)
                    query = query.Where(d => d.NgayKeDon >= tuNgay.Value);

                if (denNgay.HasValue)
                {
                    var denNgayEnd = denNgay.Value.Date.AddDays(1).AddSeconds(-1);
                    query = query.Where(d => d.NgayKeDon <= denNgayEnd);
                }

                var tongDon = await query.CountAsync();
                var donChoCapPhat = await query.CountAsync(d => d.TrangThai == "ChoCapPhat");
                var donDaCapPhat = await query.CountAsync(d => d.TrangThai == "DaCapPhat");
                var donDaHuy = await query.CountAsync(d => d.TrangThai == "DaHuy");

                var donNoiTru = await query.CountAsync(d => d.LoaiDon == "NoiTru");
                var donNgoaiTru = await query.CountAsync(d => d.LoaiDon == "NgoaiTru");

                // Thống kê theo thời gian
                var theoNgay = await query
                    .GroupBy(d => d.NgayKeDon.Date)
                    .Select(g => new
                    {
                        Ngay = g.Key,
                        SoDon = g.Count()
                    })
                    .OrderBy(x => x.Ngay)
                    .ToListAsync();

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        TongDon = tongDon,
                        DonChoCapPhat = donChoCapPhat,
                        DonDaCapPhat = donDaCapPhat,
                        DonDaHuy = donDaHuy,
                        DonNoiTru = donNoiTru,
                        DonNgoaiTru = donNgoaiTru,
                        TheoNgay = theoNgay
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error thống kê đơn thuốc");
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // GET: /DonThuoc/InDonThuoc - In đơn thuốc
        [HttpGet]
        public async Task<IActionResult> InDonThuoc(string maDonThuoc)
        {
            try
            {
                var donThuoc = await _benhVienContext.DonThuocs
                    .Include(d => d.BenhNhan)
                        .ThenInclude(b => b.TheBHYTs)
                    .Include(d => d.NhanVien)
                    .Include(d => d.ChanDoan)
                    .Include(d => d.ChiTietDonThuocs)
                        .ThenInclude(ct => ct.Thuoc)
                    .FirstOrDefaultAsync(d => d.MaDonThuoc == maDonThuoc);

                if (donThuoc == null)
                {
                    return NotFound("Không tìm thấy đơn thuốc");
                }

                ViewBag.DonThuoc = donThuoc;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in đơn thuốc {MaDonThuoc}", maDonThuoc);
                return BadRequest("Lỗi: " + ex.Message);
            }
        }

        // ==================== HELPER METHODS ====================

        private async Task LoadQuanLyDonThuocData()
        {
            // Load thống kê
            var tongDon = await _benhVienContext.DonThuocs.CountAsync();
            var donChoCapPhat = await _benhVienContext.DonThuocs
                .CountAsync(d => d.TrangThai == "ChoCapPhat");
            var donDaCapPhat = await _benhVienContext.DonThuocs
                .CountAsync(d => d.TrangThai == "DaCapPhat");

            ViewBag.TongDon = tongDon;
            ViewBag.DonChoCapPhat = donChoCapPhat;
            ViewBag.DonDaCapPhat = donDaCapPhat;

            // Load danh sách bệnh nhân
            var benhNhans = await _benhVienContext.BenhNhans
                .Where(b => b.TrangThai == "HoatDong")
                .Select(b => new { b.MaBenhNhan, b.TenBenhNhan, b.NgaySinh, b.GioiTinh })
                .OrderBy(b => b.TenBenhNhan)
                .ToListAsync();
            ViewBag.BenhNhans = benhNhans;

            // Load danh sách chẩn đoán
            var chanDoans = await _benhVienContext.ChanDoans
                .Select(c => new { c.MaChanDoan, c.ChanDoanSoBo, c.MaBenhNhan })
                .OrderByDescending(c => c.MaChanDoan)
                .ToListAsync();
            ViewBag.ChanDoans = chanDoans;

            // Load danh sách thuốc
            var thuocs = await _benhVienContext.Thuocs
                .Where(t => t.TrangThai == "KichHoat")
                .Select(t => new
                {
                    t.MaThuoc,
                    t.TenThuoc,
                    t.HamLuong,
                    t.DonViTinh,
                    DonGia = t.GiaXuat,
                    t.LaThuocBHYT,
                    t.TyLeBHYTChiTra,
                    t.TonKhoToiThieu
                })
                .OrderBy(t => t.TenThuoc)
                .ToListAsync();
            ViewBag.Thuocs = thuocs;
        }

        private async Task LoadCapPhatThuocData()
        {
            var donChoCapPhat = await _benhVienContext.DonThuocs
                .CountAsync(d => d.TrangThai == "ChoCapPhat");
            var donDaCapPhat = await _benhVienContext.DonThuocs
                .CountAsync(d => d.TrangThai == "DaCapPhat" &&
                                  d.NgayKeDon.Date == DateTime.Today);

            ViewBag.DonChoCapPhat = donChoCapPhat;
            ViewBag.DonDaCapPhat = donDaCapPhat;
        }

        private async Task<int> GetTonKhoThuoc(string maThuoc)
        {
            // Tổng nhập
            var tongNhap = await _benhVienContext.ChiTietPhieuNhaps
                .Where(ct => ct.LoThuoc.MaThuoc == maThuoc)
                .SumAsync(ct => ct.SoLuongNhap);

            // Tổng cấp
            var tongCap = await _benhVienContext.ChiTietPhieuCaps
                .Where(ct => ct.LoThuoc.MaThuoc == maThuoc)
                .SumAsync(ct => ct.SoLuongCap);

            return tongNhap - tongCap;
        }

        private async Task<string> GenerateMaDonThuoc()
        {
            var today = DateTime.Now.ToString("yyyyMMdd");
            var count = await _benhVienContext.DonThuocs
                .Where(d => d.MaDonThuoc.StartsWith($"DT{today}"))
                .CountAsync();
            return $"DT{today}{(count + 1):D4}";
        }

        private async Task<string> GenerateMaPhieuCapPhat()
        {
            var today = DateTime.Now.ToString("yyyyMMdd");
            var count = await _benhVienContext.PhieuCapPhats
                .Where(p => p.MaPhieuCap.StartsWith($"PC{today}"))
                .CountAsync();
            return $"PC{today}{(count + 1):D4}";
        }

        private async Task<string> GenerateMaHoaDon()
        {
            var today = DateTime.Now.ToString("yyyyMMdd");
            var count = await _benhVienContext.HoaDons
                .Where(h => h.MaHoaDon.StartsWith($"HD{today}"))
                .CountAsync();
            return $"HD{today}{(count + 1):D4}";
        }
    }

    // ==================== VIEW MODELS ====================

    public class TaoDonThuocModel
    {
        public string MaBenhNhan { get; set; }
        public string LoaiDon { get; set; }
        public string MaChanDoan { get; set; }
        public List<ChiTietDonThuocModel> ChiTiet { get; set; }
    }

    public class CapNhatDonThuocModel
    {
        public string MaDonThuoc { get; set; }
        public string LoaiDon { get; set; }
        public string MaChanDoan { get; set; }
        public List<ChiTietDonThuocModel> ChiTiet { get; set; }
    }

    public class ChiTietDonThuocModel
    {
        public string MaThuoc { get; set; }
        public int SoLuong { get; set; }
        public string LieuDung { get; set; }
        public int SoNgayDung { get; set; }
        public string CachDung { get; set; }
        public string GhiChu { get; set; }
    }

    public class KiemTraTonKhoModel
    {
        public List<TonKhoItem> DanhSach { get; set; }
    }

    public class TonKhoItem
    {
        public string MaThuoc { get; set; }
        public int SoLuong { get; set; }
    }

    public class CapPhatThuocModel
    {
        public string MaDonThuoc { get; set; }
        public string GhiChu { get; set; }
    }
}