using CarExpenses.DAL;
using CarExpenses.Model.Enums;
using CarExpenses.Model.Models;
using CarExpenses.Web.Api.Dtos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace CarExpenses.Test
{
    public class CarFilesApiControllerTest : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private const int TestUserId = 9001;
        private const int EmptyUserId = 9002;
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public CarFilesApiControllerTest(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateAuthenticatedClient(userId: TestUserId);
        }

        [Fact]
        public async Task GetAll_ShouldReturnNotFound_WhenCarNotExists()
        {
            // Arrange
            // Act
            var response = await _client.GetAsync("/api/cars/9999/files");

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NotFound, $"Expected 404 Not Found but got {response.StatusCode}");
        }

        [Fact]
        public async Task GetAll_ShouldReturnEmptyList_WhenNoFiles()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            await EnsureUserAsync(dbContext, EmptyUserId);
            var car = await CreateCar(dbContext, EmptyUserId);

            var client = _factory.CreateAuthenticatedClient(userId: EmptyUserId);

            // Act
            var response = await client.GetAsync($"/api/cars/{car.Id}/files");

            // Assert
            Assert.True(response.IsSuccessStatusCode, $"Expected a successful status code but got {response.StatusCode}");
            var dtos = await response.Content.ReadFromJsonAsync<List<CarFileDto>>();
            Assert.NotNull(dtos);
            Assert.IsType<List<CarFileDto>>(dtos);
            Assert.Empty(dtos);
        }

        [Fact]
        public async Task GetAll_ShouldReturnListOfFiles()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
            await EnsureUserAsync(dbContext, TestUserId);
            var car = await CreateCar(dbContext, TestUserId);

            using var uploadContent = CreateUploadContent("log.txt", "text/plain", "test file");
            var uploadResponse = await _client.PostAsync($"/api/cars/{car.Id}/files", uploadContent);
            Assert.True(uploadResponse.IsSuccessStatusCode, $"Expected a successful status code but got {uploadResponse.StatusCode}");

            // Act
            var response = await _client.GetAsync($"/api/cars/{car.Id}/files");

            // Assert
            Assert.True(response.IsSuccessStatusCode, $"Expected a successful status code but got {response.StatusCode}");
            var dtos = await response.Content.ReadFromJsonAsync<List<CarFileDto>>();
            Assert.NotNull(dtos);
            Assert.IsType<List<CarFileDto>>(dtos);
            Assert.NotEmpty(dtos);

            var dbFile = await dbContext.CarFiles
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(item => item.CarId == car.Id && item.OriginalFileName == "log.txt");
            Assert.NotNull(dbFile);

            var rootPath = GetWebRootPath(env);
            var relativePath = dbFile.RelativePath.TrimStart('/', '\\').Replace('/', Path.DirectorySeparatorChar);
            var physicalPath = Path.Combine(rootPath, relativePath);
            Assert.True(File.Exists(physicalPath), "Expected uploaded file to exist on disk.");

            if (File.Exists(physicalPath))
            {
                File.Delete(physicalPath);
            }
        }

        [Fact]
        public async Task Upload_ShouldReturnFiles_WhenValid()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
            await EnsureUserAsync(dbContext, TestUserId);
            var car = await CreateCar(dbContext, TestUserId);
            using var content = CreateUploadContent("receipt.txt", "text/plain", "file data");

            // Act
            var response = await _client.PostAsync($"/api/cars/{car.Id}/files", content);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.OK, $"Expected 200 OK but got {response.StatusCode}");
            var files = await response.Content.ReadFromJsonAsync<List<CarFileDto>>();
            Assert.NotNull(files);
            Assert.NotEmpty(files);
            Assert.Equal("receipt.txt", files[0].FileName);

            var dbFile = await dbContext.CarFiles
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(item => item.CarId == car.Id && item.OriginalFileName == "receipt.txt");
            Assert.NotNull(dbFile);

            var rootPath = GetWebRootPath(env);
            var relativePath = dbFile.RelativePath.TrimStart('/', '\\').Replace('/', Path.DirectorySeparatorChar);
            var physicalPath = Path.Combine(rootPath, relativePath);
            Assert.True(File.Exists(physicalPath), "Expected uploaded file to exist on disk.");

            if (File.Exists(physicalPath))
            {
                File.Delete(physicalPath);
            }

            dbContext.CarFiles.Remove(dbFile);
            await dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task Upload_ShouldReturnBadRequest_WhenNoFiles()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            await EnsureUserAsync(dbContext, TestUserId);
            var car = await CreateCar(dbContext, TestUserId);
            using var content = new MultipartFormDataContent();

            // Act
            var response = await _client.PostAsync($"/api/cars/{car.Id}/files", content);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.BadRequest, $"Expected 400 Bad Request but got {response.StatusCode}");
        }

        [Fact]
        public async Task Upload_ShouldReturnNotFound_WhenCarNotExists()
        {
            // Arrange
            using var content = CreateUploadContent("missing.txt", "text/plain", "file data");

            // Act
            var response = await _client.PostAsync("/api/cars/9999/files", content);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NotFound, $"Expected 404 Not Found but got {response.StatusCode}");
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenFileExists()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
            await EnsureUserAsync(dbContext, TestUserId);
            var car = await CreateCar(dbContext, TestUserId);
            using var uploadContent = CreateUploadContent("delete.txt", "text/plain", "delete me");
            var uploadResponse = await _client.PostAsync($"/api/cars/{car.Id}/files", uploadContent);
            Assert.True(uploadResponse.IsSuccessStatusCode, $"Expected a successful status code but got {uploadResponse.StatusCode}");

            var dbFile = await dbContext.CarFiles
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(item => item.CarId == car.Id && item.OriginalFileName == "delete.txt");
            Assert.NotNull(dbFile);

            var rootPath = GetWebRootPath(env);
            var relativePath = dbFile.RelativePath.TrimStart('/', '\\').Replace('/', Path.DirectorySeparatorChar);
            var physicalPath = Path.Combine(rootPath, relativePath);

            // Act
            var response = await _client.DeleteAsync($"/api/cars/{car.Id}/files/{dbFile.Id}");

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NoContent, $"Expected 204 No Content but got {response.StatusCode}");

            await dbContext.Entry(dbFile).ReloadAsync();
            var deleted = await dbContext.CarFiles
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(item => item.Id == dbFile.Id);
            Assert.NotNull(deleted);
            Assert.True(deleted.DeleatedAt.HasValue, "Expected file record to be soft deleted.");
            Assert.False(File.Exists(physicalPath), "Expected uploaded file to be removed from disk.");
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFound_WhenFileNotExists()
        {
            // Arrange
            // Act
            var response = await _client.DeleteAsync("/api/cars/9999/files/9999");

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NotFound, $"Expected 404 Not Found but got {response.StatusCode}");
        }

        private static MultipartFormDataContent CreateUploadContent(string fileName, string contentType, string content)
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            var fileContent = new ByteArrayContent(bytes);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);

            var form = new MultipartFormDataContent();
            form.Add(fileContent, "files", fileName);

            return form;
        }

        private static string GetWebRootPath(IWebHostEnvironment environment)
        {
            return string.IsNullOrWhiteSpace(environment.WebRootPath)
                ? Path.Combine(environment.ContentRootPath, "wwwroot")
                : environment.WebRootPath;
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
    }
}
