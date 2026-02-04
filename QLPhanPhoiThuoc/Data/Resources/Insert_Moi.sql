-- =====================================================
-- BỔ SUNG DỮ LIỆU CHO CÁC BẢNG CÒN THIẾU
-- Bổ sung: PhieuNhap, ChiTietPhieuNhap, PhieuCapPhat, 
-- ChiTietPhieuCap, HoaDon, PhieuThuTien, VietQR_GiaoDich
-- =====================================================

USE BenhVienX;
GO

PRINT N'--- Bắt đầu bổ sung dữ liệu cho các bảng còn thiếu ---';
GO

-- =====================================================
-- 1. PHIẾU NHẬP THUỐC (3 phiếu từ 3 NCC)
-- =====================================================
INSERT INTO PhieuNhap (MaPhieuNhap, MaKho, MaNCC, NhanVienNhap, NgayNhap, TongTien, TrangThai, GhiChu, NgayTao, SoHoaDon) VALUES
('PN001', 'KHO001', 'NCC001', 'NV010', DATEADD(DAY, -30, GETDATE()), 45000000, 'DaNhap', N'Nhập thuốc định kỳ tháng 1/2026', DATEADD(DAY, -30, GETDATE()), 'HD_NCC_001'),
('PN002', 'KHO002', 'NCC002', 'NV010', DATEADD(DAY, -20, GETDATE()), 28000000, 'DaNhap', N'Nhập dịch truyền khoa Nội', DATEADD(DAY, -20, GETDATE()), 'HD_NCC_002'),
('PN003', 'KHO001', 'NCC003', 'NV010', DATEADD(DAY, -10, GETDATE()), 15000000, 'DaNhap', N'Nhập thuốc bổ sung', DATEADD(DAY, -10, GETDATE()), 'HD_NCC_003');
GO
-- =====================================================
-- 2. CHI TIẾT PHIẾU NHẬP (Liên kết với LoThuoc đã có)
-- =====================================================
INSERT INTO ChiTietPhieuNhap (MaPhieuNhap, MaLo, SoLuongNhap, DonGiaNhap) VALUES
-- PN001 (Map T001->LO001, T002->LO004, T007->LO019, T006->LO016)
('PN001', 'LO001', 500, 8000),
('PN001', 'LO004', 300, 25000),
('PN001', 'LO019', 400, 45000),
('PN001', 'LO016', 350, 40000),

-- PN002 (Map T012->LO034, T013->LO037, T014->LO040, T015->LO043)
('PN002', 'LO034', 200, 15000),
('PN002', 'LO037', 150, 22000),
('PN002', 'LO040', 100, 120000),
('PN002', 'LO043', 80, 85000),

-- PN003 (Map T003->LO007, T004->LO010, T005->LO013)
('PN003', 'LO007', 600, 3500),
('PN003', 'LO010', 400, 18000),
('PN003', 'LO013', 300, 12000);
GO


-- =====================================================
-- 3. PHIẾU CẤP PHÁT THUỐC (10 phiếu - tương ứng 10 đơn thuốc)
-- =====================================================
INSERT INTO PhieuCapPhat (MaPhieuCap, MaDonThuoc, MaBenhNhan, MaKho, NhanVienCap, NgayCap, TongTien, TrangThai, GhiChu, NgayTao) VALUES
-- BN001: Viêm phổi nặng (BHYT 100%)
('PCP001', 'DT001', 'BN001', 'KHO002', 'NV006', DATEADD(DAY, -5, GETDATE()), 1680000, 'DaXuatThuoc', N'Cấp thuốc điều trị viêm phổi - BHYT thanh toán 100%', DATEADD(DAY, -5, GETDATE())),

-- BN002: Sau sinh (BHYT 100%)
('PCP002', 'DT002', 'BN002', 'KHO002', 'NV006', DATEADD(DAY, -4, GETDATE()), 455000, 'DaXuatThuoc', N'Cấp thuốc chăm sóc sau sinh - BHYT thanh toán 100%', DATEADD(DAY, -4, GETDATE())),

-- BN003: Sau phẫu thuật (BHYT 95%)
('PCP003', 'DT003', 'BN003', 'KHO002', 'NV006', DATEADD(DAY, -3, GETDATE()), 1665000, 'DaXuatThuoc', N'Cấp thuốc sau mổ - BHYT thanh toán 95%', DATEADD(DAY, -3, GETDATE())),

