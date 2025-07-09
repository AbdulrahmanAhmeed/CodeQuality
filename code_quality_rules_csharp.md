# Code Quality Rules with C# and .NET Examples

## 1. Readability and Clarity

Code should be written for humans to read, not just machines to execute. Use clear, descriptive variable and function names that express intent.

### ❌ Bad Example
```csharp
public decimal CalcT(decimal p, decimal r)
{
    return p * r;
}

var u = GetU();
var dt = DateTime.Now;
```

### ✅ Good Example
```csharp
public decimal CalculateTax(decimal price, decimal taxRate)
{
    return price * taxRate;
}

var currentUser = GetCurrentUser();
var currentDateTime = DateTime.Now;
```

## 2. Consistency

Follow consistent formatting, naming conventions, and coding patterns throughout your codebase.

### ❌ Bad Example
```csharp
public class userService
{
    public User getUser(int ID) { ... }
    public void UpdateUser(User user) { ... }
    public Boolean delete_user(int userId) { ... }
}
```

### ✅ Good Example
```csharp
public class UserService
{
    public User GetUser(int userId) { ... }
    public void UpdateUser(User user) { ... }
    public bool DeleteUser(int userId) { ... }
}
```

## 3. Single Responsibility Principle (SRP)

Each class and method should have only one reason to change.

### ❌ Bad Example
```csharp
public class UserManager
{
    public void SaveUser(User user)
    {
        // Validate user
        if (string.IsNullOrEmpty(user.Email))
            throw new ArgumentException("Email is required");
        
        // Save to database
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        // ... database code
        
        // Send email
        var emailService = new SmtpClient();
        emailService.Send(user.Email, "Welcome!", "Thanks for joining!");
    }
}
```

### ✅ Good Example
```csharp
public class UserManager
{
    private readonly IUserValidator _validator;
    private readonly IUserRepository _repository;
    private readonly IEmailService _emailService;

    public UserManager(IUserValidator validator, IUserRepository repository, IEmailService emailService)
    {
        _validator = validator;
        _repository = repository;
        _emailService = emailService;
    }

    public async Task SaveUserAsync(User user)
    {
        _validator.ValidateUser(user);
        await _repository.SaveAsync(user);
        await _emailService.SendWelcomeEmailAsync(user.Email);
    }
}

public class UserValidator : IUserValidator
{
    public void ValidateUser(User user)
    {
        if (string.IsNullOrEmpty(user.Email))
            throw new ArgumentException("Email is required");
        
        if (!IsValidEmail(user.Email))
            throw new ArgumentException("Invalid email format");
    }
}
```

## 4. Don't Repeat Yourself (DRY)

Eliminate code duplication by extracting common functionality into reusable methods or classes.

### ❌ Bad Example
```csharp
public class OrderService
{
    public void ProcessOnlineOrder(Order order)
    {
        if (order.Items.Count == 0)
            throw new InvalidOperationException("Order must have items");
        
        var total = order.Items.Sum(i => i.Price * i.Quantity);
        order.Total = total;
        order.Status = OrderStatus.Processing;
        // ... process online order
    }

    public void ProcessPhoneOrder(Order order)
    {
        if (order.Items.Count == 0)
            throw new InvalidOperationException("Order must have items");
        
        var total = order.Items.Sum(i => i.Price * i.Quantity);
        order.Total = total;
        order.Status = OrderStatus.Processing;
        // ... process phone order
    }
}
```

### ✅ Good Example
```csharp
public class OrderService
{
    public void ProcessOnlineOrder(Order order)
    {
        ValidateAndPrepareOrder(order);
        // ... process online order
    }

    public void ProcessPhoneOrder(Order order)
    {
        ValidateAndPrepareOrder(order);
        // ... process phone order
    }

    private void ValidateAndPrepareOrder(Order order)
    {
        if (order.Items.Count == 0)
            throw new InvalidOperationException("Order must have items");
        
        order.Total = order.Items.Sum(i => i.Price * i.Quantity);
        order.Status = OrderStatus.Processing;
    }
}
```

## 5. Error Handling

Handle errors gracefully and explicitly. Provide meaningful error messages.

### ❌ Bad Example
```csharp
public User GetUser(int userId)
{
    try
    {
        var user = _repository.GetById(userId);
        return user;
    }
    catch
    {
        return null;
    }
}
```

### ✅ Good Example
```csharp
public async Task<User> GetUserAsync(int userId)
{
    try
    {
        if (userId <= 0)
            throw new ArgumentException("User ID must be greater than zero", nameof(userId));

        var user = await _repository.GetByIdAsync(userId);
        
        if (user == null)
            throw new UserNotFoundException($"User with ID {userId} was not found");

        return user;
    }
    catch (DatabaseException ex)
    {
        _logger.LogError(ex, "Database error occurred while retrieving user {UserId}", userId);
        throw new ServiceException("An error occurred while retrieving the user", ex);
    }
}
```

