using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLPhanPhoiThuoc.Migrations.BenhVien
{
    /// <inheritdoc />
    public partial class Add_Column_Avatar_TaiKhoan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                table: "TaiKhoan",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "TaiKhoan");
        }
    }
}
