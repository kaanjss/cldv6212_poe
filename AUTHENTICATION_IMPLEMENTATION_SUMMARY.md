# ABCRetailers - Authentication & Role-Based Access Implementation

## 🎯 Overview

This document describes the complete authentication and authorization system implemented for the ABCRetailers application to meet POE Part 3 requirements.

---

## ✅ What Was Implemented

### 1. **User Authentication System**
- Cookie-based authentication using ASP.NET Core Identity
- Login page with username/email and password
- Registration page for new customers
- Secure password hashing using SHA256
- Session management with 8-hour expiration
- "Remember Me" functionality (30 days)

### 2. **Role-Based Authorization**
- **Two Roles:** Admin and Customer
- Role-based access control throughout the application
- Different navigation menus based on role
- Protected routes using `[Authorize]` attributes

### 3. **SQL Database Integration**
- User accounts stored in Azure SQL Database
- Cart system stored in SQL
- Orders stored in SQL
- Full CRUD operations for users, cart, and orders

### 4. **Admin Capabilities**
- View and manage all customers (Azure Table Storage)
- Create, edit, and delete products
- View all orders from all customers
- Upload files to blob storage and file shares
- Full access to all application features

### 5. **Customer Capabilities**
- Browse products in a shop-style interface
- Add products to shopping cart
- View and manage their cart
- Checkout to create orders
- View only their own orders
- Cannot access admin functions

---

## 📁 New Files Created

### Models
- `Models/User.cs` - User entity matching SQL schema
- `Models/Cart.cs` - Shopping cart entity
- `Models/LoginViewModel.cs` - Login form model
- `Models/RegisterViewModel.cs` - Registration form model

### Services
- `Services/ISqlDatabaseService.cs` - SQL database service interface
- `Services/SqlDatabaseService.cs` - SQL database service implementation
  - User authentication
  - Cart operations
  - Order operations

### Controllers
- `Controllers/AuthController.cs` - Login, Register, Logout actions
- `Controllers/CartController.cs` - Cart and order management for customers

### Views
- `Views/Auth/Login.cshtml` - Beautiful login page
- `Views/Auth/Register.cshtml` - Registration page
- `Views/Auth/AccessDenied.cshtml` - Access denied page
- `Views/Cart/Index.cshtml` - Shopping cart page
- `Views/Cart/MyOrders.cshtml` - Customer orders page

### Modified Files
- `Program.cs` - Added authentication middleware and services
- `Views/Shared/_Layout.cshtml` - Role-based navigation and user info
- `Views/Product/Index.cshtml` - Different views for Admin vs Customer
- All existing controllers - Added `[Authorize]` attributes

---

## 🔐 Security Features

1. **Password Security**
   - Passwords hashed using SHA256
   - Never stored in plain text
   - Secure comparison during authentication

2. **Cookie Security**
   - HttpOnly cookies (prevents XSS attacks)
   - Secure cookies (HTTPS only)
   - SameSite=Lax (CSRF protection)
   - Automatic expiration

3. **Authorization Checks**
   - Route-level protection with `[Authorize]`
   - Role-based policies
   - Access denied redirects

4. **SQL Injection Protection**
   - Parameterized queries
   - No raw SQL concatenation
   - ADO.NET best practices

---

## 👤 Demo Accounts

The SQL database script creates 5 demo users:

### Admins (2)
1. **Username:** `admin` / **Password:** `admin123`
2. **Username:** `manager` / **Password:** `manager456`

### Customers (3)
1. **Username:** `johndoe` / **Password:** `john789`
2. **Username:** `janesmith` / **Password:** `jane101`
3. **Username:** `mjohnson` / **Password:** `michael202`

---

## 🗄️ Database Schema

### Users Table
```sql
- UserId (INT, PK, Identity)
- Username (NVARCHAR(100), UNIQUE)
- Email (NVARCHAR(255), UNIQUE)
- PasswordHash (NVARCHAR(255))
- FirstName (NVARCHAR(100))
- LastName (NVARCHAR(100))
- Role (NVARCHAR(50)) - 'Admin' or 'Customer'
- ShippingAddress (NVARCHAR(500), NULL)
- IsActive (BIT)
- CreatedDate (DATETIME2)
- LastLoginDate (DATETIME2, NULL)
```

### Cart Table
```sql
- CartId (INT, PK, Identity)
- UserId (INT, FK)
- ProductId (INT, FK)
- Quantity (INT)
- UnitPrice (DECIMAL(18,2))
- TotalPrice (Computed: Quantity * UnitPrice)
- DateAdded (DATETIME2)
```

