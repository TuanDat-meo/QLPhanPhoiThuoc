-- =====================================================
-- MODULE VNEID MOCK - MÔ PHỎNG TÍCH HỢP
-- Phiên bản: 1.0
-- Tổng số bảng: 4
-- File: 2/2 - DATABASE VNEID MOCK
-- =====================================================

-- Tạo database riêng cho VNeID Mock
CREATE DATABASE IF NOT EXISTS VNeID_Mock_DB
CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

USE VNeID_Mock_DB;

-- =====================================================
-- BẢNG 1: CÔNG DÂN
-- Mô phỏng dữ liệu từ hệ thống VNeID
-- =====================================================

CREATE TABLE CongDan (
    SoDinhDanh VARCHAR(12) PRIMARY KEY COMMENT 'Số CCCD/CMND',
    HoTen NVARCHAR(100) NOT NULL,
    NgaySinh DATE NOT NULL,
    GioiTinh ENUM('Nam', 'Nu', 'Khac') NOT NULL,
    QueQuan NVARCHAR(200),
    NoiThuongTru NVARCHAR(300),
    DiaChiHienTai NVARCHAR(300),
    DanToc NVARCHAR(50),
    TonGiao NVARCHAR(50),
    QuocTich NVARCHAR(50) DEFAULT N'Việt Nam',
    NgayCap DATE,
    NoiCap NVARCHAR(200),
    NgayHetHan DATE,
    AnhChanDung TEXT COMMENT 'Base64 hoặc URL ảnh',
    TrangThai ENUM('HoatDong', 'Mat', 'Huy', 'TamKhoa') DEFAULT 'HoatDong',
    NgayTao DATETIME DEFAULT CURRENT_TIMESTAMP,
    NgayCapNhat DATETIME ON UPDATE CURRENT_TIMESTAMP,
    INDEX idx_hoten (HoTen),
    INDEX idx_ngaysinh (NgaySinh)
) ENGINE=InnoDB COMMENT='Dữ liệu công dân mô phỏng từ VNeID';

-- =====================================================
-- BẢNG 2: THẺ BHYT
-- Mô phỏng dữ liệu từ cổng BHXH
-- =====================================================

CREATE TABLE TheBHYT_Mock (
    MaThe VARCHAR(20) PRIMARY KEY,
    SoDinhDanh VARCHAR(12) NOT NULL,
    SoTheBHYT VARCHAR(15) NOT NULL UNIQUE COMMENT 'Mã thẻ BHYT 15 ký tự',
    NgayBatDau DATE NOT NULL,
    NgayHetHan DATE NOT NULL,
    MucHuong DECIMAL(5,2) NOT NULL COMMENT '80, 95, 100',
    NoiDKKCB NVARCHAR(200) COMMENT 'Nơi đăng ký khám chữa bệnh ban đầu',
    MaNoiDKKCB VARCHAR(10) COMMENT 'Mã BV đăng ký',
    DiaChi5Nam NVARCHAR(300) COMMENT 'Nơi cư trú 5 năm liên tục',
    MaKhuVuc VARCHAR(5) COMMENT 'K1, K2, K3',
    TrangThai ENUM('ConHan', 'HetHan', 'TamKhoa', 'Khoa') DEFAULT 'ConHan',
    NgayTao DATETIME DEFAULT CURRENT_TIMESTAMP,
    NgayCapNhat DATETIME ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (SoDinhDanh) REFERENCES CongDan(SoDinhDanh),
    INDEX idx_sothe (SoTheBHYT),
    INDEX idx_hethan (NgayHetHan),
    INDEX idx_sodinhhanh (SoDinhDanh)
) ENGINE=InnoDB COMMENT='Dữ liệu thẻ BHYT mô phỏng';

-- =====================================================
-- BẢNG 3: LỊCH SỬ TRA CỨU
-- Audit log cho mọi truy vấn từ hệ thống bên ngoài
-- =====================================================

