using QLPhanPhoiThuoc.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLPhanPhoiThuoc.Models.Entities
{
    [Table("ChanDoan")]
    public class ChanDoan
    {
        [Key]
        [StringLength(20)]
        public string MaChanDoan { get; set; }

        [Required]
        [StringLength(20)]
        [ForeignKey("BenhNhan")]
        public string MaBenhNhan { get; set; }

        [Required]
        [StringLength(20)]
        [ForeignKey("NhanVien")]
        public string MaNhanVien { get; set; } // Bác sĩ chẩn đoán

        [Required]
        public DateTime NgayChanDoan { get; set; }

        [Required]
        [StringLength(300)]
        public string TenBenh { get; set; } // Lưu trực tiếp tên bệnh

        [StringLength(10)]
        public string MaICD10 { get; set; }

        [Column(TypeName = "text")]
        public string TrieuChung { get; set; }

        [StringLength(500)]
        public string ChanDoanSoBo { get; set; }

        [StringLength(500)]
        public string KetLuan { get; set; }

        [StringLength(500)]
        public string GhiChu { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual BenhNhan BenhNhan { get; set; }
        public virtual NhanVien NhanVien { get; set; }
        public virtual ICollection<DonThuoc> DonThuocs { get; set; }
    }
}