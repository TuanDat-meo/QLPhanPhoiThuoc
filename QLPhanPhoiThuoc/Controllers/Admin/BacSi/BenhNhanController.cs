//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using QLPhanPhoiThuoc.Models.EF;
//using QLPhanPhoiThuoc.Models.Entities;
//using QLPhanPhoiThuoc.Models.ViewModels;
//using System.Security.Claims;

//namespace QLPhanPhoiThuoc.Controllers.Admin.BacSi
//{
//    [Area("BacSi")]
//    [Authorize(Roles = "Admin,BacSi,ThuNgan")]
//    public class BenhNhanController : Controller
//    {
//        private readonly BenhVienDbContext _benhVienContext;
//        private readonly VNeIDDbContext _vneIdContext;

//        public BenhNhanController(BenhVienDbContext benhVienContext, VNeIDDbContext vneIdContext)
//        {
//            _benhVienContext = benhVienContext;
//            _vneIdContext = vneIdContext;
//        }

//        // GET: /BacSi/BenhNhan
//        public async Task<IActionResult> Index(string search = "", int page = 1, int pageSize = 20)
//        {
//            var query = _benhVienContext.BenhNhans.AsQueryable();

//            // Search
//            if (!string.IsNullOrEmpty(search))
//            {
//                query = query.Where(b => b.TenBenhNhan.Contains(search) ||
//                                        b.MaBenhNhan.Contains(search) ||
//                                        b.CCCD.Contains(search) ||
//                                        b.SoDienThoai.Contains(search));
//            }

//            // Total count
//            var totalItems = await query.CountAsync();

//            // Get items
//            var benhNhans = await query
//                .OrderByDescending(b => b.NgayTao)
//                .Skip((page - 1) * pageSize)
//                .Take(pageSize)
//                .ToListAsync();

//            ViewBag.Search = search;
//            ViewBag.CurrentPage = page;
//            ViewBag.PageSize = pageSize;
//            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
//            ViewBag.TotalItems = totalItems;

//            return View(benhNhans);
//        }

//        // GET: /BacSi/BenhNhan/Details/5
//        public async Task<IActionResult> Details(string id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var benhNhan = await _benhVienContext.BenhNhans
//                .Include(b => b.TheBHYTs)
//                .FirstOrDefaultAsync(m => m.MaBenhNhan == id);

//            if (benhNhan == null)
//            {
//                return NotFound();
//            }

//            return View(benhNhan);
//        }

//        // GET: /BacSi/BenhNhan/TiepDon
//        [Authorize(Roles = "BacSi")]
//        public IActionResult TiepDon()
//        {
//            return View();
//        }

//        // POST: /BacSi/BenhNhan/TiepDon
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        [Authorize(Roles = "BacSi")]
//        public async Task<IActionResult> TiepDon(BenhNhanViewModel model)
//        {
//            if (!ModelState.IsValid)
//            {
//                return View(model);
//            }

//            try
//            {
//                // Check if CCCD already exists
//                var existingBN = await _benhVienContext.BenhNhans
//                    .FirstOrDefaultAsync(b => b.CCCD == model.CCCD);

//                if (existingBN != null)
//                {
//                    ModelState.AddModelError("CCCD", "Bệnh nhân đã tồn tại trong hệ thống");
//                    return View(model);
//                }

//                // Generate MaBenhNhan
//                var maBenhNhan = GenerateId("BN");

//                var benhNhan = new BenhNhan
//                {
//                    MaBenhNhan = maBenhNhan,
//                    TenBenhNhan = model.TenBenhNhan,
//                    NgaySinh = model.NgaySinh,
//                    GioiTinh = model.GioiTinh,
//                    CCCD = model.CCCD,
//                    DiaChi = model.DiaChi,
//                    SoDienThoai = model.SoDienThoai,
//                    Email = model.Email,
//                    NhomMau = model.NhomMau,
//                    NgheNghiep = model.NgheNghiep,
//                    CanNang = model.CanNang,
//                    ChieuCao = model.ChieuCao,
//                    TienSuDiUng = model.TienSuDiUng,
//                    TienSuBenhLy = model.TienSuBenhLy,
//                    GhiChu = model.GhiChu,
//                    NgayTao = DateTime.Now,
//                    NgayCapNhat = DateTime.Now
//                };