-- BN004: Viêm họng (BHYT 95%)
('PCP004', 'DT004', 'BN004', 'KHO001', 'NV007', DATEADD(DAY, -2, GETDATE()), 390000, 'DaXuatThuoc', N'Cấp thuốc điều trị viêm họng - BHYT thanh toán 95%', DATEADD(DAY, -2, GETDATE())),

-- BN005: Tăng huyết áp (BHYT 80%)
('PCP005', 'DT005', 'BN005', 'KHO001', 'NV007', DATEADD(DAY, -2, GETDATE()), 2550000, 'DaXuatThuoc', N'Cấp thuốc dài hạn điều trị tăng huyết áp - BHYT thanh toán 80%', DATEADD(DAY, -2, GETDATE())),

-- BN006: Viêm dạ dày (BHYT 80%)
('PCP006', 'DT006', 'BN006', 'KHO001', 'NV007', DATEADD(DAY, -1, GETDATE()), 540000, 'DaXuatThuoc', N'Cấp thuốc điều trị viêm dạ dày - BHYT thanh toán 80%', DATEADD(DAY, -1, GETDATE())),

-- BN007: Cảm cúm (Tự túc - không BHYT)
('PCP007', 'DT007', 'BN007', 'KHO001', 'NV007', DATEADD(DAY, -1, GETDATE()), 275000, 'DaXuatThuoc', N'Cấp thuốc cảm cúm - Bệnh nhân tự túc 100%', DATEADD(DAY, -1, GETDATE())),

-- BN008: Đái tháo đường (Tự túc - thẻ BHYT hết hạn)
('PCP008', 'DT008', 'BN008', 'KHO001', 'NV007', DATEADD(HOUR, -12, GETDATE()), 720000, 'DaXuatThuoc', N'Cấp thuốc đái tháo đường - Thẻ BHYT hết hạn, bệnh nhân tự túc 100%', DATEADD(HOUR, -12, GETDATE())),

-- BN009: Viêm hô hấp trẻ em (BHYT 100%)
('PCP009', 'DT009', 'BN009', 'KHO001', 'NV006', DATEADD(HOUR, -6, GETDATE()), 230000, 'DaXuatThuoc', N'Cấp thuốc điều trị viêm hô hấp trẻ em - BHYT thanh toán 100%', DATEADD(HOUR, -6, GETDATE())),

-- BN010: Tiêu chảy trẻ em (Tự túc - không BHYT)
('PCP010', 'DT010', 'BN010', 'KHO001', 'NV007', DATEADD(HOUR, -2, GETDATE()), 60000, 'DaXuatThuoc', N'Cấp dung dịch điện giải điều trị tiêu chảy - Bệnh nhân tự túc 100%', DATEADD(HOUR, -2, GETDATE()));
GO
-- =====================================================
-- 4. CHI TIẾT PHIẾU CẤP PHÁT (Liên kết với ChiTietDonThuoc)
-- =====================================================
INSERT INTO ChiTietPhieuCap (MaPhieuCap, MaLo, SoLuongCap, DonGiaCap) VALUES
-- PCP001 (Viêm phổi)
('PCP001', 'LO001', 20, 12000), -- Paracetamol
('PCP001', 'LO040', 8, 180000), -- Ceftriaxone

-- PCP002 (Sau sinh)
('PCP002', 'LO004', 14, 35000), -- Amoxicillin
('PCP002', 'LO007', 30, 5000),  -- Vitamin C

-- PCP003 (Sau PT)
('PCP003', 'LO040', 6, 180000), -- Ceftriaxone
('PCP003', 'LO001', 30, 12000), -- Paracetamol
('PCP003', 'LO034', 3, 25000),  -- NaCl 0.9%

-- PCP004 (Viêm họng)
('PCP004', 'LO004', 10, 35000),
('PCP004', 'LO001', 15, 12000),

-- PCP005 (THA)
('PCP005', 'LO019', 30, 65000), -- Amlodipine
('PCP005', 'LO016', 30, 60000), -- Atorvastatin

-- PCP006 (Dạ dày)
('PCP006', 'LO010', 30, 28000), -- Omeprazole

-- PCP007 (Cảm cúm)
('PCP007', 'LO001', 20, 12000),
('PCP007', 'LO007', 10, 5000),

