using QLPhanPhoiThuoc.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLPhanPhoiThuoc.Models.Entities
{
    [Table("BenhNhan")]
    public class BenhNhan
    {
        [Key]
        [StringLength(20)]
        public string MaBenhNhan { get; set; }

        [Required]
        [StringLength(100)]
        public string TenBenhNhan { get; set; }

        public DateTime? NgaySinh { get; set; }

        [StringLength(10)]
        public string GioiTinh { get; set; } // Nam, Nu, Khac

        [StringLength(300)]
        public string DiaChi { get; set; }

        [StringLength(15)]
        public string SoDienThoai { get; set; }

        [StringLength(12)]
        public string CCCD { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(5)]
        public string NhomMau { get; set; } // A, B, AB, O, Rh+/Rh-

        [StringLength(100)]
        public string NgheNghiep { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? CanNang { get; set; } // Kg - Dùng tính liều thuốc

        [Column(TypeName = "decimal(5,2)")]
        public decimal? ChieuCao { get; set; } // Cm

        [Column(TypeName = "text")]
        public string TienSuDiUng { get; set; } // QUAN TRỌNG: Danh sách thuốc/thực phẩm dị ứng

        [StringLength(20)]
        public string LoaiBenhNhan { get; set; } = "NgoaiTru"; // NoiTru, NgoaiTru

        [StringLength(20)]
        public string TrangThai { get; set; } = "HoatDong"; // HoatDong, TuVong, Khac

        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual ICollection<TheBHYT> TheBHYTs { get; set; }
        public virtual ICollection<ChanDoan> ChanDoans { get; set; }
        public virtual ICollection<DonThuoc> DonThuocs { get; set; }
        public virtual ICollection<HoaDon> HoaDons { get; set; }
    }
}