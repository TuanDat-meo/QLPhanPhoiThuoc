using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLPhanPhoiThuoc.Models.Entities
{
    [Table("TaiKhoan")]
    public class TaiKhoan
    {
        [Key]
        [StringLength(20)]
        public string MaTaiKhoan { get; set; }

        [Required]
        [StringLength(20)]
        [ForeignKey("NhanVien")]
        public string MaNhanVien { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; }

        [Required]
        [StringLength(20)]
        public string Role { get; set; } // Admin, BacSi, DuocSi, ThuNgan, TiepDon, KhoThuoc

        [StringLength(20)]
        public string TrangThai { get; set; } = "KichHoat"; // KichHoat, Khoa, Xoa

        public DateTime? LanDangNhapCuoi { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual NhanVien NhanVien { get; set; }
    }
}