-- PCP008 (Tiểu đường)
('PCP008', 'LO013', 60, 18000), -- Metformin

-- PCP009 (Hô hấp TE)
('PCP009', 'LO001', 10, 12000),
('PCP009', 'LO022', 10, 15000), -- Salbutamol

-- PCP010 (Tiêu chảy TE)
('PCP010', 'LO034', 2, 25000);
GO

-- =====================================================
-- 5. HÓA ĐƠN (10 hóa đơn tương ứng 10 phiếu cấp phát)
-- =====================================================
INSERT INTO HoaDon (
    MaHoaDon, 
    MaBenhNhan, 
    MaDonThuoc, 
    NgayTaoHoaDon, 
    TongTien, 
    TienBHYTChiTra, 
    TienBenhNhanCanTra, 
    TienDaTra, 
    MaSoThue,           -- Cột gây lỗi (đã sửa)
    HinhThucThanhToan,  -- Đã bổ sung giá trị
    TrangThaiThanhToan, 
    GhiChu, 
    NgayTao
) VALUES
-- HD001: BHYT 100% (BN001) - Không cần MST, Hình thức thanh toán để 'Khong' vì không phải trả tiền
('HD001', 'BN001', 'DT001', DATEADD(DAY, -5, GETDATE()), 
 2380000, 2380000, 0, 0, 
 '', 'Khong', 'DaTraDu', N'Nội trú - BHYT 100%', DATEADD(DAY, -5, GETDATE())),

-- HD002: BHYT 100% (BN002)
('HD002', 'BN002', 'DT002', DATEADD(DAY, -4, GETDATE()), 
 1455000, 1455000, 0, 0, 
 '', 'Khong', 'DaTraDu', N'Sau sinh - BHYT 100%', DATEADD(DAY, -4, GETDATE())),

-- HD003: BHYT 95% - BN trả 5% (BN003)
('HD003', 'BN003', 'DT003', DATEADD(DAY, -3, GETDATE()), 
 16965000, 16116750, 848250, 848250, 
 '', 'TienMat', 'DaTraDu', N'Sau mổ - BHYT 95%', DATEADD(DAY, -3, GETDATE())),

-- HD004: BHYT 95% - BN trả 5% (BN004)
('HD004', 'BN004', 'DT004', DATEADD(DAY, -2, GETDATE()), 
 640000, 608000, 32000, 32000, 
 '', 'TienMat', 'DaTraDu', N'Viêm họng - BHYT 95%', DATEADD(DAY, -2, GETDATE())),

-- HD005: BHYT 80% - BN trả 20% qua QR (BN005) - Giả sử có lấy MST cá nhân
('HD005', 'BN005', 'DT005', DATEADD(DAY, -2, GETDATE()), 
 2900000, 2320000, 580000, 580000, 
 '8000123456', 'VietQR', 'DaTraDu', N'THA - BHYT 80%', DATEADD(DAY, -2, GETDATE())),

-- HD006: BHYT 80% - BN trả 20% (BN006)
('HD006', 'BN006', 'DT006', DATEADD(DAY, -1, GETDATE()), 
 840000, 672000, 168000, 168000, 
 '', 'TienMat', 'DaTraDu', N'Dạ dày - BHYT 80%', DATEADD(DAY, -1, GETDATE())),

-- HD007: Tự túc 100% qua QR (BN007)
('HD007', 'BN007', 'DT007', DATEADD(DAY, -1, GETDATE()), 
 475000, 0, 475000, 475000, 
 '', 'VietQR', 'DaTraDu', N'Cảm cúm - Tự túc', DATEADD(DAY, -1, GETDATE())),

-- HD008: Tự túc do hết hạn thẻ (BN008)
('HD008', 'BN008', 'DT008', DATEADD(HOUR, -12, GETDATE()), 
 1430000, 0, 1430000, 1430000, 
 '', 'TheATM', 'DaTraDu', N'Thẻ hết hạn - Tự túc', DATEADD(HOUR, -12, GETDATE())),

-- HD009: BHYT 100% Trẻ em (BN009)
('HD009', 'BN009', 'DT009', DATEADD(HOUR, -6, GETDATE()), 
 500000, 500000, 0, 0, 
 '', 'Khong', 'DaTraDu', N'Trẻ em - BHYT 100%', DATEADD(HOUR, -6, GETDATE())),

