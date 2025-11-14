using ABCRetailers.Models;
using ABCRetailers.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ABCRetailers.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAzureStorageService _storageService;
        private readonly ISqlDatabaseService _sqlService;

        public HomeController(
            ILogger<HomeController> logger,
            IAzureStorageService storageService,
            ISqlDatabaseService sqlService)
        {
            _logger = logger;
            _storageService = storageService;
            _sqlService = sqlService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Get counts from appropriate sources
                // Customers: Count from SQL Database (users with Customer role)
                var sqlUsers = await _sqlService.GetAllUsersAsync();
                var customerCount = sqlUsers.Count(u => u.Role == "Customer");

                // Products: Count from Azure Table Storage (as before)
                var products = await _storageService.GetAllEntitiesAsync<Product>();
                var productCount = products.Count;

                // Orders: Combine SQL Orders + Azure Table Storage Orders
                var sqlOrders = await _sqlService.GetAllOrdersAsync();
                var azureOrders = await _storageService.GetAllEntitiesAsync<Order>();
                var orderCount = sqlOrders.Count + azureOrders.Count;

                ViewBag.CustomerCount = customerCount;
                ViewBag.ProductCount = productCount;
                ViewBag.OrderCount = orderCount;

                // Featured products for the homepage
                ViewBag.FeaturedProducts = products.Take(6).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard data");
                ViewBag.CustomerCount = 0;
                ViewBag.ProductCount = 0;
                ViewBag.OrderCount = 0;
                ViewBag.FeaturedProducts = new List<Product>();
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}