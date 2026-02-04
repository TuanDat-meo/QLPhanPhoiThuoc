using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLPhanPhoiThuoc.Models.EF;

namespace QLPhanPhoiThuoc.Controllers.Admin
{
    [Route("Admin/[controller]")]
    [Authorize(Roles = "Admin,NhanVien,BacSi,DuocSi")]
    public class TheBHYTController : Controller
    {
        private readonly BenhVienDbContext _context;

        public TheBHYTController(BenhVienDbContext context)
        {
            _context = context;
        }

        [HttpGet("_DSTheBHYT")]
        public async Task<IActionResult> DSTheBHYT(string search = "", int page = 1)
        {
            int pageSize = 10;

            // Join với bảng BenhNhan để lấy tên, sđt, cccd
            var query = _context.TheBHYTs
                .Include(t => t.BenhNhan)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim().ToLower();
                // TÌM KIẾM ĐA NĂNG: Số thẻ, Tên BN, CCCD, SĐT
                query = query.Where(t => t.SoTheBHYT.Contains(search) ||
                                         t.BenhNhan.TenBenhNhan.ToLower().Contains(search) ||
                                         t.BenhNhan.CCCD.Contains(search) ||
                                         t.BenhNhan.SoDienThoai.Contains(search));
            }

            query = query.OrderByDescending(t => t.NgayBatDau);

            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return PartialView("~/Views/TheBHYT/_DSTheBHYT.cshtml", items);
        }

        // GET: Admin/TheBHYT/Detail/{id}
        [HttpGet("Detail/{id}")]
        public async Task<IActionResult> Detail(string id)
        {
            var theBHYT = await _context.TheBHYTs
                .Include(t => t.BenhNhan)
                // SỬA DÒNG NÀY: Dùng m.MaThe cho khớp với Model
                .FirstOrDefaultAsync(m => m.MaThe == id);

            if (theBHYT == null) return NotFound();

            return PartialView("~/Views/TheBHYT/_DetailTheBHYT.cshtml", theBHYT);
        }
    }
}