using QLPhanPhoiThuoc.Models.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLPhanPhoiThuoc.Models.Entities
{
    [Table("NhanVien")]
    public class NhanVien
    {
        [Key]
        [StringLength(20)]
        public string MaNhanVien { get; set; }

        [Required]
        [StringLength(100)]
        public string TenNhanVien { get; set; }

        [StringLength(50)]
        public string ChucVu { get; set; } // BacSi, DuocSi, ThuNgan, TiepDon, KhoThuoc

        [StringLength(100)]
        public string ChuyenKhoa { get; set; } // NULL nếu không phải bác sĩ

        [StringLength(200)]
        public string BangCap { get; set; } // CK1/CK2/ThS/BS/Dược sĩ

        [StringLength(20)]
        [ForeignKey("KhoaPhong")]
        public string MaKhoa { get; set; }

        [StringLength(15)]
        public string SoDienThoai { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(12)]
        public string CCCD { get; set; }

        public DateTime? NgaySinh { get; set; }

        [StringLength(10)]
        public string GioiTinh { get; set; } // Nam, Nu, Khac

        [StringLength(300)]
        public string DiaChi { get; set; }

        [StringLength(20)]
        public string TrangThai { get; set; } = "DangLamViec"; // DangLamViec, NghiViec, TamNghi

        public DateTime? NgayVaoLam { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual KhoaPhong KhoaPhong { get; set; }
        public virtual TaiKhoan TaiKhoan { get; set; }
    }
}