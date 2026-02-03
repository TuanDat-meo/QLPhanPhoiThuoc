-- =====================================================
-- DỮ LIỆU MẪU CHO HỆ THỐNG TÍCH HỢP
-- VNeID Database + BenhVienX Database
-- Có liên kết logic qua số CCCD/CMND
-- =====================================================

USE VNeID;
GO

PRINT N'--- Đang xóa dữ liệu cũ trong VNeID ---';
-- Xóa bảng con trước
DELETE FROM LichSuTraCuu;
DELETE FROM TheBHYT_Mock;
-- Xóa bảng cha sau
DELETE FROM CongDan;
DELETE FROM CauHinhTichHop;
GO

-- =====================================================
-- PHẦN 1: DỮ LIỆU VNeID DATABASE
-- =====================================================

-- 1. Cấu hình tích hợp (Cho phép BenhVienX truy cập API)
INSERT INTO CauHinhTichHop (MaCauHinh, TenHeThong, APIKey, APISecret, IPWhitelist, SoLanTraCuuToiDa, TrangThai, NgayTao) VALUES
('CFG_BV001', N'BenhVienX', 'BV_API_KEY_2026_XYZ123', 'BV_SECRET_ABC789DEF456', '192.168.1.100,192.168.1.101,10.0.0.50', 5000, N'KichHoat', GETDATE()),
('CFG_PORTAL', N'CongDichVuCong', 'PORTAL_KEY_2026', 'PORTAL_SECRET_2026', '103.28.36.0/24', 10000, N'KichHoat', GETDATE());
GO

-- 2. Công dân (10 người - sẽ là bệnh nhân ở BenhVienX)
INSERT INTO CongDan (SoDinhDanh, HoTen, NgaySinh, GioiTinh, QueQuan, NoiThuongTru, DiaChiHienTai, DanToc, TonGiao, QuocTich, NgayCap, NoiCap, NgayHetHan, AnhChanDung, TrangThai, NgayTao) VALUES
-- Bệnh nhân nội trú
('001085001234', N'Nguyễn Văn An', '1985-03-15', N'Nam', N'Hà Nội', N'123 Đường Láng, Đống Đa, Hà Nội', N'123 Đường Láng, Đống Đa, Hà Nội', N'Kinh', N'Không', N'Việt Nam', '2020-01-15', N'Cục Cảnh sát ĐKQL cư trú và DLQG về dân cư', '2040-01-15', 'base64_image_placeholder_1', N'HoatDong', GETDATE()),
('001090002345', N'Trần Thị Bình', '1990-07-22', N'Nữ', N'Hải Phòng', N'456 Lê Hồng Phong, Ngô Quyền, Hải Phòng', N'456 Lê Hồng Phong, Ngô Quyền, Hải Phòng', N'Kinh', N'Phật giáo', N'Việt Nam', '2020-03-20', N'Công an TP Hải Phòng', '2040-03-20', 'base64_image_placeholder_2', N'HoatDong', GETDATE()),
('001078003456', N'Lê Văn Cường', '1978-11-30', N'Nam', N'Nghệ An', N'789 Quang Trung, TP Vinh, Nghệ An', N'789 Quang Trung, TP Vinh, Nghệ An', N'Kinh', N'Không', N'Việt Nam', '2019-12-10', N'Công an tỉnh Nghệ An', '2039-12-10', 'base64_image_placeholder_3', N'HoatDong', GETDATE()),

-- Bệnh nhân ngoại trú
('001095004567', N'Phạm Thị Dung', '1995-05-18', N'Nữ', N'Nam Định', N'321 Trần Hưng Đạo, TP Nam Định', N'321 Trần Hưng Đạo, TP Nam Định', N'Kinh', N'Công giáo', N'Việt Nam', '2021-06-01', N'Công an tỉnh Nam Định', '2041-06-01', 'base64_image_placeholder_4', N'HoatDong', GETDATE()),
('001088005678', N'Hoàng Văn Em', '1988-09-25', N'Nam', N'Thái Bình', N'654 Lý Thường Kiệt, TP Thái Bình', N'654 Lý Thường Kiệt, TP Thái Bình', N'Kinh', N'Không', N'Việt Nam', '2020-08-15', N'Công an tỉnh Thái Bình', '2040-08-15', 'base64_image_placeholder_5', N'HoatDong', GETDATE()),
('001092006789', N'Vũ Thị Hoa', '1992-12-08', N'Nữ', N'Hà Nam', N'987 Trần Phú, TP Phủ Lý, Hà Nam', N'987 Trần Phú, TP Phủ Lý, Hà Nam', N'Kinh', N'Phật giáo', N'Việt Nam', '2021-01-20', N'Công an tỉnh Hà Nam', '2041-01-20', 'base64_image_placeholder_6', N'HoatDong', GETDATE()),
('001087007890', N'Đỗ Văn Giang', '1987-04-14', N'Nam', N'Ninh Bình', N'147 Lê Lợi, TP Ninh Bình', N'147 Lê Lợi, TP Ninh Bình', N'Kinh', N'Không', N'Việt Nam', '2020-05-10', N'Công an tỉnh Ninh Bình', '2040-05-10', 'base64_image_placeholder_7', N'HoatDong', GETDATE()),

-- Người có thẻ BHYT hết hạn
('001983008901', N'Bùi Thị Hồng', '1983-08-20', N'Nữ', N'Thanh Hóa', N'258 Quang Trung, TP Thanh Hóa', N'258 Quang Trung, TP Thanh Hóa', N'Kinh', N'Công giáo', N'Việt Nam', '2019-10-15', N'Công an tỉnh Thanh Hóa', '2039-10-15', 'base64_image_placeholder_8', N'HoatDong', GETDATE()),

-- Trẻ em
('001115009012', N'Nguyễn Minh Khang', '2015-02-28', N'Nam', N'Hà Nội', N'369 Giải Phóng, Hoàng Mai, Hà Nội', N'369 Giải Phóng, Hoàng Mai, Hà Nội', N'Kinh', N'Không', N'Việt Nam', '2021-03-01', N'Cục Cảnh sát ĐKQL cư trú và DLQG về dân cư', '2041-03-01', 'base64_image_placeholder_9', N'HoatDong', GETDATE()),
('001118010123', N'Trần Ngọc Linh', '2018-06-15', N'Nữ', N'Hải Phòng', N'741 Lạch Tray, Ngô Quyền, Hải Phòng', N'741 Lạch Tray, Ngô Quyền, Hải Phòng', N'Kinh', N'Phật giáo', N'Việt Nam', '2021-07-01', N'Công an TP Hải Phòng', '2041-07-01', 'base64_image_placeholder_10', N'HoatDong', GETDATE());
GO

-- 3. Thẻ BHYT Mock (8 người có thẻ, 2 người không có)
INSERT INTO TheBHYT_Mock (MaThe, SoDinhDanh, SoTheBHYT, NgayBatDau, NgayHetHan, MucHuong, NoiDKKCB, MaNoiDKKCB, DiaChi5Nam, MaKhuVuc, TrangThai, NgayTao) VALUES
-- Thẻ còn hạn - Mức hưởng 100%
('BHYT001', '001085001234', 'DN4801234567890', '2024-01-01', '2026-12-31', 100.00, N'Bệnh viện Đa khoa Trung ương', '01001', N'123 Đường Láng, Đống Đa, Hà Nội', 'K1', N'ConHan', GETDATE()),
('BHYT002', '001090002345', 'DN4802345678901', '2024-01-01', '2026-12-31', 100.00, N'Bệnh viện Việt Tiệp Hải Phòng', '31001', N'456 Lê Hồng Phong, Ngô Quyền, Hải Phòng', 'K1', N'ConHan', GETDATE()),

