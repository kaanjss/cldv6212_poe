using ABCRetailers.Models;

namespace ABCRetailers.Services
{
    public interface ISqlDatabaseService
    {
        // User authentication
        Task<User?> AuthenticateUserAsync(string usernameOrEmail, string password);
        Task<User?> GetUserByIdAsync(int userId);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User> RegisterUserAsync(User user, string password);
        Task UpdateLastLoginAsync(int userId);
        Task<List<User>> GetAllUsersAsync();

        // Cart operations
        Task<List<Cart>> GetUserCartAsync(int userId);
        Task<Cart> AddToCartAsync(Cart cartItem);
        Task UpdateCartItemAsync(Cart cartItem);
        Task RemoveFromCartAsync(int cartId);
        Task ClearCartAsync(int userId);

        // Order operations
        Task<int> CreateOrderFromCartAsync(int userId);
        Task<List<dynamic>> GetUserOrdersAsync(int userId);
        Task<List<dynamic>> GetAllOrdersAsync();
        Task UpdateOrderStatusAsync(int orderId, string status);
    }
}