CREATE TABLE LichSuTraCuu (
    MaTraCuu VARCHAR(20) PRIMARY KEY,
    SoDinhDanh VARCHAR(12) NOT NULL,
    LoaiTraCuu ENUM('ThongTinCoBan', 'ThongTinBHYT', 'ThongTinDayDu') NOT NULL,
    HeThongTraCuu VARCHAR(50) NOT NULL COMMENT 'Tên hệ thống yêu cầu',
    IPAddress VARCHAR(45),
    UserAgent VARCHAR(500),
    NgayGioTraCuu DATETIME NOT NULL,
    KetQua ENUM('ThanhCong', 'KhongTimThay', 'Loi') NOT NULL,
    DuLieuTraVe TEXT COMMENT 'JSON response',
    ThoiGianXuLy INT COMMENT 'Milliseconds',
    GhiChu NVARCHAR(500),
    FOREIGN KEY (SoDinhDanh) REFERENCES CongDan(SoDinhDanh),
    INDEX idx_ngaygio (NgayGioTraCuu),
    INDEX idx_hethong (HeThongTraCuu),
    INDEX idx_sodinhhanh (SoDinhDanh)
) ENGINE=InnoDB COMMENT='Lịch sử tra cứu từ các hệ thống';

-- =====================================================
-- BẢNG 4: CẤU HÌNH TÍCH HỢP
-- Quản lý API Key và quyền truy cập
-- =====================================================

CREATE TABLE CauHinhTichHop (
    MaCauHinh VARCHAR(20) PRIMARY KEY,
    TenHeThong NVARCHAR(100) NOT NULL,
    APIKey VARCHAR(255) NOT NULL UNIQUE,
    APISecret VARCHAR(255),
    IPWhitelist TEXT COMMENT 'Danh sách IP được phép, cách nhau bởi dấu phẩy',
    SoLanTraCuuToiDa INT DEFAULT 1000 COMMENT 'Giới hạn số lần tra cứu/ngày',
    TrangThai ENUM('KichHoat', 'TamDung', 'Khoa') DEFAULT 'KichHoat',
    NgayTao DATETIME DEFAULT CURRENT_TIMESTAMP,
    NgayCapNhat DATETIME ON UPDATE CURRENT_TIMESTAMP,
    INDEX idx_apikey (APIKey)
) ENGINE=InnoDB COMMENT='Cấu hình cho các hệ thống tích hợp';

-- =====================================================
-- STORED PROCEDURES - API MÔ PHỎNG
-- =====================================================

DELIMITER //

-- Procedure 1: Tra cứu thông tin công dân
CREATE PROCEDURE sp_TraCuuCongDan(
    IN p_SoDinhDanh VARCHAR(12),
    IN p_HeThong VARCHAR(50),
    IN p_IPAddress VARCHAR(45),
    OUT p_KetQua VARCHAR(20),
    OUT p_Message NVARCHAR(200)
)
BEGIN
    DECLARE v_MaTraCuu VARCHAR(20);
    DECLARE v_Exist INT;
    
    SET v_MaTraCuu = CONCAT('TC', DATE_FORMAT(NOW(), '%Y%m%d%H%i%s'), LPAD(FLOOR(RAND() * 100), 2, '0'));
    
    SELECT COUNT(*) INTO v_Exist
    FROM CongDan
    WHERE SoDinhDanh = p_SoDinhDanh AND TrangThai = 'HoatDong';
    
    IF v_Exist > 0 THEN
        SET p_KetQua = 'ThanhCong';
        SET p_Message = N'Tra cứu thành công';
        
        INSERT INTO LichSuTraCuu (
            MaTraCuu, SoDinhDanh, LoaiTraCuu, HeThongTraCuu,
            IPAddress, NgayGioTraCuu, KetQua
        ) VALUES (
            v_MaTraCuu, p_SoDinhDanh, 'ThongTinCoBan', p_HeThong,
            p_IPAddress, NOW(), 'ThanhCong'
        );
        
        SELECT 
            SoDinhDanh,
            HoTen,
            NgaySinh,
            GioiTinh,
            QueQuan,
            NoiThuongTru,
            DiaChiHienTai,
            NgayCap,
            NoiCap,
            NgayHetHan
        FROM CongDan
        WHERE SoDinhDanh = p_SoDinhDanh;
        
    ELSE
        SET p_KetQua = 'KhongTimThay';
        SET p_Message = N'Không tìm thấy thông tin công dân';
        
        INSERT INTO LichSuTraCuu (
            MaTraCuu, SoDinhDanh, LoaiTraCuu, HeThongTraCuu,
            IPAddress, NgayGioTraCuu, KetQua
        ) VALUES (
            v_MaTraCuu, p_SoDinhDanh, 'ThongTinCoBan', p_HeThong,
            p_IPAddress, NOW(), 'KhongTimThay'
        );
    END IF;
