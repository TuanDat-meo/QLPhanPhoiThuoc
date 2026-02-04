using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLPhanPhoiThuoc.Models.EF;       // Chứa BenhVienDbContext
using QLPhanPhoiThuoc.Models.Entities; // Chứa class BenhNhan

namespace QLPhanPhoiThuoc.Controllers.Admin
{
    // Định nghĩa URL gốc: localhost:port/Admin/BenhNhan
    [Route("Admin/BenhNhan")]
    public class BenhNhanController : Controller
    {
        private readonly BenhVienDbContext _context;

        public BenhNhanController(BenhVienDbContext context)
        {
            _context = context;
        }

        // Action trả về PARTIAL VIEW (không có layout)
        [Route("_DSBenhNhan")]
        public async Task<IActionResult> DSBenhNhan(string searchString)
        {
            if (_context == null || _context.BenhNhans == null)
            {
                return Problem("Chưa kết nối được Database hoặc bảng BenhNhan bị null.");
            }

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
            var model = await query.ToListAsync();

            // Trả về PARTIAL VIEW (không có layout, chỉ có nội dung)
            return PartialView("~/Views/Admin/BenhNhan/_DSBenhNhan.cshtml", model);
        }
    }
}