## 6. Comprehensive Testing

Write tests that cover both happy paths and edge cases.

### Example Unit Test
```csharp
[TestClass]
public class UserServiceTests
{
    private Mock<IUserRepository> _mockRepository;
    private Mock<IEmailService> _mockEmailService;
    private UserService _userService;

    [TestInitialize]
    public void Setup()
    {
        _mockRepository = new Mock<IUserRepository>();
        _mockEmailService = new Mock<IEmailService>();
        _userService = new UserService(_mockRepository.Object, _mockEmailService.Object);
    }

    [TestMethod]
    public async Task CreateUserAsync_ValidUser_ReturnsCreatedUser()
    {
        // Arrange
        var user = new User { Email = "test@example.com", Name = "Test User" };
        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<User>())).ReturnsAsync(user);

        // Act
        var result = await _userService.CreateUserAsync(user);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(user.Email, result.Email);
        _mockRepository.Verify(r => r.CreateAsync(user), Times.Once);
        _mockEmailService.Verify(e => e.SendWelcomeEmailAsync(user.Email), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public async Task CreateUserAsync_InvalidEmail_ThrowsArgumentException()
    {
        // Arrange
        var user = new User { Email = "", Name = "Test User" };

        // Act
        await _userService.CreateUserAsync(user);
    }
}
```

## 7. Comments and Documentation

Write self-documenting code and use comments to explain "why" rather than "what".

### ❌ Bad Example
```csharp
// Increment i by 1
i++;

// Check if user is null
if (user == null)
{
    // Return null
    return null;
}
```

### ✅ Good Example
```csharp
/// <summary>
/// Calculates the compound interest for an investment
/// </summary>
/// <param name="principal">Initial investment amount</param>
/// <param name="rate">Annual interest rate (as decimal, e.g., 0.05 for 5%)</param>
/// <param name="years">Number of years</param>
/// <returns>The final amount after compound interest</returns>
public decimal CalculateCompoundInterest(decimal principal, decimal rate, int years)
{
    // Using the compound interest formula: A = P(1 + r)^t
    // We add 1 to the rate because the formula requires (1 + rate)
    return principal * (decimal)Math.Pow((double)(1 + rate), years);
}

public async Task<User> GetUserWithRetryAsync(int userId)
{
    // Retry logic is necessary here because the external user service
    // occasionally returns transient errors during high load periods
    var retryPolicy = Policy
        .Handle<HttpRequestException>()
        .WaitAndRetryAsync(3, retryAttempt => 
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

    return await retryPolicy.ExecuteAsync(async () =>
    {
        return await _externalUserService.GetUserAsync(userId);
    });
}
```

## 8. Performance Considerations

Write performant code but prioritize correctness and readability first.

### ❌ Bad Example
```csharp
public List<User> GetActiveUsers()
{
    var allUsers = _repository.GetAll(); // Loads all users into memory
    var activeUsers = new List<User>();
    
    foreach (var user in allUsers)
    {
        if (user.IsActive && user.LastLoginDate > DateTime.Now.AddDays(-30))
        {
            activeUsers.Add(user);
        }
    }
    
    return activeUsers;
}
```

### ✅ Good Example
```csharp
public async Task<IEnumerable<User>> GetActiveUsersAsync()
{
    // Use database filtering to avoid loading unnecessary data
    var cutoffDate = DateTime.Now.AddDays(-30);
    
    return await _repository.GetUsersAsync(
        filter: u => u.IsActive && u.LastLoginDate > cutoffDate,
        orderBy: u => u.LastLoginDate,
        take: 1000 // Limit results to prevent memory issues
    );
}

// For frequently accessed data, consider caching
public async Task<IEnumerable<User>> GetActiveUsersWithCacheAsync()
{
    const string cacheKey = "active_users";
    
    if (_cache.TryGetValue(cacheKey, out IEnumerable<User> cachedUsers))
    {
        return cachedUsers;
    }
    
    var users = await GetActiveUsersAsync();
    
    _cache.Set(cacheKey, users, TimeSpan.FromMinutes(15));
    
    return users;
}
```

## 9. Security Best Practices

Follow secure coding practices appropriate for .NET applications.

### ❌ Bad Example
```csharp
public List<User> SearchUsers(string searchTerm)
{
    // SQL Injection vulnerability
    var sql = $"SELECT * FROM Users WHERE Name LIKE '%{searchTerm}%'";
    return _database.Query<User>(sql).ToList();
}

public bool ValidateUser(string username, string password)
{
    var user = GetUserByUsername(username);
    
    // Storing passwords in plain text
    return user != null && user.Password == password;
}
```