END//

-- Procedure 2: Tra cứu thông tin BHYT
CREATE PROCEDURE sp_TraCuuBHYT(
    IN p_SoDinhDanh VARCHAR(12),
    IN p_HeThong VARCHAR(50),
    OUT p_KetQua VARCHAR(20),
    OUT p_Message NVARCHAR(200)
)
BEGIN
    DECLARE v_MaTraCuu VARCHAR(20);
    DECLARE v_Exist INT;
    
    SET v_MaTraCuu = CONCAT('BHYT', DATE_FORMAT(NOW(), '%Y%m%d%H%i%s'));
    
    SELECT COUNT(*) INTO v_Exist
    FROM TheBHYT_Mock
    WHERE SoDinhDanh = p_SoDinhDanh 
    AND TrangThai = 'ConHan'
    AND NgayHetHan >= CURDATE();
    
    IF v_Exist > 0 THEN
        SET p_KetQua = 'ThanhCong';
        SET p_Message = N'Thẻ BHYT còn hiệu lực';
        
        INSERT INTO LichSuTraCuu (
            MaTraCuu, SoDinhDanh, LoaiTraCuu, HeThongTraCuu,
            NgayGioTraCuu, KetQua
        ) VALUES (
            v_MaTraCuu, p_SoDinhDanh, 'ThongTinBHYT', p_HeThong,
            NOW(), 'ThanhCong'
        );
        
        SELECT 
            tb.SoTheBHYT,
            tb.NgayBatDau,
            tb.NgayHetHan,
            tb.MucHuong,
            tb.NoiDKKCB,
            tb.MaNoiDKKCB,
            tb.DiaChi5Nam,
            tb.MaKhuVuc,
            c.HoTen,
            c.NgaySinh
        FROM TheBHYT_Mock tb
        JOIN CongDan c ON tb.SoDinhDanh = c.SoDinhDanh
        WHERE tb.SoDinhDanh = p_SoDinhDanh
        AND tb.TrangThai = 'ConHan';
        
    ELSE
        SET p_KetQua = 'KhongTimThay';
        SET p_Message = N'Không tìm thấy thẻ BHYT hoặc đã hết hạn';
        
        INSERT INTO LichSuTraCuu (
            MaTraCuu, SoDinhDanh, LoaiTraCuu, HeThongTraCuu,
            NgayGioTraCuu, KetQua
        ) VALUES (
            v_MaTraCuu, p_SoDinhDanh, 'ThongTinBHYT', p_HeThong,
            NOW(), 'KhongTimThay'
        );
    END IF;
END//

-- Procedure 3: Tra cứu đầy đủ (Công dân + BHYT)
CREATE PROCEDURE sp_TraCuuDayDu(
    IN p_SoDinhDanh VARCHAR(12),
    IN p_HeThong VARCHAR(50),
    IN p_IPAddress VARCHAR(45)
)
BEGIN
    DECLARE v_MaTraCuu VARCHAR(20);
    
    SET v_MaTraCuu = CONCAT('FULL', DATE_FORMAT(NOW(), '%Y%m%d%H%i%s'));
    
    INSERT INTO LichSuTraCuu (
        MaTraCuu, SoDinhDanh, LoaiTraCuu, HeThongTraCuu,
        IPAddress, NgayGioTraCuu, KetQua
    ) VALUES (
        v_MaTraCuu, p_SoDinhDanh, 'ThongTinDayDu', p_HeThong,
        p_IPAddress, NOW(), 'ThanhCong'
    );
    
    SELECT 
        c.SoDinhDanh,
        c.HoTen,
        c.NgaySinh,
        c.GioiTinh,
        c.NoiThuongTru,
        c.DiaChiHienTai,
        c.NgayCap,
        c.NoiCap,
        c.NgayHetHan AS NgayHetHanCCCD,
        tb.SoTheBHYT,
        tb.NgayBatDau AS NgayBatDauBHYT,
        tb.NgayHetHan AS NgayHetHanBHYT,
        tb.MucHuong,
        tb.NoiDKKCB,
        tb.MaNoiDKKCB,
        tb.DiaChi5Nam,
        tb.TrangThai AS TrangThaiBHYT
    FROM CongDan c
    LEFT JOIN TheBHYT_Mock tb ON c.SoDinhDanh = tb.SoDinhDanh
    WHERE c.SoDinhDanh = p_SoDinhDanh;
