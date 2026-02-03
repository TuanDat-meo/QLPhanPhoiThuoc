using QLPhanPhoiThuoc.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLPhanPhoiThuoc.Models.Entities
{
    [Table("Thuoc")]
    public class Thuoc
    {
        [Key]
        [StringLength(20)]
        public string MaThuoc { get; set; }

        [Required]
        [StringLength(200)]
        public string TenThuoc { get; set; }

        [StringLength(200)]
        public string HoatChat { get; set; }

        [Required]
        [StringLength(50)]
        public string DonViTinh { get; set; }

        [StringLength(100)]
        public string HamLuong { get; set; }

        [StringLength(100)]
        public string DangBaoChe { get; set; }

        [StringLength(100)]
        public string DuongDung { get; set; }

        [StringLength(200)]
        public string NhaSanXuat { get; set; }

        [StringLength(100)]
        public string NhomThuoc { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal GiaNhap { get; set; } = 0;

        [Column(TypeName = "decimal(15,2)")]
        public decimal GiaXuat { get; set; } = 0;

        public int TonKhoToiThieu { get; set; } = 10;

        [StringLength(3)]
        public string LaThuocBHYT { get; set; } = "No"; // Yes/No

        [Column(TypeName = "decimal(5,2)")]
        public decimal TyLeBHYTChiTra { get; set; } = 0;

        [StringLength(500)]
        public string MoTa { get; set; }

        [StringLength(20)]
        public string TrangThai { get; set; } = "KichHoat"; // KichHoat, NgungSuDung

        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual ICollection<LoThuoc> LoThuocs { get; set; }
        public virtual ICollection<ChiTietDonThuoc> ChiTietDonThuocs { get; set; }
    }
}