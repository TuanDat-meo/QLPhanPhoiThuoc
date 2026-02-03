using System;
using System.ComponentModel.DataAnnotations;

namespace QLPhanPhoiThuoc.Models.Entities
{
    public class TaiKhoan
    {
        [Key]
        [MaxLength(20)]
        public string MaTaiKhoan { get; set; } = string.Empty;

        /// <summary>
        /// ⭐ PROPERTY NÀY ĐÃ ĐƯỢC THÊM ĐỂ FIX LỖI
        /// Mã nhân viên (nullable - vì User không có nhân viên)
        /// </summary>
        [MaxLength(20)]
        public string? MaNhanVien { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Role: Admin, NhanVien, User
        /// (Trước đây database có: BacSi, DuocSi, ThuNgan, KhoThuoc - đã sửa)
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Role { get; set; } = string.Empty;

        [MaxLength(20)]
        public string TrangThai { get; set; } = "HoatDong";

        public DateTime? LanDangNhapCuoi { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public DateTime? NgayCapNhat { get; set; }

        // Navigation properties
        public virtual NhanVien? NhanVien { get; set; }
        public virtual BenhNhan? BenhNhan { get; set; }
    }
}