END//

-- Procedure 4: Kiểm tra API Key
CREATE PROCEDURE sp_KiemTraAPIKey(
    IN p_APIKey VARCHAR(255),
    OUT p_HopLe BOOLEAN,
    OUT p_TenHeThong NVARCHAR(100)
)
BEGIN
    DECLARE v_TrangThai VARCHAR(20);
    
    SELECT TenHeThong, TrangThai INTO p_TenHeThong, v_TrangThai
    FROM CauHinhTichHop
    WHERE APIKey = p_APIKey;
    
    IF v_TrangThai = 'KichHoat' THEN
        SET p_HopLe = TRUE;
    ELSE
        SET p_HopLe = FALSE;
    END IF;
END//

DELIMITER ;

-- =====================================================
-- DỮ LIỆU MẪU - CÔNG DÂN
-- =====================================================

INSERT INTO CongDan VALUES
('001234567890', N'Nguyễn Văn An', '1990-05-15', 'Nam', 
 N'Hà Nội', N'Số 10 Trần Hưng Đạo, Hoàn Kiếm, Hà Nội', 
 N'Số 10 Trần Hưng Đạo, Hoàn Kiếm, Hà Nội',
 N'Kinh', NULL, N'Việt Nam', '2020-01-15', 
 N'Cục Cảnh sát ĐKQL cư trú và DLQG về dân cư',
 '2035-01-15', NULL, 'HoatDong', NOW(), NOW()),

('001234567891', N'Trần Thị Bình', '1985-08-20', 'Nu',
 N'TP. Hồ Chí Minh', N'123 Nguyễn Huệ, Quận 1, TP.HCM',
 N'123 Nguyễn Huệ, Quận 1, TP.HCM',
 N'Kinh', NULL, N'Việt Nam', '2019-03-10', 
 N'Cục Cảnh sát ĐKQL cư trú và DLQG về dân cư',
 '2034-03-10', NULL, 'HoatDong', NOW(), NOW()),

('001234567892', N'Lê Hoàng Cường', '1995-12-01', 'Nam',
 N'Đà Nẵng', N'45 Trần Phú, Hải Châu, Đà Nẵng',
 N'45 Trần Phú, Hải Châu, Đà Nẵng',
 N'Kinh', NULL, N'Việt Nam', '2021-06-20', 
 N'Cục Cảnh sát ĐKQL cư trú và DLQG về dân cư',
 '2036-06-20', NULL, 'HoatDong', NOW(), NOW()),

('001234567893', N'Phạm Thị Dung', '1992-03-25', 'Nu',
 N'Hải Phòng', N'78 Lê Lợi, Hồng Bàng, Hải Phòng',
 N'78 Lê Lợi, Hồng Bàng, Hải Phòng',
 N'Kinh', NULL, N'Việt Nam', '2020-07-15', 
 N'Cục Cảnh sát ĐKQL cư trú và DLQG về dân cư',
 '2035-07-15', NULL, 'HoatDong', NOW(), NOW()),

('001234567894', N'Hoàng Văn Em', '1988-11-10', 'Nam',
 N'Cần Thơ', N'56 Trần Hưng Đạo, Ninh Kiều, Cần Thơ',
 N'56 Trần Hưng Đạo, Ninh Kiều, Cần Thơ',
 N'Kinh', NULL, N'Việt Nam', '2019-12-20', 
 N'Cục Cảnh sát ĐKQL cư trú và DLQG về dân cư',
 '2034-12-20', NULL, 'HoatDong', NOW(), NOW());

-- =====================================================
-- DỮ LIỆU MẪU - THẺ BHYT
-- =====================================================

