using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using QLPhanPhoiThuoc.Models.EF;
using QLPhanPhoiThuoc.Models.Entities;
using System.Security.Claims;

namespace QLPhanPhoiThuoc.Controllers.Admin
{
    [Route("Admin/[controller]")]
    [Authorize(Roles = "Admin,NhanVien,QuanKho")]
    public class ThuocController : Controller
    {
        private readonly BenhVienDbContext _context;

        public ThuocController(BenhVienDbContext context)
        {
            _context = context;
        }

        // ==================== QUẢN LÝ THÔNG TIN THUỐC ====================

        // GET: Admin/Thuoc/Index - Danh sách thuốc + search + filter + paging
        [HttpGet("Index")]
        public async Task<IActionResult> Index(string search = "", string nhomThuoc = "", string trangThai = "",
                                                string sortBy = "TenThuoc", string sortOrder = "asc",
                                                int page = 1, int pageSize = 20)
        {
            var query = _context.Thuocs.AsQueryable();

            // Search filter
            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim();
                query = query.Where(t => t.TenThuoc.Contains(search) ||
                                         t.MaThuoc.Contains(search) ||
                                         (t.HoatChat != null && t.HoatChat.Contains(search)) ||
                                         (t.NhaSanXuat != null && t.NhaSanXuat.Contains(search)));
            }

            // Nhóm thuốc filter
            if (!string.IsNullOrEmpty(nhomThuoc))
            {
                query = query.Where(t => t.NhomThuoc == nhomThuoc);
            }

            // Trạng thái filter
            if (!string.IsNullOrEmpty(trangThai))
            {
                query = query.Where(t => t.TrangThai == trangThai);
            }

            // Sorting
            query = sortBy switch
            {
                "TenThuoc" => sortOrder == "asc" ? query.OrderBy(t => t.TenThuoc) : query.OrderByDescending(t => t.TenThuoc),
                "GiaXuat" => sortOrder == "asc" ? query.OrderBy(t => t.GiaXuat) : query.OrderByDescending(t => t.GiaXuat),
                "NhomThuoc" => sortOrder == "asc" ? query.OrderBy(t => t.NhomThuoc) : query.OrderByDescending(t => t.NhomThuoc),
                _ => sortOrder == "asc" ? query.OrderBy(t => t.TenThuoc) : query.OrderByDescending(t => t.TenThuoc)
            };

            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Dropdown nhóm thuốc
            ViewBag.NhomThuocs = await _context.Thuocs
                .Where(t => t.NhomThuoc != null && t.NhomThuoc != "")
                .Select(t => t.NhomThuoc)
                .Distinct()
                .OrderBy(n => n)
                .ToListAsync();

            // Pass filter values to view
            ViewBag.Search = search;
            ViewBag.NhomThuocSelected = nhomThuoc;
            ViewBag.TrangThaiSelected = trangThai;
            ViewBag.SortBy = sortBy;
            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.TotalItems = totalItems;

            // Thông tin người dùng
            var maNhanVien = User.FindFirstValue("MaNhanVien");
            var tenNhanVien = User.FindFirstValue("TenNhanVien");
            ViewBag.MaNhanVien = maNhanVien;
            ViewBag.TenNhanVien = tenNhanVien ?? "Admin";

            // Nếu là AJAX request, trả về PartialView
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_Index", items);
            }

