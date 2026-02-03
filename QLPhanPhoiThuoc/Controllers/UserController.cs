using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLPhanPhoiThuoc.Models.EF;
using System.Security.Claims;

namespace QLPhanPhoiThuoc.Controllers
{
    [Authorize(Roles = "User")]
    public class UserController : Controller
    {
        private readonly BenhVienDbContext _benhVienContext;
        private readonly VNeIDDbContext _vneIdContext;

        public UserController(BenhVienDbContext benhVienContext, VNeIDDbContext vneIdContext)
        {
            _benhVienContext = benhVienContext;
            _vneIdContext = vneIdContext;
        }

        // GET: /User/UserIndex
        public async Task<IActionResult> UserIndex()
        {
            // Lấy thông tin user từ claims
            var maBenhNhan = User.FindFirstValue("MaBenhNhan");
            var tenBenhNhan = User.FindFirstValue("TenBenhNhan");

            // Lấy thông tin chi tiết bệnh nhân từ database
            if (!string.IsNullOrEmpty(maBenhNhan))
            {
                var benhNhan = await _benhVienContext.BenhNhans
                    .FirstOrDefaultAsync(b => b.MaBenhNhan == maBenhNhan);

                if (benhNhan != null)
                {
                    ViewBag.MaBenhNhan = maBenhNhan;
                    ViewBag.TenBenhNhan = benhNhan.TenBenhNhan;
                    ViewBag.CCCD = benhNhan.CCCD;
                    ViewBag.NgaySinh = benhNhan.NgaySinh;
                    ViewBag.GioiTinh = benhNhan.GioiTinh;
                    ViewBag.DiaChi = benhNhan.DiaChi;
                    ViewBag.SoDienThoai = benhNhan.SoDienThoai;
                    ViewBag.Email = benhNhan.Email;
                    ViewBag.NhomMau = benhNhan.NhomMau;
                }
            }
            else
            {
                ViewBag.TenBenhNhan = tenBenhNhan ?? "Người dùng";
            }

            return View();
        }
    }
}