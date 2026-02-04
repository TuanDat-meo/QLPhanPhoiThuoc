using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLPhanPhoiThuoc.Models.EF;
using QLPhanPhoiThuoc.Models.Entities;

namespace QLPhanPhoiThuoc.Controllers.Admin
{
    [Route("Admin/[controller]")]
    [Authorize(Roles = "Admin,NhanVien,BacSi,DuocSi")]
    public class BenhNhanController : Controller
    {
        private readonly BenhVienDbContext _context;

        public BenhNhanController(BenhVienDbContext context)
        {
            _context = context;
        }

        // GET: Admin/BenhNhan/_DSBenhNhan
        [HttpGet("_DSBenhNhan")]
        public async Task<IActionResult> DSBenhNhan(string search = "", int page = 1)
        {
            int pageSize = 10;
            var query = _context.BenhNhans
                .Include(bn => bn.TheBHYTs) // Include để lấy mã thẻ
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(bn => bn.TenBenhNhan.ToLower().Contains(search) ||
                                          bn.SoDienThoai.Contains(search) ||
                                          bn.TheBHYTs.Any(t => t.MaThe.Contains(search)));
            }

            query = query.OrderByDescending(bn => bn.NgayTao);

            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return PartialView("~/Views/BenhNhan/_DSBenhNhan.cshtml", items);
        }

        // GET: Admin/BenhNhan/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return PartialView("~/Views/BenhNhan/_CreateBenhNhan.cshtml");
        }

        // POST: Admin/BenhNhan/Create
        [HttpPost("Create")]
        public async Task<IActionResult> Create(BenhNhan model, string? MaTheBHYT)
        {
            try
            {
                // 1. check du lieu.
                if (await _context.BenhNhans.AnyAsync(bn => bn.SoDienThoai == model.SoDienThoai))
                    return Json(new { success = false, message = "Số điện thoại này đã tồn tại!" });

                if (!string.IsNullOrEmpty(model.CCCD) && await _context.BenhNhans.AnyAsync(bn => bn.CCCD == model.CCCD))
                    return Json(new { success = false, message = "Số CCCD này đã tồn tại!" });
                if (model.NgaySinh.HasValue && model.NgaySinh.Value.Date >= DateTime.Now.Date)
                {
                    return Json(new { success = false, message = "Ngày sinh không hợp lệ (Phải trước ngày hôm nay)!" });
                }

                model.CCCD = model.CCCD ?? "";
                model.Email = model.Email ?? "";
                model.NhomMau = model.NhomMau ?? "";
                model.NgheNghiep = model.NgheNghiep ?? "";
                model.TienSuDiUng = model.TienSuDiUng ?? "";

                model.MaBenhNhan = "BN" + DateTime.Now.ToString("yyMMdd") + new Random().Next(100, 999);
                model.NgayTao = DateTime.Now;
                model.TrangThai = "DangDieuTri";

                _context.BenhNhans.Add(model);
                await _context.SaveChangesAsync();

                // Lưu thẻ BHYT 
                if (!string.IsNullOrEmpty(MaTheBHYT))
                {
                    var theBHYT = new TheBHYT
                    {
                        MaThe = MaTheBHYT,
                        MaBenhNhan = model.MaBenhNhan,
                        NgayBatDau = DateTime.Now,
                        NgayHetHan = DateTime.Now.AddYears(1)
                    };
                    _context.TheBHYTs.Add(theBHYT);
                    await _context.SaveChangesAsync();
                }

                return Json(new { success = true, message = "Thêm hồ sơ thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // POST: Admin/BenhNhan/Edit
        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(BenhNhan model, string? MaTheBHYT)
        {
            try
            {

                if (model.NgaySinh.HasValue && model.NgaySinh.Value.Date >= DateTime.Now.Date)
                {
                    return Json(new { success = false, message = "Ngày sinh không hợp lệ (Phải trước ngày hôm nay)!" });
                }

                var existing = await _context.BenhNhans
                    .Include(b => b.TheBHYTs)
                    .FirstOrDefaultAsync(b => b.MaBenhNhan == model.MaBenhNhan);

                if (existing == null) return Json(new { success = false, message = "Không tìm thấy bệnh nhân!" });

                // Cập nhật thông tin cơ bản
                existing.TenBenhNhan = model.TenBenhNhan;
                existing.NgaySinh = model.NgaySinh;
                existing.GioiTinh = model.GioiTinh;
                existing.SoDienThoai = model.SoDienThoai;
                existing.DiaChi = model.DiaChi;

                // Cập nhật các trường mới (cho phép rỗng)
                existing.CCCD = model.CCCD ?? "";
                existing.Email = model.Email ?? "";
                existing.NhomMau = model.NhomMau ?? "";
                existing.NgheNghiep = model.NgheNghiep ?? "";
                existing.TienSuDiUng = model.TienSuDiUng ?? "";

                existing.TrangThai = model.TrangThai;

                // Xử lý thẻ BHYT
                var currentCard = existing.TheBHYTs.FirstOrDefault();
                if (!string.IsNullOrEmpty(MaTheBHYT))
                {
                    if (currentCard != null)
                    {
                        currentCard.MaThe = MaTheBHYT;
                        _context.TheBHYTs.Update(currentCard);
                    }
                    else
                    {
                        _context.TheBHYTs.Add(new TheBHYT { MaThe = MaTheBHYT, MaBenhNhan = existing.MaBenhNhan, NgayBatDau = DateTime.Now, NgayHetHan = DateTime.Now.AddYears(1) });
                    }
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Cập nhật hồ sơ thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // GET: Admin/BenhNhan/Edit/{id}
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            var bn = await _context.BenhNhans
                .Include(b => b.TheBHYTs)
                .FirstOrDefaultAsync(b => b.MaBenhNhan == id);

            if (bn == null) return NotFound();
            return PartialView("~/Views/BenhNhan/_EditBenhNhan.cshtml", bn);
        }

        // POST: Admin/BenhNhan/Delete/{id}
        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var bn = await _context.BenhNhans.FindAsync(id);
                if (bn == null) return Json(new { success = false, message = "Không tìm thấy dữ liệu" });

                // Kiểm tra ràng buộc
                bool hasHistory = await _context.HoaDons.AnyAsync(h => h.MaBenhNhan == id);

                if (hasHistory)
                {
                    bn.TrangThai = "NgungDieuTri";
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Bệnh nhân đã có lịch sử khám. Đã chuyển sang trạng thái 'Ngừng theo dõi'." });
                }
                else
                {
                    // Nếu chưa có lịch sử khám, xóa luôn cả thẻ BHYT (nếu có) để sạch data
                    var cards = _context.TheBHYTs.Where(t => t.MaBenhNhan == id);
                    _context.TheBHYTs.RemoveRange(cards);

                    _context.BenhNhans.Remove(bn);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Đã xóa hồ sơ bệnh nhân vĩnh viễn." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }
    }
}