-- Thẻ còn hạn - Mức hưởng 95%
('BHYT003', '001078003456', 'GD0103456789012', '2023-06-01', '2026-05-31', 95.00, N'Bệnh viện Hữu nghị Việt Đức', '01002', N'789 Quang Trung, TP Vinh, Nghệ An', 'K2', N'ConHan', GETDATE()),
('BHYT004', '001095004567', 'GD0204567890123', '2024-03-01', '2027-02-28', 95.00, N'Bệnh viện Thanh Nhàn', '01003', N'321 Trần Hưng Đạo, TP Nam Định', 'K2', N'ConHan', GETDATE()),

-- Thẻ còn hạn - Mức hưởng 80%
('BHYT005', '001088005678', 'KC0305678901234', '2024-02-01', '2026-01-31', 80.00, N'Bệnh viện Bạch Mai', '01004', N'654 Lý Thường Kiệt, TP Thái Bình', 'K3', N'ConHan', GETDATE()),
('BHYT006', '001092006789', 'KC0206789012345', '2023-12-01', '2026-11-30', 80.00, N'Bệnh viện E', '01005', N'987 Trần Phú, TP Phủ Lý, Hà Nam', 'K2', N'ConHan', GETDATE()),

-- Thẻ hết hạn
('BHYT007', '001983008901', 'GD0108901234567', '2023-01-01', '2024-12-31', 95.00, N'Bệnh viện Đa khoa tỉnh Thanh Hóa', '37001', N'258 Quang Trung, TP Thanh Hóa', 'K2', N'HetHan', GETDATE()),

-- Thẻ trẻ em - Mức hưởng 100%
('BHYT008', '001115009012', 'TE0109012345678', '2024-01-01', '2026-12-31', 100.00, N'Bệnh viện Nhi Trung ương', '01006', N'369 Giải Phóng, Hoàng Mai, Hà Nội', 'K1', N'ConHan', GETDATE());
-- Người số 7 (Đỗ Văn Giang) và số 10 (Trần Ngọc Linh) không có thẻ BHYT
GO

-- 4. Lịch sử tra cứu (Mô phỏng BenhVienX đã tra cứu thông tin)
INSERT INTO LichSuTraCuu (MaTraCuu, SoDinhDanh, LoaiTraCuu, HeThongTraCuu, IPAddress, UserAgent, NgayGioTraCuu, KetQua, DuLieuTraVe, ThoiGianXuLy, GhiChu) VALUES
('TC001', '001085001234', N'ThongTinCoBan', 'BenhVienX', '192.168.1.100', 'Mozilla/5.0 Hospital System', DATEADD(DAY, -5, GETDATE()), N'ThanhCong', '{"HoTen":"Nguyen Van An","NgaySinh":"1985-03-15"}', 150, N'Tra cứu khi nhập viện'),
('TC002', '001085001234', N'TheBHYT', 'BenhVienX', '192.168.1.100', 'Mozilla/5.0 Hospital System', DATEADD(DAY, -5, GETDATE()), N'ThanhCong', '{"SoThe":"DN4801234567890","MucHuong":100}', 180, N'Kiểm tra BHYT'),
('TC003', '001090002345', N'ThongTinCoBan', 'BenhVienX', '192.168.1.100', 'Mozilla/5.0 Hospital System', DATEADD(DAY, -3, GETDATE()), N'ThanhCong', '{"HoTen":"Tran Thi Binh","NgaySinh":"1990-07-22"}', 145, N'Đăng ký khám'),
('TC004', '001090002345', N'TheBHYT', 'BenhVienX', '192.168.1.100', 'Mozilla/5.0 Hospital System', DATEADD(DAY, -3, GETDATE()), N'ThanhCong', '{"SoThe":"DN4802345678901","MucHuong":100}', 175, N'Kiểm tra BHYT'),
('TC005', '001095004567', N'ThongTinCoBan', 'BenhVienX', '192.168.1.101', 'Mozilla/5.0 Hospital System', DATEADD(DAY, -2, GETDATE()), N'ThanhCong', '{"HoTen":"Pham Thi Dung","NgaySinh":"1995-05-18"}', 155, N'Khám ngoại trú'),
('TC006', '001087007890', N'ThongTinCoBan', 'BenhVienX', '192.168.1.101', 'Mozilla/5.0 Hospital System', DATEADD(DAY, -1, GETDATE()), N'ThanhCong', '{"HoTen":"Do Van Giang","NgaySinh":"1987-04-14"}', 160, N'Khám ngoại trú'),
('TC007', '001087007890', N'TheBHYT', 'BenhVienX', '192.168.1.101', 'Mozilla/5.0 Hospital System', DATEADD(DAY, -1, GETDATE()), N'KhongTimThay', '{"message":"Khong tim thay the BHYT"}', 120, N'Không có thẻ BHYT'),
('TC008', '001115009012', N'ThongTinCoBan', 'BenhVienX', '192.168.1.100', 'Mozilla/5.0 Hospital System', GETDATE(), N'ThanhCong', '{"HoTen":"Nguyen Minh Khang","NgaySinh":"2015-02-28"}', 165, N'Khám nhi'),
('TC009', '001115009012', N'TheBHYT', 'BenhVienX', '192.168.1.100', 'Mozilla/5.0 Hospital System', GETDATE(), N'ThanhCong', '{"SoThe":"TE0109012345678","MucHuong":100}', 185, N'Kiểm tra BHYT trẻ em'),
('TC010', '001983008901', N'TheBHYT', 'BenhVienX', '192.168.1.101', 'Mozilla/5.0 Hospital System', DATEADD(DAY, -4, GETDATE()), N'HetHan', '{"message":"The BHYT da het han"}', 140, N'Thẻ hết hạn');
GO

PRINT N'✓ Đã insert dữ liệu VNeID Database';
GO

-- =====================================================
-- PHẦN 2: DỮ LIỆU BENHVIENX DATABASE
-- =====================================================

USE BenhVienX;
GO

PRINT N'--- Đang xóa dữ liệu cũ trong BenhVienX ---';

-- Tầng 6: Các bảng thanh toán
DELETE FROM PhieuThuTien;
DELETE FROM VietQR_GiaoDich;

-- Tầng 5: Hóa đơn
DELETE FROM HoaDon;

-- Tầng 4: Các bảng chi tiết phiếu cấp phát và chi tiết đơn thuốc
DELETE FROM ChiTietPhieuCap;
DELETE FROM ChiTietDonThuoc;
DELETE FROM ChiTietPhieuNhap;

-- Tầng 3: Phiếu cấp phát, Đơn thuốc, Phiếu nhập, Lô thuốc
DELETE FROM PhieuCapPhat;
DELETE FROM DonThuoc;
DELETE FROM PhieuNhap;
DELETE FROM LoThuoc;

-- Tầng 2.5: Tài khoản (phụ thuộc NhanVien)
DELETE FROM TaiKhoan;

-- Tầng 2: Các bảng phụ thuộc vào Bệnh nhân
DELETE FROM ChanDoan;
DELETE FROM TheBHYT;

-- Tầng 1: Các bảng danh mục chính (Bảng cha)
DELETE FROM BenhNhan;
DELETE FROM Thuoc;
DELETE FROM NhanVien;
DELETE FROM NhaCungCap;
DELETE FROM KhoaPhong;
DELETE FROM Kho;

-- Tầng 0: Cấu hình hệ thống và thông báo
DELETE FROM ThongBao;
DELETE FROM CauHinhHeThong;
DELETE FROM CauHinh_VietQR;
GO

