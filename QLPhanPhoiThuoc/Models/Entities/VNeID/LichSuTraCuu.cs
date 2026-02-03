using QLPhanPhoiThuoc.Models.Entities.VNeID;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLPhanPhoiThuoc.Models.Entities.VNeID
{
    [Table("LichSuTraCuu")]
    public class LichSuTraCuu
    {
        [Key]
        [StringLength(20)]
        public string MaTraCuu { get; set; }

        [Required]
        [StringLength(12)]
        [ForeignKey("CongDan")]
        public string SoDinhDanh { get; set; }

        [Required]
        [StringLength(50)]
        public string LoaiTraCuu { get; set; } // ThongTinCoBan, ThongTinBHYT, ThongTinDayDu

        [Required]
        [StringLength(50)]
        public string HeThongTraCuu { get; set; } // Tên hệ thống yêu cầu

        [StringLength(45)]
        public string IPAddress { get; set; }

        [StringLength(500)]
        public string UserAgent { get; set; }

        [Required]
        public DateTime NgayGioTraCuu { get; set; }

        [Required]
        [StringLength(20)]
        public string KetQua { get; set; } // ThanhCong, KhongTimThay, Loi

        [Column(TypeName = "text")]
        public string DuLieuTraVe { get; set; } // JSON response

        public int? ThoiGianXuLy { get; set; } // Milliseconds

        [StringLength(500)]
        public string GhiChu { get; set; }

        // Navigation Properties
        public virtual CongDan CongDan { get; set; }
    }
}