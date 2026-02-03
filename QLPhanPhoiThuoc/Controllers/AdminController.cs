using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLPhanPhoiThuoc.Models.EF;
using System.Security.Claims;

namespace QLPhanPhoiThuoc.Controllers
{
    [Authorize(Roles = "Admin,NhanVien")]
    public class AdminController : Controller
    {
        private readonly BenhVienDbContext _benhVienContext;
        private readonly VNeIDDbContext _vneIdContext;

        public AdminController(BenhVienDbContext benhVienContext, VNeIDDbContext vneIdContext)
        {
            _benhVienContext = benhVienContext;
            _vneIdContext = vneIdContext;
        }

        // GET: /Admin/AdminIndex
        public IActionResult AdminIndex()
        {
            // Lấy thông tin admin từ claims
            var maNhanVien = User.FindFirstValue("MaNhanVien");
            var tenNhanVien = User.FindFirstValue("TenNhanVien");
            var chucVu = User.FindFirstValue("ChucVu");

            ViewBag.MaNhanVien = maNhanVien;
            ViewBag.TenNhanVien = tenNhanVien ?? "Admin";
            ViewBag.ChucVu = chucVu ?? "Quản trị viên";

            return View();
        }
    }
}