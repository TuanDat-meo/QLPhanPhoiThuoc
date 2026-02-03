using QLPhanPhoiThuoc.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLPhanPhoiThuoc.Models.Entities
{
    [Table("DonThuoc")]
    public class DonThuoc
    {
        [Key]
        [StringLength(20)]
        public string MaDonThuoc { get; set; }

        [Required]
        [StringLength(20)]
        [ForeignKey("ChanDoan")]
        public string MaChanDoan { get; set; }

        [Required]
        [StringLength(20)]
        [ForeignKey("BenhNhan")]
        public string MaBenhNhan { get; set; }

        [Required]
        [StringLength(20)]
        [ForeignKey("NhanVien")]
        public string MaNhanVien { get; set; } // Bác sĩ kê đơn

        [Required]
        public DateTime NgayKeDon { get; set; }

        [Required]
        [StringLength(20)]
        public string LoaiDon { get; set; } // BHYT, DichVu

        [Column(TypeName = "decimal(15,2)")]
        public decimal TongTien { get; set; } = 0;

        [StringLength(500)]
        public string ChanDoanSoBo { get; set; }

        [StringLength(500)]
        public string GhiChuBacSi { get; set; }

        [StringLength(20)]
        public string TrangThai { get; set; } = "ChoXuLy"; // ChoXuLy, DaXuLy, DaHuy

        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual ChanDoan ChanDoan { get; set; }
        public virtual BenhNhan BenhNhan { get; set; }
        public virtual NhanVien NhanVien { get; set; }
        public virtual ICollection<ChiTietDonThuoc> ChiTietDonThuocs { get; set; }
    }
}