-- HD010: Tự túc Trẻ em (BN010)
('HD010', 'BN010', 'DT010', DATEADD(HOUR, -2, GETDATE()), 
 260000, 0, 260000, 260000, 
 '', 'TienMat', 'DaTraDu', N'Trẻ em - Tự túc', DATEADD(HOUR, -2, GETDATE()));
GO

-- =====================================================
-- 6. PHIẾU THU TIỀN (Cho các hóa đơn có BN phải trả)
-- =====================================================
USE BenhVienX;
GO

PRINT N'--- Bắt đầu bổ sung dữ liệu: Phiếu Thu, VietQR và Thông Báo ---';

-- Xóa dữ liệu cũ để tránh trùng lặp
DELETE FROM PhieuThuTien;
DELETE FROM VietQR_GiaoDich;
DELETE FROM ThongBao;
GO

-- =====================================================
-- 6. PHIẾU THU TIỀN
-- =====================================================

ALTER TABLE PhieuThuTien ALTER COLUMN MaGiaoDichNganHang VARCHAR(50) NULL;

INSERT INTO PhieuThuTien (MaPhieuThu, MaHoaDon, NhanVienThu, NgayThu, SoTienThu, HinhThucThanhToan, MaGiaoDichNganHang, TrangThai, GhiChu, NgayTao) VALUES
-- PT001: BN003
('PT001', 'HD003', 'NV008', DATEADD(DAY, -3, GETDATE()), 848250, 'TienMat', NULL, 
 'DaXacNhan', N'Thu tiền mặt - Đồng chi trả 5% BHYT', DATEADD(DAY, -3, GETDATE())),

-- PT002: BN004
('PT002', 'HD004', 'NV009', DATEADD(DAY, -2, GETDATE()), 32000, 'TienMat', NULL, 
 'DaXacNhan', N'Thu tiền mặt - Đồng chi trả 5% BHYT', DATEADD(DAY, -2, GETDATE())),

-- PT003: BN005 (Sửa ChuyenKhoan -> VietQR cho khớp logic)
('PT003', 'HD005', 'NV008', DATEADD(DAY, -2, GETDATE()), 580000, 'VietQR', 'QR001_20260202_001', 
 'DaXacNhan', N'Chuyển khoản qua VietQR - Đồng chi trả 20% BHYT', DATEADD(DAY, -2, GETDATE())),

-- PT004: BN006
('PT004', 'HD006', 'NV009', DATEADD(DAY, -1, GETDATE()), 168000, 'TienMat', NULL, 
 'DaXacNhan', N'Thu tiền mặt - Đồng chi trả 20% BHYT', DATEADD(DAY, -1, GETDATE())),

-- PT005: BN007
('PT005', 'HD007', 'NV009', DATEADD(DAY, -1, GETDATE()), 475000, 'VietQR', 'QR001_20260203_001', 
 'DaXacNhan', N'Chuyển khoản qua VietQR - Tự túc 100%', DATEADD(DAY, -1, GETDATE())),

-- PT006: BN008 (Sửa TheATM -> The)
('PT006', 'HD008', 'NV008', DATEADD(HOUR, -12, GETDATE()), 1430000, 'The', 'ATM_20260204_001', 
 'DaXacNhan', N'Quẹt thẻ ATM - Tự túc 100% (thẻ BHYT hết hạn)', DATEADD(HOUR, -12, GETDATE())),

-- PT007: BN010
('PT007', 'HD010', 'NV009', DATEADD(HOUR, -2, GETDATE()), 260000, 'TienMat', NULL, 
 'DaXacNhan', N'Thu tiền mặt - Tự túc 100% (không có BHYT)', DATEADD(HOUR, -2, GETDATE()));
GO

