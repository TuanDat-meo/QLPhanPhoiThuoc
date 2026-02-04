using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLPhanPhoiThuoc.Models.EF;
using System.Security.Claims;

namespace QLPhanPhoiThuoc.Controllers
{
    [Authorize(Roles = "User")]
    public class UserController : Controller
    {
        private readonly BenhVienDbContext _benhVienContext;
        private readonly VNeIDDbContext _vneIdContext;

        public UserController(BenhVienDbContext benhVienContext, VNeIDDbContext vneIdContext)
        {
            _benhVienContext = benhVienContext;
            _vneIdContext = vneIdContext;
        }

        // GET: /User/UserIndex
        public async Task<IActionResult> UserIndex()
        {
            // Lấy thông tin user từ claims
            var maBenhNhan = User.FindFirstValue("MaBenhNhan");
            var tenBenhNhan = User.FindFirstValue("TenBenhNhan");

            // Lấy thông tin chi tiết bệnh nhân từ database
            if (!string.IsNullOrEmpty(maBenhNhan))
            {
                var benhNhan = await _benhVienContext.BenhNhans
                    .FirstOrDefaultAsync(b => b.MaBenhNhan == maBenhNhan);

                if (benhNhan != null)
                {
                    ViewBag.MaBenhNhan = maBenhNhan;
                    ViewBag.TenBenhNhan = benhNhan.TenBenhNhan;
                    ViewBag.CCCD = benhNhan.CCCD;
                    ViewBag.NgaySinh = benhNhan.NgaySinh;
                    ViewBag.GioiTinh = benhNhan.GioiTinh;
                    ViewBag.DiaChi = benhNhan.DiaChi;
                    ViewBag.SoDienThoai = benhNhan.SoDienThoai;
                    ViewBag.Email = benhNhan.Email;
                    ViewBag.NhomMau = benhNhan.NhomMau;

                    // ==================== THỐNG KÊ CÁ NHÂN ====================

                    // 1. Tổng số lần khám bệnh
                    var tongLanKham = await _benhVienContext.ChanDoans
                        .Where(c => c.MaBenhNhan == maBenhNhan)
                        .CountAsync();
                    ViewBag.TongLanKham = tongLanKham;

                    // 2. Tổng số đơn thuốc
                    var tongDonThuoc = await _benhVienContext.DonThuocs
                        .Where(d => d.MaBenhNhan == maBenhNhan)
                        .CountAsync();
                    ViewBag.TongDonThuoc = tongDonThuoc;

                    // 3. Số đơn thuốc chưa lấy
                    var donChuaLayThuoc = await _benhVienContext.DonThuocs
                        .Where(d => d.MaBenhNhan == maBenhNhan &&
                                   (d.TrangThai == "ChoCapPhat" || d.TrangThai == "DangXuLy"))
                        .CountAsync();
                    ViewBag.DonChuaLayThuoc = donChuaLayThuoc;

                    // 4. Tổng chi phí đã thanh toán
                    var tongChiPhi = await _benhVienContext.HoaDons
                        .Where(h => h.MaBenhNhan == maBenhNhan &&
                                   h.TrangThaiThanhToan == "DaTra")
                        .SumAsync(h => (decimal?)h.TienDaTra) ?? 0;
                    ViewBag.TongChiPhi = tongChiPhi;

                    // 5. Kiểm tra thẻ BHYT
                    var theBHYT = await _benhVienContext.TheBHYTs
                        .Where(t => t.MaBenhNhan == maBenhNhan && t.TrangThai == "ConHan")
                        .OrderByDescending(t => t.NgayHetHan)
                        .FirstOrDefaultAsync();

                    if (theBHYT != null)
                    {
                        ViewBag.CoTheBHYT = true;
                        ViewBag.SoTheBHYT = theBHYT.SoTheBHYT;
                        ViewBag.NgayHetHanBHYT = theBHYT.NgayHetHan;
                        ViewBag.MucHuongBHYT = theBHYT.MucHuong;
                        ViewBag.NoiDKKCB = theBHYT.NoiDangKyKCB;
                    }
                    else
                    {
                        ViewBag.CoTheBHYT = false;
                    }

                    // ==================== LỊCH SỬ KHÁM BỆNH GẦN ĐÂY ====================
                    var lichSuKham = await _benhVienContext.ChanDoans
                        .Where(c => c.MaBenhNhan == maBenhNhan)
                        .OrderByDescending(c => c.NgayChanDoan)
                        .Take(5)
                        .Select(c => new
                        {
                            c.MaChanDoan,
                            c.NgayChanDoan,
                            c.TenBenh,
                            TrieuChung = c.TrieuChung ?? "",
                            ChanDoanSoBo = c.ChanDoanSoBo ?? "",
                            KetLuan = c.KetLuan ?? "",
                            BacSi = c.NhanVien != null ? c.NhanVien.TenNhanVien : "Chưa cập nhật",
                            ChuyenKhoa = c.NhanVien != null ? c.NhanVien.ChuyenKhoa : ""
                        })
                        .ToListAsync();

                    ViewBag.LichSuKham = lichSuKham;

                    // ==================== ĐỌN THUỐC GẦN ĐÂY ====================
                    var donThuocGanDay = await _benhVienContext.DonThuocs
                        .Where(d => d.MaBenhNhan == maBenhNhan)
                        .OrderByDescending(d => d.NgayKeDon)
                        .Take(5)
                        .Select(d => new
                        {
                            d.MaDonThuoc,
                            d.NgayKeDon,
                            d.LoaiDon,
                            d.ChanDoanSoBo,
                            d.TongTien,
                            d.TrangThai,
                            GhiChuBacSi = d.GhiChuBacSi ?? "",
                            BacSi = d.NhanVien != null ? d.NhanVien.TenNhanVien : "Chưa cập nhật",
                            SoLoaiThuoc = d.ChiTietDonThuocs.Count
                        })
                        .ToListAsync();

                    ViewBag.DonThuocGanDay = donThuocGanDay;

                    // ==================== HÓA ĐƠN CHƯA THANH TOÁN ====================
                    var hoaDonChuaTra = await _benhVienContext.HoaDons
                        .Where(h => h.MaBenhNhan == maBenhNhan &&
                                   (h.TrangThaiThanhToan == "ChuaTra" || h.TrangThaiThanhToan == "DangXuLy"))
                        .OrderByDescending(h => h.NgayTaoHoaDon)
                        .Select(h => new
                        {
                            h.MaHoaDon,
                            h.NgayTaoHoaDon,
                            h.TongTien,
                            h.TienBHYTChiTra,
                            h.TienBenhNhanCanTra,
                            h.TrangThaiThanhToan
                        })
                        .ToListAsync();

                    ViewBag.HoaDonChuaTra = hoaDonChuaTra;

                    // ==================== THỐNG KÊ CHI PHÍ THEO THÁNG ====================
                    var ngayHienTai = DateTime.Now;
                    var chiPhi6Thang = new List<decimal>();
                    var thang6Thang = new List<string>();

                    for (int i = 5; i >= 0; i--)
                    {
                        var thang = ngayHienTai.AddMonths(-i);
                        var dauThang = new DateTime(thang.Year, thang.Month, 1);
                        var cuoiThang = dauThang.AddMonths(1).AddDays(-1);

                        var chiPhiThang = await _benhVienContext.HoaDons
                            .Where(h => h.MaBenhNhan == maBenhNhan &&
                                       h.NgayTaoHoaDon >= dauThang &&
                                       h.NgayTaoHoaDon <= cuoiThang &&
                                       h.TrangThaiThanhToan == "DaTra")
                            .SumAsync(h => (decimal?)h.TienDaTra) ?? 0;

                        chiPhi6Thang.Add(chiPhiThang / 1000); // Chuyển sang nghìn đồng
                        thang6Thang.Add(thang.ToString("MM/yyyy"));
                    }

                    ViewBag.ChiPhi6Thang = chiPhi6Thang;
                    ViewBag.Thang6Thang = thang6Thang;

                    // ==================== HOẠT ĐỘNG GẦN ĐÂY ====================
                    var hoatDongGanDay = new List<dynamic>();

                    // Lấy các hoạt động gần đây (khám bệnh, lấy thuốc, thanh toán)
                    var khamBenh = await _benhVienContext.ChanDoans
                        .Where(c => c.MaBenhNhan == maBenhNhan)
                        .OrderByDescending(c => c.NgayChanDoan)
                        .Take(3)
                        .Select(c => new
                        {
                            LoaiHoatDong = "KhamBenh",
                            ThoiGian = c.NgayChanDoan,
                            NoiDung = $"Khám bệnh - {c.TenBenh}",
                            ChiTiet = c.NhanVien != null ? $"BS. {c.NhanVien.TenNhanVien}" : "",
                            Icon = "fa-stethoscope",
                            Color = "primary"
                        })
                        .ToListAsync();

                    var capPhatThuoc = await _benhVienContext.PhieuCapPhats
                        .Where(p => p.DonThuoc.MaBenhNhan == maBenhNhan)
                        .OrderByDescending(p => p.NgayCap)
                        .Take(3)
                        .Select(p => new
                        {
                            LoaiHoatDong = "CapPhatThuoc",
                            ThoiGian = p.NgayCap,
                            NoiDung = $"Lấy thuốc - Đơn {p.MaDonThuoc}",
                            ChiTiet = $"{p.ChiTietPhieuCaps.Count} loại thuốc",
                            Icon = "fa-pills",
                            Color = "success"
                        })
                        .ToListAsync();

                    var thanhToan = await _benhVienContext.PhieuThuTiens
                        .Where(p => p.HoaDon.MaBenhNhan == maBenhNhan)
                        .OrderByDescending(p => p.NgayThu)
                        .Take(3)
                        .Select(p => new
                        {
                            LoaiHoatDong = "ThanhToan",
                            ThoiGian = p.NgayThu,
                            NoiDung = $"Thanh toán - {p.SoTienThu:N0}đ",
                            ChiTiet = p.HinhThucThanhToan,
                            Icon = "fa-money-bill-wave",
                            Color = "info"
                        })
                        .ToListAsync();

                    // Gộp và sắp xếp theo thời gian
                    hoatDongGanDay.AddRange(khamBenh.Cast<dynamic>());
                    hoatDongGanDay.AddRange(capPhatThuoc.Cast<dynamic>());
                    hoatDongGanDay.AddRange(thanhToan.Cast<dynamic>());

                    ViewBag.HoatDongGanDay = hoatDongGanDay
                        .OrderByDescending(h => h.ThoiGian)
                        .Take(10)
                        .ToList();

                    // ==================== THÔNG BÁO ====================
                    var thongBao = await _benhVienContext.ThongBaos
                        .Where(t => t.NguoiNhan == maBenhNhan || t.NguoiNhan == null)
                        .OrderByDescending(t => t.NgayTao)
                        .Take(5)
                        .Select(t => new
                        {
                            t.MaThongBao,
                            t.TieuDe,
                            t.NoiDung,
                            t.LoaiThongBao,
                            t.DoUuTien,
                            t.DaDoc,
                            t.NgayTao
                        })
                        .ToListAsync();

                    ViewBag.ThongBao = thongBao;
                }
            }
            else
            {
                ViewBag.TenBenhNhan = tenBenhNhan ?? "Người dùng";
                ViewBag.TongLanKham = 0;
                ViewBag.TongDonThuoc = 0;
                ViewBag.DonChuaLayThuoc = 0;
                ViewBag.TongChiPhi = 0;
            }

            return View();
        }

