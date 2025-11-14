-- =============================================
-- ABCRetailers COMPLETE Database Setup - FIXED VERSION
-- Includes: Users (with login/roles), Cart, Orders
-- Products remain in Azure Table Storage (POE Part 1/2)
-- Run this in Azure SQL Database Query Editor
-- =============================================

-- Drop tables if they exist (for clean setup)
IF OBJECT_ID('Cart', 'U') IS NOT NULL DROP TABLE Cart;
IF OBJECT_ID('Orders', 'U') IS NOT NULL DROP TABLE Orders;
IF OBJECT_ID('Users', 'U') IS NOT NULL DROP TABLE Users;
GO

-- =============================================
-- Users Table (Login + Roles + Profile Info)
-- Combines authentication and customer information
-- =============================================
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(100) NOT NULL UNIQUE,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Role NVARCHAR(50) NOT NULL CHECK (Role IN ('Customer', 'Admin')),
    ShippingAddress NVARCHAR(500) NULL,
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
    LastLoginDate DATETIME2 NULL,
    CONSTRAINT CHK_User_Role CHECK (Role IN ('Customer', 'Admin'))
);
GO

-- =============================================
-- Cart Table (Shopping Cart)
-- NO FK to Products - Products are in Azure Table Storage
-- =============================================
CREATE TABLE Cart (
    CartId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    ProductId NVARCHAR(100) NOT NULL,  -- Stores Azure Table Storage GUID
    ProductName NVARCHAR(200) NOT NULL,
    ProductImageUrl NVARCHAR(500) NULL,
    Quantity INT NOT NULL CHECK (Quantity > 0),
    UnitPrice DECIMAL(18, 2) NOT NULL CHECK (UnitPrice > 0),
    TotalPrice AS (Quantity * UnitPrice) PERSISTED,
    DateAdded DATETIME2 DEFAULT GETUTCDATE(),
    CONSTRAINT FK_Cart_Users FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
);
GO

-- =============================================
-- Orders Table (Completed Orders from Cart)
-- NO FK to Products - Products are in Azure Table Storage
-- =============================================
CREATE TABLE Orders (
    OrderId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    ProductId NVARCHAR(100) NOT NULL,  -- Stores Azure Table Storage GUID
    ProductName NVARCHAR(200) NOT NULL,
    OrderDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    Quantity INT NOT NULL CHECK (Quantity > 0),
    UnitPrice DECIMAL(18, 2) NOT NULL CHECK (UnitPrice > 0),
    TotalPrice DECIMAL(18, 2) NOT NULL CHECK (TotalPrice > 0),
    Status NVARCHAR(50) NOT NULL DEFAULT 'Submitted',
    ShippingAddress NVARCHAR(500) NOT NULL,
    CONSTRAINT FK_Orders_Users FOREIGN KEY (UserId) REFERENCES Users(UserId),
    CONSTRAINT CHK_Order_Status CHECK (Status IN ('Submitted', 'Processing', 'Completed', 'Cancelled'))
);
GO

-- =============================================
-- Indexes for Performance
-- =============================================
CREATE INDEX IX_Users_Email ON Users(Email);
CREATE INDEX IX_Users_Username ON Users(Username);
CREATE INDEX IX_Users_Role ON Users(Role);
CREATE INDEX IX_Cart_UserId ON Cart(UserId);
CREATE INDEX IX_Cart_ProductId ON Cart(ProductId);
CREATE INDEX IX_Orders_UserId ON Orders(UserId);
CREATE INDEX IX_Orders_ProductId ON Orders(ProductId);
CREATE INDEX IX_Orders_Status ON Orders(Status);
CREATE INDEX IX_Orders_OrderDate ON Orders(OrderDate DESC);
GO

-- =============================================
-- Insert Sample Data
-- =============================================