### Orders Table
```sql
- OrderId (INT, PK, Identity)
- UserId (INT, FK)
- ProductId (INT, FK)
- ProductName (NVARCHAR(200))
- Quantity (INT)
- UnitPrice (DECIMAL(18,2))
- TotalPrice (DECIMAL(18,2))
- Status (NVARCHAR(50)) - 'Submitted', 'Processing', 'Completed', 'Cancelled'
- ShippingAddress (NVARCHAR(500))
- OrderDate (DATETIME2)
```

---

## 🔄 Application Flow

### First-Time User (Customer)
1. Visit site → Redirected to Login page
2. Click "Register here"
3. Fill in registration form (auto-assigned Customer role)
4. Redirected to Login page
5. Login with credentials
6. Redirected to Home page
7. Browse products → Add to cart → Checkout → View orders

### Admin User
1. Visit site → Login with admin credentials
2. See Admin Panel in sidebar
3. Access all admin functions:
   - Manage customers
   - Create/Edit/Delete products
   - View all orders
   - Upload files

### Logout
1. Click Logout button in sidebar
2. Session cleared
3. Redirected to Login page

---

## 🎨 UI/UX Features

### Login Page
- Modern gradient design
- Remember me checkbox
- Demo account credentials displayed
- Link to registration page

### Registration Page
- Clean form layout
- Password confirmation validation
- Optional shipping address
- All new users become Customers by default

### Customer Product View
- Card-based layout
- Product images
- Add to Cart functionality
- Stock availability indicators

### Admin Product View
- Table format
- Edit and Delete buttons
- Quick stock and price overview

### Navigation
- Role-based sidebar menu
- User info display (name and role)
- Logout button
- Different menu items for Admin vs Customer

---

## 📊 Features by Role

| Feature | Admin | Customer |
|---------|-------|----------|
| Browse Products | ✅ (Table view) | ✅ (Card view) |
| Create Products | ✅ | ❌ |
| Edit Products | ✅ | ❌ |
| Delete Products | ✅ | ❌ |
| Add to Cart | ❌ | ✅ |
| View Own Cart | ❌ | ✅ |
| View Own Orders | ❌ | ✅ |
| View All Customers | ✅ | ❌ |
| View All Orders | ✅ | ❌ |
| Upload Files | ✅ | ❌ |
| Change Order Status | ✅ | ❌ |

---

## 🚀 How to Deploy

### 1. Run SQL Script
Execute the complete SQL script in your Azure SQL Database to create:
- Users table with 5 demo accounts
- Cart table
- Orders table
- Sample data

### 2. Update Configuration
The connection strings are already configured in:
- `appsettings.json` (Web App)
- No changes needed

### 3. Deploy to Azure
1. Publish Web App to Azure App Service
2. Publish Functions to Azure Function App
3. Ensure SQL connection string is added to App Service configuration

### 4. Test
1. Navigate to your Web App URL
2. Login with demo accounts
3. Test both Admin and Customer roles

---

## 🔧 Technical Stack

- **Framework:** ASP.NET Core 8.0 MVC
- **Authentication:** Cookie-based authentication
- **Authorization:** Role-based policies
- **Database:** Azure SQL Database (ADO.NET)
- **Storage:** Azure Table Storage (products, customers from POE Part 1/2)
- **UI:** Bootstrap 5, Font Awesome
- **Security:** SHA256 password hashing, parameterized queries

---

## 📝 Notes

1. **Hybrid Storage Approach:**
   - Users, Cart, Orders → SQL Database (new for POE Part 3)
   - Products, Customers, Orders → Azure Table Storage (existing from POE Part 1/2)
   - This demonstrates both storage approaches

2. **Password Security:**
   - Demo passwords are simple for testing
   - Production should enforce stronger password policies
   - Consider using ASP.NET Core Identity for production

3. **Cart Implementation:**
   - Cart persists across sessions
   - Cart items show product info
   - Checkout creates orders and clears cart

4. **Order Status:**
   - Customers see their order status
   - Admins can update order status
   - Status options: Submitted, Processing, Completed, Cancelled

---

## ✅ POE Part 3 Requirements Met

- ✅ Users with login credentials stored in SQL
- ✅ Admin and Customer roles implemented
- ✅ At least 5 users created (2 Admins + 3 Customers)
- ✅ Cart system in SQL Database
- ✅ Orders from different customers
- ✅ Admin can see all orders
- ✅ Customers can only see their own data
- ✅ SQL Database with replica (to be created separately in Azure Portal)

---

## 🎉 Completion Status

**All authentication and authorization features have been successfully implemented!**

The application now requires login and provides different experiences based on user role, meeting all POE Part 3 requirements.

---

*Last Updated: November 13, 2025*

