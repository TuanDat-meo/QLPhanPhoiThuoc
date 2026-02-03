using QLPhanPhoiThuoc.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLPhanPhoiThuoc.Models.Entities
{
    [Table("PhieuNhap")]
    public class PhieuNhap
    {
        [Key]
        [StringLength(20)]
        public string MaPhieuNhap { get; set; }

        [Required]
        [StringLength(20)]
        [ForeignKey("NhaCungCap")]
        public string MaNCC { get; set; }

        [Required]
        [StringLength(20)]
        [ForeignKey("Kho")]
        public string MaKho { get; set; }

        [Required]
        public DateTime NgayNhap { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal TongTien { get; set; } = 0;

        [Required]
        [StringLength(20)]
        [ForeignKey("NhanVien")]
        public string NhanVienNhap { get; set; }

        [StringLength(50)]
        public string SoHoaDon { get; set; }

        [StringLength(20)]
        public string TrangThai { get; set; } = "DangNhap"; // DangNhap, DaNhap, DaHuy

        [StringLength(500)]
        public string GhiChu { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual NhaCungCap NhaCungCap { get; set; }
        public virtual Kho Kho { get; set; }
        public virtual NhanVien NhanVien { get; set; }
        public virtual ICollection<ChiTietPhieuNhap> ChiTietPhieuNhaps { get; set; }
    }
}