//                _benhVienContext.BenhNhans.Add(benhNhan);
//                await _benhVienContext.SaveChangesAsync();

//                TempData["SuccessMessage"] = "Tiếp đón bệnh nhân thành công!";
//                return RedirectToAction(nameof(Details), new { id = maBenhNhan });
//            }
//            catch (Exception ex)
//            {
//                ModelState.AddModelError("", "Đã xảy ra lỗi: " + ex.Message);
//                return View(model);
//            }
//        }

//        // GET: /BacSi/BenhNhan/Edit/5
//        [Authorize(Roles = "BacSi")]
//        public async Task<IActionResult> Edit(string id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var benhNhan = await _benhVienContext.BenhNhans.FindAsync(id);
//            if (benhNhan == null)
//            {
//                return NotFound();
//            }

//            var model = new BenhNhanViewModel
//            {
//                MaBenhNhan = benhNhan.MaBenhNhan,
//                TenBenhNhan = benhNhan.TenBenhNhan,
//                NgaySinh = benhNhan.NgaySinh,
//                GioiTinh = benhNhan.GioiTinh,
//                CCCD = benhNhan.CCCD,
//                DiaChi = benhNhan.DiaChi,
//                SoDienThoai = benhNhan.SoDienThoai,
//                Email = benhNhan.Email,
//                NhomMau = benhNhan.NhomMau,
//                NgheNghiep = benhNhan.NgheNghiep,
//                CanNang = benhNhan.CanNang,
//                ChieuCao = benhNhan.ChieuCao,
//                TienSuDiUng = benhNhan.TienSuDiUng,
//                TienSuBenhLy = benhNhan.TienSuBenhLy,
//                GhiChu = benhNhan.GhiChu
//            };

//            return View(model);
//        }

//        // POST: /BacSi/BenhNhan/Edit/5
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        [Authorize(Roles = "BacSi")]
//        public async Task<IActionResult> Edit(string id, BenhNhanViewModel model)
//        {
//            if (id != model.MaBenhNhan)
//            {
//                return NotFound();
//            }

//            if (!ModelState.IsValid)
//            {
//                return View(model);
//            }

//            try
//            {
//                var benhNhan = await _benhVienContext.BenhNhans.FindAsync(id);
//                if (benhNhan == null)
//                {
//                    return NotFound();
//                }

//                benhNhan.TenBenhNhan = model.TenBenhNhan;
//                benhNhan.NgaySinh = model.NgaySinh;
//                benhNhan.GioiTinh = model.GioiTinh;
//                benhNhan.DiaChi = model.DiaChi;
//                benhNhan.SoDienThoai = model.SoDienThoai;
//                benhNhan.Email = model.Email;
//                benhNhan.NhomMau = model.NhomMau;
//                benhNhan.NgheNghiep = model.NgheNghiep;
//                benhNhan.CanNang = model.CanNang;
//                benhNhan.ChieuCao = model.ChieuCao;
//                benhNhan.TienSuDiUng = model.TienSuDiUng;
//                benhNhan.TienSuBenhLy = model.TienSuBenhLy;
//                benhNhan.GhiChu = model.GhiChu;
//                benhNhan.NgayCapNhat = DateTime.Now;

//                _benhVienContext.Update(benhNhan);
//                await _benhVienContext.SaveChangesAsync();

//                TempData["SuccessMessage"] = "Cập nhật thông tin bệnh nhân thành công!";
//                return RedirectToAction(nameof(Details), new { id });
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                if (!BenhNhanExists(model.MaBenhNhan))
//                {
//                    return NotFound();
//                }
//                else
//                {
//                    throw;
//                }
//            }
//            catch (Exception ex)
//            {
//                ModelState.AddModelError("", "Đã xảy ra lỗi: " + ex.Message);
//                return View(model);
//            }
//        }

