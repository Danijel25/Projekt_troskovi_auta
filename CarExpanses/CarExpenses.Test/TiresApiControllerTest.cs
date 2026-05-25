using CarExpenses.DAL;
using CarExpenses.Model.Models;
using CarExpenses.Web.Api.Dtos;
using CarExpenses.Web.Controllers.Api;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace CarExpenses.Test
{
    public class TiresApiControllerTest : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public TiresApiControllerTest(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateAuthenticatedClient();
        }

        [Fact]
        public async Task GetAll_ShouldReturnListOfTires()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            await CreateTire(dbContext);
            await CreateTire(dbContext);
            // Act
            var response = await _client.GetAsync("/api/tires");
            // Assert
            Assert.True(response.IsSuccessStatusCode, $"Expected a successful status code but got {response.StatusCode}");
            var dtos = await response.Content.ReadFromJsonAsync<List<TireSummaryDto>>();
            Assert.NotNull(dtos);
            Assert.IsType<List<TireSummaryDto>>(dtos);
            Assert.NotEmpty(dtos);
        }

        [Fact]
        public async Task GetAll_ShouldReturnEmptyListOfTires()
        {
            // Arrange
            // Act
            var response = await _client.GetAsync("/api/tires");
            // Assert
            Assert.True(response.IsSuccessStatusCode, $"Expected a successful status code but got {response.StatusCode}");
            var dtos = await response.Content.ReadFromJsonAsync<List<TireSummaryDto>>();
            Assert.NotNull(dtos);
            Assert.IsType<List<TireSummaryDto>>(dtos);
            Assert.Empty(dtos);
        }

        [Fact]
        public async Task GetById_ShouldReturnTire_WhenTireExists()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();            
            var tire = await CreateTire(dbContext);

            // Act
            var response = await _client.GetAsync($"/api/tires/{tire.Id}");

            // Assert
            Assert.True(response.IsSuccessStatusCode, $"Expected a successful status code but got {response.StatusCode}");            
            var dto = await response.Content.ReadFromJsonAsync<TireDetailDto>(); 
            Assert.NotNull(dto);
            Assert.IsType<TireDetailDto>(dto);
            Assert.True(dto.Id == tire.Id, $"Expected ID {tire.Id} but got {dto.Id}");
        }

        [Fact]
        public async Task GetById_ShouldReturn404_WhenTireNotExists()
        {
            // Arrange
            // Act
            var response = await _client.GetAsync("/api/tires/9999");

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NotFound, $"Expected 404 Not Found but got {response.StatusCode}");
        }

        [Fact]
        public async Task Create_ShouldReturnTire_WhenValid()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            var dto = CreateValidTireCreateDto();

            // Act
            var response = await _client.PostAsJsonAsync("/api/tires", dto);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.Created, $"Expected 201 Created but got {response.StatusCode}");
            var created = await response.Content.ReadFromJsonAsync<TireDetailDto>();
            Assert.NotNull(created);
            Assert.Equal(dto.Brand, created.Brand);
            Assert.Equal(dto.Model, created.Model);
            Assert.Equal(dto.Season, created.Season);
            Assert.Equal(dto.Price, created.Price);

            var dbTire = await dbContext.Tires.FirstOrDefaultAsync(tire => tire.Id == created.Id);
            Assert.NotNull(dbTire);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenInvalid()
        {
            // Arrange
            var dto = CreateInvalidTireCreateDto();

            // Act
            var response = await _client.PostAsJsonAsync("/api/tires", dto);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.BadRequest, $"Expected 400 Bad Request but got {response.StatusCode}");
        }

        [Fact]
        public async Task Update_ShouldReturnNoContent_WhenValid()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            var tire = await CreateTire(dbContext);
            var dto = CreateValidTireUpdateDto();

            // Act
            var response = await _client.PutAsJsonAsync($"/api/tires/{tire.Id}", dto);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NoContent, $"Expected 204 No Content but got {response.StatusCode}");

            await dbContext.Entry(tire).ReloadAsync();
            Assert.Equal(dto.Brand, tire.Brand);
            Assert.Equal(dto.Model, tire.Model);
            Assert.Equal(dto.Season, tire.Season);
            Assert.Equal(dto.Price, tire.Price);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenInvalid()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            var tire = await CreateTire(dbContext);
            var dto = CreateInvalidTireUpdateDto();

            // Act
            var response = await _client.PutAsJsonAsync($"/api/tires/{tire.Id}", dto);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.BadRequest, $"Expected 400 Bad Request but got {response.StatusCode}");
        }

        [Fact]
        public async Task Update_ShouldReturn404_WhenTireNotExists()
        {
            // Arrange
            var dto = CreateValidTireUpdateDto();

            // Act
            var response = await _client.PutAsJsonAsync("/api/tires/9999", dto);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NotFound, $"Expected 404 Not Found but got {response.StatusCode}");
        }



        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenTireExists()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            var tire = await CreateTire(dbContext);

            // Act
            var response = await _client.DeleteAsync($"/api/tires/{tire.Id}");

            // Assert
            Assert.True(response.IsSuccessStatusCode, $"Expected a successful status code but got {response.StatusCode}");

            await dbContext.Entry(tire).ReloadAsync();
            var dbTire = await dbContext.Tires.IgnoreQueryFilters().FirstOrDefaultAsync(t => t.Id == tire.Id);            
            Assert.NotNull(dbTire);
            Assert.True(dbTire.DeleatedAt.HasValue, "Expected category to be soft deleted.");
        }

        [Fact]
        public async Task Delete_ShouldReturn404_WhenTireNotExists()
        {
            // Arrange
            // Act
            var response = await _client.DeleteAsync("/api/tires/9999");

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NotFound, $"Expected 404 Not Found but got {response.StatusCode}");
        }


        private async Task<Tire> CreateTire(CarExpesesDbContext dbContext)
        {
            var tire = new Tire
            {                
                Brand = "TestBrand",
                Model = "TestModel",                
                Season = "All-Season",
                Price = 123.23M
            };

            dbContext.Tires.Add(tire);
            await dbContext.SaveChangesAsync();

            return tire;
        }

        private static TireCreateDto CreateValidTireCreateDto()
        {
            return new TireCreateDto
            {
                Brand = "NewBrand",
                Model = "NewModel",
                Season = "Winter",
                Price = 210.50m
            };
        }

        private static TireUpdateDto CreateValidTireUpdateDto()
        {
            return new TireUpdateDto
            {
                Brand = "UpdatedBrand",
                Model = "UpdatedModel",
                Season = "Summer",
                Price = 189.99m
            };
        }

        private static TireCreateDto CreateInvalidTireCreateDto()
        {
            return new TireCreateDto
            {
                Brand = string.Empty,
                Model = string.Empty,
                Season = string.Empty,
                Price = -1m
            };
        }

        private static TireUpdateDto CreateInvalidTireUpdateDto()
        {
            return new TireUpdateDto
            {
                Brand = string.Empty,
                Model = string.Empty,
                Season = string.Empty,
                Price = -5m
            };
        }
    }
}
