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

        public DonThuocController(BenhVienDbContext benhVienContext)
        {
            _benhVienContext = benhVienContext;
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
            // Nếu không phải Admin hoặc DuocSi, chỉ xem đơn do mình kê
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
                query = query.Where(d => d.NgayKeDon <= denNgay.Value);
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
                    SoLoaiThuoc = d.ChiTietDonThuocs.Count(),
                    TongSoLuong = d.ChiTietDonThuocs.Sum(ct => ct.SoLuong),
                    ChanDoan = d.ChanDoan != null ? d.ChanDoan.ChanDoanSoBo : ""
                })
                .ToListAsync();

            return Json(new { success = true, data = donThuocs });
        }

        // GET: /DonThuoc/ChiTietDonThuoc - Xem chi tiết đơn thuốc
        [HttpGet]
        public async Task<IActionResult> ChiTietDonThuoc(string maDonThuoc)
        {
            var maNhanVien = User.FindFirstValue("MaNhanVien");
            var chucVu = User.FindFirstValue("ChucVu");

            var donThuoc = await _benhVienContext.DonThuocs
                .Include(d => d.BenhNhan)
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
                    donThuoc.BenhNhan.DiaChi
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
                    ct.LieuDung,
                    ct.SoNgayDung,
                    ct.CachDung,
                    ct.GhiChu
                }).ToList()
            };

            return Json(new { success = true, data = result });
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
                    // Tính tồn kho = Tổng nhập - Tổng cấp
                    var tongNhap = await _benhVienContext.ChiTietPhieuNhaps
                        .Where(ct => ct.LoThuoc.MaThuoc == item.MaThuoc)
                        .SumAsync(ct => ct.SoLuongNhap);

                    var tongCap = await _benhVienContext.ChiTietPhieuCaps
                        .Where(ct => ct.LoThuoc.MaThuoc == item.MaThuoc)
                        .SumAsync(ct => ct.SoLuongCap);

                    var tonKho = tongNhap - tongCap;

                    if (tonKho < item.SoLuong)
                    {
                        var thuoc = await _benhVienContext.Thuocs.FindAsync(item.MaThuoc);
                        thuocKhongDu.Add($"{thuoc.TenThuoc} (Tồn: {tonKho}, Cần: {item.SoLuong})");
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

                return Json(new
                {
                    success = true,
                    message = "Tạo đơn thuốc thành công",
                    maDonThuoc
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
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

                // Kiểm tra quyền cập nhật
                if (chucVu != "Admin" && donThuoc.MaNhanVien != maNhanVien)
                {
                    return Json(new { success = false, message = "Bạn không có quyền cập nhật đơn thuốc này" });
                }

                // Chỉ cho phép cập nhật nếu đơn chưa cấp phát
                if (donThuoc.TrangThai != "ChoCapPhat")
                {
                    return Json(new { success = false, message = "Chỉ có thể cập nhật đơn thuốc đang chờ cấp phát" });
                }

                // Validate dữ liệu
                if (model.ChiTiet == null || !model.ChiTiet.Any())
                {
                    return Json(new { success = false, message = "Vui lòng thêm ít nhất một loại thuốc" });
                }

                // Kiểm tra tồn kho
                var thuocKhongDu = new List<string>();
                foreach (var item in model.ChiTiet)
                {
                    // Tính tồn kho = Tổng nhập - Tổng cấp
                    var tongNhap = await _benhVienContext.ChiTietPhieuNhaps
                        .Where(ct => ct.LoThuoc.MaThuoc == item.MaThuoc)
                        .SumAsync(ct => ct.SoLuongNhap);

                    var tongCap = await _benhVienContext.ChiTietPhieuCaps
                        .Where(ct => ct.LoThuoc.MaThuoc == item.MaThuoc)
                        .SumAsync(ct => ct.SoLuongCap);

                    var tonKho = tongNhap - tongCap;

                    if (tonKho < item.SoLuong)
                    {
                        var thuoc = await _benhVienContext.Thuocs.FindAsync(item.MaThuoc);
                        thuocKhongDu.Add($"{thuoc.TenThuoc} (Tồn: {tonKho}, Cần: {item.SoLuong})");
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

                // Cập nhật thông tin đơn thuốc
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

                return Json(new
                {
                    success = true,
                    message = "Cập nhật đơn thuốc thành công"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        // POST: /DonThuoc/XoaDonThuoc - Xóa đơn thuốc
        [HttpPost]
        [Authorize(Roles = "Admin,BacSi")]
        public async Task<IActionResult> XoaDonThuoc(string maDonThuoc)
        {
            try
            {
                var maNhanVien = User.FindFirstValue("MaNhanVien");
                var chucVu = User.FindFirstValue("ChucVu");

                var donThuoc = await _benhVienContext.DonThuocs
                    .Include(d => d.ChiTietDonThuocs)
                    .FirstOrDefaultAsync(d => d.MaDonThuoc == maDonThuoc);

                if (donThuoc == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn thuốc" });
                }

                // Kiểm tra quyền xóa
                if (chucVu != "Admin" && donThuoc.MaNhanVien != maNhanVien)
                {
                    return Json(new { success = false, message = "Bạn không có quyền xóa đơn thuốc này" });
                }

                // Chỉ cho phép xóa nếu đơn chưa cấp phát
                if (donThuoc.TrangThai != "ChoCapPhat")
                {
                    return Json(new { success = false, message = "Chỉ có thể xóa đơn thuốc đang chờ cấp phát" });
                }

                // Xóa chi tiết đơn thuốc
                _benhVienContext.ChiTietDonThuocs.RemoveRange(donThuoc.ChiTietDonThuocs);

                // Xóa đơn thuốc
                _benhVienContext.DonThuocs.Remove(donThuoc);

                await _benhVienContext.SaveChangesAsync();

                return Json(new { success = true, message = "Xóa đơn thuốc thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        // ==================== CẤP PHÁT THUỐC ====================

        // POST: /DonThuoc/KiemTraTonKho - Kiểm tra tồn kho trước khi cấp phát
        [HttpPost]
        public async Task<IActionResult> KiemTraTonKho([FromBody] KiemTraTonKhoModel model)
        {
            try
            {
                var ketQua = new List<object>();

                foreach (var item in model.DanhSach)
                {
                    var thuoc = await _benhVienContext.Thuocs.FindAsync(item.MaThuoc);

                    // Tính tồn kho = Tổng nhập - Tổng cấp
                    var tongNhap = await _benhVienContext.ChiTietPhieuNhaps
                        .Where(ct => ct.LoThuoc.MaThuoc == item.MaThuoc)
                        .SumAsync(ct => ct.SoLuongNhap);

                    var tongCap = await _benhVienContext.ChiTietPhieuCaps
                        .Where(ct => ct.LoThuoc.MaThuoc == item.MaThuoc)
                        .SumAsync(ct => ct.SoLuongCap);

                    var tonKho = tongNhap - tongCap;

                    ketQua.Add(new
                    {
                        maThuoc = item.MaThuoc,
                        tenThuoc = thuoc.TenThuoc,
                        soLuongYeuCau = item.SoLuong,
                        tonKho = tonKho,
                        duKho = tonKho >= item.SoLuong
                    });
                }

                var tatCaDuKho = ketQua.All(k => (bool)((dynamic)k).duKho);

                return Json(new
                {
                    success = true,
                    tatCaDuKho,
                    chiTiet = ketQua
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        // POST: /DonThuoc/CapPhat - Cấp phát thuốc
        [HttpPost]
        [Authorize(Roles = "Admin,DuocSi")]
        public async Task<IActionResult> CapPhat([FromBody] CapPhatThuocModel model)
        {
            try
            {
                var maNhanVien = User.FindFirstValue("MaNhanVien");

                var donThuoc = await _benhVienContext.DonThuocs
                    .Include(d => d.ChiTietDonThuocs)
                        .ThenInclude(ct => ct.Thuoc)
                    .Include(d => d.BenhNhan)
                        .ThenInclude(b => b.TheBHYTs)
                    .FirstOrDefaultAsync(d => d.MaDonThuoc == model.MaDonThuoc);

                if (donThuoc == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn thuốc" });
                }

                if (donThuoc.TrangThai != "ChoCapPhat")
                {
                    return Json(new { success = false, message = "Đơn thuốc này đã được cấp phát hoặc đã hủy" });
                }

                // Kiểm tra tồn kho cho từng loại thuốc
                var thuocKhongDu = new List<string>();
                foreach (var chiTiet in donThuoc.ChiTietDonThuocs)
                {
                    // Tính tồn kho = Tổng nhập - Tổng cấp
                    var tongNhap = await _benhVienContext.ChiTietPhieuNhaps
                        .Where(ct => ct.LoThuoc.MaThuoc == chiTiet.MaThuoc)
                        .SumAsync(ct => ct.SoLuongNhap);

                    var tongCap = await _benhVienContext.ChiTietPhieuCaps
                        .Where(ct => ct.LoThuoc.MaThuoc == chiTiet.MaThuoc)
                        .SumAsync(ct => ct.SoLuongCap);

                    var tonKho = tongNhap - tongCap;

                    if (tonKho < chiTiet.SoLuong)
                    {
                        thuocKhongDu.Add($"{chiTiet.Thuoc.TenThuoc} (Tồn: {tonKho}, Cần: {chiTiet.SoLuong})");
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

                // Lấy kho mặc định
                var kho = await _benhVienContext.Khos
                    .FirstOrDefaultAsync(k => k.TrangThai == "HoatDong");

                if (kho == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy kho để cấp phát" });
                }

                // Tạo phiếu cấp phát
                var maPhieuCap = await GenerateMaPhieuCapPhat();
                decimal tongTien = 0;

                var phieuCapPhat = new PhieuCapPhat
                {
                    MaPhieuCap = maPhieuCap,
                    MaDonThuoc = model.MaDonThuoc,
                    MaBenhNhan = donThuoc.MaBenhNhan,
                    MaKho = kho.MaKho,
                    NgayCapPhat = DateTime.Now,
                    NhanVienCap = maNhanVien,
                    TongTien = 0, // Sẽ cập nhật sau
                    GhiChu = model.GhiChu,
                    TrangThai = "DaCapPhat",
                    NgayTao = DateTime.Now
                };

                _benhVienContext.PhieuCapPhats.Add(phieuCapPhat);

                // Tạo chi tiết phiếu cấp phát theo FIFO
                foreach (var chiTiet in donThuoc.ChiTietDonThuocs)
                {
                    var thuoc = chiTiet.Thuoc;
                    var donGia = thuoc.GiaXuat.HasValue ? thuoc.GiaXuat.Value : 0m;
                    var thanhTien = chiTiet.SoLuong * donGia;
                    tongTien += thanhTien;

                    var soLuongCanCap = chiTiet.SoLuong;

                    // Lấy danh sách lô theo FIFO (lô cũ nhất trước)
                    // Tính số lượng còn lại của mỗi lô = Nhập - Cấp
                    var loThuocIds = await _benhVienContext.LoThuocs
                        .Where(l => l.MaThuoc == chiTiet.MaThuoc && l.TrangThai == "ConHang")
                        .OrderBy(l => l.NgayNhap)
                        .Select(l => l.MaLo)
                        .ToListAsync();

                    foreach (var maLo in loThuocIds)
                    {
                        if (soLuongCanCap <= 0) break;

                        // Tính số lượng còn của lô này
                        var nhapLo = await _benhVienContext.ChiTietPhieuNhaps
                            .Where(ct => ct.MaLo == maLo)
                            .SumAsync(ct => ct.SoLuongNhap);

                        var capLo = await _benhVienContext.ChiTietPhieuCaps
                            .Where(ct => ct.MaLo == maLo)
                            .SumAsync(ct => ct.SoLuongCap);

                        var conLaiLo = nhapLo - capLo;

                        if (conLaiLo <= 0) continue;

                        var soLuongCapTuLo = Math.Min(soLuongCanCap, conLaiLo);

                        // Tạo chi tiết phiếu cấp
                        var chiTietPhieuCap = new ChiTietPhieuCap
                        {
                            MaPhieuCap = maPhieuCap,
                            MaLo = maLo,
                            SoLuongCap = soLuongCapTuLo,
                            DonGiaCap = donGia
                            // ThanhTien sẽ được tính tự động bởi computed column
                        };

                        _benhVienContext.ChiTietPhieuCaps.Add(chiTietPhieuCap);

                        soLuongCanCap -= soLuongCapTuLo;

                        // Cập nhật trạng thái lô nếu hết hàng
                        var loThuoc = await _benhVienContext.LoThuocs.FindAsync(maLo);
                        if (loThuoc != null && (nhapLo - capLo - soLuongCapTuLo) <= 0)
                        {
                            loThuoc.TrangThai = "HetHang";
                        }
                    }
                }

                // Cập nhật tổng tiền phiếu cấp phát
                phieuCapPhat.TongTien = tongTien;

                // Cập nhật trạng thái đơn thuốc
                donThuoc.TrangThai = "DaCapPhat";

                // Tạo hóa đơn
                var maHoaDon = await GenerateMaHoaDon();
                decimal tienBHYT = 0;

                // Kiểm tra bảo hiểm
                var theBHYT = donThuoc.BenhNhan.TheBHYTs
                    .FirstOrDefault(t => t.TrangThai == "ConHan" &&
                                        t.NgayBatDau <= DateTime.Now &&
                                        t.NgayHetHan >= DateTime.Now);

                if (theBHYT != null)
                {
                    tienBHYT = tongTien * theBHYT.MucHuong / 100;
                }

                decimal tienBenhNhan = tongTien - tienBHYT;

                var hoaDon = new HoaDon
                {
                    MaHoaDon = maHoaDon,
                    MaBenhNhan = donThuoc.MaBenhNhan,
                    MaDonThuoc = model.MaDonThuoc,
                    NgayTaoHoaDon = DateTime.Now,
                    TongTien = tongTien,
                    TienBHYTChiTra = tienBHYT,
                    TienBenhNhanCanTra = tienBenhNhan,
                    TrangThaiThanhToan = "ChuaTra",
                    NgayTao = DateTime.Now
                };

                _benhVienContext.HoaDons.Add(hoaDon);

                await _benhVienContext.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "Cấp phát thuốc thành công",
                    maPhieuCap,
                    maHoaDon,
                    tongTien,
                    tienBenhNhan
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        // ==================== HÓA ĐƠN & THANH TOÁN ====================

        // GET: /DonThuoc/HoaDon - Xem hóa đơn
        [HttpGet]
        public async Task<IActionResult> HoaDon(string maHoaDon)
        {
            try
            {
                var hoaDon = await _benhVienContext.HoaDons
                    .Include(h => h.BenhNhan)
                        .ThenInclude(b => b.TheBHYTs)
                    .Include(h => h.DonThuoc)
                        .ThenInclude(d => d.ChiTietDonThuocs)
                            .ThenInclude(ct => ct.Thuoc)
                    .FirstOrDefaultAsync(h => h.MaHoaDon == maHoaDon);

                if (hoaDon == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy hóa đơn" });
                }

                var theBHYT = hoaDon.BenhNhan.TheBHYTs
                    .FirstOrDefault(t => t.TrangThai == "ConHan");

                var result = new
                {
                    hoaDon.MaHoaDon,
                    NgayLap = hoaDon.NgayTaoHoaDon,
                    hoaDon.TongTien,
                    TyLeBaoHiem = theBHYT?.MucHuong ?? 0,
                    TienBaoHiemChiTra = hoaDon.TienBHYTChiTra,
                    hoaDon.TienBenhNhanCanTra,
                    hoaDon.TrangThaiThanhToan,
                    BenhNhan = new
                    {
                        hoaDon.BenhNhan.MaBenhNhan,
                        hoaDon.BenhNhan.TenBenhNhan,
                        MaTheBHYT = theBHYT?.SoTheBHYT
                    },
                    ChiTiet = hoaDon.DonThuoc.ChiTietDonThuocs.Select(ct => new
                    {
                        ct.MaThuoc,
                        TenThuoc = ct.Thuoc.TenThuoc,
                        ct.SoLuong,
                        DonGia = ct.Thuoc.GiaXuat.HasValue ? ct.Thuoc.GiaXuat.Value : 0m,
                        ThanhTien = ct.SoLuong * (ct.Thuoc.GiaXuat.HasValue ? ct.Thuoc.GiaXuat.Value : 0m)
                    }).ToList()
                };

                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        // POST: /DonThuoc/ThanhToan - Thanh toán hóa đơn
        [HttpPost]
        [Authorize(Roles = "Admin,DuocSi")]
        public async Task<IActionResult> ThanhToan([FromBody] ThanhToanModel model)
        {
            try
            {
                var hoaDon = await _benhVienContext.HoaDons
                    .FirstOrDefaultAsync(h => h.MaHoaDon == model.MaHoaDon);

                if (hoaDon == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy hóa đơn" });
                }

                if (hoaDon.TrangThaiThanhToan == "DaTra")
                {
                    return Json(new { success = false, message = "Hóa đơn này đã được thanh toán" });
                }

                // Kiểm tra số tiền thanh toán
                if (model.SoTienThanhToan < hoaDon.TienBenhNhanCanTra)
                {
                    return Json(new { success = false, message = "Số tiền thanh toán không đủ" });
                }

                // Cập nhật trạng thái thanh toán
                hoaDon.TrangThaiThanhToan = "DaTra";
                hoaDon.HinhThucThanhToan = model.HinhThucThanhToan;
                hoaDon.TienDaTra = model.SoTienThanhToan;

                // Tạo phiếu thu tiền
                var maPhieuThu = await GenerateMaPhieuThu();
                var phieuThu = new PhieuThuTien
                {
                    MaPhieuThu = maPhieuThu,
                    MaHoaDon = model.MaHoaDon,
                    NgayThu = DateTime.Now,
                    SoTienThu = model.SoTienThanhToan,
                    HinhThucThanhToan = model.HinhThucThanhToan,
                    NhanVienThu = User.FindFirstValue("MaNhanVien"),
                    TrangThai = "DaXacNhan",
                    NgayTao = DateTime.Now
                };

                _benhVienContext.PhieuThuTiens.Add(phieuThu);

                await _benhVienContext.SaveChangesAsync();

                var soTienThua = model.SoTienThanhToan - hoaDon.TienBenhNhanCanTra;

                return Json(new
                {
                    success = true,
                    message = "Thanh toán thành công",
                    maPhieuThu,
                    soTienThua = soTienThua > 0 ? soTienThua : 0
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
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
                .Select(b => new { b.MaBenhNhan, b.TenBenhNhan })
                .OrderBy(b => b.TenBenhNhan)
                .ToListAsync();
            ViewBag.BenhNhans = benhNhans;

            // Load danh sách chẩn đoán
            var chanDoans = await _benhVienContext.ChanDoans
                .Select(c => new { c.MaChanDoan, c.ChanDoanSoBo })
                .OrderBy(c => c.ChanDoanSoBo)
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
                    DonGia = t.GiaXuat
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

            // Load danh sách bệnh nhân và thuốc
            var benhNhans = await _benhVienContext.BenhNhans
                .Where(b => b.TrangThai == "HoatDong")
                .Select(b => new { b.MaBenhNhan, b.TenBenhNhan })
                .OrderBy(b => b.TenBenhNhan)
                .ToListAsync();
            ViewBag.BenhNhans = benhNhans;

            var thuocs = await _benhVienContext.Thuocs
                .Where(t => t.TrangThai == "KichHoat")
                .Select(t => new
                {
                    t.MaThuoc,
                    t.TenThuoc,
                    t.HamLuong,
                    t.DonViTinh,
                    DonGia = t.GiaXuat
                })
                .OrderBy(t => t.TenThuoc)
                .ToListAsync();
            ViewBag.Thuocs = thuocs;
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

        private async Task<string> GenerateMaPhieuThu()
        {
            var today = DateTime.Now.ToString("yyyyMMdd");
            var count = await _benhVienContext.PhieuThuTiens
                .Where(p => p.MaPhieuThu.StartsWith($"PT{today}"))
                .CountAsync();
            return $"PT{today}{(count + 1):D4}";
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

    public class ThanhToanModel
    {
        public string MaHoaDon { get; set; }
        public string HinhThucThanhToan { get; set; }
        public decimal SoTienThanhToan { get; set; }
    }
}