//        // POST: /BacSi/BenhNhan/SaveTheBHYT
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        [Authorize(Roles = "BacSi")]
//        public async Task<IActionResult> SaveTheBHYT(TheBHYTViewModel model)
//        {
//            if (!ModelState.IsValid)
//            {
//                return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
//            }

//            try
//            {
//                // Check if the BHYT card already exists
//                var existingCard = await _benhVienContext.TheBHYTs
//                    .FirstOrDefaultAsync(t => t.SoThe == model.SoThe);

//                if (existingCard != null)
//                {
//                    // Update existing card
//                    existingCard.MaBenhNhan = model.MaBenhNhan;
//                    existingCard.NgayBatDau = model.NgayBatDau;
//                    existingCard.NgayKetThuc = model.NgayKetThuc;
//                    existingCard.NoiDKKCB = model.NoiDKKCB;
//                    existingCard.MucHuong = model.MucHuong;
//                    existingCard.TrangThai = model.NgayKetThuc >= DateTime.Now ? "ConHan" : "HetHan";
//                    existingCard.NgayCapNhat = DateTime.Now;

//                    _benhVienContext.Update(existingCard);
//                }
//                else
//                {
//                    // Create new card
//                    var theBHYT = new TheBHYT
//                    {
//                        MaThe = GenerateId("BHYT"),
//                        MaBenhNhan = model.MaBenhNhan,
//                        SoThe = model.SoThe,
//                        NgayBatDau = model.NgayBatDau,
//                        NgayKetThuc = model.NgayKetThuc,
//                        NoiDKKCB = model.NoiDKKCB,
//                        MucHuong = model.MucHuong,
//                        TrangThai = model.NgayKetThuc >= DateTime.Now ? "ConHan" : "HetHan",
//                        NgayTao = DateTime.Now,
//                        NgayCapNhat = DateTime.Now
//                    };

//                    _benhVienContext.TheBHYTs.Add(theBHYT);
//                }

//                await _benhVienContext.SaveChangesAsync();

//                return Json(new { success = true, message = "Lưu thẻ BHYT thành công" });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message });
//            }
//        }

//        // GET: /BacSi/BenhNhan/LichSuKham/BN001
//        public async Task<IActionResult> LichSuKham(string maBenhNhan)
//        {
//            if (string.IsNullOrEmpty(maBenhNhan))
//            {
//                return NotFound();
//            }

//            var benhNhan = await _benhVienContext.BenhNhans
//                .FirstOrDefaultAsync(b => b.MaBenhNhan == maBenhNhan);

//            if (benhNhan == null)
//            {
//                return NotFound();
//            }

//            // Get lich su kham benh
//            var lichSuKham = await _benhVienContext.ChanDoans
//                .Where(c => c.MaBenhNhan == maBenhNhan)
//                .OrderByDescending(c => c.NgayChanDoan)
//                .Select(c => new
//                {
//                    c.MaChanDoan,
//                    c.NgayChanDoan,
//                    c.TenBenh,
//                    c.MoTa,
//                    c.HuongDieuTri,
//                    BacSi = c.NhanVien != null ? c.NhanVien.TenNhanVien : ""
//                })
//                .ToListAsync();

//            ViewBag.BenhNhan = benhNhan;
//            ViewBag.LichSuKham = lichSuKham;

//            return View();
//        }

//        // GET: /BacSi/BenhNhan/LichSuDonThuoc/BN001
//        public async Task<IActionResult> LichSuDonThuoc(string maBenhNhan)
//        {
//            if (string.IsNullOrEmpty(maBenhNhan))
//            {
//                return NotFound();
//            }

//            var benhNhan = await _benhVienContext.BenhNhans
//                .FirstOrDefaultAsync(b => b.MaBenhNhan == maBenhNhan);

//            if (benhNhan == null)
//            {
//                return NotFound();
//            }