-- 1. Kho
INSERT INTO Kho (MaKho, TenKho, LoaiKho, DiaDiem, GhiChu, TrangThai, NgayTao) VALUES
('KHO001', N'Kho trung tâm', N'Trung tam', N'Tầng 1, Tòa nhà A', N'Kho lưu trữ chính của bệnh viện', N'HoatDong', GETDATE()),
('KHO002', N'Kho dược lâm sàng', N'Lam sang', N'Tầng 2, Tòa nhà B', N'Kho cấp phát trực tiếp cho khoa', N'HoatDong', GETDATE()),
('KHO003', N'Kho dự trữ', N'Du tru', N'Tầng hầm, Tòa nhà A', N'Kho dự trữ thuốc dài hạn', N'HoatDong', GETDATE());
GO

-- 2. Khoa phòng
INSERT INTO KhoaPhong (MaKhoa, TenKhoa, TruongKhoa, SoDienThoai, TrangThai, NgayTao) VALUES
('K001', N'Khoa Nội tổng hợp', N'PGS.TS Nguyễn Văn Tùng', '0241234567', N'HoatDong', GETDATE()),
('K002', N'Khoa Sản', N'TS. Trần Thị Mai', '0241234568', N'HoatDong', GETDATE()),
('K003', N'Khoa Ngoại tổng hợp', N'PGS.TS Lê Văn Hùng', '0241234569', N'HoatDong', GETDATE()),
('K004', N'Khoa Nhi', N'TS. Phạm Thị Lan', '0241234570', N'HoatDong', GETDATE()),
('K005', N'Khoa Khám bệnh', N'BS.CKII Hoàng Văn Nam', '0241234571', N'HoatDong', GETDATE());
GO

-- 3. Nhân viên
INSERT INTO NhanVien (MaNhanVien, TenNhanVien, ChucVu, ChuyenKhoa, BangCap, MaKhoa, SoDienThoai, Email, CCCD, NgaySinh, GioiTinh, DiaChi, TrangThai, NgayTao) VALUES
-- Bác sĩ
('NV001', N'BS. Nguyễn Văn An', N'Bác sĩ', N'Nội khoa', N'Bác sĩ đa khoa', 'K001', '0987654321', 'bs.nguyenvana@benhvienx.vn', '079085001111', '1985-05-10', N'Nam', N'Hà Nội', N'DangLamViec', GETDATE()),
('NV002', N'BS. Trần Thị Bình', N'Bác sĩ', N'Sản phụ khoa', N'Bác sĩ chuyên khoa II', 'K002', '0987654322', 'bs.tranthib@benhvienx.vn', '079090002222', '1990-08-15', N'Nữ', N'Hải Phòng', N'DangLamViec', GETDATE()),
('NV003', N'BS. Lê Văn Cường', N'Bác sĩ', N'Ngoại khoa', N'Bác sĩ chuyên khoa I', 'K003', '0987654323', 'bs.levanc@benhvienx.vn', '079078003333', '1978-12-20', N'Nam', N'Nghệ An', N'DangLamViec', GETDATE()),
('NV004', N'BS. Phạm Thị Dung', N'Bác sĩ', N'Nhi khoa', N'Bác sĩ chuyên khoa I', 'K004', '0987654324', 'bs.phamthid@benhvienx.vn', '079095004444', '1995-03-25', N'Nữ', N'Nam Định', N'DangLamViec', GETDATE()),
('NV005', N'BS. Hoàng Văn Em', N'Bác sĩ trưởng', N'Đa khoa', N'Bác sĩ chuyên khoa II', 'K005', '0987654325', 'bs.hoangvane@benhvienx.vn', '079088005555', '1988-07-30', N'Nam', N'Thái Bình', N'DangLamViec', GETDATE()),

-- Dương sĩ (gán K005 - Khoa Khám bệnh)
('NV006', N'DS. Vũ Thị Hương', N'Dương sĩ', N'Dương lâm sàng', N'Dương sĩ đại học', 'K005', '0987654326', 'ds.vuthihuong@benhvienx.vn', '079092006666', '1992-11-12', N'Nữ', N'Hà Nam', N'DangLamViec', GETDATE()),
('NV007', N'DS. Đỗ Văn Phúc', N'Dương sĩ trưởng', N'Dương lâm sàng', N'Dương sĩ chuyên khoa I', 'K005', '0987654327', 'ds.dovanphuc@benhvienx.vn', '079087007777', '1987-09-18', N'Nam', N'Ninh Bình', N'DangLamViec', GETDATE()),

-- Thu ngân (gán K005 - Khoa Khám bệnh)
('NV008', N'Bùi Thị Lan', N'Thu ngân', N'Kế toán', N'Trung cấp kế toán', 'K005', '0987654328', 'tn.buithilan@benhvienx.vn', '079093008888', '1993-04-22', N'Nữ', N'Thanh Hóa', N'DangLamViec', GETDATE()),
('NV009', N'Nguyễn Văn Minh', N'Thu ngân', N'Kế toán', N'Cao đẳng kế toán', 'K005', '0987654329', 'tn.nguyenvanminh@benhvienx.vn', '079091009999', '1991-06-05', N'Nam', N'Hà Nội', N'DangLamViec', GETDATE()),

-- Nhân viên kho (gán K001 - Khoa Nội tổng hợp)
('NV010', N'Trần Văn Sơn', N'Quản lý kho', N'Kho thuốc', N'Trung cấp dương', 'K001', '0987654330', 'kho.tranvanson@benhvienx.vn', '079089010000', '1989-02-14', N'Nam', N'Hải Phòng', N'DangLamViec', GETDATE());
GO

-- 4. Nhà cung cấp
INSERT INTO NhaCungCap (MaNCC, TenNCC, DiaChi, SoDienThoai, Email, MaSoThue, TrangThai, NgayTao) VALUES
('NCC001', N'Công ty Dược phẩm Phương Đông', N'Số 123, Đường Giải Phóng, Hà Nội', '0243456789', 'info@phuongdongpharma.vn', '0123456789', N'HoatDong', GETDATE()),
('NCC002', N'Công ty TNHH Dược phẩm Hà Nội', N'Số 456, Đường Láng Hạ, Ba Đình, Hà Nội', '0243456790', 'contact@hanoipharma.vn', '0123456790', N'HoatDong', GETDATE()),
('NCC003', N'Công ty Cổ phần Dược Việt', N'Số 789, Đường Trần Duy Hưng, Cầu Giấy, Hà Nội', '0243456791', 'sales@duocviet.vn', '0123456791', N'HoatDong', GETDATE());
GO

