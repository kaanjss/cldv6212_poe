using ABCRetailers.Models;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace ABCRetailers.Services
{
    public class SqlDatabaseService : ISqlDatabaseService
    {
        private readonly string _connectionString;
        private readonly ILogger<SqlDatabaseService> _logger;

        public SqlDatabaseService(IConfiguration configuration, ILogger<SqlDatabaseService> logger)
        {
            _connectionString = configuration.GetConnectionString("AzureSqlDatabase")
                ?? throw new InvalidOperationException("SQL Database connection string not found");
            _logger = logger;
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        public async Task<User?> AuthenticateUserAsync(string usernameOrEmail, string password)
        {
            var passwordHash = HashPassword(password);
            
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT UserId, Username, Email, PasswordHash, FirstName, LastName, Role, ShippingAddress, IsActive, CreatedDate, LastLoginDate
                FROM Users
                WHERE (Username = @UsernameOrEmail OR Email = @UsernameOrEmail) AND PasswordHash = @PasswordHash AND IsActive = 1";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UsernameOrEmail", usernameOrEmail);
            command.Parameters.AddWithValue("@PasswordHash", passwordHash);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    UserId = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Email = reader.GetString(2),
                    PasswordHash = reader.GetString(3),
                    FirstName = reader.GetString(4),
                    LastName = reader.GetString(5),
                    Role = reader.GetString(6),
                    ShippingAddress = reader.IsDBNull(7) ? null : reader.GetString(7),
                    IsActive = reader.GetBoolean(8),
                    CreatedDate = reader.GetDateTime(9),
                    LastLoginDate = reader.IsDBNull(10) ? null : reader.GetDateTime(10)
                };
            }

            return null;
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT UserId, Username, Email, PasswordHash, FirstName, LastName, Role, ShippingAddress, IsActive, CreatedDate, LastLoginDate FROM Users WHERE UserId = @UserId";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    UserId = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Email = reader.GetString(2),
                    PasswordHash = reader.GetString(3),
                    FirstName = reader.GetString(4),
                    LastName = reader.GetString(5),
                    Role = reader.GetString(6),
                    ShippingAddress = reader.IsDBNull(7) ? null : reader.GetString(7),
                    IsActive = reader.GetBoolean(8),
                    CreatedDate = reader.GetDateTime(9),
                    LastLoginDate = reader.IsDBNull(10) ? null : reader.GetDateTime(10)
                };
            }

            return null;
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT UserId, Username, Email, PasswordHash, FirstName, LastName, Role, ShippingAddress, IsActive, CreatedDate, LastLoginDate FROM Users WHERE Username = @Username";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Username", username);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    UserId = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Email = reader.GetString(2),
                    PasswordHash = reader.GetString(3),
                    FirstName = reader.GetString(4),
                    LastName = reader.GetString(5),
                    Role = reader.GetString(6),
                    ShippingAddress = reader.IsDBNull(7) ? null : reader.GetString(7),
                    IsActive = reader.GetBoolean(8),
                    CreatedDate = reader.GetDateTime(9),
                    LastLoginDate = reader.IsDBNull(10) ? null : reader.GetDateTime(10)
                };
            }

            return null;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT UserId, Username, Email, PasswordHash, FirstName, LastName, Role, ShippingAddress, IsActive, CreatedDate, LastLoginDate FROM Users WHERE Email = @Email";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Email", email);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    UserId = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Email = reader.GetString(2),
                    PasswordHash = reader.GetString(3),
                    FirstName = reader.GetString(4),
                    LastName = reader.GetString(5),
                    Role = reader.GetString(6),
                    ShippingAddress = reader.IsDBNull(7) ? null : reader.GetString(7),
                    IsActive = reader.GetBoolean(8),
                    CreatedDate = reader.GetDateTime(9),
                    LastLoginDate = reader.IsDBNull(10) ? null : reader.GetDateTime(10)
                };
            }

            return null;
        }

        public async Task<User> RegisterUserAsync(User user, string password)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var passwordHash = HashPassword(password);

            var query = @"
                INSERT INTO Users (Username, Email, PasswordHash, FirstName, LastName, Role, ShippingAddress, IsActive, CreatedDate)
                VALUES (@Username, @Email, @PasswordHash, @FirstName, @LastName, @Role, @ShippingAddress, @IsActive, @CreatedDate);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Username", user.Username);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@PasswordHash", passwordHash);
            command.Parameters.AddWithValue("@FirstName", user.FirstName);
            command.Parameters.AddWithValue("@LastName", user.LastName);
            command.Parameters.AddWithValue("@Role", user.Role);
            command.Parameters.AddWithValue("@ShippingAddress", (object?)user.ShippingAddress ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", user.IsActive);
            command.Parameters.AddWithValue("@CreatedDate", DateTime.UtcNow);

            var userId = (int)await command.ExecuteScalarAsync();
            user.UserId = userId;
            user.PasswordHash = passwordHash;

            return user;
        }

        public async Task UpdateLastLoginAsync(int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "UPDATE Users SET LastLoginDate = @LastLoginDate WHERE UserId = @UserId";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@LastLoginDate", DateTime.UtcNow);

            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var users = new List<User>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT UserId, Username, Email, PasswordHash, FirstName, LastName, Role, ShippingAddress, IsActive, CreatedDate, LastLoginDate FROM Users ORDER BY Role DESC, UserId";

            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                users.Add(new User
                {
                    UserId = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Email = reader.GetString(2),
                    PasswordHash = reader.GetString(3),
                    FirstName = reader.GetString(4),
                    LastName = reader.GetString(5),
                    Role = reader.GetString(6),
                    ShippingAddress = reader.IsDBNull(7) ? null : reader.GetString(7),
                    IsActive = reader.GetBoolean(8),
                    CreatedDate = reader.GetDateTime(9),
                    LastLoginDate = reader.IsDBNull(10) ? null : reader.GetDateTime(10)
                });
            }

            return users;
        }

        // Cart Operations
        public async Task<List<Cart>> GetUserCartAsync(int userId)
        {
            var cartItems = new List<Cart>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // Cart table stores ProductName and ProductImageUrl directly (no JOIN needed)
            var query = @"
                SELECT CartId, UserId, ProductId, ProductName, ProductImageUrl, Quantity, UnitPrice, DateAdded
                FROM Cart
                WHERE UserId = @UserId
                ORDER BY DateAdded DESC";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                cartItems.Add(new Cart
                {
                    CartId = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    ProductId = reader.GetString(2), // Azure Table Storage GUID
                    ProductName = reader.GetString(3),
                    ProductImageUrl = reader.IsDBNull(4) ? null : reader.GetString(4),
                    Quantity = reader.GetInt32(5),
                    UnitPrice = reader.GetDecimal(6),
                    DateAdded = reader.GetDateTime(7)
                });
            }

            return cartItems;
        }

        public async Task<Cart> AddToCartAsync(Cart cartItem)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // Check if item already exists in cart
            var checkQuery = "SELECT CartId, Quantity FROM Cart WHERE UserId = @UserId AND ProductId = @ProductId";
            using var checkCommand = new SqlCommand(checkQuery, connection);
            checkCommand.Parameters.AddWithValue("@UserId", cartItem.UserId);
            checkCommand.Parameters.AddWithValue("@ProductId", cartItem.ProductId);

            using var reader = await checkCommand.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var existingCartId = reader.GetInt32(0);
                var existingQuantity = reader.GetInt32(1);
                await reader.CloseAsync();

                // Update existing cart item
                var updateQuery = "UPDATE Cart SET Quantity = @Quantity WHERE CartId = @CartId";
                using var updateCommand = new SqlCommand(updateQuery, connection);
                updateCommand.Parameters.AddWithValue("@CartId", existingCartId);
                updateCommand.Parameters.AddWithValue("@Quantity", existingQuantity + cartItem.Quantity);
                await updateCommand.ExecuteNonQueryAsync();

                cartItem.CartId = existingCartId;
                cartItem.Quantity += existingQuantity;
                return cartItem;
            }
            await reader.CloseAsync();

            // Insert new cart item
            var insertQuery = @"
                INSERT INTO Cart (UserId, ProductId, ProductName, ProductImageUrl, Quantity, UnitPrice, DateAdded)
                VALUES (@UserId, @ProductId, @ProductName, @ProductImageUrl, @Quantity, @UnitPrice, @DateAdded);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            using var insertCommand = new SqlCommand(insertQuery, connection);
            insertCommand.Parameters.AddWithValue("@UserId", cartItem.UserId);
            insertCommand.Parameters.AddWithValue("@ProductId", cartItem.ProductId);
            insertCommand.Parameters.AddWithValue("@ProductName", cartItem.ProductName ?? string.Empty);
            insertCommand.Parameters.AddWithValue("@ProductImageUrl", (object?)cartItem.ProductImageUrl ?? DBNull.Value);
            insertCommand.Parameters.AddWithValue("@Quantity", cartItem.Quantity);
            insertCommand.Parameters.AddWithValue("@UnitPrice", cartItem.UnitPrice);
            insertCommand.Parameters.AddWithValue("@DateAdded", DateTime.UtcNow);

            var cartId = (int)await insertCommand.ExecuteScalarAsync();
            cartItem.CartId = cartId;

            return cartItem;
        }

        public async Task UpdateCartItemAsync(Cart cartItem)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "UPDATE Cart SET Quantity = @Quantity WHERE CartId = @CartId";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@CartId", cartItem.CartId);
            command.Parameters.AddWithValue("@Quantity", cartItem.Quantity);

            await command.ExecuteNonQueryAsync();
        }

        public async Task RemoveFromCartAsync(int cartId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "DELETE FROM Cart WHERE CartId = @CartId";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@CartId", cartId);

            await command.ExecuteNonQueryAsync();
        }

        public async Task ClearCartAsync(int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "DELETE FROM Cart WHERE UserId = @UserId";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);

            await command.ExecuteNonQueryAsync();
        }

        // Order Operations
        public async Task<int> CreateOrderFromCartAsync(int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                // Get cart items (Cart already has ProductName, so no need to JOIN Products)
                var cartQuery = @"
                    SELECT c.ProductId, c.ProductName, c.Quantity, c.UnitPrice, u.ShippingAddress
                    FROM Cart c
                    JOIN Users u ON c.UserId = u.UserId
                    WHERE c.UserId = @UserId";

                using var cartCommand = new SqlCommand(cartQuery, connection, transaction);
                cartCommand.Parameters.AddWithValue("@UserId", userId);

                var orderItems = new List<(string ProductId, string ProductName, int Quantity, decimal UnitPrice, string ShippingAddress)>();

                using var reader = await cartCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    orderItems.Add((
                        reader.GetString(0),      // ProductId (Azure Table Storage GUID)
                        reader.GetString(1),      // ProductName
                        reader.GetInt32(2),       // Quantity
                        reader.GetDecimal(3),     // UnitPrice
                        reader.IsDBNull(4) ? "No address provided" : reader.GetString(4)  // ShippingAddress
                    ));
                }
                await reader.CloseAsync();

                if (orderItems.Count == 0)
                {
                    transaction.Rollback();
                    return 0;
                }

                // Create orders
                foreach (var item in orderItems)
                {
                    var orderQuery = @"
                        INSERT INTO Orders (UserId, ProductId, ProductName, Quantity, UnitPrice, TotalPrice, Status, ShippingAddress, OrderDate)
                        VALUES (@UserId, @ProductId, @ProductName, @Quantity, @UnitPrice, @TotalPrice, @Status, @ShippingAddress, @OrderDate)";

                    using var orderCommand = new SqlCommand(orderQuery, connection, transaction);
                    orderCommand.Parameters.AddWithValue("@UserId", userId);
                    orderCommand.Parameters.AddWithValue("@ProductId", item.ProductId);
                    orderCommand.Parameters.AddWithValue("@ProductName", item.ProductName);
                    orderCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                    orderCommand.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);
                    orderCommand.Parameters.AddWithValue("@TotalPrice", item.Quantity * item.UnitPrice);
                    orderCommand.Parameters.AddWithValue("@Status", "Submitted");
                    orderCommand.Parameters.AddWithValue("@ShippingAddress", item.ShippingAddress);
                    orderCommand.Parameters.AddWithValue("@OrderDate", DateTime.UtcNow);

                    await orderCommand.ExecuteNonQueryAsync();
                }

                // Clear cart
                var clearCartQuery = "DELETE FROM Cart WHERE UserId = @UserId";
                using var clearCartCommand = new SqlCommand(clearCartQuery, connection, transaction);
                clearCartCommand.Parameters.AddWithValue("@UserId", userId);
                await clearCartCommand.ExecuteNonQueryAsync();

                transaction.Commit();
                return orderItems.Count;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<List<dynamic>> GetUserOrdersAsync(int userId)
        {
            var orders = new List<dynamic>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT o.OrderId, o.ProductName, o.Quantity, o.UnitPrice, o.TotalPrice, o.Status, o.OrderDate, o.ShippingAddress
                FROM Orders o
                WHERE o.UserId = @UserId
                ORDER BY o.OrderDate DESC";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                orders.Add(new
                {
                    OrderId = reader.GetInt32(0),
                    ProductName = reader.GetString(1),
                    Quantity = reader.GetInt32(2),
                    UnitPrice = reader.GetDecimal(3),
                    TotalPrice = reader.GetDecimal(4),
                    Status = reader.GetString(5),
                    OrderDate = reader.GetDateTime(6),
                    ShippingAddress = reader.GetString(7)
                });
            }

            return orders;
        }

        public async Task<List<dynamic>> GetAllOrdersAsync()
        {
            var orders = new List<dynamic>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT o.OrderId, u.Username, u.FirstName, u.LastName, o.ProductName, o.Quantity, o.UnitPrice, o.TotalPrice, o.Status, o.OrderDate, o.ShippingAddress
                FROM Orders o
                JOIN Users u ON o.UserId = u.UserId
                ORDER BY o.OrderDate DESC";

            using var command = new SqlCommand(query, connection);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                orders.Add(new
                {
                    OrderId = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    CustomerName = $"{reader.GetString(2)} {reader.GetString(3)}",
                    ProductName = reader.GetString(4),
                    Quantity = reader.GetInt32(5),
                    UnitPrice = reader.GetDecimal(6),
                    TotalPrice = reader.GetDecimal(7),
                    Status = reader.GetString(8),
                    OrderDate = reader.GetDateTime(9),
                    ShippingAddress = reader.GetString(10)
                });
            }

            return orders;
        }

        public async Task UpdateOrderStatusAsync(int orderId, string status)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "UPDATE Orders SET Status = @Status WHERE OrderId = @OrderId";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@OrderId", orderId);
            command.Parameters.AddWithValue("@Status", status);

            await command.ExecuteNonQueryAsync();
        }
    }
}