        // ==================== API ENDPOINTS CHO USER DASHBOARD ====================

        // API: Lấy thông tin tổng quan
        [HttpGet]
        public async Task<IActionResult> GetUserStats()
        {
            var maBenhNhan = User.FindFirstValue("MaBenhNhan");

            if (string.IsNullOrEmpty(maBenhNhan))
            {
                return Json(new { success = false, message = "Không tìm thấy thông tin bệnh nhân" });
            }

            var stats = new
            {
                tongLanKham = await _benhVienContext.ChanDoans
                    .Where(c => c.MaBenhNhan == maBenhNhan)
                    .CountAsync(),
                tongDonThuoc = await _benhVienContext.DonThuocs
                    .Where(d => d.MaBenhNhan == maBenhNhan)
                    .CountAsync(),
                donChuaLayThuoc = await _benhVienContext.DonThuocs
                    .Where(d => d.MaBenhNhan == maBenhNhan &&
                               (d.TrangThai == "ChoCapPhat" || d.TrangThai == "DangXuLy"))
                    .CountAsync(),
                tongChiPhi = await _benhVienContext.HoaDons
                    .Where(h => h.MaBenhNhan == maBenhNhan &&
                               h.TrangThaiThanhToan == "DaTra")
                    .SumAsync(h => (decimal?)h.TienDaTra) ?? 0
            };

            return Json(new { success = true, data = stats });
        }

