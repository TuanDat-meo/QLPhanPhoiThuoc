using System.ComponentModel.DataAnnotations;

namespace QLPhanPhoiThuoc.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Display(Name = "Ghi nhớ đăng nhập")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }

    public class RegisterViewModel
    {
        // Bước 1: Tra cứu VNeID
        [Required(ErrorMessage = "Vui lòng nhập số CCCD")]
        [StringLength(12, MinimumLength = 12, ErrorMessage = "Số CCCD phải có 12 chữ số")]
        [Display(Name = "Số CCCD")]
        public string CCCD { get; set; }

        // Thông tin từ VNeID (tự động điền)
        public string HoTen { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string GioiTinh { get; set; }
        public string DiaChi { get; set; }
        public string QueQuan { get; set; }

        // Bước 2: Thông tin đăng ký tài khoản
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Tên đăng nhập từ 4-50 ký tự")]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        [Display(Name = "Xác nhận mật khẩu")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; }

        // Thông tin bệnh nhân bổ sung
        [Display(Name = "Nhóm máu")]
        public string NhomMau { get; set; }

        [Display(Name = "Nghề nghiệp")]
        public string NgheNghiep { get; set; }

        [Display(Name = "Tiền sử dị ứng")]
        [DataType(DataType.MultilineText)]
        public string TienSuDiUng { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email đăng ký")]
        public string Email { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        public string Token { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu mới")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        [Display(Name = "Xác nhận mật khẩu")]
        public string ConfirmPassword { get; set; }
    }

    public class VNeIDLookupResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public VNeIDCongDanData Data { get; set; }
    }

    public class VNeIDCongDanData
    {
        public string SoDinhDanh { get; set; }
        public string HoTen { get; set; }
        public DateTime NgaySinh { get; set; }
        public string GioiTinh { get; set; }
        public string QueQuan { get; set; }
        public string NoiThuongTru { get; set; }
        public string DiaChiHienTai { get; set; }
        public string DanToc { get; set; }
        public string TonGiao { get; set; }
    }
}