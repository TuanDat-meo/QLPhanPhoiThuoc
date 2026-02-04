using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLPhanPhoiThuoc.Models.EF;

namespace QLPhanPhoiThuoc.Controllers.Admin
{
    [Route("Admin/[controller]")]
    [Authorize(Roles = "Admin,BacSi,DuocSi")]
    public class DonThuocController : Controller
    {
        private readonly BenhVienDbContext _context;

        public DonThuocController(BenhVienDbContext context)
        {
            _context = context;
        }

        // 1. Lấy danh sách đơn thuốc (Search + Filter trạng thái + Phân trang)
        [HttpGet("_DSDonThuoc")]
        public async Task<IActionResult> DSDonThuoc(string search = "", string status = "All", int page = 1)
        {
            int pageSize = 10;
            
            // Include các bảng liên quan để hiển thị tên
            var query = _context.DonThuocs
                .Include(d => d.BenhNhan)
                .Include(d => d.NhanVien) 
                .AsQueryable();

            // Tìm kiếm theo Mã đơn hoặc Tên bệnh nhân
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower().Trim();
                query = query.Where(d => d.MaDonThuoc.ToLower().Contains(search) || 
                                         d.BenhNhan.TenBenhNhan.ToLower().Contains(search));
            }

            // Lọc theo trạng thái
            if (status != "All" && !string.IsNullOrEmpty(status))
            {
                query = query.Where(d => d.TrangThai == status);
            }

            // Sắp xếp ngày kê đơn mới nhất lên đầu
            query = query.OrderByDescending(d => d.NgayKeDon);

            var totalItems = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.Search = search;
            ViewBag.Status = status;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return PartialView("~/Views/DonThuoc/_DSDonThuoc.cshtml", items);
        }

        // 2. Xem chi tiết đơn thuốc
        [HttpGet("Detail/{id}")]
        public async Task<IActionResult> Detail(string id)
        {
            var donThuoc = await _context.DonThuocs
                .Include(d => d.BenhNhan)
                .Include(d => d.NhanVien)
                .Include(d => d.ChiTietDonThuocs)
                .ThenInclude(ct => ct.Thuoc) // Include thuốc để lấy tên thuốc
                .FirstOrDefaultAsync(d => d.MaDonThuoc == id);

            if (donThuoc == null) return NotFound();

            return PartialView("~/Views/DonThuoc/_DetailDonThuoc.cshtml", donThuoc);
        }
    }
}