        // API: Lấy lịch sử khám bệnh
        [HttpGet]
        public async Task<IActionResult> GetMedicalHistory(int page = 1, int pageSize = 10)
        {
            var maBenhNhan = User.FindFirstValue("MaBenhNhan");

            if (string.IsNullOrEmpty(maBenhNhan))
            {
                return Json(new { success = false, message = "Không tìm thấy thông tin bệnh nhân" });
            }

            var query = _benhVienContext.ChanDoans
                .Where(c => c.MaBenhNhan == maBenhNhan)
                .OrderByDescending(c => c.NgayChanDoan);

            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new
                {
                    c.MaChanDoan,
                    c.NgayChanDoan,
                    c.TenBenh,
                    trieuChung = c.TrieuChung ?? "",
                    chanDoanSoBo = c.ChanDoanSoBo ?? "",
                    ketLuan = c.KetLuan ?? "",
                    bacSi = c.NhanVien != null ? c.NhanVien.TenNhanVien : "Chưa cập nhật",
                    chuyenKhoa = c.NhanVien != null ? c.NhanVien.ChuyenKhoa : ""
                })
                .ToListAsync();

            return Json(new
            {
                success = true,
                data = items,
                totalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                currentPage = page
            });
        }

        // API: Lấy danh sách đơn thuốc
        [HttpGet]
        public async Task<IActionResult> GetPrescriptions(int page = 1, int pageSize = 10)
        {
            var maBenhNhan = User.FindFirstValue("MaBenhNhan");

            if (string.IsNullOrEmpty(maBenhNhan))
            {
                return Json(new { success = false, message = "Không tìm thấy thông tin bệnh nhân" });
            }

            var query = _benhVienContext.DonThuocs
                .Where(d => d.MaBenhNhan == maBenhNhan)
                .OrderByDescending(d => d.NgayKeDon);

            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new
                {
                    d.MaDonThuoc,
                    d.NgayKeDon,
                    d.LoaiDon,
                    d.ChanDoanSoBo,
                    d.TongTien,
                    d.TrangThai,
                    bacSi = d.NhanVien != null ? d.NhanVien.TenNhanVien : "Chưa cập nhật",
                    soLoaiThuoc = d.ChiTietDonThuocs.Count
                })
                .ToListAsync();

