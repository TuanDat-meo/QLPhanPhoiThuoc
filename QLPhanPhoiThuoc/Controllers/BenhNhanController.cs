// ==================== CẬP NHẬT BENHNHANCONTROLLER.CS ====================
// File: Controllers/BenhNhanController.cs (KHÔNG PHẢI Controllers/Admin/)

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLPhanPhoiThuoc.Models.EF;
using QLPhanPhoiThuoc.Models.Entities;

namespace QLPhanPhoiThuoc.Controllers
{
    // ========== QUAN TRỌNG: Route phải là "BenhNhan" chứ KHÔNG PHẢI "Admin/BenhNhan" ==========
    [Route("BenhNhan")]
    public class BenhNhanController : Controller
    {
        private readonly BenhVienDbContext _context;

        public BenhNhanController(BenhVienDbContext context)
        {
            _context = context;
        }

        // ==================== CÁC ACTION METHODS ====================

        // GET: /BenhNhan/_DSBenhNhan - Danh sách bệnh nhân
        [HttpGet]
        [Route("_DSBenhNhan")]
        public async Task<IActionResult> DSBenhNhan(string searchString, int page = 1)
        {
            if (_context == null || _context.BenhNhans == null)
            {
                return Problem("Chưa kết nối được Database hoặc bảng BenhNhan bị null.");
            }

            ViewBag.CurrentFilter = searchString;

            var query = from b in _context.BenhNhans select b;

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(s =>
                    s.TenBenhNhan != null && s.TenBenhNhan.Contains(searchString) ||
                    s.CCCD != null && s.CCCD.Contains(searchString) ||
                    s.SoDienThoai != null && s.SoDienThoai.Contains(searchString)
                );
            }

            query = query.OrderByDescending(b => b.NgayTao);

            // Phân trang
            int pageSize = 8;
            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.PageNumber = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = totalPages;

            return PartialView("~/Views/BenhNhan/_DSBenhNhan.cshtml", items);
        }

        // GET: /BenhNhan/_BenhNhan - Trang quản lý bệnh nhân (có tabs)
        [HttpGet]
        [Route("_BenhNhan")]
        public IActionResult BenhNhan()
        {
            return PartialView("~/Views/BenhNhan/_BenhNhan.cshtml");
        }

        // GET: /BenhNhan/ChiTiet/{id} - Xem chi tiết bệnh nhân
        [HttpGet]
        [Route("ChiTiet/{id}")]
        public async Task<IActionResult> ChiTiet(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Mã bệnh nhân không hợp lệ");
            }

            var benhNhan = await _context.BenhNhans
                .FirstOrDefaultAsync(b => b.MaBenhNhan == id);

            if (benhNhan == null)
            {
                return NotFound("Không tìm thấy bệnh nhân");
            }

            return PartialView("~/Views/BenhNhan/_ChiTietBenhNhan.cshtml", benhNhan);
        }

        // GET: /BenhNhan/LayFormSua/{id} - Lấy form sửa bệnh nhân
        [HttpGet]
        [Route("LayFormSua/{id}")]
        public async Task<IActionResult> LayFormSua(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Mã bệnh nhân không hợp lệ");
            }

            var benhNhan = await _context.BenhNhans
                .FirstOrDefaultAsync(b => b.MaBenhNhan == id);

            if (benhNhan == null)
            {
                return NotFound("Không tìm thấy bệnh nhân");
            }

            return PartialView("~/Views/BenhNhan/_SuaBenhNhan.cshtml", benhNhan);
        }

        // POST: /BenhNhan/SuaBenhNhan - Cập nhật thông tin bệnh nhân
        [HttpPost]
        [Route("SuaBenhNhan")]
        public async Task<IActionResult> SuaBenhNhan([FromBody] BenhNhan model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.MaBenhNhan))
                {
                    return Json(new { success = false, message = "Mã bệnh nhân không hợp lệ" });
                }

                var benhNhan = await _context.BenhNhans
                    .FirstOrDefaultAsync(b => b.MaBenhNhan == model.MaBenhNhan);

                if (benhNhan == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy bệnh nhân" });
                }

                // Cập nhật thông tin
                benhNhan.TenBenhNhan = model.TenBenhNhan;
                benhNhan.CCCD = model.CCCD;
                benhNhan.SoDienThoai = model.SoDienThoai;
                benhNhan.Email = model.Email;
                benhNhan.NgaySinh = model.NgaySinh;
                benhNhan.GioiTinh = model.GioiTinh;
                benhNhan.DiaChi = model.DiaChi;
                benhNhan.TrangThai = model.TrangThai;

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Cập nhật thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // POST: /BenhNhan/XoaBenhNhan/{id} - Xóa bệnh nhân
        [HttpPost]
        [Route("XoaBenhNhan/{id}")]
        public async Task<IActionResult> XoaBenhNhan(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return Json(new { success = false, message = "Mã bệnh nhân không hợp lệ" });
                }

                var benhNhan = await _context.BenhNhans
                    .FirstOrDefaultAsync(b => b.MaBenhNhan == id);

                if (benhNhan == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy bệnh nhân" });
                }

                _context.BenhNhans.Remove(benhNhan);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Xóa bệnh nhân thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // GET: /BenhNhan/ExportExcel - Export Excel
        [HttpGet]
        [Route("ExportExcel")]
        public async Task<IActionResult> ExportExcel()
        {
            // Giữ nguyên code export excel từ file cũ
            // ...
            return File(new MemoryStream(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DanhSachBenhNhan.xlsx");
        }

        // GET: /BenhNhan/_BenhAnBenhNhan - Hồ sơ bệnh án
        [HttpGet]
        [Route("_BenhAnBenhNhan")]
        public IActionResult BenhAnBenhNhan()
        {
            return PartialView("~/Views/BenhNhan/_BenhAnBenhNhan.cshtml");
        }

        // GET: /BenhNhan/_ThemBenhNhan - Form thêm bệnh nhân
        [HttpGet]
        [Route("_ThemBenhNhan")]
        public IActionResult ThemBenhNhan()
        {
            return PartialView("~/Views/BenhNhan/_ThemBenhNhan.cshtml");
        }
    }
}

// ==================== KIỂM TRA CẤU TRÚC THƯ MỤC ====================
/*
Solution/
├── Controllers/
│   └── BenhNhanController.cs ✅ (Đặt ở đây, KHÔNG PHẢI trong Admin/)
│
├── Views/
│   └── BenhNhan/          ✅ (Views/BenhNhan/ như bạn đã nói)
│       ├── _BenhNhan.cshtml
│       ├── _DSBenhNhan.cshtml
│       ├── _ChiTietBenhNhan.cshtml
│       ├── _SuaBenhNhan.cshtml
│       ├── _ThemBenhNhan.cshtml
│       └── _BenhAnBenhNhan.cshtml
*/