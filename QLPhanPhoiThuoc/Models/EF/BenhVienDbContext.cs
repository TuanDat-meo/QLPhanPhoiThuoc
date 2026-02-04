using Microsoft.EntityFrameworkCore;
using QLPhanPhoiThuoc.Models;
using QLPhanPhoiThuoc.Models.Entities;
using System;

namespace QLPhanPhoiThuoc.Models.EF
{
    public class BenhVienDbContext : DbContext
    {
        public BenhVienDbContext(DbContextOptions<BenhVienDbContext> options)
            : base(options)
        {
        }

        // NHÓM A: DANH MỤC CƠ SỞ
        public DbSet<Thuoc> Thuocs { get; set; }
        public DbSet<NhaCungCap> NhaCungCaps { get; set; }
        public DbSet<Kho> Khos { get; set; }
        public DbSet<KhoaPhong> KhoaPhongs { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }
        public DbSet<TaiKhoan> TaiKhoans { get; set; }

        // NHÓM B: QUẢN LÝ KHO THUỐC
        public DbSet<LoThuoc> LoThuocs { get; set; }
        public DbSet<PhieuNhap> PhieuNhaps { get; set; }
        public DbSet<ChiTietPhieuNhap> ChiTietPhieuNhaps { get; set; }

        // NHÓM C: BỆNH NHÂN
        public DbSet<BenhNhan> BenhNhans { get; set; }
        public DbSet<TheBHYT> TheBHYTs { get; set; }

        // NHÓM D: KHÁM BỆNH & ĐƠN THUỐC
        public DbSet<ChanDoan> ChanDoans { get; set; }
        public DbSet<DonThuoc> DonThuocs { get; set; }
        public DbSet<ChiTietDonThuoc> ChiTietDonThuocs { get; set; }

        // NHÓM E: XUẤT THUỐC
        public DbSet<PhieuCapPhat> PhieuCapPhats { get; set; }
        public DbSet<ChiTietPhieuCap> ChiTietPhieuCaps { get; set; }

        // NHÓM F: HÓA ĐƠN & THANH TOÁN
        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<PhieuThuTien> PhieuThuTiens { get; set; }
        public DbSet<VietQRGiaoDich> VietQRGiaoDichs { get; set; }
        public DbSet<CauHinhVietQR> CauHinhVietQRs { get; set; }

        // NHÓM G: HỆ THỐNG
        public DbSet<CauHinhHeThong> CauHinhHeThongs { get; set; }
        public DbSet<ThongBao> ThongBaos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== CẤU HÌNH CHO CÁC ENTITY =====

            // Thuoc
            modelBuilder.Entity<Thuoc>(entity =>
            {
                entity.ToTable("Thuoc");
                entity.HasKey(e => e.MaThuoc);
                entity.Property(e => e.MaThuoc).HasMaxLength(20);
                entity.Property(e => e.TenThuoc).HasMaxLength(200).IsRequired();
                entity.Property(e => e.HoatChat).HasMaxLength(200);
                entity.Property(e => e.DonViTinh).HasMaxLength(50).IsRequired();
                entity.Property(e => e.HamLuong).HasMaxLength(100);
                entity.Property(e => e.DangBaoChe).HasMaxLength(100);
                entity.Property(e => e.DuongDung).HasMaxLength(100);
                entity.Property(e => e.NhaSanXuat).HasMaxLength(200);
                entity.Property(e => e.NhomThuoc).HasMaxLength(100);
                entity.Property(e => e.GiaNhap).HasColumnType("decimal(15,2)").HasDefaultValue(0);
                entity.Property(e => e.GiaXuat).HasColumnType("decimal(15,2)").HasDefaultValue(0);
                entity.Property(e => e.TonKhoToiThieu).HasDefaultValue(10);
                entity.Property(e => e.LaThuocBHYT).HasMaxLength(3).HasDefaultValue("No");
                entity.Property(e => e.TyLeBHYTChiTra).HasColumnType("decimal(5,2)").HasDefaultValue(0);
                entity.Property(e => e.MoTa).HasMaxLength(500);
                entity.Property(e => e.TrangThai).HasMaxLength(20).HasDefaultValue("KichHoat");
                entity.Property(e => e.NgayTao).HasDefaultValueSql("GETDATE()");

                entity.HasIndex(e => e.TenThuoc).HasDatabaseName("idx_tenthuoc");
                entity.HasIndex(e => e.HoatChat).HasDatabaseName("idx_hoatchat");
                entity.HasIndex(e => e.NhomThuoc).HasDatabaseName("idx_nhomthuoc");
                entity.HasIndex(e => e.LaThuocBHYT).HasDatabaseName("idx_bhyt");
            });