-- Insert 5 Users (2 Admins + 3 Customers) - MEETS REQUIREMENT
-- Passwords are hashed using SHA256 + Base64 encoding (matches C# HashPassword method)
PRINT 'Inserting Users (5 total: 2 Admins + 3 Customers)...';
INSERT INTO Users (Username, Email, PasswordHash, FirstName, LastName, Role, ShippingAddress, LastLoginDate) VALUES
-- Admins
('admin', 'admin@abcretailers.com', 'JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3THIKk=', 'John', 'Smith', 'Admin', '1 Admin Lane, Johannesburg, 2000', '2025-11-13 08:30:00'),
('manager', 'manager@abcretailers.com', 'RoKsY/Jb9R6S/DtuMDPSnE9szud65aZUPkr/lpiqa0k=', 'Sarah', 'Johnson', 'Admin', '2 Manager Street, Cape Town, 8001', '2025-11-12 14:20:00'),

-- Customers
('johndoe', 'john.doe@example.com', 'jrajLNmGFnUvDEsegLxwnvwzIzzNt0F3bmcnJMHMyAw=', 'John', 'Doe', 'Customer', '123 Main St, Johannesburg, 2000', '2025-11-13 10:15:00'),
('janesmith', 'jane.smith@example.com', 'pvBy6SZSC+Kh8YtqXYRGG2ITAjlmhD1Z9WAHtaGTKvE=', 'Jane', 'Smith', 'Customer', '456 Oak Ave, Cape Town, 8001', '2025-11-11 16:45:00'),
('mjohnson', 'michael.j@example.com', 'n7JuNKk4zTSRxKMEY/OogFwIbYI3fnuJr7o2B3a7fpE=', 'Michael', 'Johnson', 'Customer', '789 Pine Rd, Durban, 4001', '2025-11-10 09:30:00');
GO

-- NOTE: Products are managed in Azure Table Storage (from POE Part 1/2)
-- Orders will be created when customers checkout from cart
-- Cart items will be created when customers add products

PRINT 'Database tables created successfully!';
PRINT '';
PRINT '==============================================';
PRINT 'CREDENTIALS FOR LOGIN TESTING:';
PRINT '==============================================';
PRINT 'ADMIN ACCOUNTS:';
PRINT '  Username: admin     | Password: admin123';
PRINT '  Username: manager   | Password: manager456';
PRINT '';
PRINT 'CUSTOMER ACCOUNTS:';
PRINT '  Username: johndoe   | Password: john789';
PRINT '  Username: janesmith | Password: jane101';
PRINT '  Username: mjohnson  | Password: michael202';
PRINT '==============================================';
PRINT '';
PRINT 'Total Users: 5 (2 Admins + 3 Customers)';
PRINT '';
PRINT 'Next Steps:';
PRINT '1. Admin logs in and creates products (stored in Azure Table Storage)';
PRINT '2. Customer logs in and adds products to cart (stored in SQL Cart table)';
PRINT '3. Customer checks out (creates orders in SQL Orders table)';
PRINT '4. Admin can view and manage all orders';
PRINT '==============================================';
GO

-- =============================================
-- Verification Queries (For Screenshots)
-- =============================================

-- Query 1: All Users with Login Details and Roles
PRINT '';
PRINT 'Query 1: All Users with Login Details and Roles';
SELECT 
    UserId,
    Username,
    Email,
    FirstName + ' ' + LastName AS FullName,
    Role,
    ShippingAddress,
    CASE WHEN IsActive = 1 THEN 'Active' ELSE 'Inactive' END AS AccountStatus,
    FORMAT(CreatedDate, 'yyyy-MM-dd') AS AccountCreated,
    FORMAT(LastLoginDate, 'yyyy-MM-dd HH:mm') AS LastLogin
FROM Users
ORDER BY Role DESC, UserId;
GO

-- Query 2: Admin Users
PRINT '';
PRINT 'Query 2: Admin Users Only';
SELECT 
    UserId,
    Username,
    Email,
    FirstName + ' ' + LastName AS FullName,
    Role,
    FORMAT(LastLoginDate, 'yyyy-MM-dd HH:mm:ss') AS LastLogin
FROM Users
WHERE Role = 'Admin';
GO

-- Query 3: Customer Users
PRINT '';
PRINT 'Query 3: Customer Users Only';
SELECT 
    UserId,
    Username,
    Email,
    FirstName + ' ' + LastName AS FullName,
    Role,
    ShippingAddress,
    FORMAT(LastLoginDate, 'yyyy-MM-dd HH:mm:ss') AS LastLogin
FROM Users
WHERE Role = 'Customer';
GO

-- Query 4: Cart Items (Will show data after customers add to cart)
PRINT '';
PRINT 'Query 4: Cart System (Active Shopping Carts)';
SELECT 
    c.CartId,
    u.Username,
    u.FirstName + ' ' + u.LastName AS CustomerName,
    c.ProductName,
    c.Quantity,
    c.UnitPrice,
    c.TotalPrice,
    FORMAT(c.DateAdded, 'yyyy-MM-dd HH:mm') AS AddedToCart
FROM Cart c
JOIN Users u ON c.UserId = u.UserId
ORDER BY c.DateAdded DESC;
GO

-- Query 5: All Orders (Admin View) - Will show after customers place orders
PRINT '';
PRINT 'Query 5: All Orders (Admin Can See Everything)';
SELECT 
    o.OrderId,
    u.Username,
    u.FirstName + ' ' + u.LastName AS CustomerName,
    o.ProductName,
    o.Quantity,
    o.UnitPrice,
    o.TotalPrice,
    o.Status,
    FORMAT(o.OrderDate, 'yyyy-MM-dd HH:mm') AS OrderDate,
    o.ShippingAddress
FROM Orders o
JOIN Users u ON o.UserId = o.UserId
ORDER BY o.OrderDate DESC;
GO

-- Query 6: Orders by Status
PRINT '';
PRINT 'Query 6: Order Status Summary';
SELECT 
    Status,
    COUNT(*) AS OrderCount,
    SUM(TotalPrice) AS TotalRevenue
FROM Orders
GROUP BY Status
ORDER BY OrderCount DESC;
GO

