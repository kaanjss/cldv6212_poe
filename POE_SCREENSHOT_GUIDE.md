# POE PART 3 - SCREENSHOT GUIDE
Complete checklist for taking all required screenshots for your submission.

---

## 📋 CHECKLIST

### ✅ REQUIREMENT 1: SQL Database Creation on Azure
**What to capture:**
- [ ] Azure Portal showing SQL Database overview
- [ ] Database connection settings page
- [ ] Query Editor interface

**How to do it:**
1. Go to Azure Portal → SQL Databases → Your database (`cldv`)
2. Take screenshot of the Overview page
3. Click on "Query Editor" in left menu
4. Take screenshot showing Query Editor interface

**Caption:** "SQL Database 'cldv' successfully created and hosted on Azure with Query Editor access"

---

### ✅ REQUIREMENT 2: User Information with Login Details and Roles (At least 5 users)
**What to capture:**
- [ ] Query results showing all 11 users (2 admins + 9 customers)
- [ ] Admin users only
- [ ] Customer users only

**How to do it:**
1. Open Azure SQL Database Query Editor
2. Paste and run **Query 1** from `SCREENSHOT_QUERIES.sql`
3. Take screenshot of the results table
4. Run **Query 2** (Admin users only)
5. Take screenshot
6. Run **Query 3** (Customer users only)
7. Take screenshot

**Captions:**
- Screenshot 1: "Azure SQL Database showing 11 users (2 Admins + 9 Customers) with usernames, emails, roles, and login credentials"
- Screenshot 2: "Admin users stored in SQL Database with login credentials (admin, manager)"
- Screenshot 3: "Customer users in SQL Database - 9 customers with active login accounts"

---

### ✅ REQUIREMENT 3: Successful Login as Customer
**What to capture:**
- [ ] Login page
- [ ] Customer credentials entered
- [ ] Customer dashboard after login
- [ ] Customer sidebar showing role-specific menu

**How to do it:**
1. Open `http://localhost:5236` in browser
2. Take screenshot of login page
3. Enter customer credentials: `johndoe` / `john789`
4. Take screenshot before clicking Login
5. Click Login
6. Take screenshot of customer dashboard showing welcome message
7. Take screenshot of sidebar showing "Shop Products, My Cart, My Orders"

**Captions:**
- Screenshot 1: "Login page - entry point for all users (Customer and Admin)"
- Screenshot 2: "Customer login credentials entered (Username: johndoe)"
- Screenshot 3: "Customer dashboard after successful login showing personalized welcome and shopping options"
- Screenshot 4: "Customer sidebar menu showing Shop Products, My Cart, and My Orders (role-based access)"

---

### ✅ REQUIREMENT 4: Successful Login as Admin
**What to capture:**
- [ ] Admin credentials entered
- [ ] Admin dashboard after login
- [ ] Admin sidebar showing management options
- [ ] Statistics/overview cards

**How to do it:**
1. Logout from customer account
2. Login with: `admin` / `admin123`
3. Take screenshot of credentials entered
4. Take screenshot of admin dashboard showing stats (Customers, Products, Orders)
5. Take screenshot of sidebar showing admin menu (Customers, Products, All Orders, Upload Files)

**Captions:**
- Screenshot 1: "Admin login credentials entered (Username: admin)"
- Screenshot 2: "Admin dashboard showing system statistics - 5 Customers, 6 Products, 27 Orders"
- Screenshot 3: "Admin sidebar menu showing management options (Customers, Products, All Orders, Upload Files)"

---

### ✅ REQUIREMENT 5: Cart System Implementation
**What to capture:**
- [ ] SQL Query showing Cart table structure
- [ ] SQL Query results showing cart items in database
- [ ] Web app showing customer's cart view

**How to do it:**
1. In Azure SQL Query Editor, run **Query 5** from `SCREENSHOT_QUERIES.sql`
2. Take screenshot of Cart data in SQL
3. In web app, login as customer
4. Go to "My Cart"
5. Take screenshot of cart page

**Captions:**
- Screenshot 1: "Cart table in Azure SQL Database storing active cart items with ProductId, Quantity, and Prices"
- Screenshot 2: "Customer cart view in web application showing products added to cart with quantities and totals"

---

### ✅ REQUIREMENT 6: Customer Adding Products to Cart
**What to capture:**
- [ ] Product listing page
- [ ] "Add to Cart" button visible
- [ ] Success message after adding
- [ ] Cart updated with item

**How to do it:**
1. Login as customer (`mamacita` / `password`)
2. Click "Shop Products"
3. Take screenshot showing product cards with "Add to Cart" buttons
4. Click "Add to Cart" on a product
5. Take screenshot of success message
6. Go to "My Cart"
7. Take screenshot showing the added item

**Captions:**
- Screenshot 1: "Products page showing available products with 'Add to Cart' buttons for customer"
- Screenshot 2: "Success message after customer adds product to cart"
- Screenshot 3: "Cart updated showing newly added product with quantity and price details"

---

### ✅ REQUIREMENT 7: Admin Viewing All Orders
**What to capture:**
- [ ] SQL Query showing orders from different customers
- [ ] Web app showing admin "All Orders" page
- [ ] At least 5 orders from different customers visible

**How to do it:**
1. In Azure SQL Query Editor, run **Query 6** from `SCREENSHOT_QUERIES.sql`
2. Take screenshot showing orders from multiple customers
3. Run **Query 8** to show orders by customer
4. Take screenshot
5. In web app, login as admin
6. Click "All Orders"
7. Take screenshot of orders page

**Captions:**
- Screenshot 1: "SQL Database showing all orders with customer information - orders from johndoe, janesmith, mamacita, halal, kandyman (5+ different customers)"
- Screenshot 2: "Orders grouped by customer showing at least 5 different customers have placed orders"
- Screenshot 3: "Admin view of all orders in web application with order details and status"