-- 5. Thuốc
INSERT INTO Thuoc (MaThuoc, TenThuoc, HoatChat, DonViTinh, HamLuong, DangBaoChe, DuongDung, NhaSanXuat, NhomThuoc, GiaNhap, GiaXuat, TonKhoToiThieu, LaThuocBHYT, TyLeBHYTChiTra, MoTa, TrangThai, NgayTao) VALUES
-- Thuốc BHYT (uống)
('T001', N'Paracetamol 500mg', N'Paracetamol', N'Viên', N'500mg', N'Viên nén', N'Uống', N'Pymepharco', N'Giảm đau - Hạ sốt', 500, 1000, 100, N'Yes', 100.00, N'Thuốc giảm đau, hạ sốt thông dụng', N'KichHoat', GETDATE()),
('T002', N'Amoxicillin 500mg', N'Amoxicillin', N'Viên', N'500mg', N'Viên nang', N'Uống', N'DHG Pharma', N'Kháng sinh', 2000, 4000, 80, N'Yes', 100.00, N'Kháng sinh nhóm Beta-lactam', N'KichHoat', GETDATE()),
('T003', N'Vitamin C 500mg', N'Acid ascorbic', N'Viên', N'500mg', N'Viên sủi', N'Uống', N'Traphaco', N'Vitamin - Khoáng chất', 1500, 3000, 50, N'Yes', 80.00, N'Bổ sung vitamin C', N'KichHoat', GETDATE()),
('T004', N'Omeprazole 20mg', N'Omeprazole', N'Viên', N'20mg', N'Viên nang', N'Uống', N'Danapha', N'Tiêu hóa', 3000, 6000, 60, N'Yes', 100.00, N'Thuốc điều trị loét dạ dày', N'KichHoat', GETDATE()),
('T005', N'Metformin 500mg', N'Metformin HCl', N'Viên', N'500mg', N'Viên nén bao phim', N'Uống', N'Sanofi', N'Đái tháo đường', 1800, 3600, 70, N'Yes', 100.00, N'Thuốc điều trị đái tháo đường type 2', N'KichHoat', GETDATE()),
('T006', N'Atorvastatin 10mg', N'Atorvastatin', N'Viên', N'10mg', N'Viên nén bao phim', N'Uống', N'Pfizer', N'Tim mạch', 4000, 8000, 40, N'Yes', 80.00, N'Thuốc giảm cholesterol', N'KichHoat', GETDATE()),
('T007', N'Amlodipine 5mg', N'Amlodipine', N'Viên', N'5mg', N'Viên nén', N'Uống', N'Novartis', N'Tim mạch', 2500, 5000, 50, N'Yes', 100.00, N'Thuốc hạ huyết áp', N'KichHoat', GETDATE()),
('T008', N'Salbutamol 2mg', N'Salbutamol sulfate', N'Viên', N'2mg', N'Viên nén', N'Uống', N'GSK', N'Hô hấp', 3500, 7000, 30, N'Yes', 100.00, N'Thuốc giãn phế quản', N'KichHoat', GETDATE()),

-- Thuốc dịch vụ (không BHYT)
('T009', N'Livolin Forte', N'Silymarin + Phospholipid', N'Viên', N'140mg + 100mg', N'Viên nang mềm', N'Uống', N'Mega We Care', N'Gan mật', 8000, 16000, 20, N'No', 0.00, N'Thuốc bảo vệ gan (dịch vụ)', N'KichHoat', GETDATE()),
('T010', N'Effer-Paralmax Extra', N'Paracetamol + Caffeine', N'Viên', N'500mg + 65mg', N'Viên sủi', N'Uống', N'Takeda', N'Giảm đau - Hạ sốt', 5000, 10000, 30, N'No', 0.00, N'Thuốc giảm đau nhanh (dịch vụ)', N'KichHoat', GETDATE()),
('T011', N'Centrum Silver', N'Multivitamin + Khoáng chất', N'Viên', N'Đa dạng', N'Viên nén bao phim', N'Uống', N'Pfizer', N'Vitamin - Khoáng chất', 12000, 24000, 15, N'No', 0.00, N'Vitamin tổng hợp cho người trên 50 tuổi', N'KichHoat', GETDATE()),

-- Thuốc tiêm / truyền
('T012', N'Natri Clorid 0.9%', N'Natri Clorid', N'Chai', N'0.9%', N'Dung dịch tiêm truyền', N'Tiêm tĩnh mạch', N'Vinafar', N'Dịch truyền', 15000, 30000, 200, N'Yes', 100.00, N'Dung dịch truyền sinh lý', N'KichHoat', GETDATE()),
('T013', N'Glucose 5%', N'Glucose', N'Chai', N'5%', N'Dung dịch tiêm truyền', N'Tiêm tĩnh mạch', N'Vinafar', N'Dịch truyền', 16000, 32000, 200, N'Yes', 100.00, N'Dung dịch truyền glucose', N'KichHoat', GETDATE()),
('T014', N'Ceftriaxone 1g', N'Ceftriaxone sodium', N'Lọ', N'1g', N'Bột pha tiêm', N'Tiêm tĩnh mạch/Tiêm bắp', N'GSK', N'Kháng sinh', 25000, 50000, 100, N'Yes', 100.00, N'Kháng sinh cephalosporin thế hệ 3', N'KichHoat', GETDATE()),
('T015', N'Heparin 5000UI/ml', N'Heparin sodium', N'Ống', N'5000UI/ml', N'Dung dịch tiêm', N'Tiêm tĩnh mạch', N'Biotest', N'Chống đông', 45000, 90000, 50, N'Yes', 80.00, N'Thuốc chống đông máu', N'KichHoat', GETDATE());
GO

-- 6. Lô thuốc (3 lô cho mỗi thuốc)
INSERT INTO LoThuoc (MaLo, MaThuoc, MaKho, SoLo, NgaySanXuat, HanSuDung, SoLuongNhap, SoLuongCon, TrangThai, NgayTao) VALUES
-- Lô thuốc cho T001 (Paracetamol)
('LO001', 'T001', 'KHO001', 'PARA-2024-001', '2024-01-15', '2027-01-15', 5000, 5000, N'ConHan', GETDATE()),
('LO002', 'T001', 'KHO001', 'PARA-2024-002', '2024-03-20', '2027-03-20', 3000, 3000, N'ConHan', GETDATE()),
('LO003', 'T001', 'KHO002', 'PARA-2024-003', '2024-05-10', '2027-05-10', 2000, 2000, N'ConHan', GETDATE()),

-- Lô thuốc cho T002 (Amoxicillin)
('LO004', 'T002', 'KHO001', 'AMOX-2024-001', '2024-02-01', '2027-02-01', 4000, 4000, N'ConHan', GETDATE()),
('LO005', 'T002', 'KHO001', 'AMOX-2024-002', '2024-04-15', '2027-04-15', 2500, 2500, N'ConHan', GETDATE()),
('LO006', 'T002', 'KHO002', 'AMOX-2024-003', '2024-06-20', '2027-06-20', 1500, 1500, N'ConHan', GETDATE()),

-- Lô thuốc cho T003 (Vitamin C)
('LO007', 'T003', 'KHO001', 'VITC-2024-001', '2024-01-10', '2026-01-10', 3000, 3000, N'ConHan', GETDATE()),
('LO008', 'T003', 'KHO001', 'VITC-2024-002', '2024-03-25', '2026-03-25', 2000, 2000, N'ConHan', GETDATE()),
('LO009', 'T003', 'KHO002', 'VITC-2024-003', '2024-05-30', '2026-05-30', 1000, 1000, N'ConHan', GETDATE()),

-- Lô thuốc cho T004 (Omeprazole)
('LO010', 'T004', 'KHO001', 'OMEP-2024-001', '2024-02-10', '2027-02-10', 2500, 2500, N'ConHan', GETDATE()),
('LO011', 'T004', 'KHO001', 'OMEP-2024-002', '2024-04-20', '2027-04-20', 1500, 1500, N'ConHan', GETDATE()),
('LO012', 'T004', 'KHO002', 'OMEP-2024-003', '2024-06-25', '2027-06-25', 1000, 1000, N'ConHan', GETDATE()),

-- Lô thuốc cho T005 (Metformin)
('LO013', 'T005', 'KHO001', 'METF-2024-001', '2024-01-20', '2027-01-20', 3500, 3500, N'ConHan', GETDATE()),
('LO014', 'T005', 'KHO001', 'METF-2024-002', '2024-03-30', '2027-03-30', 2000, 2000, N'ConHan', GETDATE()),
('LO015', 'T005', 'KHO002', 'METF-2024-003', '2024-05-15', '2027-05-15', 1500, 1500, N'ConHan', GETDATE()),