INSERT INTO TheBHYT_Mock VALUES
('BHYT001', '001234567890', 'DN4961234567890', '2024-01-01', '2025-12-31',
 80.00, N'Bệnh viện Bạch Mai', '01001', 
 N'Số 10 Trần Hưng Đạo, Hoàn Kiếm, Hà Nội',
 'K1', 'ConHan', NOW(), NOW()),

('BHYT002', '001234567891', 'HC7851234567891', '2023-06-01', '2025-06-30',
 95.00, N'Bệnh viện Chợ Rẫy', '79001', 
 N'123 Nguyễn Huệ, Quận 1, TP.HCM',
 'K1', 'ConHan', NOW(), NOW()),

('BHYT003', '001234567892', 'DN4961234567892', '2024-03-01', '2026-02-28',
 100.00, N'Bệnh viện Đà Nẵng', '43001', 
 N'45 Trần Phú, Hải Châu, Đà Nẵng',
 'K2', 'ConHan', NOW(), NOW()),

('BHYT004', '001234567893', 'HP3121234567893', '2024-01-15', '2025-12-31',
 80.00, N'Bệnh viện Việt Tiệp', '31001', 
 N'78 Lê Lợi, Hồng Bàng, Hải Phòng',
 'K1', 'ConHan', NOW(), NOW()),

('BHYT005', '001234567894', 'CT9251234567894', '2023-09-01', '2025-08-31',
 95.00, N'Bệnh viện Đa khoa Trung ương Cần Thơ', '92001', 
 N'56 Trần Hưng Đạo, Ninh Kiều, Cần Thơ',
 'K2', 'ConHan', NOW(), NOW());

-- =====================================================
-- DỮ LIỆU MẪU - CẤU HÌNH TÍCH HỢP
-- =====================================================

INSERT INTO CauHinhTichHop VALUES
('CFG_BV_001', N'Hệ thống Bệnh viện X', 
 'bv_x_api_key_2025_secure_token_abc123', 
 'bv_x_secret_key_xyz789',
 '192.168.1.0/24,10.0.0.0/8,127.0.0.1', 
 5000, 'KichHoat', NOW(), NOW()),

('CFG_BV_002', N'Hệ thống Bệnh viện Y', 
 'bv_y_api_key_2025_secure_token_def456', 
 'bv_y_secret_key_uvw123',
 '172.16.0.0/16', 
 3000, 'KichHoat', NOW(), NOW());

-- =====================================================
-- VIEWS HỮU ÍCH
-- =====================================================

-- View 1: Thống kê tra cứu theo hệ thống
CREATE VIEW v_ThongKeTraCuu AS
SELECT 
    HeThongTraCuu,
    DATE(NgayGioTraCuu) AS NgayTraCuu,
    COUNT(*) AS SoLanTraCuu,
    SUM(CASE WHEN KetQua = 'ThanhCong' THEN 1 ELSE 0 END) AS SoLanThanhCong,
    SUM(CASE WHEN KetQua = 'KhongTimThay' THEN 1 ELSE 0 END) AS SoLanKhongTimThay,
    SUM(CASE WHEN KetQua = 'Loi' THEN 1 ELSE 0 END) AS SoLanLoi
FROM LichSuTraCuu
GROUP BY HeThongTraCuu, DATE(NgayGioTraCuu);

-- View 2: Danh sách thẻ BHYT sắp hết hạn
CREATE VIEW v_TheBHYTSapHetHan AS
SELECT 
    c.SoDinhDanh,
    c.HoTen,
    c.NgaySinh,
    tb.SoTheBHYT,
    tb.NgayHetHan,
    DATEDIFF(tb.NgayHetHan, CURDATE()) AS SoNgayConLai,
    tb.NoiDKKCB
FROM TheBHYT_Mock tb
JOIN CongDan c ON tb.SoDinhDanh = c.SoDinhDanh
WHERE tb.TrangThai = 'ConHan'
AND tb.NgayHetHan BETWEEN CURDATE() AND DATE_ADD(CURDATE(), INTERVAL 30 DAY)
ORDER BY tb.NgayHetHan ASC;

-- =====================================================
-- KẾT THÚC FILE 2/2 - DATABASE VNEID MOCK
-- Tổng số bảng: 4
-- Procedures: 4
-- Views: 2
-- =====================================================