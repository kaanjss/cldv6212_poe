-- =============================================
-- POE PART 3 - SCREENSHOT QUERIES
-- Run these in Azure SQL Database Query Editor
-- =============================================

-- =============================================
-- REQUIREMENT 1: Show all Users (Admin + Customer) with Login Details and Roles
-- Caption: "SQL Database showing 11 users (2 Admins + 9 Customers) with login credentials and roles stored in Azure"
-- =============================================

SELECT 
    UserId AS 'User ID',
    Username AS 'Username (Login)',
    Email AS 'Email',
    FirstName + ' ' + LastName AS 'Full Name',
    Role AS 'User Role',
    CASE WHEN IsActive = 1 THEN 'Active' ELSE 'Inactive' END AS 'Account Status',
    ShippingAddress AS 'Address',
    FORMAT(CreatedDate, 'yyyy-MM-dd') AS 'Account Created',
    FORMAT(LastLoginDate, 'yyyy-MM-dd HH:mm') AS 'Last Login'
FROM Users
ORDER BY 
    CASE WHEN Role = 'Admin' THEN 1 ELSE 2 END,
    UserId;
GO

-- =============================================
-- REQUIREMENT 2: Show ONLY Admin Users
-- Caption: "Admin users stored in Azure SQL Database with login credentials"
-- =============================================

SELECT 
    UserId,
    Username AS 'Admin Username',
    Email,
    FirstName + ' ' + LastName AS 'Full Name',
    Role,
    FORMAT(LastLoginDate, 'yyyy-MM-dd HH:mm:ss') AS 'Last Login'
FROM Users
WHERE Role = 'Admin';
GO

-- =============================================
-- REQUIREMENT 3: Show ONLY Customer Users
-- Caption: "Customer users stored in Azure SQL Database (9 customers with login accounts)"
-- =============================================

SELECT 
    UserId,
    Username AS 'Customer Username',
    Email,
    FirstName + ' ' + LastName AS 'Full Name',
    Role,
    ShippingAddress,
    FORMAT(LastLoginDate, 'yyyy-MM-dd HH:mm:ss') AS 'Last Login'
FROM Users
WHERE Role = 'Customer'
ORDER BY UserId;
GO

-- =============================================
-- REQUIREMENT 4: User Count Summary
-- Caption: "Summary showing 2 Admin users and 9 Customer users totaling 11 accounts in Azure SQL Database"
-- =============================================

SELECT 
    Role,
    COUNT(*) AS 'Number of Users',
    STRING_AGG(Username, ', ') AS 'Usernames'
FROM Users
GROUP BY Role
UNION ALL
SELECT 
    'TOTAL' AS Role,
    COUNT(*) AS 'Number of Users',
    CAST(COUNT(*) AS NVARCHAR(10)) + ' Total Users' AS 'Usernames'
FROM Users;
GO

-- =============================================
-- REQUIREMENT 5: Show Shopping Cart Data
-- Caption: "Active shopping cart items stored in Azure SQL Database showing products added by customers"
-- =============================================

SELECT 
    c.CartId AS 'Cart ID',
    u.Username AS 'Customer',
    u.FirstName + ' ' + u.LastName AS 'Customer Name',
    c.ProductId AS 'Product ID (Azure Storage)',
    c.ProductName AS 'Product',
    c.Quantity,
    FORMAT(c.UnitPrice, 'C', 'en-US') AS 'Unit Price',
    FORMAT(c.TotalPrice, 'C', 'en-US') AS 'Total Price',
    FORMAT(c.DateAdded, 'yyyy-MM-dd HH:mm') AS 'Added to Cart'
FROM Cart c
INNER JOIN Users u ON c.UserId = u.UserId
ORDER BY c.DateAdded DESC;
GO

-- =============================================
-- REQUIREMENT 6: Show All Orders (Admin View)
-- Caption: "All customer orders visible to admin in Azure SQL Database with order details and status"
-- =============================================

SELECT 
    o.OrderId AS 'Order ID',
    u.Username AS 'Customer Username',
    u.FirstName + ' ' + u.LastName AS 'Customer Name',
    o.ProductName AS 'Product',
    o.Quantity,
    FORMAT(o.UnitPrice, 'C', 'en-US') AS 'Unit Price',
    FORMAT(o.TotalPrice, 'C', 'en-US') AS 'Total',
    o.Status AS 'Order Status',
    FORMAT(o.OrderDate, 'yyyy-MM-dd HH:mm') AS 'Order Date',
    o.ShippingAddress
