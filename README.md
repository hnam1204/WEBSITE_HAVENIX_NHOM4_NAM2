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


### Danh sách sản phẩm  


### Chi tiết sản phẩm  


### Giỏ hàng  


### Dashboard Admin  


---

# 4. Mục tiêu
- Xây dựng website bán hàng đầy đủ chức năng.  
- Thiết kế giao diện hiện đại, trực quan, dễ sử dụng.  
- Hoàn thiện toàn bộ nghiệp vụ mua sắm, hóa đơn và quản trị nội dung.  

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

- Quản lý sản phẩm  
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

Thiết kế theo chuẩn quan hệ, tối ưu để mở rộng và xử lý nghiệp vụ thương mại điện tử.

---

# 8. Quy trình xử lý đơn hàng

1. Người dùng chọn sản phẩm và thêm vào giỏ hàng  
2. Chọn phương thức thanh toán  
3. Tạo đơn hàng  
4. Admin xác minh (nếu thanh toán online)  
5. Cập nhật trạng thái  
6. Thông báo đến người dùng  

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

### Bước 4: Chạy dự án  
Nhấn **IIS Express** trong Visual Studio.

---

# 10. Hướng dẫn deploy

## 10.1. Deploy trên IIS
- Bật IIS & ASP.NET  
- Publish dạng Folder  
- Tạo Website mới trong IIS  
- Trỏ đến thư mục publish  

## 10.2. Deploy lên hosting Windows
- Publish bằng Web Deploy  
- Nhập thông tin hosting  
- Restore database  
- Cập nhật Connection String phù hợp  

---

# 11. Kết quả đạt được
- Website hoàn chỉnh, đầy đủ nghiệp vụ  
- Giao diện sạch, trực quan  
- Hệ thống Admin mạnh mẽ và tối ưu  

---

# 12. Hạn chế
- Tối ưu mobile cần cải thiện thêm  
- Chưa tích hợp thanh toán tự động  

---

# 13. Định hướng phát triển
- Tích hợp thanh toán online (Momo, ZaloPay…)  
- API Restful cho mobile app  
- Dashboard nâng cao  
- Chatbot hỗ trợ khách hàng  

---

# 14. Lời cảm ơn
Nhóm xin gửi lời tri ân sâu sắc đến giảng viên **ThS. Đinh Minh Hòa** — người đã tận tình hướng dẫn, giải đáp và truyền cảm hứng trong suốt quá trình thực hiện đồ án.

Nhờ sự tận tâm và phương pháp giảng dạy đầy nhiệt huyết của thầy, nhóm hiểu rõ hơn về quy trình xây dựng một website hoàn chỉnh: từ giao diện, xử lý logic, đến cơ sở dữ liệu và vận hành hệ thống.  

Trong thời gian thực hiện đồ án, thầy luôn tạo điều kiện để nhóm phát huy khả năng, tiếp cận kiến thức mới và rèn luyện tư duy lập trình chuyên nghiệp.  

Thành quả của đồ án hôm nay là minh chứng cho sự hỗ trợ và định hướng quý báu mà thầy đã dành cho chúng em.  

Một lần nữa, nhóm xin gửi lời cảm ơn chân thành và kính chúc thầy thật nhiều sức khỏe, thành công và giữ mãi ngọn lửa nhiệt huyết với nghề.