### ✅ Good Example
```csharp
public async Task<List<User>> SearchUsersAsync(string searchTerm)
{
    // Use parameterized queries to prevent SQL injection
    var sql = "SELECT * FROM Users WHERE Name LIKE @SearchTerm";
    var parameters = new { SearchTerm = $"%{searchTerm}%" };
    
    return (await _database.QueryAsync<User>(sql, parameters)).ToList();
}

public async Task<bool> ValidateUserAsync(string username, string password)
{
    var user = await GetUserByUsernameAsync(username);
    
    if (user == null)
        return false;
    
    // Use proper password hashing with salt
    return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
}

public async Task<User> CreateUserAsync(User user, string password)
{
    // Hash password before storing
    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt());
    
    return await _repository.CreateAsync(user);
}
```

## 10. SOLID Principles in C#

### Dependency Inversion Principle Example

### ❌ Bad Example
```csharp
public class OrderService
{
    private SqlRepository _repository; // Depends on concrete implementation
    private EmailService _emailService; // Depends on concrete implementation
    
    public OrderService()
    {
        _repository = new SqlRepository(); // Tight coupling
        _emailService = new EmailService(); // Tight coupling
    }
}
```

### ✅ Good Example
```csharp
public class OrderService
{
    private readonly IOrderRepository _repository;
    private readonly IEmailService _emailService;
    
    public OrderService(IOrderRepository repository, IEmailService emailService)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    }
    
    public async Task ProcessOrderAsync(Order order)
    {
        await _repository.SaveAsync(order);
        await _emailService.SendOrderConfirmationAsync(order);
    }
}

// Register dependencies in Program.cs (or Startup.cs)
builder.Services.AddScoped<IOrderRepository, SqlOrderRepository>();
builder.Services.AddScoped<IEmailService, SmtpEmailService>();
builder.Services.AddScoped<OrderService>();
```

## 11. Async/Await Best Practices

### ❌ Bad Example
```csharp
public User GetUser(int userId)
{
    // Blocking async call
    return _repository.GetUserAsync(userId).Result;
}

public async Task<string> GetUserEmailAsync(int userId)
{
    // Unnecessary async wrapper
    return await Task.Run(() => _repository.GetUserEmail(userId));
}
```

### ✅ Good Example
```csharp
public async Task<User> GetUserAsync(int userId)
{
    // Proper async/await usage
    return await _repository.GetUserAsync(userId);
}

public async Task<List<User>> GetMultipleUsersAsync(List<int> userIds)
{
    // Parallel execution for independent operations
    var tasks = userIds.Select(id => _repository.GetUserAsync(id));
    var users = await Task.WhenAll(tasks);
    
    return users.Where(u => u != null).ToList();
}

public async Task<UserWithOrders> GetUserWithOrdersAsync(int userId)
{
    // Parallel execution for independent operations
    var userTask = _userRepository.GetUserAsync(userId);
    var ordersTask = _orderRepository.GetOrdersByUserIdAsync(userId);
    
    await Task.WhenAll(userTask, ordersTask);
    
    return new UserWithOrders
    {
        User = await userTask,
        Orders = await ordersTask
    };
}
```

## 12. Configuration and Environment Management

### ✅ Good Example
```csharp
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MyApp;Trusted_Connection=true;"
  },
  "EmailSettings": {
    "SmtpServer": "smtp.example.com",
    "Port": 587,
    "Username": "noreply@example.com"
  },
  "ApplicationSettings": {
    "MaxRetryAttempts": 3,
    "CacheExpirationMinutes": 30
  }
}

// Configuration classes
public class EmailSettings
{
    public string SmtpServer { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}

public class ApplicationSettings
{
    public int MaxRetryAttempts { get; set; }
    public int CacheExpirationMinutes { get; set; }
}

// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Bind configuration sections to strongly-typed classes
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection("ApplicationSettings"));

// Usage in service
public class EmailService
{
    private readonly EmailSettings _emailSettings;
    
    public EmailService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }
    
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        using var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port);
        // ... email sending logic
    }
}
```

## Summary

Following these code quality rules will help you write maintainable, readable, and robust C# applications. Remember:

1. **Prioritize readability** - Code is read more than it's written
2. **Be consistent** - Follow established patterns and conventions
3. **Keep it simple** - Avoid unnecessary complexity
4. **Handle errors gracefully** - Provide meaningful error messages
5. **Test thoroughly** - Write comprehensive unit and integration tests
6. **Document when necessary** - Explain the "why" behind complex logic
7. **Follow SOLID principles** - Create flexible and maintainable designs
8. **Use async/await properly** - Don't block async operations
9. **Secure by default** - Follow security best practices from the start
10. **Configure externally** - Keep configuration separate from code

These practices will help you create high-quality C# applications that are easier to maintain, debug, and extend over time.