FROM Orders o
INNER JOIN Users u ON o.UserId = u.UserId
ORDER BY o.OrderDate DESC;
GO

-- =============================================
-- REQUIREMENT 7: Orders by Status (Admin Management)
-- Caption: "Order status breakdown showing Submitted, Processing, Completed, and Cancelled orders"
-- =============================================

SELECT 
    Status AS 'Order Status',
    COUNT(*) AS 'Number of Orders',
    FORMAT(SUM(TotalPrice), 'C', 'en-US') AS 'Total Revenue',
    STRING_AGG(CAST(OrderId AS NVARCHAR(10)), ', ') AS 'Order IDs'
FROM Orders
GROUP BY Status
ORDER BY 
    CASE Status
        WHEN 'Submitted' THEN 1
        WHEN 'Processing' THEN 2
        WHEN 'Completed' THEN 3
        WHEN 'Cancelled' THEN 4
    END;
GO

-- =============================================
-- REQUIREMENT 8: Orders by Customer (Show at least 5 different customers)
-- Caption: "Orders grouped by customer showing at least 5 different customers have placed orders"
-- =============================================

SELECT 
    u.Username AS 'Customer',
    u.FirstName + ' ' + u.LastName AS 'Customer Name',
    COUNT(o.OrderId) AS 'Total Orders',
    FORMAT(SUM(o.TotalPrice), 'C', 'en-US') AS 'Total Spent',
    STRING_AGG(o.ProductName, ', ') AS 'Products Ordered',
    MAX(FORMAT(o.OrderDate, 'yyyy-MM-dd')) AS 'Last Order Date'
FROM Orders o
INNER JOIN Users u ON o.UserId = u.UserId
GROUP BY u.Username, u.FirstName, u.LastName
ORDER BY COUNT(o.OrderId) DESC;
GO

-- =============================================
-- REQUIREMENT 9: Customer-Specific Order View
-- Caption: "Individual customer can view only their own orders (privacy maintained)"
-- =============================================

-- Example for customer 'johndoe'
SELECT 
    o.OrderId AS 'My Order ID',
    o.ProductName AS 'Product',
    o.Quantity,
    FORMAT(o.UnitPrice, 'C', 'en-US') AS 'Unit Price',
    FORMAT(o.TotalPrice, 'C', 'en-US') AS 'Total',
    o.Status AS 'Status',
    FORMAT(o.OrderDate, 'yyyy-MM-dd HH:mm') AS 'Order Date'
FROM Orders o
INNER JOIN Users u ON o.UserId = u.UserId
WHERE u.Username = 'johndoe'  -- Change this to test different customers
ORDER BY o.OrderDate DESC;
GO

-- =============================================
-- OPTIONAL: Create Test Data if Needed
-- Only run this if you need more orders for screenshots
-- =============================================

