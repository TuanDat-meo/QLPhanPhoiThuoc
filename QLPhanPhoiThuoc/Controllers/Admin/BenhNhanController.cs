using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using QLPhanPhoiThuoc.Models.EF;
using QLPhanPhoiThuoc.Models.Entities;
using System.Drawing;
using System.Text.Json;

namespace QLPhanPhoiThuoc.Controllers.Admin
{
    [Route("Admin/BenhNhan")]
    public class BenhNhanController : Controller
    {
        private readonly BenhVienDbContext _context;

        public BenhNhanController(BenhVienDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("ExportExcel")]
        public async Task<IActionResult> ExportExcel()
        {
            var data = await _context.BenhNhans.OrderByDescending(b => b.NgayTao).ToListAsync();

            ExcelPackage.License.SetNonCommercialPersonal("Duy Binh");
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Danh Sách Bệnh Nhân");

                // 1. Định dạng tiêu đề lớn
                worksheet.Cells["A1:G1"].Merge = true;
                worksheet.Cells["A1"].Value = "DANH SÁCH BỆNH NHÂN HỆ THỐNG";
                worksheet.Cells["A1"].Style.Font.Size = 16;
                worksheet.Cells["A1"].Style.Font.Bold = true;
                worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // 2. Định dạng Header bảng
                string[] headers = { "Mã BN", "Họ và Tên", "Ngày sinh", "Giới tính", "SĐT", "Email", "Trạng thái" };
                for (int i = 0; i < headers.Length; i++)
                {
                    var cell = worksheet.Cells[3, i + 1];
                    cell.Value = headers[i];
                    cell.Style.Font.Bold = true;
                    cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(30, 58, 95)); // Màu giống giao diện [cite: 375]
                    cell.Style.Font.Color.SetColor(Color.White);
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                // 3. Đổ dữ liệu vào bảng
                int row = 4;
                foreach (var item in data)
                {
                    worksheet.Cells[row, 1].Value = item.MaBenhNhan;
                    worksheet.Cells[row, 2].Value = item.TenBenhNhan;
                    worksheet.Cells[row, 3].Value = item.NgaySinh?.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 4].Value = item.GioiTinh;
                    worksheet.Cells[row, 5].Value = item.SoDienThoai;
                    worksheet.Cells[row, 6].Value = item.Email;
                    worksheet.Cells[row, 7].Value = item.TrangThai;
                    row++;
                }

                // 4. Tự động chỉnh độ rộng cột và kẻ bảng
                worksheet.Cells[3, 1, row - 1, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[3, 1, row - 1, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[3, 1, row - 1, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[3, 1, row - 1, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells.AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string fileName = $"DanhSachBenhNhan_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }

        // Danh sách bệnh nhân (đã có)
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

            // Phân trang thủ công
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

            return PartialView("~/Views/Admin/BenhNhan/_DSBenhNhan.cshtml", items);
        }

        // Xem chi tiết bệnh nhân
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

            return PartialView("~/Views/Admin/BenhNhan/_ChiTietBenhNhan.cshtml", benhNhan);
        }

        // Lấy form sửa bệnh nhân
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

            return PartialView("~/Views/Admin/BenhNhan/_SuaBenhNhan.cshtml", benhNhan);
        }

        // Cập nhật thông tin bệnh nhân
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

        // Xóa bệnh nhân
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
    }
}