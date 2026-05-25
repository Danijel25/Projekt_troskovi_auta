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
    public class ServiceRecordsApiControllerTest : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private const int TestUserId = 8001;
        private const int EmptyUserId = 8002;
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public ServiceRecordsApiControllerTest(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateAuthenticatedClient(userId: TestUserId);
        }

        [Fact]
        public async Task GetAll_ShouldReturnListOfServiceRecords()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            await EnsureUserAsync(dbContext, TestUserId);
            var car = await CreateCar(dbContext, TestUserId);
            await CreateServiceRecord(dbContext, car, "Maintenance", "Oil change");
            await CreateServiceRecord(dbContext, car, "Brakes", "Pads replacement");

            // Act
            var response = await _client.GetAsync("/api/service-records");

            // Assert
            Assert.True(response.IsSuccessStatusCode, $"Expected a successful status code but got {response.StatusCode}");
            var dtos = await response.Content.ReadFromJsonAsync<List<ServiceRecordDto>>();
            Assert.NotNull(dtos);
            Assert.IsType<List<ServiceRecordDto>>(dtos);
            Assert.NotEmpty(dtos);
        }

        [Fact]
        public async Task GetAll_ShouldReturnEmptyListOfServiceRecords()
        {
            // Arrange
            var client = _factory.CreateAuthenticatedClient(userId: EmptyUserId);

            // Act
            var response = await client.GetAsync("/api/service-records");

            // Assert
            Assert.True(response.IsSuccessStatusCode, $"Expected a successful status code but got {response.StatusCode}");
            var dtos = await response.Content.ReadFromJsonAsync<List<ServiceRecordDto>>();
            Assert.NotNull(dtos);
            Assert.IsType<List<ServiceRecordDto>>(dtos);
            Assert.Empty(dtos);
        }

        [Fact]
        public async Task GetById_ShouldReturnServiceRecord_WhenServiceRecordExists()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            await EnsureUserAsync(dbContext, TestUserId);
            var car = await CreateCar(dbContext, TestUserId);
            var record = await CreateServiceRecord(dbContext, car, "Tires", "Rotation");

            // Act
            var response = await _client.GetAsync($"/api/service-records/{record.Id}");

            // Assert
            Assert.True(response.IsSuccessStatusCode, $"Expected a successful status code but got {response.StatusCode}");
            var dto = await response.Content.ReadFromJsonAsync<ServiceRecordDto>();
            Assert.NotNull(dto);
            Assert.IsType<ServiceRecordDto>(dto);
            Assert.True(dto.Id == record.Id, $"Expected ID {record.Id} but got {dto.Id}");
        }

        [Fact]
        public async Task GetById_ShouldReturn404_WhenServiceRecordNotExists()
        {
            // Arrange
            // Act
            var response = await _client.GetAsync("/api/service-records/9999");

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NotFound, $"Expected 404 Not Found but got {response.StatusCode}");
        }

        [Fact]
        public async Task Create_ShouldReturnServiceRecord_WhenValid()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            await EnsureUserAsync(dbContext, TestUserId);
            var car = await CreateCar(dbContext, TestUserId);
            var dto = CreateValidServiceRecordCreateDto(car.Id);

            // Act
            var response = await _client.PostAsJsonAsync("/api/service-records", dto);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.Created, $"Expected 201 Created but got {response.StatusCode}");
            var created = await response.Content.ReadFromJsonAsync<ServiceRecordDto>();
            Assert.NotNull(created);
            Assert.Equal(dto.ServiceType, created.ServiceType);
            Assert.Equal(dto.Description, created.Description);
            Assert.Equal(dto.Cost, created.Cost);
            Assert.Equal(dto.ServiceDate, created.ServiceDate);
            Assert.Equal(dto.Mileage, created.Mileage);
            Assert.Equal(dto.CarId, created.CarId);

            var dbRecord = await dbContext.ServiceRecords.IgnoreQueryFilters().FirstOrDefaultAsync(item => item.Id == created.Id);
            Assert.NotNull(dbRecord);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenInvalid()
        {
            // Arrange
            var dto = CreateInvalidServiceRecordCreateDto();

            // Act
            var response = await _client.PostAsJsonAsync("/api/service-records", dto);

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
            var record = await CreateServiceRecord(dbContext, car, "Engine", "Tune-up");
            var dto = CreateValidServiceRecordUpdateDto(car.Id);

            // Act
            var response = await _client.PutAsJsonAsync($"/api/service-records/{record.Id}", dto);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NoContent, $"Expected 204 No Content but got {response.StatusCode}");

            await dbContext.Entry(record).ReloadAsync();
            Assert.Equal(dto.ServiceType, record.ServiceType);
            Assert.Equal(dto.Description, record.Description);
            Assert.Equal(dto.Cost, record.Cost);
            Assert.Equal(dto.ServiceDate, record.ServiceDate);
            Assert.Equal(dto.Mileage, record.Mileage);
            Assert.Equal(dto.CarId, record.CarId);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenInvalid()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            await EnsureUserAsync(dbContext, TestUserId);
            var car = await CreateCar(dbContext, TestUserId);
            var record = await CreateServiceRecord(dbContext, car, "Cooling", "Flush");
            var dto = CreateInvalidServiceRecordUpdateDto();

            // Act
            var response = await _client.PutAsJsonAsync($"/api/service-records/{record.Id}", dto);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.BadRequest, $"Expected 400 Bad Request but got {response.StatusCode}");
        }

        [Fact]
        public async Task Update_ShouldReturn404_WhenServiceRecordNotExists()
        {
            // Arrange
            var dto = CreateValidServiceRecordUpdateDto(1);

            // Act
            var response = await _client.PutAsJsonAsync("/api/service-records/9999", dto);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NotFound, $"Expected 404 Not Found but got {response.StatusCode}");
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenServiceRecordExists()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            await EnsureUserAsync(dbContext, TestUserId);
            var car = await CreateCar(dbContext, TestUserId);
            var record = await CreateServiceRecord(dbContext, car, "Brakes", "Fluid change");

            // Act
            var response = await _client.DeleteAsync($"/api/service-records/{record.Id}");

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NoContent, $"Expected 204 No Content but got {response.StatusCode}");

            await dbContext.Entry(record).ReloadAsync();
            var dbRecord = await dbContext.ServiceRecords.IgnoreQueryFilters().FirstOrDefaultAsync(item => item.Id == record.Id);
            Assert.NotNull(dbRecord);
            Assert.True(dbRecord.DeleatedAt.HasValue, "Expected service record to be soft deleted.");
        }

        [Fact]
        public async Task Delete_ShouldReturn404_WhenServiceRecordNotExists()
        {
            // Arrange
            // Act
            var response = await _client.DeleteAsync("/api/service-records/9999");

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

        private static async Task<ServiceRecord> CreateServiceRecord(CarExpesesDbContext dbContext, Car car, string serviceType, string description)
        {
            var record = new ServiceRecord
            {
                ServiceType = serviceType,
                Description = description,
                Cost = 350m,
                ServiceDate = new DateTime(2024, 1, 20),
                Mileage = 45800,
                CarId = car.Id
            };

            dbContext.ServiceRecords.Add(record);
            await dbContext.SaveChangesAsync();

            return record;
        }

        private static ServiceRecordCreateDto CreateValidServiceRecordCreateDto(int carId)
        {
            return new ServiceRecordCreateDto
            {
                ServiceType = "Alignment",
                Description = "Wheel alignment service",
                Cost = 220m,
                ServiceDate = new DateTime(2024, 2, 15),
                Mileage = 47000,
                CarId = carId
            };
        }

        private static ServiceRecordUpdateDto CreateValidServiceRecordUpdateDto(int carId)
        {
            return new ServiceRecordUpdateDto
            {
                ServiceType = "Updated Service",
                Description = "Updated description",
                Cost = 180m,
                ServiceDate = new DateTime(2024, 3, 18),
                Mileage = 48000,
                CarId = carId
            };
        }

        private static ServiceRecordCreateDto CreateInvalidServiceRecordCreateDto()
        {
            return new ServiceRecordCreateDto
            {
                ServiceType = string.Empty,
                Description = string.Empty,
                Cost = -1m,
                ServiceDate = default,
                Mileage = -1,
                CarId = 0
            };
        }

        private static ServiceRecordUpdateDto CreateInvalidServiceRecordUpdateDto()
        {
            return new ServiceRecordUpdateDto
            {
                ServiceType = string.Empty,
                Description = string.Empty,
                Cost = -10m,
                ServiceDate = default,
                Mileage = -5,
                CarId = 0
            };
        }
    }
}
