-- ABCRetailers Database Setup Script
-- This script creates tables to store Customer, Product, and Order information
-- Run this in Azure SQL Database Query Editor

-- Drop tables if they exist (for clean setup)
IF OBJECT_ID('Orders', 'U') IS NOT NULL DROP TABLE Orders;
IF OBJECT_ID('Products', 'U') IS NOT NULL DROP TABLE Products;
IF OBJECT_ID('Customers', 'U') IS NOT NULL DROP TABLE Customers;
GO

-- =============================================
-- Customers Table
-- =============================================
CREATE TABLE Customers (
    CustomerId NVARCHAR(50) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Surname NVARCHAR(100) NOT NULL,
    Username NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) NOT NULL,
    ShippingAddress NVARCHAR(500) NOT NULL,
    CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
    CONSTRAINT UQ_Customer_Email UNIQUE (Email),
    CONSTRAINT UQ_Customer_Username UNIQUE (Username)
);
GO

-- =============================================
-- Products Table
-- =============================================
CREATE TABLE Products (
    ProductId NVARCHAR(50) PRIMARY KEY,
    ProductName NVARCHAR(200) NOT NULL,
    Description NVARCHAR(1000) NOT NULL,
    Price DECIMAL(18, 2) NOT NULL CHECK (Price > 0),
    StockAvailable INT NOT NULL DEFAULT 0 CHECK (StockAvailable >= 0),
    ImageUrl NVARCHAR(500) NULL,
    CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
    LastModified DATETIME2 DEFAULT GETUTCDATE()
);
GO

-- =============================================
-- Orders Table
-- =============================================
CREATE TABLE Orders (
    OrderId NVARCHAR(50) PRIMARY KEY,
    CustomerId NVARCHAR(50) NOT NULL,
    Username NVARCHAR(100) NOT NULL,
    ProductId NVARCHAR(50) NOT NULL,
    ProductName NVARCHAR(200) NOT NULL,
    OrderDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    Quantity INT NOT NULL CHECK (Quantity > 0),
    UnitPrice DECIMAL(18, 2) NOT NULL CHECK (UnitPrice > 0),
    TotalPrice DECIMAL(18, 2) NOT NULL CHECK (TotalPrice > 0),
    Status NVARCHAR(50) NOT NULL DEFAULT 'Submitted',
    CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
    CONSTRAINT FK_Orders_Customers FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerId) ON DELETE CASCADE,
    CONSTRAINT FK_Orders_Products FOREIGN KEY (ProductId) REFERENCES Products(ProductId),
    CONSTRAINT CHK_Order_Status CHECK (Status IN ('Submitted', 'Processing', 'Completed', 'Cancelled'))
);
GO

-- =============================================
-- Indexes for Performance
-- =============================================
CREATE INDEX IX_Orders_CustomerId ON Orders(CustomerId);
CREATE INDEX IX_Orders_ProductId ON Orders(ProductId);
CREATE INDEX IX_Orders_OrderDate ON Orders(OrderDate DESC);
CREATE INDEX IX_Orders_Status ON Orders(Status);
CREATE INDEX IX_Products_Name ON Products(ProductName);
CREATE INDEX IX_Customers_Email ON Customers(Email);
GO

-- =============================================
-- Insert Sample Data (Optional - for testing)
-- =============================================

-- Sample Customers
INSERT INTO Customers (CustomerId, Name, Surname, Username, Email, ShippingAddress) VALUES
('cust-001', 'John', 'Doe', 'johndoe', 'john.doe@example.com', '123 Main St, Johannesburg, 2000'),
('cust-002', 'Jane', 'Smith', 'janesmith', 'jane.smith@example.com', '456 Oak Ave, Cape Town, 8001'),
('cust-003', 'Michael', 'Johnson', 'mjohnson', 'michael.j@example.com', '789 Pine Rd, Durban, 4001');
GO

-- Sample Products
INSERT INTO Products (ProductId, ProductName, Description, Price, StockAvailable, ImageUrl) VALUES
('prod-001', 'Laptop HP ProBook 450', 'High-performance business laptop with Intel i7 processor', 15999.99, 25, 'https://example.com/laptop.jpg'),
('prod-002', 'Samsung Galaxy S24', 'Latest flagship smartphone with advanced camera', 18999.99, 50, 'https://example.com/phone.jpg'),
('prod-003', 'Sony WH-1000XM5 Headphones', 'Premium noise-canceling wireless headphones', 7999.99, 40, 'https://example.com/headphones.jpg'),
('prod-004', 'Dell UltraSharp Monitor 27"', '4K UHD monitor for professionals', 8999.99, 15, 'https://example.com/monitor.jpg'),
('prod-005', 'Logitech MX Master 3S Mouse', 'Ergonomic wireless mouse for productivity', 1499.99, 100, 'https://example.com/mouse.jpg');
GO

-- Sample Orders
INSERT INTO Orders (OrderId, CustomerId, Username, ProductId, ProductName, OrderDate, Quantity, UnitPrice, TotalPrice, Status) VALUES
('order-001', 'cust-001', 'johndoe', 'prod-001', 'Laptop HP ProBook 450', GETUTCDATE(), 1, 15999.99, 15999.99, 'Submitted'),
('order-002', 'cust-002', 'janesmith', 'prod-002', 'Samsung Galaxy S24', GETUTCDATE(), 2, 18999.99, 37999.98, 'Processing'),
('order-003', 'cust-003', 'mjohnson', 'prod-003', 'Sony WH-1000XM5 Headphones', GETUTCDATE(), 1, 7999.99, 7999.99, 'Completed');
GO

-- =============================================
-- Verification Queries
-- =============================================
SELECT 'Customers Count' AS TableName, COUNT(*) AS RecordCount FROM Customers
UNION ALL
SELECT 'Products Count', COUNT(*) FROM Products
UNION ALL
SELECT 'Orders Count', COUNT(*) FROM Orders;
GO

SELECT * FROM Customers;
SELECT * FROM Products;
SELECT * FROM Orders;
GO

PRINT 'Database setup completed successfully!';

