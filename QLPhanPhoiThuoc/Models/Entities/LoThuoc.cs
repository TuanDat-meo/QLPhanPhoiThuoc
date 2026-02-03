using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLPhanPhoiThuoc.Models.Entities
{
    [Table("LoThuoc")]
    public class LoThuoc
    {
        [Key]
        [StringLength(20)]
        public string MaLo { get; set; }

        [Required]
        [StringLength(20)]
        [ForeignKey("Thuoc")]
        public string MaThuoc { get; set; }

        [Required]
        [StringLength(20)]
        [ForeignKey("Kho")]
        public string MaKho { get; set; }

        [Required]
        [StringLength(50)]
        public string SoLo { get; set; }

        public DateTime? NgaySanXuat { get; set; }

        [Required]
        public DateTime HanSuDung { get; set; }

        public int SoLuongNhap { get; set; } = 0;

        public int SoLuongCon { get; set; } = 0;

        [StringLength(20)]
        public string TrangThai { get; set; } = "ConHang"; // ConHang, HetHang, GanHetHan, HetHan

        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual Thuoc Thuoc { get; set; }
        public virtual Kho Kho { get; set; }
    }
}