using QLPhanPhoiThuoc.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLPhanPhoiThuoc.Models.Entities
{
    [Table("KhoaPhong")]
    public class KhoaPhong
    {
        [Key]
        [StringLength(20)]
        public string MaKhoa { get; set; }

        [Required]
        [StringLength(100)]
        public string TenKhoa { get; set; }

        [StringLength(100)]
        public string TruongKhoa { get; set; }

        [StringLength(15)]
        public string SoDienThoai { get; set; }

        [StringLength(20)]
        public string TrangThai { get; set; } = "HoatDong"; // HoatDong, TamDung

        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual ICollection<NhanVien> NhanViens { get; set; }
    }
}