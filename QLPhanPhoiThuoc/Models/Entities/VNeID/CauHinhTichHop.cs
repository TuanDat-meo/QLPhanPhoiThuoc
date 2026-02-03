using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLPhanPhoiThuoc.Models.Entities.VNeID
{
    [Table("CauHinhTichHop")]
    public class CauHinhTichHop
    {
        [Key]
        [StringLength(20)]
        public string MaCauHinh { get; set; }

        [Required]
        [StringLength(100)]
        public string TenHeThong { get; set; }

        [Required]
        [StringLength(255)]
        public string APIKey { get; set; }

        [StringLength(255)]
        public string APISecret { get; set; }

        [Column(TypeName = "text")]
        public string IPWhitelist { get; set; } // Danh sách IP được phép, cách nhau bởi dấu phẩy

        public int SoLanTraCuuToiDa { get; set; } = 1000; // Giới hạn số lần tra cứu/ngày

        [StringLength(20)]
        public string TrangThai { get; set; } = "KichHoat"; // KichHoat, TamDung, Khoa

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public DateTime? NgayCapNhat { get; set; }
    }
}