using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLPhanPhoiThuoc.Migrations.BenhVien
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BenhNhan",
                columns: table => new
                {
                    MaBenhNhan = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenBenhNhan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GioiTinh = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    CCCD = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NhomMau = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    NgheNghiep = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CanNang = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    ChieuCao = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    TienSuDiUng = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiBenhNhan = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "NgoaiTru"),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "HoatDong"),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BenhNhan", x => x.MaBenhNhan);
                });

            migrationBuilder.CreateTable(
                name: "CauHinh_VietQR",
                columns: table => new
                {
                    MaCauHinh = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenNganHang = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SoTaiKhoan = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenTaiKhoan = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MaNganHang = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Template = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "compact2"),
                    WebhookURL = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    APIKey = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    APISecret = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "KichHoat"),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CauHinh_VietQR", x => x.MaCauHinh);
                });

            migrationBuilder.CreateTable(
                name: "CauHinhHeThong",
                columns: table => new
                {
                    MaCauHinh = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenCauHinh = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GiaTri = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    LoaiDuLieu = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CauHinhHeThong", x => x.MaCauHinh);
                });

            migrationBuilder.CreateTable(
                name: "Kho",
                columns: table => new
                {
                    MaKho = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenKho = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LoaiKho = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DiaDiem = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "HoatDong"),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kho", x => x.MaKho);
                });

            migrationBuilder.CreateTable(
                name: "KhoaPhong",
                columns: table => new
                {
                    MaKhoa = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenKhoa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TruongKhoa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "HoatDong"),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhoaPhong", x => x.MaKhoa);
                });

            migrationBuilder.CreateTable(
                name: "NhaCungCap",
                columns: table => new
                {
                    MaNCC = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenNCC = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MaSoThue = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "HoatDong"),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhaCungCap", x => x.MaNCC);
                });

            migrationBuilder.CreateTable(
                name: "Thuoc",
                columns: table => new
                {
                    MaThuoc = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenThuoc = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    HoatChat = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DonViTinh = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    HamLuong = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DangBaoChe = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DuongDung = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NhaSanXuat = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NhomThuoc = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GiaNhap = table.Column<decimal>(type: "decimal(15,2)", nullable: false, defaultValue: 0m),
                    GiaXuat = table.Column<decimal>(type: "decimal(15,2)", nullable: false, defaultValue: 0m),
                    TonKhoToiThieu = table.Column<int>(type: "int", nullable: false, defaultValue: 10),
                    LaThuocBHYT = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "No"),
                    TyLeBHYTChiTra = table.Column<decimal>(type: "decimal(5,2)", nullable: false, defaultValue: 0m),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "KichHoat"),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Thuoc", x => x.MaThuoc);
                });

            migrationBuilder.CreateTable(
                name: "TheBHYT",
                columns: table => new
                {
                    MaThe = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaBenhNhan = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SoTheBHYT = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayHetHan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MucHuong = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    NoiDangKyKCB = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DiaChi5Nam = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "ConHan"),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TheBHYT", x => x.MaThe);
                    table.ForeignKey(
                        name: "FK_TheBHYT_BenhNhan_MaBenhNhan",
                        column: x => x.MaBenhNhan,
                        principalTable: "BenhNhan",
                        principalColumn: "MaBenhNhan",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NhanVien",
                columns: table => new
                {
                    MaNhanVien = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenNhanVien = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ChucVu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ChuyenKhoa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BangCap = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MaKhoa = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CCCD = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GioiTinh = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "DangLamViec"),
                    NgayVaoLam = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhanVien", x => x.MaNhanVien);
                    table.ForeignKey(
                        name: "FK_NhanVien_KhoaPhong_MaKhoa",
                        column: x => x.MaKhoa,
                        principalTable: "KhoaPhong",
                        principalColumn: "MaKhoa",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LoThuoc",
                columns: table => new
                {
                    MaLo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaThuoc = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaKho = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SoLo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NgaySanXuat = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HanSuDung = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SoLuongNhap = table.Column<int>(type: "int", nullable: false),
                    SoLuongCon = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "ConHang"),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoThuoc", x => x.MaLo);
                    table.ForeignKey(
                        name: "FK_LoThuoc_Kho_MaKho",
                        column: x => x.MaKho,
                        principalTable: "Kho",
                        principalColumn: "MaKho",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LoThuoc_Thuoc_MaThuoc",
                        column: x => x.MaThuoc,
                        principalTable: "Thuoc",
                        principalColumn: "MaThuoc",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChanDoan",
                columns: table => new
                {
                    MaChanDoan = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaBenhNhan = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaNhanVien = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NgayChanDoan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenBenh = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    MaICD10 = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TrieuChung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChanDoanSoBo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    KetLuan = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChanDoan", x => x.MaChanDoan);
                    table.ForeignKey(
                        name: "FK_ChanDoan_BenhNhan_MaBenhNhan",
                        column: x => x.MaBenhNhan,
                        principalTable: "BenhNhan",
                        principalColumn: "MaBenhNhan",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChanDoan_NhanVien_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NhanVien",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PhieuNhap",
                columns: table => new
                {
                    MaPhieuNhap = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaNCC = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaKho = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NgayNhap = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(15,2)", nullable: false, defaultValue: 0m),
                    NhanVienNhap = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SoHoaDon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "DangNhap"),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuNhap", x => x.MaPhieuNhap);
                    table.ForeignKey(
                        name: "FK_PhieuNhap_Kho_MaKho",
                        column: x => x.MaKho,
                        principalTable: "Kho",
                        principalColumn: "MaKho",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhieuNhap_NhaCungCap_MaNCC",
                        column: x => x.MaNCC,
                        principalTable: "NhaCungCap",
                        principalColumn: "MaNCC",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhieuNhap_NhanVien_NhanVienNhap",
                        column: x => x.NhanVienNhap,
                        principalTable: "NhanVien",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaiKhoan",
                columns: table => new
                {
                    MaTaiKhoan = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaNhanVien = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "KichHoat"),
                    LanDangNhapCuoi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoan", x => x.MaTaiKhoan);
                    table.ForeignKey(
                        name: "FK_TaiKhoan_NhanVien_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NhanVien",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ThongBao",
                columns: table => new
                {
                    MaThongBao = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TieuDe = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiThongBao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DoUuTien = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Binh"),
                    NguoiNhan = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DaDoc = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    NgayHetHan = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongBao", x => x.MaThongBao);
                    table.ForeignKey(
                        name: "FK_ThongBao_NhanVien_NguoiNhan",
                        column: x => x.NguoiNhan,
                        principalTable: "NhanVien",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "DonThuoc",
                columns: table => new
                {
                    MaDonThuoc = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaChanDoan = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaBenhNhan = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaNhanVien = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NgayKeDon = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LoaiDon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(15,2)", nullable: false, defaultValue: 0m),
                    ChanDoanSoBo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    GhiChuBacSi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "ChoXuLy"),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonThuoc", x => x.MaDonThuoc);
                    table.ForeignKey(
                        name: "FK_DonThuoc_BenhNhan_MaBenhNhan",
                        column: x => x.MaBenhNhan,
                        principalTable: "BenhNhan",
                        principalColumn: "MaBenhNhan",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DonThuoc_ChanDoan_MaChanDoan",
                        column: x => x.MaChanDoan,
                        principalTable: "ChanDoan",
                        principalColumn: "MaChanDoan",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DonThuoc_NhanVien_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NhanVien",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietPhieuNhap",
                columns: table => new
                {
                    MaPhieuNhap = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaLo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SoLuongNhap = table.Column<int>(type: "int", nullable: false),
                    DonGiaNhap = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    ThanhTien = table.Column<decimal>(type: "decimal(15,2)", nullable: true, computedColumnSql: "[SoLuongNhap] * [DonGiaNhap]")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietPhieuNhap", x => new { x.MaPhieuNhap, x.MaLo });
                    table.ForeignKey(
                        name: "FK_ChiTietPhieuNhap_LoThuoc_MaLo",
                        column: x => x.MaLo,
                        principalTable: "LoThuoc",
                        principalColumn: "MaLo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChiTietPhieuNhap_PhieuNhap_MaPhieuNhap",
                        column: x => x.MaPhieuNhap,
                        principalTable: "PhieuNhap",
                        principalColumn: "MaPhieuNhap",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietDonThuoc",
                columns: table => new
                {
                    MaDonThuoc = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaThuoc = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    LieuDung = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SoNgayDung = table.Column<int>(type: "int", nullable: false),
                    CachDung = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    ThuocMaThuoc = table.Column<string>(type: "nvarchar(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietDonThuoc", x => new { x.MaDonThuoc, x.MaThuoc });
                    table.ForeignKey(
                        name: "FK_ChiTietDonThuoc_DonThuoc_MaDonThuoc",
                        column: x => x.MaDonThuoc,
                        principalTable: "DonThuoc",
                        principalColumn: "MaDonThuoc",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietDonThuoc_Thuoc_MaThuoc",
                        column: x => x.MaThuoc,
                        principalTable: "Thuoc",
                        principalColumn: "MaThuoc",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChiTietDonThuoc_Thuoc_ThuocMaThuoc",
                        column: x => x.ThuocMaThuoc,
                        principalTable: "Thuoc",
                        principalColumn: "MaThuoc");
                });

            migrationBuilder.CreateTable(
                name: "HoaDon",
                columns: table => new
                {
                    MaHoaDon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaBenhNhan = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaDonThuoc = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NgayTaoHoaDon = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(15,2)", nullable: false, defaultValue: 0m),
                    TienBHYTChiTra = table.Column<decimal>(type: "decimal(15,2)", nullable: false, defaultValue: 0m),
                    TienBenhNhanCanTra = table.Column<decimal>(type: "decimal(15,2)", nullable: false, defaultValue: 0m),
                    TienDaTra = table.Column<decimal>(type: "decimal(15,2)", nullable: false, defaultValue: 0m),
                    MaSoThue = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    HinhThucThanhToan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TrangThaiThanhToan = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "ChuaTra"),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaDon", x => x.MaHoaDon);
                    table.ForeignKey(
                        name: "FK_HoaDon_BenhNhan_MaBenhNhan",
                        column: x => x.MaBenhNhan,
                        principalTable: "BenhNhan",
                        principalColumn: "MaBenhNhan",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HoaDon_DonThuoc_MaDonThuoc",
                        column: x => x.MaDonThuoc,
                        principalTable: "DonThuoc",
                        principalColumn: "MaDonThuoc",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PhieuCapPhat",
                columns: table => new
                {
                    MaPhieuCap = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaDonThuoc = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaBenhNhan = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaKho = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NgayCap = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(15,2)", nullable: false, defaultValue: 0m),
                    NhanVienCap = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "DangXuLy"),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuCapPhat", x => x.MaPhieuCap);
                    table.ForeignKey(
                        name: "FK_PhieuCapPhat_BenhNhan_MaBenhNhan",
                        column: x => x.MaBenhNhan,
                        principalTable: "BenhNhan",
                        principalColumn: "MaBenhNhan",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhieuCapPhat_DonThuoc_MaDonThuoc",
                        column: x => x.MaDonThuoc,
                        principalTable: "DonThuoc",
                        principalColumn: "MaDonThuoc",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhieuCapPhat_Kho_MaKho",
                        column: x => x.MaKho,
                        principalTable: "Kho",
                        principalColumn: "MaKho",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhieuCapPhat_NhanVien_NhanVienCap",
                        column: x => x.NhanVienCap,
                        principalTable: "NhanVien",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PhieuThuTien",
                columns: table => new
                {
                    MaPhieuThu = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaHoaDon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NgayThu = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SoTienThu = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    HinhThucThanhToan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaGiaoDichNganHang = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NhanVienThu = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "DaXacNhan"),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuThuTien", x => x.MaPhieuThu);
                    table.ForeignKey(
                        name: "FK_PhieuThuTien_HoaDon_MaHoaDon",
                        column: x => x.MaHoaDon,
                        principalTable: "HoaDon",
                        principalColumn: "MaHoaDon",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhieuThuTien_NhanVien_NhanVienThu",
                        column: x => x.NhanVienThu,
                        principalTable: "NhanVien",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VietQR_GiaoDich",
                columns: table => new
                {
                    MaQR = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaHoaDon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SoTienYeuCau = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    SoTienNhan = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    NoiDungChuyenKhoan = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MaGiaoDichNganHang = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    QRCodeBase64 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayThanhToan = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DuLieuWebhook = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "ChoThanhToan"),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    NgayHetHan = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VietQR_GiaoDich", x => x.MaQR);
                    table.ForeignKey(
                        name: "FK_VietQR_GiaoDich_HoaDon_MaHoaDon",
                        column: x => x.MaHoaDon,
                        principalTable: "HoaDon",
                        principalColumn: "MaHoaDon",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietPhieuCap",
                columns: table => new
                {
                    MaPhieuCap = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaLo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SoLuongCap = table.Column<int>(type: "int", nullable: false),
                    DonGiaCap = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    ThanhTien = table.Column<decimal>(type: "decimal(15,2)", nullable: true, computedColumnSql: "[SoLuongCap] * [DonGiaCap]")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietPhieuCap", x => new { x.MaPhieuCap, x.MaLo });
                    table.ForeignKey(
                        name: "FK_ChiTietPhieuCap_LoThuoc_MaLo",
                        column: x => x.MaLo,
                        principalTable: "LoThuoc",
                        principalColumn: "MaLo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChiTietPhieuCap_PhieuCapPhat_MaPhieuCap",
                        column: x => x.MaPhieuCap,
                        principalTable: "PhieuCapPhat",
                        principalColumn: "MaPhieuCap",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_sdt",
                table: "BenhNhan",
                column: "SoDienThoai");

            migrationBuilder.CreateIndex(
                name: "idx_tenbn",
                table: "BenhNhan",
                column: "TenBenhNhan");

            migrationBuilder.CreateIndex(
                name: "idx_tencauhinh",
                table: "CauHinhHeThong",
                column: "TenCauHinh",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_benhnhan",
                table: "ChanDoan",
                column: "MaBenhNhan");

            migrationBuilder.CreateIndex(
                name: "idx_ngaychandoan",
                table: "ChanDoan",
                column: "NgayChanDoan");

            migrationBuilder.CreateIndex(
                name: "IX_ChanDoan_MaNhanVien",
                table: "ChanDoan",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonThuoc_MaThuoc",
                table: "ChiTietDonThuoc",
                column: "MaThuoc");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonThuoc_ThuocMaThuoc",
                table: "ChiTietDonThuoc",
                column: "ThuocMaThuoc");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietPhieuCap_MaLo",
                table: "ChiTietPhieuCap",
                column: "MaLo");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietPhieuNhap_MaLo",
                table: "ChiTietPhieuNhap",
                column: "MaLo");

            migrationBuilder.CreateIndex(
                name: "idx_loaidon",
                table: "DonThuoc",
                column: "LoaiDon");

            migrationBuilder.CreateIndex(
                name: "idx_ngaykedon",
                table: "DonThuoc",
                column: "NgayKeDon");

            migrationBuilder.CreateIndex(
                name: "IX_DonThuoc_MaBenhNhan",
                table: "DonThuoc",
                column: "MaBenhNhan");

            migrationBuilder.CreateIndex(
                name: "IX_DonThuoc_MaChanDoan",
                table: "DonThuoc",
                column: "MaChanDoan");

            migrationBuilder.CreateIndex(
                name: "IX_DonThuoc_MaNhanVien",
                table: "DonThuoc",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "idx_ngaytaohoadon",
                table: "HoaDon",
                column: "NgayTaoHoaDon");

            migrationBuilder.CreateIndex(
                name: "idx_trangthai",
                table: "HoaDon",
                column: "TrangThaiThanhToan");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_MaBenhNhan",
                table: "HoaDon",
                column: "MaBenhNhan");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_MaDonThuoc",
                table: "HoaDon",
                column: "MaDonThuoc");

            migrationBuilder.CreateIndex(
                name: "idx_hansudung",
                table: "LoThuoc",
                column: "HanSuDung");

            migrationBuilder.CreateIndex(
                name: "idx_kho",
                table: "LoThuoc",
                column: "MaKho");

            migrationBuilder.CreateIndex(
                name: "idx_thuoc",
                table: "LoThuoc",
                column: "MaThuoc");

            migrationBuilder.CreateIndex(
                name: "uk_thuoc_kho_solo",
                table: "LoThuoc",
                columns: new[] { "MaThuoc", "MaKho", "SoLo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_tenncc",
                table: "NhaCungCap",
                column: "TenNCC");

            migrationBuilder.CreateIndex(
                name: "idx_chucvu",
                table: "NhanVien",
                column: "ChucVu");

            migrationBuilder.CreateIndex(
                name: "idx_chuyenkhoa",
                table: "NhanVien",
                column: "ChuyenKhoa");

            migrationBuilder.CreateIndex(
                name: "idx_tennv",
                table: "NhanVien",
                column: "TenNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_NhanVien_MaKhoa",
                table: "NhanVien",
                column: "MaKhoa");

            migrationBuilder.CreateIndex(
                name: "idx_donthuoc",
                table: "PhieuCapPhat",
                column: "MaDonThuoc");

            migrationBuilder.CreateIndex(
                name: "idx_ngaycap",
                table: "PhieuCapPhat",
                column: "NgayCap");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuCapPhat_MaBenhNhan",
                table: "PhieuCapPhat",
                column: "MaBenhNhan");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuCapPhat_MaKho",
                table: "PhieuCapPhat",
                column: "MaKho");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuCapPhat_NhanVienCap",
                table: "PhieuCapPhat",
                column: "NhanVienCap");

            migrationBuilder.CreateIndex(
                name: "idx_ncc",
                table: "PhieuNhap",
                column: "MaNCC");

            migrationBuilder.CreateIndex(
                name: "idx_ngaynhap",
                table: "PhieuNhap",
                column: "NgayNhap");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuNhap_MaKho",
                table: "PhieuNhap",
                column: "MaKho");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuNhap_NhanVienNhap",
                table: "PhieuNhap",
                column: "NhanVienNhap");

            migrationBuilder.CreateIndex(
                name: "idx_hoadon",
                table: "PhieuThuTien",
                column: "MaHoaDon");

            migrationBuilder.CreateIndex(
                name: "idx_ngaythu",
                table: "PhieuThuTien",
                column: "NgayThu");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuThuTien_NhanVienThu",
                table: "PhieuThuTien",
                column: "NhanVienThu");

            migrationBuilder.CreateIndex(
                name: "idx_username",
                table: "TaiKhoan",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoan_MaNhanVien",
                table: "TaiKhoan",
                column: "MaNhanVien",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_hethan",
                table: "TheBHYT",
                column: "NgayHetHan");

            migrationBuilder.CreateIndex(
                name: "idx_sothe",
                table: "TheBHYT",
                column: "SoTheBHYT",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TheBHYT_MaBenhNhan",
                table: "TheBHYT",
                column: "MaBenhNhan");

            migrationBuilder.CreateIndex(
                name: "idx_dadoc",
                table: "ThongBao",
                column: "DaDoc");

            migrationBuilder.CreateIndex(
                name: "idx_ngaytao",
                table: "ThongBao",
                column: "NgayTao");

            migrationBuilder.CreateIndex(
                name: "idx_uutien",
                table: "ThongBao",
                column: "DoUuTien");

            migrationBuilder.CreateIndex(
                name: "IX_ThongBao_NguoiNhan",
                table: "ThongBao",
                column: "NguoiNhan");

            migrationBuilder.CreateIndex(
                name: "idx_bhyt",
                table: "Thuoc",
                column: "LaThuocBHYT");

            migrationBuilder.CreateIndex(
                name: "idx_hoatchat",
                table: "Thuoc",
                column: "HoatChat");

            migrationBuilder.CreateIndex(
                name: "idx_nhomthuoc",
                table: "Thuoc",
                column: "NhomThuoc");

            migrationBuilder.CreateIndex(
                name: "idx_tenthuoc",
                table: "Thuoc",
                column: "TenThuoc");

            migrationBuilder.CreateIndex(
                name: "idx_hoadon",
                table: "VietQR_GiaoDich",
                column: "MaHoaDon");

            migrationBuilder.CreateIndex(
                name: "idx_noidung",
                table: "VietQR_GiaoDich",
                column: "NoiDungChuyenKhoan");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CauHinh_VietQR");

            migrationBuilder.DropTable(
                name: "CauHinhHeThong");

            migrationBuilder.DropTable(
                name: "ChiTietDonThuoc");

            migrationBuilder.DropTable(
                name: "ChiTietPhieuCap");

            migrationBuilder.DropTable(
                name: "ChiTietPhieuNhap");

            migrationBuilder.DropTable(
                name: "PhieuThuTien");

            migrationBuilder.DropTable(
                name: "TaiKhoan");

            migrationBuilder.DropTable(
                name: "TheBHYT");

            migrationBuilder.DropTable(
                name: "ThongBao");

            migrationBuilder.DropTable(
                name: "VietQR_GiaoDich");

            migrationBuilder.DropTable(
                name: "PhieuCapPhat");

            migrationBuilder.DropTable(
                name: "LoThuoc");

            migrationBuilder.DropTable(
                name: "PhieuNhap");

            migrationBuilder.DropTable(
                name: "HoaDon");

            migrationBuilder.DropTable(
                name: "Thuoc");

            migrationBuilder.DropTable(
                name: "Kho");

            migrationBuilder.DropTable(
                name: "NhaCungCap");

            migrationBuilder.DropTable(
                name: "DonThuoc");

            migrationBuilder.DropTable(
                name: "ChanDoan");

            migrationBuilder.DropTable(
                name: "BenhNhan");

            migrationBuilder.DropTable(
                name: "NhanVien");

            migrationBuilder.DropTable(
                name: "KhoaPhong");
        }
    }
}