---

### ✅ REQUIREMENT 8: Admin Changing Order Status
**What to capture:**
- [ ] Order with "Submitted" status
- [ ] Status dropdown or change option
- [ ] Order status changed to "Processing"
- [ ] SQL Query showing status change
- [ ] Order status changed to "Completed"

**How to do it:**
1. In web app as admin, go to "All Orders"
2. Take screenshot showing an order with "Submitted" status
3. Change status to "Processing"
4. Take screenshot of updated status
5. Change another order to "Completed"
6. Take screenshot
7. In Azure SQL, run **Query 7** from `SCREENSHOT_QUERIES.sql`
8. Take screenshot showing orders in different statuses

**Captions:**
- Screenshot 1: "Order with 'Submitted' status before admin processes it"
- Screenshot 2: "Admin changing order status to 'Processing'"
- Screenshot 3: "Order status updated to 'Completed' showing successful order processing"
- Screenshot 4: "SQL Database showing orders in different statuses (Submitted, Processing, Completed, Cancelled)"

---

### ✅ REQUIREMENT 9: Azure App Service Deployment
**What to capture:**
- [ ] Visual Studio publish dialog
- [ ] Azure App Service creation
- [ ] Deployment progress
- [ ] Deployment success message
- [ ] Deployed app URL in browser
- [ ] App Service in Azure Portal

**How to do it:**
1. In Visual Studio on VM, right-click project → Publish
2. Take screenshot of Publish dialog
3. Select Azure → Azure App Service (Windows)
4. Take screenshot of App Service creation
5. Take screenshot during deployment
6. Take screenshot of "Publish Succeeded" message
7. Open the deployed URL in browser
8. Take screenshot of running app
9. Go to Azure Portal → App Services
10. Take screenshot of your App Service

**Captions:**
- Screenshot 1: "Visual Studio Publish dialog showing Azure deployment option"
- Screenshot 2: "Creating new Azure App Service for web application"
- Screenshot 3: "Deployment in progress - publishing to Azure App Service"
- Screenshot 4: "Successful deployment message showing web app published to Azure"
- Screenshot 5: "Deployed web application running on Azure App Service (live URL: https://your-app.azurewebsites.net)"
- Screenshot 6: "Azure Portal showing deployed App Service with configuration and URL"

---

## 🎯 QUICK TEST WORKFLOW

### Before Taking Screenshots:
1. **Create test orders** (if needed):
   - Login as 5 different customers
   - Each customer: browse products → add to cart → checkout
   - This ensures you have 5+ orders from different customers

2. **Test login credentials:**
   ```
   Admin:    admin / admin123
   Admin:    manager / manager456
   Customer: johndoe / john789
   Customer: mamacita / password
   Customer: halal / password
   Customer: kandyman / password
   Customer: lordkandy / password
   ```

3. **Verify data in SQL:**
   - Run the verification query (last query in SCREENSHOT_QUERIES.sql)
   - Ensure all ✓ PASS results

---

## 📊 SQL QUERIES SUMMARY

### Query 1: All Users (11 users: 2 admins + 9 customers)
Shows complete user list with roles and login details

### Query 2: Admin Users Only
Shows 2 admin accounts

### Query 3: Customer Users Only
Shows 9 customer accounts

### Query 4: User Count Summary
Shows breakdown by role

### Query 5: Shopping Cart Data
Shows active cart items in SQL

### Query 6: All Orders (Admin View)
Shows all orders with customer information

### Query 7: Orders by Status
Shows orders grouped by status (Submitted, Processing, Completed, Cancelled)

### Query 8: Orders by Customer
Shows orders grouped by customer (verifies 5+ different customers)

### Query 9: Customer-Specific Orders
Shows orders for a single customer (privacy)

### Optional: Create Test Orders
Creates 5 sample orders if you don't have enough

### Verification Query
Checks if you have all required data

---

## ✅ FINAL CHECKLIST

Before submission, verify you have:
- [ ] 11+ users (2 admins + 9 customers) ✓
- [ ] 5+ orders from different customers ✓
- [ ] Cart system with data in SQL ✓
- [ ] Orders in multiple statuses (Submitted, Processing, Completed) ✓
- [ ] Screenshots of customer login ✓
- [ ] Screenshots of admin login ✓
- [ ] Screenshots of cart functionality ✓
- [ ] Screenshots of admin managing orders ✓
- [ ] Screenshots of Azure deployment ✓
- [ ] All screenshots have descriptive captions ✓

---

## 📝 NOTES

- **All functionality working:** CRUD operations ✓, Login ✓, Cart ✓, Orders ✓
- **Azure integration:** SQL Database ✓, Table Storage ✓, Blob Storage ✓, Functions ✓
- **Role-based access:** Admin vs Customer ✓
- **Security:** Password hashing ✓, Authentication ✓, Authorization ✓

---

## 🎓 SUBMISSION STRUCTURE

```
POE Part 3 Submission
├── 1. Azure SQL Database Creation (2-3 screenshots)
├── 2. User Data with Roles (3-4 screenshots)
├── 3. Customer Login (3-4 screenshots)
├── 4. Admin Login (2-3 screenshots)
├── 5. Cart System (2-3 screenshots)
├── 6. Add to Cart Functionality (3-4 screenshots)
├── 7. Admin View All Orders (3-4 screenshots)
├── 8. Admin Change Order Status (4-5 screenshots)
└── 9. Azure Deployment (6-8 screenshots)
```

**Total Expected Screenshots:** 28-38 screenshots with descriptive captions

---

**Good luck with your submission! 🚀**