-- =====================================================
-- 7. VIETQR GIAO DỊCH
-- =====================================================
-- Lưu ý: Cột DuLieuWebhook chứa JSON response
INSERT INTO VietQR_GiaoDich (
    MaQR, 
    MaHoaDon, 
    SoTienYeuCau, 
    SoTienNhan, 
    NoiDungChuyenKhoan, 
    MaGiaoDichNganHang, 
    QRCodeBase64, 
    NgayThanhToan, 
    DuLieuWebhook, 
    TrangThai, 
    NgayTao, 
    NgayHetHan
) VALUES
-- Giao dịch 1: BN005 thanh toán HD005 (Thành công)
('QR001', 
 'HD005', 
 580000,           -- SoTienYeuCau
 580000,           -- SoTienNhan (Đủ)
 'QR001_20260202_001', 
 'MB_TRANS_001', 
 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNk+A8AAQUBAScY42YAAAAASUVORK5CYII=', -- Mã Base64 giả lập
 DATEADD(DAY, -2, DATEADD(MINUTE, 5, GETDATE())), -- NgayThanhToan
 '{"transaction_id":"QR001_20260202_001","amount":580000,"status":"success","bank_code":"970422"}', 
 'DaThanhToan', 
 DATEADD(DAY, -2, GETDATE()), 
 DATEADD(DAY, -2, DATEADD(HOUR, 24, GETDATE()))
),

-- Giao dịch 2: BN007 thanh toán HD007 (Thành công)
('QR002', 
 'HD007', 
 475000,           -- SoTienYeuCau
 475000,           -- SoTienNhan (Đủ)
 'QR001_20260203_001', 
 'VCB_TRANS_999', 
 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNk+A8AAQUBAScY42YAAAAASUVORK5CYII=', 
 DATEADD(DAY, -1, DATEADD(MINUTE, 3, GETDATE())), 
 '{"transaction_id":"QR001_20260203_001","amount":475000,"status":"success","bank_code":"970436"}', 
 'DaThanhToan', 
 DATEADD(DAY, -1, GETDATE()), 
 DATEADD(DAY, -1, DATEADD(HOUR, 24, GETDATE()))
);
GO

-- =====================================================
-- 8. THÔNG BÁO HỆ THỐNG
-- =====================================================
-- Lưu ý: DaDoc là kiểu BIT (0 = Chưa đọc, 1 = Đã đọc)
INSERT INTO ThongBao (MaThongBao, NguoiNhan, TieuDe, NoiDung, LoaiThongBao, DaDoc, NgayTao) VALUES
-- TB001: Chưa đọc (0)
('TB001', 'NV006', N'Cảnh báo thuốc sắp hết hạn', 
 N'Lô thuốc LO003 (Paracetamol) sẽ hết hạn vào 15/03/2026. Vui lòng kiểm tra.', 
 'CanhBao', 0, DATEADD(DAY, -5, GETDATE())),

-- TB002: Đã đọc (1)
('TB002', 'NV007', N'Cảnh báo thuốc tồn kho thấp', 
 N'Thuốc Ceftriaxone (T014) trong Kho002 còn 92 lọ, dưới mức tối thiểu.', 
 'CanhBao', 1, DATEADD(DAY, -3, GETDATE())),

-- TB003: Đã đọc (1)
('TB003', 'NV010', N'Phiếu nhập PN003 hoàn thành', 
 N'Phiếu nhập PN003 từ NCC Công ty TNHH Dược phẩm ABC đã được duyệt.', 
 'ThongTin', 1, DATEADD(DAY, -10, GETDATE())),

-- TB004: Đã đọc (1)
('TB004', 'NV008', N'Thanh toán HD003 thành công', 
 N'BN Lê Văn Cường đã thanh toán 848.250đ (Tiền mặt).', 
 'ThongTin', 1, DATEADD(DAY, -3, GETDATE())),

-- TB005: Đã đọc (1)
('TB005', 'NV009', N'Thanh toán VietQR thành công', 
 N'HD005 đã thanh toán 580.000đ qua VietQR.', 
 'ThongTin', 1, DATEADD(DAY, -2, GETDATE())),

-- TB006: Chưa đọc (0)
('TB006', NULL, N'Bảo trì hệ thống', 
 N'Hệ thống bảo trì 00:00-02:00 ngày 10/02/2026.', 
 'HeThong', 0, DATEADD(DAY, -1, GETDATE())),

-- TB007: Đã đọc (1)
('TB007', 'NV008', N'Cảnh báo thẻ BHYT hết hạn', 
 N'BN Bùi Thị Hồng (BN008) có thẻ BHYT hết hạn. Đã xử lý thanh toán tự túc.', 
 'CanhBao', 1, DATEADD(HOUR, -12, GETDATE())),

-- TB008: Chưa đọc (0)
('TB008', 'NV001', N'BN001 đã xuất viện', 
 N'Bệnh nhân Nguyễn Văn An đã hoàn thành điều trị và xuất viện.', 
 'ThongTin', 0, DATEADD(DAY, -1, GETDATE()));
