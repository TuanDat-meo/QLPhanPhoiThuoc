using QLPhanPhoiThuoc.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLPhanPhoiThuoc.Models.Entities
{
    [Table("HoaDon")]
    public class HoaDon
    {
        [Key]
        [StringLength(20)]
        public string MaHoaDon { get; set; }

        [Required]
        [StringLength(20)]
        [ForeignKey("BenhNhan")]
        public string MaBenhNhan { get; set; }

        [StringLength(20)]
        [ForeignKey("DonThuoc")]
        public string MaDonThuoc { get; set; }

        [Required]
        public DateTime NgayTaoHoaDon { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal TongTien { get; set; } = 0;

        [Column(TypeName = "decimal(15,2)")]
        public decimal TienBHYTChiTra { get; set; } = 0;

        [Column(TypeName = "decimal(15,2)")]
        public decimal TienBenhNhanCanTra { get; set; } = 0;

        [Column(TypeName = "decimal(15,2)")]
        public decimal TienDaTra { get; set; } = 0;

        [StringLength(20)]
        public string MaSoThue { get; set; }

        [StringLength(50)]
        public string HinhThucThanhToan { get; set; } // TienMat, ChuyenKhoan, VietQR, TheMoBanking

        [StringLength(20)]
        public string TrangThaiThanhToan { get; set; } = "ChuaTra"; // ChuaTra, DaTra1Phan, DaTra

        [StringLength(500)]
        public string GhiChu { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual BenhNhan BenhNhan { get; set; }
        public virtual DonThuoc DonThuoc { get; set; }
        public virtual ICollection<PhieuThuTien> PhieuThuTiens { get; set; }
    }
}