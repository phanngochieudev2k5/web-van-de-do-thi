-- Tạo Database
CREATE DATABASE SmartCitizenDB;
GO
USE SmartCitizenDB;
GO

-- ==========================================
-- PHẦN 1: CÁC BẢNG DANH MỤC GỐC (KHÔNG CHỨA KHÓA NGOẠI)
-- ==========================================

-- 1. Bảng Quyền hạn
CREATE TABLE tbl_Roles (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(50) NOT NULL, -- Admin, Citizen, Department
    description NVARCHAR(255)
);

-- 2. Bảng Danh mục vấn đề
CREATE TABLE tbl_Categories (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(100) NOT NULL,
    icon VARCHAR(50)
);

-- 3. Bảng Phòng ban xử lý
CREATE TABLE tbl_Departments (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(150) NOT NULL,
    contact_email VARCHAR(100)
);

-- 4. Bảng Huy hiệu (Gamification)
CREATE TABLE tbl_Badges (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(100) NOT NULL,
    image_url VARCHAR(255),
    required_reports INT DEFAULT 0
);

-- 5. Bảng Phần thưởng / Voucher
CREATE TABLE tbl_Rewards (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(200) NOT NULL,
    point_cost INT NOT NULL,
    stock_quantity INT DEFAULT 0,
    is_active BIT DEFAULT 1
);

-- ==========================================
-- PHẦN 2: CÁC BẢNG NGƯỜI DÙNG & HỆ THỐNG
-- ==========================================

-- 6. Bảng Người dùng
CREATE TABLE tbl_Users (
    id INT IDENTITY(1,1) PRIMARY KEY,
    role_id INT NOT NULL,
    full_name NVARCHAR(100) NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    trust_points INT DEFAULT 0,
    status TINYINT DEFAULT 1, -- 1: Active, 0: Blocked
    created_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (role_id) REFERENCES tbl_Roles(id)
);

-- 7. Bảng Quản lý Menu động
CREATE TABLE tbl_Menus (
    id INT IDENTITY(1,1) PRIMARY KEY,
    title NVARCHAR(100) NOT NULL,
    url VARCHAR(255) NOT NULL,
    icon VARCHAR(50),
    parent_id INT NULL,
    display_order INT DEFAULT 0,
    required_role_id INT NULL,
    is_active BIT DEFAULT 1,
    FOREIGN KEY (parent_id) REFERENCES tbl_Menus(id),
    FOREIGN KEY (required_role_id) REFERENCES tbl_Roles(id)
);

-- ==========================================
-- PHẦN 3: CÁC BẢNG TƯƠNG TÁC GAMIFICATION
-- ==========================================

-- 8. Bảng Công dân - Huy hiệu (Quan hệ n-n)
CREATE TABLE tbl_User_Badges (
    user_id INT NOT NULL,
    badge_id INT NOT NULL,
    earned_at DATETIME DEFAULT GETDATE(),
    PRIMARY KEY (user_id, badge_id),
    FOREIGN KEY (user_id) REFERENCES tbl_Users(id),
    FOREIGN KEY (badge_id) REFERENCES tbl_Badges(id)
);

-- 9. Bảng Lịch sử đổi quà
CREATE TABLE tbl_User_Rewards (
    id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    reward_id INT NOT NULL,
    redeemed_at DATETIME DEFAULT GETDATE(),
    voucher_code VARCHAR(50) UNIQUE NOT NULL,
    FOREIGN KEY (user_id) REFERENCES tbl_Users(id),
    FOREIGN KEY (reward_id) REFERENCES tbl_Rewards(id)
);

-- ==========================================
-- PHẦN 4: CÁC BẢNG NGHIỆP VỤ CỐT LÕI (BÁO CÁO SỰ CỐ)
-- ==========================================

-- 10. Bảng Phản ánh / Báo cáo
CREATE TABLE tbl_Reports (
    id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    category_id INT NOT NULL,
    department_id INT NULL, -- Có thể NULL khi chưa được Admin phân công
    title NVARCHAR(200) NOT NULL,
    description NVARCHAR(MAX) NOT NULL,
    severity TINYINT DEFAULT 1, -- 1: Low, 2: Medium, 3: High
    status VARCHAR(50) DEFAULT 'PENDING', -- PENDING, APPROVED, IN_PROGRESS, RESOLVED, REJECTED
    ward_code VARCHAR(50),
    address NVARCHAR(255),
    latitude DECIMAL(10, 8),
    longitude DECIMAL(11, 8),
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME DEFAULT GETDATE(),
    resolved_at DATETIME NULL,
    FOREIGN KEY (user_id) REFERENCES tbl_Users(id),
    FOREIGN KEY (category_id) REFERENCES tbl_Categories(id),
    FOREIGN KEY (department_id) REFERENCES tbl_Departments(id)
);