-- Lô thuốc cho T006 (Atorvastatin)
('LO016', 'T006', 'KHO001', 'ATOR-2024-001', '2024-02-15', '2027-02-15', 2000, 2000, N'ConHan', GETDATE()),
('LO017', 'T006', 'KHO001', 'ATOR-2024-002', '2024-04-25', '2027-04-25', 1200, 1200, N'ConHan', GETDATE()),
('LO018', 'T006', 'KHO002', 'ATOR-2024-003', '2024-06-30', '2027-06-30', 800, 800, N'ConHan', GETDATE()),

-- Lô thuốc cho T007 (Amlodipine)
('LO019', 'T007', 'KHO001', 'AMLO-2024-001', '2024-01-25', '2027-01-25', 2500, 2500, N'ConHan', GETDATE()),
('LO020', 'T007', 'KHO001', 'AMLO-2024-002', '2024-03-15', '2027-03-15', 1500, 1500, N'ConHan', GETDATE()),
('LO021', 'T007', 'KHO002', 'AMLO-2024-003', '2024-05-20', '2027-05-20', 1000, 1000, N'ConHan', GETDATE()),

-- Lô thuốc cho T008 (Salbutamol)
('LO022', 'T008', 'KHO001', 'SALB-2024-001', '2024-02-20', '2027-02-20', 1500, 1500, N'ConHan', GETDATE()),
('LO023', 'T008', 'KHO001', 'SALB-2024-002', '2024-04-30', '2027-04-30', 1000, 1000, N'ConHan', GETDATE()),
('LO024', 'T008', 'KHO002', 'SALB-2024-003', '2024-06-15', '2027-06-15', 500, 500, N'ConHan', GETDATE()),

-- Lô thuốc cho T009 (Livolin - dịch vụ)
('LO025', 'T009', 'KHO001', 'LIVO-2024-001', '2024-01-30', '2026-01-30', 500, 500, N'ConHan', GETDATE()),
('LO026', 'T009', 'KHO001', 'LIVO-2024-002', '2024-03-10', '2026-03-10', 300, 300, N'ConHan', GETDATE()),
('LO027', 'T009', 'KHO002', 'LIVO-2024-003', '2024-05-25', '2026-05-25', 200, 200, N'ConHan', GETDATE()),

-- Lô thuốc cho T010 (Effer-Paralmax - dịch vụ)
('LO028', 'T010', 'KHO001', 'EFFE-2024-001', '2024-02-25', '2026-02-25', 600, 600, N'ConHan', GETDATE()),
('LO029', 'T010', 'KHO001', 'EFFE-2024-002', '2024-04-10', '2026-04-10', 400, 400, N'ConHan', GETDATE()),
('LO030', 'T010', 'KHO002', 'EFFE-2024-003', '2024-06-05', '2026-06-05', 300, 300, N'ConHan', GETDATE()),

-- Lô thuốc cho T011 (Centrum - dịch vụ)
('LO031', 'T011', 'KHO001', 'CENT-2024-001', '2024-01-05', '2026-01-05', 300, 300, N'ConHan', GETDATE()),
('LO032', 'T011', 'KHO001', 'CENT-2024-002', '2024-03-20', '2026-03-20', 200, 200, N'ConHan', GETDATE()),
('LO033', 'T011', 'KHO002', 'CENT-2024-003', '2024-05-10', '2026-05-10', 150, 150, N'ConHan', GETDATE()),

-- Lô thuốc cho T012 (NaCl 0.9%)
('LO034', 'T012', 'KHO001', 'NACL-2024-001', '2024-02-05', '2027-02-05', 5000, 5000, N'ConHan', GETDATE()),
('LO035', 'T012', 'KHO001', 'NACL-2024-002', '2024-04-05', '2027-04-05', 3000, 3000, N'ConHan', GETDATE()),
('LO036', 'T012', 'KHO002', 'NACL-2024-003', '2024-06-10', '2027-06-10', 2000, 2000, N'ConHan', GETDATE()),

-- Lô thuốc cho T013 (Glucose 5%)
('LO037', 'T013', 'KHO001', 'GLUC-2024-001', '2024-02-10', '2027-02-10', 4500, 4500, N'ConHan', GETDATE()),
('LO038', 'T013', 'KHO001', 'GLUC-2024-002', '2024-04-15', '2027-04-15', 2800, 2800, N'ConHan', GETDATE()),
('LO039', 'T013', 'KHO002', 'GLUC-2024-003', '2024-06-20', '2027-06-20', 1800, 1800, N'ConHan', GETDATE()),

-- Lô thuốc cho T014 (Ceftriaxone)
('LO040', 'T014', 'KHO001', 'CEFT-2024-001', '2024-01-15', '2027-01-15', 2500, 2500, N'ConHan', GETDATE()),
('LO041', 'T014', 'KHO001', 'CEFT-2024-002', '2024-03-25', '2027-03-25', 1500, 1500, N'ConHan', GETDATE()),
('LO042', 'T014', 'KHO002', 'CEFT-2024-003', '2024-05-30', '2027-05-30', 1000, 1000, N'ConHan', GETDATE()),

-- Lô thuốc cho T015 (Heparin)
('LO043', 'T015', 'KHO001', 'HEPA-2024-001', '2024-02-20', '2027-02-20', 1000, 1000, N'ConHan', GETDATE()),
('LO044', 'T015', 'KHO001', 'HEPA-2024-002', '2024-04-20', '2027-04-20', 600, 600, N'ConHan', GETDATE()),
('LO045', 'T015', 'KHO002', 'HEPA-2024-003', '2024-06-25', '2027-06-25', 400, 400, N'ConHan', GETDATE());
GO


-- ====== DEBUG: Dump schema BenhNhan ======
PRINT '>>> Columns in BenhNhan:';
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'BenhNhan' AND TABLE_SCHEMA = 'dbo'
ORDER BY ORDINAL_POSITION;
PRINT '>>> Columns in ChanDoan:';
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'ChanDoan' AND TABLE_SCHEMA = 'dbo'
ORDER BY ORDINAL_POSITION;
PRINT '>>> Columns in DonThuoc:';
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'DonThuoc' AND TABLE_SCHEMA = 'dbo'
ORDER BY ORDINAL_POSITION;
GO

-- 7. Bệnh nhân (Liên kết với VNeID qua CCCD)
INSERT INTO BenhNhan (MaBenhNhan, CCCD, TenBenhNhan, NgaySinh, GioiTinh, SoDienThoai, DiaChi, Email, NhomMau, NgheNghiep, TienSuDiUng, TrangThai, NgayTao) VALUES
-- Bệnh nhân nội trú (có BHYT 100%)
('BN001', '001085001234', N'Nguyễn Văn An', '1985-03-15', N'Nam', '0912345001', N'123 Đường Láng, Đống Đa, Hà Nội', 'nguyenvanan@email.com', N'O+', N'Kỹ sư', N'Không có tiền sử bệnh đặc biệt; Dị ứng: Không', N'DangDieuTri', GETDATE()),
('BN002', '001090002345', N'Trần Thị Bình', '1990-07-22', N'Nữ', '0912345002', N'456 Lê Hồng Phong, Ngô Quyền, Hải Phòng', 'tranthib@email.com', N'A+', N'Giáo viên', N'Có tiền sử cao huyết áp; Dị ứng: Không', N'DangDieuTri', GETDATE()),
('BN003', '001078003456', N'Lê Văn Cường', '1978-11-30', N'Nam', '0912345003', N'789 Quang Trung, TP Vinh, Nghệ An', 'levanc@email.com', N'B+', N'Công nhân', N'Tiền sử viêm dạ dày; Dị ứng: Penicillin', N'DangDieuTri', GETDATE()),

