using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QLPhanPhoiThuoc.Models.Entities
{
    public class NhaCungCap
    {
        [Key]
        [StringLength(20)]
        public string MaNCC { get; set; } = null!;

        [Required(ErrorMessage = "Tên nhà cung cấp là bắt buộc")]
        [StringLength(200)]
        public string TenNCC { get; set; } = null!;

        [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
        [StringLength(300)]
        public string DiaChi { get; set; } = string.Empty;

        [StringLength(15)]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string SoDienThoai { get; set; } = string.Empty;

        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        public string MaSoThue { get; set; } = string.Empty;

        [StringLength(20)]
        public string TrangThai { get; set; } = "HoatDong";

        public DateTime NgayTao { get; set; } = DateTime.Now;

        // CRITICAL: Navigation property phải nullable để không bị validation error
        public virtual ICollection<PhieuNhap>? PhieuNhaps { get; set; }
    }
}