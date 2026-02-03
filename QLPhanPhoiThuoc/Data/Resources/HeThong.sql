-- =====================================================
-- HỆ THỐNG QUẢN LÝ THUỐC - PHIÊN BẢN TỐI ƯU
-- Dành cho: Bệnh viện nhỏ 20-50 giường
-- Tổng số bảng: 23 (giảm 36% so với bản gốc)
-- Phiên bản: 3.0 OPTIMIZED
-- =====================================================

CREATE DATABASE IF NOT EXISTS BenhVienX
CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

USE BenhVienX;

-- =====================================================
-- NHÓM A: DANH MỤC CƠ SỞ (6 bảng)
-- =====================================================

-- 1. Thuốc - BỔ SUNG 4 THUỘC TÍNH
CREATE TABLE Thuoc (
    MaThuoc VARCHAR(20) PRIMARY KEY,
    TenThuoc NVARCHAR(200) NOT NULL,
    HoatChat NVARCHAR(200) COMMENT 'Hoạt chất chính - Kiểm tra dị ứng',
    DonViTinh NVARCHAR(50) NOT NULL,
    HamLuong NVARCHAR(100),
    DangBaoChe NVARCHAR(100) COMMENT 'Viên nén/Viên nang/Ống tiêm/Chai',
    DuongDung NVARCHAR(100) COMMENT 'Uống/Tiêm/Bôi/Nhỏ mắt/Xịt mũi',
    NhaSanXuat NVARCHAR(200),
    NhomThuoc NVARCHAR(100) COMMENT 'Kháng sinh/Giảm đau/Tim mạch/Tiêu hóa...',
    GiaNhap DECIMAL(15,2) NOT NULL DEFAULT 0,
    GiaXuat DECIMAL(15,2) NOT NULL DEFAULT 0,
    TonKhoToiThieu INT DEFAULT 10 COMMENT 'Ngưỡng cảnh báo đặt hàng',
    LaThuocBHYT ENUM('Yes', 'No') DEFAULT 'No',
    TyLeBHYTChiTra DECIMAL(5,2) DEFAULT 0,
    MoTa NVARCHAR(500),
    TrangThai ENUM('KichHoat', 'NgungSuDung') DEFAULT 'KichHoat',
    NgayTao DATETIME DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_tenthuoc (TenThuoc),
    INDEX idx_hoatchat (HoatChat),
    INDEX idx_nhomthuoc (NhomThuoc),
    INDEX idx_bhyt (LaThuocBHYT)
) ENGINE=InnoDB COMMENT='Danh mục thuốc - Đã bổ sung HoatChat, DuongDung, NhomThuoc, TonKhoToiThieu';

-- 2. Nhà cung cấp
CREATE TABLE NhaCungCap (
    MaNCC VARCHAR(20) PRIMARY KEY,
    TenNCC NVARCHAR(200) NOT NULL,
    DiaChi NVARCHAR(300),
    SoDienThoai VARCHAR(15),
    Email VARCHAR(100),
    MaSoThue VARCHAR(20),
    TrangThai ENUM('HoatDong', 'TamDung') DEFAULT 'HoatDong',
    NgayTao DATETIME DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_tenncc (TenNCC)
) ENGINE=InnoDB;

