using QLPhanPhoiThuoc.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLPhanPhoiThuoc.Models.Entities
{
    [Table("PhieuCapPhat")]
    public class PhieuCapPhat
    {
        [Key]
        [StringLength(20)]
        public string MaPhieuCap { get; set; }

        [Required]
        [StringLength(20)]
        [ForeignKey("DonThuoc")]
        public string MaDonThuoc { get; set; }

        [Required]
        [StringLength(20)]
        [ForeignKey("BenhNhan")]
        public string MaBenhNhan { get; set; }

        [Required]
        [StringLength(20)]
        [ForeignKey("Kho")]
        public string MaKho { get; set; }

        [Required]
        public DateTime NgayCap { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal TongTien { get; set; } = 0;

        [Required]
        [StringLength(20)]
        [ForeignKey("NhanVien")]
        public string NhanVienCap { get; set; }

        [StringLength(20)]
        public string TrangThai { get; set; } = "DangXuLy"; // DangXuLy, DaCap, DaHuy

        [StringLength(500)]
        public string GhiChu { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual DonThuoc DonThuoc { get; set; }
        public virtual BenhNhan BenhNhan { get; set; }
        public virtual Kho Kho { get; set; }
        public virtual NhanVien NhanVien { get; set; }
        public virtual ICollection<ChiTietPhieuCap> ChiTietPhieuCaps { get; set; }
    }
}