using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLPhanPhoiThuoc.Models.Entities
{
    [Table("ChiTietPhieuCap")]
    public class ChiTietPhieuCap
    {
        [Key, Column(Order = 0)]
        [StringLength(20)]
        [ForeignKey("PhieuCapPhat")]
        public string MaPhieuCap { get; set; }

        [Key, Column(Order = 1)]
        [StringLength(20)]
        [ForeignKey("LoThuoc")]
        public string MaLo { get; set; }

        [Required]
        public int SoLuongCap { get; set; }

        [Required]
        [Column(TypeName = "decimal(15,2)")]
        public decimal DonGiaCap { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Column(TypeName = "decimal(15,2)")]
        public decimal? ThanhTien { get; set; }

        // Navigation Properties
        public virtual PhieuCapPhat PhieuCapPhat { get; set; }
        public virtual LoThuoc LoThuoc { get; set; }
    }
}