            // NhaCungCap
            modelBuilder.Entity<NhaCungCap>(entity =>
            {
                entity.ToTable("NhaCungCap");
                entity.HasKey(e => e.MaNCC);
                entity.Property(e => e.MaNCC).HasMaxLength(20);
                entity.Property(e => e.TenNCC).HasMaxLength(200).IsRequired();
                entity.Property(e => e.DiaChi).HasMaxLength(300);
                entity.Property(e => e.SoDienThoai).HasMaxLength(15);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.MaSoThue).HasMaxLength(20);
                entity.Property(e => e.TrangThai).HasMaxLength(20).HasDefaultValue("HoatDong");
                entity.Property(e => e.NgayTao).HasDefaultValueSql("GETDATE()");

                entity.HasIndex(e => e.TenNCC).HasDatabaseName("idx_tenncc");
            });

            // Kho
            modelBuilder.Entity<Kho>(entity =>
            {
                entity.ToTable("Kho");
                entity.HasKey(e => e.MaKho);
                entity.Property(e => e.MaKho).HasMaxLength(20);
                entity.Property(e => e.TenKho).HasMaxLength(100).IsRequired();
                entity.Property(e => e.LoaiKho).HasMaxLength(20).IsRequired();
                entity.Property(e => e.DiaDiem).HasMaxLength(200);
                entity.Property(e => e.GhiChu).HasMaxLength(300);
                entity.Property(e => e.TrangThai).HasMaxLength(20).HasDefaultValue("HoatDong");
                entity.Property(e => e.NgayTao).HasDefaultValueSql("GETDATE()");
            });

            // KhoaPhong
            modelBuilder.Entity<KhoaPhong>(entity =>
            {
                entity.ToTable("KhoaPhong");
                entity.HasKey(e => e.MaKhoa);
                entity.Property(e => e.MaKhoa).HasMaxLength(20);
                entity.Property(e => e.TenKhoa).HasMaxLength(100).IsRequired();
                entity.Property(e => e.TruongKhoa).HasMaxLength(100);
                entity.Property(e => e.SoDienThoai).HasMaxLength(15);
                entity.Property(e => e.TrangThai).HasMaxLength(20).HasDefaultValue("HoatDong");
                entity.Property(e => e.NgayTao).HasDefaultValueSql("GETDATE()");
            });

