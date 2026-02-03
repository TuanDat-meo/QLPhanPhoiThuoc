using QLPhanPhoiThuoc.Models.Entities.VNeID;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLPhanPhoiThuoc.Models.Entities.VNeID
{
    [Table("CongDan")]
    public class CongDan
    {
        [Key]
        [StringLength(12)]
        public string SoDinhDanh { get; set; } // Số CCCD/CMND

        [Required]
        [StringLength(100)]
        public string HoTen { get; set; }

        [Required]
        public DateTime NgaySinh { get; set; }

        [Required]
        [StringLength(10)]
        public string GioiTinh { get; set; } // Nam, Nu, Khac

        [StringLength(200)]
        public string QueQuan { get; set; }

        [StringLength(300)]
        public string NoiThuongTru { get; set; }

        [StringLength(300)]
        public string DiaChiHienTai { get; set; }

        [StringLength(50)]
        public string DanToc { get; set; }

        [StringLength(50)]
        public string TonGiao { get; set; }

        [StringLength(50)]
        public string QuocTich { get; set; } = "Việt Nam";

        public DateTime? NgayCap { get; set; }

        [StringLength(200)]
        public string NoiCap { get; set; }

        public DateTime? NgayHetHan { get; set; }

        [Column(TypeName = "text")]
        public string AnhChanDung { get; set; } // Base64 hoặc URL ảnh

        [StringLength(20)]
        public string TrangThai { get; set; } = "HoatDong"; // HoatDong, Mat, Huy, TamKhoa

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public DateTime? NgayCapNhat { get; set; }

        // Navigation Properties
        public virtual ICollection<TheBHYTMock> TheBHYTMocks { get; set; }
        public virtual ICollection<LichSuTraCuu> LichSuTraCuus { get; set; }
    }
}