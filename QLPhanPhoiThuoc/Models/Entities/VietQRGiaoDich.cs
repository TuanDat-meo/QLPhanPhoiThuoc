using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLPhanPhoiThuoc.Models.Entities
{
    [Table("VietQR_GiaoDich")]
    public class VietQRGiaoDich
    {
        [Key]
        [StringLength(20)]
        public string MaQR { get; set; }

        [Required]
        [StringLength(20)]
        [ForeignKey("HoaDon")]
        public string MaHoaDon { get; set; }

        [Required]
        [Column(TypeName = "decimal(15,2)")]
        public decimal SoTienYeuCau { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? SoTienNhan { get; set; }

        [Required]
        [StringLength(200)]
        public string NoiDungChuyenKhoan { get; set; }

        [StringLength(50)]
        public string MaGiaoDichNganHang { get; set; }

        [Column(TypeName = "text")]
        public string QRCodeBase64 { get; set; }

        public DateTime? NgayThanhToan { get; set; }

        [Column(TypeName = "text")]
        public string DuLieuWebhook { get; set; }

        [StringLength(20)]
        public string TrangThai { get; set; } = "ChoThanhToan"; // ChoThanhToan, DaThanhToan, HetHan, DaHuy

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public DateTime? NgayHetHan { get; set; }

        // Navigation Properties
        public virtual HoaDon HoaDon { get; set; }
    }
}