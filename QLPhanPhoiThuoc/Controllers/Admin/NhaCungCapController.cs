using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QLPhanPhoiThuoc.Models.EF;
using QLPhanPhoiThuoc.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLPhanPhoiThuoc.Controllers.Admin
{
    [Route("Admin/[controller]")]
    [Authorize(Roles = "Admin,NhanVien")]
    public class NhaCungCapController : Controller
    {
        private readonly BenhVienDbContext _context;
        private readonly ILogger<NhaCungCapController> _logger;

        public NhaCungCapController(BenhVienDbContext context, ILogger<NhaCungCapController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Admin/NhaCungCap/Index
        [HttpGet]
        [Route("Index")]
        public async Task<IActionResult> Index(string searchString, string trangThai)
        {
            try
            {
                _logger.LogInformation("🔵 NhaCungCap/Index - Start");

                ViewData["CurrentFilter"] = searchString;
                ViewData["TrangThaiFilter"] = trangThai;

                var nhaCungCaps = from n in _context.NhaCungCaps
                                  select n;

                if (!string.IsNullOrEmpty(searchString))
                {
                    _logger.LogInformation($"Searching with: {searchString}");
                    nhaCungCaps = nhaCungCaps.Where(n =>
                        n.TenNCC.Contains(searchString) ||
                        n.MaNCC.Contains(searchString) ||
                        (n.MaSoThue != null && n.MaSoThue.Contains(searchString))
                    );
                }

                if (!string.IsNullOrEmpty(trangThai))
                {
                    _logger.LogInformation($"Filtering by status: {trangThai}");
                    nhaCungCaps = nhaCungCaps.Where(n => n.TrangThai == trangThai);
                }

                nhaCungCaps = nhaCungCaps.OrderByDescending(n => n.NgayTao);
                var data = await nhaCungCaps.ToListAsync();

                _logger.LogInformation($"✅ Found {data.Count} suppliers");

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView("~/Views/NhaCungCap/Index.cshtml", data);
                }
                return View("~/Views/NhaCungCap/Index.cshtml", data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ ERROR in NhaCungCap/Index");

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
                }

                TempData["ErrorMessage"] = $"Có lỗi xảy ra: {ex.Message}";
                return View("~/Views/NhaCungCap/Index.cshtml", new List<NhaCungCap>());
            }
        }

        // GET: Admin/NhaCungCap/Details/5
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            try
            {
                if (id == null)
                {
                    _logger.LogWarning("Details called with null id");
                    return NotFound();
                }

                _logger.LogInformation($"🔵 Loading details for: {id}");

                var nhaCungCap = await _context.NhaCungCaps
                    .Include(n => n.PhieuNhaps)
                        .ThenInclude(p => p.ChiTietPhieuNhaps)
                    .FirstOrDefaultAsync(m => m.MaNCC == id);

                if (nhaCungCap == null)
                {
                    _logger.LogWarning($"Supplier not found: {id}");
                    return NotFound();
                }

                var tongPhieuNhap = nhaCungCap.PhieuNhaps?.Count ?? 0;
                var tongGiaTriNhap = nhaCungCap.PhieuNhaps?.Sum(p => p.TongTien) ?? 0;
                var phieuNhapGanDay = nhaCungCap.PhieuNhaps?
                    .OrderByDescending(p => p.NgayNhap)
                    .Take(5)
                    .ToList();

                ViewData["TongPhieuNhap"] = tongPhieuNhap;
                ViewData["TongGiaTriNhap"] = tongGiaTriNhap;
                ViewData["PhieuNhapGanDay"] = phieuNhapGanDay;

                _logger.LogInformation($"✅ Details loaded: {tongPhieuNhap} phiếu nhập");

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView("~/Views/NhaCungCap/Details.cshtml", nhaCungCap);
                }
                return View("~/Views/NhaCungCap/Details.cshtml", nhaCungCap);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ ERROR in Details for {id}");

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = ex.Message });
                }

                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Admin/NhaCungCap/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            try
            {
                _logger.LogInformation("🔵 Opening Create form");

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView("~/Views/NhaCungCap/Create.cshtml");
                }
                return View("~/Views/NhaCungCap/Create.cshtml");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ ERROR in Create GET");
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Admin/NhaCungCap/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("MaNCC,TenNCC,DiaChi,SoDienThoai,Email,MaSoThue,TrangThai")] NhaCungCap nhaCungCap)
        {
            try
            {
                _logger.LogInformation($"🔵 Creating supplier: {nhaCungCap?.MaNCC}");

                bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
                _logger.LogInformation($"🔍 Is AJAX Request: {isAjax}");

                // CRITICAL: Remove PhieuNhaps từ ModelState vì không cần validate
                ModelState.Remove("PhieuNhaps");

                if (ModelState.IsValid)
                {
                    // Check duplicate MaNCC
                    var exists = await _context.NhaCungCaps.AnyAsync(n => n.MaNCC == nhaCungCap.MaNCC);
                    if (exists)
                    {
                        _logger.LogWarning($"Duplicate MaNCC: {nhaCungCap.MaNCC}");
                        ModelState.AddModelError("MaNCC", "Mã nhà cung cấp đã tồn tại!");

                        if (isAjax)
                        {
                            Response.StatusCode = 400;
                            return PartialView("~/Views/NhaCungCap/Create.cshtml", nhaCungCap);
                        }
                        return View("~/Views/NhaCungCap/Create.cshtml", nhaCungCap);
                    }

                    // Check duplicate MaSoThue
                    if (!string.IsNullOrEmpty(nhaCungCap.MaSoThue))
                    {
                        var mstExists = await _context.NhaCungCaps.AnyAsync(n => n.MaSoThue == nhaCungCap.MaSoThue);
                        if (mstExists)
                        {
                            _logger.LogWarning($"Duplicate MST: {nhaCungCap.MaSoThue}");
                            ModelState.AddModelError("MaSoThue", "Mã số thuế đã được sử dụng!");

                            if (isAjax)
                            {
                                Response.StatusCode = 400;
                                return PartialView("~/Views/NhaCungCap/Create.cshtml", nhaCungCap);
                            }
                            return View("~/Views/NhaCungCap/Create.cshtml", nhaCungCap);
                        }
                    }

                    // Set defaults
                    nhaCungCap.NgayTao = DateTime.Now;
                    nhaCungCap.PhieuNhaps = null; // CRITICAL: Set to null explicitly

                    _context.Add(nhaCungCap);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"✅ Created supplier: {nhaCungCap.MaNCC}");

                    if (isAjax)
                    {
                        return Json(new
                        {
                            success = true,
                            message = "Thêm nhà cung cấp thành công!",
                            data = nhaCungCap.MaNCC
                        });
                    }

                    TempData["SuccessMessage"] = "Thêm nhà cung cấp thành công!";
                    return RedirectToAction(nameof(Index));
                }

                // Log validation errors
                _logger.LogWarning("⚠️ ModelState invalid");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning($"Validation Error: {error.ErrorMessage}");
                }

                if (isAjax)
                {
                    Response.StatusCode = 400;
                    return PartialView("~/Views/NhaCungCap/Create.cshtml", nhaCungCap);
                }
                return View("~/Views/NhaCungCap/Create.cshtml", nhaCungCap);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ ERROR in Create POST");

                bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
                if (isAjax)
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Lỗi: {ex.Message}"
                    });
                }

                ModelState.AddModelError("", $"Lỗi: {ex.Message}");
                return View("~/Views/NhaCungCap/Create.cshtml", nhaCungCap);
            }
        }

        // GET: Admin/NhaCungCap/Edit/5
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                if (id == null)
                {
                    _logger.LogWarning("Edit called with null id");
                    return NotFound();
                }

                _logger.LogInformation($"🔵 Opening Edit for: {id}");

                var nhaCungCap = await _context.NhaCungCaps.FindAsync(id);
                if (nhaCungCap == null)
                {
                    _logger.LogWarning($"Supplier not found: {id}");
                    return NotFound();
                }

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView("~/Views/NhaCungCap/Edit.cshtml", nhaCungCap);
                }
                return View("~/Views/NhaCungCap/Edit.cshtml", nhaCungCap);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ ERROR in Edit GET for {id}");
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Admin/NhaCungCap/Edit/5
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaNCC,TenNCC,DiaChi,SoDienThoai,Email,MaSoThue,TrangThai,GhiChu,NgayTao")] NhaCungCap nhaCungCap)
        {
            try
            {
                if (id != nhaCungCap.MaNCC)
                {
                    _logger.LogWarning($"ID mismatch: {id} vs {nhaCungCap.MaNCC}");
                    return NotFound();
                }

                _logger.LogInformation($"🔵 Updating supplier: {id}");
                bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

                if (ModelState.IsValid)
                {
                    // Check duplicate MaSoThue
                    if (!string.IsNullOrEmpty(nhaCungCap.MaSoThue))
                    {
                        var mstExists = await _context.NhaCungCaps
                            .AnyAsync(n => n.MaSoThue == nhaCungCap.MaSoThue && n.MaNCC != nhaCungCap.MaNCC);
                        if (mstExists)
                        {
                            _logger.LogWarning($"Duplicate MST: {nhaCungCap.MaSoThue}");
                            ModelState.AddModelError("MaSoThue", "Mã số thuế đã tồn tại!");

                            if (isAjax)
                            {
                                Response.StatusCode = 400;
                                return PartialView("~/Views/NhaCungCap/Edit.cshtml", nhaCungCap);
                            }
                            return View("~/Views/NhaCungCap/Edit.cshtml", nhaCungCap);
                        }
                    }

                    _context.Update(nhaCungCap);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"✅ Updated supplier: {id}");

                    if (isAjax)
                    {
                        return Json(new
                        {
                            success = true,
                            message = "Cập nhật thành công!"
                        });
                    }

                    TempData["SuccessMessage"] = "Cập nhật thành công!";
                    return RedirectToAction(nameof(Index));
                }

                _logger.LogWarning("ModelState invalid");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning($"Validation Error: {error.ErrorMessage}");
                }

                if (isAjax)
                {
                    Response.StatusCode = 400;
                    return PartialView("~/Views/NhaCungCap/Edit.cshtml", nhaCungCap);
                }
                return View("~/Views/NhaCungCap/Edit.cshtml", nhaCungCap);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"❌ Concurrency error for {id}");

                if (!NhaCungCapExists(nhaCungCap.MaNCC))
                {
                    return NotFound();
                }

                bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
                if (isAjax)
                {
                    return Json(new { success = false, message = "Lỗi đồng thời: Dữ liệu đã được cập nhật bởi người khác." });
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ ERROR in Edit POST for {id}");
                ModelState.AddModelError("", $"Lỗi: {ex.Message}");

                bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
                if (isAjax)
                {
                    return Json(new { success = false, message = ex.Message });
                }
                return View("~/Views/NhaCungCap/Edit.cshtml", nhaCungCap);
            }
        }

        // GET: Admin/NhaCungCap/Delete/5
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (id == null)
                {
                    _logger.LogWarning("Delete called with null id");
                    return NotFound();
                }

                _logger.LogInformation($"🔵 Opening Delete for: {id}");

                var nhaCungCap = await _context.NhaCungCaps
                    .Include(n => n.PhieuNhaps)
                    .FirstOrDefaultAsync(m => m.MaNCC == id);

                if (nhaCungCap == null)
                {
                    _logger.LogWarning($"Supplier not found: {id}");
                    return NotFound();
                }

                ViewData["CoPhieuNhap"] = nhaCungCap.PhieuNhaps?.Any() ?? false;

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView("~/Views/NhaCungCap/Delete.cshtml", nhaCungCap);
                }
                return View("~/Views/NhaCungCap/Delete.cshtml", nhaCungCap);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ ERROR in Delete GET for {id}");
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost("Delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                _logger.LogInformation($"🔵 Deleting supplier: {id}");

                var nhaCungCap = await _context.NhaCungCaps
                    .Include(n => n.PhieuNhaps)
                    .FirstOrDefaultAsync(n => n.MaNCC == id);

                if (nhaCungCap != null)
                {
                    if (nhaCungCap.PhieuNhaps?.Any() ?? false)
                    {
                        _logger.LogWarning($"Cannot delete {id} - has PhieuNhap");

                        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        {
                            return Json(new { success = false, message = "Không thể xóa NCC đã có phiếu nhập!" });
                        }

                        TempData["ErrorMessage"] = "Không thể xóa NCC đã có phiếu nhập!";
                        return RedirectToAction(nameof(Delete), new { id });
                    }

                    _context.NhaCungCaps.Remove(nhaCungCap);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"✅ Deleted supplier: {id}");
                }

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, message = "Đã xóa nhà cung cấp thành công!" });
                }

                TempData["SuccessMessage"] = "Đã xóa nhà cung cấp thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ ERROR in Delete POST for {id}");

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = ex.Message });
                }

                TempData["ErrorMessage"] = $"Lỗi khi xóa: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost("ChangeStatus")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(string id, string status)
        {
            try
            {
                _logger.LogInformation($"🔵 Changing status for {id} to {status}");

                var ncc = await _context.NhaCungCaps.FindAsync(id);
                if (ncc == null)
                {
                    _logger.LogWarning($"Supplier not found: {id}");
                    return Json(new { success = false, message = "Không tìm thấy nhà cung cấp" });
                }

                ncc.TrangThai = status;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"✅ Status changed for {id}");

                return Json(new { success = true, message = "Đã cập nhật trạng thái!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ ERROR in ChangeStatus for {id}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        private bool NhaCungCapExists(string id)
        {
            return _context.NhaCungCaps.Any(e => e.MaNCC == id);
        }
    }
}