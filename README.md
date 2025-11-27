# HAVENIX STORE --- WEBSITE BÁN HÀNG THỜI TRANG

### Đồ án môn Lập Trình Trên Web

### Nhóm 4 --- Trường Đại học Ngoại ngữ -- Tin học TP. Hồ Chí Minh (HUFLIT)

------------------------------------------------------------------------

# Mục lục

-   [1. Giới thiệu](#1-giới-thiệu)
-   [2. Thành viên nhóm và phân công công
    việc](#2-thành-viên-nhóm-và-phân-công-công-việc)
-   [3. Giao diện minh họa](#3-giao-diện-minh-họa)
-   [4. Mục tiêu](#4-mục-tiêu)
-   [5. Chức năng hệ thống](#5-chức-năng-hệ-thống)
    -   [5.1. Chức năng người dùng
        (User)](#51-chức-năng-người-dùng-user)
    -   [5.2. Chức năng quản trị (Admin)](#52-chức-năng-quản-trị-admin)
-   [6. Công nghệ sử dụng](#6-công-nghệ-sử-dụng)
-   [7. Cơ sở dữ liệu](#7-cơ-sở-dữ-liệu)
-   [8. Quy trình xử lý đơn hàng](#8-quy-trình-xử-lý-đơn-hàng)
-   [9. Hướng dẫn cài đặt & chạy dự
    án](#9-hướng-dẫn-cài-đặt--chạy-dự-án)
-   [10. Hướng dẫn deploy](#10-hướng-dẫn-deploy)
    -   [10.1. Deploy trên IIS](#101-deploy-trên-iis)
    -   [10.2. Deploy lên hosting
        Windows](#102-deploy-lên-hosting-windows)
-   [11. Kết quả đạt được](#11-kết-quả-đạt-được)
-   [12. Hạn chế](#12-hạn-chế)
-   [13. Định hướng phát triển](#13-định-hướng-phát-triển)
-   [14. Lời cảm ơn](#14-lời-cảm-ơn)

------------------------------------------------------------------------

# 1. Giới thiệu

HAVENIX STORE là website bán hàng thời trang được xây dựng bằng ASP.NET
MVC 5 kết hợp SQL Server...

------------------------------------------------------------------------

# 2. Thành viên nhóm và phân công công việc

  ------------------------------------------------------------------------
  STT    Họ và tên                            MSSV            Nhiệm vụ
  ------ ------------------------------------ --------------- ------------
  1      Nguyễn Hải Nam                       24DH111148      Giỏ hàng;
                                                              Đăng ký/Đăng
                                                              nhập; Quản
                                                              lý User;
                                                              Phân quyền;
                                                              Cập nhật
                                                              Admin

  2      Nguyễn Hoàng Phước Anh               24DH110066      Trang chủ;
                                                              Danh sách
                                                              sản phẩm;
                                                              Chi tiết sản
                                                              phẩm; UI/UX

  3      Trương Gia Thuận                     24DH111812      Quản trị
                                                              Admin; CRUD;
                                                              Dashboard;
                                                              Nhật ký; Báo
                                                              cáo; Kiểm
                                                              thử
  ------------------------------------------------------------------------

------------------------------------------------------------------------

# 3. Giao diện minh họa

### Trang chủ

![Trang chủ](https://via.placeholder.com/1200x500?text=Home+Page)

### Danh sách sản phẩm

![Product List](https://via.placeholder.com/1200x500?text=Product+List)

### Chi tiết sản phẩm

![Product
Detail](https://via.placeholder.com/1200x500?text=Product+Detail)

### Giỏ hàng

![Cart](https://via.placeholder.com/1200x500?text=Cart+Page)

### Dashboard Admin

![Dashboard](https://via.placeholder.com/1200x500?text=Admin+Dashboard)

------------------------------------------------------------------------

# 4. Mục tiêu

-   Xây dựng website bán hàng hoàn chỉnh.
-   Thiết kế giao diện hiện đại, dễ dùng.
-   Xử lý nghiệp vụ đầy đủ.

------------------------------------------------------------------------

# 5. Chức năng hệ thống

## 5.1. Chức năng người dùng (User)

-   Đăng ký / Đăng nhập
-   Xem sản phẩm
-   Chi tiết sản phẩm
-   Lọc sản phẩm
-   Giỏ hàng
-   Thanh toán COD / chuyển khoản
-   Theo dõi đơn hàng
-   Đánh giá sản phẩm

## 5.2. Chức năng quản trị (Admin)

-   CRUD sản phẩm
-   Quản lý danh mục
-   Quản lý người dùng
-   Quản lý đơn hàng
-   Xác minh thanh toán
-   Dashboard
-   Nhật ký hệ thống
-   Thông báo

------------------------------------------------------------------------

# 6. Công nghệ sử dụng

-   C#, ASP.NET MVC 5
-   SQL Server
-   Entity Framework
-   Bootstrap 5
-   Visual Studio 2022

------------------------------------------------------------------------

# 7. Cơ sở dữ liệu

Gồm các bảng: Users, UserInfo, Products, Orders, OrderDetails, Cart,
CartItems, Payments, Reviews, AdminActivityLog.

------------------------------------------------------------------------

# 8. Quy trình xử lý đơn hàng

1.  Người dùng chọn sản phẩm → Giỏ hàng\
2.  Thanh toán COD hoặc chuyển khoản\
3.  Admin xác minh đơn hàng\
4.  Cập nhật trạng thái đơn

------------------------------------------------------------------------

# 9. Hướng dẫn cài đặt & chạy dự án

## Yêu cầu

-   Visual Studio 2022\
-   .NET Framework 4.8\
-   SQL Server\
-   SSMS

## Các bước cài đặt

### Bước 1: Clone repo

    git clone https://github.com/hnam1204/WEBSITE_HAVENIX_NHOM4_NAM2.git

### Bước 2: Import database

-   Tạo database HAVENIX\
-   Chạy file HAVENIX.sql

### Bước 3: Cập nhật Connection String

    <add name="ShopPAEntities" connectionString="Data Source=**MSI\SQL2012R2**;Initial Catalog=havenix;Integrated Security=True;MultipleActiveResultSets=True" providerName="System.Data.SqlClient" /> /// cập nhật" MSI\SQL2012R2 "

### Bước 4: Chạy dự án bằng IIS Express

------------------------------------------------------------------------

# 10. Hướng dẫn deploy

## 10.1. Deploy trên IIS

-   Bật IIS và ASP.NET\
-   Publish Folder\
-   Add Website trong IIS\
-   Trỏ tới thư mục publish

## 10.2. Deploy lên hosting Windows

-   Publish Web Deploy\
-   Nhập thông tin hosting\
-   Restore database\
-   Cập nhật Connection String

------------------------------------------------------------------------

# 11. Kết quả đạt được

-   Hoàn thiện website HAVENIX\
-   Giao diện trực quan\
-   Trang Admin đầy đủ chức năng

------------------------------------------------------------------------

# 12. Hạn chế

-   Chưa có thanh toán online\
-   Responsive mobile cần tối ưu

------------------------------------------------------------------------

# 13. Định hướng phát triển

-   Thanh toán online\
-   Chatbot\
-   API Restful\
-   Dashboard nâng cao

------------------------------------------------------------------------

# 14. Lời cảm ơn

Trước hết, nhóm chúng em xin gửi lời tri ân sâu sắc nhất đến giảng viên phụ trách môn Lập trình Web – ThS. Đinh Minh Hòa. Trong suốt quá trình học tập và thực hiện đồ án, thầy không chỉ truyền đạt những kiến thức chuyên môn quý giá mà còn luôn tận tình hướng dẫn, giải đáp thắc mắc và đưa ra những lời khuyên vô cùng hữu ích để chúng em có thể hoàn thiện sản phẩm của mình.

Nhờ sự chỉ bảo tận tâm và phương pháp giảng dạy đầy nhiệt huyết của thầy, nhóm chúng em hiểu rõ hơn về quy trình xây dựng một website hoàn chỉnh: từ thiết kế giao diện người dùng, xử lý logic nghiệp vụ, đến cấu trúc cơ sở dữ liệu và vận hành hệ thống. Mỗi buổi học và mỗi góp ý của thầy đều mang lại cho chúng em thêm động lực, sự tự tin và cái nhìn thực tế hơn về lập trình Web – một lĩnh vực đang phát triển mạnh mẽ và đầy cơ hội.

Trong suốt thời gian thực hiện đồ án, thầy luôn tạo điều kiện tốt nhất để nhóm có thể phát huy khả năng, tiếp cận kiến thức mới và rèn luyện tư duy lập trình một cách chuyên nghiệp. Những chia sẻ và định hướng của thầy không chỉ giúp nhóm giải quyết các khó khăn kỹ thuật mà còn truyền cảm hứng để chúng em hoàn thiện sản phẩm với tinh thần trách nhiệm cao nhất.

Nhóm 4 xin chân thành cảm ơn thầy vì sự tận tụy, sự kiên nhẫn và những đóng góp thầm lặng mà thầy dành cho chúng em. Thành quả của đồ án hôm nay là kết quả của cả quá trình học tập nghiêm túc dưới sự hướng dẫn tâm huyết của thầy. Chúng em sẽ luôn trân trọng những kiến thức và kinh nghiệm mà thầy đã truyền đạt, xem đó như hành trang quý báu trên con đường học tập và sự nghiệp sau này.

Một lần nữa, nhóm xin gửi lời cảm ơn chân thành và kính chúc thầy thật nhiều sức khỏe, thành công và luôn giữ ngọn lửa nhiệt huyết với nghề giáo. 

