using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLPhanPhoiThuoc.Models.Entities
{
    [Table("ChiTietPhieuNhap")]
    public class ChiTietPhieuNhap
    {
        [Key, Column(Order = 0)]
        [StringLength(20)]
        [ForeignKey("PhieuNhap")]
        public string MaPhieuNhap { get; set; }

        [Key, Column(Order = 1)]
        [StringLength(20)]
        [ForeignKey("LoThuoc")]
        public string MaLo { get; set; }

        [Required]
        public int SoLuongNhap { get; set; }

        [Required]
        [Column(TypeName = "decimal(15,2)")]
        public decimal DonGiaNhap { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Column(TypeName = "decimal(15,2)")]
        public decimal? ThanhTien { get; set; }

        // Navigation Properties
        public virtual PhieuNhap PhieuNhap { get; set; }
        public virtual LoThuoc LoThuoc { get; set; }
    }
}