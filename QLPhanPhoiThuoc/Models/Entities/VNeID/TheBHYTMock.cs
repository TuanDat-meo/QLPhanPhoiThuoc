using QLPhanPhoiThuoc.Models.Entities.VNeID;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLPhanPhoiThuoc.Models.Entities.VNeID
{
    [Table("TheBHYT_Mock")]
    public class TheBHYTMock
    {
        [Key]
        [StringLength(20)]
        public string MaThe { get; set; }

        [Required]
        [StringLength(12)]
        [ForeignKey("CongDan")]
        public string SoDinhDanh { get; set; }

        [Required]
        [StringLength(15)]
        public string SoTheBHYT { get; set; } // Mã thẻ BHYT 15 ký tự

        [Required]
        public DateTime NgayBatDau { get; set; }

        [Required]
        public DateTime NgayHetHan { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal MucHuong { get; set; } // 80, 95, 100

        [StringLength(200)]
        public string NoiDKKCB { get; set; } // Nơi đăng ký khám chữa bệnh ban đầu

        [StringLength(10)]
        public string MaNoiDKKCB { get; set; } // Mã BV đăng ký

        [StringLength(300)]
        public string DiaChi5Nam { get; set; } // Nơi cư trú 5 năm liên tục

        [StringLength(5)]
        public string MaKhuVuc { get; set; } // K1, K2, K3

        [StringLength(20)]
        public string TrangThai { get; set; } = "ConHan"; // ConHan, HetHan, TamKhoa, Khoa

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public DateTime? NgayCapNhat { get; set; }

        // Navigation Properties
        public virtual CongDan CongDan { get; set; }
    }
}