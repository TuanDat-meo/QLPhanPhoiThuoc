using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLPhanPhoiThuoc.Models.Entities
{
    [Table("PhieuThuTien")]
    public class PhieuThuTien
    {
        [Key]
        [StringLength(20)]
        public string MaPhieuThu { get; set; }

        [Required]
        [StringLength(20)]
        [ForeignKey("HoaDon")]
        public string MaHoaDon { get; set; }

        [Required]
        public DateTime NgayThu { get; set; }

        [Required]
        [Column(TypeName = "decimal(15,2)")]
        public decimal SoTienThu { get; set; }

        [Required]
        [StringLength(50)]
        public string HinhThucThanhToan { get; set; } // TienMat, ChuyenKhoan, VietQR, TheMoBanking

        [StringLength(50)]
        public string MaGiaoDichNganHang { get; set; }

        [Required]
        [StringLength(20)]
        [ForeignKey("NhanVien")]
        public string NhanVienThu { get; set; }

        [StringLength(20)]
        public string TrangThai { get; set; } = "DaXacNhan"; // DaXacNhan, DaHuy

        [StringLength(500)]
        public string GhiChu { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual HoaDon HoaDon { get; set; }
        public virtual NhanVien NhanVien { get; set; }
    }
}