-- Bệnh nhân ngoại trú (có BHYT 95%)
('BN004', '001095004567', N'Phạm Thị Dung', '1995-05-18', N'Nữ', '0912345004', N'321 Trần Hưng Đạo, TP Nam Định', 'phamthid@email.com', N'AB+', N'Sinh viên', N'Không có tiền sử bệnh đặc biệt; Dị ứng: Không', N'DaRaVien', GETDATE()),
('BN005', '001088005678', N'Hoàng Văn Em', '1988-09-25', N'Nam', '0912345005', N'654 Lý Thường Kiệt, TP Thái Bình', 'hoangvane@email.com', N'O-', N'Lái xe', N'Tiền sử hen suyễn; Dị ứng: Aspirin', N'DangDieuTri', GETDATE()),
('BN006', '001092006789', N'Vũ Thị Hoa', '1992-12-08', N'Nữ', '0912345006', N'987 Trần Phú, TP Phủ Lý, Hà Nam', 'vuthih@email.com', N'A-', N'Nội trợ', N'Tiền sử tiểu đường type 2; Dị ứng: Không', N'DangDieuTri', GETDATE()),

-- Bệnh nhân không có BHYT
('BN007', '001087007890', N'Đỗ Văn Giang', '1987-04-14', N'Nam', '0912345007', N'147 Lê Lợi, TP Ninh Bình', 'dovang@email.com', N'B+', N'Buôn bán', N'Không có tiền sử bệnh đặc biệt; Dị ứng: Không', N'DaRaVien', GETDATE()),

-- Bệnh nhân có BHYT hết hạn
('BN008', '001983008901', N'Bùi Thị Hồng', '1983-08-20', N'Nữ', '0912345008', N'258 Quang Trung, TP Thanh Hóa', 'buithih@email.com', N'O+', N'Hưu trí', N'Tiền sử đái tháo đường; Dị ứng: Không', N'DangDieuTri', GETDATE()),

-- Bệnh nhân trẻ em
('BN009', '001115009012', N'Nguyễn Minh Khang', '2015-02-28', N'Nam', '0912345009', N'369 Giải Phóng, Hoàng Mai, Hà Nội', 'phuhuynhkhang@email.com', N'A+', N'Học sinh', N'Trẻ em khỏe mạnh; Dị ứng: Không', N'DangDieuTri', GETDATE()),
('BN010', '001118010123', N'Trần Ngọc Linh', '2018-06-15', N'Nữ', '0912345010', N'741 Lạch Tray, Ngô Quyền, Hải Phòng', 'phuhuynhlinh@email.com', N'B+', N'Học sinh', N'Trẻ em bình thường; Dị ứng: Không', N'DangDieuTri', GETDATE());
GO

-- 8. Thẻ BHYT (Mapping từ VNeID)
INSERT INTO TheBHYT (MaThe, MaBenhNhan, SoTheBHYT, NgayBatDau, NgayHetHan, MucHuong, NoiDangKyKCB, DiaChi5Nam, TrangThai, NgayTao) VALUES
-- Thẻ còn hạn 100%
('BHYT_BN001', 'BN001', 'DN4801234567890', '2024-01-01', '2026-12-31', 100.00, N'Bệnh viện Đa khoa Trung ương', N'123 Đường Láng, Đống Đa, Hà Nội', N'ConHan', GETDATE()),
('BHYT_BN002', 'BN002', 'DN4802345678901', '2024-01-01', '2026-12-31', 100.00, N'Bệnh viện Việt Tiệp Hải Phòng', N'456 Lê Hồng Phong, Ngô Quyền, Hải Phòng', N'ConHan', GETDATE()),

-- Thẻ còn hạn 95%
('BHYT_BN003', 'BN003', 'GD0103456789012', '2023-06-01', '2026-05-31', 95.00, N'Bệnh viện Hữu nghị Việt Đức', N'789 Quang Trung, TP Vinh, Nghệ An', N'ConHan', GETDATE()),
('BHYT_BN004', 'BN004', 'GD0204567890123', '2024-03-01', '2027-02-28', 95.00, N'Bệnh viện Thanh Nhàn', N'321 Trần Hưng Đạo, TP Nam Định', N'ConHan', GETDATE()),

-- Thẻ còn hạn 80%
('BHYT_BN005', 'BN005', 'KC0305678901234', '2024-02-01', '2026-01-31', 80.00, N'Bệnh viện Bạch Mai', N'654 Lý Thường Kiệt, TP Thái Bình', N'ConHan', GETDATE()),
('BHYT_BN006', 'BN006', 'KC0206789012345', '2023-12-01', '2026-11-30', 80.00, N'Bệnh viện E', N'987 Trần Phú, TP Phủ Lý, Hà Nam', N'ConHan', GETDATE()),

-- Thẻ hết hạn
('BHYT_BN008', 'BN008', 'GD0108901234567', '2023-01-01', '2024-12-31', 95.00, N'Bệnh viện Đa khoa tỉnh Thanh Hóa', N'258 Quang Trung, TP Thanh Hóa', N'HetHan', GETDATE()),

-- Thẻ trẻ em
('BHYT_BN009', 'BN009', 'TE0109012345678', '2024-01-01', '2026-12-31', 100.00, N'Bệnh viện Nhi Trung ương', N'369 Giải Phóng, Hoàng Mai, Hà Nội', N'ConHan', GETDATE());
-- BN007 và BN010 không có thẻ BHYT
GO

-- 9. Chẩn đoán
INSERT INTO ChanDoan (MaChanDoan, MaBenhNhan, MaNhanVien, NgayChanDoan, TrieuChung, TenBenh, MaICD10, ChanDoanSoBo, KetLuan, GhiChu, NgayTao) VALUES
('CD001', 'BN001', 'NV001', DATEADD(DAY, -5, GETDATE()), N'Ho, khó thở, sốt cao', N'Viêm phổi', N'J18.9', N'Nghi ngờ Viêm phổi', N'Viêm phổi thùy trái', N'Nhập viện cấp cứu', GETDATE()),
('CD002', 'BN002', 'NV002', DATEADD(DAY, -3, GETDATE()), N'Đau bụng dưới, ra máu', N'Sau sinh tự nhiên', N'Z39.0', N'Nghi ngờ Sau sinh tự nhiên', N'Sức khỏe mẹ và bé ổn định', N'Theo dõi sau sinh', GETDATE()),
('CD003', 'BN003', 'NV003', DATEADD(DAY, -7, GETDATE()), N'Đau bụng vùng hạ vị phải', N'Viêm ruột thừa cấp', N'K35.8', N'Nghi ngờ Viêm ruột thừa cấp', N'Viêm ruột thừa cấp giờ thứ 12', N'Đã phẫu thuật thành công', GETDATE()),
('CD004', 'BN004', 'NV005', DATEADD(DAY, -2, GETDATE()), N'Đau họng, khó nuốt, sốt nhẹ', N'Viêm họng cấp', N'J06.9', N'Nghi ngờ Viêm họng cấp', N'Viêm họng cấp tính', N'Kê đơn thuốc về nhà', GETDATE()),
('CD005', 'BN005', 'NV001', DATEADD(DAY, -4, GETDATE()), N'Huyết áp cao 160/100 mmHg', N'Tăng huyết áp độ II', N'I10', N'Nghi ngờ Tăng huyết áp độ II', N'Tăng huyết áp vô căn', N'Cần tái khám sau 1 tháng', GETDATE()),
('CD006', 'BN006', 'NV001', DATEADD(DAY, -6, GETDATE()), N'Đau thượng vị, ợ nóng, khó tiêu', N'Viêm loét dạ dày tá tràng', N'K25.9', N'Nghi ngờ Viêm loét dạ dày tá tràng', N'Viêm dạ dày HP (+)', N'Tái khám sau 2 tuần', GETDATE()),
('CD007', 'BN007', 'NV005', DATEADD(DAY, -1, GETDATE()), N'Sốt, đau đầu, ho, mệt mỏi', N'Cảm cúm', N'J06.9', N'Nghi ngờ Cảm cúm', N'Cúm mùa', N'Nghỉ ngơi tại nhà', GETDATE()),
('CD008', 'BN008', 'NV001', DATEADD(DAY, -4, GETDATE()), N'Đường huyết cao, tiểu nhiều', N'Đái tháo đường type 2', N'E11.9', N'Nghi ngờ Đái tháo đường type 2', N'Đái tháo đường type 2 - biến chứng nhẹ', N'Cần kiểm soát chế độ ăn', GETDATE()),
('CD009', 'BN009', 'NV004', DATEADD(DAY, -1, GETDATE()), N'Sốt, ho, khó thở', N'Viêm đường hô hấp trên', N'J06.9', N'Nghi ngờ Viêm đường hô hấp trên', N'Viêm mũi họng cấp', N'Theo dõi tại nhà', GETDATE()),
('CD010', 'BN010', 'NV004', GETDATE(), N'Tiêu chảy, nôn, mệt', N'Tiêu chảy cấp', N'A09', N'Nghi ngờ Tiêu chảy cấp', N'Tiêu chảy cấp do nhiễm khuẩn', N'Bù nước điện giải', GETDATE());
GO