-- 11. Bảng Hình ảnh của phản ánh (1 phản ánh có nhiều ảnh)
CREATE TABLE tbl_Report_Images (
    id INT IDENTITY(1,1) PRIMARY KEY,
    report_id INT NOT NULL,
    image_url VARCHAR(255) NOT NULL,
    FOREIGN KEY (report_id) REFERENCES tbl_Reports(id) ON DELETE CASCADE
);

-- 12. Bảng Tương tác (Like/Dislike)
CREATE TABLE tbl_Interactions (
    id INT IDENTITY(1,1) PRIMARY KEY,
    report_id INT NOT NULL,
    user_id INT NOT NULL,
    type VARCHAR(20) NOT NULL, -- 'LIKE' hoặc 'DISLIKE'
    created_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (report_id) REFERENCES tbl_Reports(id) ON DELETE CASCADE,
    FOREIGN KEY (user_id) REFERENCES tbl_Users(id)
);

-- 13. Bảng Bình luận
CREATE TABLE tbl_Comments (
    id INT IDENTITY(1,1) PRIMARY KEY,
    report_id INT NOT NULL,
    user_id INT NOT NULL,
    content NVARCHAR(MAX) NOT NULL,
    created_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (report_id) REFERENCES tbl_Reports(id) ON DELETE CASCADE,
    FOREIGN KEY (user_id) REFERENCES tbl_Users(id)
);

INSERT INTO tbl_Roles (name, description) VALUES
(N'Admin', N'Quản trị viên toàn hệ thống'),
(N'Citizen', N'Công dân sử dụng ứng dụng'),
(N'Department', N'Cán bộ phòng ban xử lý sự cố');

INSERT INTO tbl_Categories (name, icon) VALUES
(N'Giao thông & Hạ tầng', 'traffic-light'),
(N'Môi trường & Rác thải', 'trash'),
(N'Chiếu sáng đô thị', 'lightbulb'),
(N'An ninh trật tự', 'shield'),
(N'Cây xanh & Công viên', 'tree');

INSERT INTO tbl_Departments (name, contact_email) VALUES
(N'Công ty CP Môi trường và Công trình Đô thị Nghệ An', 'moitruong@vinh.vn'),
(N'Đội CSGT - Công an TP Vinh', 'csgt@vinh.vn'),
(N'Công ty Điện lực TP Vinh', 'dienluc@vinh.vn'),
(N'Công ty CP Công viên Cây xanh TP Vinh', 'cayxanh@vinh.vn'),
(N'UBND Phường Trường Thi', 'ubnd.truongthi@vinh.vn'),
(N'UBND Phường Bến Thủy', 'ubnd.benthuy@vinh.vn');

INSERT INTO tbl_Badges (name, image_url, required_reports) VALUES
(N'Mắt thần giao thông', '/badges/traffic-eye.png', 10),
(N'Hiệp sĩ môi trường', '/badges/eco-knight.png', 5),
(N'Chuyên gia hạ tầng', '/badges/infra-pro.png', 5),
(N'Người dẫn đầu', '/badges/leader.png', 20),
(N'Phản ứng nhanh', '/badges/fast-response.png', 1);

INSERT INTO tbl_Rewards (name, point_cost, stock_quantity, is_active) VALUES
(N'Voucher 30k Highlands Coffee Vincom Vinh', 500, 100, 1),
(N'Vé tháng xe buýt nội đô TP Vinh (Tuyến 01, 02)', 1500, 50, 1),
(N'Giảm 50k hóa đơn Tiền nước Nghệ An', 3000, 200, 1),
(N'Voucher xem phim tại Lotte Cinema Vinh', 800, 75, 1),
(N'Voucher 50k Nhà sách Fahasa (Tầng 2 Co.opmart)', 600, 120, 1);

INSERT INTO tbl_Users (role_id, full_name, email, password_hash, trust_points, status) VALUES
(1, N'Quản trị Hệ thống', 'admin@vinh.vn', 'hashed_pwd_123', 0, 1),
(2, N'Thạch Phan Bảo', 'baothach@gmail.com', 'hashed_pwd_123', 2450, 1),
(2, N'Nguyễn Thị Mai', 'mainguyen@gmail.com', 'hashed_pwd_123', 1200, 1),
(2, N'Trần Văn Nam', 'namtran.vinh@gmail.com', 'hashed_pwd_123', 850, 1),
(2, N'Lê Hoàng Dũng', 'dunghoang99@gmail.com', 'hashed_pwd_123', 300, 1),
(3, N'Cán bộ Môi trường', 'cb.moitruong@vinh.vn', 'hashed_pwd_123', 0, 1),
(3, N'Cán bộ Giao thông', 'cb.giaothong@vinh.vn', 'hashed_pwd_123', 0, 1);

