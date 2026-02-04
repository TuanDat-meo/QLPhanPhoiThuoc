using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLPhanPhoiThuoc.Models.EF;
using QLPhanPhoiThuoc.Models.Entities;
using System.Security.Claims;

namespace QLPhanPhoiThuoc.Controllers.Admin
{
    [Route("Admin/[controller]")]
    [Authorize(Roles = "Admin")] // Chỉ Admin mới có quyền truy cập [cite: 310]
    public class NhanVienController : Controller
    {
        private readonly BenhVienDbContext _context;

        public NhanVienController(BenhVienDbContext context)
        {
            _context = context;
        }

        // GET: Admin/NhanVien/_DSNhanVien
        [HttpGet("_DSNhanVien")]
        public async Task<IActionResult> DSNhanVien(string search = "", string khoaFilter = "", int page = 1)
        {
            int pageSize = 10;
            var query = _context.NhanViens
                .Include(n => n.KhoaPhong) // Join để lấy tên khoa
                .Include(n => n.TaiKhoan)  // <--- THÊM DÒNG NÀY
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(n => n.TenNhanVien.ToLower().Contains(search) ||
                                         n.MaNhanVien.ToLower().Contains(search) ||
                                         n.SoDienThoai.Contains(search));
            }

            if (!string.IsNullOrEmpty(khoaFilter))
            {
                query = query.Where(n => n.MaKhoa == khoaFilter);
            }

            var totalItems = await query.CountAsync();
            var items = await query
                .OrderBy(n => n.TenNhanVien)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.KhoaFilter = khoaFilter;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // Lấy danh sách khoa cho dropdown lọc
            ViewBag.DanhSachKhoa = await _context.KhoaPhongs.ToListAsync();

            return PartialView("~/Views/NhanVien/_DSNhanVien.cshtml", items);
        }

        // GET: Admin/NhanVien/Details/{id}
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            var nhanVien = await _context.NhanViens
                .Include(n => n.KhoaPhong)
                .Include(n => n.TaiKhoan)
                .FirstOrDefaultAsync(n => n.MaNhanVien == id);

            if (nhanVien == null) return NotFound();

