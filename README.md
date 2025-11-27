<p align="center">
  <img src="https://huflit.edu.vn/wp-content/uploads/2024/03/HUFLIT_Logo_English_Official.png" 
       alt="HUFLIT Logo" width="200">
</p>

# HAVENIX STORE — WEBSITE BÁN HÀNG THỜI TRANG
### Đồ án môn Lập Trình Trên Web  
### Nhóm 4 — Trường Đại học Ngoại ngữ – Tin học TP. Hồ Chí Minh (HUFLIT)

---

# Mục lục
- [1. Giới thiệu](#1-giới-thiệu)
- [2. Thành viên nhóm và phân công công việc](#2-thành-viên-nhóm-và-phân-công-công-việc)
- [3. Giao diện minh họa](#3-giao-diện-minh-họa)
- [4. Mục tiêu](#4-mục-tiêu)
- [5. Chức năng hệ thống](#5-chức-năng-hệ-thống)
  - [5.1. Chức năng người dùng (User)](#51-chức-năng-người-dùng-user)
  - [5.2. Chức năng quản trị (Admin)](#52-chức-năng-quản-trị-admin)
- [6. Công nghệ sử dụng](#6-công-nghệ-sử-dụng)
- [7. Cơ sở dữ liệu](#7-cơ-sở-dữ-liệu)
- [8. Quy trình xử lý đơn hàng](#8-quy-trình-xử-lý-đơn-hàng)
- [9. Hướng dẫn cài đặt & chạy dự án](#9-hướng-dẫn-cài-đặt--chạy-dự-án)
- [10. Hướng dẫn deploy](#10-hướng-dẫn-deploy)
  - [10.1. Deploy trên IIS](#101-deploy-trên-iis)
  - [10.2. Deploy lên hosting Windows](#102-deploy-lên-hosting-windows)
- [11. Kết quả đạt được](#11-kết-quả-đạt-được)
- [12. Hạn chế](#12-hạn-chế)
- [13. Định hướng phát triển](#13-định-hướng-phát-triển)
- [14. Lời cảm ơn](#14-lời-cảm-ơn)

---

# 1. Giới thiệu
HAVENIX STORE là website bán hàng thời trang được xây dựng bằng ASP.NET MVC 5 kết hợp SQL Server, mô phỏng đầy đủ quy trình thương mại điện tử cơ bản: xem sản phẩm, giỏ hàng, thanh toán và quản trị nội dung.

---

# 2. Thành viên nhóm và phân công công việc

| STT | Họ và tên                  | MSSV       | Nhiệm vụ |
|-----|----------------------------|------------|----------|
| 1   | Nguyễn Hải Nam             | 24DH111148 | Giỏ hàng; Đăng ký/Đăng nhập; Quản lý User; Phân quyền; Cập nhật Admin |
| 2   | Nguyễn Hoàng Phước Anh     | 24DH110066 | Trang chủ; Danh sách sản phẩm; Chi tiết sản phẩm; UI/UX |
| 3   | Trương Gia Thuận           | 24DH111812 | Admin CRUD; Dashboard; Nhật ký hệ thống; Báo cáo; Kiểm thử |

---

# 3. Giao diện minh họa

### Trang chủ
<p align="center">
  <img src="https://i.ibb.co/tMKyrP18/z7267021645274-b678c3da7e3c.jpg" 
       alt="Trang chủ HAVENIX" style="max-width:100%; border-radius: 10px;">
</p>

### Danh sách sản phẩm
![Product List](https://via.placeholder.com/1200x500?text=Product+List)

### Chi tiết sản phẩm
![Product Detail](https://via.placeholder.com/1200x500?text=Product+Detail)

### Giỏ hàng
![Cart](https://via.placeholder.com/1200x500?text=Cart+Page)

### Dashboard Admin
![Dashboard](https://via.placeholder.com/1200x500?text=Admin+Dashboard)

---

# 4. Mục tiêu
- Xây dựng website bán hàng hoàn chỉnh.  
- Thiết kế giao diện hiện đại, dễ sử dụng.  
- Xử lý đầy đủ nghiệp vụ mua sắm – thanh toán – quản trị.  

---

# 5. Chức năng hệ thống

## 5.1. Chức năng người dùng (User)

- Đăng ký / Đăng nhập  
- Xem danh sách sản phẩm  
- Chi tiết sản phẩm  
- Lọc sản phẩm  
- Giỏ hàng  
- Thanh toán COD hoặc chuyển khoản  
- Theo dõi đơn hàng  
- Đánh giá sản phẩm  

## 5.2. Chức năng quản trị (Admin)

- CRUD sản phẩm  
- Quản lý danh mục  
- Quản lý người dùng  
- Quản lý đơn hàng  
- Xác minh thanh toán  
- Dashboard thống kê  
- Nhật ký hoạt động  
- Gửi thông báo  

---

# 6. Công nghệ sử dụng
- C#, ASP.NET MVC 5  
- Entity Framework  
- SQL Server  
- Bootstrap 5  
- Visual Studio 2022  

---

# 7. Cơ sở dữ liệu
Hệ thống gồm các bảng chính:  
Users, UserInfo, Products, Orders, OrderDetails, Cart, CartItems, Payments, Reviews, AdminActivityLog.  
Thiết kế theo mô hình quan hệ, chuẩn hóa và dễ mở rộng.

---

# 8. Quy trình xử lý đơn hàng

1. Người dùng chọn sản phẩm → thêm vào giỏ hàng  
2. Chọn hình thức thanh toán COD hoặc chuyển khoản  
3. Lưu đơn hàng  
4. Admin xác minh thanh toán (nếu chuyển khoản)  
5. Cập nhật trạng thái đơn  
6. Người dùng được thông báo  

---

# 9. Hướng dẫn cài đặt & chạy dự án

## Yêu cầu
- Visual Studio 2022  
- .NET Framework 4.8  
- SQL Server + SSMS  

## Các bước cài đặt

### Bước 1: Clone repo
```
git clone https://github.com/hnam1204/WEBSITE_HAVENIX_NHOM4_NAM2.git
```

### Bước 2: Import database
- Tạo database HAVENIX  
- Import file `havenix.bacpac` bằng SSMS  

### Bước 3: Cập nhật Connection String
```
<add name="AppDbContext" 
     connectionString="Data Source=SERVERNAME;Initial Catalog=HAVENIX;Integrated Security=True;" 
     providerName="System.Data.SqlClient" />
```
⚠ Thay `SERVERNAME` bằng tên server SQL của bạn.

### Bước 4: Chạy dự án  
Nhấn **IIS Express** trên Visual Studio.

---

# 10. Hướng dẫn deploy

## 10.1. Deploy trên IIS
- Bật IIS & ASP.NET  
- Publish thành Folder  
- Add Website trong IIS  
- Trỏ đến thư mục publish  

## 10.2. Deploy lên hosting Windows
- Publish Web Deploy  
- Nhập thông tin hosting  
- Restore database  
- Cập nhật Connection String  

---

# 11. Kết quả đạt được
- Website hoạt động đầy đủ chức năng  
- Giao diện trực quan  
- Hệ thống Admin mạnh mẽ và rõ ràng  

---

# 12. Hạn chế
- Responsive mobile chưa tối ưu hoàn toàn  

---

# 13. Định hướng phát triển
- Tích hợp thanh toán online  
- Chatbot tự động hỗ trợ  
- API Restful  
- Dashboard phân tích nâng cao  

---

# 14. Lời cảm ơn
Nhóm xin gửi lời tri ân sâu sắc đến giảng viên **ThS. Đinh Minh Hòa** đã tận tình hướng dẫn, hỗ trợ và truyền cảm hứng trong suốt quá trình thực hiện đồ án.
