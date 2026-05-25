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
    public class CarsApiControllerTest : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private const int DefaultUserId = 1;
        private const int OtherUserId = 999;
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public CarsApiControllerTest(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateAuthenticatedClient(userId: DefaultUserId);
        }

        [Fact]
        public async Task GetAll_ShouldReturnListOfCars()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            await EnsureUserAsync(dbContext, DefaultUserId);
            await CreateCar(dbContext, DefaultUserId);
            await CreateCar(dbContext, DefaultUserId);

            // Act
            var response = await _client.GetAsync("/api/cars");

            // Assert
            Assert.True(response.IsSuccessStatusCode, $"Expected a successful status code but got {response.StatusCode}");
            var dtos = await response.Content.ReadFromJsonAsync<List<CarListItemDto>>();
            Assert.NotNull(dtos);
            Assert.IsType<List<CarListItemDto>>(dtos);
            Assert.NotEmpty(dtos);
        }

        [Fact]
        public async Task GetAll_ShouldReturnEmptyListOfCars()
        {
            // Arrange
            var client = _factory.CreateAuthenticatedClient(userId: OtherUserId);

            // Act
            var response = await client.GetAsync("/api/cars");

            // Assert
            Assert.True(response.IsSuccessStatusCode, $"Expected a successful status code but got {response.StatusCode}");
            var dtos = await response.Content.ReadFromJsonAsync<List<CarListItemDto>>();
            Assert.NotNull(dtos);
            Assert.IsType<List<CarListItemDto>>(dtos);
            Assert.Empty(dtos);
        }

        [Fact]
        public async Task GetById_ShouldReturnCar_WhenCarExists()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            await EnsureUserAsync(dbContext, DefaultUserId);
            var car = await CreateCar(dbContext, DefaultUserId);

            // Act
            var response = await _client.GetAsync($"/api/cars/{car.Id}");

            // Assert
            Assert.True(response.IsSuccessStatusCode, $"Expected a successful status code but got {response.StatusCode}");
            var dto = await response.Content.ReadFromJsonAsync<CarDetailDto>();
            Assert.NotNull(dto);
            Assert.IsType<CarDetailDto>(dto);
            Assert.True(dto.Id == car.Id, $"Expected ID {car.Id} but got {dto.Id}");
        }

        [Fact]
        public async Task GetById_ShouldReturn404_WhenCarNotExists()
        {
            // Arrange
            // Act
            var response = await _client.GetAsync("/api/cars/9999");

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NotFound, $"Expected 404 Not Found but got {response.StatusCode}");
        }

        [Fact]
        public async Task Create_ShouldReturnCar_WhenValid()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            await EnsureUserAsync(dbContext, DefaultUserId);
            var dto = CreateValidCarCreateDto(DefaultUserId);

            // Act
            var response = await _client.PostAsJsonAsync("/api/cars", dto);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.Created, $"Expected 201 Created but got {response.StatusCode}");
            var created = await response.Content.ReadFromJsonAsync<CarDetailDto>();
            Assert.NotNull(created);
            Assert.Equal(dto.Brand, created.Brand);
            Assert.Equal(dto.Model, created.Model);
            Assert.Equal(dto.Year, created.Year);
            Assert.Equal(dto.FuelType, created.FuelType);
            Assert.Equal(DefaultUserId, created.UserId);

            var dbCar = await dbContext.Cars.IgnoreQueryFilters().FirstOrDefaultAsync(item => item.Id == created.Id);
            Assert.NotNull(dbCar);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenInvalid()
        {
            // Arrange
            var dto = CreateInvalidCarCreateDto();

            // Act
            var response = await _client.PostAsJsonAsync("/api/cars", dto);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.BadRequest, $"Expected 400 Bad Request but got {response.StatusCode}");
        }

        [Fact]
        public async Task Update_ShouldReturnNoContent_WhenValid()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            await EnsureUserAsync(dbContext, DefaultUserId);
            var car = await CreateCar(dbContext, DefaultUserId);
            var dto = CreateValidCarUpdateDto(DefaultUserId);

            // Act
            var response = await _client.PutAsJsonAsync($"/api/cars/{car.Id}", dto);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NoContent, $"Expected 204 No Content but got {response.StatusCode}");

            await dbContext.Entry(car).ReloadAsync();
            var dbCar = await dbContext.Cars.IgnoreQueryFilters().FirstOrDefaultAsync(item => item.Id == car.Id);
            Assert.NotNull(dbCar);
            Assert.Equal(dto.Brand, dbCar.Brand);
            Assert.Equal(dto.Model, dbCar.Model);
            Assert.Equal(dto.Year, dbCar.Year);
            Assert.Equal(dto.EngineVolume, dbCar.EngineVolume);
            Assert.Equal(dto.CurrentMilage, dbCar.CurrentMilage);
            Assert.Equal(dto.PurchasePrice, dbCar.PurchasePrice);
            Assert.Equal(dto.PurchaseDate, dbCar.PurchaseDate);
            Assert.Equal(dto.FuelType, dbCar.FuelType);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenInvalid()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            var car = await CreateCar(dbContext, DefaultUserId);
            var dto = CreateInvalidCarUpdateDto();

            // Act
            var response = await _client.PutAsJsonAsync($"/api/cars/{car.Id}", dto);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.BadRequest, $"Expected 400 Bad Request but got {response.StatusCode}");
        }

        [Fact]
        public async Task Update_ShouldReturn404_WhenCarNotExists()
        {
            // Arrange
            var dto = CreateValidCarUpdateDto(DefaultUserId);

            // Act
            var response = await _client.PutAsJsonAsync("/api/cars/9999", dto);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NotFound, $"Expected 404 Not Found but got {response.StatusCode}");
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenCarExists()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            var car = await CreateCar(dbContext, DefaultUserId);

            // Act
            var response = await _client.DeleteAsync($"/api/cars/{car.Id}");

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NoContent, $"Expected 204 No Content but got {response.StatusCode}");

            await dbContext.Entry(car).ReloadAsync();
            var dbCar = await dbContext.Cars.IgnoreQueryFilters().FirstOrDefaultAsync(item => item.Id == car.Id);
            Assert.NotNull(dbCar);
            Assert.True(dbCar.DeleatedAt.HasValue, "Expected car to be soft deleted.");
        }

        [Fact]
        public async Task Delete_ShouldReturn404_WhenCarNotExists()
        {
            // Arrange
            // Act
            var response = await _client.DeleteAsync("/api/cars/9999");

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

        private static CarCreateDto CreateValidCarCreateDto(int userId)
        {
            return new CarCreateDto
            {
                UserId = userId,
                Brand = "NewBrand",
                Model = "NewModel",
                Year = 2021,
                EngineVolume = 2.0,
                CurrentMilage = 34000,
                PurchasePrice = 22000m,
                PurchaseDate = new DateTime(2021, 2, 10),
                FuelType = FuelType.Diesel
            };
        }

        private static CarUpdateDto CreateValidCarUpdateDto(int userId)
        {
            return new CarUpdateDto
            {
                UserId = userId,
                Brand = "UpdatedBrand",
                Model = "UpdatedModel",
                Year = 2022,
                EngineVolume = 2.4,
                CurrentMilage = 28000,
                PurchasePrice = 24000m,
                PurchaseDate = new DateTime(2022, 3, 12),
                FuelType = FuelType.Hybrid
            };
        }

        private static CarCreateDto CreateInvalidCarCreateDto()
        {
            return new CarCreateDto
            {
                UserId = 0,
                Brand = string.Empty,
                Model = string.Empty,
                Year = 1900,
                EngineVolume = -1,
                CurrentMilage = -1,
                PurchasePrice = -5m,
                PurchaseDate = default,
                FuelType = FuelType.Petrol
            };
        }

        private static CarUpdateDto CreateInvalidCarUpdateDto()
        {
            return new CarUpdateDto
            {
                UserId = 0,
                Brand = string.Empty,
                Model = string.Empty,
                Year = 1900,
                EngineVolume = -2,
                CurrentMilage = -2,
                PurchasePrice = -10m,
                PurchaseDate = default,
                FuelType = FuelType.Petrol
            };
        }
    }
}
