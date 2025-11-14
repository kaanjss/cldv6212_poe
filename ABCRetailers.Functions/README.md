# ABCRetailers Azure Functions

This Azure Functions project provides serverless backend services for the ABCRetailers application, implementing all four required Azure storage services as specified in POE Part 2.

## 🎯 POE Part 2 Requirements Implementation

### A. Integrating Functions to Build a Robust Application Architecture

This project integrates four Azure storage functions:

#### 1. **Azure Tables Storage** ✅
- `CustomersFunctions.cs` - CRUD operations for customers
- `ProductsFunctions.cs` - CRUD operations for products  
- `OrdersFunctions.cs` - CRUD operations for orders
- Stores structured data in Azure Tables

#### 2. **Blob Storage** ✅
- `BlobFunctions.cs` - Product image uploads and management
- `UploadsFunctions.cs` - Proof of payment file uploads
- Blob trigger that logs when images are uploaded
- Public access for product images, private for payment proofs

#### 3. **Queue Storage** ✅
- `QueueProcessorFunctions.cs` - Queue message processors
- `OrdersFunctions.cs` - Sends messages to queues on order creation
- Two queues:
  - `order-notifications` - Order confirmation notifications
  - `stock-updates` - Inventory update messages

#### 4. **Azure Files** ✅
- `UploadsFunctions.cs` - Contract file management
- Stores payment metadata in Azure File Share
- Organizes files by contract type in directories

### B. Services for Improving Customer Experience

The functions include queue processors that enable:

- **Event-driven architecture** - Automatic processing when orders are placed
- **Scalable notifications** - Queue-based order and stock notifications
- **Asynchronous processing** - Non-blocking operations for better performance

> **Note:** Azure Event Hubs integration can be added for real-time event streaming at enterprise scale, while Azure Service Bus can replace queues for more advanced messaging scenarios with topics and subscriptions.

## 📋 Available Endpoints

### Customers
- `GET /api/customers` - List all customers
- `GET /api/customers/{id}` - Get customer by ID
- `POST /api/customers` - Create customer
- `PUT /api/customers/{id}` - Update customer
- `DELETE /api/customers/{id}` - Delete customer

### Products
- `GET /api/products` - List all products
- `GET /api/products/{id}` - Get product by ID
- `POST /api/products` - Create product
- `PUT /api/products/{id}` - Update product
- `DELETE /api/products/{id}` - Delete product

### Orders
- `GET /api/orders` - List all orders
- `GET /api/orders/{id}` - Get order by ID
- `POST /api/orders` - Create order (triggers queue messages)
- `DELETE /api/orders/{id}` - Delete order

### File Uploads
- `POST /api/uploads/product-image` - Upload product image (Blob Storage)
- `GET /api/uploads/product-images` - List product images
- `POST /api/uploads/proof-of-payment` - Upload payment proof (Blob + Files)
- `POST /api/uploads/contract` - Upload contract (Azure Files)
- `GET /api/uploads/contracts` - List contracts

### Background Processors
- `OnProductImageUploaded` - Blob trigger when image uploaded
- `OrderNotifications_Processor` - Queue trigger for order notifications
- `StockUpdates_Processor` - Queue trigger for stock updates

## 🚀 Setup Instructions

### 1. Configure Connection String

Update `local.settings.json` with your Azure Storage connection string:
```json
{
  "STORAGE_CONNECTION": "YOUR_AZURE_STORAGE_CONNECTION_STRING_HERE"
}
```

### 2. Run Locally

```bash
cd ABCRetailers.Functions
dotnet restore
dotnet build
func start
```

Or using Rider:
1. Open the `ABCRetailers.Functions.csproj` project
2. Set as startup project
3. Press F5 to run

The functions will be available at `http://localhost:7071/api/`

### 3. Deploy to Azure

#### Using Azure CLI:
```bash
func azure functionapp publish <your-function-app-name>
```

#### Using Rider:
1. Right-click on the project
2. Select "Publish to Azure"
3. Follow the wizard to deploy

