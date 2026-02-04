using System.ComponentModel.DataAnnotations;

namespace QLPhanPhoiThuoc.Models.ViewModels
{
    public class RegisterViewModel
    {
        // Thông tin từ VNeID
        [Required(ErrorMessage = "Vui lòng nhập số CCCD")]
        [Display(Name = "Số CCCD")]
        [StringLength(12, MinimumLength = 9, ErrorMessage = "Số CCCD phải từ 9-12 ký tự")]
        public string CCCD { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ và tên")]
        [StringLength(100, ErrorMessage = "Họ tên không được quá 100 ký tự")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn ngày sinh")]
        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime NgaySinh { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn giới tính")]
        [Display(Name = "Giới tính")]
        public string GioiTinh { get; set; } = string.Empty;

        [Display(Name = "Địa chỉ")]
        [StringLength(300, ErrorMessage = "Địa chỉ không được quá 300 ký tự")]
        public string? DiaChi { get; set; }

        [Display(Name = "Quê quán")]
        [StringLength(200, ErrorMessage = "Quê quán không được quá 200 ký tự")]
        public string? QueQuan { get; set; }

        // Thông tin tài khoản
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        [Display(Name = "Tên đăng nhập")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Tên đăng nhập phải từ 4-50 ký tự")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Tên đăng nhập chỉ được chứa chữ cái, số và dấu gạch dưới")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
        [Display(Name = "Email")]
        [StringLength(100, ErrorMessage = "Email không được quá 100 ký tự")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [StringLength(100, ErrorMessage = "Mật khẩu phải từ {2} đến {1} ký tự", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        // Yêu cầu: ít nhất 1 chữ hoa, 1 chữ thường, 1 số và 1 ký tự đặc biệt
        [RegularExpression(@"^(?=.*[a-z])(?=(.*[A-Z]))(?=.*[0-9])(?=.*[!@#$%^&*()_+{}:;<>,.?~-]).{8,}$", ErrorMessage = "Mật khẩu phải bao gồm chữ hoa, chữ thường, số và ký tự đặc biệt")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; } = string.Empty;

        // Thông tin bổ sung
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Display(Name = "Số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [RegularExpression(@"^(0[3|5|7|8|9])+([0-9]{8})$", ErrorMessage = "Số điện thoại không đúng định dạng")]
        public string SoDienThoai { get; set; } = string.Empty;

        [Display(Name = "Nhóm máu")]
        [StringLength(5, ErrorMessage = "Nhóm máu không được quá 5 ký tự")]
        public string? NhomMau { get; set; }

        [Display(Name = "Nghề nghiệp")]
        [StringLength(100, ErrorMessage = "Nghề nghiệp không được quá 100 ký tự")]
        public string? NgheNghiep { get; set; }

        [Display(Name = "Tiền sử dị ứng")]
        [StringLength(500, ErrorMessage = "Tiền sử dị ứng không được quá 500 ký tự")]
        public string? TienSuDiUng { get; set; }

        [Required(ErrorMessage = "Bạn phải đồng ý với điều khoản sử dụng")]
        [Range(typeof(bool), "true", "true", ErrorMessage = "Vui lòng xác nhận đồng ý với điều khoản")]
        public bool AgreeTerms { get; set; }
    }
}