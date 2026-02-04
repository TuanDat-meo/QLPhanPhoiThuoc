using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using QLPhanPhoiThuoc.Models.EF;
using QLPhanPhoiThuoc.Models.Entities;
using System.Drawing;
using System.Security.Claims;

namespace QLPhanPhoiThuoc.Controllers.Admin
{
    [Route("Admin/[controller]")]
    [Authorize(Roles = "Admin,NhanVien")] // [Pattern 1: Giống ThuocController]
    public class HoaDonController : Controller
    {
        private readonly BenhVienDbContext _context;

        public HoaDonController(BenhVienDbContext context)
        {
            _context = context;
        }

        // ==================== QUẢN LÝ HÓA ĐƠN ====================

        // GET: Admin/HoaDon/_DSHoaDon (Dùng cho cả Search, Filter, Paging)
        [HttpGet("_DSHoaDon")]
        public async Task<IActionResult> DSHoaDon(string searchString = "", string statusFilter = "", int page = 1, int pageSize = 5)
        {
            // [Pattern 2: Queryable & Trim search giống ThuocController]
            var query = _context.HoaDons
                .Include(h => h.BenhNhan) // Include để lấy tên hiển thị
                .AsQueryable();

            // Search filter
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim();
                query = query.Where(h => h.MaHoaDon.Contains(searchString) ||
                                         (h.BenhNhan != null && h.BenhNhan.TenBenhNhan.Contains(searchString)) ||
                                         (h.BenhNhan != null && h.BenhNhan.SoDienThoai.Contains(searchString)));
            }

            // Status filter
            if (!string.IsNullOrEmpty(statusFilter))
            {
                query = query.Where(h => h.TrangThaiThanhToan == statusFilter);
            }

            // Sorting (Mặc định mới nhất lên đầu)
            query = query.OrderByDescending(h => h.NgayTao);

            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // [Pattern 3: ViewBag đầy đủ giống ThuocController]
            ViewBag.CurrentFilter = searchString;
            ViewBag.StatusFilter = statusFilter;
            ViewBag.PageNumber = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // Thông tin người dùng
            ViewBag.TenNhanVien = User.FindFirstValue("TenNhanVien") ?? "Admin";

            // [Quant trọng]: Trỏ đúng về thư mục Views/HoaDon/
            return PartialView("~/Views/HoaDon/_DSHoaDon.cshtml", items);
        }

        [HttpGet("_HoaDonChoThanhToan")]
        public async Task<IActionResult> HoaDonChoThanhToan(string searchString, int page = 1)
        {
            return await DSHoaDon(searchString, "ChuaTra", page);
        }

        [HttpGet("_HoaDonDaThanhToan")]
        public async Task<IActionResult> HoaDonDaThanhToan(string searchString, int page = 1)
        {
            return await DSHoaDon(searchString, "DaTra", page);
        }

        // ==================== CHI TIẾT & THANH TOÁN ====================

        // GET: Admin/HoaDon/ChiTiet/{id}
        [HttpGet("ChiTiet/{id}")]
        public async Task<IActionResult> ChiTiet(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest("Mã hóa đơn không hợp lệ");

            // [Pattern 4: Include đầy đủ dữ liệu liên quan]
            var hoaDon = await _context.HoaDons
                .Include(h => h.BenhNhan)
                .Include(h => h.DonThuoc)
                    .ThenInclude(dt => dt.ChiTietDonThuocs)
                        .ThenInclude(ct => ct.Thuoc) // Lấy tên thuốc, giá thuốc
                .Include(h => h.DonThuoc)
                    .ThenInclude(dt => dt.NhanVien) // Lấy tên bác sĩ
                .FirstOrDefaultAsync(h => h.MaHoaDon == id);

            if (hoaDon == null) return NotFound();

            ViewBag.TenNhanVien = User.FindFirstValue("TenNhanVien") ?? "Admin";

            // Trả về View chi tiết (Dùng cho Modal)
            return PartialView("~/Views/HoaDon/_ChiTietHoaDon.cshtml", hoaDon);
        }

