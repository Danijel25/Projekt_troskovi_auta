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
    public class InsurancesApiControllerTest : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private const int TestUserId = 7001;
        private const int EmptyUserId = 7002;
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public InsurancesApiControllerTest(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateAuthenticatedClient(userId: TestUserId);
        }

        [Fact]
        public async Task GetAll_ShouldReturnListOfInsurances()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            await EnsureUserAsync(dbContext, TestUserId);
            var car = await CreateCar(dbContext, TestUserId);
            await CreateInsurance(dbContext, car, "Acme", "Full");
            await CreateInsurance(dbContext, car, "Shield", "Basic");

            // Act
            var response = await _client.GetAsync("/api/insurances");

            // Assert
            Assert.True(response.IsSuccessStatusCode, $"Expected a successful status code but got {response.StatusCode}");
            var dtos = await response.Content.ReadFromJsonAsync<List<InsuranceDto>>();
            Assert.NotNull(dtos);
            Assert.IsType<List<InsuranceDto>>(dtos);
            Assert.NotEmpty(dtos);
        }

        [Fact]
        public async Task GetAll_ShouldReturnEmptyListOfInsurances()
        {
            // Arrange
            var client = _factory.CreateAuthenticatedClient(userId: EmptyUserId);

            // Act
            var response = await client.GetAsync("/api/insurances");

            // Assert
            Assert.True(response.IsSuccessStatusCode, $"Expected a successful status code but got {response.StatusCode}");
            var dtos = await response.Content.ReadFromJsonAsync<List<InsuranceDto>>();
            Assert.NotNull(dtos);
            Assert.IsType<List<InsuranceDto>>(dtos);
            Assert.Empty(dtos);
        }

        [Fact]
        public async Task GetById_ShouldReturnInsurance_WhenInsuranceExists()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            await EnsureUserAsync(dbContext, TestUserId);
            var car = await CreateCar(dbContext, TestUserId);
            var insurance = await CreateInsurance(dbContext, car, "Acme", "Full");

            // Act
            var response = await _client.GetAsync($"/api/insurances/{insurance.Id}");

            // Assert
            Assert.True(response.IsSuccessStatusCode, $"Expected a successful status code but got {response.StatusCode}");
            var dto = await response.Content.ReadFromJsonAsync<InsuranceDto>();
            Assert.NotNull(dto);
            Assert.IsType<InsuranceDto>(dto);
            Assert.True(dto.Id == insurance.Id, $"Expected ID {insurance.Id} but got {dto.Id}");
        }

        [Fact]
        public async Task GetById_ShouldReturn404_WhenInsuranceNotExists()
        {
            // Arrange
            // Act
            var response = await _client.GetAsync("/api/insurances/9999");

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NotFound, $"Expected 404 Not Found but got {response.StatusCode}");
        }

        [Fact]
        public async Task Create_ShouldReturnInsurance_WhenValid()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            await EnsureUserAsync(dbContext, TestUserId);
            var car = await CreateCar(dbContext, TestUserId);
            var dto = CreateValidInsuranceCreateDto(car.Id);

            // Act
            var response = await _client.PostAsJsonAsync("/api/insurances", dto);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.Created, $"Expected 201 Created but got {response.StatusCode}");
            var created = await response.Content.ReadFromJsonAsync<InsuranceDto>();
            Assert.NotNull(created);
            Assert.Equal(dto.Company, created.Company);
            Assert.Equal(dto.InsuranceType, created.InsuranceType);
            Assert.Equal(dto.Price, created.Price);
            Assert.Equal(dto.StartDate, created.StartDate);
            Assert.Equal(dto.EndDate, created.EndDate);
            Assert.Equal(dto.CarId, created.CarId);

            var dbInsurance = await dbContext.Insurances.IgnoreQueryFilters().FirstOrDefaultAsync(item => item.Id == created.Id);
            Assert.NotNull(dbInsurance);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenInvalid()
        {
            // Arrange
            var dto = CreateInvalidInsuranceCreateDto();

            // Act
            var response = await _client.PostAsJsonAsync("/api/insurances", dto);

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
            var insurance = await CreateInsurance(dbContext, car, "Shield", "Basic");
            var dto = CreateValidInsuranceUpdateDto(car.Id);

            // Act
            var response = await _client.PutAsJsonAsync($"/api/insurances/{insurance.Id}", dto);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NoContent, $"Expected 204 No Content but got {response.StatusCode}");

            await dbContext.Entry(insurance).ReloadAsync();
            Assert.Equal(dto.Company, insurance.Company);
            Assert.Equal(dto.InsuranceType, insurance.InsuranceType);
            Assert.Equal(dto.Price, insurance.Price);
            Assert.Equal(dto.StartDate, insurance.StartDate);
            Assert.Equal(dto.EndDate, insurance.EndDate);
            Assert.Equal(dto.CarId, insurance.CarId);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenInvalid()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            await EnsureUserAsync(dbContext, TestUserId);
            var car = await CreateCar(dbContext, TestUserId);
            var insurance = await CreateInsurance(dbContext, car, "Acme", "Full");
            var dto = CreateInvalidInsuranceUpdateDto();

            // Act
            var response = await _client.PutAsJsonAsync($"/api/insurances/{insurance.Id}", dto);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.BadRequest, $"Expected 400 Bad Request but got {response.StatusCode}");
        }

        [Fact]
        public async Task Update_ShouldReturn404_WhenInsuranceNotExists()
        {
            // Arrange
            var dto = CreateValidInsuranceUpdateDto(1);

            // Act
            var response = await _client.PutAsJsonAsync("/api/insurances/9999", dto);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NotFound, $"Expected 404 Not Found but got {response.StatusCode}");
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenInsuranceExists()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            await EnsureUserAsync(dbContext, TestUserId);
            var car = await CreateCar(dbContext, TestUserId);
            var insurance = await CreateInsurance(dbContext, car, "RoadSafe", "Premium");

            // Act
            var response = await _client.DeleteAsync($"/api/insurances/{insurance.Id}");

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NoContent, $"Expected 204 No Content but got {response.StatusCode}");

            await dbContext.Entry(insurance).ReloadAsync();
            var dbInsurance = await dbContext.Insurances.IgnoreQueryFilters().FirstOrDefaultAsync(item => item.Id == insurance.Id);
            Assert.NotNull(dbInsurance);
            Assert.True(dbInsurance.DeleatedAt.HasValue, "Expected insurance to be soft deleted.");
        }

        [Fact]
        public async Task Delete_ShouldReturn404_WhenInsuranceNotExists()
        {
            // Arrange
            // Act
            var response = await _client.DeleteAsync("/api/insurances/9999");

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

        private static async Task<Insurance> CreateInsurance(CarExpesesDbContext dbContext, Car car, string company, string insuranceType)
        {
            var insurance = new Insurance
            {
                Company = company,
                InsuranceType = insuranceType,
                Price = 1200m,
                StartDate = new DateTime(2024, 1, 1),
                EndDate = new DateTime(2025, 1, 1),
                CarId = car.Id
            };

            dbContext.Insurances.Add(insurance);
            await dbContext.SaveChangesAsync();

            return insurance;
        }

        private static InsuranceCreateDto CreateValidInsuranceCreateDto(int carId)
        {
            return new InsuranceCreateDto
            {
                Company = "Acme",
                InsuranceType = "Premium",
                Price = 1500m,
                StartDate = new DateTime(2024, 2, 1),
                EndDate = new DateTime(2025, 2, 1),
                CarId = carId
            };
        }

        private static InsuranceUpdateDto CreateValidInsuranceUpdateDto(int carId)
        {
            return new InsuranceUpdateDto
            {
                Company = "Updated Company",
                InsuranceType = "Basic",
                Price = 1100m,
                StartDate = new DateTime(2024, 3, 1),
                EndDate = new DateTime(2025, 3, 1),
                CarId = carId
            };
        }

        private static InsuranceCreateDto CreateInvalidInsuranceCreateDto()
        {
            return new InsuranceCreateDto
            {
                Company = string.Empty,
                InsuranceType = string.Empty,
                Price = -1m,
                StartDate = new DateTime(2025, 1, 1),
                EndDate = new DateTime(2024, 1, 1),
                CarId = 0
            };
        }

        private static InsuranceUpdateDto CreateInvalidInsuranceUpdateDto()
        {
            return new InsuranceUpdateDto
            {
                Company = string.Empty,
                InsuranceType = string.Empty,
                Price = -10m,
                StartDate = new DateTime(2025, 5, 1),
                EndDate = new DateTime(2024, 5, 1),
                CarId = 0
            };
        }
    }
}
