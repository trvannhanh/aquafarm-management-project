<h1 align="center">
  <br>
  <a href="https://github.com/trvannhanh/AquaFarm-Management">
    <img src="https://img.icons8.com/external-flat-juicy-fish/60/000000/external-fish-aquatic-animals-flat-flat-juicy-fish.png" alt="AquaFarm" width="100">
  </a>
  <br>
  AquaFarm Management
  <br>
</h1>

<h4 align="center">
  Ứng dụng web quản lý trang trại thủy sản, hỗ trợ quản lý ao nuôi, nhật ký cho ăn, theo dõi môi trường, tiêu thụ và báo cáo thống kê.
</h4>

<p align="center">
  <a href="#key-features">Tính năng</a> •
  <a href="#tech-stack">Công nghệ</a> •
  <a href="#installation">Cài đặt</a> •
  <a href="#usage">Cách chạy</a> •
  <a href="#credits">Credits</a>
</p>

---

## 🌊 Key Features

- 🐟 **Quản lý trang trại & ao nuôi**: thông tin trang trại, ao nuôi, loại thủy sản.  
- 🧬 **Tạo & phân bổ lứa nuôi**: quản lý vòng đời nuôi trồng.  
- 🍽 **Nhật ký cho ăn**: ghi nhận loại thức ăn, số lượng, chi phí, lịch cho ăn.  
- 🌡 **Theo dõi chỉ số môi trường**: nhiệt độ, pH, oxy, độ mặn...  
- ❤️ **Theo dõi sức khỏe thủy sản**: ghi nhận tình trạng và bệnh dịch.  
- 💰 **Tiêu thụ sản phẩm**: quản lý thu hoạch, bán hàng, tính doanh thu và lợi nhuận.  
- 📊 **Báo cáo thống kê**: biến động môi trường, sản lượng, chi phí, doanh thu.  

---

## 🛠 Tech Stack

- **Backend**: ASP.NET Core MVC, Entity Framework (Database First)  
- **Frontend**: Razor Pages, Bootstrap 5, JavaScript  
- **Database**: SQL Server  
- **IDE**: Visual Studio 2022  

---

## ⚙️ Installation

Trước khi chạy dự án, hãy cài đặt:

- [.NET SDK](https://dotnet.microsoft.com/download) >= 8.0  
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)  
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (ASP.NET & Web workload)  

---

## 🚀 Usage

```bash
# Clone repository
$ git clone https://github.com/trvannhanh/AquaFarm-Management

# Mở solution bằng Visual Studio
# Cấu hình chuỗi kết nối SQL Server trong appsettings.json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=AquaFarmDB;Trusted_Connection=True;"
}

# Khởi tạo database (nếu dùng migration)
$ dotnet ef database update

# Run project
F5 hoặc dotnet run