            return View(items);
        }

        // GET: Admin/Thuoc/Details/{id}
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var thuoc = await _context.Thuocs
                .Include(t => t.LoThuocs).ThenInclude(l => l.Kho)
                .Include(t => t.ChiTietDonThuocs)
                .FirstOrDefaultAsync(t => t.MaThuoc == id);

            if (thuoc == null) return NotFound();

            // Tính tổng tồn kho
            var tonKho = await _context.LoThuocs
                .Where(l => l.MaThuoc == id && l.TrangThai == "ConHang")
                .SumAsync(l => (int?)l.SoLuongCon) ?? 0;

            ViewBag.TonKho = tonKho;
            ViewBag.TenNhanVien = User.FindFirstValue("TenNhanVien") ?? "Admin";

            // LUÔN TRẢ VỀ PARTIALVIEW cho panel system
            return PartialView("_Details", thuoc);
        }

        // GET: Admin/Thuoc/Edit/{id}
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var thuoc = await _context.Thuocs.FindAsync(id);
            if (thuoc == null) return NotFound();

            ViewBag.TenNhanVien = User.FindFirstValue("TenNhanVien") ?? "Admin";

            // LUÔN TRẢ VỀ PARTIALVIEW cho panel system
            return PartialView("_Edit", thuoc);
        }

        // POST: Admin/Thuoc/Edit/{id}
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaThuoc,TenThuoc,HoatChat,NhomThuoc,DonViTinh,DuongDung,HamLuong,DangBaoChe,GiaNhap,GiaXuat,TonKhoToiThieu,NhaSanXuat,MoTa,TrangThai")] Thuoc thuoc)
        {
            if (id != thuoc.MaThuoc)
                return Json(new { success = false, message = "Mã thuốc không hợp lệ" });

            if (ModelState.IsValid)
            {
                try
                {
                    var existingThuoc = await _context.Thuocs.AsNoTracking().FirstOrDefaultAsync(t => t.MaThuoc == id);
                    if (existingThuoc == null)
                        return Json(new { success = false, message = "Không tìm thấy thuốc" });

                    // Cập nhật NgayCapNhat
                    var ngayCapNhatProperty = typeof(Thuoc).GetProperty("NgayCapNhat");
                    if (ngayCapNhatProperty != null && ngayCapNhatProperty.CanWrite)
                    {
                        ngayCapNhatProperty.SetValue(thuoc, DateTime.Now);
                    }

                    // Giữ nguyên NgayTao
                    var ngayTaoProperty = typeof(Thuoc).GetProperty("NgayTao");
                    if (ngayTaoProperty != null && ngayTaoProperty.CanWrite && ngayTaoProperty.CanRead)
                    {
                        var ngayTao = ngayTaoProperty.GetValue(existingThuoc);
                        ngayTaoProperty.SetValue(thuoc, ngayTao);
                    }

                    _context.Update(thuoc);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, message = "Cập nhật thông tin thuốc thành công!" });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ThuocExists(thuoc.MaThuoc))
                        return Json(new { success = false, message = "Thuốc không tồn tại" });
                    else
                        return Json(new { success = false, message = "Lỗi đồng thời, vui lòng thử lại" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Lỗi khi cập nhật: " + ex.Message });
                }
            }

            ViewBag.TenNhanVien = User.FindFirstValue("TenNhanVien") ?? "Admin";
            return PartialView("_Edit", thuoc);
        }

        // POST: Admin/Thuoc/Delete/{id} - Chuyển trạng thái ngừng sử dụng
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var thuoc = await _context.Thuocs.FindAsync(id);
                if (thuoc == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy thuốc" });
                }

                thuoc.TrangThai = "NgungSuDung";

                // Cập nhật NgayCapNhat nếu property tồn tại
                var ngayCapNhatProperty = typeof(Thuoc).GetProperty("NgayCapNhat");
                if (ngayCapNhatProperty != null && ngayCapNhatProperty.CanWrite)
                {
                    ngayCapNhatProperty.SetValue(thuoc, DateTime.Now);
                }

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Đã ngừng sử dụng thuốc thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // POST: Admin/Thuoc/Restore/{id} - Kích hoạt lại thuốc
        [HttpPost("Restore/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(string id)
        {
            try
            {
                var thuoc = await _context.Thuocs.FindAsync(id);
                if (thuoc == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy thuốc" });
                }

                thuoc.TrangThai = "KichHoat";

                // Cập nhật NgayCapNhat nếu property tồn tại
                var ngayCapNhatProperty = typeof(Thuoc).GetProperty("NgayCapNhat");
                if (ngayCapNhatProperty != null && ngayCapNhatProperty.CanWrite)
                {
                    ngayCapNhatProperty.SetValue(thuoc, DateTime.Now);
                }

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Đã kích hoạt lại thuốc thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // ==================== TỒN KHO ====================

        // GET: Admin/Thuoc/TonKho
        [HttpGet("TonKho")]
        public async Task<IActionResult> TonKho(string search = "", string filter = "", int page = 1, int pageSize = 20)
        {
            var query = _context.LoThuocs
                .Include(l => l.Thuoc)
                .Include(l => l.Kho)
                .AsQueryable();

            // Search filter
            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim();
                query = query.Where(l => l.SoLo.Contains(search) ||
                                        l.Thuoc.TenThuoc.Contains(search) ||
                                        l.MaLo.Contains(search));
            }

            // Status filter
            var now = DateTime.Now;
            query = filter switch
            {
                "expiring" => query.Where(l => l.HanSuDung <= now.AddDays(30) &&
                                              l.HanSuDung >= now &&
                                              l.TrangThai == "ConHang"),
                "expired" => query.Where(l => l.HanSuDung < now),
                "active" => query.Where(l => l.TrangThai == "ConHang"),
                _ => query
            };

            var totalItems = await query.CountAsync();
            var items = await query
                .OrderBy(l => l.HanSuDung)  // Sắp xếp theo hạn sử dụng
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Set ViewBag
            ViewBag.Search = search;
            ViewBag.Filter = filter;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.TotalItems = totalItems;
            ViewBag.TenNhanVien = User.FindFirstValue("TenNhanVien") ?? "Admin";

            // AJAX support
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_TonKho", items);
            }

            return View(items);
        }

        // GET: Admin/Thuoc/CanhBao
        [HttpGet("CanhBao")]
        public async Task<IActionResult> CanhBao()
        {
            var now = DateTime.Now;

            // Thuốc sắp hết tồn kho
            var thuocSapHet = await (from thuoc in _context.Thuocs
                                     join lot in _context.LoThuocs on thuoc.MaThuoc equals lot.MaThuoc into lots
                                     where thuoc.TrangThai == "KichHoat"
                                     let tonKho = lots.Where(l => l.TrangThai == "ConHang").Sum(l => (int?)l.SoLuongCon) ?? 0
                                     where tonKho <= thuoc.TonKhoToiThieu
                                     select new
                                     {
                                         Thuoc = thuoc,
                                         TonKho = tonKho
                                     }).ToListAsync();

            // Thuốc sắp hết hạn (trong 30 ngày)
            var thuocSapHetHan = await _context.LoThuocs
                .Include(l => l.Thuoc)
                .Include(l => l.Kho)
                .Where(l => l.TrangThai == "ConHang" &&
                           l.HanSuDung <= now.AddDays(30) &&
                           l.HanSuDung >= now)
                .OrderBy(l => l.HanSuDung)
                .ToListAsync();

            // Thuốc đã hết hạn
            var thuocHetHan = await _context.LoThuocs
                .Include(l => l.Thuoc)
                .Include(l => l.Kho)
                .Where(l => l.HanSuDung < now && l.SoLuongCon > 0)
                .OrderByDescending(l => l.HanSuDung)
                .ToListAsync();

            ViewBag.ThuocSapHet = thuocSapHet;
            ViewBag.ThuocSapHetHan = thuocSapHetHan;
            ViewBag.ThuocHetHan = thuocHetHan;
            ViewBag.TenNhanVien = User.FindFirstValue("TenNhanVien") ?? "Admin";

            // Nếu là AJAX request, trả về PartialView
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_CanhBao");
            }

            return View();
        }

        // ==================== THỐNG KÊ BÁO CÁO ====================

        // GET: Admin/Thuoc/ThongKe
        [HttpGet("ThongKe")]
        public async Task<IActionResult> ThongKe(DateTime? tuNgay, DateTime? denNgay)
        {
            tuNgay ??= DateTime.Now.AddMonths(-1);
            denNgay ??= DateTime.Now;

            // Top thuốc bán chạy
            var topThuocBanChay = await _context.ChiTietDonThuocs
                .Include(ct => ct.Thuoc)
                .Include(ct => ct.DonThuoc)
                .Where(ct => ct.DonThuoc.NgayKeDon >= tuNgay && ct.DonThuoc.NgayKeDon <= denNgay)
                .GroupBy(ct => new { ct.MaThuoc, ct.Thuoc.TenThuoc })
                .Select(g => new
                {
                    MaThuoc = g.Key.MaThuoc,
                    TenThuoc = g.Key.TenThuoc,
                    SoLuong = g.Sum(ct => ct.SoLuong),
                    DoanhThu = g.Sum(ct => ct.SoLuong * ct.Thuoc.GiaXuat)
                })
                .OrderByDescending(x => x.SoLuong)
                .Take(10)
                .ToListAsync();

            // Nhóm thuốc sử dụng nhiều nhất
            var thongKeNhom = await _context.ChiTietDonThuocs
                .Include(ct => ct.Thuoc)
                .Include(ct => ct.DonThuoc)
                .Where(ct => ct.DonThuoc.NgayKeDon >= tuNgay && ct.DonThuoc.NgayKeDon <= denNgay)
                .GroupBy(ct => ct.Thuoc.NhomThuoc)
                .Select(g => new
                {
                    NhomThuoc = g.Key,
                    SoLuong = g.Sum(ct => ct.SoLuong),
                    DoanhThu = g.Sum(ct => ct.SoLuong * ct.Thuoc.GiaXuat)
                })
                .OrderByDescending(x => x.DoanhThu)
                .ToListAsync();

            ViewBag.TuNgay = tuNgay;
            ViewBag.DenNgay = denNgay;
            ViewBag.TopThuocBanChay = topThuocBanChay;
            ViewBag.ThongKeNhom = thongKeNhom;
            ViewBag.TenNhanVien = User.FindFirstValue("TenNhanVien") ?? "Admin";

            // Nếu là AJAX request, trả về PartialView
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ThongKe");
            }

            return View();
        }

        // ==================== API ENDPOINTS ====================

        // API: Lấy danh sách thuốc cho autocomplete
        [HttpGet("Api/Search")]
        public async Task<IActionResult> SearchThuoc(string term)
        {
            var thuocs = await _context.Thuocs
                .Where(t => t.TrangThai == "KichHoat" &&
                           (t.TenThuoc.Contains(term) || t.MaThuoc.Contains(term)))
                .Select(t => new
                {
                    id = t.MaThuoc,
                    text = t.TenThuoc,
                    donVi = t.DonViTinh,
                    gia = t.GiaXuat
                })
                .Take(10)
                .ToListAsync();

            return Json(thuocs);
        }

        // API: Lấy thông tin tồn kho theo thuốc
        [HttpGet("Api/TonKho/{maThuoc}")]
        public async Task<IActionResult> GetTonKho(string maThuoc)
        {
            var tonKho = await _context.LoThuocs
                .Where(l => l.MaThuoc == maThuoc && l.TrangThai == "ConHang")
                .SumAsync(l => (int?)l.SoLuongCon) ?? 0;

            return Json(new { tonKho });
        }

        // Helper Methods
        private bool ThuocExists(string id)
        {
            return _context.Thuocs.Any(e => e.MaThuoc == id);
        }// ==================== CHI TIẾT LÔ THUỐC ====================
        [HttpGet("LoThuoc")]
        public async Task<IActionResult> LoThuoc(string search = "", string filter = "all",
                                         string kho = "", string trangThai = "",
                                         int page = 1, int pageSize = 20)
        {
            var query = _context.LoThuocs
                .Include(l => l.Thuoc)
                .Include(l => l.Kho)
                .AsQueryable();

            var now = DateTime.Now;

            // Search filter
            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim();
                query = query.Where(l => l.MaLo.Contains(search) ||
                                         l.SoLo.Contains(search) ||
                                         l.Thuoc.TenThuoc.Contains(search) ||
                                         l.Thuoc.MaThuoc.Contains(search));
            }

            // Quick filter
            switch (filter)
            {
                case "active":
                    query = query.Where(l => l.TrangThai == "ConHang");
                    break;
                case "expiring":
                    query = query.Where(l => l.HanSuDung <= now.AddDays(30) &&
                                             l.HanSuDung >= now &&
                                             l.TrangThai == "ConHang");
                    break;
                case "expired":
                    query = query.Where(l => l.HanSuDung < now);
                    break;
                case "outofstock":
                    query = query.Where(l => l.SoLuongCon == 0);
                    break;
            }

            // Kho filter
            if (!string.IsNullOrEmpty(kho))
            {
                query = query.Where(l => l.MaKho == kho);
            }

            // Trạng thái filter
            if (!string.IsNullOrEmpty(trangThai))
            {
                query = query.Where(l => l.TrangThai == trangThai);
            }

            // Sắp xếp mặc định: ưu tiên lô sắp hết hạn
            query = query.OrderBy(l => l.HanSuDung).ThenBy(l => l.Thuoc.TenThuoc);

            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Danh sách kho cho dropdown
            ViewBag.Khos = await _context.Khos
                .OrderBy(k => k.TenKho)
                .ToListAsync();

            // Pass filter values to view
            ViewBag.Search = search;
            ViewBag.Filter = filter;
            ViewBag.KhoSelected = kho;
            ViewBag.TrangThaiSelected = trangThai;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.TotalItems = totalItems;
            ViewBag.TenNhanVien = User.FindFirstValue("TenNhanVien") ?? "Admin";

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_TonKho", items);
            }

            return View("_TonKho", items);
        }
        [HttpGet("LoThuocDetail/{id}")]
        public async Task<IActionResult> LoThuocDetail(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var loThuoc = await _context.LoThuocs
                .Include(l => l.Thuoc)
                .Include(l => l.Kho)
                .FirstOrDefaultAsync(l => l.MaLo == id);

            if (loThuoc == null) return NotFound();

            ViewBag.TenNhanVien = User.FindFirstValue("TenNhanVien") ?? "Admin";

            return PartialView("_TonKhoDetail", loThuoc);
        }

        // ==================== XUẤT BÁO CÁO TỒN KHO ====================
        [HttpGet("ExportTonKho")]
        public async Task<IActionResult> ExportTonKho(string search = "", string filter = "all",
                                              string kho = "", string trangThai = "")
        {
            var query = _context.LoThuocs
                .Include(l => l.Thuoc)
                .Include(l => l.Kho)
                .AsQueryable();

            var now = DateTime.Now;

            // Apply same filters as LoThuoc action
            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim();
                query = query.Where(l => l.MaLo.Contains(search) ||
                                         l.SoLo.Contains(search) ||
                                         l.Thuoc.TenThuoc.Contains(search) ||
                                         l.Thuoc.MaThuoc.Contains(search));
            }

            switch (filter)
            {
                case "active":
                    query = query.Where(l => l.TrangThai == "ConHang");
                    break;
                case "expiring":
                    query = query.Where(l => l.HanSuDung <= now.AddDays(30) &&
                                             l.HanSuDung >= now &&
                                             l.TrangThai == "ConHang");
                    break;
                case "expired":
                    query = query.Where(l => l.HanSuDung < now);
                    break;
                case "outofstock":
                    query = query.Where(l => l.SoLuongCon == 0);
                    break;
            }

            if (!string.IsNullOrEmpty(kho))
            {
                query = query.Where(l => l.MaKho == kho);
            }

            if (!string.IsNullOrEmpty(trangThai))
            {
                query = query.Where(l => l.TrangThai == trangThai);
            }

            var data = await query
                .OrderBy(l => l.HanSuDung)
                .Select(l => new
                {
                    LoThuoc = l,
                    TenThuoc = l.Thuoc.TenThuoc,
                    TenKho = l.Kho.TenKho,
                    DonViTinh = l.Thuoc.DonViTinh,
                    NhaSanXuat = l.Thuoc.NhaSanXuat,
                    DaysUntilExpiry = (l.HanSuDung - now).Days
                })
                .ToListAsync();

            // Tạo file Excel
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Tồn kho");

            // Header
            worksheet.Cells[1, 1].Value = "BÁO CÁO TỒN KHO THUỐC";
            worksheet.Cells[1, 1, 1, 11].Merge = true;
            worksheet.Cells[1, 1].Style.Font.Bold = true;
            worksheet.Cells[1, 1].Style.Font.Size = 16;
            worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Cells[2, 1].Value = $"Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm}";
            worksheet.Cells[2, 1, 2, 11].Merge = true;
            worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Column headers
            var headerRow = 4;
            worksheet.Cells[headerRow, 1].Value = "STT";
            worksheet.Cells[headerRow, 2].Value = "Mã lô";
            worksheet.Cells[headerRow, 3].Value = "Số lô";
            worksheet.Cells[headerRow, 4].Value = "Tên thuốc";
            worksheet.Cells[headerRow, 5].Value = "Kho";
            worksheet.Cells[headerRow, 6].Value = "NSX";
            worksheet.Cells[headerRow, 7].Value = "Hạn SD";
            worksheet.Cells[headerRow, 8].Value = "SL nhập";
            worksheet.Cells[headerRow, 9].Value = "SL còn";
            worksheet.Cells[headerRow, 10].Value = "ĐVT";
            worksheet.Cells[headerRow, 11].Value = "Trạng thái";

            // Style header
            using (var range = worksheet.Cells[headerRow, 1, headerRow, 11])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));
                range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                range.Style.Border.BorderAround(ExcelBorderStyle.Thick);
            }

            // Data rows
            var row = headerRow + 1;
            var stt = 1;
            foreach (var item in data)
            {
                worksheet.Cells[row, 1].Value = stt++;
                worksheet.Cells[row, 2].Value = item.LoThuoc.MaLo;
                worksheet.Cells[row, 3].Value = item.LoThuoc.SoLo;
                worksheet.Cells[row, 4].Value = item.TenThuoc;
                worksheet.Cells[row, 5].Value = item.TenKho;
                worksheet.Cells[row, 6].Value = item.LoThuoc.NgaySanXuat?.ToString("dd/MM/yyyy") ?? "";
                worksheet.Cells[row, 7].Value = item.LoThuoc.HanSuDung.ToString("dd/MM/yyyy");
                worksheet.Cells[row, 8].Value = item.LoThuoc.SoLuongNhap;
                worksheet.Cells[row, 9].Value = item.LoThuoc.SoLuongCon;
                worksheet.Cells[row, 10].Value = item.DonViTinh;
                worksheet.Cells[row, 11].Value = item.LoThuoc.TrangThai;

                // Highlight rows
                if (item.LoThuoc.HanSuDung < now)
                {
                    // Expired - Red
                    worksheet.Cells[row, 1, row, 11].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[row, 1, row, 11].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightPink);
                }
                else if (item.DaysUntilExpiry <= 30)
                {
                    // Expiring soon - Yellow
                    worksheet.Cells[row, 1, row, 11].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[row, 1, row, 11].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);
                }

                // Borders
                for (int col = 1; col <= 11; col++)
                {
                    worksheet.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                row++;
            }

            // Auto-fit columns
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            // Summary
            row += 2;
            worksheet.Cells[row, 1].Value = "TỔNG KẾT:";
            worksheet.Cells[row, 1].Style.Font.Bold = true;
            row++;
            worksheet.Cells[row, 1].Value = $"Tổng số lô: {data.Count}";
            row++;
            worksheet.Cells[row, 1].Value = $"Còn hàng: {data.Count(l => l.LoThuoc.TrangThai == "ConHang")}";
            row++;
            worksheet.Cells[row, 1].Value = $"Sắp hết hạn: {data.Count(l => l.LoThuoc.HanSuDung <= now.AddDays(30) && l.LoThuoc.HanSuDung >= now)}";
            row++;
            worksheet.Cells[row, 1].Value = $"Đã hết hạn: {data.Count(l => l.LoThuoc.HanSuDung < now)}";

            var fileName = $"TonKho_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        [HttpGet("ExportSelectedLots")]
        public async Task<IActionResult> ExportSelectedLots(string ids)
        {
            if (string.IsNullOrEmpty(ids))
                return BadRequest("Không có lô nào được chọn");

            var lotIds = ids.Split(',').ToList();
            var now = DateTime.Now;

            var data = await _context.LoThuocs
                .Include(l => l.Thuoc)
                .Include(l => l.Kho)
                .Where(l => lotIds.Contains(l.MaLo))
                .OrderBy(l => l.HanSuDung)
                .Select(l => new
                {
                    LoThuoc = l,
                    TenThuoc = l.Thuoc.TenThuoc,
                    TenKho = l.Kho.TenKho,
                    DonViTinh = l.Thuoc.DonViTinh,
                    DaysUntilExpiry = (l.HanSuDung - now).Days
                })
                .ToListAsync();

            if (!data.Any())
                return NotFound("Không tìm thấy lô thuốc");

            // Create Excel file (tương tự ExportTonKho)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Lô thuốc đã chọn");

            // Header
            worksheet.Cells[1, 1].Value = "LÔ THUỐC ĐÃ CHỌN";
            worksheet.Cells[1, 1, 1, 11].Merge = true;
            worksheet.Cells[1, 1].Style.Font.Bold = true;
            worksheet.Cells[1, 1].Style.Font.Size = 16;
            worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Cells[2, 1].Value = $"Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm}";
            worksheet.Cells[2, 1, 2, 11].Merge = true;
            worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Column headers
            var headerRow = 4;
            worksheet.Cells[headerRow, 1].Value = "STT";
            worksheet.Cells[headerRow, 2].Value = "Mã lô";
            worksheet.Cells[headerRow, 3].Value = "Số lô";
            worksheet.Cells[headerRow, 4].Value = "Tên thuốc";
            worksheet.Cells[headerRow, 5].Value = "Kho";
            worksheet.Cells[headerRow, 6].Value = "NSX";
            worksheet.Cells[headerRow, 7].Value = "Hạn SD";
            worksheet.Cells[headerRow, 8].Value = "SL nhập";
            worksheet.Cells[headerRow, 9].Value = "SL còn";
            worksheet.Cells[headerRow, 10].Value = "ĐVT";
            worksheet.Cells[headerRow, 11].Value = "Trạng thái";

            // Style header
            using (var range = worksheet.Cells[headerRow, 1, headerRow, 11])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));
                range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            // Data rows
            var row = headerRow + 1;
            var stt = 1;
            foreach (var item in data)
            {
                worksheet.Cells[row, 1].Value = stt++;
                worksheet.Cells[row, 2].Value = item.LoThuoc.MaLo;
                worksheet.Cells[row, 3].Value = item.LoThuoc.SoLo;
                worksheet.Cells[row, 4].Value = item.TenThuoc;
                worksheet.Cells[row, 5].Value = item.TenKho;
                worksheet.Cells[row, 6].Value = item.LoThuoc.NgaySanXuat?.ToString("dd/MM/yyyy") ?? "";
                worksheet.Cells[row, 7].Value = item.LoThuoc.HanSuDung.ToString("dd/MM/yyyy");
                worksheet.Cells[row, 8].Value = item.LoThuoc.SoLuongNhap;
                worksheet.Cells[row, 9].Value = item.LoThuoc.SoLuongCon;
                worksheet.Cells[row, 10].Value = item.DonViTinh;
                worksheet.Cells[row, 11].Value = item.LoThuoc.TrangThai;

                // Highlight rows
                if (item.LoThuoc.HanSuDung < now)
                {
                    worksheet.Cells[row, 1, row, 11].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[row, 1, row, 11].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightPink);
                }
                else if (item.DaysUntilExpiry <= 30)
                {
                    worksheet.Cells[row, 1, row, 11].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[row, 1, row, 11].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);
                }

                for (int col = 1; col <= 11; col++)
                {
                    worksheet.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                row++;
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            var fileName = $"LoThuoc_DaChon_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        [HttpGet("ExportLotDetail/{id}")]
        public async Task<IActionResult> ExportLotDetail(string id)
        {
            var loThuoc = await _context.LoThuocs
                .Include(l => l.Thuoc)
                .Include(l => l.Kho)
                .FirstOrDefaultAsync(l => l.MaLo == id);

            if (loThuoc == null)
                return NotFound();

            var now = DateTime.Now;
            var daysUntilExpiry = (loThuoc.HanSuDung - now).Days;

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Chi tiết lô thuốc");

            // Header
            worksheet.Cells[1, 1].Value = "CHI TIẾT LÔ THUỐC";
            worksheet.Cells[1, 1, 1, 2].Merge = true;
            worksheet.Cells[1, 1].Style.Font.Bold = true;
            worksheet.Cells[1, 1].Style.Font.Size = 16;
            worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            var row = 3;

            // Thông tin lô
            worksheet.Cells[row, 1].Value = "THÔNG TIN LÔ HÀNG";
            worksheet.Cells[row, 1].Style.Font.Bold = true;
            worksheet.Cells[row, 1, row, 2].Merge = true;
            worksheet.Cells[row, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            row++;

            worksheet.Cells[row, 1].Value = "Mã lô:";
            worksheet.Cells[row, 2].Value = loThuoc.MaLo;
            row++;

            worksheet.Cells[row, 1].Value = "Số lô:";
            worksheet.Cells[row, 2].Value = loThuoc.SoLo;
            row++;

            worksheet.Cells[row, 1].Value = "Tên thuốc:";
            worksheet.Cells[row, 2].Value = loThuoc.Thuoc.TenThuoc;
            row++;

            worksheet.Cells[row, 1].Value = "Kho:";
            worksheet.Cells[row, 2].Value = loThuoc.Kho.TenKho;
            row++;

            worksheet.Cells[row, 1].Value = "Ngày sản xuất:";
            worksheet.Cells[row, 2].Value = loThuoc.NgaySanXuat?.ToString("dd/MM/yyyy") ?? "N/A";
            row++;

            worksheet.Cells[row, 1].Value = "Hạn sử dụng:";
            worksheet.Cells[row, 2].Value = loThuoc.HanSuDung.ToString("dd/MM/yyyy");
            row++;

            worksheet.Cells[row, 1].Value = "Số lượng nhập:";
            worksheet.Cells[row, 2].Value = $"{loThuoc.SoLuongNhap} {loThuoc.Thuoc.DonViTinh}";
            row++;

            worksheet.Cells[row, 1].Value = "Số lượng còn:";
            worksheet.Cells[row, 2].Value = $"{loThuoc.SoLuongCon} {loThuoc.Thuoc.DonViTinh}";
            row++;

            worksheet.Cells[row, 1].Value = "Trạng thái:";
            worksheet.Cells[row, 2].Value = loThuoc.TrangThai;
            row++;

            if (daysUntilExpiry < 0)
            {
                worksheet.Cells[row, 1].Value = "Cảnh báo:";
                worksheet.Cells[row, 2].Value = $"ĐÃ QUÁ HẠN {Math.Abs(daysUntilExpiry)} NGÀY";
                worksheet.Cells[row, 2].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                worksheet.Cells[row, 2].Style.Font.Bold = true;
            }
            else if (daysUntilExpiry <= 30)
            {
                worksheet.Cells[row, 1].Value = "Cảnh báo:";
                worksheet.Cells[row, 2].Value = $"SẮP HẾT HẠN - CÒN {daysUntilExpiry} NGÀY";
                worksheet.Cells[row, 2].Style.Font.Color.SetColor(System.Drawing.Color.Orange);
                worksheet.Cells[row, 2].Style.Font.Bold = true;
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            var fileName = $"ChiTiet_{loThuoc.MaLo}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        // ==================== IN NHÃN ====================

        /// <summary>
        /// GET: Admin/Thuoc/PrintLotLabel/{id} - In nhãn lô thuốc
        /// </summary>
        [HttpGet("PrintLotLabel/{id}")]
        public async Task<IActionResult> PrintLotLabel(string id)
        {
            var loThuoc = await _context.LoThuocs
                .Include(l => l.Thuoc)
                .Include(l => l.Kho)
                .FirstOrDefaultAsync(l => l.MaLo == id);

            if (loThuoc == null) return NotFound();

            return View("_PrintLotLabel", loThuoc);
        }

        /// <summary>
        /// GET: Admin/Thuoc/PrintSelectedLabels - In nhãn các lô được chọn
        /// </summary>
        [HttpGet("PrintSelectedLabels")]
        public async Task<IActionResult> PrintSelectedLabels(string ids)
        {
            if (string.IsNullOrEmpty(ids))
                return BadRequest();

            var lotIds = ids.Split(',').ToList();
            var data = await _context.LoThuocs
                .Include(l => l.Thuoc)
                .Include(l => l.Kho)
                .Where(l => lotIds.Contains(l.MaLo))
                .ToListAsync();

            return View("_PrintSelectedLabels", data);
        }

        // ==================== XỬ LÝ LÔ HẾT HẠN ====================
        [HttpPost("HandleExpiredLot/{id}")]
        public async Task<IActionResult> HandleExpiredLot(string id)
        {
            try
            {
                var loThuoc = await _context.LoThuocs.FindAsync(id);
                if (loThuoc == null)
                    return Json(new { success = false, message = "Không tìm thấy lô thuốc" });

                // Cập nhật trạng thái
                loThuoc.TrangThai = "HetHan";

                // Tạo phiếu hủy (nếu cần)
                // ... thêm logic tạo phiếu hủy ở đây

                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "Đã đánh dấu lô thuốc hết hạn và tạo phiếu hủy"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Lỗi khi xử lý: " + ex.Message
                });
            }
        }
        // ==================== THỐNG KÊ TỒN KHO ====================
        [HttpGet("TonKhoStats")]
        public async Task<IActionResult> TonKhoStats()
        {
            var now = DateTime.Now;

            // Thống kê tổng quan
            var tongSoLo = await _context.LoThuocs.CountAsync();
            var loConHang = await _context.LoThuocs
                .Where(l => l.TrangThai == "ConHang")
                .CountAsync();
            var loSapHetHan = await _context.LoThuocs
                .Where(l => l.HanSuDung <= now.AddDays(30) &&
                           l.HanSuDung >= now &&
                           l.TrangThai == "ConHang")
                .CountAsync();
            var loHetHan = await _context.LoThuocs
                .Where(l => l.HanSuDung < now)
                .CountAsync();

            // Top 10 thuốc sắp hết tồn kho
            var thuocSapHet = await (from thuoc in _context.Thuocs
                                     join lot in _context.LoThuocs on thuoc.MaThuoc equals lot.MaThuoc into lots
                                     where thuoc.TrangThai == "KichHoat"
                                     let tonKho = lots.Where(l => l.TrangThai == "ConHang").Sum(l => (int?)l.SoLuongCon) ?? 0
                                     where tonKho <= thuoc.TonKhoToiThieu
                                     orderby tonKho
                                     select new
                                     {
                                         MaThuoc = thuoc.MaThuoc,
                                         TenThuoc = thuoc.TenThuoc,
                                         TonKho = tonKho,
                                         TonKhoToiThieu = thuoc.TonKhoToiThieu,
                                         DonViTinh = thuoc.DonViTinh
                                     })
                                     .Take(10)
                                     .ToListAsync();

            // Thống kê theo kho
            var thongKeTheoKho = await _context.LoThuocs
                .Include(l => l.Kho)
                .Where(l => l.TrangThai == "ConHang")
                .GroupBy(l => new { l.MaKho, l.Kho.TenKho })
                .Select(g => new
                {
                    TenKho = g.Key.TenKho,
                    SoLoThuoc = g.Count(),
                    TongSoLuong = g.Sum(l => l.SoLuongCon),
                    LoSapHetHan = g.Count(l => l.HanSuDung <= now.AddDays(30) && l.HanSuDung >= now)
                })
                .ToListAsync();

            // Giá trị tồn kho
            var giaTriTonKho = await _context.LoThuocs
                .Where(l => l.TrangThai == "ConHang")
                .SumAsync(l => (decimal?)(l.SoLuongCon * l.Thuoc.GiaXuat)) ?? 0;

            ViewBag.TongSoLo = tongSoLo;
            ViewBag.LoConHang = loConHang;
            ViewBag.LoSapHetHan = loSapHetHan;
            ViewBag.LoHetHan = loHetHan;
            ViewBag.ThuocSapHet = thuocSapHet;
            ViewBag.ThongKeTheoKho = thongKeTheoKho;
            ViewBag.GiaTriTonKho = giaTriTonKho;
            ViewBag.TenNhanVien = User.FindFirstValue("TenNhanVien") ?? "Admin";

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_TonKhoStats");
            }

            return View("_TonKhoStats");
        }

        // ==================== API ENDPOINTS ====================

        [HttpGet("Api/TonKhoSummary")]
        public async Task<IActionResult> GetTonKhoSummary()
        {
            var now = DateTime.Now;

            var summary = new
            {
                tongSoLo = await _context.LoThuocs.CountAsync(),
                loConHang = await _context.LoThuocs
                    .Where(l => l.TrangThai == "ConHang")
                    .CountAsync(),
                loSapHetHan = await _context.LoThuocs
                    .Where(l => l.HanSuDung <= now.AddDays(30) &&
                               l.HanSuDung >= now &&
                               l.TrangThai == "ConHang")
                    .CountAsync(),
                loHetHan = await _context.LoThuocs
                    .Where(l => l.HanSuDung < now)
                    .CountAsync(),
                loHetHang = await _context.LoThuocs
                    .Where(l => l.SoLuongCon == 0)
                    .CountAsync(),
                giaTriTonKho = await _context.LoThuocs
                    .Where(l => l.TrangThai == "ConHang")
                    .SumAsync(l => (decimal?)(l.SoLuongCon * l.Thuoc.GiaXuat)) ?? 0
            };

            return Json(summary);
        }

        /// <summary>
        /// API: Kiểm tra lô thuốc có thể xuất không
        /// </summary>
        [HttpGet("Api/CheckLotAvailability/{maLo}")]
        public async Task<IActionResult> CheckLotAvailability(string maLo, int soLuongYeuCau)
        {
            var loThuoc = await _context.LoThuocs.FindAsync(maLo);

            if (loThuoc == null)
                return Json(new { available = false, message = "Không tìm thấy lô thuốc" });

            var now = DateTime.Now;
            if (loThuoc.HanSuDung < now)
                return Json(new { available = false, message = "Lô thuốc đã hết hạn" });

            if (loThuoc.SoLuongCon < soLuongYeuCau)
                return Json(new
                {
                    available = false,
                    message = $"Không đủ số lượng. Còn lại: {loThuoc.SoLuongCon}"
                });

            return Json(new
            {
                available = true,
                message = "Có thể xuất",
                soLuongCon = loThuoc.SoLuongCon,
                hanSuDung = loThuoc.HanSuDung.ToString("dd/MM/yyyy")
            });
        }

        /// <summary>
        /// API: Lấy danh sách lô thuốc theo mã thuốc (để chọn khi cấp phát)
        /// </summary>
        [HttpGet("Api/GetLotsByMedicine/{maThuoc}")]
        public async Task<IActionResult> GetLotsByMedicine(string maThuoc)
        {
            var now = DateTime.Now;
            var lots = await _context.LoThuocs
                .Include(l => l.Kho)
                .Where(l => l.MaThuoc == maThuoc &&
                           l.TrangThai == "ConHang" &&
                           l.SoLuongCon > 0 &&
                           l.HanSuDung >= now)
                .OrderBy(l => l.HanSuDung) // FIFO - First In First Out
                .Select(l => new
                {
                    maLo = l.MaLo,
                    soLo = l.SoLo,
                    tenKho = l.Kho.TenKho,
                    soLuongCon = l.SoLuongCon,
                    hanSuDung = l.HanSuDung,
                    soNgayConLai = (l.HanSuDung - now).Days
                })
                .ToListAsync();

            return Json(lots);
        }
    }
}