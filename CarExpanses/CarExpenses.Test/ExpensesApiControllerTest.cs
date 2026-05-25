using CarExpenses.DAL;
using CarExpenses.Model.Models;
using CarExpenses.Web.Api.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace CarExpenses.Test
{
    public class ExpensesApiControllerTest : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private const int TestUserId = 5001;
        private const int EmptyUserId = 5002;
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public ExpensesApiControllerTest(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateAuthenticatedClient(userId: TestUserId);
        }

        [Fact]
        public async Task GetAll_ShouldReturnListOfExpenses()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            await EnsureUserAsync(dbContext, TestUserId);
            var car = await CreateCar(dbContext, TestUserId);
            var category = await CreateExpenseCategory(dbContext, "Fuel");
            await CreateExpense(dbContext, car, category);
            await CreateExpense(dbContext, car, category);

            // Act
            var response = await _client.GetAsync("/api/expenses");

            // Assert
            Assert.True(response.IsSuccessStatusCode, $"Expected a successful status code but got {response.StatusCode}");
            var dtos = await response.Content.ReadFromJsonAsync<List<ExpenseListItemDto>>();
            Assert.NotNull(dtos);
            Assert.IsType<List<ExpenseListItemDto>>(dtos);
            Assert.NotEmpty(dtos);
        }

        [Fact]
        public async Task GetAll_ShouldReturnEmptyListOfExpenses()
        {
            // Arrange
            var client = _factory.CreateAuthenticatedClient(userId: EmptyUserId);

            // Act
            var response = await client.GetAsync("/api/expenses");

            // Assert
            Assert.True(response.IsSuccessStatusCode, $"Expected a successful status code but got {response.StatusCode}");
            var dtos = await response.Content.ReadFromJsonAsync<List<ExpenseListItemDto>>();
            Assert.NotNull(dtos);
            Assert.IsType<List<ExpenseListItemDto>>(dtos);
            Assert.Empty(dtos);
        }

        [Fact]
        public async Task GetById_ShouldReturnExpense_WhenExpenseExists()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            await EnsureUserAsync(dbContext, TestUserId);
            var car = await CreateCar(dbContext, TestUserId);
            var category = await CreateExpenseCategory(dbContext, "Service");
            var expense = await CreateExpense(dbContext, car, category);

            // Act
            var response = await _client.GetAsync($"/api/expenses/{expense.Id}");

            // Assert
            Assert.True(response.IsSuccessStatusCode, $"Expected a successful status code but got {response.StatusCode}");
            var dto = await response.Content.ReadFromJsonAsync<ExpenseDetailDto>();
            Assert.NotNull(dto);
            Assert.IsType<ExpenseDetailDto>(dto);
            Assert.True(dto.Id == expense.Id, $"Expected ID {expense.Id} but got {dto.Id}");
        }

        [Fact]
        public async Task GetById_ShouldReturn404_WhenExpenseNotExists()
        {
            // Arrange
            // Act
            var response = await _client.GetAsync("/api/expenses/9999");

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NotFound, $"Expected 404 Not Found but got {response.StatusCode}");
        }

        [Fact]
        public async Task Create_ShouldReturnExpense_WhenValid()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            await EnsureUserAsync(dbContext, TestUserId);
            var car = await CreateCar(dbContext, TestUserId);
            var category = await CreateExpenseCategory(dbContext, "Parking");
            var dto = CreateValidExpenseCreateDto(category.Id, car.Id);

            // Act
            var response = await _client.PostAsJsonAsync("/api/expenses", dto);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.Created, $"Expected 201 Created but got {response.StatusCode}");
            var created = await response.Content.ReadFromJsonAsync<ExpenseDetailDto>();
            Assert.NotNull(created);
            Assert.Equal(dto.Description, created.Description);
            Assert.Equal(dto.Amount, created.Amount);
            Assert.Equal(dto.Date, created.Date);
            Assert.Equal(dto.CategoryId, created.CategoryId);
            Assert.Equal(dto.CarId, created.CarId);

            var dbExpense = await dbContext.Expenses.IgnoreQueryFilters().FirstOrDefaultAsync(item => item.Id == created.Id);
            Assert.NotNull(dbExpense);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenInvalid()
        {
            // Arrange
            var dto = CreateInvalidExpenseCreateDto();

            // Act
            var response = await _client.PostAsJsonAsync("/api/expenses", dto);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.BadRequest, $"Expected 400 Bad Request but got {response.StatusCode}");
        }

        [Fact]
        public async Task Update_ShouldReturnNoContent_WhenValid()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            await EnsureUserAsync(dbContext, TestUserId);
            var car = await CreateCar(dbContext, TestUserId);
            var category = await CreateExpenseCategory(dbContext, "Tolls");
            var expense = await CreateExpense(dbContext, car, category);
            var dto = CreateValidExpenseUpdateDto(category.Id, car.Id);

            // Act
            var response = await _client.PutAsJsonAsync($"/api/expenses/{expense.Id}", dto);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NoContent, $"Expected 204 No Content but got {response.StatusCode}");

            await dbContext.Entry(expense).ReloadAsync();
            Assert.Equal(dto.Description, expense.Description);
            Assert.Equal(dto.Amount, expense.Amount);
            Assert.Equal(dto.Date, expense.Date);
            Assert.Equal(dto.CategoryId, expense.CategoryId);
            Assert.Equal(dto.CarId, expense.CarId);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenInvalid()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            await EnsureUserAsync(dbContext, TestUserId);
            var car = await CreateCar(dbContext, TestUserId);
            var category = await CreateExpenseCategory(dbContext, "Repairs");
            var expense = await CreateExpense(dbContext, car, category);
            var dto = CreateInvalidExpenseUpdateDto();

            // Act
            var response = await _client.PutAsJsonAsync($"/api/expenses/{expense.Id}", dto);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.BadRequest, $"Expected 400 Bad Request but got {response.StatusCode}");
        }

        [Fact]
        public async Task Update_ShouldReturn404_WhenExpenseNotExists()
        {
            // Arrange
            var dto = CreateValidExpenseUpdateDto(1, 1);

            // Act
            var response = await _client.PutAsJsonAsync("/api/expenses/9999", dto);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NotFound, $"Expected 404 Not Found but got {response.StatusCode}");
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenExpenseExists()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            await EnsureUserAsync(dbContext, TestUserId);
            var car = await CreateCar(dbContext, TestUserId);
            var category = await CreateExpenseCategory(dbContext, "Maintenance");
            var expense = await CreateExpense(dbContext, car, category);

            // Act
            var response = await _client.DeleteAsync($"/api/expenses/{expense.Id}");

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NoContent, $"Expected 204 No Content but got {response.StatusCode}");

            await dbContext.Entry(expense).ReloadAsync();
            var dbExpense = await dbContext.Expenses.IgnoreQueryFilters().FirstOrDefaultAsync(item => item.Id == expense.Id);
            Assert.NotNull(dbExpense);
            Assert.True(dbExpense.DeleatedAt.HasValue, "Expected expense to be soft deleted.");
        }

        [Fact]
        public async Task Delete_ShouldReturn404_WhenExpenseNotExists()
        {
            // Arrange
            // Act
            var response = await _client.DeleteAsync("/api/expenses/9999");

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NotFound, $"Expected 404 Not Found but got {response.StatusCode}");
        }

        private static async Task EnsureUserAsync(CarExpesesDbContext dbContext, int userId)
        {
            if (await dbContext.Users.IgnoreQueryFilters().AnyAsync(user => user.Id == userId))
            {
                return;
            }

            var user = new User
            {
                Id = userId,
                UserName = $"test-user-{userId}",
                Email = $"test{userId}@example.com"
            };

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
        }

        private static async Task<Car> CreateCar(CarExpesesDbContext dbContext, int userId)
        {
            var car = new Car
            {
                UserId = userId,
                Brand = "TestBrand",
                Model = "TestModel",
                Year = 2020,
                EngineVolume = 1.6,
                CurrentMilage = 45000,
                PurchasePrice = 15000m,
                PurchaseDate = new DateTime(2020, 5, 20),
                FuelType = Model.Enums.FuelType.Petrol
            };

            dbContext.Cars.Add(car);
            await dbContext.SaveChangesAsync();

            return car;
        }

        private static async Task<ExpenseCategory> CreateExpenseCategory(CarExpesesDbContext dbContext, string name)
        {
            var category = new ExpenseCategory
            {
                Name = name
            };

            dbContext.ExpenseCategories.Add(category);
            await dbContext.SaveChangesAsync();

            return category;
        }

        private static async Task<Expense> CreateExpense(CarExpesesDbContext dbContext, Car car, ExpenseCategory category)
        {
            var expense = new Expense
            {
                Description = "Test Expense",
                Amount = 120.50m,
                Date = new DateTime(2024, 1, 15),
                CategoryId = category.Id,
                Category = category,
                CarId = car.Id,
                Car = car
            };

            dbContext.Expenses.Add(expense);
            await dbContext.SaveChangesAsync();

            return expense;
        }

        private static ExpenseCreateDto CreateValidExpenseCreateDto(int categoryId, int carId)
        {
            return new ExpenseCreateDto
            {
                Description = "Test expense",
                Amount = 250.75m,
                Date = new DateTime(2024, 2, 1),
                CategoryId = categoryId,
                CarId = carId
            };
        }

        private static ExpenseUpdateDto CreateValidExpenseUpdateDto(int categoryId, int carId)
        {
            return new ExpenseUpdateDto
            {
                Description = "Updated expense",
                Amount = 199.99m,
                Date = new DateTime(2024, 3, 5),
                CategoryId = categoryId,
                CarId = carId
            };
        }

        private static ExpenseCreateDto CreateInvalidExpenseCreateDto()
        {
            return new ExpenseCreateDto
            {
                Description = string.Empty,
                Amount = -1m,
                Date = default,
                CategoryId = 0,
                CarId = 0
            };
        }

        private static ExpenseUpdateDto CreateInvalidExpenseUpdateDto()
        {
            return new ExpenseUpdateDto
            {
                Description = string.Empty,
                Amount = -5m,
                Date = default,
                CategoryId = 0,
                CarId = 0
            };
        }
    }
}