-- 10. Đơn thuốc
INSERT INTO DonThuoc (MaDonThuoc, MaChanDoan, MaBenhNhan, MaNhanVien, NgayKeDon, LoaiDon, TongTien, ChanDoanSoBo, TrangThai, GhiChuBacSi, NgayTao) VALUES
-- Đơn thuốc nội trú
('DT001', 'CD001', 'BN001', 'NV001', DATEADD(DAY, -5, GETDATE()), N'Noi tru', 2760000.00, N'Viêm phổi', N'DaCapPhat', N'Điều trị viêm phổi - BHYT 100%', GETDATE()),
('DT002', 'CD002', 'BN002', 'NV002', DATEADD(DAY, -3, GETDATE()), N'Noi tru', 146000.00, N'Sau sinh tự nhiên', N'DaCapPhat', N'Thuốc sau sinh - BHYT 100%', GETDATE()),
('DT003', 'CD003', 'BN003', 'NV003', DATEADD(DAY, -7, GETDATE()), N'Noi tru', 420000.00, N'Viêm ruột thừa cấp - đã PT', N'DaCapPhat', N'Thuốc sau phẫu thuật - BHYT 95%', GETDATE()),

-- Đơn thuốc ngoại trú
('DT004', 'CD004', 'BN004', 'NV005', DATEADD(DAY, -2, GETDATE()), N'Ngoai tru', 25000.00, N'Viêm họng cấp', N'DaCapPhat', N'Điều trị viêm họng - BHYT 95%', GETDATE()),
('DT005', 'CD005', 'BN005', 'NV001', DATEADD(DAY, -4, GETDATE()), N'Ngoai tru', 390000.00, N'Tăng huyết áp độ II', N'DaCapPhat', N'Thuốc tăng huyết áp - BHYT 80%', GETDATE()),
('DT006', 'CD006', 'BN006', 'NV001', DATEADD(DAY, -6, GETDATE()), N'Ngoai tru', 180000.00, N'Viêm loét dạ dày tá tràng', N'DaCapPhat', N'Điều trị viêm dạ dày - BHYT 80%', GETDATE()),

-- Đơn thuốc tự túc (không BHYT)
('DT007', 'CD007', 'BN007', 'NV005', DATEADD(DAY, -1, GETDATE()), N'Ngoai tru', 50000.00, N'Cảm cúm', N'DaCapPhat', N'Cảm cúm - Tự túc', GETDATE()),
('DT008', 'CD008', 'BN008', 'NV001', DATEADD(DAY, -4, GETDATE()), N'Ngoai tru', 216000.00, N'Đái tháo đường type 2', N'DaCapPhat', N'Đái tháo đường - Thẻ hết hạn', GETDATE()),

-- Đơn thuốc trẻ em
('DT009', 'CD009', 'BN009', 'NV004', DATEADD(DAY, -1, GETDATE()), N'Ngoai tru', 80000.00, N'Viêm đường hô hấp trên', N'DaCapPhat', N'Viêm hô hấp trẻ em - BHYT 100%', GETDATE()),
('DT010', 'CD010', 'BN010', 'NV004', GETDATE(), N'Ngoai tru', 60000.00, N'Tiêu chảy cấp', N'DaCapPhat', N'Tiêu chảy trẻ em - Không BHYT', GETDATE());
GO

-- 11. Chi tiết đơn thuốc
INSERT INTO ChiTietDonThuoc (MaDonThuoc, MaThuoc, SoLuong, LieuDung, SoNgayDung, CachDung, GhiChu) VALUES
-- DT001: Viêm phổi (BHYT)
('DT001', 'T012', 10, N'1 chai/ngày', 5, N'Truyền tĩnh mạch', N'Bù nước điện giải'),
('DT001', 'T013', 10, N'1 chai/ngày', 5, N'Truyền tĩnh mạch', N'Bù năng lượng'),
('DT001', 'T014', 8, N'1 lọ/12 giờ', 4, N'Tiêm tĩnh mạch', N'Kháng sinh mạnh điều trị viêm phổi'),

-- DT002: Sau sinh (BHYT)
('DT002', 'T002', 14, N'2 viên/lần x 2 lần/ngày', 7, N'Uống sau ăn', N'Phòng nhiễm trùng sau sinh'),
('DT002', 'T003', 30, N'1 viên/ngày', 30, N'Hòa tan với nước ấm', N'Bổ sung vitamin C tăng sức đề kháng'),

-- DT003: Sau PT (BHYT)
('DT003', 'T014', 6, N'1 lọ/12 giờ', 3, N'Tiêm tĩnh mạch', N'Kháng sinh phòng nhiễm trùng sau mổ'),
('DT003', 'T001', 30, N'1-2 viên khi đau', 15, N'Uống khi đau, không quá 8 viên/ngày', N'Giảm đau vết mổ'),
('DT003', 'T012', 3, N'1 chai/ngày', 3, N'Truyền tĩnh mạch', N'Bù nước sau phẫu thuật'),

-- DT004: Viêm họng (BHYT)
('DT004', 'T002', 10, N'1 viên/lần x 3 lần/ngày', 5, N'Uống sau ăn', N'Kháng sinh điều trị viêm họng'),
('DT004', 'T001', 15, N'1-2 viên khi sốt', 5, N'Uống khi sốt trên 38.5°C', N'Hạ sốt, giảm đau họng'),

-- DT005: Tăng huyết áp (BHYT)
('DT005', 'T007', 30, N'1 viên/ngày', 30, N'Uống vào buổi sáng', N'Thuốc hạ huyết áp'),
('DT005', 'T006', 30, N'1 viên/ngày', 30, N'Uống vào buổi tối trước ngủ', N'Giảm cholesterol trong máu'),

-- DT006: Viêm dạ dày (BHYT)
('DT006', 'T004', 30, N'1 viên/lần x 2 lần/ngày', 15, N'Uống trước ăn 30 phút', N'Ức chế acid dạ dày'),

-- DT007: Cảm cúm (Tự túc - không BHYT)
('DT007', 'T001', 20, N'2 viên khi sốt hoặc đau', 5, N'Uống khi sốt/đau, cách nhau ít nhất 4 giờ', N'Hạ sốt, giảm đau'),
('DT007', 'T003', 10, N'1 viên/ngày', 10, N'Hòa tan với nước ấm, uống sáng', N'Tăng sức đề kháng'),

