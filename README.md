# ABC Retailers

An e-commerce web application built with ASP.NET Core MVC and Azure services.

## 🚀 Features

- Customer Management
- Product Catalog
- Shopping Cart
- Order Processing
- File Upload & Management
- User Authentication & Authorization
- Azure Functions Integration

## 🛠️ Tech Stack

- **Framework:** ASP.NET Core 8.0 (MVC)
- **Cloud Services:** Azure Storage (Blob, Queue, Table), Azure SQL Database
- **Serverless:** Azure Functions
- **Frontend:** Bootstrap, jQuery

## 📋 Prerequisites

- .NET 8.0 SDK
- Azure Account
- SQL Server

## ⚙️ Setup

1. Clone the repository
```bash
git clone <repository-url>
cd st10442156_cldv6212_p3
```

2. Configure Azure Services
   - Copy `appsettings.example.json` to `appsettings.json`
   - Add your Azure connection strings and SQL credentials

3. Setup Database
   - Run `DatabaseSetup.sql` on your SQL Server

4. Run the application
```bash
dotnet restore
dotnet run
```

5. For Azure Functions
```bash
cd ABCRetailers.Functions
dotnet run
```

## 📁 Project Structure

```
├── Controllers/          # MVC Controllers
├── Models/              # Data models
├── Views/               # Razor views
├── Services/            # Business logic
├── ABCRetailers.Functions/  # Azure Functions
└── wwwroot/             # Static files
```

## 🔑 Test Credentials

See `TEST_CREDENTIALS.txt` for test user accounts.

## 📝 License

This is a student project for CLDV6212.

## 👤 Author

Student ID: ST10442156

