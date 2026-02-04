using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLPhanPhoiThuoc.Models.EF;
using QLPhanPhoiThuoc.Models;
using QLPhanPhoiThuoc.Models.Entities;

namespace QLPhanPhoiThuoc.Controllers.Admin
{
    [Route("Admin/[controller]")]
    [Area("Admin")]
    public class KhoController : Controller
    {
        private readonly BenhVienDbContext _context;

        public KhoController(BenhVienDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Kho/Index
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var khos = await _context.Kho
                    .OrderByDescending(k => k.NgayTao)
                    .ToListAsync();

                // Đảm bảo khos không bao giờ null
                if (khos == null)
                {
                    khos = new List<Kho>();
                }

                // THÊM: Nếu gọi qua AJAX thì trả về PartialView với đường dẫn rõ ràng
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView("~/Views/Kho/Index.cshtml", khos);
                }

                return View("~/ViewsKho/Index.cshtml", khos);
            }
            catch (Exception ex)
            {
                // Log lỗi và trả về danh sách rỗng
                Console.WriteLine($"Error in Kho/Index: {ex.Message}");

                var emptyList = new List<Kho>();

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView("~/Views/Kho/Index.cshtml", emptyList);
                }

                return View("~/Views/Kho/Index.cshtml", emptyList);
            }
        }

        // GET: Admin/Kho/Details/5
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var kho = await _context.Kho
                .Include(k => k.LoThuocs)
                    .ThenInclude(l => l.Thuoc)
                .FirstOrDefaultAsync(k => k.MaKho == id);

            if (kho == null)
            {
                return NotFound();
            }

            // THÊM: Trả về PartialView cho Panel chi tiết
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("~/Views/Kho/Details.cshtml", kho);
            }

            return View("~/Views/Kho/Details.cshtml", kho);
        }

        // GET: Admin/Kho/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("~/Views/Kho/Create.cshtml");
            }
            return View("~/Views/Kho/Create.cshtml");
        }

        // POST: Admin/Kho/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] Kho kho)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (string.IsNullOrEmpty(kho.MaKho))
                    {
                        kho.MaKho = "KHO" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    }

                    kho.NgayTao = DateTime.Now;
                    kho.TrangThai = "HoatDong";

                    _context.Add(kho);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, message = "Tạo kho thành công!" });
                }

                return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // GET: Admin/Kho/Edit/5
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var kho = await _context.Kho.FindAsync(id);
            if (kho == null)
            {
                return NotFound();
            }

            // THÊM: Trả về PartialView cho Panel chỉnh sửa
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("~/Views/Kho/Edit.cshtml", kho);
            }

            return View("~/Views/Kho/Edit.cshtml", kho);
        }

        // POST: Admin/Kho/Edit/5
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [FromBody] Kho kho)
        {
            if (id != kho.MaKho)
            {
                return Json(new { success = false, message = "Mã kho không khớp!" });
            }

            try
            {
                if (ModelState.IsValid)
                {
                    _context.Update(kho);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, message = "Cập nhật kho thành công!" });
                }

                return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KhoExists(kho.MaKho))
                {
                    return Json(new { success = false, message = "Kho không tồn tại!" });
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // POST: Admin/Kho/Delete/5
        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var kho = await _context.Kho.FindAsync(id);
                if (kho == null)
                {
                    return Json(new { success = false, message = "Kho không tồn tại!" });
                }

                var hasLots = await _context.LoThuoc.AnyAsync(l => l.MaKho == id);
                if (hasLots)
                {
                    return Json(new { success = false, message = "Không thể xóa kho đang có hàng tồn kho!" });
                }

                _context.Kho.Remove(kho);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Xóa kho thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // API Methods (Giữ nguyên vì đã trả về Json)
        [HttpGet("GetTonKho/{maKho}")]
        public async Task<IActionResult> GetTonKho(string maKho)
        {
            var tonKho = await _context.LoThuoc
                .Where(l => l.MaKho == maKho && l.SoLuongCon > 0)
                .Include(l => l.Thuoc)
                .Select(l => new
                {
                    maThuoc = l.MaThuoc,
                    tenThuoc = l.Thuoc != null ? l.Thuoc.TenThuoc : "",
                    soLo = l.SoLo,
                    soLuongCon = l.SoLuongCon,
                    hanSuDung = l.HanSuDung,
                    trangThai = l.TrangThai
                })
                .ToListAsync();

            return Json(tonKho);
        }

        [HttpGet("GetStatistics/{maKho}")]
        public async Task<IActionResult> GetStatistics(string maKho)
        {
            var stats = new
            {
                tongThuoc = await _context.LoThuoc
                    .Where(l => l.MaKho == maKho)
                    .Select(l => l.MaThuoc)
                    .Distinct()
                    .CountAsync(),

                tongSoLuong = await _context.LoThuoc
                    .Where(l => l.MaKho == maKho)
                    .SumAsync(l => (int?)l.SoLuongCon) ?? 0,

                sapHetHan = await _context.LoThuoc
                    .Where(l => l.MaKho == maKho &&
                           l.HanSuDung <= DateTime.Now.AddMonths(3) &&
                           l.HanSuDung > DateTime.Now)
                    .CountAsync(),

                hetHan = await _context.LoThuoc
                    .Where(l => l.MaKho == maKho && l.HanSuDung <= DateTime.Now)
                    .CountAsync()
            };

            return Json(stats);
        }

        private bool KhoExists(string id)
        {
            return _context.Kho.Any(e => e.MaKho == id);
        }
    }
}