-- DT008: Đái tháo đường (Tự túc - thẻ hết hạn)
('DT008', 'T005', 60, N'1 viên/lần x 2 lần/ngày', 30, N'Uống cùng bữa ăn sáng và tối', N'Hạ đường huyết'),

-- DT009: Viêm hô hấp trẻ em (BHYT)
('DT009', 'T001', 10, N'1/2 viên khi sốt', 5, N'Uống khi sốt trên 38°C', N'Liều giảm đau cho trẻ'),
('DT009', 'T008', 10, N'1/2 viên x 2 lần/ngày', 5, N'Uống sau ăn', N'Giãn phế quản, giảm ho'),

-- DT010: Tiêu chảy trẻ em (BHYT)
('DT010', 'T012', 2, N'100ml x 4-6 lần/ngày', 2, N'Cho uống từ từ trong ngày', N'Bù nước điện giải cho trẻ tiêu chảy');
GO

-- 12. Cấu hình hệ thống
INSERT INTO CauHinhHeThong (MaCauHinh, TenCauHinh, GiaTri, MoTa, LoaiDuLieu, NgayTao) VALUES
('CFG001', 'TonKhoToiThieu', '10', N'Số lượng tồn kho tối thiểu mặc định', N'INT', GETDATE()),
('CFG002', 'SoNgayHetHan', '90', N'Cảnh báo thuốc hết hạn trong vòng X ngày', N'INT', GETDATE()),
('CFG003', 'TyLeChietKhauBHYT', '80', N'Tỷ lệ BHYT chi trả mặc định (%)', N'DECIMAL', GETDATE()),
('CFG004', 'SoNgayHanThanhToan', '7', N'Số ngày cho phép trả sau', N'INT', GETDATE()),
('CFG005', 'ChoPhepTraThuoc', 'true', N'Cho phép bệnh nhân trả thuốc', N'VARCHAR', GETDATE()),
('CFG006', 'ThoiGianHieuLucQR', '24', N'Số giờ hiệu lực mã QR thanh toán', N'INT', GETDATE()),
('CFG007', 'VNeID_APIURL', 'https://api.vneid.gov.vn/v1', N'URL API VNeID', N'VARCHAR', GETDATE()),
('CFG008', 'VNeID_APIKey', 'BV_API_KEY_2026_XYZ123', N'API Key để truy cập VNeID', N'VARCHAR', GETDATE());
GO

-- 13. Cấu hình VietQR
INSERT INTO CauHinh_VietQR (MaCauHinh, TenNganHang, SoTaiKhoan, TenTaiKhoan, MaNganHang, Template, WebhookURL, APIKey, APISecret, TrangThai, GhiChu, NgayTao) VALUES
('QR001', N'Ngân hàng TMCP Ngoại thương Việt Nam', '1234567890', N'BENH VIEN X', '970436', 'compact2', 
 'https://benhvienx.vn/api/webhook/vietqr', 'QR_API_KEY_2026', 'QR_SECRET_2026', N'KichHoat', N'Tài khoản chính', GETDATE());
GO

-- 14. Tài khoản (cho nhân viên)
INSERT INTO TaiKhoan (MaTaiKhoan, MaNhanVien, Username, PasswordHash, Role, TrangThai, NgayTao) VALUES
-- Bác sĩ
('TK001', 'NV001', 'bs.nguyenvana', 'HASHED_PASSWORD_1', N'BacSi', N'KichHoat', GETDATE()),
('TK002', 'NV002', 'bs.tranthib', 'HASHED_PASSWORD_2', N'BacSi', N'KichHoat', GETDATE()),
('TK003', 'NV003', 'bs.levanc', 'HASHED_PASSWORD_3', N'BacSi', N'KichHoat', GETDATE()),
('TK004', 'NV004', 'bs.phamthid', 'HASHED_PASSWORD_4', N'BacSi', N'KichHoat', GETDATE()),
('TK005', 'NV005', 'bs.hoangvane', 'HASHED_PASSWORD_5', N'BacSi', N'KichHoat', GETDATE()),

-- Dược sĩ
('TK006', 'NV006', 'ds.vuthihuong', 'HASHED_PASSWORD_6', N'DuocSi', N'KichHoat', GETDATE()),
('TK007', 'NV007', 'ds.dovanphuc', 'HASHED_PASSWORD_7', N'DuocSi', N'KichHoat', GETDATE()),

-- Thu ngân
('TK008', 'NV008', 'tn.buithilan', 'HASHED_PASSWORD_8', N'ThuNgan', N'KichHoat', GETDATE()),
('TK009', 'NV009', 'tn.nguyenvanminh', 'HASHED_PASSWORD_9', N'ThuNgan', N'KichHoat', GETDATE()),

-- Kho thuốc
('TK010', 'NV010', 'kho.tranvanson', 'HASHED_PASSWORD_10', N'KhoThuoc', N'KichHoat', GETDATE());
GO

PRINT N'✓ Đã insert dữ liệu BenhVienX Database';
PRINT N'';
PRINT N'=================================================================';
PRINT N'KẾT QUẢ TÍCH HỢP 2 DATABASE';
PRINT N'=================================================================';
PRINT N'';
PRINT N'VNeID Database:';
PRINT N'  - 2 Cấu hình tích hợp (cho phép BenhVienX truy cập)';
PRINT N'  - 10 Công dân (có số CCCD)';
PRINT N'  - 8 Thẻ BHYT (2 người không có thẻ)';
PRINT N'  - 10 Lịch sử tra cứu (BenhVienX đã tra cứu)';
PRINT N'';
PRINT N'BenhVienX Database:';
PRINT N'  - 3 Kho thuốc';
PRINT N'  - 5 Khoa phòng';
PRINT N'  - 10 Nhân viên (5 BS, 2 DS, 2 TN, 1 Kho)';
PRINT N'  - 3 Nhà cung cấp';
PRINT N'  - 15 Loại thuốc (8 BHYT, 3 DV, 4 tiêm)';
PRINT N'  - 45 Lô thuốc (3 lô/thuốc)';
PRINT N'  - 10 Bệnh nhân (liên kết với VNeID qua CCCD)';
PRINT N'  - 8 Thẻ BHYT (mapping từ VNeID)';
PRINT N'  - 10 Chẩn đoán';
PRINT N'  - 10 Đơn thuốc với chi tiết';
PRINT N'  - 8 Cấu hình hệ thống';
PRINT N'  - 1 Cấu hình VietQR';
PRINT N'  - 10 Tài khoản nhân viên';
PRINT N'';
PRINT N'LIÊN KẾT DỮ LIỆU:';
PRINT N'  ✓ Số CCCD ở VNeID.CongDan = BenhVienX.BenhNhan.CCCD';
PRINT N'  ✓ Số thẻ BHYT ở VNeID.TheBHYT_Mock = BenhVienX.TheBHYT.SoTheBHYT';
PRINT N'  ✓ BenhVienX có thể tra cứu thông tin công dân từ VNeID';
PRINT N'  ✓ BenhVienX có thể kiểm tra thẻ BHYT từ VNeID';
PRINT N'';
PRINT N'TÌNH HUỐNG THỰC TẾ:';
PRINT N'  - BN001-003: Nội trú, có BHYT, đã được tra cứu';
PRINT N'  - BN004-006: Ngoại trú, có BHYT, đã được tra cứu';
PRINT N'  - BN007: Ngoại trú, KHÔNG có BHYT, VNeID trả về không tìm thấy';
PRINT N'  - BN008: Ngoại trú, có BHYT nhưng HẾT HẠN';
PRINT N'  - BN009: Trẻ em, có BHYT 100%, đã được tra cứu';
PRINT N'  - BN010: Trẻ em, KHÔNG có BHYT';
PRINT N'';
PRINT N'=================================================================';
GO