### 4. Configure App Settings in Azure

After deployment, add these application settings in the Azure Portal:

```
STORAGE_CONNECTION = <your-connection-string>
TABLE_CUSTOMER = Customers
TABLE_PRODUCT = Products
TABLE_ORDER = Orders
BLOB_PRODUCT_IMAGES = product-images
BLOB_PAYMENT_PROOFS = payment-proofs
QUEUE_ORDER_NOTIFICATIONS = order-notifications
QUEUE_STOCK_UPDATES = stock-updates
FILESHARE_CONTRACTS = contracts
FILESHARE_DIR_PAYMENTS = payments
```

## 📦 NuGet Packages Used

- `Microsoft.Azure.Functions.Worker` (v1.22.0)
- `Microsoft.Azure.Functions.Worker.Sdk` (v1.17.4)
- `Microsoft.Azure.Functions.Worker.Extensions.Http` (v3.1.0)
- `Microsoft.Azure.Functions.Worker.Extensions.Storage` (v6.3.0)
- `Microsoft.Azure.Functions.Worker.Extensions.EventHubs` (v6.3.6)
- `Azure.Data.Tables` (v12.9.1)
- `Azure.Storage.Blobs` (v12.21.2)
- `Azure.Storage.Queues` (v12.17.1)
- `Azure.Storage.Files.Shares` (v12.18.1)
- `Azure.Messaging.EventHubs` (v5.11.5)

## 🔍 Testing the Functions

### Using PowerShell/cURL:

**Create a Customer:**
```powershell
Invoke-RestMethod -Uri "http://localhost:7071/api/customers" -Method Post -Body '{"Name":"John","Surname":"Doe","Email":"john@example.com"}' -ContentType "application/json"
```

**Upload Product Image:**
```powershell
$formData = @{
    ProductImage = Get-Item "C:\path\to\image.jpg"
}
Invoke-RestMethod -Uri "http://localhost:7071/api/uploads/product-image" -Method Post -Form $formData
```

**Create Order (triggers queues):**
```powershell
Invoke-RestMethod -Uri "http://localhost:7071/api/orders" -Method Post -Body '{"CustomerId":"123","ProductId":"456","Quantity":2,"UnitPrice":99.99}' -ContentType "application/json"
```

## 📝 Project Structure

```
ABCRetailers.Functions/
├── Functions/
│   ├── CustomersFunctions.cs      # Table Storage - Customers
│   ├── ProductsFunctions.cs       # Table Storage - Products
│   ├── OrdersFunctions.cs         # Table Storage + Queue
│   ├── BlobFunctions.cs           # Blob Storage + Triggers
│   ├── QueueProcessorFunctions.cs # Queue Processors
│   └── UploadsFunctions.cs        # Blob + Azure Files
├── Entities/
│   └── TableEntities.cs           # Table entity models
├── Models/
│   └── ApiModels.cs               # DTO models
├── Helpers/
│   ├── HttpJson.cs                # JSON response helpers
│   ├── Map.cs                     # Entity mapping
│   └── MultipartHelper.cs         # File upload parsing
├── Program.cs                     # Function host startup
├── host.json                      # Function host config
└── local.settings.json            # Local configuration
```

## 🎓 POE Submission Checklist

- ✅ Store information into Azure Tables (Customers, Products, Orders)
- ✅ Write to Blob Storage (Product images, Payment proofs)
- ✅ Queue written to/from for transaction information (Order notifications, Stock updates)
- ✅ Write to Azure Files (Contracts, Payment metadata)
- ✅ Queue processors for improving customer experience
- ✅ Blob triggers for event-driven processing
- ✅ HTTP-triggered functions for REST API
- ✅ All functions use the provided storage account connection string

## 📚 Additional Notes

- All functions use **Anonymous** authorization level for easy testing
- Change to **Function** level authorization in production
- The web app connection string has been updated in `appsettings.json`
- Queue messages are automatically processed in the background
- Blob triggers activate when files are uploaded to containers





