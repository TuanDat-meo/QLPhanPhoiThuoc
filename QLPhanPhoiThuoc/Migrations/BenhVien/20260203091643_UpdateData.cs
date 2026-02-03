using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLPhanPhoiThuoc.Migrations.BenhVien
{
    /// <inheritdoc />
    public partial class UpdateData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaiKhoan_NhanVien_MaNhanVien",
                table: "TaiKhoan");

            migrationBuilder.DropIndex(
                name: "IX_TaiKhoan_MaNhanVien",
                table: "TaiKhoan");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NgayTao",
                table: "TaiKhoan",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "MaNhanVien",
                table: "TaiKhoan",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayCapNhat",
                table: "TaiKhoan",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaTaiKhoan",
                table: "NhanVien",
                type: "nvarchar(20)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaTaiKhoan",
                table: "BenhNhan",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NhanVien_MaTaiKhoan",
                table: "NhanVien",
                column: "MaTaiKhoan",
                unique: true,
                filter: "[MaTaiKhoan] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "idx_cccd",
                table: "BenhNhan",
                column: "CCCD");

            migrationBuilder.CreateIndex(
                name: "idx_email",
                table: "BenhNhan",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "idx_mataikhoan",
                table: "BenhNhan",
                column: "MaTaiKhoan",
                unique: true,
                filter: "[MaTaiKhoan] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_BenhNhan_TaiKhoan_MaTaiKhoan",
                table: "BenhNhan",
                column: "MaTaiKhoan",
                principalTable: "TaiKhoan",
                principalColumn: "MaTaiKhoan",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NhanVien_TaiKhoan_MaTaiKhoan",
                table: "NhanVien",
                column: "MaTaiKhoan",
                principalTable: "TaiKhoan",
                principalColumn: "MaTaiKhoan",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BenhNhan_TaiKhoan_MaTaiKhoan",
                table: "BenhNhan");

            migrationBuilder.DropForeignKey(
                name: "FK_NhanVien_TaiKhoan_MaTaiKhoan",
                table: "NhanVien");

            migrationBuilder.DropIndex(
                name: "IX_NhanVien_MaTaiKhoan",
                table: "NhanVien");

            migrationBuilder.DropIndex(
                name: "idx_cccd",
                table: "BenhNhan");

            migrationBuilder.DropIndex(
                name: "idx_email",
                table: "BenhNhan");

            migrationBuilder.DropIndex(
                name: "idx_mataikhoan",
                table: "BenhNhan");

            migrationBuilder.DropColumn(
                name: "NgayCapNhat",
                table: "TaiKhoan");

            migrationBuilder.DropColumn(
                name: "MaTaiKhoan",
                table: "NhanVien");

            migrationBuilder.DropColumn(
                name: "MaTaiKhoan",
                table: "BenhNhan");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NgayTao",
                table: "TaiKhoan",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "MaNhanVien",
                table: "TaiKhoan",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoan_MaNhanVien",
                table: "TaiKhoan",
                column: "MaNhanVien",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TaiKhoan_NhanVien_MaNhanVien",
                table: "TaiKhoan",
                column: "MaNhanVien",
                principalTable: "NhanVien",
                principalColumn: "MaNhanVien",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
