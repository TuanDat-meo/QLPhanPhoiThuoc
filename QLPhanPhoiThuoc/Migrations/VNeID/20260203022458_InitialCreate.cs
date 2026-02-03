using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLPhanPhoiThuoc.Migrations.VNeID
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CauHinhTichHop",
                columns: table => new
                {
                    MaCauHinh = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenHeThong = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    APIKey = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    APISecret = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IPWhitelist = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "Danh sách IP được phép, cách nhau bởi dấu phẩy"),
                    SoLanTraCuuToiDa = table.Column<int>(type: "int", nullable: false, defaultValue: 1000, comment: "Giới hạn số lần tra cứu/ngày"),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "KichHoat"),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CauHinhTichHop", x => x.MaCauHinh);
                });

            migrationBuilder.CreateTable(
                name: "CongDan",
                columns: table => new
                {
                    SoDinhDanh = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false, comment: "Số CCCD/CMND"),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GioiTinh = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    QueQuan = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NoiThuongTru = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    DiaChiHienTai = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    DanToc = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TonGiao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    QuocTich = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Việt Nam"),
                    NgayCap = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NoiCap = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NgayHetHan = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AnhChanDung = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "Base64 hoặc URL ảnh"),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "HoatDong"),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CongDan", x => x.SoDinhDanh);
                });

            migrationBuilder.CreateTable(
                name: "LichSuTraCuu",
                columns: table => new
                {
                    MaTraCuu = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SoDinhDanh = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    LoaiTraCuu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    HeThongTraCuu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "Tên hệ thống yêu cầu"),
                    IPAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    NgayGioTraCuu = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KetQua = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DuLieuTraVe = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "JSON response"),
                    ThoiGianXuLy = table.Column<int>(type: "int", nullable: true, comment: "Milliseconds"),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichSuTraCuu", x => x.MaTraCuu);
                    table.ForeignKey(
                        name: "FK_LichSuTraCuu_CongDan_SoDinhDanh",
                        column: x => x.SoDinhDanh,
                        principalTable: "CongDan",
                        principalColumn: "SoDinhDanh",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TheBHYT_Mock",
                columns: table => new
                {
                    MaThe = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SoDinhDanh = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    SoTheBHYT = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false, comment: "Mã thẻ BHYT 15 ký tự"),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayHetHan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MucHuong = table.Column<decimal>(type: "decimal(5,2)", nullable: false, comment: "80, 95, 100"),
                    NoiDKKCB = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false, comment: "Nơi đăng ký khám chữa bệnh ban đầu"),
                    MaNoiDKKCB = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, comment: "Mã BV đăng ký"),
                    DiaChi5Nam = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false, comment: "Nơi cư trú 5 năm liên tục"),
                    MaKhuVuc = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false, comment: "K1, K2, K3"),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "ConHan"),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TheBHYT_Mock", x => x.MaThe);
                    table.ForeignKey(
                        name: "FK_TheBHYT_Mock_CongDan_SoDinhDanh",
                        column: x => x.SoDinhDanh,
                        principalTable: "CongDan",
                        principalColumn: "SoDinhDanh",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "idx_apikey",
                table: "CauHinhTichHop",
                column: "APIKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_hoten",
                table: "CongDan",
                column: "HoTen");

            migrationBuilder.CreateIndex(
                name: "idx_ngaysinh",
                table: "CongDan",
                column: "NgaySinh");

            migrationBuilder.CreateIndex(
                name: "idx_hethong",
                table: "LichSuTraCuu",
                column: "HeThongTraCuu");

            migrationBuilder.CreateIndex(
                name: "idx_ngaygio",
                table: "LichSuTraCuu",
                column: "NgayGioTraCuu");

            migrationBuilder.CreateIndex(
                name: "idx_sodinhhanh",
                table: "LichSuTraCuu",
                column: "SoDinhDanh");

            migrationBuilder.CreateIndex(
                name: "idx_hethan",
                table: "TheBHYT_Mock",
                column: "NgayHetHan");

            migrationBuilder.CreateIndex(
                name: "idx_sodinhhanh",
                table: "TheBHYT_Mock",
                column: "SoDinhDanh");

            migrationBuilder.CreateIndex(
                name: "idx_sothe",
                table: "TheBHYT_Mock",
                column: "SoTheBHYT",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CauHinhTichHop");

            migrationBuilder.DropTable(
                name: "LichSuTraCuu");

            migrationBuilder.DropTable(
                name: "TheBHYT_Mock");

            migrationBuilder.DropTable(
                name: "CongDan");
        }
    }
}