INSERT INTO tbl_Menus (title, url, icon, parent_id, display_order, required_role_id, is_active) VALUES
(N'Trang chủ', '/', 'home', NULL, 1, NULL, 1),
(N'Bản đồ số', '/map', 'map', NULL, 2, NULL, 1),
(N'Gửi phản ánh', '/report', 'plus', NULL, 3, NULL, 1),
(N'Bảng xếp hạng', '/leaderboard', 'trophy', NULL, 4, NULL, 1),
(N'Cửa hàng đổi quà', '/rewards', 'gift', NULL, 5, NULL, 1),
(N'Quản lý hệ thống', '/admin', 'settings', NULL, 6, 1, 1);

INSERT INTO tbl_User_Badges (user_id, badge_id) VALUES
(2, 1),
(2, 2),
(3, 5),
(4, 3),
(5, 5);

INSERT INTO tbl_User_Rewards (user_id, reward_id, voucher_code) VALUES
(2, 1, 'HLV-VINCOM-X89J2'),
(2, 4, 'LOTTE-VINH-P9K1L'),
(3, 5, 'FAHASA-COOP-A1B2'),
(4, 1, 'HLV-VINCOM-Z77X9'),
(2, 2, 'BUS-TPVINH-M4N5');

INSERT INTO tbl_Reports (user_id, category_id, department_id, title, description, severity, status, ward_code, address, latitude, longitude) VALUES
(2, 1, 2, N'Ngập úng cục bộ sau mưa lớn', N'Nước ngập sâu hơn 30cm trước cổng Trường Đại học Vinh, xe cộ chết máy hàng loạt.', 3, 'IN_PROGRESS', 'ben_thuy', N'Số 182 Lê Duẩn', 18.660144, 105.696152),
(3, 2, 1, N'Tập kết rác thải sai quy định', N'Có một đống rác lớn bốc mùi hôi thối bị đổ trộm ven hồ Goong.', 2, 'PENDING', 'truong_thi', N'Đường Phan Đăng Lưu, ven Hồ Goong', 18.673210, 105.688123),
(4, 1, 2, N'Đèn giao thông bị hỏng', N'Đèn tín hiệu tại ngã tư bị tắt hoàn toàn gây lộn xộn giao thông vào giờ cao điểm.', 3, 'RESOLVED', 'ha_huy_tap', N'Ngã tư Đại lộ V.I. Lê Nin và Phạm Đình Toái', 18.692341, 105.681122),
(2, 5, 4, N'Cành cây gãy nguy hiểm', N'Cành xà cừ lớn bị gãy treo lơ lửng trên đường, nguy cơ rơi trúng người đi đường.', 3, 'APPROVED', 'quang_trung', N'Số 50 Quang Trung', 18.675681, 105.674512),
(5, 4, 5, N'Quán nhậu lấn chiếm vỉa hè', N'Quán bày bàn ghế kín vỉa hè, người đi bộ phải đi xuống lòng đường rất nguy hiểm.', 2, 'PENDING', 'truong_thi', N'Đường Nguyễn Văn Cừ', 18.676110, 105.685231);

INSERT INTO tbl_Report_Images (report_id, image_url) VALUES
(1, '/uploads/reports/ngap-le-duan-1.jpg'),
(1, '/uploads/reports/ngap-le-duan-2.jpg'),
(2, '/uploads/reports/rac-ho-goong.jpg'),
(3, '/uploads/reports/den-hong-lenin.jpg'),
(4, '/uploads/reports/cay-gay-quang-trung.jpg');

INSERT INTO tbl_Interactions (report_id, user_id, type) VALUES
(1, 3, 'LIKE'),
(1, 4, 'LIKE'),
(1, 5, 'LIKE'),
(3, 2, 'LIKE'),
(4, 3, 'LIKE');

INSERT INTO tbl_Comments (report_id, user_id, content) VALUES
(1, 3, N'Khu vực này cứ mưa là ngập, mong thành phố sớm nâng cấp hệ thống cống thoát nước ở đây.'),
(3, 2, N'Đội CSGT xử lý nhanh quá, đi ngang qua đã thấy đèn hoạt động lại bình thường rồi.'),
(2, 4, N'Người dân vô ý thức quá, hồ Goong đang đẹp lại mang rác ra vứt.'),
(4, 5, N'Hôm qua gió to nên cành cây bị tước, các bác đi qua đường Quang Trung cẩn thận nhé.'),
(1, 6, N'Chúng tôi đã tiếp nhận thông tin và đang cử máy bơm hút nước tại khu vực này.');