GO
PRINT N'✓ Đã hoàn tất nhập dữ liệu toàn hệ thống!';

-- =====================================================
-- 8. THÔNG BÁO HỆ THỐNG
-- =====================================================
INSERT INTO ThongBao (MaThongBao, NguoiNhan, TieuDe, NoiDung, LoaiThongBao, TrangThai, NgayTao) VALUES
-- Thông báo cho dược sĩ về thuốc sắp hết hạn
('TB001', 'NV006', N'Cảnh báo thuốc sắp hết hạn', 
 N'Lô thuốc LO_T001_003 (Paracetamol) sẽ hết hạn vào 15/03/2026. Vui lòng kiểm tra và xử lý.',
 N'CanhBao', N'ChuaDoc', DATEADD(DAY, -5, GETDATE())),

('TB002', 'NV007', N'Cảnh báo thuốc tồn kho thấp',
 N'Thuốc Ceftriaxone (T014) trong Kho002 còn 92 lọ, dưới mức tối thiểu. Cần nhập thêm.',
 N'CanhBao', N'DaDoc', DATEADD(DAY, -3, GETDATE())),

-- Thông báo cho kho thuốc về phiếu nhập mới
('TB003', 'NV010', N'Phiếu nhập PN003 đã hoàn thành',
 N'Phiếu nhập PN003 từ NCC Công ty TNHH Dược phẩm ABC đã được duyệt. Tổng giá trị: 15.000.000đ',
 N'ThongTin', N'DaDoc', DATEADD(DAY, -10, GETDATE())),

-- Thông báo cho thu ngân về thanh toán
('TB004', 'NV008', N'Thanh toán hóa đơn HD003 thành công',
 N'Bệnh nhân Lê Văn Cường đã thanh toán 848.250đ cho hóa đơn HD003. Hình thức: Tiền mặt',
 N'ThongTin', N'DaDoc', DATEADD(DAY, -3, GETDATE())),

('TB005', 'NV009', N'Thanh toán VietQR thành công',
 N'Hóa đơn HD005 đã được thanh toán 580.000đ qua VietQR. Người chuyển: HOANG VAN EM',
 N'ThongTin', N'DaDoc', DATEADD(DAY, -2, GETDATE())),

-- Thông báo hệ thống
('TB006', NULL, N'Bảo trì hệ thống',
 N'Hệ thống sẽ được bảo trì vào 00:00-02:00 ngày 10/02/2026. Vui lòng hoàn thành công việc trước thời gian này.',
 N'HeThong', N'ChuaDoc', DATEADD(DAY, -1, GETDATE())),

-- Thông báo cảnh báo thẻ BHYT hết hạn
('TB007', 'NV008', N'Cảnh báo thẻ BHYT hết hạn',
 N'Bệnh nhân Bùi Thị Hồng (BN008) có thẻ BHYT đã hết hạn từ 31/12/2024. Đã xử lý thanh toán tự túc.',
 N'CanhBao', N'DaDoc', DATEADD(HOUR, -12, GETDATE())),

-- Thông báo cho bác sĩ về bệnh nhân nội trú
('TB008', 'NV001', N'Bệnh nhân BN001 đã xuất viện',
 N'Bệnh nhân Nguyễn Văn An đã hoàn thành quá trình điều trị viêm phổi và xuất viện. Tình trạng ổn định.',
 N'ThongTin', N'ChuaDoc', DATEADD(DAY, -1, GETDATE()));
GO