/*
-- Insert test orders from different customers (ONLY IF NEEDED)
DECLARE @UserId1 INT = (SELECT UserId FROM Users WHERE Username = 'johndoe');
DECLARE @UserId2 INT = (SELECT UserId FROM Users WHERE Username = 'janesmith');
DECLARE @UserId3 INT = (SELECT UserId FROM Users WHERE Username = 'mamacita');
DECLARE @UserId4 INT = (SELECT UserId FROM Users WHERE Username = 'halal');
DECLARE @UserId5 INT = (SELECT UserId FROM Users WHERE Username = 'kandyman');

-- Order 1: johndoe
IF @UserId1 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Orders WHERE UserId = @UserId1)
BEGIN
    INSERT INTO Orders (UserId, ProductId, ProductName, Quantity, UnitPrice, TotalPrice, Status, ShippingAddress, OrderDate)
    VALUES (@UserId1, 'prod-guid-1', 'Laptop HP ProBook', 1, 15999.99, 15999.99, 'Submitted', 
            (SELECT ShippingAddress FROM Users WHERE UserId = @UserId1), GETUTCDATE());
END

-- Order 2: janesmith
IF @UserId2 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Orders WHERE UserId = @UserId2)
BEGIN
    INSERT INTO Orders (UserId, ProductId, ProductName, Quantity, UnitPrice, TotalPrice, Status, ShippingAddress, OrderDate)
    VALUES (@UserId2, 'prod-guid-2', 'Samsung Galaxy S24', 2, 18999.99, 37999.98, 'Processing', 
            (SELECT ShippingAddress FROM Users WHERE UserId = @UserId2), DATEADD(HOUR, -2, GETUTCDATE()));
END

-- Order 3: mamacita
IF @UserId3 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Orders WHERE UserId = @UserId3)
BEGIN
    INSERT INTO Orders (UserId, ProductId, ProductName, Quantity, UnitPrice, TotalPrice, Status, ShippingAddress, OrderDate)
    VALUES (@UserId3, 'prod-guid-3', 'Sony Headphones', 1, 7999.99, 7999.99, 'Completed', 
            (SELECT ShippingAddress FROM Users WHERE UserId = @UserId3), DATEADD(DAY, -1, GETUTCDATE()));
END

-- Order 4: halal
IF @UserId4 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Orders WHERE UserId = @UserId4)
BEGIN
    INSERT INTO Orders (UserId, ProductId, ProductName, Quantity, UnitPrice, TotalPrice, Status, ShippingAddress, OrderDate)
    VALUES (@UserId4, 'prod-guid-4', 'Dell Monitor 27"', 1, 8999.99, 8999.99, 'Processing', 
            (SELECT ShippingAddress FROM Users WHERE UserId = @UserId4), DATEADD(HOUR, -5, GETUTCDATE()));
END

-- Order 5: kandyman
IF @UserId5 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Orders WHERE UserId = @UserId5)
BEGIN
    INSERT INTO Orders (UserId, ProductId, ProductName, Quantity, UnitPrice, TotalPrice, Status, ShippingAddress, OrderDate)
    VALUES (@UserId5, 'prod-guid-5', 'Logitech Mouse', 2, 1499.99, 2999.98, 'Submitted', 
            (SELECT ShippingAddress FROM Users WHERE UserId = @UserId5), DATEADD(HOUR, -1, GETUTCDATE()));
END

PRINT 'Test orders created successfully!';
*/

-- =============================================
-- VERIFICATION: Check Data Completeness
-- Caption: "Database verification showing all required data is present"
-- =============================================

SELECT 'Database Table' AS 'Table', 'Record Count' AS 'Metric', 'Status' AS 'Result'
UNION ALL
SELECT 'Users', CAST(COUNT(*) AS NVARCHAR(10)), 
    CASE WHEN COUNT(*) >= 5 THEN '✓ PASS (At least 5 users)' ELSE '✗ FAIL (Need 5+ users)' END
FROM Users
UNION ALL
SELECT 'Admin Users', CAST(COUNT(*) AS NVARCHAR(10)),
    CASE WHEN COUNT(*) >= 1 THEN '✓ PASS' ELSE '✗ FAIL' END
FROM Users WHERE Role = 'Admin'
UNION ALL
SELECT 'Customer Users', CAST(COUNT(*) AS NVARCHAR(10)),
    CASE WHEN COUNT(*) >= 4 THEN '✓ PASS' ELSE '✗ FAIL' END
FROM Users WHERE Role = 'Customer'
UNION ALL
SELECT 'Cart Items', CAST(COUNT(*) AS NVARCHAR(10)),
    CASE WHEN COUNT(*) >= 1 THEN '✓ PASS (Cart system working)' ELSE 'ℹ INFO (No cart items yet)' END
FROM Cart
UNION ALL
SELECT 'Orders', CAST(COUNT(*) AS NVARCHAR(10)),
    CASE WHEN COUNT(*) >= 5 THEN '✓ PASS (At least 5 orders)' ELSE '⚠ WARNING (Need 5+ orders from different customers)' END
FROM Orders
UNION ALL
SELECT 'Different Customers with Orders', CAST(COUNT(DISTINCT UserId) AS NVARCHAR(10)),
    CASE WHEN COUNT(DISTINCT UserId) >= 5 THEN '✓ PASS' ELSE '⚠ WARNING (Need orders from 5+ customers)' END
FROM Orders;
GO

PRINT '';
PRINT '=============================================';
PRINT 'SCREENSHOT QUERIES COMPLETED!';
PRINT '=============================================';
PRINT 'Use the results above for your POE screenshots.';
PRINT 'Make sure to add descriptive captions to each screenshot.';
PRINT '=============================================';