            return PartialView("~/Views/NhanVien/_ChiTietNhanVien.cshtml", nhanVien);
        }


        // GET: Admin/NhanVien/Create
        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            ViewBag.DanhSachKhoa = await _context.KhoaPhongs.ToListAsync();
            return PartialView("~/Views/NhanVien/_CreateNhanVien.cshtml");
        }

        // POST: Admin/NhanVien/Create
        [HttpPost("Create")]
        public async Task<IActionResult> Create(NhanVien model, IFormFile ImageFile, string TenDangNhap, string MatKhau)
        {
            try
            {
                // 1. Kiểm tra trùng mã nhân viên
                if (await _context.NhanViens.AnyAsync(n => n.MaNhanVien == model.MaNhanVien))
                    return Json(new { success = false, message = "Mã nhân viên đã tồn tại!" });

                if (string.IsNullOrEmpty(model.CCCD))
                {
                    return Json(new { success = false, message = "Vui lòng nhập số CCCD/CMND!" });
                }

                // check trùng CCCD
                if (await _context.NhanViens.AnyAsync(n => n.CCCD == model.CCCD))
                {
                    return Json(new { success = false, message = "Số CCCD này đã tồn tại trong hệ thống!" });
                }

                model.BangCap = model.BangCap ?? "";
                model.DiaChi = model.DiaChi ?? "";
                model.ChuyenKhoa = model.ChuyenKhoa ?? "";
                // Nếu MaKhoa bắt buộc, kiểm tra:
                if (string.IsNullOrEmpty(model.MaKhoa))
                    return Json(new { success = false, message = "Vui lòng chọn Khoa/Phòng!" });

                string avatarPath = null;

                // 2. Upload ảnh
                if (ImageFile != null)
                {
                    string folder = "wwwroot/uploads/nhanvien/";
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), folder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }
                    avatarPath = "/uploads/nhanvien/" + fileName;
                }

                // 3. Tạo tài khoản (nếu có nhập)
                if (!string.IsNullOrEmpty(TenDangNhap))
                {
                    var taiKhoan = new TaiKhoan
                    {
                       MaTaiKhoan = DateTime.Now.ToString("yyMMddHHmmss") + new Random().Next(1000, 9999), // Kết quả: 2402051030551234 (16 ký tự)
                        Username = TenDangNhap,
                        PasswordHash = MatKhau, // Nhớ hash MD5/BCrypt
                        Role = "NhanVien",
                        TrangThai = "KichHoat",
                        Avatar = avatarPath
                        
                    };
                    _context.TaiKhoans.Add(taiKhoan);
                    await _context.SaveChangesAsync();

                    model.MaTaiKhoan = taiKhoan.MaTaiKhoan;
                }
                else if (avatarPath != null)
                {
                    // Nếu không tạo TK, lưu ảnh vào NhanVien (nếu bảng có cột HinhAnh)
                    // model.HinhAnh = avatarPath; 
                    // Nếu bảng NhanVien không có HinhAnh thì ảnh này sẽ mất vì không có chỗ lưu.
                }

                model.TrangThai = "DangLamViec";
                model.NgayTao = DateTime.Now;

                _context.NhanViens.Add(model);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Thêm nhân viên thành công!" });
            }
            catch (DbUpdateException dbEx)
            {
                var msg = dbEx.InnerException?.Message ?? dbEx.Message;
                return Json(new { success = false, message = "Lỗi CSDL: " + msg });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        // GET: Admin/NhanVien/Edit/{id}
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            var nhanVien = await _context.NhanViens
                .Include(n => n.TaiKhoan)
                .FirstOrDefaultAsync(n => n.MaNhanVien == id);

            if (nhanVien == null) return NotFound();

            ViewBag.DanhSachKhoa = await _context.KhoaPhongs.ToListAsync();
            return PartialView("~/Views/NhanVien/_EditNhanVien.cshtml", nhanVien);
        }

        // POST: Admin/NhanVien/Edit
        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(NhanVien model, IFormFile? ImageFile, string? MatKhauMoi)
        {
            try
            {
                var existing = await _context.NhanViens
                    .Include(n => n.TaiKhoan)
                    .FirstOrDefaultAsync(n => n.MaNhanVien == model.MaNhanVien);

                if (existing == null) return Json(new { success = false, message = "Không tìm thấy nhân viên" });

                // --- CẬP NHẬT THÔNG TIN CƠ BẢN ---
                existing.TenNhanVien = model.TenNhanVien;
                existing.MaKhoa = model.MaKhoa;
                existing.ChucVu = model.ChucVu;
                existing.SoDienThoai = model.SoDienThoai;
                existing.Email = model.Email;
                existing.TrangThai = model.TrangThai;

                // --- CẬP NHẬT CÁC TRƯỜNG MỚI ---
                existing.CCCD = model.CCCD;
                existing.NgaySinh = model.NgaySinh;
                existing.GioiTinh = model.GioiTinh;
                existing.DiaChi = model.DiaChi;
                existing.BangCap = model.BangCap;
                existing.ChuyenKhoa = model.ChuyenKhoa;

                // Xử lý ảnh mới
                if (ImageFile != null)
                {
                    string folder = "wwwroot/uploads/nhanvien/";
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), folder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    // Lưu vào NhanVien (nếu bạn đã thêm cột HinhAnh)
                    existing.TaiKhoan.Avatar = "/uploads/nhanvien/" + fileName;

                    // Nếu có tài khoản thì cập nhật luôn Avatar của tài khoản
                    if (existing.TaiKhoan != null)
                    {
                        existing.TaiKhoan.Avatar = "/uploads/nhanvien/" + fileName;
                    }
                }

                // Cập nhật mật khẩu
                if (!string.IsNullOrEmpty(MatKhauMoi) && existing.TaiKhoan != null)
                {
                    existing.TaiKhoan.PasswordHash = MatKhauMoi; // Nên Hash
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Cập nhật thông tin thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }



        // ====== Phân quyền =========

        // GET: Admin/NhanVien/PhanQuyen
        [HttpGet("PhanQuyen")]
        public async Task<IActionResult> PhanQuyen(string search = "")
        {
            var query = _context.NhanViens
                .Include(n => n.TaiKhoan)
                .Include(n => n.KhoaPhong)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(n => n.TenNhanVien.ToLower().Contains(search) ||
                                         n.MaNhanVien.ToLower().Contains(search));
            }

            var list = await query.OrderBy(n => n.TenNhanVien).ToListAsync();
            ViewBag.Search = search;

            // Nếu là Admin đang đăng nhập, lấy ID để tránh tự hạ quyền chính mình
            ViewBag.CurrentUserId = User.FindFirstValue("MaNhanVien");

            return PartialView("~/Views/NhanVien/_PhanQuyen.cshtml", list);
        }

        // POST: Admin/NhanVien/UpdateRole
        [HttpPost("UpdateRole")]
        public async Task<IActionResult> UpdateRole(string maNhanVien, string newRole, bool isLocked)
        {
            try
            {
                // Chặn không cho tự đổi quyền của chính mình
                var currentUserId = User.FindFirstValue("MaNhanVien");
                if (maNhanVien == currentUserId)
                {
                    return Json(new { success = false, message = "Bạn không thể tự thay đổi quyền hạn của chính mình!" });
                }

                var nhanVien = await _context.NhanViens
                    .Include(n => n.TaiKhoan)
                    .FirstOrDefaultAsync(n => n.MaNhanVien == maNhanVien);

                if (nhanVien == null || nhanVien.TaiKhoan == null)
                {
                    return Json(new { success = false, message = "Nhân viên chưa có tài khoản để phân quyền!" });
                }

                // Cập nhật Role và Trạng thái khóa
                nhanVien.TaiKhoan.Role = newRole;
                nhanVien.TaiKhoan.TrangThai = isLocked ? "Khoa" : "KichHoat";

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = $"Cập nhật quyền cho {nhanVien.TenNhanVien} thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

    }
}