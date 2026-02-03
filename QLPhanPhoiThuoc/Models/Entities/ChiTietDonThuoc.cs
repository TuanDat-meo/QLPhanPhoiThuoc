using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLPhanPhoiThuoc.Models.Entities
{
    [Table("ChiTietDonThuoc")]
    public class ChiTietDonThuoc
    {
        [Key, Column(Order = 0)]
        [StringLength(20)]
        [ForeignKey("DonThuoc")]
        public string MaDonThuoc { get; set; }

        [Key, Column(Order = 1)]
        [StringLength(20)]
        [ForeignKey("Thuoc")]
        public string MaThuoc { get; set; }

        [Required]
        public int SoLuong { get; set; }

        [StringLength(200)]
        public string LieuDung { get; set; } // Ví dụ: "1 viên/lần, 2 lần/ngày"

        [Required]
        public int SoNgayDung { get; set; }

        [StringLength(200)]
        public string CachDung { get; set; } // Ví dụ: "Uống sau ăn", "Tiêm tĩnh mạch"

        [StringLength(300)]
        public string GhiChu { get; set; }

        // Navigation Properties
        public virtual DonThuoc DonThuoc { get; set; }
        public virtual Thuoc Thuoc { get; set; }
    }
}