            // NhanVien
            modelBuilder.Entity<NhanVien>(entity =>
            {
                entity.ToTable("NhanVien");
                entity.HasKey(e => e.MaNhanVien);
                entity.Property(e => e.MaNhanVien).HasMaxLength(20);
                entity.Property(e => e.TenNhanVien).HasMaxLength(100).IsRequired();
                entity.Property(e => e.ChucVu).HasMaxLength(50);
                entity.Property(e => e.ChuyenKhoa).HasMaxLength(100);
                entity.Property(e => e.BangCap).HasMaxLength(200);
                entity.Property(e => e.MaKhoa).HasMaxLength(20);
                entity.Property(e => e.SoDienThoai).HasMaxLength(15);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.CCCD).HasMaxLength(12);
                entity.Property(e => e.GioiTinh).HasMaxLength(10);
                entity.Property(e => e.DiaChi).HasMaxLength(300);
                entity.Property(e => e.TrangThai).HasMaxLength(20).HasDefaultValue("DangLamViec");
                entity.Property(e => e.NgayTao).HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.KhoaPhong)
                    .WithMany(k => k.NhanViens)
                    .HasForeignKey(e => e.MaKhoa)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.TenNhanVien).HasDatabaseName("idx_tennv");
                entity.HasIndex(e => e.ChucVu).HasDatabaseName("idx_chucvu");
                entity.HasIndex(e => e.ChuyenKhoa).HasDatabaseName("idx_chuyenkhoa");
            });


            // TaiKhoan
            modelBuilder.Entity<TaiKhoan>(entity =>
            {
                entity.ToTable("TaiKhoan");
                entity.HasKey(e => e.MaTaiKhoan);

                entity.Property(e => e.MaTaiKhoan).HasMaxLength(20);
                entity.Property(e => e.MaNhanVien).HasMaxLength(20);  // THÊM CẤU HÌNH CHO CỘT NÀY
                entity.Property(e => e.Username).HasMaxLength(50).IsRequired();
                entity.Property(e => e.PasswordHash).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Role).HasMaxLength(20).IsRequired();
                entity.Property(e => e.TrangThai).HasMaxLength(20).HasDefaultValue("KichHoat");
                entity.Property(e => e.LanDangNhapCuoi);  // THÊM CẤU HÌNH CHO CỘT NÀY
                entity.Property(e => e.NgayTao).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.NgayCapNhat);
                entity.Property(e => e.Avatar).HasMaxLength(500).IsRequired(false); // Cho phép null
                // Cấu hình quan hệ one-to-one với NhanVien
                entity.HasOne(t => t.NhanVien)
                    .WithOne(n => n.TaiKhoan)
                    .HasForeignKey<NhanVien>(n => n.MaTaiKhoan)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired(false);

                // Cấu hình quan hệ one-to-one với BenhNhan
                entity.HasOne(t => t.BenhNhan)
                    .WithOne(b => b.TaiKhoan)
                    .HasForeignKey<BenhNhan>(b => b.MaTaiKhoan)
                    .HasForeignKey<BenhNhan>(b => b.MaTaiKhoan)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired(false);

                entity.HasIndex(e => e.Username).HasDatabaseName("idx_username").IsUnique();
            });

            // LoThuoc
            modelBuilder.Entity<LoThuoc>(entity =>
            {
                entity.ToTable("LoThuoc");
                entity.HasKey(e => e.MaLo);
                entity.Property(e => e.MaLo).HasMaxLength(20);
                entity.Property(e => e.MaThuoc).HasMaxLength(20).IsRequired();
                entity.Property(e => e.MaKho).HasMaxLength(20).IsRequired();
                entity.Property(e => e.SoLo).HasMaxLength(50).IsRequired();
                entity.Property(e => e.TrangThai).HasMaxLength(20).HasDefaultValue("ConHang");
                entity.Property(e => e.NgayTao).HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.Thuoc)
                    .WithMany(t => t.LoThuocs)
                    .HasForeignKey(e => e.MaThuoc)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Kho)
                    .WithMany(k => k.LoThuocs)
                    .HasForeignKey(e => e.MaKho)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.MaThuoc).HasDatabaseName("idx_thuoc");
                entity.HasIndex(e => e.MaKho).HasDatabaseName("idx_kho");
                entity.HasIndex(e => e.HanSuDung).HasDatabaseName("idx_hansudung");
                entity.HasIndex(e => new { e.MaThuoc, e.MaKho, e.SoLo }).HasDatabaseName("uk_thuoc_kho_solo").IsUnique();
            });

            // PhieuNhap
            modelBuilder.Entity<PhieuNhap>(entity =>
            {
                entity.ToTable("PhieuNhap");
                entity.HasKey(e => e.MaPhieuNhap);
                entity.Property(e => e.MaPhieuNhap).HasMaxLength(20);
                entity.Property(e => e.MaNCC).HasMaxLength(20).IsRequired();
                entity.Property(e => e.MaKho).HasMaxLength(20).IsRequired();
                entity.Property(e => e.NgayNhap).IsRequired();
                entity.Property(e => e.TongTien).HasColumnType("decimal(15,2)").HasDefaultValue(0);
                entity.Property(e => e.NhanVienNhap).HasMaxLength(20).IsRequired();
                entity.Property(e => e.SoHoaDon).HasMaxLength(50);
                entity.Property(e => e.TrangThai).HasMaxLength(20).HasDefaultValue("DangNhap");
                entity.Property(e => e.GhiChu).HasMaxLength(500);
                entity.Property(e => e.NgayTao).HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.NhaCungCap)
                    .WithMany(n => n.PhieuNhaps)
                    .HasForeignKey(e => e.MaNCC)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Kho)
                    .WithMany(k => k.PhieuNhaps)
                    .HasForeignKey(e => e.MaKho)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.NhanVien)
                    .WithMany()
                    .HasForeignKey(e => e.NhanVienNhap)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.NgayNhap).HasDatabaseName("idx_ngaynhap");
                entity.HasIndex(e => e.MaNCC).HasDatabaseName("idx_ncc");
            });

            // ChiTietPhieuNhap
            modelBuilder.Entity<ChiTietPhieuNhap>(entity =>
            {
                entity.ToTable("ChiTietPhieuNhap");
                entity.HasKey(e => new { e.MaPhieuNhap, e.MaLo });
                entity.Property(e => e.MaPhieuNhap).HasMaxLength(20);
                entity.Property(e => e.MaLo).HasMaxLength(20);
                entity.Property(e => e.SoLuongNhap).IsRequired();
                entity.Property(e => e.DonGiaNhap).HasColumnType("decimal(15,2)").IsRequired();
                entity.Property(e => e.ThanhTien).HasColumnType("decimal(15,2)").HasComputedColumnSql("[SoLuongNhap] * [DonGiaNhap]");

                entity.HasOne(e => e.PhieuNhap)
                    .WithMany(p => p.ChiTietPhieuNhaps)
                    .HasForeignKey(e => e.MaPhieuNhap)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.LoThuoc)
                    .WithMany()
                    .HasForeignKey(e => e.MaLo)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // BenhNhan
            modelBuilder.Entity<BenhNhan>(entity =>
            {
                entity.ToTable("BenhNhan");
                entity.HasKey(e => e.MaBenhNhan);
                entity.Property(e => e.MaBenhNhan).HasMaxLength(20);
                entity.Property(e => e.TenBenhNhan).HasMaxLength(100).IsRequired();
                entity.Property(e => e.GioiTinh).HasMaxLength(10);
                entity.Property(e => e.DiaChi).HasMaxLength(300);
                entity.Property(e => e.SoDienThoai).HasMaxLength(15);
                entity.Property(e => e.CCCD).HasMaxLength(12);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.NhomMau).HasMaxLength(5);
                entity.Property(e => e.NgheNghiep).HasMaxLength(100);
                entity.Property(e => e.CanNang).HasColumnType("decimal(5,2)");
                entity.Property(e => e.ChieuCao).HasColumnType("decimal(5,2)");
                entity.Property(e => e.TienSuDiUng).HasColumnType("nvarchar(max)");

                // ✅ THÊM: MaTaiKhoan property (FK to TaiKhoan)
                entity.Property(e => e.MaTaiKhoan).HasMaxLength(20);

                entity.Property(e => e.LoaiBenhNhan).HasMaxLength(20).HasDefaultValue("NgoaiTru");
                entity.Property(e => e.TrangThai).HasMaxLength(20).HasDefaultValue("HoatDong");
                entity.Property(e => e.NgayTao).HasDefaultValueSql("GETDATE()");

                // Indexes
                entity.HasIndex(e => e.TenBenhNhan).HasDatabaseName("idx_tenbn");
                entity.HasIndex(e => e.SoDienThoai).HasDatabaseName("idx_sdt");
                entity.HasIndex(e => e.CCCD).HasDatabaseName("idx_cccd");
                entity.HasIndex(e => e.Email).HasDatabaseName("idx_email");

                // ✅ THÊM: Index cho MaTaiKhoan
                entity.HasIndex(e => e.MaTaiKhoan).HasDatabaseName("idx_mataikhoan");
            });

            // TheBHYT
            modelBuilder.Entity<TheBHYT>(entity =>
            {
                entity.ToTable("TheBHYT");
                entity.HasKey(e => e.MaThe);
                entity.Property(e => e.MaThe).HasMaxLength(20);
                entity.Property(e => e.MaBenhNhan).HasMaxLength(20).IsRequired();
                entity.Property(e => e.SoTheBHYT).HasMaxLength(15).IsRequired();
                entity.Property(e => e.MucHuong).HasColumnType("decimal(5,2)").IsRequired();
                entity.Property(e => e.NoiDangKyKCB).HasMaxLength(200);
                entity.Property(e => e.DiaChi5Nam).HasMaxLength(300);
                entity.Property(e => e.TrangThai).HasMaxLength(20).HasDefaultValue("ConHan");
                entity.Property(e => e.NgayTao).HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.BenhNhan)
                    .WithMany(b => b.TheBHYTs)
                    .HasForeignKey(e => e.MaBenhNhan)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.SoTheBHYT).HasDatabaseName("idx_sothe").IsUnique();
                entity.HasIndex(e => e.NgayHetHan).HasDatabaseName("idx_hethan");
            });

            // ChanDoan
            modelBuilder.Entity<ChanDoan>(entity =>
            {
                entity.ToTable("ChanDoan");
                entity.HasKey(e => e.MaChanDoan);
                entity.Property(e => e.MaChanDoan).HasMaxLength(20);
                entity.Property(e => e.MaBenhNhan).HasMaxLength(20).IsRequired();
                entity.Property(e => e.MaNhanVien).HasMaxLength(20).IsRequired();
                entity.Property(e => e.TenBenh).HasMaxLength(300).IsRequired();
                entity.Property(e => e.MaICD10).HasMaxLength(10);
                entity.Property(e => e.TrieuChung).HasColumnType("nvarchar(max)");
                entity.Property(e => e.ChanDoanSoBo).HasMaxLength(500);
                entity.Property(e => e.KetLuan).HasMaxLength(500);
                entity.Property(e => e.GhiChu).HasMaxLength(500);
                entity.Property(e => e.NgayTao).HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.BenhNhan)
                    .WithMany(b => b.ChanDoans)
                    .HasForeignKey(e => e.MaBenhNhan)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.NhanVien)
                    .WithMany()
                    .HasForeignKey(e => e.MaNhanVien)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.NgayChanDoan).HasDatabaseName("idx_ngaychandoan");
                entity.HasIndex(e => e.MaBenhNhan).HasDatabaseName("idx_benhnhan");
            });

            // DonThuoc
            modelBuilder.Entity<DonThuoc>(entity =>
            {
                entity.ToTable("DonThuoc");
                entity.HasKey(e => e.MaDonThuoc);
                entity.Property(e => e.MaDonThuoc).HasMaxLength(20);
                entity.Property(e => e.MaChanDoan).HasMaxLength(20).IsRequired();
                entity.Property(e => e.MaBenhNhan).HasMaxLength(20).IsRequired();
                entity.Property(e => e.MaNhanVien).HasMaxLength(20).IsRequired();
                entity.Property(e => e.LoaiDon).HasMaxLength(20).IsRequired();
                entity.Property(e => e.TongTien).HasColumnType("decimal(15,2)").HasDefaultValue(0);
                entity.Property(e => e.ChanDoanSoBo).HasMaxLength(500);
                entity.Property(e => e.GhiChuBacSi).HasMaxLength(500);
                entity.Property(e => e.TrangThai).HasMaxLength(20).HasDefaultValue("ChoXuLy");
                entity.Property(e => e.NgayTao).HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.ChanDoan)
                    .WithMany(c => c.DonThuocs)
                    .HasForeignKey(e => e.MaChanDoan)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.BenhNhan)
                    .WithMany(b => b.DonThuocs)
                    .HasForeignKey(e => e.MaBenhNhan)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.NhanVien)
                    .WithMany()
                    .HasForeignKey(e => e.MaNhanVien)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.NgayKeDon).HasDatabaseName("idx_ngaykedon");
                entity.HasIndex(e => e.LoaiDon).HasDatabaseName("idx_loaidon");
            });

            // ChiTietDonThuoc
            modelBuilder.Entity<ChiTietDonThuoc>(entity =>
            {
                entity.ToTable("ChiTietDonThuoc");
                entity.HasKey(e => new { e.MaDonThuoc, e.MaThuoc });
                entity.Property(e => e.MaDonThuoc).HasMaxLength(20);
                entity.Property(e => e.MaThuoc).HasMaxLength(20);
                entity.Property(e => e.SoLuong).IsRequired();
                entity.Property(e => e.LieuDung).HasMaxLength(200);
                entity.Property(e => e.SoNgayDung).IsRequired();
                entity.Property(e => e.CachDung).HasMaxLength(200);
                entity.Property(e => e.GhiChu).HasMaxLength(300);

                entity.HasOne(e => e.DonThuoc)
                    .WithMany(d => d.ChiTietDonThuocs)
                    .HasForeignKey(e => e.MaDonThuoc)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Thuoc)
                    .WithMany()
                    .HasForeignKey(e => e.MaThuoc)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // PhieuCapPhat
            modelBuilder.Entity<PhieuCapPhat>(entity =>
            {
                entity.ToTable("PhieuCapPhat");
                entity.HasKey(e => e.MaPhieuCap);
                entity.Property(e => e.MaPhieuCap).HasMaxLength(20);
                entity.Property(e => e.MaDonThuoc).HasMaxLength(20).IsRequired();
                entity.Property(e => e.MaBenhNhan).HasMaxLength(20).IsRequired();
                entity.Property(e => e.MaKho).HasMaxLength(20).IsRequired();
                entity.Property(e => e.TongTien).HasColumnType("decimal(15,2)").HasDefaultValue(0);
                entity.Property(e => e.NhanVienCap).HasMaxLength(20).IsRequired();
                entity.Property(e => e.TrangThai).HasMaxLength(20).HasDefaultValue("DangXuLy");
                entity.Property(e => e.GhiChu).HasMaxLength(500);
                entity.Property(e => e.NgayTao).HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.DonThuoc)
                    .WithMany()
                    .HasForeignKey(e => e.MaDonThuoc)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.BenhNhan)
                    .WithMany()
                    .HasForeignKey(e => e.MaBenhNhan)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Kho)
                    .WithMany()
                    .HasForeignKey(e => e.MaKho)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.NhanVien)
                    .WithMany()
                    .HasForeignKey(e => e.NhanVienCap)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.NgayCap).HasDatabaseName("idx_ngaycap");
                entity.HasIndex(e => e.MaDonThuoc).HasDatabaseName("idx_donthuoc");
            });

            // ChiTietPhieuCap
            modelBuilder.Entity<ChiTietPhieuCap>(entity =>
            {
                entity.ToTable("ChiTietPhieuCap");
                entity.HasKey(e => new { e.MaPhieuCap, e.MaLo });
                entity.Property(e => e.MaPhieuCap).HasMaxLength(20);
                entity.Property(e => e.MaLo).HasMaxLength(20);
                entity.Property(e => e.SoLuongCap).IsRequired();
                entity.Property(e => e.DonGiaCap).HasColumnType("decimal(15,2)").IsRequired();
                entity.Property(e => e.ThanhTien).HasColumnType("decimal(15,2)").HasComputedColumnSql("[SoLuongCap] * [DonGiaCap]");

                entity.HasOne(e => e.PhieuCapPhat)
                    .WithMany(p => p.ChiTietPhieuCaps)
                    .HasForeignKey(e => e.MaPhieuCap)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.LoThuoc)
                    .WithMany()
                    .HasForeignKey(e => e.MaLo)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // HoaDon
            modelBuilder.Entity<HoaDon>(entity =>
            {
                entity.ToTable("HoaDon");
                entity.HasKey(e => e.MaHoaDon);
                entity.Property(e => e.MaHoaDon).HasMaxLength(20);
                entity.Property(e => e.MaBenhNhan).HasMaxLength(20).IsRequired();
                entity.Property(e => e.MaDonThuoc).HasMaxLength(20);
                entity.Property(e => e.TongTien).HasColumnType("decimal(15,2)").HasDefaultValue(0);
                entity.Property(e => e.TienBHYTChiTra).HasColumnType("decimal(15,2)").HasDefaultValue(0);
                entity.Property(e => e.TienBenhNhanCanTra).HasColumnType("decimal(15,2)").HasDefaultValue(0);
                entity.Property(e => e.TienDaTra).HasColumnType("decimal(15,2)").HasDefaultValue(0);
                entity.Property(e => e.MaSoThue).HasMaxLength(20);
                entity.Property(e => e.HinhThucThanhToan).HasMaxLength(50);
                entity.Property(e => e.TrangThaiThanhToan).HasMaxLength(20).HasDefaultValue("ChuaTra");
                entity.Property(e => e.GhiChu).HasMaxLength(500);
                entity.Property(e => e.NgayTao).HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.BenhNhan)
                    .WithMany(b => b.HoaDons)
                    .HasForeignKey(e => e.MaBenhNhan)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.DonThuoc)
                    .WithMany()
                    .HasForeignKey(e => e.MaDonThuoc)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.NgayTaoHoaDon).HasDatabaseName("idx_ngaytaohoadon");
                entity.HasIndex(e => e.TrangThaiThanhToan).HasDatabaseName("idx_trangthai");
            });

            // PhieuThuTien
            modelBuilder.Entity<PhieuThuTien>(entity =>
            {
                entity.ToTable("PhieuThuTien");
                entity.HasKey(e => e.MaPhieuThu);
                entity.Property(e => e.MaPhieuThu).HasMaxLength(20);
                entity.Property(e => e.MaHoaDon).HasMaxLength(20).IsRequired();
                entity.Property(e => e.SoTienThu).HasColumnType("decimal(15,2)").IsRequired();
                entity.Property(e => e.HinhThucThanhToan).HasMaxLength(50).IsRequired();
                entity.Property(e => e.MaGiaoDichNganHang).HasMaxLength(50);
                entity.Property(e => e.NhanVienThu).HasMaxLength(20).IsRequired();
                entity.Property(e => e.TrangThai).HasMaxLength(20).HasDefaultValue("DaXacNhan");
                entity.Property(e => e.GhiChu).HasMaxLength(500);
                entity.Property(e => e.NgayTao).HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.HoaDon)
                    .WithMany(h => h.PhieuThuTiens)
                    .HasForeignKey(e => e.MaHoaDon)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.NhanVien)
                    .WithMany()
                    .HasForeignKey(e => e.NhanVienThu)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.NgayThu).HasDatabaseName("idx_ngaythu");
                entity.HasIndex(e => e.MaHoaDon).HasDatabaseName("idx_hoadon");
            });

            // VietQRGiaoDich
            modelBuilder.Entity<VietQRGiaoDich>(entity =>
            {
                entity.ToTable("VietQR_GiaoDich");
                entity.HasKey(e => e.MaQR);
                entity.Property(e => e.MaQR).HasMaxLength(20);
                entity.Property(e => e.MaHoaDon).HasMaxLength(20).IsRequired();
                entity.Property(e => e.SoTienYeuCau).HasColumnType("decimal(15,2)").IsRequired();
                entity.Property(e => e.SoTienNhan).HasColumnType("decimal(15,2)");
                entity.Property(e => e.NoiDungChuyenKhoan).HasMaxLength(200).IsRequired();
                entity.Property(e => e.MaGiaoDichNganHang).HasMaxLength(50);
                entity.Property(e => e.QRCodeBase64).HasColumnType("nvarchar(max)");
                entity.Property(e => e.DuLieuWebhook).HasColumnType("nvarchar(max)");
                entity.Property(e => e.TrangThai).HasMaxLength(20).HasDefaultValue("ChoThanhToan");
                entity.Property(e => e.NgayTao).HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.HoaDon)
                    .WithMany()
                    .HasForeignKey(e => e.MaHoaDon)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.MaHoaDon).HasDatabaseName("idx_hoadon");
                entity.HasIndex(e => e.NoiDungChuyenKhoan).HasDatabaseName("idx_noidung");
            });

            // CauHinhVietQR
            modelBuilder.Entity<CauHinhVietQR>(entity =>
            {
                entity.ToTable("CauHinh_VietQR");
                entity.HasKey(e => e.MaCauHinh);
                entity.Property(e => e.MaCauHinh).HasMaxLength(20);
                entity.Property(e => e.TenNganHang).HasMaxLength(200).IsRequired();
                entity.Property(e => e.SoTaiKhoan).HasMaxLength(20).IsRequired();
                entity.Property(e => e.TenTaiKhoan).HasMaxLength(200).IsRequired();
                entity.Property(e => e.MaNganHang).HasMaxLength(10).IsRequired();
                entity.Property(e => e.Template).HasMaxLength(20).HasDefaultValue("compact2");
                entity.Property(e => e.WebhookURL).HasMaxLength(500);
                entity.Property(e => e.APIKey).HasMaxLength(255);
                entity.Property(e => e.APISecret).HasMaxLength(255);
                entity.Property(e => e.TrangThai).HasMaxLength(20).HasDefaultValue("KichHoat");
                entity.Property(e => e.GhiChu).HasMaxLength(500);
                entity.Property(e => e.NgayTao).HasDefaultValueSql("GETDATE()");
            });

            // CauHinhHeThong
            modelBuilder.Entity<CauHinhHeThong>(entity =>
            {
                entity.ToTable("CauHinhHeThong");
                entity.HasKey(e => e.MaCauHinh);
                entity.Property(e => e.MaCauHinh).HasMaxLength(20);
                entity.Property(e => e.TenCauHinh).HasMaxLength(100).IsRequired();
                entity.Property(e => e.GiaTri).HasMaxLength(500).IsRequired();
                entity.Property(e => e.MoTa).HasMaxLength(500);
                entity.Property(e => e.LoaiDuLieu).HasMaxLength(20);
                entity.Property(e => e.NgayTao).HasDefaultValueSql("GETDATE()");

                entity.HasIndex(e => e.TenCauHinh).HasDatabaseName("idx_tencauhinh").IsUnique();
            });

            // ThongBao
            modelBuilder.Entity<ThongBao>(entity =>
            {
                entity.ToTable("ThongBao");
                entity.HasKey(e => e.MaThongBao);
                entity.Property(e => e.MaThongBao).HasMaxLength(20);
                entity.Property(e => e.TieuDe).HasMaxLength(200).IsRequired();
                entity.Property(e => e.NoiDung).HasColumnType("nvarchar(max)").IsRequired();
                entity.Property(e => e.LoaiThongBao).HasMaxLength(50).IsRequired();
                entity.Property(e => e.DoUuTien).HasMaxLength(20).HasDefaultValue("Binh");
                entity.Property(e => e.NguoiNhan).HasMaxLength(20);
                entity.Property(e => e.DaDoc).HasDefaultValue(false);
                entity.Property(e => e.NgayTao).HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.NhanVien)
                    .WithMany()
                    .HasForeignKey(e => e.NguoiNhan)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => e.NgayTao).HasDatabaseName("idx_ngaytao");
                entity.HasIndex(e => e.DoUuTien).HasDatabaseName("idx_uutien");
                entity.HasIndex(e => e.DaDoc).HasDatabaseName("idx_dadoc");
            });
        }
    }
}