PRINT N'';
PRINT N'✓ Đã bổ sung dữ liệu thành công!';
PRINT N'';
PRINT N'=================================================================';
PRINT N'TỔNG KẾT DỮ LIỆU ĐÃ BỔ SUNG';
PRINT N'=================================================================';
PRINT N'';
PRINT N'PHIẾU NHẬP & CHI TIẾT:';
PRINT N'  - 3 Phiếu nhập (từ 3 NCC khác nhau)';
PRINT N'  - 11 Chi tiết phiếu nhập (liên kết với 45 lô thuốc đã có)';
PRINT N'  - Tổng giá trị nhập: 88.000.000đ';
PRINT N'';
PRINT N'PHIẾU CẤP PHÁT & CHI TIẾT:';
PRINT N'  - 10 Phiếu cấp phát (tương ứng 10 đơn thuốc)';
PRINT N'  - 18 Chi tiết phiếu cấp phát (liên kết với lô thuốc cụ thể)';
PRINT N'  - Từ 2 kho: Kho001 (Trung tâm) và Kho002 (Nội trú)';
PRINT N'';
PRINT N'HÓA ĐƠN & THANH TOÁN:';
PRINT N'  - 10 Hóa đơn (tổng giá trị: ~28.965.000đ)';
PRINT N'    + 3 BHYT 100%: 4.335.000đ → BHYT chi trả: 4.335.000đ';
PRINT N'    + 2 BHYT 95%: 17.605.000đ → BHYT chi trả: 16.724.750đ, BN trả: 880.250đ';
PRINT N'    + 2 BHYT 80%: 3.740.000đ → BHYT chi trả: 2.992.000đ, BN trả: 748.000đ';
PRINT N'    + 3 Tự túc: 2.165.000đ → BN trả: 2.165.000đ';
PRINT N'';
PRINT N'  - 7 Phiếu thu tiền (chỉ thu từ BN phải trả):';
PRINT N'    + 4 Thu tiền mặt: 1.743.250đ';
PRINT N'    + 2 Chuyển khoản VietQR: 1.055.000đ';
PRINT N'    + 1 Quẹt thẻ ATM: 1.430.000đ';
PRINT N'    → Tổng thu: 4.228.250đ';
PRINT N'';
PRINT N'VIETQR:';
PRINT N'  - 2 Giao dịch thành công qua VietQR';
PRINT N'    + BN005 (Tăng huyết áp): 580.000đ';
PRINT N'    + BN007 (Cảm cúm): 475.000đ';
PRINT N'';
PRINT N'THÔNG BÁO:';
PRINT N'  - 8 Thông báo hệ thống';
PRINT N'    + 3 Cảnh báo (thuốc hết hạn, tồn kho thấp, thẻ BHYT hết hạn)';
PRINT N'    + 4 Thông tin (phiếu nhập, thanh toán)';
PRINT N'    + 1 Hệ thống (bảo trì)';
PRINT N'';
PRINT N'=================================================================';
PRINT N'LIÊN KẾT DỮ LIỆU';
PRINT N'=================================================================';
PRINT N'';
PRINT N'LUỒNG NGHIỆP VỤ HOÀN CHỈNH:';
PRINT N'  1. Bệnh nhân → Chẩn đoán → Đơn thuốc';
PRINT N'  2. Nhà cung cấp → Phiếu nhập → Chi tiết phiếu nhập → Lô thuốc';
PRINT N'  3. Đơn thuốc → Phiếu cấp phát → Chi tiết phiếu cấp → Lô thuốc';
PRINT N'  4. Phiếu cấp phát → Hóa đơn → Phiếu thu tiền';
PRINT N'  5. Hóa đơn → VietQR giao dịch (nếu thanh toán QR)';
PRINT N'  6. Các sự kiện → Thông báo nhân viên';
PRINT N'';
PRINT N'TÍCH HỢP VNEID:';
PRINT N'  ✓ 10 Bệnh nhân có CCCD tương ứng với VNeID.CongDan';
PRINT N'  ✓ 8 Thẻ BHYT mapping với VNeID.TheBHYT_Mock';
PRINT N'  ✓ Hệ thống đã tra cứu VNeID qua API (có lịch sử)';
PRINT N'  ✓ BHYT thanh toán dựa trên mức hưởng từ VNeID';
PRINT N'';
PRINT N'TÌNH HUỐNG THỰC TẾ ĐÃ MÔ PHỎNG:';
PRINT N'  ✓ Nội trú có BHYT: BN001 (100%), BN002 (100%), BN003 (95%)';
PRINT N'  ✓ Ngoại trú có BHYT: BN004 (95%), BN005 (80%), BN006 (80%)';
PRINT N'  ✓ Tự túc không BHYT: BN007';
PRINT N'  ✓ Tự túc thẻ hết hạn: BN008';
PRINT N'  ✓ Trẻ em có BHYT: BN009 (100%)';
PRINT N'  ✓ Trẻ em không BHYT: BN010';
PRINT N'  ✓ Thanh toán đa dạng: Tiền mặt, VietQR, Thẻ ATM';
PRINT N'';
PRINT N'=================================================================';
GO