-- 3. Kho
CREATE TABLE Kho (
    MaKho VARCHAR(20) PRIMARY KEY,
    TenKho NVARCHAR(100) NOT NULL,
    LoaiKho ENUM('BHYT', 'DichVu', 'TongHop') NOT NULL,
    DiaDiem NVARCHAR(200),
    GhiChu NVARCHAR(300),
    TrangThai ENUM('HoatDong', 'TamDung') DEFAULT 'HoatDong',
    NgayTao DATETIME DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB;

-- 4. Khoa phòng
CREATE TABLE KhoaPhong (
    MaKhoa VARCHAR(20) PRIMARY KEY,
    TenKhoa NVARCHAR(100) NOT NULL,
    TruongKhoa NVARCHAR(100),
    SoDienThoai VARCHAR(15),
    TrangThai ENUM('HoatDong', 'TamDung') DEFAULT 'HoatDong',
    NgayTao DATETIME DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB;

-- 5. Nhân viên - GỘP THÔNG TIN BÁC SĨ
CREATE TABLE NhanVien (
    MaNhanVien VARCHAR(20) PRIMARY KEY,
    TenNhanVien NVARCHAR(100) NOT NULL,
    ChucVu NVARCHAR(50) COMMENT 'BacSi/DuocSi/ThuNgan/TiepDon/KhoThuoc',
    ChuyenKhoa NVARCHAR(100) COMMENT 'NULL nếu không phải bác sĩ - Nội/Ngoại/Nhi/Sản...',
    BangCap NVARCHAR(200) COMMENT 'CK1/CK2/ThS/BS/Dược sĩ',
    MaKhoa VARCHAR(20),
    SoDienThoai VARCHAR(15),
    Email VARCHAR(100),
    CCCD VARCHAR(12),
    NgaySinh DATE,
    GioiTinh ENUM('Nam', 'Nu', 'Khac'),
    DiaChi NVARCHAR(300),
    TrangThai ENUM('DangLamViec', 'NghiViec', 'TamNghi') DEFAULT 'DangLamViec',
    NgayVaoLam DATE,
    NgayTao DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (MaKhoa) REFERENCES KhoaPhong(MaKhoa),
    INDEX idx_tennv (TenNhanVien),
    INDEX idx_chucvu (ChucVu),
    INDEX idx_chuyenkhoa (ChuyenKhoa)
) ENGINE=InnoDB COMMENT='Gộp bảng BacSi - Bác sĩ có ChucVu=BacSi và ChuyenKhoa không NULL';

-- 6. Tài khoản
CREATE TABLE TaiKhoan (
    MaTaiKhoan VARCHAR(20) PRIMARY KEY,
    MaNhanVien VARCHAR(20) NOT NULL UNIQUE,
    Username VARCHAR(50) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    Role ENUM('Admin', 'BacSi', 'DuocSi', 'ThuNgan', 'TiepDon', 'KhoThuoc') NOT NULL,
    TrangThai ENUM('KichHoat', 'Khoa', 'Xoa') DEFAULT 'KichHoat',
    LanDangNhapCuoi DATETIME,
    NgayTao DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (MaNhanVien) REFERENCES NhanVien(MaNhanVien),
    INDEX idx_username (Username)
) ENGINE=InnoDB;

-- =====================================================
-- NHÓM B: QUẢN LÝ KHO THUỐC (3 bảng)
-- =====================================================

-- 7. Lô thuốc
CREATE TABLE LoThuoc (
    MaLo VARCHAR(20) PRIMARY KEY,
    MaThuoc VARCHAR(20) NOT NULL,
    MaKho VARCHAR(20) NOT NULL,
    SoLo VARCHAR(50) NOT NULL,
    NgaySanXuat DATE,
    HanSuDung DATE NOT NULL,
    SoLuongNhap INT NOT NULL DEFAULT 0,
    SoLuongCon INT NOT NULL DEFAULT 0,
    TrangThai ENUM('ConHang', 'HetHang', 'GanHetHan', 'HetHan') DEFAULT 'ConHang',
    NgayTao DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (MaThuoc) REFERENCES Thuoc(MaThuoc),
    FOREIGN KEY (MaKho) REFERENCES Kho(MaKho),
    INDEX idx_thuoc (MaThuoc),
    INDEX idx_kho (MaKho),
    INDEX idx_hansudung (HanSuDung),
    UNIQUE KEY uk_thuoc_kho_solo (MaThuoc, MaKho, SoLo)
) ENGINE=InnoDB;

-- 8. Phiếu nhập
CREATE TABLE PhieuNhap (
    MaPhieuNhap VARCHAR(20) PRIMARY KEY,
    MaNCC VARCHAR(20) NOT NULL,
    MaKho VARCHAR(20) NOT NULL,
    NgayNhap DATETIME NOT NULL,
    TongTien DECIMAL(15,2) NOT NULL DEFAULT 0,
    NhanVienNhap VARCHAR(20) NOT NULL,
    SoHoaDon VARCHAR(50),
    TrangThai ENUM('DangNhap', 'DaNhap', 'DaHuy') DEFAULT 'DangNhap',
    GhiChu NVARCHAR(500),
    NgayTao DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (MaNCC) REFERENCES NhaCungCap(MaNCC),
    FOREIGN KEY (MaKho) REFERENCES Kho(MaKho),
    FOREIGN KEY (NhanVienNhap) REFERENCES NhanVien(MaNhanVien),
    INDEX idx_ngaynhap (NgayNhap),
    INDEX idx_ncc (MaNCC)
) ENGINE=InnoDB;

-- 9. Chi tiết phiếu nhập
CREATE TABLE ChiTietPhieuNhap (
    MaPhieuNhap VARCHAR(20),
    MaLo VARCHAR(20),
    SoLuongNhap INT NOT NULL,
    DonGiaNhap DECIMAL(15,2) NOT NULL,
    ThanhTien DECIMAL(15,2) AS (SoLuongNhap * DonGiaNhap) STORED,
    PRIMARY KEY (MaPhieuNhap, MaLo),
    FOREIGN KEY (MaPhieuNhap) REFERENCES PhieuNhap(MaPhieuNhap),
    FOREIGN KEY (MaLo) REFERENCES LoThuoc(MaLo)
) ENGINE=InnoDB;

-- =====================================================
-- NHÓM C: BỆNH NHÂN (2 bảng)
-- =====================================================

-- 10. Bệnh nhân - BỔ SUNG 5 THUỘC TÍNH
CREATE TABLE BenhNhan (
    MaBenhNhan VARCHAR(20) PRIMARY KEY,
    TenBenhNhan NVARCHAR(100) NOT NULL,
    NgaySinh DATE,
    GioiTinh ENUM('Nam', 'Nu', 'Khac'),
    DiaChi NVARCHAR(300),
    SoDienThoai VARCHAR(15),
    CCCD VARCHAR(12),
    Email VARCHAR(100),
    NhomMau VARCHAR(5) COMMENT 'A/B/AB/O, Rh+/Rh-',
    NgheNghiep NVARCHAR(100),
    CanNang DECIMAL(5,2) COMMENT 'Kg - Dùng tính liều thuốc',
    ChieuCao DECIMAL(5,2) COMMENT 'Cm',
    TienSuDiUng TEXT COMMENT 'QUAN TRỌNG: Danh sách thuốc/thực phẩm dị ứng',
    LoaiBenhNhan ENUM('NoiTru', 'NgoaiTru') DEFAULT 'NgoaiTru',
    TrangThai ENUM('HoatDong', 'TuVong', 'Khac') DEFAULT 'HoatDong',
    NgayTao DATETIME DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_tenbn (TenBenhNhan),
    INDEX idx_sdt (SoDienThoai),
    INDEX idx_cccd (CCCD)
) ENGINE=InnoDB COMMENT='Đã bổ sung: NhomMau, NgheNghiep, CanNang, ChieuCao, TienSuDiUng';

-- 11. Thẻ BHYT
CREATE TABLE TheBHYT (
    MaThe VARCHAR(20) PRIMARY KEY,
    MaBenhNhan VARCHAR(20) NOT NULL,
    SoTheBHYT VARCHAR(15) NOT NULL UNIQUE,
    NgayBatDau DATE NOT NULL,
    NgayHetHan DATE NOT NULL,
    MucHuong DECIMAL(5,2) NOT NULL,
    NoiDangKyKCB NVARCHAR(200),
    DiaChi5Nam NVARCHAR(300),
    TrangThai ENUM('ConHan', 'HetHan', 'TamKhoa') DEFAULT 'ConHan',
    NgayTao DATETIME DEFAULT CURRENT_TIMESTAMP,
    NgayCapNhat DATETIME ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (MaBenhNhan) REFERENCES BenhNhan(MaBenhNhan),
    INDEX idx_sothe (SoTheBHYT),
    INDEX idx_hethan (NgayHetHan)
) ENGINE=InnoDB;

-- =====================================================
-- NHÓM D: KHÁM BỆNH & ĐƠN THUỐC (3 bảng)
-- =====================================================

-- 12. Chẩn đoán đơn giản - GỘP TỪ 2 BẢNG (Benh + ChanDoan)
CREATE TABLE ChanDoan (
    MaChanDoan VARCHAR(20) PRIMARY KEY,
    MaBenhNhan VARCHAR(20) NOT NULL,
    MaNhanVien VARCHAR(20) NOT NULL COMMENT 'Bác sĩ chẩn đoán',
    NgayChanDoan DATETIME NOT NULL,
    TenBenh NVARCHAR(300) NOT NULL COMMENT 'Lưu trực tiếp tên bệnh thay vì tham chiếu',
    MaBenh VARCHAR(10) COMMENT 'Mã ICD-10 nếu có',
    MoTaTrieuChung TEXT,
    HuongDieuTri TEXT,
    TrangThai ENUM('DangDieuTri', 'KhoiBenh', 'ChuyenVien') DEFAULT 'DangDieuTri',
    NgayTao DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (MaBenhNhan) REFERENCES BenhNhan(MaBenhNhan),
    FOREIGN KEY (MaNhanVien) REFERENCES NhanVien(MaNhanVien),
    INDEX idx_benhnhan (MaBenhNhan),
    INDEX idx_ngaychandoan (NgayChanDoan)
) ENGINE=InnoDB COMMENT='Gộp bảng Benh và ChanDoan - Đơn giản hóa cho bệnh viện nhỏ';

-- 13. Đơn thuốc - BỔ SUNG 3 THUỘC TÍNH
CREATE TABLE DonThuoc (
    MaDonThuoc VARCHAR(20) PRIMARY KEY,
    MaBenhNhan VARCHAR(20) NOT NULL,
    MaChanDoan VARCHAR(20),
    MaNhanVien VARCHAR(20) NOT NULL COMMENT 'Bác sĩ kê đơn',
    NgayKeDon DATETIME NOT NULL,
    LoaiDon ENUM('BHYT', 'DichVu', 'TuTuc') NOT NULL,
    TongTien DECIMAL(15,2) DEFAULT 0 COMMENT 'Tổng giá trị đơn thuốc',
    ChanDoanSoBo NVARCHAR(300) COMMENT 'Chẩn đoán sơ bộ ghi trên đơn',
    GhiChuBacSi TEXT COMMENT 'Lời dặn của bác sĩ cho bệnh nhân',
    TrangThai ENUM('ChuaCapPhat', 'DaCapPhat', 'DaHuy') DEFAULT 'ChuaCapPhat',
    NgayTao DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (MaBenhNhan) REFERENCES BenhNhan(MaBenhNhan),
    FOREIGN KEY (MaChanDoan) REFERENCES ChanDoan(MaChanDoan),
    FOREIGN KEY (MaNhanVien) REFERENCES NhanVien(MaNhanVien),
    INDEX idx_benhnhan (MaBenhNhan),
    INDEX idx_ngaykedon (NgayKeDon)
) ENGINE=InnoDB COMMENT='Đã bổ sung: TongTien, ChanDoanSoBo, GhiChuBacSi';

-- 14. Chi tiết đơn thuốc - BỔ SUNG LieuDung
CREATE TABLE ChiTietDonThuoc (
    MaDonThuoc VARCHAR(20),
    MaThuoc VARCHAR(20),
    SoLuong INT NOT NULL,
    DonGia DECIMAL(15,2) NOT NULL,
    LieuDung NVARCHAR(200) COMMENT 'VD: 2 viên x 3 lần/ngày, sau ăn',
    ThanhTien DECIMAL(15,2) AS (SoLuong * DonGia) STORED,
    PRIMARY KEY (MaDonThuoc, MaThuoc),
    FOREIGN KEY (MaDonThuoc) REFERENCES DonThuoc(MaDonThuoc),
    FOREIGN KEY (MaThuoc) REFERENCES Thuoc(MaThuoc)
) ENGINE=InnoDB COMMENT='Đã bổ sung: LieuDung';

-- =====================================================
-- NHÓM E: XUẤT THUỐC & HÓA ĐƠN (4 bảng)
-- =====================================================

-- 15. Phiếu cấp thuốc ngoại trú
CREATE TABLE PhieuCapThuocNgoaiTru (
    MaPhieuCap VARCHAR(20) PRIMARY KEY,
    MaDonThuoc VARCHAR(20) NOT NULL,
    MaBenhNhan VARCHAR(20) NOT NULL,
    MaKho VARCHAR(20) NOT NULL,
    NgayCap DATETIME NOT NULL,
    NhanVienCap VARCHAR(20) NOT NULL,
    MaHoaDon VARCHAR(20),
    TongTien DECIMAL(15,2) NOT NULL DEFAULT 0,
    TrangThai ENUM('DangXuLy', 'DaXuatThuoc', 'DaHuy') DEFAULT 'DangXuLy',
    GhiChu NVARCHAR(500),
    NgayTao DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (MaDonThuoc) REFERENCES DonThuoc(MaDonThuoc),
    FOREIGN KEY (MaBenhNhan) REFERENCES BenhNhan(MaBenhNhan),
    FOREIGN KEY (MaKho) REFERENCES Kho(MaKho),
    FOREIGN KEY (NhanVienCap) REFERENCES NhanVien(MaNhanVien),
    INDEX idx_donthuoc (MaDonThuoc),
    INDEX idx_benhnhan (MaBenhNhan),
    INDEX idx_ngaycap (NgayCap)
) ENGINE=InnoDB;

-- 16. Chi tiết phiếu cấp
CREATE TABLE ChiTietPhieuCap (
    MaPhieuCap VARCHAR(20),
    MaLo VARCHAR(20),
    SoLuongCap INT NOT NULL,
    DonGia DECIMAL(15,2) NOT NULL,
    ThanhTien DECIMAL(15,2) AS (SoLuongCap * DonGia) STORED,
    PRIMARY KEY (MaPhieuCap, MaLo),
    FOREIGN KEY (MaPhieuCap) REFERENCES PhieuCapThuocNgoaiTru(MaPhieuCap),
    FOREIGN KEY (MaLo) REFERENCES LoThuoc(MaLo)
) ENGINE=InnoDB;

-- 17. Hóa đơn - BỔ SUNG 2 THUỘC TÍNH
CREATE TABLE HoaDon (
    MaHoaDon VARCHAR(20) PRIMARY KEY,
    MaBenhNhan VARCHAR(20) NOT NULL,
    MaPhieuCap VARCHAR(20),
    LoaiHoaDon ENUM('NgoaiTru', 'NoiTru') NOT NULL,
    NgayTaoHoaDon DATETIME NOT NULL,
    TongTien DECIMAL(15,2) NOT NULL DEFAULT 0,
    TienBHYTChiTra DECIMAL(15,2) DEFAULT 0,
    TienBenhNhanCanTra DECIMAL(15,2) NOT NULL DEFAULT 0,
    TienDaTra DECIMAL(15,2) DEFAULT 0,
    MaSoThue VARCHAR(20) COMMENT 'Số hóa đơn GTGT nếu yêu cầu',
    HinhThucThanhToan VARCHAR(50) COMMENT 'TienMat/ChuyenKhoan/VietQR/The',
    TrangThaiThanhToan ENUM('ChuaTra', 'DaTra1Phan', 'DaTraDu') DEFAULT 'ChuaTra',
    NhanVienLap VARCHAR(20) NOT NULL,
    GhiChu NVARCHAR(500),
    NgayTao DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (MaBenhNhan) REFERENCES BenhNhan(MaBenhNhan),
    FOREIGN KEY (MaPhieuCap) REFERENCES PhieuCapThuocNgoaiTru(MaPhieuCap),
    FOREIGN KEY (NhanVienLap) REFERENCES NhanVien(MaNhanVien),
    INDEX idx_benhnhan (MaBenhNhan),
    INDEX idx_ngayhd (NgayTaoHoaDon),
    INDEX idx_trangthai (TrangThaiThanhToan)
) ENGINE=InnoDB COMMENT='Đã bổ sung: MaSoThue, HinhThucThanhToan';

-- 18. Phiếu thu tiền
CREATE TABLE PhieuThuTien (
    MaPhieuThu VARCHAR(20) PRIMARY KEY,
    MaHoaDon VARCHAR(20) NOT NULL,
    NgayThu DATETIME NOT NULL,
    SoTienThu DECIMAL(15,2) NOT NULL,
    HinhThucThanhToan ENUM('TienMat', 'ChuyenKhoan', 'The', 'VietQR') NOT NULL,
    MaGiaoDichNganHang VARCHAR(50),
    NhanVienThu VARCHAR(20) NOT NULL,
    TrangThai ENUM('DangXuLy', 'DaXacNhan', 'DaHuy') DEFAULT 'DangXuLy',
    GhiChu NVARCHAR(500),
    NgayTao DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (MaHoaDon) REFERENCES HoaDon(MaHoaDon),
    FOREIGN KEY (NhanVienThu) REFERENCES NhanVien(MaNhanVien),
    INDEX idx_hoadon (MaHoaDon),
    INDEX idx_ngaythu (NgayThu)
) ENGINE=InnoDB;

-- =====================================================
-- NHÓM F: THANH TOÁN VIETQR (2 bảng)
-- =====================================================

-- 19. Cấu hình VietQR
CREATE TABLE CauHinhVietQR (
    MaCauHinh VARCHAR(20) PRIMARY KEY,
    TenNganHang NVARCHAR(200) NOT NULL,
    SoTaiKhoan VARCHAR(50) NOT NULL,
    TenTaiKhoan NVARCHAR(200) NOT NULL,
    MaNganHang VARCHAR(10) NOT NULL,
    Template VARCHAR(20) DEFAULT 'compact2',
    LogoURL VARCHAR(500),
    SoDienThoaiHotline VARCHAR(15),
    EmailHoTro VARCHAR(100),
    TrangThai ENUM('KichHoat', 'TamDung') DEFAULT 'KichHoat',
    GhiChu NVARCHAR(300),
    NgayTao DATETIME DEFAULT CURRENT_TIMESTAMP,
    NgayCapNhat DATETIME ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB;

-- 20. VietQR giao dịch - GỘP WEBHOOK LOG
CREATE TABLE VietQR_GiaoDich (
    MaQR VARCHAR(20) PRIMARY KEY,
    MaHoaDon VARCHAR(20) NOT NULL,
    NoiDungQR TEXT COMMENT 'Chuỗi QR code Base64',
    SoTien DECIMAL(15,2) NOT NULL,
    NoiDungChuyenKhoan NVARCHAR(200),
    MaGiaoDichNganHang VARCHAR(50),
    NgayTao DATETIME DEFAULT CURRENT_TIMESTAMP,
    NgayHetHan DATETIME,
    NgayThanhToan DATETIME,
    SoTienNhan DECIMAL(15,2),
    DuLieuWebhook TEXT COMMENT 'JSON từ webhook ngân hàng - Gộp từ bảng WebhookLog',
    TrangThai ENUM('ChoThanhToan', 'DaThanhToan', 'HetHan', 'Huy') DEFAULT 'ChoThanhToan',
    FOREIGN KEY (MaHoaDon) REFERENCES HoaDon(MaHoaDon),
    INDEX idx_hoadon (MaHoaDon),
    INDEX idx_trangthai (TrangThai)
) ENGINE=InnoDB COMMENT='Đã gộp bảng VietQR_WebhookLog vào cột DuLieuWebhook';

-- =====================================================
-- NHÓM G: QUẢN TRỊ (3 bảng)
-- =====================================================

-- 21. Tương tác thuốc
CREATE TABLE TuongTacThuoc (
    MaTuongTac VARCHAR(20) PRIMARY KEY,
    MaThuoc1 VARCHAR(20) NOT NULL,
    MaThuoc2 VARCHAR(20) NOT NULL,
    MoTaTuongTac TEXT NOT NULL,
    MucDoNghiemTrong ENUM('Thap', 'TrungBinh', 'Cao', 'RatCao') NOT NULL,
    KhuyenNghi TEXT,
    TrangThai ENUM('HoatDong', 'Khoa') DEFAULT 'HoatDong',
    NgayTao DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (MaThuoc1) REFERENCES Thuoc(MaThuoc),
    FOREIGN KEY (MaThuoc2) REFERENCES Thuoc(MaThuoc),
    INDEX idx_thuoc1 (MaThuoc1),
    INDEX idx_thuoc2 (MaThuoc2),
    UNIQUE KEY uk_thuoc_pair (MaThuoc1, MaThuoc2)
) ENGINE=InnoDB;

-- 22. Thông báo hệ thống
CREATE TABLE ThongBao (
    MaThongBao VARCHAR(20) PRIMARY KEY,
    TieuDe NVARCHAR(200) NOT NULL,
    NoiDung TEXT NOT NULL,
    LoaiThongBao ENUM('ThongTin', 'CanhBao', 'KhanCap', 'HeThong') NOT NULL,
    DoUuTien ENUM('Thap', 'TrungBinh', 'Cao') DEFAULT 'TrungBinh',
    NguoiNhan VARCHAR(20) COMMENT 'NULL = gửi tất cả',
    DaDoc BOOLEAN DEFAULT FALSE,
    NgayDoc DATETIME,
    NgayTao DATETIME DEFAULT CURRENT_TIMESTAMP,
    NgayHetHan DATETIME,
    FOREIGN KEY (NguoiNhan) REFERENCES NhanVien(MaNhanVien),
    INDEX idx_nguoinhan (NguoiNhan),
    INDEX idx_dadoc (DaDoc),
    INDEX idx_ngaytao (NgayTao)
) ENGINE=InnoDB;

-- 23. Cấu hình hệ thống
CREATE TABLE CauHinhHeThong (
    MaCauHinh VARCHAR(20) PRIMARY KEY,
    TenCauHinh VARCHAR(100) NOT NULL UNIQUE,
    GiaTri VARCHAR(500) NOT NULL,
    MoTa NVARCHAR(300),
    KieuDuLieu ENUM('INT', 'DECIMAL', 'VARCHAR', 'BOOLEAN', 'DATE') DEFAULT 'VARCHAR',
    NgayTao DATETIME DEFAULT CURRENT_TIMESTAMP,
    NgayCapNhat DATETIME ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB;

-- =====================================================
-- TRIGGERS
-- =====================================================

DELIMITER //

-- Trigger 1: Tự động cập nhật tồn kho khi nhập thuốc
CREATE TRIGGER trg_PhieuNhap_UpdateTonKho
AFTER INSERT ON ChiTietPhieuNhap
FOR EACH ROW
BEGIN
    UPDATE LoThuoc
    SET SoLuongCon = SoLuongCon + NEW.SoLuongNhap,
        TrangThai = 'ConHang'
    WHERE MaLo = NEW.MaLo;
END//

-- Trigger 2: Tự động trừ tồn kho khi xuất thuốc
CREATE TRIGGER trg_PhieuCap_UpdateTonKho
AFTER INSERT ON ChiTietPhieuCap
FOR EACH ROW
BEGIN
    DECLARE v_SoLuongCon INT;
    
    UPDATE LoThuoc
    SET SoLuongCon = SoLuongCon - NEW.SoLuongCap
    WHERE MaLo = NEW.MaLo;
    
    SELECT SoLuongCon INTO v_SoLuongCon
    FROM LoThuoc
    WHERE MaLo = NEW.MaLo;
    
    IF v_SoLuongCon <= 0 THEN
        UPDATE LoThuoc
        SET TrangThai = 'HetHang'
        WHERE MaLo = NEW.MaLo;
    END IF;
END//

-- Trigger 3: Tự động tính tổng tiền đơn thuốc
CREATE TRIGGER trg_DonThuoc_TinhTongTien
AFTER INSERT ON ChiTietDonThuoc
FOR EACH ROW
BEGIN
    UPDATE DonThuoc
    SET TongTien = (
        SELECT SUM(ThanhTien)
        FROM ChiTietDonThuoc
        WHERE MaDonThuoc = NEW.MaDonThuoc
    )
    WHERE MaDonThuoc = NEW.MaDonThuoc;
END//

-- Trigger 4: Tự động cập nhật trạng thái thanh toán
CREATE TRIGGER trg_PhieuThu_UpdateTrangThaiHD
AFTER INSERT ON PhieuThuTien
FOR EACH ROW
BEGIN
    IF NEW.TrangThai = 'DaXacNhan' THEN
        UPDATE HoaDon
        SET 
            TienDaTra = (
                SELECT COALESCE(SUM(SoTienThu), 0)
                FROM PhieuThuTien
                WHERE MaHoaDon = NEW.MaHoaDon AND TrangThai = 'DaXacNhan'
            ),
            TrangThaiThanhToan = CASE
                WHEN (
                    SELECT COALESCE(SUM(SoTienThu), 0)
                    FROM PhieuThuTien
                    WHERE MaHoaDon = NEW.MaHoaDon AND TrangThai = 'DaXacNhan'
                ) >= TienBenhNhanCanTra THEN 'DaTraDu'
                ELSE 'DaTra1Phan'
            END
        WHERE MaHoaDon = NEW.MaHoaDon;
    END IF;
END//

DELIMITER ;

-- =====================================================
-- STORED PROCEDURES - BỔ SUNG & TỐI ƯU
-- =====================================================

DELIMITER //

-- Procedure 1: Kiểm tra dị ứng thuốc
CREATE PROCEDURE sp_KiemTraDiUng(
    IN p_MaBenhNhan VARCHAR(20),
    IN p_MaDonThuoc VARCHAR(20)
)
BEGIN
    SELECT 
        t.TenThuoc,
        t.HoatChat,
        bn.TienSuDiUng,
        'CANH_BAO_DI_UNG' AS MucDo
    FROM ChiTietDonThuoc cdt
    JOIN Thuoc t ON cdt.MaThuoc = t.MaThuoc
    JOIN BenhNhan bn ON bn.MaBenhNhan = p_MaBenhNhan
    WHERE cdt.MaDonThuoc = p_MaDonThuoc
    AND bn.TienSuDiUng IS NOT NULL
    AND (
        bn.TienSuDiUng LIKE CONCAT('%', t.TenThuoc, '%') OR
        (t.HoatChat IS NOT NULL AND bn.TienSuDiUng LIKE CONCAT('%', t.HoatChat, '%'))
    );
END//

-- Procedure 2: Kiểm tra tương tác thuốc
CREATE PROCEDURE sp_KiemTraTuongTac(
    IN p_MaDonThuoc VARCHAR(20)
)
BEGIN
    SELECT DISTINCT
        t1.TenThuoc AS Thuoc1,
        t2.TenThuoc AS Thuoc2,
        tt.MoTaTuongTac,
        tt.MucDoNghiemTrong,
        tt.KhuyenNghi
    FROM ChiTietDonThuoc cdt1
    JOIN ChiTietDonThuoc cdt2 ON cdt1.MaDonThuoc = cdt2.MaDonThuoc 
    JOIN TuongTacThuoc tt ON (
        (tt.MaThuoc1 = cdt1.MaThuoc AND tt.MaThuoc2 = cdt2.MaThuoc) OR
        (tt.MaThuoc1 = cdt2.MaThuoc AND tt.MaThuoc2 = cdt1.MaThuoc)
    )
    JOIN Thuoc t1 ON cdt1.MaThuoc = t1.MaThuoc
    JOIN Thuoc t2 ON cdt2.MaThuoc = t2.MaThuoc
    WHERE cdt1.MaDonThuoc = p_MaDonThuoc
    AND cdt1.MaThuoc < cdt2.MaThuoc
    AND tt.TrangThai = 'HoatDong';
END//

-- Procedure 3: Xuất thuốc tự động FEFO
CREATE PROCEDURE sp_XuatThuoc_TuDong(
    IN p_MaPhieuCap VARCHAR(20),
    IN p_MaThuoc VARCHAR(20),
    IN p_SoLuong INT,
    IN p_MaKho VARCHAR(20)
)
BEGIN
    DECLARE v_SoLuongConLai INT DEFAULT p_SoLuong;
    DECLARE v_MaLo VARCHAR(20);
    DECLARE v_SoLuongCon INT;
    DECLARE v_DonGia DECIMAL(15,2);
    DECLARE v_SoLuongXuat INT;
    DECLARE done INT DEFAULT FALSE;
    
    DECLARE cur_Lo CURSOR FOR
        SELECT MaLo, SoLuongCon
        FROM LoThuoc
        WHERE MaThuoc = p_MaThuoc 
        AND MaKho = p_MaKho
        AND SoLuongCon > 0
        AND HanSuDung > CURDATE()
        ORDER BY HanSuDung ASC, NgayTao ASC;
    
    DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = TRUE;
    
    SELECT GiaXuat INTO v_DonGia FROM Thuoc WHERE MaThuoc = p_MaThuoc;
    
    OPEN cur_Lo;
    
    read_loop: LOOP
        FETCH cur_Lo INTO v_MaLo, v_SoLuongCon;
        
        IF done THEN
            LEAVE read_loop;
        END IF;
        
        IF v_SoLuongConLai <= 0 THEN
            LEAVE read_loop;
        END IF;
        
        SET v_SoLuongXuat = LEAST(v_SoLuongCon, v_SoLuongConLai);
        
        INSERT INTO ChiTietPhieuCap (MaPhieuCap, MaLo, SoLuongCap, DonGia)
        VALUES (p_MaPhieuCap, v_MaLo, v_SoLuongXuat, v_DonGia);
        
        SET v_SoLuongConLai = v_SoLuongConLai - v_SoLuongXuat;
    END LOOP;
    
    CLOSE cur_Lo;
    
    IF v_SoLuongConLai > 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Không đủ tồn kho để xuất thuốc';
    END IF;
END//

-- Procedure 4: Tính tiền hóa đơn tự động (BHYT)
CREATE PROCEDURE sp_TinhTienHoaDon(
    IN p_MaHoaDon VARCHAR(20)
)
BEGIN
    DECLARE v_MaBenhNhan VARCHAR(20);
    DECLARE v_MucHuong DECIMAL(5,2);
    DECLARE v_TongTien DECIMAL(15,2);
    DECLARE v_TienBHYT DECIMAL(15,2);
    
    SELECT MaBenhNhan INTO v_MaBenhNhan FROM HoaDon WHERE MaHoaDon = p_MaHoaDon;
    
    SELECT MucHuong INTO v_MucHuong
    FROM TheBHYT
    WHERE MaBenhNhan = v_MaBenhNhan
    AND TrangThai = 'ConHan'
    ORDER BY NgayHetHan DESC
    LIMIT 1;
    
    SET v_MucHuong = COALESCE(v_MucHuong, 0);
    
    SELECT SUM(cpc.ThanhTien) INTO v_TongTien
    FROM PhieuCapThuocNgoaiTru pc
    JOIN ChiTietPhieuCap cpc ON pc.MaPhieuCap = cpc.MaPhieuCap
    WHERE pc.MaHoaDon = p_MaHoaDon;
    
    SELECT SUM(cpc.ThanhTien * v_MucHuong / 100) INTO v_TienBHYT
    FROM PhieuCapThuocNgoaiTru pc
    JOIN ChiTietPhieuCap cpc ON pc.MaPhieuCap = cpc.MaPhieuCap
    JOIN LoThuoc lt ON cpc.MaLo = lt.MaLo
    JOIN Thuoc t ON lt.MaThuoc = t.MaThuoc
    WHERE pc.MaHoaDon = p_MaHoaDon
    AND t.LaThuocBHYT = 'Yes';
    
    SET v_TienBHYT = COALESCE(v_TienBHYT, 0);
    
    UPDATE HoaDon
    SET 
        TongTien = v_TongTien,
        TienBHYTChiTra = v_TienBHYT,
        TienBenhNhanCanTra = v_TongTien - v_TienBHYT
    WHERE MaHoaDon = p_MaHoaDon;
END//

-- Procedure 5: Cảnh báo thuốc tồn kho thấp
CREATE PROCEDURE sp_CanhBaoTonKhoThap()
BEGIN
    SELECT 
        t.MaThuoc,
        t.TenThuoc,
        t.TonKhoToiThieu,
        COALESCE(SUM(lt.SoLuongCon), 0) AS TonKhoHienTai,
        (t.TonKhoToiThieu - COALESCE(SUM(lt.SoLuongCon), 0)) AS SoLuongCanDat
    FROM Thuoc t
    LEFT JOIN LoThuoc lt ON t.MaThuoc = lt.MaThuoc AND lt.SoLuongCon > 0
    WHERE t.TrangThai = 'KichHoat'
    GROUP BY t.MaThuoc
    HAVING TonKhoHienTai < t.TonKhoToiThieu
    ORDER BY SoLuongCanDat DESC;
END//

-- Procedure 6: Kiểm tra tồn kho (FEFO)
CREATE PROCEDURE sp_KiemTraTonKho(
    IN p_MaThuoc VARCHAR(20),
    IN p_MaKho VARCHAR(20)
)
BEGIN
    SELECT 
        lt.MaLo,
        lt.SoLo,
        lt.HanSuDung,
        lt.SoLuongCon,
        lt.TrangThai,
        DATEDIFF(lt.HanSuDung, CURDATE()) AS SoNgayConLai,
        CASE
            WHEN DATEDIFF(lt.HanSuDung, CURDATE()) < 0 THEN 'HET_HAN'
            WHEN DATEDIFF(lt.HanSuDung, CURDATE()) <= 30 THEN 'CANH_BAO_CAO'
            WHEN DATEDIFF(lt.HanSuDung, CURDATE()) <= 90 THEN 'CANH_BAO'
            ELSE 'BINH_THUONG'
        END AS MucDoCanhBao
    FROM LoThuoc lt
    WHERE lt.MaThuoc = p_MaThuoc 
    AND lt.MaKho = p_MaKho
    AND lt.SoLuongCon > 0
    ORDER BY lt.HanSuDung ASC, lt.NgayTao ASC;
END//

-- Procedure 7: Báo cáo thuốc sắp hết hạn
CREATE PROCEDURE sp_BaoCaoThuocSapHetHan(
    IN p_SoNgay INT
)
BEGIN
    SELECT 
        lt.MaLo,
        t.TenThuoc,
        lt.SoLo,
        k.TenKho,
        lt.HanSuDung,
        lt.SoLuongCon,
        DATEDIFF(lt.HanSuDung, CURDATE()) AS SoNgayConLai,
        (lt.SoLuongCon * t.GiaNhap) AS GiaTriTonKho
    FROM LoThuoc lt
    JOIN Thuoc t ON lt.MaThuoc = t.MaThuoc
    JOIN Kho k ON lt.MaKho = k.MaKho
    WHERE lt.HanSuDung <= DATE_ADD(CURDATE(), INTERVAL p_SoNgay DAY)
    AND lt.HanSuDung >= CURDATE()
    AND lt.SoLuongCon > 0
    ORDER BY lt.HanSuDung ASC;
END//

-- Procedure 8: Xác nhận thanh toán VietQR
CREATE PROCEDURE sp_XacNhanThanhToanVietQR(
    IN p_MaGiaoDichNH VARCHAR(50),
    IN p_SoTienNhan DECIMAL(15,2),
    IN p_NoiDung NVARCHAR(200),
    IN p_DuLieuWebhook TEXT
)
BEGIN
    DECLARE v_MaQR VARCHAR(20);
    DECLARE v_MaHoaDon VARCHAR(20);
    DECLARE v_MaPhieuThu VARCHAR(20);
    
    SELECT MaQR, MaHoaDon INTO v_MaQR, v_MaHoaDon
    FROM VietQR_GiaoDich
    WHERE NoiDungChuyenKhoan = p_NoiDung 
    AND TrangThai = 'ChoThanhToan'
    LIMIT 1;
    
    IF v_MaQR IS NOT NULL THEN
        UPDATE VietQR_GiaoDich
        SET 
            TrangThai = 'DaThanhToan',
            MaGiaoDichNganHang = p_MaGiaoDichNH,
            NgayThanhToan = NOW(),
            SoTienNhan = p_SoTienNhan,
            DuLieuWebhook = p_DuLieuWebhook
        WHERE MaQR = v_MaQR;
        
        SET v_MaPhieuThu = CONCAT('PT', DATE_FORMAT(NOW(), '%Y%m%d%H%i%s'));
        
        INSERT INTO PhieuThuTien (
            MaPhieuThu, MaHoaDon, NgayThu, SoTienThu,
            HinhThucThanhToan, MaGiaoDichNganHang, 
            NhanVienThu, TrangThai
        ) VALUES (
            v_MaPhieuThu, v_MaHoaDon, NOW(), p_SoTienNhan,
            'VietQR', p_MaGiaoDichNH, 'SYSTEM', 'DaXacNhan'
        );
    END IF;
END//

DELIMITER ;

-- =====================================================
-- VIEWS - BỔ SUNG
-- =====================================================

-- View 1: Tổng hợp công nợ
CREATE VIEW v_CongNo AS
SELECT 
    hd.MaHoaDon,
    hd.MaBenhNhan,
    bn.TenBenhNhan,
    bn.SoDienThoai,
    hd.NgayTaoHoaDon,
    hd.TongTien,
    hd.TienBenhNhanCanTra,
    hd.TienDaTra,
    (hd.TienBenhNhanCanTra - hd.TienDaTra) AS TienConNo,
    hd.TrangThaiThanhToan
FROM HoaDon hd
JOIN BenhNhan bn ON hd.MaBenhNhan = bn.MaBenhNhan
WHERE hd.TrangThaiThanhToan IN ('ChuaTra', 'DaTra1Phan');

-- View 2: Thuốc tồn kho
CREATE VIEW v_TonKhoThuoc AS
SELECT 
    t.MaThuoc,
    t.TenThuoc,
    t.NhomThuoc,
    k.TenKho,
    SUM(lt.SoLuongCon) AS TongSoLuong,
    MIN(lt.HanSuDung) AS HanSuDungGanNhat,
    SUM(lt.SoLuongCon * t.GiaNhap) AS GiaTriTonKho,
    t.TonKhoToiThieu,
    CASE
        WHEN SUM(lt.SoLuongCon) < t.TonKhoToiThieu THEN 'CAN_DAT_HANG'
        ELSE 'DU_TON_KHO'
    END AS TrangThaiTonKho
FROM Thuoc t
LEFT JOIN LoThuoc lt ON t.MaThuoc = lt.MaThuoc AND lt.SoLuongCon > 0
JOIN Kho k ON lt.MaKho = k.MaKho
WHERE t.TrangThai = 'KichHoat'
GROUP BY t.MaThuoc, t.TenThuoc, k.MaKho, k.TenKho;

-- View 3: Thuốc hết hạn/sắp hết hạn
CREATE VIEW v_ThuocHetHan AS
SELECT 
    lt.MaLo,
    t.TenThuoc,
    lt.SoLo,
    k.TenKho,
    lt.HanSuDung,
    lt.SoLuongCon,
    DATEDIFF(lt.HanSuDung, CURDATE()) AS SoNgayConLai,
    (lt.SoLuongCon * t.GiaNhap) AS GiaTriTonKho,
    CASE
        WHEN DATEDIFF(lt.HanSuDung, CURDATE()) < 0 THEN 'HET_HAN'
        WHEN DATEDIFF(lt.HanSuDung, CURDATE()) <= 30 THEN 'CANH_BAO_CAO'
        WHEN DATEDIFF(lt.HanSuDung, CURDATE()) <= 90 THEN 'CANH_BAO'
        ELSE 'BINH_THUONG'
    END AS MucDo
FROM LoThuoc lt
JOIN Thuoc t ON lt.MaThuoc = t.MaThuoc
JOIN Kho k ON lt.MaKho = k.MaKho
WHERE lt.SoLuongCon > 0
ORDER BY lt.HanSuDung ASC;

-- View 4: Doanh thu theo ngày
CREATE VIEW v_DoanhThuTheoNgay AS
SELECT 
    DATE(hd.NgayTaoHoaDon) AS Ngay,
    COUNT(DISTINCT hd.MaHoaDon) AS SoHoaDon,
    COUNT(DISTINCT hd.MaBenhNhan) AS SoBenhNhan,
    SUM(hd.TongTien) AS TongDoanhThu,
    SUM(hd.TienBHYTChiTra) AS TongBHYTChiTra,
    SUM(hd.TienBenhNhanCanTra) AS TongBenhNhanTra
FROM HoaDon hd
GROUP BY DATE(hd.NgayTaoHoaDon)
ORDER BY Ngay DESC;

-- View 5: Top thuốc bán chạy
CREATE VIEW v_TopThuocBanChay AS
SELECT 
    t.MaThuoc,
    t.TenThuoc,
    t.NhomThuoc,
    COUNT(DISTINCT cpc.MaPhieuCap) AS SoLanXuat,
    SUM(cpc.SoLuongCap) AS TongSoLuongBan,
    SUM(cpc.ThanhTien) AS TongDoanhThu
FROM Thuoc t
JOIN LoThuoc lt ON t.MaThuoc = lt.MaThuoc
JOIN ChiTietPhieuCap cpc ON lt.MaLo = cpc.MaLo
GROUP BY t.MaThuoc
ORDER BY TongDoanhThu DESC;

-- View 6: Thông báo chưa đọc
CREATE VIEW v_ThongBaoChuaDoc AS
SELECT 
    tb.MaThongBao,
    tb.TieuDe,
    tb.NoiDung,
    tb.LoaiThongBao,
    tb.DoUuTien,
    tb.NguoiNhan,
    nv.TenNhanVien AS TenNguoiNhan,
    tb.NgayTao
FROM ThongBao tb
LEFT JOIN NhanVien nv ON tb.NguoiNhan = nv.MaNhanVien
WHERE tb.DaDoc = FALSE
AND (tb.NgayHetHan IS NULL OR tb.NgayHetHan >= NOW())
ORDER BY tb.DoUuTien DESC, tb.NgayTao DESC;

-- =====================================================
-- DỮ LIỆU MẪU
-- =====================================================

-- Cấu hình hệ thống
INSERT INTO CauHinhHeThong VALUES
('CFG001', 'TonKhoToiThieu', '10', 'Số lượng tồn kho tối thiểu mặc định', 'INT', NOW(), NOW()),
('CFG002', 'SoNgayHetHan', '90', 'Cảnh báo thuốc hết hạn trong vòng X ngày', 'INT', NOW(), NOW()),
('CFG003', 'TyLeChietKhauBHYT', '80', 'Tỷ lệ BHYT chi trả mặc định (%)', 'DECIMAL', NOW(), NOW()),
('CFG004', 'SoNgayHanThanhToan', '7', 'Số ngày cho phép trả sau', 'INT', NOW(), NOW()),
('CFG005', 'ChoPhepTraThuoc', 'true', 'Cho phép bệnh nhân trả thuốc', 'BOOLEAN', NOW(), NOW()),
('CFG006', 'ThoiGianHieuLucQR', '24', 'Số giờ hiệu lực mã QR thanh toán', 'INT', NOW(), NOW());

-- Cấu hình VietQR
INSERT INTO CauHinhVietQR VALUES
('CFG001', N'Ngân hàng TMCP Ngoại thương Việt Nam', 
 '1234567890', N'BENH VIEN X', '970436', 'compact2', 
 NULL, NULL, NULL, 'KichHoat', N'Tài khoản chính', NOW(), NOW());

-- Kho
INSERT INTO Kho VALUES
('KHO001', N'Kho thuốc BHYT', 'BHYT', N'Tầng 1 - Khối A', NULL, 'HoatDong', NOW()),
('KHO002', N'Kho thuốc dịch vụ', 'DichVu', N'Tầng 1 - Khối B', NULL, 'HoatDong', NOW());

-- Khoa phòng
INSERT INTO KhoaPhong VALUES
('K001', N'Khoa Nội tổng hợp', N'BS. Nguyễn Văn A', '0901234567', 'HoatDong', NOW()),
('K002', N'Khoa Ngoại', N'BS. Trần Thị B', '0901234568', 'HoatDong', NOW()),
('K003', N'Khoa Nhi', N'BS. Lê Văn C', '0901234569', 'HoatDong', NOW());

-- =====================================================
-- KẾT THÚC PHIÊN BẢN TỐI ƯU
-- Tổng số bảng: 23 (giảm 36% so với bản gốc 36 bảng)
-- Triggers: 4
-- Procedures: 8
-- Views: 6
-- =====================================================

-- GHI CHÚ QUAN TRỌNG:
-- 1. Đã BỔ SUNG 15 thuộc tính thiếu:
--    - Thuoc: HoatChat, DuongDung, NhomThuoc, TonKhoToiThieu
--    - BenhNhan: NhomMau, NgheNghiep, CanNang, ChieuCao, TienSuDiUng
--    - DonThuoc: TongTien, ChanDoanSoBo, GhiChuBacSi
--    - ChiTietDonThuoc: LieuDung
--    - HoaDon: MaSoThue, HinhThucThanhToan
--
-- 2. Đã GỘP/XÓA 13 bảng:
--    - Xóa: HoSoNhapVien, PhieuTraThuocNoiTru, LichSuQuetBHYT,
--            PhieuTraThuoc, ChiTietPhieuTra, VietQR_WebhookLog,
--            LichSuTraCuu (VNeID), CauHinhTichHop (VNeID)
--    - Gộp: BacSi → NhanVien
--    - Gộp: Benh + ChanDoan → ChanDoan
--
-- 3. PHÙ HỢP CHO:
--    - Bệnh viện nhỏ 20-50 giường
--    - Tập trung NGOẠI TRÚ
--    - Triển khai nhanh 2 ngày