//            // Get lich su don thuoc
//            var lichSuDon = await _benhVienContext.DonThuocs
//                .Where(d => d.MaBenhNhan == maBenhNhan)
//                .OrderByDescending(d => d.NgayKeDon)
//                .Select(d => new
//                {
//                    d.MaDonThuoc,
//                    d.NgayKeDon,
//                    d.LoaiDon,
//                    d.ChanDoanSoBo,
//                    d.TongTien,
//                    d.TrangThai,
//                    BacSi = d.NhanVien != null ? d.NhanVien.TenNhanVien : ""
//                })
//                .ToListAsync();

//            ViewBag.BenhNhan = benhNhan;
//            ViewBag.LichSuDon = lichSuDon;

//            return View();
//        }

//        // AJAX: Search benh nhan (for autocomplete)
//        [HttpGet]
//        public async Task<IActionResult> Search(string keyword)
//        {
//            if (string.IsNullOrEmpty(keyword))
//            {
//                return Json(new List<object>());
//            }

//            var benhNhans = await _benhVienContext.BenhNhans
//                .Where(b => b.TenBenhNhan.Contains(keyword) ||
//                           b.MaBenhNhan.Contains(keyword) ||
//                           b.CCCD.Contains(keyword) ||
//                           b.SoDienThoai.Contains(keyword))
//                .Select(b => new
//                {
//                    maBenhNhan = b.MaBenhNhan,
//                    tenBenhNhan = b.TenBenhNhan,
//                    ngaySinh = b.NgaySinh,
//                    gioiTinh = b.GioiTinh,
//                    cccd = b.CCCD,
//                    soDienThoai = b.SoDienThoai,
//                    diaChi = b.DiaChi,
//                    tienSuDiUng = b.TienSuDiUng
//                })
//                .Take(10)
//                .ToListAsync();

//            return Json(benhNhans);
//        }

//        // AJAX: Get benh nhan details
//        [HttpGet]
//        public async Task<IActionResult> GetBenhNhanDetails(string maBenhNhan)
//        {
//            var benhNhan = await _benhVienContext.BenhNhans
//                .Where(b => b.MaBenhNhan == maBenhNhan)
//                .Select(b => new
//                {
//                    maBenhNhan = b.MaBenhNhan,
//                    tenBenhNhan = b.TenBenhNhan,
//                    ngaySinh = b.NgaySinh,
//                    gioiTinh = b.GioiTinh,
//                    cccd = b.CCCD,
//                    soDienThoai = b.SoDienThoai,
//                    email = b.Email,
//                    diaChi = b.DiaChi,
//                    nhomMau = b.NhomMau,
//                    ngheNghiep = b.NgheNghiep,
//                    canNang = b.CanNang,
//                    chieuCao = b.ChieuCao,
//                    tienSuDiUng = b.TienSuDiUng,
//                    tienSuBenhLy = b.TienSuBenhLy
//                })
//                .FirstOrDefaultAsync();

//            if (benhNhan == null)
//            {
//                return Json(new { success = false, message = "Không tìm thấy bệnh nhân" });
//            }

//            // Get BHYT info if exists
//            var theBHYT = await _benhVienContext.TheBHYTs
//                .Where(t => t.MaBenhNhan == maBenhNhan && t.TrangThai == "ConHan")
//                .OrderByDescending(t => t.NgayKetThuc)
//                .FirstOrDefaultAsync();

//            return Json(new
//            {
//                success = true,
//                data = new
//                {
//                    benhNhan,
//                    theBHYT = theBHYT != null ? new
//                    {
//                        soThe = theBHYT.SoThe,
//                        ngayBatDau = theBHYT.NgayBatDau,
//                        ngayKetThuc = theBHYT.NgayKetThuc,
//                        mucHuong = theBHYT.MucHuong,
//                        noiDKKCB = theBHYT.NoiDKKCB
//                    } : null
//                }
//            });
//        }

//        private bool BenhNhanExists(string id)
//        {
//            return _benhVienContext.BenhNhans.Any(e => e.MaBenhNhan == id);
//        }

//        private string GenerateId(string prefix)
//        {
//            return $"{prefix}{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";
//        }
//    }
//}