            return Json(new
            {
                success = true,
                data = items,
                totalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                currentPage = page
            });
        }

        // API: Lấy chi tiết đơn thuốc
        [HttpGet]
        public async Task<IActionResult> GetPrescriptionDetails(string maDonThuoc)
        {
            var maBenhNhan = User.FindFirstValue("MaBenhNhan");

            if (string.IsNullOrEmpty(maBenhNhan))
            {
                return Json(new { success = false, message = "Không tìm thấy thông tin bệnh nhân" });
            }

            var donThuoc = await _benhVienContext.DonThuocs
                .Where(d => d.MaDonThuoc == maDonThuoc && d.MaBenhNhan == maBenhNhan)
                .Include(d => d.ChiTietDonThuocs)
                    .ThenInclude(ct => ct.Thuoc)
                .FirstOrDefaultAsync();

            if (donThuoc == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đơn thuốc" });
            }

            var result = new
            {
                donThuoc = new
                {
                    donThuoc.MaDonThuoc,
                    donThuoc.NgayKeDon,
                    donThuoc.LoaiDon,
                    donThuoc.ChanDoanSoBo,
                    ghiChuBacSi = donThuoc.GhiChuBacSi ?? "",
                    donThuoc.TongTien,
                    donThuoc.TrangThai,
                    bacSi = donThuoc.NhanVien != null ? donThuoc.NhanVien.TenNhanVien : "Chưa cập nhật"
                },
                chiTiet = donThuoc.ChiTietDonThuocs.Select(ct => new
                {
                    maThuoc = ct.MaThuoc,
                    maDonThuoc = ct.MaDonThuoc,
                    tenThuoc = ct.Thuoc.TenThuoc,
                    donViTinh = ct.Thuoc.DonViTinh,
                    ct.SoLuong,
                    ct.LieuDung,
                    soNgayDung = ct.SoNgayDung,
                    ct.CachDung,
                    ct.GhiChu,
                    donGia = ct.Thuoc.GiaXuat,
                    thanhTien = ct.SoLuong * ct.Thuoc.GiaXuat
                }).ToList()
            };

            return Json(new { success = true, data = result });
        }

        // API: Lấy danh sách hóa đơn chưa thanh toán
        [HttpGet]
        public async Task<IActionResult> GetUnpaidInvoices()
        {
            var maBenhNhan = User.FindFirstValue("MaBenhNhan");

            if (string.IsNullOrEmpty(maBenhNhan))
            {
                return Json(new { success = false, message = "Không tìm thấy thông tin bệnh nhân" });
            }

            var hoaDon = await _benhVienContext.HoaDons
                .Where(h => h.MaBenhNhan == maBenhNhan &&
                           (h.TrangThaiThanhToan == "ChuaTra" || h.TrangThaiThanhToan == "DangXuLy"))
                .OrderByDescending(h => h.NgayTaoHoaDon)
                .Select(h => new
                {
                    h.MaHoaDon,
                    h.NgayTaoHoaDon,
                    h.TongTien,
                    h.TienBHYTChiTra,
                    h.TienBenhNhanCanTra,
                    h.TienDaTra,
                    conLai = h.TienBenhNhanCanTra - h.TienDaTra,
                    h.TrangThaiThanhToan
                })
                .ToListAsync();

            return Json(new { success = true, data = hoaDon });
        }
    }
}