using ABCRetailers.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ABCRetailers.Models;
using ABCRetailers.Services;

namespace ABCRetailers.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CustomerController : Controller
    {
        private readonly IAzureStorageService _storageService;
        private readonly ISqlDatabaseService _sqlService;

        public CustomerController(IAzureStorageService storageService, ISqlDatabaseService sqlService)
        {
            _storageService = storageService;
            _sqlService = sqlService;
        }

        public async Task<IActionResult> Index()
        {
            // Show SQL Users (with login accounts) instead of Azure Table Storage customers
            var sqlUsers = await _sqlService.GetAllUsersAsync();
            
            // Convert SQL Users to Customer model for the view
            var customers = sqlUsers
                .Where(u => u.Role == "Customer")
                .Select(u => new Customer
                {
                    PartitionKey = "Customer",
                    RowKey = u.UserId.ToString(), // CustomerId is computed from RowKey
                    Name = u.FirstName,
                    Surname = u.LastName,
                    Username = u.Username,
                    Email = u.Email,
                    ShippingAddress = u.ShippingAddress ?? "No address provided"
                })
                .ToList();
            
            return View(customers);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer customer)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _storageService.AddEntityAsync(customer);
                    TempData["Success"] = "Customer created successfully";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error creating customer: {ex.Message}");
                }
            }
            return View(customer);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var customer = await _storageService.GetEntityAsync<Customer>("Customer", id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Customer customer)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Retrieve the original entity to preserve a valid ETag for optimistic concurrency
                    var original = await _storageService.GetEntityAsync<Customer>("Customer", customer.RowKey);
                    if (original == null)
                    {
                        return NotFound();
                    }

                    // Update fields while keeping PartitionKey/RowKey/ETag from storage
                    original.Name = customer.Name;
                    original.Surname = customer.Surname;
                    original.Username = customer.Username;
                    original.Email = customer.Email;
                    original.ShippingAddress = customer.ShippingAddress;

                    await _storageService.UpdateEntityAsync(original);
                    TempData["Success"] = "Customer updated successfully";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error updating customer: {ex.Message}");
                }
            }
            return View(customer);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _storageService.DeleteEntityAsync<Customer>("Customer", id);
                TempData["Success"] = "Customer deleted successfully";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting customer: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> DeleteAjax([FromBody] DeleteRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.id))
            {
                return Json(new { success = false, message = "Invalid request" });
            }
            try
            {
                await _storageService.DeleteEntityAsync<Customer>("Customer", request.id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public class DeleteRequest
        {
            public string? id { get; set; }
        }
    }
}