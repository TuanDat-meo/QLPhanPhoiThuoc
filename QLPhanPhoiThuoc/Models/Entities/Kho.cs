using QLPhanPhoiThuoc.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLPhanPhoiThuoc.Models.Entities
{
    [Table("Kho")]
    public class Kho
    {
        [Key]
        [StringLength(20)]
        public string MaKho { get; set; }

        [Required]
        [StringLength(100)]
        public string TenKho { get; set; }

        [Required]
        [StringLength(20)]
        public string LoaiKho { get; set; } // BHYT, DichVu, TongHop

        [StringLength(200)]
        public string DiaDiem { get; set; }

        [StringLength(300)]
        public string GhiChu { get; set; }

        [StringLength(20)]
        public string TrangThai { get; set; } = "HoatDong"; // HoatDong, TamDung

        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual ICollection<LoThuoc> LoThuocs { get; set; }
        public virtual ICollection<PhieuNhap> PhieuNhaps { get; set; }
    }
}