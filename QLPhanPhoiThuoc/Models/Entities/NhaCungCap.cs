using QLPhanPhoiThuoc.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLPhanPhoiThuoc.Models.Entities
{
    [Table("NhaCungCap")]
    public class NhaCungCap
    {
        [Key]
        [StringLength(20)]
        public string MaNCC { get; set; }

        [Required]
        [StringLength(200)]
        public string TenNCC { get; set; }

        [StringLength(300)]
        public string DiaChi { get; set; }

        [StringLength(15)]
        public string SoDienThoai { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(20)]
        public string MaSoThue { get; set; }

        [StringLength(20)]
        public string TrangThai { get; set; } = "HoatDong"; // HoatDong, TamDung

        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual ICollection<PhieuNhap> PhieuNhaps { get; set; }
    }
}