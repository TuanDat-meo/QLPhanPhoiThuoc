//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using QLPhanPhoiThuoc.Models.EF;
//using QLPhanPhoiThuoc.Models.Entities;

//namespace QLPhanPhoiThuoc.Controllers.Admin.DanhMuc
//{
//    [Authorize(Roles = "Admin,DuocSi")]
//    public class ThuocController : Controller
//    {
//        private readonly BenhVienDbContext _context;

//        public ThuocController(BenhVienDbContext context)
//        {
//            _context = context;
//        }

//        // GET: /Thuoc — Danh sách thuốc + search + filter + paging
//        public async Task<IActionResult> Index(string search = "", string nhomThuoc = "", int page = 1, int pageSize = 20)
//        {
//            var query = _context.Thuocs.AsQueryable();

//            if (!string.IsNullOrEmpty(search))
//            {
//                query = query.Where(t => t.TenThuoc.Contains(search) ||
//                                         t.MaThuoc.Contains(search) ||
//                                         (t.HoatChat != null && t.HoatChat.Contains(search)) ||
//                                         (t.NhaSanXuat != null && t.NhaSanXuat.Contains(search)));
//            }

//            if (!string.IsNullOrEmpty(nhomThuoc))
//            {
//                query = query.Where(t => t.NhomThuoc == nhomThuoc);
//            }

//            var totalItems = await query.CountAsync();
//            var items = await query
//                .OrderByDescending(t => t.NgayTao)
//                .Skip((page - 1) * pageSize)
//                .Take(pageSize)
//                .ToListAsync();

//            // Dropdown nhóm thuốc
//            ViewBag.NhomThuocs = await _context.Thuocs
//                .Where(t => t.NhomThuoc != null)
//                .Select(t => t.NhomThuoc)
//                .Distinct()
//                .ToListAsync();

//            ViewBag.Search = search;
//            ViewBag.NhomThuocSelected = nhomThuoc;
//            ViewBag.CurrentPage = page;
//            ViewBag.PageSize = pageSize;
//            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
//            ViewBag.TotalItems = totalItems;

//            return View(items);
//        }

//        // GET: /Thuoc/Details/{id}
//        public async Task<IActionResult> Details(string id)
//        {
//            if (string.IsNullOrEmpty(id)) return NotFound();

//            var thuoc = await _context.Thuocs
//                .Include(t => t.LoThuocs).ThenInclude(l => l.Kho)
//                .FirstOrDefaultAsync(t => t.MaThuoc == id);

//            if (thuoc == null) return NotFound();

//            // Tính tổng tồn kho
//            ViewBag.TongTonKho = thuoc.LoThuocs
//                .Where(l => l.TrangThai == "ConHang")
//                .Sum(l => l.SoLuongCon);

//            return View(thuoc);
//        }

//        // GET: /Thuoc/Create
//        public IActionResult Create()
//        {
//            return View();
//        }

//        // POST: /Thuoc/Create
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create(Thuoc model)
//        {
//            if (!ModelState.IsValid) return View(model);

//            try
//            {
//                model.MaThuoc = GenerateId("TC");
//                model.TrangThai = "KichHoat";
//                model.NgayTao = DateTime.Now;

//                _context.Thuocs.Add(model);
//                await _context.SaveChangesAsync();

//                TempData["SuccessMessage"] = "Thêm thuốc thành công!";
//                return RedirectToAction(nameof(Details), new { id = model.MaThuoc });
//            }
//            catch (Exception ex)
//            {
//                ModelState.AddModelError("", "Đã xảy ra lỗi: " + ex.Message);
//                return View(model);
//            }
//        }

//        // GET: /Thuoc/Edit/{id}
//        public async Task<IActionResult> Edit(string id)
//        {
//            if (string.IsNullOrEmpty(id)) return NotFound();

//            var thuoc = await _context.Thuocs.FindAsync(id);
//            return thuoc == null ? NotFound() : View(thuoc);
//        }

//        // POST: /Thuoc/Edit/{id}
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(string id, Thuoc model)
//        {
//            if (id != model.MaThuoc) return NotFound();
//            if (!ModelState.IsValid) return View(model);

//            try
//            {
//                var thuoc = await _context.Thuocs.FindAsync(id);
//                if (thuoc == null) return NotFound();

//                thuoc.TenThuoc = model.TenThuoc;
//                thuoc.HoatChat = model.HoatChat;
//                thuoc.DonViTinh = model.DonViTinh;
//                thuoc.HamLuong = model.HamLuong;
//                thuoc.DangBaoChe = model.DangBaoChe;
//                thuoc.DuongDung = model.DuongDung;
//                thuoc.NhaSanXuat = model.NhaSanXuat;
//                thuoc.NhomThuoc = model.NhomThuoc;
//                thuoc.GiaNhap = model.GiaNhap;
//                thuoc.GiaXuat = model.GiaXuat;
//                thuoc.TonKhoToiThieu = model.TonKhoToiThieu;
//                thuoc.LaThuocBHYT = model.LaThuocBHYT;
//                thuoc.TyLeBHYTChiTra = model.TyLeBHYTChiTra;
//                thuoc.MoTa = model.MoTa;
//                thuoc.TrangThai = model.TrangThai;

//                await _context.SaveChangesAsync();

//                TempData["SuccessMessage"] = "Cập nhật thuốc thành công!";
//                return RedirectToAction(nameof(Details), new { id });
//            }
//            catch (Exception ex)
//            {
//                ModelState.AddModelError("", "Đã xảy ra lỗi: " + ex.Message);
//                return View(model);
//            }
//        }

//        // POST: /Thuoc/Delete/{id}
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> Delete(string id)
//        {
//            var thuoc = await _context.Thuocs.FindAsync(id);
//            if (thuoc == null) return NotFound();

//            var hasLo = await _context.LoThuocs.AnyAsync(l => l.MaThuoc == id);
//            if (hasLo)
//            {
//                TempData["ErrorMessage"] = "Không thể xóa thuốc này do còn lô thuốc liên quan.";
//                return RedirectToAction(nameof(Index));
//            }

//            _context.Thuocs.Remove(thuoc);
//            await _context.SaveChangesAsync();

//            TempData["SuccessMessage"] = "Xóa thuốc thành công!";
//            return RedirectToAction(nameof(Index));
//        }

//        // AJAX: Tìm kiếm thuốc cho autocomplete (dùng ở Kê đơn thuốc)
//        [HttpGet]
//        public async Task<IActionResult> SearchThuoc(string keyword)
//        {
//            if (string.IsNullOrEmpty(keyword)) return Json(new List<object>());

//            var thuocs = await _context.Thuocs
//                .Where(t => t.TrangThai == "KichHoat" &&
//                           (t.TenThuoc.Contains(keyword) || t.MaThuoc.Contains(keyword) ||
//                            (t.HoatChat != null && t.HoatChat.Contains(keyword))))
//                .Select(t => new
//                {
//                    maThuoc = t.MaThuoc,
//                    tenThuoc = t.TenThuoc,
//                    hoatChat = t.HoatChat,
//                    donViTinh = t.DonViTinh,
//                    giaXuat = t.GiaXuat,
//                    laThuocBHYT = t.LaThuocBHYT,
//                    tyLeBHYTChiTra = t.TyLeBHYTChiTra
//                })
//                .Take(10)
//                .ToListAsync();

//            return Json(thuocs);
//        }

//        private string GenerateId(string prefix)
//        {
//            return $"{prefix}{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";
//        }
//    }
//}
