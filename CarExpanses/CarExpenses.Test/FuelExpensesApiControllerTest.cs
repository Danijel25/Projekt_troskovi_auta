using CarExpenses.DAL;
using CarExpenses.Model.Enums;
using CarExpenses.Model.Models;
using CarExpenses.Web.Api.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace CarExpenses.Test
{
    public class FuelExpensesApiControllerTest : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private const int TestUserId = 6001;
        private const int EmptyUserId = 6002;
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public FuelExpensesApiControllerTest(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateAuthenticatedClient(userId: TestUserId);
        }

        [Fact]
        public async Task GetAll_ShouldReturnListOfFuelExpenses()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            await EnsureUserAsync(dbContext, TestUserId);
            var car = await CreateCar(dbContext, TestUserId);
            await CreateFuelExpense(dbContext, car);
            await CreateFuelExpense(dbContext, car);

            // Act
            var response = await _client.GetAsync("/api/fuel-expenses");

            // Assert
            Assert.True(response.IsSuccessStatusCode, $"Expected a successful status code but got {response.StatusCode}");
            var dtos = await response.Content.ReadFromJsonAsync<List<FuelExpenseDto>>();
            Assert.NotNull(dtos);
            Assert.IsType<List<FuelExpenseDto>>(dtos);
            Assert.NotEmpty(dtos);
        }

        [Fact]
        public async Task GetAll_ShouldReturnEmptyListOfFuelExpenses()
        {
            // Arrange
            var client = _factory.CreateAuthenticatedClient(userId: EmptyUserId);

            // Act
            var response = await client.GetAsync("/api/fuel-expenses");

            // Assert
            Assert.True(response.IsSuccessStatusCode, $"Expected a successful status code but got {response.StatusCode}");
            var dtos = await response.Content.ReadFromJsonAsync<List<FuelExpenseDto>>();
            Assert.NotNull(dtos);
            Assert.IsType<List<FuelExpenseDto>>(dtos);
            Assert.Empty(dtos);
        }

        [Fact]
        public async Task GetById_ShouldReturnFuelExpense_WhenFuelExpenseExists()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            await EnsureUserAsync(dbContext, TestUserId);
            var car = await CreateCar(dbContext, TestUserId);
            var fuelExpense = await CreateFuelExpense(dbContext, car);

            // Act
            var response = await _client.GetAsync($"/api/fuel-expenses/{fuelExpense.Id}");

            // Assert
            Assert.True(response.IsSuccessStatusCode, $"Expected a successful status code but got {response.StatusCode}");
            var dto = await response.Content.ReadFromJsonAsync<FuelExpenseDto>();
            Assert.NotNull(dto);
            Assert.IsType<FuelExpenseDto>(dto);
            Assert.True(dto.Id == fuelExpense.Id, $"Expected ID {fuelExpense.Id} but got {dto.Id}");
        }

        [Fact]
        public async Task GetById_ShouldReturn404_WhenFuelExpenseNotExists()
        {
            // Arrange
            // Act
            var response = await _client.GetAsync("/api/fuel-expenses/9999");

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NotFound, $"Expected 404 Not Found but got {response.StatusCode}");
        }

        [Fact]
        public async Task Create_ShouldReturnFuelExpense_WhenValid()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            await EnsureUserAsync(dbContext, TestUserId);
            var car = await CreateCar(dbContext, TestUserId);
            var dto = CreateValidFuelExpenseCreateDto(car.Id);

            // Act
            var response = await _client.PostAsJsonAsync("/api/fuel-expenses", dto);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.Created, $"Expected 201 Created but got {response.StatusCode}");
            var created = await response.Content.ReadFromJsonAsync<FuelExpenseDto>();
            Assert.NotNull(created);
            Assert.Equal(dto.FuelExpenseDate, created.FuelExpenseDate);
            Assert.Equal(dto.Liters, created.Liters);
            Assert.Equal(dto.PricePerLiter, created.PricePerLiter);
            Assert.Equal(dto.Kilometars, created.Kilometars);
            Assert.Equal(dto.CarId, created.CarId);

            var dbFuelExpense = await dbContext.FuelExpenses.IgnoreQueryFilters().FirstOrDefaultAsync(item => item.Id == created.Id);
            Assert.NotNull(dbFuelExpense);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenInvalid()
        {
            // Arrange
            var dto = CreateInvalidFuelExpenseCreateDto();

            // Act
            var response = await _client.PostAsJsonAsync("/api/fuel-expenses", dto);

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
            var fuelExpense = await CreateFuelExpense(dbContext, car);
            var dto = CreateValidFuelExpenseUpdateDto(car.Id);

            // Act
            var response = await _client.PutAsJsonAsync($"/api/fuel-expenses/{fuelExpense.Id}", dto);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NoContent, $"Expected 204 No Content but got {response.StatusCode}");

            await dbContext.Entry(fuelExpense).ReloadAsync();
            Assert.Equal(dto.FuelExpenseDate, fuelExpense.FuelExpenseDate);
            Assert.Equal(dto.Liters, fuelExpense.Liters);
            Assert.Equal(dto.PricePerLiter, fuelExpense.PricePerLiter);
            Assert.Equal(dto.Kilometars, fuelExpense.Kilometars);
            Assert.Equal(dto.CarId, fuelExpense.CarId);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenInvalid()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            await EnsureUserAsync(dbContext, TestUserId);
            var car = await CreateCar(dbContext, TestUserId);
            var fuelExpense = await CreateFuelExpense(dbContext, car);
            var dto = CreateInvalidFuelExpenseUpdateDto();

            // Act
            var response = await _client.PutAsJsonAsync($"/api/fuel-expenses/{fuelExpense.Id}", dto);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.BadRequest, $"Expected 400 Bad Request but got {response.StatusCode}");
        }

        [Fact]
        public async Task Update_ShouldReturn404_WhenFuelExpenseNotExists()
        {
            // Arrange
            var dto = CreateValidFuelExpenseUpdateDto(1);

            // Act
            var response = await _client.PutAsJsonAsync("/api/fuel-expenses/9999", dto);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NotFound, $"Expected 404 Not Found but got {response.StatusCode}");
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenFuelExpenseExists()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            await EnsureUserAsync(dbContext, TestUserId);
            var car = await CreateCar(dbContext, TestUserId);
            var fuelExpense = await CreateFuelExpense(dbContext, car);

            // Act
            var response = await _client.DeleteAsync($"/api/fuel-expenses/{fuelExpense.Id}");

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NoContent, $"Expected 204 No Content but got {response.StatusCode}");

            await dbContext.Entry(fuelExpense).ReloadAsync();
            var dbFuelExpense = await dbContext.FuelExpenses.IgnoreQueryFilters().FirstOrDefaultAsync(item => item.Id == fuelExpense.Id);
            Assert.NotNull(dbFuelExpense);
            Assert.True(dbFuelExpense.DeleatedAt.HasValue, "Expected fuel expense to be soft deleted.");
        }

        [Fact]
        public async Task Delete_ShouldReturn404_WhenFuelExpenseNotExists()
        {
            // Arrange
            // Act
            var response = await _client.DeleteAsync("/api/fuel-expenses/9999");

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
                FuelType = FuelType.Petrol
            };

            dbContext.Cars.Add(car);
            await dbContext.SaveChangesAsync();

            return car;
        }

        private static async Task<FuelExpense> CreateFuelExpense(CarExpesesDbContext dbContext, Car car)
        {
            var expense = new FuelExpense
            {
                FuelExpenseDate = new DateTime(2024, 1, 10),
                Liters = 45.5m,
                PricePerLiter = 1.65m,
                Kilometars = 123456,
                CarId = car.Id
            };

            dbContext.FuelExpenses.Add(expense);
            await dbContext.SaveChangesAsync();

            return expense;
        }

        private static FuelExpenseCreateDto CreateValidFuelExpenseCreateDto(int carId)
        {
            return new FuelExpenseCreateDto
            {
                FuelExpenseDate = new DateTime(2024, 2, 12),
                Liters = 50.75m,
                PricePerLiter = 1.72m,
                Kilometars = 123900,
                CarId = carId
            };
        }

        private static FuelExpenseUpdateDto CreateValidFuelExpenseUpdateDto(int carId)
        {
            return new FuelExpenseUpdateDto
            {
                FuelExpenseDate = new DateTime(2024, 3, 20),
                Liters = 42.25m,
                PricePerLiter = 1.60m,
                Kilometars = 124500,
                CarId = carId
            };
        }

        private static FuelExpenseCreateDto CreateInvalidFuelExpenseCreateDto()
        {
            return new FuelExpenseCreateDto
            {
                FuelExpenseDate = default,
                Liters = -1m,
                PricePerLiter = -1m,
                Kilometars = -1,
                CarId = 0
            };
        }

        private static FuelExpenseUpdateDto CreateInvalidFuelExpenseUpdateDto()
        {
            return new FuelExpenseUpdateDto
            {
                FuelExpenseDate = default,
                Liters = -5m,
                PricePerLiter = -5m,
                Kilometars = -10,
                CarId = 0
            };
        }
    }
}
