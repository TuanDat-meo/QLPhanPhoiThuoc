using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLPhanPhoiThuoc.Models.Entities
{
    [Table("CauHinh_VietQR")]
    public class CauHinhVietQR
    {
        [Key]
        [StringLength(20)]
        public string MaCauHinh { get; set; }

        [Required]
        [StringLength(200)]
        public string TenNganHang { get; set; }

        [Required]
        [StringLength(20)]
        public string SoTaiKhoan { get; set; }

        [Required]
        [StringLength(200)]
        public string TenTaiKhoan { get; set; }

        [Required]
        [StringLength(10)]
        public string MaNganHang { get; set; }

        [StringLength(20)]
        public string Template { get; set; } = "compact2";

        [StringLength(500)]
        public string WebhookURL { get; set; }

        [StringLength(255)]
        public string APIKey { get; set; }

        [StringLength(255)]
        public string APISecret { get; set; }

        [StringLength(20)]
        public string TrangThai { get; set; } = "KichHoat"; // KichHoat, TamDung

        [StringLength(500)]
        public string GhiChu { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public DateTime? NgayCapNhat { get; set; }
    }
}