        // POST: Admin/HoaDon/ThanhToan/{id}
        [HttpPost("ThanhToan/{id}")]
        public async Task<IActionResult> ThanhToan(string id)
        {
            try
            {
                // check hoa don null
                if (string.IsNullOrEmpty(id))
                {
                    return Json(new { success = false, message = "Mã hóa đơn không hợp lệ (Null/Empty)" });
                }

                // tim` hd theo id
                var hoaDon = await _context.HoaDons.FirstOrDefaultAsync(h => h.MaHoaDon == id);

                if (hoaDon == null)
                {
                    return Json(new { success = false, message = $"Không tìm thấy hóa đơn có mã: {id}" });
                }

                // 3. Kiểm tra logic nghiệp vụ
                // Nếu đã thanh toán rồi thì chặn lại để tránh spam request hoặc lỗi user
                if (hoaDon.TrangThaiThanhToan == "DaTra" || hoaDon.TrangThaiThanhToan == "HoanTat")
                {
                    return Json(new { success = false, message = "Hóa đơn này đã hoàn tất thanh toán trước đó!" });
                }

                // 4. Thực hiện cập nhật trạng thái
                hoaDon.TrangThaiThanhToan = "DaTra"; // Cập nhật trạng thái
                hoaDon.TienDaTra = hoaDon.TienBenhNhanCanTra; // Gán tiền đã trả = Số tiền phải trả

                hoaDon.NgayTao = DateTime.Now; // Ngày thanh toán

                // 5. Lưu xuống Database
                _context.HoaDons.Update(hoaDon);
                await _context.SaveChangesAsync();

                // 6. Trả về thành công
                return Json(new { success = true, message = "Thanh toán thành công!" });
            }
            catch (Exception ex)
            {
                // Log lỗi ra console để debug nếu cần
                Console.WriteLine($"Error Payment: {ex.Message}");
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        // GET: Admin/HoaDon/InHoaDon/{id}
        [HttpGet("InHoaDon/{id}")]
        public async Task<IActionResult> InHoaDon(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            // Lấy dữ liệu đầy đủ để in
            var hoaDon = await _context.HoaDons
                .Include(h => h.BenhNhan)
                .Include(h => h.DonThuoc)
                    .ThenInclude(dt => dt.ChiTietDonThuocs)
                        .ThenInclude(ct => ct.Thuoc)
                .Include(h => h.DonThuoc)
                    .ThenInclude(dt => dt.NhanVien) // Bác sĩ kê đơn
                .FirstOrDefaultAsync(h => h.MaHoaDon == id);

            if (hoaDon == null) return NotFound();

            // Trả về View riêng để in (Không dùng Layout chung của Admin)
            return View("~/Views/HoaDon/InHoaDon.cshtml", hoaDon);
        }

        // GET: Admin/HoaDon/ExportExcel
        [HttpGet("ExportExcel")]
        public async Task<IActionResult> ExportExcel(string searchString = "", string statusFilter = "")
        {
            // 1. Lấy dữ liệu theo bộ lọc hiện tại (để báo cáo đúng những gì đang nhìn thấy)
            var query = _context.HoaDons
                .Include(h => h.BenhNhan)
                .Include(h => h.DonThuoc) // Lấy thêm thông tin đơn thuốc để biết bác sĩ kê
                    .ThenInclude(dt => dt.NhanVien)
                .AsQueryable();

            // Áp dụng lại logic lọc giống hệt DSHoaDon
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim();
                query = query.Where(h => h.MaHoaDon.Contains(searchString) ||
                                         (h.BenhNhan != null && h.BenhNhan.TenBenhNhan.Contains(searchString)));
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                query = query.Where(h => h.TrangThaiThanhToan == statusFilter);
            }

            var data = await query.OrderByDescending(h => h.NgayTao).ToListAsync();

            // 2. Khởi tạo Excel Package
            ExcelPackage.License.SetNonCommercialPersonal("Duy Binh");
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Danh Sách Hóa Đơn");

                // --- A. Tiêu đề lớn ---
                worksheet.Cells["A1:H1"].Merge = true;
                worksheet.Cells["A1"].Value = "BÁO CÁO DOANH THU HÓA ĐƠN";
                worksheet.Cells["A1"].Style.Font.Size = 16;
                worksheet.Cells["A1"].Style.Font.Bold = true;
                worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Row(1).Height = 30;

                // --- B. Header Bảng ---
                string[] headers = {
            "STT", "Mã Hóa Đơn", "Bệnh Nhân", "Ngày Lập",
            "Tổng Tiền", "BHYT Chi Trả", "Khách Cần Trả", "Trạng Thái"
        };

                for (int i = 0; i < headers.Length; i++)
                {
                    var cell = worksheet.Cells[3, i + 1];
                    cell.Value = headers[i];
                    cell.Style.Font.Bold = true;
                    cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cell.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#1e3a5f")); // Màu xanh thương hiệu
                    cell.Style.Font.Color.SetColor(Color.White);
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                // --- C. Đổ dữ liệu ---
                int row = 4;
                int stt = 1;
                foreach (var item in data)
                {
                    worksheet.Cells[row, 1].Value = stt;
                    worksheet.Cells[row, 2].Value = item.MaHoaDon;
                    worksheet.Cells[row, 3].Value = item.BenhNhan?.TenBenhNhan ?? "Khách lẻ";

                    // Format ngày tháng
                    worksheet.Cells[row, 4].Value = item.NgayTao;
                    worksheet.Cells[row, 4].Style.Numberformat.Format = "dd/mm/yyyy hh:mm";

                    // Format tiền tệ
                    worksheet.Cells[row, 5].Value = item.TongTien;
                    worksheet.Cells[row, 5].Style.Numberformat.Format = "#,##0";

                    worksheet.Cells[row, 6].Value = item.TienBHYTChiTra;
                    worksheet.Cells[row, 6].Style.Numberformat.Format = "#,##0";

                    worksheet.Cells[row, 7].Value = item.TienBenhNhanCanTra;
                    worksheet.Cells[row, 7].Style.Numberformat.Format = "#,##0";
                    worksheet.Cells[row, 7].Style.Font.Bold = true; // Tiền phải thu in đậm

                    // Trạng thái (Chuyển mã sang tiếng Việt)
                    string trangThaiText = (item.TrangThaiThanhToan == "DaTra" || item.TrangThaiThanhToan == "HoanTat")
                                           ? "Đã thanh toán" : "Chưa thanh toán";
                    worksheet.Cells[row, 8].Value = trangThaiText;

                    // Tô màu chữ trạng thái
                    if (trangThaiText == "Chưa thanh toán")
                    {
                        worksheet.Cells[row, 8].Style.Font.Color.SetColor(Color.Red);
                    }
                    else
                    {
                        worksheet.Cells[row, 8].Style.Font.Color.SetColor(Color.Green);
                    }

                    // Kẻ khung
                    worksheet.Cells[row, 1, row, 8].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[row, 1, row, 8].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[row, 1, row, 8].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[row, 1, row, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    row++;
                    stt++;
                }

                // --- D. Tổng kết cuối bảng ---
                worksheet.Cells[row, 1, row, 4].Merge = true;
                worksheet.Cells[row, 1].Value = "TỔNG CỘNG:";
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                worksheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                // Tổng tiền các cột
                worksheet.Cells[row, 5].Formula = $"SUM(E4:E{row - 1})";
                worksheet.Cells[row, 5].Style.Numberformat.Format = "#,##0";
                worksheet.Cells[row, 5].Style.Font.Bold = true;

                worksheet.Cells[row, 6].Formula = $"SUM(F4:F{row - 1})";
                worksheet.Cells[row, 6].Style.Numberformat.Format = "#,##0";

                worksheet.Cells[row, 7].Formula = $"SUM(G4:G{row - 1})";
                worksheet.Cells[row, 7].Style.Numberformat.Format = "#,##0";
                worksheet.Cells[row, 7].Style.Font.Bold = true;
                worksheet.Cells[row, 7].Style.Font.Color.SetColor(Color.Blue);

                // AutoFit cột
                worksheet.Cells.AutoFitColumns();

                // Xuất file
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string fileName = $"BaoCao_HoaDon_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }

    }
}