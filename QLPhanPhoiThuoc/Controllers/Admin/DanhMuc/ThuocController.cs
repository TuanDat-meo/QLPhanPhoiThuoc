using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLPhanPhoiThuoc.Models.EF;
using QLPhanPhoiThuoc.Models.Entities;
using System.Security.Claims;

namespace QLPhanPhoiThuoc.Controllers.Admin
{
    [Route("Admin/[controller]")]
    [Authorize(Roles = "Admin,NhanVien")]
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

            ViewBag.TenNhanVien = User.FindFirstValue("TenNhanVien") ?? "Admin";
            return View(thuoc);
        }

        // GET: Admin/Thuoc/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            ViewBag.TenNhanVien = User.FindFirstValue("TenNhanVien") ?? "Admin";
            return View();
        }

        // POST: Admin/Thuoc/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenThuoc,HoatChat,NhomThuoc,DonViTinh,DuongDung,HamLuong,QuyCachDongGoi,GiaNhap,GiaXuat,TonKhoToiThieu,NhaSanXuat,NuocSanXuat,SoDangKy,GhiChu,TrangThai")] Thuoc thuoc)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    thuoc.MaThuoc = GenerateId("THU");

                    var ngayTaoProperty = typeof(Thuoc).GetProperty("NgayTao");
                    if (ngayTaoProperty != null && ngayTaoProperty.CanWrite)
                    {
                        ngayTaoProperty.SetValue(thuoc, DateTime.Now);
                    }

                    if (string.IsNullOrEmpty(thuoc.TrangThai))
                        thuoc.TrangThai = "KichHoat";

                    _context.Add(thuoc);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Thêm thuốc thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi thêm thuốc: " + ex.Message);
                }
            }

            ViewBag.TenNhanVien = User.FindFirstValue("TenNhanVien") ?? "Admin";
            return View(thuoc);
        }

        // GET: Admin/Thuoc/Edit/{id}
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var thuoc = await _context.Thuocs.FindAsync(id);
            if (thuoc == null) return NotFound();

            ViewBag.TenNhanVien = User.FindFirstValue("TenNhanVien") ?? "Admin";
            return View(thuoc);
        }

        // POST: Admin/Thuoc/Edit/{id}
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaThuoc,TenThuoc,HoatChat,NhomThuoc,DonViTinh,DuongDung,HamLuong,QuyCachDongGoi,GiaNhap,GiaXuat,TonKhoToiThieu,NhaSanXuat,NuocSanXuat,SoDangKy,GhiChu,TrangThai")] Thuoc thuoc)
        {
            if (id != thuoc.MaThuoc) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingThuoc = await _context.Thuocs.AsNoTracking().FirstOrDefaultAsync(t => t.MaThuoc == id);
                    if (existingThuoc == null) return NotFound();

                    var ngayCapNhatProperty = typeof(Thuoc).GetProperty("NgayCapNhat");
                    if (ngayCapNhatProperty != null && ngayCapNhatProperty.CanWrite)
                    {
                        ngayCapNhatProperty.SetValue(thuoc, DateTime.Now);
                    }

                    var ngayTaoProperty = typeof(Thuoc).GetProperty("NgayTao");
                    if (ngayTaoProperty != null && ngayTaoProperty.CanWrite && ngayTaoProperty.CanRead)
                    {
                        var ngayTao = ngayTaoProperty.GetValue(existingThuoc);
                        ngayTaoProperty.SetValue(thuoc, ngayTao);
                    }

                    _context.Update(thuoc);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Cập nhật thuốc thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ThuocExists(thuoc.MaThuoc))
                        return NotFound();
                    else
                        throw;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi cập nhật thuốc: " + ex.Message);
                }
            }

            ViewBag.TenNhanVien = User.FindFirstValue("TenNhanVien") ?? "Admin";
            return View(thuoc);
        }

        // POST: Admin/Thuoc/Delete/{id}
        [HttpPost("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var thuoc = await _context.Thuocs.FindAsync(id);
            if (thuoc == null)
                return NotFound();

            try
            {
                thuoc.TrangThai = "NgungSuDung";
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Xóa thuốc thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi xóa thuốc: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // ==================== TỒN KHO ====================

        // GET: Admin/Thuoc/TonKho
        [HttpGet("TonKho")]
        public async Task<IActionResult> TonKho(string search = "", string filter = "all", string sortBy = "TenThuoc",
                                                string sortOrder = "asc", int page = 1, int pageSize = 20)
        {
            // Query lấy danh sách thuốc kèm tồn kho
            var query = from thuoc in _context.Thuocs
                        join lot in _context.LoThuocs on thuoc.MaThuoc equals lot.MaThuoc into lots
                        where thuoc.TrangThai == "KichHoat"
                        let tonKho = lots.Where(l => l.TrangThai == "ConHang").Sum(l => (int?)l.SoLuongCon) ?? 0
                        select new
                        {
                            Thuoc = thuoc,
                            TonKho = tonKho
                        };

            // Search
            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim();
                query = query.Where(t => t.Thuoc.TenThuoc.Contains(search) ||
                                        t.Thuoc.MaThuoc.Contains(search) ||
                                        (t.Thuoc.NhomThuoc != null && t.Thuoc.NhomThuoc.Contains(search)));
            }

            // Filter by stock status
            query = filter switch
            {
                "low" => query.Where(t => t.TonKho <= t.Thuoc.TonKhoToiThieu),
                "out" => query.Where(t => t.TonKho == 0),
                "normal" => query.Where(t => t.TonKho > t.Thuoc.TonKhoToiThieu),
                _ => query
            };

            // Sorting
            query = sortBy switch
            {
                "TenThuoc" => sortOrder == "asc" ? query.OrderBy(t => t.Thuoc.TenThuoc) : query.OrderByDescending(t => t.Thuoc.TenThuoc),
                "TonKho" => sortOrder == "asc" ? query.OrderBy(t => t.TonKho) : query.OrderByDescending(t => t.TonKho),
                "NhomThuoc" => sortOrder == "asc" ? query.OrderBy(t => t.Thuoc.NhomThuoc) : query.OrderByDescending(t => t.Thuoc.NhomThuoc),
                _ => sortOrder == "asc" ? query.OrderBy(t => t.Thuoc.TenThuoc) : query.OrderByDescending(t => t.Thuoc.TenThuoc)
            };

            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.Filter = filter;
            ViewBag.SortBy = sortBy;
            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.TotalItems = totalItems;
            ViewBag.TenNhanVien = User.FindFirstValue("TenNhanVien") ?? "Admin";

            // Nếu là AJAX request, trả về PartialView
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_TonKho", items);
            }

            return View(items);
        }

        // ==================== QUẢN LÝ LÔ THUỐC ====================

        // GET: Admin/Thuoc/LoThuoc
        [HttpGet("LoThuoc")]
        public async Task<IActionResult> LoThuoc(string search = "", string filter = "all", int page = 1, int pageSize = 20)
        {
            var query = _context.LoThuocs
                .Include(l => l.Thuoc)
                .Include(l => l.Kho)
                .AsQueryable();

            // Search
            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim();
                query = query.Where(l => l.SoLo.Contains(search) ||
                                        l.Thuoc.TenThuoc.Contains(search) ||
                                        l.MaLo.Contains(search));
            }

            // Filter
            var now = DateTime.Now;
            query = filter switch
            {
                "expiring" => query.Where(l => l.HanSuDung <= now.AddDays(30) && l.HanSuDung >= now && l.TrangThai == "ConHang"),
                "expired" => query.Where(l => l.HanSuDung < now),
                "active" => query.Where(l => l.TrangThai == "ConHang"),
                _ => query
            };

            var totalItems = await query.CountAsync();
            var items = await query
                .OrderBy(l => l.HanSuDung)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.Filter = filter;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.TotalItems = totalItems;
            ViewBag.TenNhanVien = User.FindFirstValue("TenNhanVien") ?? "Admin";

            return View(items);
        }

        // ==================== CẢNH BÁO ====================

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
        }

        private string GenerateId(string prefix)
        {
            return $"{prefix}{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";
        }
    }
}