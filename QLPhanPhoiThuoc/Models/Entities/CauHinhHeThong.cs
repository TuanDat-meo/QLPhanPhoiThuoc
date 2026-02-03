using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLPhanPhoiThuoc.Models.Entities
{
    [Table("CauHinhHeThong")]
    public class CauHinhHeThong
    {
        [Key]
        [StringLength(20)]
        public string MaCauHinh { get; set; }

        [Required]
        [StringLength(100)]
        public string TenCauHinh { get; set; }

        [Required]
        [StringLength(500)]
        public string GiaTri { get; set; }

        [StringLength(500)]
        public string MoTa { get; set; }

        [StringLength(20)]
        public string LoaiDuLieu { get; set; } // INT, DECIMAL, STRING, BOOLEAN

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public DateTime? NgayCapNhat { get; set; }
    }
}