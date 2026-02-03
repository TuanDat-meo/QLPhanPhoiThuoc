using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLPhanPhoiThuoc.Models.Entities
{
    [Table("TheBHYT")]
    public class TheBHYT
    {
        [Key]
        [StringLength(20)]
        public string MaThe { get; set; }

        [Required]
        [StringLength(20)]
        [ForeignKey("BenhNhan")]
        public string MaBenhNhan { get; set; }

        [Required]
        [StringLength(15)]
        public string SoTheBHYT { get; set; }

        [Required]
        public DateTime NgayBatDau { get; set; }

        [Required]
        public DateTime NgayHetHan { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal MucHuong { get; set; }

        [StringLength(200)]
        public string NoiDangKyKCB { get; set; }

        [StringLength(300)]
        public string DiaChi5Nam { get; set; }

        [StringLength(20)]
        public string TrangThai { get; set; } = "ConHan"; // ConHan, HetHan, TamKhoa

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public DateTime? NgayCapNhat { get; set; }

        // Navigation Properties
        public virtual BenhNhan BenhNhan { get; set; }
    }
}