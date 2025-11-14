using ABCRetailers.Models;
using ABCRetailers.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ABCRetailers.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CartController : Controller
    {
        private readonly ISqlDatabaseService _sqlService;
        private readonly IAzureStorageService _storageService;
        private readonly ILogger<CartController> _logger;

        public CartController(
            ISqlDatabaseService sqlService,
            IAzureStorageService storageService,
            ILogger<CartController> logger)
        {
            _sqlService = sqlService;
            _storageService = storageService;
            _logger = logger;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }

        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            var cartItems = await _sqlService.GetUserCartAsync(userId);

            ViewBag.TotalItems = cartItems.Sum(c => c.Quantity);
            ViewBag.TotalPrice = cartItems.Sum(c => c.TotalPrice);

            return View(cartItems);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(string productId, int quantity = 1)
        {
            try
            {
                var userId = GetCurrentUserId();

                // Get product from Azure Table Storage
                var product = await _storageService.GetEntityAsync<Product>("Product", productId);
                if (product == null)
                {
                    TempData["Error"] = "Product not found.";
                    return RedirectToAction("Index", "Product");
                }

                if (product.StockAvailable < quantity)
                {
                    TempData["Error"] = $"Only {product.StockAvailable} units available in stock.";
                    return RedirectToAction("Index", "Product");
                }

                // Use the Azure Table Storage ProductId (GUID string) directly
                var cartItem = new Cart
                {
                    UserId = userId,
                    ProductId = productId, // Store Azure Table Storage GUID directly
                    Quantity = quantity,
                    UnitPrice = product.Price,
                    ProductName = product.ProductName,
                    ProductImageUrl = product.ImageUrl
                };

                await _sqlService.AddToCartAsync(cartItem);

                TempData["Success"] = $"{product.ProductName} added to cart!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to cart");
                TempData["Error"] = "An error occurred while adding item to cart.";
                return RedirectToAction("Index", "Product");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int cartId, int quantity)
        {
            try
            {
                if (quantity < 1)
                {
                    TempData["Error"] = "Quantity must be at least 1.";
                    return RedirectToAction(nameof(Index));
                }

                var cart = await _sqlService.GetUserCartAsync(GetCurrentUserId());
                var item = cart.FirstOrDefault(c => c.CartId == cartId);

                if (item == null)
                {
                    TempData["Error"] = "Cart item not found.";
                    return RedirectToAction(nameof(Index));
                }

                item.Quantity = quantity;
                await _sqlService.UpdateCartItemAsync(item);

                TempData["Success"] = "Cart updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart quantity");
                TempData["Error"] = "An error occurred while updating cart.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItem(int cartId)
        {
            try
            {
                await _sqlService.RemoveFromCartAsync(cartId);
                TempData["Success"] = "Item removed from cart.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cart item");
                TempData["Error"] = "An error occurred while removing item.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout()
        {
            try
            {
                var userId = GetCurrentUserId();
                var ordersCreated = await _sqlService.CreateOrderFromCartAsync(userId);

                if (ordersCreated == 0)
                {
                    TempData["Error"] = "Your cart is empty.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Success"] = $"Successfully placed {ordersCreated} order(s)! Check your orders to see the status.";
                return RedirectToAction("MyOrders", "Cart");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during checkout");
                TempData["Error"] = "An error occurred during checkout.";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> MyOrders()
        {
            var userId = GetCurrentUserId();
            var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "";
            
            // Get SQL orders (new orders placed after login)
            var sqlOrders = await _sqlService.GetUserOrdersAsync(userId);
            
            // Get Azure Table Storage orders (legacy orders by username)
            var azureOrders = await _storageService.GetAllEntitiesAsync<Order>();
            var userAzureOrders = azureOrders
                .Where(o => o.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
                .Select(o => new
                {
                    OrderId = o.OrderId,
                    ProductName = o.ProductName,
                    Quantity = o.Quantity,
                    UnitPrice = o.UnitPrice,
                    TotalPrice = o.TotalPrice,
                    Status = o.Status,
                    OrderDate = o.OrderDate,
                    ShippingAddress = "Azure Table Storage" // Legacy orders
                })
                .ToList();
            
            // Combine both (cast to dynamic for view compatibility)
            var combinedOrders = new List<dynamic>();
            combinedOrders.AddRange(sqlOrders);
            combinedOrders.AddRange(userAzureOrders);
            
            // Sort by date (newest first)
            var sortedOrders = combinedOrders.OrderByDescending(o => o.OrderDate).ToList();
            
            ViewBag.HasAzureOrders = userAzureOrders.Any();
            ViewBag.HasSqlOrders = sqlOrders.Any();
            
            return View(sortedOrders);
        }
    }
}

