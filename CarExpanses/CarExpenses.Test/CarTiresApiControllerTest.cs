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
    public class CarTiresApiControllerTest : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private const int TestUserId = 11001;
        private const int EmptyUserId = 11002;
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public CarTiresApiControllerTest(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateAuthenticatedClient(userId: TestUserId);
        }

        [Fact]
        public async Task GetAll_ShouldReturnListOfCarTires()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            await EnsureUserAsync(dbContext, TestUserId);
            var car = await CreateCar(dbContext, TestUserId);
            var tire = await CreateTire(dbContext);
            await CreateCarTire(dbContext, car, tire, new DateTime(2025, 1, 10));
            await CreateCarTire(dbContext, car, tire, new DateTime(2025, 2, 10));

            // Act
            var response = await _client.GetAsync("/api/car-tires");

            // Assert
            Assert.True(response.IsSuccessStatusCode, $"Expected a successful status code but got {response.StatusCode}");
            var dtos = await response.Content.ReadFromJsonAsync<List<CarTireDto>>();
            Assert.NotNull(dtos);
            Assert.IsType<List<CarTireDto>>(dtos);
            Assert.NotEmpty(dtos);
        }

        [Fact]
        public async Task GetAll_ShouldReturnEmptyListOfCarTires()
        {
            // Arrange
            var client = _factory.CreateAuthenticatedClient(userId: EmptyUserId);

            // Act
            var response = await client.GetAsync("/api/car-tires");

            // Assert
            Assert.True(response.IsSuccessStatusCode, $"Expected a successful status code but got {response.StatusCode}");
            var dtos = await response.Content.ReadFromJsonAsync<List<CarTireDto>>();
            Assert.NotNull(dtos);
            Assert.IsType<List<CarTireDto>>(dtos);
            Assert.Empty(dtos);
        }

        [Fact]
        public async Task GetById_ShouldReturnCarTire_WhenCarTireExists()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            await EnsureUserAsync(dbContext, TestUserId);
            var car = await CreateCar(dbContext, TestUserId);
            var tire = await CreateTire(dbContext);
            var carTire = await CreateCarTire(dbContext, car, tire, new DateTime(2025, 3, 5));

            // Act
            var response = await _client.GetAsync($"/api/car-tires/{carTire.Id}");

            // Assert
            Assert.True(response.IsSuccessStatusCode, $"Expected a successful status code but got {response.StatusCode}");
            var dto = await response.Content.ReadFromJsonAsync<CarTireDto>();
            Assert.NotNull(dto);
            Assert.IsType<CarTireDto>(dto);
            Assert.True(dto.Id == carTire.Id, $"Expected ID {carTire.Id} but got {dto.Id}");
        }

        [Fact]
        public async Task GetById_ShouldReturn404_WhenCarTireNotExists()
        {
            // Arrange
            // Act
            var response = await _client.GetAsync("/api/car-tires/9999");

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NotFound, $"Expected 404 Not Found but got {response.StatusCode}");
        }

        [Fact]
        public async Task Create_ShouldReturnCarTire_WhenValid()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            await EnsureUserAsync(dbContext, TestUserId);
            var car = await CreateCar(dbContext, TestUserId);
            var tire = await CreateTire(dbContext);
            var dto = new CarTireCreateDto
            {
                CarId = car.Id,
                TireId = tire.Id,
                InstalledDate = new DateTime(2025, 4, 2)
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/car-tires", dto);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.Created, $"Expected 201 Created but got {response.StatusCode}");
            var created = await response.Content.ReadFromJsonAsync<CarTireDto>();
            Assert.NotNull(created);
            Assert.Equal(dto.CarId, created.CarId);
            Assert.Equal(dto.TireId, created.TireId);
            Assert.Equal(dto.InstalledDate, created.InstalledDate);

            var dbCarTire = await dbContext.CarTires.IgnoreQueryFilters().FirstOrDefaultAsync(item => item.Id == created.Id);
            Assert.NotNull(dbCarTire);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenCarNotExists()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            await EnsureUserAsync(dbContext, TestUserId);
            var tire = await CreateTire(dbContext);
            var dto = new CarTireCreateDto
            {
                CarId = 9999,
                TireId = tire.Id,
                InstalledDate = new DateTime(2025, 5, 1)
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/car-tires", dto);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.BadRequest, $"Expected 400 Bad Request but got {response.StatusCode}");
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenTireNotExists()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            await EnsureUserAsync(dbContext, TestUserId);
            var car = await CreateCar(dbContext, TestUserId);
            var dto = new CarTireCreateDto
            {
                CarId = car.Id,
                TireId = 9999,
                InstalledDate = new DateTime(2025, 5, 2)
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/car-tires", dto);

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
            var tire = await CreateTire(dbContext);
            var carTire = await CreateCarTire(dbContext, car, tire, new DateTime(2025, 6, 1));
            var newCar = await CreateCar(dbContext, TestUserId);
            var newTire = await CreateTire(dbContext);
            var dto = new CarTireUpdateDto
            {
                CarId = newCar.Id,
                TireId = newTire.Id,
                InstalledDate = new DateTime(2025, 7, 1)
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/car-tires/{carTire.Id}", dto);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NoContent, $"Expected 204 No Content but got {response.StatusCode}");

            await dbContext.Entry(carTire).ReloadAsync();
            Assert.Equal(dto.CarId, carTire.CarId);
            Assert.Equal(dto.TireId, carTire.TireId);
            Assert.Equal(dto.InstalledDate, carTire.InstalledDate);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenCarNotExists()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            await EnsureUserAsync(dbContext, TestUserId);
            var car = await CreateCar(dbContext, TestUserId);
            var tire = await CreateTire(dbContext);
            var carTire = await CreateCarTire(dbContext, car, tire, new DateTime(2025, 8, 1));
            var dto = new CarTireUpdateDto
            {
                CarId = 9999,
                TireId = tire.Id,
                InstalledDate = new DateTime(2025, 8, 2)
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/car-tires/{carTire.Id}", dto);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.BadRequest, $"Expected 400 Bad Request but got {response.StatusCode}");
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenTireNotExists()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            await EnsureUserAsync(dbContext, TestUserId);
            var car = await CreateCar(dbContext, TestUserId);
            var tire = await CreateTire(dbContext);
            var carTire = await CreateCarTire(dbContext, car, tire, new DateTime(2025, 9, 1));
            var dto = new CarTireUpdateDto
            {
                CarId = car.Id,
                TireId = 9999,
                InstalledDate = new DateTime(2025, 9, 2)
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/car-tires/{carTire.Id}", dto);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.BadRequest, $"Expected 400 Bad Request but got {response.StatusCode}");
        }

        [Fact]
        public async Task Update_ShouldReturn404_WhenCarTireNotExists()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            await EnsureUserAsync(dbContext, TestUserId);
            var car = await CreateCar(dbContext, TestUserId);
            var tire = await CreateTire(dbContext);
            var dto = new CarTireUpdateDto
            {
                CarId = car.Id,
                TireId = tire.Id,
                InstalledDate = new DateTime(2025, 10, 1)
            };

            // Act
            var response = await _client.PutAsJsonAsync("/api/car-tires/9999", dto);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NotFound, $"Expected 404 Not Found but got {response.StatusCode}");
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenCarTireExists()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            await EnsureUserAsync(dbContext, TestUserId);
            var car = await CreateCar(dbContext, TestUserId);
            var tire = await CreateTire(dbContext);
            var carTire = await CreateCarTire(dbContext, car, tire, new DateTime(2025, 11, 1));

            // Act
            var response = await _client.DeleteAsync($"/api/car-tires/{carTire.Id}");

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NoContent, $"Expected 204 No Content but got {response.StatusCode}");

            await dbContext.Entry(carTire).ReloadAsync();
            var dbCarTire = await dbContext.CarTires.IgnoreQueryFilters().FirstOrDefaultAsync(item => item.Id == carTire.Id);
            Assert.NotNull(dbCarTire);
            Assert.True(dbCarTire.DeleatedAt.HasValue, "Expected car tire to be soft deleted.");
        }

        [Fact]
        public async Task Delete_ShouldReturn404_WhenCarTireNotExists()
        {
            // Arrange
            // Act
            var response = await _client.DeleteAsync("/api/car-tires/9999");

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

        private static async Task<Tire> CreateTire(CarExpesesDbContext dbContext)
        {
            var tire = new Tire
            {
                Brand = "TestBrand",
                Model = "TestModel",
                Season = "All-Season",
                Price = 123.45m
            };

            dbContext.Tires.Add(tire);
            await dbContext.SaveChangesAsync();

            return tire;
        }

        private static async Task<CarTire> CreateCarTire(CarExpesesDbContext dbContext, Car car, Tire tire, DateTime installedDate)
        {
            var carTire = new CarTire
            {
                CarId = car.Id,
                TireId = tire.Id,
                InstalledDate = installedDate
            };

            dbContext.CarTires.Add(carTire);
            await dbContext.SaveChangesAsync();

            return carTire;
        }
    }
}
