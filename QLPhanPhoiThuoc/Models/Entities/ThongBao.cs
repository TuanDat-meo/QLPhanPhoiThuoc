using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLPhanPhoiThuoc.Models.Entities
{
    [Table("ThongBao")]
    public class ThongBao
    {
        [Key]
        [StringLength(20)]
        public string MaThongBao { get; set; }

        [Required]
        [StringLength(200)]
        public string TieuDe { get; set; }

        [Required]
        [Column(TypeName = "text")]
        public string NoiDung { get; set; }

        [Required]
        [StringLength(50)]
        public string LoaiThongBao { get; set; } // HetHan, TonKhoThap, ThanhToan, HeThong

        [StringLength(20)]
        public string DoUuTien { get; set; } = "Binh"; // Thap, Binh, Cao, Nguy

        [StringLength(20)]
        [ForeignKey("NhanVien")]
        public string? NguoiNhan { get; set; } 

        public bool DaDoc { get; set; } = false;

        public DateTime? NgayHetHan { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual NhanVien NhanVien { get; set; }
    }
}