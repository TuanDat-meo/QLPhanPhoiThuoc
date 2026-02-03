using Microsoft.EntityFrameworkCore;
using QLPhanPhoiThuoc.Models.Entities.VNeID;

namespace QLPhanPhoiThuoc.Models.EF
{
    public class VNeIDDbContext : DbContext
    {
        public VNeIDDbContext(DbContextOptions<VNeIDDbContext> options)
            : base(options)
        {
        }

        // Các bảng từ VNeID Mock Database
        public DbSet<CongDan> CongDans { get; set; }
        public DbSet<TheBHYTMock> TheBHYTMocks { get; set; }
        public DbSet<LichSuTraCuu> LichSuTraCuus { get; set; }
        public DbSet<CauHinhTichHop> CauHinhTichHops { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // CongDan
            modelBuilder.Entity<CongDan>(entity =>
            {
                entity.ToTable("CongDan");
                entity.HasKey(e => e.SoDinhDanh);
                entity.Property(e => e.SoDinhDanh).HasMaxLength(12).HasComment("Số CCCD/CMND");
                entity.Property(e => e.HoTen).HasMaxLength(100).IsRequired();
                entity.Property(e => e.NgaySinh).IsRequired();
                entity.Property(e => e.GioiTinh).HasMaxLength(10).IsRequired();
                entity.Property(e => e.QueQuan).HasMaxLength(200);
                entity.Property(e => e.NoiThuongTru).HasMaxLength(300);
                entity.Property(e => e.DiaChiHienTai).HasMaxLength(300);
                entity.Property(e => e.DanToc).HasMaxLength(50);
                entity.Property(e => e.TonGiao).HasMaxLength(50);
                entity.Property(e => e.QuocTich).HasMaxLength(50).HasDefaultValue("Việt Nam");
                entity.Property(e => e.NoiCap).HasMaxLength(200);
                entity.Property(e => e.AnhChanDung).HasColumnType("nvarchar(max)").HasComment("Base64 hoặc URL ảnh");
                entity.Property(e => e.TrangThai).HasMaxLength(20).HasDefaultValue("HoatDong");
                entity.Property(e => e.NgayTao).HasDefaultValueSql("GETDATE()");

                entity.HasIndex(e => e.HoTen).HasDatabaseName("idx_hoten");
                entity.HasIndex(e => e.NgaySinh).HasDatabaseName("idx_ngaysinh");
            });

            // TheBHYTMock
            modelBuilder.Entity<TheBHYTMock>(entity =>
            {
                entity.ToTable("TheBHYT_Mock");
                entity.HasKey(e => e.MaThe);
                entity.Property(e => e.MaThe).HasMaxLength(20);
                entity.Property(e => e.SoDinhDanh).HasMaxLength(12).IsRequired();
                entity.Property(e => e.SoTheBHYT).HasMaxLength(15).IsRequired().HasComment("Mã thẻ BHYT 15 ký tự");
                entity.Property(e => e.NgayBatDau).IsRequired();
                entity.Property(e => e.NgayHetHan).IsRequired();
                entity.Property(e => e.MucHuong).HasColumnType("decimal(5,2)").IsRequired().HasComment("80, 95, 100");
                entity.Property(e => e.NoiDKKCB).HasMaxLength(200).HasComment("Nơi đăng ký khám chữa bệnh ban đầu");
                entity.Property(e => e.MaNoiDKKCB).HasMaxLength(10).HasComment("Mã BV đăng ký");
                entity.Property(e => e.DiaChi5Nam).HasMaxLength(300).HasComment("Nơi cư trú 5 năm liên tục");
                entity.Property(e => e.MaKhuVuc).HasMaxLength(5).HasComment("K1, K2, K3");
                entity.Property(e => e.TrangThai).HasMaxLength(20).HasDefaultValue("ConHan");
                entity.Property(e => e.NgayTao).HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.CongDan)
                    .WithMany(c => c.TheBHYTMocks)
                    .HasForeignKey(e => e.SoDinhDanh)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.SoTheBHYT).HasDatabaseName("idx_sothe").IsUnique();
                entity.HasIndex(e => e.NgayHetHan).HasDatabaseName("idx_hethan");
                entity.HasIndex(e => e.SoDinhDanh).HasDatabaseName("idx_sodinhhanh");
            });

            // LichSuTraCuu
            modelBuilder.Entity<LichSuTraCuu>(entity =>
            {
                entity.ToTable("LichSuTraCuu");
                entity.HasKey(e => e.MaTraCuu);
                entity.Property(e => e.MaTraCuu).HasMaxLength(20);
                entity.Property(e => e.SoDinhDanh).HasMaxLength(12).IsRequired();
                entity.Property(e => e.LoaiTraCuu).HasMaxLength(50).IsRequired();
                entity.Property(e => e.HeThongTraCuu).HasMaxLength(50).IsRequired().HasComment("Tên hệ thống yêu cầu");
                entity.Property(e => e.IPAddress).HasMaxLength(45);
                entity.Property(e => e.UserAgent).HasMaxLength(500);
                entity.Property(e => e.NgayGioTraCuu).IsRequired();
                entity.Property(e => e.KetQua).HasMaxLength(20).IsRequired();
                entity.Property(e => e.DuLieuTraVe).HasColumnType("nvarchar(max)").HasComment("JSON response");
                entity.Property(e => e.ThoiGianXuLy).HasComment("Milliseconds");
                entity.Property(e => e.GhiChu).HasMaxLength(500);

                entity.HasOne(e => e.CongDan)
                    .WithMany(c => c.LichSuTraCuus)
                    .HasForeignKey(e => e.SoDinhDanh)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.NgayGioTraCuu).HasDatabaseName("idx_ngaygio");
                entity.HasIndex(e => e.HeThongTraCuu).HasDatabaseName("idx_hethong");
                entity.HasIndex(e => e.SoDinhDanh).HasDatabaseName("idx_sodinhhanh");
            });

            // CauHinhTichHop
            modelBuilder.Entity<CauHinhTichHop>(entity =>
            {
                entity.ToTable("CauHinhTichHop");
                entity.HasKey(e => e.MaCauHinh);
                entity.Property(e => e.MaCauHinh).HasMaxLength(20);
                entity.Property(e => e.TenHeThong).HasMaxLength(100).IsRequired();
                entity.Property(e => e.APIKey).HasMaxLength(255).IsRequired();
                entity.Property(e => e.APISecret).HasMaxLength(255);
                entity.Property(e => e.IPWhitelist).HasColumnType("nvarchar(max)").HasComment("Danh sách IP được phép, cách nhau bởi dấu phẩy");
                entity.Property(e => e.SoLanTraCuuToiDa).HasDefaultValue(1000).HasComment("Giới hạn số lần tra cứu/ngày");
                entity.Property(e => e.TrangThai).HasMaxLength(20).HasDefaultValue("KichHoat");
                entity.Property(e => e.NgayTao).HasDefaultValueSql("GETDATE()");

                entity.HasIndex(e => e.APIKey).HasDatabaseName("idx_apikey").IsUnique();
            });
        }
    }
}